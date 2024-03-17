using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using 卓汇数据追溯系统.ConfigHelper;
using 卓汇数据追溯系统.Models;

namespace 卓汇数据追溯系统
{
    public partial class UserLoginFrm : Form
    {
        [DllImport("kernel32.dll")]
        static extern void sleep(int dwMilliseconds);
        [DllImport("MasterRDnew.dll")]
        static extern int lib_ver(ref uint pVer);
        [DllImport("MasterRDnew.dll")]
        static extern int rf_init_com(short icdev, int port, int baud);
        [DllImport("MasterRDnew.dll")]
        static extern int rf_antenna_sta(short icdev, byte mode);
        [DllImport("MasterRDnew.dll")]
        static extern int rf_init_type(short icdev, byte type);
        [DllImport("MasterRDnew.dll")]
        static extern int rf_request(short icdev, byte mode, ref ushort pTagType);
        [DllImport("MasterRDnew.dll")]
        static extern int rf_anticoll(short icdev, byte bcnt, ref byte pSnr, ref byte pRLength);
        [DllImport("MasterRDnew.dll")]
        static extern int rf_select(short icdev, ref byte pSnr, byte srcLen, ref sbyte Size);
        [DllImport("MasterRDnew.dll")]
        static extern int rf_halt(short icdev);
        [DllImport("MasterRDnew.dll")]
        static extern int rf_M1_authentication2(short icdev, byte mode, byte block, ref byte key);
        [DllImport("MasterRDnew.dll")]
        static extern int rf_M1_initval(short icdev, byte adr, Int32 value);
        [DllImport("MasterRDnew.dll")]
        static extern int rf_M1_increment(short icdev, byte adr, Int32 value);
        [DllImport("MasterRDnew.dll")]
        static extern int rf_M1_decrement(short icdev, byte adr, Int32 value);
        [DllImport("MasterRDnew.dll")]
        static extern int rf_M1_readval(short icdev, byte adr, ref Int32 pValue);
        [DllImport("MasterRDnew.dll")]
        static extern int rf_M1_read(short icdev, byte adr, ref byte pData, ref byte pLen);
        [DllImport("MasterRDnew.dll")]
        static extern int rf_M1_write(short icdev, byte adr, ref byte pData);
        [DllImport("MasterRDnew.dll")]
        static extern int rf_ClosePort();
        private MainFrm _mainparent;
        private delegate void LabelText(Label lb, string bl);
        private delegate void Labelcolor(Label lb, Color color, string bl);
        private delegate void ShowTxt(TextBox tb, string txt);
        private delegate void ShowLabel(string lb, string txt);
        private delegate void ShowPanelVisible(bool b, string txt);
        private delegate void cboSelect(int i);
        delegate void AddItemToListBoxDelegate(string str, string Name);
        SQLServer SQL = new SQLServer();
        List<Control> List_Control = new List<Control>();
        public UserLoginFrm(MainFrm mdiParent)
        {
            InitializeComponent();
            _mainparent = mdiParent;
            Global.inidata = new IniProductFile(@"D:\ZHH\Upload\setting.ini");

            //searchTextBox1.ComparedData += CompareDatas;
            //searchTextBox2.ComparedData += CompareDatas;
            //searchTextBox3.ComparedData += CompareDatas;
            //searchTextBox4.ComparedData += CompareDatas;
            //searchTextBox5.ComparedData += CompareDatas;
        }

        public void CompareDatas(SearchModel searchModel, string value)
        {
            searchModel.DefaultValue = value;
            if (Global.GySelected.ContainsKey(searchModel.Index))
            {
                Global.GySelected.Remove(searchModel.Index);

            }
            Global.GySelected.Add(searchModel.Index, searchModel);
            Log.WriteLog($"所选默认规则变化:规则{searchModel.Index},所选值:{searchModel.DefaultValue}", "物料匹配规则默认值");
        }

        private void tabControl1_DrawItem(object sender, DrawItemEventArgs e)//自定义绘制Tab标题
        {
            string text = ((TabControl)sender).TabPages[e.Index].Text;
            //标签背景填充颜色
            SolidBrush BackBrush = new SolidBrush(Color.Gray);
            SolidBrush brush = new SolidBrush(Color.Black);
            StringFormat sf = new StringFormat();
            //设置文字对齐方式
            sf.LineAlignment = StringAlignment.Center;
            sf.Alignment = StringAlignment.Center;
            if (e.Index == this.tabControl1.SelectedIndex)//当前Tab页的样式
            {
                BackBrush = new SolidBrush(Color.DarkSeaGreen);
            }
            //绘制标签头背景颜色
            e.Graphics.FillRectangle(BackBrush, e.Bounds);
            //绘制标签头文字
            //e.Graphics.DrawString(text, SystemInformation.MenuFont, brush, e.Bounds, sf);
            e.Graphics.DrawString(text, new Font("微软雅黑", 13), brush, e.Bounds, sf);
        }

