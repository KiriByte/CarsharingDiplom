using CarsharingProject.Models;
using Microsoft.AspNetCore.SignalR.Client;

namespace CarsharingProject.Services;

public class CarTelemetryService
{
    private readonly HubConnection _hubConnection;

    public CarTelemetryService(string appName)
    {
        string url = $"http://{appName}/carshub";
        _hubConnection = new HubConnectionBuilder()
            .WithUrl(url)
            .Build();
        StartAsync().Wait();
    }

    public async Task StartAsync()
    {
        await _hubConnection.StartAsync();
    }

    public async Task<CarTelemetry> GetCarTelemetry(string vin)
    {
        return await _hubConnection.InvokeAsync<CarTelemetry>("GetCar", vin);
    }
}