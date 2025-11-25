using System.Diagnostics;
using CRUD.Models;
using CRUD.Repository;
using Microsoft.AspNetCore.Mvc;

namespace CRUD.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly Crud_Repository _crud_Repository;

        public HomeController(ILogger<HomeController> logger, Crud_Repository crud_Repository)
        {
            _logger = logger;
            _crud_Repository = crud_Repository;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Student student)
        {
            if (student != null)
            {
                _crud_Repository.AddStudent(student);
                TempData["Message"] = "Student Added Successfully";
                return RedirectToAction("Create", "Home");
            }
            TempData["Message"] = "Something Went Wrong";
            return RedirectToAction("Create", "Home");
        }

        [HttpGet]
        public IActionResult Read(List<Student> liststudent)
        {
            if (liststudent != null)
            {
                var studentList = _crud_Repository.ReadStudent(liststudent);
                TempData["Message"] = "Students Fetch Successfully . There are " + studentList.Count() + "Students";
                return View(studentList);
            }
            TempData["Message"] = "Something Went Wrong";
            return View();
        }

        public IActionResult Update(Guid id)
        {
            var checkifstdexists = _crud_Repository.FindStudent(id);
            if (checkifstdexists != null)
            {
                return View();
            }
            TempData["Message"] = "Student Not Found";
            return RedirectToAction("Read","Home");
        }

        [HttpPost]
        public IActionResult UpdateData(Student student)
        {
            var checkifstdexists = _crud_Repository.UpdateStudent(student);
            if (checkifstdexists != null)
            {
                _crud_Repository.UpdateStudent(student);
                TempData["Message"] = "Student Details Updated";
            }
            TempData["Message"] = "Something Went Wrong";
            return RedirectToAction("Read", "Home");
        }


        [HttpGet]
        public IActionResult DeleteData(Guid id)
        {
            _crud_Repository.DeleteStudent(id);
            TempData["Message"] = "Student Details Deleted";
            return RedirectToAction("Read", "Home");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
