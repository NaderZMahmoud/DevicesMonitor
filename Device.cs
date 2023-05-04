using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace devicesMessages_ports
{
    internal class Device
    {
        private readonly Random random = new Random();
        private readonly int deviceId;
        private readonly IPAddress ipAddress;
        private readonly int port;

        public Device(int deviceId, IPAddress ipAddress, int port)
        {
            this.deviceId = deviceId;
            this.ipAddress = ipAddress;
            this.port = port;
        }

        public async Task SendMeasurementsAsync(CancellationToken cancellationToken)
        {
            var endpoint = new IPEndPoint(ipAddress, port);
            using var socket = new Socket(endpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            await socket.ConnectAsync(endpoint);

            while (!cancellationToken.IsCancellationRequested)
            {
                var temperature = random.NextDouble() * 10 + 20; // generate a random temperature between 20 and 30 degrees
                var message = $"{deviceId} {temperature:F2}\n";
                var bytes = System.Text.Encoding.ASCII.GetBytes(message);
                await socket.SendAsync(new ArraySegment<byte>(bytes), SocketFlags.None);
                await Task.Delay(random.Next(500, 2000), cancellationToken); // wait for a random period of time before sending the next measurement
            }
        }
    }
}
