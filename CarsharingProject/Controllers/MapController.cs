using CarsharingProject.Data;
using CarsharingProject.Models;
using CarsharingProject.Services;
using CarsharingProject.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR.Client;

namespace CarsharingProject.Controllers
{
    public class MapController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly CarTelemetryService _carTelemetryService;

        public MapController(
            ApplicationDbContext context,
            CarTelemetryService carTelemetryService)
        {
            _context = context;
            _carTelemetryService = carTelemetryService;
        }

        public async Task<IActionResult> Index()
        {
            var availableCars = new List<AvailableCarWithCoordViewModel>();
            var carsFromDB = _context.RentCars
                .Where(c => c.IsAvailableForUsers && !c.IsRentNow).ToList();

            foreach (var car in carsFromDB)
            {
                availableCars.Add(new AvailableCarWithCoordViewModel()
                {
                    Car = car,
                    CarTelemetry = await _carTelemetryService.GetCarTelemetry(car.Vin)
                });
            }

            return View(availableCars);
        }
    }
}