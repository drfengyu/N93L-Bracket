using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 卓汇数据追溯系统
{
    public partial class SettingFrm : Form
    {
        private MainFrm _mainparent;
        string dataPath = @"D:\ZHH\Upload\setting.ini";
        delegate void Check(bool msg, string name);
        //IniProductFile inidata;
        public SettingFrm(MainFrm mdiParent)
        {
            InitializeComponent();
            _mainparent = mdiParent;
            //inidata = new IniProductFile(dataPath);
        }

        private void SettingFrm_Load(object sender, EventArgs e)
        {
            try
            {
                txt_PlcIP.Text = Global.inidata.productconfig.Plc_IP;
                txt_PlcPort.Text = Global.inidata.productconfig.Plc_Port;
                txt_PlcPort2.Text = Global.inidata.productconfig.Plc_Port2;
                txt_HansIP.Text = Global.inidata.productconfig.Hans_IP;
                txt_HansPort.Text = Global.inidata.productconfig.Hans_Port;
                txt_UAIP.Text = Global.inidata.productconfig.PDCA_UA_IP;
                txt_UAPort.Text = Global.inidata.productconfig.PDCA_UA_Port;
                txt_LAIP.Text = Global.inidata.productconfig.PDCA_LA_IP;
                txt_LAPort.Text = Global.inidata.productconfig.PDCA_LA_Port;
                txt_BarcodeIP.Text = Global.inidata.productconfig.Barcode_IP;
                txt_BarcodePort.Text = Global.inidata.productconfig.Barcode_Port;
                txt_Precitec_IP.Text = Global.inidata.productconfig.Precitec_IP;
                txt_Precitec_Port.Text = Global.inidata.productconfig.Precitec_Port;
                txt_TraceIP.Text = Global.inidata.productconfig.Trace_IP;
                txt_OEEIP.Text = Global.inidata.productconfig.OEE_IP;
                txtURL_TraceLogs_UA.Text = Global.inidata.productconfig.Trace_Logs_UA;
                txtURL_TraceLogs_LA.Text = Global.inidata.productconfig.Trace_Logs_LA;
                txtURL_Process_UA.Text = Global.inidata.productconfig.Trace_CheakSN_UA;
                txtURL_Process_LA.Text = Global.inidata.productconfig.Trace_CheakSN_LA;
                txtOEE_URL1.Text = Global.inidata.productconfig.OEE_URL1;
                txtOEE_URL2.Text = Global.inidata.productconfig.OEE_URL2;
                txtURL_Mes1.Text = Global.inidata.productconfig.MES_URL1;
                txtURL_Mes2.Text = Global.inidata.productconfig.MES_URL2;
                txtHeaders_key.Text = Global.inidata.productconfig.Headers_key;
                txtHeaders_value.Text = Global.inidata.productconfig.Headers_value;
                txtURL_PIS.Text = Global.inidata.productconfig.PIS_URL;
                txt_Threshold.Text = Global.inidata.productconfig.Threshold;
                txt_DeleteTime.Text = Global.inidata.productconfig.delete_time;
                txt_MaxCT.Text = Global.inidata.productconfig.MaxCT;
                txt_CT.Text = Global.inidata.productconfig.CT;
                txt_FixtureNumber.Text = Global.inidata.productconfig.FixtureNumber;
                txt_UA_site.Text = Global.inidata.productconfig.UA_site;
                txt_UA_product.Text = Global.inidata.productconfig.UA_product;
                txt_UA_StationType.Text = Global.inidata.productconfig.UA_station_type;
                txt_UA_location.Text = Global.inidata.productconfig.UA_location;
                txt_UA_LineNumber.Text = Global.inidata.productconfig.UA_line_number;
                txt_UA_StationNumber.Text = Global.inidata.productconfig.UA_station_number;
                txtPDCA_UA_line_type.Text = Global.inidata.productconfig.Process_UA;
                txtPDCA_UA_line_id.Text = Global.inidata.productconfig.Line_id_ua;
                txtPDCA_UA_software_name.Text = Global.inidata.productconfig.Sw_name_ua;
                txtPDCA_UA_station_id.Text = Global.inidata.productconfig.Station_id_ua;
                cmb_PDCA_IP.Text = Global.inidata.productconfig.PDCA_UA_IP;
                txtTrace_UA_line_type.Text = Global.inidata.productconfig.Process_UA;
                txtTrace_UA_line_id.Text = Global.inidata.productconfig.Line_id_ua;
                txtTrace_UA_software_name.Text = Global.inidata.productconfig.Sw_name_ua;
                txtTrace_UA_station_id.Text = Global.inidata.productconfig.Station_id_ua;
                txtTrace_UA_IP.Text = Global.inidata.productconfig.Trace_IP;
                //txtTrace_LA_line_id.Text = Global.inidata.productconfig.Line_id_la;
                //txtTrace_LA_software_name.Text = Global.inidata.productconfig.Sw_name_la;
                //txtTrace_LA_station_id.Text = Global.inidata.productconfig.Station_id_la;
                txtOEE_line_type.Text = Global.inidata.productconfig.Process_UA;
                txtOEE_Line_id.Text = Global.inidata.productconfig.Line_id_ua;
                txtOEE_Dsn.Text = Global.inidata.productconfig.OEE_Dsn;
                txtOEE_AuthCode.Text = Global.inidata.productconfig.OEE_authCode;
                if (Global.inidata.productconfig.IFactory_online == "1")
                {
                    chb_IFactory.Checked = true;
                }
                else
                {
                    chb_IFactory.Checked = false;
                }
                if (Global.inidata.productconfig.Phase_online == "true")
                {
                    chb_Phase.Checked = false;
                }
                else
                {
                    chb_Phase.Checked = true;
                }

                ///20230313Add
                if (Global.inidata.productconfig.Material_online == "1")
                {
                    chb_Matrial.Checked = true;
                }
                else
                {
                    chb_Matrial.Checked = false;
                }

                List<string> localIP = GetLocalIPv4AddressList();
                List<string> LocalMac = GetMacAddress();
                for (int i = 0; i < localIP.Count; i++)
                {
                    cmb_PDCA_IP.Items.Add(localIP[i]);
                    cmb_PDCA_Mac.Items.Add(LocalMac[i]);
                    //cmb_Trace_IP.Items.Add(localIP[i]);
                    //cmb_Trace_Mac.Items.Add(LocalMac[i]);
                    cmb_OEE_IP.Items.Add(localIP[i]);
                    cmb_OEE_Mac.Items.Add(LocalMac[i]);
                }
            }
            catch (Exception ex)
            {
                Log.WriteLog(ex.ToString());
                MessageBox.Show(ex.ToString());
            }
        }
        public void checkbox(bool b, string name)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Check(checkbox), new object[] { b, name, });
                return;
            }
            //Invoke(new Action(() => chb_getline.Checked = false));
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
        }


        public static List<string> GetLocalIPv4AddressList()
        {
            List<string> ipAddressList = new List<string>();
            try
            {
                IPHostEntry ipHost = Dns.Resolve(Dns.GetHostName());
                foreach (IPAddress ipAddress in ipHost.AddressList)
                {
                    if (!ipAddressList.Contains(ipAddress.ToString()))
                    {
                        ipAddressList.Add(ipAddress.ToString());
                    }
                }
            }
            catch
            { }
            return ipAddressList;
        }

        public static List<string> GetMacAddress()
        {
            List<string> MacAddressList = new List<string>();
            try
            {
                ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
                ManagementObjectCollection moc = mc.GetInstances();
                foreach (ManagementObject mo in moc)
                {
                    if ((bool)mo["IPEnabled"] == true)
                    {
                        MacAddressList.Add(mo["MacAddress"].ToString());
                    }
                }
                moc = null;
                mc = null;
            }
            catch
            { }
            return MacAddressList;
        }

        private void txt_FixtureNumber_TextChanged(object sender, EventArgs e)
        {
            Global.inidata.productconfig.FixtureNumber = txt_FixtureNumber.Text;
            Global.inidata.WriteProductConfigSection();
            _mainparent.txtFixtureNumber_TextChanged(sender, e);
        }

        private void btn_CommunicationSave_Click(object sender, EventArgs e)
        {
            Global.inidata.productconfig.Plc_IP = txt_PlcIP.Text;
            Global.inidata.productconfig.Plc_Port = txt_PlcPort.Text;
            Global.inidata.productconfig.Plc_Port2 = txt_PlcPort2.Text;
            Global.inidata.productconfig.Hans_IP = txt_HansIP.Text;
            Global.inidata.productconfig.Hans_Port = txt_HansPort.Text;
            Global.inidata.productconfig.PDCA_UA_IP = txt_UAIP.Text;
            Global.inidata.productconfig.PDCA_UA_Port = txt_UAPort.Text;
            Global.inidata.productconfig.PDCA_LA_IP = txt_LAIP.Text;
            Global.inidata.productconfig.PDCA_LA_Port = txt_LAPort.Text;
            Global.inidata.productconfig.Trace_IP = txt_TraceIP.Text;
            Global.inidata.productconfig.Trace_Port = txt_TracePort.Text;
            Global.inidata.productconfig.OEE_IP = txt_OEEIP.Text;
            Global.inidata.productconfig.OEE_Port = txt_OEEPort.Text;
            Global.inidata.productconfig.Barcode_IP = txt_BarcodeIP.Text;
            Global.inidata.productconfig.Barcode_Port = txt_BarcodePort.Text;
            Global.inidata.productconfig.Precitec_IP = txt_Precitec_IP.Text;
            Global.inidata.productconfig.Precitec_Port = txt_Precitec_Port.Text;
            Global.inidata.productconfig.UA_site = txt_UA_site.Text;
            Global.inidata.productconfig.UA_product = txt_UA_product.Text;
            Global.inidata.productconfig.UA_station_type = txt_UA_StationType.Text;
            Global.inidata.productconfig.UA_location = txt_UA_location.Text;
            Global.inidata.productconfig.UA_line_number = txt_UA_LineNumber.Text;
            Global.inidata.productconfig.UA_station_number = txt_UA_StationNumber.Text;
            Global.inidata.productconfig.Threshold = txt_Threshold.Text;
            Global.inidata.productconfig.delete_time = txt_DeleteTime.Text;
            Global.inidata.productconfig.MaxCT = txt_MaxCT.Text;
            Global.inidata.productconfig.CT = txt_CT.Text;
            Global.inidata.productconfig.FixtureNumber = txt_FixtureNumber.Text;
            if (chb_IFactory.Checked)//是否屏蔽IFactory前站卡控
            {
                Global.inidata.productconfig.IFactory_online = "1";
            }
            else
            {
                Global.inidata.productconfig.IFactory_online = "2";
            }
            if (chb_Phase.Checked)//是否屏蔽phase卡控
            {
                Global.inidata.productconfig.Phase_online = "fasle";
            }
            else
            {
                Global.inidata.productconfig.Phase_online = "true";
            }
            ///20230313Add
            if (chb_Matrial.Checked)
            {
                Global.inidata.productconfig.Material_online = "1";
                Global.PLC_Client.WritePLC_D(60,new short[] { 0});
            }
            else
            {
                Global.inidata.productconfig.Material_online = "2";
            }

            Global.inidata.WriteProductConfigSection();   //写入修改的参数
            SaveSoftwareParameter();
            MessageBox.Show("保存成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            SettingFrm_Load(null, null);
        }

        private void btn_PDCAUploadSave_Click(object sender, EventArgs e)
        {
            Global.inidata.productconfig.Line_id_ua = txtPDCA_UA_line_id.Text;
            Global.inidata.productconfig.Station_id_ua = txtPDCA_UA_station_id.Text;
            Global.inidata.productconfig.Sw_name_ua = txtPDCA_UA_software_name.Text;
            Global.inidata.productconfig.Line_type = txtPDCA_UA_line_type.Text;
            Global.inidata.WriteProductConfigSection();
            SaveSoftwareParameter();
            MessageBox.Show("保存成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            SettingFrm_Load(null, null);
        }

        private void btn_OEEUploadSave_Click(object sender, EventArgs e)
        {
            Global.inidata.productconfig.OEE_Dsn = txtOEE_Dsn.Text;
            Global.inidata.productconfig.OEE_authCode = txtOEE_AuthCode.Text;
            Global.inidata.WriteProductConfigSection();
            SaveSoftwareParameter();
            MessageBox.Show("保存成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            SettingFrm_Load(null, null);
        }

        private void btn_TraceUploadSave_Click(object sender, EventArgs e)
        {
            Global.inidata.productconfig.Line_id_ua = txtTrace_UA_line_id.Text;
            Global.inidata.productconfig.Station_id_ua = txtTrace_UA_station_id.Text;
            Global.inidata.productconfig.Sw_name_ua = txtTrace_UA_software_name.Text;
            Global.inidata.productconfig.Line_type = txtTrace_UA_line_type.Text;
            Global.inidata.productconfig.Trace_IP = txtTrace_UA_IP.Text;
            Global.inidata.WriteProductConfigSection();
            SaveSoftwareParameter();
            MessageBox.Show("保存成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            SettingFrm_Load(null, null);
        }

        private void SettingFrm_MouseEnter(object sender, EventArgs e)
        {
            Global.UserLoginMouseMoveTime = DateTime.Now;
        }

        private void SaveSoftwareParameter()
        {
            Global.inidata = new IniProductFile(dataPath);//读取修改的参数
            List<string> _name = new List<string>();
            List<string> _value = new List<string>();
            foreach (System.Reflection.PropertyInfo p in Global.inidata.productconfig.GetType().GetProperties())//获取所有配置文件名称和参数
            {
                _name.Add(p.Name);
                _value.Add(p.GetValue(Global.inidata.productconfig).ToString());
            }
            for (int i = 0; i < _name.Count; i++)
            {
                if (Global._listValue[i] != _value[i])//比较是否有修改参数
                {
                    Log.WriteLog(Global.Name + " 工号：" + Global.Emp + "  " + Global._listName[i] + "参数修改：" + Global._listValue[i] + "→" + _value[i] + ",参数修改记录");
                    Log.WriteLog(String.Format("{0},{1},{2},{3},{4},{5},{6}", Global.Name, Global.Title, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), Global._listName[i], Global._listValue[i], _value[i], ""), System.AppDomain.CurrentDomain.BaseDirectory + "\\参数修改记录\\");
                }
            }
            Global._listName = _name;
            Global._listValue = _value;
        }
    }
}
