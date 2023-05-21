using Bogus;
using CarSimulatorBlazorServer.Service;

namespace CarSimulatorBS.Data;

public class Car
{
    public string Vin { get; set; }
    
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public int Odometer { get; set; }
    //public DateTime DateTime { get; set; }
    // public float FuelLevel { get; set; }
    // public int Speed { get; set; }
    
    // public int EngineTemperature { get; set; }
    // public bool EngineRunning { get; set; }
    // public bool HeadlightsOn { get; set; }
    // public bool DoorsOpened { get; set; }

    // public Car(int vin, DateTime timestamp, Location location, float fuelLevel,
    //     float speed, int odometer, float engineTemperature, bool engineRunning)
    // {
    //     Vin = vin;
    //     Timestamp = timestamp;
    //     Location = location;
    //     FuelLevel = fuelLevel;
    //     Speed = speed;
    //     Odometer = odometer;
    //     EngineTemperature = engineTemperature;
    //     EngineRunning = engineRunning;
    // }

    public Car(Faker faker,(double,double) centerCoords,int rangeInKm)

    {
        Vin = faker.Vehicle.Vin();
        // Latitude = faker.Address.Latitude();
        // Longitude = faker.Address.Longitude();
        (Latitude, Longitude) = GeoHelper.GetRandomCoordinates(centerCoords.Item1, centerCoords.Item2, rangeInKm);

        Odometer = faker.Random.Int(0, 100000);

    }

    public Car()
    {
        
    }
}