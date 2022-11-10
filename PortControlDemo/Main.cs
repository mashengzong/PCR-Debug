using Sunny.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PortControlDemo
{
    public partial class Main : UIForm
    {
        #region 字段/属性
        int[] BaudRateArr = new int[] { 9600, 4800, 115200 };
        int[] DataBitArr = new int[] { 8 };
        int[] StopBitArr = new int[] { 1, 2, 3 };
        int[] TimeoutArr = new int[] { 500, 1000, 2000, 5000, 10000 };
        object[] CheckBitArr = new object[] { "None" };
        private bool ReceiveState = false;
        private PortControlHelper pchSend;
        #endregion

        /// <summary>
        /// 初始化控件
        /// </summary>
        private void InitView()
        {
            uiComboBox3.DataSource = pchSend.PortNameArr;
            uiComboBox4.DataSource = BaudRateArr;
            uiComboBox5.DataSource = DataBitArr;
            uiComboBox6.DataSource = StopBitArr;
            uiComboBox7.DataSource = CheckBitArr;
            uiComboBox8.DataSource = TimeoutArr;
        }

        public Main()
        {
            InitializeComponent();
            pchSend = new PortControlHelper();
            InitView();
        }

        /// <summary>
        /// 根据串口信息打开相应串口
        /// </summary>
        private void open_serial()
        {

            if (pchSend.PortState)
            {
                pchSend.ClosePort();
                uiButton50.Text = "打开";
                UIMessageTip.Show(AppCode.Close_success);
            }
            else
            {
                pchSend.OpenPort(uiComboBox3.Text, int.Parse(uiComboBox4.Text),
                    int.Parse(uiComboBox5.Text), int.Parse(uiComboBox6.Text),
                    int.Parse(uiComboBox8.Text));
                uiButton50.Text = "关闭";
                UIMessageTip.Show(AppCode.Open_success);
            }
            pchSend.OnComReceiveDataHandler = new PortControlHelper.ComReceiveDataHandler(ComReceiveData);

            ReceiveState = true;
        }


        /// <summary>
        /// 按键状态变化显示
        /// </summary>
        void btn_state(bool state) {
            foreach (Control control in uiTitlePanel1.Controls)
            {
                if (control is Button || control is UIButton)
                {
                    control.Enabled = state;
                }
            }
            foreach (Control control in uiTitlePanel2.Controls)
            {
                if (control is Button || control is UIButton)
                {
                    control.Enabled = state;
                }
            }
            foreach (Control control in uiTitlePanel3.Controls)
            {
                if (control is Button || control is UIButton)
                {
                    control.Enabled = state;
                }
            }
            foreach (Control control in uiTitlePanel4.Controls)
            {
                if (control is Button || control is UIButton)
                {
                    control.Enabled = state;
                }
            }
            foreach (Control control in uiTitlePanel5.Controls)
            {
                if (control is Button || control is UIButton)
                {
                    control.Enabled = state;
                }
            }
            foreach (Control control in uiTitlePanel6.Controls)
            {
                if (control is Button || control is UIButton)
                {
                    control.Enabled = state;
                }
            }
        }

        /// <summary>
        /// 接收到的数据，写入文本框内
        /// </summary>
        /// <param name="data"></param>
        private void ComReceiveData(string data)
        {
            this.Invoke(new Action(() =>
            {
                Console.WriteLine("接收" + data);
                log4netHelper.Info(string.Format("接收数据：" + data));
                if (data.Substring(0, 4).Equals("A064"))
                {
                    switch (data.Substring(4, 10))
                    {
                        #region 初始化
                        case "3000000000":
                            btn_state(true);
                            break;
                        #endregion

                        #region 激发滤光片复位
                        case "3101000000":
                            btn_state(true);
                            break;
                        #endregion

                        #region 激发滤光片123456移动
                        case "3104000000":
                            btn_state(true);
                            break;
                        #endregion

                        #region 荧光滤光片复位
                        case "3201000000":
                            btn_state(true);
                            break;
                        #endregion

                        #region 荧光滤光片123456移动
                        case "3204000000":
                            btn_state(true);
                            break;
                        #endregion

                        #region 开关风扇
                        case "3600000000":
                            if (flag == 1)
                            {
                                //开风扇返回
                                UIMessageTip.Show(AppCode.Open_success);
                            }
                            else {
                                //关风扇返回
                                UIMessageTip.Show(AppCode.Close_success);
                            }
                            break;
                        #endregion

                        #region 灯泡模式
                        case "3400000000":
                            btn_state(true);
                            if (flag == 1)
                            {
                                //热机模式
                                UIMessageTip.Show(AppCode.Open_success);
                            }
                            else
                            {
                                //工作模式
                                UIMessageTip.Show(AppCode.Open_success);
                            }
                            break;
                        #endregion

                        #region 蜂鸣器配置
                        case "3701000000":
                            btn_state(true);
                            UIMessageTip.Show(AppCode.Configure_success);
                            break;
                        #endregion

                        #region PCR不在板
                        case "3801000000":
                            btn_state(true);
                            uiTextBox1.Text = AppCode.No_board;
                            break;
                        #endregion

                        #region PCR在板
                        case "3801000001":
                            btn_state(true);
                            uiTextBox1.Text = AppCode.board;
                            break;
                        #endregion

                        #region 加热温控打开关闭
                        case "6001000000":
                            btn_state(true);
                            if (flag == 1)
                            {
                                //打开
                                UIMessageTip.Show(AppCode.Open_success);
                            }
                            else
                            {
                                //关闭
                                UIMessageTip.Show(AppCode.Close_success);
                            }
                            break;
                        #endregion

                        #region Y轴开关舱
                        case "3300000001":
                            btn_state(true);
                            if (flag == 1)
                            {
                                //打开
                                UIMessageTip.Show(AppCode.Open_success);
                            }
                            else
                            {
                                //关闭
                                UIMessageTip.Show(AppCode.Close_success);
                            }
                            break;
                        #endregion

                        #region Z轴释放压紧
                        case "3301000000":
                            btn_state(true);
                            if (flag == 1)
                            {
                                //释放
                                UIMessageTip.Show(AppCode.Release_success);
                            }
                            else
                            {
                                //压紧
                                UIMessageTip.Show(AppCode.Compression_success);
                            }
                            break;
                        #endregion

                        #region 翻盖电机开光盖
                        case "3302000000":
                            btn_state(true);
                            if (flag == 1)
                            {
                                //开盖
                                UIMessageTip.Show(AppCode.Release_success);
                            }
                            else
                            {
                                //关盖
                                UIMessageTip.Show(AppCode.Compression_success);
                            }
                            break;
                            #endregion
                    }
                }

            }));
        }

        private int flag = 1;

        #region 打开串口
        private void uiButton50_Click(object sender, EventArgs e)
        {
            if (uiComboBox3.Text == "")
            {
                MessageBox.Show("未识别端口");
                return;
            }
            open_serial();
        }
        #endregion

        #region 初始化
        private void uiButton35_Click(object sender, EventArgs e)
        {
            btn_state(false);
            pchSend.SendData("3000000000000000");
        }
        #endregion

        #region 激发滤光片复位
        private void uiButton34_Click(object sender, EventArgs e)
        {
            btn_state(false);
            pchSend.SendData("3101010000000000");
        }
        #endregion

        #region 激发滤光片1
        private void uiButton28_Click(object sender, EventArgs e)
        {
            btn_state(false);
            pchSend.SendData("3104010000000000");
        }
        #endregion

        #region 激发滤光片2
        private void uiButton29_Click(object sender, EventArgs e)
        {
            btn_state(false);
            pchSend.SendData("3104020000000000");
        }
        #endregion

        #region 激发滤光片3
        private void uiButton30_Click(object sender, EventArgs e)
        {
            btn_state(false);
            pchSend.SendData("3104030000000000");
        }
        #endregion

        #region 激发滤光片4
        private void uiButton31_Click(object sender, EventArgs e)
        {
            btn_state(false);
            pchSend.SendData("3104040000000000");
        }
        #endregion

        #region 激发滤光片5
        private void uiButton33_Click(object sender, EventArgs e)
        {
            btn_state(false);
            pchSend.SendData("3104050000000000");
        }
        #endregion

        #region 激发滤光片6
        private void uiButton32_Click(object sender, EventArgs e)
        {
            btn_state(false);
            pchSend.SendData("3104060000000000");
        }
        #endregion

        #region 荧光滤光片复位
        private void uiButton41_Click(object sender, EventArgs e)
        {
            btn_state(false);
            pchSend.SendData("3201010000000000");
        }
        #endregion

        #region 荧光滤光片1
        private void uiButton36_Click(object sender, EventArgs e)
        {
            btn_state(false);
            pchSend.SendData("3204010000000000");
        }
        #endregion

        #region 荧光滤光片2
        private void uiButton37_Click(object sender, EventArgs e)
        {
            btn_state(false);
            pchSend.SendData("3204020000000000");
        }
        #endregion

        #region 荧光滤光片3
        private void uiButton38_Click(object sender, EventArgs e)
        {
            btn_state(false);
            pchSend.SendData("3204030000000000");
        }
        #endregion

        #region 荧光滤光片4
        private void uiButton39_Click(object sender, EventArgs e)
        {
            btn_state(false);
            pchSend.SendData("3204040000000000");
        }
        #endregion

        #region 荧光滤光片5
        private void uiButton42_Click(object sender, EventArgs e)
        {
            btn_state(false);
            pchSend.SendData("3204050000000000");
        }
        #endregion

        #region 荧光滤光片6
        private void uiButton40_Click(object sender, EventArgs e)
        {
            btn_state(false);
            pchSend.SendData("3204060000000000");
        }
        #endregion

        #region 风扇控制
        private void uiSwitch1_ValueChanged(object sender, bool value)
        {
            if (uiSwitch1.Active)
            {
                //开风扇
                flag = 1;
                pchSend.SendData("3600010000000000");
            }
            else {
                //关风扇
                flag = 2;
                pchSend.SendData("3600000000000000");
            }
        }
        #endregion

        #region 热机模式
        private void uiButton45_Click(object sender, EventArgs e)
        {
            flag = 1;
            btn_state(false);
            pchSend.SendData("3400000000000000");
        }
        #endregion

        #region 工作模式
        private void uiButton46_Click(object sender, EventArgs e)
        {
            flag = 2;
            btn_state(false);
            pchSend.SendData("3400010000000000");
        }
        #endregion

        #region 蜂鸣器配置
        private void uiButton48_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(uiComboBox1.Text) || string.IsNullOrEmpty(uiComboBox2.Text))
            {
                UIMessageTip.Show(AppCode.Null_value);
                return;
            }
            if (int.Parse(uiComboBox1.Text) >= 65535 || int.Parse(uiComboBox2.Text) >= 65535)
            {
                UIMessageTip.Show(AppCode.Value_large);
                return;
            }
            btn_state(false);
            pchSend.SendData("3701" + Convert.ToString(int.Parse(uiComboBox1.Text), 16).PadLeft(2, '0')
                + Convert.ToString(int.Parse(uiComboBox2.Text), 16).PadLeft(4, '0') + "000000");
        }
        #endregion

        #region PCR在板监测
        private void uiButton49_Click(object sender, EventArgs e)
        {
            btn_state(false);
            pchSend.SendData("3801000000000000");
        }
        #endregion

        #region RGB指示灯颜色配置
        private void uiButton47_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(uiDoubleUpDown1.Value.ToString()) || string.IsNullOrEmpty(uiDoubleUpDown2.Value.ToString())
                || string.IsNullOrEmpty(uiDoubleUpDown3.Value.ToString()))
            {
                UIMessageTip.Show(AppCode.Null_value);
                return;
            }
            if (uiDoubleUpDown1.Value > 256 || uiDoubleUpDown2.Value > 256 || uiDoubleUpDown3.Value > 256)
            {
                UIMessageTip.Show(AppCode.ValueColor_large);
                return;
            }
            btn_state(false);
            pchSend.SendData("3700" + Convert.ToString(int.Parse(uiDoubleUpDown1.Value.ToString()), 16).PadLeft(2, '0')
                + Convert.ToString(int.Parse(uiDoubleUpDown2.Value.ToString()), 16).PadLeft(2, '0')
                 + Convert.ToString(int.Parse(uiDoubleUpDown3.Value.ToString()), 16).PadLeft(2, '0') + "000000");
        }
        #endregion

        #region 查询
        private void uiButton51_Click(object sender, EventArgs e)
        {

        }
        #endregion

        #region 加热温控打开
        private void uiButton55_Click(object sender, EventArgs e)
        {
            flag = 1;
            btn_state(false);
            pchSend.SendData("600101" + Convert.ToString(uiRadioButton2.Checked == true ? 1 : 2, 16).PadLeft(2, '0')
                + Convert.ToString(int.Parse(uiDoubleUpDown11.Text), 16).PadLeft(2, '0')
                 + Convert.ToString(int.Parse(uiDoubleUpDown10.Text), 16).PadLeft(2, '0') + "0000");
        }
        #endregion

        #region 加热温控关闭
        private void uiButton54_Click(object sender, EventArgs e)
        {
            flag = 2;
            btn_state(false);
            pchSend.SendData("600100" + Convert.ToString(uiRadioButton2.Checked == true ? 1 : 2, 16).PadLeft(2, '0')
                + Convert.ToString(int.Parse(uiDoubleUpDown11.Text), 16).PadLeft(2, '0')
                 + Convert.ToString(int.Parse(uiDoubleUpDown10.Text), 16).PadLeft(2, '0') + "0000");
        }
        #endregion

        #region 获取温度查询
        private void uiButton57_Click(object sender, EventArgs e)
        {
            int i = uiRadioButton_tec1.Checked == true ? 1 : uiRadioButton_tec2.Checked == true ? 2 : uiRadioButton_tec3.Checked == true ? 3 :
                uiRadioButton_tec4.Checked == true ? 4 : uiRadioButton_tec5.Checked == true ? 5 : uiRadioButton_tec6.Checked == true ? 6 :
                uiRadioButton3.Checked == true ? 7 : uiRadioButton4.Checked == true ? 8 : 9;//获取radiobutton选中项
            btn_state(false);
            pchSend.SendData("FF01" + Convert.ToString(i, 16).PadLeft(2, '0') + "0000000000");
        }
        #endregion

        #region Y电机开舱
        private void uiButton23_Click(object sender, EventArgs e)
        {
            flag = 1;
            btn_state(false);
            pchSend.SendData("3300000000000000");
        }
        #endregion

        #region 关舱
        private void uiButton22_Click(object sender, EventArgs e)
        {
            flag = 2;
            btn_state(false);
            pchSend.SendData("3300010000000000");
        }
        #endregion

        #region z释放
        private void uiButton44_Click(object sender, EventArgs e)
        {
            flag = 1;
            btn_state(false);
            pchSend.SendData("3301000000000000");
        }
        #endregion

        #region z压紧
        private void uiButton43_Click(object sender, EventArgs e)
        {
            flag = 2;
            btn_state(false);
            pchSend.SendData("3301010000000000");
        }
        #endregion

        #region 翻盖电机开盖
        private void uiButton25_Click(object sender, EventArgs e)
        {
            flag = 1;
            btn_state(false);
            pchSend.SendData("3302000000000000");
        }
        #endregion

        #region 翻盖电机关盖
        private void uiButton24_Click(object sender, EventArgs e)
        {
            flag = 2;
            btn_state(false);
            pchSend.SendData("3302010000000000");
        }
        #endregion
    }
}
