using CarSimulatorBS.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace CarSimulatorBS.Service;

public class CarsHub:Hub
{
    private readonly CarService _carService;
    public CarsHub(CarService carService)
    {
        _carService = carService;
    }
    
    public Car GetCar(string vin)
    {
        return _carService.Cars.Find(car=>car.Vin==vin);
    }

    public IEnumerable<Car> GetCars()
    {
        return _carService.Cars;
    }
}