        private void btn_Login_Click(object sender, EventArgs e)//用户登录
        {
            Global.UserLoginMouseMoveTime = DateTime.Now;
            Global.IfLogin = true;
            if (cbo_Login.SelectedIndex == 0)
            {
                if (txt_pwd.Text == Global.Operator_pwd)
                {
                    lbl_LoginLevel.Text = "操作员";
                    Global.Login = Global.LoginLevel.Operator;
                    labelcolor(lbl_LoginMes, Color.LimeGreen, "操作员登录成功");
                    Global.PLC_Client.WritePLC_D(20502, new short[] { 1 });
                    if (Global.client1.Connected)
                    {
                        Global.client1.Send(String.Format("USER;{0};{1};{2};\r\n", "LEV1", "", ""));// 发送权限等级给大族焊接机-1
                    }
                    if (Global.client2.Connected)
                    {
                        Global.client2.Send(String.Format("USER;{0};{1};{2};\r\n", "LEV1", "", ""));// 发送权限等级给大族焊接机-2
                    }
                }
                else
                {
                    labelcolor(lbl_LoginMes, Color.Red, "密码错误,请重新输入！");
                    UiText(txt_pwd, "");
                }
            }
            else if (cbo_Login.SelectedIndex == 1)
            {
                if (txt_pwd.Text == Global.Technician_pwd)
                {
                    lbl_LoginLevel.Text = "技术员";
                    Global.Login = Global.LoginLevel.Technician;
                    labelcolor(lbl_LoginMes, Color.LimeGreen, "技术员登录成功");
                    try
                    {
                        Global.PLC_Client.WritePLC_D(20502, new short[] { 2 });
                        if (Global.client1.Connected)
                        {
                            Global.client1.Send(String.Format("USER;{0};{1};{2};\r\n", "LEV2", "", ""));// 发送权限等级给大族焊接机-1
                        }
                        if (Global.client2.Connected)
                        {
                            Global.client2.Send(String.Format("USER;{0};{1};{2};\r\n", "LEV2", "", ""));// 发送权限等级给大族焊接机-2
                        }
                    }
                    catch
                    { }
                }
                else
                {
                    labelcolor(lbl_LoginMes, Color.Red, "密码错误,请重新输入！");
                    UiText(txt_pwd, "");
                }
            }
            else
            {
                if (txt_pwd.Text == Global.Administrator_pwd)
                {
                    lbl_LoginLevel.Text = "工程师";
                    Global.Login = Global.LoginLevel.Administrator;
                    labelcolor(lbl_LoginMes, Color.LimeGreen, "工程师登录成功");
                    try
                    {
                        Global.PLC_Client.WritePLC_D(20502, new short[] { 3 });
                        if (Global.client1.Connected)
                        {
                            Global.client1.Send(String.Format("USER;{0};{1};{2};\r\n", "LEV3", "", ""));// 发送权限等级给大族焊接机-1
                        }
                        if (Global.client2.Connected)
                        {
                            Global.client2.Send(String.Format("USER;{0};{1};{2};\r\n", "LEV3", "", ""));// 发送权限等级给大族焊接机-2
                        }
                    }
                    catch
                    { }
                }
                else
                {
                    labelcolor(lbl_LoginMes, Color.Red, "密码错误,请重新输入！");
                    UiText(txt_pwd, "");
                }
            }
        }

