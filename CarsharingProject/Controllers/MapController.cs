using CarsharingProject.Data;
using CarsharingProject.Models;
using CarsharingProject.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR.Client;

namespace CarsharingProject.Controllers
{
    public class MapController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MapController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            List<AvailableCarWithCoordViewModel> availableCars = new List<AvailableCarWithCoordViewModel>();
            var carsFromDB = _context.RentCars.Where(c => c.IsAvailableForUsers && !c.IsRentNow).ToList();
            var connection = new HubConnectionBuilder()
                .WithUrl("http://192.168.1.69/carshub")
                .Build();
            await connection.StartAsync();
            foreach (var car in carsFromDB)
            {
                availableCars.Add(new AvailableCarWithCoordViewModel()
                {
                    Car = car,
                    CarTelemetry = await connection.InvokeAsync<CarTelemetry>("GetCar", $"{car.Vin}")
                });
            }
            await connection.StopAsync();


            return View(availableCars);
        }
    }
}