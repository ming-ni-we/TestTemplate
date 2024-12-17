using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Channels;
using System.Timers;

namespace TestItemTemplate.Models
{
    class ByteArray
    {
        public byte[] buffer = new byte[1024];
        public int len = 0;
    }

    /// <summary>
    /// 串口服务器
    /// </summary>
    public class SerialPortServer
    {
        /// <summary>
        /// socket连接
        /// </summary>
#pragma warning disable CS8625 // 无法将 null 字面量转换为非 null 的引用类型。
        public Socket socket = null;
#pragma warning restore CS8625 // 无法将 null 字面量转换为非 null 的引用类型。
        /// <summary>
        /// IP地址
        /// </summary>
        public string ipAddress;
        /// <summary>
        /// 端口号
        /// </summary>
        public int port;
        /// <summary>
        /// 是否已连接
        /// </summary>
        public bool connected = false;
        /// <summary>
        /// 串口服务器构造函数
        /// </summary>
        /// <param name="battery">对应的电池</param>
        /// <param name="ipAdderss">IP地址</param>
        /// <param name="port">端口号</param>
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
        public SerialPortServer(string ipAdderss = "127.0.0.1", int port = 8080)
#pragma warning restore CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
        {
            this.ipAddress = ipAdderss;
            this.port = port;
        }
        /// <summary>
        /// 开启TCP连接
        /// </summary>
        /// <returns>返回True为连接成功，返回False为连接失败</returns>
        public void StartConnect()
        {
            try
            {
                int TargetPoint = port;
                IPAddress TargetIP = IPAddress.Parse(ipAddress);
                IPEndPoint ipe = new IPEndPoint(TargetIP, TargetPoint);
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.Connect(ipe);
                connected = true;
            }
            catch
            {
                connected = false;
            }
        }

        /// <summary>
        /// 关闭TCP连接
        /// </summary>
        public void CloseConnect()
        {
#pragma warning disable CS0168 // 声明了变量，但从未使用过
            try
            {
                //断开连接
                socket.Close();
            }
            catch (Exception err)
            {
            }
#pragma warning restore CS0168 // 声明了变量，但从未使用过
        }
        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="data">具体需要发送的内容</param>
        /// <returns>返回True为发送数据成功，返回False为发送数据失败</returns>
        public bool SendData(byte[] data)
        {
            try
            {
                if (connected)
                {
                    socket.Send(data, data.Length, 0);
                    return true;
                }
            }
            catch
            {
                return false;
            }
            return false;
        }
        /// <summary>
        /// 定义接收数据的事件委托
        /// </summary>
        /// <param name="data">接收的数据</param>
        /// <param name="dataLength">接收数据的长度</param>
        public delegate void DataReceived(byte[] data, int dataLength);
        public event DataReceived OnDataReceived;


        public bool hasReceive = false;
        /// <summary>
        /// 接收数据任务
        /// </summary>
        Task task = null;
        /// <summary>
        /// 开启接收数据的任务
        /// </summary>
        public void ReceiveMessageFromServer()
        {

            task = new Task(() =>
            {
                while (true)
                {
                    try
                    {
                        if (connected)
                        {
                            byte[] buffer = new byte[1024];
                            int bytesRead = socket.Receive(buffer);
                            hasReceive = true;
                            if (OnDataReceived != null)
                            {
                                OnDataReceived(buffer, bytesRead);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Thread.Sleep(1000);
                        connected = false;
                        StartConnect();
                    }
                }
            });
            task.Start();
        }
    }
}
