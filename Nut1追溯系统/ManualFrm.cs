using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using uPLibrary.Networking.M2Mqtt.Messages;
using static System.Net.Mime.MediaTypeNames;

namespace 卓汇数据追溯系统
{
    public partial class ManualFrm : Form
    {
        private MainFrm _mainparent;        
        private delegate void buttonflag(bool flag, int index);
        private delegate void Labelcolor(Color color, string bl, string Name);
        private delegate void Labelvision(string bl, string Name);
        List<Control> List_Control = new List<Control>();
        SQLServer SQL = new SQLServer();
        delegate void RefreachTable(Chart chart, string[] Point_X, double[] Point_Y, int index);
        string[] HourDT_X = new string[] { "待料", "生产", "宕机", "机台/治具保养", "调试", "关机", "吃饭休息", "更换耗材", "首件" };
        double[] HourDT_Y = new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        public ManualFrm(MainFrm mdiParent)
        {
            InitializeComponent();
            _mainparent = mdiParent;
        }
        public void dataTableToListView_fixtrue(ListView lv, DataTable dt)
        {
            if (dt != null)
            {
                lv.View = View.Details;
                lv.GridLines = true;//显示网格线
                lv.Items.Clear();//所有的项
                lv.Columns.Clear();//标题
                //对表格重新排序赋值
                DataTable dt2 = new DataTable();

                //dt2.Columns[0].ColumnName = "序号";
                //dt2.Columns[1].ColumnName = "时间段";
                //dt2.Columns[2].ColumnName = "治具SN不相同的数量";
                //dt2.Columns[3].ColumnName = "治具SN重复次数>设定值的数量";
                DataColumn dataColumn = new DataColumn("序号");
                DataColumn dataColumn1 = new DataColumn("时间段");
                DataColumn dataColumn2 = new DataColumn("治具SN不相同的数量");
                DataColumn dataColumn3 = new DataColumn("治具SN重复次数>设定值的数量");

                dt2.Columns.Add(dataColumn);
                dt2.Columns.Add(dataColumn1);
                dt2.Columns.Add(dataColumn2);
                dt2.Columns.Add(dataColumn3);

                for (int i = 0; i < dt2.Columns.Count; i++)
                {
                    lv.Columns.Add(dt2.Columns[i].Caption.ToString());//增加标题
                }
                //数据库 时间 24
                string[] DateTime_fixtrue = { "08:00-09:00", "09:00-10:00", "10:00-11:00", "11:00-12:00", "12:00-13:00", "13:00-14:00", "14:00-15:00", "15:00-16:00", "16:00-17:00", "17:00-18:00", "18:00-19:00", "19:00-20:00", "20:00-21:00", "21:00-22:00", "22:00-23:00", "23:00-24:00", "00:00-01:00", "01:00-02:00", "02:00-03:00", "03:00-04:00", "04:00-05:00", "05:00-06:00", "06:00-07:00", "07:00-08:00" };
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    // lv.Items.Add((i + 1).ToString(), DateTime_fixtrue[i], dt.Columns[i + 1].ToString());
                    dt2.Rows.Add(i + 1, DateTime_fixtrue[i], dt.Columns[i + 1].ToString(), dt.Columns[i+24].ToString());
                }

                for (int i = 0; i < dt2.Rows.Count; i++)
                {
                    ListViewItem lvi = new ListViewItem(dt2.Rows[i][0].ToString());
                    for (int j = 1; j < dt2.Columns.Count; j++)
                    {
                        // lvi.ImageIndex = 0;
                        lvi.SubItems.Add(dt2.Rows[i][j].ToString());
                    }
                    lv.Items.Add(lvi);
                }
                lv.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);//调整列的宽度
            }
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
            e.Graphics.DrawString(text, new Font("微软雅黑", 12), brush, e.Bounds, sf);

        }

        private void btn_Output_Click(object sender, EventArgs e)
        {
            try
            {
                SaveDataToCSVFile(lv_OEEData);
            }
            catch (Exception ex)
            {
                Log.WriteLog(ex.ToString());
                MessageBox.Show(ex.ToString());
            }
        }

        private void CB_errorinfo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CB_errorinfo.SelectedItem.ToString().Contains("手动选择故障代码"))
            {
                LB_ErrorCode.Text = "[无,无]";
            }
            else
            {
                if (CB_errorinfo.SelectedItem.ToString().Contains("人工停止复位(M)"))
                {
                    LB_ErrorCode.Text = "[70020006,7]";
                }
                if (CB_errorinfo.SelectedItem.ToString().Contains("设备保养(M)"))
                {
                    LB_ErrorCode.Text = "[30010001,4]";
                }
                if (CB_errorinfo.SelectedItem.ToString().Contains("治具保养(M)"))
                {
                    LB_ErrorCode.Text = "[30010002,4]";
                }
                if (CB_errorinfo.SelectedItem.ToString().Contains("更换零配件(M)"))
                {
                    LB_ErrorCode.Text = "[30010003,8]";
                }
                if (CB_errorinfo.SelectedItem.ToString().Contains("镭焊机参数调整(M)"))
                {
                    LB_ErrorCode.Text = "[70010001,5]";
                }
                if (CB_errorinfo.SelectedItem.ToString().Contains("其他原因工艺参数调整(M)"))
                {
                    LB_ErrorCode.Text = "[70020002,5]";
                }
                if (CB_errorinfo.SelectedItem.ToString().Contains("其他原因设备调试(M)"))
                {
                    LB_ErrorCode.Text = "[70020003,5]";
                }
                if (CB_errorinfo.SelectedItem.ToString().Contains("点位调试(M)"))
                {
                    LB_ErrorCode.Text = "[70020004,5]";
                }
                if (CB_errorinfo.SelectedItem.ToString().Contains("机械手点位调试(M)"))
                {
                    LB_ErrorCode.Text = "[70020008,5]";
                }
                if (CB_errorinfo.SelectedItem.ToString().Contains("CCD视觉调试(M)"))
                {
                    LB_ErrorCode.Text = "[70020005,5]";
                }
                if (CB_errorinfo.SelectedItem.ToString().Contains("设备软件调试(M)"))
                {
                    LB_ErrorCode.Text = "[70020007,5]";
                }
                if (CB_errorinfo.SelectedItem.ToString().Contains("更换小料(M)"))
                {
                    LB_ErrorCode.Text = "[80010001,8]";
                }
                if (CB_errorinfo.SelectedItem.ToString().Contains("更换吸嘴(M)"))
                {
                    LB_ErrorCode.Text = "[80010002,8]";
                }
                Global.errorcode = LB_ErrorCode.Text.Split(',')[0].Replace("[", "");
                Global.errorStatus = LB_ErrorCode.Text.Split(',')[1].Replace("]", "");
                Global.errorinfo = CB_errorinfo.SelectedItem.ToString();
            }
        }

        private void CB_PendingReason_SelectedIndexChanged(object sender, EventArgs e)
        {
            //if (CB_PendingReason.SelectedItem.ToString().Contains("其他原因待料"))
            //{
            //    CB_PendingCode.Text = "[12010001,1]";
            //}
            //if (CB_PendingReason.SelectedItem.ToString().Contains("HSG待料"))
            //{
            //    CB_PendingCode.Text = "[12010002,1]";
            //}
            //if (CB_PendingReason.SelectedItem.ToString().Contains("NUT待料"))
            //{
            //    CB_PendingCode.Text = "[12010003,1]";
            //}
            //if (CB_PendingReason.SelectedItem.ToString().Contains("NUT和HSG待料"))
            //{
            //    CB_PendingCode.Text = "[12010004,1]";
            //}
            //Global.labelerror = CB_PendingCode.Text.Split(',')[0].Replace("[", "");
            //Global.labelStatus = CB_PendingCode.Text.Split(',')[1].Replace("]", "");
            //Global.errorselect = CB_PendingReason.SelectedItem.ToString();
        }

        private void ManualFrm_Load(object sender, EventArgs e)
        {
            CB_errorinfo.SelectedIndex = 0;
            //CB_PendingReason.SelectedIndex = 0;
            List_Control = GetAllControls(this);//列表中添加所有窗体控件
            #region 饼图总DT
            //标题
            chart_TotalDT.Titles.Add("Total DT");
            chart_TotalDT.Titles[0].ForeColor = Color.Green;
            chart_TotalDT.Titles[0].Font = new Font("Calibri", 12f, FontStyle.Bold);
            chart_TotalDT.Titles[0].Alignment = ContentAlignment.TopCenter;
            //chart_TotalDT.Titles.Add("合计：25412 宗");
            //chart_TotalDT.Titles[1].ForeColor = Color.White;
            //chart_TotalDT.Titles[1].Font = new Font("微软雅黑", 8f, FontStyle.Regular);
            //chart_TotalDT.Titles[1].Alignment = ContentAlignment.TopRight;
            //控件背景
            chart_TotalDT.BackColor = Color.Transparent;
            //图表区背景
            chart_TotalDT.ChartAreas[0].BackColor = Color.Transparent;
            chart_TotalDT.ChartAreas[0].BorderColor = Color.Transparent;
            //X轴标签间距
            //chart_TotalDT.ChartAreas[0].AxisX.Interval = 1;
            //chart_TotalDT.ChartAreas[0].AxisX.LabelStyle.IsStaggered = true;
            //chart_TotalDT.ChartAreas[0].AxisX.LabelStyle.Angle = -45;
            //chart_TotalDT.ChartAreas[0].AxisX.TitleFont = new Font("Calibri", 14f, FontStyle.Regular);
            //chart_TotalDT.ChartAreas[0].AxisX.TitleForeColor = Color.Black;
            //X坐标轴颜色
            //chart_TotalDT.ChartAreas[0].AxisX.LineColor = ColorTranslator.FromHtml("#38587a"); ;
            //chart_TotalDT.ChartAreas[0].AxisX.LabelStyle.ForeColor = Color.Black;
            //chart_TotalDT.ChartAreas[0].AxisX.LabelStyle.Font = new Font("Calibri", 10f, FontStyle.Regular);
            //X坐标轴标题
            //chart_TotalDT.ChartAreas[0].AxisX.Title = "数量(宗)";
            //chart_TotalDT.ChartAreas[0].AxisX.TitleFont = new Font("Calibri", 10f, FontStyle.Regular);
            //chart_TotalDT.ChartAreas[0].AxisX.TitleForeColor = Color.Black;
            //chart_TotalDT.ChartAreas[0].AxisX.TextOrientation = TextOrientation.Horizontal;
            //chart_TotalDT.ChartAreas[0].AxisX.ToolTip = "数量(宗)";
            //X轴网络线条
            //chart_TotalDT.ChartAreas[0].AxisX.MajorGrid.Enabled = true;
            //chart_TotalDT.ChartAreas[0].AxisX.MajorGrid.LineColor = ColorTranslator.FromHtml("#2c4c6d");
            //Y坐标轴颜色
            //chart_TotalDT.ChartAreas[0].AxisY.LineColor = ColorTranslator.FromHtml("#38587a");
            //chart_TotalDT.ChartAreas[0].AxisY.LabelStyle.ForeColor = Color.Black;
            //chart_TotalDT.ChartAreas[0].AxisY.LabelStyle.Font = new Font("Calibri", 10f, FontStyle.Regular);
            //Y坐标轴标题
            //chart_TotalDT.ChartAreas[0].AxisY.Title = "数量(宗)";
            //chart_TotalDT.ChartAreas[0].AxisY.TitleFont = new Font("Calibri", 10f, FontStyle.Regular);
            //chart_TotalDT.ChartAreas[0].AxisY.TitleForeColor = Color.Black;
            //chart_TotalDT.ChartAreas[0].AxisY.TextOrientation = TextOrientation.Rotated270;
            //chart_TotalDT.ChartAreas[0].AxisY.ToolTip = "数量(宗)";
            //Y轴网格线条
            //chart_TotalDT.ChartAreas[0].AxisY.MajorGrid.Enabled = true;
            //chart_TotalDT.ChartAreas[0].AxisY.MajorGrid.LineColor = ColorTranslator.FromHtml("#2c4c6d");
            //chart_TotalDT.ChartAreas[0].AxisY2.LineColor = Color.Transparent;
            //背景渐变
            chart_TotalDT.ChartAreas[0].BackGradientStyle = GradientStyle.None;
            //图例样式
            Legend legend3 = new Legend("#VALX #PERCENT{P2} #VAL{N2}");
            legend3.Title = "图例";
            legend3.TitleBackColor = Color.Transparent;
            legend3.BackColor = Color.Transparent;
            legend3.TitleForeColor = Color.Black;
            legend3.TitleFont = new Font("Calibri", 10f, FontStyle.Regular);
            legend3.Font = new Font("Calibri", 8f, FontStyle.Regular);
            legend3.ForeColor = Color.Black;
            chart_TotalDT.Series[0].XValueType = ChartValueType.String;  //设置X轴上的值类型
            chart_TotalDT.Series[0].Label = "#VALX";                //设置显示X Y的值    
            chart_TotalDT.Series[0].LabelForeColor = Color.Black;
            chart_TotalDT.Series[0].ToolTip = "#VALX:#VAL";     //鼠标移动到对应点显示数值
            chart_TotalDT.Series[0].ChartType = SeriesChartType.Pie;    //图类型(折线)
            chart_TotalDT.Series[0].Color = Color.Lime;
            chart_TotalDT.Series[0].LegendText = legend3.Name;
            chart_TotalDT.Series[0].IsValueShownAsLabel = true;
            chart_TotalDT.Series[0].LabelForeColor = Color.Black;
            //chart_TotalDT.Series[0].CustomProperties = "DrawingStyle = Cylinder";
            //chart_TotalDT.Series[0].CustomProperties = "PieLabelStyle = Outside";
            chart_TotalDT.Legends.Add(legend3);
            chart_TotalDT.Legends[0].Position.Auto = true;
            chart_TotalDT.Series[0].IsValueShownAsLabel = true;
            //是否显示图例
            chart_TotalDT.Series[0].IsVisibleInLegend = true;
            chart_TotalDT.Series[0].ShadowOffset = 0;
            //饼图折线
            chart_TotalDT.Series[0]["PieLineColor"] = "Black";
            //绑定数据
            //chart_TotalDT.Series[0].Points.DataBindXY(x1, y1);
            //绑定颜色
            chart_TotalDT.Series[0].Palette = ChartColorPalette.BrightPastel;

            #endregion
        }

        public void DT_Pie(string StartTime, string EndTime)
        {
            int[] Runstatus = new int[9];
            double[] TimeSpan = new double[9];
            double[] Baifenbi = new double[9];
            try
            {
                string[] HourDT_X = new string[] { "待料", "生产", "宕机", "机台/治具保养", "调试", "关机", "吃饭休息", "更换耗材", "首件" };
                double[] HourDT_Y = new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                double TotalTime = 0;
                string SelectStr = "SELECT sum(cast(分钟 as float)) ,运行状态 FROM Select_OEEDT " +
               string.Format("where cast(开始时间 as datetime) between '{0}' and '{1}' ", StartTime, EndTime) +
               "group by 运行状态 order by 运行状态 ";
                DataTable d1 = SQL.ExecuteQuery(SelectStr);
                if (d1.Rows.Count > 0)
                {
                    for (int i = 0; i < d1.Rows.Count; i++)
                    {
                        TimeSpan[i] = Convert.ToDouble(d1.Rows[i][0].ToString());
                        Runstatus[i] = Convert.ToInt32(d1.Rows[i][1].ToString());
                        TotalTime += Convert.ToDouble(d1.Rows[i][0].ToString());
                        HourDT_Y[Runstatus[i] - 1] = TimeSpan[i];
                    }
                    //for (int i = 0; i < d1.Rows.Count; i++)
                    //{
                    //    Baifenbi[i] = (TimeSpan[i] / TotalTime) * 100;
                    //    HourDT_Y[Runstatus[i] - 1] = Baifenbi[i];
                    //}
                }
                RefreachData(chart_TotalDT, HourDT_X, HourDT_Y, 0);
            }
            catch (Exception ex)
            {
                Log.WriteLog(ex.ToString().Replace("\r\n", ""));
            }
        }

        private void btn_errortime_Click(object sender, EventArgs e)
        {
            //try
            //{
            //    if (Global.j != 3)
            //    {
            //        Global.ed[Global.j].stop_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            //        DateTime t1 = Convert.ToDateTime(Global.ed[Global.j].start_time);
            //        DateTime t2 = Convert.ToDateTime(Global.ed[Global.j].stop_time);
            //        string ts = (t2 - t1).TotalMinutes.ToString("0.00");
            //        Log.WriteCSV(DateTime.Now.ToString("HH:mm:ss") + "," + Global.ed[Global.j].errorCode + "," + Global.ed[Global.j].start_time + "," + Global.ed[Global.j].start_time + "," + "自动发送成功" + "," + Global.ed[Global.j].errorStatus + "," + Global.ed[Global.j].errorinfo + "," + ts, System.AppDomain.CurrentDomain.BaseDirectory + "\\系统配置\\System_ini\\");
            //    }
            //    else
            //    {
            //        Global.ed[Global.Error_num - 1].stop_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            //        DateTime t1 = Convert.ToDateTime(Global.ed[Global.Error_num - 1].start_time);
            //        DateTime t2 = Convert.ToDateTime(Global.ed[Global.Error_num - 1].stop_time);
            //        string ts = (t2 - t1).TotalMinutes.ToString("0.00");
            //        Log.WriteCSV(DateTime.Now.ToString("HH:mm:ss") + "," + Global.ed[Global.Error_num - 1].errorCode + "," + Global.ed[Global.Error_num - 1].start_time + "," + Global.ed[Global.Error_num - 1].start_time + "," + "自动发送成功" + "," + Global.ed[Global.Error_num - 1].errorStatus + "," + Global.ed[Global.Error_num - 1].errorinfo + "," + ts, System.AppDomain.CurrentDomain.BaseDirectory + "\\系统配置\\System_ini\\");
            //    }
            //    string OEE_DT = "";
            //    string msg = "";
            //    string EventTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");

            //    var IP = _mainparent.GetIp();
            //    var Mac = _mainparent.GetMac();
            //    OEE_DT = string.Format("{{\"Status\":\"{0}\",\"ErrorCode\":\"{1}\",\"EventTime\":\"{2}\",\"CreateTime\":\"{3}\",\"Isreckon\":\"{4}\",\"Machineno\":\"{5}\"}}", "1", "21050004", EventTime, EventTime, "Y", "Alaska_Nut_001");
            //    Log.WriteLog("OEE_DT手动:" + OEE_DT);
            //    var rst = RequestAPI.Request(Global.inidata.productconfig.OEE_URL1, Global.inidata.productconfig.OEE_URL2, IP, Mac, Global.inidata.productconfig.OEE_Dsn, Global.inidata.productconfig.OEE_authCode, 2, OEE_DT, out msg);
            //    if (rst)
            //    {
            //        _mainparent.AppendText(lstSelectSN, "       " + "21050004" + "    触发时间=" + EventTime + "  接受时间=" + EventTime + "    运行状态:" + "1" + "    故障描述:" + "机台做验证做首件" + "     手动发送成功", 1);
            //        //Log.WriteCSV(DateTime.Now.ToString("HH:mm:ss") + "," + "21050004" + "," + EventTime + "," + EventTime + "," + "手动发送成功" + "," + "1" + "," + "机台做验证做首件", System.AppDomain.CurrentDomain.BaseDirectory + "\\系统配置\\System_ini\\");
            //        Log.WriteLog("OEE_DT手动errorCode发送成功");
            //        Global.ConnectOEEFlag = true;
            //    }
            //    else
            //    {
            //        _mainparent.AppendText(lstSelectSN, "       " + "21050004" + "    触发时间=" + EventTime + "  接受时间=" + EventTime + "    运行状态:" + "1" + "    故障描述:" + "机台做验证做首件" + "     手动发送失败", 1);
            //        //Log.WriteCSV(DateTime.Now.ToString("HH:mm:ss") + "," + "21050004" + "," + EventTime + "," + EventTime + "," + "手动发送失败" + "," + "1" + "," + "机台做验证做首件", System.AppDomain.CurrentDomain.BaseDirectory + "\\系统配置\\System_ini\\");
            //        Log.WriteLog("OEE_DT手动errorCode发送失败");
            //        Global.ConnectOEEFlag = false;
            //        //Access.InsertData_OEE_DT("PDCA", "OEE_DownTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "OEE_DT", "1", "21050004", EventTime, EventTime, "Y", "Alaska_Nut_001", "", "机台做验证做首件");
            //    }
            //    Global.PLC_Client.WritePLC_D(23021, new short[] { 1 });//通知PLC开始做首件做验证
            //}
            //catch (Exception ex)
            //{
            //    Log.WriteLog(ex.ToString());
            //}
            //Global.errorTime1 = true;
            //Global.errorStartTime = DateTime.Now;
            //Global.errordata.errorStatus = "1";
            //Global.errordata.errorCode = "21050004";
            //Global.errordata.errorinfo = "机台做验证做首件";
            //_mainparent.UiText(Global.errorStartTime.ToString("yyyy-MM-dd HH:mm:ss"), txt_errortime_display);
            // errortime_display_TB.Text = errorStartTime.ToString("yyyy-MM-dd HH:mm:ss");
            //_mainparent.Btn_IfEnable(true, label52);
        }

        public void ButtonFlag(bool Flag, int index)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new buttonflag(ButtonFlag), new object[] { Flag, index });
                return;
            }
            btnManualOEEStatus.Enabled = Flag;
        }

        public void labelcolor(Color color, string str, string Name)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Labelcolor(labelcolor), new object[] { color, str, Name });
                return;
            }
            foreach (Control ctrl in List_Control)
            {
                if (ctrl.GetType() == typeof(Label))
                {
                    if (ctrl.Name == Name)
                    {
                        ctrl.BackColor = color;
                        ctrl.Text = str;
                    }
                }
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

        public void dataTableToListView(ListView lv, DataTable dt, string startTime, string EndTime)
        {
            if (dt != null)
            {
                lv.View = View.Details;
                lv.GridLines = true;//显示网格线
                lv.Items.Clear();//所有的项
                lv.Columns.Clear();//标题
                //对表格重新排序赋值
                dt.Columns["EventTime"].SetOrdinal(1);
                dt.Columns["DateTime"].SetOrdinal(2);
                dt.Columns["TimeSpan"].SetOrdinal(3);
                dt.Columns["EventTime"].ColumnName = "开始时间";
                dt.Columns["DateTime"].ColumnName = "结束时间";
                dt.Columns["ErrorCode"].ColumnName = "故障代码";
                dt.Columns["ModuleCode"].ColumnName = "模组代码";
                dt.Columns["RunStatus"].ColumnName = "运行状态";
                dt.Columns["ErrorInfo"].ColumnName = "故障信息";
                dt.Columns["TimeSpan"].ColumnName = "分钟";
                dt.Rows[0][1] = startTime;          //把机台实际状态改变的时间替换为查找开始时间
                dt.Rows[0][3] = (Convert.ToDateTime(dt.Rows[0][2].ToString()) - Convert.ToDateTime(dt.Rows[0][1].ToString())).TotalMinutes.ToString("0.00");//计算状态发生的时长

                if (Convert.ToDateTime(EndTime) <= Convert.ToDateTime(dt.Rows[dt.Rows.Count - 1][2].ToString()))//判断结束时间是否大于database中的结束时间
                {
                    dt.Rows[dt.Rows.Count - 1][2] = EndTime;//把机台实际状态结束的时间替换为查找结束时间
                    dt.Rows[dt.Rows.Count - 1][3] = (Convert.ToDateTime(dt.Rows[dt.Rows.Count - 1][2].ToString()) - Convert.ToDateTime(dt.Rows[dt.Rows.Count - 1][1].ToString())).TotalMinutes.ToString("0.00");//计算状态发生的时长
                }
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    lv.Columns.Add(dt.Columns[i].Caption.ToString());//增加标题
                }
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    ListViewItem lvi = new ListViewItem(dt.Rows[i][0].ToString());
                    for (int j = 1; j < dt.Columns.Count; j++)
                    {
                        // lvi.ImageIndex = 0;
                        lvi.SubItems.Add(dt.Rows[i][j].ToString());
                    }
                    lv.Items.Add(lvi);
                }
                lv.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);//调整列的宽度

                string deleteStr = "delete from Select_OEEDT";
                SQL.ExecuteUpdate(deleteStr);
                foreach (DataRow datarow in dt.Rows)
                {
                    string InsertOEEStr = "insert into Select_OEEDT([开始时间],[结束时间],[故障代码],[模组代码],[运行状态],[故障信息],[分钟])" +
                    "VALUES('" + datarow["开始时间"].ToString() + "'" +
                    ",'" + datarow["结束时间"].ToString() + "'" +
                    ",'" + datarow["故障代码"].ToString() + "'" +
                    ",'" + datarow["模组代码"].ToString() + "'" +
                    ",'" + datarow["运行状态"].ToString() + "'" +
                    ",'" + datarow["故障信息"].ToString() + "'" +
                    ",'" + datarow["分钟"].ToString() + "')";
                    SQL.ExecuteUpdate(InsertOEEStr);
                }
                DT_Pie(dtp_Start.Text, dtp_End.Text);
                Log.WriteLog("插入Select_OEEDT表格OEEDT查询记录");
            }
        }

        private void SendOEEStatus()
        {
          
            if (CB_errorinfo.SelectedItem.ToString().Contains("手动选择故障代码"))
            {
                MessageBox.Show("故障信息不能选择为‘无’,请选择正确的故障信息！");
            }
            else
            {
                CB_errorinfo.SelectedIndex = 0;
                UpDatalabel("选择成功", "LB_ManualSelect");
                Global.SelectManualErrorCode = true;
                Global.PLC_Client.WritePLC_D(10020, new short[] { 1 });//手动选择打开安全门原因，机台可以运行
                //ButtonFlag(false, 0);
            }
        }
        private void btnManualOEEStatus_Click(object sender, EventArgs e)
        {
            //Thread th = new Thread(SendOEEStatus);
            //th.IsBackground = true;
            //th.Start();
            if (CB_errorinfo.SelectedItem.ToString().Contains("手动选择故障代码"))
            {
                MessageBox.Show("故障信息不能选择为‘无’,请选择正确的故障信息！");
            }
            else
            {
                CB_errorinfo.SelectedIndex = 0;
                UpDatalabel("选择成功", "LB_ManualSelect");
                Global.SelectManualErrorCode = true;
                Global.PLC_Client2.WritePLC_D(10020, new short[] { 1 });//手动选择打开安全门原因，机台可以运行
                //ButtonFlag(false, 0);
            }

        }

        public void UpDatalabel(string txt, string Name)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Labelvision(UpDatalabel), new object[] { txt, Name });
                return;
            }
            foreach (Control ctrl in List_Control)
            {
                if (ctrl.GetType() == typeof(Label))
                {
                    if (ctrl.Name == Name)
                    {
                        ctrl.Text = txt;
                    }
                }
            }
        }

        public void RefreachData(Chart chart, string[] Point_X, double[] Point_Y, int index)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new RefreachTable(RefreachData), new object[] { chart, Point_X, Point_Y, index });
                return;
            }
            switch (index)
            {
                case 0:
                    chart.Series[0].Points.DataBindXY(Point_X, Point_Y);
                    break;
                case 1:
                    //chart.Series[1].Points.DataBindXY(Point_X, Point_Y);
                    break;
                case 100:
                    double[] Y = new double[9];
                    for (int i = 0; i < Y.Length; i++)
                    {
                        Y[i] = 0;
                    }
                    chart.Series[0].Points.DataBindXY(Point_X, Y);
                    break;
                default:
                    break;
            }
        }

        private void txtSelectSN_TextChanged(object sender, EventArgs e)
        {
            if (txtSelectSN.Text.Length == 19)
            {
                string serial_number = txtSelectSN.Text;
                //txtSelectSN.Text = string.Empty;
                string SelectStr = string.Format("SELECT * FROM Trace_UA_SendNG WHERE band='{0}'", serial_number);//sql查询语句
                DataTable d1 = SQL.ExecuteQuery(SelectStr);
                string SelectStr2 = string.Format("SELECT * FROM PDCA_SendNG WHERE band='{0}'", serial_number);//sql查询语句
                DataTable d2 = SQL.ExecuteQuery(SelectStr2);
                if (d1.Rows.Count > 0 || d2.Rows.Count > 0)
                {
                    lstSelectSN.Items.Add(string.Format("本机已查询到{0}上传失败缓存记录！", serial_number));
                    if (d1.Rows.Count > 0)
                    {
                        _mainparent.ShowData(dgv_TraceUASendNG, d1, 0);
                        _mainparent.dgv_AutoSize(dgv_TraceUASendNG);
                    }
                    if (d2.Rows.Count > 0)
                    {
                        _mainparent.ShowData(dgv_PDCASendNG, d2, 0);
                        _mainparent.dgv_AutoSize(dgv_PDCASendNG);
                    }
                }
                else
                {
                    lstSelectSN.Items.Add(string.Format("本机未查询到{0}上传失败缓存记录！", serial_number));
                }
            }
            else
            {
            }
        }

        private void btn_UpLoad_Click(object sender, EventArgs e)
        {
            try
            {
                if (Global.Login != Global.LoginLevel.Operator)
                {
                    //防止虚拟SN
                    try
                    {
                        string callresult = "";
                        string errmsg = "";
                        string band = string.Empty;
                        GetSNData SP = new GetSNData();
                        txtSelectSN.Invoke(new Action(() => { band = txtSelectSN.Text; }));
                        JsonSerializerSettings jsetting = new JsonSerializerSettings();
                        jsetting.NullValueHandling = NullValueHandling.Ignore;//Json不输出空值
                        string url = string.Format("http://17.239.116.110/api/v2/parts?serial_type=band&serial={0}&process_name=bd-bc-le&last_log=true", band);
                        var rst = RequestAPI2.CallBobcat5(url, "", out callresult, out errmsg);
                        SP = JsonConvert.DeserializeObject<GetSNData>(callresult);
                        string SendData = JsonConvert.SerializeObject(SP, Formatting.None, jsetting);
                        if (rst)
                        {
                            if (SP.serials.sp != "" && SP.serials.sp.Contains("DRD"))
                            {
                            }
                            else
                            {
                                MessageBox.Show("获取SP码格式异常,sp为;" + SP.serials.sp);
                                return;
                            }
                        }
                        else
                        {
                            MessageBox.Show("获取SP码失败无法补传Trace");
                            return;
                        }
                    }
                    catch (Exception EX)
                    {
                        MessageBox.Show("获取SP码失败无法补传Trace");
                        return;
                    }
                    if (dgv_TraceUASendNG.Rows.Count > 0)//Trace_UA上传失败缓存
                    {
                        TraceMesRequest_ua TraceData = new TraceMesRequest_ua();
                        BAilData bail = new BAilData();
                        TraceData.serials = new SN();
                        TraceData.data = new data();
                        TraceData.data.insight = new Insight();
                        TraceData.data.insight.test_attributes = new Test_attributes();
                        TraceData.data.insight.test_station_attributes = new Test_station_attributes();
                        TraceData.data.insight.uut_attributes = new Uut_attributes();
                        TraceData.data.insight.results = new Result[19];
                        TraceData.data.items = new ExpandoObject();
                        for (int i = 0; i < TraceData.data.insight.results.Length; i++)
                        {
                            TraceData.data.insight.results[i] = new Result();
                        }
                        TraceData.serials.sp = dgv_TraceUASendNG.Rows[0].Cells[2].EditedFormattedValue.ToString();
                        TraceData.data.insight.uut_attributes.full_sn = dgv_TraceUASendNG.Rows[0].Cells[3].EditedFormattedValue.ToString();
                        TraceData.data.insight.test_attributes.test_result = dgv_TraceUASendNG.Rows[0].Cells[4].EditedFormattedValue.ToString();
                        TraceData.data.insight.test_attributes.unit_serial_number = TraceData.data.insight.uut_attributes.full_sn.Remove(17);
                        TraceData.data.insight.test_station_attributes.fixture_id = dgv_TraceUASendNG.Rows[0].Cells[5].EditedFormattedValue.ToString();
                        TraceData.data.insight.uut_attributes.fixture_id = dgv_TraceUASendNG.Rows[0].Cells[5].EditedFormattedValue.ToString();
                        TraceData.data.insight.uut_attributes.tossing_item = dgv_TraceUASendNG.Rows[0].Cells[6].EditedFormattedValue.ToString();
                        TraceData.data.insight.uut_attributes.STATION_STRING = dgv_TraceUASendNG.Rows[0].Cells[7].EditedFormattedValue.ToString();
                        TraceData.data.insight.test_attributes.uut_start = dgv_TraceUASendNG.Rows[0].Cells[8].EditedFormattedValue.ToString();
                        TraceData.data.insight.uut_attributes.laser_start_time = dgv_TraceUASendNG.Rows[0].Cells[9].EditedFormattedValue.ToString();
                        TraceData.data.insight.uut_attributes.laser_stop_time = dgv_TraceUASendNG.Rows[0].Cells[10].EditedFormattedValue.ToString();
                        TraceData.data.insight.test_attributes.uut_stop = dgv_TraceUASendNG.Rows[0].Cells[11].EditedFormattedValue.ToString();
                        //根据缓存下来的tossing_item 确定缓存下来的焊接参数result

                        if (TraceData.data.insight.uut_attributes.tossing_item != "" && TraceData.data.insight.uut_attributes.tossing_item != null)
                        {
                            //预设上传的焊接结果为pass
                            for (int r = 0; r < 36; r++)
                            {
                                TraceData.data.insight.results[r].result = "pass";
                            }
                        }
                        else
                        {
                            if (TraceData.data.insight.uut_attributes.tossing_item.Contains("location1 CCD NG"))
                            {
                                TraceData.data.insight.results[0].result = "fail";
                                TraceData.data.insight.results[4].result = "fail";
                                TraceData.data.insight.results[5].result = "fail";
                                TraceData.data.insight.results[6].result = "fail";
                                TraceData.data.insight.results[7].result = "fail";
                                TraceData.data.insight.results[8].result = "fail";
                                TraceData.data.insight.results[9].result = "fail";
                                TraceData.data.insight.results[10].result = "fail";
                                TraceData.data.insight.results[11].result = "fail";
                            }
                            if (TraceData.data.insight.uut_attributes.tossing_item.Contains("location2 CCD NG"))
                            {
                                TraceData.data.insight.results[1].result = "fail";
                                TraceData.data.insight.results[12].result = "fail";
                                TraceData.data.insight.results[13].result = "fail";
                                TraceData.data.insight.results[14].result = "fail";
                                TraceData.data.insight.results[15].result = "fail";
                                TraceData.data.insight.results[16].result = "fail";
                                TraceData.data.insight.results[17].result = "fail";
                                TraceData.data.insight.results[18].result = "fail";
                                TraceData.data.insight.results[19].result = "fail";
                            }
                            if (TraceData.data.insight.uut_attributes.tossing_item.Contains("location3 CCD NG"))
                            {
                                TraceData.data.insight.results[2].result = "fail";
                                TraceData.data.insight.results[20].result = "fail";
                                TraceData.data.insight.results[21].result = "fail";
                                TraceData.data.insight.results[22].result = "fail";
                                TraceData.data.insight.results[23].result = "fail";
                                TraceData.data.insight.results[24].result = "fail";
                                TraceData.data.insight.results[25].result = "fail";
                                TraceData.data.insight.results[26].result = "fail";
                                TraceData.data.insight.results[27].result = "fail";
                            }
                        }

                        _mainparent.Out_Trace_ua.Add(TraceData.data.insight.uut_attributes.full_sn);
                        _mainparent.Trace_ua.Add(TraceData.data.insight.uut_attributes.full_sn, TraceData);
                        lstSelectSN.Items.Add(string.Format("手动上传TraceUA-{0}缓存记录！", TraceData.data.insight.uut_attributes.full_sn));
                        Log.WriteLog(string.Format("手动上传TraceUA-{0}缓存记录！", TraceData.data.insight.uut_attributes.full_sn));
                        string DeleteStr = string.Format("delete from Trace_UA_SendNG where band = '{0}'", TraceData.data.insight.uut_attributes.full_sn);
                        SQL.ExecuteUpdate(DeleteStr);
                        Thread.Sleep(1000);
                        string SelectStr = string.Format("SELECT * FROM Trace_UA_SendNG where band = '{0}'", TraceData.data.insight.uut_attributes.full_sn);//sql查询语句
                        DataTable d1 = SQL.ExecuteQuery(SelectStr);
                        _mainparent.ShowData(dgv_TraceUASendNG, d1, 0);
                        _mainparent.dgv_AutoSize(dgv_TraceUASendNG);
                        TraceData = null;
                    }
                    if (dgv_PDCASendNG.Rows.Count > 0)//PDCA_UA上传失败缓存
                    {
                        txtSelectSN.Text = string.Empty;
                        BAilData bail = new BAilData();
                        bail.full_sn = dgv_PDCASendNG.Rows[0].Cells[2].EditedFormattedValue.ToString();
                        bail.test_result = dgv_PDCASendNG.Rows[0].Cells[3].EditedFormattedValue.ToString();
                        bail.Fixture_id = dgv_PDCASendNG.Rows[0].Cells[4].EditedFormattedValue.ToString();
                        bail.tossing_item = dgv_PDCASendNG.Rows[0].Cells[5].EditedFormattedValue.ToString();
                        bail.STATION_STRING = dgv_PDCASendNG.Rows[0].Cells[6].EditedFormattedValue.ToString();
                        bail.Start_Time = Convert.ToDateTime(dgv_PDCASendNG.Rows[0].Cells[7].EditedFormattedValue.ToString());
                        bail.Weld_start_time = Convert.ToDateTime(dgv_PDCASendNG.Rows[0].Cells[8].EditedFormattedValue.ToString());
                        bail.Weld_stop_time = Convert.ToDateTime(dgv_PDCASendNG.Rows[0].Cells[9].EditedFormattedValue.ToString());
                        bail.Stop_Time = Convert.ToDateTime(dgv_PDCASendNG.Rows[0].Cells[10].EditedFormattedValue.ToString());
                        bail.auto_send = 2;
                        _mainparent.Out_ua.Add(bail.full_sn);
                        _mainparent.bail_ua.Add(bail.full_sn, bail);
                        lstSelectSN.Items.Add(string.Format("手动上传PDCA-{0}缓存记录！", bail.full_sn));
                        Log.WriteLog(string.Format("手动上传PDCA-{0}缓存记录！", bail.full_sn));
                        string DeleteStr = string.Format("delete from PDCA_SendNG where band = '{0}'", bail.full_sn);
                        SQL.ExecuteUpdate(DeleteStr);
                        Thread.Sleep(1000);
                        string SelectStr = string.Format("SELECT * FROM PDCA_SendNG where band = '{0}'", bail.full_sn);//sql查询语句
                        DataTable d1 = SQL.ExecuteQuery(SelectStr);
                        _mainparent.ShowData(dgv_PDCASendNG, d1, 0);
                        _mainparent.dgv_AutoSize(dgv_PDCASendNG);
                        bail = null;
                    }
                }
                else
                {
                    MessageBox.Show("权限不足请刷卡");
                }

            }
            catch (Exception ex)
            {
                Log.WriteLog("手动上传缓存数据异常失败！" + ex.ToString().Replace("\r\n", ""));
            }
        }

        private void btn_Refresh_Click(object sender, EventArgs e)
        {
            try
            {
                string SelectStr = string.Format("select * from OEE_DT where cast(DateTime as datetime) >= '{0}' and cast(EventTime as datetime) <= '{1}'", dtp_Start.Text, dtp_End.Text);
                DataTable d1 = SQL.ExecuteQuery(SelectStr);
                if (d1.Rows.Count > 0)
                {
                    dataTableToListView(lv_OEEData, d1, dtp_Start.Text, dtp_End.Text);
                }
                else
                {
                    lv_OEEData.View = View.Details;
                    lv_OEEData.GridLines = true;//显示网格线
                    lv_OEEData.Items.Clear();//所有的项
                    lv_OEEData.Columns.Clear();//标题
                    DT_Pie(dtp_Start.Text, dtp_End.Text);
                }
            }
            catch (Exception ex)
            {
                Log.WriteLog(ex.ToString());
            }
        }

        private static bool SaveDataToCSVFile(ListView lv)
        {
            if (lv.Items.Count == 0)
            {
                MessageBox.Show("没有数据可导出!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }
            SaveFileDialog sf = new SaveFileDialog();
            sf.Title = "文档导出";
            sf.Filter = "文档(*.csv)|*.csv";
            sf.FileName = DateTime.Now.Date.ToString("yyyyMMdd");
            if (sf.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string str = sf.FileName;
                using (StreamWriter sw = new StreamWriter(str, false, Encoding.Default))
                {
                    try
                    {
                        sw.WriteLine("序号,开始时间,结束时间,分钟,故障代码,模组代码,运行状态,故障信息");
                        for (int t = 0; t < lv.Items.Count; t++)
                        {
                            string oeestr = "";
                            for (int t2 = 0; t2 < lv.Columns.Count; t2++)
                            {
                                oeestr += lv.Items[t].SubItems[t2].Text.ToString() + ",";
                            }
                            sw.WriteLine(oeestr);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "导出错误", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return false;
                    }
                    sw.Close();
                    sw.Dispose();
                }
            }
            return true;
        }

        private void Btn_Start_errortime_Click(object sender, EventArgs e)//首件开始
        {
            try
            {
                short[] ReadStatus = new short[6];
                ReadStatus = Global.PLC_Client.ReadPLC_D(13000, 6);
                Global.currentCount = 0;//定时计数清零
                Global.timer.Enabled = true;//定时器启用
                Global.timer.Start();//定时器开始
                Btn_Start_errortime.Enabled = false;
                Btn_planStopStart.Enabled = false;
                var IP = GetIp();
                var Mac = GetOEEMac();
                string sendTopic = Global.inidata.productconfig.EMT + "/upload/downtime";
                string ClientPcName = Dns.GetHostName();
                if (ReadStatus[4] != 1)//判断是否处于空跑状态（PLC屏蔽部分功能如：安全门，扫码枪，机械手）
                {
                    if (Global.j == 1)//处于待料状态
                    {
                        Global.ed[Global.Error_PendingNum + 1].stop_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                        DateTime t1 = Convert.ToDateTime(Global.ed[Global.Error_PendingNum + 1].start_time);
                        DateTime t2 = Convert.ToDateTime(Global.ed[Global.Error_PendingNum + 1].stop_time);
                        string ts = (t2 - t1).TotalMinutes.ToString("0.00");
                        string InsertOEEStr = "insert into OEE_DT([DateTime],[ErrorCode],[EventTime],[ModuleCode],[RunStatus],[ErrorInfo],[TimeSpan])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "'" + "," + "'" + Global.ed[Global.Error_PendingNum + 1].errorCode + "'" + "," + "'" + Global.ed[Global.Error_PendingNum + 1].start_time + "'" + ","
                                           + "'" + "" + "'" + "," + "'" + Global.ed[Global.Error_PendingNum + 1].errorStatus + "'" + "," + "'" + Global.ed[Global.Error_PendingNum + 1].errorinfo + "'" + "," + "'" + ts + "'" + ")";
                        SQL.ExecuteUpdate(InsertOEEStr);
                        Log.WriteCSV(DateTime.Now.ToString("HH:mm:ss") + "," + Global.ed[Global.Error_PendingNum + 1].errorCode + "," + Global.ed[Global.Error_PendingNum + 1].start_time + "," + "自动发送成功" + "," + Global.ed[Global.Error_PendingNum + 1].errorStatus + "," + Global.ed[Global.Error_PendingNum + 1].errorinfo + "," + ts, System.AppDomain.CurrentDomain.BaseDirectory + "\\系统配置\\System_ini\\");
                        if (Global.Feeding)//人工上料中开启安全门自动结束人工上料补传一笔HSG待料
                        {
                            Global.SendHSG_start_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                            Global.Feeding = false;
                            _mainparent.SendHSG();
                            Global.SendHSG_start_time = null;
                        }
                    }
                    else if (Global.j == 2)//处于运行状态
                    {
                        Global.ed[Global.j].stop_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                        DateTime t1 = Convert.ToDateTime(Global.ed[Global.j].start_time);
                        DateTime t2 = Convert.ToDateTime(Global.ed[Global.j].stop_time);
                        string ts = (t2 - t1).TotalMinutes.ToString("0.00");
                        string InsertOEEStr = "insert into OEE_DT([DateTime],[ErrorCode],[EventTime],[ModuleCode],[RunStatus],[ErrorInfo],[TimeSpan])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "'" + "," + "'" + Global.ed[Global.j].errorCode + "'" + "," + "'" + Global.ed[Global.j].start_time + "'" + ","
                                           + "'" + "" + "'" + "," + "'" + Global.ed[Global.j].errorStatus + "'" + "," + "'" + Global.ed[Global.j].errorinfo + "'" + "," + "'" + ts + "'" + ")";
                        SQL.ExecuteUpdate(InsertOEEStr);
                        Log.WriteCSV(DateTime.Now.ToString("HH:mm:ss") + "," + Global.ed[Global.j].errorCode + "," + Global.ed[Global.j].start_time + "," + "自动发送成功" + "," + Global.ed[Global.j].errorStatus + "," + Global.ed[Global.j].errorinfo + "," + ts, System.AppDomain.CurrentDomain.BaseDirectory + "\\系统配置\\System_ini\\");
                    }
                    else if (Global.j == 3)
                    {
                        Global.ed[Global.Error_num + 1].stop_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                        DateTime t1 = Convert.ToDateTime(Global.ed[Global.Error_num + 1].start_time);
                        DateTime t2 = Convert.ToDateTime(Global.ed[Global.Error_num + 1].stop_time);
                        string ts = (t2 - t1).TotalMinutes.ToString("0.00");
                        if (Global.Error_num == 115 || Global.Error_num == 110 || Global.Error_num == 111 || Global.Error_num == 112)//机台打开安全门
                        {
                            string OEE_DT3 = "";
                            Guid guid = Guid.NewGuid();
                            OEE_DT3 = string.Format("{{\"guid\":\"{0}\",\"EMT\":\"{1}\",\"PoorNum\":\"{2}\",\"TotalNum\":\"{3}\",  \"Status\":\"{4}\",\"ErrorCode\":\"{5}\",\"EventTime\":\"{6}\",\"ModuleCode\":\"{7}\",\"ClientPcName\":\"{8}\",\"MAC\":\"{9}\",\"IP\":\"{10}\"}}", guid, Global.inidata.productconfig.EMT, "0", "0", Global.errorStatus, Global.errorcode, Global.ed[Global.Error_num + 1].start_time, "", ClientPcName, Mac, IP);
                            Log.WriteLog("OEE_DT安全门打开:" + OEE_DT3 + ",OEELog");
                            //Global._mqttClient.Publish(sendTopic, Encoding.UTF8.GetBytes(OEE_DT3), MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE, false);
                            //bool rst3 = _mainparent.SendMqttResult(guid);
                            //if (rst3)
                            //{
                            //    if (Global.respond[guid].Result == "OK")
                            //    {
                            //        Global.ConnectOEEFlag = true;
                            //        Log.WriteLog("OEE_DT安全门打开自动errorCode发送成功" + ",OEELog");
                            //        _mainparent._homefrm.AppendRichText(Global.errorcode + ",触发时间=" + Global.ed[Global.Error_num + 1].start_time + ",运行状态:" + Global.errorStatus + ",故障描述:" + Global.errorinfo + ",安全门打开自动发送成功", "rtx_DownTimeMsg");
                            //        Global.respond.TryRemove(guid, out Global.Respond);
                            //    }
                            //    else
                            //    {
                            //        Global.ConnectOEEFlag = false;
                            //        Log.WriteLog("OEE_DT安全门打开自动errorCode发送失败" + ",OEELog");
                            //        Log.WriteLog(Global.respond[guid].GUID.ToString() + "," + Global.respond[guid].Result + "," + Global.respond[guid].ErrorCode, System.AppDomain.CurrentDomain.BaseDirectory + "\\MQTT返回NG\\");
                            //        _mainparent._homefrm.AppendRichText(Global.errorcode + ",触发时间=" + Global.ed[Global.Error_num + 1].start_time + ",运行状态:" + Global.errorStatus + ",故障描述:" + Global.errorinfo + ",安全门打开自动发送失败", "rtx_DownTimeMsg");
                            //        _mainparent._homefrm.AppendRichText("失败原因:" + Global.respond[guid].ErrorCode + Global.respond[guid].Result, "rtx_DownTimeMsg");
                            //        Global.respond.TryRemove(guid, out Global.Respond);
                            //    }
                            //}
                            //else
                            //{
                            //    Global.ConnectOEEFlag = false;
                            //    Log.WriteLog("OEE_DT安全门打开自动errorCode发送失败,超时无反馈:" + ",OEELog");
                            //    _mainparent._homefrm.AppendRichText(Global.errorcode + ",触发时间=" + Global.ed[Global.Error_num + 1].start_time + ",运行状态:" + Global.errorStatus + ",故障描述:" + Global.errorinfo + ",安全门打开自动发送失败", "rtx_DownTimeMsg");
                            //    _mainparent._homefrm.AppendRichText("网络异常,超时无反馈", "rtx_DownTimeMsg");
                            //    string s = "insert into OEE_DTSendNG([DateTime],[Product],[GUID],[EMT],[PoorNum],[TotalNum],[Status],[ErrorCode],[ModuleCode],[ClientPcName],[MAC],[IP],[EventTime],[Moduleinfo],[errorinfo])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," + "'" + "OEE_DT" + "'" + "," + "'" + guid + "'" + "," + "'" + Global.inidata.productconfig.EMT + "'" + "," + "'" + "0" + "'" + "," + "'" + "0" + "'" + "," + "'" + Global.errorStatus + "'" + "," + "'" + Global.errorcode + "'" + ","
                            //           + "'" + "" + "'" + "," + "'" + ClientPcName + "'" + "," + "'" + Mac + "'" + "," + "'" + IP + "'" + "," + "'" + Global.ed[Global.Error_num + 1].start_time + "'" + "," + "'" + "" + "'" + "," + "'" + Global.errorinfo + "'" + ")";
                            //    int r = SQL.ExecuteUpdate(s);
                            //    Log.WriteLog(string.Format("插入了{0}行OEE_DownTime缓存数据", r) + ",OEELog");
                            //}
                            labelcolor(Color.Transparent, "未选择", "LB_ManualSelect");
                            string InsertOEEStr = "insert into OEE_DT([DateTime],[ErrorCode],[EventTime],[ModuleCode],[RunStatus],[ErrorInfo],[TimeSpan])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "'" + "," + "'" + Global.errorcode + "'" + "," + "'" + Global.ed[Global.Error_num + 1].start_time + "'" + ","
                           + "'" + "" + "'" + "," + "'" + Global.errorStatus + "'" + "," + "'" + Global.errorinfo + "'" + "," + "'" + ts + "'" + ")";
                            SQL.ExecuteUpdate(InsertOEEStr);
                            InsertOEEStr = "insert into OEE_TraceDT([DateTime],[Status],[ErrorCode],[EventTime],[ModuleCode])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," + "'" + Global.errorStatus + "'" + "," + "'" + Global.errorcode + "'" + ","
                                    + "'" + Global.ed[Global.Error_num + 1].start_time + "'" + "," + "'" + Global.ed[Global.Error_num + 1].ModuleCode + "'" + ")";
                            SQL.ExecuteUpdate(InsertOEEStr);
                            Log.WriteLog(DateTime.Now.ToString("HH:mm:ss") + "," + Global.errorcode + "," + Global.ed[Global.Error_num + 1].start_time + "," + "" + "," + "自动发送成功" + "," + Global.errorStatus + "," + Global.errorinfo + "," + ts, System.AppDomain.CurrentDomain.BaseDirectory + "\\系统配置\\System_ini\\");
                        }
                        else//机台处于其它异常状态中
                        {
                            string InsertOEEStr = "insert into OEE_DT([DateTime],[ErrorCode],[EventTime],[ModuleCode],[RunStatus],[ErrorInfo],[TimeSpan])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "'" + "," + "'" + Global.ed[Global.Error_num + 1].errorCode + "'" + "," + "'" + Global.ed[Global.Error_num + 1].start_time + "'" + ","
                                           + "'" + Global.ed[Global.Error_num + 1].ModuleCode + "'" + "," + "'" + Global.ed[Global.Error_num + 1].errorStatus + "'" + "," + "'" + Global.ed[Global.Error_num + 1].errorinfo + "'" + "," + "'" + ts + "'" + ")";
                            SQL.ExecuteUpdate(InsertOEEStr);
                            Log.WriteLog(DateTime.Now.ToString("HH:mm:ss") + "," + Global.ed[Global.Error_num + 1].errorCode + "," + Global.ed[Global.Error_num + 1].start_time + "," + "自动发送成功" + "," + Global.ed[Global.Error_num + 1].errorStatus + "," + Global.ed[Global.Error_num + 1].errorinfo + "," + ts, System.AppDomain.CurrentDomain.BaseDirectory + "\\系统配置\\System_ini\\");
                        }
                    }
                    else if (Global.j == 4)//处于人工停止状态
                    {
                        Global.ed[Global.Error_Stopnum + 1].stop_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                        string c = "c=UPLOAD_DOWNTIME&tsn=Test_station&mn=Machine#1&start_time=" + Global.ed[Global.Error_Stopnum + 1].start_time + "&stop_time=" + Global.ed[Global.Error_Stopnum + 1].stop_time + "&ec=" + Global.ed[Global.Error_Stopnum + 1].errorCode;
                        Log.WriteLog(c + ",OEELog");
                        if (Global.ed[Global.Error_Stopnum + 1].start_time != null)
                        {
                            DateTime t1 = Convert.ToDateTime(Global.ed[Global.Error_Stopnum + 1].start_time);
                            DateTime t2 = Convert.ToDateTime(Global.ed[Global.Error_Stopnum + 1].stop_time);
                            string ts = (t2 - t1).TotalMinutes.ToString("0.00");
                            if (Global.Error_Stopnum == 115 || Global.Error_Stopnum == 110 || Global.Error_Stopnum == 111 || Global.Error_Stopnum == 112 || Global.SelectManualErrorCode)//机台打开安全门或者手动选择ErrorCode状态开启
                            {
                                string OEE_DT4 = "";
                                Global.Art_stop = true;
                                Guid guid = Guid.NewGuid();
                                OEE_DT4 = string.Format("{{\"guid\":\"{0}\",\"EMT\":\"{1}\",\"PoorNum\":\"{2}\",\"TotalNum\":\"{3}\",  \"Status\":\"{4}\",\"ErrorCode\":\"{5}\",\"EventTime\":\"{6}\",\"ModuleCode\":\"{7}\",\"ClientPcName\":\"{8}\",\"MAC\":\"{9}\",\"IP\":\"{10}\"}}", guid, Global.inidata.productconfig.EMT, "0", "0", Global.errorStatus, Global.errorcode, Global.ed[Global.Error_Stopnum + 1].start_time, "", ClientPcName, Mac, IP);
                                Log.WriteLog("OEE_DT安全门打开:" + OEE_DT4 + ",OEELog");
                                //Global._mqttClient.Publish(sendTopic, Encoding.UTF8.GetBytes(OEE_DT4), MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE, false);
                                //bool rst4 = _mainparent.SendMqttResult(guid);
                                //if (rst4)
                                //{
                                //    if (Global.respond[guid].Result == "OK")
                                //    {
                                //        Global.ConnectOEEFlag = true;
                                //        Log.WriteLog("OEE_DT安全门打开自动errorCode发送成功" + ",OEELog");
                                //        _mainparent._homefrm.AppendRichText(Global.errorcode + ",触发时间=" + Global.ed[Global.Error_Stopnum + 1].start_time + ",运行状态:" + Global.errorStatus + ",故障描述:" + Global.errorinfo + ",安全门打开自动发送成功", "rtx_DownTimeMsg");
                                //        Global.respond.TryRemove(guid, out Global.Respond);
                                //    }
                                //    else
                                //    {
                                //        Global.ConnectOEEFlag = false;
                                //        Log.WriteLog("OEE_DT安全门打开自动errorCode发送失败" + ",OEELog");
                                //        Log.WriteLog(Global.respond[guid].GUID.ToString() + "," + Global.respond[guid].Result + "," + Global.respond[guid].ErrorCode, System.AppDomain.CurrentDomain.BaseDirectory + "\\MQTT返回NG\\");
                                //        _mainparent._homefrm.AppendRichText(Global.errorcode + ",触发时间=" + Global.ed[Global.Error_Stopnum + 1].start_time + ",运行状态:" + Global.errorStatus + ",故障描述:" + Global.errorinfo + ",安全门打开自动发送失败", "rtx_DownTimeMsg");
                                //        _mainparent._homefrm.AppendRichText("失败原因:" + Global.respond[guid].ErrorCode + Global.respond[guid].Result, "rtx_DownTimeMsg");
                                //        Global.respond.TryRemove(guid, out Global.Respond);
                                //    }
                                //}
                                //else
                                //{
                                //    Global.ConnectOEEFlag = false;
                                //    Log.WriteLog("OEE_DT人工停止复位自动errorCode发送失败,超时无反馈:" + ",OEELog");
                                //    _mainparent._homefrm.AppendRichText(Global.errorcode + ",触发时间=" + Global.ed[Global.Error_Stopnum + 1].start_time + ",运行状态:" + Global.errorStatus + ",故障描述:" + Global.errorinfo + ",安全门打开自动发送失败", "rtx_DownTimeMsg");
                                //    _mainparent._homefrm.AppendRichText("网络异常,超时无反馈", "rtx_DownTimeMsg");
                                //    string s = "insert into OEE_DTSendNG([DateTime],[Product],[GUID],[EMT],[PoorNum],[TotalNum],[Status],[ErrorCode],[ModuleCode],[ClientPcName],[MAC],[IP],[EventTime],[Moduleinfo],[errorinfo])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," + "'" + "OEE_DT" + "'" + "," + "'" + guid + "'" + "," + "'" + Global.inidata.productconfig.EMT + "'" + "," + "'" + "0" + "'" + "," + "'" + "0" + "'" + "," + "'" + Global.errorStatus + "'" + "," + "'" + Global.errorcode + "'" + ","
                                //           + "'" + "" + "'" + "," + "'" + ClientPcName + "'" + "," + "'" + Mac + "'" + "," + "'" + IP + "'" + "," + "'" + Global.ed[Global.Error_Stopnum + 1].start_time + "'" + "," + "'" + "" + "'" + "," + "'" + Global.errorinfo + "'" + ")";
                                //    int r = SQL.ExecuteUpdate(s);
                                //    Log.WriteLog(string.Format("插入了{0}行OEE_DownTime缓存数据", r) + ",OEELog");
                                //}
                                labelcolor(Color.Transparent, "未选择", "LB_ManualSelect");
                                string InsertOEEStr = "insert into OEE_DT([DateTime],[ErrorCode],[EventTime],[ModuleCode],[RunStatus],[ErrorInfo],[TimeSpan])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "'" + "," + "'" + Global.errorcode + "'" + "," + "'" + Global.ed[Global.Error_Stopnum + 1].start_time + "'" + ","
                               + "'" + "" + "'" + "," + "'" + Global.errorStatus + "'" + "," + "'" + Global.errorinfo + "'" + "," + "'" + ts + "'" + ")";
                                SQL.ExecuteUpdate(InsertOEEStr);
                                InsertOEEStr = "insert into OEE_TraceDT([DateTime],[Status],[ErrorCode],[EventTime],[ModuleCode])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," + "'" + Global.errorStatus + "'" + "," + "'" + Global.errorcode + "'" + ","
                                        + "'" + Global.ed[Global.Error_Stopnum + 1].start_time + "'" + "," + "'" + Global.ed[Global.Error_Stopnum + 1].ModuleCode + "'" + ")";
                                SQL.ExecuteUpdate(InsertOEEStr);
                                Log.WriteLog(DateTime.Now.ToString("HH:mm:ss") + "," + Global.errorcode + "," + Global.ed[Global.Error_Stopnum + 1].start_time + "," + "" + "," + "自动发送成功" + "," + Global.errorStatus + "," + Global.errorinfo + "," + ts, System.AppDomain.CurrentDomain.BaseDirectory + "\\系统配置\\System_ini\\");
                            }
                            else if (Global.Error_Stopnum == 278)//机台人工停止
                            {
                                string OEE_DT4 = "";
                                Guid guid = Guid.NewGuid();
                                OEE_DT4 = string.Format("{{\"guid\":\"{0}\",\"EMT\":\"{1}\",\"PoorNum\":\"{2}\",\"TotalNum\":\"{3}\",  \"Status\":\"{4}\",\"ErrorCode\":\"{5}\",\"EventTime\":\"{6}\",\"ModuleCode\":\"{7}\",\"ClientPcName\":\"{8}\",\"MAC\":\"{9}\",\"IP\":\"{10}\"}}", guid, Global.inidata.productconfig.EMT, "0", "0", Global.ed[Global.Error_Stopnum + 1].errorStatus, Global.ed[Global.Error_Stopnum + 1].errorCode, Global.ed[Global.Error_Stopnum + 1].start_time, Global.ed[Global.Error_Stopnum + 1].ModuleCode, ClientPcName, Mac, IP);
                                Log.WriteLog("OEE_DT人工停止复位:" + OEE_DT4 + ",OEELog");
                                //Global._mqttClient.Publish(sendTopic, Encoding.UTF8.GetBytes(OEE_DT4), MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE, false);
                                //bool rst4 = _mainparent.SendMqttResult(guid);
                                //if (rst4)
                                //{
                                //    if (Global.respond[guid].Result == "OK")
                                //    {
                                //        Global.ConnectOEEFlag = true;
                                //        Log.WriteLog("OEE_DT人工停止复位自动errorCode发送成功" + ",OEELog");
                                //        _mainparent._homefrm.AppendRichText(Global.ed[Global.Error_Stopnum + 1].errorCode + ",触发时间=" + Global.ed[Global.Error_Stopnum + 1].start_time + ",运行状态:" + Global.ed[Global.Error_Stopnum + 1].errorStatus + ",故障描述:" + Global.ed[Global.Error_Stopnum + 1].errorinfo + ",安全门打开自动发送成功", "rtx_DownTimeMsg");
                                //        Global.respond.TryRemove(guid, out Global.Respond);
                                //    }
                                //    else
                                //    {
                                //        Global.ConnectOEEFlag = false;
                                //        Log.WriteLog("OEE_DT人工停止复位自动errorCode发送失败" + ",OEELog");
                                //        Log.WriteLog(Global.respond[guid].GUID.ToString() + "," + Global.respond[guid].Result + "," + Global.respond[guid].ErrorCode, System.AppDomain.CurrentDomain.BaseDirectory + "\\MQTT返回NG\\");
                                //        _mainparent._homefrm.AppendRichText(Global.ed[Global.Error_Stopnum + 1].errorCode + ",触发时间=" + Global.ed[Global.Error_Stopnum + 1].start_time + ",运行状态:" + Global.ed[Global.Error_Stopnum + 1].errorStatus + ",故障描述:" + Global.ed[Global.Error_Stopnum + 1].errorinfo + ",安全门打开自动发送失败", "rtx_DownTimeMsg");
                                //        _mainparent._homefrm.AppendRichText("失败原因:" + Global.respond[guid].ErrorCode + Global.respond[guid].Result, "rtx_DownTimeMsg");
                                //        Global.respond.TryRemove(guid, out Global.Respond);
                                //    }
                                //}
                                //else
                                //{
                                //    Global.ConnectOEEFlag = false;
                                //    Log.WriteLog("OEE_DT人工停止复位自动errorCode发送失败,超时无反馈:" + ",OEELog");
                                //    _mainparent._homefrm.AppendRichText(Global.ed[Global.Error_Stopnum + 1].errorCode + ",触发时间=" + Global.ed[Global.Error_Stopnum + 1].start_time + ",运行状态:" + Global.ed[Global.Error_Stopnum + 1].errorStatus + ",故障描述:" + Global.ed[Global.Error_Stopnum + 1].errorinfo + ",安全门打开自动发送失败", "rtx_DownTimeMsg");
                                //    _mainparent._homefrm.AppendRichText("网络异常,超时无反馈", "rtx_DownTimeMsg");
                                //    string s = "insert into OEE_DTSendNG([DateTime],[Product],[GUID],[EMT],[PoorNum],[TotalNum],[Status],[ErrorCode],[ModuleCode],[ClientPcName],[MAC],[IP],[EventTime],[Moduleinfo],[errorinfo])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," + "'" + "OEE_DT" + "'" + "," + "'" + guid + "'" + "," + "'" + Global.inidata.productconfig.EMT + "'" + "," + "'" + "0" + "'" + "," + "'" + "0" + "'" + "," + "'" + Global.ed[Global.Error_Stopnum + 1].errorStatus + "'" + "," + "'" + Global.ed[Global.Error_Stopnum + 1].errorCode + "'" + ","
                                //           + "'" + Global.ed[Global.Error_Stopnum + 1].ModuleCode + "'" + "," + "'" + ClientPcName + "'" + "," + "'" + Mac + "'" + "," + "'" + IP + "'" + "," + "'" + Global.ed[Global.Error_Stopnum + 1].start_time + "'" + "," + "'" + Global.ed[Global.Error_Stopnum + 1].Moduleinfo + "'" + "," + "'" + Global.ed[Global.Error_Stopnum + 1].errorinfo + "'" + ")";
                                //    int r = SQL.ExecuteUpdate(s);
                                //    Log.WriteLog(string.Format("插入了{0}行OEE_DownTime缓存数据", r) + ",OEELog");
                                //}
                                labelcolor(Color.Transparent, "未选择", "LB_ManualSelect");
                                string InsertOEEStr = "insert into OEE_DT([DateTime],[ErrorCode],[EventTime],[ModuleCode],[RunStatus],[ErrorInfo],[TimeSpan])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "'" + "," + "'" + Global.ed[Global.Error_Stopnum + 1].errorCode + "'" + "," + "'" + Global.ed[Global.Error_Stopnum + 1].start_time + "'" + ","
                               + "'" + "" + "'" + "," + "'" + Global.ed[Global.Error_Stopnum + 1].errorStatus + "'" + "," + "'" + Global.ed[Global.Error_Stopnum + 1].errorinfo + "'" + "," + "'" + ts + "'" + ")";
                                SQL.ExecuteUpdate(InsertOEEStr);
                                InsertOEEStr = "insert into OEE_TraceDT([DateTime],[Status],[ErrorCode],[EventTime],[ModuleCode])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," + "'" + Global.ed[Global.Error_Stopnum + 1].errorStatus + "'" + "," + "'" + Global.ed[Global.Error_Stopnum + 1].errorCode + "'" + ","
                                 + "'" + Global.ed[Global.Error_Stopnum + 1].start_time + "'" + "," + "'" + Global.ed[Global.Error_Stopnum + 1].ModuleCode + "'" + ")";
                                SQL.ExecuteUpdate(InsertOEEStr);
                                Log.WriteLog(DateTime.Now.ToString("HH:mm:ss") + "," + Global.ed[Global.Error_Stopnum + 1].errorCode + "," + Global.ed[Global.Error_Stopnum + 1].start_time + "," + "" + "," + "自动发送成功" + "," + Global.ed[Global.Error_Stopnum + 1].errorStatus + "," + Global.ed[Global.Error_Stopnum + 1].errorinfo + "," + ts, System.AppDomain.CurrentDomain.BaseDirectory + "\\系统配置\\System_ini\\");
                            }
                            else//机台处于其它异常状态中
                            {
                                Log.WriteLog("PLC人工停止ErrorCode异常" + Global.Error_Stopnum + ",OEELog");
                            }
                        }
                        Log.WriteLog(Global.ed[Global.Error_Stopnum + 1].Moduleinfo + "_" + Global.ed[Global.Error_Stopnum + 1].errorinfo + "：结束计时 " + Global.ed[Global.Error_Stopnum + 1].stop_time + ",OEELog");
                        //Global.ed[Global.Error_Stopnum + 1].start_time = null;
                        //Global.ed[Global.Error_Stopnum + 1].stop_time = null;
                    }
                }
                else
                {
                    if (Global.SelectTestRunModel == true && Global.ed[313].start_time != null)//空运行结束写入OEE_DT数据表中
                    {
                        Global.ed[313].stop_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                        DateTime t1 = Convert.ToDateTime(Global.ed[313].start_time);
                        DateTime t2 = Convert.ToDateTime(Global.ed[313].stop_time);
                        string ts = (t2 - t1).TotalMinutes.ToString("0.00");
                        string InsertStr6 = "insert into OEE_DT([DateTime],[ErrorCode],[EventTime],[ModuleCode],[RunStatus],[ErrorInfo],[TimeSpan])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "'" + "," + "'" + Global.ed[313].errorCode + "'" + "," + "'" + Global.ed[313].start_time + "'" + ","
                           + "'" + Global.ed[313].ModuleCode + "'" + "," + "'" + Global.ed[313].errorStatus + "'" + "," + "'" + Global.ed[313].errorinfo + "'" + "," + "'" + ts + "'" + ")";
                        SQL.ExecuteUpdate(InsertStr6);
                        Log.WriteCSV(DateTime.Now.ToString("HH:mm:ss") + "," + Global.ed[313].errorCode + "," + Global.ed[313].start_time + "," + Global.ed[313].ModuleCode + "," + "自动发送成功" + "," + Global.ed[313].errorStatus + "," + Global.ed[313].errorinfo + "," + ts, System.AppDomain.CurrentDomain.BaseDirectory + "\\系统配置\\System_ini\\");
                        //Global.ed[280].start_time = null;
                        //Global.ed[280].stop_time = null;
                    }
                    Global.SelectTestRunModel = false;
                }
                string OEE_DT = "";
                string EventTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                Guid guid1 = Guid.NewGuid();
                string InsertStr = "insert into OEE_TraceDT([DateTime],[Status],[ErrorCode],[EventTime],[ModuleCode])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," + "'" + "9" + "'" + "," + "'" + "20010001" + "'" + ","
                        + "'" + EventTime + "'" + "," + "'" + "" + "'" + ")";
                SQL.ExecuteUpdate(InsertStr);
                OEE_DT = string.Format("{{\"guid\":\"{0}\",\"EMT\":\"{1}\",\"PoorNum\":\"{2}\",\"TotalNum\":\"{3}\",  \"Status\":\"{4}\",\"ErrorCode\":\"{5}\",\"EventTime\":\"{6}\",\"ModuleCode\":\"{7}\",\"ClientPcName\":\"{8}\",\"MAC\":\"{9}\",\"IP\":\"{10}\"}}", guid1, Global.inidata.productconfig.EMT, "0", "0", "9", "20010001", EventTime, "", ClientPcName, Mac, IP);
                Log.WriteLog("OEE_DT手动:" + OEE_DT + ",OEELog");
                //Global._mqttClient.Publish(sendTopic, Encoding.UTF8.GetBytes(OEE_DT), MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE, false);
                //bool rst = _mainparent.SendMqttResult(guid1);
                //if (rst)
                //{
                //    if (Global.respond[guid1].Result == "OK")
                //    {
                //        Global.ConnectOEEFlag = true;
                //        Log.WriteLog("OEE_DT机台做首件发送成功" + ",OEELog");
                //        Log.WriteLog(DateTime.Now.ToString("HH:mm:ss") + "," + "20010001" + "," + EventTime + "," + "手动发送成功" + "," + "9" + "," + "机台做验证做首件", System.AppDomain.CurrentDomain.BaseDirectory + "\\系统配置\\System_ini\\");
                //        _mainparent._homefrm.AppendRichText("20010001" + ",触发时间=" + EventTime + ",运行状态:" + "9" + ",故障描述:" + "首件" + ",手动发送成功", "rtx_DownTimeMsg");
                //        Global.respond.TryRemove(guid1, out Global.Respond);
                //    }
                //    else
                //    {
                //        Global.ConnectOEEFlag = false;
                //        Log.WriteLog("OEE_DT机台做首件发送失败" + ",OEELog");
                //        Log.WriteLog(DateTime.Now.ToString("HH:mm:ss") + "," + "20010001" + "," + EventTime + "," + "手动发送失败" + "," + "9" + "," + "机台做验证做首件", System.AppDomain.CurrentDomain.BaseDirectory + "\\系统配置\\System_ini\\");
                //        Log.WriteLog(Global.respond[guid1].GUID.ToString() + "," + Global.respond[guid1].Result + "," + Global.respond[guid1].ErrorCode, System.AppDomain.CurrentDomain.BaseDirectory + "\\MQTT返回NG\\");
                //        _mainparent._homefrm.AppendRichText("失败原因:" + Global.respond[guid1].ErrorCode + Global.respond[guid1].Result, "rtx_DownTimeMsg");
                //        Global.respond.TryRemove(guid1, out Global.Respond);
                //    }
                //}
                //else
                //{
                //    Global.ConnectOEEFlag = false;
                //    Log.WriteLog("OEE_DT机台做首件发送失败" + ",OEELog");
                //    Log.WriteLog(DateTime.Now.ToString("HH:mm:ss") + "," + "20010001" + "," + EventTime + "," + "手动发送失败" + "," + "1" + "," + "机台做验证做首件", System.AppDomain.CurrentDomain.BaseDirectory + "\\系统配置\\System_ini\\");
                //    _mainparent._homefrm.AppendRichText("20010001" + ",触发时间=" + EventTime + ",运行状态:" + "9" + ",故障描述:" + "首件" + ",手动发送失败", "rtx_DownTimeMsg");
                //    _mainparent._homefrm.AppendRichText("网络异常,超时无反馈", "rtx_DownTimeMsg");
                //    string s = "insert into OEE_DTSendNG([DateTime],[Product],[GUID],[EMT],[PoorNum],[TotalNum],[Status],[ErrorCode],[ModuleCode],[ClientPcName],[MAC],[IP],[EventTime],[Moduleinfo],[errorinfo])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," + "'" + "OEE_DT" + "'" + "," + "'" + guid1 + "'" + "," + "'" + Global.inidata.productconfig.EMT + "'" + "," + "'" + "0" + "'" + "," + "'" + "0" + "'" + "," + "'" + "9" + "'" + "," + "'" + "20010001" + "'" + ","
                //           + "'" + "" + "'" + "," + "'" + ClientPcName + "'" + "," + "'" + Mac + "'" + "," + "'" + IP + "'" + "," + "'" + EventTime + "'" + "," + "'" + "" + "'" + "," + "'" + "首件" + "'" + ")";
                //    int r = SQL.ExecuteUpdate(s);
                //    Log.WriteLog(string.Format("插入了{0}行OEE_DownTime缓存数据", r));
                //}
                Global.PLC_Client.WritePLC_D(16014, new short[] { 1 });//通知PLC开始做首件做验证
                Global.errorTime1 = true;
                Global.errordata.start_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                Global.errordata.errorStatus = "9";
                Global.errordata.errorCode = "20010001";
                Global.errordata.errorinfo = "首件";
                Btn_UpLoad_errortime.Enabled = true;
                lb_errorMsg.Visible = true;
                Global.SelectFirstModel = true;
                Global.SelectFirst = true;//首件开始标志位，用来卡小料抛料
                string InsertOEEStr3 = "insert into OEE_StartTime([Status],[ErrorCode],[EventTime],[ModuleCode],[Name])" + " " + "values(" + "'" + "9" + "'" + "," + "'" + "20010001" + "'" + "," + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "'" + "," + "'" + "" + "'" + "," + "'" + "首件" + "'" + ")";
                SQL.ExecuteUpdate(InsertOEEStr3);//插入首件开始时间
            }
            catch (Exception ex)
            {
                Log.WriteLog("首件开始异常失败！" + ex.ToString().Replace("\r\n", ""));
            }
        }

        public void Btn_UpLoad_errortime_Click(object sender, EventArgs e)
        {
            Global.currentCount = 0;//定时计数清零
            Global.timer.Stop();//定时器结束
            bool timeset = false;
            Global.SelectFirstModel = false;
            Global.SelectFirst = false;
            if (Global.errorTime1 == true)
            {
                Global.errordata.stop_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                timeset = true;
            }
            if (Global.errordata.errorCode != null && Global.errordata.start_time != null && timeset == true)
            {
                timeset = false;
                Global.errorTime1 = false;
                string c = "c=UPLOAD_DOWNTIME&tsn=Test_station&mn=Machine#1&start_time=" + Global.errordata.start_time + "&stop_time=" + Global.errordata.stop_time + "&ec=" + Global.errordata.errorCode;
                Log.WriteLog(c);
                DateTime t1 = Convert.ToDateTime(Global.errordata.start_time);
                DateTime t2 = Convert.ToDateTime(Global.errordata.stop_time);
                string ts = (t2 - t1).TotalMinutes.ToString("0.00");
                string InsertOEEStr = "insert into OEE_DT([DateTime],[ErrorCode],[EventTime],[ModuleCode],[RunStatus],[ErrorInfo],[TimeSpan])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "'" + "," + "'" + Global.errordata.errorCode + "'" + "," + "'" + Global.errordata.start_time + "'" + ","
                                     + "'" + "" + "'" + "," + "'" + Global.errordata.errorStatus + "'" + "," + "'" + Global.errordata.errorinfo + "'" + "," + "'" + ts + "'" + ")";
                SQL.ExecuteUpdate(InsertOEEStr);
                Log.WriteCSV(DateTime.Now.ToString("HH:mm:ss") + "," + Global.errordata.errorCode + "," + Global.errordata.start_time + "," + "手动发送成功" + "," + Global.errordata.errorStatus + "," + Global.errordata.errorinfo + "," + ts, System.AppDomain.CurrentDomain.BaseDirectory + "\\系统配置\\System_ini\\");
            }
            else
            {
                Log.WriteLog("1.请选择好错误信息和时间 2.请检查网线与网址！");
                Log.WriteLog("errorCode:" + Global.errordata.errorCode + "," + "start_time:" + Global.errordata.start_time + "," + "timeset:" + timeset);
            }
            try
            {
                Global.PLC_Client.WritePLC_D(16014, new short[] { 0 });//通知PLC做首件做验证已结束
            }
            catch
            { }
            Btn_UpLoad_errortime.BeginInvoke(new Action(() => { Btn_UpLoad_errortime.Enabled = false; }));
            lb_errorMsg.BeginInvoke(new Action(() => { lb_errorMsg.Visible = false; }));
            Btn_Start_errortime.BeginInvoke(new Action(() => { Btn_Start_errortime.Enabled = true; }));
            Btn_planStopStart.BeginInvoke(new Action(() => { Btn_planStopStart.Enabled = true; }));
        }

        public string GetIp()//获取本机IP
        {
            NetworkInterface[] interfaces = System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces();
            int len = interfaces.Length;
            string mip = "";
            for (int i = 0; i < len; i++)
            {
                NetworkInterface ni = interfaces[i];
                if (ni.Name == "OEE")
                {
                    IPInterfaceProperties property = ni.GetIPProperties();
                    foreach (UnicastIPAddressInformation ip in property.UnicastAddresses)
                    {
                        if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        {
                            mip = ip.Address.ToString();
                        }
                    }
                }
            }
            return mip;
        }
        public string GetOEEMac()
        {
            string OEEMac = "";
            string Mac = "";
            NetworkInterface[] netWorks = NetworkInterface.GetAllNetworkInterfaces(); foreach (NetworkInterface netWork in netWorks)
            {
                if (netWork.Name == "OEE")
                {

                    OEEMac = netWork.GetPhysicalAddress().ToString();
                    for (int i = 0; i < OEEMac.Length - 3; i++)
                    {
                        if (i % 2 == 0)
                        {
                            Mac += OEEMac.Substring(i, 2) + "-";
                        }

                    }
                    Mac += OEEMac.Substring(OEEMac.Length - 2, 2);
                }
            }
            return Mac;
        }
        public string GetMac()//获取本机MAC地址
        {
            string strMac = "";
            ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection moc = mc.GetInstances();
            foreach (ManagementObject mo in moc)
            {
                if ((bool)mo["IPEnabled"] == true)
                {
                    strMac = mo["MacAddress"].ToString();
                    mo.Dispose();
                    break;
                }
            }
            moc = null;
            mc = null;
            return strMac;
        }

        private void Btn_Start_EatandRest_Click(object sender, EventArgs e)
        {
            try
            {
                short[] ReadStatus = new short[6];
                ReadStatus = Global.PLC_Client.ReadPLC_D(13000, 6);
                if (ReadStatus[0] == 2 || ReadStatus[0] == 3)
                {
                    try
                    {
                        Global.EatAndRest_currentCount = 0;//定时计数清零
                        Global.EatAndRest_timer.Enabled = true;//定时器启用
                        Global.EatAndRest_timer.Start();//定时器开始
                        Btn_planStopStart.Enabled = false;
                        Btn_Start_errortime.Enabled = false;
                        var IP = GetIp();
                        var Mac = GetOEEMac();
                        string sendTopic = Global.inidata.productconfig.EMT + "/upload/downtime";
                        string ClientPcName = Dns.GetHostName();
                        if (ReadStatus[4] != 1)//判断是否处于空跑状态（PLC屏蔽部分功能如：安全门，扫码枪，机械手）
                        {
                            if (Global.j == 1)//处于待料状态
                            {
                                Global.ed[Global.Error_PendingNum + 1].stop_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                                DateTime t1 = Convert.ToDateTime(Global.ed[Global.Error_PendingNum + 1].start_time);
                                DateTime t2 = Convert.ToDateTime(Global.ed[Global.Error_PendingNum + 1].stop_time);
                                string ts = (t2 - t1).TotalMinutes.ToString("0.00");
                                string InsertOEEStr = "insert into OEE_DT([DateTime],[ErrorCode],[EventTime],[ModuleCode],[RunStatus],[ErrorInfo],[TimeSpan])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "'" + "," + "'" + Global.ed[Global.Error_PendingNum + 1].errorCode + "'" + "," + "'" + Global.ed[Global.Error_PendingNum + 1].start_time + "'" + ","
                                                   + "'" + "" + "'" + "," + "'" + Global.ed[Global.Error_PendingNum + 1].errorStatus + "'" + "," + "'" + Global.ed[Global.Error_PendingNum + 1].errorinfo + "'" + "," + "'" + ts + "'" + ")";
                                SQL.ExecuteUpdate(InsertOEEStr);
                                Log.WriteCSV(DateTime.Now.ToString("HH:mm:ss") + "," + Global.ed[Global.Error_PendingNum + 1].errorCode + "," + Global.ed[Global.Error_PendingNum + 1].start_time + "," + "自动发送成功" + "," + Global.ed[Global.Error_PendingNum + 1].errorStatus + "," + Global.ed[Global.Error_PendingNum + 1].errorinfo + "," + ts, System.AppDomain.CurrentDomain.BaseDirectory + "\\系统配置\\System_ini\\");
                                if (Global.Feeding)//人工上料中开启安全门自动结束人工上料补传一笔HSG待料
                                {
                                    Global.SendHSG_start_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                                    Global.Feeding = false;
                                    _mainparent.SendHSG();
                                    Global.SendHSG_start_time = null;
                                }
                            }
                            else if (Global.j == 2)//处于运行状态
                            {
                                Global.ed[Global.j].stop_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                                DateTime t1 = Convert.ToDateTime(Global.ed[Global.j].start_time);
                                DateTime t2 = Convert.ToDateTime(Global.ed[Global.j].stop_time);
                                string ts = (t2 - t1).TotalMinutes.ToString("0.00");
                                string InsertOEEStr = "insert into OEE_DT([DateTime],[ErrorCode],[EventTime],[ModuleCode],[RunStatus],[ErrorInfo],[TimeSpan])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "'" + "," + "'" + Global.ed[Global.j].errorCode + "'" + "," + "'" + Global.ed[Global.j].start_time + "'" + ","
                                                   + "'" + "" + "'" + "," + "'" + Global.ed[Global.j].errorStatus + "'" + "," + "'" + Global.ed[Global.j].errorinfo + "'" + "," + "'" + ts + "'" + ")";
                                SQL.ExecuteUpdate(InsertOEEStr);
                                Log.WriteCSV(DateTime.Now.ToString("HH:mm:ss") + "," + Global.ed[Global.j].errorCode + "," + Global.ed[Global.j].start_time + "," + "自动发送成功" + "," + Global.ed[Global.j].errorStatus + "," + Global.ed[Global.j].errorinfo + "," + ts, System.AppDomain.CurrentDomain.BaseDirectory + "\\系统配置\\System_ini\\");
                            }
                            else if (Global.j == 3)
                            {
                                Global.ed[Global.Error_num + 1].stop_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                                DateTime t1 = Convert.ToDateTime(Global.ed[Global.Error_num + 1].start_time);
                                DateTime t2 = Convert.ToDateTime(Global.ed[Global.Error_num + 1].stop_time);
                                string ts = (t2 - t1).TotalMinutes.ToString("0.00");
                                if (Global.Error_num == 9 || Global.Error_num == 10 || Global.Error_num == 11 || Global.Error_num == 12)//机台打开安全门
                                {
                                    string OEE_DT3 = "";
                                    Guid guid = Guid.NewGuid();
                                    OEE_DT3 = string.Format("{{\"guid\":\"{0}\",\"EMT\":\"{1}\",\"PoorNum\":\"{2}\",\"TotalNum\":\"{3}\",  \"Status\":\"{4}\",\"ErrorCode\":\"{5}\",\"EventTime\":\"{6}\",\"ModuleCode\":\"{7}\",\"ClientPcName\":\"{8}\",\"MAC\":\"{9}\",\"IP\":\"{10}\"}}", guid, Global.inidata.productconfig.EMT, "0", "0", Global.errorStatus, Global.errorcode, Global.ed[Global.Error_num + 1].start_time, "", ClientPcName, Mac, IP);
                                    Log.WriteLog("OEE_DT安全门打开:" + OEE_DT3 + ",OEELog");
                                    //Global._mqttClient.Publish(sendTopic, Encoding.UTF8.GetBytes(OEE_DT3), MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE, false);
                                    //bool rst3 = _mainparent.SendMqttResult(guid);
                                    //if (rst3)
                                    //{
                                    //    if (Global.respond[guid].Result == "OK")
                                    //    {
                                    //        Global.ConnectOEEFlag = true;
                                    //        Log.WriteLog("OEE_DT安全门打开自动errorCode发送成功" + ",OEELog");
                                    //        _mainparent._homefrm.AppendRichText(Global.errorcode + ",触发时间=" + Global.ed[Global.Error_num + 1].start_time + ",运行状态:" + Global.errorStatus + ",故障描述:" + Global.errorinfo + ",安全门打开自动发送成功", "rtx_DownTimeMsg");
                                    //        Global.respond.TryRemove(guid, out Global.Respond);
                                    //    }
                                    //    else
                                    //    {
                                    //        Global.ConnectOEEFlag = false;
                                    //        Log.WriteLog("OEE_DT安全门打开自动errorCode发送失败" + ",OEELog");
                                    //        Log.WriteLog(Global.respond[guid].GUID.ToString() + "," + Global.respond[guid].Result + "," + Global.respond[guid].ErrorCode, System.AppDomain.CurrentDomain.BaseDirectory + "\\MQTT返回NG\\");
                                    //        _mainparent._homefrm.AppendRichText(Global.errorcode + ",触发时间=" + Global.ed[Global.Error_num + 1].start_time + ",运行状态:" + Global.errorStatus + ",故障描述:" + Global.errorinfo + ",安全门打开自动发送失败", "rtx_DownTimeMsg");
                                    //        _mainparent._homefrm.AppendRichText("失败原因:" + Global.respond[guid].ErrorCode + Global.respond[guid].Result, "rtx_DownTimeMsg");
                                    //        Global.respond.TryRemove(guid, out Global.Respond);
                                    //    }
                                    //}
                                    //else
                                    //{
                                    //    Global.ConnectOEEFlag = false;
                                    //    Log.WriteLog("OEE_DT安全门打开自动errorCode发送失败,超时无反馈:" + ",OEELog");
                                    //    _mainparent._homefrm.AppendRichText(Global.errorcode + ",触发时间=" + Global.ed[Global.Error_num + 1].start_time + ",运行状态:" + Global.errorStatus + ",故障描述:" + Global.errorinfo + ",安全门打开自动发送失败", "rtx_DownTimeMsg");
                                    //    _mainparent._homefrm.AppendRichText("网络异常,超时无反馈", "rtx_DownTimeMsg");
                                    //    string s = "insert into OEE_DTSendNG([DateTime],[Product],[GUID],[EMT],[PoorNum],[TotalNum],[Status],[ErrorCode],[ModuleCode],[ClientPcName],[MAC],[IP],[EventTime],[Moduleinfo],[errorinfo])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," + "'" + "OEE_DT" + "'" + "," + "'" + guid + "'" + "," + "'" + Global.inidata.productconfig.EMT + "'" + "," + "'" + "0" + "'" + "," + "'" + "0" + "'" + "," + "'" + Global.errorStatus + "'" + "," + "'" + Global.errorcode + "'" + ","
                                    //           + "'" + "" + "'" + "," + "'" + ClientPcName + "'" + "," + "'" + Mac + "'" + "," + "'" + IP + "'" + "," + "'" + Global.ed[Global.Error_num + 1].start_time + "'" + "," + "'" + "" + "'" + "," + "'" + Global.errorinfo + "'" + ")";
                                    //    int r = SQL.ExecuteUpdate(s);
                                    //    Log.WriteLog(string.Format("插入了{0}行OEE_DownTime缓存数据", r) + ",OEELog");
                                    //}
                                    labelcolor(Color.Transparent, "未选择", "LB_ManualSelect");
                                    string InsertOEEStr = "insert into OEE_DT([DateTime],[ErrorCode],[EventTime],[ModuleCode],[RunStatus],[ErrorInfo],[TimeSpan])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "'" + "," + "'" + Global.errorcode + "'" + "," + "'" + Global.ed[Global.Error_num + 1].start_time + "'" + ","
                                   + "'" + "" + "'" + "," + "'" + Global.errorStatus + "'" + "," + "'" + Global.errorinfo + "'" + "," + "'" + ts + "'" + ")";
                                    SQL.ExecuteUpdate(InsertOEEStr);
                                    InsertOEEStr = "insert into OEE_TraceDT([DateTime],[Status],[ErrorCode],[EventTime],[ModuleCode])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," + "'" + Global.errorStatus + "'" + "," + "'" + Global.errorcode + "'" + ","
                                            + "'" + Global.ed[Global.Error_num + 1].start_time + "'" + "," + "'" + Global.ed[Global.Error_num + 1].ModuleCode + "'" + ")";
                                    SQL.ExecuteUpdate(InsertOEEStr);
                                    Log.WriteLog(DateTime.Now.ToString("HH:mm:ss") + "," + Global.errorcode + "," + Global.ed[Global.Error_num + 1].start_time + "," + "" + "," + "自动发送成功" + "," + Global.errorStatus + "," + Global.errorinfo + "," + ts, System.AppDomain.CurrentDomain.BaseDirectory + "\\系统配置\\System_ini\\");
                                }
                                else//机台处于其它异常状态中
                                {
                                    string InsertOEEStr = "insert into OEE_DT([DateTime],[ErrorCode],[EventTime],[ModuleCode],[RunStatus],[ErrorInfo],[TimeSpan])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "'" + "," + "'" + Global.ed[Global.Error_num + 1].errorCode + "'" + "," + "'" + Global.ed[Global.Error_num + 1].start_time + "'" + ","
                                                   + "'" + Global.ed[Global.Error_num + 1].ModuleCode + "'" + "," + "'" + Global.ed[Global.Error_num + 1].errorStatus + "'" + "," + "'" + Global.ed[Global.Error_num + 1].errorinfo + "'" + "," + "'" + ts + "'" + ")";
                                    SQL.ExecuteUpdate(InsertOEEStr);
                                    Log.WriteLog(DateTime.Now.ToString("HH:mm:ss") + "," + Global.ed[Global.Error_num + 1].errorCode + "," + Global.ed[Global.Error_num + 1].start_time + "," + "自动发送成功" + "," + Global.ed[Global.Error_num + 1].errorStatus + "," + Global.ed[Global.Error_num + 1].errorinfo + "," + ts, System.AppDomain.CurrentDomain.BaseDirectory + "\\系统配置\\System_ini\\");
                                }
                            }
                            else if (Global.j == 4)//处于人工停止状态
                            {
                                Global.ed[Global.Error_Stopnum + 1].stop_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                                string c = "c=UPLOAD_DOWNTIME&tsn=Test_station&mn=Machine#1&start_time=" + Global.ed[Global.Error_Stopnum + 1].start_time + "&stop_time=" + Global.ed[Global.Error_Stopnum + 1].stop_time + "&ec=" + Global.ed[Global.Error_Stopnum + 1].errorCode;
                                Log.WriteLog(c + ",OEELog");
                                if (Global.ed[Global.Error_Stopnum + 1].start_time != null)
                                {
                                    DateTime t1 = Convert.ToDateTime(Global.ed[Global.Error_Stopnum + 1].start_time);
                                    DateTime t2 = Convert.ToDateTime(Global.ed[Global.Error_Stopnum + 1].stop_time);
                                    string ts = (t2 - t1).TotalMinutes.ToString("0.00");
                                    if (Global.Error_Stopnum == 9 || Global.Error_Stopnum == 10 || Global.Error_Stopnum == 11 || Global.Error_Stopnum == 12 || Global.SelectManualErrorCode)//机台打开安全门或者手动选择ErrorCode状态开启
                                    {
                                        string OEE_DT4 = "";
                                        Global.Art_stop = true;
                                        Guid guid = Guid.NewGuid();
                                        OEE_DT4 = string.Format("{{\"guid\":\"{0}\",\"EMT\":\"{1}\",\"PoorNum\":\"{2}\",\"TotalNum\":\"{3}\",  \"Status\":\"{4}\",\"ErrorCode\":\"{5}\",\"EventTime\":\"{6}\",\"ModuleCode\":\"{7}\",\"ClientPcName\":\"{8}\",\"MAC\":\"{9}\",\"IP\":\"{10}\"}}", guid, Global.inidata.productconfig.EMT, "0", "0", Global.errorStatus, Global.errorcode, Global.ed[Global.Error_Stopnum + 1].start_time, "", ClientPcName, Mac, IP);
                                        Log.WriteLog("OEE_DT安全门打开:" + OEE_DT4 + ",OEELog");
                                        //Global._mqttClient.Publish(sendTopic, Encoding.UTF8.GetBytes(OEE_DT4), MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE, false);
                                        //bool rst4 = _mainparent.SendMqttResult(guid);
                                        //if (rst4)
                                        //{
                                        //    if (Global.respond[guid].Result == "OK")
                                        //    {
                                        //        Global.ConnectOEEFlag = true;
                                        //        Log.WriteLog("OEE_DT安全门打开自动errorCode发送成功" + ",OEELog");
                                        //        _mainparent._homefrm.AppendRichText(Global.errorcode + ",触发时间=" + Global.ed[Global.Error_Stopnum + 1].start_time + ",运行状态:" + Global.errorStatus + ",故障描述:" + Global.errorinfo + ",安全门打开自动发送成功", "rtx_DownTimeMsg");
                                        //        Global.respond.TryRemove(guid, out Global.Respond);
                                        //    }
                                        //    else
                                        //    {
                                        //        Global.ConnectOEEFlag = false;
                                        //        Log.WriteLog("OEE_DT安全门打开自动errorCode发送失败" + ",OEELog");
                                        //        Log.WriteLog(Global.respond[guid].GUID.ToString() + "," + Global.respond[guid].Result + "," + Global.respond[guid].ErrorCode, System.AppDomain.CurrentDomain.BaseDirectory + "\\MQTT返回NG\\");
                                        //        _mainparent._homefrm.AppendRichText(Global.errorcode + ",触发时间=" + Global.ed[Global.Error_Stopnum + 1].start_time + ",运行状态:" + Global.errorStatus + ",故障描述:" + Global.errorinfo + ",安全门打开自动发送失败", "rtx_DownTimeMsg");
                                        //        _mainparent._homefrm.AppendRichText("失败原因:" + Global.respond[guid].ErrorCode + Global.respond[guid].Result, "rtx_DownTimeMsg");
                                        //        Global.respond.TryRemove(guid, out Global.Respond);
                                        //    }
                                        //}
                                        //else
                                        //{
                                        //    Global.ConnectOEEFlag = false;
                                        //    Log.WriteLog("OEE_DT人工停止复位自动errorCode发送失败,超时无反馈:" + ",OEELog");
                                        //    _mainparent._homefrm.AppendRichText(Global.errorcode + ",触发时间=" + Global.ed[Global.Error_Stopnum + 1].start_time + ",运行状态:" + Global.errorStatus + ",故障描述:" + Global.errorinfo + ",安全门打开自动发送失败", "rtx_DownTimeMsg");
                                        //    _mainparent._homefrm.AppendRichText("网络异常,超时无反馈", "rtx_DownTimeMsg");
                                        //    string s = "insert into OEE_DTSendNG([DateTime],[Product],[GUID],[EMT],[PoorNum],[TotalNum],[Status],[ErrorCode],[ModuleCode],[ClientPcName],[MAC],[IP],[EventTime],[Moduleinfo],[errorinfo])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," + "'" + "OEE_DT" + "'" + "," + "'" + guid + "'" + "," + "'" + Global.inidata.productconfig.EMT + "'" + "," + "'" + "0" + "'" + "," + "'" + "0" + "'" + "," + "'" + Global.errorStatus + "'" + "," + "'" + Global.errorcode + "'" + ","
                                        //           + "'" + "" + "'" + "," + "'" + ClientPcName + "'" + "," + "'" + Mac + "'" + "," + "'" + IP + "'" + "," + "'" + Global.ed[Global.Error_Stopnum + 1].start_time + "'" + "," + "'" + "" + "'" + "," + "'" + Global.errorinfo + "'" + ")";
                                        //    int r = SQL.ExecuteUpdate(s);
                                        //    Log.WriteLog(string.Format("插入了{0}行OEE_DownTime缓存数据", r) + ",OEELog");
                                        //}
                                        labelcolor(Color.Transparent, "未选择", "LB_ManualSelect");
                                        string InsertOEEStr = "insert into OEE_DT([DateTime],[ErrorCode],[EventTime],[ModuleCode],[RunStatus],[ErrorInfo],[TimeSpan])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "'" + "," + "'" + Global.errorcode + "'" + "," + "'" + Global.ed[Global.Error_Stopnum + 1].start_time + "'" + ","
                                       + "'" + "" + "'" + "," + "'" + Global.errorStatus + "'" + "," + "'" + Global.errorinfo + "'" + "," + "'" + ts + "'" + ")";
                                        SQL.ExecuteUpdate(InsertOEEStr);
                                        InsertOEEStr = "insert into OEE_TraceDT([DateTime],[Status],[ErrorCode],[EventTime],[ModuleCode])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," + "'" + Global.errorStatus + "'" + "," + "'" + Global.errorcode + "'" + ","
                                                + "'" + Global.ed[Global.Error_Stopnum + 1].start_time + "'" + "," + "'" + Global.ed[Global.Error_Stopnum + 1].ModuleCode + "'" + ")";
                                        SQL.ExecuteUpdate(InsertOEEStr);
                                        Log.WriteLog(DateTime.Now.ToString("HH:mm:ss") + "," + Global.errorcode + "," + Global.ed[Global.Error_Stopnum + 1].start_time + "," + "" + "," + "自动发送成功" + "," + Global.errorStatus + "," + Global.errorinfo + "," + ts, System.AppDomain.CurrentDomain.BaseDirectory + "\\系统配置\\System_ini\\");
                                    }
                                    else if (Global.Error_Stopnum == 278)//机台人工停止
                                    {
                                        string OEE_DT4 = "";
                                        Guid guid = Guid.NewGuid();
                                        OEE_DT4 = string.Format("{{\"guid\":\"{0}\",\"EMT\":\"{1}\",\"PoorNum\":\"{2}\",\"TotalNum\":\"{3}\",  \"Status\":\"{4}\",\"ErrorCode\":\"{5}\",\"EventTime\":\"{6}\",\"ModuleCode\":\"{7}\",\"ClientPcName\":\"{8}\",\"MAC\":\"{9}\",\"IP\":\"{10}\"}}", guid, Global.inidata.productconfig.EMT, "0", "0", Global.ed[Global.Error_Stopnum + 1].errorStatus, Global.ed[Global.Error_Stopnum + 1].errorCode, Global.ed[Global.Error_Stopnum + 1].start_time, Global.ed[Global.Error_Stopnum + 1].ModuleCode, ClientPcName, Mac, IP);
                                        Log.WriteLog("OEE_DT人工停止复位:" + OEE_DT4 + ",OEELog");
                                        //Global._mqttClient.Publish(sendTopic, Encoding.UTF8.GetBytes(OEE_DT4), MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE, false);
                                        //bool rst4 = _mainparent.SendMqttResult(guid);
                                        //if (rst4)
                                        //{
                                        //    if (Global.respond[guid].Result == "OK")
                                        //    {
                                        //        Global.ConnectOEEFlag = true;
                                        //        Log.WriteLog("OEE_DT人工停止复位自动errorCode发送成功" + ",OEELog");
                                        //        _mainparent._homefrm.AppendRichText(Global.ed[Global.Error_Stopnum + 1].errorCode + ",触发时间=" + Global.ed[Global.Error_Stopnum + 1].start_time + ",运行状态:" + Global.ed[Global.Error_Stopnum + 1].errorStatus + ",故障描述:" + Global.ed[Global.Error_Stopnum + 1].errorinfo + ",安全门打开自动发送成功", "rtx_DownTimeMsg");
                                        //        Global.respond.TryRemove(guid, out Global.Respond);
                                        //    }
                                        //    else
                                        //    {
                                        //        Global.ConnectOEEFlag = false;
                                        //        Log.WriteLog("OEE_DT人工停止复位自动errorCode发送失败" + ",OEELog");
                                        //        Log.WriteLog(Global.respond[guid].GUID.ToString() + "," + Global.respond[guid].Result + "," + Global.respond[guid].ErrorCode, System.AppDomain.CurrentDomain.BaseDirectory + "\\MQTT返回NG\\");
                                        //        _mainparent._homefrm.AppendRichText(Global.ed[Global.Error_Stopnum + 1].errorCode + ",触发时间=" + Global.ed[Global.Error_Stopnum + 1].start_time + ",运行状态:" + Global.ed[Global.Error_Stopnum + 1].errorStatus + ",故障描述:" + Global.ed[Global.Error_Stopnum + 1].errorinfo + ",安全门打开自动发送失败", "rtx_DownTimeMsg");
                                        //        _mainparent._homefrm.AppendRichText("失败原因:" + Global.respond[guid].ErrorCode + Global.respond[guid].Result, "rtx_DownTimeMsg");
                                        //        Global.respond.TryRemove(guid, out Global.Respond);
                                        //    }
                                        //}
                                        //else
                                        //{
                                        //    Global.ConnectOEEFlag = false;
                                        //    Log.WriteLog("OEE_DT人工停止复位自动errorCode发送失败,超时无反馈:" + ",OEELog");
                                        //    _mainparent._homefrm.AppendRichText(Global.ed[Global.Error_Stopnum + 1].errorCode + ",触发时间=" + Global.ed[Global.Error_Stopnum + 1].start_time + ",运行状态:" + Global.ed[Global.Error_Stopnum + 1].errorStatus + ",故障描述:" + Global.ed[Global.Error_Stopnum + 1].errorinfo + ",安全门打开自动发送失败", "rtx_DownTimeMsg");
                                        //    _mainparent._homefrm.AppendRichText("网络异常,超时无反馈", "rtx_DownTimeMsg");
                                        //    string s = "insert into OEE_DTSendNG([DateTime],[Product],[GUID],[EMT],[PoorNum],[TotalNum],[Status],[ErrorCode],[ModuleCode],[ClientPcName],[MAC],[IP],[EventTime],[Moduleinfo],[errorinfo])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," + "'" + "OEE_DT" + "'" + "," + "'" + guid + "'" + "," + "'" + Global.inidata.productconfig.EMT + "'" + "," + "'" + "0" + "'" + "," + "'" + "0" + "'" + "," + "'" + Global.ed[Global.Error_Stopnum + 1].errorStatus + "'" + "," + "'" + Global.ed[Global.Error_Stopnum + 1].errorCode + "'" + ","
                                        //           + "'" + Global.ed[Global.Error_Stopnum + 1].ModuleCode + "'" + "," + "'" + ClientPcName + "'" + "," + "'" + Mac + "'" + "," + "'" + IP + "'" + "," + "'" + Global.ed[Global.Error_Stopnum + 1].start_time + "'" + "," + "'" + Global.ed[Global.Error_Stopnum + 1].Moduleinfo + "'" + "," + "'" + Global.ed[Global.Error_Stopnum + 1].errorinfo + "'" + ")";
                                        //    int r = SQL.ExecuteUpdate(s);
                                        //    Log.WriteLog(string.Format("插入了{0}行OEE_DownTime缓存数据", r) + ",OEELog");
                                        //}
                                        labelcolor(Color.Transparent, "未选择", "LB_ManualSelect");
                                        string InsertOEEStr = "insert into OEE_DT([DateTime],[ErrorCode],[EventTime],[ModuleCode],[RunStatus],[ErrorInfo],[TimeSpan])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "'" + "," + "'" + Global.ed[Global.Error_Stopnum + 1].errorCode + "'" + "," + "'" + Global.ed[Global.Error_Stopnum + 1].start_time + "'" + ","
                                       + "'" + "" + "'" + "," + "'" + Global.ed[Global.Error_Stopnum + 1].errorStatus + "'" + "," + "'" + Global.ed[Global.Error_Stopnum + 1].errorinfo + "'" + "," + "'" + ts + "'" + ")";
                                        SQL.ExecuteUpdate(InsertOEEStr);
                                        InsertOEEStr = "insert into OEE_TraceDT([DateTime],[Status],[ErrorCode],[EventTime],[ModuleCode])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," + "'" + Global.ed[Global.Error_Stopnum + 1].errorStatus + "'" + "," + "'" + Global.ed[Global.Error_Stopnum + 1].errorCode + "'" + ","
                                         + "'" + Global.ed[Global.Error_Stopnum + 1].start_time + "'" + "," + "'" + Global.ed[Global.Error_Stopnum + 1].ModuleCode + "'" + ")";
                                        SQL.ExecuteUpdate(InsertOEEStr);
                                        Log.WriteLog(DateTime.Now.ToString("HH:mm:ss") + "," + Global.ed[Global.Error_Stopnum + 1].errorCode + "," + Global.ed[Global.Error_Stopnum + 1].start_time + "," + "" + "," + "自动发送成功" + "," + Global.ed[Global.Error_Stopnum + 1].errorStatus + "," + Global.ed[Global.Error_Stopnum + 1].errorinfo + "," + ts, System.AppDomain.CurrentDomain.BaseDirectory + "\\系统配置\\System_ini\\");
                                    }
                                    else//机台处于其它异常状态中
                                    {
                                        Log.WriteLog("PLC人工停止ErrorCode异常" + Global.Error_Stopnum + ",OEELog");
                                    }
                                }
                                Log.WriteLog(Global.ed[Global.Error_Stopnum + 1].Moduleinfo + "_" + Global.ed[Global.Error_Stopnum + 1].errorinfo + "：结束计时 " + Global.ed[Global.Error_Stopnum + 1].stop_time + ",OEELog");
                                //Global.ed[Global.Error_Stopnum + 1].start_time = null;
                                //Global.ed[Global.Error_Stopnum + 1].stop_time = null;                            
                            }
                        }
                        else
                        {
                            if (Global.SelectTestRunModel == true && Global.ed[313].start_time != null)//空运行结束写入OEE_DT数据表中
                            {
                                Global.ed[313].stop_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                                DateTime t1 = Convert.ToDateTime(Global.ed[313].start_time);
                                DateTime t2 = Convert.ToDateTime(Global.ed[313].stop_time);
                                string ts = (t2 - t1).TotalMinutes.ToString("0.00");
                                string InsertStr6 = "insert into OEE_DT([DateTime],[ErrorCode],[EventTime],[ModuleCode],[RunStatus],[ErrorInfo],[TimeSpan])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "'" + "," + "'" + Global.ed[313].errorCode + "'" + "," + "'" + Global.ed[313].start_time + "'" + ","
                                   + "'" + Global.ed[313].ModuleCode + "'" + "," + "'" + Global.ed[313].errorStatus + "'" + "," + "'" + Global.ed[313].errorinfo + "'" + "," + "'" + ts + "'" + ")";
                                SQL.ExecuteUpdate(InsertStr6);
                                Log.WriteCSV(DateTime.Now.ToString("HH:mm:ss") + "," + Global.ed[313].errorCode + "," + Global.ed[313].start_time + "," + Global.ed[313].ModuleCode + "," + "自动发送成功" + "," + Global.ed[313].errorStatus + "," + Global.ed[313].errorinfo + "," + ts, System.AppDomain.CurrentDomain.BaseDirectory + "\\系统配置\\System_ini\\");
                                //Global.ed[280].start_time = null;
                                //Global.ed[280].stop_time = null;
                            }
                            Global.SelectTestRunModel = false;
                        }
                        string OEE_DT = "";
                        Guid guid1 = Guid.NewGuid();
                        string EventTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                        string InsertStr = "insert into OEE_TraceDT([DateTime],[Status],[ErrorCode],[EventTime],[ModuleCode])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," + "'" + "7" + "'" + "," + "'" + "11010001" + "'" + ","
                                 + "'" + EventTime + "'" + "," + "'" + "" + "'" + ")";
                        SQL.ExecuteUpdate(InsertStr);
                        OEE_DT = string.Format("{{\"guid\":\"{0}\",\"EMT\":\"{1}\",\"PoorNum\":\"{2}\",\"TotalNum\":\"{3}\",  \"Status\":\"{4}\",\"ErrorCode\":\"{5}\",\"EventTime\":\"{6}\",\"ModuleCode\":\"{7}\",\"ClientPcName\":\"{8}\",\"MAC\":\"{9}\",\"IP\":\"{10}\"}}", guid1, Global.inidata.productconfig.EMT, "0", "0", "7", "11010001", EventTime, "", ClientPcName, Mac, IP);
                        Log.WriteLog("OEE_DT手动:" + OEE_DT + ",OEELog");
                        //Global._mqttClient.Publish(sendTopic, Encoding.UTF8.GetBytes(OEE_DT), MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE, false);
                        //bool rst = _mainparent.SendMqttResult(guid1);
                        //if (rst)
                        //{
                        //    if (Global.respond[guid1].Result == "OK")
                        //    {
                        //        Global.ConnectOEEFlag = true;
                        //        Log.WriteLog("OEE_DT机台吃饭休息发送成功" + ",OEELog");
                        //        Log.WriteLog(DateTime.Now.ToString("HH:mm:ss") + "," + "11010001" + "," + EventTime + "," + "手动发送成功" + "," + "7" + "," + "吃饭休息", System.AppDomain.CurrentDomain.BaseDirectory + "\\系统配置\\System_ini\\");
                        //        _mainparent._homefrm.AppendRichText("11010001" + ",触发时间=" + EventTime + ",运行状态:" + "7" + ",故障描述:" + "吃饭休息" + ",手动发送成功", "rtx_DownTimeMsg");
                        //        Global.respond.TryRemove(guid1, out Global.Respond);
                        //    }
                        //    else
                        //    {
                        //        Global.ConnectOEEFlag = false;
                        //        Log.WriteLog("OEE_DT机台吃饭休息发送成功" + ",OEELog");
                        //        Log.WriteLog(DateTime.Now.ToString("HH:mm:ss") + "," + "11010001" + "," + EventTime + "," + "手动发送失败" + "," + "7" + "," + "机台做验证做首件", System.AppDomain.CurrentDomain.BaseDirectory + "\\系统配置\\System_ini\\");
                        //        Log.WriteLog(Global.respond[guid1].GUID.ToString() + "," + Global.respond[guid1].Result + "," + Global.respond[guid1].ErrorCode, System.AppDomain.CurrentDomain.BaseDirectory + "\\MQTT返回NG\\");
                        //        _mainparent._homefrm.AppendRichText("失败原因:" + Global.respond[guid1].ErrorCode + Global.respond[guid1].Result, "rtx_DownTimeMsg");
                        //        Global.respond.TryRemove(guid1, out Global.Respond);
                        //    }
                        //}
                        //else
                        //{
                        //    Global.ConnectOEEFlag = false;
                        //    Log.WriteLog("OEE_DT机台吃饭休息发送成功" + ",OEELog");
                        //    Log.WriteLog(DateTime.Now.ToString("HH:mm:ss") + "," + "11010001" + "," + EventTime + "," + "手动发送失败" + "," + "7" + "," + "机台做验证做首件", System.AppDomain.CurrentDomain.BaseDirectory + "\\系统配置\\System_ini\\");
                        //    _mainparent._homefrm.AppendRichText("11010001" + ",触发时间=" + EventTime + ",运行状态:" + "7" + ",故障描述:" + "吃饭休息" + ",手动发送失败", "rtx_DownTimeMsg");
                        //    _mainparent._homefrm.AppendRichText("网络异常,超时无反馈", "rtx_DownTimeMsg");
                        //    string s = "insert into OEE_DTSendNG([DateTime],[Product],[GUID],[EMT],[PoorNum],[TotalNum],[Status],[ErrorCode],[ModuleCode],[ClientPcName],[MAC],[IP],[EventTime],[Moduleinfo],[errorinfo])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," + "'" + "OEE_DT" + "'" + "," + "'" + guid1 + "'" + "," + "'" + Global.inidata.productconfig.EMT + "'" + "," + "'" + "0" + "'" + "," + "'" + "0" + "'" + "," + "'" + "7" + "'" + "," + "'" + "11010001" + "'" + ","
                        //           + "'" + "" + "'" + "," + "'" + ClientPcName + "'" + "," + "'" + Mac + "'" + "," + "'" + IP + "'" + "," + "'" + EventTime + "'" + "," + "'" + "" + "'" + "," + "'" + "吃饭休息" + "'" + ")";
                        //    int r = SQL.ExecuteUpdate(s);
                        //    Log.WriteLog(string.Format("插入了{0}行OEE_DownTime缓存数据", r));
                        //}
                        Global.errorTime1 = true;
                        Global.errordata.start_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                        Global.errordata.errorStatus = "7";
                        Global.errordata.errorCode = "11010001";
                        Global.errordata.errorinfo = "计划停机";
                        Btn_planStopEnd.Enabled = true;
                        lb_EatandRest.Visible = true;
                        Global.SelectFirstModel = true;
                        string InsertOEEStr3 = "insert into OEE_StartTime([Status],[ErrorCode],[EventTime],[ModuleCode],[Name])" + " " + "values(" + "'" + "7" + "'" + "," + "'" + "11010001" + "'" + "," + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "'" + "," + "'" + "" + "'" + "," + "'" + "吃饭休息" + "'" + ")";
                        SQL.ExecuteUpdate(InsertOEEStr3);//插入吃饭休息开始时间
                    }
                    catch (Exception ex)
                    {
                        Log.WriteLog("吃饭休息开始异常失败！" + ex.ToString().Replace("\r\n", ""));
                    }
                }
                else
                {
                    MessageBox.Show("非待料状态下无法选择吃饭休息");
                }

            }
            catch
            {

            }


        }

        public void Btn_UpLoad_EatandRest_Click(object sender, EventArgs e)
        {
            Global.EatAndRest_currentCount = 0;//定时计数清零
            Global.EatAndRest_timer.Stop();//定时器结束
            bool timeset = false;
            Global.SelectFirstModel = false;
            if (Global.errorTime1 == true)
            {
                Global.errordata.stop_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                timeset = true;
            }
            if (Global.errordata.errorCode != null && Global.errordata.start_time != null && timeset == true)
            {
                timeset = false;
                Global.errorTime1 = false;
                //txt_errortime_display.Text = "0000-00-00 00:00:00";
                string c = "c=UPLOAD_DOWNTIME&tsn=Test_station&mn=Machine#1&start_time=" + Global.errordata.start_time + "&stop_time=" + Global.errordata.stop_time + "&ec=" + Global.errordata.errorCode;
                Log.WriteLog(c);
                DateTime t1 = Convert.ToDateTime(Global.errordata.start_time);
                DateTime t2 = Convert.ToDateTime(Global.errordata.stop_time);
                string ts = (t2 - t1).TotalMinutes.ToString("0.00");
                string InsertOEEStr = "insert into OEE_DT([DateTime],[ErrorCode],[EventTime],[ModuleCode],[RunStatus],[ErrorInfo],[TimeSpan])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "'" + "," + "'" + Global.errordata.errorCode + "'" + "," + "'" + Global.errordata.start_time + "'" + ","
                                     + "'" + "" + "'" + "," + "'" + Global.errordata.errorStatus + "'" + "," + "'" + Global.errordata.errorinfo + "'" + "," + "'" + ts + "'" + ")";
                SQL.ExecuteUpdate(InsertOEEStr);
                Log.WriteCSV(DateTime.Now.ToString("HH:mm:ss") + "," + Global.errordata.errorCode + "," + Global.errordata.start_time + "," + "手动发送成功" + "," + Global.errordata.errorStatus + "," + Global.errordata.errorinfo + "," + ts, System.AppDomain.CurrentDomain.BaseDirectory + "\\系统配置\\System_ini\\");
            }
            else
            {
                Log.WriteLog("1.请选择好错误信息和时间 2.请检查网线与网址！");
                Log.WriteLog("errorCode:" + Global.errordata.errorCode + "," + "start_time:" + Global.errordata.start_time + "," + "timeset:" + timeset);
            }
            Btn_planStopEnd.BeginInvoke(new Action(() => { Btn_planStopEnd.Enabled = false; }));
            lb_EatandRest.BeginInvoke(new Action(() => { lb_EatandRest.Visible = false; }));
            Btn_Start_errortime.BeginInvoke(new Action(() => { Btn_Start_errortime.Enabled = true; }));
            Btn_planStopStart.BeginInvoke(new Action(() => { Btn_planStopStart.Enabled = true; }));
        }
    }
}
