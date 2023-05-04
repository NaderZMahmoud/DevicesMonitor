using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace devicesMessages_ports
{
    internal class TemperatureService
    {
        private readonly ConcurrentDictionary<int, (double sum, int count)> temperatures = new ConcurrentDictionary<int, (double, int)>();

        public async Task StartListeningAsync(int port, CancellationToken cancellationToken)
        {
            var endpoint = new IPEndPoint(IPAddress.Any, port);
            var listener = new Socket(endpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            listener.Bind(endpoint);
            listener.Listen();

            while (!cancellationToken.IsCancellationRequested)
            {
                var socket = await listener.AcceptAsync();
                _ = Task.Run(() => HandleClientAsync(socket), cancellationToken);
            }
        }

        private async Task HandleClientAsync(Socket socket)
        {
            try
            {
                var buffer = new byte[1024];
                var message = "";

                while (true)
                {
                    var bytesRead = await socket.ReceiveAsync(buffer, SocketFlags.None);
                    if (bytesRead == 0)
                    {
                        break;
                    }

                    message += System.Text.Encoding.ASCII.GetString(buffer, 0, bytesRead);
                    if (message.EndsWith("\n"))
                    {
                        var parts = message.TrimEnd('\n').Split(' ');
                        if (parts.Length == 2 && int.TryParse(parts[0], out int deviceId) && double.TryParse(parts[1], out double temperature))
                        {
                            temperatures.AddOrUpdate(deviceId, (temperature, 1), (_, tuple) => (tuple.sum + temperature, tuple.count + 1));
                            var averageTemperature = temperatures[deviceId].sum / temperatures[deviceId].count;
                            Console.WriteLine($"Device {deviceId}: average temperature is {averageTemperature:F2}");
                        }

                        message = "";
                    }
                }
            }
            catch (SocketException ex) { Console.WriteLine(ex.Message); }
            finally
            {
                socket.Dispose();
            }
        }
    }

}
