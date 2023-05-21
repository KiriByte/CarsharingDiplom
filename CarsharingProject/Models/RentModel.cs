using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarsharingProject.Models;

public class RentModel
{
    [Key]
    public int Id { get; set; }

    [ForeignKey("CarId")]
    public virtual RentCarsModel RentCar { get; set; }

    public virtual ApplicationUser User { get; set; }

    public DateTime RentStartDateTime { get; set; }

    public DateTime? RentEndDateTime { get; set; }

    public int RentOdometerStart { get; set; }

    public int? RentOdometerEnd { get; set; }


    public double? TotalPrice { get; set; }
}