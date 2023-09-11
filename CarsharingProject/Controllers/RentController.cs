using CarsharingProject.Data;
using CarsharingProject.Models;
using CarsharingProject.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.EntityFrameworkCore;

namespace CarsharingProject.Controllers;

public class RentController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<UserModel> _userManager;
    private readonly CarTelemetryService _carTelemetryService;

    public RentController(
        ApplicationDbContext context,
        UserManager<UserModel> userManager,
        CarTelemetryService carTelemetryService)
    {
        _context = context;
        _userManager = userManager;
        _carTelemetryService = carTelemetryService;
    }

    public async Task<IActionResult> NewRent(int carId)
    {
        var car = await _context.RentCars.FindAsync(carId);
        if (car == null)
        {
            ViewData["MessageError"] = "Incorrect Id car.";
            return View("NewRent");
        }

        var currentUser = await _userManager.GetUserAsync(User);
        var cards = _context.Users
            .Include(u => u.BankCards)
            .FirstOrDefault(u => u.Id == currentUser.Id)?
            .BankCards
            .ToList();
        ViewData["MessageError"] = "No linked bank cards.";
        ViewData["CarId"] = carId;
        ViewData["CarNumber"] = car.NumberCar;
        return View(cards);
    }

    public async Task<IActionResult> RentCar(int carId, string CardNumber)
    {
        var car = await _context.RentCars.FindAsync(carId);
        if (car == null)
        {
            ViewData["MessageError"] = "Incorrect Id car.";
            return View("NewRent");
        }

        if (!car.IsAvailableForUsers)
        {
            ViewData["MessageError"] = "The car is not available for rent";
            return View("NewRent");
        }

        if (car.IsRentNow)
        {
            ViewData["MessageError"] = "The car is busy";
            return View("NewRent");
        }

        if (string.IsNullOrEmpty(CardNumber))
        {
            ViewData["MessageError"] = "No linked bank cards.";
            return View("NewRent");
        }

        var user = await _userManager.GetUserAsync(User);
        if (!await _userManager.IsInRoleAsync(user, "verifiedUser"))
        {
            ViewData["MessageError"] = "You are not verified.";
            return View("NewRent");
        }

        // var connection = new HubConnectionBuilder()
        //     .WithUrl("http://192.168.1.69/carshub")
        //     .Build();
        //
        // await connection.StartAsync();
        //
        // var carTelemetry = await connection.InvokeAsync<CarTelemetry>("GetCar", car.Vin);
        //
        // await connection.StopAsync();
        var carTelemetry = await _carTelemetryService.GetCarTelemetry(car.Vin);
        var rent = new RentModel()
        {
            RentCar = car,
            RentStartDateTime = DateTime.Now,
            User = user,
            RentOdometerStart = carTelemetry.Odometer
        };

        _context.RentHistory.Add(rent);
        _context.RentCars.Update(car);
        car.IsRentNow = true;
        await _context.SaveChangesAsync();

        return RedirectToAction("Index", "Home");
    }

    public async Task<IActionResult> RentHistoryForUser()
    {
        var user = await _userManager.GetUserAsync(User);
        var rentHistory = _context.RentHistory.Include(c => c.RentCar).Where(u => u.User == user).ToList();
        return View(rentHistory);
    }

    public async Task<IActionResult> EndRent(int rentId)
    {
        var user = await _userManager.GetUserAsync(User);
        var rent = await _context.RentHistory.Include(c => c.RentCar)
            .FirstOrDefaultAsync(r => r.Id == rentId);
        if (rent.User != user)
        {
            return RedirectToAction("Index", "Home");
        }

        var endDateTime = DateTime.Now;

        TimeSpan differenceTime = endDateTime - rent.RentStartDateTime;
        var minutes = differenceTime.TotalMinutes;


        var carTelemetry = await _carTelemetryService.GetCarTelemetry(rent.RentCar.Vin);


        var kmInRoad = carTelemetry.Odometer - rent.RentOdometerStart;

        var totalPrice = (minutes * rent.RentCar.PricePerMinute)
                         + kmInRoad * rent.RentCar.PricePerKM;

        rent.RentEndDateTime = endDateTime;
        rent.TotalPrice = totalPrice;
        rent.RentOdometerEnd = carTelemetry.Odometer;
        _context.Update(rent);

        //бновить состояние авто
        var car = rent.RentCar;
        car.IsRentNow = false;
        _context.Update(car);

        await _context.SaveChangesAsync();

        return RedirectToAction("Index", "Map");
    }
}