using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using TestItemTemplate.Models;

namespace TestItemTemplate.Helpers
{
    //定义CANmessage包
    [ObservableObject]
    public partial class CanMsg//定义CanMsg包
    {
        [ObservableProperty]
        public int iD;
        [ObservableProperty]
        public byte[] data = new byte[8];
        [ObservableProperty]
        public string dataType;
        [ObservableProperty]
        public long timeStamp;
    }
    public partial class CANDevice : ObservableObject
    {

        #region CAN通讯参数
        const int DEV_USBCAN = 3;
        const int DEV_USBCAN2 = 4;

        static UInt32 m_devtype = 21;//USBCAN2

        UInt32 m_bOpen = 0;
        UInt32 m_devind = 0;
        UInt32 m_canind = 0;

        VCI_CAN_OBJ[] m_recobj = new VCI_CAN_OBJ[1000];

        UInt32[] m_arrdevtype = new UInt32[20];
        public Single fVolt = 0.0f;

        public Boolean refbu;
        public IntPtr hDevice;

        public Int32 DeviceLgcID;

        public Int32 exint;

        public static float[] DC_info = new float[3];

        public int DC_Count = 0;

        public bool CH1_Run;

        public bool CH1_Save;

        bool MES_Enb = false;
        UInt32 CH1_m_bOpen = 0;//CH1_CAN连接/断开标志
        public static string[] strAyyay = new string[30];

        public static bool fm3_run = false;
        Thread[] workThreads = new Thread[6];//线程数

        public SerialPort Com_port2 = new SerialPort();
        string st_rparameter;
        string CH_SN_Infstr = "";
        byte Con_Mode = 0x00;
        string CH1_instr;// 
        string[] Test_Uilt = new string[2];

        public bool CH1_WriteFile = false;
        const UInt32 STATUS_OK = 1;
        Int32[] CH1_Decline = new Int32[3];

        //usb-e-u 波特率
        static UInt32[] GCanBrTab = new UInt32[10]{
                    0x060003, 0x060004, 0x060007,
                        0x1C0008, 0x1C0011, 0x160023,
                        0x1C002C, 0x1600B3, 0x1C00E0,
                        0x1C01C1
                };
        private float x;//定义当前窗体的宽度
        private float y;//定义当前窗体的高度
        private bool a = false;
        [ObservableProperty]
        List<CanMsg> canMsgList = new List<CanMsg>();//CAN报文数组

        /// <summary>
        /// 打开BMS板放电MOS
        /// </summary>
        public byte[] OpenMOS = new byte[8] { 0x01, 0x0A, 0x55, 0x0A, 0x00, 0x00, 0x00, 0x00 };
        /// <summary>
        /// 关闭BMS板放电MOS
        /// </summary>
        public byte[] CloseMOS = new byte[8] { 0x01, 0x0A, 0x55, 0x05, 0x00, 0x00, 0x00, 0x00 };


        /// <summary>
        /// 读取温度
        /// </summary>
        public byte[] ReadTemperature = new byte[8] { 0x04, 0x43, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00 };
        /// <summary>
        /// 读取电压
        /// </summary>
        public byte[] ReadVoltage = new byte[8] { 0x04, 0x44, 0x01, 0x00, 0x00, 0x04, 0x00, 0x00 };

        public int IdAdd = Convert.ToInt32("0x" + "100301F2", 16);
        public int IdAddC = Convert.ToInt32("0x" + "1004F201", 16);
        #endregion
        //初始化CH1_CAN通讯
        CAN_2E_U.VCI_INIT_CONFIG[] configs = new CAN_2E_U.VCI_INIT_CONFIG[2];
        unsafe public void Init_CAN1(UInt32 m_devtype, UInt32 m_devind, UInt32 m_canind, UInt16 UBaud)
        {
            try
            {
                if (CAN_2E_U.VCI_OpenDevice(m_devtype, m_devind, 0) == 0)
                {
                    System.Windows.MessageBox.Show("打开设备失败,请检查设备类型和设备索引号是否正确");
                    return;
                }

                //USB-E-U 代码
                UInt32 baud;
                baud = GCanBrTab[UBaud];//波特率
                if (CAN_2E_U.VCI_SetReference(m_devtype, m_devind, m_canind, 0, (byte*)&baud) != 1)
                {
                    System.Windows.MessageBox.Show("设置波特率错误，打开设备失败!", "错误");
                    CAN_2E_U.VCI_CloseDevice(m_devtype, m_devind);
                    return;
                }
                //滤波设置
                //////////////////////////////////////////////////////////////////////////
                CH1_m_bOpen = 1;
                CAN_2E_U.VCI_INIT_CONFIG config = new CAN_2E_U.VCI_INIT_CONFIG();
                config.AccCode = System.Convert.ToUInt32("0x" + "00000000", 16);
                config.AccMask = System.Convert.ToUInt32("0x" + "FFFFFFFF", 16);
                config.Timing0 = System.Convert.ToByte("0x" + "00", 16);
                config.Timing1 = System.Convert.ToByte("0x" + "14", 16);
                config.Filter = 1;//(Byte)comboBox_Filter.SelectedIndex;
                config.Mode = 0;// (Byte)comboBox_Mode.SelectedIndex;
                CAN_2E_U.VCI_InitCAN(m_devtype, m_devind, m_canind, ref config);

                //////////////////////////////////////////////////////////////////////////
                Int32 filterMode = 2;// comboBox_e_u_Filter.SelectedIndex;
                if (2 != filterMode)//不是禁用
                {
                    CAN_2E_U.VCI_FILTER_RECORD filterRecord = new CAN_2E_U.VCI_FILTER_RECORD();
                    filterRecord.ExtFrame = (UInt32)filterMode;
                    filterRecord.Start = System.Convert.ToUInt32("0x" + 1, 16);
                    filterRecord.End = System.Convert.ToUInt32("0x" + "FF", 16);
                    //填充滤波表格
                    CAN_2E_U.VCI_SetReference(m_devtype, m_devind, m_canind, 1, (byte*)&filterRecord);
                    //使滤波表格生效
                    byte tm = 0;
                    if (CAN_2E_U.VCI_SetReference(m_devtype, m_devind, m_canind, 2, &tm) != STATUS_OK)
                    {
                        System.Windows.MessageBox.Show("设置滤波失败", "错误");
                        CAN_2E_U.VCI_CloseDevice(m_devtype, m_devind);
                        return;
                    }
                }
                CAN_2E_U.VCI_StartCAN(m_devtype, m_devind, m_canind);


                Task.Factory.StartNew(DataReadCAN);
            }
            catch (Exception ex) { System.Windows.MessageBox.Show(ex.ToString() + "CH1_CAN连接错误！", "提示"); }
        }

