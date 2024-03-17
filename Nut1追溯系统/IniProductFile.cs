namespace 卓汇数据追溯系统
{
    public class ProductConfig
    {

        #region 系统参数
        public string Plc_IP { get; set; }
        public string Plc_Port{ get; set; }
        public string Plc_Port2{ get; set; }
        public string PDCA_UA_IP{ get; set; }
        public string PDCA_UA_Port{ get; set; }
        public string PDCA_LA_IP{ get; set; }
        public string PDCA_LA_Port{ get; set; }
        public string Barcode_IP{ get; set; }
        public string Barcode_Port{ get; set; }
        public string Hans_IP{ get; set; }
        public string Hans_Port{ get; set; }
        public string Trace_IP{ get; set; }
        public string Trace_Port{ get; set; }
        public string OEE_IP{ get; set; }
        public string OEE_Port{ get; set; }
        public string Precitec_IP{ get; set; }
        public string Precitec_Port{ get; set; }
        public string product_num_ok{ get; set; }
        public string product_num_ng{ get; set; }
        public string theory_product{ get; set; }
        public string product_num_ua_ok{ get; set; }
        public string product_num_ua_ng{ get; set; }
        public string product_num_la_ok{ get; set; }
        public string product_num_la_ng{ get; set; }
        public string trace_process_ok{ get; set; }
        public string trace_process_ng{ get; set; }
        public string fixtruetime { get; set; }
        public string fixtruetimes { get; set; }
        public string product_num_mes_ok{ get; set; }
        public string product_num_mes_ng{ get; set; }
        public string ThrowCount{ get; set; }
        public string ThrowOKCount{ get; set; }
        public string TotalThrowCount{ get; set; }
        public string NutCount{ get; set; }
        public string NutOKCount{ get; set; }
        public string UACount{ get; set; }
        public string LACount{ get; set; }
        public string fixture_ok{ get; set; }
        public string fixture_ng{ get; set; }
        public string UA_OK_Count{ get; set; }
        public string LA_OK_Count{ get; set; }
        public string oee_ok{ get; set; }
        public string oee_ng{ get; set; }
        public string trace_ua_ok{ get; set; }
        public string trace_ua_ng{ get; set; }
        public string trace_ua2_ok{ get; set; }
        public string trace_ua2_ng{ get; set; }


        public string Itm_DT { get; set; }
        public string TraceTab_Error_D { get; set; }
        public string TraceTab_Error_N { get; set; }
        public string TraceThench_Error_D { get; set; }
        public string TraceThench_Error_N { get; set; }


        public string CCD_Throwing_D { get; set; }
        public string CCD_Throwing_N { get; set; }
        public string Welding_Throwing_D { get; set; }
        public string Welding_Throwing_N { get; set; }

        public string Smallmaterial_Input_D { get; set; }
        public string Smallmaterial_Input_N { get; set; }
        public string Smallmaterial_throwing_D { get; set; }
        public string Smallmaterial_throwing_N { get; set; }
        public string Product_Total_D { get; set; }
        public string Product_Total_N { get; set; }
        public string Product_OK_D { get; set; }
        public string Product_OK_N { get; set; }
        public string TraceUpLoad_Error_D { get; set; }
        public string TraceUpLoad_Error_N { get; set; }
        public string PDCAUpLoad_Error_D { get; set; }
        public string PDCAUpLoad_Error_N { get; set; }
        public string TracePVCheck_Error_D { get; set; }
        public string TracePVCheck_Error_N { get; set; }
        public string ReadBarcode_NG_N { get; set; }
        public string ReadBarcode_NG_D { get; set; }
        public string PictureUpLoad_Error_D { get; set; }
        public string PictureUpLoad_Error_N { get; set; }
        public string OEEUpLoad_Error_D { get; set; }
        public string OEEUpLoad_Error_N { get; set; }
        public string CCDCheck_Error_D { get; set; }
        public string CCDCheck_Error_N { get; set; }
        public string Band_NG_D { get; set; }
        public string Band_NG_N { get; set; }
        public string Welding_Error_D { get; set; }
        public string location1_CCDNG_D { get; set; }
        public string location1_CCDNG_N { get; set; }
        public string location2_CCDNG_D { get; set; }
        public string location2_CCDNG_N { get; set; }
        public string location3_CCDNG_D { get; set; }
        public string location3_CCDNG_N { get; set; }
        public string location4_CCDNG_D { get; set; }
        public string location4_CCDNG_N { get; set; }
        public string location5_CCDNG_D { get; set; }
        public string location5_CCDNG_N { get; set; }
        public string HansDataError_D { get; set; }

        public string HansDataError_N { get; set; }
        public string Welding_Error_N { get; set; }        
              
        public string jgp_url{ get; set; }
        public string jgp_online{ get; set; }
        public string error_online{ get; set; }
        public string trace_online{ get; set; }
        public string process_online{ get; set; }
        public string mes_online{ get; set; }
        public string uploadpic_online{ get; set; }
        public string IFactory_online{ get; set; }
        public string Phase_online { get; set; }
        public string GetTracelineData { get; set; }
        public string UserName{ get; set; }
        public string UserPassword { get; set; }
        public string IC_COM { get; set; }
        public string ID_COM { get; set; }
        public string OpenCard { get; set; }

        public string EMT { get; set; }
        public string MQTT_IP { get; set; }
        public string MQTTUserName { get; set; }
        public string MQTTPassword { get; set; }

        public string Brk_PrgPath { get; set; }
        public string Brk_PtsPath { get; set; }
        public string Hsg_PrgPath { get; set; }
        public string Hsg_PtsPath { get; set; }


        public string SiteName { get; set; }
        public string OEECheckParamURL { get; set; }        
        public string MacQueryUrl { get; set; }
        public string code{ get; set; }
        public string Threshold{ get; set; }
        public string delete_time{ get; set; }
        public string FixtureNumber{ get; set; }
        public string MaxCT{ get; set; }
        public string CT{ get; set; }

        //MAC_MINI配置信息
        public string UA_site{ get; set; }
        public string UA_product{ get; set; }
        public string UA_station_type{ get; set; }
        public string UA_location{ get; set; }
        public string UA_line_number{ get; set; }
        public string UA_station_number{ get; set; }
        public string LA_site{ get; set; }
        public string LA_product{ get; set; }
        public string LA_station_type{ get; set; }
        public string LA_location{ get; set; }
        public string LA_line_number{ get; set; }
        public string LA_station_number{ get; set; }

        //BAil属性
        public string Head{ get; set; }
        public string Test_head_id_la{ get; set; }
        public string Test_head_id_ua{ get; set; }
        public string Version{ get; set; }
        public string Sw_version{ get; set; }
        public string Station_id_ua{ get; set; }
        public string Station_id_ipqc { get; set; }
        public string PICPath { get; set; }
        public string Station_id_la{ get; set; }
        public string Line_id_ua{ get; set; }
        public string Line_id_la{ get; set; }
        public string Line_type{ get; set; }
        public string machine_id{ get; set; }
        public string Air_pressure{ get; set; }
        public string Sw_name_ua{ get; set; }
        public string Sw_name_la{ get; set; }
        public string TraceCheckParamURL{ get; set; }
        public string TraceCheckParamURL54 { get; set; }
        public string Process_UA{ get; set; }
        public string Process_LA{ get; set; }
        public string TraceCheckParam_Online{ get; set; }
        //Jgp属性
        public string Trace_Logs_UA{ get; set; }
        public string Trace_Logs_LA{ get; set; }
        public string Trace_CheakSN_UA{ get; set; }
        public string Trace_CheakSN_LA{ get; set; }
        public string OEE_URL1{ get; set; }
        public string OEE_URL2{ get; set; }
        public string OEE_Dsn{ get; set; }
        public string OEE_authCode{ get; set; }
        public string MES_URL1{ get; set; }
        public string MES_URL2{ get; set; }
        public string Headers_key{ get; set; }
        public string Headers_value{ get; set; }
        public string PIS_URL{ get; set; }
        public string IQC_URL { get; set; }
        public string OktoStart4G_URL { get; set; }
        public string OktoStart5G_URL { get; set; }
        //public string SwVersion{ get; set; }
        //各级权限密码
        public string Operator_pwd{ get; set; }
        public string Technician_pwd{ get; set; }
        public string Administrator_pwd{ get; set; }

        /// <summary>
        /// 20230226Add
        /// </summary>
        public string FixtureCount { get;  set; }
        public string Material_online { get; set; }
        //CCD权限等级
        //public string Level{ get; set; }
        #endregion

    }

    public class IniProductFile
    {
        #region 初始化
        private string _path;
        private IniOperation _iniOperation;
        private ProductConfig _productconfig;

        #endregion

        #region Property
        public ProductConfig productconfig
        {
            get { return _productconfig; }
            set { _productconfig = value; }
        }

        #endregion

        public IniProductFile(string path)
        {
            this._path = path;
            _iniOperation = new IniOperation(_path);
            _productconfig = new ProductConfig();
            ReadProductConfigSection();

        }

        public void ReadProductConfigSection()
        {
            string sectionName = "ProductConfig";
            _productconfig.Plc_IP = _iniOperation.ReadValue(sectionName, "PLC_IP");
            _productconfig.Plc_Port = _iniOperation.ReadValue(sectionName, "PLC_PORT");
            _productconfig.Plc_Port2 = _iniOperation.ReadValue(sectionName, "PLC_PORT2");
            _productconfig.PDCA_UA_IP = _iniOperation.ReadValue(sectionName, "PDCA_UA_IP");
            _productconfig.PDCA_UA_Port = _iniOperation.ReadValue(sectionName, "PDCA_UA_Port");
            _productconfig.PDCA_LA_IP = _iniOperation.ReadValue(sectionName, "PDCA_LA_IP");
            _productconfig.PDCA_LA_Port = _iniOperation.ReadValue(sectionName, "PDCA_LA_Port");
            _productconfig.Barcode_IP = _iniOperation.ReadValue(sectionName, "Barcode_IP");
            _productconfig.Barcode_Port = _iniOperation.ReadValue(sectionName, "Barcode_Port");
            _productconfig.Hans_IP = _iniOperation.ReadValue(sectionName, "Hans_IP");
            _productconfig.Hans_Port = _iniOperation.ReadValue(sectionName, "Hans_Port");
            _productconfig.Trace_IP = _iniOperation.ReadValue(sectionName, "Trace_IP");
            _productconfig.Trace_Port = _iniOperation.ReadValue(sectionName, "Trace_Port");
            _productconfig.OEE_IP = _iniOperation.ReadValue(sectionName, "OEE_IP");
            _productconfig.OEE_Port = _iniOperation.ReadValue(sectionName, "OEE_Port");
            _productconfig.Precitec_IP = _iniOperation.ReadValue(sectionName, "Precitec_IP");
            _productconfig.Precitec_Port = _iniOperation.ReadValue(sectionName, "Precitec_Port");
            _productconfig.jgp_url = _iniOperation.ReadValue(sectionName, "JGP_URL");
            _productconfig.jgp_online = _iniOperation.ReadValue(sectionName, "JGP_Online");
            _productconfig.error_online = _iniOperation.ReadValue(sectionName, "DownTime_Online");
            _productconfig.trace_online = _iniOperation.ReadValue(sectionName, "Trace_Online");
            _productconfig.process_online = _iniOperation.ReadValue(sectionName, "ProcessControl_Online");
            _productconfig.IFactory_online = _iniOperation.ReadValue(sectionName, "IFactory_online");
            _productconfig.Material_online = _iniOperation.ReadValue(sectionName, "Material_online");
            _productconfig.Phase_online = _iniOperation.ReadValue(sectionName, "Phase_online");
            _productconfig.UserName = _iniOperation.ReadValue(sectionName, "UserName");
            _productconfig.UserPassword = _iniOperation.ReadValue(sectionName, "UserPassword");
            _productconfig.ID_COM = _iniOperation.ReadValue(sectionName, "ID_COM");
            _productconfig.IC_COM = _iniOperation.ReadValue(sectionName, "IC_COM");
            _productconfig.OpenCard = _iniOperation.ReadValue(sectionName, "OpenCard");

            _productconfig.EMT = _iniOperation.ReadValue(sectionName, "EMT");
            _productconfig.MQTT_IP = _iniOperation.ReadValue(sectionName, "MQTT_IP");
            _productconfig.MQTTUserName = _iniOperation.ReadValue(sectionName, "MQTTUserName");
            _productconfig.MQTTPassword = _iniOperation.ReadValue(sectionName, "MQTTPassword");
            _productconfig.Hsg_PrgPath = _iniOperation.ReadValue(sectionName, "Hsg_PrgPath");
            _productconfig.Hsg_PtsPath = _iniOperation.ReadValue(sectionName, "Hsg_PtsPath");
            _productconfig.Brk_PrgPath = _iniOperation.ReadValue(sectionName, "Brk_PrgPath");
            _productconfig.Brk_PtsPath = _iniOperation.ReadValue(sectionName, "Brk_PtsPath");

            _productconfig.SiteName = _iniOperation.ReadValue(sectionName, "SiteName");
            _productconfig.fixtruetime = _iniOperation.ReadValue(sectionName, "fixtruetime");
            _productconfig.fixtruetimes = _iniOperation.ReadValue(sectionName, "fixtruetimes");
            _productconfig.mes_online = _iniOperation.ReadValue(sectionName, "MES_Online");
            _productconfig.uploadpic_online = _iniOperation.ReadValue(sectionName, "UpLoadPic_Online");
            _productconfig.GetTracelineData = _iniOperation.ReadValue(sectionName, "GetTracelineData");
            _productconfig.code = _iniOperation.ReadValue(sectionName, "Code");
            _productconfig.product_num_ok = _iniOperation.ReadValue(sectionName, "product_num_ok");
            _productconfig.product_num_ng = _iniOperation.ReadValue(sectionName, "product_num_ng");
            _productconfig.product_num_ua_ok = _iniOperation.ReadValue(sectionName, "product_num_ua_ok");
            _productconfig.product_num_ua_ng = _iniOperation.ReadValue(sectionName, "product_num_ua_ng");
            _productconfig.product_num_la_ok = _iniOperation.ReadValue(sectionName, "product_num_la_ok");
            _productconfig.product_num_la_ng = _iniOperation.ReadValue(sectionName, "product_num_la_ng");
            _productconfig.trace_process_ok = _iniOperation.ReadValue(sectionName, "trace_process_ok");
            _productconfig.trace_process_ng = _iniOperation.ReadValue(sectionName, "trace_process_ng");
            _productconfig.product_num_mes_ok = _iniOperation.ReadValue(sectionName, "product_num_mes_ok");
            _productconfig.product_num_mes_ng = _iniOperation.ReadValue(sectionName, "product_num_mes_ng");
            _productconfig.theory_product = _iniOperation.ReadValue(sectionName, "theory_product");
            _productconfig.oee_ok = _iniOperation.ReadValue(sectionName, "oee_ok");
            _productconfig.oee_ng = _iniOperation.ReadValue(sectionName, "oee_ng");
            _productconfig.trace_ua_ok = _iniOperation.ReadValue(sectionName, "trace_ua_ok");
            _productconfig.trace_ua_ng = _iniOperation.ReadValue(sectionName, "trace_ua_ng");
            _productconfig.trace_ua2_ok = _iniOperation.ReadValue(sectionName, "trace_la_ok");
            _productconfig.trace_ua2_ng = _iniOperation.ReadValue(sectionName, "trace_la_ng");
            _productconfig.fixture_ok = _iniOperation.ReadValue(sectionName, "fixture_ok");
            _productconfig.fixture_ng = _iniOperation.ReadValue(sectionName, "fixture_ng");
            _productconfig.Threshold = _iniOperation.ReadValue(sectionName, "threshold");
            _productconfig.delete_time = _iniOperation.ReadValue(sectionName, "delete_time");
            _productconfig.UA_site = _iniOperation.ReadValue(sectionName, "UA_site");
            _productconfig.UA_product = _iniOperation.ReadValue(sectionName, "UA_product");
            _productconfig.UA_station_type = _iniOperation.ReadValue(sectionName, "UA_station_type");
            _productconfig.UA_location = _iniOperation.ReadValue(sectionName, "UA_location");
            _productconfig.UA_line_number = _iniOperation.ReadValue(sectionName, "UA_line_number");
            _productconfig.UA_station_number = _iniOperation.ReadValue(sectionName, "UA_station_number");
            _productconfig.LA_site = _iniOperation.ReadValue(sectionName, "LA_site");
            _productconfig.LA_product = _iniOperation.ReadValue(sectionName, "LA_product");
            _productconfig.LA_station_type = _iniOperation.ReadValue(sectionName, "LA_station_type");
            _productconfig.LA_location = _iniOperation.ReadValue(sectionName, "LA_location");
            _productconfig.LA_line_number = _iniOperation.ReadValue(sectionName, "LA_line_number");
            _productconfig.LA_station_number = _iniOperation.ReadValue(sectionName, "LA_station_number");
            _productconfig.FixtureNumber = _iniOperation.ReadValue(sectionName, "FixtureNumber");
            _productconfig.ThrowCount = _iniOperation.ReadValue(sectionName, "ThrowCount");
            _productconfig.ThrowOKCount = _iniOperation.ReadValue(sectionName, "ThrowOKCount");
            _productconfig.TotalThrowCount = _iniOperation.ReadValue(sectionName, "TotalThrowCount");
            _productconfig.NutCount = _iniOperation.ReadValue(sectionName, "NutCount");
            _productconfig.NutOKCount = _iniOperation.ReadValue(sectionName, "NutOKCount");
            _productconfig.UACount = _iniOperation.ReadValue(sectionName, "UACount");
            _productconfig.LACount = _iniOperation.ReadValue(sectionName, "LACount");
            _productconfig.UA_OK_Count = _iniOperation.ReadValue(sectionName, "UA_OK_Count");
            _productconfig.LA_OK_Count = _iniOperation.ReadValue(sectionName, "LA_OK_Count");
            _productconfig.MaxCT = _iniOperation.ReadValue(sectionName, "MaxCT");
            _productconfig.CT = _iniOperation.ReadValue(sectionName, "CT");
            ///20230226Add
            _productconfig.FixtureCount= _iniOperation.ReadValue(sectionName, "FixtureCount");

            _productconfig.Product_Total_D = _iniOperation.ReadValue(sectionName, "Product_Total_D");
            _productconfig.Product_Total_N = _iniOperation.ReadValue(sectionName, "Product_Total_N");
            _productconfig.Smallmaterial_Input_D = _iniOperation.ReadValue(sectionName, "Smallmaterial_Input_D");
            _productconfig.Smallmaterial_Input_N = _iniOperation.ReadValue(sectionName, "Smallmaterial_Input_N");
            _productconfig.location1_CCDNG_D = _iniOperation.ReadValue(sectionName, "location1_CCDNG_D");
            _productconfig.location1_CCDNG_N = _iniOperation.ReadValue(sectionName, "location1_CCDNG_N");
            _productconfig.location2_CCDNG_D = _iniOperation.ReadValue(sectionName, "location2_CCDNG_D");
            _productconfig.location2_CCDNG_N = _iniOperation.ReadValue(sectionName, "location2_CCDNG_N");
            _productconfig.location3_CCDNG_D = _iniOperation.ReadValue(sectionName, "location3_CCDNG_D");
            _productconfig.location3_CCDNG_N = _iniOperation.ReadValue(sectionName, "location3_CCDNG_N");
            _productconfig.location4_CCDNG_D = _iniOperation.ReadValue(sectionName, "location4_CCDNG_D");
            _productconfig.location4_CCDNG_N = _iniOperation.ReadValue(sectionName, "location4_CCDNG_N");
            _productconfig.location5_CCDNG_D = _iniOperation.ReadValue(sectionName, "location5_CCDNG_D");
            _productconfig.location5_CCDNG_N = _iniOperation.ReadValue(sectionName, "location5_CCDNG_N");
            _productconfig.HansDataError_D = _iniOperation.ReadValue(sectionName, "HansDataError_D");
            _productconfig.HansDataError_N = _iniOperation.ReadValue(sectionName, "HansDataError_N");
            _productconfig.Product_OK_D = _iniOperation.ReadValue(sectionName, "Product_OK_D");
            _productconfig.Product_OK_N = _iniOperation.ReadValue(sectionName, "Product_OK_N");
            _productconfig.TraceUpLoad_Error_D = _iniOperation.ReadValue(sectionName, "TraceUpLoad_Error_D");
            _productconfig.TraceUpLoad_Error_N = _iniOperation.ReadValue(sectionName, "TraceUpLoad_Error_N");

            _productconfig.Itm_DT = _iniOperation.ReadValue(sectionName, "Itm_DT");
            _productconfig.TraceTab_Error_D = _iniOperation.ReadValue(sectionName, "TraceTab_Error_D");
            _productconfig.TraceTab_Error_N = _iniOperation.ReadValue(sectionName, "TraceTab_Error_N");
            _productconfig.TraceThench_Error_D = _iniOperation.ReadValue(sectionName, "TraceThench_Error_D");
            _productconfig.TraceThench_Error_N = _iniOperation.ReadValue(sectionName, "TraceThench_Error_N");
            _productconfig.PDCAUpLoad_Error_D = _iniOperation.ReadValue(sectionName, "PDCAUpLoad_Error_D");
            _productconfig.PDCAUpLoad_Error_N = _iniOperation.ReadValue(sectionName, "PDCAUpLoad_Error_N");
            _productconfig.TracePVCheck_Error_D = _iniOperation.ReadValue(sectionName, "TracePVCheck_Error_D");
            _productconfig.TracePVCheck_Error_N = _iniOperation.ReadValue(sectionName, "TracePVCheck_Error_N");
            _productconfig.PictureUpLoad_Error_D = _iniOperation.ReadValue(sectionName, "PictureUpLoad_Error_D");
            _productconfig.PictureUpLoad_Error_N = _iniOperation.ReadValue(sectionName, "PictureUpLoad_Error_N");
            _productconfig.ReadBarcode_NG_D = _iniOperation.ReadValue(sectionName, "ReadBarcode_NG_D");
            _productconfig.ReadBarcode_NG_N = _iniOperation.ReadValue(sectionName, "ReadBarcode_NG_N");
            _productconfig.OEEUpLoad_Error_D = _iniOperation.ReadValue(sectionName, "OEEUpLoad_Error_D");
            _productconfig.OEEUpLoad_Error_N = _iniOperation.ReadValue(sectionName, "OEEUpLoad_Error_N");
            _productconfig.CCDCheck_Error_D = _iniOperation.ReadValue(sectionName, "CCDCheck_Error_D");
            _productconfig.CCDCheck_Error_N = _iniOperation.ReadValue(sectionName, "CCDCheck_Error_N");
            _productconfig.Welding_Error_D = _iniOperation.ReadValue(sectionName, "Welding_Error_D");
            _productconfig.Welding_Error_N = _iniOperation.ReadValue(sectionName, "Welding_Error_N");
            _productconfig.Smallmaterial_Input_D = _iniOperation.ReadValue(sectionName, "Smallmaterial_Input_D");
            _productconfig.Smallmaterial_Input_N = _iniOperation.ReadValue(sectionName, "Smallmaterial_Input_N");
            _productconfig.Smallmaterial_throwing_D = _iniOperation.ReadValue(sectionName, "Smallmaterial_throwing_D");
            _productconfig.Smallmaterial_throwing_N = _iniOperation.ReadValue(sectionName, "Smallmaterial_throwing_N");
            _productconfig.Band_NG_D = _iniOperation.ReadValue(sectionName, "Band_NG_D");
            _productconfig.Band_NG_N = _iniOperation.ReadValue(sectionName, "Band_NG_N");
            _productconfig.Welding_Throwing_D = _iniOperation.ReadValue(sectionName, "Welding_Throwing_D");
            _productconfig.Welding_Throwing_N = _iniOperation.ReadValue(sectionName, "Welding_Throwing_N");
            _productconfig.CCD_Throwing_D = _iniOperation.ReadValue(sectionName, "CCD_Throwing_D");
            _productconfig.CCD_Throwing_N = _iniOperation.ReadValue(sectionName, "CCD_Throwing_N");
            string sectionName1 = "BailConfig";
            _productconfig.Head = _iniOperation.ReadValue(sectionName1, "head");
            _productconfig.Test_head_id_la = _iniOperation.ReadValue(sectionName1, "test_head_id_la");
            _productconfig.Test_head_id_ua = _iniOperation.ReadValue(sectionName1, "test_head_id_ua");
            _productconfig.PICPath = _iniOperation.ReadValue(sectionName1, "PICPath");
            _productconfig.Station_id_ua = _iniOperation.ReadValue(sectionName1, "station_id_ua");
            _productconfig.Station_id_ipqc = _iniOperation.ReadValue(sectionName1, "station_id_ipqc");
            _productconfig.Station_id_la = _iniOperation.ReadValue(sectionName1, "station_id_la");
            _productconfig.Line_id_ua = _iniOperation.ReadValue(sectionName1, "Line_id_ua");
            _productconfig.Line_id_la = _iniOperation.ReadValue(sectionName1, "Line_id_la");
            _productconfig.Line_type = _iniOperation.ReadValue(sectionName1, "line_type");
            _productconfig.machine_id = _iniOperation.ReadValue(sectionName1, "machine_id");
            _productconfig.Sw_version = _iniOperation.ReadValue(sectionName1, "sw_version");
            _productconfig.Version = _iniOperation.ReadValue(sectionName1, "Version");
            _productconfig.Air_pressure = _iniOperation.ReadValue(sectionName1, "air_pressure");
            _productconfig.Sw_name_ua = _iniOperation.ReadValue(sectionName1, "Sw_name_ua");
            _productconfig.Sw_name_la = _iniOperation.ReadValue(sectionName1, "Sw_name_la");
            _productconfig.OEECheckParamURL = _iniOperation.ReadValue(sectionName1, "OEECheckParamURL");
            _productconfig.MacQueryUrl = _iniOperation.ReadValue(sectionName1, "MacQueryUrl");
            _productconfig.TraceCheckParamURL = _iniOperation.ReadValue(sectionName1, "TraceCheckParamURL");
            _productconfig.TraceCheckParamURL54 = _iniOperation.ReadValue(sectionName1, "TraceCheckParamURL_54");             
            _productconfig.Process_UA = _iniOperation.ReadValue(sectionName1, "Process_UA");
            _productconfig.Process_LA = _iniOperation.ReadValue(sectionName1, "Process_LA");
            _productconfig.TraceCheckParam_Online = _iniOperation.ReadValue(sectionName1, "TraceCheckParam_Online");
            string sectionName2 = "JgpConfig";
            _productconfig.Trace_Logs_UA = _iniOperation.ReadValue(sectionName2, "Trace_Logs_UA");
            _productconfig.Trace_Logs_LA = _iniOperation.ReadValue(sectionName2, "Trace_Logs_LA");
            _productconfig.Trace_CheakSN_UA = _iniOperation.ReadValue(sectionName2, "Trace_CheakSN_UA");
            _productconfig.Trace_CheakSN_LA = _iniOperation.ReadValue(sectionName2, "Trace_CheakSN_LA");
            _productconfig.OEE_URL1 = _iniOperation.ReadValue(sectionName2, "OEE_URL1");
            _productconfig.OEE_URL2 = _iniOperation.ReadValue(sectionName2, "OEE_URL2");
            _productconfig.OEE_Dsn = _iniOperation.ReadValue(sectionName2, "OEE_Dsn");
            _productconfig.OEE_authCode = _iniOperation.ReadValue(sectionName2, "OEE_authCode");
            _productconfig.MES_URL1 = _iniOperation.ReadValue(sectionName2, "MES_URL1");
            _productconfig.MES_URL2 = _iniOperation.ReadValue(sectionName2, "MES_URL2");
            _productconfig.Headers_key = _iniOperation.ReadValue(sectionName2, "Headers_key");
            _productconfig.Headers_value = _iniOperation.ReadValue(sectionName2, "Headers_value");
            _productconfig.PIS_URL = _iniOperation.ReadValue(sectionName2, "PIS_URL");
            _productconfig.IQC_URL = _iniOperation.ReadValue(sectionName2, "IQC_URL");
            _productconfig.OktoStart4G_URL = _iniOperation.ReadValue(sectionName2, "OktoStart4G_URL");
            _productconfig.OktoStart5G_URL = _iniOperation.ReadValue(sectionName2, "OktoStart5G_URL");
            //_productconfig.SwVersion = _iniOperation.ReadValue(sectionName2, "SwVersion");
            string sectionName3 = "PassWord";
            _productconfig.Operator_pwd = _iniOperation.ReadValue(sectionName3, "Operator_pwd");
            _productconfig.Technician_pwd = _iniOperation.ReadValue(sectionName3, "Technician_pwd");
            _productconfig.Administrator_pwd = _iniOperation.ReadValue(sectionName3, "Administrator_pwd");
        }

        public void WriteProductConfigSection()
        {
            string sectionName = "ProductConfig";
            _iniOperation.WriteValue(sectionName, "PLC_IP", _productconfig.Plc_IP);
            _iniOperation.WriteValue(sectionName, "PLC_PORT", _productconfig.Plc_Port);
            _iniOperation.WriteValue(sectionName, "PLC_PORT2", _productconfig.Plc_Port2);
            _iniOperation.WriteValue(sectionName, "PDCA_UA_IP", _productconfig.PDCA_UA_IP);
            _iniOperation.WriteValue(sectionName, "PDCA_UA_Port", _productconfig.PDCA_UA_Port);
            _iniOperation.WriteValue(sectionName, "PDCA_LA_IP", _productconfig.PDCA_LA_IP);
            _iniOperation.WriteValue(sectionName, "PDCA_LA_Port", _productconfig.PDCA_LA_Port);
            _iniOperation.WriteValue(sectionName, "Barcode_IP", _productconfig.Barcode_IP);
            _iniOperation.WriteValue(sectionName, "Barcode_Port", _productconfig.Barcode_Port);
            _iniOperation.WriteValue(sectionName, "Trace_IP", _productconfig.Trace_IP);
            _iniOperation.WriteValue(sectionName, "Trace_Port", _productconfig.Trace_Port);
            _iniOperation.WriteValue(sectionName, "OEE_IP", _productconfig.OEE_IP);
            _iniOperation.WriteValue(sectionName, "OEE_Port", _productconfig.OEE_Port);
            _iniOperation.WriteValue(sectionName, "Hans_IP", _productconfig.Hans_IP);
            _iniOperation.WriteValue(sectionName, "Hans_Port", _productconfig.Hans_Port);
            _iniOperation.WriteValue(sectionName, "Precitec_IP", _productconfig.Precitec_IP);
            _iniOperation.WriteValue(sectionName, "Precitec_Port", _productconfig.Precitec_Port);
            _iniOperation.WriteValue(sectionName, "JGP_URL", _productconfig.jgp_url);
            _iniOperation.WriteValue(sectionName, "threshold", _productconfig.Threshold);
            _iniOperation.WriteValue(sectionName, "delete_time", _productconfig.delete_time);
            _iniOperation.WriteValue(sectionName, "fixtruetime", _productconfig.fixtruetime);
            _iniOperation.WriteValue(sectionName, "fixtruetimes", _productconfig.fixtruetimes);
            _iniOperation.WriteValue(sectionName, "UA_site", _productconfig.UA_site);
            _iniOperation.WriteValue(sectionName, "UA_product", _productconfig.UA_product);
            _iniOperation.WriteValue(sectionName, "UA_station_type", _productconfig.UA_station_type);
            _iniOperation.WriteValue(sectionName, "UA_location", _productconfig.UA_location);
            _iniOperation.WriteValue(sectionName, "UA_line_number", _productconfig.UA_line_number);
            _iniOperation.WriteValue(sectionName, "UA_station_number", _productconfig.UA_station_number);
            _iniOperation.WriteValue(sectionName, "LA_site", _productconfig.LA_site);
            _iniOperation.WriteValue(sectionName, "LA_product", _productconfig.LA_product);
            _iniOperation.WriteValue(sectionName, "LA_station_type", _productconfig.LA_station_type);
            _iniOperation.WriteValue(sectionName, "LA_location", _productconfig.LA_location);
            _iniOperation.WriteValue(sectionName, "LA_line_number", _productconfig.LA_line_number);
            _iniOperation.WriteValue(sectionName, "LA_station_number", _productconfig.LA_station_number);
            _iniOperation.WriteValue(sectionName, "FixtureNumber", _productconfig.FixtureNumber);

            _iniOperation.WriteValue(sectionName, "JGP_Online", _productconfig.jgp_online);
            _iniOperation.WriteValue(sectionName, "DownTime_Online", _productconfig.error_online);
            _iniOperation.WriteValue(sectionName, "Trace_Online", _productconfig.trace_online);
            _iniOperation.WriteValue(sectionName, "ProcessControl_Online", _productconfig.process_online);
            _iniOperation.WriteValue(sectionName, "MES_Online", _productconfig.mes_online);
            _iniOperation.WriteValue(sectionName, "UpLoadPic_Online", _productconfig.uploadpic_online);
            _iniOperation.WriteValue(sectionName, "GetTracelineData", _productconfig.GetTracelineData);
            _iniOperation.WriteValue(sectionName, "IFactory_online", _productconfig.IFactory_online);

            _iniOperation.WriteValue(sectionName, "Material_online", _productconfig.Material_online);
            
            _iniOperation.WriteValue(sectionName, "Phase_online", _productconfig.Phase_online);
            _iniOperation.WriteValue(sectionName, "MaxCT", _productconfig.MaxCT);
            _iniOperation.WriteValue(sectionName, "CT", _productconfig.CT);
            ///20230226Add
            _iniOperation.WriteValue(sectionName,"FixtureCount",_productconfig.FixtureCount);

            string sectionName1 = "BailConfig";
            _iniOperation.WriteValue(sectionName1, "TraceCheckParamURL", _productconfig.TraceCheckParamURL);
            _iniOperation.WriteValue(sectionName1, "TraceCheckParamURL_54", _productconfig.TraceCheckParamURL54);
            _iniOperation.WriteValue(sectionName1, "Process_UA", _productconfig.Process_UA);
            _iniOperation.WriteValue(sectionName1, "Process_LA", _productconfig.Process_LA);
            _iniOperation.WriteValue(sectionName1, "Station_id_ua", _productconfig.Station_id_ua);
            _iniOperation.WriteValue(sectionName1, "Station_id_la", _productconfig.Station_id_la);
            _iniOperation.WriteValue(sectionName1, "Line_id_ua", _productconfig.Line_id_ua);
            _iniOperation.WriteValue(sectionName1, "Line_id_la", _productconfig.Line_id_la);
            _iniOperation.WriteValue(sectionName1, "Sw_name_ua", _productconfig.Sw_name_ua);
            _iniOperation.WriteValue(sectionName1, "Sw_name_la", _productconfig.Sw_name_la);
            _iniOperation.WriteValue(sectionName1, "TraceCheckParam_Online", _productconfig.TraceCheckParam_Online);
            _iniOperation.WriteValue(sectionName1, "sw_version", _productconfig.Sw_version);

            string sectionName2 = "PassWord";
            _iniOperation.WriteValue(sectionName2, "Operator_pwd", _productconfig.Operator_pwd);
            _iniOperation.WriteValue(sectionName2, "Technician_pwd", _productconfig.Technician_pwd);
            _iniOperation.WriteValue(sectionName2, "Administrator_pwd", _productconfig.Administrator_pwd);
        }

        public void WriteProductnumSection()
        {
            string sectionName = "ProductConfig";
            _iniOperation.WriteValue(sectionName, "product_num_ok", _productconfig.product_num_ok);
            _iniOperation.WriteValue(sectionName, "product_num_ng", _productconfig.product_num_ng);
            _iniOperation.WriteValue(sectionName, "theory_product", _productconfig.theory_product);
            _iniOperation.WriteValue(sectionName, "product_num_ua_ok", _productconfig.product_num_ua_ok);
            _iniOperation.WriteValue(sectionName, "product_num_ua_ng", _productconfig.product_num_ua_ng);
            _iniOperation.WriteValue(sectionName, "product_num_la_ok", _productconfig.product_num_la_ok);
            _iniOperation.WriteValue(sectionName, "product_num_la_ng", _productconfig.product_num_la_ng);
            _iniOperation.WriteValue(sectionName, "trace_process_ok", _productconfig.trace_process_ok);
            _iniOperation.WriteValue(sectionName, "trace_process_ng", _productconfig.trace_process_ng);
            _iniOperation.WriteValue(sectionName, "product_num_mes_ok", _productconfig.product_num_mes_ok);
            _iniOperation.WriteValue(sectionName, "product_num_mes_ng", _productconfig.product_num_mes_ng);
            _iniOperation.WriteValue(sectionName, "oee_ok", _productconfig.oee_ok);
            _iniOperation.WriteValue(sectionName, "oee_ng", _productconfig.oee_ng);
            _iniOperation.WriteValue(sectionName, "trace_ua_ok", _productconfig.trace_ua_ok);
            _iniOperation.WriteValue(sectionName, "trace_ua_ng", _productconfig.trace_ua_ng);
            _iniOperation.WriteValue(sectionName, "trace_la_ok", _productconfig.trace_ua2_ok);
            _iniOperation.WriteValue(sectionName, "trace_la_ng", _productconfig.trace_ua2_ng);
            _iniOperation.WriteValue(sectionName, "ThrowCount", _productconfig.ThrowCount);
            _iniOperation.WriteValue(sectionName, "ThrowOKCount", _productconfig.ThrowOKCount);
            _iniOperation.WriteValue(sectionName, "TotalThrowCount", _productconfig.TotalThrowCount);
            _iniOperation.WriteValue(sectionName, "NutCount", _productconfig.NutCount);
            _iniOperation.WriteValue(sectionName, "NutOKCount", _productconfig.NutOKCount);
            _iniOperation.WriteValue(sectionName, "UACount", _productconfig.UACount);
            _iniOperation.WriteValue(sectionName, "LACount", _productconfig.LACount);
            _iniOperation.WriteValue(sectionName, "UA_OK_Count", _productconfig.UA_OK_Count);
            _iniOperation.WriteValue(sectionName, "LA_OK_Count", _productconfig.LA_OK_Count);
            _iniOperation.WriteValue(sectionName, "fixture_ok", _productconfig.fixture_ok);
            _iniOperation.WriteValue(sectionName, "fixture_ng", _productconfig.fixture_ng);

            _iniOperation.WriteValue(sectionName, "Product_Total_D", _productconfig.Product_Total_D);
            _iniOperation.WriteValue(sectionName, "Product_Total_N", _productconfig.Product_Total_N);
            //_iniOperation.WriteValue(sectionName, "Product_OK_D", _productconfig.Product_OK_D);
            //_iniOperation.WriteValue(sectionName, "Product_OK_N", _productconfig.Product_OK_N);
            _iniOperation.WriteValue(sectionName, "location1_CCDNG_D", _productconfig.location1_CCDNG_D);
            _iniOperation.WriteValue(sectionName, "location1_CCDNG_N", _productconfig.location1_CCDNG_N);
            _iniOperation.WriteValue(sectionName, "location2_CCDNG_D", _productconfig.location2_CCDNG_D);
            _iniOperation.WriteValue(sectionName, "location2_CCDNG_N", _productconfig.location2_CCDNG_N);
            _iniOperation.WriteValue(sectionName, "location3_CCDNG_D", _productconfig.location3_CCDNG_D);
            _iniOperation.WriteValue(sectionName, "location3_CCDNG_N", _productconfig.location3_CCDNG_N);
            _iniOperation.WriteValue(sectionName, "location4_CCDNG_D", _productconfig.location4_CCDNG_D);
            _iniOperation.WriteValue(sectionName, "location4_CCDNG_N", _productconfig.location4_CCDNG_N);
            _iniOperation.WriteValue(sectionName, "location5_CCDNG_D", _productconfig.location5_CCDNG_D);
            _iniOperation.WriteValue(sectionName, "location5_CCDNG_N", _productconfig.location5_CCDNG_N);
            _iniOperation.WriteValue(sectionName, "HansDataError_D", _productconfig.HansDataError_D);
            _iniOperation.WriteValue(sectionName, "HansDataError_N", _productconfig.HansDataError_N);
            _iniOperation.WriteValue(sectionName, "TraceUpLoad_Error_D", _productconfig.TraceUpLoad_Error_D);
            _iniOperation.WriteValue(sectionName, "TraceUpLoad_Error_N", _productconfig.TraceUpLoad_Error_N);

            _iniOperation.WriteValue(sectionName, "Itm_DT", _productconfig.Itm_DT);
            _iniOperation.WriteValue(sectionName, "TraceTab_Error_D", _productconfig.TraceTab_Error_D);
            _iniOperation.WriteValue(sectionName, "TraceTab_Error_N", _productconfig.TraceTab_Error_N);
            _iniOperation.WriteValue(sectionName, "TraceThench_Error_D", _productconfig.TraceThench_Error_D);
            _iniOperation.WriteValue(sectionName, "TraceThench_Error_N", _productconfig.TraceThench_Error_N);
            _iniOperation.WriteValue(sectionName, "PDCAUpLoad_Error_D", _productconfig.PDCAUpLoad_Error_D);
            _iniOperation.WriteValue(sectionName, "PDCAUpLoad_Error_N", _productconfig.PDCAUpLoad_Error_N);
            _iniOperation.WriteValue(sectionName, "TracePVCheck_Error_D", _productconfig.TracePVCheck_Error_D);
            _iniOperation.WriteValue(sectionName, "TracePVCheck_Error_N", _productconfig.TracePVCheck_Error_N);
            _iniOperation.WriteValue(sectionName, "PictureUpLoad_Error_D", _productconfig.PictureUpLoad_Error_D);
            _iniOperation.WriteValue(sectionName, "PictureUpLoad_Error_N", _productconfig.PictureUpLoad_Error_N);
            _iniOperation.WriteValue(sectionName, "OEEUpLoad_Error_D", _productconfig.OEEUpLoad_Error_D);
            _iniOperation.WriteValue(sectionName, "OEEUpLoad_Error_N", _productconfig.OEEUpLoad_Error_N);
            _iniOperation.WriteValue(sectionName, "ReadBarcode_NG_D", _productconfig.ReadBarcode_NG_D);
            _iniOperation.WriteValue(sectionName, "ReadBarcode_NG_N", _productconfig.ReadBarcode_NG_N);
            _iniOperation.WriteValue(sectionName, "CCDCheck_Error_D", _productconfig.CCDCheck_Error_D);
            _iniOperation.WriteValue(sectionName, "CCDCheck_Error_N", _productconfig.CCDCheck_Error_N);
            //_iniOperation.WriteValue(sectionName, "Welding_Error_D", _productconfig.location2_CCDNG_D);
            _iniOperation.WriteValue(sectionName, "Smallmaterial_Input_D", _productconfig.Smallmaterial_Input_D);
            _iniOperation.WriteValue(sectionName, "Smallmaterial_Input_N", _productconfig.Smallmaterial_Input_N);
            _iniOperation.WriteValue(sectionName, "Welding_Error_N", _productconfig.Welding_Error_N);
            _iniOperation.WriteValue(sectionName, "Smallmaterial_throwing_D", _productconfig.Smallmaterial_throwing_D);
            _iniOperation.WriteValue(sectionName, "Smallmaterial_throwing_N", _productconfig.Smallmaterial_throwing_N);
            _iniOperation.WriteValue(sectionName, "Band_NG_D", _productconfig.Band_NG_D);
            _iniOperation.WriteValue(sectionName, "Band_NG_N", _productconfig.Band_NG_N);

            _iniOperation.WriteValue(sectionName, "CCD_Throwing_D", _productconfig.CCD_Throwing_D);
            _iniOperation.WriteValue(sectionName, "CCD_Throwing_N", _productconfig.CCD_Throwing_N);
            _iniOperation.WriteValue(sectionName, "Welding_Throwing_D", _productconfig.Welding_Throwing_D);
            _iniOperation.WriteValue(sectionName, "Welding_Throwing_N", _productconfig.Welding_Throwing_N);
        }
    }
}
