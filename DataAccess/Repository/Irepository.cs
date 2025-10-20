using ResturantApp.Data;
using ResturantApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Interface
{
    public interface Irepository
    {
        public List<Resturant> GetResturants();
        public void AddResturants(Resturant resturant);
        public void UpdateResturantByID(Resturant resturant);
    }
}
