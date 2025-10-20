namespace API.Models
{
    public class ResturantDTO
    {
        public int Id { get; set; }       // Primary key
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
    }

    public class ResturantDTOS
    {
        public ResturantDTO ResturantDTO { get; set; }
        public ResturantDTOS(ResturantDTO ResturantDTO)
        {
            this.ResturantDTO = ResturantDTO;
        }
    }
}
