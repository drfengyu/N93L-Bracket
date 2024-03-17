
using PlcComm;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uPLibrary.Networking.M2Mqtt;
using 卓汇数据追溯系统.ConfigHelper;
using 卓汇数据追溯系统.Models;

namespace 卓汇数据追溯系统
{
    public class Global
    {
      public static Gy LoginGy{set;get;}
        public static MaterialRule materialRule { set; get; }
        public static linkDatass linkDatas { set; get; }

        public static Dictionary<int, SearchModel> GySelected = new Dictionary<int, SearchModel>();
        public static void GetData()
        {
            
        }

        public static Bitmap ReadImageFile(string path)
        {
            if (!File.Exists(path))
            {
                return null;//文件不存在
            }
            FileStream fs = File.OpenRead(path); //OpenRead
            int filelength = 0;
            filelength = (int)fs.Length; //获得文件长度 
            Byte[] image = new Byte[filelength]; //建立一个字节数组 
            fs.Read(image, 0, filelength); //按字节流读取 
            System.Drawing.Image result = System.Drawing.Image.FromStream(fs);
            fs.Close();
            Bitmap bit = new Bitmap(result);
            return bit;
        }

       

        public enum LoginLevel
        {
            Operator,
            Technician,
            Administrator
        }
        
        public static string Operator_pwd = string.Empty;
        public static string Technician_pwd = string.Empty;
        public static string Administrator_pwd = string.Empty;
        public static LoginLevel Login;
        public static AsyncTcpClient client1;
        public static AsyncTcpClient client2;
        public static List<string> Setuplist = new List<string>();
        public static List<string> IQlist = new List<string>();
        public static List<string> OQlist = new List<string>();
        public static List<string> PQlist = new List<string>();
        public static List<string> MQlist = new List<string>();
        public static List<string> Productionlist = new List<string>();
        public static DateTime MouseMoveTime = DateTime.Now;
        public static DateTime UserLoginMouseMoveTime = DateTime.Now;
        public static string UserLoginTime ;//用户刷卡登入时间
        public static string ErrorMsg = "";
        public static int ErrorIndex = 0;
        public static bool Error_PendingStatus = false;//待料状态下是否开启安全门
        public static int j = -1;//OEE机台状态初始值
        public static Dictionary<int, ErrorData> ed = new Dictionary<int, ErrorData>();
        public static Dictionary<int, ErrorData> ED = new Dictionary<int, ErrorData>();
        public static Dictionary<int, PLCDataName> PLC_DataName = new Dictionary<int, PLCDataName>();
        //public static Dictionary<int, ModuleData> Modulecode = new Dictionary<int, ModuleData>();
        public static bool IfReadUserID = false;//标志位 判断打卡式刷卡机是否读卡
        public static bool IfLoginbtn = false;//标志位 判断是否按下登入按钮
        public static bool IfLogin = false;
        public static bool IfUserLogin = false; //标志位 判断是否已经有人登入
        public static bool IfPQStatus = false; //标志位 判断是否已经PQ approved
        public static bool b_VerifyResult = false;
        public static string FixtureOutID = string.Empty; //手动排出的治具号
        public static List<string> _listName = new List<string>(); //软件配置参数名称列表
        public static List<string> _listValue = new List<string>();//软件配置参数数值列表

        public static List<string> SN_string = new List<string>();//前站重码卡控 --10片
        public static string SN_out= "";
        public static bool SN_Request = false;
        public static bool Replace = false;



        public static bool bConnectedDevice = false;
        public static string Emp = string.Empty;
        public static string Name = "操作员";
        public static string Title = "Operator";
        public static string Count = "0";
        public static SerialPort _serial1;
        public static SerialPort _serialFixed ;
        public static int[] oldData = new int[700];
        public static int[] newData = new int[700];
        public static IniProductFile inidata;
        public static System.Timers.Timer timer;
        public static System.Timers.Timer EatAndRest_timer;        
        public static int Maintaintime_currentCount = 0;
        public static int currentCount = 0;
        public static int EatAndRest_currentCount = 0;
        public static int Trace_process_ok;
        public static int Trace_process_ng;
        public static int PDCA_ua_Data_NG;
        public static int oee_ok;// OEE发送成功次数
        public static int oee_ng;//OEE发送失败次数
        public static int oeeSend_ng = 0;
        public static int Trace_ua_ok;
        public static int Trace_ua_ng;
        public static int Trace_ua2_ok;
        public static int Trace_ua2_ng;
        public static int Product_num_ua_ok;
        public static int Product_num_ua_ng;
        public static int Error_num = 0;
        public static int Error_Stopnum = 0;
        public static int Error_PendingNum = 0;
        public static int TotalThrowCount = 0;
        public static int ThrowCount = 0;
        public static int ThrowOKCount = 0;
        public static int NutCount = 0;
        public static int NutOKCount = 0;
        public static int Fixture_ok = 0;
        public static int Fixture_ng = 0;
        public static int TraceSendua_ng = 0;

