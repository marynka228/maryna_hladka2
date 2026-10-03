using maryna_hladka2.Models;

namespace maryna_hladka2
{
    public class StudentUpdatedMessage
    {
        public Student OriginalStudent { get; }
        public Student UpdatedStudent { get; }

        public StudentUpdatedMessage(Student originalStudent, Student updatedStudent)
        {
            OriginalStudent = originalStudent;
            UpdatedStudent = updatedStudent;
        }
    }
}