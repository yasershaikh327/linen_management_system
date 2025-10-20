using API.Models;
using ResturantApp.Models;
using System.Collections.Generic;
using System.Linq;

namespace API.Mapper
{
    public class ResturantDTOMapperToResturant : IResturantDTOMapperToResturant
    {
        // Map single Student to StudentDTO
        public ResturantDTO Map(Resturant resturant)
        {
            if (resturant == null) return null;

            return new ResturantDTO
            {
                Id = resturant.Id,
                Name = resturant.Name,
                Email = resturant.Email,
                Phone = resturant.Phone
            };
        }

        public List<ResturantDTO> Map(IEnumerable<Resturant> resturants)
        {
            throw new NotImplementedException();
        }

    }
}
