using DataAccess.Interface;
using ResturantApp.Data;
using ResturantApp.Models;
using System.Collections.Generic;
using System.Linq;

namespace DataAccess.Repository
{
    public class Repository : Irepository
    {
        private readonly ApplicationDbContext _context;

        public Repository(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Student> GetStudents()
        {
            // Fetch all students from the database
            return _context.student.ToList();
        }
    }
}
