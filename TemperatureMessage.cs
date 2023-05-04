using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace devicesMessages_ports
{
    internal class TemperatureMessage
    {
        public int DeviceId { get; set; }
        public double Temperature { get; set; }
    }
}
