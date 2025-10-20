using API.Models;
using ResturantApp.Models;
using System.Collections.Generic;
using System.Linq;

namespace API.Mapper
{
    public class StudentDTOMapperToStudent : IStudentDTOMapperToStudent
    {
        // Map single Student to StudentDTO
        public StudentDTO Map(Student student)
        {
            if (student == null) return null;

            return new StudentDTO
            {
                Id = student.Id,
                Name = student.Name,
                Email = student.Email,
                Phone = student.Phone
            };
        }

        public List<StudentDTO> Map(IEnumerable<Student> students)
        {
            throw new NotImplementedException();
        }

    }
}
