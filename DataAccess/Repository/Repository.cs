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

        public void AddResturants(Resturant resturant)
        {
            _context.resturant.Add(resturant);
            _context.SaveChanges();
        }

        public List<Resturant> GetResturants()
        {
            // Fetch all Resturants from the database
            return _context.resturant.ToList();
        }

        public void UpdateResturantByID(Resturant resturant)
        {
            var searchresturantById = _context.resturant.FirstOrDefault(x => x.Id == resturant.Id);
            if (searchresturantById != null)
            {
                var getName = searchresturantById.Name;
                var getEmail = searchresturantById.Email;
                var getPhone = searchresturantById.Phone;
                getName = resturant.Name;
                getEmail = resturant.Email;
                getPhone = resturant.Phone;
                _context.resturant.Update(resturant);
                _context.SaveChanges();
            }
        }
    }
}
