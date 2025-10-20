using API.Models;
using DataAccess.Interface;
using DataAccess.Repository;
using Microsoft.AspNetCore.Mvc;
using ResturantApp.Models;

namespace ResturantApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly Irepository _repository;
        public HomeController(Irepository repository)
        {
            _repository = repository;
        }

        [HttpGet("index")]
        public JsonResult Index()
        {
            var Resturants = _repository.GetResturants();
            return new JsonResult(Resturants);
        }
       
     
        //Add
        public IActionResult Add()
        {
           return View();
        }

        [HttpPost("AddData")]
        public JsonResult AddData([FromBody] Resturant ResturantDTO)
        {
            if (ResturantDTO == null)
                return Json(new { success = false, message = "Invalid data" });

            // Save ResturantDTO to database
            // Example:
             _repository.AddResturants(ResturantDTO);

            return Json(new { success = true, message = "Resturant Added successfully" });
        }


        //Update
        public IActionResult Update()
        {
           return View();
        }

        [HttpPost("UpdateData")]
        public JsonResult UpdateData([FromBody] Resturant ResturantDTO)
        {
            if (ResturantDTO == null)
                return Json(new { success = false, message = "Invalid data" });

            // Save ResturantDTO to database
            // Example:
            _repository.UpdateResturantByID(ResturantDTO);

            return Json(new { success = true, message = "Resturant Updated successfully" });
        }

    }
}
