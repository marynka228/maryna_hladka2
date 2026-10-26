using System.Security.Cryptography;
using System.Text;

namespace maryna_hladka2.Services
{
    // AES-256-CBC. Випадковий IV на кожне шифрування додається на початок результату (Base64).
    // Ключ генерується один раз і зберігається в SecureStorage.
    public class EncryptionService : IEncryptionService
    {
        private const string KeyName = "db_aes_key";
        private readonly SemaphoreSlim _lock = new(1, 1);
        private byte[]? _key;

        private async Task<byte[]> GetKeyAsync()
        {
            if (_key is not null) return _key;

            await _lock.WaitAsync();
            try
            {
                if (_key is not null) return _key;

                string? b64 = null;
                try { b64 = await SecureStorage.Default.GetAsync(KeyName); } catch { }

                // Запасний варіант: на Windows без пакування SecureStorage може бути недоступний
                if (string.IsNullOrEmpty(b64))
                {
                    var fallback = Preferences.Get(KeyName, string.Empty);
                    if (!string.IsNullOrEmpty(fallback)) b64 = fallback;
                }

                if (string.IsNullOrEmpty(b64))
                {
                    b64 = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
                    try { await SecureStorage.Default.SetAsync(KeyName, b64); }
                    catch { Preferences.Set(KeyName, b64); }
                }

                _key = Convert.FromBase64String(b64);
                return _key;
            }
            finally
            {
                _lock.Release();
            }
        }

        public async Task<string> EncryptAsync(string plainText)
        {
            if (string.IsNullOrEmpty(plainText)) return string.Empty;

            var key = await GetKeyAsync();
            using var aes = Aes.Create();
            aes.Key = key;
            aes.GenerateIV();

            using var encryptor = aes.CreateEncryptor();
            var data = Encoding.UTF8.GetBytes(plainText);
            var cipher = encryptor.TransformFinalBlock(data, 0, data.Length);

            var result = new byte[aes.IV.Length + cipher.Length];
            Buffer.BlockCopy(aes.IV, 0, result, 0, aes.IV.Length);
            Buffer.BlockCopy(cipher, 0, result, aes.IV.Length, cipher.Length);
            return Convert.ToBase64String(result);
        }

        public async Task<string> DecryptAsync(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText)) return string.Empty;

            try
            {
                var key = await GetKeyAsync();
                var all = Convert.FromBase64String(cipherText);

                using var aes = Aes.Create();
                aes.Key = key;
                var iv = new byte[16];
                Buffer.BlockCopy(all, 0, iv, 0, 16);
                aes.IV = iv;

                using var decryptor = aes.CreateDecryptor();
                var plain = decryptor.TransformFinalBlock(all, 16, all.Length - 16);
                return Encoding.UTF8.GetString(plain);
            }
            catch
            {
                return "[не вдалося розшифрувати]";
            }
        }
    }
}