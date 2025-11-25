using CRUD.Models;

namespace CRUD.Repository
{
    public class Crud_Repository
    {
        private readonly AppDbContext _DbContext;
        public Crud_Repository(AppDbContext context)
        {
            _DbContext = context;
        }

        public string AddStudent(Student student)
        {
            if (student != null)
            {
                _DbContext.Students.Add(student);
                _DbContext.SaveChanges();
                return "Success";
            }
            return "Failed";
        }
        public string FindStudent(Guid id)
        {
            var student = _DbContext.Students.Find(id);
            if (student != null)
            {
                return student.Id.ToString();
            }
            return "Student Not Found";
        }

        public List<Student> ReadStudent(List<Student> liststudent)
        {
            if (liststudent != null)
            {
                var getStudents = new List<Student>();
                return _DbContext.Students.ToList();
            }
            return null;
        }

        public string UpdateStudent(Student student)
        {
            if (student != null)
            {
                var studentedit = _DbContext.Students.FirstOrDefault(x =>  x.Id == student.Id);
                if (studentedit != null)
                {
                    studentedit.Name = student.Name;
                    student.DateAdded = DateTime.Now;
                    _DbContext.SaveChanges();
                }
                return "Success";
            }
            return "Failed";
        }
        public string DeleteStudent(Guid id)
        {
         
            var checkifstdexists = _DbContext.Students.Find(id);
            if (checkifstdexists != null)
            {
                _DbContext.Students.Remove(checkifstdexists);
                _DbContext.SaveChanges();
                return "Success";
            }
            return "Failed";
        }
    }
}
