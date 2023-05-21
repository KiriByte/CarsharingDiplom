using System.ComponentModel.DataAnnotations;

namespace CarsharingProject.Models
{
    public class RentCarsModel
    {
        [Key]
        public int Id { get; set; }

        public string Vin { get; set; }
        public string NumberCar { get; set; } = "";
        public bool IsRentNow { get; set; }=false;
        public bool IsAvailableForUsers { get; set; } = false;
        public double PricePerKM { get; set; } = 0;
        public double PricePerMinute { get; set; } = 0;

        public List<RentModel> RentHistory { get; set; } = new List<RentModel>();
    }
}
