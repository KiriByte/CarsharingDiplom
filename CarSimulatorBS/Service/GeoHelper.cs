namespace CarSimulatorBlazorServer.Service;

public class GeoHelper
{
    // Returns a random number between min and max
    private static readonly Random _random = new Random();

    // Returns a random point in a circle of radius rangeInKm around the given point
    public static (double latitude, double longitude) GetRandomCoordinates(double centerLatitude, double centerLongitude, double radiusInKm)
    {
        // Convert radius from km to degrees
        double radiusInDegrees = radiusInKm / 111.12;

        // Generate random angle in radians
        double randomAngle = _random.NextDouble() * Math.PI * 2;

        // Generate random distance from center in degrees
        double randomDistance = _random.NextDouble() * radiusInDegrees;

        // Calculate new latitude and longitude
        double newLatitude = centerLatitude + (randomDistance * Math.Cos(randomAngle));
        double newLongitude = centerLongitude + (randomDistance * Math.Sin(randomAngle));

        return (newLatitude, newLongitude);
    }
}