

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Dynamic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using 卓汇数据追溯系统.ConfigHelper;
using 卓汇数据追溯系统.Models;
using static 卓汇数据追溯系统.MainFrm;

namespace 卓汇数据追溯系统
{
    public partial class MainFrm : Form
    {
        #region 声明
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
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool SetLocalTime(ref timer _unifyTimer);
        public class UyTime
        {
            public string ServerTime { get; set; }
        }
        public struct timer
        {
            public ushort wYear;
            public ushort wMonth;
            public ushort wDayOfWeek;
            public ushort wDay;
            public ushort wHour;
            public ushort wMinute;
            public ushort wSecond;
            public ushort wMiliseconds;
        }
        public class ThreadInfo
        {
            public string FileName { get; set; }
            public int SelectedIndex { get; set; }
            public string SN { get; set; }
            public string FixtureID { get; set; }
        }

        public class PrecitecData
        {
            public string precitec_grading = "0";
            public string precitec_rev = "0";
            public string precitec_value = "0";
        }
        public class CheckOEEMachine
        {
            public string SiteName { get; set; }
            public string EMT { get; set; }
            public string TraceIP { get; set; }
            public string TraceStationID { get; set; }
            public string MacAddress { get; set; }
        }
        public class CheckOEEMachineRespond
        {
            public string Result { get; set; }
            public string Message { get; set; }
        }
        public class MacQueryresult
        {
            public EMT EMT;
            public TraceStationID TraceStationID;
            public SiteName SiteName;
        }
        public class EMT
        {
            public bool IsMatch;
            public string FromJMCC;
            public string FromMachine;
        }
        public class TraceStationID
        {
            public bool IsMatch;
            public string FromJMCC;
            public string FromMachine;
        }
        public class SiteName
        {
            public bool IsMatch;
            public string FromJMCC;
            public string FromMachine;
        }
        //public class OEEDiscardLog
        //{
        //    public string SN;
        //    public string SystemType;
        //    public string JSONBody;
        //    public string Error;
        //    public string Frenquency;
        //}
        //public class DiscardData
        //{
        //    public string sn;
        //    public string SystemType;
        //    public string uptime;
        //    public string JSONBody;
        //    public string error;
        //    public string frenquency;
        //}
        public class CheckTraceMachine
        {
            public string mode { get; set; }
            public Data data { get; set; }
        }
        public class Data
        {
            public string plant { get; set; }
            public string dsn { get; set; }
            public string traceIP { get; set; }
            public string machine { get; set; }
        }
        public class Data2
        {
            public string status { get; set; }
            public string message { get; set; }
        }
        public class CheckTraceMachineRespond
        {
            public string success { get; set; }
            public Data2 data { get; set; }
        }
        public class MaterielData
        {
            public string date;
            public string count;
            public string totalcount;
            public string parttype;
        }
        public struct JgpData
        {
            public string SN;
            public string SN2;
            public string start_Time;
            public string end_Time;
            public string ct;
            public string result;
        }
        public HomeFrm _homefrm;
        public ManualFrm _manualfrm;
        public SettingFrm _sttingfrm;
        public AbnormalFrm _Abnormalfrm;
        public UserLoginFrm _userloginfrm;
        public HelpFrm _helpfrm;
        public MachineFrm _machinefrm;
        public DataStatisticsFrm _datastatisticsfrm;
        public IOMonitorFrm _iomonitorfrm;
        string LogPath = System.AppDomain.CurrentDomain.BaseDirectory + "\\bmp\\";
        Image[] img = new Image[16];
        delegate void DGVAutoSize(DataGridView dgv);
        delegate void ShowDataTable(DataGridView dgv, DataTable dt, int index);
        delegate void ShowPlcStatue(string txt, Color color, int id);
        private delegate void tssLabelcolor(ToolStripStatusLabel tsslbl, Color color, string bl);
        private delegate void AddItemToRichTextBoxDelegate(string msg, RichTextBox richtextbox);
        private delegate void btnEnable(ToolStripButton btn, bool b);
        private delegate void AddItemToListBoxDelegate(ListBox listbox, string str, int index);
        private delegate void ShowTxt(string txt, TextBox tb);
        private delegate void Labelcolor(Label lb, Color color, string bl);
        private delegate void Labelvision(Label lb, string bl);
        private delegate void buttonflag(bool flag, Button bt);
        List<JgpData> jd1 = new List<JgpData>();
        public List<string> Out_ua = new List<string>();
        List<string> Out_ua_NG = new List<string>();
        List<string> Out_la = new List<string>();
        List<string> Out_oee = new List<string>();
        List<string> Out_oee_NG = new List<string>();
        List<string> Out_oee_Mqtt = new List<string>();
        List<string> Out_oee_discard = new List<string>();
        public List<string> Out_Trace_ua = new List<string>();
        List<string> Out_Trace_ua2 = new List<string>();
        List<string> Out_Trace_la = new List<string>();
        public Dictionary<string, BAilData> bail_ua = new Dictionary<string, BAilData>();
        Dictionary<string, BAilData> bail_ua_NG = new Dictionary<string, BAilData>();
        Dictionary<string, BAilData> bail_la = new Dictionary<string, BAilData>();
        Dictionary<string, OEEData> OEE = new Dictionary<string, OEEData>();
        Dictionary<string, OEEData> OEE_Mqtt = new Dictionary<string, OEEData>();
        Dictionary<string, OEEData> OEE_NG = new Dictionary<string, OEEData>();
        public Dictionary<string, string> SP_sn = new Dictionary<string, string>();
        //Dictionary<string, OEEDiscardLog> OEE_discard = new Dictionary<string, OEEDiscardLog>();
        public Dictionary<string, TraceMesRequest_ua> Trace_ua = new Dictionary<string, TraceMesRequest_ua>();
        Dictionary<string, TraceMesRequest_ua> Trace_ua2 = new Dictionary<string, TraceMesRequest_ua>();
        Dictionary<string, TraceMesRequest_la> Trace_la = new Dictionary<string, TraceMesRequest_la>();
        //List<string> Out_oee_discard_pdca = new List<string>();
        List<string> Out_oee_discard_trace = new List<string>();
        //Dictionary<string, OEEDiscardLog> OEE_discard_PDCA = new Dictionary<string, OEEDiscardLog>();
        //Dictionary<string, OEEDiscardLog> OEE_discard_Trace = new Dictionary<string, OEEDiscardLog>();
        Dictionary<string, HansData_U_Bracket> HansDatas_ua = new Dictionary<string, HansData_U_Bracket>();
        Dictionary<string, HansData_U_Bracket> HansDatas_PDCA_ua = new Dictionary<string, HansData_U_Bracket>();
        Dictionary<string, PrecitecData> PrecitecData_ua = new Dictionary<string, PrecitecData>();
        Dictionary<string, PrecitecData> PrecitecData_la = new Dictionary<string, PrecitecData>();
        Dictionary<string, PrecitecData> PrecitecData_PDCA_ua = new Dictionary<string, PrecitecData>();
        Dictionary<string, PrecitecData> PrecitecData_PDCA_la = new Dictionary<string, PrecitecData>();
        BailCilent bc = new BailCilent();
        BailCilent bc2 = new BailCilent();
        private static object Lock1 = new object();
        private static object Lock2 = new object();
        private static object LockHans = new object();
        private static object Lock = new object();
        private static object LockUA = new object();
        private static object LockLA = new object();
        short[] ReadStatus = new short[20];
        short[] ReadHandFeeding = new short[20];
        short[] ReadTestRunStatus = new short[20];
        short[] ReadOpenDoorStatus = new short[20];
        //public MainDisplay maindis;
        bool bclose = true;
        bool ison = false;//PLC联机信号取反     
        bool ConnectPLC = false;
        string IP = string.Empty;
        string Mac = string.Empty;
        string ClientPcName = string.Empty;
        double timenum = 5;
        short[] PLCTrg = new short[] { };

        short[] process = new short[1];//前站校验触发标志位
        short[] t1 = new short[1];//1#焊接机触发标志位
        short[] t2 = new short[1];//2#焊接机触发标志位
        short[] t3 = new short[1];//3#焊接机触发标志位
        short[] t4 = new short[1];//4#焊接机触发标志位
        short[] t5 = new short[1];//4#焊接机触发标志位
        short[] t_Code = new short[1];//读码器触发标志位
        short[] la_Trg = new short[1];//1#焊接机la工站触发标志位
        short[] ua_Trg1 = new short[1];//1#焊接机ua工站触发标志位
        short[] ua_Trg2 = new short[1];//1#焊接机ua工站NG触发标志位
        short[] ProductWayOut = new short[1];//产品出流道触发标志位
        short[] P = new short[1];//PIS治具保养触发标志位
        short[] Nut1_trg = new short[1];//抛小料触发标志位
        short[] Nut2_trg = new short[1];//小料放置成功触发标志位
        short[] CT_LOG = new short[35];//CT LOG触发标志位
        short[] oldTrg = new short[1];//PLC参数修改前触发标志位
        short[] newTrg = new short[1];//PLC参数修改后触发标志位
        short[] SmallmaterialInputTrg = new short[1];//小料投入触发标志位
        string[] hansdatas = new string[15];//大族焊接参数
        short[] SmallmaterialTrg = new short[1];//小料抛料触发标志位
        short[] CCDCheckNGTrg = new short[1];//CCD检测NG触发标志位
        short[] ReadBarcodeNGTrg = new short[1];//CCD读码NG触发标志位
        string Trace_str_ua = "";
        string Trace_str_la = "";
        string Fullsn1 = "";
        string FixtureCode = "";
        string Fullsn2 = "";
        string Fullsn3 = "";
        string Fullsn4 = "";
        string Fullsn5 = "";
        short[] Module = new short[1];//区分 模组1 、 2

        bool Mac_mini_server_ua = true;
        bool Mac_mini_server_la = true;
        bool OEE_Default_flag = true;
        bool Trace_Logs_flag = true;
        bool Trace_check_flag = true;
        bool production_num_falg = true;
        bool listBox_flag = true;
        bool DeleteFile_flag = true;
        bool Link_PLC = true;
        bool Link_Mac_Mini_Server = true; //Ping Mac mini的服务器的返回值
        bool TCPconnected = true;
        bool PrecitecTCPconnected = true;
        bool isopen = false;//连接状态判读开启
        bool flag1 = true;
        bool flag2 = true;
        bool flag3 = true;
        bool flag4 = true;
        bool flag5 = false;
        bool Call_PIS_API_flag = true;
        bool InsertSQLFlag = true;
        bool PLCHeart = true;
        int number = 0;
        int i = 0, l = 0;   //i,l为UA、LA发送失败次数
        int Product_num_Mes_NG;
        int time = 0;
        //PLC读写地址
        public const int Address_errorCode = 10000;//OEE-机台状态(1待料2运行3宕机)
        public const int Address_OEE_errorCode = 10010;//OEE-机台状态(1待料2运行3宕机)
        public const int Address_O_WatchDog = 24000;
        public const int Address_data = 19000;
        public const int Address_sndata = 5850;
        SQLServer SQL = new SQLServer();

        double Product_Lianglv_08_09 = 0;
        double Product_Lianglv_09_10 = 0;
        double Product_Lianglv_10_11 = 0;
        double Product_Lianglv_11_12 = 0;
        double Product_Lianglv_12_13 = 0;
        double Product_Lianglv_13_14 = 0;
        double Product_Lianglv_14_15 = 0;
        double Product_Lianglv_15_16 = 0;
        double Product_Lianglv_16_17 = 0;
        double Product_Lianglv_17_18 = 0;
        double Product_Lianglv_18_19 = 0;
        double Product_Lianglv_19_20 = 0;
        double Product_Lianglv_20_21 = 0;
        double Product_Lianglv_21_22 = 0;
        double Product_Lianglv_22_23 = 0;
        double Product_Lianglv_23_00 = 0;
        double Product_Lianglv_00_01 = 0;
        double Product_Lianglv_01_02 = 0;
        double Product_Lianglv_02_03 = 0;
        double Product_Lianglv_03_04 = 0;
        double Product_Lianglv_04_05 = 0;
        double Product_Lianglv_05_06 = 0;
        double Product_Lianglv_06_07 = 0;
        double Product_Lianglv_07_08 = 0;
        double Product_Lianglv_08_20 = 0;
        double Product_Lianglv_20_08 = 0;

        DateTime dateTime;
        DateTime dateTime1;
        #endregion

        #region 初始化
        public MainFrm()
        {
            InitializeComponent();
            //MessageManager.gInit();
            // MessageManagerLogger.gInit("Log\\", "DcckVision", 30);
        }

        private void MainFrm_Load(object sender, EventArgs e)
        {
            // maindis = new MainDisplay();//CCD程序实例化
            //读取开机配置文件
            #region 焊接数据存储测试
            //string str = "FM7GZU00WCH0000CP1;0.000;0.000;4;0.5;0;0;0;0;PASS;2024-02-29 21:21:27;HFF661912237;4000012413;0;47.2;49;1;700;110;0.5;0;49;NA;shim-NE;0ms0%/0ms0%/0ms0%;35.0000;900.0000;12;0;110.0000;3000.0000;500;0;0ms0%/0ms0%/0ms0%;45.0000;900.0000;12;0;110.0000;3000.0000;500;0;0ms0%/0ms0%/0ms0%;35.0000;900.0000;12;0;110.0000;3000.0000;500;0;shimSL.hs;0ms0%/0ms0%/0ms0%;35.0000;900.0000;12;0;110.0000;3000.0000;0;0;0ms0%/0ms0%/0ms0%;35.0000;900.0000;12;0;110.0000;3000.0000;0;0;0ms0%/0ms0%/0ms0%;35.0000;900.0000;12;0;110.0000;3000.0000;0;0;shim-SR;0ms0%/0ms0%/0ms0%;35.0000;900.0000;12;0;110.0000;3000.0000;0;0;0ms0%/0ms0%/0ms0%;35.0000;900.0000;12;0;110.0000;3000.0000;0;0;0ms0%/0ms0%/0ms0%;35.0000;900.0000;12;0;110.0000;3000.0000;0;0;Trench bracket;0ms0%/0ms0%/0ms0%;35.0000;900.0000;12;0;110.0000;3000.0000;0;0;0ms0%/0ms0%/0ms0%;35.0000;900.0000;12;0;110.0000;3000.0000;0;0;0ms0%/0ms0%/0ms0%;35.0000;900.0000;12;0;110.0000;3000.0000;0;0;";

           
            //string[] data = str.Replace("\r\n", "").Split(';');
            //string insertStr1 = string.Format("insert into HansData([DateTime],[SN],[Nut],[power_ll],[power_ul],[pattern_type],[spot_size],[hatch],[swing_amplitude],[swing_freq],[JudgeResult],[MeasureTime],[MachineSN],[PulseProfile_measure],[ActualPower],[Power_measure],[WaveForm_measure],[Frequency_measure],[LinearSpeed_measure],[QRelease_measure],[PulseEnergy_measure],[PeakPower_measure],[laser_sensor],[pulse_profile],[laser_power],[frequency],[waveform],[pulse_energy],[laser_speed],[jump_speed],[jump_delay],[scanner_delay],[Station]) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}','{20}','{21}','{22}','{23}','{24}','{25}','{26}','{27}','{28}','{29}','{30}','{31}','{32}')",
            //              DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), data[0], data[23], data[1], data[2], data[3], data[4], data[5], data[6], data[7], data[9], data[10], data[11], data[13], data[14], data[15], data[16], data[17], data[18], data[19], data[20], data[21], data[22], data[24], data[25], data[26], data[27], data[28], data[29], data[30], data[31], data[32], "L_Bracket");
            //int r = SQL.ExecuteUpdate(insertStr1);
            //Log.WriteLog(string.Format("插入了{0}行{1}焊接参数", r, data[23]));
            //string insertStr2 = string.Format("insert into HansData([DateTime],[SN],[Nut],[power_ll],[power_ul],[pattern_type],[spot_size],[hatch],[swing_amplitude],[swing_freq],[JudgeResult],[MeasureTime],[MachineSN],[PulseProfile_measure],[ActualPower],[Power_measure],[WaveForm_measure],[Frequency_measure],[LinearSpeed_measure],[QRelease_measure],[PulseEnergy_measure],[PeakPower_measure],[laser_sensor],[pulse_profile],[laser_power],[frequency],[waveform],[pulse_energy],[laser_speed],[jump_speed],[jump_delay],[scanner_delay],[Station]) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}','{20}','{21}','{22}','{23}','{24}','{25}','{26}','{27}','{28}','{29}','{30}','{31}','{32}')",
            //  DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), data[0], data[51], data[1], data[2], data[3], data[4], data[5], data[6], data[7], data[9], data[10], data[11], data[13], data[14], data[15], data[16], data[17], data[18], data[19], data[20], data[21], data[22], data[52], data[53], data[54], data[55], data[56], data[57], data[58], data[59], data[60], "L_Bracket");
            //int r2 = SQL.ExecuteUpdate(insertStr2);
            //Log.WriteLog(string.Format("插入了{0}行{1}焊接参数", r2, data[51]));
            //string insertStr3 = string.Format("insert into HansData([DateTime],[SN],[Nut],[power_ll],[power_ul],[pattern_type],[spot_size],[hatch],[swing_amplitude],[swing_freq],[JudgeResult],[MeasureTime],[MachineSN],[PulseProfile_measure],[ActualPower],[Power_measure],[WaveForm_measure],[Frequency_measure],[LinearSpeed_measure],[QRelease_measure],[PulseEnergy_measure],[PeakPower_measure],[laser_sensor],[pulse_profile],[laser_power],[frequency],[waveform],[pulse_energy],[laser_speed],[jump_speed],[jump_delay],[scanner_delay],[Station]) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}','{20}','{21}','{22}','{23}','{24}','{25}','{26}','{27}','{28}','{29}','{30}','{31}','{32}')",
            //  DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), data[0], data[79], data[1], data[2], data[3], data[4], data[5], data[6], data[7], data[9], data[10], data[11], data[13], data[14], data[15], data[16], data[17], data[18], data[19], data[20], data[21], data[22], data[80], data[81], data[82], data[83], data[84], data[85], data[86], data[87], data[88], "L_Bracket");
            //int r3 = SQL.ExecuteUpdate(insertStr3);
            //Log.WriteLog(string.Format("插入了{0}行{1}焊接参数", r3, data[79]));

            //string insertStr4 = string.Format("insert into HansData([DateTime],[SN],[Nut],[power_ll],[power_ul],[pattern_type],[spot_size],[hatch],[swing_amplitude],[swing_freq],[JudgeResult],[MeasureTime],[MachineSN],[PulseProfile_measure],[ActualPower],[Power_measure],[WaveForm_measure],[Frequency_measure],[LinearSpeed_measure],[QRelease_measure],[PulseEnergy_measure],[PeakPower_measure],[laser_sensor],[pulse_profile],[laser_power],[frequency],[waveform],[pulse_energy],[laser_speed],[jump_speed],[jump_delay],[scanner_delay],[Station]) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}','{20}','{21}','{22}','{23}','{24}','{25}','{26}','{27}','{28}','{29}','{30}','{31}','{32}')",
            //  DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), data[0], data[107], data[1], data[2], data[3], data[4], data[5], data[6], data[7], data[9], data[10], data[11], data[13], data[14], data[15], data[16], data[17], data[18], data[19], data[20], data[21], data[22], data[108], data[109], data[110], data[111], data[112], data[113], data[114], data[115], data[116], "L_Bracket");
            //int r4 = SQL.ExecuteUpdate(insertStr4);
            //Log.WriteLog(string.Format("插入了{0}行{1}焊接参数", r4, data[116]));
            #endregion
            try
            {
                string dataPath = @"D:\ZHH\Upload\setting.ini";
                if (File.Exists(dataPath))
                {
                    Global.inidata = new IniProductFile(dataPath);
                    foreach (System.Reflection.PropertyInfo p in Global.inidata.productconfig.GetType().GetProperties())//获取所有配置文件名称和参数
                    {
                        Global._listName.Add(p.Name);
                        Global._listValue.Add(p.GetValue(Global.inidata.productconfig).ToString());
                    }
                    Log.WriteLog("读取参数成功");
                }
                else
                {
                    MessageBox.Show("配置文件不存在");
                    Log.WriteLog("配置文件不存在");
                    Environment.Exit(1);
                }
            }
            catch (Exception ex)
            {
                Log.WriteLog("读取配置文件异常" + ex.ToString());
            }

            //连接PLC
            try
            {
                Global.PLC_Client.sClient(Global.inidata.productconfig.Plc_IP, Global.inidata.productconfig.Plc_Port);
                Global.PLC_Client.Connect();
                Global.PLC_Client2.sClient(Global.inidata.productconfig.Plc_IP, Global.inidata.productconfig.Plc_Port2);
                Global.PLC_Client2.Connect();
                if (Global.PLC_Client.IsConnected && Global.PLC_Client2.IsConnected)
                {
                    //timer1.Enabled = true;
                    Log.WriteLog("已连接PLC");
                    isopen = true;
                    ConnectPLC = true;
                    //Global.PLC_Client.WritePLC_D(13022, new short[] { 1 }); // Trace 参数校验结果清零
                    Global.PLC_Client.WritePLC_D(16014, new short[] { 0 });//给PLC首件标志清0,防止收首件进行中程序崩溃不给首件标志清0
                }
                else
                {
                    MessageBox.Show("PLC通信无法连接");
                    Log.WriteLog("PLC通信无法连接");
                    ShowStatus("与PLC断开连接", Color.Red, 0);
                    ConnectPLC = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("PLC通信无法连接");
                Log.WriteLog("PLC通信无法连接");
                Environment.Exit(1);
            }
            if (Global.inidata.productconfig.TraceCheckParam_Online == "1")
            {
                Global.PLC_Client.WritePLC_D(10110, new short[] { 1 });
                var IP = GetTraceIp();
                string Msg = string.Empty;
                bool TraceVerify = true;
                bool OEEVerify = true;
                Global.b_VerifyResult = false;
                try
                {
                    string CallResult = "";
                    string errMsg = "";
                    JsonSerializerSettings jsetting = new JsonSerializerSettings();
                    jsetting.NullValueHandling = NullValueHandling.Ignore;//Json不输出空值
                    CheckOEEMachine CheckMachine = new CheckOEEMachine();
                    CheckMachine.SiteName = Global.inidata.productconfig.SiteName;
                    CheckMachine.EMT = Global.inidata.productconfig.EMT;
                    CheckMachine.TraceIP = GetTraceIp();
                    CheckMachine.TraceStationID = Global.inidata.productconfig.Station_id_ua;
                    CheckMachine.MacAddress = GetOEEMac();
                    string SendTraceMachine = JsonConvert.SerializeObject(CheckMachine, Formatting.None, jsetting);
                    Log.WriteLog("OEE校验参数数据:" + SendTraceMachine);
                    var result = RequestAPI2.CallBobcat2(Global.inidata.productconfig.OEECheckParamURL, SendTraceMachine, out CallResult, out errMsg, false);
                    CheckOEEMachineRespond Respond = JsonConvert.DeserializeObject<CheckOEEMachineRespond>(CallResult);
                    Log.WriteLog("CallResult " + CallResult.ToString() + "  errMsg:" + errMsg.ToString());
                    try
                    {
                        if (Respond.Result == "Success")//&& Respond_UA.Contains("Pass")
                        {
                            ShowStatus("Trace参数状态", Color.DarkSeaGreen, 7);
                            Log.WriteLog("OEE校验参数结果:" + JsonConvert.SerializeObject(CallResult));
                            //初步比对成功后,用Mac地址获取后台有无更新数据有更新就更新本地setting对应数据
                            string Mac = string.Format("{{\"MacAddress\":\"{0}\"}}", CheckMachine.MacAddress);
                            //result = RequestAPI2.CallBobcat2(Global.inidata.productconfig.MacQueryUrl, Mac, out CallResult, out errMsg, false);
                            //Log.WriteLog("通过Mac获取OEE参数:" + JsonConvert.SerializeObject(CallResult));
                            //MacQueryresult TraceResult = JsonConvert.DeserializeObject<MacQueryresult>(CallResult);
                            //if (!TraceResult.EMT.IsMatch && TraceResult.EMT.FromJMCC != null && TraceResult.EMT.FromJMCC != "")
                            //{
                            //    Global.inidata.productconfig.EMT = TraceResult.EMT.FromJMCC;
                            //}
                            //if (!TraceResult.TraceStationID.IsMatch && TraceResult.TraceStationID.FromJMCC != null && TraceResult.TraceStationID.FromJMCC != "")
                            //{
                            //    Global.inidata.productconfig.Station_id_ua = TraceResult.TraceStationID.FromJMCC;
                            //}
                            //if (!TraceResult.SiteName.IsMatch && TraceResult.SiteName.FromJMCC != null && TraceResult.SiteName.FromJMCC != "")
                            //{
                            //    Global.inidata.productconfig.SiteName = TraceResult.SiteName.FromJMCC;
                            //}
                            //if (!TraceResult.EMT.IsMatch || !TraceResult.TraceStationID.IsMatch || !TraceResult.SiteName.IsMatch)
                            //{
                            //    Global.inidata.WriteProductConfigSection();
                            //    Global.inidata.ReadProductConfigSection();
                            //}
                        }
                        else
                        {
                            ShowStatus("Trace参数状态", Color.Red, 7);
                            OEEVerify = false;
                            MessageBox.Show(errMsg + Respond.Message, "校验Trace配置参数失败", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            Log.WriteLog("OEE校验参数:" + errMsg + JsonConvert.SerializeObject(CallResult));
                        }
                    }
                    catch
                    {
                        ShowStatus("Trace参数状态", Color.Red, 7);
                        Log.WriteLog("OEE参数校验异常" + CallResult + errMsg);
                        MessageBox.Show(errMsg + JsonConvert.SerializeObject(CallResult), "校验Trace配置参数失败", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception EX)
                {
                    Log.WriteLog("OEE参数校验异常:" + EX.ToString());
                }
                //trench 站点校验
                //Global.inidata.productconfig.Station_id_ua = "JACD_E03-2F-OQC01_83001_DEVELOPMENT31";
                string Respond_UA = RequestAPI3.HttpPostWebService(Global.inidata.productconfig.TraceCheckParamURL54, IP, Global.inidata.productconfig.Process_UA, Global.inidata.productconfig.Line_id_ua, Global.inidata.productconfig.Station_id_ua, Global.inidata.productconfig.Sw_name_ua, Global.inidata.productconfig.Version, out Msg);
                Log.WriteLog("Trace_trench校验参数数据:" + IP + Global.inidata.productconfig.Process_UA + Global.inidata.productconfig.Line_id_ua + Global.inidata.productconfig.Station_id_ua + Global.inidata.productconfig.Sw_name_ua + Global.inidata.productconfig.Version);
                if (!Respond_UA.Contains("Pass"))
                {
                    TraceVerify = false;
                    ShowStatus("Trace参数状态", Color.Red, 7);
                    Log.WriteLog("Trace参数校验异常: " + Respond_UA + Msg.ToString());
                }
                //Tab 站点校验
                string Respond_LA = RequestAPI3.HttpPostWebService(Global.inidata.productconfig.TraceCheckParamURL54, IP, Global.inidata.productconfig.Process_LA, Global.inidata.productconfig.Line_id_la, Global.inidata.productconfig.Station_id_la, Global.inidata.productconfig.Sw_name_la, Global.inidata.productconfig.Version, out Msg);
                Log.WriteLog("Trace_Tab校验参数数据:" + IP + Global.inidata.productconfig.Process_LA + Global.inidata.productconfig.Line_id_la + Global.inidata.productconfig.Station_id_la + Global.inidata.productconfig.Sw_name_la + Global.inidata.productconfig.Version);
                if (!Respond_LA.Contains("Pass"))
                {
                    TraceVerify = false;
                    ShowStatus("Trace参数状态", Color.Red, 7);
                    Log.WriteLog("Trace参数校验异常: " + Respond_UA + Msg.ToString());
                }
                if (!TraceVerify)
                {
                    Global.PLC_Client.WritePLC_D(10110, new short[] { 2 });
                }
                else if (TraceVerify)
                {
                    Global.b_VerifyResult = true;
                    Global.PLC_Client.WritePLC_D(10110, new short[] { 1 });
                }
            }
            //读取已配置数据并显示
            Global.Operator_pwd = Global.inidata.productconfig.Operator_pwd;
            Global.Technician_pwd = Global.inidata.productconfig.Technician_pwd;
            Global.Administrator_pwd = Global.inidata.productconfig.Administrator_pwd;
            Global.Threshold = Global.inidata.productconfig.Threshold;
            //MDI父窗体
            _homefrm = new HomeFrm(this);
            _manualfrm = new ManualFrm(this);
            _sttingfrm = new SettingFrm(this);
            _Abnormalfrm = new AbnormalFrm(this);
            _userloginfrm = new UserLoginFrm(this);
            _helpfrm = new HelpFrm(this);
            _machinefrm = new MachineFrm(this);
            _datastatisticsfrm = new DataStatisticsFrm(this);
            _iomonitorfrm = new IOMonitorFrm(this);
            ShowView();
            //导入报警信息表&PLC参数信息
            try
            {
                FileStream fs = new FileStream("报警目录.csv", FileMode.Open, FileAccess.Read);
                StreamReader sr = new StreamReader(fs, Encoding.Default);
                string lineData;
                while ((lineData = sr.ReadLine()) != null)
                {
                    ErrorData er = new ErrorData();
                    int ed_key = Convert.ToInt32(lineData.Split(',')[0]);
                    er.errorCode = lineData.Split(',')[1];//载入OEE异常代码
                    er.errorinfo = lineData.Split(',')[2];//载入OEE异常信息
                    er.errorStatus = lineData.Split(',')[3];//载入OEE异常状态
                    //er.ModuleCode = lineData.Split(',')[4].Replace("|", "");//载入OEE异常模组代码
                    //er.Moduleinfo = lineData.Split(',')[5];//载入OEE异常模组状态
                    //Log.WriteLog(ed_key.ToString() + "  " + er.errorinfo.ToString());
                    Global.ed.Add(ed_key, er);
                    Global.ED.Add(ed_key, er);
                }
                sr.Close();
                fs.Close();
                Log.WriteLog("导入报警信息表成功  " + Global.ed.Count);
                //------------------------------------------------------------------------
                FileStream fs2 = new FileStream("参数名称.csv", FileMode.Open, FileAccess.Read);
                StreamReader sr2 = new StreamReader(fs2, Encoding.Default);
                string lineData2;
                while ((lineData2 = sr2.ReadLine()) != null)
                {
                    PLCDataName er = new PLCDataName();
                    int ed_key = Convert.ToInt32(lineData2.Split(',')[0]);
                    er.PLC_Name = lineData2.Split(',')[1];//载入PLC参数名称
                    er.PLC_Address = lineData2.Split(',')[2];//载入PLC寄存器地址
                    Global.PLC_DataName.Add(ed_key, er);
                }
                fs2.Close();
                sr2.Close();
                Log.WriteLog("导入PLC参数信息表成功");
            }
            catch (Exception ex)
            {
                MessageBox.Show("导入报警信息表/PLC参数信息表失败！" + ex.ToString().Replace("\n", ""));
                Log.WriteLog("导入报警信息表/PLC参数信息表失败！" + ex.ToString().Replace("\n", ""));
                Environment.Exit(1);
            }
            //   Thread.Sleep(10000);

            //连接2个大族镭焊机
            Global.client1 = new AsyncTcpClient(new IPEndPoint(IPAddress.Parse("192.168.111.1"), Convert.ToInt32("9000")));
            Global.client1.ServerDisconnected += new EventHandler<TcpServerDisconnectedEventArgs>(client1_ServerDisconnected);
            Global.client1.PlaintextReceived += new EventHandler<TcpDatagramReceivedEventArgs<string>>(client1_PlaintextReceived);
            Global.client1.ServerConnected += new EventHandler<TcpServerConnectedEventArgs>(client1_ServerConnected);
            Global.client1.Connect();
            Global.client2 = new AsyncTcpClient(new IPEndPoint(IPAddress.Parse("192.168.111.2"), Convert.ToInt32("9000")));
            Global.client2.ServerDisconnected += new EventHandler<TcpServerDisconnectedEventArgs>(client2_ServerDisconnected);
            Global.client2.PlaintextReceived += new EventHandler<TcpDatagramReceivedEventArgs<string>>(client2_PlaintextReceived);
            Global.client2.ServerConnected += new EventHandler<TcpServerConnectedEventArgs>(client2_ServerConnected);
            Global.client2.Connect();
            //连接 PDCA ua - MAC min\

            try
            {
                bc.openServerConnection(Global.inidata.productconfig.PDCA_UA_IP, Int32.Parse(Global.inidata.productconfig.PDCA_UA_Port), "169.254.1.100");
                Log.WriteLog("已连接MacMini");
                ShowStatus("已连接MacMini", Color.DarkSeaGreen, 1);
                isopen = true;
                Global.PLC_Client.WritePLC_D(10102, new short[] { 1 });
            }
            catch
            {
                Log.WriteLog("与MacMini断开连接");
                ShowStatus("与MacMini断开连接", Color.Red, 1);
                Global.PLC_Client.WritePLC_D(10102, new short[] { 2 });
            }
            Thread.Sleep(10);
            //连接Mac mini共享文件夹
            //status = connectState(@"\\169.254.1.10\Public", "gdlocal", "gdlocal");
            //if (status)
            //{
            //    Log.WriteLog("连接Macmini共享文件成功");
            //}
            //else
            //{
            //    Log.WriteLog("未能连接Macmini共享文件！");
            //}
            //MAC_mini配置参数校验
            try
            {
                bc.sendMessage("ghi_site\n");
                string uaSite = bc.getReply();
                bc.sendMessage("ghi_product\n");
                string uaProduct = bc.getReply();
                bc.sendMessage("ghi_station_type\n");
                string uaStation_type = bc.getReply();
                bc.sendMessage("ghi_location\n");
                string uaLocation = bc.getReply();
                bc.sendMessage("ghi_line_number\n");
                string uaLine_number = bc.getReply();
                bc.sendMessage("ghi_station_number\n");
                string uaStation_number = bc.getReply();

                string iniua_site = Global.inidata.productconfig.UA_site + "\n";
                string iniua_product = Global.inidata.productconfig.UA_product + "\n";
                string iniua_station_type = Global.inidata.productconfig.UA_station_type + "\n";
                string iniua_location = Global.inidata.productconfig.UA_location + "\n";
                string iniua_line_number = Global.inidata.productconfig.UA_line_number + "\n";
                string iniua_station_number = Global.inidata.productconfig.UA_station_number + "\n";

                if (uaSite == iniua_site && uaProduct == iniua_product && uaStation_type == iniua_station_type &&
                    uaLocation == iniua_location && uaLine_number == iniua_line_number && uaStation_number == iniua_station_number)
                {
                    Log.WriteLog("MAC_mini_ua配置信息校验成功");
                    Mac_mini_server_ua = true;
                }
                else
                {
                    Log.WriteLog("MAC_mini_ua配置信息校验错误");
                    Global.PLC_Client.WritePLC_D(10102, new short[] { 2 });
                    MessageBox.Show("MAC_mini_ua配置信息错误，请找相关人员处理!!");
                    Mac_mini_server_ua = false;
                }
            }
            catch
            {
                Log.WriteLog("MAC_mini配置信息校验异常");
                Global.PLC_Client.WritePLC_D(10102, new short[] { 2 });
                MessageBox.Show("MAC_mini配置信息异常，请找相关人员处理!!");
                Mac_mini_server_ua = false;
            }

            try
            {
                //读取逾期治具
                string strMes = "";
                string[] PISMsg = new string[] { };
                string URL = Global.inidata.productconfig.PIS_URL.Replace("Project", "Project=" + Global.project).Replace("Station", "Station=" + Global.station).Replace("Type", "Type=" + Global.type);
                RequestAPI2.PIS_System(URL, out strMes, out PISMsg);
                Txt.WriteLine(PISMsg);
                for (int J = 0; J < PISMsg.Length; J++)
                {
                    if (PISMsg[J] != null)
                    {
                        _homefrm.AddList(PISMsg[J], "list_FixtureMsg");
                        Log.WriteLog("定时读取逾期保养治具编号：" + strMes + PISMsg[J]);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.WriteLog("逾期治具接口异常" + ex.ToString());
                MessageBox.Show("逾期治具接口异常，请找相关人员处理!!");
            }

            try
            {
                string strMes1 = "";
                JsonSerializerSettings jsetting = new JsonSerializerSettings();
                jsetting.NullValueHandling = NullValueHandling.Ignore;//Json不输出空值
                IQCSystemDATA IQCData = new IQCSystemDATA();
                IQCData.Plant_Organization_Name = "CTU";
                IQCData.BG_Organization_Name = "OP2";
                IQCData.FunPlant_Organization_Name = "组装";
                IQCData.Project_Name = "Burbank";
                IQCData.WorkStation_Name = "Trench Bracket";
                string SendIQCData = JsonConvert.SerializeObject(IQCData, Formatting.None, jsetting);

                string[] PISMsg1 = new string[] { };
                string URL1 = Global.inidata.productconfig.IQC_URL;
                var IQCResult = RequestAPI2.IQC_System(URL1, SendIQCData, out strMes1, out PISMsg1);
                if (IQCResult)
                {
                    Txt.WriteLine1(PISMsg1);
                    for (int J = 0; J < PISMsg1.Length; J++)
                    {
                        if (PISMsg1[J] != null && !PISMsg1[J].Contains("参数错误"))
                        {
                            _homefrm.AddList(PISMsg1[J], "list_IQCFixture");
                            Log.WriteLog("定时读取IQC治具编号：" + strMes1 + PISMsg1[J]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.WriteLog("IQC接口异常" + ex.ToString());
                MessageBox.Show("IQC接口异常，请找相关人员处理!!");
            }
            #region 禁用2024
            //try
            //{


            //    //连接打卡式刷卡机
            //    Global._serial1 = new SerialPort();
            //    Global._serial1.DataReceived += new SerialDataReceivedEventHandler(_userloginfrm.serial1_DataReceived);
            //    Global._serial1.PortName = Global.inidata.productconfig.ID_COM;          //串口号
            //    Global._serial1.BaudRate = 9600;            //波特率
            //    Global._serial1.DataBits = 8;               //数据位
            //    Global._serial1.StopBits = StopBits.One;    //停止位
            //    Global._serial1.Parity = Parity.None;       //校验位
            //    if (Global._serial1.IsOpen == true)         //如果打开状态，则先关闭一下
            //    {
            //        Global._serial1.Close();
            //    }
            //    Global._serial1.Open();                     //打开串口
            //                                                //连接固定式刷卡机
            //    if (!Global.bConnectedDevice)
            //    {
            //        int port = 0;
            //        int baud = 0;
            //        int status1;
            //        port = Convert.ToInt32(Global.inidata.productconfig.IC_COM);              //串口号
            //        baud = Convert.ToInt32(19200);          //波特率
            //        status1 = rf_init_com(0, port, baud);
            //        if (0 == status1)
            //        {
            //            Global.bConnectedDevice = true;
            //            Log.WriteLog("固定式刷卡机端口连接成功");
            //        }
            //        else
            //        {
            //            string strError;
            //            strError = "固定式刷卡机端口连接失败";
            //            Global.bConnectedDevice = false;
            //            MessageBox.Show(strError, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //            Log.WriteLog("固定式刷卡机端口连接失败");
            //        }
            //    }
            //    else
            //    {
            //        rf_ClosePort();
            //        Global.bConnectedDevice = false;
            //        Log.WriteLog("关闭固定式刷卡机端口");
            //    }
            //    if (Global.bConnectedDevice && Global._serial1.IsOpen)
            //    {
            //        ShowStatus("已连接刷卡机", Color.DarkSeaGreen, 5);
            //    }
            //    else
            //    {
            //        ShowStatus("与刷卡机断开", Color.Red, 5);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Log.WriteLog(ex.Message);

            //} 
            #endregion
            //发送初始权限
            Global.PLC_Client.WritePLC_D(16000, new short[] { 0 });//发送权限等级给PLC
            Global.Login = Global.LoginLevel.Operator;// 切换追溯软件权限                
            if (Global.client1.Connected && Global.client2.Connected)
            {
                if (Global.client1.Connected && Global.client2.Connected)
                {
                    Global.client1.Send(String.Format("USER;{0};{1};{2};\r\n", "LEV1", "", ""));// 发送权限等级给大族焊接机-1
                }
                if (Global.client2.Connected)
                {
                    Global.client2.Send(String.Format("USER;{0};{1};{2};\r\n", "LEV1", "", ""));// 发送权限等级给大族焊接机-2
                }
            }
            ////连线OEE
            //try
            //{

            //    Global._mqttClient = new MqttClient(Global.inidata.productconfig.MQTT_IP);  //Host Name  
            //    Global._mqttClient.MqttMsgPublishReceived += Client_MqttMsgPublishReceived;//接收信息
            //    Global._mqttClient.MqttMsgSubscribed += Client_MqttMsgSubscribed;
            //    Global._mqttClient.MqttMsgPublished += MqttClient_MqttMsgPublished;//发布
            //    Global._mqttClient.ConnectionClosed += MqttClient_ConnectionClosed;//断线
            //    Global._mqttClient.Connect(Guid.NewGuid().ToString(), Global.inidata.productconfig.MQTTUserName, Global.inidata.productconfig.MQTTPassword, false, 60);  //username, password
            //    if (Global._mqttClient.IsConnected)
            //    {
            //        string[] responseTopics = new string[4]
            //        {
            //            Global.inidata.productconfig.EMT + "/respond/oee",
            //            Global.inidata.productconfig.EMT + "/respond/downtime",
            //            Global.inidata.productconfig.EMT + "/respond/pant",
            //            "getservertime"
            //        };
            //        byte[] qosLevels = new byte[4]
            //        {
            //            MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE,
            //            MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE,
            //            MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE,
            //            MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE
            //        };
            //        Global._mqttClient.Subscribe(responseTopics, qosLevels);
            //    }
            //    else
            //    {
            //        Global.ConnectOEEFlag = false;
            //        MessageBox.Show("OEE连线失败");
            //        Log.WriteLog("OEE连线失败");
            //        ShowStatus("与OEE断开连接", Color.Red, 3);
            //    }
            //}
            //catch (Exception EX)
            //{
            //    Global.ConnectOEEFlag = false;
            //    Log.WriteLog("OEE连线失败" + EX.ToString() + ",OEELog");
            //    ShowStatus("与OEE断开连接", Color.Red, 3);
            //    MessageBox.Show("OEE连线失败");
            //}
            Global.PLC_Client.WritePLC_D(16014, new short[] { 0 });//通知PLC做首件做验证已结束
            InitTimer();//初始化定时器           
            Worker_thread();
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;//初始化窗体最大化

            ///20230228Add Test
            //string[] data = ("FM7GSK000CF00004RT;0.000;0.000;4;0.5;0;0;0;1;PASS;2023-02-28 10:15:51;HFF662004058;4000309600;0.00ms100.00%/100.00ms100.00%/0.0ms0.0%;48.1;50;0;700;50;0.5;0;0;NA;Trenh bracke;0ms0%/0ms0%/0ms0%;0;0;0;0;0;0;0;0;0ms0%/0ms0%/0ms0%;0;0;0;0;0;0;0;0;0ms0%/0ms0%/0ms0%;0;0;0;0;0;0;0;0;HSG Ground Tab右;0ms0%/0ms0%/0ms0%;0;0;0;0;0;0;0;0;0ms0%/0ms0%/0ms0%;0;0;0;0;0;0;0;0;0ms0%/0ms0%/0ms0%;0;0;0;0;0;0;0;0;HSG Ground Tab左;0ms0%/0ms0%/0ms0%;0;0;0;0;0;0;0;0;0ms0%/0ms0%/0ms0%;0;0;0;0;0;0;0;0;0ms0%/0ms0%/0ms0%;0;0;0;0;0;0;0;0;").Replace("\r\n", "").Split(';');
            //SendlaseripqcData(data);
        }
        #endregion

        #region 开启工作线程
        private void Worker_thread()
        {
            Thread plcDeal = new Thread(EthPolling);//PLC交互     
            plcDeal.IsBackground = true;
            plcDeal.Start();

            Thread plcDeal1 = new Thread(EthPolling1);//PLC交互
            plcDeal1.IsBackground = true;
            plcDeal1.Start();
            Thread plcData = new Thread(EthPolling_Data);//PLC 产能统计
            plcData.IsBackground = true;
            plcData.Start();
            Thread Trace_UA = new Thread(EthTraceUA);//Trace_ua1发送数据 //ua1表示焊接的Brace
            Trace_UA.IsBackground = true;
            Trace_UA.Start();

            //Thread Trace_UA2 = new Thread(EthTraceUA2);//Trace_ua2发送数据  //UA2 焊接的长条
            //Trace_UA2.IsBackground = true;
            //Trace_UA2.Start();

            Thread lv_fixtrue = new Thread(_homefrm.Show_lvFixtrue);//治具数量界面显示
            lv_fixtrue.IsBackground = true;
            lv_fixtrue.Start();

            Thread PDCA_UA = new Thread(EthPdcaUA);//PDCA_ua发送数据
            PDCA_UA.IsBackground = true;
            PDCA_UA.Start();
            Thread Clear_Pic = new Thread(ClearPic);//定时删除图片
            Clear_Pic.IsBackground = true;
            Clear_Pic.Start();
            //Thread Default = new Thread(EthOEEDefault);//OEE发送过站数据
            //Default.IsBackground = true;
            //Default.Start();
            //Thread HeartBeat = new Thread(EthOEEHeartBeat_Mqtt);//发送OEE 心跳信号
            //HeartBeat.IsBackground = true;
            //HeartBeat.Start();
            Thread DownTime = new Thread(EthDownTime_Mqtt);//发送OEE DOWN TIME
            DownTime.IsBackground = true;
            DownTime.Start();
            //Thread DownTime_Retry = new Thread(OEE_DownTime_Retry);//缓存重传OEE DT
            //DownTime_Retry.IsBackground = true;
            //DownTime_Retry.Start();
            //Thread Default_Retry = new Thread(OEE_Default_Retry);//缓存重传OEE 过站数据
            //Default_Retry.IsBackground = true;
            //Default_Retry.Start();
            Thread UD_dt = new Thread(_datastatisticsfrm.UD_DataTable);
            UD_dt.IsBackground = true;
            UD_dt.Start();
            ThreadPool.QueueUserWorkItem(On_Time_doing);//按时做某事
            ThreadPool.QueueUserWorkItem(CacheQuantity);//上传NG缓存数量
            ThreadPool.QueueUserWorkItem(PLC_autolink);//PLC自动重连
            ThreadPool.QueueUserWorkItem(Ping_ip);//检测PLC与Macmini是否连接
            ThreadPool.QueueUserWorkItem(CheckConnected);//连接状态判断
            ThreadPool.QueueUserWorkItem(Timing);//发送PC时间给PLC
            ThreadPool.QueueUserWorkItem(Permission_switch);//权限自动切换
            ThreadPool.QueueUserWorkItem(ReadOperatorID);//固定式刷卡机读卡
            Thread.Sleep(10);
        }
        #endregion
        #region 连接状态判断
        private void CheckConnected(object ob)
        {
            while (isopen)
            {
                try
                {
                    //------------------------PLC------------------------------
                    if (Link_PLC && ConnectPLC)
                    {
                        ShowStatus("已连接PLC", Color.DarkSeaGreen, 0);
                    }
                    else
                    {
                        ShowStatus("与PLC断开连接", Color.Red, 0);
                    }
                    //------------------------OEE-----------------------------
                    if (Global.oeeSend_ng == 0 && OEE_Default_flag == true && Global.ConnectOEEFlag)
                    {
                        ShowStatus("已连接OEE", Color.DarkSeaGreen, 3);
                        Global.PLC_Client.WritePLC_D(10108, new short[] { 1 });
                    }
                    else if (Global.oeeSend_ng > 0 && Global.oeeSend_ng < Convert.ToInt32(Global.Threshold) && OEE_Default_flag == true)
                    {
                        ShowStatus("与OEE断开连接", Color.Red, 3);
                    }
                    else if (Global.oeeSend_ng >= 0 && Global.oeeSend_ng < Convert.ToInt32(Global.Threshold) && OEE_Default_flag == false)
                    {
                        ShowStatus("与OEE断开连接", Color.Red, 3);
                    }
                    else if (!Global.ConnectOEEFlag)
                    {
                        ShowStatus("与OEE断开连接", Color.Red, 3);
                    }
                    //------------------------Trace----------------------------
                    if (Global.TraceSendua_ng == 0 && Trace_Logs_flag == true && Trace_check_flag)
                    {
                        ShowStatus("已连接Trace", Color.DarkSeaGreen, 2);
                        Global.PLC_Client.WritePLC_D(10104, new short[] { 1 });
                    }
                    else if (Global.TraceSendua_ng > 0 && Global.TraceSendua_ng < Convert.ToInt32(1) && Trace_Logs_flag == true)
                    {
                        ShowStatus("与Trace断开连接", Color.Red, 2);
                    }
                    else if (Global.TraceSendua_ng >= Convert.ToInt32(1) && Trace_Logs_flag == true)
                    {
                        ShowStatus("与Trace断开连接", Color.Red, 2);
                        Global.PLC_Client.WritePLC_D(10104, new short[] { 2 });
                    }
                    else if (Global.TraceSendua_ng >= 0 && Global.TraceSendua_ng < Convert.ToInt32(1) && Trace_Logs_flag == false)
                    {
                        ShowStatus("与Trace断开连接", Color.Red, 2);
                    }
                    else if (Global.TraceSendua_ng >= Convert.ToInt32(1) && Trace_Logs_flag == false)
                    {
                        ShowStatus("与Trace断开连接", Color.Red, 2);
                        Global.PLC_Client.WritePLC_D(10104, new short[] { 2 });
                    }
                    else if (!Trace_check_flag)
                    {
                        ShowStatus("与Trace断开连接", Color.Red, 2);
                    }
                    //------------------------PDCA-ua----------------------------
                    if (Global.PDCA_ua_Data_NG == 0 && Mac_mini_server_ua && Link_Mac_Mini_Server)
                    {
                        ShowStatus("已连接MacMini", Color.DarkSeaGreen, 1);
                        Global.PLC_Client.WritePLC_D(10102, new short[] { 1 });
                    }
                    else if (Global.PDCA_ua_Data_NG > 0 && Global.PDCA_ua_Data_NG < Convert.ToInt32(Global.Threshold) && Mac_mini_server_ua == true)
                    {
                        ShowStatus("与MacMini断开连接", Color.Red, 1);
                    }
                    else if (Global.PDCA_ua_Data_NG >= Convert.ToInt32(Global.Threshold) && Mac_mini_server_ua == true)
                    {
                        ShowStatus("与MacMini断开连接", Color.Red, 1);
                        Global.PLC_Client.WritePLC_D(10102, new short[] { 2 });
                    }
                    else if (Global.PDCA_ua_Data_NG >= 0 && Global.PDCA_ua_Data_NG < Convert.ToInt32(Global.Threshold) && Mac_mini_server_ua == false)
                    {
                        ShowStatus("与MacMini断开连接", Color.Red, 1);
                    }
                    else if (Global.PDCA_ua_Data_NG >= Convert.ToInt32(Global.Threshold) && Mac_mini_server_ua == false)
                    {
                        ShowStatus("与MacMini断开连接", Color.Red, 1);
                        Global.PLC_Client.WritePLC_D(10102, new short[] { 2 });
                    }
                    else if (!Mac_mini_server_ua || !Link_Mac_Mini_Server)
                    {
                        ShowStatus("与MacMini断开连接", Color.Red, 1);
                    }
                }
                catch (Exception ex)
                {
                    Log.WriteLog("显示连接状态异常失败！" + ex.ToString().Replace("\n", ""));
                }

                try
                {
                    if (Global.client1.Connected && Global.client2.Connected)
                    {
                        ShowStatus("已连接大族镭焊机", Color.DarkSeaGreen, 4);
                        Global.PLC_Client.WritePLC_D(10106, new short[] { 1 });
                        TCPconnected = true;
                    }
                    else//与大族焊接机断开重连
                    {
                        ShowStatus("与大族镭焊机断开", Color.Red, 4);
                        Global.PLC_Client.WritePLC_D(10106, new short[] { 2 });
                        TCPconnected = false;
                        if (!Global.client1.Connected)
                        {
                            Global.client1.Close();
                            Global.client1 = new AsyncTcpClient(new IPEndPoint(IPAddress.Parse("192.168.111.1"), Convert.ToInt32("9000")));
                            Global.client1.ServerDisconnected += new EventHandler<TcpServerDisconnectedEventArgs>(client1_ServerDisconnected);
                            Global.client1.PlaintextReceived += new EventHandler<TcpDatagramReceivedEventArgs<string>>(client1_PlaintextReceived);
                            Global.client1.ServerConnected += new EventHandler<TcpServerConnectedEventArgs>(client1_ServerConnected);
                            Global.client1.Connect();  
                        }
                        if (!Global.client2.Connected)
                        {
                            Global.client2.Close();
                            Global.client2 = new AsyncTcpClient(new IPEndPoint(IPAddress.Parse("192.168.111.2"), Convert.ToInt32("9000")));
                            Global.client2.ServerDisconnected += new EventHandler<TcpServerDisconnectedEventArgs>(client2_ServerDisconnected);
                            Global.client2.PlaintextReceived += new EventHandler<TcpDatagramReceivedEventArgs<string>>(client2_PlaintextReceived);
                            Global.client2.ServerConnected += new EventHandler<TcpServerConnectedEventArgs>(client2_ServerConnected);
                            Global.client2.Connect();
                        }
                        Thread.Sleep(1000);
                    }
                }
                catch (Exception ex)
                {
                    Log.WriteLog("向PLC写入与大族连接状态失败！" + ex.ToString().Replace("\n", ""));
                    TCPconnected = false;
                }
                try
                {
                    //刷卡机重连
                    if (tssl_ReaderStatus.BackColor == Color.Red)
                    {
                        if (!Global._serial1.IsOpen)
                        {
                            //连接打卡式刷卡机
                            Global._serial1 = new SerialPort();
                            Global._serial1.DataReceived += new SerialDataReceivedEventHandler(_userloginfrm.serial1_DataReceived);
                            Global._serial1.PortName = Global.inidata.productconfig.ID_COM;          //串口号
                            Global._serial1.BaudRate = 9600;            //波特率
                            Global._serial1.DataBits = 8;               //数据位
                            Global._serial1.StopBits = StopBits.One;    //停止位
                            Global._serial1.Parity = Parity.None;       //校验位
                            if (Global._serial1.IsOpen == true)         //如果打开状态，则先关闭一下
                            {
                                Global._serial1.Close();
                            }
                            Global._serial1.Open();
                            if (Global._serial1.IsOpen)
                            {
                                Log.WriteLog("打卡式刷卡机端口重连成功");
                            }
                        }
                        else if (Global.bConnectedDevice)
                        {
                            //连接固定式刷卡机
                            int port = 0;
                            int baud = 0;
                            int status1;
                            port = Convert.ToInt32(Global.inidata.productconfig.IC_COM);              //串口号
                            baud = Convert.ToInt32(19200);          //波特率
                            status1 = rf_init_com(0, port, baud);
                            if (0 == status1)
                            {
                                Global.bConnectedDevice = true;
                                Log.WriteLog("固定式刷卡机端口重连成功");
                            }
                        }
                        Thread.Sleep(1000);
                    }
                }
                catch (Exception EX)
                {
                    Log.WriteLog("连接刷卡机异常" + EX.ToString());
                    Thread.Sleep(1000);
                }
                //0EE断线重连
                //if (!Global._mqttClient.IsConnected)
                //{
                //    try
                //    {
                //        Global._mqttClient = new MqttClient(Global.inidata.productconfig.MQTT_IP);  //Host Name  
                //        Global._mqttClient.MqttMsgPublishReceived += Client_MqttMsgPublishReceived;//接收信息
                //        Global._mqttClient.MqttMsgSubscribed += Client_MqttMsgSubscribed;
                //        Global._mqttClient.MqttMsgPublished += MqttClient_MqttMsgPublished;//发布
                //        Global._mqttClient.ConnectionClosed += MqttClient_ConnectionClosed;//断线
                //        Global._mqttClient.Connect(Guid.NewGuid().ToString(), Global.inidata.productconfig.MQTTUserName, Global.inidata.productconfig.MQTTPassword, false, 60);  //username, password
                //        if (Global._mqttClient.IsConnected)
                //        {
                //            string[] responseTopics = new string[4]
                //            {
                //                    Global.inidata.productconfig.EMT + "/respond/oee",
                //                    Global.inidata.productconfig.EMT + "/respond/downtime",
                //                    Global.inidata.productconfig.EMT + "/respond/pant",
                //                    "getservertime"
                //            };
                //            byte[] qosLevels = new byte[4]
                //            {
                //                    MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE,
                //                    MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE,
                //                    MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE,
                //                    MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE
                //            };
                //            Global._mqttClient.Subscribe(responseTopics, qosLevels);
                //        }
                //        Thread.Sleep(1000);
                //    }
                //    catch (Exception EX)
                //    {
                //        Log.WriteLog("OEE网络异常" + EX.ToString() + ",OEELog");
                //        ShowStatus("与OEE断开连接", Color.Red, 3);
                //        Thread.Sleep(1000);
                //    }
                //}
                Thread.Sleep(500);
            }
        }


        #endregion

        private void EthPolling1()
        {
            while (true && bclose)
            {
                try
                {
                    //DateTime dateTime = DateTime.Now;
                    t1 = Global.PLC_Client2.ReadPLC_D(10250, 15);//发送SN给?#焊接机
                    ua_Trg1 = Global.PLC_Client2.ReadPLC_D(10300, 1);
                    P = Global.PLC_Client2.ReadPLC_D(10130, 1);
                    process = Global.PLC_Client2.ReadPLC_D(10120, 1);//Trace前站TRG
                    ProductWayOut = Global.PLC_Client.ReadPLC_D(13000, 1);//产品出流道
                    //Global.PLC_Client.WritePLC_D(10120, new short[] { 1 });
                    if (process[0] == 1)//Trace前站TRG
                    {
                        string Fullsn1 = "";
                        Fullsn1 = Global.PLC_Client2.ReadPLC_Dstring(10140, 20).Replace(" ", "").Replace("\0", "");
                        if (Fullsn1.Length>18)
                        {
                            Global.PLC_Client.WritePLC_D(10122, new short[] { 1 });
                            Global.PLC_Client2.WritePLC_D(10120, new short[] { 0 });
                            ThreadInfo threadInfo = new ThreadInfo();
                            threadInfo.SN = Fullsn1;
                            ThreadPool.QueueUserWorkItem(new WaitCallback(ProcessControl), threadInfo);
                        }
                        else
                        { Global.PLC_Client.WritePLC_D(10122, new short[] { 2 }); }
                        
                    }
                    if (P[0] == 1)//治具校验
                    {
                        string FixtureID = Global.PLC_Client2.ReadPLC_Dstring(10160, 15).Replace(" ", "").Replace("\0", "");
                        Global.PLC_Client2.WritePLC_D(10130, new short[] { 0 });
                        ThreadInfo threadInfo = new ThreadInfo();
                        threadInfo.FixtureID = FixtureID;
                        ThreadPool.QueueUserWorkItem(new WaitCallback(CheckFixtureID), threadInfo);
                    }
                    if (ua_Trg1[0] == 1)//上传Trace_Bracket1数据
                    {
                        dateTime = DateTime.Now;
                        Global.PLC_Client2.WritePLC_D(10300, new short[] { 0 });//TRG清零
                        ThreadInfo threadInfo = new ThreadInfo();
                        threadInfo.FileName = "Bracket";
                        threadInfo.SelectedIndex = 1;
                        ThreadPool.QueueUserWorkItem(new WaitCallback(UploadData_UA1), threadInfo);
                    }
                    //if (ua_Trg1[0] == 1)//上传Trace_Bracket1数据
                    //{
                    //    Global.PLC_Client2.WritePLC_D(10300, new short[] { 0 });//TRG清零
                    //    ThreadInfo threadInfo = new ThreadInfo();
                    //    threadInfo.FileName = "Bracket";
                    //    threadInfo.SelectedIndex = 1;
                    //    ThreadPool.QueueUserWorkItem(new WaitCallback(UploadData_UA1), threadInfo);
                    //}
                    if (t1[0] == 1)//1#焊前
                    {
                        string Fullsn1 = Global.PLC_Client2.ReadPLC_Dstring(10200, 20).Replace(" ", "").Replace("\0", "");
                        Global.PLC_Client2.WritePLC_D(10250, new short[] { 0 });
                        ThreadInfo threadInfo = new ThreadInfo();
                        threadInfo.SelectedIndex = 1;
                        threadInfo.SN = Fullsn1;
                        ThreadPool.QueueUserWorkItem(new WaitCallback(SendDataToHans), threadInfo);
                    }
                    if (t1[2] == 1)//2#焊前
                    {
                        string Fullsn1 = Global.PLC_Client2.ReadPLC_Dstring(10220, 20).Replace(" ", "").Replace("\0", "");
                        Global.PLC_Client2.WritePLC_D(10252, new short[] { 0 });
                        ThreadInfo threadInfo = new ThreadInfo();
                        threadInfo.SelectedIndex = 2;
                        threadInfo.SN = Fullsn1;
                        ThreadPool.QueueUserWorkItem(new WaitCallback(SendDataToHans), threadInfo);
                    }
                    if (t1[10] == 1)//1#焊后
                    {
                        string Fullsn = Global.PLC_Client2.ReadPLC_Dstring(10200, 20).Replace(" ", "").Replace("\0", "");
                        Global.PLC_Client2.WritePLC_D(10260, new short[] { 0 });
                        ThreadInfo threadInfo = new ThreadInfo();
                        threadInfo.SelectedIndex = 1;
                        threadInfo.SN = Fullsn;
                        ThreadPool.QueueUserWorkItem(new WaitCallback(GetDataForHans), threadInfo);
                    }
                    if (t1[12] == 1)//2#焊后
                    {
                        string Fullsn = Global.PLC_Client2.ReadPLC_Dstring(10220, 20).Replace(" ", "").Replace("\0", "");
                        Global.PLC_Client2.WritePLC_D(10262, new short[] { 0 });
                        ThreadInfo threadInfo = new ThreadInfo();
                        threadInfo.SelectedIndex = 2;
                        threadInfo.SN = Fullsn;
                        ThreadPool.QueueUserWorkItem(new WaitCallback(GetDataForHans), threadInfo);
                    }
                    if (ProductWayOut[0] == 1)
                    {
                        Global.PLC_Client.WritePLC_D(13000, new short[] { 0 });
                        string SN = Global.PLC_Client.ReadPLC_Dstring(12520, 20);//SN
                        short Way = Global.PLC_Client.ReadPLC_D(13002, 1)[0];//流道（1-OK 2-NG 3-首件）
                        string WayOut = string.Empty;
                        switch (Way)
                        {
                            case 1:
                                WayOut = "OK流道";
                                break;
                            case 2:
                                WayOut = "NG流道";
                                break;
                            case 3:
                                WayOut = "首件流道";
                                break;
                        }
                        Task T1 = new Task(() => Log.WriteCSV(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + SN + "," + WayOut, System.AppDomain.CurrentDomain.BaseDirectory + "\\产品出流道记录\\"));
                        T1.Start();
                    }
                    //_homefrm.AppendRichText((DateTime.Now - dateTime).TotalMilliseconds.ToString(), "rtx_HeartBeatMsg");
                }
                catch (Exception ex)
                { Log.WriteLog(ex.ToString()); }
            }
        }
        #region PLC交互处理  MES-校验前站 Trace-校验前站
        private void EthPolling()
        {
            while (true && bclose)
            {
                //DateTime dateTime = DateTime.Now;
                try
                {
                    //与PLC心跳信号
                    if (PLCHeart)
                    {
                        if (ison)
                        {
                            try
                            {
                                Global.PLC_Client.WritePLC_D(10100, new short[] { 1 });
                            }
                            catch
                            {
                            }
                            ison = false;
                        }
                        else
                        {
                            try
                            {
                                Global.PLC_Client.WritePLC_D(10100, new short[] { 0 });
                            }
                            catch
                            {
                            }
                            ison = true;
                        }
                    }

                    //process = Global.PLC_Client.ReadPLC_D(10120, 1);//Trace前站TRG
                    //P = Global.PLC_Client.ReadPLC_D(10130, 1);//PIS治具校验TRG
                    //t1 = Global.PLC_Client.ReadPLC_D(10250, 15);//发送SN给?#焊接机
                    //ua_Trg1 = Global.PLC_Client.ReadPLC_D(10300, 1);//Bracket1数据上传TRG
                    // ua_Trg2 = Global.PLC_Client.ReadPLC_D(10500, 1);//Bracket2数据上传TRG

                    CT_LOG = Global.PLC_Client.ReadPLC_D(9550, 35);//CT记录TRG
                    oldTrg = Global.PLC_Client.ReadPLC_D(16002, 1);//修改前TRG
                    newTrg = Global.PLC_Client.ReadPLC_D(16003, 1);//修改后TRG
                    SmallmaterialTrg = Global.PLC_Client.ReadPLC_D(196, 1);//小料抛料TRG
                    CCDCheckNGTrg = Global.PLC_Client.ReadPLC_D(16008, 1);//CCD检测治具上小料NG TRG
                    SmallmaterialInputTrg = Global.PLC_Client.ReadPLC_D(194, 1);//小料投入TRG
                    ReadBarcodeNGTrg = Global.PLC_Client.ReadPLC_D(197, 1);//读码NG TRG
                    SmallmaterialInputTrg = Global.PLC_Client.ReadPLC_D(1810, 1);//小料投入TRG
                    ReadBarcodeNGTrg = Global.PLC_Client.ReadPLC_D(1801, 1);//读码NG TRG

                    //if (process[0] == 1)//Trace前站TRG
                    //{
                    //    string Fullsn1 = "";
                    //    Fullsn1 = Global.PLC_Client.ReadPLC_Dstring(10140, 20).Replace(" ", "").Replace("\0", "");
                    //    Global.PLC_Client.WritePLC_D(10120, new short[] { 0 });
                    //    ThreadInfo threadInfo = new ThreadInfo();
                    //    threadInfo.SN = Fullsn1;
                    //    //Global.PLC_Client.WritePLC_D(10122, new short[] { 1 });
                    //ThreadPool.QueueUserWorkItem(new WaitCallback(ProcessControl), threadInfo);
                    //}

                    //if (P[0] == 1)//治具校验
                    //{
                    //    Stopwatch ST = new Stopwatch();
                    //    ST.Start();
                    //    string FixtureID = Global.PLC_Client.ReadPLC_Dstring(10160, 15).Replace(" ", "").Replace("\0", "");
                    //    Global.PLC_Client.WritePLC_D(10130, new short[] { 0 });
                    //    ThreadInfo threadInfo = new ThreadInfo();
                    //    threadInfo.FixtureID = FixtureID;
                    //    ThreadPool.QueueUserWorkItem(new WaitCallback(CheckFixtureID), threadInfo);
                    //    ST.Stop();
                    //    Log.WriteLog("治具校验用时：" + ST.ElapsedMilliseconds.ToString());
                    //}
                    //if (t1[0] == 1)//1#焊前
                    //{
                    //    string Fullsn1 = Global.PLC_Client.ReadPLC_Dstring(10200, 20).Replace(" ", "").Replace("\0", "");
                    //    Global.PLC_Client.WritePLC_D(10250, new short[] { 0 });
                    //    ThreadInfo threadInfo = new ThreadInfo();
                    //    threadInfo.SelectedIndex = 1;
                    //    threadInfo.SN = Fullsn1;
                    //ThreadPool.QueueUserWorkItem(new WaitCallback(SendDataToHans), threadInfo);
                    //}
                    //if (t1[10] == 1)//1#焊后
                    //{
                    //    string Fullsn = Global.PLC_Client.ReadPLC_Dstring(10200, 20).Replace(" ", "").Replace("\0", "");
                    //    Global.PLC_Client.WritePLC_D(10260, new short[] { 0 });
                    //    ThreadInfo threadInfo = new ThreadInfo();
                    //    threadInfo.SelectedIndex = 1;
                    //    threadInfo.SN = Fullsn;
                    //    ThreadPool.QueueUserWorkItem(new WaitCallback(GetDataForHans), threadInfo);
                    //}
                    //if (ua_Trg1[0] == 1)//上传Trace_Bracket1数据
                    //{
                    //    Global.PLC_Client.WritePLC_D(10300, new short[] { 0 });//TRG清零
                    //    ThreadInfo threadInfo = new ThreadInfo();
                    //    threadInfo.FileName = "Bracket";
                    //    threadInfo.SelectedIndex = 1;
                    //    ThreadPool.QueueUserWorkItem(new WaitCallback(UploadData_UA1), threadInfo);
                    //}

                    if (oldTrg[0] == 1)
                    {
                        Global.PLC_Client.WritePLC_D(16002, new short[] { 0 });//TRG清零
                        ThreadInfo threadInfo = new ThreadInfo();
                        threadInfo.SelectedIndex = 1;
                        ThreadPool.QueueUserWorkItem(new WaitCallback(WritePLCData), threadInfo);
                    }
                    if (newTrg[0] == 1)
                    {
                        Global.PLC_Client.WritePLC_D(16003, new short[] { 0 });//TRG清零
                        ThreadInfo threadInfo = new ThreadInfo();
                        threadInfo.SelectedIndex = 2;
                        ThreadPool.QueueUserWorkItem(new WaitCallback(WritePLCData), threadInfo);
                    }
                    //--------------------------------------------------------------------------

                    if (SmallmaterialInputTrg[0] == 1)//小料投入计数
                    {
                        Global.PLC_Client.WritePLC_D(194, new short[] { 0 });
                        if (Global.SelectFirst == false)
                        {
                            if (Convert.ToDateTime(DateTime.Now.ToString("HH:mm")).CompareTo(Convert.ToDateTime("6:00")) >= 0 && Convert.ToDateTime(DateTime.Now.ToString("HH:mm")).CompareTo(Convert.ToDateTime("18:00")) < 0)
                            {
                                //白班小料投入计数累加
                                Global.Smallmaterial_Input_D = Global.Smallmaterial_Input_D + 2;
                                _datastatisticsfrm.UpDataDGV_D(1, 1, Global.Smallmaterial_Input_D.ToString());
                                Global.inidata.productconfig.Smallmaterial_Input_D = Global.Smallmaterial_Input_D.ToString();

                            }
                            else
                            {
                                //夜班小料投入计数累加
                                Global.Smallmaterial_Input_N = Global.Smallmaterial_Input_N + 2;
                                _datastatisticsfrm.UpDataDGV_N(1, 1, Global.Smallmaterial_Input_N.ToString());
                                Global.inidata.productconfig.Smallmaterial_Input_N = Global.Smallmaterial_Input_N.ToString();
                            }
                        }
                    }
                    if (ReadBarcodeNGTrg[0] == 1)//读码NG计数
                    {
                        Global.PLC_Client.WritePLC_D(197, new short[] { 0 });
                        if (Global.SelectFirst == false)
                        {
                            if (Convert.ToDateTime(DateTime.Now.ToString("HH:mm")).CompareTo(Convert.ToDateTime("6:00")) >= 0 && Convert.ToDateTime(DateTime.Now.ToString("HH:mm")).CompareTo(Convert.ToDateTime("18:00")) < 0)
                            {
                                Global.ReadBarcode_NG_D++;//白班读码NG计数累加
                                _datastatisticsfrm.UpDataDGV_D(13, 1, Global.ReadBarcode_NG_D.ToString());
                                Global.inidata.productconfig.ReadBarcode_NG_D = Global.ReadBarcode_NG_D.ToString();
                            }
                            else
                            {
                                Global.ReadBarcode_NG_N++;//夜班读码NG计数累加
                                _datastatisticsfrm.UpDataDGV_N(13, 1, Global.ReadBarcode_NG_N.ToString());
                                Global.inidata.productconfig.ReadBarcode_NG_N = Global.ReadBarcode_NG_N.ToString();
                            }
                        }
                    }
                    //_homefrm.AppendRichText((DateTime.Now - dateTime).TotalMilliseconds.ToString(), "rtx_HeartBeatMsg");
                }
                catch (Exception ex)
                {
                    Log.WriteLog("PC与PLC通讯异常：" + ex.ToString().Replace("\n", ""));
                }
                Thread.Sleep(50);
            }
        }

        private void EthPolling_Data()//产能统计 DT统计
        {
            while (true && bclose)
            {
                try
                {
                    Product_DataStatistics();//产能统计
                    DT_DataStatistics();//运行状态统计
                    //Dumpingstatistics();//抛料统计
                    //_datastatisticsfrm.ShowHtml(1);
                    //_datastatisticsfrm.ShowHtml(2);
                    //_datastatisticsfrm.ShowHtml(3);
                }
                catch (Exception ex)
                {
                    Log.WriteLog("PC与PLC通讯异常：" + ex.ToString().Replace("\n", ""));
                }
                Thread.Sleep(500);
            }

        }
        #endregion


        #region Trace-Bracket1
        private void EthTraceUA()
        {
            while (true)
            {
                if (Trace_ua.Count > 0)
                {
                    if (Out_Trace_ua.Count > 0)
                    {
                        string item = Out_Trace_ua[0];
                        #region
                        Global.SN_Request = true;
                        HansData_U_Bracket UA_data = new HansData_U_Bracket();
                        GetAllHansData(item, out UA_data, 1);
                        if (item.Contains("FM") && !Regex.IsMatch(item, "[a-z]"))
                        {
                            JsonSerializerSettings jsetting = new JsonSerializerSettings();
                            jsetting.NullValueHandling = NullValueHandling.Ignore;//Json不输出空值
                            string Trcae_logs_str = null;
                            TimeSpan ts1, ts2;
                            if (Trace_ua[item].data.insight.test_attributes.test_result == "pass" && UA_data.HanDataUAResult)
                            {
                                Trace_ua[item].data.insight.test_attributes.test_result = "pass";
                            }
                            else if (Trace_ua[item].data.insight.test_attributes.test_result == "fail" && !UA_data.HanDataUAResult)
                            {
                                Trace_ua[item].data.insight.test_attributes.test_result = "fail";
                                if (Trace_ua[item].data.insight.uut_attributes.tossing_item == "")
                                {
                                    Trace_ua[item].data.insight.uut_attributes.tossing_item += "Welding data acquire NG";
                                }
                                Trace_ua[item].data.insight.uut_attributes.tossing_item += "/Welding data acquire NG";
                                if (Global.SelectFirst == false)
                                {
                                    if (Convert.ToDateTime(DateTime.Now.ToString("HH:mm")).CompareTo(Convert.ToDateTime("6:00")) >= 0 && Convert.ToDateTime(DateTime.Now.ToString("HH:mm")).CompareTo(Convert.ToDateTime("18:00")) < 0)
                                    {
                                        Global.HansDataError_D++;//白班焊接NG计数累加                                                                 
                                        _datastatisticsfrm.UpDataDGV_D(7, 1, Global.HansDataError_D.ToString());
                                        Global.inidata.productconfig.HansDataError_D = Global.HansDataError_D.ToString();
                                    }
                                    else
                                    {
                                        Global.HansDataError_N++;//夜班焊接NG计数累加
                                        _datastatisticsfrm.UpDataDGV_N(7, 1, Global.HansDataError_N.ToString());
                                        Global.inidata.productconfig.HansDataError_N = Global.HansDataError_N.ToString();
                                    }
                                }
                            }
                            else if (Trace_ua[item].data.insight.test_attributes.test_result == "fail" && UA_data.HanDataUAResult)
                            {
                                Trace_ua[item].data.insight.test_attributes.test_result = "fail";

                            }
                            else if (Trace_ua[item].data.insight.test_attributes.test_result == "pass" && !UA_data.HanDataUAResult)
                            {
                                Trace_ua[item].data.insight.test_attributes.test_result = "fail";
                                Trace_ua[item].data.insight.uut_attributes.tossing_item = "Welding data acquire NG";
                                if (Global.SelectFirst == false)
                                {
                                    if (Convert.ToDateTime(DateTime.Now.ToString("HH:mm")).CompareTo(Convert.ToDateTime("6:00")) >= 0 && Convert.ToDateTime(DateTime.Now.ToString("HH:mm")).CompareTo(Convert.ToDateTime("18:00")) < 0)
                                    {
                                        Global.HansDataError_D++;//白班焊接NG计数累加                                                                 
                                        _datastatisticsfrm.UpDataDGV_D(7, 1, Global.HansDataError_D.ToString());
                                        Global.inidata.productconfig.HansDataError_D = Global.HansDataError_D.ToString();
                                    }
                                    else
                                    {
                                        Global.HansDataError_N++;//夜班焊接NG计数累加
                                        _datastatisticsfrm.UpDataDGV_N(7, 1, Global.HansDataError_N.ToString());
                                        Global.inidata.productconfig.HansDataError_N = Global.HansDataError_N.ToString();
                                    }
                                }
                            }

                            Trace_ua[item].data.insight.uut_attributes.precitec_grading = "0";
                            Trace_ua[item].data.insight.uut_attributes.precitec_rev = "0";
                            Trace_ua[item].data.insight.uut_attributes.hatch = UA_data.hatch;
                            Trace_ua[item].data.insight.uut_attributes.precitec_value = "0";
                            Trace_ua[item].data.insight.uut_attributes.pattern_type = UA_data.pattern_type;
                            Trace_ua[item].data.insight.uut_attributes.spot_size = UA_data.spot_size;
                            Trace_ua[item].data.insight.uut_attributes.swing_amplitude = UA_data.swing_amplitude;
                            Trace_ua[item].data.insight.uut_attributes.swing_freq = UA_data.swing_freq;

                            Trace_ua[item].data.insight.results[0].test = "location1_Trenh bracke";
                            Trace_ua[item].data.insight.results[0].units = "/";
                            Trace_ua[item].data.insight.results[0].value = "0";
                            // Trace_ua[item].data.insight.results[0].result = "pass";
                            Trace_ua[item].data.insight.results[1].test = "location2_shim-NE";
                            Trace_ua[item].data.insight.results[1].units = "/";
                            Trace_ua[item].data.insight.results[1].value = "0";
                            // Trace_ua[item].data.insight.results[1].result = "pass";
                            Trace_ua[item].data.insight.results[2].test = "location3_shim-SL.hs";
                            Trace_ua[item].data.insight.results[2].units = "/";
                            Trace_ua[item].data.insight.results[2].value = "0";

                            Trace_ua[item].data.insight.results[3].test = "location4_shim-SR";
                            Trace_ua[item].data.insight.results[3].units = "/";
                            Trace_ua[item].data.insight.results[3].value = "0";
                            //Trace_ua2[item].data.insight.results[0].result = "pass";
                           

                            //Trace_ua2[item].data.insight.results[1].result = "pass";
                            Trace_ua[item].data.insight.results[4].test = "location1_layer1_laser_power";
                            Trace_ua[item].data.insight.results[4].units = "W";
                            Trace_ua[item].data.insight.results[4].value = UA_data.location4_layer1_laser_power;
                            //   Trace_ua2[item].data.insight.results[2].result = "pass";
                            Trace_ua[item].data.insight.results[5].test = "location1_layer1_frequency";
                            Trace_ua[item].data.insight.results[5].units = "KHz";
                            Trace_ua[item].data.insight.results[5].value = UA_data.location4_layer1_frequency;
                            //  Trace_ua2[item].data.insight.results[3].result = "pass";
                            Trace_ua[item].data.insight.results[6].test = "location1_layer1_waveform";
                            Trace_ua[item].data.insight.results[6].units = "/";
                            Trace_ua[item].data.insight.results[6].value = UA_data.location4_layer1_waveform;
                            //  Trace_ua2[item].data.insight.results[4].result = "pass";
                            Trace_ua[item].data.insight.results[7].test = "location1_layer1_pulse_energy";
                            Trace_ua[item].data.insight.results[7].units = "J";
                            Trace_ua[item].data.insight.results[7].value = UA_data.location4_layer1_pulse_energy;
                            //  Trace_ua2[item].data.insight.results[5].result = "pass";
                            Trace_ua[item].data.insight.results[8].test = "location1_layer1_laser_speed";
                            Trace_ua[item].data.insight.results[8].units = "mm/s";
                            Trace_ua[item].data.insight.results[8].value = UA_data.location4_layer1_laser_speed;
                            //  Trace_ua2[item].data.insight.results[6].result = "pass";
                            Trace_ua[item].data.insight.results[9].test = "location1_layer1_jump_speed";
                            Trace_ua[item].data.insight.results[9].units = "mm/s";
                            Trace_ua[item].data.insight.results[9].value = UA_data.location4_layer1_jump_speed;
                            //   Trace_ua2[item].data.insight.results[7].result = "pass";
                            Trace_ua[item].data.insight.results[10].test = "location1_layer1_jump_delay";
                            Trace_ua[item].data.insight.results[10].units = "us";
                            Trace_ua[item].data.insight.results[10].value = UA_data.location4_layer1_jump_delay;
                            //   Trace_ua2[item].data.insight.results[8].result = "pass";
                            Trace_ua[item].data.insight.results[11].test = "location1_layer1_scanner_delay";
                            Trace_ua[item].data.insight.results[11].units = "us";
                            Trace_ua[item].data.insight.results[11].value = UA_data.location4_layer1_scanner_delay;

                            //  注特别留意  GetAllHansData方法中 location1_HSG Ground Tab左 的值赋值为变量location3

                            //Trace_ua[item].data.insight.results[2].result = "pass";
                            Trace_ua[item].data.insight.results[12].test = "location2_layer1_laser_power";
                            Trace_ua[item].data.insight.results[12].units = "W";
                            Trace_ua[item].data.insight.results[12].value = UA_data.location1_layer1_laser_power;
                            // Trace_ua[item].data.insight.results[3].result = "pass";
                            Trace_ua[item].data.insight.results[13].test = "location2_layer1_frequency";
                            Trace_ua[item].data.insight.results[13].units = "KHz";
                            Trace_ua[item].data.insight.results[13].value = UA_data.location1_layer1_frequency;
                            // Trace_ua[item].data.insight.results[4].result = "pass";
                            Trace_ua[item].data.insight.results[14].test = "location2_layer1_waveform";
                            Trace_ua[item].data.insight.results[14].units = "/";
                            Trace_ua[item].data.insight.results[14].value = UA_data.location1_layer1_waveform;
                            // Trace_ua[item].data.insight.results[5].result = "pass";
                            Trace_ua[item].data.insight.results[15].test = "location2_layer1_pulse_energy";
                            Trace_ua[item].data.insight.results[15].units = "J";
                            Trace_ua[item].data.insight.results[15].value = UA_data.location1_layer1_pulse_energy;
                            // Trace_ua[item].data.insight.results[6].result = "pass";
                            Trace_ua[item].data.insight.results[16].test = "location2_layer1_laser_speed";
                            Trace_ua[item].data.insight.results[16].units = "mm/s";
                            Trace_ua[item].data.insight.results[16].value = UA_data.location1_layer1_laser_speed;
                            //  Trace_ua[item].data.insight.results[7].result = "pass";
                            Trace_ua[item].data.insight.results[17].test = "location2_layer1_jump_speed";
                            Trace_ua[item].data.insight.results[17].units = "mm/s";
                            Trace_ua[item].data.insight.results[17].value = UA_data.location1_layer1_jump_speed;
                            //  Trace_ua[item].data.insight.results[8].result = "pass";
                            Trace_ua[item].data.insight.results[18].test = "location2_layer1_jump_delay";
                            Trace_ua[item].data.insight.results[18].units = "us";
                            Trace_ua[item].data.insight.results[18].value = UA_data.location1_layer1_jump_delay;
                            //  Trace_ua[item].data.insight.results[9].result = "pass";
                            Trace_ua[item].data.insight.results[19].test = "location2_layer1_scanner_delay";
                            Trace_ua[item].data.insight.results[19].units = "us";
                            Trace_ua[item].data.insight.results[19].value = UA_data.location1_layer1_scanner_delay;

                            // Trace_ua[item].data.insight.results[10].result = "pass";
                            Trace_ua[item].data.insight.results[20].test = "location3_layer1_laser_power";
                            Trace_ua[item].data.insight.results[20].units = "W";
                            Trace_ua[item].data.insight.results[20].value = UA_data.location2_layer1_laser_power;
                            //   Trace_ua[item].data.insight.results[11].result = "pass";
                            Trace_ua[item].data.insight.results[21].test = "location3_layer1_frequency";
                            Trace_ua[item].data.insight.results[21].units = "KHz";
                            Trace_ua[item].data.insight.results[21].value = UA_data.location2_layer1_frequency;
                            // Trace_ua[item].data.insight.results[12].result = "pass";
                            Trace_ua[item].data.insight.results[22].test = "location3_layer1_waveform";
                            Trace_ua[item].data.insight.results[22].units = "/";
                            Trace_ua[item].data.insight.results[22].value = UA_data.location2_layer1_waveform;
                            //Trace_ua[item].data.insight.results[13].result = "pass";
                            Trace_ua[item].data.insight.results[23].test = "location3_layer1_pulse_energy";
                            Trace_ua[item].data.insight.results[23].units = "J";
                            Trace_ua[item].data.insight.results[23].value = UA_data.location2_layer1_pulse_energy;
                            //Trace_ua[item].data.insight.results[14].result = "pass";
                            Trace_ua[item].data.insight.results[24].test = "location3_layer1_laser_speed";
                            Trace_ua[item].data.insight.results[24].units = "mm/s";
                            Trace_ua[item].data.insight.results[24].value = UA_data.location2_layer1_laser_speed;
                            // Trace_ua[item].data.insight.results[15].result = "pass";
                            Trace_ua[item].data.insight.results[25].test = "location3_layer1_jump_speed";
                            Trace_ua[item].data.insight.results[25].units = "mm/s";
                            Trace_ua[item].data.insight.results[25].value = UA_data.location2_layer1_jump_speed;
                            //  Trace_ua[item].data.insight.results[16].result = "pass";
                            Trace_ua[item].data.insight.results[26].test = "location3_layer1_jump_delay";
                            Trace_ua[item].data.insight.results[26].units = "us";
                            Trace_ua[item].data.insight.results[26].value = UA_data.location2_layer1_jump_delay;
                            //   Trace_ua[item].data.insight.results[17].result = "pass";
                            Trace_ua[item].data.insight.results[27].test = "location3_layer1_scanner_delay";
                            Trace_ua[item].data.insight.results[27].units = "us";
                            Trace_ua[item].data.insight.results[27].value = UA_data.location2_layer1_scanner_delay;

                            // Trace_ua[item].data.insight.results[10].result = "pass";
                            Trace_ua[item].data.insight.results[28].test = "location4_layer1_laser_power";
                            Trace_ua[item].data.insight.results[28].units = "W";
                            Trace_ua[item].data.insight.results[28].value = UA_data.location3_layer1_laser_power;
                            //   Trace_ua[item].data.insight.results[11].result = "pass";
                            Trace_ua[item].data.insight.results[29].test = "location4_layer1_frequency";
                            Trace_ua[item].data.insight.results[29].units = "KHz";
                            Trace_ua[item].data.insight.results[29].value = UA_data.location3_layer1_frequency;
                            // Trace_ua[item].data.insight.results[12].result = "pass";
                            Trace_ua[item].data.insight.results[30].test = "location4_layer1_waveform";
                            Trace_ua[item].data.insight.results[30].units = "/";
                            Trace_ua[item].data.insight.results[30].value = UA_data.location3_layer1_waveform;
                            //Trace_ua[item].data.insight.results[13].result = "pass";
                            Trace_ua[item].data.insight.results[31].test = "location4_layer1_pulse_energy";
                            Trace_ua[item].data.insight.results[31].units = "J";
                            Trace_ua[item].data.insight.results[31].value = UA_data.location3_layer1_pulse_energy;
                            //Trace_ua[item].data.insight.results[14].result = "pass";
                            Trace_ua[item].data.insight.results[32].test = "location4_layer1_laser_speed";
                            Trace_ua[item].data.insight.results[32].units = "mm/s";
                            Trace_ua[item].data.insight.results[32].value = UA_data.location3_layer1_laser_speed;
                            // Trace_ua[item].data.insight.results[15].result = "pass";
                            Trace_ua[item].data.insight.results[33].test = "location4_layer1_jump_speed";
                            Trace_ua[item].data.insight.results[33].units = "mm/s";
                            Trace_ua[item].data.insight.results[33].value = UA_data.location3_layer1_jump_speed;
                            //  Trace_ua[item].data.insight.results[16].result = "pass";
                            Trace_ua[item].data.insight.results[34].test = "location4_layer1_jump_delay";
                            Trace_ua[item].data.insight.results[34].units = "us";
                            Trace_ua[item].data.insight.results[34].value = UA_data.location3_layer1_jump_delay;
                            //   Trace_ua[item].data.insight.results[17].result = "pass";
                            Trace_ua[item].data.insight.results[35].test = "location4_layer1_scanner_delay";
                            Trace_ua[item].data.insight.results[35].units = "us";
                            Trace_ua[item].data.insight.results[35].value = UA_data.location3_layer1_scanner_delay;

                            Trace_ua[item].data.insight.results[36].result = "pass";
                            Trace_ua[item].data.insight.results[36].test = "laser_sensor";
                            Trace_ua[item].data.insight.results[36].units = "";
                            Trace_ua[item].data.insight.results[36].value = UA_data.laser_sensor;

                            Trace_ua[item].data.insight.test_station_attributes.line_id = Global.inidata.productconfig.Line_id_la;
                            Trace_ua[item].data.insight.test_station_attributes.station_id = Global.inidata.productconfig.Station_id_la;
                            Trace_ua[item].data.insight.test_station_attributes.head_id = Global.PLC_Client.ReadPLC_D(258, 1)[0].ToString() == "1" ? "1" : "2"; 
                            Trace_ua[item].data.insight.test_station_attributes.software_name = Global.inidata.productconfig.Sw_name_ua;
                            Trace_ua[item].data.insight.test_station_attributes.software_version = Global.inidata.productconfig.Sw_version;

                            string SelectStr = "SELECT * FROM OEE_TraceDT";//sql查询语句
                            DataTable d1 = SQL.ExecuteQuery(SelectStr);
                            if (d1 != null && d1.Rows.Count > 0)
                            {
                                for (int i = 0; i < d1.Rows.Count; i++)
                                {
                                    (Trace_ua[item].data.items as ICollection<KeyValuePair<string, object>>).Add(new KeyValuePair<string, object>("error_" + (i + 1), d1.Rows[i][3].ToString() + "_" + (Convert.ToDateTime(d1.Rows[i][4])).ToString("yyyy-MM-dd HH:mm:ss")));
                                }
                            }
                            string DeleteStr = string.Format("delete from [ZHH].[dbo].[OEE_TraceDT] where ID  in(select top {0} ID  from  [ZHH].[dbo].[OEE_TraceDT] order  by  ID  asc)", d1.Rows.Count);
                            SQL.ExecuteUpdate(DeleteStr);
                            string SendTraceLogs = JsonConvert.SerializeObject(Trace_ua[item], Formatting.None, jsetting);
                            Log.WriteLog("Trace_U_Bracket_Data:" + SendTraceLogs + ",TraceLog");
                            TraceRespondID Msg = null;
                            _homefrm.AppendRichText("SendTraceLogs_U_Bracket:" + SendTraceLogs, "rtx_TraceMsg");
                            
                            try
                            {
                                Stopwatch sw = new Stopwatch();
                                sw.Start();
                                var Trcae_logs_result = RequestAPI2.Trcae_logs(Global.inidata.productconfig.Trace_Logs_LA, SendTraceLogs, out Trcae_logs_str, out Msg);
                                sw.Stop();
                                Log.WriteLog("U_Bracket上传时间:" + sw.ElapsedMilliseconds.ToString() + ",TraceLog");
                                if (Trcae_logs_result)
                                {
                                    //string DeleteStr = string.Format("delete from [ZHH].[dbo].[OEE_TraceDT] where ID  in(select top {0} ID  from  [ZHH].[dbo].[OEE_TraceDT] order  by  ID  asc)", d1.Rows.Count);
                                    //SQL.ExecuteUpdate(DeleteStr);
                                    if (Trace_ua[item].data.insight.test_attributes.test_result == "pass")
                                    {
                                        Global.Trace_ua_ok++;
                                        Global.TraceSendua_ng = 0;
                                        _homefrm.UpdateDataGridView_FixtureOK(Trace_ua[item].data.insight.test_station_attributes.fixture_id);
                                        _homefrm.UpDatalabel(Global.Trace_ua_ok.ToString(), "lb_TraceUAOK");
                                    }
                                    else
                                    {
                                        Global.Trace_ua_ng++;
                                        Global.TraceSendua_ng = 0;
                                        string InsertStr = "insert into FixtureNG([DateTime],[Fixture])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "'" + "," + "'" + Trace_ua[item].data.insight.test_station_attributes.fixture_id + "'" + ")";
                                        SQL.ExecuteUpdate(InsertStr);//插入NG治具
                                        if (DateTime.Now.ToString("yyyy-MM-dd") == Global.SelectFixturetime.ToString("yyyy-MM-dd"))
                                        {
                                            _homefrm.AddList(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "  [治具码:" + Trace_ua[item].data.insight.test_station_attributes.fixture_id + ",NG]", "list_FixtureNG");
                                        }
                                        _homefrm.UpdateDataGridView_FixtureNG(Trace_ua[item].data.insight.test_station_attributes.fixture_id);
                                        _homefrm.UpDatalabel(Global.Trace_ua_ng.ToString(), "lb_TraceUANG");
                                        string JsonBody = SendTraceLogs.Replace(',', ';');
                                        Log.WriteCSV_DiscardLog("Product" + "," + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + "" + "," + Global.inidata.productconfig.OEE_Dsn + "," + item + "," + JsonBody + "," + "U_Bracket焊接NG" + "," + "1");
                                    }
                                    Trace_Logs_flag = true;
                                    Global.PLC_Client2.WritePLC_D(10301, new short[] { 1 });
                                    _homefrm.UpDatalabelcolor(Color.Green, "Trace_U_Bracket发送成功", "lb_Trace_UA_SendStatus");
                                    _homefrm.AppendRichText(Trcae_logs_str + JsonConvert.SerializeObject(Msg), "rtx_TraceMsg");
                                    Log.WriteLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + "U_Bracket" + ","
                                        + Trace_ua[item].data.insight.uut_attributes.full_sn + ","
                                        + Trace_ua[item].data.insight.test_station_attributes.fixture_id + ","
                                        + Trace_ua[item].data.insight.test_attributes.uut_start + ","
                                        + Trace_ua[item].data.insight.uut_attributes.laser_start_time + ","
                                        + Trace_ua[item].data.insight.uut_attributes.laser_stop_time + ","
                                        + Trace_ua[item].data.insight.test_attributes.uut_stop + ","
                                        + Trace_ua[item].data.insight.uut_attributes.tossing_item + ","
                                        + "OK-success" + ","
                                        + Trace_ua[item].data.insight.test_attributes.test_result, System.AppDomain.CurrentDomain.BaseDirectory + "\\Trace_Logs_U_Bracket数据\\");
                                }
                                else
                                {
                                    Global.Trace_ua_ng++;
                                    Global.TraceSendua_ng++;
                                    _homefrm.UpDatalabel(Global.Trace_ua_ng.ToString(), "lb_TraceNG");
                                    if (Global.SelectFirst == false)
                                    {
                                        if (Convert.ToDateTime(DateTime.Now.ToString("HH:mm")).CompareTo(Convert.ToDateTime("6:00")) >= 0 && Convert.ToDateTime(DateTime.Now.ToString("HH:mm")).CompareTo(Convert.ToDateTime("18:00")) < 0)
                                        {
                                            Global.TraceUpLoad_Error_D++;//白班Trace上传异常计数累加
                                            Global.TraceTab_Error_D++;
                                            _datastatisticsfrm.UpDataDGV_D(10, 1, Global.TraceUpLoad_Error_D.ToString());
                                            Global.inidata.productconfig.TraceUpLoad_Error_D = Global.TraceUpLoad_Error_D.ToString();
                                            // Global.inidata.WriteProductnumSection();
                                        }
                                        else
                                        {
                                            Global.TraceUpLoad_Error_N++;//夜班Trace上传异常计数累加
                                            Global.TraceTab_Error_N++;
                                            _datastatisticsfrm.UpDataDGV_N(10, 1, Global.TraceUpLoad_Error_N.ToString());
                                            Global.inidata.productconfig.TraceUpLoad_Error_N = Global.TraceUpLoad_Error_N.ToString();
                                            // Global.inidata.WriteProductnumSection();
                                        }
                                    }
                                    Trace_Logs_flag = false;
                                    Global.PLC_Client2.WritePLC_D(10301, new short[] { 2 });
                                    _homefrm.UpDatalabelcolor(Color.Red, "Trace_U_Bracket发送失败", "lb_Trace_UA_SendStatus");
                                    _homefrm.AppendRichText(Trcae_logs_str + JsonConvert.SerializeObject(Msg), "rtx_TraceMsg");
                                    string insStr = string.Format("insert into Trace_UA_SendNG([DateTime],[sp],[band],[test_result],[fixture_id],[tossing_item],"
                                             + "[STATION_STRING],[uut_start],[Weld_start_time],[Weld_stop_time],[uut_stop]"
                                             + ") values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}'"
                                             + ")", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), Trace_ua[item].serials.sp, Trace_ua[item].data.insight.uut_attributes.full_sn, Trace_ua[item].data.insight.test_attributes.test_result,
                                             Trace_ua[item].data.insight.test_station_attributes.fixture_id, Trace_ua[item].data.insight.uut_attributes.tossing_item, Trace_ua[item].data.insight.uut_attributes.STATION_STRING,
                                             Trace_ua[item].data.insight.test_attributes.uut_start, Trace_ua[item].data.insight.uut_attributes.laser_start_time, Trace_ua[item].data.insight.uut_attributes.laser_stop_time,
                                             Trace_ua[item].data.insight.test_attributes.uut_stop);
                                    int r = SQL.ExecuteUpdate(insStr);
                                    Log.WriteLog(string.Format("插入了{0}行Trace_UA_SendNG数据", r) + ",TraceLog");
                                    Log.WriteLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + "U_Bracket" + ","
                                        + Trace_ua[item].data.insight.uut_attributes.full_sn + ","
                                        + Trace_ua[item].data.insight.test_station_attributes.fixture_id + ","
                                        + Trace_ua[item].data.insight.test_attributes.uut_start + ","
                                        + Trace_ua[item].data.insight.uut_attributes.laser_start_time + ","
                                        + Trace_ua[item].data.insight.uut_attributes.laser_stop_time + ","
                                        + Trace_ua[item].data.insight.test_attributes.uut_stop + ","
                                        + Trace_ua[item].data.insight.uut_attributes.tossing_item + ","
                                        + "NG-Trace发送失败" + ","
                                        + Trace_ua[item].data.insight.test_attributes.test_result, System.AppDomain.CurrentDomain.BaseDirectory + "\\Trace_Logs_U_Bracket_NG数据\\");
                                    string JsonBody = SendTraceLogs.Replace(',', ';');
                                    Log.WriteCSV_DiscardLog("Trace" + "," + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + "Trace_U_Bracket" + "," + Global.inidata.productconfig.OEE_Dsn + "," + item + "," + JsonBody + "," + Trcae_logs_str + "," + "1");
                                }
                                Log.WriteLog(Trcae_logs_str + ",TraceLog");
                                Log.WriteLog(item + "上传结果：" + JsonConvert.SerializeObject(Msg) + ",TraceLog");
                                _homefrm.UiText(Trace_ua[item].data.insight.uut_attributes.full_sn, "txt_Trace_band_ua");
                                _homefrm.UiText(Trace_ua[item].data.insight.test_attributes.test_result, "txt_Trace_test_result_ua");
                                _homefrm.UiText(Trace_ua[item].data.insight.test_attributes.unit_serial_number, "txt_Trace_unit_serial_number_ua");
                                _homefrm.UiText(Trace_ua[item].data.insight.test_station_attributes.fixture_id, "txt_Trace_fixture_id_ua");
                                _homefrm.UiText(Trace_ua[item].data.insight.test_attributes.uut_start, "txt_Trace_uut_start_ua");
                                _homefrm.UiText(Trace_ua[item].data.insight.test_attributes.uut_stop, "txt_Trace_uut_stop_ua");
                                _homefrm.UiText(Trace_ua[item].data.insight.uut_attributes.laser_start_time, "txt_Trace_weld_start_time_ua");
                                _homefrm.UiText(Trace_ua[item].data.insight.uut_attributes.laser_stop_time, "txt_Trace_weld_stop_time_ua");
                                _homefrm.UiText(Trace_ua[item].data.insight.results[0].value, "txt_Trace_weld_wait_ct_ua");
                                _homefrm.UiText(Trace_ua[item].data.insight.results[1].value, "txt_Trace_actual_weld_ct_ua");
                                Out_Trace_ua.RemoveAt(0);
                                Trace_ua.Remove(item);
                            }
                            catch (Exception ex)
                            {
                                try
                                {
                                    Global.Trace_ua_ng++;
                                    Global.TraceSendua_ng++;
                                    _homefrm.UpDatalabelcolor(Color.Red, Global.Trace_ua_ng.ToString(), "lb_Trace_UA_SendStatus");
                                    if (Global.SelectFirst == false)
                                    {
                                        if (Convert.ToDateTime(DateTime.Now.ToString("HH:mm")).CompareTo(Convert.ToDateTime("6:00")) >= 0 && Convert.ToDateTime(DateTime.Now.ToString("HH:mm")).CompareTo(Convert.ToDateTime("18:00")) < 0)
                                        {
                                            Global.TraceUpLoad_Error_D++;//白班Trace上传异常计数累加
                                            Global.TraceTab_Error_D++;
                                            _datastatisticsfrm.UpDataDGV_D(10, 1, Global.TraceUpLoad_Error_D.ToString());
                                            Global.inidata.productconfig.TraceUpLoad_Error_D = Global.TraceUpLoad_Error_D.ToString();
                                            //Global.inidata.WriteProductnumSection();
                                        }
                                        else
                                        {
                                            Global.TraceUpLoad_Error_N++;//夜班Trace上传异常计数累加
                                            Global.TraceTab_Error_N++;
                                            _datastatisticsfrm.UpDataDGV_N(10, 1, Global.TraceUpLoad_Error_N.ToString());
                                            Global.inidata.productconfig.TraceUpLoad_Error_N = Global.TraceUpLoad_Error_N.ToString();
                                            //Global.inidata.WriteProductnumSection();
                                        }
                                    }
                                    ShowStatus("与Trace断开连接", Color.Red, 2);
                                    Trace_Logs_flag = false;
                                    Global.PLC_Client2.WritePLC_D(10301, new short[] { 2 });
                                    Log.WriteLog(item + "Trace_Logs_U_Bracket发送异常" + ex.ToString().Replace("\r\n", "") + ",TraceLog");
                                    Log.WriteLog(item + "Trace_Logs_U_Bracket发送异常" + ex.ToString().Replace("\r\n", "") + ",TraceLog");
                                    string insStr = string.Format("insert into Trace_UA_SendNG([DateTime],[sp],[band],[test_result],[fixture_id],[tossing_item],"
                                           + "[STATION_STRING],[uut_start],[Weld_start_time],[Weld_stop_time],[uut_stop]"
                                           + ") values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}'"
                                           + ")", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), Trace_ua[item].serials.sp, Trace_ua[item].data.insight.uut_attributes.full_sn, Trace_ua[item].data.insight.test_attributes.test_result,
                                           Trace_ua[item].data.insight.test_station_attributes.fixture_id, Trace_ua[item].data.insight.uut_attributes.tossing_item, Trace_ua[item].data.insight.uut_attributes.STATION_STRING,
                                           Trace_ua[item].data.insight.test_attributes.uut_start, Trace_ua[item].data.insight.uut_attributes.laser_start_time, Trace_ua[item].data.insight.uut_attributes.laser_stop_time,
                                           Trace_ua[item].data.insight.test_attributes.uut_stop);
                                    int r = SQL.ExecuteUpdate(insStr);
                                    Log.WriteLog(string.Format("插入了{0}行Trace_ua_SendNG数据", r) + ",TraceLog");
                                    Log.WriteLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + "U_Bracket" + ","
                                            + Trace_ua[item].data.insight.uut_attributes.full_sn + ","
                                            + Trace_ua[item].data.insight.test_station_attributes.fixture_id + ","
                                            + Trace_ua[item].data.insight.test_attributes.uut_start + ","
                                            + Trace_ua[item].data.insight.uut_attributes.laser_start_time + ","
                                            + Trace_ua[item].data.insight.uut_attributes.laser_stop_time + ","
                                            + Trace_ua[item].data.insight.test_attributes.uut_stop + ","
                                            + Trace_ua[item].data.insight.uut_attributes.tossing_item + ","
                                            + "NG-Trace网络异常" + ","
                                            + Trace_ua[item].data.insight.test_attributes.test_result, System.AppDomain.CurrentDomain.BaseDirectory + "\\Trace_Logs_U_Bracket_NG数据\\");
                                    Out_Trace_ua.RemoveAt(0);
                                    Trace_ua.Remove(item);
                                    string JsonBody = SendTraceLogs.Replace(',', ';');
                                    Log.WriteCSV_DiscardLog("Trace" + "," + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + "Trace_U_Bracket" + "," + Global.inidata.productconfig.OEE_Dsn + "," + item + "," + JsonBody + "," + Trcae_logs_str + "," + "1");
                                }
                                catch (Exception ex2)
                                {
                                    Out_Trace_ua.RemoveAt(0);
                                    Trace_ua.Remove(item);
                                    MessageBox.Show("Trace_U_Bracket上传异常，请找工程师排查!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    Log.WriteLog("Trace_U-Bracket上传异常!" + ex2.ToString() + ",TraceLog");
                                }
                            }
                        }
                        else
                        {
                            Out_Trace_ua.RemoveAt(0);
                            Trace_ua.Remove(item);
                            Log.WriteLog("Trace_U_Bracket上传SN格式不正确:" + item + ",TraceLog");
                            Global.PLC_Client2.WritePLC_D(10301, new short[] { 2 });
                            _homefrm.AppendRichText("Trace_U_Bracket上传SN格式不正确:" + item, "rtx_TraceMsg");
                        }
                        #endregion
                    }
                }
                Thread.Sleep(5);
            }
        }


        private void EthTraceUA2()
        {
            while (true)
            {
                if (Trace_ua2.Count > 0)
                {
                    if (Out_Trace_ua2.Count > 0)
                    {
                        string item = Out_Trace_ua2[0];
                        #region
                        HansData_U_Bracket UA_data = new HansData_U_Bracket();
                        GetAllHansData(item, out UA_data, 1);
                        if (item.Contains("FM") && !Regex.IsMatch(item, "[a-z]"))
                        {
                            JsonSerializerSettings jsetting = new JsonSerializerSettings();
                            jsetting.NullValueHandling = NullValueHandling.Ignore;//Json不输出空值
                            string Trcae_logs_str = null;
                            TimeSpan ts1, ts2;
                            if (Trace_ua2[item].data.insight.test_attributes.test_result == "pass" && UA_data.HanDataUAResult)
                            {
                                Trace_ua2[item].data.insight.test_attributes.test_result = "pass";
                            }
                            else if (Trace_ua2[item].data.insight.test_attributes.test_result == "fail" && !UA_data.HanDataUAResult)
                            {
                                Trace_ua2[item].data.insight.test_attributes.test_result = "fail";
                                if (Trace_ua2[item].data.insight.uut_attributes.tossing_item == "")
                                {
                                    Trace_ua2[item].data.insight.uut_attributes.tossing_item += "Welding data acquire NG";
                                }
                                else
                                { Trace_ua2[item].data.insight.uut_attributes.tossing_item += "/Welding data acquire NG"; }

                                if (Global.SelectFirst != true)
                                {
                                    if (Convert.ToDateTime(DateTime.Now.ToString("HH:mm")).CompareTo(Convert.ToDateTime("6:00")) >= 0 && Convert.ToDateTime(DateTime.Now.ToString("HH:mm")).CompareTo(Convert.ToDateTime("18:00")) < 0)
                                    {
                                        Global.HansDataError_D++;//白班焊接NG计数累加                                                                 
                                        _datastatisticsfrm.UpDataDGV_D(7, 1, Global.HansDataError_D.ToString());
                                        Global.inidata.productconfig.HansDataError_D = Global.HansDataError_D.ToString();
                                    }
                                    else
                                    {
                                        Global.HansDataError_N++;//夜班焊接NG计数累加
                                        _datastatisticsfrm.UpDataDGV_N(7, 1, Global.HansDataError_N.ToString());
                                        Global.inidata.productconfig.HansDataError_N = Global.HansDataError_N.ToString();
                                    }
                                }
                            }
                            else if (Trace_ua2[item].data.insight.test_attributes.test_result == "fail" && UA_data.HanDataUAResult)
                            {
                                Trace_ua2[item].data.insight.test_attributes.test_result = "fail";

                            }
                            else if (Trace_ua2[item].data.insight.test_attributes.test_result == "pass" && !UA_data.HanDataUAResult)
                            {
                                Trace_ua2[item].data.insight.test_attributes.test_result = "fail";
                                Trace_ua2[item].data.insight.uut_attributes.tossing_item = "Welding data acquire NG";
                                if (Global.SelectFirst != true)
                                {
                                    if (Convert.ToDateTime(DateTime.Now.ToString("HH:mm")).CompareTo(Convert.ToDateTime("6:00")) >= 0 && Convert.ToDateTime(DateTime.Now.ToString("HH:mm")).CompareTo(Convert.ToDateTime("18:00")) < 0)
                                    {
                                        Global.HansDataError_D++;//白班焊接NG计数累加                                                                 
                                        _datastatisticsfrm.UpDataDGV_D(7, 1, Global.HansDataError_D.ToString());
                                        Global.inidata.productconfig.HansDataError_D = Global.HansDataError_D.ToString();
                                    }
                                    else
                                    {
                                        Global.HansDataError_N++;//夜班焊接NG计数累加
                                        _datastatisticsfrm.UpDataDGV_N(7, 1, Global.HansDataError_N.ToString());
                                        Global.inidata.productconfig.HansDataError_N = Global.HansDataError_N.ToString();
                                    }
                                }
                            }
                            Trace_ua2[item].data.insight.uut_attributes.precitec_grading = "0";
                            Trace_ua2[item].data.insight.uut_attributes.precitec_rev = "0";
                            Trace_ua2[item].data.insight.uut_attributes.hatch = UA_data.hatch;
                            Trace_ua2[item].data.insight.uut_attributes.precitec_value = "0";
                            Trace_ua2[item].data.insight.uut_attributes.pattern_type = UA_data.pattern_type;
                            Trace_ua2[item].data.insight.uut_attributes.spot_size = UA_data.spot_size;
                            Trace_ua2[item].data.insight.uut_attributes.swing_amplitude = UA_data.swing_amplitude;
                            Trace_ua2[item].data.insight.uut_attributes.swing_freq = UA_data.swing_freq;

                            //Trace_ua2[item].data.insight.results[0].result = "pass";
                            Trace_ua2[item].data.insight.results[0].test = "location1_Trenh bracke";
                            Trace_ua2[item].data.insight.results[0].units = "/";
                            Trace_ua2[item].data.insight.results[0].value = "0";

                            //Trace_ua2[item].data.insight.results[1].result = "pass";
                            Trace_ua2[item].data.insight.results[1].test = "location1_layer1_laser_power";
                            Trace_ua2[item].data.insight.results[1].units = "W";
                            Trace_ua2[item].data.insight.results[1].value = UA_data.location4_layer1_laser_power;
                            //   Trace_ua2[item].data.insight.results[2].result = "pass";
                            Trace_ua2[item].data.insight.results[2].test = "location1_layer1_frequency";
                            Trace_ua2[item].data.insight.results[2].units = "KHz";
                            Trace_ua2[item].data.insight.results[2].value = UA_data.location4_layer1_frequency;
                            //  Trace_ua2[item].data.insight.results[3].result = "pass";
                            Trace_ua2[item].data.insight.results[3].test = "location1_layer1_waveform";
                            Trace_ua2[item].data.insight.results[3].units = "/";
                            Trace_ua2[item].data.insight.results[3].value = UA_data.location4_layer1_waveform;
                            //  Trace_ua2[item].data.insight.results[4].result = "pass";
                            Trace_ua2[item].data.insight.results[4].test = "location1_layer1_pulse_energy";
                            Trace_ua2[item].data.insight.results[4].units = "J";
                            Trace_ua2[item].data.insight.results[4].value = UA_data.location4_layer1_pulse_energy;
                            //  Trace_ua2[item].data.insight.results[5].result = "pass";
                            Trace_ua2[item].data.insight.results[5].test = "location1_layer1_laser_speed";
                            Trace_ua2[item].data.insight.results[5].units = "mm/s";
                            Trace_ua2[item].data.insight.results[5].value = UA_data.location4_layer1_laser_speed;
                            //  Trace_ua2[item].data.insight.results[6].result = "pass";
                            Trace_ua2[item].data.insight.results[6].test = "location1_layer1_jump_speed";
                            Trace_ua2[item].data.insight.results[6].units = "mm/s";
                            Trace_ua2[item].data.insight.results[6].value = UA_data.location4_layer1_jump_speed;
                            //   Trace_ua2[item].data.insight.results[7].result = "pass";
                            Trace_ua2[item].data.insight.results[7].test = "location1_layer1_jump_delay";
                            Trace_ua2[item].data.insight.results[7].units = "us";
                            Trace_ua2[item].data.insight.results[7].value = UA_data.location4_layer1_jump_delay;
                            //   Trace_ua2[item].data.insight.results[8].result = "pass";
                            Trace_ua2[item].data.insight.results[8].test = "location1_layer1_scanner_delay";
                            Trace_ua2[item].data.insight.results[8].units = "us";
                            Trace_ua2[item].data.insight.results[8].value = UA_data.location4_layer1_scanner_delay;


                            Trace_ua2[item].data.insight.results[9].result = "pass";
                            Trace_ua2[item].data.insight.results[9].test = "laser_sensor";
                            Trace_ua2[item].data.insight.results[9].units = "";
                            Trace_ua2[item].data.insight.results[9].value = UA_data.laser_sensor;

                            Trace_ua2[item].data.insight.test_station_attributes.line_id = Global.inidata.productconfig.Line_id_ua;
                            Trace_ua2[item].data.insight.test_station_attributes.station_id = Global.inidata.productconfig.Station_id_ua;//STATION1635
                            Trace_ua2[item].data.insight.test_station_attributes.head_id = Global.PLC_Client.ReadPLC_D(258, 1)[0].ToString() == "1" ? "1" : "2";
                            Trace_ua2[item].data.insight.test_station_attributes.software_name = Global.inidata.productconfig.Sw_name_ua;
                            Trace_ua2[item].data.insight.test_station_attributes.software_version = Global.inidata.productconfig.Sw_version;
                            string SelectStr = "SELECT * FROM OEE_TraceDT";//sql查询语句
                            DataTable d1 = SQL.ExecuteQuery(SelectStr);
                            if (d1 != null && d1.Rows.Count > 0)
                            {
                                for (int i = 0; i < d1.Rows.Count; i++)
                                {
                                    (Trace_ua2[item].data.items as ICollection<KeyValuePair<string, object>>).Add(new KeyValuePair<string, object>("error_" + (i + 1), d1.Rows[i][3].ToString() + "_" + (Convert.ToDateTime(d1.Rows[i][4])).ToString("yyyy-MM-dd HH:mm:ss")));
                                }
                            }

                            string SendTraceLogs = JsonConvert.SerializeObject(Trace_ua2[item], Formatting.None, jsetting);
                            Log.WriteLog("Trace_Trench_Bracket_Data:" + SendTraceLogs + ",TraceLog");
                            TraceRespondID Msg = null;
                            _homefrm.AppendRichText("SendTraceLogs_Trench_Bracket:" + SendTraceLogs, "rtx_TraceMsg");
                            try
                            {
                                Stopwatch sw = new Stopwatch();
                                sw.Start();
                                var Trcae_logs_result = RequestAPI2.Trcae_logs(Global.inidata.productconfig.Trace_Logs_UA, SendTraceLogs, out Trcae_logs_str, out Msg);
                                sw.Stop();
                                Log.WriteLog("Trench_Bracket上传时间:" + sw.ElapsedMilliseconds.ToString() + ",TraceLog");
                                if (Trcae_logs_result)
                                {
                                    string DeleteStr = string.Format("delete from [ZHH].[dbo].[OEE_TraceDT] where ID  in(select top {0} ID  from  [ZHH].[dbo].[OEE_TraceDT] order  by  ID  asc)", d1.Rows.Count);
                                    SQL.ExecuteUpdate(DeleteStr);

                                    if (Trace_ua2[item].data.insight.test_attributes.test_result == "pass")
                                    {
                                        Global.Trace_ua2_ok++;
                                        Global.TraceSendua_ng = 0;
                                        _homefrm.UpdateDataGridView_FixtureOK(Trace_ua2[item].data.insight.test_station_attributes.fixture_id);
                                        _homefrm.UpDatalabel(Global.Trace_ua_ok.ToString(), "lb_TraceLAOK");
                                    }
                                    else
                                    {
                                        Global.Trace_ua2_ng++;
                                        Global.TraceSendua_ng = 0;
                                        string InsertStr = "insert into FixtureNG([DateTime],[Fixture])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "'" + "," + "'" + Trace_ua2[item].data.insight.test_station_attributes.fixture_id + "'" + ")";
                                        SQL.ExecuteUpdate(InsertStr);//插入NG治具
                                        if (DateTime.Now.ToString("yyyy-MM-dd") == Global.SelectFixturetime.ToString("yyyy-MM-dd"))
                                        {
                                            _homefrm.AddList(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "  [治具码:" + Trace_ua2[item].data.insight.test_station_attributes.fixture_id + ",NG]", "list_FixtureNG");
                                        }
                                        _homefrm.UpdateDataGridView_FixtureNG(Trace_ua2[item].data.insight.test_station_attributes.fixture_id);
                                        _homefrm.UpDatalabel(Global.Trace_ua2_ng.ToString(), "lb_TraceLANG");
                                        string JsonBody = SendTraceLogs.Replace(',', ';');
                                        Log.WriteCSV_DiscardLog("Product" + "," + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + "" + "," + Global.inidata.productconfig.OEE_Dsn + "," + item + "," + JsonBody + "," + "Trench_Bracket焊接NG" + "," + "1");
                                    }
                                    Trace_Logs_flag = true;
                                    Global.PLC_Client.WritePLC_D(10305, new short[] { 1 });
                                    _homefrm.UpDatalabelcolor(Color.Green, "Trace_Trench_Bracket发送成功", "lb_Trace_LA_SendStatus");
                                    _homefrm.AppendRichText(Trcae_logs_str + JsonConvert.SerializeObject(Msg), "rtx_TraceMsg");
                                    Log.WriteLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + "Bracket_Trench" + ","
                                        + Trace_ua2[item].data.insight.uut_attributes.full_sn + ","
                                        + Trace_ua2[item].data.insight.test_station_attributes.fixture_id + ","
                                        + Trace_ua2[item].data.insight.test_attributes.uut_start + ","
                                        + Trace_ua2[item].data.insight.uut_attributes.laser_start_time + ","
                                        + Trace_ua2[item].data.insight.uut_attributes.laser_stop_time + ","
                                        + Trace_ua2[item].data.insight.test_attributes.uut_stop + ","
                                        + Trace_ua2[item].data.insight.uut_attributes.tossing_item + ","
                                        + "OK-success" + ","
                                        + Trace_ua2[item].data.insight.test_attributes.test_result, System.AppDomain.CurrentDomain.BaseDirectory + "\\Trace_Logs_U_Bracket数据\\");
                                }
                                else
                                {
                                    Global.Trace_ua2_ng++;
                                    Global.TraceSendua_ng++;
                                    _homefrm.UpDatalabel(Global.Trace_ua2_ng.ToString(), "lb_TraceLANG");
                                    if (Global.SelectFirst != true)
                                    {
                                        if (Convert.ToDateTime(DateTime.Now.ToString("HH:mm")).CompareTo(Convert.ToDateTime("6:00")) >= 0 && Convert.ToDateTime(DateTime.Now.ToString("HH:mm")).CompareTo(Convert.ToDateTime("18:00")) < 0)
                                        {
                                            Global.TraceUpLoad_Error_D++;//白班Trace上传异常计数累加
                                            Global.TraceThench_Error_D++;
                                            _datastatisticsfrm.UpDataDGV_D(10, 1, Global.TraceUpLoad_Error_D.ToString());
                                            Global.inidata.productconfig.TraceUpLoad_Error_D = Global.TraceUpLoad_Error_D.ToString();
                                        }
                                        else
                                        {
                                            Global.TraceUpLoad_Error_N++;//夜班Trace上传异常计数累加
                                            Global.TraceThench_Error_N++;
                                            _datastatisticsfrm.UpDataDGV_N(10, 1, Global.TraceUpLoad_Error_N.ToString());
                                            Global.inidata.productconfig.TraceUpLoad_Error_N = Global.TraceUpLoad_Error_N.ToString();
                                        }
                                    }
                                    Trace_Logs_flag = false;
                                    Global.PLC_Client.WritePLC_D(10305, new short[] { 2 });
                                    _homefrm.UpDatalabelcolor(Color.Red, "Trace_U_Bracket发送失败", "lb_Trace_LA_SendStatus");
                                    _homefrm.AppendRichText(Trcae_logs_str + JsonConvert.SerializeObject(Msg), "rtx_TraceMsg");
                                    string insStr = string.Format("insert into Trace_LA_SendNG([DateTime],[sp],[band],[test_result],[fixture_id],[tossing_item],"
                                             + "[STATION_STRING],[uut_start],[Weld_start_time],[Weld_stop_time],[uut_stop]"
                                             + ") values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}'"
                                             + ")", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), Trace_ua2[item].serials.sp, Trace_ua2[item].data.insight.uut_attributes.full_sn, Trace_ua2[item].data.insight.test_attributes.test_result,
                                             Trace_ua2[item].data.insight.test_station_attributes.fixture_id, Trace_ua2[item].data.insight.uut_attributes.tossing_item, Trace_ua2[item].data.insight.uut_attributes.STATION_STRING,
                                             Trace_ua2[item].data.insight.test_attributes.uut_start, Trace_ua2[item].data.insight.uut_attributes.laser_start_time, Trace_ua2[item].data.insight.uut_attributes.laser_stop_time,
                                             Trace_ua2[item].data.insight.test_attributes.uut_stop);
                                    int r = SQL.ExecuteUpdate(insStr);
                                    Log.WriteLog(string.Format("插入了{0}行Trace_Trench_Bracket_SendNG数据", r) + ",TraceLog");
                                    Log.WriteLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + "Trench_Bracket" + ","
                                        + Trace_ua2[item].data.insight.uut_attributes.full_sn + ","
                                        + Trace_ua2[item].data.insight.test_station_attributes.fixture_id + ","
                                        + Trace_ua2[item].data.insight.test_attributes.uut_start + ","
                                        + Trace_ua2[item].data.insight.uut_attributes.laser_start_time + ","
                                        + Trace_ua2[item].data.insight.uut_attributes.laser_stop_time + ","
                                        + Trace_ua2[item].data.insight.test_attributes.uut_stop + ","
                                        + Trace_ua2[item].data.insight.uut_attributes.tossing_item + ","
                                        + "NG-Trace发送失败" + ","
                                        + Trace_ua2[item].data.insight.test_attributes.test_result, System.AppDomain.CurrentDomain.BaseDirectory + "\\Trace_Logs_U_Bracket_NG数据\\");
                                    string JsonBody = SendTraceLogs.Replace(',', ';');
                                    Log.WriteCSV_DiscardLog("Trace" + "," + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + "Trace_Trench_Bracket" + "," + Global.inidata.productconfig.OEE_Dsn + "," + item + "," + JsonBody + "," + Trcae_logs_str + "," + "1");
                                }
                                Log.WriteLog(Trcae_logs_str + ",TraceLog");
                                Log.WriteLog(item + "上传结果：" + JsonConvert.SerializeObject(Msg) + ",TraceLog");
                                _homefrm.UiText(Trace_ua2[item].data.insight.uut_attributes.full_sn, "txt_Trace_band_la");
                                _homefrm.UiText(Trace_ua2[item].data.insight.test_attributes.test_result, "txt_Trace_test_result_la");
                                _homefrm.UiText(Trace_ua2[item].data.insight.test_attributes.unit_serial_number, "txt_Trace_unit_serial_number_la");
                                _homefrm.UiText(Trace_ua2[item].data.insight.test_station_attributes.fixture_id, "txt_Trace_fixture_id_la");
                                _homefrm.UiText(Trace_ua2[item].data.insight.test_attributes.uut_start, "txt_Trace_uut_start_la");
                                _homefrm.UiText(Trace_ua2[item].data.insight.test_attributes.uut_stop, "txt_Trace_uut_stop_la");
                                _homefrm.UiText(Trace_ua2[item].data.insight.uut_attributes.laser_start_time, "txt_Trace_weld_start_time_la");
                                _homefrm.UiText(Trace_ua2[item].data.insight.uut_attributes.laser_stop_time, "txt_Trace_weld_stop_time_la");
                                _homefrm.UiText("NA", "txt_Trace_weld_wait_ct_la");
                                _homefrm.UiText("NA", "txt_Trace_actual_weld_ct_la");
                                Out_Trace_ua2.RemoveAt(0);
                                Trace_ua2.Remove(item);
                            }
                            catch (Exception ex)
                            {
                                try
                                {
                                    Global.Trace_ua2_ng++;
                                    Global.TraceSendua_ng++;
                                    _homefrm.UpDatalabelcolor(Color.Red, Global.Trace_ua2_ng.ToString(), "lb_Trace_LA_SendStatus");
                                    if (Global.SelectFirst != true)
                                    {
                                        if (Convert.ToDateTime(DateTime.Now.ToString("HH:mm")).CompareTo(Convert.ToDateTime("6:00")) >= 0 && Convert.ToDateTime(DateTime.Now.ToString("HH:mm")).CompareTo(Convert.ToDateTime("18:00")) < 0)
                                        {
                                            Global.TraceUpLoad_Error_D++;//白班Trace上传异常计数累加
                                            Global.TraceThench_Error_D++;
                                            _datastatisticsfrm.UpDataDGV_D(10, 1, Global.TraceUpLoad_Error_D.ToString());
                                            Global.inidata.productconfig.TraceUpLoad_Error_D = Global.TraceUpLoad_Error_D.ToString();
                                        }
                                        else
                                        {
                                            Global.TraceUpLoad_Error_N++;//夜班Trace上传异常计数累加
                                            Global.TraceThench_Error_N++;
                                            _datastatisticsfrm.UpDataDGV_N(10, 1, Global.TraceUpLoad_Error_N.ToString());
                                            Global.inidata.productconfig.TraceUpLoad_Error_N = Global.TraceUpLoad_Error_N.ToString();
                                        }
                                    }
                                    ShowStatus("与Trace断开连接", Color.Red, 2);
                                    Trace_Logs_flag = false;
                                    Global.PLC_Client.WritePLC_D(10305, new short[] { 2 });
                                    Log.WriteLog(item + "Trace_Logs_Trench_Bracket发送异常" + ex.ToString().Replace("\r\n", "") + ",TraceLog");
                                    string insStr = string.Format("insert into Trace_LA_SendNG([DateTime],[sp],[band],[test_result],[fixture_id],[tossing_item],"
                                           + "[STATION_STRING],[uut_start],[Weld_start_time],[Weld_stop_time],[uut_stop]"
                                           + ") values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}'"
                                           + ")", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), Trace_ua2[item].serials.sp, Trace_ua2[item].data.insight.uut_attributes.full_sn, Trace_ua2[item].data.insight.test_attributes.test_result,
                                           Trace_ua2[item].data.insight.test_station_attributes.fixture_id, Trace_ua2[item].data.insight.uut_attributes.tossing_item, Trace_ua2[item].data.insight.uut_attributes.STATION_STRING,
                                           Trace_ua2[item].data.insight.test_attributes.uut_start, Trace_ua2[item].data.insight.uut_attributes.laser_start_time, Trace_ua2[item].data.insight.uut_attributes.laser_stop_time,
                                           Trace_ua2[item].data.insight.test_attributes.uut_stop);
                                    int r = SQL.ExecuteUpdate(insStr);
                                    Log.WriteLog(string.Format("插入了{0}行Trace_Trench_Bracket_SendNG数据", r) + ",TraceLog");
                                    Log.WriteLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + "Trench_Bracket" + ","
                                            + Trace_ua2[item].data.insight.uut_attributes.full_sn + ","
                                            + Trace_ua2[item].data.insight.test_station_attributes.fixture_id + ","
                                            + Trace_ua2[item].data.insight.test_attributes.uut_start + ","
                                            + Trace_ua2[item].data.insight.uut_attributes.laser_start_time + ","
                                            + Trace_ua2[item].data.insight.uut_attributes.laser_stop_time + ","
                                            + Trace_ua2[item].data.insight.test_attributes.uut_stop + ","
                                            + Trace_ua2[item].data.insight.uut_attributes.tossing_item + ","
                                            + "NG-Trace网络异常" + ","
                                            + Trace_ua2[item].data.insight.test_attributes.test_result, System.AppDomain.CurrentDomain.BaseDirectory + "\\Trace_Logs_U_Bracket_NG数据\\");
                                    Out_Trace_ua2.RemoveAt(0);
                                    Trace_ua2.Remove(item);
                                    string JsonBody = SendTraceLogs.Replace(',', ';');
                                    Log.WriteCSV_DiscardLog("Trace" + "," + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + "Trace_Trench_Bracket" + "," + Global.inidata.productconfig.OEE_Dsn + "," + item + "," + JsonBody + "," + Trcae_logs_str + "," + "1");
                                }
                                catch (Exception ex2)
                                {
                                    Out_Trace_ua2.RemoveAt(0);
                                    Trace_ua2.Remove(item);
                                    Global.PLC_Client.WritePLC_D(10305, new short[] { 2 });
                                    MessageBox.Show("Trace_Trench_Bracket上传异常，请找工程师排查!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    Log.WriteLog("Trace_Trench_Bracket上传异常!" + ex2.ToString() + ",TraceLog");
                                }
                            }
                        }
                        else
                        {
                            Out_Trace_ua2.RemoveAt(0);
                            Trace_ua2.Remove(item);
                            Log.WriteLog("Trace_Trench_Bracket上传SN格式不正确:" + item + ",TraceLog");
                            Global.PLC_Client.WritePLC_D(10305, new short[] { 2 });
                            _homefrm.AppendRichText("Trace_Trench_Bracket上传SN格式不正确:" + item, "rtx_TraceMsg");
                        }
                        #endregion
                    }
                }
                Thread.Sleep(5);
            }
        }
        #endregion



        #region PDCA-Bracket
        private void EthPdcaUA()
        {
            while (true)
            {
                if (bail_ua.Count > 0)
                {
                    if (Out_ua.Count > 0)
                    {
                        string item = Out_ua[0];
                        try
                        {
                            Stopwatch SW_time2 = new Stopwatch();
                            SW_time2.Start();
                            HansData_U_Bracket UA_data = new HansData_U_Bracket();
                            GetAllHansData(item, out UA_data, 1);
                            if (item.Contains("FM") && !Regex.IsMatch(item, "[a-z]"))
                            {
                                TimeSpan ts1, ts2;
                                if (bail_ua[item].test_result == "PASS" && UA_data.HanDataUAResult)
                                {
                                    bail_ua[item].test_result = "PASS";
                                }
                                else if (bail_ua[item].test_result == "FAIL" && !UA_data.HanDataUAResult)
                                {
                                    bail_ua[item].test_result = "FAIL";
                                    bail_ua[item].tossing_item += "/Welding data acquire NG";
                                }
                                else if (bail_ua[item].test_result == "FAIL" && UA_data.HanDataUAResult)
                                {
                                    bail_ua[item].test_result = "FAIL";
                                }
                                else if (bail_ua[item].test_result == "PASS" && !UA_data.HanDataUAResult)
                                {
                                    bail_ua[item].test_result = "FAIL";
                                    bail_ua[item].tossing_item = "Welding data acquire NG";
                                }
                                ts1 = bail_ua[item].Weld_start_time - bail_ua[item].Start_Time;
                                ts2 = bail_ua[item].Weld_stop_time - bail_ua[item].Weld_start_time;
                                bail_ua[item].Weld_wait_ct = ts1.TotalSeconds.ToString("0");
                                bail_ua[item].Actual_weld_ct = ts2.TotalSeconds.ToString("0");
                                string[] msg = new string[200];
                                bail_ua[item].SN = bail_ua[item].full_sn;
                                bc.SN = bail_ua[item].SN;
                                // ----------------------------------上传图片-----------------------
                                //DirectoryInfo theFolder = new DirectoryInfo(@"\\169.254.1.10\Public");
                                //相对共享文件夹的路径
                                Log.WriteLog(item);
                                string fielpath = @"Z:\" + item.Remove(18);
                                try
                                {
                                    //发送图片给Mac mini
                                    SendPicture(fielpath, item.Remove(18), 1);
                                }
                                catch (Exception ex)
                                {
                                    Log.WriteLog(string.Format("{0}图片发送失败", item) + ex.ToString().Replace("\r\n", "") + ",PDCALog");
                                }
                                //}
                                //-----------------------------------------------------------------
                                msg[0] = bc.createStartMessage(bail_ua[item].SN);
                                msg[1] = bc.createPriority("1");
                                msg[2] = bc.createAttrMessage("unit_serial_number", bail_ua[item].SN);//17位
                                msg[3] = bc.createAttrMessage("test_result", bail_ua[item].test_result);
                                msg[4] = bc.createStartTimeMessage(bail_ua[item].Start_Time);
                                msg[5] = bc.createStopTimeMessage(bail_ua[item].Stop_Time);
                                msg[6] = bc.createAttrMessage("line_id", Global.inidata.productconfig.Line_id_ua);
                                msg[7] = bc.createAttrMessage("station_id", Global.inidata.productconfig.Station_id_ua);
                                msg[8] = bc.createDUT_POSMessage(bail_ua[item].Fixture_id.Replace("*", ""), "2");
                                msg[9] = bc.createAttrMessage("software_name", Global.inidata.productconfig.Sw_name_ua);
                                msg[10] = bc.createAttrMessage("software_version", Global.inidata.productconfig.Sw_version);
                                msg[11] = bc.createAttrMessage("full_sn", bail_ua[item].full_sn);//18位
                                msg[12] = bc.createAttrMessage("fixture_id", bail_ua[item].Fixture_id.Replace("*", ""));
                                msg[13] = bc.createAttrMessage("laser_start_time", bail_ua[item].Weld_start_time.ToString("yyyy-MM-dd HH:mm:ss"));
                                msg[14] = bc.createAttrMessage("laser_stop_time", bail_ua[item].Weld_stop_time.ToString("yyyy-MM-dd HH:mm:ss"));
                                msg[15] = bc.createAttrMessage("tossing_item", bail_ua[item].tossing_item);
                                msg[16] = bc.createAttrMessage("precitec_rev", "0");
                                msg[17] = bc.createAttrMessage("precitec_grading", "0");
                                msg[18] = bc.createPDataMessage("precitec_value", "0", "", "", "%");
                                msg[19] = bc.createAttrMessage("pattern_type", UA_data.pattern_type);
                                msg[20] = bc.createAttrMessage("spot_size", UA_data.spot_size);
                                msg[21] = bc.createAttrMessage("hatch", UA_data.hatch);
                                msg[22] = bc.createAttrMessage("swing_amplitude", UA_data.swing_amplitude);
                                msg[23] = bc.createAttrMessage("swing_freq", UA_data.swing_freq);

                                msg[24] = bc.createPDataMessage("location1_shim-NE", "0", "", "", "/");
                                msg[25] = bc.createPDataMessage("location2_shimSL.hs", "0", "", "", "/");
                                msg[26] = bc.createPDataMessage("location3_shim-SR", "0", "", "", "/");
                                msg[27] = bc.createPDataMessage("location4_Trench bracket", "0", "", "", "/");

                                msg[36] = bc.createPDataMessage("location1_layer1_laser_power", UA_data.location1_layer1_laser_power, "", "", "W");
                                msg[37] = bc.createPDataMessage("location1_layer1_frequency", UA_data.location1_layer1_frequency, "", "", "KHz");
                                msg[38] = bc.createPDataMessage("location1_layer1_waveform", UA_data.location1_layer1_waveform, "", "", "/");
                                msg[39] = bc.createPDataMessage("location1_layer1_pulse_energy", UA_data.location1_layer1_pulse_energy, "", "", "J");
                                msg[40] = bc.createPDataMessage("location1_layer1_laser_speed", UA_data.location1_layer1_laser_speed, "", "", "mm/s");
                                msg[41] = bc.createPDataMessage("location1_layer1_jump_speed", UA_data.location1_layer1_jump_speed, "", "", "mm/s");
                                msg[42] = bc.createPDataMessage("location1_layer1_jump_delay", UA_data.location1_layer1_jump_delay, "", "", "us");
                                msg[43] = bc.createPDataMessage("location1_layer1_scanner_delay", UA_data.location1_layer1_scanner_delay, "", "", "us");

                                msg[44] = bc.createPDataMessage("location2_layer1_laser_power", UA_data.location2_layer1_laser_power, "", "", "W");
                                msg[45] = bc.createPDataMessage("location2_layer1_frequency", UA_data.location2_layer1_frequency, "", "", "KHz");
                                msg[46] = bc.createPDataMessage("location2_layer1_waveform", UA_data.location2_layer1_waveform, "", "", "/");
                                msg[47] = bc.createPDataMessage("location2_layer1_pulse_energy", UA_data.location2_layer1_pulse_energy, "", "", "J");
                                msg[48] = bc.createPDataMessage("location2_layer1_laser_speed", UA_data.location2_layer1_laser_speed, "", "", "mm/s");
                                msg[49] = bc.createPDataMessage("location2_layer1_jump_speed", UA_data.location2_layer1_jump_speed, "", "", "mm/s");
                                msg[50] = bc.createPDataMessage("location2_layer1_jump_delay", UA_data.location2_layer1_jump_delay, "", "", "us");
                                msg[51] = bc.createPDataMessage("location2_layer1_scanner_delay", UA_data.location2_layer1_scanner_delay, "", "", "us");

                                msg[52] = bc.createPDataMessage("location3_layer1_laser_power", UA_data.location3_layer1_laser_power, "", "", "W");
                                msg[53] = bc.createPDataMessage("location3_layer1_frequency", UA_data.location3_layer1_frequency, "", "", "KHz");
                                msg[54] = bc.createPDataMessage("location3_layer1_waveform", UA_data.location3_layer1_waveform, "", "", "/");
                                msg[55] = bc.createPDataMessage("location3_layer1_pulse_energy", UA_data.location3_layer1_pulse_energy, "", "", "J");
                                msg[56] = bc.createPDataMessage("location3_layer1_laser_speed", UA_data.location3_layer1_laser_speed, "", "", "mm/s");
                                msg[57] = bc.createPDataMessage("location3_layer1_jump_speed", UA_data.location3_layer1_jump_speed, "", "", "mm/s");
                                msg[58] = bc.createPDataMessage("location3_layer1_jump_delay", UA_data.location3_layer1_jump_delay, "", "", "us");
                                msg[59] = bc.createPDataMessage("location3_layer1_scanner_delay", UA_data.location3_layer1_scanner_delay, "", "", "us");

                                msg[61] = bc.createPDataMessage("location4_layer1_laser_power", UA_data.location4_layer1_laser_power, "", "", "W");
                                msg[62] = bc.createPDataMessage("location4_layer1_frequency", UA_data.location4_layer1_frequency, "", "", "KHz");
                                msg[63] = bc.createPDataMessage("location4_layer1_waveform", UA_data.location4_layer1_waveform, "", "", "/");
                                msg[64] = bc.createPDataMessage("location4_layer1_pulse_energy", UA_data.location4_layer1_pulse_energy, "", "", "J");
                                msg[65] = bc.createPDataMessage("location4_layer1_laser_speed", UA_data.location4_layer1_laser_speed, "", "", "mm/s");
                                msg[66] = bc.createPDataMessage("location4_layer1_jump_speed", UA_data.location4_layer1_jump_speed, "", "", "mm/s");
                                msg[67] = bc.createPDataMessage("location4_layer1_jump_delay", UA_data.location4_layer1_jump_delay, "", "", "us");
                                msg[68] = bc.createPDataMessage("location4_layer1_scanner_delay", UA_data.location4_layer1_scanner_delay, "", "", "us");


                                msg[198] = bc.createSubmitMessage(Global.inidata.productconfig.Sw_version);
                                msg[199] = "begin\n";
                                for (int i = 0; i < msg.Length - 1; i++)
                                {
                                    msg[199] += msg[i];
                                }
                                msg[199] = msg[199] + "end\n";
                                Log.WriteLog(msg[199].Replace("\n", "") + ",PDCALog");
                                //return;
                                _homefrm.AppendRichText("PDCA_U_Bracket:" + msg[199], "rtx_PDCAMsg");
                                string reply = "";
                                Stopwatch sw = new Stopwatch();
                                try
                                {
                                    sw.Start();
                                    bc.sendMessage(msg[199]);
                                    reply = bc.getReply();
                                    sw.Stop();
                                    Log.WriteLog("PDCA_U_Bracket上传时间：" + sw.ElapsedMilliseconds.ToString() + ",PDCALog");
                                    Log.WriteLog(reply.Replace("\n", "") + ",PDCALog");
                                    if (reply.Contains("NOT_RESPONDING") || reply.Contains("bad") || reply.Contains("FUNCTIONING") || reply.Contains("PROCESS") || reply.Contains("INSTANTPUDDING") || reply.Contains("Signature") || reply.Contains("Timeout"))
                                    {
                                        Global.PDCA_ua_Data_NG++;
                                        Global.Product_num_ua_ng++;
                                        _homefrm.UpDatalabel(Global.Product_num_ua_ng.ToString(), "lb_PDCAUANG");
                                        if (Global.SelectFirst == false)
                                        {
                                            if (Convert.ToDateTime(DateTime.Now.ToString("HH:mm")).CompareTo(Convert.ToDateTime("6:00")) >= 0 && Convert.ToDateTime(DateTime.Now.ToString("HH:mm")).CompareTo(Convert.ToDateTime("18:00")) < 0)
                                            {
                                                Global.PDCAUpLoad_Error_D++;//白班PDCA上传异常计数累加
                                                _datastatisticsfrm.UpDataDGV_D(11, 1, Global.PDCAUpLoad_Error_D.ToString());
                                                Global.inidata.productconfig.PDCAUpLoad_Error_D = Global.PDCAUpLoad_Error_D.ToString();
                                            }
                                            else
                                            {
                                                Global.PDCAUpLoad_Error_N++;//夜班PDCA上传异常计数累加
                                                _datastatisticsfrm.UpDataDGV_N(11, 1, Global.PDCAUpLoad_Error_N.ToString());
                                                Global.inidata.productconfig.PDCAUpLoad_Error_N = Global.PDCAUpLoad_Error_N.ToString();
                                            }
                                        }
                                        Log.WriteLog("PDCA_U_Bracket数据上传失败数量：" + Global.PDCA_ua_Data_NG + "Pcs" + ",PDCALog");
                                        string insStr = string.Format("insert into PDCA_SendNG([DateTime],[band],[test_result],[fixture_id],[tossing_item],[STATION_STRING],"
                                                + "[uut_start],[weld_start_time],[Weld_stop_time],[uut_stop]"
                                                + ") values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}'"
                                                + ")", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), bail_ua[item].full_sn, bail_ua[item].test_result, bail_ua[item].Fixture_id,
                                                bail_ua[item].tossing_item, bail_ua[item].STATION_STRING, bail_ua[item].Start_Time.ToString("yyyy-MM-dd HH:mm:ss"), bail_ua[item].Weld_start_time.ToString("yyyy-MM-dd HH:mm:ss"), bail_ua[item].Weld_stop_time.ToString("yyyy-MM-dd HH:mm:ss"),
                                                bail_ua[item].Stop_Time.ToString("yyyy-MM-dd HH:mm:ss"));
                                        int r = SQL.ExecuteUpdate(insStr);
                                        Log.WriteLog(string.Format("插入了{0}行PDCA_UA_SendNG数据", r) + ",PDCALog");
                                        Mac_mini_server_ua = false;
                                        Log.WriteLog("数据发送PDCA_ua_Sever失败" + reply + ",PDCALog");
                                        Log.WriteCSV_NG(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + "U_Bracket" + "," + bail_ua[item].full_sn + "," + bail_ua[item].Fixture_id.Replace("*", "") + "," + bail_ua[item].Start_Time.ToString("yyyy-MM-dd HH:mm:ss") + "," + bail_ua[item].Weld_start_time.ToString("yyyy-MM-dd HH:mm:ss") + "," + bail_ua[item].Weld_stop_time.ToString("yyyy-MM-dd HH:mm:ss") + "," + bail_ua[item].Stop_Time.ToString("yyyy-MM-dd HH:mm:ss") + "," + bail_ua[item].Weld_wait_ct + "," + bail_ua[item].Actual_weld_ct + "," + "OK-网络异常-BEB003", System.AppDomain.CurrentDomain.BaseDirectory + "\\bail_U_BracketsendNG数据\\");
                                        Log.WriteCSV(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + "U_Bracket" + "," + bail_ua[item].full_sn + "," + bail_ua[item].Fixture_id.Replace("*", "") + "," + bail_ua[item].Start_Time.ToString("yyyy-MM-dd HH:mm:ss") + "," + bail_ua[item].Weld_start_time.ToString("yyyy-MM-dd HH:mm:ss") + "," + bail_ua[item].Weld_stop_time.ToString("yyyy-MM-dd HH:mm:ss") + "," + bail_ua[item].Stop_Time.ToString("yyyy-MM-dd HH:mm:ss") + "," + bail_ua[item].Weld_wait_ct + "," + bail_ua[item].Actual_weld_ct + "," + "OK-网络异常" + "," + bail_ua[item].test_result, System.AppDomain.CurrentDomain.BaseDirectory + "\\bail_U_Bracket数据\\");
                                        Log.WriteCSV(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + "U_Bracket" + "," + bail_ua[item].full_sn + "," + bail_ua[item].Fixture_id.Replace("*", "") + "," + bail_ua[item].Start_Time.ToString("yyyy-MM-dd HH:mm:ss") + "," + bail_ua[item].Weld_start_time.ToString("yyyy-MM-dd HH:mm:ss") + "," + bail_ua[item].Weld_stop_time.ToString("yyyy-MM-dd HH:mm:ss") + "," + bail_ua[item].Stop_Time.ToString("yyyy-MM-dd HH:mm:ss") + "," + bail_ua[item].Weld_wait_ct + "," + bail_ua[item].Actual_weld_ct + "," + "OK-网络异常" + "," + bail_ua[item].test_result, System.AppDomain.CurrentDomain.BaseDirectory + "\\PDCA上传异常数据\\");
                                        if (bail_ua[item].auto_send == 1)
                                        {
                                            Global.PLC_Client2.WritePLC_D(10302, new short[] { 2 });
                                            _homefrm.UpDatalabelcolor(Color.Red, "SN发送失败", "lb_PDCA_UA_SendStatus");
                                            _homefrm.AppendRichText(reply + ",网络异常", "rtx_PDCAMsg");
                                        }
                                        else
                                        {
                                            _homefrm.UpDatalabelcolor(Color.Red, "SN发送失败", "lb_PDCA_UA_SendStatus");
                                            _homefrm.AppendRichText(reply + ",网络异常", "rtx_PDCAMsg");
                                        }
                                        Out_ua.RemoveAt(0);
                                        bail_ua.Remove(item);
                                        string JSONBody = msg[199].Replace("\n", "|");
                                        Log.WriteCSV_DiscardLog("PDCA" + "," + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + "U_Bracket" + "," + Global.inidata.productconfig.OEE_Dsn + "," + item + "," + JSONBody + "," + reply.Replace("\n", "") + "," + "1");
                                        _homefrm.UiText(bail_ua[item].full_sn, "txt_PDCA_sp_ua");
                                        _homefrm.UiText(bail_ua[item].test_result, "txt_PDCA_test_result_ua");
                                        _homefrm.UiText(bail_ua[item].SN, "txt_PDCA_unit_serial_number_ua");
                                        _homefrm.UiText(bail_ua[item].Fixture_id, "txt_PDCA_fixture_id_ua");
                                        _homefrm.UiText(bail_ua[item].Start_Time.ToString("yyyy-MM-dd HH:mm:ss"), "txt_PDCA_uut_start_ua");
                                        _homefrm.UiText(bail_ua[item].Weld_start_time.ToString("yyyy-MM-dd HH:mm:ss"), "txt_PDCA_weld_start_time_ua");
                                        _homefrm.UiText(bail_ua[item].Weld_stop_time.ToString("yyyy-MM-dd HH:mm:ss"), "txt_PDCA_weld_stop_time_ua");
                                        _homefrm.UiText(bail_ua[item].Stop_Time.ToString("yyyy-MM-dd HH:mm:ss"), "txt_PDCA_uut_stop_ua");
                                        _homefrm.UiText(bail_ua[item].Weld_wait_ct.ToString(), "txt_PDCA_weld_wait_ct_ua");
                                        _homefrm.UiText(bail_ua[item].Actual_weld_ct.ToString(), "txt_PDCA_actual_weld_ct_ua");
                                    }
                                    else if (reply.Contains("ok"))
                                    {
                                        if (bail_ua[item].test_fail)
                                        {
                                            Global.Product_num_ua_ok++;
                                            _homefrm.UpDatalabel(Global.Product_num_ua_ok.ToString(), "lb_PDCAUAOK");
                                        }
                                        else
                                        {
                                            Global.Product_num_ua_ng++;
                                            _homefrm.UpDatalabel(Global.Product_num_ua_ng.ToString(), "lb_PDCAUANG");
                                            string JSONBody = msg[199].Replace("\n", "|");
                                            Log.WriteCSV_DiscardLog("Product" + "," + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + "U_Bracket" + "," + Global.inidata.productconfig.OEE_Dsn + "," + item + "," + JSONBody + "," + "焊接NG" + "," + "1");
                                        }
                                        Global.PDCA_ua_Data_NG = 0;
                                        Mac_mini_server_ua = true;
                                        if (bail_ua[item].auto_send == 1)
                                        {                                            
                                            Global.PLC_Client2.WritePLC_D(10302, new short[] { 1 });
                                            _homefrm.UpDatalabelcolor(Color.Green, "SN发送成功", "lb_PDCA_UA_SendStatus");
                                            _homefrm.AppendRichText(reply, "rtx_PDCAMsg");
                                            dateTime1 = DateTime.Now;
                                            double date = (dateTime1 - dateTime).TotalMilliseconds;
                                            _homefrm.AppendRichText(date + "  ", "rtx_PDCAMsg");
                                        }
                                        else
                                        {
                                            _homefrm.UpDatalabelcolor(Color.Green, "SN发送成功", "lb_PDCA_UA_SendStatus");
                                            _homefrm.AppendRichText(reply, "rtx_PDCAMsg");
                                        }
                                        Log.WriteCSV(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + "U_Bracket" + "," + bail_ua[item].full_sn + "," + bail_ua[item].Fixture_id.Replace("*", "") + "," + bail_ua[item].Start_Time.ToString("yyyy-MM-dd HH:mm:ss") + "," + bail_ua[item].Weld_start_time.ToString("yyyy-MM-dd HH:mm:ss") + "," + bail_ua[item].Weld_stop_time.ToString("yyyy-MM-dd HH:mm:ss") + "," + bail_ua[item].Stop_Time.ToString("yyyy-MM-dd HH:mm:ss") + "," + bail_ua[item].Weld_wait_ct + "," + bail_ua[item].Actual_weld_ct + "," + "OK-success" + "," + bail_ua[item].test_result, System.AppDomain.CurrentDomain.BaseDirectory + "\\bail_U_Bracket数据\\");
                                        try
                                        {
                                            _homefrm.UiText(bail_ua[item].full_sn, "txt_PDCA_sp_ua");
                                            _homefrm.UiText(bail_ua[item].test_result, "txt_PDCA_test_result_ua");
                                            _homefrm.UiText(bail_ua[item].SN, "txt_PDCA_unit_serial_number_ua");
                                            _homefrm.UiText(bail_ua[item].Fixture_id, "txt_PDCA_fixture_id_ua");
                                            _homefrm.UiText(bail_ua[item].Start_Time.ToString("yyyy-MM-dd HH:mm:ss"), "txt_PDCA_uut_start_ua");
                                            _homefrm.UiText(bail_ua[item].Weld_start_time.ToString("yyyy-MM-dd HH:mm:ss"), "txt_PDCA_weld_start_time_ua");
                                            _homefrm.UiText(bail_ua[item].Weld_stop_time.ToString("yyyy-MM-dd HH:mm:ss"), "txt_PDCA_weld_stop_time_ua");
                                            _homefrm.UiText(bail_ua[item].Stop_Time.ToString("yyyy-MM-dd HH:mm:ss"), "txt_PDCA_uut_stop_ua");
                                            _homefrm.UiText(bail_ua[item].Weld_wait_ct.ToString(), "txt_PDCA_weld_wait_ct_ua");
                                            _homefrm.UiText(bail_ua[item].Actual_weld_ct.ToString(), "txt_PDCA_actual_weld_ct_ua");
                                        }
                                        catch
                                        { }
                                        Out_ua.RemoveAt(0);
                                        bail_ua.Remove(item);
                                        Log.WriteLog("数据发送PDCA_U_Bracket_Sever成功" + ",PDCALog");
                                    }
                                    else
                                    {
                                        Mac_mini_server_ua = false;
                                        Log.WriteLog("数据发送PDCA_U_Bracket_Sever失败" + ",PDCALog");
                                        Global.PDCA_ua_Data_NG++;
                                        Global.Product_num_ua_ng++;
                                        _homefrm.UpDatalabel(Global.Product_num_ua_ng.ToString(), "lb_PDCAUANG");
                                        if (Global.SelectFirst == false)
                                        {
                                            if (Convert.ToDateTime(DateTime.Now.ToString("HH:mm")).CompareTo(Convert.ToDateTime("6:00")) >= 0 && Convert.ToDateTime(DateTime.Now.ToString("HH:mm")).CompareTo(Convert.ToDateTime("18:00")) < 0)
                                            {
                                                Global.PDCAUpLoad_Error_D++;//白班PDCA上传异常计数累加
                                                _datastatisticsfrm.UpDataDGV_D(11, 1, Global.PDCAUpLoad_Error_D.ToString());
                                                Global.inidata.productconfig.PDCAUpLoad_Error_D = Global.PDCAUpLoad_Error_D.ToString();
                                                // Global.inidata.WriteProductnumSection();
                                            }
                                            else
                                            {
                                                Global.PDCAUpLoad_Error_N++;//夜班PDCA上传异常计数累加
                                                _datastatisticsfrm.UpDataDGV_N(11, 1, Global.PDCAUpLoad_Error_N.ToString());
                                                Global.inidata.productconfig.PDCAUpLoad_Error_N = Global.PDCAUpLoad_Error_N.ToString();
                                                //Global.inidata.WriteProductnumSection();
                                            }
                                        }
                                        Log.WriteLog("PDCA_U_Bracket数据上传失败数量：" + Global.PDCA_ua_Data_NG + "Pcs" + ",PDCALog");
                                        string insStr = string.Format("insert into PDCA_SendNG([DateTime],[band],[test_result],[fixture_id],[tossing_item],[STATION_STRING],"
                                             + "[uut_start],[weld_start_time],[Weld_stop_time],[uut_stop]"
                                             + ") values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}'"
                                             + ")", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), bail_ua[item].full_sn, bail_ua[item].test_result, bail_ua[item].Fixture_id,
                                             bail_ua[item].tossing_item, bail_ua[item].STATION_STRING, bail_ua[item].Start_Time.ToString("yyyy-MM-dd HH:mm:ss"), bail_ua[item].Weld_start_time.ToString("yyyy-MM-dd HH:mm:ss"), bail_ua[item].Weld_stop_time.ToString("yyyy-MM-dd HH:mm:ss"),
                                             bail_ua[item].Stop_Time.ToString("yyyy-MM-dd HH:mm:ss"));
                                        int r = SQL.ExecuteUpdate(insStr);
                                        Log.WriteLog(string.Format("插入了{0}行PDCA_UA_SendNG数据", r) + ",PDCALog");
                                        Log.WriteCSV_NG(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + "U_Bracket" + "," + bail_ua[item].full_sn + "," + bail_ua[item].Fixture_id.Replace("*", "") + "," + bail_ua[item].Start_Time.ToString("yyyy-MM-dd HH:mm:ss") + "," + bail_ua[item].Weld_start_time.ToString("yyyy-MM-dd HH:mm:ss") + "," + bail_ua[item].Weld_stop_time.ToString("yyyy-MM-dd HH:mm:ss") + "," + bail_ua[item].Stop_Time.ToString("yyyy-MM-dd HH:mm:ss") + "," + bail_ua[item].Weld_wait_ct + "," + bail_ua[item].Actual_weld_ct + "," + "OK-网络异常-BEB003", System.AppDomain.CurrentDomain.BaseDirectory + "\\bail_U_BracketsendNG数据\\");
                                        Log.WriteCSV(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + "U_Bracket" + "," + bail_ua[item].full_sn + "," + bail_ua[item].Fixture_id.Replace("*", "") + "," + bail_ua[item].Start_Time.ToString("yyyy-MM-dd HH:mm:ss") + "," + bail_ua[item].Weld_start_time.ToString("yyyy-MM-dd HH:mm:ss") + "," + bail_ua[item].Weld_stop_time.ToString("yyyy-MM-dd HH:mm:ss") + "," + bail_ua[item].Stop_Time.ToString("yyyy-MM-dd HH:mm:ss") + "," + bail_ua[item].Weld_wait_ct + "," + bail_ua[item].Actual_weld_ct + "," + "OK-网络异常" + "," + bail_ua[item].test_result, System.AppDomain.CurrentDomain.BaseDirectory + "\\bail_U_Bracket数据\\");
                                        Log.WriteCSV(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + "U_Bracket" + "," + bail_ua[item].full_sn + "," + bail_ua[item].Fixture_id.Replace("*", "") + "," + bail_ua[item].Start_Time.ToString("yyyy-MM-dd HH:mm:ss") + "," + bail_ua[item].Weld_start_time.ToString("yyyy-MM-dd HH:mm:ss") + "," + bail_ua[item].Weld_stop_time.ToString("yyyy-MM-dd HH:mm:ss") + "," + bail_ua[item].Stop_Time.ToString("yyyy-MM-dd HH:mm:ss") + "," + bail_ua[item].Weld_wait_ct + "," + bail_ua[item].Actual_weld_ct + "," + "OK-网络异常" + "," + bail_ua[item].test_result, System.AppDomain.CurrentDomain.BaseDirectory + "\\PDCA上传异常数据\\");
                                        if (bail_ua[item].auto_send == 1)
                                        {
                                            Global.PLC_Client2.WritePLC_D(10302, new short[] { 2 });
                                            _homefrm.UpDatalabelcolor(Color.Red, "SN发送失败", "lb_PDCA_UA_SendStatus");
                                            _homefrm.AppendRichText(reply, "rtx_PDCAMsg");
                                        }
                                        else
                                        {
                                            _homefrm.UpDatalabelcolor(Color.Red, "SN发送失败", "lb_PDCA_UA_SendStatus");
                                            _homefrm.AppendRichText(reply, "rtx_PDCAMsg");
                                        }
                                        Out_ua.RemoveAt(0);
                                        bail_ua.Remove(item);
                                        string JSONBody = msg[199].Replace("\n", "|");
                                        Log.WriteCSV_DiscardLog("PDCA" + "," + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + "U_Bracket" + "," + Global.inidata.productconfig.OEE_Dsn + "," + item + "," + JSONBody + "," + reply.Replace("\n", "") + "," + "1");
                                    }
                                    SW_time2.Stop();
                                    Log.WriteLog("PDCA_U_Bracket上传总时间为：" + SW_time2.ElapsedMilliseconds.ToString() + ",PDCALog");
                                }
                                catch (Exception EX)
                                {
                                    try
                                    {
                                        Mac_mini_server_ua = false;
                                        Global.PDCA_ua_Data_NG++;
                                        Global.Product_num_ua_ng++;
                                        _homefrm.UpDatalabel(Global.Product_num_ua_ng.ToString(), "lb_PDCAUANG");
                                        if (Global.SelectFirst == false)
                                        {
                                            if (Convert.ToDateTime(DateTime.Now.ToString("HH:mm")).CompareTo(Convert.ToDateTime("6:00")) >= 0 && Convert.ToDateTime(DateTime.Now.ToString("HH:mm")).CompareTo(Convert.ToDateTime("18:00")) < 0)
                                            {
                                                Global.PDCAUpLoad_Error_D++;//白班PDCA上传异常计数累加
                                                _datastatisticsfrm.UpDataDGV_D(11, 1, Global.PDCAUpLoad_Error_D.ToString());
                                                Global.inidata.productconfig.PDCAUpLoad_Error_D = Global.PDCAUpLoad_Error_D.ToString();
                                                //  Global.inidata.WriteProductnumSection();
                                            }
                                            else
                                            {
                                                Global.PDCAUpLoad_Error_N++;//夜班PDCA上传异常计数累加
                                                _datastatisticsfrm.UpDataDGV_N(11, 1, Global.PDCAUpLoad_Error_N.ToString());
                                                Global.inidata.productconfig.PDCAUpLoad_Error_N = Global.PDCAUpLoad_Error_N.ToString();
                                                // Global.inidata.WriteProductnumSection();
                                            }
                                        }
                                        Log.WriteLog("PDCA_U_Bracket数据上传失败数量：" + Global.PDCA_ua_Data_NG + "Pcs" + ",PDCALog");
                                        string insStr = string.Format("insert into PDCA_SendNG([DateTime],[band],[test_result],[fixture_id],[tossing_item],[STATION_STRING],"
                                               + "[uut_start],[weld_start_time],[Weld_stop_time],[uut_stop]"
                                               + ") values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}'"
                                               + ")", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), bail_ua[item].full_sn, bail_ua[item].test_result, bail_ua[item].Fixture_id,
                                               bail_ua[item].tossing_item, bail_ua[item].STATION_STRING, bail_ua[item].Start_Time.ToString("yyyy-MM-dd HH:mm:ss"), bail_ua[item].Weld_start_time.ToString("yyyy-MM-dd HH:mm:ss"), bail_ua[item].Weld_stop_time.ToString("yyyy-MM-dd HH:mm:ss"),
                                               bail_ua[item].Stop_Time.ToString("yyyy-MM-dd HH:mm:ss"));
                                        int r = SQL.ExecuteUpdate(insStr);
                                        Log.WriteLog(string.Format("插入了{0}行PDCA_UA_SendNG数据", r) + ",PDCALog");
                                        Log.WriteLog("数据发送PDCA_U_Bracket_Sever失败" + EX.ToString().Replace("\r\n", "") + ",PDCALog");
                                        Log.WriteCSV_NG(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + "U_Bracket" + "," + bail_ua[item].full_sn + "," + bail_ua[item].Fixture_id.Replace("*", "") + "," + bail_ua[item].Start_Time.ToString("yyyy-MM-dd HH:mm:ss") + "," + bail_ua[item].Weld_start_time.ToString("yyyy-MM-dd HH:mm:ss") + "," + bail_ua[item].Weld_stop_time.ToString("yyyy-MM-dd HH:mm:ss") + "," + bail_ua[item].Stop_Time.ToString("yyyy-MM-dd HH:mm:ss") + "," + bail_ua[item].Weld_wait_ct + "," + bail_ua[item].Actual_weld_ct + "," + "OK-网络异常-BEB003", System.AppDomain.CurrentDomain.BaseDirectory + "\\bail_U_BracketsendNG数据\\");
                                        Log.WriteCSV(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + "U_Bracket" + "," + bail_ua[item].full_sn + "," + bail_ua[item].Fixture_id.Replace("*", "") + "," + bail_ua[item].Start_Time.ToString("yyyy-MM-dd HH:mm:ss") + "," + bail_ua[item].Weld_start_time.ToString("yyyy-MM-dd HH:mm:ss") + "," + bail_ua[item].Weld_stop_time.ToString("yyyy-MM-dd HH:mm:ss") + "," + bail_ua[item].Stop_Time.ToString("yyyy-MM-dd HH:mm:ss") + "," + bail_ua[item].Weld_wait_ct + "," + bail_ua[item].Actual_weld_ct + "," + "OK-网络异常" + "," + bail_ua[item].test_result, System.AppDomain.CurrentDomain.BaseDirectory + "\\bail_U_Bracket数据\\");
                                        Log.WriteCSV(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + "U_Bracket" + "," + bail_ua[item].full_sn + "," + bail_ua[item].Fixture_id.Replace("*", "") + "," + bail_ua[item].Start_Time.ToString("yyyy-MM-dd HH:mm:ss") + "," + bail_ua[item].Weld_start_time.ToString("yyyy-MM-dd HH:mm:ss") + "," + bail_ua[item].Weld_stop_time.ToString("yyyy-MM-dd HH:mm:ss") + "," + bail_ua[item].Stop_Time.ToString("yyyy-MM-dd HH:mm:ss") + "," + bail_ua[item].Weld_wait_ct + "," + bail_ua[item].Actual_weld_ct + "," + "OK-网络异常" + "," + bail_ua[item].test_result, System.AppDomain.CurrentDomain.BaseDirectory + "\\PDCA上传异常数据\\");
                                        if (bail_ua[item].auto_send == 1)
                                        {
                                            Global.PLC_Client2.WritePLC_D(10302, new short[] { 2 });
                                            _homefrm.UpDatalabelcolor(Color.Red, "SN发送失败", "lb_PDCA_UA_SendStatus");
                                            _homefrm.AppendRichText(reply + ",网络异常", "rtx_PDCAMsg");
                                        }
                                        else
                                        {
                                            _homefrm.UpDatalabelcolor(Color.Red, "SN发送失败", "lb_PDCA_UA_SendStatus");
                                            _homefrm.AppendRichText(reply + ",网络异常", "rtx_PDCAMsg");
                                        }
                                        Out_ua.RemoveAt(0);
                                        bail_ua.Remove(item);
                                        string JSONBody = msg[199].Replace("\n", "|");
                                        Log.WriteCSV_DiscardLog("PDCA" + "," + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + "U_Bracket" + "," + Global.inidata.productconfig.OEE_Dsn + "," + item + "," + JSONBody + "," + "与PDCA服务器网络连接异常" + "," + "1");
                                    }
                                    catch (Exception ex2)
                                    {
                                        Out_ua.RemoveAt(0);
                                        bail_ua.Remove(item);
                                        MessageBox.Show("PDCA_U_Bracket上传异常，请找工程师排查!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                        Log.WriteLog("PDCA_U_Bracket上传异常!" + ex2.ToString() + ",PDCALog");
                                    }
                                }
                            }
                            else
                            {
                                Out_ua.RemoveAt(0);
                                bail_ua.Remove(item);
                                Log.WriteLog("PDCA_U_Bracket上传的SN格式不正确:" + item + ",PDCALog");
                                _homefrm.AppendRichText("PDCA_U_Bracket上传的SN格式不正确:" + item, "rtx_PDCAMsg");
                                Global.PLC_Client2.WritePLC_D(10302, new short[] { 2 });
                            }
                        }
                        catch (Exception EX)
                        {
                            Out_ua.RemoveAt(0);
                            bail_ua.Remove(item);
                            Global.PLC_Client2.WritePLC_D(10302, new short[] { 2 });
                            Log.WriteLog("PDCA_U_Bracket上传异常：" + EX.ToString().Replace("\r\n", "") + ",PDCALog");
                        }
                    }
                    Thread.Sleep(1);
                }

                Thread.Sleep(5);
            }
        }
        #endregion


        #region OEE Down Time
        private void EthDownTime_Mqtt()
        {
            try
            {
                //先获取计算机的主机名以及 OEE 网络的 MAC 物理地址
                ClientPcName = Dns.GetHostName();
                Mac = GetOEEMac();
                //-----记录OEE 关闭软件时长
                string SelectStr = "select * from OEE_MCOff where  1=1";
                DataTable d1 = SQL.ExecuteQuery(SelectStr);
                string StartTime = string.Empty;
                string sendTopic = Global.inidata.productconfig.EMT + "/upload/downtime";
                if (d1.Rows.Count > 0)//判断上一次是否正常关闭软件-有正常关机时间
                {
                    StartTime = d1.Rows[0][1].ToString();
                    DateTime T1 = Convert.ToDateTime(StartTime);
                    DateTime T2 = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    string TS = (T2 - T1).TotalMinutes.ToString("0.00");
                    string InsertOEEStr3 = "insert into OEE_DT([DateTime],[ErrorCode],[EventTime],[ModuleCode],[RunStatus],[ErrorInfo],[TimeSpan])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "'" + "," + "'" + "10010001" + "'" + "," + "'" + (Convert.ToDateTime(StartTime)).ToString("yyyy-MM-dd HH:mm:ss.fff") + "'" + ","
                                   + "'" + "" + "'" + "," + "'" + "6" + "'" + "," + "'" + "软件关闭" + "'" + "," + "'" + TS + "'" + ")";
                    SQL.ExecuteUpdate(InsertOEEStr3);
                    Log.WriteLog(DateTime.Now.ToString("HH:mm:ss") + "," + "10010001" + "," + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "," + "" + "," + "自动发送成功" + "," + "6" + "," + "软件关闭" + "," + TS, @"E:\装机软件\系统配置\System_ini\");
                }
                else//非正常关机
                {
                    string SelectStr2 = "select * from OEE_StartTime where ID =(SELECT MAX(ID) from OEE_StartTime)";
                    DataTable d2 = SQL.ExecuteQuery(SelectStr2);
                    string StartTime2 = string.Empty;
                    if (d2.Rows.Count > 0)//搜索非正常关机前的最后一次状态开始时间,记录写入OEE数据库中
                    {
                        StartTime = d2.Rows[0][3].ToString();
                        DateTime T11 = Convert.ToDateTime(StartTime);
                        DateTime T12 = Convert.ToDateTime(DateTime.Now.AddMinutes(-1).ToString("yyyy-MM-dd HH:mm:ss"));
                        string TS2 = (T12 - T11).TotalMinutes.ToString("0.00");
                        string InsertOEEStr4 = "insert into OEE_DT([DateTime],[ErrorCode],[EventTime],[ModuleCode],[RunStatus],[ErrorInfo],[TimeSpan])" + " " + "values(" + "'" + DateTime.Now.AddMinutes(-1).ToString("yyyy-MM-dd HH:mm:ss.fff") + "'" + "," + "'" + d2.Rows[0][2].ToString() + "'" + "," + "'" + (Convert.ToDateTime(StartTime)).ToString("yyyy-MM-dd HH:mm:ss.fff") + "'" + ","
                                       + "'" + d2.Rows[0][4].ToString() + "'" + "," + "'" + d2.Rows[0][1].ToString() + "'" + "," + "'" + d2.Rows[0][5].ToString() + "'" + "," + "'" + TS2 + "'" + ")";
                        SQL.ExecuteUpdate(InsertOEEStr4);
                        Log.WriteLog(DateTime.Now.AddMinutes(-1).ToString("HH:mm:ss") + "," + d2.Rows[0][2].ToString() + "," + DateTime.Now.AddMinutes(-1).ToString("yyyy-MM-dd HH:mm:ss.fff") + "," + d2.Rows[0][4].ToString() + "," + "自动发送成功" + "," + d2.Rows[0][1].ToString() + "," + d2.Rows[0][5].ToString() + "," + TS2, @"E:\装机软件\系统配置\System_ini\");
                    }
                    //非正常关机后，默认下一次开机时间的之前一分钟为关机开始时间
                    string OEEDownTime = "";
                    string EventTime = DateTime.Now.AddMinutes(-1).ToString("yyyy-MM-dd HH:mm:ss.fff");
                    Guid guid = Guid.NewGuid();
                    OEEDownTime = string.Format("{{\"guid\":\"{0}\",\"EMT\":\"{1}\",\"PoorNum\":\"{2}\",\"TotalNum\":\"{3}\",  \"Status\":\"{4}\",\"ErrorCode\":\"{5}\",\"EventTime\":\"{6}\",\"ModuleCode\":\"{7}\",\"ClientPcName\":\"{8}\",\"MAC\":\"{9}\",\"IP\":\"{10}\"}}", guid, Global.inidata.productconfig.EMT, "0", "0", "6", "10010001", EventTime, "", ClientPcName, Mac, IP);
                    Log.WriteLog("OEE_DT 补传关机数据:" + OEEDownTime + ",OEELog");
                    string InsertStr = "insert into OEE_TraceDT([DateTime],[Status],[ErrorCode],[EventTime],[ModuleCode])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," + "'" + "6" + "'" + "," + "'" + "10010001" + "'" + ","
                                          + "'" + EventTime + "'" + "," + "'" + "" + "'" + ")";
                    SQL.ExecuteUpdate(InsertStr);
                    //Global._mqttClient.Publish(sendTopic, Encoding.UTF8.GetBytes(OEEDownTime), MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE, false);
                    //var rst = SendMqttResult(guid);
                    //if (rst)
                    //{
                    //    if (Global.respond[guid].Result == "OK")
                    //    {
                    //        Global.ConnectOEEFlag = true;
                    //        _homefrm.AppendRichText("10010001" + ",触发时间=" + EventTime + ",运行状态:" + "6" + ",故障描述:" + "关机" + ",自动发送成功", "rtx_DownTimeMsg");
                    //        Log.WriteLog("OEE_DT补传关机errorCode发送成功");
                    //        Global.respond.TryRemove(guid, out Global.Respond);
                    //    }
                    //    else
                    //    {
                    //        Global.ConnectOEEFlag = false;
                    //        Log.WriteLog("OEE_DT补传关机errorCode发送失败");
                    //        _homefrm.AppendRichText("10010001" + ",触发时间=" + EventTime + ",运行状态:" + "6" + ",故障描述:" + "关机" + ",自动发送失败", "rtx_DownTimeMsg");
                    //        _homefrm.AppendRichText("失败原因:" + Global.respond[guid].ErrorCode, "rtx_DownTimeMsg");
                    //        Global.respond.TryRemove(guid, out Global.Respond);
                    //    }
                    //}
                    //else
                    //{
                    //    _homefrm.AppendRichText("10010001" + ",触发时间=" + EventTime + ",运行状态:" + "6" + ",故障描述:" + "关机" + ",自动发送失败", "rtx_DownTimeMsg");
                    //    _homefrm.AppendRichText("网络异常,超时无反馈", "rtx_DownTimeMsg");
                    //    Log.WriteLog("OEE_DT补传关机errorCode发送失败");
                    //    Global.ConnectOEEFlag = false;
                    //    string s = "insert into OEE_DTSendNG([DateTime],[Product],[GUID],[EMT],[PoorNum],[TotalNum],[Status],[ErrorCode],[ModuleCode],[ClientPcName],[MAC],[IP],[EventTime],[Moduleinfo],[errorinfo])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," + "'" + "OEE_DT" + "'" + "," + "'" + guid + "'" + "," + "'" + Global.inidata.productconfig.EMT + "'" + "," + "'" + "0" + "'" + "," + "'" + "0" + "'" + "," + "'" + "6" + "'" + "," + "'" + "10010001" + "'" + ","
                    //    + "'" + "" + "'" + "," + "'" + ClientPcName + "'" + "," + "'" + Mac + "'" + "," + "'" + IP + "'" + "," + "'" + EventTime + "'" + "," + "'" + "" + "'" + "," + "'" + "关机" + "'" + ")";
                    //    int r = SQL.ExecuteUpdate(s);
                    //    Log.WriteLog(string.Format("插入了{0}行OEE_DownTime补传关机缓存数据", r));
                    //}

                    DateTime T1 = Convert.ToDateTime(EventTime);
                    DateTime T2 = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    string TS = (T2 - T1).TotalMinutes.ToString("0.00");
                    string InsertOEEStr3 = "insert into OEE_DT([DateTime],[ErrorCode],[EventTime],[ModuleCode],[RunStatus],[ErrorInfo],[TimeSpan])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "'" + "," + "'" + "10010001" + "'" + "," + "'" + EventTime + "'" + ","
                                   + "'" + "" + "'" + "," + "'" + "6" + "'" + "," + "'" + "软件关闭" + "'" + "," + "'" + TS + "'" + ")";
                    SQL.ExecuteUpdate(InsertOEEStr3);
                    Log.WriteLog(DateTime.Now.ToString("HH:mm:ss") + "," + "10010001" + "," + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "," + "" + "," + "自动发送成功" + "," + "6" + "," + "软件关闭" + "," + TS, @"E:\装机软件\系统配置\System_ini\");
                }
                string DeleteOEEStr = "delete OEE_MCOff";
                SQL.ExecuteUpdate(DeleteOEEStr);//清空关机时间
            }
            catch (Exception ex)
            {
                Log.WriteLog(ex.ToString() + ",OEELog");
            }
            //-----------------------------------------------------------------------------------------------------

            while (true)
            {
                //ReadHandFeeding = Global.PLC_Client.ReadPLC_D(16010, 2);//人工上料开始、结束trg
                ReadTestRunStatus = Global.PLC_Client.ReadPLC_D(10030, 1);//空运行
                ReadStatus = Global.PLC_Client.ReadPLC_D(Address_errorCode, 6);//13005 待料中开启安全门或者暂停标志位
                ReadOpenDoorStatus = Global.PLC_Client.ReadPLC_D(10040, 1);//待料中开启安全门或者暂停标志位
                //MQTT上传时的数据索引
                string sendTopic = Global.inidata.productconfig.EMT + "/upload/downtime";

                if (ReadHandFeeding[0] == 1)
                {

                    Global.PLC_Client.WritePLC_D(16010, new short[] { 0 });
                    if (!Global.SelectFirstModel && !Global.SelectTestRunModel && !Global.Feeding && !Global.Error_PendingStatus && ReadStatus[0] == 1)//防止开机时已经传了一笔人工上料后再重复传一笔 且当前为非首件以及空跑状态
                    {
                        Global.ed[Global.Error_PendingNum + 1].stop_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                        if (Global.ed[Global.Error_PendingNum + 1].start_time != null)
                        {
                            DateTime t3 = Convert.ToDateTime(Global.ed[Global.Error_PendingNum + 1].start_time);
                            DateTime t2 = Convert.ToDateTime(Global.ed[Global.Error_PendingNum + 1].stop_time);
                            string ts = (t2 - t3).TotalMinutes.ToString("0.00");
                            string InsertStr = "insert into OEE_DT([DateTime],[ErrorCode],[EventTime],[ModuleCode],[RunStatus],[ErrorInfo],[TimeSpan])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "'" + "," + "'" + Global.ed[Global.Error_PendingNum + 1].errorCode + "'" + "," + "'" + Global.ed[Global.Error_PendingNum + 1].start_time + "'" + ","
                                + "'" + Global.ed[Global.Error_PendingNum + 1].ModuleCode + "'" + "," + "'" + Global.ed[Global.Error_PendingNum + 1].errorStatus + "'" + "," + "'" + Global.ed[Global.Error_PendingNum + 1].errorinfo + "'" + "," + "'" + ts + "'" + ")";
                            SQL.ExecuteUpdate(InsertStr);
                            Log.WriteLog(DateTime.Now.ToString("HH:mm:ss") + "," + Global.ed[Global.Error_PendingNum + 1].errorCode + "," + Global.ed[Global.Error_PendingNum + 1].start_time + "," + Global.ed[Global.Error_PendingNum + 1].ModuleCode + "," + "自动发送成功" + "," + Global.ed[Global.Error_PendingNum + 1].errorStatus + "," + Global.ed[Global.Error_PendingNum + 1].errorinfo + "," + ts, System.AppDomain.CurrentDomain.BaseDirectory + "\\系统配置\\System_ini\\");
                        }
                        Log.WriteLog(Global.ed[Global.Error_PendingNum + 1].Moduleinfo + "_" + Global.ed[Global.Error_PendingNum + 1].errorinfo + "：结束计时 " + Global.ed[Global.Error_PendingNum + 1].stop_time);
                        PendingStatus(278);
                        Global.Feeding = true;
                    }
                    Global.PLC_Client.WritePLC_D(16012, new short[] { 0 });
                }
                if (ReadHandFeeding[1] == 1)
                {
                    Global.PLC_Client.WritePLC_D(16011, new short[] { 0 });
                    if (!Global.SelectFirstModel && ReadTestRunStatus[0] != 1 && Global.Feeding && ReadOpenDoorStatus[0] != 1 && ReadStatus[0] == 1)//空跑、首件状态下不上传
                    {
                        Global.ed[Global.Error_PendingNum + 1].stop_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                        if (Global.ed[Global.Error_PendingNum + 1].start_time != null)
                        {
                            DateTime t3 = Convert.ToDateTime(Global.ed[Global.Error_PendingNum + 1].start_time);
                            DateTime t2 = Convert.ToDateTime(Global.ed[Global.Error_PendingNum + 1].stop_time);
                            string ts = (t2 - t3).TotalMinutes.ToString("0.00");
                            string InsertStr = "insert into OEE_DT([DateTime],[ErrorCode],[EventTime],[ModuleCode],[RunStatus],[ErrorInfo],[TimeSpan])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "'" + "," + "'" + Global.ed[Global.Error_PendingNum + 1].errorCode + "'" + "," + "'" + Global.ed[Global.Error_PendingNum + 1].start_time + "'" + ","
                                + "'" + Global.ed[Global.Error_PendingNum + 1].ModuleCode + "'" + "," + "'" + Global.ed[Global.Error_PendingNum + 1].errorStatus + "'" + "," + "'" + Global.ed[Global.Error_PendingNum + 1].errorinfo + "'" + "," + "'" + ts + "'" + ")";
                            SQL.ExecuteUpdate(InsertStr);
                            Log.WriteLog(DateTime.Now.ToString("HH:mm:ss") + "," + Global.ed[Global.Error_PendingNum + 1].errorCode + "," + Global.ed[Global.Error_PendingNum + 1].start_time + "," + Global.ed[Global.Error_PendingNum + 1].ModuleCode + "," + "自动发送成功" + "," + Global.ed[Global.Error_PendingNum + 1].errorStatus + "," + Global.ed[Global.Error_PendingNum + 1].errorinfo + "," + ts, System.AppDomain.CurrentDomain.BaseDirectory + "\\系统配置\\System_ini\\");
                            PendingStatus();
                        }
                    }
                    Global.Feeding = false;
                }
                //tsslbl_MachineStatus 标签 与 OEE_DT同步机台状态
                try
                {
                    if (ReadStatus[0] == 1 && ReadStatus[0] != Global.j)
                    {
                        tsslbl_MachineStatus.Text = "状态：设备待料中";
                        tsslbl_MachineStatus.BackColor = Color.DarkSeaGreen;
                    }
                    if (ReadStatus[0] == 2 && ReadStatus[0] != Global.j)
                    {
                        tsslbl_MachineStatus.Text = "状态：设备自动中";
                        tsslbl_MachineStatus.BackColor = Color.DarkSeaGreen;
                    }
                    if (ReadStatus[0] == 3 && ReadStatus[0] != Global.j)
                    {
                        tsslbl_MachineStatus.Text = "状态：设备宕机中";
                        tsslbl_MachineStatus.BackColor = Color.Red;
                    }
                    if (ReadStatus[0] == 4 && ReadStatus[0] != Global.j)
                    {
                        tsslbl_MachineStatus.Text = "状态：设备人工停止中";
                        tsslbl_MachineStatus.BackColor = Color.Red;
                    }
                }
                catch
                {
                }
                if (ReadStatus != null)
                {
                    try
                    {
                        if (!Global.SelectFirstModel)//当前是否属于手动(首件)状态
                        {
                            if (ReadTestRunStatus[0] != 1)//判断是否处于空跑状态（PLC屏蔽部分功能如：安全门，扫码枪，机械手）
                            {
                                if (Global.SelectTestRunModel == true && Global.ed[313].start_time != null)//空运行结束写入OEE_DT数据表中
                                {
                                    Global.ed[313].stop_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                                    DateTime t1 = Convert.ToDateTime(Global.ed[313].start_time);
                                    DateTime t2 = Convert.ToDateTime(Global.ed[313].stop_time);
                                    string ts = (t2 - t1).TotalMinutes.ToString("0.00");
                                    string InsertStr = "insert into OEE_DT([DateTime],[ErrorCode],[EventTime],[ModuleCode],[RunStatus],[ErrorInfo],[TimeSpan])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "'" + "," + "'" + Global.ed[313].errorCode + "'" + "," + "'" + Global.ed[313].start_time + "'" + ","
                                       + "'" + Global.ed[313].ModuleCode + "'" + "," + "'" + Global.ed[313].errorStatus + "'" + "," + "'" + Global.ed[313].errorinfo + "'" + "," + "'" + ts + "'" + ")";
                                    SQL.ExecuteUpdate(InsertStr);
                                    Log.WriteLog(DateTime.Now.ToString("HH:mm:ss") + "," + Global.ed[313].errorCode + "," + Global.ed[313].start_time + "," + Global.ed[313].ModuleCode + "," + "自动发送成功" + "," + Global.ed[313].errorStatus + "," + Global.ed[313].errorinfo + "," + ts, System.AppDomain.CurrentDomain.BaseDirectory + "\\系统配置\\System_ini\\");
                                    Global.ed[313].start_time = null;
                                    Global.ed[313].stop_time = null;
                                }
                                Global.SelectTestRunModel = false;
                                //当人工停止复位时间超过5分钟时，前五分钟为人工停止复位,后为手选原因
                                if (ReadStatus[0] == 4 && Global.ed[Global.Error_Stopnum + 1].start_time != null && Global.Art_stop && (Global.Error_Stopnum == 9 || Global.Error_Stopnum == 10 || Global.Error_Stopnum == 11 || Global.Error_Stopnum == 12 || Global.SelectManualErrorCode))
                                {
                                    if ((DateTime.Now - Convert.ToDateTime(Global.ed[Global.Error_Stopnum + 1].start_time)).Minutes >= 5)
                                    {
                                        Global.Art_stop = false;//防止重复上传人工停止复位
                                        string OEE_DT = "";
                                        string ts = "5.00";
                                        Guid guid = Guid.NewGuid();
                                        OEE_DT = string.Format("{{\"guid\":\"{0}\",\"EMT\":\"{1}\",\"PoorNum\":\"{2}\",\"TotalNum\":\"{3}\",  \"Status\":\"{4}\",\"ErrorCode\":\"{5}\",\"EventTime\":\"{6}\",\"ModuleCode\":\"{7}\",\"ClientPcName\":\"{8}\",\"MAC\":\"{9}\",\"IP\":\"{10}\"}}", guid, Global.inidata.productconfig.EMT, "0", "0", Global.ed[Global.Error_Stopnum + 1].errorStatus, Global.ed[Global.Error_Stopnum + 1].errorCode, Global.ed[Global.Error_Stopnum + 1].start_time, Global.ed[Global.Error_Stopnum + 1].ModuleCode, ClientPcName, Mac, IP);
                                        Log.WriteLog("OEE_DT暂停按钮停机超5分钟触发人工停止复位:" + OEE_DT + ",OEELog");
                                        //Global._mqttClient.Publish(sendTopic, Encoding.UTF8.GetBytes(OEE_DT), MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE, false);
                                        //bool rst = SendMqttResult(guid);
                                        //if (rst)
                                        //{
                                        //    if (Global.respond[guid].Result == "OK")
                                        //    {
                                        //        Global.ConnectOEEFlag = true;
                                        //        _homefrm.AppendRichText(Global.ed[Global.Error_Stopnum + 1].errorCode + ",触发时间=" + Global.ed[Global.Error_Stopnum + 1].start_time + ",运行状态:" + Global.ed[Global.Error_Stopnum + 1].errorStatus + ",故障描述:" + Global.ed[Global.Error_Stopnum + 1].errorinfo + ",自动发送成功", "rtx_DownTimeMsg");
                                        //        Log.WriteLog("OEE_DT安全门打开自动errorCode发送成功" + ",OEELog");
                                        //        Global.respond.TryRemove(guid, out Global.Respond);
                                        //    }
                                        //    else
                                        //    {
                                        //        Global.ConnectOEEFlag = false;
                                        //        Log.WriteLog("OEE_DT人工停止复位errorCode发送失败" + ",OEELog");
                                        //        _homefrm.AppendRichText(Global.ed[Global.Error_Stopnum + 1].errorCode + ",触发时间=" + Global.ed[Global.Error_Stopnum + 1].start_time + ",运行状态:" + Global.ed[Global.Error_Stopnum + 1].errorStatus + ",故障描述:" + Global.ed[Global.Error_Stopnum + 1].errorinfo + ",自动发送失败", "rtx_DownTimeMsg");
                                        //        _homefrm.AppendRichText("失败原因:" + Global.respond[guid].ErrorCode + Global.respond[guid].Result, "rtx_DownTimeMsg");
                                        //        Log.WriteLog(Global.respond[guid].GUID.ToString() + "," + Global.respond[guid].Result + "," + Global.respond[guid].ErrorCode, System.AppDomain.CurrentDomain.BaseDirectory + "\\MQTT返回NG\\");
                                        //        Global.respond.TryRemove(guid, out Global.Respond);
                                        //    }
                                        //}
                                        //else
                                        //{
                                        //    Global.ConnectOEEFlag = false;
                                        //    Log.WriteLog("OEE_DT人工停止复位自动errorCode发送失败,超时无反馈:" + ",OEELog");
                                        //    _homefrm.AppendRichText(Global.ed[Global.Error_Stopnum + 1].errorCode + ",触发时间=" + Global.ed[Global.Error_Stopnum + 1].start_time + ",运行状态:" + Global.ed[Global.Error_Stopnum + 1].errorStatus + ",故障描述:" + Global.ed[Global.Error_Stopnum + 1].errorinfo + ",人工停止复位自动发送失败", "rtx_DownTimeMsg");
                                        //    _homefrm.AppendRichText("网络异常,超时无反馈", "rtx_DownTimeMsg");
                                        //    string s = "insert into OEE_DTSendNG([DateTime],[Product],[GUID],[EMT],[PoorNum],[TotalNum],[Status],[ErrorCode],[ModuleCode],[ClientPcName],[MAC],[IP],[EventTime],[Moduleinfo],[errorinfo])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," + "'" + "OEE_DT" + "'" + "," + "'" + guid + "'" + "," + "'" + Global.inidata.productconfig.EMT + "'" + "," + "'" + "0" + "'" + "," + "'" + "0" + "'" + "," + "'" + Global.ed[Global.Error_Stopnum + 1].errorStatus + "'" + "," + "'" + Global.ed[Global.Error_Stopnum + 1].errorCode + "'" + ","
                                        //                           + "'" + Global.ed[Global.Error_Stopnum + 1].ModuleCode + "'" + "," + "'" + ClientPcName + "'" + "," + "'" + Mac + "'" + "," + "'" + IP + "'" + "," + "'" + Global.ed[Global.Error_Stopnum + 1].start_time + "'" + "," + "'" + Global.ed[Global.Error_Stopnum + 1].Moduleinfo + "'" + "," + "'" + Global.ed[Global.Error_Stopnum + 1].errorinfo + "'" + ")";
                                        //    int r = SQL.ExecuteUpdate(s);
                                        //    Log.WriteLog(string.Format("插入了{0}行OEE_DownTime缓存数据", r) + ",OEELog");
                                        //}
                                        _manualfrm.labelcolor(Color.Transparent, "未选择", "LB_ManualSelect");
                                        string InsertOEEStr = "insert into OEE_DT([DateTime],[ErrorCode],[EventTime],[ModuleCode],[RunStatus],[ErrorInfo],[TimeSpan])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "'" + "," + "'" + Global.ed[Global.Error_Stopnum + 1].errorCode + "'" + "," + "'" + Global.ed[Global.Error_Stopnum + 1].start_time + "'" + ","
                                       + "'" + "" + "'" + "," + "'" + Global.ed[Global.Error_Stopnum + 1].errorStatus + "'" + "," + "'" + Global.ed[Global.Error_Stopnum + 1].errorinfo + "'" + "," + "'" + ts + "'" + ")";
                                        SQL.ExecuteUpdate(InsertOEEStr);
                                        InsertOEEStr = "insert into OEE_TraceDT([DateTime],[Status],[ErrorCode],[EventTime],[ModuleCode])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," + "'" + Global.ed[Global.Error_Stopnum + 1].errorStatus + "'" + "," + "'" + Global.ed[Global.Error_Stopnum + 1].errorCode + "'" + ","
                                        + "'" + Global.ed[Global.Error_Stopnum + 1].start_time + "'" + "," + "'" + Global.ed[Global.Error_Stopnum + 1].ModuleCode + "'" + ")";
                                        SQL.ExecuteUpdate(InsertOEEStr);
                                        Log.WriteLog(DateTime.Now.ToString("HH:mm:ss") + "," + Global.ed[Global.Error_Stopnum + 1].errorCode + "," + Global.ed[Global.Error_Stopnum + 1].start_time + "," + "" + "," + "自动发送成功" + "," + Global.ed[Global.Error_Stopnum + 1].errorStatus + "," + Global.ed[Global.Error_Stopnum + 1].errorinfo + "," + ts, System.AppDomain.CurrentDomain.BaseDirectory + "\\系统配置\\System_ini\\");
                                        Global.ed[Global.Error_Stopnum + 1].start_time = Convert.ToDateTime(Global.ed[Global.Error_Stopnum + 1].start_time).AddMinutes(5).ToString("yyyy-MM-dd HH:mm:ss.fff");//人工停止复位中超5分钟 时这时的时间为手选原因时间
                                    }

                                }
                                if (ReadStatus[0] == 1 || ReadStatus[0] == 2 || ReadStatus[0] == 3 || ReadStatus[0] == 4)//j为机台运行大状态（-1初始值、1待料、2运行、3宕机、4人工停止）
                                {
                                    string EventTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                                    if (Global.j == -1)//判断是否初始化/结束首件状态
                                    {
                                        if (ReadStatus[0] == 4)//机台处于人工停止4状态
                                        {
                                            StopStatus();
                                        }
                                        else if (ReadStatus[0] == 3)//机台处于宕机3状态
                                        {
                                            ErrorStatus();
                                        }
                                        else if (ReadStatus[0] == 2)//机台处于运行2状态
                                        {
                                            RunStatus();
                                        }
                                        else if (ReadStatus[0] == 1)//处于待料1状态
                                        {
                                            PendingStatus();
                                        }
                                    }
                                    else//上一个状态与当前状态发生变动且上一个状态为非宕机状态时1、2
                                    {
                                        if (Global.STOP)//机台运行中人工停止
                                        {
                                            if (ReadStatus[3] == 9 || ReadStatus[3] == 10 || ReadStatus[3] == 11 || ReadStatus[3] == 12)//并且打开安全门
                                            {
                                                Global.STOP = false;
                                                Global.PLC_Client.WritePLC_D(10020, new short[] { 2 });//未手动选择打开安全门原因，机台不能运行
                                            }
                                        }

                                        if (ReadStatus[0] == 1)//判断当前状态为待料状态时1
                                        {
                                            if (ReadOpenDoorStatus[0] == 1)//40015 判断是否在待料中开安全门或者按下暂停按钮
                                            {
                                                Global.PLC_Client.WritePLC_D(10040, new short[] { 0 });
                                                Global.PLC_Client.WritePLC_D(10020, new short[] { 2 });//未手动选择打开安全门原因，机台不能运行
                                                Global.Error_PendingStatus = true;
                                                Global.Feeding = false;
                                                Global.ed[Global.Error_PendingNum + 1].stop_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                                                string c = "c=UPLOAD_DOWNTIME&tsn=Test_station&mn=Machine#1&start_time=" + Global.ed[Global.Error_PendingNum + 1].start_time + "&stop_time=" + Global.ed[Global.Error_PendingNum + 1].stop_time + "&ec=" + Global.ed[Global.Error_PendingNum + 1].errorCode;
                                                Log.WriteLog(c + ",OEELog");
                                                Log.WriteLog("待料时安全门打开" + ",OEELog");
                                                if (Global.ed[Global.Error_PendingNum + 1].start_time != null)
                                                {
                                                    DateTime t1 = Convert.ToDateTime(Global.ed[Global.Error_PendingNum + 1].start_time);
                                                    DateTime t2 = Convert.ToDateTime(Global.ed[Global.Error_PendingNum + 1].stop_time);
                                                    string ts = (t2 - t1).TotalMinutes.ToString("0.00");
                                                    string InsertStr = "insert into OEE_DT([DateTime],[ErrorCode],[EventTime],[ModuleCode],[RunStatus],[ErrorInfo],[TimeSpan])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "'" + "," + "'" + Global.ed[Global.Error_PendingNum + 1].errorCode + "'" + "," + "'" + Global.ed[Global.Error_PendingNum + 1].start_time + "'" + ","
                                                        + "'" + Global.ed[Global.Error_PendingNum + 1].ModuleCode + "'" + "," + "'" + Global.ed[Global.Error_PendingNum + 1].errorStatus + "'" + "," + "'" + Global.ed[Global.Error_PendingNum + 1].errorinfo + "'" + "," + "'" + ts + "'" + ")";
                                                    SQL.ExecuteUpdate(InsertStr);
                                                    Log.WriteLog(DateTime.Now.ToString("HH:mm:ss") + "," + Global.ed[Global.Error_PendingNum + 1].errorCode + "," + Global.ed[Global.Error_PendingNum + 1].start_time + "," + Global.ed[Global.Error_PendingNum + 1].ModuleCode + "," + "自动发送成功" + "," + Global.ed[Global.Error_PendingNum + 1].errorStatus + "," + Global.ed[Global.Error_PendingNum + 1].errorinfo + "," + ts, System.AppDomain.CurrentDomain.BaseDirectory + "\\系统配置\\System_ini\\");
                                                    _manualfrm.labelcolor(Color.Transparent, "未选择", "LB_ManualSelect");
                                                }
                                                if (Global.Feeding)//人工上料中开启安全门自动结束人工上料补传一笔HSG待料
                                                {
                                                    Global.SendHSG_start_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                                                    Global.Feeding = false;
                                                    SendHSG();
                                                    Global.SendHSG_start_time = null;
                                                }
                                                Log.WriteLog(Global.ed[Global.Error_PendingNum + 1].Moduleinfo + "_" + Global.ed[Global.Error_PendingNum + 1].errorinfo + "：结束计时 " + Global.ed[Global.Error_PendingNum + 1].stop_time);
                                                Global.ed[Global.Error_PendingNum + 1].start_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");//待料中开启安全门或者按下暂停后,待料结束,进入手动选择异常原因状态,待料结束时间为手动选择状态的开始时间
                                            }
                                        }
                                        if (Global.j != ReadStatus[0] && Global.j == 1)//上一个状态与当前状态发生变动且上一个状态为待料状态时1
                                        {
                                            if (Global.Error_PendingStatus)//判断待料时是否打开安全门/按下暂停键
                                            {
                                                Global.Error_PendingStatus = false;
                                                Global.Feeding = false;
                                                Global.ed[Global.Error_PendingNum + 1].stop_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                                                if (Global.ed[Global.Error_PendingNum + 1].start_time != null)
                                                {
                                                    DateTime t1 = Convert.ToDateTime(Global.ed[Global.Error_PendingNum + 1].start_time);
                                                    DateTime t2 = Convert.ToDateTime(Global.ed[Global.Error_PendingNum + 1].stop_time);
                                                    string ts = (t2 - t1).TotalMinutes.ToString("0.00");
                                                    string OEE_DT = "";
                                                    Guid guid = Guid.NewGuid();
                                                    OEE_DT = string.Format("{{\"guid\":\"{0}\",\"EMT\":\"{1}\",\"PoorNum\":\"{2}\",\"TotalNum\":\"{3}\",  \"Status\":\"{4}\",\"ErrorCode\":\"{5}\",\"EventTime\":\"{6}\",\"ModuleCode\":\"{7}\",\"ClientPcName\":\"{8}\",\"MAC\":\"{9}\",\"IP\":\"{10}\"}}", guid, Global.inidata.productconfig.EMT, "0", "0", Global.errorStatus, Global.errorcode, Global.ed[Global.Error_PendingNum + 1].start_time, "", ClientPcName, Mac, IP);
                                                    Log.WriteLog("OEE_DT待料中安全门打开:" + OEE_DT + ",OEELog");
                                                    //Global._mqttClient.Publish(sendTopic, Encoding.UTF8.GetBytes(OEE_DT), MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE, false);
                                                    //bool rst = SendMqttResult(guid);
                                                    //if (rst)
                                                    //{
                                                    //    if (Global.respond[guid].Result == "OK")
                                                    //    {
                                                    //        Global.ConnectOEEFlag = true;
                                                    //        Log.WriteLog("OEE_DT安全门打开自动errorCode发送成功" + ",OEELog");
                                                    //        _homefrm.AppendRichText(Global.errorcode + ",触发时间=" + Global.ed[Global.Error_PendingNum + 1].start_time + ",运行状态:" + Global.errorStatus + ",故障描述:" + Global.errorinfo + ",安全门打开自动发送成功", "rtx_DownTimeMsg");
                                                    //        Global.respond.TryRemove(guid, out Global.Respond);
                                                    //    }
                                                    //    else
                                                    //    {
                                                    //        Global.ConnectOEEFlag = false;
                                                    //        _homefrm.AppendRichText(Global.errorcode + ",触发时间=" + Global.ed[Global.Error_PendingNum + 1].start_time + ",运行状态:" + Global.errorStatus + ",故障描述:" + Global.errorinfo + ",自动发送失败", "rtx_DownTimeMsg");
                                                    //        _homefrm.AppendRichText("失败原因:" + Global.respond[guid].ErrorCode + Global.respond[guid].Result, "rtx_DownTimeMsg");
                                                    //        Log.WriteLog("OEE_DT安全门打开自动errorCode发送失败" + ",OEELog");
                                                    //        Log.WriteLog(Global.respond[guid].GUID.ToString() + "," + Global.respond[guid].Result + "," + Global.respond[guid].ErrorCode, System.AppDomain.CurrentDomain.BaseDirectory + "\\MQTT返回NG\\");
                                                    //        Global.respond.TryRemove(guid, out Global.Respond);
                                                    //    }
                                                    //}
                                                    //else
                                                    //{
                                                    //    Global.ConnectOEEFlag = false;
                                                    //    Log.WriteLog("OEE_DT安全门打开自动errorCode发送失败,超时无反馈:" + ",OEELog");
                                                    //    _homefrm.AppendRichText(Global.errorcode + ",触发时间=" + Global.ed[Global.Error_PendingNum + 1].start_time + ",运行状态:" + Global.errorStatus + ",故障描述:" + Global.errorinfo + ",安全门打开自动发送失败", "rtx_DownTimeMsg");
                                                    //    _homefrm.AppendRichText("网络异常,超时无反馈", "rtx_DownTimeMsg");
                                                    //    string s = "insert into OEE_DTSendNG([DateTime],[Product],[GUID],[EMT],[PoorNum],[TotalNum],[Status],[ErrorCode],[ModuleCode],[ClientPcName],[MAC],[IP],[EventTime],[Moduleinfo],[errorinfo])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," + "'" + "OEE_DT" + "'" + "," + "'" + guid + "'" + "," + "'" + Global.inidata.productconfig.EMT + "'" + "," + "'" + "0" + "'" + "," + "'" + "0" + "'" + "," + "'" + Global.errorStatus + "'" + "," + "'" + Global.errorcode + "'" + ","
                                                    //           + "'" + "" + "'" + "," + "'" + ClientPcName + "'" + "," + "'" + Mac + "'" + "," + "'" + IP + "'" + "," + "'" + Global.ed[Global.Error_PendingNum + 1].start_time + "'" + "," + "'" + "" + "'" + "," + "'" + Global.errorinfo + "'" + ")";
                                                    //    int r = SQL.ExecuteUpdate(s);
                                                    //    Log.WriteLog(string.Format("插入了{0}行OEE_DownTime缓存数据", r) + ",OEELog");
                                                    //}
                                                    _manualfrm.labelcolor(Color.Transparent, "未选择", "LB_ManualSelect");
                                                    string InsertOEEStr = "insert into OEE_DT([DateTime],[ErrorCode],[EventTime],[ModuleCode],[RunStatus],[ErrorInfo],[TimeSpan])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "'" + "," + "'" + Global.errorcode + "'" + "," + "'" + Global.ed[Global.Error_PendingNum + 1].start_time + "'" + ","
                                                   + "'" + "" + "'" + "," + "'" + Global.errorStatus + "'" + "," + "'" + Global.errorinfo + "'" + "," + "'" + ts + "'" + ")";
                                                    SQL.ExecuteUpdate(InsertOEEStr);
                                                    InsertOEEStr = "insert into OEE_TraceDT([DateTime],[Status],[ErrorCode],[EventTime],[ModuleCode])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," + "'" + Global.errorStatus + "'" + "," + "'" + Global.errorcode + "'" + ","
                                                    + "'" + Global.ed[Global.Error_PendingNum + 1].start_time + "'" + "," + "'" + Global.ed[Global.Error_PendingNum + 1].ModuleCode + "'" + ")";
                                                    SQL.ExecuteUpdate(InsertOEEStr);
                                                    Log.WriteLog(DateTime.Now.ToString("HH:mm:ss") + "," + Global.errorcode + "," + Global.ed[Global.Error_PendingNum + 1].start_time + "," + "" + "," + "自动发送成功" + "," + Global.errorStatus + "," + Global.errorinfo + "," + ts, System.AppDomain.CurrentDomain.BaseDirectory + "\\系统配置\\System_ini\\");
                                                    Log.WriteLog("" + "_" + Global.errorinfo + "：结束计时 " + Global.ed[Global.Error_PendingNum + 1].stop_time);
                                                }
                                            }
                                            else
                                            {
                                                Global.ed[Global.Error_PendingNum + 1].stop_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                                                _manualfrm.labelcolor(Color.Transparent, "未选择", "LB_ManualSelect");
                                                string c = "c=UPLOAD_DOWNTIME&tsn=Test_station&mn=Machine#1&start_time=" + Global.ed[Global.Error_PendingNum + 1].start_time + "&stop_time=" + Global.ed[Global.Error_PendingNum + 1].stop_time + "&ec=" + Global.ed[Global.Error_PendingNum + 1].errorCode;
                                                Log.WriteLog(c + ",OEELog");
                                                if (Global.ed[Global.Error_PendingNum + 1].start_time != null)
                                                {
                                                    DateTime t1 = Convert.ToDateTime(Global.ed[Global.Error_PendingNum + 1].start_time);
                                                    DateTime t2 = Convert.ToDateTime(Global.ed[Global.Error_PendingNum + 1].stop_time);
                                                    string ts = (t2 - t1).TotalMinutes.ToString("0.00");
                                                    string InsertStr = "insert into OEE_DT([DateTime],[ErrorCode],[EventTime],[ModuleCode],[RunStatus],[ErrorInfo],[TimeSpan])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "'" + "," + "'" + Global.ed[Global.Error_PendingNum + 1].errorCode + "'" + "," + "'" + Global.ed[Global.Error_PendingNum + 1].start_time + "'" + ","
                                                       + "'" + Global.ed[Global.Error_PendingNum + 1].ModuleCode + "'" + "," + "'" + Global.ed[Global.Error_PendingNum + 1].errorStatus + "'" + "," + "'" + Global.ed[Global.Error_PendingNum + 1].errorinfo + "'" + "," + "'" + ts + "'" + ")";
                                                    SQL.ExecuteUpdate(InsertStr);
                                                    Log.WriteLog(DateTime.Now.ToString("HH:mm:ss") + "," + Global.ed[Global.Error_PendingNum + 1].errorCode + "," + Global.ed[Global.Error_PendingNum + 1].start_time + "," + Global.ed[Global.Error_PendingNum + 1].ModuleCode + "," + "自动发送成功" + "," + Global.ed[Global.Error_PendingNum + 1].errorStatus + "," + Global.ed[Global.Error_PendingNum + 1].errorinfo + "," + ts, System.AppDomain.CurrentDomain.BaseDirectory + "\\系统配置\\System_ini\\");
                                                    _manualfrm.labelcolor(Color.Transparent, "未选择", "LB_ManualSelect");
                                                }
                                                Log.WriteLog(Global.ed[Global.Error_PendingNum + 1].Moduleinfo + "_" + Global.ed[Global.Error_PendingNum + 1].errorinfo + "：结束计时 " + Global.ed[Global.Error_PendingNum + 1].stop_time + ",OEELog");
                                                if (Global.Feeding)//人工上料中机台启动自动结束人工上料补传一笔HSG待料
                                                {
                                                    Global.SendHSG_start_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                                                    Global.Feeding = false;
                                                    SendHSG();
                                                    Global.SendHSG_start_time = null;
                                                }
                                            }
                                            Global.ed[Global.Error_PendingNum + 1].start_time = null;
                                            Global.ed[Global.Error_PendingNum + 1].stop_time = null;
                                            Global.j = ReadStatus[0];
                                            //-------------上一个状态结束，当前状态开始计时--------------
                                            if (ReadStatus[0] == 4)//机台处于人工停止4状态
                                            {
                                                StopStatus();
                                            }
                                            else if (ReadStatus[0] == 3)//机台处于宕机3状态
                                            {
                                                ErrorStatus();
                                            }
                                            else if (ReadStatus[0] == 2)//机台处于运行2状态
                                            {
                                                RunStatus();
                                            }
                                            else if (ReadStatus[0] == 1)//机台处于待机待料1状态
                                            {
                                                PendingStatus();
                                            }
                                        }
                                        else if (Global.j != ReadStatus[0] && Global.j == 2)//上一个状态与当前状态发生变动且上一个状态为运行状态时2
                                        {
                                            Global.ed[Global.j].stop_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                                            if (Global.ed[Global.j].start_time != null)
                                            {
                                                DateTime t1 = Convert.ToDateTime(Global.ed[Global.j].start_time);
                                                DateTime t2 = Convert.ToDateTime(Global.ed[Global.j].stop_time);
                                                string ts = (t2 - t1).TotalMinutes.ToString("0.00");
                                                string InsertStr = "insert into OEE_DT([DateTime],[ErrorCode],[EventTime],[ModuleCode],[RunStatus],[ErrorInfo],[TimeSpan])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "'" + "," + "'" + Global.ed[Global.j].errorCode + "'" + "," + "'" + Global.ed[Global.j].start_time + "'" + ","
                                                   + "'" + Global.ed[Global.j].ModuleCode + "'" + "," + "'" + Global.ed[Global.j].errorStatus + "'" + "," + "'" + Global.ed[Global.j].errorinfo + "'" + "," + "'" + ts + "'" + ")";
                                                SQL.ExecuteUpdate(InsertStr);
                                                Log.WriteLog(DateTime.Now.ToString("HH:mm:ss") + "," + Global.ed[Global.j].errorCode + "," + Global.ed[Global.j].start_time + "," + Global.ed[Global.j].ModuleCode + "," + "自动发送成功" + "," + Global.ed[Global.j].errorStatus + "," + Global.ed[Global.j].errorinfo + "," + ts, System.AppDomain.CurrentDomain.BaseDirectory + "\\系统配置\\System_ini\\");
                                                _manualfrm.labelcolor(Color.Transparent, "未选择", "LB_ManualSelect");
                                            }
                                            Log.WriteLog(Global.ed[Global.j].Moduleinfo + "_" + Global.ed[Global.j].errorinfo + "：结束计时 " + Global.ed[Global.j].stop_time);
                                            Global.ed[Global.j].start_time = null;
                                            Global.ed[Global.j].stop_time = null;
                                            Global.j = ReadStatus[0];
                                            //-------------上一个状态结束，当前状态开始计时--------------
                                            if (ReadStatus[0] == 4)//机台处于人工停止4状态
                                            {
                                                StopStatus();
                                            }
                                            else if (ReadStatus[0] == 3)//机台处于宕机3状态
                                            {
                                                ErrorStatus();
                                            }
                                            else if (ReadStatus[0] == 2)//机台处于运行2状态
                                            {
                                                RunStatus();
                                            }
                                            else if (ReadStatus[0] == 1)//机台处于待机待料1状态
                                            {
                                                PendingStatus();
                                            }
                                        }
                                        else if (Global.j != ReadStatus[0] && Global.j == 3)//上一个状态与当前状态发生变动且上一个状态为宕机状态3时
                                        {
                                            Global.ed[Global.Error_num + 1].stop_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                                            string c = "c=UPLOAD_DOWNTIME&tsn=Test_station&mn=Machine#1&start_time=" + Global.ed[Global.Error_num + 1].start_time + "&stop_time=" + Global.ed[Global.Error_num + 1].stop_time + "&ec=" + Global.ed[Global.Error_num + 1].errorCode;
                                            Log.WriteLog(c + ",OEELog");
                                            if (Global.ed[Global.Error_num + 1].start_time != null)
                                            {
                                                DateTime t1 = Convert.ToDateTime(Global.ed[Global.Error_num + 1].start_time);
                                                DateTime t2 = Convert.ToDateTime(Global.ed[Global.Error_num + 1].stop_time);
                                                string ts = (t2 - t1).TotalMinutes.ToString("0.00");
                                                if (Global.Error_num == 9 || Global.Error_num == 10 || Global.Error_num == 11 || Global.Error_num == 12 || Global.Error_num == 53)//机台打开安全门
                                                {
                                                    string OEE_DT = "";
                                                    Guid guid = Guid.NewGuid();
                                                    OEE_DT = string.Format("{{\"guid\":\"{0}\",\"EMT\":\"{1}\",\"PoorNum\":\"{2}\",\"TotalNum\":\"{3}\",  \"Status\":\"{4}\",\"ErrorCode\":\"{5}\",\"EventTime\":\"{6}\",\"ModuleCode\":\"{7}\",\"ClientPcName\":\"{8}\",\"MAC\":\"{9}\",\"IP\":\"{10}\"}}", guid, Global.inidata.productconfig.EMT, "0", "0", Global.errorStatus, Global.errorcode, Global.ed[Global.Error_num + 1].start_time, "", ClientPcName, Mac, IP);
                                                    Log.WriteLog("OEE_DT宕机中安全门打开:" + OEE_DT + ",OEELog");
                                                    //Global._mqttClient.Publish(sendTopic, Encoding.UTF8.GetBytes(OEE_DT), MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE, false);
                                                    //bool rst = SendMqttResult(guid);
                                                    //if (rst)
                                                    //{
                                                    //    if (Global.respond[guid].Result == "OK")
                                                    //    {
                                                    //        Global.ConnectOEEFlag = true;
                                                    //        Log.WriteLog("OEE_DT安全门打开自动errorCode发送成功" + ",OEELog");
                                                    //        _homefrm.AppendRichText(Global.errorcode + ",触发时间=" + Global.ed[Global.Error_num + 1].start_time + ",运行状态:" + Global.errorStatus + ",故障描述:" + Global.errorinfo + ",安全门打开自动发送成功", "rtx_DownTimeMsg");
                                                    //        Global.respond.TryRemove(guid, out Global.Respond);
                                                    //    }
                                                    //    else
                                                    //    {
                                                    //        Global.ConnectOEEFlag = false;
                                                    //        Log.WriteLog("OEE_DT安全门打开自动errorCode发送失败" + ",OEELog");
                                                    //        Log.WriteLog(Global.respond[guid].GUID.ToString() + "," + Global.respond[guid].Result + "," + Global.respond[guid].ErrorCode, System.AppDomain.CurrentDomain.BaseDirectory + "\\MQTT返回NG\\");
                                                    //        _homefrm.AppendRichText(Global.errorcode + ",触发时间=" + Global.ed[Global.Error_num + 1].start_time + ",运行状态:" + Global.errorStatus + ",故障描述:" + Global.errorinfo + ",安全门打开自动发送失败", "rtx_DownTimeMsg");
                                                    //        _homefrm.AppendRichText("失败原因:" + Global.respond[guid].ErrorCode + Global.respond[guid].Result, "rtx_DownTimeMsg");
                                                    //        Global.respond.TryRemove(guid, out Global.Respond);
                                                    //    }
                                                    //}
                                                    //else
                                                    //{
                                                    //    Global.ConnectOEEFlag = false;
                                                    //    Log.WriteLog("OEE_DT安全门打开自动errorCode发送失败,超时无反馈:" + ",OEELog");
                                                    //    _homefrm.AppendRichText(Global.errorcode + ",触发时间=" + Global.ed[Global.Error_num + 1].start_time + ",运行状态:" + Global.errorStatus + ",故障描述:" + Global.errorinfo + ",安全门打开自动发送失败", "rtx_DownTimeMsg");
                                                    //    _homefrm.AppendRichText("网络异常,超时无反馈", "rtx_DownTimeMsg");
                                                    //    string s = "insert into OEE_DTSendNG([DateTime],[Product],[GUID],[EMT],[PoorNum],[TotalNum],[Status],[ErrorCode],[ModuleCode],[ClientPcName],[MAC],[IP],[EventTime],[Moduleinfo],[errorinfo])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," + "'" + "OEE_DT" + "'" + "," + "'" + guid + "'" + "," + "'" + Global.inidata.productconfig.EMT + "'" + "," + "'" + "0" + "'" + "," + "'" + "0" + "'" + "," + "'" + Global.errorStatus + "'" + "," + "'" + Global.errorcode + "'" + ","
                                                    //           + "'" + "" + "'" + "," + "'" + ClientPcName + "'" + "," + "'" + Mac + "'" + "," + "'" + IP + "'" + "," + "'" + Global.ed[Global.Error_num + 1].start_time + "'" + "," + "'" + "" + "'" + "," + "'" + Global.errorinfo + "'" + ")";
                                                    //    int r = SQL.ExecuteUpdate(s);
                                                    //    Log.WriteLog(string.Format("插入了{0}行OEE_DownTime缓存数据", r) + ",OEELog");
                                                    //}
                                                    _manualfrm.labelcolor(Color.Transparent, "未选择", "LB_ManualSelect");
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
                                                    Log.WriteLog(DateTime.Now.ToString("HH:mm:ss") + "," + Global.ed[Global.Error_num + 1].errorCode + "," + Global.ed[Global.Error_num + 1].start_time + "," + Global.ed[Global.Error_num + 1].ModuleCode + "," + "自动发送成功" + "," + Global.ed[Global.Error_num + 1].errorStatus + "," + Global.ed[Global.Error_num + 1].errorinfo + "," + ts, System.AppDomain.CurrentDomain.BaseDirectory + "\\系统配置\\System_ini\\");
                                                }
                                            }
                                            Log.WriteLog(Global.ed[Global.Error_num + 1].Moduleinfo + "_" + Global.ed[Global.Error_num + 1].errorinfo + "：结束计时 " + Global.ed[Global.Error_num + 1].stop_time + ",OEELog");
                                            Global.ed[Global.Error_num + 1].start_time = null;
                                            Global.ed[Global.Error_num + 1].stop_time = null;
                                            Global.j = ReadStatus[0];
                                            //-------------上一个状态结束，当前状态开始计时--------------
                                            if (ReadStatus[0] == 4)//机台处于人工停止4状态
                                            {
                                                StopStatus();
                                            }
                                            else if (ReadStatus[0] == 3)//机台处于宕机3状态
                                            {
                                                ErrorStatus();
                                            }
                                            else if (ReadStatus[0] == 2)//机台处于运行2状态
                                            {
                                                RunStatus();
                                            }
                                            else if (ReadStatus[0] == 1)//机台处于待机待料1状态
                                            {
                                                PendingStatus();
                                            }
                                        }
                                        else if (Global.j != ReadStatus[0] && Global.j == 4)//上一个状态与当前状态发生变动且上一个状态为人工停止状态4时
                                        {
                                            Global.ed[Global.Error_Stopnum + 1].stop_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                                            string c = "c=UPLOAD_DOWNTIME&tsn=Test_station&mn=Machine#1&start_time=" + Global.ed[Global.Error_Stopnum + 1].start_time + "&stop_time=" + Global.ed[Global.Error_Stopnum + 1].stop_time + "&ec=" + Global.ed[Global.Error_Stopnum + 1].errorCode;
                                            Log.WriteLog(c + ",OEELog");
                                            if (Global.ed[Global.Error_Stopnum + 1].start_time != null)
                                            {
                                                DateTime t1 = Convert.ToDateTime(Global.ed[Global.Error_Stopnum + 1].start_time);
                                                DateTime t2 = Convert.ToDateTime(Global.ed[Global.Error_Stopnum + 1].stop_time);
                                                string ts = (t2 - t1).TotalMinutes.ToString("0.00");
                                                if (ReadStatus[3] == 9 || ReadStatus[3] == 10 || ReadStatus[3] == 11 || ReadStatus[3] == 12 || ReadStatus[3] == 53 || Global.SelectManualErrorCode)//机台打开安全门或者手动选择ErrorCode状态开启
                                                {
                                                    string OEE_DT = "";
                                                    Global.Art_stop = true;
                                                    Guid guid = Guid.NewGuid();
                                                    OEE_DT = string.Format("{{\"guid\":\"{0}\",\"EMT\":\"{1}\",\"PoorNum\":\"{2}\",\"TotalNum\":\"{3}\",  \"Status\":\"{4}\",\"ErrorCode\":\"{5}\",\"EventTime\":\"{6}\",\"ModuleCode\":\"{7}\",\"ClientPcName\":\"{8}\",\"MAC\":\"{9}\",\"IP\":\"{10}\"}}", guid, Global.inidata.productconfig.EMT, "0", "0", Global.errorStatus, Global.errorcode, Global.ed[Global.Error_Stopnum + 1].start_time, "", ClientPcName, Mac, IP);
                                                    Log.WriteLog("OEE_DT人工停止中安全门打开:" + OEE_DT + ",OEELog");
                                                    //Global._mqttClient.Publish(sendTopic, Encoding.UTF8.GetBytes(OEE_DT), MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE, false);
                                                    //bool rst = SendMqttResult(guid);
                                                    //if (rst)
                                                    //{
                                                    //    if (Global.respond[guid].Result == "OK")
                                                    //    {
                                                    //        Global.ConnectOEEFlag = true;
                                                    //        Log.WriteLog("OEE_DT安全门打开自动errorCode发送成功" + ",OEELog");
                                                    //        _homefrm.AppendRichText(Global.errorcode + ",触发时间=" + Global.ed[Global.Error_Stopnum + 1].start_time + ",运行状态:" + Global.errorStatus + ",故障描述:" + Global.errorinfo + ",安全门打开自动发送成功", "rtx_DownTimeMsg");
                                                    //        Global.respond.TryRemove(guid, out Global.Respond);
                                                    //    }
                                                    //    else
                                                    //    {
                                                    //        Global.ConnectOEEFlag = false;
                                                    //        Log.WriteLog("OEE_DT安全门打开自动errorCode发送失败" + ",OEELog");
                                                    //        Log.WriteLog(Global.respond[guid].GUID.ToString() + "," + Global.respond[guid].Result + "," + Global.respond[guid].ErrorCode, System.AppDomain.CurrentDomain.BaseDirectory + "\\MQTT返回NG\\");
                                                    //        _homefrm.AppendRichText(Global.errorcode + ",触发时间=" + Global.ed[Global.Error_Stopnum + 1].start_time + ",运行状态:" + Global.errorStatus + ",故障描述:" + Global.errorinfo + ",安全门打开自动发送失败", "rtx_DownTimeMsg");
                                                    //        _homefrm.AppendRichText("失败原因:" + Global.respond[guid].ErrorCode + Global.respond[guid].Result, "rtx_DownTimeMsg");
                                                    //        Global.respond.TryRemove(guid, out Global.Respond);
                                                    //    }
                                                    //}
                                                    //else
                                                    //{
                                                    //    Global.ConnectOEEFlag = false;
                                                    //    Log.WriteLog("OEE_DT人工停止复位自动errorCode发送失败,超时无反馈:" + ",OEELog");
                                                    //    _homefrm.AppendRichText(Global.errorcode + ",触发时间=" + Global.ed[Global.Error_Stopnum + 1].start_time + ",运行状态:" + Global.errorStatus + ",故障描述:" + Global.errorinfo + ",安全门打开自动发送失败", "rtx_DownTimeMsg");
                                                    //    _homefrm.AppendRichText("网络异常,超时无反馈", "rtx_DownTimeMsg");
                                                    //    string s = "insert into OEE_DTSendNG([DateTime],[Product],[GUID],[EMT],[PoorNum],[TotalNum],[Status],[ErrorCode],[ModuleCode],[ClientPcName],[MAC],[IP],[EventTime],[Moduleinfo],[errorinfo])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," + "'" + "OEE_DT" + "'" + "," + "'" + guid + "'" + "," + "'" + Global.inidata.productconfig.EMT + "'" + "," + "'" + "0" + "'" + "," + "'" + "0" + "'" + "," + "'" + Global.errorStatus + "'" + "," + "'" + Global.errorcode + "'" + ","
                                                    //           + "'" + "" + "'" + "," + "'" + ClientPcName + "'" + "," + "'" + Mac + "'" + "," + "'" + IP + "'" + "," + "'" + Global.ed[Global.Error_Stopnum + 1].start_time + "'" + "," + "'" + "" + "'" + "," + "'" + Global.errorinfo + "'" + ")";
                                                    //    int r = SQL.ExecuteUpdate(s);
                                                    //    Log.WriteLog(string.Format("插入了{0}行OEE_DownTime缓存数据", r) + ",OEELog");
                                                    //}
                                                    _manualfrm.labelcolor(Color.Transparent, "未选择", "LB_ManualSelect");
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
                                                    string OEE_DT = "";
                                                    Guid guid = Guid.NewGuid();
                                                    OEE_DT = string.Format("{{\"guid\":\"{0}\",\"EMT\":\"{1}\",\"PoorNum\":\"{2}\",\"TotalNum\":\"{3}\",  \"Status\":\"{4}\",\"ErrorCode\":\"{5}\",\"EventTime\":\"{6}\",\"ModuleCode\":\"{7}\",\"ClientPcName\":\"{8}\",\"MAC\":\"{9}\",\"IP\":\"{10}\"}}", guid, Global.inidata.productconfig.EMT, "0", "0", Global.ed[Global.Error_Stopnum + 1].errorStatus, Global.ed[Global.Error_Stopnum + 1].errorCode, Global.ed[Global.Error_Stopnum + 1].start_time, Global.ed[Global.Error_Stopnum + 1].ModuleCode, ClientPcName, Mac, IP);
                                                    Log.WriteLog("OEE_DT人工停止复位:" + OEE_DT + ",OEELog");
                                                    //Global._mqttClient.Publish(sendTopic, Encoding.UTF8.GetBytes(OEE_DT), MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE, false);
                                                    //bool rst = SendMqttResult(guid);
                                                    //if (rst)
                                                    //{
                                                    //    if (Global.respond[guid].Result == "OK")
                                                    //    {
                                                    //        Global.ConnectOEEFlag = true;
                                                    //        Log.WriteLog("OEE_DT人工停止复位自动errorCode发送成功" + ",OEELog");
                                                    //        _homefrm.AppendRichText(Global.ed[Global.Error_Stopnum + 1].errorCode + ",触发时间=" + Global.ed[Global.Error_Stopnum + 1].start_time + ",运行状态:" + Global.ed[Global.Error_Stopnum + 1].errorStatus + ",故障描述:" + Global.ed[Global.Error_Stopnum + 1].errorinfo + ",自动发送成功", "rtx_DownTimeMsg");
                                                    //        Global.respond.TryRemove(guid, out Global.Respond);
                                                    //    }
                                                    //    else
                                                    //    {
                                                    //        Global.ConnectOEEFlag = false;
                                                    //        Log.WriteLog("OEE_DT人工停止复位自动errorCode发送失败" + ",OEELog");
                                                    //        Log.WriteLog(Global.respond[guid].GUID.ToString() + "," + Global.respond[guid].Result + "," + Global.respond[guid].ErrorCode, System.AppDomain.CurrentDomain.BaseDirectory + "\\MQTT返回NG\\");
                                                    //        _homefrm.AppendRichText(Global.ed[Global.Error_Stopnum + 1].errorCode + ",触发时间=" + Global.ed[Global.Error_Stopnum + 1].start_time + ",运行状态:" + Global.ed[Global.Error_Stopnum + 1].errorStatus + ",故障描述:" + Global.ed[Global.Error_Stopnum + 1].errorinfo + ",自动发送失败", "rtx_DownTimeMsg");
                                                    //        _homefrm.AppendRichText("失败原因:" + Global.respond[guid].ErrorCode + Global.respond[guid].Result, "rtx_DownTimeMsg");
                                                    //        Global.respond.TryRemove(guid, out Global.Respond);
                                                    //    }
                                                    //}
                                                    //else
                                                    //{
                                                    //    Global.ConnectOEEFlag = false;
                                                    //    Log.WriteLog("OEE_DT人工停止复位自动errorCode发送失败,超时无反馈:" + ",OEELog");
                                                    //    _homefrm.AppendRichText(Global.ed[Global.Error_Stopnum + 1].errorCode + ",触发时间=" + Global.ed[Global.Error_Stopnum + 1].start_time + ",运行状态:" + Global.ed[Global.Error_Stopnum + 1].errorStatus + ",故障描述:" + Global.ed[Global.Error_Stopnum + 1].errorinfo + ",自动发送失败", "rtx_DownTimeMsg");
                                                    //    _homefrm.AppendRichText("网络异常,超时无反馈", "rtx_DownTimeMsg");
                                                    //    string s = "insert into OEE_DTSendNG([DateTime],[Product],[GUID],[EMT],[PoorNum],[TotalNum],[Status],[ErrorCode],[ModuleCode],[ClientPcName],[MAC],[IP],[EventTime],[Moduleinfo],[errorinfo])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," + "'" + "OEE_DT" + "'" + "," + "'" + guid + "'" + "," + "'" + Global.inidata.productconfig.EMT + "'" + "," + "'" + "0" + "'" + "," + "'" + "0" + "'" + "," + "'" + Global.ed[Global.Error_Stopnum + 1].errorStatus + "'" + "," + "'" + Global.ed[Global.Error_Stopnum + 1].errorCode + "'" + ","
                                                    //           + "'" + Global.ed[Global.Error_Stopnum + 1].ModuleCode + "'" + "," + "'" + ClientPcName + "'" + "," + "'" + Mac + "'" + "," + "'" + IP + "'" + "," + "'" + Global.ed[Global.Error_Stopnum + 1].start_time + "'" + "," + "'" + Global.ed[Global.Error_Stopnum + 1].Moduleinfo + "'" + "," + "'" + Global.ed[Global.Error_Stopnum + 1].errorinfo + "'" + ")";
                                                    //    int r = SQL.ExecuteUpdate(s);
                                                    //    Log.WriteLog(string.Format("插入了{0}行OEE_DownTime缓存数据", r) + ",OEELog");
                                                    //}
                                                    _manualfrm.labelcolor(Color.Transparent, "未选择", "LB_ManualSelect");
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
                                            Global.ed[Global.Error_Stopnum + 1].start_time = null;
                                            Global.ed[Global.Error_Stopnum + 1].stop_time = null;
                                            Global.j = ReadStatus[0];
                                            //-------------上一个状态结束，当前状态开始计时--------------
                                            if (ReadStatus[0] == 4)//机台处于人工停止4状态
                                            {
                                                StopStatus();
                                            }
                                            else if (ReadStatus[0] == 3)//机台处于宕机3状态
                                            {
                                                ErrorStatus();
                                            }
                                            else if (ReadStatus[0] == 2)//机台处于运行2状态
                                            {
                                                RunStatus();
                                            }
                                            else if (ReadStatus[0] == 1)//机台处于待机待料1状态
                                            {
                                                PendingStatus();
                                            }
                                        }
                                        else
                                        { }
                                    }
                                }
                                else
                                {
                                }
                            }
                            else//处于空跑(PLC屏蔽部分功能)状态
                            {
                                if (Global.SelectTestRunModel == false)
                                {
                                    Global.SelectTestRunModel = true;
                                    var IP = GetIp();
                                    var Mac = GetOEEMac();
                                    if (Global.j == 1)//处于待料状态
                                    {
                                        if (Global.Error_PendingStatus)//判断待料时是否打开安全门/按下暂停键
                                        {
                                            Global.Error_PendingStatus = false;
                                            Global.Feeding = false;
                                            Global.ed[Global.Error_PendingNum + 1].stop_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                                            if (Global.ed[Global.Error_PendingNum + 1].start_time != null)
                                            {
                                                DateTime t1 = Convert.ToDateTime(Global.ed[Global.Error_PendingNum + 1].start_time);
                                                DateTime t2 = Convert.ToDateTime(Global.ed[Global.Error_PendingNum + 1].stop_time);
                                                string ts = (t2 - t1).TotalMinutes.ToString("0.00");
                                                string OEE_DT1 = "";
                                                Guid guid1 = Guid.NewGuid();
                                                OEE_DT1 = string.Format("{{\"guid\":\"{0}\",\"EMT\":\"{1}\",\"PoorNum\":\"{2}\",\"TotalNum\":\"{3}\",  \"Status\":\"{4}\",\"ErrorCode\":\"{5}\",\"EventTime\":\"{6}\",\"ModuleCode\":\"{7}\",\"ClientPcName\":\"{8}\",\"MAC\":\"{9}\",\"IP\":\"{10}\"}}", guid1, Global.inidata.productconfig.EMT, "0", "0", Global.errorStatus, Global.errorcode, Global.ed[Global.Error_PendingNum + 1].start_time, "", ClientPcName, Mac, IP);
                                                Log.WriteLog("OEE_DT待料中安全门打开:" + OEE_DT1 + ",OEELog");
                                                //Global._mqttClient.Publish(sendTopic, Encoding.UTF8.GetBytes(OEE_DT1), MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE, false);
                                                //bool rst1 = SendMqttResult(guid1);
                                                //if (rst1)
                                                //{
                                                //    if (Global.respond[guid1].Result == "OK")
                                                //    {
                                                //        Global.ConnectOEEFlag = true;
                                                //        Log.WriteLog("OEE_DT安全门打开自动errorCode发送成功" + ",OEELog");
                                                //        _homefrm.AppendRichText(Global.errorcode + ",触发时间=" + Global.ed[Global.Error_PendingNum + 1].start_time + ",运行状态:" + Global.errorStatus + ",故障描述:" + Global.errorinfo + ",安全门打开自动发送成功", "rtx_DownTimeMsg");
                                                //        Global.respond.TryRemove(guid1, out Global.Respond);
                                                //    }
                                                //    else
                                                //    {
                                                //        Global.ConnectOEEFlag = false;
                                                //        _homefrm.AppendRichText(Global.errorcode + ",触发时间=" + Global.ed[Global.Error_PendingNum + 1].start_time + ",运行状态:" + Global.errorStatus + ",故障描述:" + Global.errorinfo + ",自动发送失败", "rtx_DownTimeMsg");
                                                //        _homefrm.AppendRichText("失败原因:" + Global.respond[guid1].ErrorCode + Global.respond[guid1].Result, "rtx_DownTimeMsg");
                                                //        Log.WriteLog("OEE_DT安全门打开自动errorCode发送失败" + ",OEELog");
                                                //        Log.WriteLog(Global.respond[guid1].GUID.ToString() + "," + Global.respond[guid1].Result + "," + Global.respond[guid1].ErrorCode, System.AppDomain.CurrentDomain.BaseDirectory + "\\MQTT返回NG\\");
                                                //        Global.respond.TryRemove(guid1, out Global.Respond);
                                                //    }
                                                //}
                                                //else
                                                //{
                                                //    Global.ConnectOEEFlag = false;
                                                //    Log.WriteLog("OEE_DT安全门打开自动errorCode发送失败,超时无反馈:" + ",OEELog");
                                                //    _homefrm.AppendRichText(Global.errorcode + ",触发时间=" + Global.ed[Global.Error_PendingNum + 1].start_time + ",运行状态:" + Global.errorStatus + ",故障描述:" + Global.errorinfo + ",安全门打开自动发送失败", "rtx_DownTimeMsg");
                                                //    _homefrm.AppendRichText("网络异常,超时无反馈", "rtx_DownTimeMsg");
                                                //    string s = "insert into OEE_DTSendNG([DateTime],[Product],[GUID],[EMT],[PoorNum],[TotalNum],[Status],[ErrorCode],[ModuleCode],[ClientPcName],[MAC],[IP],[EventTime],[Moduleinfo],[errorinfo])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," + "'" + "OEE_DT" + "'" + "," + "'" + guid1 + "'" + "," + "'" + Global.inidata.productconfig.EMT + "'" + "," + "'" + "0" + "'" + "," + "'" + "0" + "'" + "," + "'" + Global.errorStatus + "'" + "," + "'" + Global.errorcode + "'" + ","
                                                //           + "'" + "" + "'" + "," + "'" + ClientPcName + "'" + "," + "'" + Mac + "'" + "," + "'" + IP + "'" + "," + "'" + Global.ed[Global.Error_PendingNum + 1].start_time + "'" + "," + "'" + "" + "'" + "," + "'" + Global.errorinfo + "'" + ")";
                                                //    int r = SQL.ExecuteUpdate(s);
                                                //    Log.WriteLog(string.Format("插入了{0}行OEE_DownTime缓存数据", r) + ",OEELog");
                                                //}
                                                _manualfrm.labelcolor(Color.Transparent, "未选择", "LB_ManualSelect");
                                                string InsertOEEStr = "insert into OEE_DT([DateTime],[ErrorCode],[EventTime],[ModuleCode],[RunStatus],[ErrorInfo],[TimeSpan])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "'" + "," + "'" + Global.errorcode + "'" + "," + "'" + Global.ed[Global.Error_PendingNum + 1].start_time + "'" + ","
                                               + "'" + "" + "'" + "," + "'" + Global.errorStatus + "'" + "," + "'" + Global.errorinfo + "'" + "," + "'" + ts + "'" + ")";
                                                SQL.ExecuteUpdate(InsertOEEStr);
                                                InsertOEEStr = "insert into OEE_TraceDT([DateTime],[Status],[ErrorCode],[EventTime],[ModuleCode])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," + "'" + Global.errorStatus + "'" + "," + "'" + Global.errorcode + "'" + ","
                                                + "'" + Global.ed[Global.Error_PendingNum + 1].start_time + "'" + "," + "'" + Global.ed[Global.Error_PendingNum + 1].ModuleCode + "'" + ")";
                                                SQL.ExecuteUpdate(InsertOEEStr);
                                                Log.WriteLog(DateTime.Now.ToString("HH:mm:ss") + "," + Global.errorcode + "," + Global.ed[Global.Error_PendingNum + 1].start_time + "," + "" + "," + "自动发送成功" + "," + Global.errorStatus + "," + Global.errorinfo + "," + ts, System.AppDomain.CurrentDomain.BaseDirectory + "\\系统配置\\System_ini\\");
                                                Log.WriteLog("" + "_" + Global.errorinfo + "：结束计时 " + Global.ed[Global.Error_PendingNum + 1].stop_time);
                                            }
                                        }
                                        else
                                        {
                                            Global.ed[Global.Error_PendingNum + 1].stop_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                                            _manualfrm.labelcolor(Color.Transparent, "未选择", "LB_ManualSelect");
                                            string c = "c=UPLOAD_DOWNTIME&tsn=Test_station&mn=Machine#1&start_time=" + Global.ed[Global.Error_PendingNum + 1].start_time + "&stop_time=" + Global.ed[Global.Error_PendingNum + 1].stop_time + "&ec=" + Global.ed[Global.Error_PendingNum + 1].errorCode;
                                            Log.WriteLog(c + ",OEELog");
                                            if (Global.ed[Global.Error_PendingNum + 1].start_time != null)
                                            {
                                                DateTime t1 = Convert.ToDateTime(Global.ed[Global.Error_PendingNum + 1].start_time);
                                                DateTime t2 = Convert.ToDateTime(Global.ed[Global.Error_PendingNum + 1].stop_time);
                                                string ts = (t2 - t1).TotalMinutes.ToString("0.00");
                                                string InsertStr1 = "insert into OEE_DT([DateTime],[ErrorCode],[EventTime],[ModuleCode],[RunStatus],[ErrorInfo],[TimeSpan])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "'" + "," + "'" + Global.ed[Global.Error_PendingNum + 1].errorCode + "'" + "," + "'" + Global.ed[Global.Error_PendingNum + 1].start_time + "'" + ","
                                                   + "'" + Global.ed[Global.Error_PendingNum + 1].ModuleCode + "'" + "," + "'" + Global.ed[Global.Error_PendingNum + 1].errorStatus + "'" + "," + "'" + Global.ed[Global.Error_PendingNum + 1].errorinfo + "'" + "," + "'" + ts + "'" + ")";
                                                SQL.ExecuteUpdate(InsertStr1);
                                                Log.WriteLog(DateTime.Now.ToString("HH:mm:ss") + "," + Global.ed[Global.Error_PendingNum + 1].errorCode + "," + Global.ed[Global.Error_PendingNum + 1].start_time + "," + Global.ed[Global.Error_PendingNum + 1].ModuleCode + "," + "自动发送成功" + "," + Global.ed[Global.Error_PendingNum + 1].errorStatus + "," + Global.ed[Global.Error_PendingNum + 1].errorinfo + "," + ts, System.AppDomain.CurrentDomain.BaseDirectory + "\\系统配置\\System_ini\\");
                                                _manualfrm.labelcolor(Color.Transparent, "未选择", "LB_ManualSelect");
                                            }
                                            Log.WriteLog(Global.ed[Global.Error_PendingNum + 1].Moduleinfo + "_" + Global.ed[Global.Error_PendingNum + 1].errorinfo + "：结束计时 " + Global.ed[Global.Error_PendingNum + 1].stop_time + ",OEELog");
                                            if (Global.Feeding)//人工上料中机台启动自动结束人工上料补传一笔HSG待料
                                            {
                                                Global.SendHSG_start_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                                                Global.Feeding = false;
                                                SendHSG();
                                                Global.SendHSG_start_time = null;
                                            }
                                        }
                                        Global.ed[Global.Error_PendingNum + 1].start_time = null;
                                        Global.ed[Global.Error_PendingNum + 1].stop_time = null;
                                        Global.j = ReadStatus[0];
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
                                        Log.WriteLog(DateTime.Now.ToString("HH:mm:ss") + "," + Global.ed[Global.j].errorCode + "," + Global.ed[Global.j].start_time + "," + "'" + "" + "'" + "," + "自动发送成功" + "," + Global.ed[Global.j].errorStatus + "," + Global.ed[Global.j].errorinfo + "," + ts, System.AppDomain.CurrentDomain.BaseDirectory + "\\系统配置\\System_ini\\");
                                    }
                                    else if (Global.j == 3)//处于宕机状态
                                    {
                                        Global.ed[Global.Error_num + 1].stop_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                                        DateTime t1 = Convert.ToDateTime(Global.ed[Global.Error_num + 1].start_time);
                                        DateTime t2 = Convert.ToDateTime(Global.ed[Global.Error_num + 1].stop_time);
                                        string ts = (t2 - t1).TotalMinutes.ToString("0.00");
                                        if (Global.Error_num == 9 || Global.Error_num == 10 || Global.Error_num == 11 || Global.Error_num == 12)//机台打开安全门
                                        {
                                            string OEE_DT3 = "";
                                            Guid guid3 = Guid.NewGuid();
                                            OEE_DT3 = string.Format("{{\"guid\":\"{0}\",\"EMT\":\"{1}\",\"PoorNum\":\"{2}\",\"TotalNum\":\"{3}\",  \"Status\":\"{4}\",\"ErrorCode\":\"{5}\",\"EventTime\":\"{6}\",\"ModuleCode\":\"{7}\",\"ClientPcName\":\"{8}\",\"MAC\":\"{9}\",\"IP\":\"{10}\"}}", guid3, Global.inidata.productconfig.EMT, "0", "0", Global.errorStatus, Global.errorcode, Global.ed[Global.Error_num + 1].start_time, "", ClientPcName, Mac, IP);
                                            Log.WriteLog("OEE_DT安全门打开:" + OEE_DT3 + ",OEELog");
                                            //Global._mqttClient.Publish(sendTopic, Encoding.UTF8.GetBytes(OEE_DT3), MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE, false);
                                            //bool rst3 = SendMqttResult(guid3);
                                            //if (rst3)
                                            //{
                                            //    if (Global.respond[guid3].Result == "OK")
                                            //    {
                                            //        Global.ConnectOEEFlag = true;
                                            //        Log.WriteLog("OEE_DT安全门打开自动errorCode发送成功" + ",OEELog");
                                            //        _homefrm.AppendRichText(Global.errorcode + ",触发时间=" + Global.ed[Global.Error_num + 1].start_time + ",运行状态:" + Global.errorStatus + ",故障描述:" + Global.errorinfo + ",安全门打开自动发送成功", "rtx_DownTimeMsg");
                                            //        Global.respond.TryRemove(guid3, out Global.Respond);
                                            //    }
                                            //    else
                                            //    {
                                            //        Global.ConnectOEEFlag = false;
                                            //        Log.WriteLog("OEE_DT安全门打开自动errorCode发送失败" + ",OEELog");
                                            //        Log.WriteLog(Global.respond[guid3].GUID.ToString() + "," + Global.respond[guid3].Result + "," + Global.respond[guid3].ErrorCode, System.AppDomain.CurrentDomain.BaseDirectory + "\\MQTT返回NG\\");
                                            //        _homefrm.AppendRichText(Global.errorcode + ",触发时间=" + Global.ed[Global.Error_num + 1].start_time + ",运行状态:" + Global.errorStatus + ",故障描述:" + Global.errorinfo + ",安全门打开自动发送失败", "rtx_DownTimeMsg");
                                            //        _homefrm.AppendRichText("失败原因:" + Global.respond[guid3].ErrorCode + Global.respond[guid3].Result, "rtx_DownTimeMsg");
                                            //        Global.respond.TryRemove(guid3, out Global.Respond);
                                            //    }
                                            //}
                                            //else
                                            //{
                                            //    Global.ConnectOEEFlag = false;
                                            //    Log.WriteLog("OEE_DT安全门打开自动errorCode发送失败,超时无反馈:" + ",OEELog");
                                            //    _homefrm.AppendRichText(Global.errorcode + ",触发时间=" + Global.ed[Global.Error_num + 1].start_time + ",运行状态:" + Global.errorStatus + ",故障描述:" + Global.errorinfo + ",安全门打开自动发送失败", "rtx_DownTimeMsg");
                                            //    _homefrm.AppendRichText("网络异常,超时无反馈", "rtx_DownTimeMsg");
                                            //    string s = "insert into OEE_DTSendNG([DateTime],[Product],[GUID],[EMT],[PoorNum],[TotalNum],[Status],[ErrorCode],[ModuleCode],[ClientPcName],[MAC],[IP],[EventTime],[Moduleinfo],[errorinfo])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," + "'" + "OEE_DT" + "'" + "," + "'" + guid3 + "'" + "," + "'" + Global.inidata.productconfig.EMT + "'" + "," + "'" + "0" + "'" + "," + "'" + "0" + "'" + "," + "'" + Global.errorStatus + "'" + "," + "'" + Global.errorcode + "'" + ","
                                            //           + "'" + "" + "'" + "," + "'" + ClientPcName + "'" + "," + "'" + Mac + "'" + "," + "'" + IP + "'" + "," + "'" + Global.ed[Global.Error_num + 1].start_time + "'" + "," + "'" + "" + "'" + "," + "'" + Global.errorinfo + "'" + ")";
                                            //    int r = SQL.ExecuteUpdate(s);
                                            //    Log.WriteLog(string.Format("插入了{0}行OEE_DownTime缓存数据", r) + ",OEELog");
                                            //}
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
                                        if (Global.ed[Global.Error_Stopnum + 1].start_time != null)
                                        {
                                            DateTime t1 = Convert.ToDateTime(Global.ed[Global.Error_Stopnum + 1].start_time);
                                            DateTime t2 = Convert.ToDateTime(Global.ed[Global.Error_Stopnum + 1].stop_time);
                                            string ts = (t2 - t1).TotalMinutes.ToString("0.00");
                                            if (ReadStatus[3] == 9 || ReadStatus[3] == 10 || ReadStatus[3] == 11 || ReadStatus[3] == 12 || Global.SelectManualErrorCode)//机台打开安全门或者手动选择ErrorCode状态开启
                                            {
                                                string OEE_DT4 = "";
                                                Guid guid4 = Guid.NewGuid();
                                                OEE_DT4 = string.Format("{{\"guid\":\"{0}\",\"EMT\":\"{1}\",\"PoorNum\":\"{2}\",\"TotalNum\":\"{3}\",  \"Status\":\"{4}\",\"ErrorCode\":\"{5}\",\"EventTime\":\"{6}\",\"ModuleCode\":\"{7}\",\"ClientPcName\":\"{8}\",\"MAC\":\"{9}\",\"IP\":\"{10}\"}}", guid4, Global.inidata.productconfig.EMT, "0", "0", Global.errorStatus, Global.errorcode, Global.ed[Global.Error_Stopnum + 1].start_time, "", ClientPcName, Mac, IP);
                                                Log.WriteLog("OEE_DT安全门打开:" + OEE_DT4 + ",OEELog");
                                                //Global._mqttClient.Publish(sendTopic, Encoding.UTF8.GetBytes(OEE_DT4), MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE, false);
                                                //bool rst4 = SendMqttResult(guid4);
                                                //if (rst4)
                                                //{
                                                //    if (Global.respond[guid4].Result == "OK")
                                                //    {
                                                //        Global.ConnectOEEFlag = true;
                                                //        Log.WriteLog("OEE_DT安全门打开自动errorCode发送成功" + ",OEELog");
                                                //        _homefrm.AppendRichText(Global.errorcode + ",触发时间=" + Global.ed[Global.Error_Stopnum + 1].start_time + ",运行状态:" + Global.errorStatus + ",故障描述:" + Global.errorinfo + ",安全门打开自动发送成功", "rtx_DownTimeMsg");
                                                //        Global.respond.TryRemove(guid4, out Global.Respond);
                                                //    }
                                                //    else
                                                //    {
                                                //        Global.ConnectOEEFlag = false;
                                                //        Log.WriteLog("OEE_DT安全门打开自动errorCode发送失败" + ",OEELog");
                                                //        Log.WriteLog(Global.respond[guid4].GUID.ToString() + "," + Global.respond[guid4].Result + "," + Global.respond[guid4].ErrorCode, System.AppDomain.CurrentDomain.BaseDirectory + "\\MQTT返回NG\\");
                                                //        _homefrm.AppendRichText(Global.errorcode + ",触发时间=" + Global.ed[Global.Error_Stopnum + 1].start_time + ",运行状态:" + Global.errorStatus + ",故障描述:" + Global.errorinfo + ",安全门打开自动发送失败", "rtx_DownTimeMsg");
                                                //        _homefrm.AppendRichText("失败原因:" + Global.respond[guid4].ErrorCode + Global.respond[guid4].Result, "rtx_DownTimeMsg");
                                                //        Global.respond.TryRemove(guid4, out Global.Respond);
                                                //    }
                                                //}
                                                //else
                                                //{
                                                //    Global.ConnectOEEFlag = false;
                                                //    Log.WriteLog("OEE_DT人工停止复位自动errorCode发送失败,超时无反馈:" + ",OEELog");
                                                //    _homefrm.AppendRichText(Global.errorcode + ",触发时间=" + Global.ed[Global.Error_Stopnum + 1].start_time + ",运行状态:" + Global.errorStatus + ",故障描述:" + Global.errorinfo + ",安全门打开自动发送失败", "rtx_DownTimeMsg");
                                                //    _homefrm.AppendRichText("网络异常,超时无反馈", "rtx_DownTimeMsg");
                                                //    string s = "insert into OEE_DTSendNG([DateTime],[Product],[GUID],[EMT],[PoorNum],[TotalNum],[Status],[ErrorCode],[ModuleCode],[ClientPcName],[MAC],[IP],[EventTime],[Moduleinfo],[errorinfo])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," + "'" + "OEE_DT" + "'" + "," + "'" + guid4 + "'" + "," + "'" + Global.inidata.productconfig.EMT + "'" + "," + "'" + "0" + "'" + "," + "'" + "0" + "'" + "," + "'" + Global.errorStatus + "'" + "," + "'" + Global.errorcode + "'" + ","
                                                //           + "'" + "" + "'" + "," + "'" + ClientPcName + "'" + "," + "'" + Mac + "'" + "," + "'" + IP + "'" + "," + "'" + Global.ed[Global.Error_Stopnum + 1].start_time + "'" + "," + "'" + "" + "'" + "," + "'" + Global.errorinfo + "'" + ")";
                                                //    int r = SQL.ExecuteUpdate(s);
                                                //    Log.WriteLog(string.Format("插入了{0}行OEE_DownTime缓存数据", r) + ",OEELog");
                                                //}
                                                _manualfrm.labelcolor(Color.Transparent, "未选择", "LB_ManualSelect");
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
                                                Guid guid4 = Guid.NewGuid();
                                                OEE_DT4 = string.Format("{{\"guid\":\"{0}\",\"EMT\":\"{1}\",\"PoorNum\":\"{2}\",\"TotalNum\":\"{3}\",  \"Status\":\"{4}\",\"ErrorCode\":\"{5}\",\"EventTime\":\"{6}\",\"ModuleCode\":\"{7}\",\"ClientPcName\":\"{8}\",\"MAC\":\"{9}\",\"IP\":\"{10}\"}}", guid4, Global.inidata.productconfig.EMT, "0", "0", Global.ed[Global.Error_Stopnum + 1].errorStatus, Global.ed[Global.Error_Stopnum + 1].errorCode, Global.ed[Global.Error_Stopnum + 1].start_time, Global.ed[Global.Error_Stopnum + 1].ModuleCode, ClientPcName, Mac, IP);
                                                Log.WriteLog("OEE_DT人工停止复位:" + OEE_DT4 + ",OEELog");
                                                //Global._mqttClient.Publish(sendTopic, Encoding.UTF8.GetBytes(OEE_DT4), MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE, false);
                                                //bool rst4 = SendMqttResult(guid4);
                                                //if (rst4)
                                                //{
                                                //    if (Global.respond[guid4].Result == "OK")
                                                //    {
                                                //        Global.ConnectOEEFlag = true;
                                                //        Log.WriteLog("OEE_DT人工停止复位自动errorCode发送成功" + ",OEELog");
                                                //        _homefrm.AppendRichText(Global.ed[Global.Error_Stopnum + 1].errorCode + ",触发时间=" + Global.ed[Global.Error_Stopnum + 1].start_time + ",运行状态:" + Global.ed[Global.Error_Stopnum + 1].errorStatus + ",故障描述:" + Global.ed[Global.Error_Stopnum + 1].errorinfo + ",安全门打开自动发送成功", "rtx_DownTimeMsg");
                                                //        Global.respond.TryRemove(guid4, out Global.Respond);
                                                //    }
                                                //    else
                                                //    {
                                                //        Global.ConnectOEEFlag = false;
                                                //        Log.WriteLog("OEE_DT人工停止复位自动errorCode发送失败" + ",OEELog");
                                                //        Log.WriteLog(Global.respond[guid4].GUID.ToString() + "," + Global.respond[guid4].Result + "," + Global.respond[guid4].ErrorCode, System.AppDomain.CurrentDomain.BaseDirectory + "\\MQTT返回NG\\");
                                                //        _homefrm.AppendRichText(Global.ed[Global.Error_Stopnum + 1].errorCode + ",触发时间=" + Global.ed[Global.Error_Stopnum + 1].start_time + ",运行状态:" + Global.ed[Global.Error_Stopnum + 1].errorStatus + ",故障描述:" + Global.ed[Global.Error_Stopnum + 1].errorinfo + ",安全门打开自动发送失败", "rtx_DownTimeMsg");
                                                //        _homefrm.AppendRichText("失败原因:" + Global.respond[guid4].ErrorCode + Global.respond[guid4].Result, "rtx_DownTimeMsg");
                                                //        Global.respond.TryRemove(guid4, out Global.Respond);
                                                //    }
                                                //}
                                                //else
                                                //{
                                                //    Global.ConnectOEEFlag = false;
                                                //    Log.WriteLog("OEE_DT人工停止复位自动errorCode发送失败,超时无反馈:" + ",OEELog");
                                                //    _homefrm.AppendRichText(Global.ed[Global.Error_Stopnum + 1].errorCode + ",触发时间=" + Global.ed[Global.Error_Stopnum + 1].start_time + ",运行状态:" + Global.ed[Global.Error_Stopnum + 1].errorStatus + ",故障描述:" + Global.ed[Global.Error_Stopnum + 1].errorinfo + ",安全门打开自动发送失败", "rtx_DownTimeMsg");
                                                //    _homefrm.AppendRichText("网络异常,超时无反馈", "rtx_DownTimeMsg");
                                                //    string s = "insert into OEE_DTSendNG([DateTime],[Product],[GUID],[EMT],[PoorNum],[TotalNum],[Status],[ErrorCode],[ModuleCode],[ClientPcName],[MAC],[IP],[EventTime],[Moduleinfo],[errorinfo])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," + "'" + "OEE_DT" + "'" + "," + "'" + guid4 + "'" + "," + "'" + Global.inidata.productconfig.EMT + "'" + "," + "'" + "0" + "'" + "," + "'" + "0" + "'" + "," + "'" + Global.ed[Global.Error_Stopnum + 1].errorStatus + "'" + "," + "'" + Global.ed[Global.Error_Stopnum + 1].errorCode + "'" + ","
                                                //           + "'" + Global.ed[Global.Error_Stopnum + 1].ModuleCode + "'" + "," + "'" + ClientPcName + "'" + "," + "'" + Mac + "'" + "," + "'" + IP + "'" + "," + "'" + Global.ed[Global.Error_Stopnum + 1].start_time + "'" + "," + "'" + Global.ed[Global.Error_Stopnum + 1].Moduleinfo + "'" + "," + "'" + Global.ed[Global.Error_Stopnum + 1].errorinfo + "'" + ")";
                                                //    int r = SQL.ExecuteUpdate(s);
                                                //    Log.WriteLog(string.Format("插入了{0}行OEE_DownTime缓存数据", r) + ",OEELog");
                                                //}
                                                _manualfrm.labelcolor(Color.Transparent, "未选择", "LB_ManualSelect");
                                                string InsertOEEStr = "insert into OEE_DT([DateTime],[ErrorCode],[EventTime],[ModuleCode],[RunStatus],[ErrorInfo],[TimeSpan])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "'" + "," + "'" + Global.ed[Global.Error_Stopnum + 1].errorCode + "'" + "," + "'" + Global.ed[Global.Error_Stopnum + 1].start_time + "'" + ","
                                               + "'" + "" + "'" + "," + "'" + Global.ed[Global.Error_Stopnum + 1].errorStatus + "'" + "," + "'" + Global.ed[Global.Error_Stopnum + 1].errorinfo + "'" + "," + "'" + ts + "'" + ")";
                                                SQL.ExecuteUpdate(InsertOEEStr);
                                                InsertOEEStr = "insert into OEE_TraceDT([DateTime],[Status],[ErrorCode],[EventTime],[ModuleCode])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," + "'" + Global.ed[Global.Error_Stopnum + 1].errorStatus + "'" + "," + "'" + Global.ed[Global.Error_Stopnum + 1].errorCode + "'" + ","
                                                + "'" + Global.ed[Global.Error_Stopnum + 1].start_time + "'" + "," + "'" + Global.ed[Global.Error_Stopnum + 1].ModuleCode + "'" + ")";
                                                SQL.ExecuteUpdate(InsertOEEStr);
                                                Log.WriteLog(DateTime.Now.ToString("HH:mm:ss") + "," + Global.ed[Global.Error_Stopnum + 1].errorCode + "," + Global.ed[Global.Error_Stopnum + 1].start_time + "," + "" + "," + "自动发送成功" + "," + Global.ed[Global.Error_Stopnum + 1].errorStatus + "," + Global.ed[Global.Error_Stopnum + 1].errorinfo + "," + ts, System.AppDomain.CurrentDomain.BaseDirectory + "\\系统配置\\System_ini\\");
                                            }
                                        }
                                    }
                                    string OEE_DT = "";
                                    Guid guid = Guid.NewGuid();
                                    string EventTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                                    Global.ed[313].start_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");//空跑开始时间
                                    OEE_DT = string.Format("{{\"guid\":\"{0}\",\"EMT\":\"{1}\",\"PoorNum\":\"{2}\",\"TotalNum\":\"{3}\",  \"Status\":\"{4}\",\"ErrorCode\":\"{5}\",\"EventTime\":\"{6}\",\"ModuleCode\":\"{7}\",\"ClientPcName\":\"{8}\",\"MAC\":\"{9}\",\"IP\":\"{10}\"}}", guid, Global.inidata.productconfig.EMT, "0", "0", Global.ed[313].errorStatus, Global.ed[313].errorCode, Global.ed[313].start_time, Global.ed[313].ModuleCode, ClientPcName, Mac, IP);
                                    Log.WriteLog("OEE_DT空跑:" + OEE_DT);
                                    string InsertStr = "insert into OEE_TraceDT([DateTime],[Status],[ErrorCode],[EventTime],[ModuleCode])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," + "'" + "5" + "'" + "," + "'" + Global.ed[313].errorCode + "'" + ","
                                             + "'" + EventTime + "'" + "," + "'" + "" + "'" + ")";
                                    SQL.ExecuteUpdate(InsertStr);
                                    //Global._mqttClient.Publish(sendTopic, Encoding.UTF8.GetBytes(OEE_DT), MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE, false);
                                    //bool rst = SendMqttResult(guid);
                                    //if (rst)
                                    //{
                                    //    if (Global.respond[guid].Result == "OK")
                                    //    {
                                    //        Global.ConnectOEEFlag = true;
                                    //        Log.WriteLog("OEE_DT机台空跑发送成功");
                                    //        Log.WriteLog(DateTime.Now.ToString("HH:mm:ss") + "," + Global.ed[313].errorCode + "," + EventTime + "," + "手动发送成功" + "," + "5" + "," + "空跑", System.AppDomain.CurrentDomain.BaseDirectory + "\\系统配置\\System_ini\\");
                                    //        _homefrm.AppendRichText(Global.ed[313].errorCode + ",触发时间=" + EventTime + ",运行状态:" + "5" + ",故障描述:" + "空跑" + ",自动发送成功", "rtx_DownTimeMsg");
                                    //        Global.respond.TryRemove(guid, out Global.Respond);
                                    //    }
                                    //    else
                                    //    {
                                    //        Global.ConnectOEEFlag = false;
                                    //        Log.WriteLog("OEE_DT机台空跑发送失败");
                                    //        Log.WriteLog(Global.respond[guid].GUID.ToString() + "," + Global.respond[guid].Result + "," + Global.respond[guid].ErrorCode, System.AppDomain.CurrentDomain.BaseDirectory + "\\MQTT返回NG\\");
                                    //        Log.WriteLog(DateTime.Now.ToString("HH:mm:ss") + "," + Global.ed[313].errorCode + "," + EventTime + "," + "手动发送失败" + "," + "5" + "," + "空跑", System.AppDomain.CurrentDomain.BaseDirectory + "\\系统配置\\System_ini\\");
                                    //        _homefrm.AppendRichText(Global.ed[313].errorCode + ",触发时间=" + EventTime + ",运行状态:" + "5" + ",故障描述:" + "空跑" + ",自动发送失败", "rtx_DownTimeMsg");
                                    //        _homefrm.AppendRichText("失败原因:" + Global.respond[guid].ErrorCode + Global.respond[guid].Result, "rtx_DownTimeMsg");
                                    //        Global.respond.TryRemove(guid, out Global.Respond);
                                    //    }


                                    //}
                                    //else
                                    //{
                                    //    Global.ConnectOEEFlag = false;
                                    //    Log.WriteLog("OEE_DT机台空跑发送失败");
                                    //    Log.WriteLog(DateTime.Now.ToString("HH:mm:ss") + "," + Global.ed[313].errorCode + "," + EventTime + "," + "手动发送失败" + "," + "5" + "," + "空跑", System.AppDomain.CurrentDomain.BaseDirectory + "\\系统配置\\System_ini\\");
                                    //    _homefrm.AppendRichText(Global.ed[313].errorCode + ",触发时间=" + EventTime + ",运行状态:" + "5" + ",故障描述:" + "空跑" + ",自动发送失败", "rtx_DownTimeMsg");
                                    //    _homefrm.AppendRichText("网络异常,超时无反馈", "rtx_DownTimeMsg");
                                    //    string s = "insert into OEE_DTSendNG([DateTime],[Product],[GUID],[EMT],[PoorNum],[TotalNum],[Status],[ErrorCode],[ModuleCode],[ClientPcName],[MAC],[IP],[EventTime],[Moduleinfo],[errorinfo])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," + "'" + "OEE_DT" + "'" + "," + "'" + guid + "'" + "," + "'" + Global.inidata.productconfig.EMT + "'" + "," + "'" + "0" + "'" + "," + "'" + "0" + "'" + "," + "'" + Global.ed[313].errorStatus + "'" + "," + "'" + Global.ed[313].errorCode + "'" + ","
                                    //           + "'" + Global.ed[313].ModuleCode + "'" + "," + "'" + ClientPcName + "'" + "," + "'" + Mac + "'" + "," + "'" + IP + "'" + "," + "'" + Global.ed[313].start_time + "'" + "," + "'" + Global.ed[313].Moduleinfo + "'" + "," + "'" + Global.ed[313].errorinfo + "'" + ")";
                                    //    int r = SQL.ExecuteUpdate(s);
                                    //    Log.WriteLog(string.Format("插入了{0}行OEE_DownTime缓存数据", r) + ",OEELog");
                                    //}
                                    Global.j = -1;
                                    string InsertOEEStr3 = "insert into OEE_StartTime([Status],[ErrorCode],[EventTime],[ModuleCode],[Name])" + " " + "values(" + "'" + "5" + "'" + "," + "'" + Global.ed[313].errorCode + "'" + "," + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "'" + "," + "'" + "" + "'" + "," + "'" + Global.ed[313].errorinfo + "'" + ")";
                                    SQL.ExecuteUpdate(InsertOEEStr3);//插入空跑开始时间
                                }
                            }
                        }
                        else//处于手动(首件)状态
                        {
                            Global.j = -1;
                            Global.SelectTestRunModel = false;
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.WriteLog("OEE-DwonTime异常！" + ex.ToString() + ",OEELog");
                        _homefrm.AppendRichText("OEE-DwonTime异常！", "rtx_DownTimeMsg");
                    }
                }
                Thread.Sleep(100);
            }
        }
        #endregion
        #region  OEE-心跳信号
        private void EthOEEHeartBeat_Mqtt()
        {
            while (true)
            {
                try
                {
                    string sendTopic = Global.inidata.productconfig.EMT + "/upload/pant";
                    var IP = GetIp();
                    var Mac = GetOEEMac();
                    ErrorData Ed = new ErrorData();
                    Ed.GUID = Guid.NewGuid();
                    Ed.EMT = Global.inidata.productconfig.EMT;
                    Ed.ClientPcName = Dns.GetHostName();
                    Ed.MAC = GetOEEMac();
                    Ed.IP = GetIp();
                    Ed.Sw_version = Global.inidata.productconfig.Sw_version;
                    Ed.EventTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                    string OEEHeartBeat = string.Format("{{\"GUID\":\"{0}\",\"EMT\":\"{1}\",\"ClientPcName\":\"{2}\",\"MAC\":\"{3}\",\"IP\":\"{4}\",\"SwVersion\":\"{5}\",\"EventTime\":\"{6}\"}}", Ed.GUID, Ed.EMT, Ed.ClientPcName, Ed.MAC, Ed.IP, Ed.Sw_version, Ed.EventTime);
                    Log.WriteLog("OEEHeartBeat:" + OEEHeartBeat + ",OEELog");
                    //Global._mqttClient.Publish(sendTopic, Encoding.UTF8.GetBytes(OEEHeartBeat), MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE, false);
                    //bool rst = SendMqttResult1(Ed.GUID);
                    //if (rst)
                    //{
                    //    if (Global.respond1[Ed.GUID].Result == "OK")
                    //    {
                    //        Global.ConnectOEEFlag = true;
                    //        Log.WriteLog("OEEHeartBeat:发送成功" + ",OEELog");
                    //        _homefrm.AppendRichText("OEEHeartBeat :" + Global.respond1[Ed.GUID].Result + Global.respond1[Ed.GUID].ErrorCode, "rtx_HeartBeatMsg");
                    //        Global.respond1.Clear();
                    //    }
                    //    else
                    //    {
                    //        Global.ConnectOEEFlag = false;
                    //        Log.WriteLog("OEEHeartBeat: 发送失败" + ",OEELog");
                    //        _homefrm.AppendRichText("OEEHeartBeat异常,异常原因:" + Global.respond1[Ed.GUID].ErrorCode + Global.respond1[Ed.GUID].Result, "rtx_HeartBeatMsg");
                    //        Global.respond1.Clear();
                    //    }
                    //}
                    //else
                    //{
                    //    Global.ConnectOEEFlag = false;
                    //    Log.WriteLog("OEEHeartBeat发送失败,超时无反馈:" + OEEHeartBeat);
                    //    _homefrm.AppendRichText("OEE网络异常,超时无反馈", "rtx_HeartBeatMsg");
                    //    Global.respond1.Clear();
                    //}
                }
                catch
                {
                    Log.WriteLog("OEE心跳异常，请检查OEE网络！" + ",OEELog");
                    Global.ConnectOEEFlag = false;
                    Global.respond1.Clear();
                    _homefrm.AppendRichText(DateTime.Now.ToString("OEE心跳异常，请检查OEE网络！"), "rtx_HeartBeatMsg");
                }
                Thread.Sleep(60000);//每分钟上传心跳信息
            }
        }
        #endregion
        #region  OEE Default上传
        private void EthOEEDefault()
        {
            while (true)
            {
                if (OEE_Mqtt.Count > 0)
                {
                    string item = Out_oee_Mqtt[0];
                    #region
                    string ErrorCode = string.Empty;
                    string PFErrorCode = string.Empty;
                    Respond respond = new Respond();
                    string sendTopic = string.Empty;
                    string OEE_Data = "";
                    OEE_Mqtt[item].SwVersion = Global.inidata.productconfig.Sw_version;
                    OEE_Mqtt[item].GUID = Guid.NewGuid();
                    OEE_Mqtt[item].EMT = Global.inidata.productconfig.EMT;
                    OEE_Mqtt[item].ScanCount = "1";
                    OEE_Mqtt[item].Cavity = "0";
                    OEE_Mqtt[item].ClientPcName = Dns.GetHostName();
                    OEE_Mqtt[item].MAC = GetOEEMac();
                    OEE_Mqtt[item].IP = GetIp();
                    OEE_Mqtt[item].EventTime = DateTime.Now;
                    //判断小料抛料表格的值与上一片料上传时抛料表格值比较，有变化写入errorcode/PFerrorcode,过渡变量写入当前值，
                    if (OEE_Mqtt[item].Status == "NG")
                    {
                        if (Convert.ToDateTime(DateTime.Now.ToString("HH:mm")).CompareTo(Convert.ToDateTime("6:00")) >= 0 && Convert.ToDateTime(DateTime.Now.ToString("HH:mm")).CompareTo(Convert.ToDateTime("18:00")) < 0)
                        {
                            if (Global.itm_TracePVCheck_Error_D != Global.inidata.productconfig.TracePVCheck_Error_D)
                            {
                                PFErrorCode += ";080001";
                                Global.itm_TracePVCheck_Error_D = Global.inidata.productconfig.TracePVCheck_Error_D;
                            }
                            if (Global.itm_ReadBarcode_NG_D != Global.inidata.productconfig.ReadBarcode_NG_D)
                            {
                                PFErrorCode += ";080002";
                                Global.itm_ReadBarcode_NG_D = Global.inidata.productconfig.ReadBarcode_NG_D;
                            }
                            if (Global.itm_Band_NG_D != Global.inidata.productconfig.Band_NG_D)
                            {
                                PFErrorCode += "";
                                Global.itm_Band_NG_D = Global.inidata.productconfig.Band_NG_D;
                            }
                            if (Global.itm_TraceUpLoad_Error_D != Global.inidata.productconfig.TraceUpLoad_Error_D)
                            {
                                PFErrorCode += ";080007";
                                Global.itm_TraceUpLoad_Error_D = Global.inidata.productconfig.TraceUpLoad_Error_D;
                            }
                            if (Global.itm_PDCAUpLoad_Error_D != Global.inidata.productconfig.PDCAUpLoad_Error_D)
                            {
                                PFErrorCode += ";080008";
                                Global.itm_PDCAUpLoad_Error_D = Global.inidata.productconfig.PDCAUpLoad_Error_D;
                            }
                            if (PFErrorCode != "")
                            {
                                //移除最前面多余的分(;)号
                                PFErrorCode = PFErrorCode.Substring(1, PFErrorCode.Length - 1);
                            }
                            if (OEE_Mqtt[item].ErrorCode != "" && OEE_Mqtt[item].ErrorCode != null)
                            {
                                //移除最前面多余的分(;)号
                                OEE_Mqtt[item].ErrorCode = OEE_Mqtt[item].ErrorCode.Substring(1, OEE_Mqtt[item].ErrorCode.Length - 1);
                            }
                            OEE_Mqtt[item].PFErrorCode = PFErrorCode;
                        }
                        else
                        {
                            if (Global.itm_TracePVCheck_Error_N != Global.inidata.productconfig.TracePVCheck_Error_N)
                            {
                                PFErrorCode += ";080001";
                                Global.itm_TracePVCheck_Error_N = Global.inidata.productconfig.TracePVCheck_Error_N;
                            }
                            if (Global.itm_ReadBarcode_NG_N != Global.inidata.productconfig.ReadBarcode_NG_N)
                            {
                                PFErrorCode += ";080002";
                                Global.itm_ReadBarcode_NG_N = Global.inidata.productconfig.ReadBarcode_NG_N;
                            }
                            if (Global.itm_Band_NG_N != Global.inidata.productconfig.Band_NG_N)
                            {
                                PFErrorCode += "";
                                Global.itm_Band_NG_N = Global.inidata.productconfig.Band_NG_N;
                            }
                            if (Global.itm_TraceUpLoad_Error_N != Global.inidata.productconfig.TraceUpLoad_Error_N)
                            {
                                PFErrorCode += ";080007";
                                Global.itm_TraceUpLoad_Error_N = Global.inidata.productconfig.TraceUpLoad_Error_N;
                            }
                            if (Global.itm_PDCAUpLoad_Error_N != Global.inidata.productconfig.PDCAUpLoad_Error_N)
                            {
                                PFErrorCode += ";080008";
                                Global.itm_PDCAUpLoad_Error_N = Global.inidata.productconfig.PDCAUpLoad_Error_N;
                            }
                            if (PFErrorCode != "")
                            {
                                //移除最前面多余的分(;)号
                                PFErrorCode = PFErrorCode.Substring(1, PFErrorCode.Length - 1);
                            }
                            if (OEE_Mqtt[item].ErrorCode != "" && OEE_Mqtt[item].ErrorCode != null)
                            {
                                //移除最前面多余的分(;)号
                                OEE_Mqtt[item].ErrorCode = OEE_Mqtt[item].ErrorCode.Substring(1, OEE_Mqtt[item].ErrorCode.Length - 1);
                            }
                            OEE_Mqtt[item].PFErrorCode = PFErrorCode;
                        }
                    }
                    else
                    {
                        OEE_Mqtt[item].PFErrorCode = "";
                        OEE_Mqtt[item].ErrorCode = "";
                    }
                    try
                    {
                        if (item.Contains("FM") && !Regex.IsMatch(item, "[a-z]"))
                        {
                            string SelectStr = string.Format("SELECT * FROM HansData WHERE SN='{0}' and Station='L_Bracket'", item);//sql查询语句
                            DataTable d1 = SQL.ExecuteQuery(SelectStr);
                            if (OEE_Mqtt[item].Status == "OK")
                            {
                                if (d1 != null && d1.Rows.Count > 2)
                                {
                                    OEE_Mqtt[item].Status = "OK";
                                    OEE_Mqtt[item].ErrorCode = "";
                                }
                                else
                                {
                                    if (OEE_Mqtt[item].ErrorCode == "")
                                    {
                                        OEE_Mqtt[item].ErrorCode += "080006";
                                    }
                                    else
                                    {
                                        OEE_Mqtt[item].ErrorCode += ";080006";
                                    }
                                    OEE_Mqtt[item].Status = "NG";

                                }
                            }
                            else
                            {
                                if (d1 == null || d1.Rows.Count < 3)
                                {
                                    if (OEE_Mqtt[item].ErrorCode == "")
                                    {
                                        OEE_Mqtt[item].ErrorCode += "080006";
                                    }
                                    else
                                    {
                                        OEE_Mqtt[item].ErrorCode += ";080006";
                                    }
                                }
                            }
                            if (OEE_Mqtt[item].SerialNumber == "" || OEE_Mqtt[item].Fixture == "")
                            {
                                OEE_Data = string.Format("{{\"GUID\":\"{0}\",\"EMT\":\"{1}\",\"SerialNumber\":\"{2}\",\"BGBarcode\":\"{3}\",\"Fixture\":\"{4}\",\"StartTime\":\"{5}\",\"EndTime\":\"{6}\",\"Status\":\"{7}\",\"ActualCT\":\"{8}\",\"SwVersion\":\"{9}\",\"ScanCount\":\"{10}\",\"ErrorCode\":\"{11}\",\"PFErrorCode\":\"{12}\",\"Cavity\":\"{13}\",\"ClientPcName\":\"{14}\",\"MAC\":\"{15}\",\"IP\":\"{16}\",\"EventTime\":\"{17}\"}}", OEE_Mqtt[item].GUID, OEE_Mqtt[item].EMT, OEE_Mqtt[item].SerialNumber, OEE_Mqtt[item].BGBarcode, OEE_Mqtt[item].Fixture, OEE_Mqtt[item].StartTime.ToString("yyyy-MM-dd HH:mm:ss"), OEE_Mqtt[item].EndTime.ToString("yyyy-MM-dd HH:mm:ss"), OEE_Mqtt[item].Status, OEE_Mqtt[item].ActualCT, OEE_Mqtt[item].SwVersion, OEE_Mqtt[item].ScanCount, OEE_Mqtt[item].ErrorCode, OEE_Mqtt[item].PFErrorCode, OEE_Mqtt[item].Cavity, OEE_Mqtt[item].ClientPcName, OEE_Mqtt[item].MAC, OEE_Mqtt[item].IP, OEE_Mqtt[item].EventTime.ToString("yyyy-MM-dd HH:mm:ss"));
                            }
                            else
                            {
                                OEE_Data = string.Format("{{\"GUID\":\"{0}\",\"EMT\":\"{1}\",\"SerialNumber\":\"{2}\",\"BGBarcode\":\"{3}\",\"Fixture\":\"{4}\",\"StartTime\":\"{5}\",\"EndTime\":\"{6}\",\"Status\":\"{7}\",\"ActualCT\":\"{8}\",\"SwVersion\":\"{9}\",\"ScanCount\":\"{10}\",\"ErrorCode\":\"{11}\",\"PFErrorCode\":\"{12}\",\"Cavity\":\"{13}\",\"ClientPcName\":\"{14}\",\"MAC\":\"{15}\",\"IP\":\"{16}\",\"EventTime\":\"{17}\"}}", OEE_Mqtt[item].GUID, OEE_Mqtt[item].EMT, OEE_Mqtt[item].SerialNumber, OEE_Mqtt[item].BGBarcode, OEE_Mqtt[item].Fixture, OEE_Mqtt[item].StartTime.ToString("yyyy-MM-dd HH:mm:ss"), OEE_Mqtt[item].EndTime.ToString("yyyy-MM-dd HH:mm:ss"), OEE_Mqtt[item].Status, OEE_Mqtt[item].ActualCT, OEE_Mqtt[item].SwVersion, OEE_Mqtt[item].ScanCount, OEE_Mqtt[item].ErrorCode, OEE_Mqtt[item].PFErrorCode, OEE_Mqtt[item].Cavity, OEE_Mqtt[item].ClientPcName, OEE_Mqtt[item].MAC, OEE_Mqtt[item].IP, OEE_Mqtt[item].EventTime.ToString("yyyy-MM-dd HH:mm:ss"));
                            }
                            Log.WriteLog("OEE_Default:" + OEE_Data + ",OEELog");
                            _homefrm.AppendRichText(OEE_Data, "rtx_OEEDefaultMsg");
                            sendTopic = Global.inidata.productconfig.EMT + "/upload/oee";
                            //Global._mqttClient.Publish(sendTopic, Encoding.UTF8.GetBytes(OEE_Data), MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE, false);
                            //bool rst = SendMqttResult(OEE_Mqtt[item].GUID);
                            //if (rst)
                            //{
                            //    if (Global.respond[OEE_Mqtt[item].GUID].Result == "OK")
                            //    {
                            //        Global.oee_ok++;
                            //        _homefrm.UpDatalabel(Global.oee_ok.ToString(), "lb_OEEOK");
                            //        Global.oeeSend_ng = 0;
                            //        Global.ConnectOEEFlag = true;
                            //        OEE_Default_flag = true;
                            //        _homefrm.AppendRichText(JsonConvert.SerializeObject(Global.respond[OEE_Mqtt[item].GUID]), "rtx_OEEDefaultMsg");
                            //        _homefrm.UpDatalabelcolor(Color.Green, "OEE_Default发送成功", "lb_OEE_UA_SendStatus");
                            //        Log.WriteLog("OEE_Default返回结果-OK:" + JsonConvert.SerializeObject(Global.respond[OEE_Mqtt[item].GUID]));
                            //        Log.WriteLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + "OEE_Default" + "," + OEE_Mqtt[item].GUID.ToString() + "," + OEE_Mqtt[item].EMT + ","
                            //        + OEE_Mqtt[item].SerialNumber + "," + OEE_Mqtt[item].BGBarcode + "," + OEE_Mqtt[item].Fixture + "," + OEE_Mqtt[item].StartTime.ToString("yyyy-MM-dd HH:mm:ss") + ","
                            //        + OEE_Mqtt[item].EndTime.ToString("yyyy-MM-dd HH:mm:ss") + "," + OEE_Mqtt[item].Status + "," + OEE_Mqtt[item].ActualCT + ","
                            //        + OEE_Mqtt[item].SwVersion + "," + OEE_Mqtt[item].ScanCount + "," + OEE_Mqtt[item].ErrorCode + "," + OEE_Mqtt[item].PFErrorCode + ","
                            //        + OEE_Mqtt[item].Cavity + "," + OEE_Mqtt[item].ClientPcName + "," + OEE_Mqtt[item].MAC + ","
                            //        + OEE_Mqtt[item].IP + "," + OEE_Mqtt[item].EventTime.ToString("yyyy-MM-dd HH:mm:ss") + ","
                            //        + "OK-OEE_Default", System.AppDomain.CurrentDomain.BaseDirectory + "\\OEE_MQTT_Default数据\\");

                            //        _homefrm.UiText(OEE_Mqtt[item].SerialNumber, "txtOEE_SerialNumber");
                            //        _homefrm.UiText(OEE_Mqtt[item].Fixture, "txtOEE_Fixture");
                            //        _homefrm.UiText(OEE_Mqtt[item].StartTime.ToString("yyyy-MM-dd HH:mm:ss"), "txtOEE_StartTime");
                            //        _homefrm.UiText(OEE_Mqtt[item].EndTime.ToString("yyyy-MM-dd HH:mm:ss"), "txtOEE_EndTime");
                            //        _homefrm.UiText(OEE_Mqtt[item].Status.ToString(), "txtOEE_Status");
                            //        _homefrm.UiText(OEE_Mqtt[item].ActualCT.ToString(), "txtOEE_ActualCT");
                            //        _homefrm.UiText(OEE_Mqtt[item].SwVersion, "txtOEE_sw");
                            //        _homefrm.UiText(OEE_Mqtt[item].ScanCount, "txtOEE_ScanCount");
                            //        _homefrm.UiText(OEE_Mqtt[item].GUID.ToString(), "txtOEE_GUID");
                            //        Global.respond.TryRemove(OEE_Mqtt[item].GUID, out Global.Respond);
                            //        Out_oee_Mqtt.RemoveAt(0);
                            //        OEE_Mqtt.Remove(item);
                            //    }
                            //    else
                            //    {
                            //        Global.oee_ng++;
                            //        _homefrm.UpDatalabel(Global.oee_ng.ToString(), "lb_OEENG");
                            //        Global.oeeSend_ng++;
                            //        OEE_Default_flag = false;
                            //        Global.ConnectOEEFlag = false;
                            //        _homefrm.UpDatalabelcolor(Color.Red, "OEE_Default发送失败", "lb_OEE_UA_SendStatus");
                            //        _homefrm.AppendRichText(JsonConvert.SerializeObject(Global.respond[OEE_Mqtt[item].GUID]), "rtx_OEEDefaultMsg");
                            //        Log.WriteLog("OEE_Default返回结果-NG" + JsonConvert.SerializeObject(Global.respond[OEE_Mqtt[item].GUID]) + ",OEELog");
                            //        Log.WriteLog(Global.respond[OEE_Mqtt[item].GUID].GUID.ToString() + "," + Global.respond[OEE_Mqtt[item].GUID].Result + "," + Global.respond[OEE_Mqtt[item].GUID].ErrorCode, System.AppDomain.CurrentDomain.BaseDirectory + "\\MQTT返回NG\\");
                            //        Log.WriteLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + "OEE_Default" + "," + OEE_Mqtt[item].GUID.ToString() + "," + OEE_Mqtt[item].EMT + ","
                            //        + OEE_Mqtt[item].SerialNumber + "," + OEE_Mqtt[item].BGBarcode + "," + OEE_Mqtt[item].Fixture + "," + OEE_Mqtt[item].StartTime.ToString("yyyy-MM-dd HH:mm:ss") + ","
                            //        + OEE_Mqtt[item].EndTime.ToString("yyyy-MM-dd HH:mm:ss") + "," + OEE_Mqtt[item].Status + "," + OEE_Mqtt[item].ActualCT + ","
                            //        + OEE_Mqtt[item].SwVersion + "," + OEE_Mqtt[item].ScanCount + "," + OEE_Mqtt[item].ErrorCode + "," + OEE_Mqtt[item].PFErrorCode + ","
                            //        + OEE_Mqtt[item].Cavity + "," + OEE_Mqtt[item].ClientPcName + "," + OEE_Mqtt[item].MAC + ","
                            //        + OEE_Mqtt[item].IP + "," + OEE_Mqtt[item].EventTime.ToString("yyyy-MM-dd HH:mm:ss") + ","
                            //        + "NG-OEE_Default", System.AppDomain.CurrentDomain.BaseDirectory + "\\OEE_MQTT_Default数据NG\\");
                            //        _homefrm.UiText(OEE_Mqtt[item].SerialNumber, "txtOEE_SerialNumber");
                            //        _homefrm.UiText(OEE_Mqtt[item].Fixture, "txtOEE_Fixture");
                            //        _homefrm.UiText(OEE_Mqtt[item].StartTime.ToString("yyyy-MM-dd HH:mm:ss"), "txtOEE_StartTime");
                            //        _homefrm.UiText(OEE_Mqtt[item].EndTime.ToString("yyyy-MM-dd HH:mm:ss"), "txtOEE_EndTime");
                            //        _homefrm.UiText(OEE_Mqtt[item].Status.ToString(), "txtOEE_Status");
                            //        _homefrm.UiText(OEE_Mqtt[item].ActualCT.ToString(), "txtOEE_ActualCT");
                            //        _homefrm.UiText(OEE_Mqtt[item].SwVersion, "txtOEE_sw");
                            //        _homefrm.UiText(OEE_Mqtt[item].ScanCount, "txtOEE_ScanCount");
                            //        _homefrm.UiText(OEE_Mqtt[item].GUID.ToString(), "txtOEE_GUID");
                            //        Global.respond.TryRemove(OEE_Mqtt[item].GUID, out Global.Respond);
                            //        Out_oee_Mqtt.RemoveAt(0);
                            //        OEE_Mqtt.Remove(item);
                            //    }
                            //}
                            //else
                            //{
                            //    Global.ConnectOEEFlag = false;
                            //    OEE_Default_flag = false;
                            //    _homefrm.UpDatalabelcolor(Color.Red, "OEE_Default发送失败", "lb_OEE_UA_SendStatus");
                            //    Log.WriteLog("OEE过站数据超时8秒无反馈,GUID:" + OEE_Mqtt[item].GUID.ToString() + " ,OEELog");
                            //    Log.WriteLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + "OEE_Default" + "," + OEE_Mqtt[item].GUID.ToString() + "," + OEE_Mqtt[item].EMT + ","
                            //        + OEE_Mqtt[item].SerialNumber + "," + OEE_Mqtt[item].BGBarcode + "," + OEE_Mqtt[item].Fixture + "," + OEE_Mqtt[item].StartTime.ToString("yyyy-MM-dd HH:mm:ss") + ","
                            //        + OEE_Mqtt[item].EndTime.ToString("yyyy-MM-dd HH:mm:ss") + "," + OEE_Mqtt[item].Status + "," + OEE_Mqtt[item].ActualCT + ","
                            //        + OEE_Mqtt[item].SwVersion + "," + OEE_Mqtt[item].ScanCount + "," + OEE_Mqtt[item].ErrorCode + "," + OEE_Mqtt[item].PFErrorCode + ","
                            //        + OEE_Mqtt[item].Cavity + "," + OEE_Mqtt[item].ClientPcName + "," + OEE_Mqtt[item].MAC + ","
                            //        + OEE_Mqtt[item].IP + "," + OEE_Mqtt[item].EventTime.ToString("yyyy-MM-dd HH:mm:ss") + ","
                            //        + "NG-OEE_Default", System.AppDomain.CurrentDomain.BaseDirectory + "\\OEE_MQTT_Default数据NG\\");
                            //    string s = "insert into OEE_DefaultSendNG([DateTime],[GUID],[EMT],[SerialNumber],[BGBarcode ],[Fixture],[StartTime],[EndTime],[Status],[ActualCT],[SwVersion],[ScanCount],[ErrorCode ],[PFErrorCode ],[Cavity],[ClientPcName],[MAC],[IP],[EventTime])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" + ","
                            //        + "'" + OEE_Mqtt[item].GUID.ToString() + "'" + "," + "'" + OEE_Mqtt[item].EMT + "'" + "," + "'" + OEE_Mqtt[item].SerialNumber + "'" + ","
                            //        + "'" + OEE_Mqtt[item].BGBarcode + "'" + "," + "'" + OEE_Mqtt[item].Fixture + "'" + "," + "'" + OEE_Mqtt[item].StartTime.ToString("yyyy-MM-dd HH:mm:ss") + "'" + ","
                            //        + "'" + OEE_Mqtt[item].EndTime.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," + "'" + OEE_Mqtt[item].Status + "'" + "," + "'" + OEE_Mqtt[item].ActualCT + "'" + ","
                            //        + "'" + OEE_Mqtt[item].SwVersion + "'" + "," + "'" + OEE_Mqtt[item].ScanCount + "'" + "," + "'" + OEE_Mqtt[item].ErrorCode + "'" + ","
                            //        + "'" + OEE_Mqtt[item].PFErrorCode + "'" + "," + "'" + OEE_Mqtt[item].Cavity + "'" + "," + "'" + OEE_Mqtt[item].ClientPcName + "'" + ","
                            //        + "'" + OEE_Mqtt[item].MAC + "'" + "," + "'" + OEE_Mqtt[item].IP + "'" + "," + "'" + OEE_Mqtt[item].EventTime + "'" + ")";
                            //    int r = SQL.ExecuteUpdate(s);
                            //    _homefrm.UiText(OEE_Mqtt[item].SerialNumber, "txtOEE_SerialNumber");
                            //    _homefrm.UiText(OEE_Mqtt[item].Fixture, "txtOEE_Fixture");
                            //    _homefrm.UiText(OEE_Mqtt[item].StartTime.ToString("yyyy-MM-dd HH:mm:ss"), "txtOEE_StartTime");
                            //    _homefrm.UiText(OEE_Mqtt[item].EndTime.ToString("yyyy-MM-dd HH:mm:ss"), "txtOEE_EndTime");
                            //    _homefrm.UiText(OEE_Mqtt[item].Status.ToString(), "txtOEE_Status");
                            //    _homefrm.UiText(OEE_Mqtt[item].ActualCT.ToString(), "txtOEE_ActualCT");
                            //    _homefrm.UiText(OEE_Mqtt[item].SwVersion, "txtOEE_sw");
                            //    _homefrm.UiText(OEE_Mqtt[item].ScanCount, "txtOEE_ScanCount");
                            //    _homefrm.UiText(OEE_Mqtt[item].GUID.ToString(), "txtOEE_GUID");
                            //    Log.WriteLog(string.Format("插入了{0}行OEEE_MQTT_DefaultSendNG缓存数据", r) + ",OEELog");
                            //    Out_oee_Mqtt.RemoveAt(0);
                            //    OEE_Mqtt.Remove(item);
                            //}
                        }
                        else
                        {
                            Log.WriteLog("OEE过站数据产品码:" + item + "格式不对" + " ,OEELog");
                            _homefrm.AppendRichText("OEE_Default上传的SN格式不正确", "rtx_OEEDefaultMsg");
                            Out_oee_Mqtt.RemoveAt(0);
                            OEE_Mqtt.Remove(item);
                        }
                    }
                    catch (Exception EX)
                    {
                        Log.WriteLog("OEE异常" + EX.ToString());
                        Global.oee_ng++;
                        _homefrm.UpDatalabel(Global.oee_ng.ToString(), "lb_OEENG");
                        Global.oeeSend_ng++;
                        OEE_Default_flag = false;
                        Global.ConnectOEEFlag = false;
                        _homefrm.AppendRichText("OEE_Default-网络异常", "rtx_OEEDefaultMsg");
                        _homefrm.UpDatalabelcolor(Color.Red, "OEE_Default-网络异常", "lb_OEE_UA_SendStatus");
                        Log.WriteLog("MQTT上传OEE过站数据异常" + ",OEELog");
                        Log.WriteLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + "OEE_Default" + "," + OEE_Mqtt[item].GUID.ToString() + "," + OEE_Mqtt[item].EMT + ","
                                    + OEE_Mqtt[item].SerialNumber + "," + OEE_Mqtt[item].BGBarcode + "," + OEE_Mqtt[item].Fixture + "," + OEE_Mqtt[item].StartTime.ToString("yyyy-MM-dd HH:mm:ss") + ","
                                    + OEE_Mqtt[item].EndTime.ToString("yyyy-MM-dd HH:mm:ss") + "," + OEE_Mqtt[item].Status + "," + OEE_Mqtt[item].ActualCT + ","
                                    + OEE_Mqtt[item].SwVersion + "," + OEE_Mqtt[item].ScanCount + "," + OEE_Mqtt[item].ErrorCode + "," + OEE_Mqtt[item].PFErrorCode + ","
                                    + OEE_Mqtt[item].Cavity + "," + OEE_Mqtt[item].ClientPcName + "," + OEE_Mqtt[item].MAC + ","
                                    + OEE_Mqtt[item].IP + "," + OEE_Mqtt[item].EventTime.ToString("yyyy-MM-dd HH:mm:ss") + ","
                                    + "NG-OEE_Default", System.AppDomain.CurrentDomain.BaseDirectory + "\\OEE_MQTT_Default数据NG\\");
                        string s = "insert into OEE_DefaultSendNG([DateTime],[GUID],[EMT ],[SerialNumber],[BGBarcode ],[Fixture],[StartTime],[EndTime],[Status],[ActualCT],[SwVersion],[ScanCount],[ErrorCode ],[PFErrorCode ],[Cavity],[ClientPcName],[MAC],[IP],[EventTime])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" + ","
                            + "'" + OEE_Mqtt[item].GUID.ToString() + "'" + "," + "'" + OEE_Mqtt[item].EMT + "'" + "," + "'" + OEE_Mqtt[item].SerialNumber + "'" + ","
                            + "'" + OEE_Mqtt[item].BGBarcode + "'" + "," + "'" + OEE_Mqtt[item].Fixture + "'" + "," + "'" + OEE_Mqtt[item].StartTime.ToString("yyyy-MM-dd HH:mm:ss") + "'" + ","
                            + "'" + OEE_Mqtt[item].EndTime.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," + "'" + OEE_Mqtt[item].Status + "'" + "," + "'" + OEE_Mqtt[item].ActualCT + "'" + ","
                            + "'" + OEE_Mqtt[item].SwVersion + "'" + "," + "'" + OEE_Mqtt[item].ScanCount + "'" + "," + "'" + OEE_Mqtt[item].ErrorCode + "'" + ","
                            + "'" + OEE_Mqtt[item].PFErrorCode + "'" + "," + "'" + OEE_Mqtt[item].Cavity + "'" + "," + "'" + OEE_Mqtt[item].ClientPcName + "'" + ","
                            + "'" + OEE_Mqtt[item].MAC + "'" + "," + "'" + OEE_Mqtt[item].IP + "'" + "," + "'" + OEE_Mqtt[item].EventTime + "'" + ")";
                        int r = SQL.ExecuteUpdate(s);
                        _homefrm.UiText(OEE_Mqtt[item].SerialNumber, "txtOEE_SerialNumber");
                        _homefrm.UiText(OEE_Mqtt[item].Fixture, "txtOEE_Fixture");
                        _homefrm.UiText(OEE_Mqtt[item].StartTime.ToString("yyyy-MM-dd HH:mm:ss"), "txtOEE_StartTime");
                        _homefrm.UiText(OEE_Mqtt[item].EndTime.ToString("yyyy-MM-dd HH:mm:ss"), "txtOEE_EndTime");
                        _homefrm.UiText(OEE_Mqtt[item].Status.ToString(), "txtOEE_Status");
                        _homefrm.UiText(OEE_Mqtt[item].ActualCT.ToString(), "txtOEE_ActualCT");
                        _homefrm.UiText(OEE_Mqtt[item].SwVersion, "txtOEE_sw");
                        _homefrm.UiText(OEE_Mqtt[item].ScanCount, "txtOEE_ScanCount");
                        _homefrm.UiText(OEE_Mqtt[item].GUID.ToString(), "txtOEE_GUID");
                        Log.WriteLog(string.Format("插入了{0}行OEEE_MQTT_DefaultSendNG缓存数据", r) + ",OEELog");
                        OEEData OEEDa = new OEEData();
                        if (Global.respond.TryGetValue(OEE_Mqtt[item].GUID, out Global.Respond))
                        {
                            Global.respond.TryRemove(OEE_Mqtt[item].GUID, out Global.Respond);
                        }
                        if (Out_oee_Mqtt.Contains(item))
                        {
                            Out_oee_Mqtt.RemoveAt(0);
                        }
                        if (OEE_Mqtt.TryGetValue(item, out OEEDa))
                        {
                            OEE_Mqtt.Remove(item);
                        }

                    }
                    #endregion
                }
                Thread.Sleep(30);
            }
        }


        #endregion

        #region  OEE Default_NG上传
        private void EthOEEDefault_NG()
        {
            while (true)
            {
                if (OEE_NG.Count > 0)
                {
                    string item = Out_oee_NG[0];
                    string msg = "";
                    string OEE_Data = "";
                    OEE_NG[item].SwVersion = Global.inidata.productconfig.Sw_version;
                    try
                    {
                        if (item.Contains("FM") && item.Length == 19 && !Regex.IsMatch(item, "[a-z]"))
                        {

                            string SelectStr = string.Format("SELECT * FROM HansData WHERE SN='{0}' and Station='L_Bracket'", item);//sql查询语句
                            DataTable d1 = SQL.ExecuteQuery(SelectStr);
                            if (OEE_NG[item].Status == "OK")
                            {
                                if (d1 != null && d1.Rows.Count > 3)
                                {
                                    OEE_NG[item].Status = "OK";
                                }
                                else
                                {

                                    OEE_NG[item].Status = "NG";
                                }
                            }
                            if (OEE_NG[item].SerialNumber == "" || OEE_NG[item].Fixture == "")
                            {
                                OEE_Data = string.Format("{{\"SerialNumber\":\"{0}\",\"BGBarcode\":\"{1}\",\"Fixture\":\"{2}\",\"StartTime\":\"{3}\",\"EndTime\":\"{4}\",\"Status\":\"{5}\",\"ActualCT\":\"{6}\",\"SwVersion\":\"{7}\",\"ScanCount\":\"{8}\"}}", DateTime.Now.ToString("yyyyMMddHHmmss"), "", "", OEE_NG[item].StartTime.ToString("yyyy-MM-dd HH:mm:ss"), OEE_NG[item].EndTime.ToString("yyyy-MM-dd HH:mm:ss"), OEE_NG[item].Status, OEE_NG[item].ActualCT, OEE_NG[item].SwVersion, "1");
                            }
                            else
                            {
                                OEE_Data = string.Format("{{\"SerialNumber\":\"{0}\",\"BGBarcode\":\"{1}\",\"Fixture\":\"{2}\",\"StartTime\":\"{3}\",\"EndTime\":\"{4}\",\"Status\":\"{5}\",\"ActualCT\":\"{6}\",\"SwVersion\":\"{7}\",\"ScanCount\":\"{8}\"}}", OEE_NG[item].SerialNumber, "", OEE_NG[item].Fixture, OEE_NG[item].StartTime.ToString("yyyy-MM-dd HH:mm:ss"), OEE_NG[item].EndTime.ToString("yyyy-MM-dd HH:mm:ss"), OEE_NG[item].Status, OEE_NG[item].ActualCT, OEE_NG[item].SwVersion, "1");
                            }
                            Log.WriteLog("OEE_Default:" + OEE_Data + ",OEELog");
                            _homefrm.AppendRichText(OEE_Data, "rtx_OEEDefaultMsg");
                            var IP = GetIp();
                            var Mac = GetOEEMac();
                            var rst = RequestAPI.Request(Global.inidata.productconfig.OEE_URL1, Global.inidata.productconfig.OEE_URL2, IP, Mac, Global.inidata.productconfig.OEE_Dsn, Global.inidata.productconfig.OEE_authCode, 1, OEE_Data, out msg);
                            if (rst)
                            {
                                Global.oee_ok++;
                                _homefrm.UpDatalabel(Global.oee_ok.ToString(), "lb_OEEOK");
                                Global.oeeSend_ng = 0;
                                OEE_Default_flag = true;
                                _homefrm.AppendRichText(msg, "rtx_OEEDefaultMsg");
                                _homefrm.UpDatalabelcolor(Color.Green, "OEE_Default发送成功", "lb_OEE_UA_SendStatus");
                                Global.ConnectOEEFlag = true;
                                Global.PLC_Client.WritePLC_D(10504, new short[] { 1 });
                                Log.WriteLog("OEE_Default返回结果-OK" + msg + ",OEELog");
                                Log.WriteCSV(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + "OEE_Default" + "," + OEE_NG[item].SerialNumber + "," + OEE_NG[item].Fixture + "," + OEE_NG[item].StartTime + "," + OEE_NG[item].EndTime + "," + OEE_NG[item].Status + "," + OEE_NG[item].ActualCT + "," + OEE_NG[item].SwVersion + "," + "1" + "," + "OK-OEE_Default", System.AppDomain.CurrentDomain.BaseDirectory + "\\OEE_Default数据\\");
                                _homefrm.UiText(OEE_NG[item].SerialNumber, "txtOEE_SerialNumber");
                                _homefrm.UiText(OEE_NG[item].Fixture, "txtOEE_Fixture");
                                _homefrm.UiText(OEE_NG[item].StartTime.ToString("yyyy-MM-dd HH:mm:ss"), "txtOEE_StartTime");
                                _homefrm.UiText(OEE_NG[item].EndTime.ToString("yyyy-MM-dd HH:mm:ss"), "txtOEE_EndTime");
                                _homefrm.UiText(OEE_NG[item].Status.ToString(), "txtOEE_Status");
                                _homefrm.UiText(OEE_NG[item].ActualCT.ToString(), "txtOEE_ActualCT");
                                _homefrm.UiText(OEE_NG[item].SwVersion, "txtOEE_sw");
                                _homefrm.UiText(OEE_NG[item].ScanCount, "txtOEE_ScanCount");
                                Out_oee_NG.RemoveAt(0);
                                OEE_NG.Remove(item);
                                Log.WriteLog("OEE_Default_UI更新成功" + ",OEELog");
                            }
                            else
                            {
                                Global.oee_ng++;
                                _homefrm.UpDatalabel(Global.oee_ng.ToString(), "lb_OEENG");
                                //if (Convert.ToDateTime(DateTime.Now.ToString("HH:mm")).CompareTo(Convert.ToDateTime("6:00")) >= 0 && Convert.ToDateTime(DateTime.Now.ToString("HH:mm")).CompareTo(Convert.ToDateTime("18:00")) < 0)
                                //{
                                //    Global.OEEUpLoad_Error_D++;//白班OEE上传异常计数累加
                                //    _datastatisticsfrm.UpDataText(Global.OEEUpLoad_Error_D.ToString(), "txt_OEEUpLoad_Error_D");
                                //    Global.inidata.productconfig.OEEUpLoad_Error_D = Global.OEEUpLoad_Error_D.ToString();
                                //    Global.inidata.WriteProductnumSection();
                                //}
                                //else
                                //{
                                //    Global.OEEUpLoad_Error_N++;//夜班OEE上传异常计数累加
                                //    _datastatisticsfrm.UpDataText(Global.OEEUpLoad_Error_N.ToString(), "txt_OEEUpLoad_Error_N");
                                //    Global.inidata.productconfig.OEEUpLoad_Error_N = Global.OEEUpLoad_Error_N.ToString();
                                //    Global.inidata.WriteProductnumSection();
                                //}
                                Global.oeeSend_ng++;
                                //_homefrm.UpDatalabel(Global.oeeSend_ng.ToString(), "lb_OEESendNG");
                                OEE_Default_flag = false;
                                _homefrm.AppendRichText(msg, "rtx_OEEDefaultMsg");
                                _homefrm.UpDatalabelcolor(Color.Green, "OEE_Default发送失败", "lb_OEE_UA_SendStatus");
                                Global.ConnectOEEFlag = false;
                                Global.PLC_Client.WritePLC_D(10504, new short[] { 2 });
                                Log.WriteLog("OEE_Default返回结果-NG" + msg + ",OEELog");
                                //Log.WriteCSV_NG(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + "OEE_Default" + "," + OEE_NG[item].SerialNumber + "," + OEE_NG[item].Fixture + "," + OEE_NG[item].StartTime.ToString("yyyy-MM-dd HH:mm:ss") + "," + OEE_NG[item].EndTime.ToString("yyyy-MM-dd HH:mm:ss") + "," + OEE_NG[item].Status + "," + OEE_NG[item].ActualCT + "," + OEE_NG[item].SwVersion + "," + OEE_NG[item].ScanCount + "," + "NG-OEE_Default", System.AppDomain.CurrentDomain.BaseDirectory + "\\OEE_DefaultSendNG数据\\");
                                string s = "insert into OEE_DefaultSendNG([DateTime],[SerialNumber],[Fixture],[StartTime],[EndTime],[Status],[ActualCT],[SwVersion],[ScanCount])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," + "'" + OEE_NG[item].SerialNumber + "'" + "," + "'" + OEE_NG[item].Fixture + "'" + ","
                                                     + "'" + OEE_NG[item].StartTime.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," + "'" + OEE_NG[item].EndTime.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," + "'" + OEE_NG[item].Status + "'" + "," + "'" + OEE_NG[item].ActualCT + "'" + "," + "'" + OEE_NG[item].SwVersion + "'" + "," + "'" + "1" + "'" + ")";
                                int r = SQL.ExecuteUpdate(s);
                                Log.WriteLog(string.Format("插入了{0}行OEEData_SendNG缓存数据", r) + ",OEELog");
                                Log.WriteCSV(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + "OEE_Default" + "," + OEE_NG[item].SerialNumber + "," + OEE_NG[item].Fixture + "," + OEE_NG[item].StartTime.ToString("yyyy-MM-dd HH:mm:ss") + "," + OEE_NG[item].EndTime.ToString("yyyy-MM-dd HH:mm:ss") + "," + OEE_NG[item].Status + "," + OEE_NG[item].ActualCT + "," + OEE_NG[item].SwVersion + "," + "1" + "," + "NG-OEE_Default", System.AppDomain.CurrentDomain.BaseDirectory + "\\OEE_Default数据\\");
                                Log.WriteCSV(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + "OEE_Default" + "," + OEE_NG[item].SerialNumber + "," + OEE_NG[item].Fixture + "," + OEE_NG[item].StartTime.ToString("yyyy-MM-dd HH:mm:ss") + "," + OEE_NG[item].EndTime.ToString("yyyy-MM-dd HH:mm:ss") + "," + OEE_NG[item].Status + "," + OEE_NG[item].ActualCT + "," + OEE_NG[item].SwVersion + "," + "1" + "," + "NG-OEE_Default", System.AppDomain.CurrentDomain.BaseDirectory + "\\OEE_Default上传NG数据\\");
                                Out_oee_NG.RemoveAt(0);
                                OEE_NG.Remove(item);
                            }
                        }
                        else
                        {
                            Log.WriteLog("OEE_Default上传的SN格式不正确:" + item + ",OEELog");
                            _homefrm.AppendRichText("OEE_Default上传的SN格式不正确", "rtx_OEEDefaultMsg");
                            Global.PLC_Client.WritePLC_D(10504, new short[] { 2 });
                            Out_oee_NG.RemoveAt(0);
                            OEE_NG.Remove(item);
                        }
                    }
                    catch
                    {
                        Global.oee_ng++;
                        _homefrm.UpDatalabel(Global.oee_ng.ToString(), "lb_OEENG");
                        //if (Convert.ToDateTime(DateTime.Now.ToString("HH:mm")).CompareTo(Convert.ToDateTime("6:00")) >= 0 && Convert.ToDateTime(DateTime.Now.ToString("HH:mm")).CompareTo(Convert.ToDateTime("18:00")) < 0)
                        //{
                        //    Global.OEEUpLoad_Error_D++;//白班OEE上传异常计数累加
                        //    _datastatisticsfrm.UpDataText(Global.OEEUpLoad_Error_D.ToString(), "txt_OEEUpLoad_Error_D");
                        //    Global.inidata.productconfig.OEEUpLoad_Error_D = Global.OEEUpLoad_Error_D.ToString();
                        //    Global.inidata.WriteProductnumSection();
                        //}
                        //else
                        //{
                        //    Global.OEEUpLoad_Error_N++;//夜班OEE上传异常计数累加
                        //    _datastatisticsfrm.UpDataText(Global.OEEUpLoad_Error_N.ToString(), "txt_OEEUpLoad_Error_N");
                        //    Global.inidata.productconfig.OEEUpLoad_Error_N = Global.OEEUpLoad_Error_N.ToString();
                        //    Global.inidata.WriteProductnumSection();
                        //}
                        Global.oeeSend_ng++;
                        //_homefrm.UpDatalabel(Global.oeeSend_ng.ToString(), "lb_OEESendNG");
                        OEE_Default_flag = false;
                        _homefrm.AppendRichText("OEE_Default-网络异常", "rtx_OEEDefaultMsg");
                        _homefrm.UpDatalabelcolor(Color.Green, "OEE_Default-网络异常", "lb_OEE_UA_SendStatus");
                        Global.PLC_Client.WritePLC_D(10504, new short[] { 2 });
                        Log.WriteLog("数据发送OEE_Default异常_fail" + ",OEELog");
                        //Log.WriteCSV_NG(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + "OEE_Default" + "," + OEE_NG[item].SerialNumber + "," + OEE_NG[item].Fixture + "," + OEE_NG[item].StartTime.ToString("yyyy-MM-dd HH:mm:ss") + "," + OEE_NG[item].EndTime.ToString("yyyy-MM-dd HH:mm:ss") + "," + OEE_NG[item].Status + "," + OEE_NG[item].ActualCT + "," + OEE_NG[item].SwVersion + "," + OEE_NG[item].ScanCount + "," + "NG-OEE_Default网络异常", System.AppDomain.CurrentDomain.BaseDirectory + "\\OEE_DefaultSendNG数据\\");
                        string s = "insert into OEE_DefaultSendNG([DateTime],[SerialNumber],[Fixture],[StartTime],[EndTime],[Status],[ActualCT],[SwVersion],[ScanCount])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," + "'" + OEE_NG[item].SerialNumber + "'" + "," + "'" + OEE_NG[item].Fixture + "'" + ","
                                                      + "'" + OEE_NG[item].StartTime.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," + "'" + OEE_NG[item].EndTime.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," + "'" + OEE_NG[item].Status + "'" + "," + "'" + OEE_NG[item].ActualCT + "'" + "," + "'" + OEE_NG[item].SwVersion + "'" + "," + "'" + "1" + "'" + ")";
                        int r = SQL.ExecuteUpdate(s);
                        Log.WriteLog(string.Format("插入了{0}行OEEData_SendNG缓存数据", r) + ",OEELog");
                        Log.WriteCSV(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + "OEE_Default" + "," + OEE_NG[item].SerialNumber + "," + OEE_NG[item].Fixture + "," + OEE_NG[item].StartTime.ToString("yyyy-MM-dd HH:mm:ss") + "," + OEE_NG[item].EndTime.ToString("yyyy-MM-dd HH:mm:ss") + "," + OEE_NG[item].Status + "," + OEE_NG[item].ActualCT + "," + OEE_NG[item].SwVersion + "," + OEE_NG[item].ScanCount + "," + "NG-OEE_Default网络异常", System.AppDomain.CurrentDomain.BaseDirectory + "\\OEE_Default数据\\");
                        Log.WriteCSV(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + "OEE_Default" + "," + OEE_NG[item].SerialNumber + "," + OEE_NG[item].Fixture + "," + OEE_NG[item].StartTime.ToString("yyyy-MM-dd HH:mm:ss") + "," + OEE_NG[item].EndTime.ToString("yyyy-MM-dd HH:mm:ss") + "," + OEE_NG[item].Status + "," + OEE_NG[item].ActualCT + "," + OEE_NG[item].SwVersion + "," + OEE_NG[item].ScanCount + "," + "NG-OEE_Default网络异常", System.AppDomain.CurrentDomain.BaseDirectory + "\\OEE_Default上传NG数据\\");
                        Out_oee_NG.RemoveAt(0);
                        OEE_NG.Remove(item);
                    }
                }
                Thread.Sleep(30);
            }
        }


        #endregion


        #region  OEE Machine上传
        public void txtFixtureNumber_TextChanged(object sender, EventArgs e)
        {
            string OEEMachine = "";
            string OEEMachineMsg = "";
            string FixtureNumber = Global.inidata.productconfig.FixtureNumber;
            try
            {
                var IP = GetIp();
                var Mac = GetOEEMac();
                OEEMachine = string.Format("{{\"FixtureNum\":\"{0}\"}}", FixtureNumber);
                var rst = RequestAPI.Request(Global.inidata.productconfig.OEE_URL1, Global.inidata.productconfig.OEE_URL2, IP, Mac, Global.inidata.productconfig.OEE_Dsn, Global.inidata.productconfig.OEE_authCode, 3, OEEMachine, out OEEMachineMsg);
                Log.WriteLog("OEE_Machine：" + OEEMachine);
                if (rst)
                {
                    _homefrm.AppendRichText("OEE_Machine上传成功" + ":  " + OEEMachineMsg, "rtx_HeartBeatMsg");
                    Log.WriteLog("OEE_Machine上传OK:" + OEEMachineMsg);
                    Global.ConnectOEEFlag = true;
                    Log.WriteCSV(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + "OEE_Machine" + "," + FixtureNumber + "," + "OK-OEE_Machine", System.AppDomain.CurrentDomain.BaseDirectory + "\\OEE_Machine数据\\");
                }
                else
                {
                    _homefrm.AppendRichText("OEE_Machine上传失败" + ":  " + OEEMachineMsg, "rtx_HeartBeatMsg");
                    Log.WriteLog("OEE_Machine上传NG:" + OEEMachineMsg);
                    Global.ConnectOEEFlag = false;
                    Log.WriteCSV(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + "OEE_Machine" + "," + FixtureNumber + "," + "NG-OEE_Machine", System.AppDomain.CurrentDomain.BaseDirectory + "\\OEE_Machine上传NG数据\\");
                }
            }
            catch
            {
                _homefrm.AppendRichText("OEE_Machine上传异常,请检查OEE网络！" + ":  " + OEEMachineMsg, "rtx_HeartBeatMsg");
                Log.WriteLog("OEE_Machine上传异常");
                Log.WriteCSV(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + "OEE_Machine" + "," + FixtureNumber + "," + "NG-OEE_Machine网络异常", System.AppDomain.CurrentDomain.BaseDirectory + "\\OEE_Machine上传NG数据\\");
            }
        }
        #endregion
        #region  OEE NG抛料日志上传上传
        private void EthOEEDiscardLog()
        {
            //JsonSerializerSettings jsetting = new JsonSerializerSettings();
            //jsetting.NullValueHandling = NullValueHandling.Ignore;//Json不输出空值
            //while (true)
            //{
            //    if (Out_oee_discard.Count > 0)
            //    {
            //        if (OEE_discard.Count > 0)
            //        {
            //            string msg = "";
            //            string item = Out_oee_discard[0];
            //            string uptime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            //            try
            //            {
            //                DiscardData NG_data = new DiscardData();
            //                NG_data.sn = OEE_discard[item].SN;
            //                NG_data.SystemType = OEE_discard[item].SystemType;
            //                NG_data.uptime = uptime;
            //                NG_data.JSONBody = OEE_discard[item].JSONBody;
            //                NG_data.error = OEE_discard[item].Error;
            //                NG_data.frenquency = OEE_discard[item].Frenquency;
            //                string OEE_NG_log = JsonConvert.SerializeObject(NG_data, Formatting.None, jsetting);
            //                Log.WriteLog("OEE_DiscardLog:" + OEE_NG_log);
            //                var IP = GetIp();
            //                var Mac = GetMac();
            //                var rst = RequestAPI.Request(Global.inidata.productconfig.OEE_URL1, Global.inidata.productconfig.OEE_URL2, IP, Mac, Global.inidata.productconfig.OEE_Dsn, Global.inidata.productconfig.OEE_authCode, 5, OEE_NG_log, out msg);
            //                if (rst)
            //                {
            //                    Log.WriteLog("OEE_DiscardLog上传OK:" + msg);
            //                    Global.ConnectOEEFlag = true;
            //                    Log.WriteCSV(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + item.Split(',')[0] + "," + OEE_discard[item].SystemType + "," + uptime + "," + OEE_discard[item].JSONBody + "," + OEE_discard[item].Error + "," + OEE_discard[item].Frenquency + "," + "OK-OEE_DiscardLog", System.AppDomain.CurrentDomain.BaseDirectory + "\\OEE_抛料日志数据\\");
            //                }
            //                else
            //                {
            //                    Log.WriteLog("OEE_DiscardLog上传NG:" + msg);
            //                    Global.ConnectOEEFlag = false;
            //                    Log.WriteCSV(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + item.Split(',')[0] + "," + OEE_discard[item].SystemType + "," + uptime + "," + OEE_discard[item].JSONBody + "," + OEE_discard[item].Error + "," + OEE_discard[item].Frenquency + "," + "NG-OEE_DiscardLog", System.AppDomain.CurrentDomain.BaseDirectory + "\\OEE_抛料日志数据\\");
            //                }
            //                Out_oee_discard.RemoveAt(0);
            //                OEE_discard.Remove(item);
            //            }
            //            catch (Exception ex)
            //            {
            //                Log.WriteLog("发送DiscardLog异常_fail" + ex.ToString().Replace("\n",""));
            //                Log.WriteCSV(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + item.Split(',')[0] + "," + OEE_discard[item].SystemType + "," + uptime + "," + OEE_discard[item].JSONBody + "," + OEE_discard[item].Error + "," + OEE_discard[item].Frenquency + "," + "NG-OEE_DiscardLog", System.AppDomain.CurrentDomain.BaseDirectory + "\\OEE_抛料日志数据\\");
            //                Out_oee_discard.RemoveAt(0);
            //                OEE_discard.Remove(item);
            //            }
            //        }
            //    }
            //    Thread.Sleep(200);
            //}
        }

        #endregion
        #region  OEE 小料抛料计数上传
        private void EthOEEMateriel()
        {
            JsonSerializerSettings jsetting = new JsonSerializerSettings();
            jsetting.NullValueHandling = NullValueHandling.Ignore;//Json不输出空值
            while (true)
            {
                string msg = "";
                MaterielData data = new MaterielData();
                Global.TotalThrowCount = Global.TotalThrowCount + Global.ThrowCount + Global.ThrowOKCount;
                try
                {
                    data.date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    data.count = (Global.NutCount).ToString();
                    data.totalcount = Global.TotalThrowCount.ToString();
                    data.parttype = string.Format("{0}", Global.NutCount);
                    _homefrm.UpDatalabel(Global.TotalThrowCount.ToString(), "lb_Materiel_Total");
                    string Materiel_data = JsonConvert.SerializeObject(data, Formatting.None, jsetting);
                    Log.WriteLog("OEE_MaterielData:" + Materiel_data + ",OEELog");
                    _homefrm.AppendRichText(Materiel_data, "rtx_OEEMateriel");
                    var IP = GetIp();
                    var Mac = GetOEEMac();
                    var rst = RequestAPI.Request(Global.inidata.productconfig.OEE_URL1, Global.inidata.productconfig.OEE_URL2, IP, Mac, Global.inidata.productconfig.OEE_Dsn, Global.inidata.productconfig.OEE_authCode, 6, Materiel_data, out msg);
                    if (rst)
                    {
                        _homefrm.AppendRichText(Materiel_data, msg);
                        Global.ConnectOEEFlag = true;
                        Log.WriteLog("OEE_MaterielData上传OK:" + msg + ",OEELog");
                        Log.WriteCSV(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + data.count + "," + data.totalcount + "," + data.parttype + "," + "OK-OEE_MaterielData", System.AppDomain.CurrentDomain.BaseDirectory + "\\OEE_小料抛料计数数据\\");
                    }
                    else
                    {
                        _homefrm.AppendRichText(Materiel_data, msg);
                        Global.ConnectOEEFlag = false;
                        Log.WriteLog("OEE_MaterielData上传NG:" + msg + ",OEELog");
                        Log.WriteCSV(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + data.count + "," + data.totalcount + "," + data.parttype + "," + "NG-OEE_MaterielData", System.AppDomain.CurrentDomain.BaseDirectory + "\\OEE_小料抛料计数数据\\");
                        //Access.InsertData_OEE_Materiel("PDCA", "OEE_MaterielData", data.date, data.count, data.totalcount, UACount.ToString(), LACount.ToString());
                    }
                }
                catch (Exception ex)
                {
                    Log.WriteLog("发送MaterielData异常_fail" + ex.ToString().Replace("\n", "") + ",OEELog");
                    Log.WriteCSV(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + data.count + "," + data.totalcount + "," + data.parttype + "," + "NG-OEE_MaterielData", System.AppDomain.CurrentDomain.BaseDirectory + "\\OEE_小料抛料计数数据\\");
                }
                _homefrm.UpDatalabel("0", "lb_Materiel_AllNut");
                _homefrm.UpDatalabel("0", "lb_Materiel_Nut");
                _homefrm.UpDatalabel("0", "lb_Materiel_AllOK");
                _homefrm.UpDatalabel("0", "lb_Materiel_OK");
                Thread.Sleep(60000);
            }
        }
        #endregion

        # region 数据操作
        private bool GetHSGData(object obj, out string HSGedition)//通过band_sn 获取SP_sn
        {
            ThreadInfo threadInfo = obj as ThreadInfo;
            try
            {
                string callresult = "";
                string errmsg = "";
                string TMP;
                JsonSerializerSettings jsetting = new JsonSerializerSettings();
                jsetting.NullValueHandling = NullValueHandling.Ignore;//Json不输出空值
                string url = string.Format("http://17.239.116.110/api/v2/parts?serial_type=band&serial={0}&process_name=bd-bc-le&last_log=true", threadInfo.SN);
                var rst = RequestAPI2.CallBobcat5(url, "", out callresult, out errmsg);
                _homefrm.AppendRichText(callresult + errmsg, "rtx_ProcessControl");
                GetSNData HSG = JsonConvert.DeserializeObject<GetSNData>(callresult);
                string SendData = JsonConvert.SerializeObject(HSG, Formatting.None, jsetting);
                Log.WriteLog("通过band " + threadInfo.SN + "获取HSG数据:" + SendData);
                if (rst)
                {
                    if (HSG.properties.DOE != "Y")
                    {
                        if (HSG.properties.Sidefire != "" && HSG.properties.Sidefire != null)
                        {
                            if (HSG.properties.Sidefire == "NSF")//获取到的HSG版本  //NSF 4G   
                            {

                                Log.WriteLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss: ") + "," + threadInfo.SN + "," + HSG.serials.sp, System.AppDomain.CurrentDomain.BaseDirectory + "band 获取 HSG 结果\\");
                                HSGedition = "4G";
                                return true;
                            }
                            else if (HSG.properties.Sidefire == "SF")//SF 5G
                            {
                                Log.WriteLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss: ") + "," + threadInfo.SN + "," + HSG.serials.sp, System.AppDomain.CurrentDomain.BaseDirectory + "band 获取 HSG 结果\\");
                                HSGedition = "5G";
                                return true;
                            }
                            else
                            {
                                Log.WriteLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss: ") + "," + threadInfo.SN + "," + HSG.serials.sp, System.AppDomain.CurrentDomain.BaseDirectory + "band 获取 HSG 结果\\");
                                HSGedition = "NG";
                                return false;
                            }
                        }
                        else
                        {
                            Log.WriteLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss: ") + "," + threadInfo.SN + "," + HSG.serials.sp, System.AppDomain.CurrentDomain.BaseDirectory + "band 获取 HSG 结果\\");
                            HSGedition = "NG";
                            return false;
                        }
                    }
                    else
                    {
                        Log.WriteLog("首件料跳过 OKstart 验证");
                        HSGedition = "NG-doey";
                        return false;
                    }
                }
                else
                {

                    Log.WriteLog("通过band 获取HSG失败,通过band_sn: " + threadInfo.SN + "获取HSG数据失败异常为:" + errmsg);
                    HSGedition = "NG";
                    return false;
                }
            }
            catch (Exception EX)
            {
                Log.WriteLog("通过band获取HSG失败" + EX.ToString());
                if (Global.SelectFirst == false)//收件时抛料不计数
                {
                    if (Convert.ToDateTime(DateTime.Now.ToString("HH:mm")).CompareTo(Convert.ToDateTime("6:00")) >= 0 && Convert.ToDateTime(DateTime.Now.ToString("HH:mm")).CompareTo(Convert.ToDateTime("18:00")) < 0)
                    {
                        Global.HSG_NG_D++;
                        _datastatisticsfrm.UpDataDGV_D(16, 1, Global.HSG_NG_D.ToString());
                        Global.inidata.productconfig.Band_NG_D = Global.HSG_NG_D.ToString();
                    }
                    else
                    {
                        Global.HSG_NG_N++;
                        _datastatisticsfrm.UpDataDGV_N(16, 1, Global.HSG_NG_N.ToString());
                        Global.inidata.productconfig.Band_NG_N = Global.HSG_NG_N.ToString();
                    }
                }
                HSGedition = "NG";
                return false;
            }
        }
        private void ProcessControl(object obj)//Trace前站校验
        {
            ProcessControlData Msg_ua;
            Trace_str_ua = "";
            string callResult = "";
            string errMsg = "";
            string callResult1 = "";
            string errMsg1 = "";
            string HSGedition = string.Empty;
            bool IFactory = true;
            bool PhaseResult = true;
            ThreadInfo threadInfo = (ThreadInfo)obj;
            Global.PLC_Client2.WritePLC_D(10122, new short[] { 0 });
            _homefrm.AppendRichText("SendProcessControl_SN:" + threadInfo.SN, "rtx_ProcessControl");
            try
            {
                for (int i = 0; i < Global.SN_string.Count; i++)
                {
                    if (threadInfo.SN == Global.SN_string[i])
                    {
                        Global.PLC_Client2.WritePLC_D(10122, new short[] { 2 });
                        Log.WriteLog(threadInfo.SN + "  重码");
                        Global.Replace = true;
                        break;
                    }
                }
                if (Global.SN_string.Count > 10)
                {
                    Global.SN_string.RemoveAt(0);
                }
            }
            catch (Exception ex)
            {
                Global.Replace = false;
                Log.WriteLog("前站 " + ex.ToString());
            }

            try
            {

                if (threadInfo.SN.Remove(2) == "FM" && !Global.Replace)
                {

                    // 2023 - 11 - 22 添加前站重码
                    Global.SN_string.Add(threadInfo.SN);
                    Global.Replace = false;
                    ///20230311 updated
                    if (Global.inidata.productconfig.Material_online != "1")
                    {
                        string callResultGetLinkData, errMsgGetLinkData;
                        bool resultGetLinkData = RequestAPI2.CallBobcat($"http://10.128.10.7/Webapi/api/IFactory/GetLinkData?SerialNumber={threadInfo.SN}&Customer=Atlanta-CTU-Housing", "", out callResultGetLinkData, out errMsgGetLinkData);
                        if (resultGetLinkData)
                        {
                            Global.linkDatas = JsonConvert.DeserializeObject<linkDatass>(callResultGetLinkData);
                            foreach (var item in Global.GySelected)
                            {
                                var ComparedType = Global.linkDatas?.linkData.Where(t => t.linkObj == item.Value.LinkObj).FirstOrDefault();
                                if (ComparedType != null)
                                {
                                    string substr = ComparedType.linkValue.Substring(int.Parse(item.Value.Start) - 1, int.Parse(item.Value.End) - int.Parse(item.Value.Start) + 1);
                                    if (substr != item.Value.DefaultValue)
                                    {
                                        //MessageBox.Show("与规则不匹配!");
                                        Global.PLC_Client.WritePLC_D(12601, new short[] { 2 });
                                        _homefrm.UpDatalabelcolor(Color.Red, "校验物料规则不匹配", "txt_ProcessControl_Status");
                                        _homefrm.AppendRichText($"校验物料规则不匹配:{substr}!={item.Value.DefaultValue},{threadInfo.SN}", "rtx_ProcessControl");
                                        Global.Trace_process_ng++;
                                        _homefrm.UpDatalabel(Global.Trace_process_ng.ToString(), "lb_ProcessControlNG");
                                        Product_num_Mes_NG++;
                                        Log.WriteLog($"校验物料规则不匹配:{substr}!={item.Value.DefaultValue},{threadInfo.SN}, TraceLog");
                                        Log.WriteCSV(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + "" + "," + threadInfo.SN + "," + "," + "," + "," + "," + "," + "," + "," + $"校验物料规则不匹配:{substr}!={item.Value.DefaultValue},{threadInfo.SN}", System.AppDomain.CurrentDomain.BaseDirectory + "\\前站Trace/JP异常数据\\");
                                    }
                                }
                            }

                        }
                    }
                    if (Global.inidata.productconfig.IFactory_online != "1")//IFactory前站校验是否屏蔽
                    {

                        bool GetHSGeditionResult = GetHSGData(obj, out HSGedition);//先获取HSG版本才能确定 oktostart 接口
                        string IFactoryURL = string.Empty;
                        if (GetHSGeditionResult)
                        {
                            if (HSGedition == "4G")
                            {
                                //IFactoryURL = string.Format("http://10.128.10.7/Webapi/api/IFactory/OkToStart?Customer=Tampa-CTU-Housing&Resource=Tampa-B3Ground-Tab&Route=Tampa-Housing-V1-1&RouteStep=Tampa-B3Ground-Tab&SerialNumber={0}&Factory=Metal-OP2-Tampa&MaterialName=Metal-Tampa-Housing", threadInfo.SN);
                                IFactoryURL = Global.inidata.productconfig.OktoStart4G_URL.Replace("{0}", threadInfo.SN);
                            }
                            else
                            {
                                //IFactoryURL = string.Format("http://10.128.10.7/Webapi/api/IFactory/OkToStart?Customer=Tampa-CTU-Housing&Resource=Tampa-B3Ground(5G)-Tab&Route=Tampa-Housing-V1-1&RouteStep=Tampa-B3Ground(5G)-Tab&SerialNumber={0}&Factory=Metal-OP2-Tampa&MaterialName=Metal-Tampa-Housing", threadInfo.SN);
                                IFactoryURL = Global.inidata.productconfig.OktoStart5G_URL.Replace("{0}", threadInfo.SN);
                            }
                            var result = RequestAPI2.CallBobcat4(IFactoryURL, "", out callResult, out errMsg, false);//进行JGP IFactory前站校验
                            Log.WriteCSV(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + threadInfo.SN + "," + callResult + "," + errMsg, System.AppDomain.CurrentDomain.BaseDirectory + "\\IFactory前站校验\\");
                            IFactory = callResult.Contains("pass");
                            //try
                            //{
                            //    IFactoryURL = string.Format("http://10.128.10.7/Webapi/api/IFactory/OkToStart?Customer=Atlanta-CTU-Housing&Resource=Atlanta-BracketW-In&Route=Atlanta-Housing-V1-1&RouteStep=Atlanta-BracketW-In&SerialNumber={0}&Factory=Metal-OP2-Atlanta&MaterialName=Metal-Atlanta-Housing", threadInfo.SN);
                            //    result = RequestAPI2.CallBobcat4(IFactoryURL, "", out callResult1, out errMsg1, false);
                            //    Log.WriteCSV(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + threadInfo.SN + "," + callResult1 + "," + errMsg1, System.AppDomain.CurrentDomain.BaseDirectory + "\\OkToStart前站校验\\");
                            //    //IFactory = !(callResult.Contains("pass"));
                            //}
                            //catch (Exception ex)
                            //{ Log.WriteLog("新接口 " + ex.ToString()); }


                        }
                        else//获取HSG版本失败 oktostart 不进行校验直接抛料
                        {
                            if (HSGedition == "NG-doey") //首件跳过Okstart 验证接口
                            {
                                IFactory = true;
                            }
                            else
                            { IFactory = false; }
                        }
                    }
                    if (HSGedition == "NG-doey") //首件跳过Okstart 验证接口
                    {
                        _homefrm.UpDatalabelcolor(Color.Green, "Trace校验SN成功", "txt_ProcessControl_Status");
                        Global.Trace_process_ok++;
                        _homefrm.UpDatalabel(Global.Trace_process_ok.ToString(), "lb_ProcessControlOK");
                        _homefrm.AppendRichText(Trace_str_ua, "rtx_ProcessControl");
                        Global.PLC_Client2.WritePLC_D(10122, new short[] { 1 });
                        _homefrm.AppendRichText("N/A", "rtx_ProcessControlErrorMsg");
                        Log.WriteLog(threadInfo.SN + "  首件料跳过全部前站" + ",TraceLog");
                    }
                    else
                    {
                        if (IFactory)
                        {
                            string pass = "1";
                            string strMes = "";
                            //string URL = string.Format("http://10.128.10.7/webapi/api/Trace/GetTestResultByStation?sn={0}&station={1}", threadInfo.SN, "grd-tab-wld");
                            //RequestAPI2.GetTestResult(threadInfo.SN, URL, out strMes, out pass);
                            //string RequestTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            string Url = string.Format("http://17.239.116.110/api/v2/parts?serial_type=band&serial={0}&process_name=trch-bt-wld", threadInfo.SN.Replace("+", "%2B"));
                            RequestAPI2.GetTestResult1(threadInfo.SN, Url, out strMes, out pass);
                            _homefrm.UiText(threadInfo.SN, "txt_ProcessControl_SN");
                            RequestAPI2.Trace_process_control(Global.inidata.productconfig.Trace_CheakSN_UA, threadInfo.SN, out Trace_str_ua, out Msg_ua);//Trace_ua校验前站
                            Log.WriteLog("Trace校验UA前站SN：" + threadInfo.SN + "  " + "结果：" + Trace_str_ua.Replace("\n", "") + JsonConvert.SerializeObject(Msg_ua).Replace("\r\n", "") + ",TraceLog");
                            if (Msg_ua.Pass == "True" && (pass == "0" || pass == ""))//校验U-Bracket前站
                            {
                                _homefrm.UpDatalabelcolor(Color.Green, "Trace校验SN成功", "txt_ProcessControl_Status");
                                Global.Trace_process_ok++;
                                _homefrm.UpDatalabel(Global.Trace_process_ok.ToString(), "lb_ProcessControlOK");
                                _homefrm.AppendRichText(Trace_str_ua, "rtx_ProcessControl");
                                Global.PLC_Client2.WritePLC_D(10122, new short[] { 1 });
                                _homefrm.AppendRichText("N/A", "rtx_ProcessControlErrorMsg");
                                Log.WriteLog(threadInfo.SN + "前站校验OK" + ",TraceLog");
                            }
                            else
                            {
                                if (Msg_ua.Pass != "True")
                                {
                                    if (Global.SelectFirst == false)
                                    {
                                        if (Convert.ToDateTime(DateTime.Now.ToString("HH:mm")).CompareTo(Convert.ToDateTime("6:00")) >= 0 && Convert.ToDateTime(DateTime.Now.ToString("HH:mm")).CompareTo(Convert.ToDateTime("18:00")) < 0)
                                        {
                                            Global.TracePVCheck_Error_D++;//白班读码NG计数累加
                                            _datastatisticsfrm.UpDataDGV_D(12, 1, Global.TracePVCheck_Error_D.ToString());
                                            Global.inidata.productconfig.TracePVCheck_Error_D = Global.TracePVCheck_Error_D.ToString();
                                            Global.inidata.WriteProductnumSection();
                                        }
                                        else
                                        {
                                            Global.TracePVCheck_Error_N++;//夜班读码NG计数累加
                                            _datastatisticsfrm.UpDataDGV_N(12, 1, Global.TracePVCheck_Error_N.ToString());
                                            Global.inidata.productconfig.TracePVCheck_Error_N = Global.TracePVCheck_Error_N.ToString();
                                            Global.inidata.WriteProductnumSection();
                                        }
                                    }
                                    Log.WriteLog(threadInfo.SN + "前站校验NG" + ",TraceLog");
                                    _homefrm.UpDatalabelcolor(Color.Red, "Trace校验SN失败", "txt_ProcessControl_Status");
                                }
                                else if (pass != "0")
                                {
                                    Log.WriteLog(threadInfo.SN + "产品过站NG " + ",TraceLog");
                                    _homefrm.UpDatalabelcolor(Color.Red, "产品过站NG", "txt_ProcessControl_Status");
                                }
                                Global.Trace_process_ng++;
                                _homefrm.AppendRichText("N/A", "rtx_ProcessControlErrorMsg");
                                _homefrm.UpDatalabel(Global.Trace_process_ng.ToString(), "lb_ProcessControlNG");
                                _homefrm.AppendRichText(Trace_str_ua, "rtx_ProcessControl");
                                Global.PLC_Client2.WritePLC_D(10122, new short[] { 2 });
                                string FalseData = JsonConvert.SerializeObject(Msg_ua, Formatting.Indented);
                                _homefrm.AppendRichText(FalseData, "rtx_ProcessControlErrorMsg");
                                _homefrm.AppendRichText("产品过站NG", "rtx_ProcessControlErrorMsg");
                            }
                        }
                        else
                        {
                            _homefrm.UpDatalabelcolor(Color.Red, "JGP IFactory前站校验失败", "txt_ProcessControl_Status");
                            Global.Trace_process_ng++;
                            _homefrm.AppendRichText("N/A", "rtx_ProcessControlErrorMsg");
                            _homefrm.UpDatalabel(Global.Trace_process_ng.ToString(), "lb_ProcessControlNG");
                            _homefrm.AppendRichText(callResult + errMsg, "rtx_ProcessControl");
                            Global.PLC_Client2.WritePLC_D(10122, new short[] { 2 });
                            Log.WriteLog(threadInfo.SN + "JGP IFactory前站校验失败," + callResult + errMsg + ",TraceLog");
                        }
                        Log.WriteCSV(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + "" + "," + threadInfo.SN + "," + "," + "," + "," + "," + "," + "," + "," + "前站校验", System.AppDomain.CurrentDomain.BaseDirectory + "\\前站Trace数据\\");

                    }
                }
                else
                {
                    Global.Replace = false;
                    Global.PLC_Client2.WritePLC_D(10122, new short[] { 2 });
                    Global.Trace_process_ng++;
                    _homefrm.UpDatalabel(Global.Trace_process_ng.ToString(), "lb_ProcessControlNG");
                    Product_num_Mes_NG++;
                    Log.WriteLog("校验前站SN格式不正确," + threadInfo.SN + ",TraceLog");
                    Log.WriteCSV(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + "" + "," + threadInfo.SN + "," + "," + "," + "," + "," + "," + "," + "," + "前站Trace/JP异常,BED001", System.AppDomain.CurrentDomain.BaseDirectory + "\\前站Trace/JP异常数据\\");
                }
            }
            catch (Exception ex)
            {
                Global.Replace = false;
                Log.WriteLog("卡上一站失败:" + Trace_str_ua + "," + Trace_str_la + ",TraceLog");
                Log.WriteLog("卡上一站失败:" + ex.ToString().Replace("\n", "") + ",TraceLog");
                if (Global.SelectFirst == false)
                {
                    if (Convert.ToDateTime(DateTime.Now.ToString("HH:mm")).CompareTo(Convert.ToDateTime("6:00")) >= 0 && Convert.ToDateTime(DateTime.Now.ToString("HH:mm")).CompareTo(Convert.ToDateTime("18:00")) < 0)
                    {
                        Global.TracePVCheck_Error_D++;//白班读码NG计数累加
                        _datastatisticsfrm.UpDataDGV_D(12, 1, Global.TracePVCheck_Error_D.ToString());
                        Global.inidata.productconfig.TracePVCheck_Error_D = Global.TracePVCheck_Error_D.ToString();
                        Global.inidata.WriteProductnumSection();
                    }
                    else
                    {
                        Global.TracePVCheck_Error_N++;//夜班读码NG计数累加
                        _datastatisticsfrm.UpDataDGV_N(12, 1, Global.TracePVCheck_Error_N.ToString());
                        Global.inidata.productconfig.TracePVCheck_Error_N = Global.TracePVCheck_Error_N.ToString();
                        Global.inidata.WriteProductnumSection();
                    }
                }
                _homefrm.AppendRichText(Trace_str_ua, "rtx_ProcessControl");
                _homefrm.UpDatalabelcolor(Color.Red, "Trace校验异常，请检查网络", "txt_ProcessControl_Status");
                Trace_check_flag = true;
                Global.PLC_Client2.WritePLC_D(10122, new short[] { 2 });
                string JSONBody = Trace_str_ua.Replace(",", ";").Replace("\n", "");
                Log.WriteCSV_DiscardLog("TracePV" + "," + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + "" + "," + Global.inidata.productconfig.OEE_Dsn + "," + threadInfo.SN + "," + threadInfo.SN + "," + JSONBody + "," + "1");
            }
        }

        private void SendDataToHans(object obj)//发送SN给大族焊接机和普雷斯特
        {
            lock (LockHans)
            {
                int result = 0; 
                try
                {
                    //AsyncTcpClient client = null;
                 ThreadInfo threadInfo = obj as ThreadInfo;
                switch (threadInfo.SelectedIndex)
                {
                    case 1:
                       result = 10251;
                            if (TCPconnected)
                            {
                                if (threadInfo.SN.Contains("FM"))
                                {
                                    Global.client1.Send(threadInfo.SN + "\r\n");
                                    Log.WriteLog(string.Format("发送大族1：SN:{0}",threadInfo.SN));
                                    Global.PLC_Client2.WritePLC_D(result, new short[] { 1 });
                                }
                                else
                                {
                                    Log.WriteLog(string.Format("发送大族1的SN格式异常:{0}", threadInfo.SN));
                                    Global.PLC_Client2.WritePLC_D(result, new short[] { 2 });
                                }
                            }
                            break;
                    case 2:
                        result = 10253;
                            if (TCPconnected)
                            {
                                if (threadInfo.SN.Contains("FM"))
                                {
                                    Global.client2.Send(threadInfo.SN + "\r\n");
                                    Log.WriteLog(string.Format("发送大族2SN：{0}", threadInfo.SN));
                                    Global.PLC_Client2.WritePLC_D(result, new short[] { 1 });
                                }
                                else
                                {
                                    Log.WriteLog(string.Format("发送大族2的SN：{0}格式异常", threadInfo.SN));
                                    Global.PLC_Client2.WritePLC_D(result, new short[] { 2 });
                                }
                            }
                            break;      
                }
                    
                }
                catch (Exception ex)
                {
                    //Global.PLC_Client2.WritePLC_D(result, new short[] { 2 });
                    Log.WriteLog("发送SN给大族异常失败！" + ex.ToString().Replace("\n", ""));
                }
            }
        }

        private void GetDataForHans(object obj)//获取UA大族焊接机和普雷斯特参数
        {
            lock (Lock)
            {
                try
                {
                    ThreadInfo threadInfo = obj as ThreadInfo;
                    if (TCPconnected)
                    {
                        if (threadInfo.SelectedIndex==1)
                        {
                            if (threadInfo.SN.Contains("FM"))
                            {
                                Global.client1.Send("D1," + threadInfo.SN + "\r\n");//请求获取大族焊接参数
                                Log.WriteLog(string.Format("请求获取大族1参数SN:{0}", threadInfo.SN));
                                Global.PLC_Client.WritePLC_D(10261, new short[] { 1 });
                            }
                            else
                            {
                                Global.PLC_Client.WritePLC_D(10261, new short[] { 2 });
                                Log.WriteLog(string.Format("请求获取大族1的SN格式异常:{0}", threadInfo.SN));
                            }
                        }
                        if (threadInfo.SelectedIndex == 2)
                        {
                            if (threadInfo.SN.Contains("FM"))
                            {
                                Global.client2.Send("D1," + threadInfo.SN + "\r\n");//请求获取大族焊接参数
                                Log.WriteLog(string.Format("请求获取大族2参数SN:{0}", threadInfo.SN));
                                Global.PLC_Client.WritePLC_D(10263, new short[] { 1 });
                            }
                            else
                            {
                                Global.PLC_Client.WritePLC_D(10263, new short[] { 2 });
                                Log.WriteLog(string.Format("请求获取大族2的SN格式异常:{0}", threadInfo.SN));
                            }
                        }

                    }
                    else
                    {
                        Log.WriteLog(string.Format("请求获取大族1的SN格式异常:{0}", threadInfo.SN));
                        //Global.PLC_Client.WritePLC_D(12651, new short[] { 2 });
                    }

                }
                catch (Exception ex)
                {
                    Log.WriteLog("发送SN给大族/普雷斯特异常失败！" + ex.ToString().Replace("\n", ""));
                }
            }
        }

        private void UploadData_UA1(object obj)//Trace/PDCA上传数据
        {
            lock (LockUA)
            {
                try
                {
                    int Address_SN = 0;
                    int Address_fixture_id = 0;
                    int Address_start_time = 0;
                    int Address_Tab_weld_start = 0;
                    int Address_Tab_weld_stop = 0;
                    int Address_Trench_Weld_stop = 0;
                    int Address_Trench_Weld_start = 0;
                    int Address_stop_time = 0;
                    int Address_status = 0;
                    int Address_CT = 0;
                    int Address_NG = 0;
                    int result = 0;
                    int result2 = 0;
                    int result3 = 0;
                    int result4 = 0;
                    int result5 = 0;
                    string Full_SN1 = "";
                    string FullSN1 = "";
                    string Fixture_id = "";
                    string tossing_item = "NA";//PDCA NG明细
                    string tossing_item2 = "NA";// Trace Trench NG 明细
                    string tossing_item3 = "NA";//Trace Brace NG 明细
                    short[] Start_time = new short[6];
                    short[] Tab_Weld_start = new short[6];
                    short[] Tab_Weld_stop = new short[6];
                    short[] Trench_Weld_start = new short[6];
                    short[] Trench_Weld_stop = new short[6];
                    short[] Stop_time = new short[6];


                    string Start_t = string.Empty;
                    string Trench_Weld_st = string.Empty;
                    string Trench_Weld_sp = string.Empty;
                    string Tab_Weld_st = string.Empty;
                    string Tab_Weld_sp = string.Empty;
                    string Stop_t = string.Empty;
                    string station = string.Empty;
                    string tmp = string.Empty;
                    short[] Status = new short[1];
                    short[] Status1 = new short[1];
                    short[] HansUA_NGStation = new short[4];
                    OEEData oeedata = new OEEData();
                    BAilData bail = new BAilData();
                    //Brace Bracket
                    TraceMesRequest_ua TraceData = new TraceMesRequest_ua();
                    TraceData.serials = new SN();
                    TraceData.data = new data();
                    TraceData.data.insight = new Insight();
                    TraceData.data.insight.test_attributes = new Test_attributes();
                    TraceData.data.insight.test_station_attributes = new Test_station_attributes();
                    TraceData.data.insight.uut_attributes = new Uut_attributes();
                    TraceData.data.insight.results = new Result[37];
                    TraceData.data.items = new ExpandoObject();
                    for (int i = 0; i < TraceData.data.insight.results.Length; i++)
                    {
                        TraceData.data.insight.results[i] = new Result();
                    }
                    //Trench Bracket
                    //TraceMesRequest_ua TraceData2 = new TraceMesRequest_ua();
                    //TraceData2.serials = new SN();
                    //TraceData2.data = new data();
                    //TraceData2.data.insight = new Insight();
                    //TraceData2.data.insight.test_attributes = new Test_attributes();
                    //TraceData2.data.insight.test_station_attributes = new Test_station_attributes();
                    //TraceData2.data.insight.uut_attributes = new Uut_attributes();
                    //TraceData2.data.insight.results = new Result[10];
                    //TraceData2.data.items = new ExpandoObject();
                    //for (int i = 0; i < TraceData2.data.insight.results.Length; i++)
                    //{
                    //    TraceData2.data.insight.results[i] = new Result();
                    //}
                    Int32 ActualCT = 0;//OEE使用
                    short DefectCode = 0;//OEE使用                
                    //预设上传的焊接结果为fail
                    for (int r = 0; r < 37; r++)
                    {
                        TraceData.data.insight.results[r].result = "pass";
                    }
                    //for (int r = 0; r < 10; r++)
                    //{
                    //    TraceData2.data.insight.results[r].result = "pass";
                    //}
                    //Global.PLC_Client.WritePLC_D(10301, new short[] { 0 });//Trace上传结果清零
                    //Global.PLC_Client.WritePLC_D(10302, new short[] { 0 });//PDCA上传结果清零
                    ThreadInfo threadInfo = obj as ThreadInfo;
                    switch (threadInfo.SelectedIndex)
                    {
                        case 1:
                            result = 10301;//TRACE_焊后上传-TRG反馈结果
                            result2 = 10302;//
                            result3 = 10303;
                            result4 = 10304;
                            result5 = 10305;
                            Address_SN = 10330;
                            Address_fixture_id = 10310;
                            Address_start_time = 10350;//XX开始时间
                            Address_Trench_Weld_start = 10356;//长条开始焊接时间
                            Address_Trench_Weld_stop = 10374;//长条结束焊接时间
                            Address_Tab_weld_start = 10368;//Tab 小料开始焊接时间
                            Address_Tab_weld_stop = 10374;//Tab 小料结束焊接时间
                            Address_stop_time = 10380;//XX结束时间
                            Address_status = 10390;// 焊接总结果
                            Address_CT = 10400;
                            Address_NG = 10391;
                            break;
                        default:
                            break;
                    }
                    Global.PLC_Client2.WritePLC_D(result, new short[] { 0 });//Trace Brace上传结果清零
                    Global.PLC_Client2.WritePLC_D(result2, new short[] { 0 });//PDCA上传结果清零
                    Global.PLC_Client2.WritePLC_D(result3, new short[] { 0 });//PDCA上传结果清零
                    Global.PLC_Client2.WritePLC_D(result4, new short[] { 0 });//焊接参数结果清零
                    Global.PLC_Client2.WritePLC_D(result5, new short[] { 0 });//Trace Trench上传结果清零
                    try
                    {
                        Full_SN1 = Global.PLC_Client2.ReadPLC_Dstring(Address_SN, 15).Replace(" ", "").Replace("\0", "");
                        //Full_SN1 = "FM7H20000130000224+5";
                        if (Full_SN1.Length > 18)
                        {
                            FullSN1 = Full_SN1.Remove(18);
                        }
                        //Full_SN1 = "FM7H1N000260000224";
                        Fixture_id = Global.PLC_Client2.ReadPLC_Dstring(Address_fixture_id, 15).Trim().Replace("\0", "");
                        //Fixture_id = "H-76HO-SMA40-2200-A-00003";
                        _homefrm.UiText(Full_SN1, "txt_Trace_unit_serial_number_la");
                        _homefrm.UiText(Fixture_id, "txt_Trace_fixture_id_la");
                        Start_time = Global.PLC_Client2.ReadPLC_D(Address_start_time, 6);//XX开始时间
                        Trench_Weld_start = Global.PLC_Client2.ReadPLC_D(Address_Trench_Weld_start, 6);//长条开始焊接时间
                        Trench_Weld_stop = Global.PLC_Client2.ReadPLC_D(Address_Trench_Weld_stop, 6);//长条结束焊接时间
                        //Tab_Weld_start = Global.PLC_Client2.ReadPLC_D(Address_Tab_weld_start, 6);//Tab 小料开始焊接时间
                        //Tab_Weld_stop = Global.PLC_Client2.ReadPLC_D(Address_Tab_weld_stop, 6);//Tab 小料结束焊接时间
                        Stop_time = Global.PLC_Client2.ReadPLC_D(Address_stop_time, 6);//XX结束时间
                        Status = Global.PLC_Client2.ReadPLC_D(10390, 1);// 焊接总结果
                        //Status1 = Global.PLC_Client2.ReadPLC_D(253,1);//  小料焊接结果
                        ActualCT = Global.PLC_Client2.ReadPLC_DD(Address_CT, 2)[0];//CT时间
                        HansUA_NGStation = Global.PLC_Client2.ReadPLC_D(Address_NG, 4);//中是否有那个小料焊接NG
                        for (int i = 0; i < HansUA_NGStation.Length; i++)
                        {
                            if (HansUA_NGStation[i] != 1)
                            {
                                if (tossing_item.Contains("NA"))
                                {
                                    tossing_item = tossing_item.Replace("NA", "");
                                }
                                //if (tossing_item2.Contains("NA"))
                                //{
                                //    tossing_item2 = tossing_item2.Replace("NA", "");
                                //}
                                if (tossing_item3.Contains("NA"))
                                {
                                    tossing_item3 = tossing_item3.Replace("NA", "");
                                }
                                tossing_item += string.Format("location{0} CCD NG/", i + 1);
                                //焊接结果与上传的焊接数据result保持一致
                                if (i + 1 == 1)//长条
                                {
                                    tossing_item3 += string.Format("location{0} CCD NG/", i + 1);
                                    //oeedata.ErrorCode += ";080003";
                                }
                                if (i + 1 == 2)
                                {
                                    tossing_item3 += string.Format("location{0} CCD NG/", i + 1);
                                    //oeedata.ErrorCode += ";080004";
                                }
                                if (i + 1 == 3)
                                {
                                    tossing_item3 += string.Format("location{0} CCD NG/", i + 1);
                                    //oeedata.ErrorCode += ";080005";
                                }
                                if (i + 1 == 4)
                                {
                                    tossing_item3 += string.Format("location{0} CCD NG/", i + 1);
                                    //oeedata.ErrorCode += ";080006";
                                }
                                if (i + 1 == 4) //Brace 焊接3 结果NG
                                {
                                    TraceData.data.insight.results[3].result = "fail";
                                    TraceData.data.insight.results[28].result = "fail";
                                    TraceData.data.insight.results[29].result = "fail";
                                    TraceData.data.insight.results[30].result = "fail";
                                    TraceData.data.insight.results[31].result = "fail";
                                    TraceData.data.insight.results[32].result = "fail";
                                    TraceData.data.insight.results[33].result = "fail";
                                    TraceData.data.insight.results[34].result = "fail";
                                    TraceData.data.insight.results[35].result = "fail";
                                }
                                if (i + 1 == 3) //Brace 焊接2 结果NG
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
                                if (i + 1 == 2)//Brace 焊接1 结果NG
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
                                if (i + 1 == 1)//Trench 焊接 结果NG
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
                                #region NG明细添加  
                                if (Global.SelectFirst == false)
                                {
                                    if (i + 1 == 1)
                                    {
                                        if (Convert.ToDateTime(DateTime.Now.ToString("HH:mm")).CompareTo(Convert.ToDateTime("6:00")) >= 0 && Convert.ToDateTime(DateTime.Now.ToString("HH:mm")).CompareTo(Convert.ToDateTime("18:00")) < 0)
                                        {
                                            Global.location1_CCDNG_D++;//白班焊接NG计数累加                                                                 
                                            _datastatisticsfrm.UpDataDGV_D(4, 1, Global.location1_CCDNG_D.ToString());
                                            Global.inidata.productconfig.location1_CCDNG_D = Global.location1_CCDNG_D.ToString();
                                        }
                                        else
                                        {
                                            Global.location1_CCDNG_N++;//夜班焊接NG计数累加
                                            _datastatisticsfrm.UpDataDGV_N(4, 1, Global.location1_CCDNG_N.ToString());
                                            Global.inidata.productconfig.location1_CCDNG_N = Global.location1_CCDNG_N.ToString();
                                        }
                                    }
                                    if (i + 1 == 2)
                                    {
                                        if (Convert.ToDateTime(DateTime.Now.ToString("HH:mm")).CompareTo(Convert.ToDateTime("6:00")) >= 0 && Convert.ToDateTime(DateTime.Now.ToString("HH:mm")).CompareTo(Convert.ToDateTime("18:00")) < 0)
                                        {
                                            Global.location2_CCDNG_D++;//白班焊接NG计数累加                                                                 
                                            _datastatisticsfrm.UpDataDGV_D(5, 1, Global.location2_CCDNG_D.ToString());
                                            Global.inidata.productconfig.location2_CCDNG_D = Global.location2_CCDNG_D.ToString();
                                        }
                                        else
                                        {
                                            Global.location2_CCDNG_N++;//夜班焊接NG计数累加
                                            _datastatisticsfrm.UpDataDGV_N(5, 1, Global.location2_CCDNG_N.ToString());
                                            Global.inidata.productconfig.location2_CCDNG_N = Global.location2_CCDNG_N.ToString();
                                        }
                                    }
                                    if (i + 1 == 3)
                                    {
                                        if (Convert.ToDateTime(DateTime.Now.ToString("HH:mm")).CompareTo(Convert.ToDateTime("6:00")) >= 0 && Convert.ToDateTime(DateTime.Now.ToString("HH:mm")).CompareTo(Convert.ToDateTime("18:00")) < 0)
                                        {
                                            Global.location3_CCDNG_D++;//白班焊接NG计数累加                                                                 
                                            _datastatisticsfrm.UpDataDGV_D(6, 1, Global.location3_CCDNG_D.ToString());
                                            Global.inidata.productconfig.location3_CCDNG_D = Global.location3_CCDNG_D.ToString();
                                        }
                                        else
                                        {
                                            Global.location3_CCDNG_N++;//夜班焊接NG计数累加
                                            _datastatisticsfrm.UpDataDGV_N(6, 1, Global.location3_CCDNG_N.ToString());
                                            Global.inidata.productconfig.location3_CCDNG_N = Global.location3_CCDNG_N.ToString();
                                        }
                                    }
                                }
                                #endregion
                            }
                        }
                        #region 合并时间
                        for (int t = 0; t < 6; t++)
                        {
                            if (t < 2)
                            {
                                Start_t += Start_time[t].ToString() + "-";
                                Trench_Weld_st += Trench_Weld_start[t].ToString() + "-";
                                Trench_Weld_sp += Trench_Weld_stop[t].ToString() + "-";
                                //Tab_Weld_st += Tab_Weld_start[t].ToString() + "-";
                                //Tab_Weld_sp += Tab_Weld_stop[t].ToString() + "-";
                                Stop_t += Stop_time[t].ToString() + "-";
                            }
                            else if (t == 2)
                            {
                                Start_t += Start_time[t].ToString() + " ";
                                Trench_Weld_st += Trench_Weld_start[t].ToString() + " ";
                                Trench_Weld_sp += Trench_Weld_stop[t].ToString() + " ";
                                //Tab_Weld_st += Tab_Weld_start[t].ToString() + " ";
                                //Tab_Weld_sp += Tab_Weld_stop[t].ToString() + " ";
                                Stop_t += Stop_time[t].ToString() + " ";
                            }
                            else if (t > 2 && t < 5)
                            {
                                Start_t += Start_time[t].ToString() + ":";
                                Trench_Weld_st += Trench_Weld_start[t].ToString() + ":";
                                Trench_Weld_sp += Trench_Weld_stop[t].ToString() + ":";
                                //Tab_Weld_st += Tab_Weld_start[t].ToString() + ":";
                                //Tab_Weld_sp += Tab_Weld_stop[t].ToString() + ":";
                                Stop_t += Stop_time[t].ToString() + ":";
                            }
                            else if (t == 5)
                            {
                                Start_t += Start_time[t].ToString();
                                Trench_Weld_st += Trench_Weld_start[t].ToString();
                                Trench_Weld_sp += Trench_Weld_stop[t].ToString();
                                //Tab_Weld_st += Tab_Weld_start[t].ToString();
                                //Tab_Weld_sp += Tab_Weld_stop[t].ToString();
                                Stop_t += Stop_time[t].ToString();
                            }
                        }
                        #endregion
                        string str = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + threadInfo.FileName + "," + Full_SN1 + "," + Fixture_id + "," + Start_t.ToString() + "," + Trench_Weld_st.ToString() + "," + Trench_Weld_sp.ToString() + "," + Stop_t.ToString() + "," + Status[0].ToString() + "," + HansUA_NGStation[0].ToString() + "," + HansUA_NGStation[1].ToString() + "," + HansUA_NGStation[2].ToString();
                        Log.WriteCSV(str, System.AppDomain.CurrentDomain.BaseDirectory + string.Format("\\{0}完整数据\\", threadInfo.FileName));
                    }
                    catch (Exception ex)
                    {
                        Log.WriteLog(ex.ToString().Replace("\n", ""));
                    }
                    if (Full_SN1 != "" && Fixture_id != "" && Full_SN1.Contains("FM"))
                    {
                        if (!Trace_ua.ContainsKey(Full_SN1))
                        {
                            bail.full_sn = FullSN1;//18
                            bail.Fixture_id = Fixture_id.ToString();
                            //oeedata.SerialNumber = Full_SN1;
                            //oeedata.Fixture = Fixture_id;
                            if (ActualCT > (Convert.ToDouble(Global.inidata.productconfig.MaxCT) * 10))
                            {
                                double ct = (Convert.ToDouble(Global.inidata.productconfig.CT) * 10);
                                oeedata.ActualCT = (ct / 10).ToString("0.0");
                            }
                            else
                            {
                                double ct = Convert.ToDouble(ActualCT);
                                oeedata.ActualCT = (ct / 10).ToString("0.0");
                            }
                            //Brace      
                            TraceData.serials.band = Full_SN1;
                            TraceData.data.insight.test_attributes.unit_serial_number = FullSN1;//18
                            TraceData.data.insight.uut_attributes.full_sn = Full_SN1;
                            TraceData.data.insight.test_station_attributes.fixture_id = Fixture_id;
                            TraceData.data.insight.uut_attributes.fixture_id = Fixture_id;
                            //Trench
                            //TraceData.serials.band = Full_SN1;
                            //TraceData.data.insight.test_attributes.unit_serial_number = Full_SN1;
                            //TraceData.data.insight.uut_attributes.full_sn = Full_SN1;
                            //TraceData.data.insight.test_station_attributes.fixture_id = Fixture_id;
                            //TraceData.data.insight.uut_attributes.fixture_id = Fixture_id;
                            if (tossing_item != "")//PDCA
                            {
                                if (tossing_item.Substring(tossing_item.Length - 1, 1) == "/")
                                {
                                    tossing_item = tossing_item.Remove(tossing_item.Length - 1);
                                }
                            }
                            //if (tossing_item2 != "")//TRENCH
                            //{
                            //    if (tossing_item2.Substring(tossing_item2.Length - 1, 1) == "/")
                            //    {
                            //        tossing_item2 = tossing_item2.Remove(tossing_item2.Length - 1);
                            //    }
                            //}
                            if (tossing_item3 != "")//TAB
                            {
                                if (tossing_item3.Substring(tossing_item3.Length - 1, 1) == "/")
                                {
                                    tossing_item3 = tossing_item3.Remove(tossing_item3.Length - 1);
                                }
                            }
                            if (tossing_item.Contains("NA"))
                            {
                                TraceData.data.insight.uut_attributes.tossing_item = tossing_item3.Replace("NA", "");
                                //TraceData2.data.insight.uut_attributes.tossing_item = tossing_item2.Replace("NA", "");
                                bail.tossing_item = tossing_item.Replace("NA", "OK");
                            }
                            else if (!tossing_item.Contains("NA"))
                            {
                                //if (!tossing_item2.Contains("NA"))
                                //{
                                //    TraceData2.data.insight.uut_attributes.tossing_item = tossing_item2;
                                //}
                                if (!tossing_item3.Contains("NA"))
                                {
                                    TraceData.data.insight.uut_attributes.tossing_item = tossing_item3;
                                }
                                bail.tossing_item = tossing_item;
                            }
                            TraceData.data.insight.uut_attributes.STATION_STRING = string.Format("{{\"ActualCT \":\"{0}\",\"ScanCount \":\"{1}\"}}", oeedata.ActualCT, "1");
                            //TraceData2.data.insight.uut_attributes.STATION_STRING = string.Format("{{\"ActualCT \":\"{0}\",\"ScanCount \":\"{1}\"}}", oeedata.ActualCT, "1");
                            bail.auto_send = 1;
                            oeedata.auto_send = 1;
                            if (Start_t == "0-0-0 0:0:0" || Tab_Weld_st == "0-0-0 0:0:0" || Tab_Weld_sp == "0-0-0 0:0:0" || Trench_Weld_st == "0-0-0 0:0:0" || Trench_Weld_sp == "0-0-0 0:0:0" || Stop_t == "0-0-0 0:0:0")
                            {
                                bail.Start_Time = DateTime.Now.AddSeconds(-35);
                                bail.Stop_Time = DateTime.Now;
                                bail.Weld_start_time = DateTime.Now.AddSeconds(-28);
                                bail.Weld_stop_time = DateTime.Now.AddSeconds(-8);
                                oeedata.StartTime = DateTime.Now.AddSeconds(-35);
                                oeedata.EndTime = DateTime.Now;
                                TraceData.data.insight.test_attributes.uut_start = DateTime.Now.AddSeconds(-35).ToString("yyyy-MM-dd HH:mm:ss");
                                TraceData.data.insight.uut_attributes.laser_start_time = DateTime.Now.AddSeconds(-28).ToString("yyyy-MM-dd HH:mm:ss");
                                TraceData.data.insight.uut_attributes.laser_stop_time = DateTime.Now.AddSeconds(-8).ToString("yyyy-MM-dd HH:mm:ss");
                                TraceData.data.insight.test_attributes.uut_stop = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                                //TraceData2.data.insight.test_attributes.uut_start = DateTime.Now.AddSeconds(-35).ToString("yyyy-MM-dd HH:mm:ss");
                                //TraceData2.data.insight.uut_attributes.laser_start_time = DateTime.Now.AddSeconds(-28).ToString("yyyy-MM-dd HH:mm:ss");
                                //TraceData2.data.insight.uut_attributes.laser_stop_time = DateTime.Now.AddSeconds(-8).ToString("yyyy-MM-dd HH:mm:ss");
                                //TraceData2.data.insight.test_attributes.uut_stop = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            }
                            else
                            {
                                bail.Start_Time = Convert.ToDateTime(Start_t);
                                bail.Stop_Time = Convert.ToDateTime(Stop_t);
                                bail.Weld_start_time = Convert.ToDateTime(Trench_Weld_st);
                                bail.Weld_stop_time = Convert.ToDateTime(Trench_Weld_sp);
                                //oeedata.StartTime = Convert.ToDateTime(Start_t);
                                //oeedata.EndTime = Convert.ToDateTime(Stop_t);
                                TraceData.data.insight.test_attributes.uut_start = (Convert.ToDateTime(Start_t)).ToString("yyyy-MM-dd HH:mm:ss");
                                TraceData.data.insight.test_attributes.uut_stop = (Convert.ToDateTime(Stop_t)).ToString("yyyy-MM-dd HH:mm:ss");

                                TraceData.data.insight.uut_attributes.laser_start_time = (Convert.ToDateTime(Trench_Weld_st)).ToString("yyyy-MM-dd HH:mm:ss");
                                TraceData.data.insight.uut_attributes.laser_stop_time = (Convert.ToDateTime(Trench_Weld_sp)).ToString("yyyy-MM-dd HH:mm:ss");

                                //TraceData.data.insight.uut_attributes.laser_start_time = (Convert.ToDateTime(Tab_Weld_st)).ToString("yyyy-MM-dd HH:mm:ss");
                                //TraceData.data.insight.uut_attributes.laser_stop_time = (Convert.ToDateTime(Tab_Weld_sp)).ToString("yyyy-MM-dd HH:mm:ss");

                                //TraceData2.data.insight.test_attributes.uut_start = (Convert.ToDateTime(Start_t)).ToString("yyyy-MM-dd HH:mm:ss");
                                //TraceData2.data.insight.test_attributes.uut_stop = (Convert.ToDateTime(Stop_t)).ToString("yyyy-MM-dd HH:mm:ss");

                                //TraceData2.data.insight.uut_attributes.laser_start_time = (Convert.ToDateTime(Start_t)).ToString("yyyy-MM-dd HH:mm:ss");
                                //TraceData2.data.insight.uut_attributes.laser_stop_time = (Convert.ToDateTime(Stop_t)).ToString("yyyy-MM-dd HH:mm:ss");
                                //TraceData2.data.insight.uut_attributes.laser_start_time = (Convert.ToDateTime(Trench_Weld_st)).ToString("yyyy-MM-dd HH:mm:ss");
                                //TraceData2.data.insight.uut_attributes.laser_stop_time = (Convert.ToDateTime(Trench_Weld_sp)).ToString("yyyy-MM-dd HH:mm:ss");
                            }
                            if (Status[0] == 1)
                            {
                                bail.test_fail = true;
                                bail.test_result = "PASS";
                                TraceData.data.insight.test_attributes.test_result = "pass";
                                //TraceData2.data.insight.test_attributes.test_result = "pass";
                                //oeedata.Status = "OK";
                                //oeedata.ScanCount = "1";
                            }
                            else//焊接总结果NG
                            {
                                #region
                                //if (HansUA_NGStation[0] == 1)//长条焊接OK
                                //{
                                //    TraceData2.data.insight.test_attributes.test_result = "pass";
                                //}
                                //else // 长条焊接NG
                                //{
                                //    TraceData2.data.insight.test_attributes.test_result = "fail";
                                //}
                                //if (HansUA_NGStation[1] == 1 && HansUA_NGStation[2] == 1 && HansUA_NGStation[3] == 1)
                                //{
                                //    TraceData.data.insight.test_attributes.test_result = "pass";
                                //}
                                //else
                                //{
                                //    TraceData.data.insight.test_attributes.test_result = "fail";
                                //}

                                //if (Status[0] == 2 && HansUA_NGStation[1] == 0 && HansUA_NGStation[2] == 0 && HansUA_NGStation[3] == 0 && !tossing_item2.Contains("NA")) //长条焊接NG，小料未焊
                                //{
                                //    TraceData.data.insight.uut_attributes.tossing_item = tossing_item2;
                                //}
                                #endregion

                                bail.test_fail = false;
                                bail.test_result = "FAIL";
                                TraceData.data.insight.test_attributes.test_result = "fail";
                                //oeedata.Status = "NG";
                                //oeedata.ScanCount = "1";
                                _homefrm.AddList(string.Format("{0}[治具码：{1},NG]", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), Fixture_id), "list_FixtureNG");
                                string InsertStr = "insert into FixtureNG([DateTime],[Fixture])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "'" + "," + "'" + Fixture_id + "'" + ")";
                                SQL.ExecuteUpdate(InsertStr);
                            }

                            //string strMes = "";
                            string pass = "0";
                            //string Url = string.Format("http://17.239.116.110/api/v2/parts?serial_type=band&serial={0}&process_name=trch-bt-wld", Full_SN1);
                            //RequestAPI2.GetTestResult2(Full_SN1, Url, out strMes, out pass);
                            if (pass == "0")
                            {
                                //Brace
                                Out_Trace_ua.Add(Full_SN1);
                                Trace_ua.Add(Full_SN1, TraceData);

                                //Trench
                                //Out_Trace_ua2.Add(Full_SN1);
                                //Trace_ua2.Add(Full_SN1, TraceData2);


                                Out_ua.Add(Full_SN1);
                                bail_ua.Add(Full_SN1, bail);

                                //Out_oee_Mqtt.Add(Full_SN1);
                                //OEE_Mqtt.Add(Full_SN1, oeedata);

                            }
                            else
                            {
                                Global.PLC_Client2.WritePLC_D(result, new short[] { 2 });
                                Global.PLC_Client2.WritePLC_D(result2, new short[] { 2 });
                                Global.PLC_Client2.WritePLC_D(result3, new short[] { 2 });//图片上传NG
                                Global.PLC_Client2.WritePLC_D(result4, new short[] { 2 });
                                Global.PLC_Client2.WritePLC_D(result5, new short[] { 2 });
                                _homefrm.UpDatalabelcolor(Color.Red, "OEE_Default_NG", "lb_OEE_UA_SendStatus");
                                _homefrm.UpDatalabelcolor(Color.Red, "SN发送失败", "lb_PDCA_UA_SendStatus");
                                _homefrm.UpDatalabelcolor(Color.Red, "Trace_U_Bracket_NG", "lb_Trace_UA_SendStatus");
                                _homefrm.UpDatalabelcolor(Color.Red, "Trace_Trench_Bracket_NG", "lb_Trace_LA_SendStatus");
                                Log.WriteLog("重复触发UA,不上传！" + Full_SN1);

                            }
                            oeedata = null;
                            //TraceData2 = null;
                            bail = null;
                            TraceData = null;


                        }
                        else
                        {
                            Log.WriteLog("PLC重复触发UA,不上传！" + Full_SN1);
                            _homefrm.AppendRichText("PLC重复触发UA,不上传！" + Full_SN1, "rtx_TraceMsg");
                            _homefrm.AppendRichText("PLC重复触发UA,不上传！" + Full_SN1, "rtx_PDCAMsg");
                            Global.PLC_Client2.WritePLC_D(result, new short[] { 2 });
                            Global.PLC_Client2.WritePLC_D(result2, new short[] { 2 });
                            Global.PLC_Client2.WritePLC_D(result3, new short[] { 2 });//图片上传NG
                            Global.PLC_Client2.WritePLC_D(result4, new short[] { 2 });
                            Global.PLC_Client2.WritePLC_D(result5, new short[] { 2 });
                        }
                    }
                    else
                    {
                        Log.WriteLog("产品二维码或载具二维码格式不正确" + "1#焊机" + Full_SN1 + "," + Fixture_id);
                        Global.PLC_Client2.WritePLC_D(result, new short[] { 2 });
                        Global.PLC_Client2.WritePLC_D(result2, new short[] { 2 });
                        Global.PLC_Client2.WritePLC_D(result3, new short[] { 2 });//图片上传NG
                        Global.PLC_Client2.WritePLC_D(result4, new short[] { 2 });
                        Global.PLC_Client2.WritePLC_D(result5, new short[] { 2 });
                    }
                }
                catch (Exception ex)
                {
                    Log.WriteLog("获取参数异常失败:" + ex.ToString().Replace("\n", ""));
                }
            }
        }

        private void CheckFixtureID(object obj)//校验治具是否逾期保养
        {
            ThreadInfo threadInfo = (ThreadInfo)obj;
            try
            {
                bool IfTimeOut = Txt.FixtureTimeOut(threadInfo.FixtureID);
                bool IfIQCContains = Txt.IQCFixtrue(threadInfo.FixtureID);
                bool IfDTFixtures = Txt.Fixtrues_count(threadInfo.FixtureID);//判断治具在此时间段使用没有
                if (Global._fixture_ng.Contains(threadInfo.FixtureID))
                {
                    Global.PLC_Client.WritePLC_D(10132, new short[] { 2 });//治具待保养
                    Log.WriteLog(threadInfo.FixtureID + "治具待保养！");
                }
                else if (Global._fixture_tossing_ng.Contains(threadInfo.FixtureID))
                {
                    Global.PLC_Client.WritePLC_D(10132, new short[] { 2 });//治具待维修
                    Log.WriteLog(threadInfo.FixtureID + "治具待维修！");
                }
                else if (Global.FixtureOutID == threadInfo.FixtureID)
                {
                    Global.PLC_Client.WritePLC_D(10132, new short[] { 2 });//该治具手动排出
                    Log.WriteLog(threadInfo.FixtureID + "手动排出！");
                    Global.FixtureOutID = string.Empty;
                }
                else if (IfTimeOut && IfIQCContains)
                {
                    Global.PLC_Client.WritePLC_D(10132, new short[] { 1 });//该治具已定时保养
                    Log.WriteLog(threadInfo.FixtureID + "治具正常生产!");
                }
                else if (!IfIQCContains)
                {
                    Global.PLC_Client.WritePLC_D(10132, new short[] { 2 });//该治具不在IQC治具列表中
                    _homefrm.AddList(threadInfo.FixtureID, "list_IQCFixtureNG");
                    Log.WriteLog(threadInfo.FixtureID + "治具未录入IQC系统！");
                }
                else if (!IfTimeOut)
                {
                    Global.PLC_Client.WritePLC_D(10132, new short[] { 2 });//该治具逾期保养
                    _homefrm.AddList(threadInfo.FixtureID, "list_FixtureMsgNG");
                    Log.WriteLog(threadInfo.FixtureID + "治具逾期保养！");
                }

                if (!IfDTFixtures)//添加治具进入时间段数据
                {
                    //Txt.Fixtrues_WriteLine(threadInfo.FixtureID);
                    string insertStr = string.Format("insert into FixtureCount([DateTime],[FixtureID],[UsingTimes],[YesNo]) values('{0}','{1}','{2}','{3}')",
                          DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), threadInfo.FixtureID, "0", "N");
                    int r = SQL.ExecuteUpdate(insertStr);
                    Log.WriteLog(insertStr);
                }

                if (threadInfo.FixtureID.Length >= 15)//对治具号进行检查
                {
                    if (Global._fixture.Contains(threadInfo.FixtureID))//判断治具是否存在数据库中，如果存在更新使用时间和次数，不存在则录入数据库
                    {
                        _homefrm.UpdateDataGridView_Fixture_usingtimes(threadInfo.FixtureID);

                    }
                    else
                    {
                        Global._fixture.Add(threadInfo.FixtureID);
                        string insertStr = string.Format("insert into FixtureStatus([FixtureID],[Complete],[UsingTimes],[Status]) values('{0}','{1}','{2}','{3}')",
                        threadInfo.FixtureID, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "0", "正常使用中");
                        int r = SQL.ExecuteUpdate(insertStr);
                        string insertStr1 = string.Format("insert into FixtureTossing([Fixture],[TossingTime],[TossingContinuation],[TossingCount],[ContinuationNG],[CountNG]) values('{0}','{1}','{2}','{3}','{4}','{5}')",
                        threadInfo.FixtureID, "", "0", "0", "OK", "OK");
                        int r1 = SQL.ExecuteUpdate(insertStr1);//插入抛料治具表
                        _homefrm.UpdateDataGridView();
                        ///20230226Add
                        _homefrm.UpdateSjFixtureCount();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.WriteLog(ex.ToString().Replace("\r\n", ""));
            }
        }

        private void OEEMateriel(object obj)//OEE抛小料
        {
            lock (Lock1)
            {
                ThreadInfo threadInfo = obj as ThreadInfo;
                switch (threadInfo.SelectedIndex)
                {
                    case 1:
                        Global.NutCount = Global.NutCount + 1;
                        break;
                    case 2:
                        Global.NutOKCount = Global.NutOKCount + 1;
                        break;
                    default:
                        break;
                }
                if (threadInfo.SelectedIndex == 1)
                {
                    Global.ThrowCount = Global.NutCount;
                    _homefrm.UpDatalabel(Global.NutCount.ToString(), "lb_Materiel_Nut");
                    _homefrm.UpDatalabel(Global.ThrowCount.ToString(), "lb_Materiel_AllNut");
                    Log.WriteLog("小料抛料：" + Global.NutCount + "个" + " , " + "一天总计抛料：" + Global.TotalThrowCount + "个");
                }
                else
                {
                    Global.ThrowOKCount = Global.NutOKCount;
                    _homefrm.UpDatalabel(Global.NutOKCount.ToString(), "lb_Materiel_OK");
                    _homefrm.UpDatalabel(Global.ThrowOKCount.ToString(), "lb_Materiel_AllOK");
                    Log.WriteLog("小料OK：" + Global.NutOKCount + "个");
                }
            }
        }
        #endregion

        private void WritePLCData(object obj)//记录PLC参数
        {
            ThreadInfo threadInfo = obj as ThreadInfo;
            switch (threadInfo.SelectedIndex)
            {
                case 1:
                    Global.oldData = Global.PLC_Client.ReadPLC_DD(20000, 354);
                    break;
                case 2:
                    Global.newData = Global.PLC_Client.ReadPLC_DD(20000, 354);
                    for (int i = 0; i < Global.newData.Length - 1; i++)
                    {
                        if (Global.oldData[i] != Global.newData[i])
                        {
                            Log.WriteCSV(String.Format("{0},{1},{2},{3},{4},{5}", Global.Name, Global.Title, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), Global.PLC_DataName[2 * i + 1].PLC_Name, Global.oldData[i], Global.newData[i]), @"D:\ZHH\PLC参数修改记录\");
                            Log.WriteLog(String.Format("{0},{1},{2},{3},{4},{5}", Global.Name, Global.Title, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), Global.PLC_DataName[2 * i + 1].PLC_Name, Global.oldData[i], Global.newData[i]) + ",参数修改记录");

                        }
                    }
                    break;
            }
        }

        #region 数据缓存重传
        private void Trace_Retry_UA(object ob)
        {

        }

        //private void Trace_Retry_LA(object ob)
        //{
        //    while (true)
        //    {
        //        if (Trace_Logs_flag == true && TraceSendLA_ng == 0)
        //        {
        //            PDCA_Data data = new PDCA_Data();
        //            try
        //            {
        //                if (Access.QueryNumber("PDCA", "Trace_LA_SendNG") > 0)
        //                {
        //                    Log.WriteLog("Trace_LA上传失败数据重新上传");
        //                    for (int j = 0; j < Access.QueryNumber("PDCA", "Trace_LA_SendNG"); j++)
        //                    {
        //                        TraceMesRequest tracedata = new TraceMesRequest();
        //                        HansData hd = new HansData();
        //                        tracedata.serials = new SN();
        //                        tracedata.data = new data2();
        //                        tracedata.data.insight = new Insight();
        //                        tracedata.data.insight.test_attributes = new Test_attributes();
        //                        tracedata.data.insight.test_station_attributes = new Test_station_attributes();
        //                        tracedata.data.insight.uut_attributes = new Uut_attributes();
        //                        tracedata.data.insight.results = new Result[12];
        //                        tracedata.data.insight.results[0] = new Result();
        //                        tracedata.data.insight.results[1] = new Result();
        //                        tracedata.data.insight.results[2] = new Result();
        //                        tracedata.data.insight.results[3] = new Result();
        //                        tracedata.data.insight.results[4] = new Result();
        //                        tracedata.data.insight.results[5] = new Result();
        //                        tracedata.data.insight.results[6] = new Result();
        //                        tracedata.data.insight.results[7] = new Result();
        //                        tracedata.data.insight.results[8] = new Result();
        //                        tracedata.data.insight.results[9] = new Result();
        //                        tracedata.data.insight.results[10] = new Result();
        //                        tracedata.data.insight.results[11] = new Result();
        //                        Access.QueryLastData("PDCA", "Trace_LA_SendNG", out data);
        //                        if (data.full_sn != null)
        //                        {
        //                            tracedata.data.insight.test_attributes.test_result = data.test_result;
        //                            tracedata.serials.band = data.full_sn;
        //                            tracedata.data.insight.test_attributes.unit_serial_number = data.full_sn.Remove(17);
        //                            tracedata.data.insight.test_station_attributes.fixture_id = data.Fixture_id;
        //                            tracedata.data.insight.test_attributes.uut_start = (Convert.ToDateTime(data.Start_Time)).ToString("yyyy-MM-dd HH:mm:ss");
        //                            tracedata.data.insight.uut_attributes.la_weld_start_time = (Convert.ToDateTime(data.Weld_start_time)).ToString("yyyy-MM-dd HH:mm:ss");
        //                            tracedata.data.insight.uut_attributes.la_weld_stop_time = (Convert.ToDateTime(data.Weld_stop_time)).ToString("yyyy-MM-dd HH:mm:ss");
        //                            tracedata.data.insight.test_attributes.uut_stop = (Convert.ToDateTime(data.Stop_time)).ToString("yyyy-MM-dd HH:mm:ss");
        //                            hd.power_ll = data.power_ll;
        //                            hd.power_ul = data.power_ul;
        //                            hd.pattern_type = data.pattern_type;
        //                            hd.frequency = data.frequency;
        //                            hd.linear_speed = data.linear_speed;
        //                            hd.spot_size = data.spot_size;
        //                            hd.pulse_energy = data.pulse_energy;
        //                            hd.power = data.power;
        //                            hd.filling_pattern = data.filling_pattern;
        //                            hd.hatch = data.hatch;
        //                            Access.DeleteData("PDCA", "Trace_LA_SendNG");
        //                        }
        //                        if (!Trace_la.ContainsKey(data.full_sn))
        //                        {
        //                            Trace_la.Add(data.full_sn, tracedata);
        //                            Out_trace_la.Add(data.full_sn);
        //                            HansDatas_la.Add(data.full_sn, hd);
        //                        }
        //                        tracedata = null;
        //                        Thread.Sleep(1000);
        //                    }
        //                }
        //            }
        //            catch
        //            {
        //                Log.WriteLog("Trace_LA数据重传异常");
        //            }
        //        }
        //        //for (int m = 0; m < 10; m++)
        //        //{
        //        //    Thread.Sleep(1000);
        //        //}
        //    }
        //}

        //private void OEE_Default_Retry(object ob)//OEE上传失败重新上传
        //{
        //    while (true)
        //    {
        //        if (OEE_Default_flag == true && oeeSend_ng == 0)
        //        {
        //            OEE_Data data = new OEE_Data();
        //            try
        //            {
        //                if (Access.QueryNumber("PDCA", "OEEData_SendNG") > 0)
        //                {
        //                    Log.WriteLog("OEEData上传失败数据重新上传");
        //                    for (int j = 0; j < Access.QueryNumber("PDCA", "OEEData_SendNG"); j++)
        //                    {
        //                        OEEData oee_default = new OEEData();
        //                        Access.QueryLastData_oee("PDCA", "OEEData_SendNG", out data);
        //                        if (data.SerialNumber != null)
        //                        {
        //                            oee_default.SerialNumber = data.SerialNumber;
        //                            oee_default.Fixture = data.Fixture;
        //                            oee_default.StartTime = Convert.ToDateTime(data.StartTime);
        //                            oee_default.EndTime = Convert.ToDateTime(data.EndTime);
        //                            oee_default.Status = data.Status;
        //                            oee_default.ActualCT = data.ActualCT;
        //                            oee_default.SwVersion = data.SwVersion;
        //                            oee_default.DefectCode = data.DefectCode;
        //                            Access.DeleteData("PDCA", "OEEData_SendNG");
        //                        }
        //                        if (!OEE.ContainsKey(data.SerialNumber))
        //                        {
        //                            OEE.Add(data.SerialNumber, oee_default);
        //                            Out_oee.Add(data.SerialNumber);
        //                        }
        //                        oee_default = null;
        //                        Thread.Sleep(1000);
        //                    }
        //                }
        //            }
        //            catch
        //            {
        //                Log.WriteLog("OEEData数据重传异常");
        //            }
        //        }
        //    }
        //    //for (int m = 0; m < 10; m++)
        //    //{
        //    //    Thread.Sleep(1000);
        //    //}
        //}
        #region mqtt接收数据
        private void Client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            string topic = e.Topic; //訂閱接收到訊息的方式
            string receivedString = Encoding.UTF8.GetString(e.Message);//訂閱接收到訊息的结果
            if (e.Topic.Contains(Global.inidata.productconfig.EMT) || e.Topic.Contains("getservertime"))
            {
                Log.WriteLog("MQTT 返回数据" + receivedString);
            }

            if (topic.Contains("getservertime"))
            {
                try
                {
                    DateTime _unifydt = new DateTime();
                    UyTime uy = JsonConvert.DeserializeObject<UyTime>(receivedString);
                    _unifydt = Convert.ToDateTime(uy.ServerTime);
                    timer _unifyTimer = new timer();
                    _unifyTimer.wYear = Convert.ToUInt16(_unifydt.Year);
                    _unifyTimer.wMonth = Convert.ToUInt16(_unifydt.Month);
                    _unifyTimer.wDay = Convert.ToUInt16(_unifydt.Day);
                    _unifyTimer.wHour = Convert.ToUInt16(_unifydt.Hour);
                    _unifyTimer.wDayOfWeek = Convert.ToUInt16(_unifydt.DayOfWeek);
                    _unifyTimer.wMinute = Convert.ToUInt16(_unifydt.Minute);
                    _unifyTimer.wSecond = Convert.ToUInt16(_unifydt.Second);
                    _unifyTimer.wMiliseconds = Convert.ToUInt16(_unifydt.Millisecond);
                    try
                    {
                        SetLocalTime(ref _unifyTimer);
                    }
                    catch (Exception ex)
                    {
                        string err = ex.ToString();
                    }
                    Log.WriteLog("与OEE服务器同步时间成功,同步为:" + uy.ServerTime);
                }
                catch
                {
                    Log.WriteLog("与OEE服务器同步时间失败,同步为:" + topic);
                }
            }
            if (e.Topic.Contains(Global.inidata.productconfig.EMT + "/respond/downtime") || e.Topic.Contains(Global.inidata.productconfig.EMT + "/respond/oee"))
            {
                try
                {
                    Respond _respond = JsonConvert.DeserializeObject<Respond>(receivedString);
                    if (_respond.ErrorCode.Contains("Data Existed"))
                    {
                        Log.WriteCSV(_respond.GUID.ToString() + "," + _respond.Result + "," + _respond.ErrorCode, System.AppDomain.CurrentDomain.BaseDirectory + "MQTT 返回已上传过的数据");
                    }
                    Global.respond.TryAdd(_respond.GUID, _respond);
                }
                catch (Exception ex)
                {
                    Log.WriteLog("接收OEE订阅数据失败" + ex.ToString());
                }
            }
            if (e.Topic.Contains(Global.inidata.productconfig.EMT + "/respond/pant"))
            {
                try
                {
                    Respond _respond = JsonConvert.DeserializeObject<Respond>(receivedString);
                    if (_respond.ErrorCode.Contains("Data Existed"))
                    {
                        Log.WriteCSV(_respond.GUID.ToString() + "," + _respond.Result + "," + _respond.ErrorCode, System.AppDomain.CurrentDomain.BaseDirectory + "MQTT 返回已上传过的数据");
                    }
                    Global.respond1.TryAdd(_respond.GUID, _respond);
                }
                catch (Exception ex)
                {
                    Log.WriteLog("接收OEE订阅数据失败" + ex.ToString());
                }
            }
        }
        #region  OEE判定 MQTT接收数据超时
        //OEE_DT 与OEE过站数据判定接收
        public bool SendMqttResult(Guid GUID)
        {
            int cnt = 0;
            Respond respond = new Respond();
            while (true)
            {
                if (cnt < 100)
                {
                    if (Global.respond.Count > 0)
                    {
                        try
                        {
                            if (Global.respond.TryGetValue(GUID, out respond))
                            {
                                return true;
                            }
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                }
                else
                {
                    return false;
                }
                cnt++;
                Thread.Sleep(150);
            }
        }
        //OEE心跳过站数据判定接收
        public bool SendMqttResult1(Guid GUID)
        {
            int cnt = 0;
            Respond respond = new Respond();
            while (true)
            {
                if (cnt < 100)
                {
                    if (Global.respond1.Count > 0)
                    {
                        try
                        {
                            if (Global.respond1.TryGetValue(GUID, out respond))
                            {
                                return true;
                            }
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                }
                else
                {
                    return false;
                }
                cnt++;
                Thread.Sleep(150);
            }
        }
        #endregion
        private void Client_MqttMsgSubscribed(object sender, MqttMsgSubscribedEventArgs e)
        {
            //
        }
        private void MqttClient_MqttMsgPublished(object sender, MqttMsgPublishedEventArgs e)
        {
            //Log.WriteLog("IsPublished:{" + e.IsPublished + "}, MessageID:{" + e.MessageId + "}");            
        }
        private void MqttClient_ConnectionClosed(object sender, EventArgs e)
        {
            Log.WriteLog("MQTT Disconnect!");
        }

        #endregion
        private void OEE_DownTime_Retry(object ob)//OEE_DT上传失败重新上传
        {
            int Retrynum = 0;
            while (true)
            {
                if (DateTime.Now.Hour == 4 && DateTime.Now.Minute == 0 && DateTime.Now.Second == 0)
                {
                    Retrynum = 0;
                }
                if (Global.ConnectOEEFlag == true)
                {
                    string sendTopic = Global.inidata.productconfig.EMT + "/upload/downtime";
                    try
                    {
                        string SelectStr = string.Format("select count(*) from OEE_DTSendNG");
                        DataTable d1 = SQL.ExecuteQuery(SelectStr);
                        if (Convert.ToInt32(d1.Rows[0][0].ToString()) > 0)
                        {
                            Log.WriteLog("OEE_DT上传失败数据重新上传");
                            for (int j = 0; j < Convert.ToInt32(d1.Rows[0][0].ToString()); j++)
                            {
                                string SelectStr2 = "select * from OEE_DTSendNG where ID =(SELECT MIN(ID) FROM OEE_DTSendNG)";
                                DataTable d2 = SQL.ExecuteQuery(SelectStr2);
                                Guid guid = new Guid(d2.Rows[0][3].ToString());
                                if (!Global.respond.TryGetValue(guid, out Global.Respond))
                                {
                                    string OEE_DT = string.Format("{{\"guid\":\"{0}\",\"EMT\":\"{1}\",\"PoorNum\":\"{2}\",\"TotalNum\":\"{3}\",  \"Status\":\"{4}\",\"ErrorCode\":\"{5}\",\"EventTime\":\"{6}\",\"ModuleCode\":\"{7}\",\"ClientPcName\":\"{8}\",\"MAC\":\"{9}\",\"IP\":\"{10}\"}}", d2.Rows[0][3].ToString(), d2.Rows[0][4].ToString(), d2.Rows[0][5].ToString(), d2.Rows[0][6].ToString(), d2.Rows[0][7].ToString(), d2.Rows[0][8].ToString(), d2.Rows[0][13].ToString(), d2.Rows[0][9].ToString(), d2.Rows[0][10].ToString(), d2.Rows[0][11].ToString(), d2.Rows[0][12].ToString());
                                    //Global._mqttClient.Publish(sendTopic, Encoding.UTF8.GetBytes(OEE_DT), MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE, false);
                                    //Log.WriteLog("发送缓存OEE_DT:" + OEE_DT);
                                    //bool rst = SendMqttResult(guid);
                                    //if (rst)
                                    //{
                                    //    if (Global.respond[guid].Result == "OK")
                                    //    {
                                    //        // Retrynum = 0;
                                    //        Global.ConnectOEEFlag = true;
                                    //        Log.WriteLog(JsonConvert.SerializeObject(Global.respond[guid]) + ",OEELog");
                                    //        _homefrm.AppendRichText(d2.Rows[0][9].ToString() + ",触发时间=" + d2.Rows[0][13].ToString() + ",运行状态:" + d2.Rows[0][7].ToString() + ",故障描述:" + d2.Rows[0][15].ToString() + ",缓存发送成功", "rtx_DownTimeMsg");
                                    //        Global.respond.TryRemove(guid, out Global.Respond);
                                    //        string DeleteStr = "delete from OEE_DTSendNG where ID = (SELECT MIN(ID) FROM OEE_DTSendNG)";
                                    //        SQL.ExecuteUpdate(DeleteStr);
                                    //    }
                                    //    else
                                    //    {
                                    //        // Retrynum++;
                                    //        Global.ConnectOEEFlag = false;
                                    //        Log.WriteLog(JsonConvert.SerializeObject(Global.respond[guid]) + ",OEELog");
                                    //        _homefrm.AppendRichText(d2.Rows[0][9].ToString() + ",触发时间=" + d2.Rows[0][13].ToString() + ",运行状态:" + d2.Rows[0][7].ToString() + ",故障描述:" + d2.Rows[0][15].ToString() + ",缓存发送失败", "rtx_DownTimeMsg");
                                    //        _homefrm.AppendRichText("缓存发送失败原因: " + Global.respond[guid].ErrorCode, "rtx_DownTimeMsg");
                                    //        Log.WriteLog(Global.respond[guid].GUID.ToString() + "," + Global.respond[guid].Result + "," + Global.respond[guid].ErrorCode, System.AppDomain.CurrentDomain.BaseDirectory + "\\MQTT返回NG\\");
                                    //        Global.respond.TryRemove(guid, out Global.Respond);
                                    //        string DeleteStr = "delete from OEE_DTSendNG where ID = (SELECT MIN(ID) FROM OEE_DTSendNG)";
                                    //        SQL.ExecuteUpdate(DeleteStr);
                                    //    }
                                    //}
                                    //else
                                    //{
                                    //    //Retrynum++;
                                    //    Global.ConnectOEEFlag = false;
                                    //    Log.WriteLog("OEEDownTime异常,超时无反馈:" + OEE_DT);
                                    //    _homefrm.AppendRichText(d2.Rows[0][9].ToString() + ",触发时间=" + d2.Rows[0][13].ToString() + ",运行状态:" + d2.Rows[0][7].ToString() + ",故障描述:" + d2.Rows[0][15].ToString() + ",缓存发送失败", "rtx_DownTimeMsg");
                                    //    _homefrm.AppendRichText("网络异常,超时无反馈", "rtx_DownTimeMsg");
                                    //    Log.WriteLog("OEE_DT自动errorCode缓存发送失败");
                                    //    break;
                                    //}
                                }
                                else
                                {
                                    Log.WriteLog("补传前获取到上一次的结果" + JsonConvert.SerializeObject(Global.respond[guid]));
                                    string DeleteStr = "delete from OEE_DTSendNG where ID = (SELECT MIN(ID) FROM OEE_DTSendNG)";
                                    SQL.ExecuteUpdate(DeleteStr);
                                    Global.respond.TryRemove(guid, out Global.Respond);
                                }

                                Thread.Sleep(1000);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.WriteLog("OEE_DT数据缓存重传异常" + ex.ToString().Replace("\r\n", ""));
                    }

                }
                //删除超时两天的DT数据
                try
                {
                    string SelectStr = string.Format("select count(*) from OEE_DTSendNG");
                    DataTable d1 = SQL.ExecuteQuery(SelectStr);
                    if (Convert.ToInt32(d1.Rows[0][0].ToString()) > 0)
                    {
                        for (int j = 0; j < Convert.ToInt32(d1.Rows[0][0].ToString()); j++)
                        {
                            string SelectStr2 = "select * from OEE_DTSendNG where ID =(SELECT MIN(ID) FROM OEE_DTSendNG)";
                            DataTable d2 = SQL.ExecuteQuery(SelectStr2);
                            TimeSpan ts = DateTime.Now - Convert.ToDateTime(d2.Rows[0][13]);
                            if (ts.Days > 2)
                            {
                                string OEE_DT = string.Format("{{\"guid\":\"{0}\",\"EMT\":\"{1}\",\"PoorNum\":\"{2}\",\"TotalNum\":\"{3}\",  \"Status\":\"{4}\",\"ErrorCode\":\"{5}\",\"EventTime\":\"{6}\",\"ModuleCode\":\"{7}\",\"ClientPcName\":\"{8}\",\"MAC\":\"{9}\",\"IP\":\"{10}\"}}", d2.Rows[0][3].ToString(), d2.Rows[0][4].ToString(), d2.Rows[0][5].ToString(), d2.Rows[0][6].ToString(), d2.Rows[0][7].ToString(), d2.Rows[0][8].ToString(), d2.Rows[0][13].ToString(), d2.Rows[0][9].ToString(), d2.Rows[0][10].ToString(), d2.Rows[0][11].ToString(), d2.Rows[0][12].ToString());
                                Log.WriteLog("OEE_DT缓存超2天,删除DT数据:" + OEE_DT);
                                string DeleteStr = "delete from OEE_DTSendNG where ID = (SELECT MIN(ID) FROM OEE_DTSendNG)";
                                SQL.ExecuteUpdate(DeleteStr);
                            }
                        }
                        Thread.Sleep(1000);
                    }
                }
                catch (Exception ex)
                {
                    Log.WriteLog("OEE_DTSendNG缓存超2天,数据不需上传,删除数据时出现异常:" + ex.ToString());
                }
                Thread.Sleep(100);
            }
        }

        private void OEE_Default_Retry(object ob)//OEE_default上传失败重新上传
        {
            var IP = GetIp();
            var Mac = GetOEEMac();
            int RetryNum = 4;
            string sendTopic = Global.inidata.productconfig.EMT + "/upload/oee"; ;
            while (true)
            {
                if (DateTime.Now.Hour == 4 && DateTime.Now.Minute == 0 && DateTime.Now.Second == 0)
                {
                    RetryNum = 0;
                }
                if (Global.ConnectOEEFlag == true)//缓存失败计数 > 4隔天凌晨4点统一补传
                {
                    try
                    {
                        string SelectStr = string.Format("select count(*) from OEE_DefaultSendNG");
                        DataTable d1 = SQL.ExecuteQuery(SelectStr);
                        if (Convert.ToInt32(d1.Rows[0][0].ToString()) > 0)
                        {
                            Log.WriteLog("OEE_Default上传失败数据重新上传");
                            for (int j = 0; j < Convert.ToInt32(d1.Rows[0][0].ToString()); j++)
                            {
                                string SelectStr2 = "select * from OEE_DefaultSendNG where ID =(SELECT MIN(ID) FROM OEE_DefaultSendNG)";
                                DataTable d2 = SQL.ExecuteQuery(SelectStr2);
                                Guid guid = new Guid(d2.Rows[0][2].ToString());
                                if (!Global.respond.TryGetValue(guid, out Global.Respond))
                                {
                                    string OEE_Default = string.Format("{{\"GUID\":\"{0}\",\"EMT\":\"{1}\",\"SerialNumber\":\"{2}\",\"BGBarcode\":\"{3}\",\"Fixture\":\"{4}\",\"StartTime\":\"{5}\",\"EndTime\":\"{6}\",\"Status\":\"{7}\",\"ActualCT\":\"{8}\",\"SwVersion\":\"{9}\",\"ScanCount\":\"{10}\",\"ErrorCode\":\"{11}\",\"PFErrorCode\":\"{12}\",\"Cavity\":\"{13}\",\"ClientPcName\":\"{14}\",\"MAC\":\"{15}\",\"IP\":\"{16}\",\"EventTime\":\"{17}\"}}", d2.Rows[0][2].ToString(), d2.Rows[0][3].ToString(), d2.Rows[0][4].ToString(), d2.Rows[0][5].ToString(), d2.Rows[0][6].ToString(), d2.Rows[0][7].ToString(), d2.Rows[0][8].ToString(), d2.Rows[0][9].ToString(), d2.Rows[0][10].ToString(), d2.Rows[0][11].ToString(), d2.Rows[0][12].ToString(), d2.Rows[0][13].ToString(), d2.Rows[0][14].ToString(), d2.Rows[0][15].ToString(), d2.Rows[0][16].ToString(), d2.Rows[0][17].ToString(), d2.Rows[0][18].ToString(), d2.Rows[0][19].ToString());
                                    _homefrm.AppendRichText(OEE_Default, "rtx_OEEDefaultMsg");
                                    //Global._mqttClient.Publish(sendTopic, Encoding.UTF8.GetBytes(OEE_Default), MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE, false);
                                    //bool rst = SendMqttResult(guid);
                                    //if (rst)
                                    //{
                                    //    if (Global.respond[guid].Result == "OK")
                                    //    {
                                    //        Global.oee_ok++;
                                    //        // RetryNum = 0;//缓存失败计数清零
                                    //        _homefrm.UpDatalabel(Global.oee_ok.ToString(), "lb_OEEOK");
                                    //        Global.oeeSend_ng = 0;
                                    //        OEE_Default_flag = true;
                                    //        _homefrm.AppendRichText(OEE_Default + Global.respond[guid].Result, "rtx_OEEDefaultMsg");
                                    //        _homefrm.UpDatalabelcolor(Color.Green, "OEE_Default发送成功", "lb_OEE_UA_SendStatus");
                                    //        Global.ConnectOEEFlag = true;
                                    //        Global.PLC_Client.WritePLC_D(9140, new short[] { 1 });
                                    //        Log.WriteLog("OEE_Default:" + OEE_Default + "||" + JsonConvert.SerializeObject(Global.respond[guid]));
                                    //        Log.WriteLog("OEE_Default自动缓存发送成功");
                                    //        Log.WriteLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + "OEE_Default" + "," + d2.Rows[0][2].ToString() + ","
                                    //            + d2.Rows[0][3].ToString() + "," + d2.Rows[0][4].ToString() + "," + d2.Rows[0][5].ToString() + "," + d2.Rows[0][6].ToString() + ","
                                    //            + d2.Rows[0][7].ToString() + "," + d2.Rows[0][8].ToString() + "," + d2.Rows[0][9].ToString() + ","
                                    //            + d2.Rows[0][10].ToString() + "," + d2.Rows[0][11].ToString() + "," + d2.Rows[0][12].ToString() + ","
                                    //            + d2.Rows[0][13].ToString() + "," + d2.Rows[0][14].ToString() + "," + d2.Rows[0][15].ToString() + ","
                                    //            + d2.Rows[0][16].ToString() + "," + d2.Rows[0][17].ToString() + "," + d2.Rows[0][18].ToString() + "," + "1" + ","
                                    //            + "OK-OEE_Default", System.AppDomain.CurrentDomain.BaseDirectory + "\\OEE_MQTT_Default数据\\");
                                    //        _homefrm.UiText(d2.Rows[0][4].ToString(), "txtOEE_SerialNumber");
                                    //        _homefrm.UiText(d2.Rows[0][6].ToString(), "txtOEE_Fixture");
                                    //        _homefrm.UiText(d2.Rows[0][7].ToString(), "txtOEE_StartTime");
                                    //        _homefrm.UiText(d2.Rows[0][8].ToString(), "txtOEE_EndTime");
                                    //        _homefrm.UiText(d2.Rows[0][9].ToString(), "txtOEE_Status");
                                    //        _homefrm.UiText(d2.Rows[0][10].ToString(), "txtOEE_ActualCT");
                                    //        _homefrm.UiText(d2.Rows[0][11].ToString(), "txtOEE_sw");
                                    //        _homefrm.UiText(d2.Rows[0][12].ToString(), "txtOEE_ScanCount");
                                    //        Log.WriteLog("OEE_Default_UI更新成功");
                                    //        Global.respond.TryRemove(guid, out Global.Respond);
                                    //        string DeleteStr = "delete from OEE_DefaultSendNG where ID = (SELECT MIN(ID) FROM OEE_DefaultSendNG)";
                                    //        SQL.ExecuteUpdate(DeleteStr);
                                    //    }
                                    //    else
                                    //    {
                                    //        // RetryNum++;
                                    //        Log.WriteLog("OEE_Default:" + OEE_Default + "||" + JsonConvert.SerializeObject(Global.respond[guid]));
                                    //        Log.WriteLog(Global.respond[guid].GUID.ToString() + "," + Global.respond[guid].Result + "," + Global.respond[guid].ErrorCode, System.AppDomain.CurrentDomain.BaseDirectory + "\\MQTT返回NG\\");
                                    //        _homefrm.AppendRichText(OEE_Default, "rtx_OEEDefaultMsg");
                                    //        _homefrm.AppendRichText("OEE_Default缓存发送数据格式异常:" + Global.respond[guid].ErrorCode, "rtx_OEEDefaultMsg");
                                    //        Log.WriteLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + "OEE_Default" + "," + d2.Rows[0][2].ToString() + ","
                                    //           + d2.Rows[0][3].ToString() + "," + d2.Rows[0][4].ToString() + "," + d2.Rows[0][5].ToString() + "," + d2.Rows[0][6].ToString() + ","
                                    //           + d2.Rows[0][7].ToString() + "," + d2.Rows[0][8].ToString() + "," + d2.Rows[0][9].ToString() + ","
                                    //           + d2.Rows[0][10].ToString() + "," + d2.Rows[0][11].ToString() + "," + d2.Rows[0][12].ToString() + ","
                                    //           + d2.Rows[0][13].ToString() + "," + d2.Rows[0][14].ToString() + "," + d2.Rows[0][15].ToString() + ","
                                    //           + d2.Rows[0][16].ToString() + "," + d2.Rows[0][17].ToString() + "," + d2.Rows[0][18].ToString() + "," + "1" + ","
                                    //           + "OK-OEE_Default", System.AppDomain.CurrentDomain.BaseDirectory + "\\OEE_MQTT_Default数据\\");
                                    //        _homefrm.UiText(d2.Rows[0][4].ToString(), "txtOEE_SerialNumber");
                                    //        _homefrm.UiText(d2.Rows[0][6].ToString(), "txtOEE_Fixture");
                                    //        _homefrm.UiText(d2.Rows[0][7].ToString(), "txtOEE_StartTime");
                                    //        _homefrm.UiText(d2.Rows[0][8].ToString(), "txtOEE_EndTime");
                                    //        _homefrm.UiText(d2.Rows[0][9].ToString(), "txtOEE_Status");
                                    //        _homefrm.UiText(d2.Rows[0][10].ToString(), "txtOEE_ActualCT");
                                    //        _homefrm.UiText(d2.Rows[0][11].ToString(), "txtOEE_sw");
                                    //        _homefrm.UiText(d2.Rows[0][12].ToString(), "txtOEE_ScanCount");
                                    //        Log.WriteLog("OEE_Default_UI更新成功");
                                    //        Global.respond.TryRemove(guid, out Global.Respond);
                                    //        string DeleteStr = "delete from OEE_DefaultSendNG where ID = (SELECT MIN(ID) FROM OEE_DefaultSendNG)";
                                    //        SQL.ExecuteUpdate(DeleteStr);
                                    //    }
                                    //}
                                    //else
                                    //{
                                    //    // RetryNum++;
                                    //    Global.ConnectOEEFlag = false;
                                    //    _homefrm.AppendRichText("缓存发送失败,", "rtx_OEEDefaultMsg");
                                    //    Log.WriteLog("OEE_Default自动缓存发送失败,OEE网络异常,超时无反馈");
                                    //    break;
                                    //}
                                }
                                else
                                {
                                    Log.WriteLog("补传前获取到上一次的结果" + JsonConvert.SerializeObject(Global.respond[guid]) + "不上传");
                                    Global.respond.TryRemove(guid, out Global.Respond);
                                    string DeleteStr = "delete from OEE_DefaultSendNG where ID = (SELECT MIN(ID) FROM OEE_DefaultSendNG)";
                                    SQL.ExecuteUpdate(DeleteStr);
                                }

                                Thread.Sleep(1000);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.WriteLog("OEE_Default数据缓存重传异常" + ex.ToString().Replace("\r\n", ""));
                    }

                }
                //删除超过两天的OEE产品数据
                try
                {
                    string SelectStr = string.Format("select count(*) from OEE_DefaultSendNG");
                    DataTable d1 = SQL.ExecuteQuery(SelectStr);
                    if (Convert.ToInt32(d1.Rows[0][0].ToString()) > 0)
                    {
                        for (int j = 0; j < Convert.ToInt32(d1.Rows[0][0].ToString()); j++)
                        {
                            string SelectStr2 = "select * from OEE_DefaultSendNG where ID =(SELECT MIN(ID) FROM OEE_DefaultSendNG)";
                            DataTable d2 = SQL.ExecuteQuery(SelectStr2);
                            TimeSpan ts = DateTime.Now - Convert.ToDateTime(d2.Rows[0][19]);
                            if (ts.Days > 2)
                            {
                                string DeleteStr = "delete from OEE_DefaultSendNG where ID = (SELECT MIN(ID) FROM OEE_DefaultSendNG)";
                                SQL.ExecuteUpdate(DeleteStr);
                                string OEE_Default = string.Format("{{\"GUID\":\"{0}\",\"EMT\":\"{1}\",\"SerialNumber\":\"{2}\",\"BGBarcode\":\"{3}\",\"Fixture\":\"{4}\",\"StartTime\":\"{5}\",\"EndTime\":\"{6}\",\"Status\":\"{7}\",\"ActualCT\":\"{8}\",\"SwVersion\":\"{9}\",\"ScanCount\":\"{10}\",\"ErrorCode\":\"{11}\",\"PFErrorCode\":\"{12}\",\"Cavity\":\"{13}\",\"ClientPcName\":\"{14}\",\"MAC\":\"{15}\",\"IP\":\"{16}\",\"EventTime\":\"{17}\"}}", d2.Rows[0][2].ToString(), d2.Rows[0][3].ToString(), d2.Rows[0][4].ToString(), d2.Rows[0][5].ToString(), d2.Rows[0][6].ToString(), d2.Rows[0][7].ToString(), d2.Rows[0][8].ToString(), d2.Rows[0][9].ToString(), d2.Rows[0][10].ToString(), d2.Rows[0][11].ToString(), d2.Rows[0][12].ToString(), d2.Rows[0][13].ToString(), d2.Rows[0][14].ToString(), d2.Rows[0][15].ToString(), d2.Rows[0][16].ToString(), d2.Rows[0][17].ToString(), d2.Rows[0][18].ToString(), d2.Rows[0][19].ToString());
                                Log.WriteLog("删除超时2天的OEE数据:" + OEE_Default);
                                Thread.Sleep(500);
                            }

                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.WriteLog("超时2天删除OEE数据失败" + ex.ToString());
                }
                Thread.Sleep(1000);
            }
        }

        //private void OEE_Materiel_Retry(object ob)//OEE 小料抛料计数上传失败重新上传
        //{
        //    var IP = GetIp();
        //    var Mac = GetMac();
        //    while (true)
        //    {
        //        if (ConnectOEEFlag == true)
        //        {
        //            OEE_MaterielData data = new OEE_MaterielData();
        //            string msg = "";
        //            try
        //            {
        //                if (Access.QueryNumber("PDCA", "OEE_MaterielData") > 0)
        //                {
        //                    Log.WriteLog("OEE_MaterielData上传失败数据重新上传");
        //                    for (int j = 0; j < Access.QueryNumber("PDCA", "OEE_MaterielData"); j++)
        //                    {
        //                        Access.QueryLastData_OEE_Materiel("PDCA", "OEE_MaterielData", out data);
        //                        string parttype = data.uacount.ToString() + "," + data.lacount.ToString();
        //                        string OEE_MaterielData = string.Format("{{\"date\":\"{0}\",\"count\":\"{1}\",\"totalcount\":\"{2}\",\"parttype\":\"{3}\"}}", data.date, data.count, data.totalcount, parttype);
        //                        Log.WriteLog("OEE_MaterielData缓存:" + OEE_MaterielData);
        //                        AppendText(listBox8, OEE_MaterielData);
        //                        var rst = RequestAPI.Request(inidata.productconfig.URL1, inidata.productconfig.URL2, IP, Mac, inidata.productconfig.Dsn, inidata.productconfig.authCode, 6, OEE_MaterielData, out msg);
        //                        if (rst)
        //                        {
        //                            AppendText(listBox8, msg);
        //                            ConnectOEEFlag = true;
        //                            Log.WriteLog("OEE_MaterielData自动缓存上传OK:" + msg);
        //                            Log.WriteCSV(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + data.count + "," + data.totalcount + "," + parttype + "," + "OK-OEE_MaterielData缓存", System.AppDomain.CurrentDomain.BaseDirectory + "\\OEE_小料抛料计数数据\\");
        //                            Access.DeleteData("PDCA", "OEE_MaterielData");
        //                        }
        //                        else
        //                        {
        //                            AppendText(listBox8, msg);
        //                            ConnectOEEFlag = false;
        //                            Log.WriteLog("OEE_MaterielData自动缓存上传NG:" + msg);
        //                            Log.WriteCSV(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + data.count + "," + data.totalcount + "," + parttype + "," + "NG-OEE_MaterielData缓存", System.AppDomain.CurrentDomain.BaseDirectory + "\\OEE_小料抛料计数数据\\");
        //                        }
        //                        Thread.Sleep(1000);
        //                    }
        //                }
        //            }
        //            catch
        //            {
        //                Log.WriteLog("OEE_MaterielData数据缓存重传异常");
        //            }
        //        }
        //        Thread.Sleep(20);
        //    }
        //}
        #endregion

        #region 断线重连
        private void PLC_autolink(object ob)
        {
            while (true)
            {
                try
                {
                    if (Link_PLC == true && (Global.PLC_Client.client == null || !Global.PLC_Client.IsConnected))
                    {
                        //连接PLC

                        Global.PLC_Client.sClient(Global.inidata.productconfig.Plc_IP, Global.inidata.productconfig.Plc_Port);
                        Global.PLC_Client.Connect();
                        Log.WriteLog("PLC通信 1 已建立");
                        isopen = true;
                    }
                    if (Link_PLC == true && (Global.PLC_Client2.client == null || !Global.PLC_Client2.IsConnected))
                    {
                        Global.PLC_Client2.sClient(Global.inidata.productconfig.Plc_IP, Global.inidata.productconfig.Plc_Port2);
                        Global.PLC_Client2.Connect();
                        Log.WriteLog("PLC通信 2 已建立");
                        isopen = true;
                    }
                }
                catch
                {
                    Log.WriteLog("PLC通信无法连接");
                    Environment.Exit(1);
                }
                Thread.Sleep(100);
            }
        }
        #endregion

        #region Timer定时方法
        private void timer1_Tick(object sender, EventArgs e)
        {
            tsslabelcolor(tsslbl_time, Color.Black, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            switch (Global.Login)
            {
                case Global.LoginLevel.Operator:
                    tsslabelcolor(tsslbl_UserLogin, Color.Black, "当前用户：操作员");
                    Btn_IfEnable(btn_Manual, true);
                    Btn_IfEnable(btn_Setting, false);
                    break;
                case Global.LoginLevel.Technician:
                    tsslabelcolor(tsslbl_UserLogin, Color.Black, "当前用户：技术员");
                    Btn_IfEnable(btn_Manual, true);
                    Btn_IfEnable(btn_Setting, false);
                    break;
                case Global.LoginLevel.Administrator:
                    tsslabelcolor(tsslbl_UserLogin, Color.Black, "当前用户：工程师");
                    Btn_IfEnable(btn_Manual, true);
                    Btn_IfEnable(btn_Setting, true);
                    break;
            }
            SetText(Global.ErrorIndex);
            Global.ErrorIndex++;
        }

        private void InitTimer()//初始化定时器
        {
            int interval = 1000;
            Global.timer = new System.Timers.Timer(interval);
            Global.timer.AutoReset = true;
            Global.timer.Elapsed += new System.Timers.ElapsedEventHandler(TimerUp);
            Global.EatAndRest_timer = new System.Timers.Timer(interval);
            Global.EatAndRest_timer.AutoReset = true;
            Global.EatAndRest_timer.Elapsed += new System.Timers.ElapsedEventHandler(TimerUp2);
        }

        private void TimerUp(object sender, System.Timers.ElapsedEventArgs e)//定时执行事件-首件模式持续时间过久提示！
        {
            try
            {
                Global.currentCount += 1;
                if (Global.currentCount == 900)//15分钟
                {
                    MessageBox.Show("首件开始已持续15分钟！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                if (Global.currentCount % 3600 == 0 && ReadStatus[0] == 1)//计时开始整小时并且判断当前状态是否为待料状态
                {
                    _manualfrm.Btn_UpLoad_errortime_Click(null, null);
                    Log.WriteLog(string.Format("首件模式已持续{0}小时以上，并且机台处于待料状态，自动退出！", Global.currentCount / 3600));
                }
                if (Global.currentCount > 3600 && ReadStatus[0] != 1)
                {
                    Global.PLC_Client.WritePLC_D(13024, new short[] { 2 });//通知PLC首件超时
                    Thread.Sleep(2000);
                    _manualfrm.Btn_UpLoad_errortime_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("执行定时到点事件失败：" + ex.Message);
            }
        }
        private void TimerUp2(object sender, System.Timers.ElapsedEventArgs e)//定时执行事件-吃饭休息模式持续一小时自动退出！
        {
            try
            {
                Global.EatAndRest_currentCount += 1;
                if (ReadStatus[0] != 1 && Global.EatAndRest_currentCount < 3600)//吃饭休息开始后一小时内机台实时状态非待料状态时，自动结束吃饭休息
                {
                    _manualfrm.Btn_UpLoad_EatandRest_Click(null, null);
                    Log.WriteLog("一小时内机台实时状态非待料状态时，自动退出！");
                }
                if (Global.EatAndRest_currentCount == 3600)//1小时
                {
                    _manualfrm.Btn_UpLoad_EatandRest_Click(null, null);
                    Log.WriteLog("吃饭休息模式已持续一小时，自动退出！");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("执行定时到点事件失败：" + ex.Message);
            }
        }

        #endregion

        #region 手动按钮
        private void btn_home_Click(object sender, EventArgs e)
        {
            btn_home.Image = Global.ReadImageFile(LogPath + "home1" + ".bmp");
            btn_DataStatistics.Image = Global.ReadImageFile(LogPath + "tu" + ".bmp");
            btn_IOMonitor.Image = Global.ReadImageFile(LogPath + "monitor" + ".bmp");
            btn_Manual.Image = Global.ReadImageFile(LogPath + "man" + ".bmp");
            btn_Setting.Image = Global.ReadImageFile(LogPath + "set" + ".bmp");
            btn_Abnormal.Image = Global.ReadImageFile(LogPath + "alarm" + ".bmp");
            btn_UserLogin.Image = Global.ReadImageFile(LogPath + "user" + ".bmp");
            btn_Help.Image = Global.ReadImageFile(LogPath + "help" + ".bmp");
            label_MachineName.BackColor = Color.White;
            if (!ExistsMdiChildrenInstance(_homefrm.Name))
            {
                this._homefrm.MdiParent = this;
                this._homefrm.Dock = DockStyle.Fill;
                this._homefrm.Show();
            }
            else
            {
                ShowView();
                this._homefrm.Activate();
            }
            //Cursor.Current = Cursors.Arrow;

        }

        private void btn_DataStatistics_Click(object sender, EventArgs e)
        {
            btn_DataStatistics.Image = Global.ReadImageFile(LogPath + "tu1" + ".bmp");
            btn_home.Image = Global.ReadImageFile(LogPath + "home" + ".bmp");
            btn_IOMonitor.Image = Global.ReadImageFile(LogPath + "monitor" + ".bmp");
            btn_Manual.Image = Global.ReadImageFile(LogPath + "man" + ".bmp");
            btn_Setting.Image = Global.ReadImageFile(LogPath + "set" + ".bmp");
            btn_Abnormal.Image = Global.ReadImageFile(LogPath + "alarm" + ".bmp");
            btn_UserLogin.Image = Global.ReadImageFile(LogPath + "user" + ".bmp");
            btn_Help.Image = Global.ReadImageFile(LogPath + "help" + ".bmp");
            label_MachineName.BackColor = Color.White;
            if (!ExistsMdiChildrenInstance(_datastatisticsfrm.Name))
            {
                this._datastatisticsfrm.MdiParent = this;
                this._datastatisticsfrm.Dock = DockStyle.Fill;
                this._datastatisticsfrm.Show();
            }
            else
            {
                _datastatisticsfrm.Show();
                this._datastatisticsfrm.Activate();
            }
        }

        private void btn_IOMonitor_Click(object sender, EventArgs e)
        {
            btn_IOMonitor.Image = Global.ReadImageFile(LogPath + "monitor1" + ".bmp");
            btn_DataStatistics.Image = Global.ReadImageFile(LogPath + "tu" + ".bmp");
            btn_home.Image = Global.ReadImageFile(LogPath + "home" + ".bmp");
            btn_Manual.Image = Global.ReadImageFile(LogPath + "man" + ".bmp");
            btn_Setting.Image = Global.ReadImageFile(LogPath + "set" + ".bmp");
            btn_Abnormal.Image = Global.ReadImageFile(LogPath + "alarm" + ".bmp");
            btn_UserLogin.Image = Global.ReadImageFile(LogPath + "user" + ".bmp");
            btn_Help.Image = Global.ReadImageFile(LogPath + "help" + ".bmp");
            label_MachineName.BackColor = Color.White;
            if (!ExistsMdiChildrenInstance(_iomonitorfrm.Name))
            {
                this._iomonitorfrm.MdiParent = this;
                this._iomonitorfrm.Dock = DockStyle.Fill;
                this._iomonitorfrm.Show();
            }
            else
            {
                _iomonitorfrm.Show();
                this._iomonitorfrm.Activate();
            }
        }

        private void btn_Manual_Click(object sender, EventArgs e)
        {
            btn_Manual.Image = Global.ReadImageFile(LogPath + "man1" + ".bmp");
            btn_IOMonitor.Image = Global.ReadImageFile(LogPath + "monitor" + ".bmp");
            btn_DataStatistics.Image = Global.ReadImageFile(LogPath + "tu" + ".bmp");
            btn_home.Image = Global.ReadImageFile(LogPath + "home" + ".bmp");
            btn_Setting.Image = Global.ReadImageFile(LogPath + "set" + ".bmp");
            btn_Abnormal.Image = Global.ReadImageFile(LogPath + "alarm" + ".bmp");
            btn_UserLogin.Image = Global.ReadImageFile(LogPath + "user" + ".bmp");
            btn_Help.Image = Global.ReadImageFile(LogPath + "help" + ".bmp");
            label_MachineName.BackColor = Color.White;
            if (!ExistsMdiChildrenInstance(_manualfrm.Name))
            {
                this._manualfrm.MdiParent = this;
                this._manualfrm.Dock = DockStyle.Fill;
                this._manualfrm.Show();
            }
            else
            {
                _manualfrm.Show();
                this._manualfrm.Activate();
            }
        }

        private void btn_Setting_Click(object sender, EventArgs e)
        {
            btn_Setting.Image = Global.ReadImageFile(LogPath + "set1" + ".bmp");
            btn_Manual.Image = Global.ReadImageFile(LogPath + "man" + ".bmp");
            btn_IOMonitor.Image = Global.ReadImageFile(LogPath + "monitor" + ".bmp");
            btn_DataStatistics.Image = Global.ReadImageFile(LogPath + "tu" + ".bmp");
            btn_home.Image = Global.ReadImageFile(LogPath + "home" + ".bmp");
            btn_Abnormal.Image = Global.ReadImageFile(LogPath + "alarm" + ".bmp");
            btn_UserLogin.Image = Global.ReadImageFile(LogPath + "user" + ".bmp");
            btn_Help.Image = Global.ReadImageFile(LogPath + "help" + ".bmp");
            label_MachineName.BackColor = Color.White;
            if (!ExistsMdiChildrenInstance(_sttingfrm.Name))
            {
                this._sttingfrm.MdiParent = this;
                this._sttingfrm.Dock = DockStyle.Fill;
                this._sttingfrm.Show();
            }
            else
            {
                _sttingfrm.Show();
                this._sttingfrm.Activate();
            }
        }

        private void btn_Abnormal_Click(object sender, EventArgs e)
        {
            btn_Abnormal.Image = Global.ReadImageFile(LogPath + "alarm1" + ".bmp");
            btn_Setting.Image = Global.ReadImageFile(LogPath + "set" + ".bmp");
            btn_Manual.Image = Global.ReadImageFile(LogPath + "man" + ".bmp");
            btn_IOMonitor.Image = Global.ReadImageFile(LogPath + "monitor" + ".bmp");
            btn_DataStatistics.Image = Global.ReadImageFile(LogPath + "tu" + ".bmp");
            btn_home.Image = Global.ReadImageFile(LogPath + "home" + ".bmp");
            btn_UserLogin.Image = Global.ReadImageFile(LogPath + "user" + ".bmp");
            btn_Help.Image = Global.ReadImageFile(LogPath + "help" + ".bmp");
            label_MachineName.BackColor = Color.White;
            if (!ExistsMdiChildrenInstance(_Abnormalfrm.Name))
            {
                this._Abnormalfrm.MdiParent = this;
                this._Abnormalfrm.Dock = DockStyle.Fill;
                this._Abnormalfrm.Show();
            }
            else
            {
                _Abnormalfrm.Show();
                this._Abnormalfrm.Activate();
            }
        }

        private void btn_UserLogin_Click(object sender, EventArgs e)
        {
            btn_UserLogin.Image = Global.ReadImageFile(LogPath + "user1" + ".bmp");
            btn_Abnormal.Image = Global.ReadImageFile(LogPath + "alarm" + ".bmp");
            btn_Setting.Image = Global.ReadImageFile(LogPath + "set" + ".bmp");
            btn_Manual.Image = Global.ReadImageFile(LogPath + "man" + ".bmp");
            btn_IOMonitor.Image = Global.ReadImageFile(LogPath + "monitor" + ".bmp");
            btn_DataStatistics.Image = Global.ReadImageFile(LogPath + "tu" + ".bmp");
            btn_home.Image = Global.ReadImageFile(LogPath + "home" + ".bmp");
            btn_Help.Image = Global.ReadImageFile(LogPath + "help" + ".bmp");
            label_MachineName.BackColor = Color.White;
            if (!ExistsMdiChildrenInstance(_userloginfrm.Name))
            {
                this._userloginfrm.MdiParent = this;
                this._userloginfrm.Dock = DockStyle.Fill;



                this._userloginfrm.Show();
            }
            else
            {
                _userloginfrm.Show();
                this._userloginfrm.Activate();
            }
            ///20230313Add
            //this._userloginfrm.CheckUserLogin();
            //if (Global.inidata.productconfig.Material_online == "1") { this._userloginfrm.tableLayoutPanel12.Visible = false; }
            //else
            //{
            //    this._userloginfrm.tableLayoutPanel12.Visible = true;
            //}
        }

        private void btn_Help_Click(object sender, EventArgs e)
        {
            btn_Help.Image = Global.ReadImageFile(LogPath + "help1" + ".bmp");
            btn_UserLogin.Image = Global.ReadImageFile(LogPath + "user" + ".bmp");
            btn_Abnormal.Image = Global.ReadImageFile(LogPath + "alarm" + ".bmp");
            btn_Setting.Image = Global.ReadImageFile(LogPath + "set" + ".bmp");
            btn_Manual.Image = Global.ReadImageFile(LogPath + "man" + ".bmp");
            btn_IOMonitor.Image = Global.ReadImageFile(LogPath + "monitor" + ".bmp");
            btn_DataStatistics.Image = Global.ReadImageFile(LogPath + "tu" + ".bmp");
            btn_home.Image = Global.ReadImageFile(LogPath + "home" + ".bmp");
            label_MachineName.BackColor = Color.White;
            if (!ExistsMdiChildrenInstance(_helpfrm.Name))
            {
                this._helpfrm.MdiParent = this;
                this._helpfrm.Dock = DockStyle.Fill;
                this._helpfrm.Show();
            }
            else
            {
                _helpfrm.Show();
                this._helpfrm.Activate();
            }
        }

        private void label_MachineName_Click(object sender, EventArgs e)
        {
            label_MachineName.BackColor = Color.DarkSeaGreen;
            btn_Help.Image = Global.ReadImageFile(LogPath + "help" + ".bmp");
            btn_UserLogin.Image = Global.ReadImageFile(LogPath + "user" + ".bmp");
            btn_Abnormal.Image = Global.ReadImageFile(LogPath + "alarm" + ".bmp");
            btn_Setting.Image = Global.ReadImageFile(LogPath + "set" + ".bmp");
            btn_Manual.Image = Global.ReadImageFile(LogPath + "man" + ".bmp");
            btn_IOMonitor.Image = Global.ReadImageFile(LogPath + "monitor" + ".bmp");
            btn_DataStatistics.Image = Global.ReadImageFile(LogPath + "tu" + ".bmp");
            btn_home.Image = Global.ReadImageFile(LogPath + "home" + ".bmp");
            if (!ExistsMdiChildrenInstance(_machinefrm.Name))
            {
                this._machinefrm.MdiParent = this;
                this._machinefrm.Dock = DockStyle.Fill;
                this._machinefrm.Show();
            }
            else
            {
                _machinefrm.Show();
                this._machinefrm.Activate();
            }
        }

        public void ButtonFlag(bool Flag, Button bt)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new buttonflag(ButtonFlag), new object[] { Flag, bt });
                return;
            }
            bt.Enabled = Flag;
        }
        #endregion

        #region 委托显示
        public void Btn_IfEnable(ToolStripButton btn, bool b)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new btnEnable(Btn_IfEnable), new object[] { btn, b });
                return;
            }
            btn.Enabled = b;
        }

        public void tsslabelcolor(ToolStripStatusLabel lb, Color color, string str)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new tssLabelcolor(tsslabelcolor), new object[] { lb, color, str });
                return;
            }
            lb.ForeColor = color;
            lb.Text = str;
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
            dgv.RowsDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            //设置表格列字体居中
            dgv.EnableHeadersVisualStyles = false;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("微软雅黑", 12, FontStyle.Bold);
            dgv.RowsDefaultCellStyle.Font = new Font("微软雅黑", 9);
            dgv.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgv.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.False;//禁止列标题换行
        }
        public void AppendRichText(string msg, RichTextBox richtextbox)
        {
            if (this.InvokeRequired)
            {

                this.BeginInvoke(new AddItemToRichTextBoxDelegate(AppendRichText), new object[] { msg, richtextbox });
                return;
            }

            richtextbox.AppendText(msg + "\r\n");

        }
        public void ShowData(DataGridView dgv, DataTable dt, int index)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new ShowDataTable(ShowData), new object[] { dt, index });
                return;
            }
            switch (index)
            {
                case 0:
                    dgv.DataSource = dt;
                    break;
                case 1:
                    break;
                default:
                    break;
            }
        }

        public void AppendText(ListBox listbox1, string msg, int index)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new AddItemToListBoxDelegate(AppendText), new object[] { listbox1, msg, index });
                return;
            }
            listbox1.SelectedItem = listbox1.Items.Count;
            switch (index)
            {
                case 0:
                    listbox1.Items.Add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":" + msg);
                    break;
                case 1:
                    listbox1.Items.Add(msg);
                    break;
            }
            listbox1.TopIndex = listbox1.Items.Count - 1;
        }

        public void UiText(string str1, TextBox tb)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new ShowTxt(UiText), new object[] { str1, tb });
                return;
            }
            tb.Text = str1;
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

        public void labelenvision(Label lb, string txt)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Labelvision(labelenvision), new object[] { lb, txt });
                return;
            }
            lb.Text = txt;
        }

        public void ShowStatus(string txt, Color color, int id)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new ShowPlcStatue(ShowStatus), new object[] { txt, color, id });
                return;
            }
            switch (id)
            {
                case 0:
                    tssl_PLCStatus.Text = txt;
                    tssl_PLCStatus.BackColor = color;
                    break;
                case 1:
                    tssl_PDCAStatus.Text = txt;
                    tssl_PDCAStatus.BackColor = color;
                    break;
                case 2:
                    tssl_TraceStatus.Text = txt;
                    tssl_TraceStatus.BackColor = color;
                    break;
                case 3:
                    tssl_OEEStatus.Text = txt;
                    tssl_OEEStatus.BackColor = color;
                    break;
                case 4:
                    tssl_HansStatus.Text = txt;
                    tssl_HansStatus.BackColor = color;
                    break;
                case 5:
                    tssl_ReaderStatus.Text = txt;
                    tssl_ReaderStatus.BackColor = color;
                    break;
                case 6:
                    //bailStatus3.Text = txt;
                    //bailStatus3.BackColor = color;
                    break;
                case 7:
                    TraceParamStatus.Text = txt;
                    TraceParamStatus.BackColor = color;
                    break;
                default:
                    break;
            }
        }

        public void ListData(string str)
        {
            //if (this.InvokeRequired)
            //{
            //    this.BeginInvoke(new ShowList(ListData), new object[] { str });
            //    return;
            //}
            //listBox1.Items.Clear();
            //listBox1.Items.Add(str);
        }
        #endregion

        # region 删除长期文件
        private void ClearPic()
        {
            try
            {
                while (true)
                {
                    if (DateTime.Now.Minute == 20 && DateTime.Now.Second == 0)
                    {
                        ClearOverdueFile cof1;
                        ClearOverdueFile cof2;
                        cof1 = new ClearOverdueFile(@Global.inidata.productconfig.PICPath, int.Parse(Global.inidata.productconfig.delete_time));     //超过设定时间*24*60 
                        cof1.FileDeal();
                        Thread.Sleep(500);
                        cof2 = new ClearOverdueFile("D:\\SendPicture", int.Parse(Global.inidata.productconfig.delete_time));     //超过设定时间*24*60 
                        cof2.FileDeal();
                    }
                    Thread.Sleep(1000);//1s循环扫描
                }
            }
            catch (Exception ex)
            {
                Log.WriteLog("循环删除文件时间出现错误：" + ex.Message);
            }
        }
        # endregion

        #region TCP/IP通讯 事件方法
        //与TCP服务器已连接
        void client1_ServerConnected(object sender, TcpServerConnectedEventArgs e)
        {
            Log.WriteLog("Client1已连接");
            //Global.PLC_Client.WritePLC_D(11320, new short[] { 1 });
        }
        void client2_ServerConnected(object sender, TcpServerConnectedEventArgs e)
        {
            Log.WriteLog("Client2已连接");
            //Global.PLC_Client.WritePLC_D(11321, new short[] { 1 });
        }

        //与TCP服务器断开连接
        void client1_ServerDisconnected(object sender, TcpServerDisconnectedEventArgs e)
        {
            //Log.WriteLog("Client1已断开");
            // Global.PLC_Client.WritePLC_D(11320, new short[] { 2 });
        }
        void client2_ServerDisconnected(object sender, TcpServerDisconnectedEventArgs e)
        {
            //Log.WriteLog("Client2已断开");
            //  Global.PLC_Client.WritePLC_D(11321, new short[] { 2 });
        }

        ///20230228Add
        public void SendlaseripqcData(string[] data)
        {
            try
            {

                string callResult;
                string errMsg;
                serials serials = new serials() { band = data[0] };

                string msg = JsonConvert.SerializeObject(new Laseripqc()
                {
                    serials = serials,
                    data = new Models.data()
                    {
                        items = new items
                        {
                            actual_power = data[14],
                            actual_power_judgment = data[9],
                            actual_power_measure_time = data[10],
                            laser_settings_frequency = data[17],
                            laser_settings_laser_speed = data[18],
                            laser_settings_peak_power = data[21],
                            laser_settings_power = data[15],
                            laser_settings_pulse_energy = data[20],
                            laser_settings_q_release = data[19],
                            laser_settings_waveform = data[16],
                            machine_RFID = data[12],
                            machine_id = data[11],
                            power_ll = data[1],
                            power_ul = data[2],
                            process_name = "trch-bt-wld",
                            station_id = Global.inidata.productconfig.Station_id_ipqc,
                            //station_id = "JACD_B01-2F-OQC01_83003_DEVELOPMENT333",
                        }
                    }
                });
                var result = RequestAPI2.CallBobcat4("http://localhost:8765/v2/logs?agent_alias=laser-ipqc", msg, out callResult, out errMsg, false);

                ///Test
                Log.WriteCSV(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + msg + "," + callResult + "," + errMsg, System.AppDomain.CurrentDomain.BaseDirectory + "\\Hans能量检测\\");
                //string insertStr1 = string.Format("insert into HansData([DateTime],[SN],[Nut],[power_ll],[power_ul],[pattern_type],[spot_size],[hatch],[swing_amplitude],[swing_freq],[JudgeResult],[MeasureTime],[MachineSN],[PulseProfile_measure],[ActualPower],[Power_measure],[WaveForm_measure],[Frequency_measure],[LinearSpeed_measure],[QRelease_measure],[PulseEnergy_measure],[PeakPower_measure],[laser_sensor],[pulse_profile],[laser_power],[frequency],[waveform],[pulse_energy],[laser_speed],[jump_speed],[jump_delay],[scanner_delay],[Station]) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}','{20}','{21}','{22}','{23}','{24}','{25}','{26}','{27}','{28}','{29}','{30}','{31}','{32}')",
                //      DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), data[0], data[23], data[1], data[2], data[3], data[4], data[5], data[6], data[7], data[9], data[10], data[11], data[13], data[14], data[15], data[16], data[17], data[18], data[19], data[20], data[21], data[22], data[24], data[25], data[26], data[27], data[28], data[29], data[30], data[31], data[32], "L_Bracket");
                //int r = SQL.ExecuteUpdate(insertStr1);
                //Log.WriteLog(string.Format("插入了{0}行{1}焊接参数", r, data[23]));
                //string insertStr2 = string.Format("insert into HansData([DateTime],[SN],[Nut],[power_ll],[power_ul],[pattern_type],[spot_size],[hatch],[swing_amplitude],[swing_freq],[JudgeResult],[MeasureTime],[MachineSN],[PulseProfile_measure],[ActualPower],[Power_measure],[WaveForm_measure],[Frequency_measure],[LinearSpeed_measure],[QRelease_measure],[PulseEnergy_measure],[PeakPower_measure],[laser_sensor],[pulse_profile],[laser_power],[frequency],[waveform],[pulse_energy],[laser_speed],[jump_speed],[jump_delay],[scanner_delay],[Station]) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}','{20}','{21}','{22}','{23}','{24}','{25}','{26}','{27}','{28}','{29}','{30}','{31}','{32}')",
                //  DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), data[0], data[51], data[1], data[2], data[3], data[4], data[5], data[6], data[7], data[9], data[10], data[11], data[13], data[14], data[15], data[16], data[17], data[18], data[19], data[20], data[21], data[22], data[52], data[53], data[54], data[55], data[56], data[57], data[58], data[59],data[60], "L_Bracket");
                //int r2 = SQL.ExecuteUpdate(insertStr2);
                //Log.WriteLog(string.Format("插入了{0}行{1}焊接参数", r2, data[51]));
                //string insertStr3 = string.Format("insert into HansData([DateTime],[SN],[Nut],[power_ll],[power_ul],[pattern_type],[spot_size],[hatch],[swing_amplitude],[swing_freq],[JudgeResult],[MeasureTime],[MachineSN],[PulseProfile_measure],[ActualPower],[Power_measure],[WaveForm_measure],[Frequency_measure],[LinearSpeed_measure],[QRelease_measure],[PulseEnergy_measure],[PeakPower_measure],[laser_sensor],[pulse_profile],[laser_power],[frequency],[waveform],[pulse_energy],[laser_speed],[jump_speed],[jump_delay],[scanner_delay],[Station]) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}','{20}','{21}','{22}','{23}','{24}','{25}','{26}','{27}','{28}','{29}','{30}','{31}','{32}')",
                //  DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), data[0], data[79], data[1], data[2], data[3], data[4], data[5], data[6], data[7], data[9], data[10], data[11], data[13], data[14], data[15], data[16], data[17], data[18], data[19], data[20], data[21], data[22], data[80], data[81], data[82], data[83], data[84], data[85], data[86], data[87], data[88], "L_Bracket");
                //int r3 = SQL.ExecuteUpdate(insertStr3);
                //Log.WriteLog(string.Format("插入了{0}行{1}焊接参数", r3, data[79]));
            }
            catch (Exception ex)
            {
                Log.WriteCSV(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + ex.Message + ",", System.AppDomain.CurrentDomain.BaseDirectory + "\\Hans能量检测\\");
            }
        }

        //接收到TCP服务器反馈
        void client1_PlaintextReceived(object sender, TcpDatagramReceivedEventArgs<string> e)
        {
            Log.WriteLog("接收到Server1数据：" + e.Datagram.Replace("\r\n", ""));
            if (e.Datagram == "OK")
            {
                Global.PLC_Client.WritePLC_D(12621, new short[] { 1 });
                flag1 = true;
            }
            try
            {
                if (e.Datagram.Contains("FM"))
                {
                    Global.PLC_Client.WritePLC_D(12651, new short[] { 1 });
                    string[] data = e.Datagram.Replace("\r\n", "").Split(';');

                    ///20230228Update
                    int value;
                    if (int.TryParse(data[8], out value))
                    {
                        //if (value == 1)
                        //{
                        //    Task.Factory.StartNew(() =>
                        //    {
                        //        //SendlaseripqcData(data);
                        //    });
                        //}
                    };
                    ///20230228update
                    string insertStr1 = string.Format("insert into HansData([DateTime],[SN],[Nut],[power_ll],[power_ul],[pattern_type],[spot_size],[hatch],[swing_amplitude],[swing_freq],[JudgeResult],[MeasureTime],[MachineSN],[PulseProfile_measure],[ActualPower],[Power_measure],[WaveForm_measure],[Frequency_measure],[LinearSpeed_measure],[QRelease_measure],[PulseEnergy_measure],[PeakPower_measure],[laser_sensor],[pulse_profile],[laser_power],[frequency],[waveform],[pulse_energy],[laser_speed],[jump_speed],[jump_delay],[scanner_delay],[Station]) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}','{20}','{21}','{22}','{23}','{24}','{25}','{26}','{27}','{28}','{29}','{30}','{31}','{32}')",
                          DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), data[0], data[23], data[1], data[2], data[3], data[4], data[5], data[6], data[7], data[9], data[10], data[11], data[13], data[14], data[15], data[16], data[17], data[18], data[19], data[20], data[21], data[22], data[24], data[25], data[26], data[27], data[28], data[29], data[30], data[31], data[32], "L_Bracket");
                    int r = SQL.ExecuteUpdate(insertStr1);
                    Log.WriteLog(string.Format("插入了{0}行{1}焊接参数", r, data[23]));
                    string insertStr2 = string.Format("insert into HansData([DateTime],[SN],[Nut],[power_ll],[power_ul],[pattern_type],[spot_size],[hatch],[swing_amplitude],[swing_freq],[JudgeResult],[MeasureTime],[MachineSN],[PulseProfile_measure],[ActualPower],[Power_measure],[WaveForm_measure],[Frequency_measure],[LinearSpeed_measure],[QRelease_measure],[PulseEnergy_measure],[PeakPower_measure],[laser_sensor],[pulse_profile],[laser_power],[frequency],[waveform],[pulse_energy],[laser_speed],[jump_speed],[jump_delay],[scanner_delay],[Station]) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}','{20}','{21}','{22}','{23}','{24}','{25}','{26}','{27}','{28}','{29}','{30}','{31}','{32}')",
                      DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), data[0], data[51], data[1], data[2], data[3], data[4], data[5], data[6], data[7], data[9], data[10], data[11], data[13], data[14], data[15], data[16], data[17], data[18], data[19], data[20], data[21], data[22], data[52], data[53], data[54], data[55], data[56], data[57], data[58], data[59], data[60], "L_Bracket");
                    int r2 = SQL.ExecuteUpdate(insertStr2);
                    Log.WriteLog(string.Format("插入了{0}行{1}焊接参数", r2, data[51]));
                    string insertStr3 = string.Format("insert into HansData([DateTime],[SN],[Nut],[power_ll],[power_ul],[pattern_type],[spot_size],[hatch],[swing_amplitude],[swing_freq],[JudgeResult],[MeasureTime],[MachineSN],[PulseProfile_measure],[ActualPower],[Power_measure],[WaveForm_measure],[Frequency_measure],[LinearSpeed_measure],[QRelease_measure],[PulseEnergy_measure],[PeakPower_measure],[laser_sensor],[pulse_profile],[laser_power],[frequency],[waveform],[pulse_energy],[laser_speed],[jump_speed],[jump_delay],[scanner_delay],[Station]) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}','{20}','{21}','{22}','{23}','{24}','{25}','{26}','{27}','{28}','{29}','{30}','{31}','{32}')",
                      DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), data[0], data[79], data[1], data[2], data[3], data[4], data[5], data[6], data[7], data[9], data[10], data[11], data[13], data[14], data[15], data[16], data[17], data[18], data[19], data[20], data[21], data[22], data[80], data[81], data[82], data[83], data[84], data[85], data[86], data[87], data[88], "L_Bracket");
                    int r3 = SQL.ExecuteUpdate(insertStr3);
                    Log.WriteLog(string.Format("插入了{0}行{1}焊接参数", r3, data[79]));

                    string insertStr4 = string.Format("insert into HansData([DateTime],[SN],[Nut],[power_ll],[power_ul],[pattern_type],[spot_size],[hatch],[swing_amplitude],[swing_freq],[JudgeResult],[MeasureTime],[MachineSN],[PulseProfile_measure],[ActualPower],[Power_measure],[WaveForm_measure],[Frequency_measure],[LinearSpeed_measure],[QRelease_measure],[PulseEnergy_measure],[PeakPower_measure],[laser_sensor],[pulse_profile],[laser_power],[frequency],[waveform],[pulse_energy],[laser_speed],[jump_speed],[jump_delay],[scanner_delay],[Station]) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}','{20}','{21}','{22}','{23}','{24}','{25}','{26}','{27}','{28}','{29}','{30}','{31}','{32}')",
                      DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), data[0], data[107], data[1], data[2], data[3], data[4], data[5], data[6], data[7], data[9], data[10], data[11], data[13], data[14], data[15], data[16], data[17], data[18], data[19], data[20], data[21], data[22], data[108], data[109], data[110], data[111], data[112], data[113], data[114], data[115], data[116], "L_Bracket");
                    int r4 = SQL.ExecuteUpdate(insertStr4);
                    Log.WriteLog(string.Format("插入了{0}行{1}焊接参数", r4, data[116]));
                }
            }
            catch (Exception EX)
            {
                Log.WriteLog("大族参数格式异常" + EX.ToString());
                PLCHeart = false;
                if (MessageBox.Show("大族焊接参数格式异常，请检查DMS软件参数版本") == DialogResult.OK)
                {
                    PLCHeart = true;
                }
            }
        }
        void client2_PlaintextReceived(object sender, TcpDatagramReceivedEventArgs<string> e)
        {
            if (e.Datagram == "OK")
            {
                Global.PLC_Client.WritePLC_D(12621, new short[] { 1 });
                flag1 = true;
            }
            try
            {
                if (e.Datagram.Contains("FM"))
                {
                    Global.PLC_Client.WritePLC_D(12651, new short[] { 1 });
                    string[] data = e.Datagram.Replace("\r\n", "").Split(';');

                    ///20230228Update
                    int value;
                    if (int.TryParse(data[8], out value))
                    {
                        //if (value == 1)
                        //{
                        //    Task.Factory.StartNew(() =>
                        //    {
                        //        SendlaseripqcData(data);
                        //    });
                        //}
                    };
                    ///20230228update
                    string insertStr1 = string.Format("insert into HansData([DateTime],[SN],[Nut],[power_ll],[power_ul],[pattern_type],[spot_size],[hatch],[swing_amplitude],[swing_freq],[JudgeResult],[MeasureTime],[MachineSN],[PulseProfile_measure],[ActualPower],[Power_measure],[WaveForm_measure],[Frequency_measure],[LinearSpeed_measure],[QRelease_measure],[PulseEnergy_measure],[PeakPower_measure],[laser_sensor],[pulse_profile],[laser_power],[frequency],[waveform],[pulse_energy],[laser_speed],[jump_speed],[jump_delay],[scanner_delay],[Station]) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}','{20}','{21}','{22}','{23}','{24}','{25}','{26}','{27}','{28}','{29}','{30}','{31}','{32}')",
                          DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), data[0], data[23], data[1], data[2], data[3], data[4], data[5], data[6], data[7], data[9], data[10], data[11], data[13], data[14], data[15], data[16], data[17], data[18], data[19], data[20], data[21], data[22], data[24], data[25], data[26], data[27], data[28], data[29], data[30], data[31], data[32], "L_Bracket");
                    int r = SQL.ExecuteUpdate(insertStr1);
                    Log.WriteLog(string.Format("插入了{0}行{1}焊接参数", r, data[23]));
                    string insertStr2 = string.Format("insert into HansData([DateTime],[SN],[Nut],[power_ll],[power_ul],[pattern_type],[spot_size],[hatch],[swing_amplitude],[swing_freq],[JudgeResult],[MeasureTime],[MachineSN],[PulseProfile_measure],[ActualPower],[Power_measure],[WaveForm_measure],[Frequency_measure],[LinearSpeed_measure],[QRelease_measure],[PulseEnergy_measure],[PeakPower_measure],[laser_sensor],[pulse_profile],[laser_power],[frequency],[waveform],[pulse_energy],[laser_speed],[jump_speed],[jump_delay],[scanner_delay],[Station]) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}','{20}','{21}','{22}','{23}','{24}','{25}','{26}','{27}','{28}','{29}','{30}','{31}','{32}')",
                      DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), data[0], data[51], data[1], data[2], data[3], data[4], data[5], data[6], data[7], data[9], data[10], data[11], data[13], data[14], data[15], data[16], data[17], data[18], data[19], data[20], data[21], data[22], data[52], data[53], data[54], data[55], data[56], data[57], data[58], data[59], data[60], "L_Bracket");
                    int r2 = SQL.ExecuteUpdate(insertStr2);
                    Log.WriteLog(string.Format("插入了{0}行{1}焊接参数", r2, data[51]));
                    string insertStr3 = string.Format("insert into HansData([DateTime],[SN],[Nut],[power_ll],[power_ul],[pattern_type],[spot_size],[hatch],[swing_amplitude],[swing_freq],[JudgeResult],[MeasureTime],[MachineSN],[PulseProfile_measure],[ActualPower],[Power_measure],[WaveForm_measure],[Frequency_measure],[LinearSpeed_measure],[QRelease_measure],[PulseEnergy_measure],[PeakPower_measure],[laser_sensor],[pulse_profile],[laser_power],[frequency],[waveform],[pulse_energy],[laser_speed],[jump_speed],[jump_delay],[scanner_delay],[Station]) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}','{20}','{21}','{22}','{23}','{24}','{25}','{26}','{27}','{28}','{29}','{30}','{31}','{32}')",
                      DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), data[0], data[79], data[1], data[2], data[3], data[4], data[5], data[6], data[7], data[9], data[10], data[11], data[13], data[14], data[15], data[16], data[17], data[18], data[19], data[20], data[21], data[22], data[80], data[81], data[82], data[83], data[84], data[85], data[86], data[87], data[88], "L_Bracket");
                    int r3 = SQL.ExecuteUpdate(insertStr3);
                    Log.WriteLog(string.Format("插入了{0}行{1}焊接参数", r3, data[79]));

                    string insertStr4 = string.Format("insert into HansData([DateTime],[SN],[Nut],[power_ll],[power_ul],[pattern_type],[spot_size],[hatch],[swing_amplitude],[swing_freq],[JudgeResult],[MeasureTime],[MachineSN],[PulseProfile_measure],[ActualPower],[Power_measure],[WaveForm_measure],[Frequency_measure],[LinearSpeed_measure],[QRelease_measure],[PulseEnergy_measure],[PeakPower_measure],[laser_sensor],[pulse_profile],[laser_power],[frequency],[waveform],[pulse_energy],[laser_speed],[jump_speed],[jump_delay],[scanner_delay],[Station]) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}','{20}','{21}','{22}','{23}','{24}','{25}','{26}','{27}','{28}','{29}','{30}','{31}','{32}')",
                      DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), data[0], data[107], data[1], data[2], data[3], data[4], data[5], data[6], data[7], data[9], data[10], data[11], data[13], data[14], data[15], data[16], data[17], data[18], data[19], data[20], data[21], data[22], data[108], data[109], data[110], data[111], data[112], data[113], data[114], data[115], data[116], "L_Bracket");
                    int r4 = SQL.ExecuteUpdate(insertStr4);
                    Log.WriteLog(string.Format("插入了{0}行{1}焊接参数", r4, data[116]));
                }
            }
            catch (Exception EX)
            {
                Log.WriteLog("大族参数格式异常" + EX.ToString());
                PLCHeart = false;
                if (MessageBox.Show("大族焊接参数格式异常，请检查DMS软件参数版本") == DialogResult.OK)
                {
                    PLCHeart = true;
                }
            }
        }

        #endregion

        private bool ExistsMdiChildrenInstance(string mdiChildrenClassName)//检测子窗体是否存在
        {
            foreach (Form childForm in this.MdiChildren)
            {
                if (mdiChildrenClassName == childForm.Name)
                {
                    return true;
                }
            }
            return false;
        }

        private void ShowView()//窗体显示
        {
            _homefrm.MdiParent = this;
            _homefrm.Dock = DockStyle.Fill;
            _homefrm.Show();
        }

        private void SetText(int index)
        {
            if (index <= Global.ErrorMsg.Length)
            {
                tsslabelcolor(tsslbl_ErrorMeg, Color.Red, Global.ErrorMsg.Substring(Global.ErrorIndex, Global.ErrorMsg.Length - Global.ErrorIndex));
            }
            else
            {
                Global.ErrorIndex = -1;
            }
        }

        private void Errortime()
        {
            while (true)
            {
                while (Global.errordisplay)
                {
                    double a = timenum / 10;
                    _manualfrm.UpDatalabel(a.ToString("0.00"), "lb_WaitingTime");
                    timenum++;
                    Thread.Sleep(6000);
                }
                Thread.Sleep(3);
            }
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
        public string GetTraceIp()//获取本机Trace_IP
        {
            //string hostName = Dns.GetHostName();   //获取本机名
            //IPHostEntry localhost = Dns.GetHostByName(hostName);    //方法已过期，可以获取IPv4的地址
            ////IPHostEntry localhost = Dns.GetHostEntry(hostName);   //获取IPv6地址
            //IPAddress localaddr = localhost.AddressList[0];
            //return localaddr.ToString();
            NetworkInterface[] interfaces = System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces();
            int len = interfaces.Length;
            string mip = "";
            for (int i = 0; i < len; i++)
            {
                NetworkInterface ni = interfaces[i];
                if (ni.Name == "0Trace")
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

        public void SendPicture(string filename, string sn, int station)//发送PDCA图片
        {
            try
            {
                string SendPicPath = string.Format(@"D:\SendPicture\{0}", sn);
                string[] files1 = null;
                int i = 0;
                bool file2 = false;
                try
                {
                    files1 = Directory.GetFiles(string.Format(@Global.inidata.productconfig.PICPath + "\\{0}", sn), "*.jpg", SearchOption.AllDirectories);
                }
                catch (Exception ex)
                {
                    Log.WriteLog(ex.ToString());
                    file2 = true;
                    Log.WriteLog(string.Format("本机不存在或缺少{0}文件夹", sn) + ",PDCALog");
                }
                if (files1.Length >= 1)
                {
                    foreach (string path in files1)
                    {
                        i++;
                        if (!Directory.Exists(filename))
                        {
                            Directory.CreateDirectory(filename);
                        }
                        if (!Directory.Exists(SendPicPath))
                        {
                            Directory.CreateDirectory(SendPicPath);
                        }
                        using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
                        {
                            //将图片以文件流的形式进行保存
                            BinaryReader br = new BinaryReader(fs);
                            byte[] imgBytesIn = br.ReadBytes((int)fs.Length); //将流读入到字节数组中
                            using (MemoryStream ms = new MemoryStream(imgBytesIn))
                            {
                                Image image = System.Drawing.Image.FromStream(ms);
                                Bitmap bmp = new Bitmap(image);
                                bmp.Save(filename + "\\" + i + ".jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
                                bmp.Save(SendPicPath + "\\" + i + ".jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
                            }
                        }
                    }
                    Log.WriteLog(string.Format("已成功发送{0}图片", sn) + ",PDCALog");
                    Log.WriteCSV(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + "U-Bracket" + "," + sn, System.AppDomain.CurrentDomain.BaseDirectory + "\\PDCA图片上传成功数据\\");
                    _homefrm.AppendRichText(string.Format("已成功发送{0}图片", sn), "rtx_PDCAMsg");
                    if (station == 1)
                    {
                        Global.PLC_Client2.WritePLC_D(10303, new short[] { 1 });//图片上传成功给plc置1
                    }
                    else
                    {
                        Global.PLC_Client2.WritePLC_D(10303, new short[] { 1 });//图片上传成功给plc置1
                    }
                }
                else
                {
                    Log.WriteLog(string.Format("{0}图片数量不符，有{1}张", sn, files1.Length) + ",PDCALog");
                    Log.WriteCSV(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + "U-Bracket" + "," + sn, System.AppDomain.CurrentDomain.BaseDirectory + "\\PDCA图片上传异常数据\\");
                    _homefrm.AppendRichText(string.Format("{0}图片数量不符，有{1}张", sn, files1.Length), "rtx_PDCAMsg");

                    if (station == 1)
                    {
                        Global.PLC_Client2.WritePLC_D(10303, new short[] { 2 });//图片上传失败给plc置2
                    }
                    else
                    {
                        Global.PLC_Client2.WritePLC_D(10303, new short[] { 2 });//图片上传失败给plc置2
                    }
                }
            }
            catch (Exception ex)
            {
                //PDCA_Pic_NG++;
                //labelenvision(LB_Pic_ng, PDCA_Pic_NG.ToString());
                //Log.WriteLog("PDCA图片上传失败数量：" + PDCA_Pic_NG + "Pcs");
                Log.WriteLog(ex.ToString().Replace("\n", "") + ",PDCALog");
                Log.WriteLog(string.Format("发送SN:{0}图片给MAC mini异常失败！", sn) + ",PDCALog");
                Log.WriteCSV(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + "U-Bracket" + "," + sn, System.AppDomain.CurrentDomain.BaseDirectory + "\\PDCA图片上传异常数据\\");
                _homefrm.AppendRichText(string.Format("发送SN:{0}图片给MAC mini异常失败！", sn), "rtx_PDCAMsg");

                if (station == 1)
                {
                    Global.PLC_Client2.WritePLC_D(10303, new short[] { 2 });//图片上传失败给plc置2
                }
                else
                {
                    Global.PLC_Client2.WritePLC_D(10303, new short[] { 2 });//图片上传失败给plc置2
                }
            }
        }

        public void SendCSVFile(string toFilePath, string sn)
        {
            try
            {
                string fromFilePath = string.Format(@"O:\{0}", sn) + ".csv";
                if (File.Exists(fromFilePath))
                {
                    if (File.Exists(toFilePath))
                    {
                        File.Delete(toFilePath);
                    }
                    File.Copy(fromFilePath, toFilePath + ".csv");
                    Log.WriteLog(sn + "普雷斯特CSV上传Macmini成功");
                }
                else
                {
                    Log.WriteLog(string.Format("本机不存在普雷斯特{0}CSV文件", sn));
                }
            }
            catch (Exception ex)
            {
                Log.WriteLog("发送普雷斯特CSV文档异常！" + ex.ToString().Replace("\n", ""));
            }
        }

        private void Ping_ip(object ob) //PLC、MAC mini连线检测
        {
            while (true)
            {
                try
                {
                    using (System.Net.NetworkInformation.Ping PingSender = new System.Net.NetworkInformation.Ping())
                    {
                        PingOptions Options = new PingOptions();
                        Options.DontFragment = true;
                        string Data = "test";
                        byte[] DataBuffer = Encoding.ASCII.GetBytes(Data);
                        PingReply Reply = PingSender.Send(Global.inidata.productconfig.Plc_IP, 1000, DataBuffer, Options);
                        if (Reply.Status == IPStatus.Success)
                        {
                            Link_PLC = true;
                        }
                        else
                        {
                            Link_PLC = false;
                        }
                    }
                }
                catch
                {
                    Log.WriteLog("Ping PLC IP ERROR!!!");
                }

                try
                {
                    using (System.Net.NetworkInformation.Ping PingSender = new System.Net.NetworkInformation.Ping())
                    {
                        PingOptions Options = new PingOptions();
                        Options.DontFragment = true;
                        string Data = "test";
                        byte[] DataBuffer = Encoding.ASCII.GetBytes(Data);
                        PingReply Reply = PingSender.Send(Global.inidata.productconfig.PDCA_UA_IP, 1000, DataBuffer, Options);
                        if (Reply.Status == IPStatus.Success)
                        {
                            Link_Mac_Mini_Server = true;
                        }
                        else
                        {
                            Link_Mac_Mini_Server = false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.WriteLog("Ping Mac Mini server IP ERROR!!!");
                    Link_Mac_Mini_Server = false;
                }
                Thread.Sleep(5000);
            }
        }

        private void On_Time_doing(object ob)//按时做某事
        {
            while (true)
            {
                try
                {
                    int TotalCount = 0;//UI界面显示
                    short NGCount = 0;//UI界面显示          
                    short Product_Lianglv = 0;//UI界面显示                                  
                    short Product_Paoliao = 0;//抛料数 
                    short Paoliaolv = 0;//抛料率                                           
                    short ActualCT = 0;
                    ActualCT = Global.PLC_Client.ReadPLC_D(2900, 1)[0];//CT时间
                    TotalCount = Global.PLC_Client.ReadPLC_DD(2902, 2)[0];//总产量
                    NGCount = Global.PLC_Client.ReadPLC_D(2904, 1)[0];//NG产量
                    Product_Lianglv = Global.PLC_Client.ReadPLC_D(2906, 1)[0];//良率
                    Product_Paoliao = Global.PLC_Client.ReadPLC_D(2920, 1)[0];//抛料数
                    Paoliaolv = Global.PLC_Client.ReadPLC_D(2922, 1)[0];//抛料率              

                    double ct = Convert.ToDouble(ActualCT);
                    string CT = (ct / 10).ToString("0.0");
                    double lianglv = 0;
                    double paoliaolv = 0;
                    if (TotalCount == 0)
                    {
                        lianglv = 0;
                    }
                    else
                    {
                        lianglv = (double)(TotalCount - NGCount) / TotalCount * 100;
                    }
                    if (Product_Paoliao == 0)
                    {
                        paoliaolv = 0;
                    }
                    else
                    {
                        paoliaolv = (double)(Product_Paoliao) / TotalCount * 100;
                    }
                    _homefrm.UiText((Convert.ToDouble(ActualCT) / 10).ToString("0.0"), "txt_CT");
                    _homefrm.UiText(TotalCount.ToString(), "txt_Count");
                    _homefrm.UiText(NGCount.ToString(), "txt_NGcount");
                    _homefrm.UiText((Convert.ToDouble(Product_Lianglv) / 100).ToString("0.00") + "%", "txt_lianglv");
                    _homefrm.UiText(Product_Paoliao.ToString(), "txt_paoliao");
                    _homefrm.UiText((Convert.ToDouble(Paoliaolv) / 100).ToString("0.00") + "%", "txt_paoliaolv");
                    _homefrm.UpDatalabel(CT, "CT");
                    _homefrm.UpDatalabel(TotalCount.ToString(), "TotalCount");
                    _homefrm.UpDatalabel(NGCount.ToString(), "NGCount");
                    _homefrm.UpDatalabel(lianglv.ToString("0.00") + "%", "Lianglv");
                    _homefrm.UpDatalabel(Product_Paoliao.ToString(), "Paoliaoshu");
                    _homefrm.UpDatalabel(paoliaolv.ToString("0.00") + "%", "Paoliaolv");

                    if (Global.inidata.productconfig.TraceCheckParam_Online == "1" && DateTime.Now.Hour == 5 && DateTime.Now.Minute == 30 && DateTime.Now.Second == 0)
                    {
                        var IP = GetTraceIp();
                        string Msg = string.Empty;
                        bool TraceVerify = true;
                        bool OEEVerify = true;
                        Global.b_VerifyResult = false;
                        try
                        {
                            string CallResult = "";
                            string errMsg = "";
                            JsonSerializerSettings jsetting = new JsonSerializerSettings();
                            jsetting.NullValueHandling = NullValueHandling.Ignore;//Json不输出空值
                            CheckOEEMachine CheckMachine = new CheckOEEMachine();
                            CheckMachine.SiteName = Global.inidata.productconfig.SiteName;
                            CheckMachine.EMT = Global.inidata.productconfig.EMT;
                            CheckMachine.TraceIP = GetTraceIp();
                            CheckMachine.TraceStationID = Global.inidata.productconfig.Station_id_ua;
                            CheckMachine.MacAddress = GetOEEMac();
                            string SendTraceMachine = JsonConvert.SerializeObject(CheckMachine, Formatting.None, jsetting);
                            Log.WriteLog("OEE校验参数数据:" + SendTraceMachine);
                            var result = RequestAPI2.CallBobcat2(Global.inidata.productconfig.OEECheckParamURL, SendTraceMachine, out CallResult, out errMsg, false);
                            CheckOEEMachineRespond Respond = JsonConvert.DeserializeObject<CheckOEEMachineRespond>(CallResult);
                            Log.WriteLog("OEE校验参数结果: " + CallResult + errMsg);
                            try
                            {
                                if (Respond.Result == "Success")//&& Respond_UA.Contains("Pass")
                                {
                                    ShowStatus("Trace参数状态", Color.DarkSeaGreen, 7);
                                    Log.WriteLog("OEE校验参数结果:" + JsonConvert.SerializeObject(CallResult));
                                    //初步比对成功后,用Mac地址获取后台有无更新数据有更新就更新本地setting对应数据
                                    string Mac = string.Format("{{\"MacAddress\":\"{0}\"}}", CheckMachine.MacAddress);
                                    result = RequestAPI2.CallBobcat2(Global.inidata.productconfig.MacQueryUrl, Mac, out CallResult, out errMsg, false);
                                    Log.WriteLog("通过Mac获取OEE参数:" + JsonConvert.SerializeObject(CallResult) + errMsg);
                                    MacQueryresult TraceResult = JsonConvert.DeserializeObject<MacQueryresult>(CallResult);
                                    if (!TraceResult.EMT.IsMatch && TraceResult.EMT.FromJMCC != null && TraceResult.EMT.FromJMCC != "")
                                    {
                                        Global.inidata.productconfig.EMT = TraceResult.EMT.FromJMCC;
                                    }
                                    if (!TraceResult.TraceStationID.IsMatch && TraceResult.TraceStationID.FromJMCC != null && TraceResult.TraceStationID.FromJMCC != "")
                                    {
                                        Global.inidata.productconfig.Station_id_ua = TraceResult.TraceStationID.FromJMCC;
                                    }
                                    if (!TraceResult.SiteName.IsMatch && TraceResult.SiteName.FromJMCC != null && TraceResult.SiteName.FromJMCC != "")
                                    {
                                        Global.inidata.productconfig.SiteName = TraceResult.SiteName.FromJMCC;
                                    }
                                    if (!TraceResult.EMT.IsMatch || !TraceResult.TraceStationID.IsMatch || !TraceResult.SiteName.IsMatch)
                                    {
                                        Global.inidata.WriteProductConfigSection();
                                        Global.inidata.ReadProductConfigSection();
                                    }
                                }
                                else
                                {
                                    ShowStatus("Trace参数状态", Color.Red, 7);
                                    OEEVerify = false;
                                    MessageBox.Show(errMsg + Respond.Message, "校验Trace配置参数失败", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    Log.WriteLog("OEE校验参数:" + errMsg + JsonConvert.SerializeObject(CallResult));
                                }
                            }
                            catch
                            {
                                ShowStatus("Trace参数状态", Color.Red, 7);
                                OEEVerify = false;
                                MessageBox.Show(errMsg + JsonConvert.SerializeObject(CallResult), "校验Trace配置参数失败", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        }
                        catch (Exception EX)
                        {
                            OEEVerify = false;
                            Log.WriteLog("OEE参数校验异常:" + EX.ToString());

                        }
                        string Respond_UA = RequestAPI3.HttpPostWebService(Global.inidata.productconfig.TraceCheckParamURL54, IP, Global.inidata.productconfig.Process_UA, Global.inidata.productconfig.Line_id_ua, Global.inidata.productconfig.Station_id_ua, Global.inidata.productconfig.Sw_name_ua, Global.inidata.productconfig.Version, out Msg);
                        Log.WriteLog("Trace校验参数数据:" + IP + Global.inidata.productconfig.Process_UA + Global.inidata.productconfig.Line_id_ua + Global.inidata.productconfig.Station_id_ua + Global.inidata.productconfig.Sw_name_ua + Global.inidata.productconfig.Version);
                        if (!Respond_UA.Contains("Pass"))
                        {
                            TraceVerify = false;
                            ShowStatus("Trace参数状态", Color.Red, 7);
                            Log.WriteLog("Trace参数校验异常: " + Respond_UA + Msg.ToString());
                        }
                        //Tab 站点校验
                        string Respond_LA = RequestAPI3.HttpPostWebService(Global.inidata.productconfig.TraceCheckParamURL54, IP, Global.inidata.productconfig.Process_LA, Global.inidata.productconfig.Line_id_la, Global.inidata.productconfig.Station_id_la, Global.inidata.productconfig.Sw_name_la, Global.inidata.productconfig.Version, out Msg);
                        Log.WriteLog("Trace_Tab校验参数数据:" + IP + Global.inidata.productconfig.Process_LA + Global.inidata.productconfig.Line_id_la + Global.inidata.productconfig.Station_id_la + Global.inidata.productconfig.Sw_name_la + Global.inidata.productconfig.Version);
                        if (!Respond_LA.Contains("Pass"))
                        {
                            TraceVerify = false;
                            ShowStatus("Trace参数状态", Color.Red, 7);
                            Log.WriteLog("Trace参数校验异常: " + Respond_UA + Msg.ToString());
                        }
                        if (!TraceVerify)
                        {
                            Global.PLC_Client.WritePLC_D(10110, new short[] { 2 });
                        }
                        else if (!OEEVerify)
                        {
                            Global.PLC_Client.WritePLC_D(10110, new short[] { 2 });
                        }
                        else if (OEEVerify && TraceVerify)
                        {
                            Global.b_VerifyResult = true;
                            Global.PLC_Client.WritePLC_D(10110, new short[] { 1 });
                        }
                    }
                    if ((DateTime.Now.Hour == 6 || DateTime.Now.Hour == 18) && DateTime.Now.Minute == 00 && DateTime.Now.Second == 0)
                    {

                        Log.WriteCSV_NUM(DateTime.Now.ToString("yyyy-MM-dd") + "," + Global.Trace_ua_ok.ToString() + "," + Global.Trace_ua_ng.ToString());
                        Log.WriteCSV_PDCA(DateTime.Now.ToString("yyyy-MM-dd") + "," + (Global.Trace_ua_ok + Global.Trace_ua_ng).ToString());
                        _homefrm.UpDatalabel("0", "lb_TraceUAOK");
                        _homefrm.UpDatalabel("0", "lb_TraceUANG");
                        _homefrm.UpDatalabel("0", "lb_TraceLAOK");
                        _homefrm.UpDatalabel("0", "lb_TraceLANG");
                        _homefrm.UpDatalabel("0", "lb_PDCAUAOK");
                        _homefrm.UpDatalabel("0", "lb_PDCAUANG");
                        _homefrm.UpDatalabel("0", "lb_PDCALAOK");
                        _homefrm.UpDatalabel("0", "lb_PDCALANG");
                        _homefrm.UpDatalabel("0", "lb_OEEOK");
                        _homefrm.UpDatalabel("0", "lb_OEENG");
                        _homefrm.UpDatalabel("0", "lb_FixtureOK");
                        _homefrm.UpDatalabel("0", "lb_FixtureNG");
                        _homefrm.UpDatalabel("0", "lb_ProcessControlOK");
                        _homefrm.UpDatalabel("0", "lb_ProcessControlNG");
                        _homefrm.AppendRichText("N/A", "rtx_TraceMsg");
                        _homefrm.AppendRichText("N/A", "rtx_PDCAMsg");
                        _homefrm.AppendRichText("N/A", "rtx_OEEDefaultMsg");
                        _homefrm.AppendRichText("N/A", "rtx_HeartBeatMsg");
                        _homefrm.AppendRichText("N/A", "rtx_DownTimeMsg");
                        _homefrm.AppendRichText("N/A", "rtx_OEEMateriel");
                        _homefrm.AppendRichText("N/A", "rtx_ProcessControl");
                        _homefrm.AddList("N/A", "list_FixtureMsg");
                        _homefrm.AddList("N/A", "list_IQCFixture");
                        _homefrm.AddList("N/A", "list_FixtureMsgNG");
                        _homefrm.AddList("N/A", "list_IQCFixtureNG");
                        _userloginfrm.AddList("N/A", "list_UploadLogin");
                        _userloginfrm.AddList("N/A", "list_UserLogin");
                        _userloginfrm.AddList("N/A", "list_FixtureNG");

                        if (DateTime.Now.Hour == 6)//每天6点执行
                        {
                            //清空异常抛料统计数据
                            Global.Smallmaterial_Input_D = 0;
                            Global.Smallmaterial_Input_N = 0;
                            Global.TracePVCheck_Error_D = 0;
                            Global.TracePVCheck_Error_N = 0;
                            Global.ReadBarcode_NG_D = 0;
                            Global.ReadBarcode_NG_N = 0;
                            Global.Product_Total_D = 0;
                            Global.Product_Total_N = 0;
                            Global.TraceUpLoad_Error_D = 0;
                            Global.TraceUpLoad_Error_N = 0;
                            Global.PDCAUpLoad_Error_D = 0;
                            Global.PDCAUpLoad_Error_N = 0;
                            Global.CCDCheck_Error_D = 0;
                            Global.CCDCheck_Error_N = 0;
                            Global.location1_CCDNG_D = 0;
                            Global.location1_CCDNG_N = 0;
                            Global.HansDataError_D = 0;
                            Global.HansDataError_N = 0;
                            //Global.Welding_Error_D = 0;
                            //Global.Welding_Error_N = 0;
                            Global.Smallmaterial_throwing_D = 0;
                            Global.Smallmaterial_throwing_N = 0;
                            Global.TraceTab_Error_D = 0;
                            Global.TraceTab_Error_N = 0;
                            Global.TraceThench_Error_D = 0;
                            Global.TraceThench_Error_N = 0;

                            Global.inidata.productconfig.Product_Total_D = "0";
                            Global.inidata.productconfig.Product_Total_N = "0";
                            Global.inidata.productconfig.Smallmaterial_Input_D = "0";
                            Global.inidata.productconfig.Smallmaterial_Input_N = "0";
                            Global.inidata.productconfig.TraceUpLoad_Error_D = "0";
                            Global.inidata.productconfig.TraceUpLoad_Error_N = "0";
                            Global.inidata.productconfig.PDCAUpLoad_Error_D = "0";
                            Global.inidata.productconfig.PDCAUpLoad_Error_N = "0";
                            Global.inidata.productconfig.TracePVCheck_Error_D = "0";
                            Global.inidata.productconfig.TracePVCheck_Error_N = "0";
                            Global.inidata.productconfig.ReadBarcode_NG_D = "0";
                            Global.inidata.productconfig.ReadBarcode_NG_N = "0";
                            Global.inidata.productconfig.CCDCheck_Error_D = "0";
                            Global.inidata.productconfig.CCDCheck_Error_N = "0";
                            Global.inidata.productconfig.location1_CCDNG_D = "0";
                            Global.inidata.productconfig.location1_CCDNG_N = "0";
                            Global.inidata.productconfig.location2_CCDNG_D = "0";
                            Global.inidata.productconfig.location2_CCDNG_N = "0";
                            Global.inidata.productconfig.location3_CCDNG_D = "0";
                            Global.inidata.productconfig.location3_CCDNG_N = "0";
                            Global.inidata.productconfig.HansDataError_D = "0";
                            Global.inidata.productconfig.HansDataError_N = "0";
                            Global.inidata.productconfig.Smallmaterial_throwing_D = "0";
                            Global.inidata.productconfig.Smallmaterial_throwing_N = "0";
                            Global.inidata.productconfig.Band_NG_D = "0";
                            Global.inidata.productconfig.Band_NG_N = "0";
                            Global.inidata.WriteProductnumSection();
                            Log.WriteLog("定时清空异常抛料统计数据");
                            //抛料数据清除后，与oee过站数据相关的过渡数据重新赋值
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
                            Global.itm_location1_CCDNG_D = Global.inidata.productconfig.location1_CCDNG_D;
                            Global.itm_location1_CCDNG_N = Global.inidata.productconfig.location1_CCDNG_N;
                            Global.itm_location2_CCDNG_D = Global.inidata.productconfig.location2_CCDNG_D;
                            Global.itm_location2_CCDNG_N = Global.inidata.productconfig.location2_CCDNG_N;
                            Global.itm_location3_CCDNG_D = Global.inidata.productconfig.location3_CCDNG_D;
                            Global.itm_location3_CCDNG_N = Global.inidata.productconfig.location3_CCDNG_N;
                            Global.itm_HansDataError_D = Global.inidata.productconfig.HansDataError_D;
                            Global.itm_HansDataError_N = Global.inidata.productconfig.HansDataError_N;
                            Global.itm_Band_NG_D = Global.inidata.productconfig.Band_NG_D;
                            Global.itm_Band_NG_N = Global.inidata.productconfig.Band_NG_N;

                            Global.itm_TraceTab_Error_D = Global.inidata.productconfig.TraceTab_Error_D;
                            Global.itm_TraceTab_Error_N = Global.inidata.productconfig.TraceTab_Error_N;
                            Global.itm_TraceThench_Error_D = Global.inidata.productconfig.TraceThench_Error_D;
                            Global.itm_TraceThench_Error_N = Global.inidata.productconfig.TraceThench_Error_N;
                            Global.inidata.productconfig.CCD_Throwing_D = "0";
                            Global.inidata.productconfig.CCD_Throwing_N = "0";
                            Global.inidata.productconfig.Welding_Throwing_D = "0";
                            Global.inidata.productconfig.Welding_Throwing_N = "0";
                            Global.inidata.ReadProductConfigSection();
                            _homefrm.AppendRichTextClear("", "rtx_FixtureMaintainMsg");
                        }
                        Thread.Sleep(1000);
                    }
                    if ((DateTime.Now.Hour == 8 || DateTime.Now.Hour == 20) && DateTime.Now.Minute == 30 && DateTime.Now.Second == 0)//定时读取超时治具编号
                    {
                        _homefrm.AddList("N/A", "list_FixtureMsg");
                        string strMes = "";
                        string[] PISMsg = new string[] { };
                        string URL = Global.inidata.productconfig.PIS_URL.Replace("Project", "Project=" + Global.project).Replace("Station", "Station=" + Global.station).Replace("Type", "Type=" + Global.type);
                        var PIS_Result = RequestAPI2.PIS_System(URL, out strMes, out PISMsg);
                        if (PIS_Result)
                        {
                            Txt.WriteLine(PISMsg);
                            for (int J = 0; J < PISMsg.Length; J++)
                            {
                                if (PISMsg[J] != null)
                                {
                                    _homefrm.AddList(PISMsg[J], "list_FixtureMsg");
                                    Log.WriteLog("定时读取逾期保养治具编号：" + strMes + PISMsg[J]);
                                }
                            }
                        }
                        Thread.Sleep(1000);
                    }

                    if (DateTime.Now.Minute == 0 && DateTime.Now.Second == 0)//定时读取IQC治具
                    {
                        try
                        {
                            _homefrm.AddList("N/A", "list_IQCFixture");
                            string strMes = "";
                            JsonSerializerSettings jsetting = new JsonSerializerSettings();
                            jsetting.NullValueHandling = NullValueHandling.Ignore;//Json不输出空值
                            IQCSystemDATA IQCData = new IQCSystemDATA();
                            IQCData.Plant_Organization_Name = "CTU";
                            IQCData.BG_Organization_Name = "OP2";
                            IQCData.FunPlant_Organization_Name = "组装";
                            IQCData.Project_Name = "Burbank";
                            IQCData.WorkStation_Name = "Trench Bracket";
                            string SendIQCData = JsonConvert.SerializeObject(IQCData, Formatting.None, jsetting);

                            string[] PISMsg = new string[] { };
                            string URL = Global.inidata.productconfig.IQC_URL;
                            var IQCResult = RequestAPI2.IQC_System(URL, SendIQCData, out strMes, out PISMsg);
                            if (IQCResult)
                            {
                                Txt.WriteLine1(PISMsg);
                                for (int J = 0; J < PISMsg.Length; J++)
                                {
                                    if (PISMsg[J] != null && !PISMsg[J].Contains("参数错误"))
                                    {
                                        _homefrm.AddList(PISMsg[J], "list_IQCFixture");
                                        Log.WriteLog("定时读取IQC治具编号：" + strMes + PISMsg[J]);
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.WriteLog("IQC接口异常" + ex.ToString());
                        }
                        Thread.Sleep(1000);
                    }

                    //写入夜班产能与小料抛料数据
                    if (DateTime.Now.Hour == 9 && DateTime.Now.Minute == 30 && DateTime.Now.Second == 0 && Global.Product_status)
                    {
                        string InsertStr1 = string.Format("UPDATE [Product] SET [Hour_13] = '{0}',[Hour_14] = '{1}',[Hour_15] = '{2}',[Hour_16] = '3',[Hour_17] = '{4}',[Hour_18]= '{5}',[Hour_19]= '{6}'"
                                    + " ,[Hour_20]= '{7}',[Hour_21]= '{8}',[Hour_22] = '{9}',[Hour_23] = '{10}',[Hour_24] = '{11}' where [DateTime] = '{12}' and [Status] = '{13}'",
                                     Global.Product_OK_N_1[0], Global.Product_OK_N_1[1], Global.Product_OK_N_1[2], Global.Product_OK_N_1[3], Global.Product_OK_N_1[4], Global.Product_OK_N_1[5], Global.Product_OK_N_2[0],
                                     Global.Product_OK_N_2[1], Global.Product_OK_N_2[2], Global.Product_OK_N_2[3], Global.Product_OK_N_2[4], Global.Product_OK_N_2[5], DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd"), "OK产量");
                        SQL.ExecuteUpdate(InsertStr1);
                        Log.WriteLog("写入夜班OK产能数据" + InsertStr1);

                        string InsertStr2 = string.Format("UPDATE [Product] SET [Hour_13] = '{0}',[Hour_14] = '{1}',[Hour_15] = '{2}',[Hour_16] = '3',[Hour_17] = '{4}',[Hour_18]= '{5}',[Hour_19]= '{6}'"
                                    + " ,[Hour_20]= '{7}',[Hour_21]= '{8}',[Hour_22] = '{9}',[Hour_23] = '{10}',[Hour_24] = '{11}' where [DateTime] = '{12}' and [Status] = '{13}'",
                                     Global.Product_NG_N_1[0], Global.Product_NG_N_1[1], Global.Product_NG_N_1[2], Global.Product_NG_N_1[3], Global.Product_NG_N_1[4], Global.Product_NG_N_1[5], Global.Product_NG_N_2[0],
                                     Global.Product_NG_N_2[1], Global.Product_NG_N_2[2], Global.Product_NG_N_2[3], Global.Product_NG_N_2[4], Global.Product_NG_N_2[5], DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd"), "NG产量");
                        SQL.ExecuteUpdate(InsertStr2);
                        Log.WriteLog("写入夜班NG产能数据" + InsertStr2);

                        string InsertStr3 = string.Format("UPDATE [HourDT] SET [Hour_DT13] = '{0}',[Hour_DT14] = '{1}',[Hour_DT15] = '{2}',[Hour_DT16] = '3',[Hour_DT17] = '{4}',[Hour_DT18]= '{5}',[Hour_DT19]= '{6}'"
                                    + " ,[Hour_DT20]= '{7}',[Hour_DT21]= '{8}',[Hour_DT22] = '{9}',[Hour_DT23] = '{10}',[Hour_DT24] = '{11}' where [DateTime] = '{12}' and [Status] = '{13}'",
                                     (Convert.ToDouble(Global.DT_RunTime_N1[0])).ToString("0.00"), (Convert.ToDouble(Global.DT_RunTime_N1[1])).ToString("0.00"), (Convert.ToDouble(Global.DT_RunTime_N1[2])).ToString("0.00"),
                                     (Convert.ToDouble(Global.DT_RunTime_N1[3])).ToString("0.00"), (Convert.ToDouble(Global.DT_RunTime_N1[4])).ToString("0.00"), (Convert.ToDouble(Global.DT_RunTime_N1[5])).ToString("0.00"),
                                     (Convert.ToDouble(Global.DT_RunTime_N2[0])).ToString("0.00"), (Convert.ToDouble(Global.DT_RunTime_N2[1])).ToString("0.00"), (Convert.ToDouble(Global.DT_RunTime_N2[2])).ToString("0.00"),
                                     (Convert.ToDouble(Global.DT_RunTime_N2[3])).ToString("0.00"), (Convert.ToDouble(Global.DT_RunTime_N2[4])).ToString("0.00"), (Convert.ToDouble(Global.DT_RunTime_N2[5])).ToString("0.00"),
                                     DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd"), "运行时间");
                        SQL.ExecuteUpdate(InsertStr3);
                        Log.WriteLog("写入夜班运行时间" + InsertStr3);

                        string InsertStr4 = string.Format("UPDATE [HourDT] SET [Hour_DT13] = '{0}',[Hour_DT14] = '{1}',[Hour_DT15] = '{2}',[Hour_DT16] = '3',[Hour_DT17] = '{4}',[Hour_DT18]= '{5}',[Hour_DT19]= '{6}'"
                                    + " ,[Hour_DT20]= '{7}',[Hour_DT21]= '{8}',[Hour_DT22] = '{9}',[Hour_DT23] = '{10}',[Hour_DT24] = '{11}' where [DateTime] = '{12}' and [Status] = '{13}'",
                                     (Convert.ToDouble(Global.DT_ErrorTime_N1[0])).ToString("0.00"), (Convert.ToDouble(Global.DT_ErrorTime_N1[1])).ToString("0.00"), (Convert.ToDouble(Global.DT_ErrorTime_N1[2])).ToString("0.00"),
                                     (Convert.ToDouble(Global.DT_ErrorTime_N1[3])).ToString("0.00"), (Convert.ToDouble(Global.DT_ErrorTime_N1[4])).ToString("0.00"), (Convert.ToDouble(Global.DT_ErrorTime_N1[5])).ToString("0.00"),
                                     (Convert.ToDouble(Global.DT_ErrorTime_N2[0])).ToString("0.00"), (Convert.ToDouble(Global.DT_ErrorTime_N2[1])).ToString("0.00"), (Convert.ToDouble(Global.DT_ErrorTime_N2[2])).ToString("0.00"),
                                     (Convert.ToDouble(Global.DT_ErrorTime_N2[3])).ToString("0.00"), (Convert.ToDouble(Global.DT_ErrorTime_N2[4])).ToString("0.00"), (Convert.ToDouble(Global.DT_ErrorTime_N2[5])).ToString("0.00"),
                                     DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd"), "异常时间");
                        SQL.ExecuteUpdate(InsertStr4);
                        Log.WriteLog("写入夜班异常时间" + InsertStr4);

                        string InsertStr5 = string.Format("UPDATE [HourDT] SET [Hour_DT13] = '{0}',[Hour_DT14] = '{1}',[Hour_DT15] = '{2}',[Hour_DT16] = '3',[Hour_DT17] = '{4}',[Hour_DT18]= '{5}',[Hour_DT19]= '{6}'"
                                    + " ,[Hour_DT20]= '{7}',[Hour_DT21]= '{8}',[Hour_DT22] = '{9}',[Hour_DT23] = '{10}',[Hour_DT24] = '{11}' where [DateTime] = '{12}' and [Status] = '{13}'",
                                     (Convert.ToDouble(Global.DT_PendingTime_N1[0])).ToString("0.00"), (Convert.ToDouble(Global.DT_PendingTime_N1[1])).ToString("0.00"), (Convert.ToDouble(Global.DT_PendingTime_N1[2])).ToString("0.00"),
                                     (Convert.ToDouble(Global.DT_PendingTime_N1[3])).ToString("0.00"), (Convert.ToDouble(Global.DT_PendingTime_N1[4])).ToString("0.00"), (Convert.ToDouble(Global.DT_PendingTime_N1[5])).ToString("0.00"),
                                     (Convert.ToDouble(Global.DT_PendingTime_N2[0])).ToString("0.00"), (Convert.ToDouble(Global.DT_PendingTime_N2[1])).ToString("0.00"), (Convert.ToDouble(Global.DT_PendingTime_N2[2])).ToString("0.00"),
                                     (Convert.ToDouble(Global.DT_PendingTime_N2[3])).ToString("0.00"), (Convert.ToDouble(Global.DT_PendingTime_N2[4])).ToString("0.00"), (Convert.ToDouble(Global.DT_PendingTime_N2[5])).ToString("0.00"),
                                     DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd"), "待料时间");
                        SQL.ExecuteUpdate(InsertStr5);
                        Log.WriteLog("写入夜班待料时间" + InsertStr5);

                        string InsertStr6 = string.Format("UPDATE [ErrorDataStatistics] SET [Product_Total_N] = '{0}',[Smallmaterial_Input_N] = '{1}',[Location1_NG_N] = '{2}',[Location2_NG_N] = '{3}',[Location3_NG_N] = '{4}',"
                            + "[Location4_NG_N] = '{5}',[Location5_NG_N] = '{6}',[HansDataError_N] = '{7}',[TraceUpLoad_Error_N] = '{8}',[PDCAUpLoad_Error_N] = '{9}',[TracePVCheck_Error_N] = '{10}',[ReadBarcode_NG_N] = '{11}',"
                            + "[CCDCheck_Error_N] = '{12}',[Smallmaterial_throwing_N] = '{13}',[Band_NG_N]='{14}' where DateTime = '{15}'",
                            Global.Product_Total_N.ToString(), Global.Smallmaterial_Input_N.ToString(), Global.location1_CCDNG_N.ToString(), Global.location2_CCDNG_N.ToString(), Global.location3_CCDNG_N.ToString(), Global.TraceTab_Error_N.ToString(),
                            Global.TraceThench_Error_N.ToString(), Global.HansDataError_N.ToString(), Global.TraceUpLoad_Error_N.ToString(), Global.PDCAUpLoad_Error_N.ToString(), Global.TracePVCheck_Error_N.ToString(), Global.ReadBarcode_NG_N.ToString(),
                            Global.CCDCheck_Error_N.ToString(), Global.Smallmaterial_throwing_N.ToString(), Global.HSG_NG_N.ToString(), DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd"));
                        SQL.ExecuteUpdate(InsertStr6);
                        Log.WriteLog("写入夜班小料抛料数据" + InsertStr6);

                        //string InsertStr7 = string.Format("UPDATE [ErrorDataStatistics] SET [Throwing] = '{0}',[Welding] = '{1}'   where DateTime = '{2}'",
                        //    Global.Throwing_N.ToString(), Global.Welding_N.ToString(), DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd"));
                        //SQL.ExecuteUpdate(InsertStr7);
                        //Log.WriteLog("写入夜班小料抛料数据" + InsertStr7);
                        Thread.Sleep(1000);
                    }
                    //写入白班产能与小料抛料数据
                    if (DateTime.Now.Hour == 21 && DateTime.Now.Minute == 30 && DateTime.Now.Second == 0 && Global.Product_status)
                    {

                        InsertSQLFlag = false;
                        string InsertStr1 = "insert into Product([DateTime],[Status],[Hour_1],[Hour_2],[Hour_3],[Hour_4],[Hour_5],"
                           + "[Hour_6],[Hour_7],[Hour_8],[Hour_9],[Hour_10],[Hour_11],[Hour_12]"
                           + ")" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd") + "'" + "," + "'" + "OK产量" + "'" + ","
                           + "'" + Global.Product_OK[0] + "'" + "," + "'" + Global.Product_OK[1] + "'" + "," + "'" + Global.Product_OK[2] + "'" + ","
                           + "'" + Global.Product_OK[3] + "'" + "," + "'" + Global.Product_OK[4] + "'" + "," + "'" + Global.Product_OK[5] + "'" + ","
                           + "'" + Global.Product_OK[6] + "'" + "," + "'" + Global.Product_OK[7] + "'" + "," + "'" + Global.Product_OK[8] + "'" + ","
                           + "'" + Global.Product_OK[9] + "'" + "," + "'" + Global.Product_OK[10] + "'" + "," + "'" + Global.Product_OK[11] + "'" + ")";
                        SQL.ExecuteUpdate(InsertStr1);
                        Log.WriteLog("写入白班OK产能数据" + InsertStr1);

                        string InsertStr2 = "insert into Product([DateTime],[Status],[Hour_1],[Hour_2],[Hour_3],[Hour_4],[Hour_5],"
                           + "[Hour_6],[Hour_7],[Hour_8],[Hour_9],[Hour_10],[Hour_11],[Hour_12]"
                           + ")" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd") + "'" + "," + "'" + "NG产量" + "'" + ","
                           + "'" + Global.Product_NG[0] + "'" + "," + "'" + Global.Product_NG[1] + "'" + "," + "'" + Global.Product_NG[2] + "'" + ","
                           + "'" + Global.Product_NG[3] + "'" + "," + "'" + Global.Product_NG[4] + "'" + "," + "'" + Global.Product_NG[5] + "'" + ","
                           + "'" + Global.Product_NG[6] + "'" + "," + "'" + Global.Product_NG[7] + "'" + "," + "'" + Global.Product_NG[8] + "'" + ","
                           + "'" + Global.Product_NG[9] + "'" + "," + "'" + Global.Product_NG[10] + "'" + "," + "'" + Global.Product_NG[11] + "'" + ")";
                        SQL.ExecuteUpdate(InsertStr2);
                        Log.WriteLog("写入白班NG产能数据" + InsertStr2);

                        string InsertStr3 = "insert into HourDT([DateTime],[Status],[Hour_DT1],[Hour_DT2],[Hour_DT3],[Hour_DT4],[Hour_DT5],"
                         + "[Hour_DT6],[Hour_DT7],[Hour_DT8],[Hour_DT9],[Hour_DT10],[Hour_DT11],[Hour_DT12]"
                         + ")" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd") + "'" + "," + "'" + "运行时间" + "'" + ","
                         + "'" + (Convert.ToDouble(Global.DT_RunTime[0])).ToString("0.00") + "'" + "," + "'" + (Convert.ToDouble(Global.DT_RunTime[1])).ToString("0.00") + "'" + "," + "'" + (Convert.ToDouble(Global.DT_RunTime[2])).ToString("0.00") + "'" + ","
                         + "'" + (Convert.ToDouble(Global.DT_RunTime[3])).ToString("0.00") + "'" + "," + "'" + (Convert.ToDouble(Global.DT_RunTime[4])).ToString("0.00") + "'" + "," + "'" + (Convert.ToDouble(Global.DT_RunTime[5])).ToString("0.00") + "'" + ","
                         + "'" + (Convert.ToDouble(Global.DT_RunTime[6])).ToString("0.00") + "'" + "," + "'" + (Convert.ToDouble(Global.DT_RunTime[7])).ToString("0.00") + "'" + "," + "'" + (Convert.ToDouble(Global.DT_RunTime[8])).ToString("0.00") + "'" + ","
                         + "'" + (Convert.ToDouble(Global.DT_RunTime[9])).ToString("0.00") + "'" + "," + "'" + (Convert.ToDouble(Global.DT_RunTime[10])).ToString("0.00") + "'" + "," + "'" + (Convert.ToDouble(Global.DT_RunTime[11])).ToString("0.00") + "'" + ")";
                        SQL.ExecuteUpdate(InsertStr3);
                        Log.WriteLog("写入白班运行时间数据" + InsertStr3);

                        string InsertStr4 = "insert into HourDT([DateTime],[Status],[Hour_DT1],[Hour_DT2],[Hour_DT3],[Hour_DT4],[Hour_DT5],"
                         + "[Hour_DT6],[Hour_DT7],[Hour_DT8],[Hour_DT9],[Hour_DT10],[Hour_DT11],[Hour_DT12]"
                         + ")" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd") + "'" + "," + "'" + "异常时间" + "'" + ","
                         + "'" + (Convert.ToDouble(Global.DT_ErrorTime[0])).ToString("0.00") + "'" + "," + "'" + (Convert.ToDouble(Global.DT_ErrorTime[1])).ToString("0.00") + "'" + "," + "'" + (Convert.ToDouble(Global.DT_ErrorTime[2])).ToString("0.00") + "'" + ","
                         + "'" + (Convert.ToDouble(Global.DT_ErrorTime[3])).ToString("0.00") + "'" + "," + "'" + (Convert.ToDouble(Global.DT_ErrorTime[4])).ToString("0.00") + "'" + "," + "'" + (Convert.ToDouble(Global.DT_ErrorTime[5])).ToString("0.00") + "'" + ","
                         + "'" + (Convert.ToDouble(Global.DT_ErrorTime[6])).ToString("0.00") + "'" + "," + "'" + (Convert.ToDouble(Global.DT_ErrorTime[7])).ToString("0.00") + "'" + "," + "'" + (Convert.ToDouble(Global.DT_ErrorTime[8])).ToString("0.00") + "'" + ","
                         + "'" + (Convert.ToDouble(Global.DT_ErrorTime[9])).ToString("0.00") + "'" + "," + "'" + (Convert.ToDouble(Global.DT_ErrorTime[10])).ToString("0.00") + "'" + "," + "'" + (Convert.ToDouble(Global.DT_ErrorTime[11])).ToString("0.00") + "'" + ")";
                        SQL.ExecuteUpdate(InsertStr4);
                        Log.WriteLog("写入白班异常时间数据" + InsertStr4);

                        string InsertStr5 = "insert into HourDT([DateTime],[Status],[Hour_DT1],[Hour_DT2],[Hour_DT3],[Hour_DT4],[Hour_DT5],"
                         + "[Hour_DT6],[Hour_DT7],[Hour_DT8],[Hour_DT9],[Hour_DT10],[Hour_DT11],[Hour_DT12]"
                         + ")" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd") + "'" + "," + "'" + "待料时间" + "'" + ","
                         + "'" + (Convert.ToDouble(Global.DT_PendingTime[0])).ToString("0.00") + "'" + "," + "'" + (Convert.ToDouble(Global.DT_PendingTime[1])).ToString("0.00") + "'" + "," + "'" + (Convert.ToDouble(Global.DT_PendingTime[2])).ToString("0.00") + "'" + ","
                         + "'" + (Convert.ToDouble(Global.DT_PendingTime[3])).ToString("0.00") + "'" + "," + "'" + (Convert.ToDouble(Global.DT_PendingTime[4])).ToString("0.00") + "'" + "," + "'" + (Convert.ToDouble(Global.DT_PendingTime[5])).ToString("0.00") + "'" + ","
                         + "'" + (Convert.ToDouble(Global.DT_PendingTime[6])).ToString("0.00") + "'" + "," + "'" + (Convert.ToDouble(Global.DT_PendingTime[7])).ToString("0.00") + "'" + "," + "'" + (Convert.ToDouble(Global.DT_PendingTime[8])).ToString("0.00") + "'" + ","
                         + "'" + (Convert.ToDouble(Global.DT_PendingTime[9])).ToString("0.00") + "'" + "," + "'" + (Convert.ToDouble(Global.DT_PendingTime[10])).ToString("0.00") + "'" + "," + "'" + (Convert.ToDouble(Global.DT_PendingTime[11])).ToString("0.00") + "'" + ")";
                        SQL.ExecuteUpdate(InsertStr5);
                        Log.WriteLog("写入白班待料时间数据" + InsertStr5);

                        string InsertStr6 = "insert into ErrorDataStatistics([DateTime],[Product_Total_D],[Smallmaterial_Input_D],[Location1_NG_D],[Location2_NG_D],[Location3_NG_D],[Location4_NG_D],"
                            + "[Location5_NG_D],[HansDataError_D],[TraceUpLoad_Error_D],[PDCAUpLoad_Error_D],[TracePVCheck_Error_D],[ReadBarcode_NG_D],[CCDCheck_Error_D],[Smallmaterial_throwing_D],[Band_NG_D]"
                        + ")" + " " + "values(" + "'"
                        + DateTime.Now.ToString("yyyy-MM-dd") + "'" + "," + "'" + Global.Product_Total_D.ToString() + "'" + "," + "'" + Global.Smallmaterial_Input_D.ToString() + "'" + "," + "'"
                        + Global.location1_CCDNG_D.ToString() + "'" + "," + "'" + Global.location2_CCDNG_D.ToString() + "'" + "," + "'" + Global.location3_CCDNG_D.ToString() + "'" + "," + "'"
                        + Global.TraceTab_Error_D.ToString() + "'" + "," + "'" + Global.TraceThench_Error_D.ToString() + "'" + "," + "'" + Global.HansDataError_D.ToString() + "'" + "," + "'"
                        + Global.TraceUpLoad_Error_D.ToString() + "'" + "," + "'" + Global.PDCAUpLoad_Error_D.ToString() + "'" + "," + "'" + Global.TracePVCheck_Error_D.ToString() + "'" + "," + "'"
                        + Global.ReadBarcode_NG_D.ToString() + "'" + "," + "'" + Global.CCDCheck_Error_D.ToString() + "'" + "," + "'" + Global.Smallmaterial_throwing_D.ToString() + "'" + ",'" + Global.HSG_NG_D.ToString() + "'" + ")";
                        SQL.ExecuteUpdate(InsertStr6);
                        Log.WriteLog("写入白班抛料统计数据" + InsertStr6);

                        //string InsertStr7 = "insert into ErrorDataStatistics([DateTime],[Throwing],[Welding]"
                        //+ ")" + " " + "values(" + "'"
                        //+ DateTime.Now.ToString("yyyy-MM-dd") + "'" + "," + "'" + Global.Throwing_D.ToString() + "'" + "," + "'" + Global.Welding_D.ToString() + "'" + ")";
                        //SQL.ExecuteUpdate(InsertStr7);
                        //Log.WriteLog("写入白班辅助抛料统计数据" + InsertStr7);
                        Thread.Sleep(1000);
                    }

                    Thread.Sleep(10);
                }
                catch (Exception EX)
                {
                    Log.WriteLog("按时做某事线程异常" + EX.ToString());
                }
            }
        }

        public void Permission_switch(object ob)//按时切换权限
        {
            while (true)
            {
                try
                {
                    if (Global.IfLoginbtn && Global.Login != Global.LoginLevel.Operator)
                    {
                        if ((DateTime.Now - Global.UserLoginMouseMoveTime).TotalMinutes > 1)
                        {
                            _userloginfrm.btn_UserLogin_Click(null, null);
                            this.Invoke(new Action(() => btn_home_Click(null, null)));
                            _userloginfrm.ComboBoxSelect(1);
                        }
                    }
                    if (Global.inidata.productconfig.OpenCard != "true" && Global.Login != Global.LoginLevel.Operator && Global.IfLogin == true)
                    {
                        if ((DateTime.Now - Global.UserLoginMouseMoveTime).TotalMinutes > 1)
                        {
                            this.Invoke(new Action(() => btn_home_Click(null, null)));
                            _userloginfrm.ComboBoxSelect(1);
                            Global.IfLogin = false;
                            Global.Login = Global.LoginLevel.Operator;
                            Global.PLC_Client.WritePLC_D(16000, new short[] { 0 });//发送权限等级给PLC
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
                            Global.inidata.WriteProductnumSection();// 发送权限等级给CCD      
                        }
                    }
                    if (Global.IfReadUserID)
                    {
                        if ((DateTime.Now - Global.UserLoginMouseMoveTime).TotalMinutes > 15)
                        {
                            Global.IfReadUserID = false;
                            _userloginfrm.UiLabel("", "lb_CardNo");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.WriteLog(ex.ToString());
                }
                Thread.Sleep(100);
            }
        }

        public void ReadOperatorID(object ob)//固定式打卡机读取操作员卡号
        {
            while (Global.IfPQStatus)//每隔5分钟读取一次操作员卡号
            {
                Log.WriteLog("固定式刷卡机开始读卡！");
                FixedRead();
                Thread.Sleep(300000);
            }
        }

        private void FixedRead()
        {
            try
            {
                string CardNo = string.Empty;
                short icdev = 0x0000;
                int status;
                byte mode = 0x52;
                byte bcnt = 0x04;
                byte[] pSnr = new byte[10];
                ushort tagtype = 0;
                byte pRLength = 0;
                byte srcLen = 0;
                sbyte Size = 0;

                if (!Global.bConnectedDevice)
                {
                    MessageBox.Show("固定式刷卡机串口未连接成功！");
                    return;
                }

                status = rf_request(icdev, mode, ref tagtype);
                if (0 != status)
                {
                    //MessageBox.Show("固定式刷卡机读卡失败！");
                    Log.WriteLog("固定式刷卡机读卡失败！");
                    //txtCardNo.Text = "";
                    return;
                }
                //lblInfo.Text = "寻卡成功";
                status = rf_anticoll(icdev, bcnt, ref pSnr[0], ref pRLength);
                if (0 != status)
                {
                    //MessageBox.Show("固定式刷卡机防冲突失败！");
                    Log.WriteLog("固定式刷卡机防冲突失败！");
                    return;
                }
                else
                {
                    //txtCardNo.Text = "";
                    string temp = string.Empty;
                    for (int i = 0; i < pRLength; i++)
                    {
                        temp = pSnr[i].ToString("X");
                        temp = temp.Length == 1 ? "0" + temp : temp;        //如果返回的是1字节的1个位，前面需要补0
                        CardNo = CardNo + temp;
                    }
                    //lblInfo.Text = "防冲突操作成功";
                    string var = CardNo.Substring(6, 2) + CardNo.Substring(4, 2) + CardNo.Substring(2, 2) + CardNo.Substring(0, 2);
                    UInt32 x = Convert.ToUInt32(var, 16);//字符串转16进制32位无符号整数
                    CardNo = x.ToString();
                    //MessageBox.Show("固定式刷卡机读卡成功" + x.ToString());
                    Log.WriteLog("固定式刷卡机读卡成功,卡号: " + x.ToString());
                }
                status = rf_select(icdev, ref pSnr[0], srcLen, ref Size);
                if (0 != status)
                {
                    //MessageBox.Show("固定式刷卡机选卡失败！");
                    Log.WriteLog("固定式刷卡机防冲突失败！");
                    return;
                }
            }
            catch (Exception ex)
            {
                Log.WriteLog(ex.ToString());
            }
        }

        public bool dataGridViewToCSV(DataGridView dataGridView)
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

        private void Timing(object ob)             //和PLC对表
        {
            while (true)
            {
                if (DateTime.Now.Minute == 33)
                {
                    Global.PLC_Client.WritePLC_D(10280, new short[] { 0 });
                    time = DateTime.Now.Hour;
                    Global.PLC_Client.WritePLC_D(10270, new short[] { (short)DateTime.Now.Year });
                    Global.PLC_Client.WritePLC_D(10271, new short[] { (short)DateTime.Now.Month });
                    Global.PLC_Client.WritePLC_D(10272, new short[] { (short)DateTime.Now.Day });
                    Global.PLC_Client.WritePLC_D(10273, new short[] { (short)DateTime.Now.Hour });
                    Global.PLC_Client.WritePLC_D(10274, new short[] { (short)DateTime.Now.Minute });
                    Global.PLC_Client.WritePLC_D(10275, new short[] { (short)DateTime.Now.Second });
                    Global.PLC_Client.WritePLC_D(10276, new short[] { (short)DateTime.Now.DayOfWeek });
                    Thread.Sleep(500);
                    Global.PLC_Client.WritePLC_D(10280, new short[] { 1 });
                    Log.WriteLog("与PLC同步时间成功：" + DateTime.Now.ToString());
                }
                Thread.Sleep(50000);
            }
        }

        public void SaveASToWord(DataGridView datagridview1)
        {
            //if (datagridview1.CurrentRow == null)
            //{
            //    MessageBox.Show("无数据可导出！", "来自系统的消息");
            //}
            //else
            //{
            //    Microsoft.Office.Interop.Word.ApplicationClass wordApp = new Microsoft.Office.Interop.Word.ApplicationClass();
            //    Microsoft.Office.Interop.Word.Document document;
            //    Microsoft.Office.Interop.Word.Table wordTable;

            //    Microsoft.Office.Interop.Word.Selection wordSelection;
            //    object wordObj = System.Reflection.Missing.Value;

            //    document = wordApp.Documents.Add(ref wordObj, ref wordObj, ref wordObj, ref wordObj);
            //    wordSelection = wordApp.Selection;//显示word文档
            //    wordApp.Visible = true;
            //    if (wordApp == null)
            //    {
            //        MessageBox.Show("本地Word程序无法启动!请检查您的Microsoft Office正确安装并能正常使用", "提示");
            //        return;
            //    }
            //    document.Select();
            //    wordTable = document.Tables.Add(wordSelection.Range, datagridview1.Rows.Count, datagridview1.Columns.Count - 1, ref wordObj, ref wordObj);
            //    //设置列宽
            //    wordTable.Columns.SetWidth(50.0F, Microsoft.Office.Interop.Word.WdRulerStyle.wdAdjustSameWidth);
            //    //标题数据
            //    for (int i = 1; i < datagridview1.Columns.Count; i++)
            //    {
            //        wordTable.Cell(1, i).Range.InsertAfter(datagridview1.Columns[i].HeaderText);
            //    }
            //    //输出表中数据
            //    try
            //    {
            //        for (int i = 0; i <= datagridview1.Rows.Count - 1; i++)
            //        {
            //            for (int j = 1; j < datagridview1.Columns.Count; j++)
            //            {
            //                wordTable.Cell(i + 2, j).Range.InsertAfter(datagridview1.Rows[i].Cells[j].Value.ToString());
            //            }
            //        }
            //    }
            //    catch (Exception e)
            //    {
            //        MessageBox.Show("导出成功！", "来自系统的消息");
            //    }
            //    //wordTable.Borders.InsideLineStyle = Microsoft.Office.Interop.Word.WdLineStyle.wdLineStyleSingle;
            //    //wordTable.Borders.OutsideLineStyle = Microsoft.Office.Interop.Word.WdLineStyle.wdLineStyleSingle;
            //}
        }//另存为Word


        private void MainFrm_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (Global.SelectFirst == true)//首件中关闭软件,通知Plc结束首件,防止产能不计数
                {
                    try
                    {
                        Global.PLC_Client.WritePLC_D(13023, new short[] { 0 });//通知PLC做首件做验证已结束
                    }
                    catch
                    { }
                }
                PassWordFrm pw = new PassWordFrm();
                //pw.PermissionIndex += new Form1.PermissionEventHandler(SetPermissionIndex);
                pw.ShowDialog();
                if (pw.DialogResult == System.Windows.Forms.DialogResult.OK)
                {
                    //把小料抛料数据写进setting
                    Global.inidata.WriteProductnumSection();
                    var IP = GetIp();
                    var Mac = GetOEEMac();
                    string sendTopic = Global.inidata.productconfig.EMT + "/upload/downtime";
                    string ClientPcName = Dns.GetHostName();
                    string StopTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                    if (!Global.SelectFirstModel)//当前是否属于手动(首件)状态
                    {
                        if (!Global.SelectTestRunModel)//判断是否处于空跑状态（PLC屏蔽部分功能如：安全门，扫码枪，机械手）
                        {
                            if (Global.j == 1 || Global.j == 2 || Global.j == 3 || Global.j == 4)//j为机台运行大状态（-1初始值、1待料、2运行、3宕机、4人工停止）
                            {
                                if (Global.j == 1 && Global.ed[Global.Error_PendingNum + 1].start_time != null)//当前是否属于待料状态
                                {
                                    DateTime t1 = Convert.ToDateTime(Global.ed[Global.Error_PendingNum + 1].start_time);
                                    DateTime t2 = Convert.ToDateTime(StopTime);
                                    string ts = (t2 - t1).TotalMinutes.ToString("0.00");
                                    if (Global.Error_PendingStatus)//判断待料时是否打开安全门/按下暂停键
                                    {
                                        Global.ed[Global.Error_PendingNum + 1].stop_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                                        if (Global.ed[Global.Error_PendingNum + 1].start_time != null)
                                        {
                                            string OEE_DT1 = "";
                                            Guid guid1 = Guid.NewGuid();
                                            OEE_DT1 = string.Format("{{\"guid\":\"{0}\",\"EMT\":\"{1}\",\"PoorNum\":\"{2}\",\"TotalNum\":\"{3}\",  \"Status\":\"{4}\",\"ErrorCode\":\"{5}\",\"EventTime\":\"{6}\",\"ModuleCode\":\"{7}\",\"ClientPcName\":\"{8}\",\"MAC\":\"{9}\",\"IP\":\"{10}\"}}", guid1, Global.inidata.productconfig.EMT, "0", "0", Global.errorStatus, Global.errorcode, Global.ed[Global.Error_PendingNum + 1].start_time, "", ClientPcName, Mac, IP);
                                            Log.WriteLog("OEE_DT前待料中安全门打开:" + OEE_DT1 + ",OEELog");
                                            //Global._mqttClient.Publish(sendTopic, Encoding.UTF8.GetBytes(OEE_DT1), MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE, false);
                                            //bool rst1 = SendMqttResult(guid1);
                                            //if (rst1)
                                            //{
                                            //    if (Global.respond[guid1].Result == "OK")
                                            //    {
                                            //        Log.WriteLog("OEE_DT安全门打开自动errorCode发送成功" + ",OEELog");
                                            //        _homefrm.AppendRichText(Global.errorcode + ",触发时间=" + Global.ed[Global.Error_PendingNum + 1].start_time + ",运行状态:" + Global.errorStatus + ",故障描述:" + Global.errorinfo + ",安全门打开自动发送成功", "rtx_DownTimeMsg");
                                            //        Global.respond.TryRemove(guid1, out Global.Respond);
                                            //    }
                                            //    else
                                            //    {
                                            //        _homefrm.AppendRichText(Global.errorcode + ",触发时间=" + Global.ed[Global.Error_PendingNum + 1].start_time + ",运行状态:" + Global.errorStatus + ",故障描述:" + Global.errorinfo + ",自动发送失败", "rtx_DownTimeMsg");
                                            //        _homefrm.AppendRichText("失败原因:" + Global.respond[guid1].ErrorCode + Global.respond[guid1].Result, "rtx_DownTimeMsg");
                                            //        Log.WriteLog("OEE_DT安全门打开自动errorCode发送失败" + ",OEELog");
                                            //        Log.WriteLog(Global.respond[guid1].GUID.ToString() + "," + Global.respond[guid1].Result + "," + Global.respond[guid1].ErrorCode, System.AppDomain.CurrentDomain.BaseDirectory + "\\MQTT返回NG\\");
                                            //        Global.respond.TryRemove(guid1, out Global.Respond);
                                            //    }
                                            //}
                                            //else
                                            //{
                                            //    Log.WriteLog("OEE_DT安全门打开自动errorCode发送失败,超时无反馈:" + ",OEELog");
                                            //    _homefrm.AppendRichText(Global.errorcode + ",触发时间=" + Global.ed[Global.Error_PendingNum + 1].start_time + ",运行状态:" + Global.errorStatus + ",故障描述:" + Global.errorinfo + ",安全门打开自动发送失败", "rtx_DownTimeMsg");
                                            //    _homefrm.AppendRichText("网络异常,超时无反馈", "rtx_DownTimeMsg");
                                            //    string s = "insert into OEE_DTSendNG([DateTime],[Product],[GUID],[EMT],[PoorNum],[TotalNum],[Status],[ErrorCode],[ModuleCode],[ClientPcName],[MAC],[IP],[EventTime],[Moduleinfo],[errorinfo])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," + "'" + "OEE_DT" + "'" + "," + "'" + guid1 + "'" + "," + "'" + Global.inidata.productconfig.EMT + "'" + "," + "'" + "0" + "'" + "," + "'" + "0" + "'" + "," + "'" + Global.errorStatus + "'" + "," + "'" + Global.errorcode + "'" + ","
                                            //           + "'" + "" + "'" + "," + "'" + ClientPcName + "'" + "," + "'" + Mac + "'" + "," + "'" + IP + "'" + "," + "'" + Global.ed[Global.Error_PendingNum + 1].start_time + "'" + "," + "'" + "" + "'" + "," + "'" + Global.errorinfo + "'" + ")";
                                            //    int r = SQL.ExecuteUpdate(s);
                                            //    Log.WriteLog(string.Format("插入了{0}行OEE_DownTime缓存数据", r) + ",OEELog");
                                            //}
                                            string InsertOEEStr = "insert into OEE_DT([DateTime],[ErrorCode],[EventTime],[ModuleCode],[RunStatus],[ErrorInfo],[TimeSpan])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "'" + "," + "'" + Global.errorcode + "'" + "," + "'" + Global.ed[Global.Error_PendingNum + 1].start_time + "'" + ","
                                           + "'" + "" + "'" + "," + "'" + Global.errorStatus + "'" + "," + "'" + Global.errorinfo + "'" + "," + "'" + ts + "'" + ")";
                                            SQL.ExecuteUpdate(InsertOEEStr);
                                            InsertOEEStr = "insert into OEE_TraceDT([DateTime],[Status],[ErrorCode],[EventTime],[ModuleCode])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," + "'" + Global.errorStatus + "'" + "," + "'" + Global.errorcode + "'" + ","
                                            + "'" + Global.ed[Global.Error_PendingNum + 1].start_time + "'" + "," + "'" + Global.ed[Global.Error_PendingNum + 1].ModuleCode + "'" + ")";
                                            SQL.ExecuteUpdate(InsertOEEStr);
                                            Log.WriteLog(DateTime.Now.ToString("HH:mm:ss") + "," + Global.errorcode + "," + Global.ed[Global.Error_PendingNum + 1].start_time + "," + "" + "," + "自动发送成功" + "," + Global.errorStatus + "," + Global.errorinfo + "," + ts, @"E:\装机软件\系统配置\System_ini\");
                                            Log.WriteLog("" + "_" + Global.errorinfo + "：结束计时 " + Global.ed[Global.Error_PendingNum + 1].stop_time);
                                        }
                                    }
                                    else
                                    {
                                        Global.ed[Global.Error_PendingNum + 1].stop_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                                        string c = "c=UPLOAD_DOWNTIME&tsn=Test_station&mn=Machine#1&start_time=" + Global.ed[Global.Error_PendingNum + 1].start_time + "&stop_time=" + Global.ed[Global.Error_PendingNum + 1].stop_time + "&ec=" + Global.ed[Global.Error_PendingNum + 1].errorCode;
                                        Log.WriteLog(c + ",OEELog");
                                        if (Global.ed[Global.Error_PendingNum + 1].start_time != null)
                                        {
                                            string InsertStr1 = "insert into OEE_DT([DateTime],[ErrorCode],[EventTime],[ModuleCode],[RunStatus],[ErrorInfo],[TimeSpan])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "'" + "," + "'" + Global.ed[Global.Error_PendingNum + 1].errorCode + "'" + "," + "'" + Global.ed[Global.Error_PendingNum + 1].start_time + "'" + ","
                                               + "'" + Global.ed[Global.Error_PendingNum + 1].ModuleCode + "'" + "," + "'" + Global.ed[Global.Error_PendingNum + 1].errorStatus + "'" + "," + "'" + Global.ed[Global.Error_PendingNum + 1].errorinfo + "'" + "," + "'" + ts + "'" + ")";
                                            SQL.ExecuteUpdate(InsertStr1);
                                            Log.WriteLog(DateTime.Now.ToString("HH:mm:ss") + "," + Global.ed[Global.Error_PendingNum + 1].errorCode + "," + Global.ed[Global.Error_PendingNum + 1].start_time + "," + Global.ed[Global.Error_PendingNum + 1].ModuleCode + "," + "自动发送成功" + "," + Global.ed[Global.Error_PendingNum + 1].errorStatus + "," + Global.ed[Global.Error_PendingNum + 1].errorinfo + "," + ts, System.AppDomain.CurrentDomain.BaseDirectory + "\\系统配置\\System_ini\\");
                                            _manualfrm.labelcolor(Color.Transparent, "未选择", "LB_ManualSelect");
                                        }
                                        Log.WriteLog(Global.ed[Global.Error_PendingNum + 1].Moduleinfo + "_" + Global.ed[Global.Error_PendingNum + 1].errorinfo + "：结束计时 " + Global.ed[Global.Error_PendingNum + 1].stop_time + ",OEELog");
                                        if (Global.Feeding)//人工上料中机台启动自动结束人工上料补传一笔HSG待料
                                        {
                                            Global.SendHSG_start_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                                            Global.Feeding = false;
                                            SendHSG();
                                            Global.SendHSG_start_time = null;
                                        }
                                    }
                                }
                                else if (Global.j == 2 && Global.ed[Global.j].start_time != null)//当前是否属于运行状态
                                {
                                    DateTime t1 = Convert.ToDateTime(Global.ed[Global.j].start_time);
                                    DateTime t2 = Convert.ToDateTime(StopTime);
                                    string ts = (t2 - t1).TotalMinutes.ToString("0.00");
                                    string InsertStr2 = "insert into OEE_DT([DateTime],[ErrorCode],[EventTime],[ModuleCode],[RunStatus],[ErrorInfo],[TimeSpan])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "'" + "," + "'" + Global.ed[Global.j].errorCode + "'" + "," + "'" + Global.ed[Global.j].start_time + "'" + ","
                                       + "'" + Global.ed[Global.j].ModuleCode + "'" + "," + "'" + Global.ed[Global.j].errorStatus + "'" + "," + "'" + Global.ed[Global.j].errorinfo + "'" + "," + "'" + ts + "'" + ")";
                                    SQL.ExecuteUpdate(InsertStr2);
                                    Log.WriteCSV(DateTime.Now.ToString("HH:mm:ss") + "," + Global.ed[Global.j].errorCode + "," + Global.ed[Global.j].start_time + "," + Global.ed[Global.j].ModuleCode + "," + "自动发送成功" + "," + Global.ed[Global.j].errorStatus + "," + Global.ed[Global.j].errorinfo + "," + ts, System.AppDomain.CurrentDomain.BaseDirectory + "\\系统配置\\System_ini\\");
                                }
                                else if (Global.j == 3 && Global.ed[Global.Error_num + 1].start_time != null)//当前是否属于宕机状态
                                {
                                    DateTime t1 = Convert.ToDateTime(Global.ed[Global.Error_num + 1].start_time);
                                    DateTime t2 = Convert.ToDateTime(StopTime);
                                    string ts = (t2 - t1).TotalMinutes.ToString("0.00");
                                    string InsertStr3 = "insert into OEE_DT([DateTime],[ErrorCode],[EventTime],[ModuleCode],[RunStatus],[ErrorInfo],[TimeSpan])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "'" + "," + "'" + Global.ed[Global.Error_num + 1].errorCode + "'" + "," + "'" + Global.ed[Global.Error_num + 1].start_time + "'" + ","
                                       + "'" + Global.ed[Global.Error_num + 1].ModuleCode + "'" + "," + "'" + Global.ed[Global.Error_num + 1].errorStatus + "'" + "," + "'" + Global.ed[Global.Error_num + 1].errorinfo + "'" + "," + "'" + ts + "'" + ")";
                                    SQL.ExecuteUpdate(InsertStr3);
                                    Log.WriteCSV(DateTime.Now.ToString("HH:mm:ss") + "," + Global.ed[Global.Error_num + 1].errorCode + "," + Global.ed[Global.Error_num + 1].start_time + "," + Global.ed[Global.Error_num + 1].ModuleCode + "," + "自动发送成功" + "," + Global.ed[Global.Error_num + 1].errorStatus + "," + Global.ed[Global.Error_num + 1].errorinfo + "," + ts, System.AppDomain.CurrentDomain.BaseDirectory + "\\系统配置\\System_ini\\");
                                }
                                else if (Global.j == 4 && Global.ed[Global.Error_Stopnum + 1].start_time != null)//当前是否属于人工停止复位状态
                                {
                                    Global.ed[Global.Error_Stopnum + 1].stop_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                                    string c = "c=UPLOAD_DOWNTIME&tsn=Test_station&mn=Machine#1&start_time=" + Global.ed[Global.Error_Stopnum + 1].start_time + "&stop_time=" + Global.ed[Global.Error_Stopnum + 1].stop_time + "&ec=" + Global.ed[Global.Error_Stopnum + 1].errorCode;
                                    Log.WriteLog(c + ",OEELog");
                                    if (Global.ed[Global.Error_Stopnum + 1].start_time != null)
                                    {
                                        DateTime t1 = Convert.ToDateTime(Global.ed[Global.Error_Stopnum + 1].start_time);
                                        DateTime t2 = Convert.ToDateTime(Global.ed[Global.Error_Stopnum + 1].stop_time);
                                        string ts = (t2 - t1).TotalMinutes.ToString("0.00");
                                        if (Global.Error_Stopnum == 10 || Global.Error_Stopnum == 11 || Global.Error_Stopnum == 12 || Global.Error_Stopnum == 13 || Global.Error_Stopnum == 54 || Global.SelectManualErrorCode)//机台打开安全门或者手动选择ErrorCode状态开启
                                        {
                                            string OEE_DT = "";
                                            Guid guid = Guid.NewGuid();
                                            OEE_DT = string.Format("{{\"guid\":\"{0}\",\"EMT\":\"{1}\",\"PoorNum\":\"{2}\",\"TotalNum\":\"{3}\",  \"Status\":\"{4}\",\"ErrorCode\":\"{5}\",\"EventTime\":\"{6}\",\"ModuleCode\":\"{7}\",\"ClientPcName\":\"{8}\",\"MAC\":\"{9}\",\"IP\":\"{10}\"}}", guid, Global.inidata.productconfig.EMT, "0", "0", Global.errorStatus, Global.errorcode, Global.ed[Global.Error_Stopnum + 1].start_time, "", ClientPcName, Mac, IP);
                                            string InsertStr1 = "insert into OEE_TraceDT([DateTime],[Status],[ErrorCode],[EventTime],[ModuleCode])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," + "'" + Global.errorStatus + "'" + "," + "'" + Global.errorcode + "'" + ","
                                            + "'" + Global.ed[Global.Error_Stopnum + 1].start_time + "'" + "," + "'" + Global.ed[Global.Error_Stopnum + 1].ModuleCode + "'" + ")";
                                            SQL.ExecuteUpdate(InsertStr1);
                                            Log.WriteLog("关机前 OEE_DT安全门打开:" + OEE_DT + ",OEELog");
                                            //Global._mqttClient.Publish(sendTopic, Encoding.UTF8.GetBytes(OEE_DT), MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE, false);
                                            //bool rst4 = SendMqttResult(guid);
                                            //if (rst4)
                                            //{
                                            //    if (Global.respond[guid].Result == "OK")
                                            //    {
                                            //        Log.WriteLog("关机前 OEE_DT安全门打开自动errorCode发送成功" + ",OEELog");
                                            //        _homefrm.AppendRichText(Global.errorcode + ",触发时间=" + Global.ed[Global.Error_Stopnum + 1].start_time + ",运行状态:" + Global.errorStatus + ",故障描述:" + Global.errorinfo + ",安全门打开自动发送成功", "rtx_DownTimeMsg");
                                            //        Global.respond.TryRemove(guid, out Global.Respond);
                                            //    }
                                            //    else
                                            //    {
                                            //        Log.WriteLog("关机前 OEE_DT安全门打开自动errorCode发送失败" + ",OEELog");
                                            //        Log.WriteLog(Global.respond[guid].GUID.ToString() + "," + Global.respond[guid].Result + "," + Global.respond[guid].ErrorCode, System.AppDomain.CurrentDomain.BaseDirectory + "\\MQTT返回NG\\");
                                            //        _homefrm.AppendRichText(Global.errorcode + ",触发时间=" + Global.ed[Global.Error_Stopnum + 1].start_time + ",运行状态:" + Global.errorStatus + ",故障描述:" + Global.errorinfo + ",安全门打开自动发送失败", "rtx_DownTimeMsg");
                                            //        _homefrm.AppendRichText("失败原因:" + Global.respond[guid].ErrorCode + Global.respond[guid].Result, "rtx_DownTimeMsg");
                                            //        string s = "insert into OEE_DTSendNG([DateTime],[Product],[GUID],[EMT],[PoorNum],[TotalNum],[Status],[ErrorCode],[ModuleCode],[ClientPcName],[MAC],[IP],[EventTime],[Moduleinfo],[errorinfo])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," + "'" + "OEE_DT" + "'" + "," + "'" + guid + "'" + "," + "'" + Global.inidata.productconfig.EMT + "'" + "," + "'" + "0" + "'" + "," + "'" + "0" + "'" + "," + "'" + Global.errorStatus + "'" + "," + "'" + Global.errorcode + "'" + ","
                                            //           + "'" + "" + "'" + "," + "'" + ClientPcName + "'" + "," + "'" + Mac + "'" + "," + "'" + IP + "'" + "," + "'" + Global.ed[Global.Error_Stopnum + 1].start_time + "'" + "," + "'" + "" + "'" + "," + "'" + Global.errorinfo + "'" + ")";
                                            //        int r = SQL.ExecuteUpdate(s);
                                            //        Log.WriteLog(string.Format("插入了{0}行OEE_DownTime缓存数据", r) + ",OEELog");
                                            //        Global.respond.TryRemove(guid, out Global.Respond);
                                            //    }
                                            //}
                                            //else
                                            //{
                                            //    Log.WriteLog("OEE_DT人工停止复位自动errorCode发送失败,超时无反馈:" + ",OEELog");
                                            //    _homefrm.AppendRichText(Global.errorcode + ",触发时间=" + Global.ed[Global.Error_Stopnum + 1].start_time + ",运行状态:" + Global.errorStatus + ",故障描述:" + Global.errorinfo + ",安全门打开自动发送失败", "rtx_DownTimeMsg");
                                            //    _homefrm.AppendRichText("网络异常,超时无反馈", "rtx_DownTimeMsg");
                                            //    string s = "insert into OEE_DTSendNG([DateTime],[Product],[GUID],[EMT],[PoorNum],[TotalNum],[Status],[ErrorCode],[ModuleCode],[ClientPcName],[MAC],[IP],[EventTime],[Moduleinfo],[errorinfo])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," + "'" + "OEE_DT" + "'" + "," + "'" + guid + "'" + "," + "'" + Global.inidata.productconfig.EMT + "'" + "," + "'" + "0" + "'" + "," + "'" + "0" + "'" + "," + "'" + Global.errorStatus + "'" + "," + "'" + Global.errorcode + "'" + ","
                                            //           + "'" + "" + "'" + "," + "'" + ClientPcName + "'" + "," + "'" + Mac + "'" + "," + "'" + IP + "'" + "," + "'" + Global.ed[Global.Error_Stopnum + 1].start_time + "'" + "," + "'" + "" + "'" + "," + "'" + Global.errorinfo + "'" + ")";
                                            //    int r = SQL.ExecuteUpdate(s);
                                            //    Log.WriteLog(string.Format("插入了{0}行OEE_DownTime缓存数据", r) + ",OEELog");
                                            //}
                                            string InsertOEEStr = "insert into OEE_DT([DateTime],[ErrorCode],[EventTime],[ModuleCode],[RunStatus],[ErrorInfo],[TimeSpan])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "'" + "," + "'" + Global.errorcode + "'" + "," + "'" + Global.ed[Global.Error_Stopnum + 1].start_time + "'" + ","
                                           + "'" + "" + "'" + "," + "'" + Global.errorStatus + "'" + "," + "'" + Global.errorinfo + "'" + "," + "'" + ts + "'" + ")";
                                            SQL.ExecuteUpdate(InsertOEEStr);
                                            Log.WriteLog(DateTime.Now.ToString("HH:mm:ss") + "," + Global.errorcode + "," + Global.ed[Global.Error_Stopnum + 1].start_time + "," + "" + "," + "自动发送成功" + "," + Global.errorStatus + "," + Global.errorinfo + "," + ts, System.AppDomain.CurrentDomain.BaseDirectory + "\\系统配置\\System_ini\\");
                                        }
                                        else if (Global.Error_Stopnum == 280)//机台人工停止
                                        {
                                            string OEE_DT = "";
                                            Guid guid = Guid.NewGuid();
                                            OEE_DT = string.Format("{{\"guid\":\"{0}\",\"EMT\":\"{1}\",\"PoorNum\":\"{2}\",\"TotalNum\":\"{3}\",  \"Status\":\"{4}\",\"ErrorCode\":\"{5}\",\"EventTime\":\"{6}\",\"ModuleCode\":\"{7}\",\"ClientPcName\":\"{8}\",\"MAC\":\"{9}\",\"IP\":\"{10}\"}}", guid, Global.inidata.productconfig.EMT, "0", "0", Global.ed[Global.Error_Stopnum + 1].errorStatus, Global.ed[Global.Error_Stopnum + 1].errorCode, Global.ed[Global.Error_Stopnum + 1].start_time, Global.ed[Global.Error_Stopnum + 1].ModuleCode, ClientPcName, Mac, IP);
                                            string InsertStr2 = "insert into OEE_TraceDT([DateTime],[Status],[ErrorCode],[EventTime],[ModuleCode])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," + "'" + Global.ed[Global.Error_Stopnum + 1].errorStatus + "'" + "," + "'" + Global.ed[Global.Error_Stopnum + 1].errorCode + "'" + ","
                                            + "'" + Global.ed[Global.Error_Stopnum + 1].start_time + "'" + "," + "'" + Global.ed[Global.Error_Stopnum + 1].ModuleCode + "'" + ")";
                                            SQL.ExecuteUpdate(InsertStr2);
                                            Log.WriteLog("关机前 OEE_DT人工停止复位打开:" + OEE_DT + ",OEELog");
                                            //Global._mqttClient.Publish(sendTopic, Encoding.UTF8.GetBytes(OEE_DT), MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE, false);
                                            //bool rst4 = SendMqttResult(guid);
                                            //if (rst4)
                                            //{
                                            //    if (Global.respond[guid].Result == "OK")
                                            //    {
                                            //        Log.WriteLog("关机前 OEE_DT人工停止复位自动errorCode发送成功" + ",OEELog");
                                            //        _homefrm.AppendRichText(Global.ed[Global.Error_Stopnum + 1].errorCode + ",触发时间=" + Global.ed[Global.Error_Stopnum + 1].start_time + ",运行状态:" + Global.ed[Global.Error_Stopnum + 1].errorStatus + ",故障描述:" + Global.ed[Global.Error_Stopnum + 1].errorinfo + ",自动发送成功", "rtx_DownTimeMsg");
                                            //        Global.respond.TryRemove(guid, out Global.Respond);
                                            //    }
                                            //    else
                                            //    {
                                            //        Log.WriteLog("关机前 OEE_DT人工停止复位自动errorCode发送失败" + ",OEELog");
                                            //        Log.WriteLog(Global.respond[guid].GUID.ToString() + "," + Global.respond[guid].Result + "," + Global.respond[guid].ErrorCode, System.AppDomain.CurrentDomain.BaseDirectory + "\\MQTT返回NG\\");
                                            //        _homefrm.AppendRichText(Global.ed[Global.Error_Stopnum + 1].errorCode + ",触发时间=" + Global.ed[Global.Error_Stopnum + 1].start_time + ",运行状态:" + Global.ed[Global.Error_Stopnum + 1].errorStatus + ",故障描述:" + Global.ed[Global.Error_Stopnum + 1].errorinfo + ",自动发送失败", "rtx_DownTimeMsg");
                                            //        _homefrm.AppendRichText("失败原因:" + Global.respond[guid].ErrorCode + Global.respond[guid].Result, "rtx_DownTimeMsg");
                                            //        string s = "insert into OEE_DTSendNG([DateTime],[Product],[GUID],[EMT],[PoorNum],[TotalNum],[Status],[ErrorCode],[ModuleCode],[ClientPcName],[MAC],[IP],[EventTime],[Moduleinfo],[errorinfo])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," + "'" + "OEE_DT" + "'" + "," + "'" + guid + "'" + "," + "'" + Global.inidata.productconfig.EMT + "'" + "," + "'" + "0" + "'" + "," + "'" + "0" + "'" + "," + "'" + Global.ed[Global.Error_Stopnum + 1].errorStatus + "'" + "," + "'" + Global.ed[Global.Error_Stopnum + 1].errorCode + "'" + ","
                                            //           + "'" + Global.ed[Global.Error_Stopnum + 1].ModuleCode + "'" + "," + "'" + ClientPcName + "'" + "," + "'" + Mac + "'" + "," + "'" + IP + "'" + "," + "'" + Global.ed[Global.Error_Stopnum + 1].start_time + "'" + "," + "'" + Global.ed[Global.Error_Stopnum + 1].Moduleinfo + "'" + "," + "'" + Global.ed[Global.Error_Stopnum + 1].errorinfo + "'" + ")";
                                            //        int r = SQL.ExecuteUpdate(s);
                                            //        Global.respond.TryRemove(guid, out Global.Respond);
                                            //    }
                                            //}
                                            //else
                                            //{
                                            //    Log.WriteLog("关机前 OEE_DT人工停止复位自动errorCode发送失败,超时无反馈:" + ",OEELog");
                                            //    _homefrm.AppendRichText(Global.ed[Global.Error_Stopnum + 1].errorCode + ",触发时间=" + Global.ed[Global.Error_Stopnum + 1].start_time + ",运行状态:" + Global.ed[Global.Error_Stopnum + 1].errorStatus + ",故障描述:" + Global.ed[Global.Error_Stopnum + 1].errorinfo + ",自动发送失败", "rtx_DownTimeMsg");
                                            //    _homefrm.AppendRichText("网络异常,超时无反馈", "rtx_DownTimeMsg");
                                            //    string s = "insert into OEE_DTSendNG([DateTime],[Product],[GUID],[EMT],[PoorNum],[TotalNum],[Status],[ErrorCode],[ModuleCode],[ClientPcName],[MAC],[IP],[EventTime],[Moduleinfo],[errorinfo])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," + "'" + "OEE_DT" + "'" + "," + "'" + guid + "'" + "," + "'" + Global.inidata.productconfig.EMT + "'" + "," + "'" + "0" + "'" + "," + "'" + "0" + "'" + "," + "'" + Global.ed[Global.Error_Stopnum + 1].errorStatus + "'" + "," + "'" + Global.ed[Global.Error_Stopnum + 1].errorCode + "'" + ","
                                            //           + "'" + Global.ed[Global.Error_Stopnum + 1].ModuleCode + "'" + "," + "'" + ClientPcName + "'" + "," + "'" + Mac + "'" + "," + "'" + IP + "'" + "," + "'" + Global.ed[Global.Error_Stopnum + 1].start_time + "'" + "," + "'" + Global.ed[Global.Error_Stopnum + 1].Moduleinfo + "'" + "," + "'" + Global.ed[Global.Error_Stopnum + 1].errorinfo + "'" + ")";
                                            //    int r = SQL.ExecuteUpdate(s);
                                            //    Log.WriteLog(string.Format("插入了{0}行OEE_DownTime缓存数据", r) + ",OEELog");
                                            //}
                                            string InsertOEEStr = "insert into OEE_DT([DateTime],[ErrorCode],[EventTime],[ModuleCode],[RunStatus],[ErrorInfo],[TimeSpan])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "'" + "," + "'" + Global.ed[Global.Error_Stopnum + 1].errorCode + "'" + "," + "'" + Global.ed[Global.Error_Stopnum + 1].start_time + "'" + ","
                                           + "'" + "" + "'" + "," + "'" + Global.ed[Global.Error_Stopnum + 1].errorStatus + "'" + "," + "'" + Global.ed[Global.Error_Stopnum + 1].errorinfo + "'" + "," + "'" + ts + "'" + ")";
                                            SQL.ExecuteUpdate(InsertOEEStr);
                                            Log.WriteLog(DateTime.Now.ToString("HH:mm:ss") + "," + Global.ed[Global.Error_Stopnum + 1].errorCode + "," + Global.ed[Global.Error_Stopnum + 1].start_time + "," + "" + "," + "自动发送成功" + "," + Global.ed[Global.Error_Stopnum + 1].errorStatus + "," + Global.ed[Global.Error_Stopnum + 1].errorinfo + "," + ts, System.AppDomain.CurrentDomain.BaseDirectory + "\\系统配置\\System_ini\\");
                                        }
                                        else//机台处于其它异常状态中
                                        {
                                            Log.WriteLog("PLC人工停止ErrorCode异常" + Global.Error_Stopnum + ",OEELog");
                                        }
                                    }
                                    Log.WriteLog(Global.ed[Global.Error_Stopnum + 1].Moduleinfo + "_" + Global.ed[Global.Error_Stopnum + 1].errorinfo + "：结束计时 " + Global.ed[Global.Error_Stopnum + 1].stop_time + ",OEELog");
                                    Global.ed[Global.Error_Stopnum + 1].start_time = null;
                                    Global.ed[Global.Error_Stopnum + 1].stop_time = null;
                                }
                            }
                        }
                        else//当前状态为（空跑）状态
                        {
                            DateTime t1 = Convert.ToDateTime(Global.ed[313].start_time);
                            DateTime t2 = Convert.ToDateTime(StopTime);
                            string ts = (t2 - t1).TotalMinutes.ToString("0.00");
                            string InsertStr5 = "insert into OEE_DT([DateTime],[ErrorCode],[EventTime],[ModuleCode],[RunStatus],[ErrorInfo],[TimeSpan])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "'" + "," + "'" + Global.ed[313].errorCode + "'" + "," + "'" + Global.ed[313].start_time + "'" + ","
                               + "'" + Global.ed[313].ModuleCode + "'" + "," + "'" + Global.ed[313].errorStatus + "'" + "," + "'" + Global.ed[313].errorinfo + "'" + "," + "'" + ts + "'" + ")";
                            SQL.ExecuteUpdate(InsertStr5);
                            Log.WriteCSV(DateTime.Now.ToString("HH:mm:ss") + "," + Global.ed[313].errorCode + "," + Global.ed[313].start_time + "," + Global.ed[313].ModuleCode + "," + "自动发送成功" + "," + Global.ed[313].errorStatus + "," + Global.ed[313].errorinfo + "," + ts, System.AppDomain.CurrentDomain.BaseDirectory + "\\系统配置\\System_ini\\");
                        }
                    }
                    else//当前状态为（首件）状态
                    {
                        DateTime t1 = Convert.ToDateTime(Global.errordata.start_time);
                        DateTime t2 = Convert.ToDateTime(StopTime);
                        string ts = (t2 - t1).TotalMinutes.ToString("0.00");
                        string InsertOEEStr6 = "insert into OEE_DT([DateTime],[ErrorCode],[EventTime],[ModuleCode],[RunStatus],[ErrorInfo],[TimeSpan])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "'" + "," + "'" + Global.errordata.errorCode + "'" + "," + "'" + Global.errordata.start_time + "'" + ","
                                             + "'" + "" + "'" + "," + "'" + Global.errordata.errorStatus + "'" + "," + "'" + Global.errordata.errorinfo + "'" + "," + "'" + ts + "'" + ")";
                        SQL.ExecuteUpdate(InsertOEEStr6);
                        Log.WriteCSV(DateTime.Now.ToString("HH:mm:ss") + "," + Global.errordata.errorCode + "," + Global.errordata.start_time + "," + "手动发送成功" + "," + Global.errordata.errorStatus + "," + Global.errordata.errorinfo + "," + ts, System.AppDomain.CurrentDomain.BaseDirectory + "\\系统配置\\System_ini\\");
                    }

                    string OEEDownTime = "";
                    Guid guid6 = Guid.NewGuid();
                    string EventTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    OEEDownTime = string.Format("{{\"guid\":\"{0}\",\"EMT\":\"{1}\",\"PoorNum\":\"{2}\",\"TotalNum\":\"{3}\",  \"Status\":\"{4}\",\"ErrorCode\":\"{5}\",\"EventTime\":\"{6}\",\"ModuleCode\":\"{7}\",\"ClientPcName\":\"{8}\",\"MAC\":\"{9}\",\"IP\":\"{10}\"}}", guid6, Global.inidata.productconfig.EMT, "0", "0", "6", "10010001", EventTime, "", ClientPcName, Mac, IP);
                    Log.WriteLog("OEE_DT:" + OEEDownTime + ",OEELog");
                    string InsertStr = "insert into OEE_TraceDT([DateTime],[Status],[ErrorCode],[EventTime],[ModuleCode])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," + "'" + "6" + "'" + "," + "'" + "10010001" + "'" + ","
                                          + "'" + EventTime + "'" + "," + "'" + "" + "'" + ")";
                    SQL.ExecuteUpdate(InsertStr);
                    //Global._mqttClient.Publish(sendTopic, Encoding.UTF8.GetBytes(OEEDownTime), MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE, false);
                    //bool rst = SendMqttResult(guid6);
                    //if (rst)
                    //{
                    //    if (Global.respond[guid6].Result == "OK")
                    //    {
                    //        Log.WriteLog("OEE_DT机台关机自动发送error_code成功" + ",OEELog");
                    //        Log.WriteLog(DateTime.Now.ToString("HH:mm:ss") + "," + "10010001" + "," + EventTime + "," + "手动发送成功" + "," + "6" + "," + "软件关闭", System.AppDomain.CurrentDomain.BaseDirectory + "\\系统配置\\System_ini\\");
                    //        _homefrm.AppendRichText("10010001" + ",触发时间=" + EventTime + ",运行状态:" + "6" + ",故障描述:" + "软件关闭" + ",手动发送成功", "rtx_DownTimeMsg");
                    //        Global.respond.TryRemove(guid6, out Global.Respond);
                    //    }
                    //    else
                    //    {
                    //        Log.WriteLog("OEE_DT软件关闭发送失败" + ",OEELog");
                    //        Log.WriteLog(DateTime.Now.ToString("HH:mm:ss") + "," + "10010001" + "," + EventTime + "," + "手动发送失败" + "," + "6" + "," + "软件关闭", System.AppDomain.CurrentDomain.BaseDirectory + "\\系统配置\\System_ini\\");
                    //        Log.WriteLog(Global.respond[guid6].GUID.ToString() + "," + Global.respond[guid6].Result + "," + Global.respond[guid6].ErrorCode, System.AppDomain.CurrentDomain.BaseDirectory + "\\MQTT返回NG\\");
                    //        _homefrm.AppendRichText("10010001" + ",触发时间=" + EventTime + ",运行状态:" + "6" + ",故障描述:" + "软件关闭" + ",手动发送失败", "rtx_DownTimeMsg");
                    //        _homefrm.AppendRichText("失败原因:" + Global.respond[guid6].ErrorCode + Global.respond[guid6].Result, "rtx_DownTimeMsg");
                    //        string s = "insert into OEE_DTSendNG([DateTime],[Product],[GUID],[EMT],[PoorNum],[TotalNum],[Status],[ErrorCode],[ModuleCode],[ClientPcName],[MAC],[IP],[EventTime],[Moduleinfo],[errorinfo])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," + "'" + "OEE_DT" + "'" + "," + "'" + guid6 + "'" + "," + "'" + Global.inidata.productconfig.EMT + "'" + "," + "'" + "0" + "'" + "," + "'" + "0" + "'" + "," + "'" + "6" + "'" + "," + "'" + "10010001" + "'" + ","
                    //           + "'" + "" + "'" + "," + "'" + ClientPcName + "'" + "," + "'" + Mac + "'" + "," + "'" + IP + "'" + "," + "'" + EventTime + "'" + "," + "'" + "" + "'" + "," + "'" + "软件关闭" + "'" + ")";
                    //        int r = SQL.ExecuteUpdate(s);
                    //        Global.respond.TryRemove(guid6, out Global.Respond);
                    //    }
                    //}
                    //else
                    //{
                    //    Log.WriteLog("OEE_DT机台关机自动发送error_code失败" + ",OEELog");
                    //    Log.WriteLog(DateTime.Now.ToString("HH:mm:ss") + "," + "10010001" + "," + EventTime + "," + "手动发送失败" + "," + "6" + "," + "软件关闭", System.AppDomain.CurrentDomain.BaseDirectory + "\\系统配置\\System_ini\\");
                    //    _homefrm.AppendRichText("10010001" + ",触发时间=" + EventTime + ",运行状态:" + "6" + ",故障描述:" + "软件关闭" + ",手动发送失败", "rtx_DoTimeMsg");
                    //    _homefrm.AppendRichText("网络异常,超时无反馈", "rtx_DownTimeMsg");
                    //    string s = "insert into OEE_DTSendNG([DateTime],[Product],[GUID],[EMT],[PoorNum],[TotalNum],[Status],[ErrorCode],[ModuleCode],[ClientPcName],[MAC],[IP],[EventTime],[Moduleinfo],[errorinfo])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," + "'" + "OEE_DT" + "'" + "," + "'" + guid6 + "'" + "," + "'" + Global.inidata.productconfig.EMT + "'" + "," + "'" + "0" + "'" + "," + "'" + "0" + "'" + "," + "'" + "6" + "'" + "," + "'" + "10010001" + "'" + ","
                    //           + "'" + "" + "'" + "," + "'" + ClientPcName + "'" + "," + "'" + Mac + "'" + "," + "'" + IP + "'" + "," + "'" + EventTime + "'" + "," + "'" + "" + "'" + "," + "'" + "软件关闭" + "'" + ")";
                    //    int r = SQL.ExecuteUpdate(s);
                    //    Log.WriteLog(string.Format("插入了{0}行OEE_DownTime缓存数据", r));
                    //}
                    string InsertOEEStr3 = "insert into OEE_MCOff([DateTime],[Name])" + " " + "values(" + "'" + EventTime + "'" + "," + "'" + "软件关闭" + "'" + ")";
                    SQL.ExecuteUpdate(InsertOEEStr3);//插入关机时间
                    if (Global.inidata.productconfig.OpenCard == "true")
                    {
                        _userloginfrm.btn_UserLogin_Click(null, null); //关机前权限登出
                    }
                    //maindis.Shutdown();
                    this.FormClosing -= new FormClosingEventHandler(this.MainFrm_FormClosing);
                    System.Diagnostics.Process.GetCurrentProcess().Kill();
                    this.Dispose();
                }
                else
                {
                    e.Cancel = true;
                }


            }
            catch (Exception ex)
            {
                Log.WriteLog(ex.ToString());
                // maindis.Shutdown();
                this.FormClosing -= new FormClosingEventHandler(this.MainFrm_FormClosing);
                System.Diagnostics.Process.GetCurrentProcess().Kill();
                this.Dispose();
            }
            finally
            {
                Process.GetCurrentProcess().Kill();
            }
        }

        private byte[] ConvertPrecitecData(string Head, int Length, string sn)//发送precitec数据
        {
            //普雷斯特数据长度格式转换
            string binaryNum = Convert.ToString(Length, 16);
            string result = string.Empty;
            if (binaryNum.Length < 8)
            {
                int length = 8 - binaryNum.Length;
                for (int i = 0; i < length; i++)
                {
                    result += "0";
                }
                result += binaryNum;
            }
            //普雷斯特数据长度格式高低位转换
            string SNlength = result.Substring(6, 2) + result.Substring(4, 2) + result.Substring(2, 2) + result.Substring(0, 2);
            //普雷斯特sn格式转换ASCII码
            byte[] ba = System.Text.ASCIIEncoding.Default.GetBytes(sn);
            StringBuilder sb = new StringBuilder();
            foreach (byte b in ba)
            {
                sb.Append(b.ToString("x"));
            }
            //所有数据格式转换16进制
            string SendData = Head + SNlength + sb.ToString();
            Log.WriteLog(string.Format("发送普雷斯特SN:{0}", SendData));
            byte[] buffer = new byte[SendData.Length / 2];
            for (int i = 0; i < SendData.Length; i += 2)
                buffer[i / 2] = (byte)Convert.ToByte(SendData.Substring(i, 2), 16);
            return buffer;

        }

        public static byte[] HexStringToByteArray(string s)//字符串转化16进制
        {
            s = s.Replace(" ", "");
            byte[] buffer = new byte[s.Length / 2];
            for (int i = 0; i < s.Length; i += 2)
                buffer[i / 2] = (byte)Convert.ToByte(s.Substring(i, 2), 16);
            return buffer;
        }

        public static string ByteArrayToHexSring(byte[] data)//16进制转化字符串
        {
            StringBuilder sb = new StringBuilder(data.Length * 3);
            foreach (byte b in data)
            {
                sb.Append(Convert.ToString(b, 16).PadLeft(2, '0'));
            }
            return sb.ToString().ToUpper();

        }

        public static float HexStringToFloat(string s)//字符串转化float
        {
            UInt32 x = Convert.ToUInt32(s, 16);//字符串转16进制32位无符号整数
            float fy = BitConverter.ToSingle(BitConverter.GetBytes(x), 0);//IEEE754 字节转换float
            return fy;
        }

        private string ASCIITo16(String str) //ASCII字符串转16进制数
        {
            byte[] ba = System.Text.ASCIIEncoding.Default.GetBytes(str);
            StringBuilder sb = new StringBuilder();
            foreach (byte b in ba)
            {
                sb.Append(b.ToString("x"));
            }
            return sb.ToString();
        }

        private void StopStatus()//OEE 处于人工停止状态
        {
            IP = GetIp();
            Mac = GetOEEMac();
            Global.STOP = true;
            Global.j = ReadStatus[0];
            Global.Error_Stopnum = ReadStatus[3];
            Log.WriteCSV(DateTime.Now.ToString() + "," + ReadStatus[0] + "," + ReadStatus[3], System.AppDomain.CurrentDomain.BaseDirectory + "\\PLC_DT记录\\");
            Global.ed[Global.Error_Stopnum + 1].start_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            Log.WriteLog(Global.ed[Global.Error_Stopnum + 1].Moduleinfo + "_" + Global.ed[Global.Error_Stopnum + 1].errorinfo + "：开始计时 " + Global.ed[Global.Error_Stopnum + 1].start_time);
            if (Global.Error_Stopnum == 9 || Global.Error_Stopnum == 10 || Global.Error_Stopnum == 11 || Global.Error_Stopnum == 12 || Global.Error_Stopnum == 53)//机台打开安全门
            {
                Global.PLC_Client.WritePLC_D(10020, new short[] { 2 });//未手动选择打开安全门原因，机台不能运行                
            }
            Global.PLC_Client.WritePLC_D(10020, new short[] { 2 });//未手动选择打开安全门原因，机台不能运行   
            string InsertOEEStr = "insert into OEE_StartTime([Status],[ErrorCode],[EventTime],[ModuleCode],[Name])" + " " + "values(" + "'" + Global.ed[Global.Error_Stopnum + 1].errorStatus + "'" + "," + "'" + Global.ed[Global.Error_Stopnum + 1].errorCode + "'" + "," + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "'" + "," + "'" + Global.ed[Global.Error_Stopnum + 1].ModuleCode + "'" + "," + "'" + Global.ed[Global.Error_Stopnum + 1].errorinfo + "'" + ")";
            SQL.ExecuteUpdate(InsertOEEStr);//插入人工停止复位开始时间
        }
        private void ErrorStatus()//OEE 处于异常状态
        {
            string sendTopic = Global.inidata.productconfig.EMT + "/upload/downtime";
            Guid guid = Guid.NewGuid();
            ClientPcName = Dns.GetHostName();
            IP = GetIp();
            Mac = GetOEEMac();
            Global.j = ReadStatus[0];
            Global.Error_num = ReadStatus[1];
            Log.WriteCSV(DateTime.Now.ToString() + "," + ReadStatus[0] + "," + ReadStatus[1], System.AppDomain.CurrentDomain.BaseDirectory + "\\PLC_DT记录\\");
            Global.ed[Global.Error_num + 1].start_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            Log.WriteLog(Global.ed[Global.Error_num + 1].Moduleinfo + "_" + Global.ed[Global.Error_num + 1].errorinfo + "：开始计时 " + Global.ed[Global.Error_num + 1].start_time + ",OEELog");
            if (Global.Error_num == 9 || Global.Error_num == 10 || Global.Error_num == 11 || Global.Error_num == 12 || Global.Error_num == 53)//机台打开安全门
            {
                Global.PLC_Client.WritePLC_D(10020, new short[] { 2 });//未手动选择打开安全门原因，机台不能运行
            }
            else
            {
                string OEE_DT = "";
                OEE_DT = string.Format("{{\"guid\":\"{0}\",\"EMT\":\"{1}\",\"PoorNum\":\"{2}\",\"TotalNum\":\"{3}\",  \"Status\":\"{4}\",\"ErrorCode\":\"{5}\",\"EventTime\":\"{6}\",\"ModuleCode\":\"{7}\",\"ClientPcName\":\"{8}\",\"MAC\":\"{9}\",\"IP\":\"{10}\"}}", guid, Global.inidata.productconfig.EMT, "0", "0", Global.ed[Global.Error_num + 1].errorStatus, Global.ed[Global.Error_num + 1].errorCode, Global.ed[Global.Error_num + 1].start_time, Global.ed[Global.Error_num + 1].ModuleCode, ClientPcName, Mac, IP);
                Log.WriteLog("OEE_DT:" + OEE_DT + ",OEELog");
                string InsertStr = "insert into OEE_TraceDT([DateTime],[Status],[ErrorCode],[EventTime],[ModuleCode])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," + "'" + Global.ed[Global.Error_num + 1].errorStatus + "'" + "," + "'" + Global.ed[Global.Error_num + 1].errorCode + "'" + ","
                         + "'" + Global.ed[Global.Error_num + 1].start_time + "'" + "," + "'" + Global.ed[Global.Error_num + 1].ModuleCode + "'" + ")";
                SQL.ExecuteUpdate(InsertStr);
                //Global._mqttClient.Publish(sendTopic, Encoding.UTF8.GetBytes(OEE_DT), MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE, false);
                //bool rst = SendMqttResult(guid);
                //if (rst)
                //{
                //    if (Global.respond[guid].Result == "OK")
                //    {
                //        Global.ConnectOEEFlag = true;
                //        Log.WriteLog("OEE_DT自动errorCode发送成功" + ",OEELog");
                //        _homefrm.AppendRichText(Global.ed[Global.Error_num + 1].ModuleCode + "," + Global.ed[Global.Error_num + 1].errorCode + ",触发时间=" + Global.ed[Global.Error_num + 1].start_time + ",运行状态:" + Global.ed[Global.Error_num + 1].errorStatus + ",故障描述:" + Global.ed[Global.Error_num + 1].errorinfo + ",自动发送成功", "rtx_DownTimeMsg");
                //        Global.respond.TryRemove(guid, out Global.Respond);
                //    }
                //    else
                //    {
                //        Global.ConnectOEEFlag = false;
                //        Log.WriteLog("OEE_DT自动errorCode发送失败" + ",OEELog");
                //        _homefrm.AppendRichText(Global.ed[Global.Error_num + 1].ModuleCode + "," + Global.ed[Global.Error_num + 1].errorCode + ",触发时间=" + Global.ed[Global.Error_num + 1].start_time + ",运行状态:" + Global.ed[Global.Error_num + 1].errorStatus + ",故障描述:" + Global.ed[Global.Error_num + 1].errorinfo + ",自动发送失败", "rtx_DownTimeMsg");
                //        _homefrm.AppendRichText("失败原因:" + Global.respond[guid].ErrorCode + Global.respond[guid].Result, "rtx_DownTimeMsg");
                //        Log.WriteLog(Global.respond[guid].GUID.ToString() + "," + Global.respond[guid].Result + "," + Global.respond[guid].ErrorCode, System.AppDomain.CurrentDomain.BaseDirectory + "\\MQTT返回NG\\");
                //        Global.respond.TryRemove(guid, out Global.Respond);
                //    }
                //}
                //else
                //{
                //    Global.ConnectOEEFlag = false;
                //    Log.WriteLog("OEE_DT自动自动errorCode发送失败,超时无反馈:" + ",OEELog");
                //    _homefrm.AppendRichText(Global.ed[Global.Error_num + 1].ModuleCode + "," + Global.ed[Global.Error_num + 1].errorCode + ",触发时间=" + Global.ed[Global.Error_num + 1].start_time + ",运行状态:" + Global.ed[Global.Error_num + 1].errorStatus + ",故障描述:" + Global.ed[Global.Error_num + 1].errorinfo + ",自动发送失败", "rtx_DownTimeMsg");
                //    _homefrm.AppendRichText("网络异常,超时无反馈", "rtx_DownTimeMsg");
                //    string s = "insert into OEE_DTSendNG([DateTime],[Product],[GUID],[EMT],[PoorNum],[TotalNum],[Status],[ErrorCode],[ModuleCode],[ClientPcName],[MAC],[IP],[EventTime],[Moduleinfo],[errorinfo])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," + "'" + "OEE_DT" + "'" + "," + "'" + guid + "'" + "," + "'" + Global.inidata.productconfig.EMT + "'" + "," + "'" + "0" + "'" + "," + "'" + "0" + "'" + "," + "'" + Global.ed[Global.Error_num + 1].errorStatus + "'" + "," + "'" + Global.ed[Global.Error_num + 1].errorCode + "'" + ","
                //           + "'" + Global.ed[Global.Error_num + 1].ModuleCode + "'" + "," + "'" + ClientPcName + "'" + "," + "'" + Mac + "'" + "," + "'" + IP + "'" + "," + "'" + Global.ed[Global.Error_num + 1].start_time + "'" + "," + "'" + Global.ed[Global.Error_num + 1].Moduleinfo + "'" + "," + "'" + Global.ed[Global.Error_num + 1].errorinfo + "'" + ")";
                //    int r = SQL.ExecuteUpdate(s);
                //    Log.WriteLog(string.Format("插入了{0}行OEE_DownTime缓存数据", r) + ",OEELog");
                //}
                _manualfrm.labelcolor(Color.Transparent, "未选择", "LB_ManualSelect");
                string InsertOEEStr = "insert into OEE_StartTime([Status],[ErrorCode],[EventTime],[ModuleCode],[Name])" + " " + "values(" + "'" + Global.ed[Global.Error_num + 1].errorStatus + "'" + "," + "'" + Global.ed[Global.Error_num + 1].errorCode + "'" + "," + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "'" + "," + "'" + Global.ed[Global.Error_num + 1].ModuleCode + "'" + "," + "'" + Global.ed[Global.Error_num + 1].errorinfo + "'" + ")";
                SQL.ExecuteUpdate(InsertOEEStr);//插入异常开始时间
            }
        }
        private void RunStatus()//OEE 处于运行状态
        {
            Global.SelectManualErrorCode = false;//结束手动选择ErrorCode状态
            string sendTopic = Global.inidata.productconfig.EMT + "/upload/downtime";
            Guid guid = Guid.NewGuid();
            ClientPcName = Dns.GetHostName();
            IP = GetIp();
            Mac = GetOEEMac();
            Global.j = ReadStatus[0];
            Log.WriteCSV(DateTime.Now.ToString() + "," + ReadStatus[0] + "," + ReadStatus[0] + "," + Global.ed[ReadStatus[0]].errorinfo, System.AppDomain.CurrentDomain.BaseDirectory + "\\PLC_DT记录\\");
            Global.ed[Global.j].start_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            Log.WriteLog(Global.ed[Global.j].Moduleinfo + "_" + Global.ed[Global.j].errorinfo + "：开始计时 " + Global.ed[Global.j].start_time + ",OEELog");
            string OEE_DT = "";
            OEE_DT = string.Format("{{\"guid\":\"{0}\",\"EMT\":\"{1}\",\"PoorNum\":\"{2}\",\"TotalNum\":\"{3}\",  \"Status\":\"{4}\",\"ErrorCode\":\"{5}\",\"EventTime\":\"{6}\",\"ModuleCode\":\"{7}\",\"ClientPcName\":\"{8}\",\"MAC\":\"{9}\",\"IP\":\"{10}\"}}", guid, Global.inidata.productconfig.EMT, "0", "0", Global.ed[Global.j].errorStatus, Global.ed[Global.j].errorCode, Global.ed[Global.j].start_time, Global.ed[Global.j].ModuleCode, ClientPcName, Mac, IP);
            Log.WriteLog("OEE_DT:" + OEE_DT + ",OEELog");
            string InsertStr = "insert into OEE_TraceDT([DateTime],[Status],[ErrorCode],[EventTime],[ModuleCode])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," + "'" + Global.ed[Global.j].errorStatus + "'" + "," + "'" + "00000000" + "'" + ","
                         + "'" + Global.ed[Global.j].start_time + "'" + "," + "'" + Global.ed[Global.j].ModuleCode + "'" + ")";
            SQL.ExecuteUpdate(InsertStr);
            //Global._mqttClient.Publish(sendTopic, Encoding.UTF8.GetBytes(OEE_DT), MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE, false);
            //bool rst = SendMqttResult(guid);
            //if (rst)
            //{
            //    if (Global.respond[guid].Result == "OK")
            //    {
            //        Global.ConnectOEEFlag = true;
            //        Log.WriteLog("OEE_DT自动errorCode发送成功" + ",OEELog");
            //        _homefrm.AppendRichText(Global.ed[Global.j].ModuleCode + "," + "Run" + ",触发时间=" + Global.ed[Global.j].start_time + ",运行状态:" + Global.ed[Global.j].errorStatus + ",故障描述:" + Global.ed[Global.j].errorinfo + ",自动发送成功", "rtx_DownTimeMsg");
            //        Global.respond.TryRemove(guid, out Global.Respond);
            //    }
            //    else
            //    {
            //        Global.ConnectOEEFlag = false;
            //        Log.WriteLog("OEE_DT自动errorCode发送失败" + ",OEELog");
            //        _homefrm.AppendRichText(Global.ed[Global.j].ModuleCode + "," + "Run" + ",触发时间=" + Global.ed[Global.j].start_time + ",运行状态:" + Global.ed[Global.j].errorStatus + ",故障描述:" + Global.ed[Global.j].errorinfo + ",自动发送失败", "rtx_DownTimeMsg");
            //        _homefrm.AppendRichText("失败原因:" + Global.respond[guid].ErrorCode + Global.respond[guid].Result, "rtx_DownTimeMsg");
            //        Log.WriteLog(Global.respond[guid].GUID.ToString() + "," + Global.respond[guid].Result + "," + Global.respond[guid].ErrorCode, System.AppDomain.CurrentDomain.BaseDirectory + "\\MQTT返回NG\\");
            //        Global.respond.TryRemove(guid, out Global.Respond);
            //    }
            //}
            //else
            //{
            //    Global.ConnectOEEFlag = false;
            //    Log.WriteLog("OEE_DT自动errorCode发送失败,超时无反馈:" + ",OEELog");
            //    _homefrm.AppendRichText(Global.ed[Global.j].ModuleCode + "," + "Run" + ",触发时间=" + Global.ed[Global.j].start_time + ",运行状态:" + Global.ed[Global.j].errorStatus + ",故障描述:" + Global.ed[Global.j].errorinfo + ",自动发送失败", "rtx_DownTimeMsg");
            //    _homefrm.AppendRichText("网络异常,超时无反馈", "rtx_DownTimeMsg");
            //    string s = "insert into OEE_DTSendNG([DateTime],[Product],[GUID],[EMT],[PoorNum],[TotalNum],[Status],[ErrorCode],[ModuleCode],[ClientPcName],[MAC],[IP],[EventTime],[Moduleinfo],[errorinfo])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," + "'" + "OEE_DT" + "'" + "," + "'" + guid + "'" + "," + "'" + Global.inidata.productconfig.EMT + "'" + "," + "'" + "0" + "'" + "," + "'" + "0" + "'" + "," + "'" + Global.ed[Global.j].errorStatus + "'" + "," + "'" + Global.ed[Global.j].errorCode + "'" + ","
            //           + "'" + Global.ed[Global.j].ModuleCode + "'" + "," + "'" + ClientPcName + "'" + "," + "'" + Mac + "'" + "," + "'" + IP + "'" + "," + "'" + Global.ed[Global.j].start_time + "'" + "," + "'" + Global.ed[Global.j].Moduleinfo + "'" + "," + "'" + Global.ed[Global.j].errorinfo + "'" + ")";
            //    int r = SQL.ExecuteUpdate(s);
            //    Log.WriteLog(string.Format("插入了{0}行OEE_DownTime缓存数据", r) + ",OEELog");
            //}
            _manualfrm.labelcolor(Color.Transparent, "未选择", "LB_ManualSelect");
            string InsertOEEStr = "insert into OEE_StartTime([Status],[ErrorCode],[EventTime],[ModuleCode],[Name])" + " " + "values(" + "'" + Global.ed[Global.j].errorStatus + "'" + "," + "'" + Global.ed[Global.j].errorCode + "'" + "," + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "'" + "," + "'" + Global.ed[Global.j].ModuleCode + "'" + "," + "'" + Global.ed[Global.j].errorinfo + "'" + ")";
            SQL.ExecuteUpdate(InsertOEEStr);//插入运行开始时间
        }
        private void PendingStatus()//OEE 处于待料状态
        {
            string sendTopic = Global.inidata.productconfig.EMT + "/upload/downtime";
            Guid guid = Guid.NewGuid();
            ClientPcName = Dns.GetHostName();
            IP = GetIp();
            Mac = GetOEEMac();
            Global.j = ReadStatus[0];
            Global.Error_PendingNum = ReadStatus[2];//待料细节字
            Log.WriteCSV(DateTime.Now.ToString() + "," + ReadStatus[0] + "," + ReadStatus[2], System.AppDomain.CurrentDomain.BaseDirectory + "\\PLC_DT记录\\");
            Global.ed[Global.Error_PendingNum + 1].start_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            Log.WriteLog(Global.ed[Global.Error_PendingNum + 1].Moduleinfo + "_" + Global.ed[Global.Error_PendingNum + 1].errorinfo + "：开始计时 " + Global.ed[Global.Error_PendingNum + 1].start_time + ",OEELog");
            string OEE_DT = "";
            OEE_DT = string.Format("{{\"guid\":\"{0}\",\"EMT\":\"{1}\",\"PoorNum\":\"{2}\",\"TotalNum\":\"{3}\",  \"Status\":\"{4}\",\"ErrorCode\":\"{5}\",\"EventTime\":\"{6}\",\"ModuleCode\":\"{7}\",\"ClientPcName\":\"{8}\",\"MAC\":\"{9}\",\"IP\":\"{10}\"}}", guid, Global.inidata.productconfig.EMT, "0", "0", Global.ed[Global.Error_PendingNum + 1].errorStatus, Global.ed[Global.Error_PendingNum + 1].errorCode, Global.ed[Global.Error_PendingNum + 1].start_time, Global.ed[Global.Error_PendingNum + 1].ModuleCode, ClientPcName, Mac, IP);
            Log.WriteLog("OEE_DT:" + OEE_DT + ",OEELog");
            string InsertStr = "insert into OEE_TraceDT([DateTime],[Status],[ErrorCode],[EventTime],[ModuleCode])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," + "'" + Global.ed[Global.Error_PendingNum + 1].errorStatus + "'" + "," + "'" + Global.ed[Global.Error_PendingNum + 1].errorCode + "'" + ","
                    + "'" + Global.ed[Global.Error_PendingNum + 1].start_time + "'" + "," + "'" + Global.ed[Global.Error_PendingNum + 1].ModuleCode + "'" + ")";
            SQL.ExecuteUpdate(InsertStr);
            //Global._mqttClient.Publish(sendTopic, Encoding.UTF8.GetBytes(OEE_DT), MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE, false);
            //bool rst = SendMqttResult(guid);
            //if (rst)
            //{
            //    if (Global.respond[guid].Result == "OK")
            //    {
            //        Global.ConnectOEEFlag = true;
            //        _homefrm.AppendRichText(Global.ed[Global.Error_PendingNum + 1].ModuleCode + "," + Global.ed[Global.Error_PendingNum + 1].errorCode + ",触发时间=" + Global.ed[Global.Error_PendingNum + 1].start_time + ",运行状态:" + Global.ed[Global.Error_PendingNum + 1].errorStatus + ",故障描述:" + Global.ed[Global.Error_PendingNum + 1].errorinfo + ",自动发送成功", "rtx_DownTimeMsg");
            //        Log.WriteLog("OEE_DT自动errorCode发送成功" + ",OEELog");
            //        Global.respond.TryRemove(guid, out Global.Respond);
            //    }
            //    else
            //    {
            //        Global.ConnectOEEFlag = false;
            //        _homefrm.AppendRichText(Global.ed[Global.Error_PendingNum + 1].ModuleCode + "," + Global.ed[Global.Error_PendingNum + 1].errorCode + ",触发时间=" + Global.ed[Global.Error_PendingNum + 1].start_time + ",运行状态:" + Global.ed[Global.Error_PendingNum + 1].errorStatus + ",故障描述:" + Global.ed[Global.Error_PendingNum + 1].errorinfo + ",自动发送失败", "rtx_DownTimeMsg");
            //        _homefrm.AppendRichText("失败原因:" + Global.respond[guid].ErrorCode + Global.respond[guid].Result, "rtx_DownTimeMsg");
            //        Log.WriteLog("OEE_DT自动errorCode发送失败" + ",OEELog");
            //        Log.WriteLog(Global.respond[guid].GUID.ToString() + "," + Global.respond[guid].Result + "," + Global.respond[guid].ErrorCode, System.AppDomain.CurrentDomain.BaseDirectory + "\\MQTT返回NG\\");
            //        Global.respond.TryRemove(guid, out Global.Respond);
            //    }
            //}
            //else
            //{
            //    Global.ConnectOEEFlag = false;
            //    Log.WriteLog("OEE_DT自动errorCode发送失败,超时无反馈:" + ",OEELog");
            //    _homefrm.AppendRichText(Global.ed[Global.Error_PendingNum + 1].ModuleCode + "," + Global.ed[Global.Error_PendingNum + 1].errorCode + ",触发时间=" + Global.ed[Global.Error_PendingNum + 1].start_time + ",运行状态:" + Global.ed[Global.Error_PendingNum + 1].errorStatus + ",故障描述:" + Global.ed[Global.Error_PendingNum + 1].errorinfo + ",自动发送失败", "rtx_DownTimeMsg");
            //    _homefrm.AppendRichText("网络异常,超时无反馈", "rtx_DownTimeMsg");
            //    string s = "insert into OEE_DTSendNG([DateTime],[Product],[GUID],[EMT],[PoorNum],[TotalNum],[Status],[ErrorCode],[ModuleCode],[ClientPcName],[MAC],[IP],[EventTime],[Moduleinfo],[errorinfo])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," + "'" + "OEE_DT" + "'" + "," + "'" + guid + "'" + "," + "'" + Global.inidata.productconfig.EMT + "'" + "," + "'" + "0" + "'" + "," + "'" + "0" + "'" + "," + "'" + Global.ed[Global.Error_PendingNum + 1].errorStatus + "'" + "," + "'" + Global.ed[Global.Error_PendingNum + 1].errorCode + "'" + ","
            //           + "'" + Global.ed[Global.Error_PendingNum + 1].ModuleCode + "'" + "," + "'" + ClientPcName + "'" + "," + "'" + Mac + "'" + "," + "'" + IP + "'" + "," + "'" + Global.ed[Global.Error_PendingNum + 1].start_time + "'" + "," + "'" + Global.ed[Global.Error_PendingNum + 1].Moduleinfo + "'" + "," + "'" + Global.ed[Global.Error_PendingNum + 1].errorinfo + "'" + ")";
            //    int r = SQL.ExecuteUpdate(s);
            //    Log.WriteLog(string.Format("插入了{0}行OEE_DownTime缓存数据", r) + ",OEELog");
            //}
            _manualfrm.labelcolor(Color.Transparent, "未选择", "LB_ManualSelect");
            string InsertOEEStr = "insert into OEE_StartTime([Status],[ErrorCode],[EventTime],[ModuleCode],[Name])" + " " + "values(" + "'" + Global.ed[Global.Error_PendingNum + 1].errorStatus + "'" + "," + "'" + Global.ed[Global.Error_PendingNum + 1].errorCode + "'" + "," + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "'" + "," + "'" + Global.ed[Global.Error_PendingNum + 1].ModuleCode + "'" + "," + "'" + Global.ed[Global.Error_PendingNum + 1].errorinfo + "'" + ")";
            SQL.ExecuteUpdate(InsertOEEStr);//插入待料开始时间
        }
        private void PendingStatus(int num)//OEE 处于待料状态
        {
            string sendTopic = Global.inidata.productconfig.EMT + "/upload/downtime";
            Guid guid = Guid.NewGuid();
            ClientPcName = Dns.GetHostName();
            IP = GetIp();
            Mac = GetOEEMac();
            Global.j = ReadStatus[0];
            Global.Error_PendingNum = num;//默认传入的值为待料细节字
            Global.ed[Global.Error_PendingNum + 1].start_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            Log.WriteLog(Global.ed[Global.Error_PendingNum + 1].Moduleinfo + "_" + Global.ed[Global.Error_PendingNum + 1].errorinfo + "：开始计时 " + Global.ed[Global.Error_PendingNum + 1].start_time + ",OEELog");
            string OEE_DT = "";
            OEE_DT = string.Format("{{\"guid\":\"{0}\",\"EMT\":\"{1}\",\"PoorNum\":\"{2}\",\"TotalNum\":\"{3}\",  \"Status\":\"{4}\",\"ErrorCode\":\"{5}\",\"EventTime\":\"{6}\",\"ModuleCode\":\"{7}\",\"ClientPcName\":\"{8}\",\"MAC\":\"{9}\",\"IP\":\"{10}\"}}", guid, Global.inidata.productconfig.EMT, "0", "0", Global.ed[Global.Error_PendingNum + 1].errorStatus, Global.ed[Global.Error_PendingNum + 1].errorCode, Global.ed[Global.Error_PendingNum + 1].start_time, Global.ed[Global.Error_PendingNum + 1].ModuleCode, ClientPcName, Mac, IP);
            Log.WriteLog("OEE_DT:" + OEE_DT + ",OEELog");
            string InsertStr = "insert into OEE_TraceDT([DateTime],[Status],[ErrorCode],[EventTime],[ModuleCode])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," + "'" + Global.ed[Global.Error_PendingNum + 1].errorStatus + "'" + "," + "'" + Global.ed[Global.Error_PendingNum + 1].errorCode + "'" + ","
                    + "'" + Global.ed[Global.Error_PendingNum + 1].start_time + "'" + "," + "'" + Global.ed[Global.Error_PendingNum + 1].ModuleCode + "'" + ")";
            SQL.ExecuteUpdate(InsertStr);
            //Global._mqttClient.Publish(sendTopic, Encoding.UTF8.GetBytes(OEE_DT), MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE, false);
            //bool rst = SendMqttResult(guid);
            //if (rst)
            //{
            //    if (Global.respond[guid].Result == "OK")
            //    {
            //        Global.ConnectOEEFlag = true;
            //        _homefrm.AppendRichText(Global.ed[Global.Error_PendingNum + 1].ModuleCode + "," + Global.ed[Global.Error_PendingNum + 1].errorCode + ",触发时间=" + Global.ed[Global.Error_PendingNum + 1].start_time + ",运行状态:" + Global.ed[Global.Error_PendingNum + 1].errorStatus + ",故障描述:" + Global.ed[Global.Error_PendingNum + 1].errorinfo + ",自动发送成功", "rtx_DownTimeMsg");
            //        Log.WriteLog("OEE_DT自动errorCode发送成功" + ",OEELog");
            //        Global.respond.TryRemove(guid, out Global.Respond);
            //    }
            //    else
            //    {
            //        Global.ConnectOEEFlag = false;
            //        _homefrm.AppendRichText(Global.ed[Global.Error_PendingNum + 1].ModuleCode + "," + Global.ed[Global.Error_PendingNum + 1].errorCode + ",触发时间=" + Global.ed[Global.Error_PendingNum + 1].start_time + ",运行状态:" + Global.ed[Global.Error_PendingNum + 1].errorStatus + ",故障描述:" + Global.ed[Global.Error_PendingNum + 1].errorinfo + ",自动发送失败", "rtx_DownTimeMsg");
            //        _homefrm.AppendRichText("失败原因:" + Global.respond[guid].ErrorCode + Global.respond[guid].Result, "rtx_DownTimeMsg");
            //        Log.WriteLog("OEE_DT自动errorCode发送失败" + ",OEELog");
            //        Log.WriteLog(Global.respond[guid].GUID.ToString() + "," + Global.respond[guid].Result + "," + Global.respond[guid].ErrorCode, System.AppDomain.CurrentDomain.BaseDirectory + "\\MQTT返回NG\\");
            //        Global.respond.TryRemove(guid, out Global.Respond);
            //    }
            //}
            //else
            //{
            //    Global.ConnectOEEFlag = false;
            //    Log.WriteLog("OEE_DT自动errorCode发送失败,超时无反馈:" + ",OEELog");
            //    _homefrm.AppendRichText(Global.ed[Global.Error_PendingNum + 1].ModuleCode + "," + Global.ed[Global.Error_PendingNum + 1].errorCode + ",触发时间=" + Global.ed[Global.Error_PendingNum + 1].start_time + ",运行状态:" + Global.ed[Global.Error_PendingNum + 1].errorStatus + ",故障描述:" + Global.ed[Global.Error_PendingNum + 1].errorinfo + ",自动发送失败", "rtx_DownTimeMsg");
            //    _homefrm.AppendRichText("网络异常,超时无反馈", "rtx_DownTimeMsg");
            //    string s = "insert into OEE_DTSendNG([DateTime],[Product],[GUID],[EMT],[PoorNum],[TotalNum],[Status],[ErrorCode],[ModuleCode],[ClientPcName],[MAC],[IP],[EventTime],[Moduleinfo],[errorinfo])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," + "'" + "OEE_DT" + "'" + "," + "'" + guid + "'" + "," + "'" + Global.inidata.productconfig.EMT + "'" + "," + "'" + "0" + "'" + "," + "'" + "0" + "'" + "," + "'" + Global.ed[Global.Error_PendingNum + 1].errorStatus + "'" + "," + "'" + Global.ed[Global.Error_PendingNum + 1].errorCode + "'" + ","
            //           + "'" + Global.ed[Global.Error_PendingNum + 1].ModuleCode + "'" + "," + "'" + ClientPcName + "'" + "," + "'" + Mac + "'" + "," + "'" + IP + "'" + "," + "'" + Global.ed[Global.Error_PendingNum + 1].start_time + "'" + "," + "'" + Global.ed[Global.Error_PendingNum + 1].Moduleinfo + "'" + "," + "'" + Global.ed[Global.Error_PendingNum + 1].errorinfo + "'" + ")";
            //    int r = SQL.ExecuteUpdate(s);
            //    Log.WriteLog(string.Format("插入了{0}行OEE_DownTime缓存数据", r) + ",OEELog");
            //}
            _manualfrm.labelcolor(Color.Transparent, "未选择", "LB_ManualSelect");
            string InsertOEEStr = "insert into OEE_StartTime([Status],[ErrorCode],[EventTime],[ModuleCode],[Name])" + " " + "values(" + "'" + Global.ed[Global.Error_PendingNum + 1].errorStatus + "'" + "," + "'" + Global.ed[Global.Error_PendingNum + 1].errorCode + "'" + "," + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "'" + "," + "'" + Global.ed[Global.Error_PendingNum + 1].ModuleCode + "'" + "," + "'" + Global.ed[Global.Error_PendingNum + 1].errorinfo + "'" + ")";
            SQL.ExecuteUpdate(InsertOEEStr);//插入待料开始时间
        }
        public void SendHSG()//OEE补传HSG状态
        {
            string SelectStr = "select * from OEE_DT where ID =((SELECT MAX(ID )from OEE_DT) -1)";
            DataTable d1 = SQL.ExecuteQuery(SelectStr);
            string sendTopic = Global.inidata.productconfig.EMT + "/upload/downtime";
            Guid guid = Guid.NewGuid();
            ClientPcName = Dns.GetHostName();
            IP = GetIp();
            Mac = GetOEEMac();
            string OEE_DT = "";
            string ts = string.Empty;
            OEE_DT = string.Format("{{\"guid\":\"{0}\",\"EMT\":\"{1}\",\"PoorNum\":\"{2}\",\"TotalNum\":\"{3}\",  \"Status\":\"{4}\",\"ErrorCode\":\"{5}\",\"EventTime\":\"{6}\",\"ModuleCode\":\"{7}\",\"ClientPcName\":\"{8}\",\"MAC\":\"{9}\",\"IP\":\"{10}\"}}", guid, Global.inidata.productconfig.EMT, "0", "0", d1.Rows[0][5].ToString(), d1.Rows[0][2].ToString(), Global.SendHSG_start_time, "", ClientPcName, Mac, IP);
            Log.WriteLog("OEE_DT:补传待料" + OEE_DT + ",OEELog");
            DateTime t1 = Convert.ToDateTime(Global.SendHSG_start_time);
            DateTime t2 = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
            ts = (t2 - t1).TotalMinutes.ToString("0.00");
            //Global._mqttClient.Publish(sendTopic, Encoding.UTF8.GetBytes(OEE_DT), MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE, false);
            //bool rst = SendMqttResult(guid);
            //if (rst)
            //{
            //    if (Global.respond[guid].Result == "OK")
            //    {
            //        Global.ConnectOEEFlag = true;
            //        Log.WriteLog("OEE_DT:补传待料自动errorCode发送成功" + ",OEELog");
            //        _homefrm.AppendRichText("" + "," + d1.Rows[0][2].ToString() + ",触发时间=" + Global.SendHSG_start_time + ",运行状态:" + d1.Rows[0][5].ToString() + ",故障描述:" + d1.Rows[0][6].ToString() + ",自动发送成功", "rtx_DownTimeMsg");
            //        Global.respond.TryRemove(guid, out Global.Respond);
            //    }
            //    else
            //    {
            //        Global.ConnectOEEFlag = false;
            //        _homefrm.AppendRichText("" + "," + d1.Rows[0][2].ToString() + ",触发时间=" + Global.SendHSG_start_time + ",运行状态:" + d1.Rows[0][5].ToString() + ",故障描述:" + d1.Rows[0][6].ToString() + ",自动发送成功", "rtx_DownTimeMsg");
            //        _homefrm.AppendRichText("失败原因:" + Global.respond[guid].ErrorCode + Global.respond[guid].Result, "rtx_DownTimeMsg");
            //        Log.WriteLog("OEE_DT:补传待料自动errorCode发送失败" + ",OEELog");
            //        Log.WriteLog(Global.respond[guid].GUID.ToString() + "," + Global.respond[guid].Result + "," + Global.respond[guid].ErrorCode, System.AppDomain.CurrentDomain.BaseDirectory + "\\MQTT返回NG\\");
            //        Global.respond.TryRemove(guid, out Global.Respond);
            //    }
            //}
            //else
            //{
            //    Global.ConnectOEEFlag = false;
            //    Log.WriteLog("OEE_DT:补传待料自动errorCode发送失败,超时无反馈:" + ",OEELog");
            //    _homefrm.AppendRichText("" + "," + d1.Rows[0][2].ToString() + ",触发时间=" + Global.SendHSG_start_time + ",运行状态:" + d1.Rows[0][5].ToString() + ",故障描述:" + d1.Rows[0][6].ToString() + ",自动发送成功", "rtx_DownTimeMsg");
            //    _homefrm.AppendRichText("网络异常,超时无反馈", "rtx_DownTimeMsg");
            //    string s = "insert into OEE_DTSendNG([DateTime],[Product],[GUID],[EMT],[PoorNum],[TotalNum],[Status],[ErrorCode],[ModuleCode],[ClientPcName],[MAC],[IP],[EventTime],[Moduleinfo],[errorinfo])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," + "'" + "OEE_DT" + "'" + "," + "'" + guid + "'" + "," + "'" + Global.inidata.productconfig.EMT + "'" + "," + "'" + "0" + "'" + "," + "'" + "0" + "'" + "," + "'" + d1.Rows[0][5].ToString() + "'" + "," + "'" + d1.Rows[0][2].ToString() + "'" + ","
            //                   + "'" + "" + "'" + "," + "'" + ClientPcName + "'" + "," + "'" + Mac + "'" + "," + "'" + IP + "'" + "," + "'" + Global.SendHSG_start_time + "'" + "," + "'" + "" + "'" + "," + "'" + d1.Rows[0][6].ToString() + "'" + ")";
            //    int r = SQL.ExecuteUpdate(s);
            //    Log.WriteLog(string.Format("插入了{0}行OEE_DownTime缓存数据", r) + ",OEELog");
            //}
            //=======记录补传的待料时间=========                
            string InsertStr = "insert into OEE_DT([DateTime],[ErrorCode],[EventTime],[ModuleCode],[RunStatus],[ErrorInfo],[TimeSpan])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "'" + "," + "'" + d1.Rows[0][2].ToString() + "'" + "," + "'" + Global.SendHSG_start_time + "'" + ","
                + "'" + "" + "'" + "," + "'" + d1.Rows[0][5].ToString() + "'" + "," + "'" + d1.Rows[0][6].ToString() + "'" + "," + "'" + ts + "'" + ")";
            SQL.ExecuteUpdate(InsertStr);
            InsertStr = "insert into OEE_TraceDT([DateTime],[Status],[ErrorCode],[EventTime],[ModuleCode])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," + "'" + d1.Rows[0][5].ToString() + "'" + "," + "'" + d1.Rows[0][2].ToString() + "'" + ","
                    + "'" + Global.SendHSG_start_time + "'" + "," + "'" + d1.Rows[0][4].ToString() + "'" + ")";
            SQL.ExecuteUpdate(InsertStr);
        }
        private void CacheQuantity(object ob)//上传失败缓存数量
        {
            try
            {
                while (true)
                {
                    DataTable dt = SQL.ExecuteQuery("select * from  Trace_UA_SendNG");
                    DataTable dt5 = SQL.ExecuteQuery("select * from  Trace_LA_SendNG");
                    _homefrm.UpDatalabel((dt.Rows.Count + dt5.Rows.Count).ToString(), "lb_TraceSendNG");
                    DataTable dt2 = SQL.ExecuteQuery("select * from  PDCA_SendNG");
                    _homefrm.UpDatalabel(dt2.Rows.Count.ToString(), "lb_PDCASendNG");
                    DataTable dt3 = SQL.ExecuteQuery("select * from  OEE_DefaultSendNG");
                    DataTable dt4 = SQL.ExecuteQuery("select * from  OEE_DTSendNG");
                    _homefrm.UpDatalabel((dt3.Rows.Count + dt4.Rows.Count).ToString(), "lb_OEESendNG");
                    Thread.Sleep(5000);
                }
            }
            catch (Exception ex)
            {
                Log.WriteLog(ex.ToString().Replace("\n", ""));
            }
        }

        /// <summary>
        /// 拷贝文件
        /// </summary>
        /// <param name="fromFilePath">文件的路径</param>
        /// <param name="toFilePath">文件要拷贝到的路径</param>
        private bool CopyFile(string fromFilePath, string toFilePath)
        {
            try
            {
                if (File.Exists(fromFilePath))
                {
                    if (File.Exists(toFilePath))
                    {
                        File.Delete(toFilePath);
                    }
                    File.Copy(fromFilePath, toFilePath);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// 发送CSV文件给Macmini解析
        /// </summary>
        /// <param name="url">MacminiURL路径</param>
        /// <param name="timeOut">超时</param>
        /// <param name="fileKeyName">Key</param>
        /// <param name="filePath">Value</param>
        /// <param name="stringDict">键值对集合</param>
        /// <returns></returns>
        public string HttpPostData(string url, int timeOut, string fileKeyName, string filePath, NameValueCollection stringDict)
        {
            string responseContent;
            var memStream = new MemoryStream();
            var webRequest = (HttpWebRequest)WebRequest.Create(url);
            // 边界符
            var boundary = "---------------" + DateTime.Now.Ticks.ToString("x");
            // 边界符
            var beginBoundary = Encoding.ASCII.GetBytes("--" + boundary + "\r\n");
            var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            // 最后的结束符
            var endBoundary = Encoding.ASCII.GetBytes("--" + boundary + "--\r\n");

            // 设置属性
            webRequest.Method = "POST";
            webRequest.Timeout = timeOut;
            webRequest.ContentType = "multipart/form-data; boundary=" + boundary;

            // 写入文件
            const string filePartHeader =
        "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\n" +
        "Content-Type: application/octet-stream\r\n\r\n";
            var header = string.Format(filePartHeader, fileKeyName, filePath);
            var headerbytes = Encoding.UTF8.GetBytes(header);

            memStream.Write(beginBoundary, 0, beginBoundary.Length);
            memStream.Write(headerbytes, 0, headerbytes.Length);

            var buffer = new byte[1024];
            int bytesRead; // =0

            while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
            {
                memStream.Write(buffer, 0, bytesRead);
            }

            // 写入字符串的Key
            var stringKeyHeader = "\r\n--" + boundary +
                 "\r\nContent-Disposition: form-data; name=\"{0}\"" +
                 "\r\n\r\n{1}\r\n";

            foreach (byte[] formitembytes in from string key in stringDict.Keys
                                             select string.Format(stringKeyHeader, key, stringDict[key])
                                                 into formitem
                                             select Encoding.UTF8.GetBytes(formitem))
            {
                memStream.Write(formitembytes, 0, formitembytes.Length);
            }

            // 写入最后的结束边界符
            memStream.Write(endBoundary, 0, endBoundary.Length);

            webRequest.ContentLength = memStream.Length;

            var requestStream = webRequest.GetRequestStream();

            memStream.Position = 0;
            var tempBuffer = new byte[memStream.Length];
            memStream.Read(tempBuffer, 0, tempBuffer.Length);
            memStream.Close();

            requestStream.Write(tempBuffer, 0, tempBuffer.Length);
            requestStream.Close();

            var httpWebResponse = (HttpWebResponse)webRequest.GetResponse();

            using (var httpStreamReader = new StreamReader(httpWebResponse.GetResponseStream(),
                                    Encoding.GetEncoding("utf-8")))
            {
                responseContent = httpStreamReader.ReadToEnd();
            }

            fileStream.Close();
            httpWebResponse.Close();
            webRequest.Abort();

            return responseContent;
        }

        private void GetAllHansData(string SN, out HansData_U_Bracket data, int station)
        {
            lock (LockLA)
            {
                int HansDataResult = 0;
                switch (station)
                {
                    case 1:
                        HansDataResult = 10304;
                        break;
                    default:
                        break;
                }
                HansData_U_Bracket UA_data = new HansData_U_Bracket();
                string SelectStr = string.Format("SELECT * FROM HansData WHERE SN='{0}'", SN);//sql查询语句
                DataTable d1 = SQL.ExecuteQuery(SelectStr);
                if (d1 != null && d1.Rows.Count > 0)
                {
                    if (d1.Rows.Count < 4)
                    {
                        UA_data.HanDataUAResult = false;
                        Log.WriteLog(string.Format(("UA获取焊接参数数量不足，有{0}笔，SN为{1}"), d1.Rows.Count, SN));
                        Global.PLC_Client2.WritePLC_D(HansDataResult, new short[] { 2 });
                    }
                    else
                    {
                        UA_data.HanDataUAResult = true;
                        Global.PLC_Client2.WritePLC_D(HansDataResult, new short[] { 1 });
                    }
                    UA_data.power_ll = d1.Rows[0][4].ToString();
                    UA_data.power_ul = d1.Rows[0][5].ToString();
                    UA_data.pattern_type = d1.Rows[0][6].ToString();
                    UA_data.spot_size = d1.Rows[0][7].ToString();
                    UA_data.hatch = d1.Rows[0][8].ToString();
                    UA_data.swing_amplitude = d1.Rows[0][9].ToString();
                    UA_data.swing_freq = d1.Rows[0][10].ToString();
                    UA_data.JudgeResult = d1.Rows[0][11].ToString();
                    UA_data.MeasureTime = d1.Rows[0][12].ToString();
                    UA_data.MachineSN = d1.Rows[0][13].ToString();
                    UA_data.PulseProfile_measure = d1.Rows[0][14].ToString();
                    UA_data.ActualPower = d1.Rows[0][15].ToString();
                    UA_data.Power_measure = d1.Rows[0][16].ToString();
                    UA_data.WaveForm_measure = d1.Rows[0][17].ToString();
                    UA_data.Frequency_measure = d1.Rows[0][18].ToString();
                    UA_data.LinearSpeed_measure = d1.Rows[0][19].ToString();
                    UA_data.QRelease_measure = d1.Rows[0][20].ToString();
                    UA_data.PulseEnergy_measure = d1.Rows[0][21].ToString();
                    UA_data.PeakPower_measure = d1.Rows[0][22].ToString();
                    UA_data.laser_sensor = d1.Rows[0][23].ToString();
                    for (int i = 0; i < d1.Rows.Count; i++)
                    {
                        if (d1.Rows[i][3].ToString() == "shim-NE")
                        {
                            //UA_data.pulse_profile= d1.Rows[i][24].ToString();
                            UA_data.location1_layer1_laser_power = d1.Rows[i][25].ToString();
                            UA_data.location1_layer1_frequency = d1.Rows[i][26].ToString();
                            UA_data.location1_layer1_waveform = d1.Rows[i][27].ToString();
                            UA_data.location1_layer1_pulse_energy = d1.Rows[i][28].ToString();
                            UA_data.location1_layer1_laser_speed = d1.Rows[i][29].ToString();
                            UA_data.location1_layer1_jump_speed = d1.Rows[i][30].ToString();
                            UA_data.location1_layer1_jump_delay = d1.Rows[i][31].ToString();
                            UA_data.location1_layer1_scanner_delay = d1.Rows[i][32].ToString();
                        }
                        if (d1.Rows[i][3].ToString() == "shim-SL.hs")
                        {
                            //UA_data.pulse_profile = d1.Rows[i][24].ToString();
                            UA_data.location2_layer1_laser_power = d1.Rows[i][25].ToString();
                            UA_data.location2_layer1_frequency = d1.Rows[i][26].ToString();
                            UA_data.location2_layer1_waveform = d1.Rows[i][27].ToString();
                            UA_data.location2_layer1_pulse_energy = d1.Rows[i][28].ToString();
                            UA_data.location2_layer1_laser_speed = d1.Rows[i][29].ToString();
                            UA_data.location2_layer1_jump_speed = d1.Rows[i][30].ToString();
                            UA_data.location2_layer1_jump_delay = d1.Rows[i][31].ToString();
                            UA_data.location2_layer1_scanner_delay = d1.Rows[i][32].ToString();
                        }
                        if (d1.Rows[i][3].ToString() == "shim-SR")
                        {
                            //UA_data.pulse_profile = d1.Rows[i][24].ToString();
                            UA_data.location3_layer1_laser_power = d1.Rows[i][25].ToString();
                            UA_data.location3_layer1_frequency = d1.Rows[i][26].ToString();
                            UA_data.location3_layer1_waveform = d1.Rows[i][27].ToString();
                            UA_data.location3_layer1_pulse_energy = d1.Rows[i][28].ToString();
                            UA_data.location3_layer1_laser_speed = d1.Rows[i][29].ToString();
                            UA_data.location3_layer1_jump_speed = d1.Rows[i][30].ToString();
                            UA_data.location3_layer1_jump_delay = d1.Rows[i][31].ToString();
                            UA_data.location3_layer1_scanner_delay = d1.Rows[i][32].ToString();
                        }
                        if (d1.Rows[i][3].ToString() == "Trenh bracke")
                        {
                            //UA_data.pulse_profile = d1.Rows[i][24].ToString();
                            UA_data.location4_layer1_laser_power = d1.Rows[i][25].ToString();
                            UA_data.location4_layer1_frequency = d1.Rows[i][26].ToString();
                            UA_data.location4_layer1_waveform = d1.Rows[i][27].ToString();
                            UA_data.location4_layer1_pulse_energy = d1.Rows[i][28].ToString();
                            UA_data.location4_layer1_laser_speed = d1.Rows[i][29].ToString();
                            UA_data.location4_layer1_jump_speed = d1.Rows[i][30].ToString();
                            UA_data.location4_layer1_jump_delay = d1.Rows[i][31].ToString();
                            UA_data.location4_layer1_scanner_delay = d1.Rows[i][32].ToString();
                        }
                    }
                }
                else
                {
                    UA_data.HanDataUAResult = false;
                    Global.PLC_Client.WritePLC_D(HansDataResult, new short[] { 2 });
                    Log.WriteLog(string.Format(("UA获取焊接参数数量不足，有{0}笔，SN为{1}"), d1.Rows.Count, SN));
                }
                data = UA_data;
            }
        }

        private void Product_DataStatistics()//产能数据统计
        {
            //-------------------------------白班产能统计--------------------------------------------
            Global.Product_Total = Global.PLC_Client.ReadPLC_D(2030, 12);//白班总产能
            Global.Product_NG = Global.PLC_Client.ReadPLC_D(2056, 12);//白班NG产能
            Global.Product_OK = Global.PLC_Client.ReadPLC_D(2006, 12);//白班OK产能
            short Product_Total_08_09 = Global.Product_Total[0];//白班8-9点总产能
            short Product_Total_09_10 = Global.Product_Total[1];
            short Product_Total_10_11 = Global.Product_Total[2];
            short Product_Total_11_12 = Global.Product_Total[3];
            short Product_Total_12_13 = Global.Product_Total[4];
            short Product_Total_13_14 = Global.Product_Total[5];
            short Product_Total_14_15 = Global.Product_Total[6];
            short Product_Total_15_16 = Global.Product_Total[7];
            short Product_Total_16_17 = Global.Product_Total[8];
            short Product_Total_17_18 = Global.Product_Total[9];
            short Product_Total_18_19 = Global.Product_Total[10];
            short Product_Total_19_20 = Global.Product_Total[11];//白班19-20点总产能
            short Product_NG_08_09 = Global.Product_NG[0];//白班8-9点NG产能
            short Product_NG_09_10 = Global.Product_NG[1];
            short Product_NG_10_11 = Global.Product_NG[2];
            short Product_NG_11_12 = Global.Product_NG[3];
            short Product_NG_12_13 = Global.Product_NG[4];
            short Product_NG_13_14 = Global.Product_NG[5];
            short Product_NG_14_15 = Global.Product_NG[6];
            short Product_NG_15_16 = Global.Product_NG[7];
            short Product_NG_16_17 = Global.Product_NG[8];
            short Product_NG_17_18 = Global.Product_NG[9];
            short Product_NG_18_19 = Global.Product_NG[10];
            short Product_NG_19_20 = Global.Product_NG[11];//白班19-20点NG产能

            if (Product_Total_08_09 == 0)
            {
                Product_Lianglv_08_09 = 0;
            }
            else
            {
                Product_Lianglv_08_09 = ((double)(Product_Total_08_09 - Product_NG_08_09) / (double)Product_Total_08_09) * 100;//白班8-9点良率
            }
            if (Product_Total_09_10 == 0)
            {
                Product_Lianglv_09_10 = 0;
            }
            else
            {
                Product_Lianglv_09_10 = ((double)(Product_Total_09_10 - Product_NG_09_10) / (double)Product_Total_09_10) * 100;
            }
            if (Product_Total_10_11 == 0)
            {
                Product_Lianglv_10_11 = 0;
            }
            else
            {
                Product_Lianglv_10_11 = ((double)(Product_Total_10_11 - Product_NG_10_11) / (double)Product_Total_10_11) * 100;
            }
            if (Product_Total_11_12 == 0)
            {
                Product_Lianglv_11_12 = 0;
            }
            else
            {
                Product_Lianglv_11_12 = ((double)(Product_Total_11_12 - Product_NG_11_12) / (double)Product_Total_11_12) * 100;
            }
            if (Product_Total_12_13 == 0)
            {
                Product_Lianglv_12_13 = 0;
            }
            else
            {
                Product_Lianglv_12_13 = ((double)(Product_Total_12_13 - Product_NG_12_13) / (double)Product_Total_12_13) * 100;
            }
            if (Product_Total_13_14 == 0)
            {
                Product_Lianglv_13_14 = 0;
            }
            else
            {
                Product_Lianglv_13_14 = ((double)(Product_Total_13_14 - Product_NG_13_14) / (double)Product_Total_13_14) * 100;
            }
            if (Product_Total_14_15 == 0)
            {
                Product_Lianglv_14_15 = 0;
            }
            else
            {
                Product_Lianglv_14_15 = ((double)(Product_Total_14_15 - Product_NG_14_15) / (double)Product_Total_14_15) * 100;
            }
            if (Product_Total_15_16 == 0)
            {
                Product_Lianglv_15_16 = 0;
            }
            else
            {
                Product_Lianglv_15_16 = ((double)(Product_Total_15_16 - Product_NG_15_16) / (double)Product_Total_15_16) * 100;
            }
            if (Product_Total_16_17 == 0)
            {
                Product_Lianglv_16_17 = 0;
            }
            else
            {
                Product_Lianglv_16_17 = ((double)(Product_Total_16_17 - Product_NG_16_17) / (double)Product_Total_16_17) * 100;
            }
            if (Product_Total_17_18 == 0)
            {
                Product_Lianglv_17_18 = 0;
            }
            else
            {
                Product_Lianglv_17_18 = ((double)(Product_Total_17_18 - Product_NG_17_18) / (double)Product_Total_17_18) * 100;
            }
            if (Product_Total_18_19 == 0)
            {
                Product_Lianglv_18_19 = 0;
            }
            else
            {
                Product_Lianglv_18_19 = ((double)(Product_Total_18_19 - Product_NG_18_19) / (double)Product_Total_18_19) * 100;
            }
            if (Product_Total_19_20 == 0)
            {
                Product_Lianglv_19_20 = 0;
            }
            else
            {
                Product_Lianglv_19_20 = ((double)(Product_Total_19_20 - Product_NG_19_20) / (double)Product_Total_19_20) * 100;//白班19-20点良率
            }

            short Product_Total_08_20 = Global.PLC_Client.ReadPLC_D(2950, 1)[0];//白班总产能
            short Product_NG_08_20 = Global.PLC_Client.ReadPLC_D(2952, 1)[0];//白班NG总产能
            if (Global.PLC_Client.ReadPLC_D(2950, 1)[0] == 0)
            {
                Product_Lianglv_08_20 = 0;
            }
            else
            {
                Product_Lianglv_08_20 = ((double)(Global.PLC_Client.ReadPLC_D(2950, 1)[0] - Global.PLC_Client.ReadPLC_D(2952, 1)[0]) / (double)Global.PLC_Client.ReadPLC_D(2950, 1)[0]) * 100;//白班总良率
            }
            Global.Product_Total_D = Product_Total_08_20;
            Global.Product_OK_D = Product_Total_08_20 - Product_NG_08_20;
            if (DateTime.Now.ToString("yyyy-MM-dd") == Global.SelectDateTime.ToString("yyyy-MM-dd"))
            {
                _datastatisticsfrm.UpDatalabel(Product_Total_08_09.ToString(), "lb_Product_Total_08_09");
                _datastatisticsfrm.UpDatalabel(Product_Total_09_10.ToString(), "lb_Product_Total_09_10");
                _datastatisticsfrm.UpDatalabel(Product_Total_10_11.ToString(), "lb_Product_Total_10_11");
                _datastatisticsfrm.UpDatalabel(Product_Total_11_12.ToString(), "lb_Product_Total_11_12");
                _datastatisticsfrm.UpDatalabel(Product_Total_12_13.ToString(), "lb_Product_Total_12_13");
                _datastatisticsfrm.UpDatalabel(Product_Total_13_14.ToString(), "lb_Product_Total_13_14");
                _datastatisticsfrm.UpDatalabel(Product_Total_14_15.ToString(), "lb_Product_Total_14_15");
                _datastatisticsfrm.UpDatalabel(Product_Total_15_16.ToString(), "lb_Product_Total_15_16");
                _datastatisticsfrm.UpDatalabel(Product_Total_16_17.ToString(), "lb_Product_Total_16_17");
                _datastatisticsfrm.UpDatalabel(Product_Total_17_18.ToString(), "lb_Product_Total_17_18");
                _datastatisticsfrm.UpDatalabel(Product_Total_18_19.ToString(), "lb_Product_Total_18_19");
                _datastatisticsfrm.UpDatalabel(Product_Total_19_20.ToString(), "lb_Product_Total_19_20");

                _datastatisticsfrm.UpDatalabel(Product_NG_08_09.ToString(), "lb_Product_NG_08_09");
                _datastatisticsfrm.UpDatalabel(Product_NG_09_10.ToString(), "lb_Product_NG_09_10");
                _datastatisticsfrm.UpDatalabel(Product_NG_10_11.ToString(), "lb_Product_NG_10_11");
                _datastatisticsfrm.UpDatalabel(Product_NG_11_12.ToString(), "lb_Product_NG_11_12");
                _datastatisticsfrm.UpDatalabel(Product_NG_12_13.ToString(), "lb_Product_NG_12_13");
                _datastatisticsfrm.UpDatalabel(Product_NG_13_14.ToString(), "lb_Product_NG_13_14");
                _datastatisticsfrm.UpDatalabel(Product_NG_14_15.ToString(), "lb_Product_NG_14_15");
                _datastatisticsfrm.UpDatalabel(Product_NG_15_16.ToString(), "lb_Product_NG_15_16");
                _datastatisticsfrm.UpDatalabel(Product_NG_16_17.ToString(), "lb_Product_NG_16_17");
                _datastatisticsfrm.UpDatalabel(Product_NG_17_18.ToString(), "lb_Product_NG_17_18");
                _datastatisticsfrm.UpDatalabel(Product_NG_18_19.ToString(), "lb_Product_NG_18_19");
                _datastatisticsfrm.UpDatalabel(Product_NG_19_20.ToString(), "lb_Product_NG_19_20");

                _datastatisticsfrm.UpDatalabel(Product_Lianglv_08_09.ToString("0.00") + "%", "lb_Product_Lianglv_08_09");
                _datastatisticsfrm.UpDatalabel(Product_Lianglv_09_10.ToString("0.00") + "%", "lb_Product_Lianglv_09_10");
                _datastatisticsfrm.UpDatalabel(Product_Lianglv_10_11.ToString("0.00") + "%", "lb_Product_Lianglv_10_11");
                _datastatisticsfrm.UpDatalabel(Product_Lianglv_11_12.ToString("0.00") + "%", "lb_Product_Lianglv_11_12");
                _datastatisticsfrm.UpDatalabel(Product_Lianglv_12_13.ToString("0.00") + "%", "lb_Product_Lianglv_12_13");
                _datastatisticsfrm.UpDatalabel(Product_Lianglv_13_14.ToString("0.00") + "%", "lb_Product_Lianglv_13_14");
                _datastatisticsfrm.UpDatalabel(Product_Lianglv_14_15.ToString("0.00") + "%", "lb_Product_Lianglv_14_15");
                _datastatisticsfrm.UpDatalabel(Product_Lianglv_15_16.ToString("0.00") + "%", "lb_Product_Lianglv_15_16");
                _datastatisticsfrm.UpDatalabel(Product_Lianglv_16_17.ToString("0.00") + "%", "lb_Product_Lianglv_16_17");
                _datastatisticsfrm.UpDatalabel(Product_Lianglv_17_18.ToString("0.00") + "%", "lb_Product_Lianglv_17_18");
                _datastatisticsfrm.UpDatalabel(Product_Lianglv_18_19.ToString("0.00") + "%", "lb_Product_Lianglv_18_19");
                _datastatisticsfrm.UpDatalabel(Product_Lianglv_19_20.ToString("0.00") + "%", "lb_Product_Lianglv_19_20");

                _datastatisticsfrm.UpDatalabel(Product_Total_08_20.ToString(), "lb_Product_Total_08_20");
                _datastatisticsfrm.UpDatalabel(Product_NG_08_20.ToString(), "lb_Product_NG_08_20");
                _datastatisticsfrm.UpDatalabel(Product_Lianglv_08_20.ToString("0.00") + "%", "lb_Product_Lianglv_08_20");

                _datastatisticsfrm.UpDataDGV_D(0, 1, Global.Product_Total_D.ToString());
                _datastatisticsfrm.UpDataDGV_D(1, 1, Global.inidata.productconfig.Smallmaterial_Input_D.ToString());
                _datastatisticsfrm.UpDataDGV_D(4, 1, Global.inidata.productconfig.location1_CCDNG_D);
                double location1_CCDNG_D = (Convert.ToDouble(Global.inidata.productconfig.location1_CCDNG_D) / Convert.ToDouble(Global.Product_Total_D)) * 100;
                _datastatisticsfrm.UpDataDGV_D(4, 2, location1_CCDNG_D.ToString("0.00") + "%");
                _datastatisticsfrm.UpDataDGV_D(5, 1, Global.inidata.productconfig.location2_CCDNG_D);
                double location2_CCDNG_D = (Convert.ToDouble(Global.inidata.productconfig.location2_CCDNG_D) / Convert.ToDouble(Global.Product_Total_D)) * 100;
                _datastatisticsfrm.UpDataDGV_D(5, 2, location2_CCDNG_D.ToString("0.00") + "%");
                _datastatisticsfrm.UpDataDGV_D(6, 1, Global.inidata.productconfig.location3_CCDNG_D);
                double location3_CCDNG_D = (Convert.ToDouble(Global.inidata.productconfig.location3_CCDNG_D) / Convert.ToDouble(Global.Product_Total_D)) * 100;
                _datastatisticsfrm.UpDataDGV_D(6, 2, location3_CCDNG_D.ToString("0.00") + "%");
                //_datastatisticsfrm.UpDataDGV_D(7, 1, Global.inidata.productconfig.location4_CCDNG_D);
                //double location4_CCDNG_D = (Convert.ToDouble(Global.inidata.productconfig.location4_CCDNG_D) / Convert.ToDouble(Global.Product_Total_D)) * 100;
                //_datastatisticsfrm.UpDataDGV_D(7, 2, location4_CCDNG_D.ToString("0.00") + "%");
                //_datastatisticsfrm.UpDataDGV_D(8, 1, Global.inidata.productconfig.location5_CCDNG_D);
                //double location5_CCDNG_D = (Convert.ToDouble(Global.inidata.productconfig.location5_CCDNG_D) / Convert.ToDouble(Global.Product_Total_D)) * 100;
                //_datastatisticsfrm.UpDataDGV_D(8, 2, location5_CCDNG_D.ToString("0.00") + "%");
                _datastatisticsfrm.UpDataDGV_D(7, 1, Global.inidata.productconfig.HansDataError_D);
                double HansDataError_D = (Convert.ToDouble(Global.inidata.productconfig.HansDataError_D) / Convert.ToDouble(Global.Product_Total_D)) * 100;
                _datastatisticsfrm.UpDataDGV_D(7, 2, HansDataError_D.ToString("0.00") + "%");

                _datastatisticsfrm.UpDataDGV_D(10, 1, Global.inidata.productconfig.TraceUpLoad_Error_D);
                double TraceUpLoad_Error_D = (Convert.ToDouble(Global.inidata.productconfig.TraceUpLoad_Error_D) / Convert.ToDouble(Global.Product_Total_D)) * 100;
                _datastatisticsfrm.UpDataDGV_D(10, 2, TraceUpLoad_Error_D.ToString("0.00") + "%");
                _datastatisticsfrm.UpDataDGV_D(11, 1, Global.inidata.productconfig.PDCAUpLoad_Error_D);
                double PDCAUpLoad_Error_D = (Convert.ToDouble(Global.inidata.productconfig.PDCAUpLoad_Error_D) / Global.Product_Total_D) * 100;
                _datastatisticsfrm.UpDataDGV_D(11, 2, PDCAUpLoad_Error_D.ToString("0.00") + "%");
                _datastatisticsfrm.UpDataDGV_D(12, 1, Global.inidata.productconfig.TracePVCheck_Error_D);
                double TracePVCheck_Error_D = (Convert.ToDouble(Global.inidata.productconfig.TracePVCheck_Error_D) / Global.Product_Total_D) * 100;
                _datastatisticsfrm.UpDataDGV_D(12, 2, TracePVCheck_Error_D.ToString("0.00") + "%");

                _datastatisticsfrm.UpDataDGV_D(13, 1, Global.inidata.productconfig.ReadBarcode_NG_D);
                double ReadBarcode_NG_D = (Convert.ToDouble(Global.inidata.productconfig.ReadBarcode_NG_D) / Global.Product_Total_D) * 100;
                _datastatisticsfrm.UpDataDGV_D(13, 2, ReadBarcode_NG_D.ToString("0.00") + "%");

                _datastatisticsfrm.UpDataDGV_D(16, 1, Global.inidata.productconfig.TraceTab_Error_D);
                double TraceTab_Error_D = (Convert.ToDouble(Global.inidata.productconfig.TraceTab_Error_D) / Convert.ToDouble(Global.Product_Total_D)) * 100;
                _datastatisticsfrm.UpDataDGV_D(16, 2, TraceTab_Error_D.ToString("0.00") + "%");
                _datastatisticsfrm.UpDataDGV_D(17, 1, Global.inidata.productconfig.TraceThench_Error_D);
                double TraceThench_Error_D = (Convert.ToDouble(Global.inidata.productconfig.TraceThench_Error_D) / Convert.ToDouble(Global.Product_Total_D)) * 100;
                _datastatisticsfrm.UpDataDGV_D(17, 2, TraceThench_Error_D.ToString("0.00") + "%");
            }

            //-------------------------------夜班产能统计--------------------------------------------
            Global.Product_Total_N_1 = Global.PLC_Client.ReadPLC_D(2042, 6);//夜班产能1
            Global.Product_Total_N_2 = Global.PLC_Client.ReadPLC_D(2024, 6);//夜班产能2
            Global.Product_NG_N_1 = Global.PLC_Client.ReadPLC_D(2068, 6);//夜班NG产能1
            Global.Product_NG_N_2 = Global.PLC_Client.ReadPLC_D(2050, 6);//夜班NG产能2
            Global.Product_OK_N_1 = Global.PLC_Client.ReadPLC_D(2018, 6);//夜班OK产能1
            Global.Product_OK_N_2 = Global.PLC_Client.ReadPLC_D(2000, 6);//夜班OK产能2
            short Product_Total_20_21 = Global.Product_Total_N_1[0];//夜班17:30-5:30点总产能
            short Product_Total_21_22 = Global.Product_Total_N_1[1];
            short Product_Total_22_23 = Global.Product_Total_N_1[2];
            short Product_Total_23_00 = Global.Product_Total_N_1[3];
            short Product_Total_00_01 = Global.Product_Total_N_1[4];
            short Product_Total_01_02 = Global.Product_Total_N_1[5];
            short Product_Total_02_03 = Global.Product_Total_N_2[0];
            short Product_Total_03_04 = Global.Product_Total_N_2[1];
            short Product_Total_04_05 = Global.Product_Total_N_2[2];
            short Product_Total_05_06 = Global.Product_Total_N_2[3];
            short Product_Total_06_07 = Global.Product_Total_N_2[4];
            short Product_Total_07_08 = Global.Product_Total_N_2[5];//夜班4:30-5:30点总产能
            short Product_NG_20_21 = Global.Product_NG_N_1[0];//夜班8-9点NG产能
            short Product_NG_21_22 = Global.Product_NG_N_1[1];
            short Product_NG_22_23 = Global.Product_NG_N_1[2];
            short Product_NG_23_00 = Global.Product_NG_N_1[3];
            short Product_NG_00_01 = Global.Product_NG_N_1[4];
            short Product_NG_01_02 = Global.Product_NG_N_1[5];
            short Product_NG_02_03 = Global.Product_NG_N_2[0];
            short Product_NG_03_04 = Global.Product_NG_N_2[1];
            short Product_NG_04_05 = Global.Product_NG_N_2[2];
            short Product_NG_05_06 = Global.Product_NG_N_2[3];
            short Product_NG_06_07 = Global.Product_NG_N_2[4];
            short Product_NG_07_08 = Global.Product_NG_N_2[5];//夜班19-20点NG产能

            if (Product_Total_20_21 == 0)
            {
                Product_Lianglv_20_21 = 0;
            }
            else
            {
                Product_Lianglv_20_21 = ((double)(Product_Total_20_21 - Product_NG_20_21) / (double)Product_Total_20_21) * 100;//夜班20-21点良率
            }
            if (Product_Total_21_22 == 0)
            {
                Product_Lianglv_21_22 = 0;
            }
            else
            {
                Product_Lianglv_21_22 = ((double)(Product_Total_21_22 - Product_NG_21_22) / (double)Product_Total_21_22) * 100;
            }
            if (Product_Total_22_23 == 0)
            {
                Product_Lianglv_22_23 = 0;
            }
            else
            {
                Product_Lianglv_22_23 = ((double)(Product_Total_22_23 - Product_NG_22_23) / (double)Product_Total_22_23) * 100;
            }
            if (Product_Total_23_00 == 0)
            {
                Product_Lianglv_23_00 = 0;
            }
            else
            {
                Product_Lianglv_23_00 = ((double)(Product_Total_23_00 - Product_NG_23_00) / (double)Product_Total_23_00) * 100;
            }
            if (Product_Total_00_01 == 0)
            {
                Product_Lianglv_00_01 = 0;
            }
            else
            {
                Product_Lianglv_00_01 = ((double)(Product_Total_00_01 - Product_NG_00_01) / (double)Product_Total_00_01) * 100;
            }
            if (Product_Total_01_02 == 0)
            {
                Product_Lianglv_01_02 = 0;
            }
            else
            {
                Product_Lianglv_01_02 = ((double)(Product_Total_01_02 - Product_NG_01_02) / (double)Product_Total_01_02) * 100;
            }
            if (Product_Total_02_03 == 0)
            {
                Product_Lianglv_02_03 = 0;
            }
            else
            {
                Product_Lianglv_02_03 = ((double)(Product_Total_02_03 - Product_NG_02_03) / (double)Product_Total_02_03) * 100;
            }
            if (Product_Total_03_04 == 0)
            {
                Product_Lianglv_03_04 = 0;
            }
            else
            {
                Product_Lianglv_03_04 = ((double)(Product_Total_03_04 - Product_NG_03_04) / (double)Product_Total_03_04) * 100;
            }
            if (Product_Total_04_05 == 0)
            {
                Product_Lianglv_04_05 = 0;
            }
            else
            {
                Product_Lianglv_04_05 = ((double)(Product_Total_04_05 - Product_NG_04_05) / (double)Product_Total_04_05) * 100;
            }
            if (Product_Total_05_06 == 0)
            {
                Product_Lianglv_05_06 = 0;
            }
            else
            {
                Product_Lianglv_05_06 = ((double)(Product_Total_05_06 - Product_NG_05_06) / (double)Product_Total_05_06) * 100;
            }
            if (Product_Total_06_07 == 0)
            {
                Product_Lianglv_06_07 = 0;
            }
            else
            {
                Product_Lianglv_06_07 = ((double)(Product_Total_06_07 - Product_NG_06_07) / (double)Product_Total_06_07) * 100;
            }
            if (Product_Total_07_08 == 0)
            {
                Product_Lianglv_07_08 = 0;
            }
            else
            {
                Product_Lianglv_07_08 = ((double)(Product_Total_07_08 - Product_NG_07_08) / (double)Product_Total_07_08) * 100;//夜班07-08点良率
            }

            short Product_Total_20_08 = Global.PLC_Client.ReadPLC_D(2960, 1)[0];//夜班总产能
            short Product_NG_20_08 = Global.PLC_Client.ReadPLC_D(2962, 1)[0];//夜班NG总产能
            if (Global.PLC_Client.ReadPLC_D(2964, 1)[0] == 0)
            {
                Product_Lianglv_20_08 = 0;
            }
            else
            {
                Product_Lianglv_20_08 = ((double)(Global.PLC_Client.ReadPLC_D(2960, 1)[0] - Global.PLC_Client.ReadPLC_D(2962, 1)[0]) / (double)Global.PLC_Client.ReadPLC_D(2960, 1)[0]) * 100;//夜班总良率
            }
            Global.Product_Total_N = Product_Total_20_08;
            Global.Product_OK_N = Product_Total_20_08 - Product_NG_20_08;
            if (DateTime.Now.ToString("yyyy-MM-dd") == Global.SelectDateTime.ToString("yyyy-MM-dd"))
            {
                if (Convert.ToDateTime(DateTime.Now.ToString("HH:mm")).CompareTo(Convert.ToDateTime("18:00")) >= 0 || Convert.ToDateTime(DateTime.Now.ToString("HH:mm")).CompareTo(Convert.ToDateTime("6:00")) < 0)
                {
                    _datastatisticsfrm.UpDatalabel(Product_Total_20_21.ToString(), "lb_Product_Total_20_21");
                    _datastatisticsfrm.UpDatalabel(Product_Total_21_22.ToString(), "lb_Product_Total_21_22");
                    _datastatisticsfrm.UpDatalabel(Product_Total_22_23.ToString(), "lb_Product_Total_22_23");
                    _datastatisticsfrm.UpDatalabel(Product_Total_23_00.ToString(), "lb_Product_Total_23_00");
                    _datastatisticsfrm.UpDatalabel(Product_Total_00_01.ToString(), "lb_Product_Total_00_01");
                    _datastatisticsfrm.UpDatalabel(Product_Total_01_02.ToString(), "lb_Product_Total_01_02");
                    _datastatisticsfrm.UpDatalabel(Product_Total_02_03.ToString(), "lb_Product_Total_02_03");
                    _datastatisticsfrm.UpDatalabel(Product_Total_03_04.ToString(), "lb_Product_Total_03_04");
                    _datastatisticsfrm.UpDatalabel(Product_Total_04_05.ToString(), "lb_Product_Total_04_05");
                    _datastatisticsfrm.UpDatalabel(Product_Total_05_06.ToString(), "lb_Product_Total_05_06");
                    _datastatisticsfrm.UpDatalabel(Product_Total_06_07.ToString(), "lb_Product_Total_06_07");
                    _datastatisticsfrm.UpDatalabel(Product_Total_07_08.ToString(), "lb_Product_Total_07_08");

                    _datastatisticsfrm.UpDatalabel(Product_NG_20_21.ToString(), "lb_Product_NG_20_21");
                    _datastatisticsfrm.UpDatalabel(Product_NG_21_22.ToString(), "lb_Product_NG_21_22");
                    _datastatisticsfrm.UpDatalabel(Product_NG_22_23.ToString(), "lb_Product_NG_22_23");
                    _datastatisticsfrm.UpDatalabel(Product_NG_23_00.ToString(), "lb_Product_NG_23_00");
                    _datastatisticsfrm.UpDatalabel(Product_NG_00_01.ToString(), "lb_Product_NG_00_01");
                    _datastatisticsfrm.UpDatalabel(Product_NG_01_02.ToString(), "lb_Product_NG_01_02");
                    _datastatisticsfrm.UpDatalabel(Product_NG_02_03.ToString(), "lb_Product_NG_02_03");
                    _datastatisticsfrm.UpDatalabel(Product_NG_03_04.ToString(), "lb_Product_NG_03_04");
                    _datastatisticsfrm.UpDatalabel(Product_NG_04_05.ToString(), "lb_Product_NG_04_05");
                    _datastatisticsfrm.UpDatalabel(Product_NG_05_06.ToString(), "lb_Product_NG_05_06");
                    _datastatisticsfrm.UpDatalabel(Product_NG_06_07.ToString(), "lb_Product_NG_06_07");
                    _datastatisticsfrm.UpDatalabel(Product_NG_07_08.ToString(), "lb_Product_NG_07_08");

                    _datastatisticsfrm.UpDatalabel(Product_Lianglv_20_21.ToString("0.00") + "%", "lb_Product_Lianglv_20_21");
                    _datastatisticsfrm.UpDatalabel(Product_Lianglv_21_22.ToString("0.00") + "%", "lb_Product_Lianglv_21_22");
                    _datastatisticsfrm.UpDatalabel(Product_Lianglv_22_23.ToString("0.00") + "%", "lb_Product_Lianglv_22_23");
                    _datastatisticsfrm.UpDatalabel(Product_Lianglv_23_00.ToString("0.00") + "%", "lb_Product_Lianglv_23_00");
                    _datastatisticsfrm.UpDatalabel(Product_Lianglv_00_01.ToString("0.00") + "%", "lb_Product_Lianglv_00_01");
                    _datastatisticsfrm.UpDatalabel(Product_Lianglv_01_02.ToString("0.00") + "%", "lb_Product_Lianglv_01_02");
                    _datastatisticsfrm.UpDatalabel(Product_Lianglv_02_03.ToString("0.00") + "%", "lb_Product_Lianglv_02_03");
                    _datastatisticsfrm.UpDatalabel(Product_Lianglv_03_04.ToString("0.00") + "%", "lb_Product_Lianglv_03_04");
                    _datastatisticsfrm.UpDatalabel(Product_Lianglv_04_05.ToString("0.00") + "%", "lb_Product_Lianglv_04_05");
                    _datastatisticsfrm.UpDatalabel(Product_Lianglv_05_06.ToString("0.00") + "%", "lb_Product_Lianglv_05_06");
                    _datastatisticsfrm.UpDatalabel(Product_Lianglv_06_07.ToString("0.00") + "%", "lb_Product_Lianglv_06_07");
                    _datastatisticsfrm.UpDatalabel(Product_Lianglv_07_08.ToString("0.00") + "%", "lb_Product_Lianglv_07_08");

                    _datastatisticsfrm.UpDatalabel(Product_Total_20_08.ToString(), "lb_Product_Total_20_08");
                    _datastatisticsfrm.UpDatalabel(Product_NG_20_08.ToString(), "lb_Product_NG_20_08");
                    _datastatisticsfrm.UpDatalabel(Product_Lianglv_20_08.ToString("0.00") + "%", "lb_Product_Lianglv_20_08");

                    _datastatisticsfrm.UpDataDGV_N(0, 1, Global.Product_Total_N.ToString());
                    _datastatisticsfrm.UpDataDGV_N(1, 1, Global.inidata.productconfig.Smallmaterial_Input_N.ToString());
                    _datastatisticsfrm.UpDataDGV_N(4, 1, Global.inidata.productconfig.location1_CCDNG_N);
                    double location1_CCDNG_N = (Convert.ToDouble(Global.inidata.productconfig.location1_CCDNG_N) / Convert.ToDouble(Global.Product_Total_N)) * 100;
                    _datastatisticsfrm.UpDataDGV_N(4, 2, location1_CCDNG_N.ToString("0.00") + "%");
                    _datastatisticsfrm.UpDataDGV_N(5, 1, Global.inidata.productconfig.location2_CCDNG_N);
                    double location2_CCDNG_N = (Convert.ToDouble(Global.inidata.productconfig.location2_CCDNG_N) / Convert.ToDouble(Global.Product_Total_N)) * 100;
                    _datastatisticsfrm.UpDataDGV_N(5, 2, location2_CCDNG_N.ToString("0.00") + "%");
                    _datastatisticsfrm.UpDataDGV_N(6, 1, Global.inidata.productconfig.location3_CCDNG_N);
                    double location3_CCDNG_N = (Convert.ToDouble(Global.inidata.productconfig.location3_CCDNG_N) / Convert.ToDouble(Global.Product_Total_N)) * 100;
                    _datastatisticsfrm.UpDataDGV_N(6, 2, location3_CCDNG_N.ToString("0.00") + "%");
                    //_datastatisticsfrm.UpDataDGV_N(7, 1, Global.inidata.productconfig.location4_CCDNG_N);
                    //double location4_CCDNG_N = (Convert.ToDouble(Global.inidata.productconfig.location4_CCDNG_N) / Convert.ToDouble(Global.Product_Total_N)) * 100;
                    //_datastatisticsfrm.UpDataDGV_N(7, 2, location4_CCDNG_N.ToString("0.00") + "%");
                    //_datastatisticsfrm.UpDataDGV_N(8, 1, Global.inidata.productconfig.location5_CCDNG_N);
                    //double location5_CCDNG_N = (Convert.ToDouble(Global.inidata.productconfig.location5_CCDNG_N) / Convert.ToDouble(Global.Product_Total_N)) * 100;
                    //_datastatisticsfrm.UpDataDGV_N(8, 2, location5_CCDNG_N.ToString("0.00") + "%");
                    _datastatisticsfrm.UpDataDGV_N(7, 1, Global.inidata.productconfig.HansDataError_N);
                    double HansDataError_N = (Convert.ToDouble(Global.inidata.productconfig.HansDataError_N) / Convert.ToDouble(Global.Product_Total_N)) * 100;
                    _datastatisticsfrm.UpDataDGV_N(7, 2, HansDataError_N.ToString("0.00") + "%");
                    _datastatisticsfrm.UpDataDGV_N(10, 1, Global.inidata.productconfig.TraceUpLoad_Error_N);
                    double TraceUpLoad_Error_N = (Convert.ToDouble(Global.inidata.productconfig.TraceUpLoad_Error_N) / Global.Product_Total_N) * 100;
                    _datastatisticsfrm.UpDataDGV_N(10, 2, TraceUpLoad_Error_N.ToString("0.00") + "%");
                    _datastatisticsfrm.UpDataDGV_N(11, 1, Global.inidata.productconfig.PDCAUpLoad_Error_N);
                    double PDCAUpLoad_Error_N = (Convert.ToDouble(Global.inidata.productconfig.PDCAUpLoad_Error_N) / Global.Product_Total_N) * 100;
                    _datastatisticsfrm.UpDataDGV_N(11, 2, PDCAUpLoad_Error_N.ToString("0.00") + "%");
                    _datastatisticsfrm.UpDataDGV_N(12, 1, Global.inidata.productconfig.TracePVCheck_Error_N);
                    double TracePVCheck_Error_N = (Convert.ToDouble(Global.inidata.productconfig.TracePVCheck_Error_N) / Global.Product_Total_N) * 100;
                    _datastatisticsfrm.UpDataDGV_N(12, 2, TracePVCheck_Error_N.ToString("0.00") + "%");
                    _datastatisticsfrm.UpDataDGV_N(13, 1, Global.inidata.productconfig.ReadBarcode_NG_N);
                    double ReadBarcode_NG_N = (Convert.ToDouble(Global.inidata.productconfig.ReadBarcode_NG_N) / Global.Product_Total_N) * 100;
                    _datastatisticsfrm.UpDataDGV_N(13, 2, ReadBarcode_NG_N.ToString("0.00") + "%");

                    _datastatisticsfrm.UpDataDGV_N(16, 1, Global.inidata.productconfig.TraceTab_Error_N);
                    double TraceTab_Error_N = (Convert.ToDouble(Global.inidata.productconfig.TraceTab_Error_N) / Convert.ToDouble(Global.Product_Total_N)) * 100;
                    _datastatisticsfrm.UpDataDGV_N(16, 2, TraceTab_Error_N.ToString("0.00") + "%");
                    _datastatisticsfrm.UpDataDGV_N(17, 1, Global.inidata.productconfig.TraceThench_Error_N);
                    double TraceThench_Error_N = (Convert.ToDouble(Global.inidata.productconfig.TraceThench_Error_N) / Convert.ToDouble(Global.Product_Total_N)) * 100;
                    _datastatisticsfrm.UpDataDGV_N(17, 2, TraceThench_Error_N.ToString("0.00") + "%");
                }
                else
                {
                    _datastatisticsfrm.UpDatalabel("0", "lb_Product_Total_20_21");
                    _datastatisticsfrm.UpDatalabel("0", "lb_Product_Total_21_22");
                    _datastatisticsfrm.UpDatalabel("0", "lb_Product_Total_22_23");
                    _datastatisticsfrm.UpDatalabel("0", "lb_Product_Total_23_00");
                    _datastatisticsfrm.UpDatalabel("0", "lb_Product_Total_00_01");
                    _datastatisticsfrm.UpDatalabel("0", "lb_Product_Total_01_02");
                    _datastatisticsfrm.UpDatalabel("0", "lb_Product_Total_02_03");
                    _datastatisticsfrm.UpDatalabel("0", "lb_Product_Total_03_04");
                    _datastatisticsfrm.UpDatalabel("0", "lb_Product_Total_04_05");
                    _datastatisticsfrm.UpDatalabel("0", "lb_Product_Total_05_06");
                    _datastatisticsfrm.UpDatalabel("0", "lb_Product_Total_06_07");
                    _datastatisticsfrm.UpDatalabel("0", "lb_Product_Total_07_08");

                    _datastatisticsfrm.UpDatalabel("0", "lb_Product_NG_20_21");
                    _datastatisticsfrm.UpDatalabel("0", "lb_Product_NG_21_22");
                    _datastatisticsfrm.UpDatalabel("0", "lb_Product_NG_22_23");
                    _datastatisticsfrm.UpDatalabel("0", "lb_Product_NG_23_00");
                    _datastatisticsfrm.UpDatalabel("0", "lb_Product_NG_00_01");
                    _datastatisticsfrm.UpDatalabel("0", "lb_Product_NG_01_02");
                    _datastatisticsfrm.UpDatalabel("0", "lb_Product_NG_02_03");
                    _datastatisticsfrm.UpDatalabel("0", "lb_Product_NG_03_04");
                    _datastatisticsfrm.UpDatalabel("0", "lb_Product_NG_04_05");
                    _datastatisticsfrm.UpDatalabel("0", "lb_Product_NG_05_06");
                    _datastatisticsfrm.UpDatalabel("0", "lb_Product_NG_06_07");
                    _datastatisticsfrm.UpDatalabel("0", "lb_Product_NG_07_08");

                    _datastatisticsfrm.UpDatalabel("0.00%", "lb_Product_Lianglv_20_21");
                    _datastatisticsfrm.UpDatalabel("0.00%", "lb_Product_Lianglv_21_22");
                    _datastatisticsfrm.UpDatalabel("0.00%", "lb_Product_Lianglv_22_23");
                    _datastatisticsfrm.UpDatalabel("0.00%", "lb_Product_Lianglv_23_00");
                    _datastatisticsfrm.UpDatalabel("0.00%", "lb_Product_Lianglv_00_01");
                    _datastatisticsfrm.UpDatalabel("0.00%", "lb_Product_Lianglv_01_02");
                    _datastatisticsfrm.UpDatalabel("0.00%", "lb_Product_Lianglv_02_03");
                    _datastatisticsfrm.UpDatalabel("0.00%", "lb_Product_Lianglv_03_04");
                    _datastatisticsfrm.UpDatalabel("0.00%", "lb_Product_Lianglv_04_05");
                    _datastatisticsfrm.UpDatalabel("0.00%", "lb_Product_Lianglv_05_06");
                    _datastatisticsfrm.UpDatalabel("0.00%", "lb_Product_Lianglv_06_07");
                    _datastatisticsfrm.UpDatalabel("0.00%", "lb_Product_Lianglv_07_08");

                    _datastatisticsfrm.UpDatalabel("0", "lb_Product_Total_20_08");
                    _datastatisticsfrm.UpDatalabel("0", "lb_Product_NG_20_08");
                    _datastatisticsfrm.UpDatalabel("0.00%", "lb_Product_Lianglv_20_08");

                    _datastatisticsfrm.UpDataDGV_N(0, 1, "0");
                    _datastatisticsfrm.UpDataDGV_N(1, 1, "0");

                    _datastatisticsfrm.UpDataDGV_N(4, 1, "0");
                    _datastatisticsfrm.UpDataDGV_N(4, 2, "0.00%");
                    _datastatisticsfrm.UpDataDGV_N(5, 1, "0");
                    _datastatisticsfrm.UpDataDGV_N(5, 2, "0.00%");
                    _datastatisticsfrm.UpDataDGV_N(6, 1, "0");
                    _datastatisticsfrm.UpDataDGV_N(6, 2, "0.00%");
                    _datastatisticsfrm.UpDataDGV_N(7, 1, "0");
                    _datastatisticsfrm.UpDataDGV_N(7, 2, "0.00%");

                    _datastatisticsfrm.UpDataDGV_N(10, 1, "0");
                    _datastatisticsfrm.UpDataDGV_N(10, 2, "0.00%");
                    _datastatisticsfrm.UpDataDGV_N(11, 1, "0");
                    _datastatisticsfrm.UpDataDGV_N(11, 2, "0.00%");
                    _datastatisticsfrm.UpDataDGV_N(12, 1, "0");
                    _datastatisticsfrm.UpDataDGV_N(12, 2, "0.00%");
                    _datastatisticsfrm.UpDataDGV_N(13, 1, "0");
                    _datastatisticsfrm.UpDataDGV_N(13, 2, "0.00%");

                    _datastatisticsfrm.UpDataDGV_N(16, 1, "0");
                    _datastatisticsfrm.UpDataDGV_N(16, 2, "0.00%");
                    _datastatisticsfrm.UpDataDGV_N(17, 1, "0");
                    _datastatisticsfrm.UpDataDGV_N(17, 2, "0.00%");
                }
            }

        }

        private void DT_DataStatistics()//运行状态统计
        {
            //-------------------------------白班DT统计--------------------------------------------
            Global.DT_RunTime = Global.PLC_Client.ReadPLC_D(2156, 12);//白班运行时间
            Global.DT_ErrorTime = Global.PLC_Client.ReadPLC_D(2206, 12);//白班异常时间
            Global.DT_PendingTime = Global.PLC_Client.ReadPLC_D(2256, 12);//白班待料时间
            short DT_RunTime_08_09 = Global.DT_RunTime[0];//白班8-9点运行时间
            short DT_RunTime_09_10 = Global.DT_RunTime[1];
            short DT_RunTime_10_11 = Global.DT_RunTime[2];
            short DT_RunTime_11_12 = Global.DT_RunTime[3];
            short DT_RunTime_12_13 = Global.DT_RunTime[4];
            short DT_RunTime_13_14 = Global.DT_RunTime[5];
            short DT_RunTime_14_15 = Global.DT_RunTime[6];
            short DT_RunTime_15_16 = Global.DT_RunTime[7];
            short DT_RunTime_16_17 = Global.DT_RunTime[8];
            short DT_RunTime_17_18 = Global.DT_RunTime[9];
            short DT_RunTime_18_19 = Global.DT_RunTime[10];
            short DT_RunTime_19_20 = Global.DT_RunTime[11];//白班19-20点运行时间
            short DT_ErrorTime_08_09 = Global.DT_ErrorTime[0];//白班8-9点异常时间
            short DT_ErrorTime_09_10 = Global.DT_ErrorTime[1];
            short DT_ErrorTime_10_11 = Global.DT_ErrorTime[2];
            short DT_ErrorTime_11_12 = Global.DT_ErrorTime[3];
            short DT_ErrorTime_12_13 = Global.DT_ErrorTime[4];
            short DT_ErrorTime_13_14 = Global.DT_ErrorTime[5];
            short DT_ErrorTime_14_15 = Global.DT_ErrorTime[6];
            short DT_ErrorTime_15_16 = Global.DT_ErrorTime[7];
            short DT_ErrorTime_16_17 = Global.DT_ErrorTime[8];
            short DT_ErrorTime_17_18 = Global.DT_ErrorTime[9];
            short DT_ErrorTime_18_19 = Global.DT_ErrorTime[10];
            short DT_ErrorTime_19_20 = Global.DT_ErrorTime[11];//白班19-20点异常时间
            short DT_PendingTime_08_09 = Global.DT_PendingTime[0];//白班8-9点待料时间
            short DT_PendingTime_09_10 = Global.DT_PendingTime[1];
            short DT_PendingTime_10_11 = Global.DT_PendingTime[2];
            short DT_PendingTime_11_12 = Global.DT_PendingTime[3];
            short DT_PendingTime_12_13 = Global.DT_PendingTime[4];
            short DT_PendingTime_13_14 = Global.DT_PendingTime[5];
            short DT_PendingTime_14_15 = Global.DT_PendingTime[6];
            short DT_PendingTime_15_16 = Global.DT_PendingTime[7];
            short DT_PendingTime_16_17 = Global.DT_PendingTime[8];
            short DT_PendingTime_17_18 = Global.DT_PendingTime[9];
            short DT_PendingTime_18_19 = Global.DT_PendingTime[10];
            short DT_PendingTime_19_20 = Global.DT_PendingTime[11];//白班19-20点待料时间
            short DT_RunTime_08_20 = Global.PLC_Client.ReadPLC_D(2176, 1)[0];//白班总运行时间
            short DT_ErrorTime_08_20 = Global.PLC_Client.ReadPLC_D(2226, 1)[0];//白班总异常时间
            short DT_PendingTime_08_20 = Global.PLC_Client.ReadPLC_D(2276, 1)[0];//白班总待料时间
            short DT_jiadonglv_08_20 = Global.PLC_Client.ReadPLC_D(4224, 1)[0];//白班稼动率时间
            if (DateTime.Now.ToString("yyyy-MM-dd") == Global.SelectDTTime.ToString("yyyy-MM-dd"))
            {
                _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_RunTime_08_09)).ToString("0.00"), "lb_RunTime_08_09");
                _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_RunTime_09_10)).ToString("0.00"), "lb_RunTime_09_10");
                _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_RunTime_10_11)).ToString("0.00"), "lb_RunTime_10_11");
                _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_RunTime_11_12)).ToString("0.00"), "lb_RunTime_11_12");
                _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_RunTime_12_13)).ToString("0.00"), "lb_RunTime_12_13");
                _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_RunTime_13_14)).ToString("0.00"), "lb_RunTime_13_14");
                _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_RunTime_14_15)).ToString("0.00"), "lb_RunTime_14_15");
                _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_RunTime_15_16)).ToString("0.00"), "lb_RunTime_15_16");
                _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_RunTime_16_17)).ToString("0.00"), "lb_RunTime_16_17");
                _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_RunTime_17_18)).ToString("0.00"), "lb_RunTime_17_18");
                _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_RunTime_18_19)).ToString("0.00"), "lb_RunTime_18_19");
                _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_RunTime_19_20)).ToString("0.00"), "lb_RunTime_19_20");

                _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_ErrorTime_08_09)).ToString("0.00"), "lb_ErrorTime_08_09");
                _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_ErrorTime_09_10)).ToString("0.00"), "lb_ErrorTime_09_10");
                _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_ErrorTime_10_11)).ToString("0.00"), "lb_ErrorTime_10_11");
                _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_ErrorTime_11_12)).ToString("0.00"), "lb_ErrorTime_11_12");
                _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_ErrorTime_12_13)).ToString("0.00"), "lb_ErrorTime_12_13");
                _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_ErrorTime_13_14)).ToString("0.00"), "lb_ErrorTime_13_14");
                _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_ErrorTime_14_15)).ToString("0.00"), "lb_ErrorTime_14_15");
                _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_ErrorTime_15_16)).ToString("0.00"), "lb_ErrorTime_15_16");
                _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_ErrorTime_16_17)).ToString("0.00"), "lb_ErrorTime_16_17");
                _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_ErrorTime_17_18)).ToString("0.00"), "lb_ErrorTime_17_18");
                _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_ErrorTime_18_19)).ToString("0.00"), "lb_ErrorTime_18_19");
                _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_ErrorTime_19_20)).ToString("0.00"), "lb_ErrorTime_19_20");

                _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_PendingTime_08_09)).ToString("0.00"), "lb_PendingTime_08_09");
                _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_PendingTime_09_10)).ToString("0.00"), "lb_PendingTime_09_10");
                _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_PendingTime_10_11)).ToString("0.00"), "lb_PendingTime_10_11");
                _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_PendingTime_11_12)).ToString("0.00"), "lb_PendingTime_11_12");
                _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_PendingTime_12_13)).ToString("0.00"), "lb_PendingTime_12_13");
                _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_PendingTime_13_14)).ToString("0.00"), "lb_PendingTime_13_14");
                _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_PendingTime_14_15)).ToString("0.00"), "lb_PendingTime_14_15");
                _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_PendingTime_15_16)).ToString("0.00"), "lb_PendingTime_15_16");
                _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_PendingTime_16_17)).ToString("0.00"), "lb_PendingTime_16_17");
                _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_PendingTime_17_18)).ToString("0.00"), "lb_PendingTime_17_18");
                _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_PendingTime_18_19)).ToString("0.00"), "lb_PendingTime_18_19");
                _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_PendingTime_19_20)).ToString("0.00"), "lb_PendingTime_19_20");

                _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_RunTime_08_20)).ToString("0.00"), "lb_RunTime_08_20");
                _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_ErrorTime_08_20)).ToString("0.00"), "lb_ErrorTime_08_20");
                _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_PendingTime_08_20)).ToString("0.00"), "lb_PendingTime_08_20");
                _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_jiadonglv_08_20) / 100).ToString("0.00") + "%", "lb_jiadonglv_08_20");
            }

            //-------------------------------夜班DT统计--------------------------------------------
            Global.DT_RunTime_N1 = Global.PLC_Client.ReadPLC_D(2168, 6);//夜班运行时间
            Global.DT_RunTime_N2 = Global.PLC_Client.ReadPLC_D(2150, 6);//夜班运行时间
            Global.DT_ErrorTime_N1 = Global.PLC_Client.ReadPLC_D(2218, 6);//夜班异常时间
            Global.DT_ErrorTime_N2 = Global.PLC_Client.ReadPLC_D(2200, 6);//夜班异常时间
            Global.DT_PendingTime_N1 = Global.PLC_Client.ReadPLC_D(2268, 6);//夜班待料时间
            Global.DT_PendingTime_N2 = Global.PLC_Client.ReadPLC_D(2250, 6);//夜班待料时间
            short DT_RunTime_20_21 = Global.DT_RunTime_N1[0];
            short DT_RunTime_21_22 = Global.DT_RunTime_N1[1];
            short DT_RunTime_22_23 = Global.DT_RunTime_N1[2];
            short DT_RunTime_23_00 = Global.DT_RunTime_N1[3];
            short DT_RunTime_00_01 = Global.DT_RunTime_N1[4];
            short DT_RunTime_01_02 = Global.DT_RunTime_N1[5];
            short DT_RunTime_02_03 = Global.DT_RunTime_N2[0];
            short DT_RunTime_03_04 = Global.DT_RunTime_N2[1];
            short DT_RunTime_04_05 = Global.DT_RunTime_N2[2];
            short DT_RunTime_05_06 = Global.DT_RunTime_N2[3];
            short DT_RunTime_06_07 = Global.DT_RunTime_N2[4];
            short DT_RunTime_07_08 = Global.DT_RunTime_N2[5];
            short DT_ErrorTime_20_21 = Global.DT_ErrorTime_N1[0];
            short DT_ErrorTime_21_22 = Global.DT_ErrorTime_N1[1];
            short DT_ErrorTime_22_23 = Global.DT_ErrorTime_N1[2];
            short DT_ErrorTime_23_00 = Global.DT_ErrorTime_N1[3];
            short DT_ErrorTime_00_01 = Global.DT_ErrorTime_N1[4];
            short DT_ErrorTime_01_02 = Global.DT_ErrorTime_N1[5];
            short DT_ErrorTime_02_03 = Global.DT_ErrorTime_N2[0];
            short DT_ErrorTime_03_04 = Global.DT_ErrorTime_N2[1];
            short DT_ErrorTime_04_05 = Global.DT_ErrorTime_N2[2];
            short DT_ErrorTime_05_06 = Global.DT_ErrorTime_N2[3];
            short DT_ErrorTime_06_07 = Global.DT_ErrorTime_N2[4];
            short DT_ErrorTime_07_08 = Global.DT_ErrorTime_N2[5];
            short DT_PendingTime_20_21 = Global.DT_PendingTime_N1[0];
            short DT_PendingTime_21_22 = Global.DT_PendingTime_N1[1];
            short DT_PendingTime_22_23 = Global.DT_PendingTime_N1[2];
            short DT_PendingTime_23_00 = Global.DT_PendingTime_N1[3];
            short DT_PendingTime_00_01 = Global.DT_PendingTime_N1[4];
            short DT_PendingTime_01_02 = Global.DT_PendingTime_N1[5];
            short DT_PendingTime_02_03 = Global.DT_PendingTime_N2[0];
            short DT_PendingTime_03_04 = Global.DT_PendingTime_N2[1];
            short DT_PendingTime_04_05 = Global.DT_PendingTime_N2[2];
            short DT_PendingTime_05_06 = Global.DT_PendingTime_N2[3];
            short DT_PendingTime_06_07 = Global.DT_PendingTime_N2[4];
            short DT_PendingTime_07_08 = Global.DT_PendingTime_N2[5];
            short DT_RunTime_20_08 = Global.PLC_Client.ReadPLC_D(2178, 1)[0];//夜班总运行时间
            short DT_ErrorTime_20_08 = Global.PLC_Client.ReadPLC_D(2228, 1)[0];//夜班总异常时间
            short DT_PendingTime_20_08 = Global.PLC_Client.ReadPLC_D(2278, 1)[0];//夜班总待料时间
            short DT_jiadonglv_20_08 = Global.PLC_Client.ReadPLC_D(4254, 1)[0];//夜班稼动率时间
            if (DateTime.Now.ToString("yyyy-MM-dd") == Global.SelectDTTime.ToString("yyyy-MM-dd"))
            {
                if (Convert.ToDateTime(DateTime.Now.ToString("HH:mm")).CompareTo(Convert.ToDateTime("18:00")) >= 0 || Convert.ToDateTime(DateTime.Now.ToString("HH:mm")).CompareTo(Convert.ToDateTime("6:00")) < 0)
                {
                    _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_RunTime_20_21)).ToString("0.00"), "lb_RunTime_20_21");
                    _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_RunTime_21_22)).ToString("0.00"), "lb_RunTime_21_22");
                    _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_RunTime_22_23)).ToString("0.00"), "lb_RunTime_22_23");
                    _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_RunTime_23_00)).ToString("0.00"), "lb_RunTime_23_00");
                    _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_RunTime_00_01)).ToString("0.00"), "lb_RunTime_00_01");
                    _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_RunTime_01_02)).ToString("0.00"), "lb_RunTime_01_02");
                    _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_RunTime_02_03)).ToString("0.00"), "lb_RunTime_02_03");
                    _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_RunTime_03_04)).ToString("0.00"), "lb_RunTime_03_04");
                    _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_RunTime_04_05)).ToString("0.00"), "lb_RunTime_04_05");
                    _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_RunTime_05_06)).ToString("0.00"), "lb_RunTime_05_06");
                    _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_RunTime_06_07)).ToString("0.00"), "lb_RunTime_06_07");
                    _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_RunTime_07_08)).ToString("0.00"), "lb_RunTime_07_08");

                    _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_ErrorTime_20_21)).ToString("0.00"), "lb_ErrorTime_20_21");
                    _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_ErrorTime_21_22)).ToString("0.00"), "lb_ErrorTime_21_22");
                    _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_ErrorTime_22_23)).ToString("0.00"), "lb_ErrorTime_22_23");
                    _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_ErrorTime_23_00)).ToString("0.00"), "lb_ErrorTime_23_00");
                    _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_ErrorTime_00_01)).ToString("0.00"), "lb_ErrorTime_00_01");
                    _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_ErrorTime_01_02)).ToString("0.00"), "lb_ErrorTime_01_02");
                    _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_ErrorTime_02_03)).ToString("0.00"), "lb_ErrorTime_02_03");
                    _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_ErrorTime_03_04)).ToString("0.00"), "lb_ErrorTime_03_04");
                    _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_ErrorTime_04_05)).ToString("0.00"), "lb_ErrorTime_04_05");
                    _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_ErrorTime_05_06)).ToString("0.00"), "lb_ErrorTime_05_06");
                    _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_ErrorTime_06_07)).ToString("0.00"), "lb_ErrorTime_06_07");
                    _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_ErrorTime_07_08)).ToString("0.00"), "lb_ErrorTime_07_08");

                    _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_PendingTime_20_21)).ToString("0.00"), "lb_PendingTime_20_21");
                    _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_PendingTime_21_22)).ToString("0.00"), "lb_PendingTime_21_22");
                    _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_PendingTime_22_23)).ToString("0.00"), "lb_PendingTime_22_23");
                    _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_PendingTime_23_00)).ToString("0.00"), "lb_PendingTime_23_00");
                    _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_PendingTime_00_01)).ToString("0.00"), "lb_PendingTime_00_01");
                    _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_PendingTime_01_02)).ToString("0.00"), "lb_PendingTime_01_02");
                    _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_PendingTime_02_03)).ToString("0.00"), "lb_PendingTime_02_03");
                    _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_PendingTime_03_04)).ToString("0.00"), "lb_PendingTime_03_04");
                    _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_PendingTime_04_05)).ToString("0.00"), "lb_PendingTime_04_05");
                    _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_PendingTime_05_06)).ToString("0.00"), "lb_PendingTime_05_06");
                    _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_PendingTime_06_07)).ToString("0.00"), "lb_PendingTime_06_07");
                    _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_PendingTime_07_08)).ToString("0.00"), "lb_PendingTime_07_08");

                    _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_RunTime_20_08)).ToString("0.00"), "lb_RunTime_20_08");
                    _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_ErrorTime_20_08)).ToString("0.00"), "lb_ErrorTime_20_08");
                    _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_PendingTime_20_08)).ToString("0.00"), "lb_PendingTime_20_08");
                    _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_jiadonglv_20_08) / 100).ToString("0.00") + "%", "lb_jiadonglv_20_08");
                }
                else
                {
                    _datastatisticsfrm.UpDatalabel("0", "lb_RunTime_20_21");
                    _datastatisticsfrm.UpDatalabel("0", "lb_RunTime_21_22");
                    _datastatisticsfrm.UpDatalabel("0", "lb_RunTime_22_23");
                    _datastatisticsfrm.UpDatalabel("0", "lb_RunTime_23_00");
                    _datastatisticsfrm.UpDatalabel("0", "lb_RunTime_00_01");
                    _datastatisticsfrm.UpDatalabel("0", "lb_RunTime_01_02");
                    _datastatisticsfrm.UpDatalabel("0", "lb_RunTime_02_03");
                    _datastatisticsfrm.UpDatalabel("0", "lb_RunTime_03_04");
                    _datastatisticsfrm.UpDatalabel("0", "lb_RunTime_04_05");
                    _datastatisticsfrm.UpDatalabel("0", "lb_RunTime_05_06");
                    _datastatisticsfrm.UpDatalabel("0", "lb_RunTime_06_07");
                    _datastatisticsfrm.UpDatalabel("0", "lb_RunTime_07_08");

                    _datastatisticsfrm.UpDatalabel("0", "lb_ErrorTime_20_21");
                    _datastatisticsfrm.UpDatalabel("0", "lb_ErrorTime_21_22");
                    _datastatisticsfrm.UpDatalabel("0", "lb_ErrorTime_22_23");
                    _datastatisticsfrm.UpDatalabel("0", "lb_ErrorTime_23_00");
                    _datastatisticsfrm.UpDatalabel("0", "lb_ErrorTime_00_01");
                    _datastatisticsfrm.UpDatalabel("0", "lb_ErrorTime_01_02");
                    _datastatisticsfrm.UpDatalabel("0", "lb_ErrorTime_02_03");
                    _datastatisticsfrm.UpDatalabel("0", "lb_ErrorTime_03_04");
                    _datastatisticsfrm.UpDatalabel("0", "lb_ErrorTime_04_05");
                    _datastatisticsfrm.UpDatalabel("0", "lb_ErrorTime_05_06");
                    _datastatisticsfrm.UpDatalabel("0", "lb_ErrorTime_06_07");
                    _datastatisticsfrm.UpDatalabel("0", "lb_ErrorTime_07_08");

                    _datastatisticsfrm.UpDatalabel("0", "lb_PendingTime_20_21");
                    _datastatisticsfrm.UpDatalabel("0", "lb_PendingTime_21_22");
                    _datastatisticsfrm.UpDatalabel("0", "lb_PendingTime_22_23");
                    _datastatisticsfrm.UpDatalabel("0", "lb_PendingTime_23_00");
                    _datastatisticsfrm.UpDatalabel("0", "lb_PendingTime_00_01");
                    _datastatisticsfrm.UpDatalabel("0", "lb_PendingTime_01_02");
                    _datastatisticsfrm.UpDatalabel("0", "lb_PendingTime_02_03");
                    _datastatisticsfrm.UpDatalabel("0", "lb_PendingTime_03_04");
                    _datastatisticsfrm.UpDatalabel("0", "lb_PendingTime_04_05");
                    _datastatisticsfrm.UpDatalabel("0", "lb_PendingTime_05_06");
                    _datastatisticsfrm.UpDatalabel("0", "lb_PendingTime_06_07");
                    _datastatisticsfrm.UpDatalabel("0", "lb_PendingTime_07_08");

                    _datastatisticsfrm.UpDatalabel("0", "lb_RunTime_20_08");
                    _datastatisticsfrm.UpDatalabel("0", "lb_ErrorTime_20_08");
                    _datastatisticsfrm.UpDatalabel("0", "lb_PendingTime_20_08");
                    _datastatisticsfrm.UpDatalabel("0.00%", "lb_jiadonglv_20_08");
                }
            }

        }

        private void Dumpingstatistics()
        {
            Global.Product_Throwing_Total = Global.PLC_Client.ReadPLC_D(2030, 12);//白班小时抛料总数
            Global.Product_Throwing_NG = Global.PLC_Client.ReadPLC_D(2056, 12);//白班抛料

            short Product_Throwing_08_09 = Global.Product_Throwing_Total[0];//白班8-9点总产能
            short Product_Throwing_09_10 = Global.Product_Throwing_Total[1];
            short Product_Throwing_10_11 = Global.Product_Throwing_Total[2];
            short Product_Throwing_11_12 = Global.Product_Throwing_Total[3];
            short Product_Throwing_12_13 = Global.Product_Throwing_Total[4];
            short Product_Throwing_13_14 = Global.Product_Throwing_Total[5];
            short Product_Throwing_14_15 = Global.Product_Throwing_Total[6];
            short Product_Throwing_15_16 = Global.Product_Throwing_Total[7];
            short Product_Throwing_16_17 = Global.Product_Throwing_Total[8];
            short Product_Throwing_17_18 = Global.Product_Throwing_Total[9];
            short Product_Throwing_18_19 = Global.Product_Throwing_Total[10];
            short Product_Throwing_19_20 = Global.Product_Throwing_Total[11];//白班19-20点总产能

            Global.Product_Throwing_08_20 = Global.PLC_Client.ReadPLC_D(1910, 1)[0];//白班总抛料
            Global.Throwing_D = Global.PLC_Client.ReadPLC_D(1910, 1);
            Global.Welding_D = Global.PLC_Client.ReadPLC_D(1910, 1);

            if (DateTime.Now.ToString("yyyy-MM-dd") == Global.SelectDateTime.ToString("yyyy-MM-dd"))
            {
                _datastatisticsfrm.UpDatalabel(Product_Throwing_08_09.ToString(), "lb_Throwing_08_09");
                _datastatisticsfrm.UpDatalabel(Product_Throwing_09_10.ToString(), "lb_Throwing_09_10");
                _datastatisticsfrm.UpDatalabel(Product_Throwing_10_11.ToString(), "lb_Throwing_10_11");
                _datastatisticsfrm.UpDatalabel(Product_Throwing_11_12.ToString(), "lb_Throwing_11_12");
                _datastatisticsfrm.UpDatalabel(Product_Throwing_12_13.ToString(), "lb_Throwing_12_13");
                _datastatisticsfrm.UpDatalabel(Product_Throwing_13_14.ToString(), "lb_Throwing_13_14");
                _datastatisticsfrm.UpDatalabel(Product_Throwing_14_15.ToString(), "lb_Throwing_14_15");
                _datastatisticsfrm.UpDatalabel(Product_Throwing_15_16.ToString(), "lb_Throwing_15_16");
                _datastatisticsfrm.UpDatalabel(Product_Throwing_16_17.ToString(), "lb_Throwing_16_17");
                _datastatisticsfrm.UpDatalabel(Product_Throwing_17_18.ToString(), "lb_Throwing_17_18");
                _datastatisticsfrm.UpDatalabel(Product_Throwing_18_19.ToString(), "lb_Throwing_18_19");
                _datastatisticsfrm.UpDatalabel(Product_Throwing_19_20.ToString(), "lb_Throwing_19_20");

                _datastatisticsfrm.UpDatalabel(Global.Product_Throwing_08_20.ToString(), "lb_Product_Throwing_08_20");

                _datastatisticsfrm.UpData_Throwing_DGV_D(0, 1, Global.Product_Throwing_08_20.ToString());
                _datastatisticsfrm.UpData_Throwing_DGV_D(0, 1, Global.Throwing_D.ToString());
                _datastatisticsfrm.UpData_Throwing_DGV_N(1, 1, Global.Welding_D.ToString());

            }

            //-------------------------------夜班产能统计--------------------------------------------
            Global.Product_Throwing_Total_N = Global.PLC_Client.ReadPLC_D(2030, 12);//夜班小时抛料总数
            Global.Product_Throwing_NG = Global.PLC_Client.ReadPLC_D(2056, 12);//夜班抛料

            short Product_Throwing_20_21 = Global.Product_Throwing_Total_N[0];//夜班8-9点总产能
            short Product_Throwing_21_22 = Global.Product_Throwing_Total_N[1];
            short Product_Throwing_22_23 = Global.Product_Throwing_Total_N[2];
            short Product_Throwing_23_24 = Global.Product_Throwing_Total_N[3];
            short Product_Throwing_00_01 = Global.Product_Throwing_Total_N[4];
            short Product_Throwing_01_02 = Global.Product_Throwing_Total_N[5];
            short Product_Throwing_02_03 = Global.Product_Throwing_Total_N[6];
            short Product_Throwing_03_04 = Global.Product_Throwing_Total_N[7];
            short Product_Throwing_04_05 = Global.Product_Throwing_Total_N[8];
            short Product_Throwing_05_06 = Global.Product_Throwing_Total_N[9];
            short Product_Throwing_06_07 = Global.Product_Throwing_Total_N[10];
            short Product_Throwing_07_08 = Global.Product_Throwing_Total_N[11];//夜班19-20点总产能

            Global.Product_Throwing_08_20_N = Global.PLC_Client.ReadPLC_D(1910, 1)[0];//夜班总抛料
            Global.Throwing_N = Global.PLC_Client.ReadPLC_D(1910, 1);
            Global.Welding_N = Global.PLC_Client.ReadPLC_D(1910, 1);

            if (DateTime.Now.ToString("yyyy-MM-dd") == Global.SelectDateTime.ToString("yyyy-MM-dd"))
            {
                if (Convert.ToDateTime(DateTime.Now.ToString("HH:mm")).CompareTo(Convert.ToDateTime("18:00")) >= 0 || Convert.ToDateTime(DateTime.Now.ToString("HH:mm")).CompareTo(Convert.ToDateTime("6:00")) < 0)
                {
                    _datastatisticsfrm.UpDatalabel(Product_Throwing_20_21.ToString(), "lb_Throwing_20_21");
                    _datastatisticsfrm.UpDatalabel(Product_Throwing_21_22.ToString(), "lb_Throwing_21_22");
                    _datastatisticsfrm.UpDatalabel(Product_Throwing_22_23.ToString(), "lb_Throwing_22_23");
                    _datastatisticsfrm.UpDatalabel(Product_Throwing_23_24.ToString(), "lb_Throwing_23_24");
                    _datastatisticsfrm.UpDatalabel(Product_Throwing_00_01.ToString(), "lb_Throwing_00_01");
                    _datastatisticsfrm.UpDatalabel(Product_Throwing_01_02.ToString(), "lb_Throwing_01_02");
                    _datastatisticsfrm.UpDatalabel(Product_Throwing_02_03.ToString(), "lb_Throwing_02_03");
                    _datastatisticsfrm.UpDatalabel(Product_Throwing_03_04.ToString(), "lb_Throwing_03_04");
                    _datastatisticsfrm.UpDatalabel(Product_Throwing_04_05.ToString(), "lb_Throwing_04_05");
                    _datastatisticsfrm.UpDatalabel(Product_Throwing_05_06.ToString(), "lb_Throwing_05_06");
                    _datastatisticsfrm.UpDatalabel(Product_Throwing_06_07.ToString(), "lb_Throwing_06_07");
                    _datastatisticsfrm.UpDatalabel(Product_Throwing_07_08.ToString(), "lb_Throwing_07_08");

                    _datastatisticsfrm.UpDatalabel(Global.Product_Throwing_08_20_N.ToString(), "lb_Product_Throwing_20_08");

                    _datastatisticsfrm.UpData_Throwing_DGV_D(0, 1, Global.Product_Throwing_08_20_N.ToString());
                    _datastatisticsfrm.UpData_Throwing_DGV_D(0, 1, Global.Throwing_N.ToString());
                    _datastatisticsfrm.UpData_Throwing_DGV_N(1, 1, Global.Welding_N.ToString());

                }

            }
        }
    }
}
