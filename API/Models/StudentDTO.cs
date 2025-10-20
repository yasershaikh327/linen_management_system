namespace API.Models
{
    public class StudentDTO
    {
        public int Id { get; set; }       // Primary key
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
    }

    public class StudentDTOS
    {
        public StudentDTO studentDTO { get; set; }
        public StudentDTOS(StudentDTO studentDTO)
        {
            this.studentDTO = studentDTO;
        }
    }
}
