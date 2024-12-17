using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestItemTemplate.Helpers;

namespace TestItemTemplate.Models
{
    public enum ElectronicLoadMode
    {
        /// <summary>
        /// 恒流
        /// </summary>
        CC = 0x00,
        /// <summary>
        /// 恒压
        /// </summary>
        CV = 0x01,
        /// <summary>
        /// 恒功率
        /// </summary>
        CP = 0x03
    }
    public partial class Load2108 : ObservableObject
    {
        private static readonly object _lockObject = new object();
        public static byte[] ElectronicLoadState = { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
        public SerialCommnication serialPort = null;
        /// <summary>
        /// 索引
        /// </summary>
        [ObservableProperty]
        private byte index;
        /// <summary>
        /// 是否开启
        /// </summary>
        [ObservableProperty]
        private bool isOpen;
        /// <summary>
        /// 当前电压值
        /// </summary>
        [ObservableProperty]
        private float voltage;
        /// <summary>
        /// 当前电流值
        /// </summary>
        [ObservableProperty]
        private float current;
        /// <summary>
        /// 当前功率值
        /// </summary>
        [ObservableProperty]
        private float power;
        /// <summary>
        /// 设置电压值
        /// </summary>
        [ObservableProperty]
        private int voltageSet = 27000;
        /// <summary>
        /// 设置电流值
        /// </summary>
        [ObservableProperty]
        private int currentSet = 5000;
        /// <summary>
        /// 设置功率值
        /// </summary>
        [ObservableProperty]
        private int powerSet = 15;
        /// <summary>
        /// 设置模式
        /// </summary>
        [ObservableProperty]
        ElectronicLoadMode modeEl = ElectronicLoadMode.CV;
        [RelayCommand]
        /// <summary>
        /// 开启电子负载
        /// </summary>
        public void OnPowerOn(byte channle)
        {
            PowerON_OF(IndexToChannleOpen());
            IsOpen = true;
        }
        [RelayCommand]
        /// <summary>
        /// 关闭电子负载
        /// </summary>
        public void OnPowerOff(byte channle)
        {
            PowerON_OF(IndexToChannleClose());
            IsOpen = false;
            Voltage = 0;
            Current = 0;
        }
        /// <summary>
        /// 电子负载ON/OFF操作
        /// </summary>
        /// <param name="Channel"></param>
        private void PowerON_OF(byte open)
        {
            try
            {
                ElectronicLoadState[(Index - 1) / 8] = open;
                byte[] arraybuf = { (byte)((Index - 1) / 8 + 1), 0x06, 0x00, 0x00, 0x00, open, 0x00, 0x00 };
                CRC_16.MakeCRC_16(arraybuf, arraybuf.Length);
                lock (_lockObject)
                {
                    serialPort.EnqueueCommand(arraybuf);
                    Thread.Sleep(100);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        [RelayCommand]
        /// <summary>
        /// 批量设置直流电子负载CS1109
        /// </summary>
        /// <param name="startAddress"></param>
        /// <param name="PwValue"></param>
        /// <param name="RunMode"></param>
        /// <param name="ConbstantMode"></param>
        /// <returns></returns>
        public void Set_1109Parameter()
        {
            byte ConbstantMode = (byte)ModeEl;
            byte[] bytda = new byte[2];//设置功率数据
            switch (ModeEl)
            {
                case ElectronicLoadMode.CC:
                    bytda = HextoFolat.shorToByte(HextoFolat.flat2ffegvalue((float)VoltageSet / 1000));
                    break;
                case ElectronicLoadMode.CV:
                    bytda = HextoFolat.shorToByte(HextoFolat.flat2ffegvalue((float)CurrentSet / 1000));
                    break;
                case ElectronicLoadMode.CP:
                    bytda = HextoFolat.shorToByte(HextoFolat.flat2ffegvalue(PowerSet));
                    break;
                default:
                    break;
            }
            byte[] pdata = { (byte)((Index - 1) / 8+1), 0x10, 00, 1, 00,  0x58, 0xB0,
                0x00, ConbstantMode, bytda[0], bytda[1], 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00,
                0x00, ConbstantMode, bytda[0], bytda[1], 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00,
                0x00, ConbstantMode, bytda[0], bytda[1], 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00,
                0x00, ConbstantMode, bytda[0], bytda[1], 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00,
                0x00, ConbstantMode, bytda[0], bytda[1], 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00,
                0x00, ConbstantMode, bytda[0], bytda[1], 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00,
                0x00, ConbstantMode, bytda[0], bytda[1], 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00,
                0x00, ConbstantMode, bytda[0], bytda[1], 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00,
                0x00, 0x00};//批量写入寄存器数据；
            if (ModeEl == ElectronicLoadMode.CC)
            {
                pdata = [
                    00, 0x10, 00, 1, 00, 0x58, 0xB0,
                0x00, ConbstantMode, bytda[0], bytda[1], 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, bytda[0], bytda[1], 00, 00,
                0x00, ConbstantMode, bytda[0], bytda[1], 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, bytda[0], bytda[1], 00, 00,
                0x00, ConbstantMode, bytda[0], bytda[1], 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, bytda[0], bytda[1], 00, 00,
                0x00, ConbstantMode, bytda[0], bytda[1], 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, bytda[0], bytda[1], 00, 00,
                0x00, ConbstantMode, bytda[0], bytda[1], 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, bytda[0], bytda[1], 00, 00,
                0x00, ConbstantMode, bytda[0], bytda[1], 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, bytda[0], bytda[1], 00, 00,
                0x00, ConbstantMode, bytda[0], bytda[1], 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, bytda[0], bytda[1], 00, 00,
                0x00, ConbstantMode, bytda[0], bytda[1], 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, bytda[0], bytda[1], 00, 00,
                0x00, 0x00];//批量写入寄存器数据；
            }
            try
            {
                CRC_16.MakeCRC_16(pdata, pdata.Length);
                lock (_lockObject)
                {
                    serialPort.EnqueueCommand(pdata);
                    Thread.Sleep(100);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }


        /// <summary>
        /// 读取DC负载数据
        /// </summary>
        /// <param name="Adderss"></param>
        public void Read_Dcload_Value()
        {
            byte[] pdata = { Index, 0x04, 00, 0x09, 00, 0x18, 00, 00 };
            try
            {
                CRC_16.MakeCRC_16(pdata, pdata.Length);
                lock (_lockObject)
                {
                    serialPort.EnqueueCommand(pdata);
                    Thread.Sleep(100);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
        private byte IndexToChannleOpen()
        {
            return (byte)((0x01 << ((Index - 1) % 8)) | ElectronicLoadState[(Index - 1) / 8]);
        }

        private byte IndexToChannleClose()
        {
            return (byte)(~(0x01 << ((Index - 1) % 8)) & ElectronicLoadState[(Index - 1) / 8]);
        }
    }
}
