using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 卓汇数据追溯系统
{
    public class OEEData
    {
        public string OEE_Dsn;
        public string OEE_authCode;
        public string SerialNumber;
        public string Fixture;
        public DateTime StartTime;
        public DateTime EndTime;
        public string ActualCT;
        public string Status;
        public string SwVersion;
        public Guid GUID;
        public string EMT;
        public string BGBarcode;
        public string Cavity;
        public string ErrorCode;
        public string PFErrorCode;
        public string ScanCount;
        public string ClientPcName;
        public int auto_send = 0;
        public int station = 0;
        public string MAC;
        public string IP;
        public DateTime EventTime;
    }
}
