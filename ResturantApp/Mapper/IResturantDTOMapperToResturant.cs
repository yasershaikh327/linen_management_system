using API.Models;
using ResturantApp.Models;

namespace API.Mapper
{
    public interface IResturantDTOMapperToResturant
    {
        public List<ResturantDTO> Map(IEnumerable<Resturant> resturants);
        public ResturantDTO Map(Resturant resturant);
    }

}