        public void labelText(Label lb, string txt)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new LabelText(labelText), new object[] { lb, txt });
                return;
            }
            lb.Text = txt;
        }

        public void labelcolor(Label lb, Color color, string str)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Labelcolor(labelcolor), new object[] { lb, color, str });
                return;
            }
            lb.BackColor = color;
            lb.Text = str;
        }

        public void UiText(TextBox tb, string str)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new ShowTxt(UiText), new object[] { tb, str });
                return;
            }
            tb.Text = str;
        }

        public void UiLabel(string str1, string Name)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new ShowLabel(UiLabel), new object[] { str1, Name });
                return;
            }
            foreach (Control ctrl in List_Control)
            {
                if (ctrl.GetType() == typeof(Label))
                {
                    if (ctrl.Name == Name)
                    {
                        ctrl.Text = str1;
                    }
                }
            }
        }

        public void AddList(string msg, string Name)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new AddItemToListBoxDelegate(AddList), new object[] { msg, Name });
                return;
            }
            foreach (Control ctrl in List_Control)
            {
                if (ctrl.GetType() == typeof(ListBox))
                {
                    if (ctrl.Name == Name)
                    {
                        if (msg != "N/A")
                        {
                            ((ListBox)ctrl).Items.Add(msg + "\r\n");
                        }
                        else
                        {
                            ((ListBox)ctrl).Items.Clear();
                        }
                    }
                }
            }

        }

        public void PanelVisible(bool str1, string Name)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new ShowPanelVisible(PanelVisible), new object[] { str1, Name });
                return;
            }
            foreach (Control ctrl in List_Control)
            {
                if (ctrl.GetType() == typeof(TableLayoutPanel))
                {
                    if (ctrl.Name == Name)
                    {
                        ctrl.Visible = str1;
                    }
                }
            }
        }
        public void showpq(bool status)
        {
            if (status)
            {
                Global.IfPQStatus = true;
                cb_Setup.Enabled = true;
                cb_IQ.Enabled = true;
                cb_OQ.Enabled = true;
                cb_PQ.Enabled = true;

                rb_MQ.Enabled = false;
                rb_Production.Enabled = false;
                //PanelVisible(true, "panel_PQ_Y");
                //PanelVisible(false, "panel_PQ_N");
            }
            else if (!status)
            {
                Global.IfPQStatus = false;
                cb_Setup.Enabled = false;
                cb_IQ.Enabled = false;
                cb_OQ.Enabled = false;
                cb_PQ.Enabled = false;

                rb_MQ.Enabled = true;
                rb_Production.Enabled = true;
            }

        }

        private void chk_DisplayPwd_CheckedChanged(object sender, EventArgs e)
        {
            if (chk_DisplayPwd.Checked)
            {
                txt_pwd.PasswordChar = new char();
            }
            else
            {
                txt_pwd.PasswordChar = '*';
            }
        }

        private void chk_DisplayOldPwd_CheckedChanged(object sender, EventArgs e)
        {
            if (chk_DisplayOldPwd.Checked)
            {
                txt_oldPwd.PasswordChar = new char();
            }
            else
            {
                txt_oldPwd.PasswordChar = '*';
            }
        }

        private void chk_DisplayNewPwd_CheckedChanged(object sender, EventArgs e)
        {
            if (chk_DisplayNewPwd.Checked)
            {
                txt_NewPwd.PasswordChar = new char();
            }
            else
            {
                txt_NewPwd.PasswordChar = '*';
            }
        }

        private void cbo_Login_SelectedIndexChanged(object sender, EventArgs e)
        {
            UiText(txt_pwd, "");
            txt_pwd.Focus();
        }

        private void btn_ChangePwd_Click(object sender, EventArgs e)
        {
            if (Global.Login == Global.LoginLevel.Operator)
            {
                if (txt_oldPwd.Text == Global.Operator_pwd)
                {
                    Global.inidata.productconfig.Operator_pwd = txt_NewPwd.Text;
                    Global.inidata.WriteProductConfigSection();
                    Global.Operator_pwd = Global.inidata.productconfig.Operator_pwd;
                    labelcolor(lbl_ChangePwdMes, Color.LimeGreen, "操作员密码更改成功");
                    UiText(txt_oldPwd, "");
                    UiText(txt_NewPwd, "");
                }
                else
                {
                    labelcolor(lbl_ChangePwdMes, Color.Red, "旧密码不正确,请重新输入!");
                    UiText(txt_oldPwd, "");
                    UiText(txt_NewPwd, "");
                    txt_oldPwd.Focus();
                }
            }
            else if (Global.Login == Global.LoginLevel.Technician)
            {
                if (txt_oldPwd.Text == Global.Technician_pwd)
                {
                    Global.inidata.productconfig.Technician_pwd = txt_NewPwd.Text;
                    Global.inidata.WriteProductConfigSection();
                    Global.Technician_pwd = Global.inidata.productconfig.Technician_pwd;
                    labelcolor(lbl_ChangePwdMes, Color.LimeGreen, "技术员密码更改成功");
                    UiText(txt_oldPwd, "");
                    UiText(txt_NewPwd, "");
                }
                else
                {
                    labelcolor(lbl_ChangePwdMes, Color.Red, "旧密码不正确,请重新输入!");
                    UiText(txt_oldPwd, "");
                    UiText(txt_NewPwd, "");
                    txt_oldPwd.Focus();
                }
            }
            else if (Global.Login == Global.LoginLevel.Administrator)
            {
                if (txt_oldPwd.Text == Global.Administrator_pwd)
                {
                    Global.inidata.productconfig.Administrator_pwd = txt_NewPwd.Text;
                    Global.inidata.WriteProductConfigSection();
                    Global.Administrator_pwd = Global.inidata.productconfig.Administrator_pwd;
                    labelcolor(lbl_ChangePwdMes, Color.LimeGreen, "管理员密码更改成功");
                    UiText(txt_oldPwd, "");
                    UiText(txt_NewPwd, "");
                }
                else
                {
                    labelcolor(lbl_ChangePwdMes, Color.Red, "旧密码不正确,请重新输入!");
                    UiText(txt_oldPwd, "");
                    UiText(txt_NewPwd, "");
                    txt_oldPwd.Focus();
                }
            }
        }

        private void UserLoginFrm_Load(object sender, EventArgs e)
        {
            List_Control = GetAllControls(this);//列表中添加所有窗体控件


            tabControl1.Controls.Remove(tabPage2);

            if (Global.inidata.productconfig.OpenCard == "true")
            {
                tabControl1.TabPages.Remove(tabPage1);
            }
            if (Global.Login == Global.LoginLevel.Operator)
            {
                lbl_LoginLevel.Text = "操作员";
                cbo_Login.SelectedIndex = 0;
            }
            if (Global.Login == Global.LoginLevel.Technician)
            {
                lbl_LoginLevel.Text = "技术员";
                cbo_Login.SelectedIndex = 1;
            }
            if (Global.Login == Global.LoginLevel.Administrator)
            {
                lbl_LoginLevel.Text = "工程师";
                cbo_Login.SelectedIndex = 2;
            }

            //读取PQ状态
            //try
            //{
            //    string Msg_pq;
            //    string str_pq = "";
            //    RequestAPI2.CallBobcat2(string.Format("http://10.128.10.7/Webapi/api/WorkTime/GetPqStatus?dsn={0}", Global.inidata.productconfig.OEE_Dsn), "", out str_pq, out Msg_pq, false);
            //    PqStatus status = JsonConvert.DeserializeObject<PqStatus>(str_pq);
            //    if (status.msg[0].isPQ)
            //    {
            //        Global.IfPQStatus = true;
            //        cb_Setup.Enabled = true;
            //        cb_IQ.Enabled = true;
            //        cb_OQ.Enabled = true;
            //        cb_PQ.Enabled = true;

            //        rb_MQ.Enabled = false;
            //        rb_Production.Enabled = false;
            //        //PanelVisible(true, "panel_PQ_Y");
            //        //PanelVisible(false, "panel_PQ_N");
            //    }
            //    else if (!status.msg[0].isPQ)
            //    {
            //        Global.IfPQStatus = false;
            //        cb_Setup.Enabled = false;
            //        cb_IQ.Enabled = false;
            //        cb_OQ.Enabled = false;
            //        cb_PQ.Enabled = false;

            //        rb_MQ.Enabled = true;
            //        rb_Production.Enabled = true;
            //    }
            //    else
            //    {
            //        Global.IfPQStatus = false;
            //        cb_Setup.Enabled = false;
            //        cb_IQ.Enabled = false;
            //        cb_OQ.Enabled = false;
            //        cb_PQ.Enabled = false;
            //        rb_MQ.Enabled = false;
            //        rb_Production.Enabled = false;
            //        MessageBox.Show("获取PQ状态异常");
            //    }
            //    Log.WriteLog("获取PQ状态接收:" + JsonConvert.SerializeObject(str_pq));
            //}
            //catch (Exception EX)
            //{
            //    Log.WriteLog("读取PQ状态异常:" + EX.ToString());
            //}

        }
        public void CheckUserLogin(bool IsLoginFail = false)
        {
            //if (Global.LoginGy==null)
            //{
            //    txtpwd.Text = "";
            //    labelIsLogin.Text =IsLoginFail==false?"未登录":"登录失败";
            //    labelIsLogin.ForeColor = Color.Red;
            //}
            //else
            //{
            //    txtuserName.Text = Global.LoginGy.EmployeeID;
            //    txtpwd.Text = Global.LoginGy.pwd;
            //    labelIsLogin.Text = "登录成功";
            //    labelIsLogin.ForeColor = Color.Green;
            //}
        }
        public void ComboBoxSelect(int i)//切换为技术员等级
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new cboSelect(ComboBoxSelect), new object[] { i });
                return;
            }
            cbo_Login.SelectedIndex = i;
            lbl_LoginLevel.Text = "技术员";
            lbl_LoginMes.BackColor = Color.Transparent;
            lbl_LoginMes.Text = "";
        }

        public void serial1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (Global._serial1.IsOpen)     //此处可能没有必要判断是否打开串口，但为了严谨性，我还是加上了
            {
                //输出当前时间
                Thread.Sleep(500);
                try
                {
                    Byte[] receivedData = new Byte[Global._serial1.BytesToRead];        //创建接收字节数组
                    Global._serial1.Read(receivedData, 0, receivedData.Length);         //读取数据
                                                                                        //string text = sp1.Read();   //Encoding.ASCII.GetString(receivedData);
                    Global._serial1.DiscardInBuffer();                                  //清空SerialPort控件的Buffer
                                                                                        //这是用以显示字符串
                    string strRcv = null;
                    for (int i = 0; i < receivedData.Length; i++)
                    {
                        strRcv += ((char)Convert.ToInt32(receivedData[i]));
                    }
                    strRcv = strRcv.Substring(1, 10);
                    if (Global.IfLoginbtn)
                    {
                        Invoke(new Action(() => txt_UserID.Text = strRcv));             //显示参数修改界面刷卡信息
                    }
                    Invoke(new Action(() => lb_CardNo.Text = strRcv));                  //显示上班登入界面刷卡信息
                    Invoke(new Action(() => lb_CardNo.ForeColor = Color.Black));
                    Global.IfReadUserID = true;
                    Global.UserLoginMouseMoveTime = DateTime.Now;
                    Thread.Sleep(50);
                    if (txt_UserID.Text.Length == 10 && !Global.IfUserLogin)
                    {
                        Invoke(new Action(() => lb_remind.Text = "刷卡成功"));
                        Invoke(new Action(() => lb_remind.ForeColor = Color.Green));
                        string CallResult = "";
                        string errMsg = "";
                        string URL = string.Format("http://10.128.10.7/Webapi/api/WorkTime/GetUserInfo?cardNo={0}", strRcv);
                        var result = RequestAPI2.CallBobcat(URL, "", out CallResult, out errMsg);
                        Log.WriteLog("刷卡获取用户信息：" + JsonConvert.SerializeObject(CallResult));
                        GetUserInfo RespondData = JsonConvert.DeserializeObject<GetUserInfo>(CallResult);
                        string[] Data = RespondData.msg.Replace("{", "").Replace("}", "").Split(',');
                        if (RespondData.status == true)
                        {
                            Global.IfUserLogin = true;
                            for (int i = 0; i < Data.Length; i++)
                            {
                                if (Data[i].Split(':')[0].Replace("\"", "") == "Emp")
                                {
                                    Global.Emp = Data[i].Split(':')[1].Replace("\"", "");
                                }
                                else if (Data[i].Split(':')[0].Replace("\"", "") == "Name")
                                {
                                    Global.Name = Data[i].Split(':')[1].Replace("\"", "");
                                }
                                else if (Data[i].Split(':')[0].Replace("\"", "") == "Title")
                                {
                                    Global.Title = Data[i].Split(':')[1].Replace("\"", "");
                                }
                                else if (Data[i].Split(':')[0].Replace("\"", "") == "Count")
                                {
                                    Global.Count = Data[i].Split(':')[1].Replace("\"", "");
                                    if (Convert.ToInt32(Global.Count) > 2)
                                    {
                                        //当Count登录不同机台次数>2台时，限制登录第三台
                                        string SelectStr = string.Format("select * from UserLogin where 登入时间 >= '{0}' and 用户名称 = '{1}'", DateTime.Now.Date.ToString(), Global.Name);
                                        DataTable d1 = SQL.ExecuteQuery(SelectStr);
                                        if (d1.Rows.Count == 0)
                                        {
                                            Invoke(new Action(() => list_UserLogin.Items.Add(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss ") + "登录人：" + Global.Name + ",权限：" + Global.Title + "超出每天限制刷卡机台数")));
                                            Global.IfUserLogin = false;
                                            Global.IfReadUserID = false;
                                            return;
                                        }
                                    }
                                }
                            }
                            Invoke(new Action(() => list_UserLogin.Items.Add(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss ") + "登录人：" + Global.Name + ",权限：" + Global.Title)));
                            Log.WriteLog("登录人：" + Global.Name + ",权限：" + Global.Title);
                            Global.UserLoginTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff");
                            //判断登录人的权限等级
                            if (Global.Title == "Operator")
                            {
                                Global.PLC_Client.WritePLC_D(20502, new short[] { 1 });//发送权限等级给PLC
                                Global.Login = Global.LoginLevel.Operator;// 切换追溯软件权限
                                Global.inidata.WriteProductnumSection();// 发送权限等级给CCD
                                if (Global.client1.Connected)
                                {
                                    Global.client1.Send(String.Format("USER;{0};{1};{2};\r\n", "LEV1", Global.Name, Global.Emp));// 发送权限等级给大族焊接机-1
                                }
                                if (Global.client2.Connected)
                                {
                                    Global.client2.Send(String.Format("USER;{0};{1};{2};\r\n", "LEV1", Global.Name, Global.Emp));// 发送权限等级给大族焊接机-2
                                }

                            }
                            else if (Global.Title == "Technician" || Global.Title == "Vendor_Technician")
                            {
                                Global.PLC_Client.WritePLC_D(20502, new short[] { 2 });
                                Global.Login = Global.LoginLevel.Technician;
                                Global.inidata.WriteProductnumSection();
                                if (Global.client1.Connected)
                                {
                                    Global.client1.Send(String.Format("USER;{0};{1};{2};\r\n", "LEV2", Global.Name, Global.Emp));// 发送权限等级给大族焊接机-1
                                }
                                if (Global.client2.Connected)
                                {
                                    Global.client2.Send(String.Format("USER;{0};{1};{2};\r\n", "LEV2", Global.Name, Global.Emp));// 发送权限等级给大族焊接机-2
                                }

                            }
                            else if (Global.Title == "Vendor_Engineer" || Global.Title == "Vendor_Administrator")
                            {
                                Global.PLC_Client.WritePLC_D(20502, new short[] { 3 });
                                Global.Login = Global.LoginLevel.Administrator;
                                Global.inidata.WriteProductnumSection();
                                if (Global.client1.Connected)
                                {
                                    Global.client1.Send(String.Format("USER;{0};{1};{2};\r\n", "LEV3", Global.Name, Global.Emp));// 发送权限等级给大族焊接机-1
                                }
                                if (Global.client2.Connected)
                                {
                                    Global.client2.Send(String.Format("USER;{0};{1};{2};\r\n", "LEV3", Global.Name, Global.Emp));// 发送权限等级给大族焊接机-2
                                }

                            }
                            else
                            {
                                Global.PLC_Client.WritePLC_D(20502, new short[] { 0 });
                                Global.Login = Global.LoginLevel.Technician;
                                Global.inidata.WriteProductnumSection();
                                if (Global.client1.Connected)
                                {
                                    Global.client1.Send(String.Format("USER;{0};{1};{2};\r\n", "LEV1", Global.Name, Global.Emp));// 发送权限等级给大族焊接机-1
                                }
                                if (Global.client2.Connected)
                                {
                                    Global.client2.Send(String.Format("USER;{0};{1};{2};\r\n", "LEV1", Global.Name, Global.Emp));// 发送权限等级给大族焊接机-2
                                }

                            }
                        }
                        else
                        {
                            Invoke(new Action(() => list_UserLogin.Items.Add(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss ") + JsonConvert.SerializeObject(CallResult))));
                            Log.WriteLog("获取用户信息错误！" + JsonConvert.SerializeObject(CallResult));
                        }
                    }
                    else
                    {
                        if (txt_UserID.Text.Length != 10)
                        {
                            Invoke(new Action(() => lb_remind.Text = "请先点击【登入】按钮！"));
                            Invoke(new Action(() => lb_remind.ForeColor = Color.Red));
                        }
                        if (Global.IfUserLogin)
                        {
                            Invoke(new Action(() => lb_remind.Text = "已登入！请先登出！"));
                            Invoke(new Action(() => lb_remind.ForeColor = Color.Red));
                        }
                    }
                    //}
                    //string strRcv = null;
                    //int decNum = 0;//存储十进制
                    //for (int i = 0; i < receivedData.Length; i++) //窗体显示
                    //{
                    //    strRcv += receivedData[i].ToString("X2");  //16进制显示
                    //}

                }
                catch (System.Exception ex)
                {
                    MessageBox.Show(ex.Message, "出错提示");
                }
            }
        }

        public void btn_UserLogin_Click(object sender, EventArgs e)
        {
            Global.UserLoginMouseMoveTime = DateTime.Now;
            if (Global.inidata.productconfig.OpenCard != "true")
            {
                MessageBox.Show("刷卡系统未启用,请联系相关人员");
                return;
            }
            if (!Global.IfLoginbtn)
            {
                Global.IfLoginbtn = true;
                Invoke(new Action(() => btn_UserLogin.Text = "登出"));
                Invoke(new Action(() => lb_remind.Text = "请刷卡"));
                Invoke(new Action(() => lb_remind.ForeColor = Color.Red));
            }
            else
            {
                if (Global.Login == Global.LoginLevel.Operator || Global.Title == "Operator")
                {
                    Invoke(new Action(() => lb_remind.Text = "无权限,请刷卡！"));
                    Invoke(new Action(() => lb_remind.ForeColor = Color.Red));
                    return;
                }
                Global.IfLoginbtn = false;
                Invoke(new Action(() => btn_UserLogin.Text = "登入"));
                Invoke(new Action(() => lb_remind.Text = ""));
                Invoke(new Action(() => txt_UserID.Text = ""));
                Invoke(new Action(() => list_UserLogin.Items.Add(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss ") + "登出人：" + Global.Name + ",权限：" + Global.Title)));
                string InsertStr = "insert into UserLogin([用户名称],[用户ID],[权限等级],[登入时间],[登出时间])" + " " + "values(" + "'" + Global.Name + "'" + "," + "'" + Global.Emp + "'" + "," + "'" + Global.Title + "'" + "," + "'" + Global.UserLoginTime + "'" + "," + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "'" + ")";
                SQL.ExecuteUpdate(InsertStr);
                Log.WriteLog("登出人：" + Global.Name + ",权限：" + Global.Title);
                Global.Emp = string.Empty;
                Global.Name = "操作员";
                Global.Title = "Operator";
                Global.IfUserLogin = false;//标志位 权限已登出
                Global.PLC_Client.WritePLC_D(20502, new short[] { 0 });//发送权限等级给PLC
                Global.Login = Global.LoginLevel.Operator;// 切换追溯软件权限
                Global.inidata.WriteProductnumSection();// 发送权限等级给CCD
                if (Global.client1.Connected || Global.client2.Connected)
                {
                    if (Global.client1.Connected)
                    {
                        Global.client1.Send(String.Format("USER;{0};{1};{2};\r\n", "LEV1", "", ""));// 发送权限等级给大族焊接机-1
                    }
                    if (Global.client2.Connected)
                    {
                        Global.client2.Send(String.Format("USER;{0};{1};{2};\r\n", "LEV1", "", ""));// 发送权限等级给大族焊接机-2
                    }

                }
            }
        }

        private void btn_UpLoadLogin_Click(object sender, EventArgs e)
        {
            if ((!cb_Setup.Checked && !cb_IQ.Checked && !cb_OQ.Checked && !cb_PQ.Checked) && (!rb_MQ.Checked && !rb_Production.Checked))
            {
                Invoke(new Action(() => lb_CardNo.Text = "请选择LQ阶段！"));
                Invoke(new Action(() => lb_CardNo.ForeColor = Color.Red));
                return;
            }
            if (cb_UserClass.SelectedIndex != 0 && cb_UserClass.SelectedIndex != 1)
            {
                Invoke(new Action(() => lb_CardNo.Text = "请选择白夜班次！"));
                Invoke(new Action(() => lb_CardNo.ForeColor = Color.Red));
                return;
            }
            if (lb_CardNo.Text.Length == 10)
            {
                //上传打卡数据
                JsonSerializerSettings jsetting = new JsonSerializerSettings();
                jsetting.NullValueHandling = NullValueHandling.Ignore;//Json不输出空值
                IDCard CardData = new IDCard();
                CardData.cardNo = lb_CardNo.Text;                   //用户卡号
                CardData.dsn = Global.inidata.productconfig.OEE_Dsn;//机台dsn
                CardData.shift = 1;                                 //班别代号，从接口二获取的id
                CardData.machineType = 1;                           //打卡机类型。1：固定式刷卡机。0：刷卡式
                CardData.swipeType = 0;                             //0:上班，1：下班
                CardData.stage = new string[] { };                  //阶段，类型为数组 Setup，IQ，OQ，PQ，MQ，Productio
                List<string> _list = new List<string>();
                if (panel_PQ_N.Visible)
                {
                    if (cb_Setup.Checked)
                    {
                        _list.Add("Setup");
                    }
                    if (cb_IQ.Checked)
                    {
                        _list.Add("IQ");
                    }
                    if (cb_OQ.Checked)
                    {
                        _list.Add("OQ");
                    }
                    if (cb_PQ.Checked)
                    {
                        _list.Add("PQ");
                    }
                }
                if (panel_PQ_Y.Visible)
                {
                    if (rb_MQ.Checked)
                    {
                        _list.Add("MQ");
                    }
                    if (rb_Production.Checked)
                    {
                        _list.Add("Production");
                    }
                }
                CardData.stage = _list.ToArray();
                string SendCardData = JsonConvert.SerializeObject(CardData, Formatting.None, jsetting);
                string Msg_ua;
                string Trace_str_ua = "";
                list_UploadLogin.Items.Add(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss ") + SendCardData + "\r\n");
                Log.WriteLog("上传打卡数据:" + SendCardData);
                RequestAPI2.CallBobcat2("http://10.128.10.7/Webapi/api/WorkTime/UpdateData", SendCardData, out Trace_str_ua, out Msg_ua, false);
                list_UploadLogin.Items.Add(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss ") + JsonConvert.SerializeObject(Trace_str_ua) + "\r\n");
                Log.WriteLog("打卡数据接收:" + JsonConvert.SerializeObject(Trace_str_ua));
                Invoke(new Action(() => lb_CardNo.Text = ""));
            }
            else
            {
                Invoke(new Action(() => lb_CardNo.Text = "请刷卡！"));
                Invoke(new Action(() => lb_CardNo.ForeColor = Color.Red));
            }
        }

        public List<Control> GetAllControls(Control control)
        {
            var list = new List<Control>();
            foreach (Control con in control.Controls)
            {
                list.Add(con);
                if (con.Controls.Count > 0)
                {
                    list.AddRange(GetAllControls(con));
                }
            }
            return list;
        }

        private void UserLoginFrm_MouseEnter(object sender, EventArgs e)
        {
            Global.UserLoginMouseMoveTime = DateTime.Now;
        }

        //private void button1_Click(object sender, EventArgs e)
        //{
        // string a = MD5Helper.EncryptMD5(txtpwd.Text);
        // string b = MD5Helper.GetBase64Encode(txtpwd.Text);
        // Gy LoginGy = new Gy() { EmployeeID = txtuserName.Text, pwd = MD5Helper.EncryptMD5(txtpwd.Text), Equipment = Global.inidata.productconfig.Station_id_ua };
        // string msg = JsonConvert.SerializeObject(LoginGy);
        // string callResult,errMsg, callResult2, errMsg2;

        //bool result= RequestAPI2.CallBobcat2("http://10.128.10.7/webapi/api/NPI/login", msg, out callResult, out errMsg, false);
        // try
        // {
        //     //var c = JsonConvert.DeserializeObject<GyApiReturn>(callResult);
        //     if (JsonConvert.DeserializeObject<GyApiReturn>(callResult)?.status == "True" || JsonConvert.DeserializeObject<GyApiReturn>(callResult)?.status == "true")
        //     {
        //         Global.LoginGy = LoginGy;
        //         CheckUserLogin();
        //         bool result2 = RequestAPI2.CallBobcat($"http://10.128.10.7/webapi/api/NPI/GetRoutestepProperties?employeeno={Global.LoginGy.EmployeeID}&machineID=JACD_B02-2F-OQC01_84001_STATION1721", "", out callResult2, out errMsg2);
        //         Global.materialRule = JsonConvert.DeserializeObject<MaterialRule>(callResult2);
        //         GetData(Global.materialRule);
        //     }
        //     else
        //     {
        //         Global.LoginGy = null;
        //         CheckUserLogin(true);
        //     }
        // }
        // catch (Exception ex)
        // {

        //     Global.LoginGy = null;
        //     CheckUserLogin(true);
    }

    //}

    //public void GetData(MaterialRule materialRule)
    //{

    //    for (int i = 0; i < materialRule.msg.materialCheckRules.Count(); i++)
    //    {
    //        switch (i)
    //        {
    //            case 0:
    //                searchTextBox1.searchModel.Index = 1;
    //                searchTextBox1.searchModel.LinkObj = materialRule.msg.materialCheckRules[i].linkObj;
    //                searchTextBox1.searchModel.Start = materialRule.msg.materialCheckRules[i].begin;
    //                searchTextBox1.searchModel.End = materialRule.msg.materialCheckRules[i].end;
    //                searchTextBox1.searchModel.LeiBie = materialRule.msg.materialCheckRules[i].type;
    //                searchTextBox1.searchModel.Value = new List<string>(materialRule.msg.materialCheckRules[i].value);
    //                searchTextBox1.Clear();
    //                foreach (var item in searchTextBox1.searchModel.Value)
    //                {
    //                    searchTextBox1.model.AddItem(item);
    //                }
    //                break;
    //            case 1:
    //                searchTextBox2.searchModel.Index = 2;
    //                searchTextBox2.searchModel.LinkObj = materialRule.msg.materialCheckRules[i].linkObj;
    //                searchTextBox2.searchModel.Start = materialRule.msg.materialCheckRules[i].begin;
    //                searchTextBox2.searchModel.End = materialRule.msg.materialCheckRules[i].end;
    //                searchTextBox2.searchModel.LeiBie = materialRule.msg.materialCheckRules[i].type;
    //                searchTextBox2.searchModel.Value = new List<string>(materialRule.msg.materialCheckRules[i].value);
    //                searchTextBox2.Clear();
    //                foreach (var item in searchTextBox2.searchModel.Value)
    //                {
    //                    searchTextBox2.model.AddItem(item);
    //                }
    //                break;
    //            case 2:
    //                searchTextBox3.searchModel.Index = 3;
    //                searchTextBox3.searchModel.LinkObj = materialRule.msg.materialCheckRules[i].linkObj;
    //                searchTextBox3.searchModel.Start = materialRule.msg.materialCheckRules[i].begin;
    //                searchTextBox3.searchModel.End = materialRule.msg.materialCheckRules[i].end;
    //                searchTextBox3.searchModel.LeiBie = materialRule.msg.materialCheckRules[i].type;
    //                searchTextBox3.searchModel.Value = new List<string>(materialRule.msg.materialCheckRules[i].value);
    //                searchTextBox3.Clear();
    //                foreach (var item in searchTextBox3.searchModel.Value)
    //                {
    //                    searchTextBox3.model.AddItem(item);
    //                }
    //                break;
    //            case 3:
    //                searchTextBox4.searchModel.Index = 4;
    //                searchTextBox4.searchModel.LinkObj = materialRule.msg.materialCheckRules[i].linkObj;
    //                searchTextBox4.searchModel.Start = materialRule.msg.materialCheckRules[i].begin;
    //                searchTextBox4.searchModel.End = materialRule.msg.materialCheckRules[i].end;
    //                searchTextBox4.searchModel.LeiBie = materialRule.msg.materialCheckRules[i].type;
    //                searchTextBox4.searchModel.Value = new List<string>(materialRule.msg.materialCheckRules[i].value);
    //                searchTextBox4.Clear();
    //                foreach (var item in searchTextBox4.searchModel.Value)
    //                {
    //                    searchTextBox4.model.AddItem(item);
    //                }
    //                break;
    //            case 4:
    //                searchTextBox5.searchModel.Index = 5;
    //                searchTextBox5.searchModel.LinkObj = materialRule.msg.materialCheckRules[i].linkObj;
    //                searchTextBox5.searchModel.Start = materialRule.msg.materialCheckRules[i].begin;
    //                searchTextBox5.searchModel.End = materialRule.msg.materialCheckRules[i].end;
    //                searchTextBox5.searchModel.LeiBie = materialRule.msg.materialCheckRules[i].type;
    //                searchTextBox5.searchModel.Value = new List<string>(materialRule.msg.materialCheckRules[i].value);
    //                searchTextBox5.Clear();
    //                foreach (var item in searchTextBox5.searchModel.Value)
    //                {
    //                    searchTextBox5.model.AddItem(item);
    //                }
    //                break;
    //            default:
    //                break;
    //        }
    //    }
    //}
}

