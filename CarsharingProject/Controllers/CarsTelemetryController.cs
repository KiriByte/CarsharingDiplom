using CarsharingProject.Data;
using CarsharingProject.Models;
using CarsharingProject.Services;
using CarsharingProject.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.EntityFrameworkCore;

namespace CarsharingProject.Controllers
{
    [Authorize(Roles = "admin, employee")]
    public class CarsTelemetryController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly CarTelemetryService _carTelemetryService;

        public CarsTelemetryController(ApplicationDbContext context,
            CarTelemetryService carTelemetryService)
        {
            _context = context;
            _carTelemetryService = carTelemetryService;
        }

        public async Task<IActionResult> Index()
        {
            
            var carsTelemetryList = await _carTelemetryService.GetCarsTelemetry();
            
            var cars = new List<CarTelemetryWithDBViewModel>();
            
            foreach (var ct in carsTelemetryList)
            {
                var carViewModel = new CarTelemetryWithDBViewModel
                {
                    Vin = ct.Vin,
                    Latitude = ct.Latitude,
                    Longitude = ct.Longitude,
                    Odometer = ct.Odometer
                };

                carViewModel.AvailableInDB = await _context.RentCars.AnyAsync(c => c.Vin == ct.Vin);

                cars.Add(carViewModel);
            }
            
            return View(cars);
        }

        [HttpPost]
        public ActionResult AddCarToDB(string vin)
        {
            var car = new RentCarsModel()
            {
                Vin = vin,
                NumberCar = "",
                IsRentNow = false,
                IsAvailableForUsers = false,
            };
            _context.RentCars.Add(car);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult DeleteCarFromDB(string vin)
        {
            var car = _context.RentCars.FirstOrDefault(c => c.Vin == vin);
            _context.RentCars.Remove(car);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}