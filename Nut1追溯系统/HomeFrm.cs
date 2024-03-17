using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Forms;
using uPLibrary.Networking.M2Mqtt.Messages;
using Vision;

namespace 卓汇数据追溯系统
{
    public partial class HomeFrm : Form
    {
        private MainFrm _mainparent;
        private ManualFrm _manualfrm;
        delegate void AddItemToListBoxDelegate(string str, string Name);
        delegate void ShowDataGridView(DataGridView dgv, DataTable dt, int index);
        delegate void DGVAutoSize(DataGridView dgv);
        delegate void ShowDataGridView1();
        delegate string GetTextbox(TextBox tb);
        delegate void ShowTextbox(TextBox tb,string value);
        delegate void ShowDataGridView2(string fixture);
        delegate void ShowLvFixtrue();

        /// <summary>
        /// 20230226Add
        /// </summary>
        /// <param name="bl"></param>
        /// <param name="Name"></param>
        /// 

        delegate void UpdateSjFixture();


        private delegate void Labelvision(string bl, string Name);
        private delegate void Labelcolor(Color color, string bl, string Name);
        
        delegate void DGV_BlackColor(DataGridView dgv, int row, int column, Color c);
        private delegate void ShowTxt(string txt, string Name);
        string LogPath = System.AppDomain.CurrentDomain.BaseDirectory + "\\日志文件\\";
        string Conn = "provider=microsoft.jet.oledb.4.0;data source=mydata.mdb;";
        UcMain ucCCD;
        SQLServer SQL = new SQLServer();
        List<Control> List_Control = new List<Control>();
        int rth_Number = 0;
        private static object _lock = new object();
        public HomeFrm(MainFrm mdiParent)
        {
            InitializeComponent();
            _mainparent = mdiParent;


            ///20230226Add
            ///
            domainUpDownFixturecount.Text = Global.inidata.productconfig.FixtureCount;
        }

        private void DGV_Refrush()
        {
            //this.dgv_SpareParts.Refresh();
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
            e.Graphics.DrawString(text, new Font("微软雅黑", 15), brush, e.Bounds, sf);


            if(text == "视觉检测")
            {
                ucCCD.Dock = DockStyle.Fill;
                tabPageCCD.Controls.Add(ucCCD);
                ucCCD.Show();
            }
        }

        private void HomeFrm_Load(object sender, EventArgs e)
        {
            //panel1.Controls.Add(_mainparent.maindis);
            //_mainparent.maindis.Dock = DockStyle.Fill;
            ShowSparePartData();
            #region 主界面显示lable
            List_Control = GetAllControls(this);//列表中添加所有窗体控件
            Global.Trace_ua_ok = Convert.ToInt32(Global.inidata.productconfig.trace_ua_ok);
            lb_TraceUAOK.Text = Global.inidata.productconfig.trace_ua_ok;
            Global.Trace_ua_ng = Convert.ToInt32(Global.inidata.productconfig.trace_ua_ng);          
            lb_TraceUANG.Text = Global.inidata.productconfig.trace_ua_ng;

            Global.Trace_ua2_ok = Convert.ToInt32(Global.inidata.productconfig.trace_ua2_ok);
            lb_TraceLAOK.Text = Global.inidata.productconfig.trace_ua2_ok;
            Global.Trace_ua2_ng = Convert.ToInt32(Global.inidata.productconfig.trace_ua2_ng);
            lb_TraceLANG.Text = Global.inidata.productconfig.trace_ua2_ng;


            Global.Product_num_ua_ok = Convert.ToInt32(Global.inidata.productconfig.product_num_ua_ok);
            lb_PDCAUAOK.Text = Global.inidata.productconfig.product_num_ua_ok;
            Global.Product_num_ua_ng = Convert.ToInt32(Global.inidata.productconfig.product_num_ua_ng);
            lb_PDCAUANG.Text = Global.inidata.productconfig.product_num_ua_ng;
            Global.oee_ok = Convert.ToInt32(Global.inidata.productconfig.oee_ok);
            lb_OEEOK.Text = Global.inidata.productconfig.oee_ok;
            Global.oee_ng = Convert.ToInt32(Global.inidata.productconfig.oee_ng);
            lb_OEENG.Text = Global.inidata.productconfig.oee_ng;
            Global.ThrowCount = Convert.ToInt32(Global.inidata.productconfig.ThrowCount);
            lb_Materiel_AllNut.Text = Global.inidata.productconfig.ThrowCount;
            Global.ThrowOKCount = Convert.ToInt32(Global.inidata.productconfig.ThrowOKCount);
            lb_Materiel_AllOK.Text = Global.inidata.productconfig.ThrowOKCount;
            Global.TotalThrowCount = Convert.ToInt32(Global.inidata.productconfig.TotalThrowCount);
            lb_Materiel_Total.Text = Global.inidata.productconfig.TotalThrowCount;
            Global.NutCount = Convert.ToInt32(Global.inidata.productconfig.NutCount);
            lb_Materiel_Nut.Text = Global.inidata.productconfig.NutCount;
            Global.NutOKCount = Convert.ToInt32(Global.inidata.productconfig.NutOKCount);
            lb_Materiel_OK.Text = Global.inidata.productconfig.NutOKCount;
            Global.Trace_process_ok = Convert.ToInt32(Global.inidata.productconfig.trace_process_ok);
            lb_ProcessControlOK.Text = Global.inidata.productconfig.trace_process_ok;
            Global.Trace_process_ng = Convert.ToInt32(Global.inidata.productconfig.trace_process_ng);
            lb_ProcessControlNG.Text = Global.inidata.productconfig.trace_process_ng;
            #endregion


            #region 小料抛料数据加载
            //加载setting中小料抛料数据
            Global.location1_CCDNG_D = Convert.ToInt32(Global.inidata.productconfig.location1_CCDNG_D);
            Global.location1_CCDNG_N = Convert.ToInt32(Global.inidata.productconfig.location1_CCDNG_N);
            Global.location2_CCDNG_D = Convert.ToInt32(Global.inidata.productconfig.location2_CCDNG_D);
            Global.location2_CCDNG_N = Convert.ToInt32(Global.inidata.productconfig.location2_CCDNG_N);
            Global.location3_CCDNG_D = Convert.ToInt32(Global.inidata.productconfig.location3_CCDNG_D);
            Global.location3_CCDNG_N = Convert.ToInt32(Global.inidata.productconfig.location3_CCDNG_N);
            Global.HansDataError_D = Convert.ToInt32(Global.inidata.productconfig.HansDataError_D);
            Global.HansDataError_N = Convert.ToInt32(Global.inidata.productconfig.HansDataError_N);
            Global.TraceUpLoad_Error_D = Convert.ToInt32(Global.inidata.productconfig.TraceUpLoad_Error_D);
            Global.TraceUpLoad_Error_N = Convert.ToInt32(Global.inidata.productconfig.TraceUpLoad_Error_N);
            Global.PDCAUpLoad_Error_D = Convert.ToInt32(Global.inidata.productconfig.PDCAUpLoad_Error_D);
            Global.PDCAUpLoad_Error_N = Convert.ToInt32(Global.inidata.productconfig.PDCAUpLoad_Error_N);
            Global.TracePVCheck_Error_D = Convert.ToInt32(Global.inidata.productconfig.TracePVCheck_Error_D);
            Global.TracePVCheck_Error_N = Convert.ToInt32(Global.inidata.productconfig.TracePVCheck_Error_N);

            Global.TraceTab_Error_D = Convert.ToInt32(Global.inidata.productconfig.TraceTab_Error_D);
            Global.TraceTab_Error_N = Convert.ToInt32(Global.inidata.productconfig.TraceTab_Error_N);
            Global.TraceThench_Error_D = Convert.ToInt32(Global.inidata.productconfig.TraceThench_Error_D);
            Global.TraceThench_Error_N = Convert.ToInt32(Global.inidata.productconfig.TraceThench_Error_N);

            Global.itm_DT = Global.inidata.productconfig.Itm_DT;

            //关机后setting中的抛料数据作为上一片料数据
            //PFerroecode
            Global.itm_TracePVCheck_Error_D = Global.inidata.productconfig.TracePVCheck_Error_D;
            Global.itm_TracePVCheck_Error_N = Global.inidata.productconfig.TracePVCheck_Error_N;
            Global.itm_ReadBarcode_NG_D = Global.inidata.productconfig.ReadBarcode_NG_D;
            Global.itm_ReadBarcode_NG_N = Global.inidata.productconfig.ReadBarcode_NG_N;
            Global.itm_CCDCheck_Error_D = Global.inidata.productconfig.CCDCheck_Error_D;
            Global.itm_CCDCheck_Error_N = Global.inidata.productconfig.CCDCheck_Error_N;
            Global.itm_TraceUpLoad_Error_D = Global.inidata.productconfig.TraceUpLoad_Error_D;
            Global.itm_TraceUpLoad_Error_N = Global.inidata.productconfig.TraceUpLoad_Error_N;
            Global.itm_PDCAUpLoad_Error_D = Global.inidata.productconfig.PDCAUpLoad_Error_D;
            Global.itm_PDCAUpLoad_Error_N = Global.inidata.productconfig.PDCAUpLoad_Error_N;
            Global.itm_Smallmaterial_throwing_D = Global.inidata.productconfig.Smallmaterial_throwing_D;
            Global.itm_Smallmaterial_throwing_N = Global.inidata.productconfig.Smallmaterial_throwing_N;
            Global.itm_Band_NG_D = Global.inidata.productconfig.Band_NG_D;
            Global.itm_Band_NG_N = Global.inidata.productconfig.Band_NG_N;

            Global.itm_TraceTab_Error_D = Global.inidata.productconfig.TraceTab_Error_D;
            Global.itm_TraceTab_Error_N = Global.inidata.productconfig.TraceTab_Error_N;
            Global.itm_TraceThench_Error_D = Global.inidata.productconfig.TraceThench_Error_D;
            Global.itm_TraceThench_Error_N = Global.inidata.productconfig.TraceThench_Error_N;
            //1st Yield errorcode
            Global.itm_location1_CCDNG_D = Global.inidata.productconfig.location1_CCDNG_D;
            Global.itm_location1_CCDNG_N = Global.inidata.productconfig.location1_CCDNG_N;
            Global.itm_location2_CCDNG_D = Global.inidata.productconfig.location2_CCDNG_D;
            Global.itm_location2_CCDNG_N = Global.inidata.productconfig.location2_CCDNG_N;
            Global.itm_location3_CCDNG_D = Global.inidata.productconfig.location3_CCDNG_D;
            Global.itm_location3_CCDNG_N = Global.inidata.productconfig.location3_CCDNG_N;
            Global.itm_HansDataError_D = Global.inidata.productconfig.HansDataError_D;
            Global.itm_HansDataError_N = Global.inidata.productconfig.HansDataError_N;
            #endregion
            //显示治具保养方案依据 1依照保养时间2依照使用次数
            if (Global.inidata.productconfig.fixtruetime == "1")
            {
                rb_fixtruetime.Checked = true;
            }
            if (Global.inidata.productconfig.fixtruetimes == "1")
            {
                rb_fixturetimes.Checked = true;
            }            

            //通过执行文件生成软件版本号
            string filePath = System.AppDomain.CurrentDomain.BaseDirectory + "卓汇数据追溯系统.exe";
            System.Text.StringBuilder builder = new System.Text.StringBuilder();
            if (filePath != "")
            {
                using (var md5 = new System.Security.Cryptography.MD5CryptoServiceProvider())
                {
                    System.IO.File.Copy(filePath, filePath + "e",true);//复制一份，防止占\

                                                                  //利用复制的执行档建立MD5码
                    using (System.IO.FileStream fs = new System.IO.FileStream(filePath + "e", FileMode.Open))
                    {
                        byte[] bt = md5.ComputeHash(fs);
                        for (int i = 0; i < bt.Length; i++)
                        {
                            builder.Append(bt[i].ToString("x2"));
                        }
                    }
                    System.IO.File.Delete(filePath + "e");//删除复制的文件，这里没处理异常.
                }
            }
            Global.inidata.productconfig.Sw_version = builder.ToString();
            Global.inidata.WriteProductConfigSection();
            #region 治具保养
            //将待保养治具写入List中
            Global._fixture_ng = Txt.AddFixture_ng();
            //将待维修治具写入List中
            Global._fixture_tossing_ng = Txt.AddFixture_ng1();

            //治具小保养导入参数
            grid_AllFixture.Columns.Clear();
            grid_AllFixture.AutoGenerateColumns = true; ;
            string[] fixture = new string[] { };
            string SelectStr = "SELECT [ID],[FixtureID],[Time],[Complete],[UsingTimes],[Status] FROM FixtureStatus";//sql查询语句
            DataTable d1 = SQL.ExecuteQuery(SelectStr);
            for (int i = 0; i < d1.Rows.Count; i++)
            {
                Global._fixture.Add(d1.Rows[i][1].ToString());
            }

            ///20230226Add
            UpdateSjFixtureCount();

            d1.Columns["ID"].ColumnName = "序号";
            d1.Columns["FixtureID"].ColumnName = "治具码";
            ///20230226Add
            d1.Columns["Time"].ColumnName = "排出时间";
            d1.Columns["Complete"].ColumnName = "保养完成时间";
            d1.Columns["UsingTimes"].ColumnName = "使用次数";
            d1.Columns["Status"].ColumnName = "状态";
            grid_AllFixture.DataSource = d1;
            dgv_AutoSize(grid_AllFixture);
            grid_AllFixture.AutoGenerateColumns = false;

            //治具抛料次数导入
            DataGridViewButtonColumn btn = new DataGridViewButtonColumn();
            btn.Name = "BtnModify";
            btn.HeaderText = "维修确认";
            btn.DefaultCellStyle.NullValue = "确认";
            btn.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            string SelectStr1 = "SELECT [Fixture],[TossingTime],[TossingContinuation],[TossingCount],[ContinuationNG],[CountNG] FROM FixtureTossing";//sql查询语句
            DataTable d2 = SQL.ExecuteQuery(SelectStr1);
            d2.Columns["Fixture"].ColumnName = "治具码";
            d2.Columns["TossingTime"].ColumnName = "抛料时间";
            d2.Columns["TossingContinuation"].ColumnName = "连续抛料次数(Max 3)";
            d2.Columns["TossingCount"].ColumnName = "抛料次数(Max 5)";
            d2.Columns["ContinuationNG"].ColumnName = "连续NG";
            d2.Columns["CountNG"].ColumnName = "次数NG";
            grid_TossingFixture.DataSource = d2;
            grid_TossingFixture.Columns.Add(btn);
            dgv_AutoSize(grid_TossingFixture);
            grid_TossingFixture.AutoGenerateColumns = false;

            //配件管控            
            DataGridViewButtonColumn btn1 = new DataGridViewButtonColumn();
            btn1.Name = "BtnModify";
            btn1.HeaderText = "更换确认";
            btn1.DefaultCellStyle.NullValue = "确认";
            btn1.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

            dgv_Spare_parts_control.Columns.Add(btn1);
            dgv_AutoSize(dgv_Spare_parts_control);
            dgv_Spare_parts_control.AutoGenerateColumns = false;

            #endregion


            //CCD
            ucCCD = new UcMain(Global.PLC_Client);


            Thread.Sleep(300);
        }

        private void dgv_Dispaly_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            //int i, j; float k = 0.00F; j = dgv_Dispaly.Rows.Count;
            //for (i = 0; i < j; i++)
            //{
            //    k = float.Parse(dgv_Dispaly.Rows[i].Cells["部件寿命残值"].Value.ToString());
            //    if ((k < 0.7) && (k > 0.3))
            //    {
            //        dgv_Dispaly.Rows[i].DefaultCellStyle.BackColor = Color.Orange;
            //    }
            //    else if (k >= 0.7)
            //    {
            //        dgv_Dispaly.Rows[i].DefaultCellStyle.BackColor = Color.LightGreen;
            //    }
            //    else if (k <= 0.3)
            //    {
            //        dgv_Dispaly.Rows[i].DefaultCellStyle.BackColor = Color.Red;
            //    }
            //}
        }

        public void ShowSparePartData()
        {
            //DataTable d1 = new DataTable();
            //string myStr1 = string.Format("select ID,类别,品名,规格,上次更换时间,标准寿命,实际使用次数,部件寿命残值 from SparePartData where 1=1");//sql查询语句
            //d1 = server.ExecuteQuery(myStr1);
            //ShowDGV(dgv_Dispaly, d1, 1);

        }

