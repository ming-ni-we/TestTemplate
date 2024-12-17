using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestItemTemplate.Helpers
{
    public class SerialCommnicationString
    {
        public SerialPort serialPort;
        public Queue<string> commandQueue = new Queue<string>();
        private readonly object lockObject = new object();
        private const int MaxReties = 3;
        private int delay;

        private int receviveDelay;
        public SerialCommnicationString(string portName, int baudRate, int delay = 200, int receviveDelay = 300)
        {
            serialPort = new SerialPort(portName, baudRate, Parity.None, 8, StopBits.One);
            serialPort.RtsEnable = true;
            serialPort.DtrEnable = true;
            serialPort.DataReceived += SerialPort_DataReceived;
            this.delay = delay;
            this.receviveDelay = receviveDelay;
        }

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            System.Threading.Thread.Sleep(receviveDelay);
            try
            {
                do
                {
                    int count = serialPort.BytesToRead;
                    char[] byteMessage = new char[count];
                    serialPort.Read(byteMessage, 0, count);
                    OnDataReceived(byteMessage, byteMessage.Length);
                }
                while (serialPort.BytesToRead > 0);
                serialPort.DiscardInBuffer();
            }
            catch (Exception ex) { 
            }
        }

        public void Open()
        {
            try
            {
                if (!serialPort.IsOpen)
                {
                    serialPort.Open();
                    StartCommunicationThread();
                }
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(e.Message);
            }
        }
        public void Close()
        {
            if (!serialPort.IsOpen)
            {
                serialPort.Close();
            }
        }

        public void EnqueueCommand(string command)
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
                    string currentCommand = DequeueCommand();
                    if (currentCommand != null)
                    {
                        int retryCount = 0;
                        SendCommand(currentCommand);
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
        public delegate void DataReceived(char[] data, int dataLength);
        public event DataReceived OnDataReceived;
        private bool WaitForResponse()
        {
            System.Threading.Thread.Sleep(receviveDelay);
            try
            {
                do
                {
                    int count = serialPort.BytesToRead;
                    char[] byteMessage = new char[count];
                    serialPort.Read(byteMessage, 0, count);
                    OnDataReceived(byteMessage, byteMessage.Length);
                }
                while (serialPort.BytesToRead > 0);
                serialPort.DiscardInBuffer();
                return true;
            }
            catch (Exception ex) { return false; }
        }

        private void SendCommand(string currentCommand)
        {
            if (serialPort.IsOpen)
            {
                serialPort.Write(currentCommand);
            }
        }

        private string? DequeueCommand()
        {
            lock (lockObject)
            {
                return commandQueue.Count > 0 ? commandQueue.Dequeue() : null;
            }
        }
    }
}
