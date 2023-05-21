using CarsharingProject.Models;

namespace CarsharingProject.ViewModels;

public class AvailableCarWithCoordViewModel
{
    public RentCarsModel Car { get; set;}
    public CarTelemetry CarTelemetry { get; set; }
}