        public static int Smallmaterial_Input_D = 0;
        public static int Smallmaterial_Input_N = 0;
        public static int Smallmaterial_throwing_D = 0;
        public static int Smallmaterial_throwing_N = 0;
        public static int Product_Total_D = 0;
        public static int Product_Total_N = 0;
        public static int Product_OK_D = 0;
        public static int Product_OK_N = 0;
        public static int TraceUpLoad_Error_D = 0;
        public static int TraceUpLoad_Error_N = 0;
        public static int TraceTab_Error_D = 0;
        public static int TraceTab_Error_N = 0;
        public static int TraceThench_Error_D = 0;
        public static int TraceThench_Error_N = 0;
        public static int PDCAUpLoad_Error_D = 0;
        public static int PDCAUpLoad_Error_N = 0;
        public static int TracePVCheck_Error_D = 0;
        public static int TracePVCheck_Error_N = 0;
        public static int ReadBarcode_NG_D = 0;
        public static int ReadBarcode_NG_N = 0;
        public static int PictureUpLoad_Error_D = 0;
        public static int PictureUpLoad_Error_N = 0;
        public static int OEEUpLoad_Error_D = 0;
        public static int OEEUpLoad_Error_N = 0;
        public static int CCDCheck_Error_D = 0;
        public static int CCDCheck_Error_N = 0;
        public static int Welding_Error_D = 0;
        public static int HSG_NG_D = 0;
        public static int HSG_NG_N = 0;
        public static int location1_CCDNG_D = 0;
        public static int location1_CCDNG_N = 0;
        public static int location2_CCDNG_D = 0;
        public static int location2_CCDNG_N = 0;
        public static int location3_CCDNG_D = 0;
        public static int location3_CCDNG_N = 0;
        public static int location4_CCDNG_D = 0;
        public static int location4_CCDNG_N = 0;
        public static int location5_CCDNG_D = 0;
        public static int location5_CCDNG_N = 0;
        public static int HansDataError_D = 0;
        public static int HansDataError_N = 0;
        public static bool Product_status = true;
        //public static int Module_num = 0;
        public static DateTime SelectDateTime = DateTime.Now;
        public static DateTime SelectDTTime = DateTime.Now;
        public static DateTime SelectTOP5Time = DateTime.Now;
        public static DateTime SelectFixturetime = DateTime.Now;
        public static Melsoft_PLC_TCP2 PLC_Client = new Melsoft_PLC_TCP2();//三菱PLC
        public static Melsoft_PLC_TCP3 PLC_Client2 = new Melsoft_PLC_TCP3();//三菱PLC
        public static bool ConnectOEEFlag = true;
        public static ErrorData errordata = new ErrorData();
        public static bool errorTime1 = false;
        public static DateTime errorStartTime;
        public static bool errordisplay = false;
        public static string labelerror = "12010002";
        public static string labelStatus = "1";
        public static string errorselect = "HSG待料";
        public static string errorStatus = "5";
        public static string errorcode = "70010001";
        public static string errorinfo = "镭焊机参数调整";
        public static bool Delay = false;
        public static string Threshold = "1";
        public static string project = "Burbank";
        public static string station = "Trench%20Bracket";
        public static string type = "D24";

        //2小时治具相关
        public static string Fixture = "";//单个删除治具号
        public static List<string> _fixture_ng = new List<string>();//待小保养治具
        public static List<string> _fixture_tossing_ng = new List<string>();//治具抛料NG待排除列表
        public static List<string> _fixture = new List<string>();//机台所有治具
        public static int Fixture_maintance_times = 2550;//治具小保养次数
        public static int Fixture_maintance_time = 10000;//治具小保养时间
        public static int Fixture_ContinuationNG = 3;//治具连续抛料次数
        public static int Fixture_CountNG = 5;//治具总抛料次数
        public static int DataGridView_Select_RowIndex = -1;//DataGridView选中的某行索引
        //
        public static string Setpoint = "50";//治具次数设定值
        public static int Int_Setpint = 0;
        public static bool Storage = false;
        public static int Number = 0;
        public static int Qualified = 0;
        public static bool Selete_Status = false;
        public static string itm_DT = "0";