        private void DataReadCAN()
        {
            while (true)
            {
                //读取CAN报文
                canRead(-1, out List<CanMsg> CanMsgList1);
                //添加到CAN报文数组中
                CanMsgList.AddRange(CanMsgList1);
                for (int i = 0; i < CanMsgList1.Count; i++)
                {
                    if (CanMsgList1[i].ID == IdAddC)
                    {
                        CanMsgList.Add(CanMsgList1[i]);
                    }
                }

                Thread.Sleep(500);
            }
        }
        //写入CAN数据
        unsafe public void canWrite(int ID, byte[] data, string dataType)
        {

            VCI_CAN_OBJ VCO = new VCI_CAN_OBJ
            {
                ID = (uint)ID,
                RemoteFlag = 0,
                DataLen = 8,
                SendType = 0,
                ExternFlag = 1
            };
            for (int i = 0; i < 8; i++)
            {
                VCO.Data[i] = data[i];
            }

            if (CAN_2E_U.VCI_Transmit(m_devtype, 0, 0, ref VCO, 1) == 0)
            {
                System.Windows.MessageBox.Show("CH1_CAN发送失败", "错误");
            }
        }

        //读取CAN数据
        unsafe public void canRead(int filterID, out List<CanMsg> CanMsgList)
        {
            CanMsgList = new List<CanMsg>();
            uint receiveNum = CAN_2E_U.VCI_GetReceiveNum(m_devtype, 0, 0);
            if (receiveNum <= 0)
                return;
            if (receiveNum > 100)
                receiveNum = 100;
            IntPtr pt = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(VCI_CAN_OBJ)) * (Int32)receiveNum);
            uint len = CAN_2E_U.VCI_Receive(m_devtype, 0, 0, pt, receiveNum, -1);
            if (len <= 0)
            {
                //注意：如果没有读到数据则必须调用此函数来读取出当前的错误码
                CAN_2E_U.VCI_ERR_INFO errInfo = new CAN_2E_U.VCI_ERR_INFO();
                CAN_2E_U.VCI_ReadErrInfo(m_devtype, 0, 0, ref errInfo);
            }
            else
            {
                for (int i = 0; i < receiveNum; i++)
                {
                    VCI_CAN_OBJ obj = (VCI_CAN_OBJ)Marshal.PtrToStructure((IntPtr)((UInt64)pt +
                        (UInt64)i * (UInt64)Marshal.SizeOf(typeof(VCI_CAN_OBJ))), typeof(VCI_CAN_OBJ));
                    if ((filterID == -1 & obj.TimeStamp > 0) | (filterID != -1 & obj.ID == filterID & obj.TimeStamp > 0))
                    {
                        CanMsg CanMsgTemp = new CanMsg();
                        CanMsgTemp.TimeStamp = obj.TimeStamp / 10;
                        CanMsgTemp.ID = (int)obj.ID;
                        for (int j = 0; j < 8; j++)
                        {
                            CanMsgTemp.Data[j] = obj.Data[j];
                        }
                        if (obj.ExternFlag == 0)
                            CanMsgTemp.dataType = "标准帧";
                        if (obj.ExternFlag == 1)
                            CanMsgTemp.dataType = "扩展帧";
                        CanMsgList.Add(CanMsgTemp);
                    }
                }
            }
        }

        public void CloseCanDevice()
        {
            CAN_2E_U.VCI_CloseDevice(m_devtype, m_devind);
        }
    }
}
