using System;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace PortControlDemo
{
    public class PortControlHelper
    {
        #region 字段/属性/委托
        /// <summary>
        /// 串行端口对象
        /// </summary>
        private SerialPort sp;

        /// <summary>
        /// 串口接收数据委托
        /// </summary>
        public delegate void ComReceiveDataHandler(string data);

        public ComReceiveDataHandler OnComReceiveDataHandler = null;

        /// <summary>
        /// 端口名称数组
        /// </summary>
        public string[] PortNameArr { get; set; }

        /// <summary>
        /// 串口通信开启状态
        /// </summary>
        public bool PortState { get; set; } = false;
        
        /// <summary>
        /// 编码类型
        /// </summary>
        public Encoding EncodingType { get; set; } = Encoding.UTF8;
        #endregion

        #region 方法
        public PortControlHelper()
        {
            PortNameArr = SerialPort.GetPortNames();
            sp = new SerialPort();
            sp.DataReceived += new SerialDataReceivedEventHandler(DataReceived);
        }

        /// <summary>
        /// 打开端口
        /// </summary>
        /// <param name="portName">端口名称</param>
        /// <param name="boudRate">波特率</param>
        /// <param name="dataBit">数据位</param>
        /// <param name="stopBit">停止位</param>
        /// <param name="timeout">超时时间</param>
        public void OpenPort(string portName , int boudRate = 115200, int dataBit = 8, int stopBit = 1, int timeout = 5000)
        {
            try
            {
                sp.PortName = portName;
                sp.BaudRate = boudRate;
                sp.DataBits = dataBit;
                sp.StopBits = (StopBits)stopBit;
                sp.ReadTimeout = timeout;
                sp.Open();
                PortState = true;
            }
            catch (Exception e)
            {
                MessageBox.Show("端口连接失败");
            }
        }

        /// <summary>
        /// 关闭端口
        /// </summary>
        public void ClosePort()
        {
            try
            {
                sp.Close();
                PortState = false;
            }
            catch (Exception e)
            {
                MessageBox.Show("端口关闭失败");
            }
        }


        /// <summary>
        /// 十六进制转换字节数组
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public byte[] HexStringToByteArray(string s)
        {
            s = s.Replace(" ", "");
            byte[] buffer = new byte[s.Length / 2];
            for (int i = 0; i < s.Length; i += 2)
                buffer[i / 2] = (byte)Convert.ToByte(s.Substring(i, 2), 16);
            return buffer;
        }

        /// <summary>
        /// 负数转十六进制
        /// </summary>
        /// <param name="iNumber"></param>
        /// <returns></returns>
        public static string NegativeToHexString(int iNumber, int lenth)
        {
            string strResult = string.Empty;
            if (iNumber > 0)
            {
                iNumber = -iNumber;
                if (iNumber < 0)
                {
                    iNumber = -iNumber;//转为正数
                    string strNegt = string.Empty;
                    char[] binChar = Convert.ToString(iNumber, 2).PadLeft(32, '0').ToArray();//把iNumber转换成2进制，从左侧用0填充达到总长度
                    foreach (char ch in binChar)
                    {
                        if (Convert.ToInt32(ch) == 48)
                        {
                            strNegt += "1";
                        }
                        else
                        {
                            strNegt += "0";
                        }
                    }
                    int iComplement = Convert.ToInt32(strNegt, 2) + 1;//strNegt由2进制转为10进制
                    strResult = Convert.ToString(iComplement, 16).ToUpper();//iComplement由十进制转为16进制
                }
            }
            return Regex.Replace(strResult.PadLeft(lenth, 'F'), @"(?<=[0-9A-F]{2})[0-9A-F]{2}", " $0");//将字符串用空格隔开
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="sendData"></param>
        public void SendData(string sendData)
        {
            try
            {
                sp.Encoding = EncodingType;
                byte[] byteData = HexStringToByteArray(sendData);

                sp.Write(byteData, 0, byteData.Length);
                Console.WriteLine("发送"+ sendData);
                log4netHelper.Info(string.Format("发送数据：" + sendData));

            }
            catch (Exception e)
            {
                MessageBox.Show("端口连接失败");
            }
        }

        /// <summary>
        /// Byte 数组转十六进制字符串
        /// </summary>
        /// <param name="Bytes"></param>
        /// <returns></returns>
        public static string ByteToHex(byte[] Bytes)
        {
            string str = string.Empty;
            foreach (byte Byte in Bytes)
            {
                str += String.Format("{0:X2}", Byte) + " ";
            }
            return str.Trim();
        }


        string str = "";

        /// <summary>
        /// 接收数据回调用
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataReceived(object sender, SerialDataReceivedEventArgs e)
        {

            byte[] buffer = new byte[sp.BytesToRead];
            sp.Read(buffer, 0, buffer.Length);
            str += ByteToHex(buffer);
            if (OnComReceiveDataHandler != null && str.Replace(" ", "").Length >= 16)
            {                
                OnComReceiveDataHandler(str.Replace(" ", ""));
            }
            if (str.Replace(" ", "").Length >= 16) {
                str = "";
            }
        }
        #endregion
    }
}