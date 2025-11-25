namespace CRUD.Models
{
    public class Student
    {
        public Guid Id { get; set; }  
        public string Name { get; set; }
        public DateTime DateAdded { get; set; } = DateTime.Now;

    }
}
