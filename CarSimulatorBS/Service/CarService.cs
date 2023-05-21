using Bogus;
using CarSimulatorBS.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;

namespace CarSimulatorBS.Service;

public class CarService
{
    public List<Car> Cars = new List<Car>();
    public readonly string fileName = "cars.json";
    (double, double) grodnoCoords = (Latitude: 53.670868, Longitude: 23.829871);
    public readonly int radiusInKm = 10;

    public CarService()
    {
        LoadCarsFromJson();
    }

    public void GenerateNewCars(int count)
    {
        var faker = new Faker();
        for (int i = 0; i < count; i++)
        {
            Cars.Add(new Car(faker, grodnoCoords, radiusInKm));
        }

        SaveCarsToJson();
    }

    public void SaveCarsToJson()
    {
        var json = JsonConvert.SerializeObject(Cars);
        File.WriteAllText(fileName, json);
    }

    public void LoadCarsFromJson()
    {
        if (File.Exists(fileName))
        {
            var json = File.ReadAllText(fileName);
            Cars = JsonConvert.DeserializeObject<List<Car>>(json);
        }
    }

    public void ClearCarsAndDeleteFile()
    {
        Cars.Clear();
        if (File.Exists(fileName))
        {
            File.Delete(fileName);
        }
    }
    
    public async Task<Car> GetCar(string vin)
    {
        var car = Cars.Find(car=>car.Vin==vin);
        return car;
    }
}