using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestItemTemplate.Helpers;

namespace TestItemTemplate.Models
{
    public partial class CurrentReader : ObservableObject
    {
        public SerialCommnication serialPort = null;
        [ObservableProperty]
        private byte index;

        [ObservableProperty]
        private double current;

        public CurrentReader()
        {
            serialPort.OnDataReceived += SerialPort_OnDataReceived;
        }
        private void SerialPort_OnDataReceived(byte[] data, int dataLength)
        {
            if (data[0] == Index && data[1] == 0x04)
            {

            }
        }
        public void GetCurrentValue()
        {
            byte[] arraybuf = new byte[] { Index, 04, 00, 0x0A, 00, 04, 00, 00 };
            CRC_16.MakeCRC_16(arraybuf, arraybuf.Length);
            serialPort.EnqueueCommand(arraybuf);
            Thread.Sleep(300);
        }
    }
}
