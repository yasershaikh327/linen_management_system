using API.Models;
using ResturantApp.Models;

namespace API.Mapper
{
    public interface IStudentDTOMapperToStudent
    {
        public List<StudentDTO> Map(IEnumerable<Student> students);
        public StudentDTO Map(Student student);
    }

}
