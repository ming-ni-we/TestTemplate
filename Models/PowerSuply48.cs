using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestItemTemplate.Helpers;

namespace TestItemTemplate.Models
{
    public partial class PowerSuply48 : ObservableObject
    {
        public SerialCommnication serialPort = null;

        [ObservableProperty]
        private byte index;

        [ObservableProperty]
        private double voltage;
        [ObservableProperty]
        private double current;
        [ObservableProperty]
        private double power;
        [ObservableProperty]
        private double voltageSet;
        [ObservableProperty]
        private double currentSet;

        [ObservableProperty]
        private bool open;

        public PowerSuply48(SerialCommnication serialCommnication)
        {
            serialPort = serialCommnication;
            serialPort.OnDataReceived += SerialPort_OnDataReceived;
            voltage = 0;
            current = 0;
            power = 0;
            voltageSet = 0;
            currentSet = 0;
            open = false;
        }

        private void SerialPort_OnDataReceived(byte[] data, int dataLength)
        {
            throw new NotImplementedException();
        }
    }
}
