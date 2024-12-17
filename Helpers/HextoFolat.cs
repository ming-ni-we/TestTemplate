using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestItemTemplate.Helpers
{
    internal class HextoFolat
    {
        //16进制浮点转单精度数（不安全代码）
        public static float ToFloat(byte[] data)
        {
            byte[] bytes = new byte[4];
            for (int j = 0; j < data.Length; j++)

            { bytes[j] = data[3 - j]; }

            float a = 0;
            byte i;
            byte[] x = bytes;
            unsafe
            {
                void* pf;
                fixed (byte* px = x)
                {
                    pf = &a;
                    for (i = 0; i < bytes.Length; i++)
                    {
                        *((byte*)pf + i) = *(px + i);
                    }
                }
            }
            return a;
        }

        //16进制浮点数转单精度
        public static float Hex_To_Float(byte[] Hex_data)
        {
            byte[] bytes = new byte[4];
            for (int i = 0; i < Hex_data.Length; i++)

                bytes[i] = Hex_data[3 - i];

            float m = BitConverter.ToSingle(bytes, 0);

            return m;

        }

        //将文本框中的内容转为字节数组
        public static byte[] HexStringToByteArr(string s)
        {
            s = s.Replace(" ", "");

            byte[] buffer = new byte[s.Length / 2];
            for (int i = 0; i < s.Length; i += 2)
            {
                buffer[i / 2] = (byte)Convert.ToByte(s.Substring(i, 2), 16);
            }

            return buffer;
        }

        //单精度浮点数转16进制字节
        public static byte[] ToByte(float data)
        {
            unsafe
            {
                byte* pdata = (byte*)&data;
                byte[] byteArray = new byte[sizeof(float)];
                for (int i = 0; i < sizeof(float); ++i)
                    byteArray[i] = *pdata++;

                return byteArray;
            }
        }
        //Byte转二进制函数
        public static string ByteTostring(byte Bytet)
        {

            string DataToBin = Convert.ToString(Bytet, 2);

            if (DataToBin.Length < 8)

            {
                DataToBin = DataToBin.PadLeft(8, '0');

            }

            return DataToBin;
        }

        //int转二进制函数
        public static string intTostring(int indata)
        {
            string DataToBin = Convert.ToString(indata, 2);
            if (DataToBin.Length < 16)

            {
                DataToBin = DataToBin.PadLeft(16, '0');

            }
            return DataToBin;
        }

        //int转状态显示
        public static string intToBinstring(int indata)
        {
            string strBin = "";
            int Bindata;
            for (int i = 0; i < 16; i++)
            {
                Bindata = indata % 2;
                indata = indata >> 1;
                strBin = strBin + (Convert.ToString(Bindata)).TrimStart();
            }

            return strBin;
        }

        //Byte转状态显示
        public static string ByteToBinstring(byte byted)
        {
            string strBin = "";

            int Bindata, sumdta;

            sumdta = Convert.ToInt16(byted);

            for (int i = 0; i < 8; i++)
            {
                Bindata = sumdta % 2;
                sumdta = sumdta >> 1;
                strBin = strBin + (Convert.ToString(Bindata)).TrimStart();
            }

            return strBin;
        }


        //将文本框中的内容转为字节数组
        public static byte[] LockHexStringToByteArr(string s)
        {
            s = s.Replace("-", "");

            byte[] buffer = new byte[s.Length / 2];
            for (int i = 0; i < s.Length; i += 2)
            {
                buffer[i / 2] = (byte)Convert.ToByte(s.Substring(i, 2), 16);
            }

            return buffer;
        }

        //交流负载功率设定转换
        public static ushort flat2ffegvalue(float fValue)
        {
            int iTepValue = 0;
            ushort nRegValue = 0;
            if (fValue > 9999.999)
            {
                return nRegValue;
            }
            else if (nRegValue > 999.9999)
            {
                nRegValue = (ushort)fValue;
            }
            else
            {
                iTepValue = (int)(fValue * 1000000);
                if (iTepValue % 10 > 0)
                {
                    iTepValue += 1;
                }
                iTepValue /= 10;

                if ((iTepValue <= 99990000) && (iTepValue >= 10000000))  //55065997
                {
                    nRegValue = (ushort)(iTepValue / 10000 + 10000);
                }
                else if (iTepValue >= 1000000)
                {
                    nRegValue = (ushort)(iTepValue / 1000 + 20000);
                }
                else if (iTepValue >= 10000)
                {
                    nRegValue = (ushort)(iTepValue / 100 + 30000);
                }
                else if (iTepValue >= 1000)
                {
                    nRegValue = (ushort)(iTepValue / 10 + 40000);
                }
                else if (iTepValue > 0)
                {
                    nRegValue = (ushort)(iTepValue + 50000);
                }
                else
                {
                    nRegValue = 0;
                }
            }

            return nRegValue;
        }

        //负载数据接收转换
        public static float fRegValue2float(ushort nValue)
        {
            float fValue = 0.0f;

            byte i, ucDotNumber = 0;

            if (nValue < 10000)
            {
                fValue = nValue;
            }

            else
            {
                ucDotNumber = Convert.ToByte(nValue / 10000);
                fValue = (float)(nValue % 10000);

                for (i = 0; i < ucDotNumber; i++)
                {
                    fValue /= 10;
                }
            }
            return fValue;
        }

        //short转字节
        public static byte[] shorToByte(ushort Value)
        {
            byte[] Data_p = new byte[2];

            Data_p[0] = Convert.ToByte(Value >> 8 & 0xff);

            Data_p[1] = Convert.ToByte(Value & 0xff);

            return Data_p;
        }

        //ChargingTexToHex
        public static string[] TexToHex(string[] Texstr)
        {
            string[] w_str = new string[3];

            w_str[0] = "00";

            try
            {
                w_str[1] = Convert.ToString(Convert.ToUInt32(Texstr[0]) / 10, 16);

                if (w_str[1].Length < 4) { if (w_str[1].Length == 3) { w_str[1] = "0" + w_str[1]; } else if (w_str[1].Length == 2) { w_str[1] = "00" + w_str[1]; } else if (w_str[1].Length == 1) { w_str[1] = "000" + w_str[1]; } }

                w_str[2] = Convert.ToString(Convert.ToUInt32(Texstr[1]), 16);

                if (w_str[2].Length < 4) { if (w_str[2].Length == 3) { w_str[2] = "0" + w_str[2]; } else if (w_str[2].Length == 2) { w_str[2] = "00" + w_str[2]; } else if (w_str[2].Length == 1) { w_str[2] = "000" + w_str[2]; } }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }

            return w_str;

        }
        public static byte[] HexStaingTobyteArray(string s)
        {
            s = s.Replace(" ", "");

            byte[] Buff = new byte[s.Length / 2];

            for (int i = 0; i < s.Length; i += 2)
            {
                Buff[i / 2] = (byte)Convert.ToByte(s.Substring(i, 2), 16);

            }
            return Buff;
        }

    }
}