        //MQTT通讯相关
        public static MqttClient _mqttClient;
        public static ConcurrentDictionary<Guid, Respond> respond = new ConcurrentDictionary<Guid, Respond>();//存储OEE_DT与OEE过站数据
        public static ConcurrentDictionary<Guid, Respond> respond1 = new ConcurrentDictionary<Guid, Respond>();//存储OEE心跳
        public static Respond Respond = new Respond();

        //小料抛料过渡变量,判断是否OEE过站数据上传Errorcode/PFerrorcode
        public static string itm_Smallmaterial_throwing_D = string.Empty;
        public static string itm_Smallmaterial_throwing_N = string.Empty;
        public static string itm_location1_CCDNG_D = string.Empty;
        public static string itm_location1_CCDNG_N = string.Empty;
        public static string itm_location2_CCDNG_D = string.Empty;
        public static string itm_location2_CCDNG_N = string.Empty;
        public static string itm_location3_CCDNG_D = string.Empty;
        public static string itm_location3_CCDNG_N = string.Empty;
        public static string itm_location4_CCDNG_D = string.Empty;
        public static string itm_location4_CCDNG_N = string.Empty;
        public static string itm_location5_CCDNG_D = string.Empty;
        public static string itm_location5_CCDNG_N = string.Empty;
        public static string itm_HansDataError_D = string.Empty;
        public static string itm_HansDataError_N = string.Empty;
        public static string itm_TraceUpLoad_Error_D = string.Empty;
        public static string itm_TraceUpLoad_Error_N = string.Empty;
        public static string itm_PDCAUpLoad_Error_D = string.Empty;
        public static string itm_PDCAUpLoad_Error_N = string.Empty;
        public static string itm_TracePVCheck_Error_D = string.Empty;
        public static string itm_TracePVCheck_Error_N = string.Empty;
        public static string itm_ReadBarcode_NG_D = string.Empty;
        public static string itm_ReadBarcode_NG_N = string.Empty;
        public static string itm_CCDCheck_Error_D = string.Empty;
        public static string itm_CCDCheck_Error_N = string.Empty;
        public static string itm_Band_NG_D = string.Empty;
        public static string itm_Band_NG_N = string.Empty;
        public static string itm_TraceTab_Error_D = string.Empty;
        public static string itm_TraceTab_Error_N = string.Empty;
        public static string itm_TraceThench_Error_D = string.Empty;
        public static string itm_TraceThench_Error_N = string.Empty;
        //
        public static bool SelectManualErrorCode = false;
        public static bool STOP = false;//是否按下停止按钮
        public static bool Art_stop = true;//人工停止复位超5分钟
        public static bool SelectFirstModel = false;
        public static bool SelectFirst = false;//首件开始标志位，首件开始小料抛料不计数
        public static bool SelectTestRunModel = false;//空跑标志位
        public static bool Feeding = false;//HSG待料中是否人工上料
        public static string SendHSG_start_time;//人员上下料提前结束补传待料开始时间;
        public static short[] Product_Total;//白班总产能
        public static short[] Product_NG;//白班NG产能
        public static short[] Product_OK;//白班OK产能
        public static short[] Product_Total_N_1;//夜班总产能1
        public static short[] Product_Total_N_2;//夜班总产能2
        public static short[] Product_NG_N_1;//夜班NG产能1
        public static short[] Product_NG_N_2;//夜班NG产能2
        public static short[] Product_OK_N_1;//夜班OK产能1
        public static short[] Product_OK_N_2;//夜班OK产能2
        public static short[] DT_RunTime;//白班运行时间
        public static short[] DT_ErrorTime;//白班异常时间
        public static short[] DT_PendingTime;//白班待料时间
        public static short[] DT_RunTime_N1;//夜班运行时间
        public static short[] DT_RunTime_N2;//夜班运行时间
        public static short[] DT_ErrorTime_N1;//夜班异常时间
        public static short[] DT_ErrorTime_N2;//夜班异常时间
        public static short[] DT_PendingTime_N1;//夜班待料时间
        public static short[] DT_PendingTime_N2;//夜班待料时间

        public static short[] Product_Throwing_Total;//白班小时抛料
        public static short[] Product_Throwing_NG; //白班抛料
        public static short Product_Throwing_08_20;//白班总抛料
        public static short[] Throwing_D;
        public static short[] Throwing_N;
        public static short[] Welding_D;
        public static short[] Welding_N;

        public static short[] Product_Throwing_Total_N;//白班小时抛料
        public static short[] Product_Throwing_NG_N; //白班抛料
        public static short Product_Throwing_08_20_N;//白班总抛料

        //辅助抛料
        public static int CCDCheck_Throwiing_D = 0;
        public static int CCDCheck_Throwiing_N = 0;
        public static int Welding_throwing_D = 0;
        public static int Welding_throwing_N = 0;
    }
}
