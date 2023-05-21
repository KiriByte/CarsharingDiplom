using CarsharingProject.Models;

namespace CarsharingProject.ViewModels
{
    public class CarTelemetryWithDBViewModel : CarTelemetry
    {
        public bool AvailableInDB { get; set; } = false;
        public string? NumberCar { get; set; }
    }
}
