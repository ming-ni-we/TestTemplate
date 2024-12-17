using System.Collections.ObjectModel;
using TestItemTemplate.Helpers;

namespace TestItemTemplate.Models
{
    public partial class IoRelayItem : ObservableObject
    {
        [ObservableProperty]
        public int index;
        [ObservableProperty]
        public bool value;
    }
    public partial class IORelay : ObservableObject
    {
        public SerialCommnication serialPort = null;
        [ObservableProperty]
        private byte index;
        [ObservableProperty]
        public ObservableCollection<IoRelayItem> ioRelayState;
        public IORelay(SerialCommnication serialCommnication)
        {
            ioRelayState = new ObservableCollection<IoRelayItem>();
            for (int i = 0; i < 16; i++)
            {
                ioRelayState.Add(new IoRelayItem() { Index = i, Value = false });
            }
            serialPort = serialCommnication;
            serialPort.OnDataReceived += SerialPort_OnDataReceived;
        }
        private void SerialPort_OnDataReceived(byte[] data, int dataLength)
        {
            throw new NotImplementedException();
        }


        public void ShortCircle(List<int> ints)
        {
            foreach (var item in ints)
            {
                IoRelayState[item].Value = true;
            }
            SendIOState();
        }


        public void CloseShortCircle(List<int> ints)
        {
            foreach (var item in ints)
            {
                IoRelayState[item].Value = false;
            }
            SendIOState();
        }
        public void SendIOState()
        {
            byte[] data = [ 1, 0x10, 0x00, 0x00, 0x00, 0x10,0x20,
                0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
                0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
                0x00, 0x00 ];
            for (int i = 0; i < IoRelayState.Count; i++)
            {
                data[8 + 2 * i] = (byte)(IoRelayState[i].Value ? 1 : 0);
            }
            CRC_16.MakeCRC_16(data, data.Length);
            serialPort.EnqueueCommand(data);
        }
    }
}
