using System.IO.Ports;

namespace TestItemTemplate.Helpers
{
    public class SerialCommnication
    {
        public SerialPort serialPort;
        public Queue<byte[]> commandQueue = new Queue<byte[]>();
        private readonly object lockObject = new object();
        private const int MaxReties = 3;
        private int delay;

        private int receviveDelay;
        public SerialCommnication(string portName, int baudRate, int delay = 200, int receviveDelay = 300)
        {
            serialPort = new SerialPort(portName, baudRate, Parity.None, 8, StopBits.One);
            serialPort.RtsEnable = true;
            this.delay = delay;
            this.receviveDelay = receviveDelay;
        }
        public void Open()
        {
            if (!serialPort.IsOpen)
            {
                serialPort.Open();
                StartCommunicationThread();
            }
        }
        public void Close()
        {
            if (!serialPort.IsOpen)
            {
                serialPort.Close();
            }
        }

        public void EnqueueCommand(byte[] command)
        {
            lock (lockObject)
            {
                commandQueue.Enqueue(command);
            }
        }

        private void StartCommunicationThread()
        {
            Task.Run(() =>
            {
                while (true)
                {
                    byte[] currentCommand = DequeueCommand();
                    if (currentCommand != null)
                    {
                        int retryCount = 0;
                        while (retryCount < MaxReties)
                        {
                            SendCommand(currentCommand);
                            bool success = WaitForResponse();
                            if (success)
                            {
                                break;
                            }
                            retryCount++;
                        }
                    }
                    Thread.Sleep(delay);
                }
            });
        }

        /// <summary>
        /// 定义接收数据的事件委托
        /// </summary>
        /// <param name="data">接收的数据</param>
        /// <param name="dataLength">接收数据的长度</param>
        public delegate void DataReceived(byte[] data, int dataLength);
        public event DataReceived OnDataReceived;
        private bool WaitForResponse()
        {

            Thread.Sleep(receviveDelay);
            try
            {
                do
                {
                    int count = serialPort.BytesToRead;
                    byte[] byteMessage = new byte[count];
                    serialPort.Read(byteMessage, 0, count);
                    OnDataReceived(byteMessage, byteMessage.Length);
                }
                while (serialPort.BytesToRead > 0);
                serialPort.DiscardInBuffer();
                return true;
            }
            catch (Exception ex) { return false; }
        }

        private void SendCommand(byte[] currentCommand)
        {
            if (serialPort.IsOpen)
            {
                serialPort.Write(currentCommand, 0, currentCommand.Length);
            }
        }

        private byte[]? DequeueCommand()
        {
            lock (lockObject)
            {
                return commandQueue.Count > 0 ? commandQueue.Dequeue() : null;
            }
        }

    }
}
