using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestItemTemplate.Helpers;

namespace TestItemTemplate.Models
{
    public partial class VoltageItem : ObservableObject
    {
        [ObservableProperty]
        public int index;
        [ObservableProperty]
        public double value;
    }
    public partial class VoltageReader : ObservableObject
    {
        public SerialCommnication serialPort = null;
        [ObservableProperty]
        private byte index;

        [ObservableProperty]
        private ObservableCollection<VoltageItem> voltages;
        public VoltageReader()
        {
            /*            serialPort.OnDataReceived += SerialPort_OnDataReceived;*/
            Voltages = new ObservableCollection<VoltageItem>();
            for (int i = 0; i < 16; i++)
            {
                Voltages.Add(new VoltageItem { Index = i, Value = 0 });
            }
        }

        private void SerialPort_OnDataReceived(byte[] data, int dataLength)
        {
            if (data[0] == Index && data[1] == 0x04)
            {
                for (int i = 0; i < data[2] / 2; i++)
                {
                    byte[] bytes = [data[2 * i + 3], data[2 * i + 4]];
                    float voltage = HextoFolat.Hex_To_Float(bytes);
                    Voltages[i].Value = voltage;
                }
            }
        }
        public void GetVoltageValue()
        {
            byte[] arraybuf = new byte[] { Index, 04, 00, 0x00, 00, 10, 00, 00 };
            CRC_16.MakeCRC_16(arraybuf, arraybuf.Length);
            serialPort.EnqueueCommand(arraybuf);
            Thread.Sleep(300);
        }
    }
}