        private void ShowDGV(DataGridView dgv, DataTable dt, int index)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new ShowDataGridView(ShowDGV), new object[] { dgv, dt, index });
                return;
            }
            switch (index)
            {
                case 0:
                    dgv.DataSource = dt;
                    break;
                case 1:
                    dgv.DataSource = dt;
                    dgv.Columns["部件寿命残值"].DefaultCellStyle.Format = "p3";//设定datagridview寿命残值显示为百分比
                    dgv_AutoSize(dgv);
                    dgv.Sort(dgv.Columns["部件寿命残值"], ListSortDirection.Ascending);//升序排列
                    break;
                default:
                    break;
            }
        }

        public void dgv_AutoSize(DataGridView dgv)//dgv表格自适应
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new DGVAutoSize(dgv_AutoSize), new object[] { dgv });
                return;
            }
            int width = 0;
            //对于DataGridView的每一个列都调整
            for (int i = 0; i < dgv.Columns.Count; i++)
            {
                //将每一列都调整为自动适应模式
                dgv.AutoResizeColumn(i, DataGridViewAutoSizeColumnMode.AllCells);
                //记录整个DataGridView的宽度
                width += dgv.Columns[i].Width;
            }
            //判断调整后的宽度与原来设定的宽度的关系，如果是调整后的宽度大于原来设定的宽度，
            //则将DataGridView的列自动调整模式设置为显示的列即可，             //如果是小于原来设定的宽度，将模式改为填充。
            if (width > dgv.Size.Width)
            {
                dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
            }
            else
            {
                dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            }
            //设置表格字体居中
            dgv.RowsDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            //设置表格列字体居中
            dgv.EnableHeadersVisualStyles = false;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("微软雅黑", 12, FontStyle.Bold);
            dgv.RowsDefaultCellStyle.Font = new Font("微软雅黑", 9);
            dgv.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgv.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.False;//禁止列标题换行
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

        public void UpDatalabelcolor(Color color, string str, string Name)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Labelcolor(UpDatalabelcolor), new object[] { color, str, Name });
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

        public void AppendRichText(string msg, string Name)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new AddItemToListBoxDelegate(AppendRichText), new object[] { msg, Name });
                return;
            }
            foreach (Control ctrl in List_Control)
            {
                if (ctrl.GetType() == typeof(RichTextBox))
                {
                    if (ctrl.Name == Name)
                    {
                        if (msg != "N/A")
                        {
                            ((RichTextBox)ctrl).AppendText(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":  " + msg + "\r\n");
                            //将光标位置设置到当前内容的末尾
                            ((RichTextBox)ctrl).SelectionStart = ((RichTextBox)ctrl).Text.Length;
                            //滚动到光标位置
                            ((RichTextBox)ctrl).ScrollToCaret();
                            if (ctrl.Name == "rtx_TraceMsg" || ctrl.Name == "rtx_PDCAMsg")//判断trace和pdca上传多少次后清空Rth控件
                            {
                                rth_Number++;
                                if (rth_Number > 15)
                                {
                                    rth_Number = 0;
                                    rtx_TraceMsg.Clear();
                                    rtx_PDCAMsg.Clear();
                                }
                            }
                        }
                        else
                        {
                            ((RichTextBox)ctrl).Clear();
                        }
                    }
                }
            }

        }        
        public void AppendRichTextClear(string msg, string Name)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new AddItemToListBoxDelegate(AppendRichTextClear), new object[] { msg, Name });
                return;
            }
            rtx_FixtureMaintainMsg.Clear();
            Log.WriteLog("每天6点清空保养记录");
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
                            if (Name != "list_AllFixture")
                            {
                                ((ListBox)ctrl).Items.Add(msg + "\r\n");
                            }
                            else
                            {
                                                                
                            }
                        }
                        else
                        {
                            ((ListBox)ctrl).Items.Clear();
                        }
                    }
                }
            }

        }

        public void UiText(string str1, string Name)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new ShowTxt(UiText), new object[] { str1, Name });
                return;
            }
            foreach (Control ctrl in List_Control)
            {
                if (ctrl.GetType() == typeof(TextBox))
                {
                    if (ctrl.Name == Name)
                    {
                        ctrl.Text = str1;
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

        public void dgvBlackColor(DataGridView dgv, int r, int c, Color color)
        {
            try
            {
                if (this.InvokeRequired)
                {
                    this.BeginInvoke(new DGV_BlackColor(dgvBlackColor), new object[] { dgv, r, c, color });
                    return;
                }
                dgv[r, c].Style.BackColor = color;
            }
            catch (Exception EX)
            {
                Log.WriteLog(EX.ToString());
            }
        }

        private void lb_TraceUAOK_TextChanged(object sender, EventArgs e)
        {
            Global.Trace_ua_ok = Convert.ToInt32(lb_TraceUAOK.Text);
            Global.inidata.productconfig.trace_ua_ok = lb_TraceUAOK.Text;
            Global.inidata.WriteProductnumSection();
        }

        private void lb_TraceUANG_TextChanged(object sender, EventArgs e)
        {
            Global.Trace_ua_ng = Convert.ToInt32(lb_TraceUANG.Text);
            Global.inidata.productconfig.trace_ua_ng = lb_TraceUANG.Text;
            Global.inidata.WriteProductnumSection();
        }

        private void lb_PDCAUAOK_TextChanged(object sender, EventArgs e)
        {
            Global.Product_num_ua_ok = Convert.ToInt32(lb_PDCAUAOK.Text);
            Global.inidata.productconfig.product_num_ua_ok = lb_PDCAUAOK.Text;
            Global.inidata.WriteProductnumSection();
        }

        private void lb_PDCAUANG_TextChanged(object sender, EventArgs e)
        {
            Global.Product_num_ua_ng = Convert.ToInt32(lb_PDCAUANG.Text);
            Global.inidata.productconfig.product_num_ua_ng = lb_PDCAUANG.Text;
            Global.inidata.WriteProductnumSection();
        }

        private void lb_OEEOK_TextChanged(object sender, EventArgs e)
        {
            Global.oee_ok = Convert.ToInt32(lb_OEEOK.Text);
            Global.inidata.productconfig.oee_ok = lb_OEEOK.Text;
            Global.inidata.WriteProductnumSection();
        }

        private void lb_OEENG_TextChanged(object sender, EventArgs e)
        {
            Global.oee_ng = Convert.ToInt32(lb_OEENG.Text);
            Global.inidata.productconfig.oee_ng = lb_OEENG.Text;
            Global.inidata.WriteProductnumSection();
        }

        private void lb_Materiel_Total_TextChanged(object sender, EventArgs e)
        {
            Global.TotalThrowCount = Convert.ToInt32(lb_Materiel_Total.Text);
            Global.inidata.productconfig.TotalThrowCount = lb_Materiel_Total.Text;
            Global.inidata.WriteProductnumSection();
        }

        private void lb_Materiel_AllNut_TextChanged(object sender, EventArgs e)
        {
            Global.ThrowCount = Convert.ToInt32(lb_Materiel_AllNut.Text);
            Global.inidata.productconfig.ThrowCount = lb_Materiel_AllNut.Text;
            Global.inidata.WriteProductnumSection();
        }

        private void lb_Materiel_Nut_TextChanged(object sender, EventArgs e)
        {
            Global.NutCount = Convert.ToInt32(lb_Materiel_Nut.Text);
            Global.inidata.productconfig.NutCount = lb_Materiel_Nut.Text;
            Global.inidata.WriteProductnumSection();
        }

        private void lb_Materiel_AllOK_TextChanged(object sender, EventArgs e)
        {
            Global.ThrowOKCount = Convert.ToInt32(lb_Materiel_AllOK.Text);
            Global.inidata.productconfig.ThrowOKCount = lb_Materiel_AllOK.Text;
            Global.inidata.WriteProductnumSection();
        }

        private void lb_Materiel_OK_TextChanged(object sender, EventArgs e)
        {
            Global.NutOKCount = Convert.ToInt32(lb_Materiel_OK.Text);
            Global.inidata.productconfig.NutOKCount = lb_Materiel_OK.Text;
            Global.inidata.WriteProductnumSection();
        }

        private void lb_ProcessControlOK_TextChanged(object sender, EventArgs e)
        {
            Global.Trace_process_ok = Convert.ToInt32(lb_ProcessControlOK.Text);
            Global.inidata.productconfig.trace_process_ok = lb_ProcessControlOK.Text;
            Global.inidata.WriteProductnumSection();
        }

        private void lb_ProcessControlNG_TextChanged(object sender, EventArgs e)
        {
            Global.Trace_process_ng = Convert.ToInt32(lb_ProcessControlNG.Text);
            Global.inidata.productconfig.trace_process_ng = lb_ProcessControlNG.Text;
            Global.inidata.WriteProductnumSection();
        }

        private void OEE_upload_Click(object sender, EventArgs e)
        {
            try
            {                
                int i = 0;
                DateTime time = DateTime.Now;
                bool rst1 = true;
                if ((Convert.ToInt32(OEE_index.Text) > 0 && Convert.ToInt32(OEE_index.Text) <= Global.ed.Count) && Global.ed[Convert.ToInt32(OEE_index.Text)].errorStatus != "")
                {
                    string sendTopic = Global.inidata.productconfig.EMT + "/upload/downtime";
                    string ClientPcName = Dns.GetHostName();
                    string IP = _mainparent.GetIp();
                    string Mac = _mainparent.GetOEEMac();
                    string OEE_DT = "";
                    i = Convert.ToInt32(OEE_index.Text);
                    Guid guid1 = Guid.NewGuid();
                    OEE_DT = string.Format("{{\"guid\":\"{0}\",\"EMT\":\"{1}\",\"PoorNum\":\"{2}\",\"TotalNum\":\"{3}\",  \"Status\":\"{4}\",\"ErrorCode\":\"{5}\",\"EventTime\":\"{6}\",\"ModuleCode\":\"{7}\",\"ClientPcName\":\"{8}\",\"MAC\":\"{9}\",\"IP\":\"{10}\"}}", guid1, Global.inidata.productconfig.EMT, "0", "0", Global.ed[i].errorStatus, Global.ed[i].errorCode, time.ToString("yyyy-MM-dd HH:mm:ss.fff"), Global.ed[i].ModuleCode, ClientPcName, Mac, IP);
                    //Global._mqttClient.Publish(sendTopic, Encoding.UTF8.GetBytes(OEE_DT), MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE, false);
                    //rst1 = _mainparent.SendMqttResult(guid1);
                    //if (rst1)
                    //{
                    //    if (Global.respond[guid1].Result == "OK")
                    //    {
                    //        Log.WriteLog(guid1 + "," + Global.inidata.productconfig.EMT + "," + "0" + "," + "0" + "," + Global.ed[i].errorStatus + "," + Global.ed[i].errorCode + "," + time.ToString("yyyy-MM-dd HH:mm:ss.fff") + "," + Global.ed[i].ModuleCode + "," + ClientPcName + "," + Mac + "," + IP, System.AppDomain.CurrentDomain.BaseDirectory + "\\手动上传OEE_DT\\");
                    //        Global.respond.TryRemove(guid1, out Global.Respond);
                    //    }
                    //    else
                    //    {
                    //        AppendRichText("故障代码=" + Global.ed[i].errorCode + ",触发时间=" + time.ToString("yyyy-MM-dd HH:mm:ss.fff") + ",运行状态:" + Global.ed[i].errorStatus + ",故障描述:" + Global.ed[i].errorinfo + ",模组代码:" + Global.ed[i].ModuleCode + ",自动发送失败", "rtx_OEEMsg");
                    //        AppendRichText("失败原因:" + Global.respond[guid1].ErrorCode + Global.respond[guid1].Result, "rtx_OEEMsg");
                    //        Global.respond.TryRemove(guid1, out Global.Respond);
                    //    }
                    //}
                    //else
                    //{
                    //    AppendRichText("网络异常,超时无反馈", "rtx_OEEMsg");
                    //}
                }
                else
                {
                    MessageBox.Show(string.Format(("请输入1至{0}的整数！"), Global.ed.Count));
                }
            }
            catch (Exception ex)
            {
                Log.WriteLog("OEE手动上传失败，" + ex.ToString());
            }
        }

        private void OEE_uploadAll_Click(object sender, EventArgs e)
        {
            Global.b_VerifyResult = true;
            if (!Global.b_VerifyResult)
            {
                MessageBox.Show("Trace参数异常,不能上传OQ数据");
                return;
            }
            Thread th = new Thread(UploadAll);
            th.IsBackground = true;
            th.Start();
        }

        public static bool dataGridViewToCSV(DataGridView dataGridView)
        {
            if (dataGridView.Rows.Count == 0)
            {
                MessageBox.Show("没有数据可导出!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "CSV files (*.csv)|*.csv";
            saveFileDialog.FilterIndex = 0;
            saveFileDialog.RestoreDirectory = true;
            saveFileDialog.CreatePrompt = true;
            saveFileDialog.FileName = null;
            saveFileDialog.Title = "保存";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                Stream stream = saveFileDialog.OpenFile();
                StreamWriter sw = new StreamWriter(stream, System.Text.Encoding.GetEncoding(-0));
                string strLine = "";
                try
                {
                    //表头
                    for (int i = 0; i < dataGridView.ColumnCount; i++)
                    {
                        if (i > 0)
                            strLine += ",";
                        strLine += dataGridView.Columns[i].HeaderText;
                    }
                    strLine.Remove(strLine.Length - 1);
                    sw.WriteLine(strLine);
                    strLine = "";
                    //表的内容
                    for (int j = 0; j < dataGridView.Rows.Count; j++)
                    {
                        strLine = "";
                        int colCount = dataGridView.Columns.Count;
                        for (int k = 0; k < colCount; k++)
                        {
                            if (k > 0 && k < colCount)
                                strLine += ",";
                            if (dataGridView.Rows[j].Cells[k].Value == null)
                                strLine += "";
                            else
                            {
                                string cell = dataGridView.Rows[j].Cells[k].Value.ToString().Trim();
                                //防止里面含有特殊符号
                                cell = cell.Replace("\"", "\"\"");
                                cell = "\"" + cell + "\"";
                                strLine += cell;
                            }
                        }
                        sw.WriteLine(strLine);
                    }
                    sw.Close();
                    stream.Close();
                    MessageBox.Show("数据被导出到：" + saveFileDialog.FileName.ToString(), "导出完毕", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "导出错误", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return false;
                }
            }
            return true;
        }

        private void UploadAll(object ob)
        {

            try
            {
                string band = "";
                GetSNData SP = new GetSNData();
                // 先获取band对应的SP码

                string callresult = "";
                string errmsg = "";
                //band = tb_bandsn.Text;
                tb_bandsn.Invoke(new Action(() => { band = tb_bandsn.Text.Remove(18); }));
                //JsonSerializerSettings jsetting = new JsonSerializerSettings();
                //jsetting.NullValueHandling = NullValueHandling.Ignore;//Json不输出空值
                //string url = string.Format("http://17.239.116.110/api/v2/parts?serial_type=band&serial={0}&process_name=bd-bc-le&last_log=true", band);              
                string sendTopic = Global.inidata.productconfig.EMT + "/upload/downtime";
                string ClientPcName = Dns.GetHostName();
                string IP = _mainparent.GetIp();
                string Mac = _mainparent.GetOEEMac();
                string Trcae_logs_str = string.Empty;
                TraceRespondID Msg = null;
                for (int i = 1; i < Global.ed.Count + 1; i++)
                {
                    if (Global.ed.Keys.Contains(i))
                    {
                        string OEE_DT = "";
                        Guid guid = Guid.NewGuid();
                        if (Global.ed[i].errorStatus == "2")
                        {
                            DateTime time = DateTime.Now;
                            OEE_DT = string.Format("{{\"guid\":\"{0}\",\"EMT\":\"{1}\",\"PoorNum\":\"{2}\",\"TotalNum\":\"{3}\",  \"Status\":\"{4}\",\"ErrorCode\":\"{5}\",\"EventTime\":\"{6}\",\"ModuleCode\":\"{7}\",\"ClientPcName\":\"{8}\",\"MAC\":\"{9}\",\"IP\":\"{10}\"}}", guid, Global.inidata.productconfig.EMT, "0", "0", Global.ed[i].errorStatus, Global.ed[i].errorCode, time.ToString("yyyy-MM-dd HH:mm:ss.fff"), Global.ed[i].ModuleCode, ClientPcName, Mac, IP);
                            //Global._mqttClient.Publish(sendTopic, Encoding.UTF8.GetBytes(OEE_DT), MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE, false);
                            //bool rst = _mainparent.SendMqttResult(guid);
                            bool rst = true;
                            if (rst)
                            {
                                if (true)
                                {
                                    string InsertOEEStr = "insert into OEE_TraceDT([DateTime],[Status],[ErrorCode],[EventTime],[ModuleCode])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," + "'" + Global.ed[i].errorStatus + "'" + "," + "'" + "00000000" + "'" + ","
                                                + "'" + time.ToString("yyyy-MM-dd HH:mm:ss.fff") + "'" + "," + "'" + Global.ed[i].ModuleCode + "'" + ")";
                                    SQL.ExecuteUpdate(InsertOEEStr);
                                    AppendRichText("故障代码=" + Global.ed[i].errorCode + ",触发时间=" + time.ToString("yyyy-MM-dd HH:mm:ss.fff") + ",运行状态:" + Global.ed[i].errorStatus + ",故障描述:" + Global.ed[i].errorinfo + ",模组代码:" + Global.ed[i].ModuleCode + ",自动发送成功", "rtx_OEEMsg");
                                    Global.respond.TryRemove(guid, out Global.Respond);
                                    Log.WriteLog(guid + "," + Global.inidata.productconfig.EMT + "," + "0" + "," + "0" + "," + Global.ed[i].errorStatus + "," + Global.ed[i].errorCode + "," + time.ToString("yyyy-MM-dd HH:mm:ss.fff") + "," + Global.ed[i].ModuleCode + "," + ClientPcName + "," + Mac + "," + IP, System.AppDomain.CurrentDomain.BaseDirectory + "\\手动上传OEE_DT\\");
                                }
                                else
                                {
                                    AppendRichText("故障代码=" + Global.ed[i].errorCode + ",触发时间=" + time.ToString("yyyy-MM-dd HH:mm:ss.fff") + ",运行状态:" + Global.ed[i].errorStatus + ",故障描述:" + Global.ed[i].errorinfo + ",模组代码:" + Global.ed[i].ModuleCode + ",自动发送失败", "rtx_OEEMsg");
                                    AppendRichText("失败原因:" + Global.respond[guid].ErrorCode + Global.respond[guid].Result, "rtx_OEEMsg");
                                    Global.respond.TryRemove(guid, out Global.Respond);
                                }
                            }
                            else
                            {
                                AppendRichText("网络异常,超时无反馈", "rtx_OEEMsg");
                            }
                            Thread.Sleep(200);
                        }
                        if (Global.ed[i].errorStatus != "" && Global.ed[i].errorStatus != "2")
                        {
                            DateTime time = DateTime.Now;
                            OEE_DT = string.Format("{{\"guid\":\"{0}\",\"EMT\":\"{1}\",\"PoorNum\":\"{2}\",\"TotalNum\":\"{3}\",  \"Status\":\"{4}\",\"ErrorCode\":\"{5}\",\"EventTime\":\"{6}\",\"ModuleCode\":\"{7}\",\"ClientPcName\":\"{8}\",\"MAC\":\"{9}\",\"IP\":\"{10}\"}}", guid, Global.inidata.productconfig.EMT, "0", "0", Global.ed[i].errorStatus, Global.ed[i].errorCode, time.ToString("yyyy-MM-dd HH:mm:ss.fff"), Global.ed[i].ModuleCode, ClientPcName, Mac, IP);
                            //Global._mqttClient.Publish(sendTopic, Encoding.UTF8.GetBytes(OEE_DT), MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE, false);
                            //bool rst = _mainparent.SendMqttResult(guid);
                            bool rst = true;
                            if (rst)
                            {
                                if (true)
                                {
                                    string InsertOEEStr = "insert into OEE_TraceDT([DateTime],[Status],[ErrorCode],[EventTime],[ModuleCode])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," + "'" + Global.ed[i].errorStatus + "'" + "," + "'" + Global.ed[i].errorCode + "'" + ","
                                                + "'" + time.ToString("yyyy-MM-dd HH:mm:ss.fff") + "'" + "," + "'" + Global.ed[i].ModuleCode + "'" + ")";
                                    SQL.ExecuteUpdate(InsertOEEStr);
                                    AppendRichText("故障代码=" + Global.ed[i].errorCode + ",触发时间=" + time.ToString("yyyy-MM-dd HH:mm:ss.fff") + ",运行状态:" + Global.ed[i].errorStatus + ",故障描述:" + Global.ed[i].errorinfo + ",模组代码:" + Global.ed[i].ModuleCode + ",自动发送成功", "rtx_OEEMsg");
                                    Global.respond.TryRemove(guid, out Global.Respond);
                                    Log.WriteLog(guid + "," + Global.inidata.productconfig.EMT + "," + "0" + "," + "0" + "," + Global.ed[i].errorStatus + "," + Global.ed[i].errorCode + "," + time.ToString("yyyy-MM-dd HH:mm:ss.fff") + "," + Global.ed[i].ModuleCode + "," + ClientPcName + "," + Mac + "," + IP, System.AppDomain.CurrentDomain.BaseDirectory + "\\手动上传OEE_DT\\");
                                }
                                else
                                {
                                    AppendRichText("故障代码=" + Global.ed[i].errorCode + ",触发时间=" + time.ToString("yyyy-MM-dd HH:mm:ss.fff") + ",运行状态:" + Global.ed[i].errorStatus + ",故障描述:" + Global.ed[i].errorinfo + ",模组代码:" + Global.ed[i].ModuleCode + ",自动发送失败", "rtx_OEEMsg");
                                    AppendRichText("失败原因:" + Global.respond[guid].ErrorCode + Global.respond[guid].Result, "rtx_OEEMsg");
                                    Global.respond.TryRemove(guid, out Global.Respond);
                                }
                            }
                            else
                            {
                                AppendRichText("网络异常,超时无反馈", "rtx_OEEMsg");
                            }
                            Thread.Sleep(200);
                        }
                        else
                        {
                        }


                    }
                }
                JsonSerializerSettings jsetting1 = new JsonSerializerSettings();
                jsetting1.NullValueHandling = NullValueHandling.Ignore;//Json不输出空值
                TraceMesRequest_ua trace_ua = new TraceMesRequest_ua();
                trace_ua.serials = new SN();
                trace_ua.data = new data();
                trace_ua.data.insight = new Insight();
                trace_ua.data.insight.test_attributes = new Test_attributes();
                trace_ua.data.insight.test_station_attributes = new Test_station_attributes();
                trace_ua.data.insight.uut_attributes = new Uut_attributes();
                trace_ua.data.insight.results = new Result[1];
                for (int i = 0; i < trace_ua.data.insight.results.Length; i++)
                {
                    trace_ua.data.insight.results[i] = new Result();
                }
                trace_ua.data.items = new ExpandoObject();
                trace_ua.serials.band = band;
                trace_ua.data.insight.test_attributes.test_result = "fail";
                trace_ua.data.insight.test_attributes.unit_serial_number = band;
                trace_ua.data.insight.test_attributes.uut_start = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                trace_ua.data.insight.test_attributes.uut_stop = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                trace_ua.data.insight.test_station_attributes.fixture_id = "H-76HO-SMA40-2200-A-00003";
                trace_ua.data.insight.test_station_attributes.head_id = "1";
                trace_ua.data.insight.test_station_attributes.line_id = Global.inidata.productconfig.Line_id_la;
                trace_ua.data.insight.test_station_attributes.software_name = Global.inidata.productconfig.Sw_name_la;
                trace_ua.data.insight.test_station_attributes.software_version = Global.inidata.productconfig.Version;
                trace_ua.data.insight.test_station_attributes.station_id = Global.inidata.productconfig.Station_id_la;

                //trace_ua.data.insight.uut_attributes.STATION_STRING = "";
                trace_ua.data.insight.uut_attributes.STATION_STRING = string.Format("{{\"ActualCT \":\"{0}\",\"ScanCount \":\"{1}\"}}", "10", "1");
                //trace_ua.data.insight.uut_attributes.fixture_id = "H-76HO-SMA40-2200-A-00003";
                trace_ua.data.insight.uut_attributes.full_sn = band;
                trace_ua.data.insight.uut_attributes.fixture_id = "XXXXX";
                trace_ua.data.insight.uut_attributes.laser_start_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                trace_ua.data.insight.uut_attributes.laser_stop_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                trace_ua.data.insight.uut_attributes.pattern_type = "1";
                trace_ua.data.insight.uut_attributes.spot_size = "0.45";
                trace_ua.data.insight.uut_attributes.hatch = "0.05";
                trace_ua.data.insight.uut_attributes.swing_amplitude = "0.2";
                trace_ua.data.insight.uut_attributes.swing_freq = "10000";
                trace_ua.data.insight.uut_attributes.tossing_item = "NG";

                trace_ua.data.insight.results[0].result = "fail";
                trace_ua.data.insight.results[0].test = "laser_sensor";
                trace_ua.data.insight.results[0].units = "";
                trace_ua.data.insight.results[0].value = "0";
                //trace_ua.data.insight.results[1].result = "pass";
                //trace_ua.data.insight.results[1].test = "locationx_locationname_layerx";
                //trace_ua.data.insight.results[1].units = "";
                //trace_ua.data.insight.results[1].value = "0";

                //JsonSerializerSettings jsetting1 = new JsonSerializerSettings();
                //jsetting1.NullValueHandling = NullValueHandling.Ignore;//Json不输出空值

                //TraceMesRequest_ua trace_ua = new TraceMesRequest_ua();
                //trace_ua.serials = new SN();
                //trace_ua.data = new data();
                //trace_ua.data.insight = new Insight();
                //trace_ua.data.insight.test_attributes = new Test_attributes();
                //trace_ua.data.insight.test_station_attributes = new Test_station_attributes();
                //trace_ua.data.insight.uut_attributes = new Uut_attributes();
                //trace_ua.data.insight.results = new Result[1];
                //for (int i = 0; i < trace_ua.data.insight.results.Length; i++)
                //{
                //    trace_ua.data.insight.results[i] = new Result();
                //}
                //trace_ua.serials.band = band;
                ////test_attributes
                //trace_ua.data.insight.test_attributes.test_result = "fail";
                //trace_ua.data.insight.test_attributes.unit_serial_number = band;//17位
                //trace_ua.data.insight.test_attributes.uut_start = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                //trace_ua.data.insight.test_attributes.uut_stop = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                ////test_station_attributes
                //trace_ua.data.insight.test_station_attributes.fixture_id = "H-76HO-SMA40-2200-A-00003";
                //trace_ua.data.insight.test_station_attributes.head_id = "1";
                //trace_ua.data.insight.test_station_attributes.line_id = Global.inidata.productconfig.Line_id_la;
                //trace_ua.data.insight.test_station_attributes.software_name = Global.inidata.productconfig.Sw_name_la;
                //trace_ua.data.insight.test_station_attributes.software_version = Global.inidata.productconfig.Version;
                //trace_ua.data.insight.test_station_attributes.station_id = Global.inidata.productconfig.Station_id_la;
                ////uut_attributes
                //trace_ua.data.insight.uut_attributes.STATION_STRING = string.Format("{{\"ActualCT \":\"{0}\",\"ScanCount \":\"{1}\"}}", "10", "1");
                //trace_ua.data.insight.uut_attributes.full_sn = band;
                //trace_ua.data.insight.uut_attributes.fixture_id = "XXXXX";
                //trace_ua.data.insight.uut_attributes.laser_start_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                //trace_ua.data.insight.uut_attributes.laser_stop_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                //trace_ua.data.insight.uut_attributes.pattern_type = "1";
                //trace_ua.data.insight.uut_attributes.spot_size = "0.45";
                //trace_ua.data.insight.uut_attributes.hatch = "0.05";
                //trace_ua.data.insight.uut_attributes.swing_amplitude = "0.2";
                //trace_ua.data.insight.uut_attributes.swing_freq = "10000";
                //trace_ua.data.insight.uut_attributes.tossing_item = "NG";

                ////trace_ua.data.insight.results[0].result = "pass";
                ////trace_ua.data.insight.results[0].test = "riveting_depth_layerx";
                ////trace_ua.data.insight.results[0].units = "mm";
                ////trace_ua.data.insight.results[0].value = "NA";
                //trace_ua.data.insight.results[0].result = "fail";
                //trace_ua.data.insight.results[0].test = "laser_sensor";
                //trace_ua.data.insight.results[0].units = "";
                //trace_ua.data.insight.results[0].value = "0";

                //trace_ua.data.items = new ExpandoObject();

                string SendTraceLogs = JsonConvert.SerializeObject(trace_ua, Formatting.None, jsetting1);
                Log.WriteLog(SendTraceLogs);
                var Trcae_logs_result = RequestAPI2.Trcae_logs(Global.inidata.productconfig.Trace_Logs_LA, SendTraceLogs, out Trcae_logs_str, out Msg);
                if (Trcae_logs_result)
                {
                    AppendRichText("Trace_Tab手动上传结果:" + Trcae_logs_result, "rtx_OEEMsg");
                    AppendRichText("Trace_Tab手动上传结果:" + band + "  " + JsonConvert.SerializeObject(Msg), "rtx_OEEMsg");

                }
                else
                {
                    AppendRichText("Trace手动上传结果:false", "rtx_OEEMsg");
                }

                trace_ua.data.insight.test_station_attributes.line_id = Global.inidata.productconfig.Line_id_ua;
                trace_ua.data.insight.test_station_attributes.software_name = Global.inidata.productconfig.Sw_name_ua;
                trace_ua.data.insight.test_station_attributes.software_version = Global.inidata.productconfig.Version;
                trace_ua.data.insight.test_station_attributes.station_id = Global.inidata.productconfig.Station_id_ua;
                
                string SelectStr = "SELECT * FROM OEE_TraceDT";//sql查询语句
                DataTable d1 = SQL.ExecuteQuery(SelectStr);
                if (d1 != null && d1.Rows.Count > 0)
                {
                    for (int i = 0; i < d1.Rows.Count; i++)
                    {
                        (trace_ua.data.items as ICollection<KeyValuePair<string, object>>).Add(new KeyValuePair<string, object>("error_" + (i + 1), d1.Rows[i][3].ToString() + "_" + (Convert.ToDateTime(d1.Rows[i][4])).ToString("yyyy-MM-dd HH:mm:ss")));
                    }
                }
                SendTraceLogs = JsonConvert.SerializeObject(trace_ua, Formatting.None, jsetting1);                
                 Trcae_logs_result = RequestAPI2.Trcae_logs(Global.inidata.productconfig.Trace_Logs_UA, SendTraceLogs, out Trcae_logs_str, out Msg);
                if (Trcae_logs_result)
                {
                    AppendRichText("Trace_Trench手动上传结果:" + Trcae_logs_result, "rtx_OEEMsg");
                    AppendRichText("Trace_Trench手动上传结果:" + band+"  " + JsonConvert.SerializeObject(Msg), "rtx_OEEMsg");

                }
                else
                {
                    AppendRichText("Trace_Trench手动上传结果:false", "rtx_OEEMsg");
                }              
                
                string DeleteStr = string.Format("delete from [ZHH].[dbo].[OEE_TraceDT] where ID  in(select top {0} ID  from  [ZHH].[dbo].[OEE_TraceDT] order  by  ID  asc)", d1.Rows.Count);
                SQL.ExecuteUpdate(DeleteStr);
                Log.WriteLog(SendTraceLogs);
           }
            catch (Exception ex)
            {
                Log.WriteLog("OEE手动上传失败，" + ex.ToString());
            }
        }

        private void btn_SelectFixture_Click(object sender, EventArgs e)
        {
            if (txt_SelectFixture.Text != string.Empty)
            {
                string Select = string.Format("SELECT * FROM FixtureNG  where Fixture='{0}' and cast(DateTime as datetime) >='{1}' and cast(DateTime as datetime) <='{2}' ", txt_SelectFixture.Text, Convert.ToDateTime(dtp_SelectFixture.Text).ToString("yyyy/MM/dd") + " 06:00:00", Convert.ToDateTime(dtp_SelectFixture.Text).AddDays(1).ToString("yyyy/MM/dd") + " 06:00:00");
                DataTable dt = SQL.ExecuteQuery(Select);//1、查找选择当天6：00-隔天6：00所有数据
                list_FixtureNG.Items.Clear();
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        list_FixtureNG.Items.Add(string.Format("{0}[治具码:{1},NG]", dt.Rows[i][1].ToString(), dt.Rows[i][2].ToString()));
                    }
                }
            }
            else
            {
                MessageBox.Show("需要查询的治具SN不能为空！");
            }
        }
        
        private void btn_OutPut_Click(object sender, EventArgs e)
        {
            try
            {
                if (list_FixtureNG.Items.Count == 0)
                {
                    MessageBox.Show("没有数据可导出!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                            sw.WriteLine("时间,治具号,结果");
                            for (int t = 0; t < list_FixtureNG.Items.Count; t++)
                            {
                                string oeestr = "";
                                oeestr += list_FixtureNG.Items[t].ToString().Replace("[", ",");
                                sw.WriteLine(oeestr);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, "导出错误", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        sw.Close();
                        sw.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.WriteLog(ex.ToString());
            }
        }               
        private void btn_EndPISFixture_Click(object sender, EventArgs e)
        {
            
        }

        private void btn_Uploadsetting_Click(object sender, EventArgs e)
        {
            if (rb_fixtruetime.Checked)
            {
                Global.inidata.productconfig.fixtruetime = "1";
                Global.inidata.productconfig.fixtruetimes = "2";
            }
            if (rb_fixturetimes.Checked)
            {
                Global.inidata.productconfig.fixtruetime = "2";
                Global.inidata.productconfig.fixtruetimes = "1";
            }
            Global.inidata.WriteProductConfigSection();//重新写入setting文件
        }

        private void btn_FixtureOut_Click(object sender, EventArgs e)
        {
            try
            {
                Global.DataGridView_Select_RowIndex = grid_AllFixture.CurrentRow.Index;   //所选的行的索引
                if (Global.DataGridView_Select_RowIndex != -1)
                {
                    string fixture_ng = grid_AllFixture.Rows[Global.DataGridView_Select_RowIndex].Cells[1].EditedFormattedValue.ToString();
                    if (!Global._fixture_ng.Contains(fixture_ng))
                    {
                        Global._fixture_ng.Add(fixture_ng);
                    }
                    Global.DataGridView_Select_RowIndex = -1;
                    Txt.WriteLine2(Global._fixture_ng);
                    string insertStr = string.Format("UPDATE [FixtureStatus] SET [Status] = '逾期未保养',[Time]='{1}' where [FixtureID] = '{0}'", fixture_ng, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    int r1 = SQL.ExecuteUpdate(insertStr);
                    UpdateDataGridView();
                    Log.WriteLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + fixture_ng + "," + "治具手动排出", @"D:\ZHH\治具小保养记录\");
                }
            }
            catch
            { }            
        }

        private void btn_endmaintain_Click(object sender, EventArgs e)
        {
            //点击确认则会更新该治具为初始状态  
            try
            {
                int i = grid_AllFixture.CurrentRow.Index;   //所选的行的索引
                string fixture = grid_AllFixture.Rows[i].Cells[1].Value.ToString();//治具码
                grid_AllFixture.Rows[i].Cells[2].Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                grid_AllFixture.Rows[i].Cells[4].Value = "0";
                grid_AllFixture.Rows[i].Cells[5].Value = "正常使用";
                Global._fixture_ng.Remove(fixture);//在list中删除该治具
                Txt.WriteLine2(Global._fixture_ng);//在TXT中删除该治具

                ///20230226Update
                string insertStr1 = string.Format("UPDATE [FixtureStatus] SET Complete = '{0}',UsingTimes = '{1}', [Status] = '正常使用中' where [FixtureID] = '{2}'", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "0", fixture);
                int r1 = SQL.ExecuteUpdate(insertStr1);
                UpdateDataGridView();//更新DataGridView  治具抛料表
                Log.WriteLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + fixture + "," + "治具保养完成", @"D:\ZHH\治具小保养记录\");
            }
            catch (Exception EX)
            {
                Log.WriteLog("治具保养异常：" + EX.ToString());
            }
        }
        #region 保养治具相关
        private void grid_TossingFixture_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (grid_TossingFixture.Columns[e.ColumnIndex].Name == "BtnModify" && grid_TossingFixture.RowCount > 0 && e.RowIndex >= 0)
            {
                //点击确认则会更新该治具为初始状态  
                string fixture = grid_TossingFixture[0, e.RowIndex].Value.ToString();
                Global._fixture_tossing_ng.Remove(fixture);//在list中删除该治具
                Txt.WriteLine3(Global._fixture_tossing_ng);//在TXT中删除该治具
                grid_TossingFixture.Rows[e.RowIndex].Cells[1].Value = "";
                grid_TossingFixture.Rows[e.RowIndex].Cells[2].Value = "0";
                grid_TossingFixture.Rows[e.RowIndex].Cells[3].Value = "0";
                grid_TossingFixture.Rows[e.RowIndex].Cells[4].Value = "OK";
                grid_TossingFixture.Rows[e.RowIndex].Cells[5].Value = "OK";
                string insertStr1 = string.Format("UPDATE [FixtureTossing] SET [TossingTime] = '{0}',[TossingContinuation] = '{1}', [TossingCount] = '{2}',[ContinuationNG] = '{3}',[CountNG] = '{4}' where [Fixture] = '{5}'",
                                           "", "0", "0", "OK", "OK", fixture);
                DataTable dt = SQL.ExecuteQuery(insertStr1);
                Log.WriteLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + fixture + "," + "治具维修完成", @"D:\ZHH\治具维修记录\");
                UpdateDataGridView();
            }
        }
        public void UpdateDataGridView()//加载治具抛料记录数据表
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new ShowDataGridView1(UpdateDataGridView), new object[] { });
                return;
            }
            try
            {
                grid_AllFixture.Columns.Clear();
                grid_AllFixture.AutoGenerateColumns = true;
                string SelectStr = "SELECT * FROM FixtureStatus";//sql查询语句
                DataTable d1 = SQL.ExecuteQuery(SelectStr);
                d1.Columns["ID"].ColumnName = "序号";
                d1.Columns["FixtureID"].ColumnName = "治具码";
                d1.Columns["Time"].ColumnName = "排出时间";
                d1.Columns["Complete"].ColumnName = "保养完成时间";
                d1.Columns["UsingTimes"].ColumnName = "使用次数";
                d1.Columns["Status"].ColumnName = "状态";
                grid_AllFixture.DataSource = d1;     //绑定数据源为 FixtureStatus 表中数据
                dgv_AutoSize(grid_AllFixture);      // 控件自适应大小
                grid_AllFixture.AutoGenerateColumns = false;
                for (int i = 0; i < grid_AllFixture.RowCount - 1; i++)
                {
                    if (grid_AllFixture.Rows[i].Cells[5].EditedFormattedValue.ToString() == "逾期未保养")  //获取每行的治具状态
                    {
                        grid_AllFixture.Rows[i].Cells[5].Style.BackColor = Color.Red;
                    }
                    else
                    {
                        grid_AllFixture.Rows[i].Cells[5].Style.BackColor = Color.Green;
                    }
                }

                //治具抛料数据导入
                grid_TossingFixture.Columns.Clear();//治具抛料记录
                grid_TossingFixture.AutoGenerateColumns = true; ;
                DataGridViewButtonColumn btn = new DataGridViewButtonColumn();
                btn.Name = "BtnModify";
                btn.HeaderText = "维修确认";
                btn.DefaultCellStyle.NullValue = "确认";
                btn.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                string SelectStr1 = "SELECT [Fixture],[TossingTime],[TossingContinuation],[TossingCount],[ContinuationNG],[CountNG] FROM FixtureTossing";//sql查询语句
                DataTable d2 = SQL.ExecuteQuery(SelectStr1);
                d2.Columns["Fixture"].ColumnName = "治具码";
                d2.Columns["TossingTime"].ColumnName = "抛料时间";
                d2.Columns["TossingContinuation"].ColumnName = "连续抛料次数(Max 3)";
                d2.Columns["TossingCount"].ColumnName = "抛料次数(Max 5)";
                d2.Columns["ContinuationNG"].ColumnName = "连续NG";
                d2.Columns["CountNG"].ColumnName = "次数NG";
                grid_TossingFixture.DataSource = d2;        //绑定数据源
                grid_TossingFixture.Columns.Add(btn);       //添加确认按钮
                dgv_AutoSize(grid_TossingFixture);          //控件自适应大小
                grid_TossingFixture.AutoGenerateColumns = false;
                for (int i = 0; i < grid_TossingFixture.RowCount; i++)
                {
                    if (grid_TossingFixture.Rows[i].Cells[4].ToString() == "NG")
                    {
                        grid_TossingFixture.Rows[i].Cells[4].Style.BackColor = Color.Red;
                    }
                    else
                    {
                        grid_TossingFixture.Rows[i].Cells[4].Style.BackColor = Color.Green;
                    }
                    if (grid_TossingFixture.Rows[i].Cells[5].ToString() == "NG")
                    {
                        grid_TossingFixture.Rows[i].Cells[5].Style.BackColor = Color.Red;
                    }
                    else
                    {
                        grid_TossingFixture.Rows[i].Cells[5].Style.BackColor = Color.Green;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.WriteLog("加载datagridview异常：治具抛料记录" + ex.ToString());
            }
        }
        /// <summary>
        /// 20230226Add
        /// </summary>
        public void UpdateSjFixtureCount() {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new UpdateSjFixture(UpdateSjFixtureCount));
                return;
            }
            labelFixtureSjCount.Text = Global._fixture.Count.ToString();
            labelFixtureSjCount.BackColor = labelFixtureSjCount.Text != Global.inidata.productconfig.FixtureCount ? Color.Red : Color.Green;
        }

        public void update_fixtures(string fixture)
        {
            try
            {
                string selectStr2 = string.Format("select * from FixtureCount  where [FixtureID] = '{0}'", fixture);
                DataTable d1 = SQL.ExecuteQuery(selectStr2);
                if (d1!=null && d1.Rows.Count>0)
                {
                    if (int.Parse(Global.Setpoint) < int.Parse(d1.Rows[0][2].ToString()))
                    {
                        string updateStr2 = string.Format("UPDATE [FixtureCount] SET [UsingTimes] = [UsingTimes] + 1 ,[YesNo] = Y  where [FixtureID] = '{0}'", fixture);

                        int r = SQL.ExecuteUpdate(updateStr2);
                    }
                    else
                    {
                        string updateStr2 = string.Format("UPDATE [FixtureCount] SET [UsingTimes] = [UsingTimes] + 1  where [FixtureID] = '{0}'", fixture);
                        Log.WriteLog(updateStr2);
                        int r = SQL.ExecuteUpdate(updateStr2);
                    }
                }                
            }
            catch (Exception EX)
            { Log.WriteLog(EX.ToString()); }
                                      
        }

        private void ShowLvFixtrues()
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new ShowLvFixtrue(ShowLvFixtrues), new object[] { });
                return;
            }
            string selectStr = string.Format("select * from FixtureNumber");
            DataTable d2 = SQL.ExecuteQuery(selectStr);
            dataTableToListView_fixtrue(lv_Fixtures, d2);
        }
        public void dataTableToListView_fixtrue(ListView lv, DataTable dt)
        {
            try
            {
                if (dt != null && dt.Rows.Count>0)
                {
                    lv.View = View.Details;
                    lv.GridLines = true;//显示网格线
                    lv.Items.Clear();//所有的项
                    lv.Columns.Clear();//标题
                                       //对表格重新排序赋值
                    DataTable dt2 = new DataTable();
                    //dt2.Rows.Add(0, "08:00-09:00", 0, 0);

                    //dt2.Columns[0].ColumnName = "序号";
                    //dt2.Columns[1].ColumnName = "时间段";
                    //dt2.Columns[2].ColumnName = "治具SN不相同的数量";
                    //dt2.Columns[3].ColumnName = "治具SN重复次数>设定值的数量";                  
                    DataColumn dataColumn = new DataColumn("序号");
                    DataColumn dataColumn1 = new DataColumn("时间段");
                    DataColumn dataColumn2 = new DataColumn("治具SN不相同的数量");
                    DataColumn dataColumn3 = new DataColumn("治具SN重复次数>设定值的数量");

                    //dt2.Columns.
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
                    for (int i = 0; i < dt.Columns.Count - 25; i++)
                    {
                        // lv.Items.Add((i + 1).ToString(), DateTime_fixtrue[i], dt.Columns[i + 1].ToString());
                        dt2.Rows.Add(i + 1, DateTime_fixtrue[i], dt.Rows[0][i + 1].ToString(), dt.Rows[0][i + 25].ToString());
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
            catch (Exception ex)
            { Log.WriteLog(ex.ToString()); }

        }

        public void Show_lvFixtrue()
        {
            while (true)
            {
                try
                {                    
                    string DT = DateTime.Now.Hour.ToString();              
                    if (DT == "8" && Global.Int_Setpint == 0)
                    {
                        Global.Int_Setpint = 1;
                        string InsertStr2 = "insert into FixtureNumber([DateTime],[Hour_0],[Hour_1],[Hour_2],[Hour_3],[Hour_4],[Hour_5],[Hour_6],[Hour_7],[Hour_8],[Hour_9],[Hour_10],[Hour_11],[Hour_12],"
                        + "[Hour_13],[Hour_14],[Hour_15],[Hour_16],[Hour_17],[Hour_18],[Hour_19],[Hour_20],[Hour_21],[Hour_22],[Hour_23],"
                        + "[Hour_re0],[Hour_re1],[Hour_re2],[Hour_re3],[Hour_re4],[Hour_re5],[Hour_re6],[Hour_re7],[Hour_re8],[Hour_re9],[Hour_re10],[Hour_re11],[Hour_re12],"
                        + "[Hour_re13],[Hour_re14],[Hour_re15],[Hour_re16],[Hour_re17],[Hour_re18],[Hour_re19],[Hour_re20],[Hour_re21],[Hour_re22],[Hour_re23]"
                        + ")" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd") + "'" + "," + "'" + 0 + "'" + "," + "'" + 0 + "'" + ","
                        + "'" + 0 + "'" + "," + "'" + 0 + "'" + "," + "'" + 0 + "'" + "," + "'" + 0 + "'" + ","
                        + "'" + 0 + "'" + "," + "'" + 0 + "'" + "," + "'" + 0 + "'" + "," + "'" + 0 + "'" + ","
                        + "'" + 0 + "'" + "," + "'" + 0 + "'" + "," + "'" + 0 + "'" + "," + "'" + 0 + "'" + ","
                        + "'" + 0 + "'" + "," + "'" + 0 + "'" + "," + "'" + 0 + "'" + "," + "'" + 0 + "'" + ","
                        + "'" + 0 + "'" + "," + "'" + 0 + "'" + "," + "'" + 0 + "'" + "," + "'" + 0 + "'" + ","
                        + "'" + 0 + "'" + "," + "'" + 0 + "'" + "," + "'" + 0 + "'" + "," + "'" + 0 + "'" + ","
                        + "'" + 0 + "'" + "," + "'" + 0 + "'" + "," + "'" + 0 + "'" + "," + "'" + 0 + "'" + ","
                        + "'" + 0 + "'" + "," + "'" + 0 + "'" + "," + "'" + 0 + "'" + "," + "'" + 0 + "'" + ","
                        + "'" + 0 + "'" + "," + "'" + 0 + "'" + "," + "'" + 0 + "'" + "," + "'" + 0 + "'" + ","
                        + "'" + 0 + "'" + "," + "'" + 0 + "'" + "," + "'" + 0 + "'" + "," + "'" + 0 + "'" + ","
                        + "'" + 0 + "'" + "," + "'" + 0 + "'" + "," + "'" + 0 + "'" + "," + "'" + 0 + "'" + ","
                        + "'" + 0 + "'" + "," + "'" + 0 + "'"
                        + ")";
                        Log.WriteLog(InsertStr2);
                        SQL.ExecuteUpdate(InsertStr2);
                    }
                    
                    if (Global.Selete_Status)
                    {
                        Global.Selete_Status = false;
                        Thread.Sleep(10000);
                        ShowLvFixtrues();
                    }
                    else
                    { ShowLvFixtrues(); }

                    if (Global.itm_DT != DT)
                    {
                        string selectStr2 = string.Format("select * from FixtureCount");
                        DataTable d1 = SQL.ExecuteQuery(selectStr2);
                        int Count = 0;
                        for (int i = 0; i < d1.Rows.Count; i++)
                        {
                            if (d1.Rows[i][3].ToString() == "Y")
                            {
                                Count++;
                            }
                        }
                        if (d1.Rows.Count != Global.Number || Count != Global.Qualified)
                        {

                            Global.Number = d1.Rows.Count;
                            Global.Qualified = Count;
                            string[] str_DtaeTime = new string[] { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "20", "21", "22", "23", "24" };
                            for (int i = 0; i < str_DtaeTime.Length; i++)
                            {
                                if (str_DtaeTime[i] == DT)
                                {
                                    if (int.Parse(DT) > 8)
                                    {
                                        Global.Storage = true;
                                        string update = string.Format("update FixtureNumber set [Hour_{0}]={1},[Hour_re{2}]={3} where [DateTime] = '{4}'", (int.Parse(DT) - 9), d1.Rows.Count, (int.Parse(DT) - 9), Count, DateTime.Now.ToString("yyyy-MM-dd"));
                                        Log.WriteLog(update);
                                        SQL.ExecuteUpdate(update);
                                        //显示数据
                                        ShowLvFixtrues();
                                    }
                                    else
                                    {
                                        Global.Storage = true;
                                        string update = string.Format("update FixtureNumber set [Hour_{0}]={1},[Hour_re{2}]={3} where [DateTime] = '{4}'", (int.Parse(DT) + 15), d1.Rows.Count, (int.Parse(DT) + 15), Count, DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd"));
                                        Log.WriteLog(update);
                                        SQL.ExecuteUpdate(update);
                                        //显示数据
                                        ShowLvFixtrues();
                                    }
                                }
                            }
                        }
                        Global.Storage = false;
                        try
                        {                           
                            string str = "delete from [ZHH].[dbo].[FixtureCount]";
                            SQL.ExecuteUpdate(str);
                            Log.WriteLog("整点小时治具除");
                            Global.itm_DT = DT;
                            Global.inidata.productconfig.Itm_DT = DT;
                        }                        
                        catch (Exception ex) { Log.WriteLog(ex.ToString()); }
                        
                    }
                    if (DT != "8" && Global.Int_Setpint == 1)
                    {
                        Global.Int_Setpint = 0;
                    }

                }
                catch (Exception ex)
                { Log.WriteLog(ex.ToString()); }
                Thread.Sleep(10000);
            }
            
        }

        public void UpdateDataGridView_Fixture_usingtimes(string fixture)//治具使用次数加1
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new ShowDataGridView2(UpdateDataGridView_Fixture_usingtimes), new object[] { fixture });
                return;
            }
            try
            {
                update_fixtures(fixture); //判断次数与设定值              

                for (int i = 0; i < grid_AllFixture.RowCount - 1; i++)
                {
                    Log.WriteLog("治具校验"+"    " +fixture  + "  " + grid_AllFixture.Rows[i].Cells[1].Value.ToString());
                    if (fixture == grid_AllFixture.Rows[i].Cells[1].Value.ToString())
                    {
                        grid_AllFixture.Rows[i].Cells[4].Value = Convert.ToInt32(grid_AllFixture.Rows[i].Cells[4].Value) + 1;
                        if (Convert.ToInt32(grid_AllFixture.Rows[i].Cells[4].Value) >= Global.Fixture_maintance_times && rb_fixturetimes.Checked)//当两小时治具保养卡条件为保养次数时
                        {
                            grid_AllFixture.Rows[i].Cells[5].Value = "逾期未保养";
                            grid_AllFixture.Rows[i].Cells[5].Style.BackColor = Color.Red;
                            if (!Global._fixture_ng.Contains(fixture))
                            {
                                Global._fixture_ng.Add(fixture);//待保养治具写入list中
                            }
                            Txt.WriteLine2(Global._fixture_ng);//待保养治具写入TXT中
                            Log.WriteLog("治具自动排出" + fixture + "使用次数为" + grid_AllFixture.Rows[i].Cells[4].Value);
                            Log.WriteLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + fixture + "," + "治具自动排出", @"D:\ZHH\治具小保养记录\");
                            string insertStr1 = string.Format("UPDATE [FixtureStatus] SET  [UsingTimes] = [UsingTimes] + 1 ,[Status] = '逾期未保养' where [FixtureID] = '{0}'", fixture);
                            int r1 = SQL.ExecuteUpdate(insertStr1);
                        }
                        if ((DateTime.Now - Convert.ToDateTime(grid_AllFixture.Rows[i].Cells[3].Value)).Hours > 2 && rb_fixtruetime.Checked)//当治具保养时间超过两小时且设定为依时间保养时
                        {
                            grid_AllFixture.Rows[i].Cells[5].Value = "逾期未保养";
                            grid_AllFixture.Rows[i].Cells[5].Style.BackColor = Color.Red;
                            if (!Global._fixture_ng.Contains(fixture))
                            {
                                Global._fixture_ng.Add(fixture);//待保养治具写入list中
                            }
                            Txt.WriteLine2(Global._fixture_ng);//待保养治具写入TXT中
                            Log.WriteLog("治具自动排出" + fixture + "保养时间为" + grid_AllFixture.Rows[i].Cells[2].Value);
                            Log.WriteLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + fixture + "," + "治具自动排出", @"D:\ZHH\治具小保养记录\");
                            string insertStr1 = string.Format("UPDATE [FixtureStatus] SET  [UsingTimes] = [UsingTimes] + 1 ,[Status] = '逾期未保养' where [FixtureID] = '{0}'", fixture);
                            int r1 = SQL.ExecuteUpdate(insertStr1);
                        }
                        else
                        {
                            string insertStr2 = string.Format("UPDATE [FixtureStatus] SET [UsingTimes] = [UsingTimes] + 1  where [FixtureID] = '{0}'", fixture);
                            int r2 = SQL.ExecuteUpdate(insertStr2);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.WriteLog("更新DataGridView治具使用计数异常" + ex.ToString());
            }
        }
        public void UpdateDataGridView_FixtureOK(string fixture)//焊接OK时连续抛料计数清零
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new ShowDataGridView2(UpdateDataGridView_FixtureOK), new object[] { fixture });
                return;
            }
            try
            {
                for (int i = 0; i < grid_TossingFixture.RowCount; i++)
                {
                    if (grid_TossingFixture.Rows[i].Cells[0].Value.ToString() == fixture)
                    {
                        grid_TossingFixture.Rows[i].Cells[2].Value = 0;
                        string insertStr1 = string.Format("UPDATE [FixtureTossing] SET [TossingContinuation] = '{0}'  where [Fixture] = '{1}'", "0", fixture);
                        int r1 = SQL.ExecuteUpdate(insertStr1);
                        Log.WriteLog("更新抛料治具SQL语句" + insertStr1);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.WriteLog("更新DataGridView治具抛料计数异常" + ex.ToString());
            }

        }

        public void UpdateDataGridView_FixtureNG(string fixture)//焊接NG时连续抛料计数和抛料总数加1
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new ShowDataGridView2(UpdateDataGridView_FixtureNG), new object[] { fixture });
                return;
            }
            try
            {
                for (int i = 0; i < grid_TossingFixture.RowCount; i++)
                {
                    if (fixture == grid_TossingFixture.Rows[i].Cells[0].Value.ToString())
                    {
                        grid_TossingFixture.Rows[i].Cells[2].Value = Convert.ToInt16(grid_TossingFixture.Rows[i].Cells[2].Value) + 1;
                        grid_TossingFixture.Rows[i].Cells[3].Value = Convert.ToInt16(grid_TossingFixture.Rows[i].Cells[3].Value) + 1;
                        if (Convert.ToInt16(grid_TossingFixture.Rows[i].Cells[2].Value) >= Global.Fixture_ContinuationNG)
                        {
                            grid_TossingFixture.Rows[i].Cells[4].Value = "NG";
                            grid_TossingFixture.Rows[i].Cells[4].Style.BackColor = Color.Red;
                            if (!Global._fixture_tossing_ng.Contains(fixture))
                            {
                                Global._fixture_tossing_ng.Add(fixture);
                                Txt.WriteLine3(Global._fixture_tossing_ng);
                            }
                        }
                        if (Convert.ToInt16(grid_TossingFixture.Rows[i].Cells[3].Value) >= Global.Fixture_CountNG)
                        {
                            grid_TossingFixture.Rows[i].Cells[5].Value = "NG";
                            grid_TossingFixture.Rows[i].Cells[5].Style.BackColor = Color.Red;
                            if (!Global._fixture_tossing_ng.Contains(fixture))
                            {
                                Global._fixture_tossing_ng.Add(fixture);
                                Txt.WriteLine3(Global._fixture_tossing_ng);
                            }
                        }
                        string insertStr1 = string.Format("UPDATE [FixtureTossing] SET [TossingTime] = '{0}',[TossingContinuation] = '{1}', [TossingCount] = '{2}',[ContinuationNG] = '{3}',[CountNG] = '{4}' where [Fixture] = '{5}'",
                                            DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), grid_TossingFixture.Rows[i].Cells[2].Value, grid_TossingFixture.Rows[i].Cells[3].Value, grid_TossingFixture.Rows[i].Cells[4].Value, grid_TossingFixture.Rows[i].Cells[5].Value, fixture);
                        Log.WriteLog("更新抛料治具SQL语句" + insertStr1);
                        int r1 = SQL.ExecuteUpdate(insertStr1);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.WriteLog("更新DataGridView治具抛料计数异常" + ex.ToString());
            }

        }
        public void UpdateDataGridView_CountDowm()
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new ShowDataGridView1(UpdateDataGridView_CountDowm), new object[] { });
                return;
            }
            try
            {
                for (int i = 0; i < grid_TossingFixture.RowCount; i++)
                {
                    grid_TossingFixture.Rows[i].Cells[4].Value = (Convert.ToDouble(grid_TossingFixture.Rows[i].Cells[4].EditedFormattedValue) - 1).ToString("0.00");
                    if (Convert.ToDouble(grid_TossingFixture.Rows[i].Cells[4].Value) <= 0)
                    {
                        string fixture = grid_TossingFixture.Rows[i].Cells[1].Value.ToString();
                        string insertStr1 = string.Format("UPDATE [FixtureStatus] SET [CountDown] = {0} ,[Status] = '逾期未保养' where [FixtureID] = '{1}'", grid_TossingFixture.Rows[i].Cells[4].Value, fixture);
                        int r1 = SQL.ExecuteUpdate(insertStr1);
                        if (!Global._fixture_ng.Contains(fixture))
                        {
                            Global._fixture_ng.Add(fixture);//待保养治具写入list中
                        }
                        Txt.WriteLine2(Global._fixture_ng);//待保养治具写入TXT中
                        Log.WriteLog("治具自动排出" + fixture + "使用次数为" + grid_TossingFixture.Rows[i].Cells[3].Value + "使用倒计时为" + grid_TossingFixture.Rows[i].Cells[4].Value);
                        Log.WriteLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + fixture + "," + "治具自动排出", @"D:\ZHH\治具小保养记录\");
                    }
                }
            }
            catch (Exception ex)
            {
                Log.WriteLog("更新DataGridView倒计时列异常" + ex.ToString());
            }

        }
        #endregion

        #region 获取Textbox 控件输入值
        public string Gettextbox(TextBox tb)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new GetTextbox(Gettextbox), new object[] {tb});
                return "";
            }
            return tb.Text;
        }
        public void Showtextbox(TextBox tb,string value)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new ShowTextbox(Showtextbox), new object[] { tb ,value});
                return ;
            }
            tb.Text = value;
        }
        #endregion
        private void grid_AllFixture_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            for (int i = 0; i < grid_AllFixture.RowCount - 1; i++)
            {
                if (grid_AllFixture[5, i].EditedFormattedValue.ToString() == "逾期未保养")
                {
                    grid_AllFixture[5, i].Style.BackColor = Color.Red;
                }
                else
                {
                    grid_AllFixture[5, i].Style.BackColor = Color.Green;
                }
            }
        }

        private void grid_TossingFixture_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            for (int i = 0; i < grid_TossingFixture.RowCount; i++)
            {
                if (grid_TossingFixture[4, i].EditedFormattedValue.ToString() == "NG")
                {
                    grid_TossingFixture[4, i].Style.BackColor = Color.Red;
                }
                else
                {
                    grid_TossingFixture[4, i].Style.BackColor = Color.Green;
                }
                if (grid_TossingFixture[5, i].EditedFormattedValue.ToString() == "NG")
                {
                    grid_TossingFixture[5, i].Style.BackColor = Color.Red;
                }
                else
                {
                    grid_TossingFixture[5, i].Style.BackColor = Color.Green;
                }
            }
        }

        private void list_FixtureNG_SelectedValueChanged(object sender, EventArgs e)
        {
            try
            {
                int startIndex = list_FixtureNG.SelectedItems[0].ToString().IndexOf("治具码:");
                int endIndex = list_FixtureNG.SelectedItems[0].ToString().IndexOf(",NG]");
                if (startIndex != endIndex && endIndex != -1)
                {
                    string fixtrue = list_FixtureNG.SelectedItems[0].ToString().Substring(startIndex, endIndex - startIndex);
                    txt_SelectFixture.Text = fixtrue.Replace("治具码:", "");
                }
            }
            catch
            {

            }
        }

        private void OEE_DT_Click(object sender, EventArgs e)
        {
            Global.b_VerifyResult = true;
            if (!Global.b_VerifyResult)
            {
                MessageBox.Show("Trace参数异常,不能上传OQ数据");
                return;
            }
            Thread th = new Thread(UploadOEE);
            th.IsBackground = true;
            th.Start();
        }
        public void UploadOEE(object obj)
        {
            string band = "";
            GetSNData SP = new GetSNData();
            try
            {
                string callresult = "";
                string errmsg = "";
                
                tb_bandsn.Invoke(new Action(() => { band = tb_bandsn.Text.Remove(18); }));
                JsonSerializerSettings jsetting1 = new JsonSerializerSettings();
                jsetting1.NullValueHandling = NullValueHandling.Ignore;//Json不输出空值
                string url = string.Format("http://17.239.116.110/api/v2/parts?serial_type=band&serial={0}&process_name=bd-bc-le&last_log=true", band);
            
            }
            catch (Exception EX)
            {
                MessageBox.Show("获取SP码失败");
                return;
            }
            //DT状态切换 运行 -> 手选 -> 运行 -> 待料 -> 运行 -> 宕机 -> 运行 -> 人工停止复位 -> 手选 -> 运行
            //DT状态切换 2 (10分钟) -> 5 (10分钟) ->2 (10分钟) ->1 (10分钟) ->2 (10分钟) ->3 (10分钟) ->2 (5分钟) ->5 (10分钟) ->2 (5分钟)
            DateTime time = DateTime.Now;
            var rst = true;
            string sendTopic = Global.inidata.productconfig.EMT + "/upload/downtime";
            string ClientPcName = Dns.GetHostName();
            string IP = _mainparent.GetIp();
            string Mac = _mainparent.GetOEEMac();
            string OEE_DT = "";
            string Trcae_logs_str = string.Empty;
            TraceRespondID Msg = null;
            Guid guid1 = Guid.NewGuid();
            OEE_DT = string.Format("{{\"guid\":\"{0}\",\"EMT\":\"{1}\",\"PoorNum\":\"{2}\",\"TotalNum\":\"{3}\",  \"Status\":\"{4}\",\"ErrorCode\":\"{5}\",\"EventTime\":\"{6}\",\"ModuleCode\":\"{7}\",\"ClientPcName\":\"{8}\",\"MAC\":\"{9}\",\"IP\":\"{10}\"}}", guid1, Global.inidata.productconfig.EMT, "0", "0", "2", "", time.ToString("yyyy-MM-dd HH:mm:ss.fff"), "", ClientPcName, Mac, IP);
            //Global._mqttClient.Publish(sendTopic, Encoding.UTF8.GetBytes(OEE_DT), MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE, false);
            rst = true;
            if (rst)
            {
                if (true)
                {
                    string InsertOEEStr = "insert into OEE_TraceDT([DateTime],[Status],[ErrorCode],[EventTime],[ModuleCode])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," + "'" + "2" + "'" + "," + "'" + "00000000" + "'" + ","
                                            + "'" + time.ToString("yyyy-MM-dd HH:mm:ss.fff") + "'" + "," + "'" + "" + "'" + ")";
                    SQL.ExecuteUpdate(InsertOEEStr);
                    AppendRichText("故障代码=" + "" + ",触发时间=" + time.ToString("yyyy-MM-dd HH:mm:ss.fff") + ",运行状态:" + "2" + ",故障描述:" + "自动运行" + ",模组代码:" + "" + ",自动发送成功", "rtx_OEEMsg");
                    Log.WriteLog(guid1 + "," + Global.inidata.productconfig.EMT + "," + "0" + "," + "0" + "," + "2" + "," + "" + "," + time.ToString("yyyy-MM-dd HH:mm:ss.fff") + "," + "" + "," + ClientPcName + "," + Mac + "," + IP, System.AppDomain.CurrentDomain.BaseDirectory + "\\手动上传OEE_DT\\");
                    Global.respond.TryRemove(guid1,out Global.Respond);
                }
                else
                {
                    AppendRichText("故障代码=" + "" + ",触发时间=" + time.ToString("yyyy-MM-dd HH:mm:ss.fff") + ",运行状态:" + "2" + ",故障描述:" + "自动运行" + ",模组代码:" + "" + ",自动发送失败", "rtx_OEEMsg");
                    AppendRichText("失败原因:" + Global.respond[guid1].ErrorCode + Global.respond[guid1].Result, "rtx_OEEMsg");
                    Global.respond.TryRemove(guid1, out Global.Respond);
                }
            }
            else
            {
                AppendRichText("网络异常,超时无反馈", "rtx_OEEMsg");
            }
            if (true)//手选原因 保持10分钟
            {
                Guid guid = Guid.NewGuid();
                OEE_DT = string.Format("{{\"guid\":\"{0}\",\"EMT\":\"{1}\",\"PoorNum\":\"{2}\",\"TotalNum\":\"{3}\",  \"Status\":\"{4}\",\"ErrorCode\":\"{5}\",\"EventTime\":\"{6}\",\"ModuleCode\":\"{7}\",\"ClientPcName\":\"{8}\",\"MAC\":\"{9}\",\"IP\":\"{10}\"}}", guid, Global.inidata.productconfig.EMT, "0", "0", "5", "7002002", time.AddMinutes(10).ToString("yyyy-MM-dd HH:mm:ss.fff"), "", ClientPcName, Mac, IP);
                //Global._mqttClient.Publish(sendTopic, Encoding.UTF8.GetBytes(OEE_DT), MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE, false);
                rst =true;
                if (rst)
                {
                    if (true)
                    {
                        string InsertOEEStr = "insert into OEE_TraceDT([DateTime],[Status],[ErrorCode],[EventTime],[ModuleCode])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," + "'" + "5" + "'" + "," + "'" + "7002002" + "'" + ","
                                            + "'" + time.AddMinutes(10).ToString("yyyy-MM-dd HH:mm:ss.fff") + "'" + "," + "'" + "" + "'" + ")";
                        SQL.ExecuteUpdate(InsertOEEStr);
                        AppendRichText("故障代码=" + "7002002" + ",触发时间=" + time.AddMinutes(10).ToString("yyyy-MM-dd HH:mm:ss.fff") + ",运行状态:" + "5" + ",故障描述:" + "其他原因设备调试(M)" + ",模组代码:" + "" + ",自动发送成功", "rtx_OEEMsg");
                        Log.WriteLog(guid + "," + Global.inidata.productconfig.EMT + "," + "0" + "," + "0" + "," + "5" + "," + "7002003" + "," + time.AddMinutes(10).ToString("yyyy-MM-dd HH:mm:ss.fff") + "," + "" + "," + ClientPcName + "," + Mac + "," + IP, System.AppDomain.CurrentDomain.BaseDirectory + "\\手动上传OEE_DT\\");
                        Global.respond.TryRemove(guid, out Global.Respond);
                    }
                    else
                    {
                        AppendRichText("故障代码=" + "7002002" + ",触发时间=" + time.AddMinutes(10).ToString("yyyy-MM-dd HH:mm:ss.fff") + ",运行状态:" + "5" + ",故障描述:" + "其他原因设备调试(M)" + ",模组代码:" + "" + ",自动发送失败", "rtx_OEEMsg");
                        AppendRichText("失败原因:" + Global.respond[guid].ErrorCode + Global.respond[guid].Result, "rtx_OEEMsg");
                        Global.respond.TryRemove(guid, out Global.Respond);
                    }
                }
                else
                {
                    AppendRichText("网络异常,超时无反馈", "rtx_OEEMsg");
                }
            }
            if (true)//运行 保持10分钟
            {
                Guid guid = Guid.NewGuid();
                OEE_DT = string.Format("{{\"guid\":\"{0}\",\"EMT\":\"{1}\",\"PoorNum\":\"{2}\",\"TotalNum\":\"{3}\",  \"Status\":\"{4}\",\"ErrorCode\":\"{5}\",\"EventTime\":\"{6}\",\"ModuleCode\":\"{7}\",\"ClientPcName\":\"{8}\",\"MAC\":\"{9}\",\"IP\":\"{10}\"}}", guid, Global.inidata.productconfig.EMT, "0", "0", "2", "", time.AddMinutes(20).ToString("yyyy-MM-dd HH:mm:ss.fff"), "", ClientPcName, Mac, IP);
                //Global._mqttClient.Publish(sendTopic, Encoding.UTF8.GetBytes(OEE_DT), MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE, false);
                rst = true;
                if (rst)
                {
                    if (true)
                    {
                        string InsertOEEStr = "insert into OEE_TraceDT([DateTime],[Status],[ErrorCode],[EventTime],[ModuleCode])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," + "'" + "2" + "'" + "," + "'" + "0000000" + "'" + ","
                                                + "'" + time.AddMinutes(20).ToString("yyyy-MM-dd HH:mm:ss.fff") + "'" + "," + "'" + "" + "'" + ")";
                        SQL.ExecuteUpdate(InsertOEEStr);
                        AppendRichText("故障代码=" + "" + ",触发时间=" + time.AddMinutes(20).ToString("yyyy-MM-dd HH:mm:ss.fff") + ",运行状态:" + "2" + ",故障描述:" + "自动运行" + ",模组代码:" + "" + ",自动发送成功", "rtx_OEEMsg");
                        Log.WriteLog(guid + "," + Global.inidata.productconfig.EMT + "," + "0" + "," + "0" + "," + "2" + "," + "" + "," + time.AddMinutes(20).ToString("yyyy-MM-dd HH:mm:ss.fff") + "," + "" + "," + ClientPcName + "," + Mac + "," + IP, System.AppDomain.CurrentDomain.BaseDirectory + "\\手动上传OEE_DT\\");
                        Global.respond.TryRemove(guid, out Global.Respond);
                    }
                    else
                    {
                        AppendRichText("故障代码=" + "" + ",触发时间=" + time.AddMinutes(20).ToString("yyyy-MM-dd HH:mm:ss.fff") + ",运行状态:" + "2" + ",故障描述:" + "自动运行" + ",模组代码:" + "" + ",自动发送失败", "rtx_OEEMsg");
                        AppendRichText("失败原因:" + Global.respond[guid].ErrorCode + Global.respond[guid].Result, "rtx_OEEMsg");
                        Global.respond.TryRemove(guid, out Global.Respond);
                    }
                }
                else
                {
                    AppendRichText("网络异常,超时无反馈", "rtx_OEEMsg");
                }
            }
            if (true)//待料 保持10分钟
            {
                Guid guid = Guid.NewGuid();
                OEE_DT = string.Format("{{\"guid\":\"{0}\",\"EMT\":\"{1}\",\"PoorNum\":\"{2}\",\"TotalNum\":\"{3}\",  \"Status\":\"{4}\",\"ErrorCode\":\"{5}\",\"EventTime\":\"{6}\",\"ModuleCode\":\"{7}\",\"ClientPcName\":\"{8}\",\"MAC\":\"{9}\",\"IP\":\"{10}\"}}", guid, Global.inidata.productconfig.EMT, "0", "0", "1", "1201002", time.AddMinutes(30).ToString("yyyy-MM-dd HH:mm:ss.fff"), "", ClientPcName, Mac, IP);
                //Global._mqttClient.Publish(sendTopic, Encoding.UTF8.GetBytes(OEE_DT), MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE, false);
                rst =true;
                if (rst)
                {
                    if (true)
                    {
                        string InsertOEEStr = "insert into OEE_TraceDT([DateTime],[Status],[ErrorCode],[EventTime],[ModuleCode])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," + "'" + "1" + "'" + "," + "'" + "1201002" + "'" + ","
                                            + "'" + time.AddMinutes(30).ToString("yyyy-MM-dd HH:mm:ss.fff") + "'" + "," + "'" + "" + "'" + ")";
                        SQL.ExecuteUpdate(InsertOEEStr);
                        AppendRichText("故障代码=" + "1201002" + ",触发时间=" + time.AddMinutes(30).ToString("yyyy-MM-dd HH:mm:ss.fff") + ",运行状态:" + "1" + ",故障描述:" + "HSG待料" + ",模组代码:" + "" + ",自动发送成功", "rtx_OEEMsg");
                        Log.WriteLog(guid + "," + Global.inidata.productconfig.EMT + "," + "0" + "," + "0" + "," + "1" + "," + "12010002" + "," + time.AddMinutes(30).ToString("yyyy-MM-dd HH:mm:ss.fff") + "," + "" + "," + ClientPcName + "," + Mac + "," + IP, System.AppDomain.CurrentDomain.BaseDirectory + "\\手动上传OEE_DT\\");
                        Global.respond.TryRemove(guid, out Global.Respond);
                    }
                    else
                    {
                        AppendRichText("故障代码=" + "1201002" + ",触发时间=" + time.AddMinutes(30).ToString("yyyy-MM-dd HH:mm:ss.fff") + ",运行状态:" + "1" + ",故障描述:" + "HSG待料" + ",模组代码:" + "" + ",自动发送失败", "rtx_OEEMsg");
                        AppendRichText("失败原因:" + Global.respond[guid].ErrorCode + Global.respond[guid].Result, "rtx_OEEMsg");
                        Global.respond.TryRemove(guid, out Global.Respond);
                    }
                }
                else
                {
                    AppendRichText("网络异常,超时无反馈", "rtx_OEEMsg");
                }
            }
            if (true)//运行 保持10分钟
            {
                Guid guid = Guid.NewGuid();
                OEE_DT = string.Format("{{\"guid\":\"{0}\",\"EMT\":\"{1}\",\"PoorNum\":\"{2}\",\"TotalNum\":\"{3}\",  \"Status\":\"{4}\",\"ErrorCode\":\"{5}\",\"EventTime\":\"{6}\",\"ModuleCode\":\"{7}\",\"ClientPcName\":\"{8}\",\"MAC\":\"{9}\",\"IP\":\"{10}\"}}", guid, Global.inidata.productconfig.EMT, "0", "0", "2", "", time.AddMinutes(40).ToString("yyyy-MM-dd HH:mm:ss.fff"), "", ClientPcName, Mac, IP);
                //Global._mqttClient.Publish(sendTopic, Encoding.UTF8.GetBytes(OEE_DT), MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE, false);
                rst = true;
                if (rst)
                {
                    if (true)
                    {
                        string InsertOEEStr = "insert into OEE_TraceDT([DateTime],[Status],[ErrorCode],[EventTime],[ModuleCode])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," + "'" + "2" + "'" + "," + "'" + "0000000" + "'" + ","
                                                + "'" + time.AddMinutes(40).ToString("yyyy-MM-dd HH:mm:ss.fff") + "'" + "," + "'" + "" + "'" + ")";
                        SQL.ExecuteUpdate(InsertOEEStr);
                        AppendRichText("故障代码=" + "" + ",触发时间=" + time.AddMinutes(40).ToString("yyyy-MM-dd HH:mm:ss.fff") + ",运行状态:" + "2" + ",故障描述:" + "自动运行" + ",模组代码:" + "" + ",自动发送成功", "rtx_OEEMsg");
                        Log.WriteLog(guid + "," + Global.inidata.productconfig.EMT + "," + "0" + "," + "0" + "," + "2" + "," + "" + "," + time.AddMinutes(40).ToString("yyyy-MM-dd HH:mm:ss.fff") + "," + "" + "," + ClientPcName + "," + Mac + "," + IP, System.AppDomain.CurrentDomain.BaseDirectory + "\\手动上传OEE_DT\\");
                        Global.respond.TryRemove(guid, out Global.Respond);
                    }
                    else
                    {
                        AppendRichText("故障代码=" + "" + ",触发时间=" + time.AddMinutes(40).ToString("yyyy-MM-dd HH:mm:ss.fff") + ",运行状态:" + "2" + ",故障描述:" + "自动运行" + ",模组代码:" + "" + ",自动发送失败", "rtx_OEEMsg");
                        AppendRichText("失败原因:" + Global.respond[guid].ErrorCode + Global.respond[guid].Result, "rtx_OEEMsg");
                        Global.respond.TryRemove(guid, out Global.Respond);
                    }
                }
                else
                {
                    AppendRichText("网络异常,超时无反馈", "rtx_OEEMsg");
                }
            }
            if (true)//宕机 保持10分钟
            {
                Guid guid = Guid.NewGuid();
                OEE_DT = string.Format("{{\"guid\":\"{0}\",\"EMT\":\"{1}\",\"PoorNum\":\"{2}\",\"TotalNum\":\"{3}\",  \"Status\":\"{4}\",\"ErrorCode\":\"{5}\",\"EventTime\":\"{6}\",\"ModuleCode\":\"{7}\",\"ClientPcName\":\"{8}\",\"MAC\":\"{9}\",\"IP\":\"{10}\"}}", guid, Global.inidata.productconfig.EMT, "0", "0", "3", "60080004", time.AddMinutes(50).ToString("yyyy-MM-dd HH:mm:ss.fff"), "", ClientPcName, Mac, IP);
                //Global._mqttClient.Publish(sendTopic, Encoding.UTF8.GetBytes(OEE_DT), MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE, false);
                rst =true;
                if (rst)
                {
                    if (true)
                    {
                        string InsertOEEStr = "insert into OEE_TraceDT([DateTime],[Status],[ErrorCode],[EventTime],[ModuleCode])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," + "'" + "3" + "'" + "," + "'" + "6008040" + "'" + ","
                                            + "'" + time.AddMinutes(50).ToString("yyyy-MM-dd HH:mm:ss.fff") + "'" + "," + "'" + "" + "'" + ")";
                        SQL.ExecuteUpdate(InsertOEEStr);
                        AppendRichText("故障代码=" + "6008040" + ",触发时间=" + time.AddMinutes(50).ToString("yyyy-MM-dd HH:mm:ss.fff") + ",运行状态:" + "3" + ",故障描述:" + "HSG连续扫码NG" + ",模组代码:" + "" + ",自动发送成功", "rtx_OEEMsg");
                        Log.WriteLog(guid + "," + Global.inidata.productconfig.EMT + "," + "0" + "," + "0" + "," + "3" + "," + "6008040" + "," + time.AddMinutes(50).ToString("yyyy-MM-dd HH:mm:ss.fff") + "," + "" + "," + ClientPcName + "," + Mac + "," + IP, System.AppDomain.CurrentDomain.BaseDirectory + "\\手动上传OEE_DT\\");
                        Global.respond.TryRemove(guid, out Global.Respond);
                    }
                    else
                    {
                        AppendRichText("故障代码=" + "6008040" + ",触发时间=" + time.ToString("yyyy-MM-dd HH:mm:ss.fff") + ",运行状态:" + "3" + ",故障描述:" + "HSG连续扫码NG" + ",模组代码:" + "" + ",自动发送失败", "rtx_OEEMsg");
                        AppendRichText("失败原因:" + Global.respond[guid].ErrorCode + Global.respond[guid].Result, "rtx_OEEMsg");
                        Global.respond.TryRemove(guid, out Global.Respond);
                    }
                }
                else
                {
                    AppendRichText("网络异常,超时无反馈", "rtx_OEEMsg");
                }
            }
            if (true)//运行 保持10分钟
            {
                Guid guid = Guid.NewGuid();
                OEE_DT = string.Format("{{\"guid\":\"{0}\",\"EMT\":\"{1}\",\"PoorNum\":\"{2}\",\"TotalNum\":\"{3}\",  \"Status\":\"{4}\",\"ErrorCode\":\"{5}\",\"EventTime\":\"{6}\",\"ModuleCode\":\"{7}\",\"ClientPcName\":\"{8}\",\"MAC\":\"{9}\",\"IP\":\"{10}\"}}", guid, Global.inidata.productconfig.EMT, "0", "0", "2", "", time.AddMinutes(60).ToString("yyyy-MM-dd HH:mm:ss.fff"), "", ClientPcName, Mac, IP);
                //Global._mqttClient.Publish(sendTopic, Encoding.UTF8.GetBytes(OEE_DT), MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE, false);
                rst = true;
                if (rst)
                {
                    if (true)
                    {
                        string InsertOEEStr = "insert into OEE_TraceDT([DateTime],[Status],[ErrorCode],[EventTime],[ModuleCode])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," + "'" + "2" + "'" + "," + "'" + "0000000" + "'" + ","
                                                + "'" + time.AddMinutes(60).ToString("yyyy-MM-dd HH:mm:ss.fff") + "'" + "," + "'" + "" + "'" + ")";
                        SQL.ExecuteUpdate(InsertOEEStr);
                        AppendRichText("故障代码=" + "" + ",触发时间=" + time.AddMinutes(60).ToString("yyyy-MM-dd HH:mm:ss.fff") + ",运行状态:" + "2" + ",故障描述:" + "自动运行" + ",模组代码:" + "" + ",自动发送成功", "rtx_OEEMsg");
                        Log.WriteLog(guid + "," + Global.inidata.productconfig.EMT + "," + "0" + "," + "0" + "," + "2" + "," + "" + "," + time.AddMinutes(60).ToString("yyyy-MM-dd HH:mm:ss.fff") + "," + "" + "," + ClientPcName + "," + Mac + "," + IP, System.AppDomain.CurrentDomain.BaseDirectory + "\\手动上传OEE_DT\\");
                        Global.respond.TryRemove(guid, out Global.Respond);
                    }
                    else
                    {
                        AppendRichText("故障代码=" + "" + ",触发时间=" + time.AddMinutes(60).ToString("yyyy-MM-dd HH:mm:ss.fff") + ",运行状态:" + "2" + ",故障描述:" + "自动运行" + ",模组代码:" + "" + ",自动发送失败", "rtx_OEEMsg");
                        AppendRichText("失败原因:" + Global.respond[guid].ErrorCode + Global.respond[guid].Result, "rtx_OEEMsg");
                        Global.respond.TryRemove(guid, out Global.Respond);
                    }
                }
                else
                {
                    AppendRichText("网络异常,超时无反馈", "rtx_OEEMsg");
                }
            }
            if (true)//手选原因 保持15分钟
            {
                Guid guid = Guid.NewGuid();
                OEE_DT = string.Format("{{\"guid\":\"{0}\",\"EMT\":\"{1}\",\"PoorNum\":\"{2}\",\"TotalNum\":\"{3}\",  \"Status\":\"{4}\",\"ErrorCode\":\"{5}\",\"EventTime\":\"{6}\",\"ModuleCode\":\"{7}\",\"ClientPcName\":\"{8}\",\"MAC\":\"{9}\",\"IP\":\"{10}\"}}", guid, Global.inidata.productconfig.EMT, "0", "0", "5", "70020003", time.AddMinutes(70).ToString("yyyy-MM-dd HH:mm:ss.fff"), "", ClientPcName, Mac, IP);
                //Global._mqttClient.Publish(sendTopic, Encoding.UTF8.GetBytes(OEE_DT), MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE, false);
                rst = true;
                if (rst)
                {
                    if (true)
                    {
                        string InsertOEEStr = "insert into OEE_TraceDT([DateTime],[Status],[ErrorCode],[EventTime],[ModuleCode])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," + "'" + "5" + "'" + "," + "'" + "7002002" + "'" + ","
                                            + "'" + time.AddMinutes(70).ToString("yyyy-MM-dd HH:mm:ss.fff") + "'" + "," + "'" + "" + "'" + ")";
                        SQL.ExecuteUpdate(InsertOEEStr);
                        AppendRichText("故障代码=" + "7002002" + ",触发时间=" + time.AddMinutes(70).ToString("yyyy-MM-dd HH:mm:ss.fff") + ",运行状态:" + "5" + ",故障描述:" + "其他原因设备调试(M)" + ",模组代码:" + "" + ",自动发送成功", "rtx_OEEMsg");
                        Log.WriteLog(guid + "," + Global.inidata.productconfig.EMT + "," + "0" + "," + "0" + "," + "5" + "," + "7002002" + "," + time.AddMinutes(75).ToString("yyyy-MM-dd HH:mm:ss.fff") + "," + "" + "," + ClientPcName + "," + Mac + "," + IP, System.AppDomain.CurrentDomain.BaseDirectory + "\\手动上传OEE_DT\\");
                        Global.respond.TryRemove(guid, out Global.Respond);
                    }
                    else
                    {
                        AppendRichText("故障代码=" + "7002002" + ",触发时间=" + time.AddMinutes(70).ToString("yyyy-MM-dd HH:mm:ss.fff") + ",运行状态:" + "5" + ",故障描述:" + "其他原因设备调试(M)" + ",模组代码:" + "" + ",自动发送失败", "rtx_OEEMsg");
                        AppendRichText("失败原因:" + Global.respond[guid].ErrorCode + Global.respond[guid].Result, "rtx_OEEMsg");
                        Global.respond.TryRemove(guid, out Global.Respond);
                    }
                }
                else
                {
                    AppendRichText("网络异常,超时无反馈", "rtx_OEEMsg");
                }
            }
            if (true)//运行 保持5分钟
            {
                Guid guid = Guid.NewGuid();
                OEE_DT = string.Format("{{\"guid\":\"{0}\",\"EMT\":\"{1}\",\"PoorNum\":\"{2}\",\"TotalNum\":\"{3}\",  \"Status\":\"{4}\",\"ErrorCode\":\"{5}\",\"EventTime\":\"{6}\",\"ModuleCode\":\"{7}\",\"ClientPcName\":\"{8}\",\"MAC\":\"{9}\",\"IP\":\"{10}\"}}", guid, Global.inidata.productconfig.EMT, "0", "0", "2", "", time.AddMinutes(85).ToString("yyyy-MM-dd HH:mm:ss.fff"), "", ClientPcName, Mac, IP);
                //Global._mqttClient.Publish(sendTopic, Encoding.UTF8.GetBytes(OEE_DT), MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE, false);
                rst = true;
                if (rst)
                {
                    if (true)
                    {
                        string InsertOEEStr = "insert into OEE_TraceDT([DateTime],[Status],[ErrorCode],[EventTime],[ModuleCode])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," + "'" + "2" + "'" + "," + "'" + "0000000" + "'" + ","
                                                + "'" + time.AddMinutes(85).ToString("yyyy-MM-dd HH:mm:ss.fff") + "'" + "," + "'" + "" + "'" + ")";
                        SQL.ExecuteUpdate(InsertOEEStr);
                        AppendRichText("故障代码=" + "" + ",触发时间=" + time.AddMinutes(85).ToString("yyyy-MM-dd HH:mm:ss.fff") + ",运行状态:" + "2" + ",故障描述:" + "自动运行" + ",模组代码:" + "" + ",自动发送成功", "rtx_OEEMsg");
                        Log.WriteLog(guid + "," + Global.inidata.productconfig.EMT + "," + "0" + "," + "0" + "," + "2" + "," + "" + "," + time.AddMinutes(85).ToString("yyyy-MM-dd HH:mm:ss.fff") + "," + "" + "," + ClientPcName + "," + Mac + "," + IP, System.AppDomain.CurrentDomain.BaseDirectory + "\\手动上传OEE_DT\\");
                        Global.respond.TryRemove(guid, out Global.Respond);
                    }
                    else
                    {
                        AppendRichText("故障代码=" + "" + ",触发时间=" + time.AddMinutes(85).ToString("yyyy-MM-dd HH:mm:ss.fff") + ",运行状态:" + "2" + ",故障描述:" + "自动运行" + ",模组代码:" + "" + ",自动发送失败", "rtx_OEEMsg");
                        AppendRichText("失败原因:" + Global.respond[guid].ErrorCode + Global.respond[guid].Result, "rtx_OEEMsg");
                        Global.respond.TryRemove(guid, out Global.Respond);
                    }
                }
                else
                {
                    AppendRichText("网络异常,超时无反馈", "rtx_OEEMsg");
                }
            }
            if (true)//空跑
            {
                Guid guid = Guid.NewGuid();
                OEE_DT = string.Format("{{\"guid\":\"{0}\",\"EMT\":\"{1}\",\"PoorNum\":\"{2}\",\"TotalNum\":\"{3}\",  \"Status\":\"{4}\",\"ErrorCode\":\"{5}\",\"EventTime\":\"{6}\",\"ModuleCode\":\"{7}\",\"ClientPcName\":\"{8}\",\"MAC\":\"{9}\",\"IP\":\"{10}\"}}", guid, Global.inidata.productconfig.EMT, "0", "0", "5", "70020002", time.AddMinutes(90).ToString("yyyy-MM-dd HH:mm:ss.fff"), "", ClientPcName, Mac, IP);
                //Global._mqttClient.Publish(sendTopic, Encoding.UTF8.GetBytes(OEE_DT), MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE, false);
                rst = true;
                if (rst)
                {
                    if (true)
                    {
                        string InsertOEEStr = "insert into OEE_TraceDT([DateTime],[Status],[ErrorCode],[EventTime],[ModuleCode])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," + "'" + "5" + "'" + "," + "'" + "7002001" + "'" + ","
                                            + "'" + time.AddMinutes(90).ToString("yyyy-MM-dd HH:mm:ss.fff") + "'" + "," + "'" + "" + "'" + ")";
                        SQL.ExecuteUpdate(InsertOEEStr);
                        AppendRichText("故障代码=" + "7002001" + ",触发时间=" + time.AddMinutes(90).ToString("yyyy-MM-dd HH:mm:ss.fff") + ",运行状态:" + "5" + ",故障描述:" + "空跑" + ",模组代码:" + "" + ",自动发送成功", "rtx_OEEMsg");
                        Log.WriteLog(guid + "," + Global.inidata.productconfig.EMT + "," + "0" + "," + "0" + "," + "5" + "," + "7002001" + "," + time.AddMinutes(90).ToString("yyyy-MM-dd HH:mm:ss.fff") + "," + "" + "," + ClientPcName + "," + Mac + "," + IP, System.AppDomain.CurrentDomain.BaseDirectory + "\\手动上传OEE_DT\\");
                        Global.respond.TryRemove(guid, out Global.Respond);
                    }
                    else
                    {
                        AppendRichText("故障代码=" + "7002001" + ",触发时间=" + time.AddMinutes(90).ToString("yyyy-MM-dd HH:mm:ss.fff") + ",运行状态:" + "5" + ",故障描述:" + "空跑" + ",模组代码:" + "" + ",自动发送失败", "rtx_OEEMsg");
                        AppendRichText("失败原因:" + Global.respond[guid].ErrorCode + Global.respond[guid].Result, "rtx_OEEMsg");
                        Global.respond.TryRemove(guid, out Global.Respond);
                    }
                }
                else
                {
                    AppendRichText("网络异常,超时无反馈", "rtx_OEEMsg");
                }
            }
            //JsonSerializerSettings jsetting = new JsonSerializerSettings();
            //jsetting.NullValueHandling = NullValueHandling.Ignore;//Json不输出空值
            //TraceMesRequest_ua trace_ua = new TraceMesRequest_ua();
            //trace_ua.serials = new SN();
            //trace_ua.data = new data();
            //trace_ua.data.insight = new Insight();
            //trace_ua.data.insight.test_attributes = new Test_attributes();
            //trace_ua.data.insight.test_station_attributes = new Test_station_attributes();
            //trace_ua.data.insight.uut_attributes = new Uut_attributes();
            //trace_ua.data.insight.results = new Result[2];
            //for (int i = 0; i < trace_ua.data.insight.results.Length; i++)
            //{
            //    trace_ua.data.insight.results[i] = new Result();
            //}
            //trace_ua.data.items = new ExpandoObject();
            //trace_ua.serials.band = band;
            //trace_ua.data.insight.test_attributes.test_result = "fail";
            //trace_ua.data.insight.test_attributes.unit_serial_number = band;
            //trace_ua.data.insight.test_attributes.uut_start = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            //trace_ua.data.insight.test_attributes.uut_stop = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            //trace_ua.data.insight.test_station_attributes.fixture_id = "H-76HO-SMA40-2200-A-00003";
            //trace_ua.data.insight.test_station_attributes.head_id = "1";            

            //trace_ua.data.insight.uut_attributes.STATION_STRING = "";
            //trace_ua.data.insight.uut_attributes.fixture_id = "H-76HO-SMA40-2200-A-00003";
            //trace_ua.data.insight.uut_attributes.precitec_grading = "0";
            //trace_ua.data.insight.uut_attributes.precitec_rev = "0";
            //trace_ua.data.insight.uut_attributes.pattern_type = "1";
            //trace_ua.data.insight.uut_attributes.precitec_value = "40";
            //trace_ua.data.insight.uut_attributes.hatch = "0.04";
            //trace_ua.data.insight.uut_attributes.spot_size = "0.27";
            //trace_ua.data.insight.uut_attributes.swing_amplitude = "0.02";
            //trace_ua.data.insight.uut_attributes.swing_freq = "10000";
            //trace_ua.data.insight.uut_attributes.full_sn = band;
            //trace_ua.data.insight.uut_attributes.tossing_item = "location1 CCD NG/location2 CCD NG";
            //trace_ua.data.insight.uut_attributes.laser_start_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            //trace_ua.data.insight.uut_attributes.laser_stop_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            //trace_ua.data.insight.results[0].result = "pass";
            //trace_ua.data.insight.results[0].test = "laser_sensor";
            //trace_ua.data.insight.results[0].units = "";
            //trace_ua.data.insight.results[0].value = "";
            //trace_ua.data.insight.results[1].result = "pass";
            //trace_ua.data.insight.results[1].test = "locationx_locationname_layerx";
            //trace_ua.data.insight.results[1].units = "";
            //trace_ua.data.insight.results[1].value = "0";           
            JsonSerializerSettings jsetting = new JsonSerializerSettings();
            jsetting.NullValueHandling = NullValueHandling.Ignore;//Json不输出空值

            TraceMesRequest_ua trace_ua = new TraceMesRequest_ua();
            trace_ua.serials = new SN();
            trace_ua.data = new data();
            trace_ua.data.insight = new Insight();
            trace_ua.data.insight.test_attributes = new Test_attributes();
            trace_ua.data.insight.test_station_attributes = new Test_station_attributes();
            trace_ua.data.insight.uut_attributes = new Uut_attributes();
            trace_ua.data.insight.results = new Result[1];
            for (int i = 0; i < trace_ua.data.insight.results.Length; i++)
            {
                trace_ua.data.insight.results[i] = new Result();
            }
            trace_ua.data.items = new ExpandoObject();
            trace_ua.serials.band = band;
            //test_attributes
            trace_ua.data.insight.test_attributes.test_result = "fail";
            trace_ua.data.insight.test_attributes.unit_serial_number = band;//17位
            trace_ua.data.insight.test_attributes.uut_start = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            trace_ua.data.insight.test_attributes.uut_stop = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            //test_station_attributes
            trace_ua.data.insight.test_station_attributes.fixture_id = "H-76HO-SMA40-2200-A-00003";
            trace_ua.data.insight.test_station_attributes.head_id = "1";
            trace_ua.data.insight.test_station_attributes.line_id = Global.inidata.productconfig.Line_id_la;
            trace_ua.data.insight.test_station_attributes.software_name = Global.inidata.productconfig.Sw_name_la;
            trace_ua.data.insight.test_station_attributes.software_version = Global.inidata.productconfig.Sw_version;
            trace_ua.data.insight.test_station_attributes.station_id = Global.inidata.productconfig.Station_id_la;

            //uut_attributes
            trace_ua.data.insight.uut_attributes.STATION_STRING = string.Format("{{\"ActualCT \":\"{0}\",\"ScanCount \":\"{1}\"}}", "10", "1");
            trace_ua.data.insight.uut_attributes.full_sn = band;
            trace_ua.data.insight.uut_attributes.fixture_id = "XXXXX";
            trace_ua.data.insight.uut_attributes.laser_start_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            trace_ua.data.insight.uut_attributes.laser_stop_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            trace_ua.data.insight.uut_attributes.pattern_type = "1";
            trace_ua.data.insight.uut_attributes.spot_size = "0.45";
            trace_ua.data.insight.uut_attributes.hatch = "0.05";
            trace_ua.data.insight.uut_attributes.swing_amplitude = "0.2";
            trace_ua.data.insight.uut_attributes.swing_freq = "10000";
            trace_ua.data.insight.uut_attributes.tossing_item = "NG";

            //trace_ua.data.insight.results[0].result = "pass";
            //trace_ua.data.insight.results[0].test = "riveting_depth_layerx";
            //trace_ua.data.insight.results[0].units = "mm";
            //trace_ua.data.insight.results[0].value = "NA";
            trace_ua.data.insight.results[0].result = "fail";
            trace_ua.data.insight.results[0].test = "laser_sensor";
            trace_ua.data.insight.results[0].units = "";
            trace_ua.data.insight.results[0].value = "0";

            
            
            string SendTraceLogs = JsonConvert.SerializeObject(trace_ua, Formatting.None, jsetting);
            Log.WriteLog(SendTraceLogs);
            var Trcae_logs_result = RequestAPI2.Trcae_logs(Global.inidata.productconfig.Trace_Logs_LA, SendTraceLogs, out Trcae_logs_str, out Msg);
            if (Trcae_logs_result)
            {
                AppendRichText("Trace_Tab手动上传结果:" + Trcae_logs_result, "rtx_OEEMsg");                
               AppendRichText("Trace_Tab手动上传结果:" + band +"  " + JsonConvert.SerializeObject(Msg), "rtx_OEEMsg");

            }
            else
            {
                AppendRichText("Trace手动上传结果:false", "rtx_OEEMsg");
            }


            trace_ua.data.insight.test_station_attributes.line_id = Global.inidata.productconfig.Line_id_ua;
            trace_ua.data.insight.test_station_attributes.software_name = Global.inidata.productconfig.Sw_name_ua;
            trace_ua.data.insight.test_station_attributes.software_version = Global.inidata.productconfig.Sw_version;
            trace_ua.data.insight.test_station_attributes.station_id = Global.inidata.productconfig.Station_id_ua;
            string SelectStr = "SELECT * FROM OEE_TraceDT";//sql查询语句
            DataTable d1 = SQL.ExecuteQuery(SelectStr);
            if (d1 != null && d1.Rows.Count > 0)
            {
                for (int i = 0; i < d1.Rows.Count; i++)
                {
                    (trace_ua.data.items as ICollection<KeyValuePair<string, object>>).Add(new KeyValuePair<string, object>("error_" + (i + 1), d1.Rows[i][3].ToString() + "_" + (Convert.ToDateTime(d1.Rows[i][4])).ToString("yyyy-MM-dd HH:mm:ss")));
                }
            }
            SendTraceLogs = JsonConvert.SerializeObject(trace_ua, Formatting.None, jsetting);            
            Trcae_logs_result = RequestAPI2.Trcae_logs(Global.inidata.productconfig.Trace_Logs_UA, SendTraceLogs, out Trcae_logs_str, out Msg);
            if (Trcae_logs_result)
            {
                AppendRichText("Trace_Trench手动上传结果:" + Trcae_logs_result, "rtx_OEEMsg");
                AppendRichText("Trace_Trench手动上传结果:" + band + "  " + JsonConvert.SerializeObject(Msg), "rtx_OEEMsg");


            }
            else
            {
                AppendRichText("Trace_Trench手动上传结果:false", "rtx_OEEMsg");
            }

            string DeleteStr = string.Format("delete from [ZHH].[dbo].[OEE_TraceDT] where ID  in(select top {0} ID  from  [ZHH].[dbo].[OEE_TraceDT] order  by  ID  asc)", d1.Rows.Count);
            SQL.ExecuteUpdate(DeleteStr);
            Log.WriteLog(SendTraceLogs);
        }

        private void dtp_SelectFixture_ValueChanged(object sender, EventArgs e)
        {
            Global.SelectFixturetime = Convert.ToDateTime(dtp_SelectFixture.Text);
        }

        private void btn_csvPath_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog fbd = new FolderBrowserDialog())
            {
                if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    tb_outPath.Text = fbd.SelectedPath;
                    fbd.Dispose();
                }
            }
        }

        private void btn_outcsv_Click(object sender, EventArgs e)
        {
            try
            {
                string[] values = new string[40];
                string text = string.Empty;                
                string path3 = tb_outPath.Text + "\\" + tb_csvName.Text + ".csv";               

                if (tb_outPath.Text == null || tb_csvName.Text == null)
                {
                    MessageBox.Show("导出路径与名称不能为空");
                    return;
                }

                string D_dataName = string.Empty;

                D_dataName = "SP上料(B)_入盘升降轴底端坐标,SP上料(B)_入盘升降轴首盘坐标,SP上料(B)_盘横移轴当前坐标,SP上料(B)_盘横移轴当前转矩%,SP上料(B)_盘横移轴RUN速度,SP上料(B)_盘横移轴取盘坐标,SP上料(B)_盘横移轴完成坐标,SP上料(B)_盘横移轴取料坐标,SP上料(B)_出盘升降轴完成坐标,SP上料(B)_出盘升降轴首盘坐标,SP上料(B)_出盘升降轴取盘坐标,SP上料(B)_取料Y轴Y原始坐标,SP上料(B)_取料Y轴取SP坐标,SP上料(B)_取料Y轴放SP坐标,SP上料(B)_取料Y轴CCD坐标,SP上料(B)_取料Z轴原始坐标,SP上料(B)_取料Z轴取SP坐标,SP上料(B)_取料Z轴放SP坐标,SP上料(B)_取料Z轴CCD坐标,组装机(A)_取料Y轴原始坐标,组装机(A)_取料Y轴取料坐标,组装机(A)_取料Y轴扫码1坐标,组装机(A)_取料Y轴放料1坐标,组装机(A)_取料Y轴检测1坐标,组装机(A)_取料Y轴扫码2坐标,组装机(A)_取料Y轴放料2坐标,组装机(A)_取料Y轴检测2坐标,组装机(A)_取料Y轴NG排料坐标,组装机(A)_取料Z轴原始坐标,组装机(A)_取料Z轴取料坐标,组装机(A)_取料Z轴扫码1坐标,组装机(A)_取料Z轴放料1坐标,组装机(A)_取料Z轴检测1坐标,组装机(A)_取料Z轴扫码2坐标,组装机(A)_1#载具运送轴原始坐标,组装机(A)_1#载具运送轴进出坐标,组装机(A)_1#载具运送轴盖板坐标,组装机(A)_1#载具运送轴扫码坐标,组装机(A)_1#载具运送轴放料坐标,组装机(A)_1#载具运送轴检测坐标,";
                D_dataName += "组装机(A)_1#载具运送轴放SP坐标,组装机(A)_2#载具运送轴原始坐标,组装机(A)_2#载具运送轴进出坐标,组装机(A)_2#载具运送轴盖板坐标,组装机(A)_2#载具运送轴扫码坐标,组装机(A)_2#载具运送轴放料坐标,组装机(A)_2#载具运送轴检测坐标,组装机(A)_2#载具运送轴放SP坐标,组装机(A)_搬载具轴原始坐标,组装机(A)_搬载具轴定位台坐标,组装机(A)_搬载具轴夹爪1载具1坐,";
                D_dataName += "组装机(A)_搬载具轴夹爪1载具2坐,组装机(A)_搬载具轴夹爪2载具1坐,组装机(A)_搬载具轴夹爪2载具2坐,组装机(A)_搬载具轴爪1抓NG载具坐,1#焊接机(C)_模组1X轴原始坐标,1#焊接机(C)_模组1X轴接载具坐标,1#焊接机(C)_模组1X轴焊接1坐标,1#焊接机(C)_模组1X轴焊接2坐标,1#焊接机(C)_模组1X轴焊接3坐标,1#焊接机(C)_模组1X轴焊接4坐标,1#焊接机_模组1X轴焊接5坐标,1#焊接机(C)_模组1Y轴原始坐标,1#焊接机(C)_模组1Y轴接载具坐标,1#焊接机(C)_模组1Y轴焊接1坐标,1#焊接机(C)_模组1Y轴焊接2坐标,1#焊接机(C)_模组1Y轴焊接3坐标,";
                D_dataName += "1#焊接机(C)_模组1Y轴焊接4坐标,1#焊接机_模组1Y轴焊接5坐标,1#焊接机(C)_模组1Z轴原始坐标,1#焊接机(C)_模组1Z轴接载具坐标,1#焊接机(C)_模组1Z轴焊接1坐标,1#焊接机(C)_模组1Z轴焊接2坐标,1#焊接机(C)_模组1Z轴焊接3坐标,1#焊接机(C)_模组1Z轴焊接4坐标,1#焊接机(C)_模组1Z轴焊接5坐标,1#焊接机(C)_模组2X轴原始坐标,1#焊接机(C)_模组2X轴接载具坐标,1#焊接机(C)_模组2X轴焊接1坐标,1#焊接机(C)_模组2X轴焊接2坐标,1#焊接机(C)_模组2X轴焊接3坐标,1#焊接机(C)_模组2X轴焊接4坐标,1#焊接机(C)_模组2X轴焊接5坐标,";
                D_dataName += "1#焊接机(C)_模组2Y轴原始坐标,1#焊接机(C)_模组2Y轴接载具坐标,1#焊接机(C)_模组2Y轴焊接1坐标,1#焊接机(C)_模组2Y轴焊接2坐标,1#焊接机(C)_模组2Y轴焊接3坐标,1#焊接机(C)_模组2Y轴焊接4坐标,1#焊接机_模组2Y轴焊接5坐标,1#焊接机(C)_模组2Z轴原始坐标,1#焊接机(C)_模组2Z轴接载具坐标,1#焊接机(C)_模组2Z轴焊接1坐标,1#焊接机(C)_模组2Z轴焊接2坐标,1#焊接机(C)_模组2Z轴焊接3坐标,1#焊接机(C)_模组2Z轴焊接4坐标,1#焊接机_模组2Z轴焊接5坐标,XYZU模组X轴原始坐标,XYZU模组X轴取进出载具位坐标,XYZU模组X轴取定位台载具坐标,";
                D_dataName += "XYZU模组X轴取模组1载具坐标,XYZU模组X轴放模组1载具坐标,XYZU模组X轴取模组2载具坐标,XYZU模组X轴放载具2坐标,XYZU模组X轴放进出载具位坐标,XYZU模组X轴放定位台坐标,XYZU模组Y轴放载具进出位坐标,XYZU模组Y轴放定位台坐标,XYZU模组Y轴原始位坐标,XYZU模组Y轴主机侧旋转位坐标,XYZU模组Y轴取载具进出位坐标,XYZU模组Y轴取定位台坐标,";
                D_dataName += "XYZU模组Y轴焊机侧旋转位坐标,XYZU模组Y轴取模组1坐标,XYZU模组Y轴取模组2坐标,XYZU模组Y轴放模组1坐标,XYZU模组Y轴放模组2坐标,XYZU模组Z轴放进出载具坐标,XYZU模组Z轴放定位台坐标,XYZU模组Z轴原始坐标,XYZU模组Z轴主机侧上升点,XYZU模组Z轴取进出载具坐标,XYZU模组Z轴取定位台坐标,XYZU模组Z轴焊机侧上升点,XYZU模组Z轴取模组1坐标,XYZU模组Z轴取模组2坐标,XYZU模组Z轴放模组1坐标,XYZU模组Z轴放模组2坐标,";
                D_dataName += "XYZU模组U轴放模组2旋转2位坐标,XYZU模组U轴原始坐标,XYZU模组U轴主机侧旋转位坐标,XYZU模组U轴取模组1位坐标,XYZU模组U轴取模组2位坐标,XYZU模组U轴放模组1位坐标,XYZU模组U轴原始位置相对坐标,XYZU模组U轴主机侧旋转位相对坐标,XYZU模组U轴取模组1位相对坐标,XYZU模组U轴取模组2位相对坐标,XYZU模组U轴放模组1位相对坐标,XYZU模组U轴放模组2位相对坐标,主机2-5》Tray盘拍照X轴初始位,主机2-5》Tray盘拍照X轴避让位,主机2-5》Tray盘拍照X轴拍照位,主机2-5》Tray盘X轴拍照计算位,";
                D_dataName += "主机2-6》Tray盘拍照Y轴初始位,主机2-6》Tray盘拍照Y轴避让位1,主机2-6》Tray盘拍照Y轴拍照位1,主机2-6》Tray盘Y轴拍照计算位,主机2-7》Tray盘升降轴接盘位,主机2-7》Tray盘升降轴准备位,主机2-7》Tray盘升降轴夹盘首位,主机2-7》Tray盘升降轴止退首位,主机2-7》Tray盘升降轴夹盘计算,主机2-7》Tray盘升降轴放横移位,主机2-8》Tray盘横移轴初始位,主机2-8》Tray盘横移轴取盘位,主机2-8》Tray盘横移轴工作位,主机2-8》Tray盘横移轴避让位,";
                //读PLC D区坐标数据
                string[] ArrD_dataName = D_dataName.Split(',');


                double[] D_data1 = Global.PLC_Client.ReadPLC_DDD(20000,640); //坐标           
                System.IO.StreamWriter sw = null;
                if (!File.Exists(path3))
                {
                    using (sw = new StreamWriter(path3, true, Encoding.Default))
                    {
                        string value = "时间,D坐标名,坐标值";
                        sw.WriteLine(value);
                    }
                }
                lock (_lock)
                {
                    try
                    {
                        using (sw = new StreamWriter(path3, true, Encoding.Default))
                        {
                            int J = 0;
                            for (int i = 0; i < ArrD_dataName.Length; i++)
                            {
                                text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + ArrD_dataName[i] + "," + D_data1[i];//(20000 + J).ToString() + "," +
                                sw.WriteLine(text);
                                
                            }
                            
                        }
                        sw.Close();
                        sw.Dispose();
                    }
                    catch
                    {
                    }
                }
            }
            catch (Exception ex)
            {

                Log.WriteLog("关键参数导出异常:" + ex.ToString());
            }
        }

        private void grid_AllFixture_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            Global.DataGridView_Select_RowIndex = e.RowIndex;
        }

        private void lb_TraceLAOK_TextChanged(object sender, EventArgs e)
        {
            Global.Trace_ua2_ok = Convert.ToInt32(lb_TraceLAOK.Text);
            Global.inidata.productconfig.trace_ua2_ok = lb_TraceLAOK.Text;
            Global.inidata.WriteProductnumSection();
        }

        private void lb_TraceLANG_TextChanged(object sender, EventArgs e)
        {
            Global.Trace_ua2_ng = Convert.ToInt32(lb_TraceLANG.Text);
            Global.inidata.productconfig.trace_ua2_ng = lb_TraceLANG.Text;
            Global.inidata.WriteProductnumSection();
        }
        /// <summary>
        /// 标准治具数量设定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button20_Click(object sender, EventArgs e)
        {
            Global.inidata.productconfig.FixtureCount =domainUpDownFixturecount.Text;
            Global.inidata.WriteProductConfigSection();
            if (labelFixtureSjCount.Text!=domainUpDownFixturecount.Text)
            {
                labelFixtureSjCount.BackColor = Color.Red;

            }
            else
            {
                labelFixtureSjCount.BackColor = Color.Green;
            }
        }
        

        private void selete_fixtrue_Click(object sender, EventArgs e)
        {
            string selete = "select * from FixtureNumber where Datetime ='" + DateTime.Parse(dateTimePicker1.Text).ToString("yyyy-MM-dd") + "'";
            DataTable d1 = SQL.ExecuteQuery(selete);
            if (d1.Rows.Count > 0)
            {
                dataTableToListView_fixtrue(lv_Fixtures, d1);
            }
            else
            {
                lv_Fixtures.View = View.Details;
                lv_Fixtures.GridLines = true;//显示网格线
                lv_Fixtures.Items.Clear();//所有的项
                lv_Fixtures.Columns.Clear();//标题
            }
            Global.Selete_Status = true;
        }

        private void Fixtures_outcsv_Click(object sender, EventArgs e)
        {
            try
            {
                out_fixtrueCSV();
            }
            catch (Exception ex)
            {
                Log.WriteLog(ex.ToString());
                MessageBox.Show(ex.ToString());
            }            
          
        }
        private void out_fixtrueCSV()
        {
            try
            {
                //导出今日
                //(Convert.ToDateTime(DateTime.Now.ToString("yyyyMMdd")).CompareTo(Convert.ToDateTime(dateTimePicker1.Text.ToString()).ToString("yyyyMMdd"))) >= 0)//
                if (DateTime.Now.Date.ToString("yyyyMMdd") == DateTime.Parse(dateTimePicker1.Text).ToString("yyyyMMdd"))
                {
                    SaveFileDialog sf = new SaveFileDialog();
                    sf.Title = "治具数量导出";
                    sf.Filter = "文档(*.csv)|*.csv";
                    sf.FileName = DateTime.Now.Date.ToString("yyyyMMdd");
                    if (sf.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        string str = sf.FileName;
                        using (StreamWriter sw = new StreamWriter(str, false, Encoding.Default))
                        {
                            try
                            {
                                sw.WriteLine("序号,时间段,治具SN不相同的数量,治具SN重复次数>设定值的数量");
                                for (int i = 0; i < lv_Fixtures.Items.Count; i++)
                                {
                                    string oeestr = "";
                                    for (int t = 0; t < lv_Fixtures.Columns.Count; t++)
                                    {
                                        oeestr += lv_Fixtures.Items[i].SubItems[t].Text.ToString().Replace("[", ",") + ",";
                                    }
                                    sw.WriteLine(oeestr);
                                }

                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, "导出错误", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            sw.Close();
                            sw.Dispose();
                        }
                    }
                }
                else
                {
                    SaveFileDialog sf = new SaveFileDialog();
                    sf.Title = "治具数量导出";
                    sf.Filter = "文档(*.csv)|*.csv";
                    sf.FileName = DateTime.Parse(dateTimePicker1.Text).ToString("yyyyMMdd");
                    if (sf.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        string str = sf.FileName;
                        using (StreamWriter sw = new StreamWriter(str, false, Encoding.Default))
                        {
                            try
                            {
                                string selete = "select * from FixtureNumber where Datetime ='" + DateTime.Parse(dateTimePicker1.Text).ToString("yyyy-MM-dd") + "'";
                                DataTable d1 = SQL.ExecuteQuery(selete);
                                DataTable dt2 = new DataTable();
                                DataColumn dataColumn = new DataColumn("序号");
                                DataColumn dataColumn1 = new DataColumn("时间段");
                                DataColumn dataColumn2 = new DataColumn("治具SN不相同的数量");
                                DataColumn dataColumn3 = new DataColumn("治具SN重复次数>设定值的数量");
                                dt2.Columns.Add(dataColumn);
                                dt2.Columns.Add(dataColumn1);
                                dt2.Columns.Add(dataColumn2);
                                dt2.Columns.Add(dataColumn3);
                                string[] DateTime_fixtrue = { "08:00-09:00", "09:00-10:00", "10:00-11:00", "11:00-12:00", "12:00-13:00", "13:00-14:00", "14:00-15:00", "15:00-16:00", "16:00-17:00", "17:00-18:00", "18:00-19:00", "19:00-20:00", "20:00-21:00", "21:00-22:00", "22:00-23:00", "23:00-24:00", "00:00-01:00", "01:00-02:00", "02:00-03:00", "03:00-04:00", "04:00-05:00", "05:00-06:00", "06:00-07:00", "07:00-08:00" };
                                for (int i = 0; i < d1.Columns.Count - 25; i++)
                                {
                                    // lv.Items.Add((i + 1).ToString(), DateTime_fixtrue[i], dt.Columns[i + 1].ToString());
                                    dt2.Rows.Add(i + 1, DateTime_fixtrue[i], d1.Rows[0][i + 1].ToString(), d1.Rows[0][i + 25].ToString());
                                }

                                sw.WriteLine("序号,时间段,治具SN不相同的数量,治具SN重复次数>设定值的数量");
                                for (int t = 0; t < dt2.Rows.Count; t++)
                                {
                                    string oeestr = "";
                                    //oeestr += dt2.Rows[t][0].ToString().Replace("[", ",") + ",";
                                    for (int i = 0; i < dt2.Columns.Count; i++)
                                    {
                                        oeestr += dt2.Rows[t][i].ToString().Replace("[", ",") + ",";

                                    }
                                    sw.WriteLine(oeestr);
                                }

                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, "导出错误", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            sw.Close();
                            sw.Dispose();
                        }
                    }

                }

            }
            catch (Exception ex)
            {
                Log.WriteLog(ex.ToString());
            }
        }
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
          Global.Setpoint = textBox1.Text.ToString();
        }

        private void dgv_Spare_parts_control_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgv_Spare_parts_control.Columns[e.ColumnIndex].Name == "BtnModify" && grid_TossingFixture.RowCount > 0 && e.RowIndex >= 0)
            {
                PartControlFrm partControlFrm = new PartControlFrm();
                partControlFrm.ShowDialog();
                object PartName = dgv_Spare_parts_control.Rows[e.RowIndex].Cells[0].Value;
                object PartModel = dgv_Spare_parts_control[1, e.RowIndex].Value;
                object PartLife = dgv_Spare_parts_control[2, e.RowIndex].Value;
                object PartCode = dgv_Spare_parts_control[3, e.RowIndex].Value;
                object PartUploadTime = dgv_Spare_parts_control[4, e.RowIndex].Value;
                object PartNowLife = dgv_Spare_parts_control[5, e.RowIndex].Value;
            }
        }

        private void button21_Click(object sender, EventArgs e)
        {
            try
            {
                if (Global.Fixture == "")
                {
                    int i = grid_AllFixture.CurrentRow.Index;   //所选的行的索引
                    Global.Fixture = grid_AllFixture.Rows[i].Cells[1].Value.ToString();//治具码
                    string delete = $"delete FixtureStatus where FixtureID ='{Global.Fixture}'";
                    SQL.ExecuteUpdate(delete);
                    Global._fixture.Remove(Global.Fixture);
                    Log.WriteLog("grid_AllFixture 控件" + Global.Fixture + " 手动清除成功");
                    button22_Click(null, null);
                }   
                else
                {
                    string delete = $"delete FixtureStatus where FixtureID ='{Global.Fixture}'";
                    SQL.ExecuteUpdate(delete);
                    Global._fixture.Remove(Global.Fixture);
                    Log.WriteLog("grid_AllFixture 控件" + Global.Fixture + " 手动清除成功");
                    Global.Fixture = "";
                    UpdateDataGridView();
                }                                
            }
            catch (Exception ex)
            {
                Log.WriteLog("清除单个治具异常："+ ex.ToString());
            }

        }

        private void button22_Click(object sender, EventArgs e)
        {
            try
            {
                if (Global.Fixture == "")
                {
                    int i = grid_TossingFixture.CurrentRow.Index;   //所选的行的索引
                    Global.Fixture = grid_TossingFixture.Rows[i].Cells[0].Value.ToString();//治具码
                    string delete = $"delete FixtureTossing where Fixture ='{Global.Fixture}'";
                    SQL.ExecuteUpdate(delete);
                    Global._fixture_tossing_ng.Remove(Global.Fixture);
                    Log.WriteLog("grid_TossingFixture 控件 " + Global.Fixture + " 手动清除成功");
                    button21_Click(null, null);
                }
                else
                {
                    string delete = $"delete FixtureTossing where Fixture ='{Global.Fixture}'";
                    SQL.ExecuteUpdate(delete);
                    Global._fixture_tossing_ng.Remove(Global.Fixture);
                    Log.WriteLog("grid_TossingFixture 控件 " + Global.Fixture + " 手动清除成功");
                    Global.Fixture = "";
                    UpdateDataGridView();
                } 
            }
            catch (Exception ex)
            {
                Log.WriteLog("清除单个治具异常：" + ex.ToString());
            }
        }
    }
}
