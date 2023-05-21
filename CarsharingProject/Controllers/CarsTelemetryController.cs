using CarsharingProject.Data;
using CarsharingProject.Models;
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

        public CarsTelemetryController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var connection = new HubConnectionBuilder()
                .WithUrl("http://192.168.1.69/carshub")
                .Build();

            await connection.StartAsync();

            var cars = await connection.InvokeAsync<IEnumerable<CarTelemetryWithDBViewModel>>("GetCars");

            await connection.StopAsync();

            foreach (var car in cars)
            {
                car.AvailableInDB = await _context.RentCars.AnyAsync(c => c.Vin == car.Vin);
            }


            return View(cars);
        }

        [HttpPost]
        public ActionResult AddCarToDB(string vin)
        {

            var car = new RentCarsModel()
            {
                Vin = vin,
                NumberCar = "test",
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

        public async Task<CarTelemetry> GetCarFromBlazorServer(string vin)
        {
            var connection = new HubConnectionBuilder()
                .WithUrl("http://192.168.1.69/carshub")
                .Build();

            await connection.StartAsync();

            var car = await connection.InvokeAsync<CarTelemetry>("GetCar", vin);

            await connection.StopAsync();

            return car;
        }

    }
}
