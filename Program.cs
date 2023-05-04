using devicesMessages_ports;
using System.Net;

internal class Program
{
    private static async Task Main()
    {
        var cancellationTokenSource = new CancellationTokenSource();
        var port = 12345;
        var devices = new[] { (deviceId: 1, ipAddress: IPAddress.Parse("127.0.0.1"), port), (deviceId: 2, ipAddress: IPAddress.Parse("127.0.0.1"), port), (deviceId: 3, ipAddress: IPAddress.Parse("127.0.0.1"), port) };
        var deviceTasks = devices.Select(device => new Device(device.deviceId, device.ipAddress, device.port).SendMeasurementsAsync(cancellationTokenSource.Token)).ToArray();

        var service = new TemperatureService();
        var serviceTask = service.StartListeningAsync(port, cancellationTokenSource.Token);

        Console.WriteLine("Press any key to stop the service.");
        Console.ReadKey();
        cancellationTokenSource.Cancel();

        await Task.WhenAll(serviceTask, Task.WhenAll(deviceTasks));
    }
}