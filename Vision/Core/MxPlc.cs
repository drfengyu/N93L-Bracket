using PlcComm;

namespace Vision.Core
{
    /// <summary>
    /// plc适配器类
    /// 用来适配原有的plc类
    /// </summary>
    public class MxPlc
    {
        private readonly Melsoft_PLC_TCP2 _plc;

        public bool IsConnected => _plc != null && _plc.IsConnected;

        public MxPlc(Melsoft_PLC_TCP2 plc)
        {
            _plc = plc;
        }

        public bool WriteBool(string addr, bool value)
        {
            if (_plc == null) return false;
            return _plc.WritePLC_M(
                 int.Parse(addr.Substring(1, addr.Length - 1)),
                  value ? (short)1 : (short)0);
        }

        public bool WriteShort(string addr, short value)
        {
            if (_plc == null) return false;
            return _plc.WritePLC_D(
                int.Parse(addr.Substring(1, addr.Length - 1)),
               new[] { value });
            
        }

        public bool WriteShort(string addr, short[] values)
        {
            if (_plc == null) return false;
            return _plc.WritePLC_D(
                int.Parse(addr.Substring(1, addr.Length - 1)),
                values);
        }

        public bool WriteInt(string addr, int value)
        {
            if (_plc == null) return false;
            return _plc.WritePLC_DD(
                int.Parse(addr.Substring(1, addr.Length - 1)),
                value);
        }

        /// <summary>
        /// 写double数据
        /// </summary>
        /// <param name="addr">地址</param>
        /// <param name="value">值</param>
        /// <param name="pointNum">乘以多少小数</param>
        /// <returns></returns>
        public bool WriteDouble(string addr, double value, int pointNum = 3)
        {
            if (_plc == null) return false;

            int temp = 0;
            switch (pointNum)
            {
                case 0:
                    temp = (int)value; break;
                case 1:
                    temp = (int)(value * 10); break;
                case 2:
                    temp = (int)(value * 100); break;
                case 3:
                    temp = (int)(value * 1000); break;
            }
            return _plc.WritePLC_DD(
                int.Parse(addr.Substring(1, addr.Length - 1)),
                temp);
        }

        public void WriteString(string addr, string value)
        {
            if (_plc == null) return;
            _plc.WriteStringToD(
               int.Parse(addr.Substring(1, addr.Length - 1)),
               value);
        }

        public bool ReadBool(string addr)
        {
            if (_plc == null) return false;
            var res = _plc.ReadPLC_M(int.Parse(addr.Substring(1, addr.Length - 1)));
            if (res == 0) return false;
            else if (res == 1) return true;
            return false;
        }

        public short ReadShort(string addr)
        {
            if (_plc == null) return default;
            var res = _plc.ReadPLC_D(int.Parse(addr.Substring(1, addr.Length - 1)), 1);
            return res[0];
        }

        public short[] ReadShort(string addr, short count)
        {
            if (_plc == null) return default;
            var res = _plc.ReadPLC_D(int.Parse(addr.Substring(1, addr.Length - 1)), count);
            return res;
        }

        public int ReadInt(string addr)
        {
            if (_plc == null) return default;
            var res = _plc.ReadPLC_DD(int.Parse(addr.Substring(1, addr.Length - 1)), 1);
            return res[0];
        }

        public int[] ReadInt(string addr, short count)
        {
            if (_plc == null) return default;
            var res = _plc.ReadPLC_DD(int.Parse(addr.Substring(1, addr.Length - 1)), count);
            return res;
        }

        public double ReadDouble(string addr, int pointNum = 3)
        {
            if (_plc == null) return default;
            if (string.IsNullOrEmpty(addr)) return default;
            double temp = 0;
            try
            {
                var res = _plc.ReadPLC_DD(int.Parse(addr.Substring(1, addr.Length - 1)), 2);
                switch (pointNum)
                {
                    case 0:
                        temp = res[0];
                        break;
                    case 1:
                        temp = res[0] / 10.0;
                        break;
                    case 2:
                        temp = res[0] / 100.0;
                        break;
                    case 3:
                        temp = res[0] / 1000.0;
                        break;
                    case 4:
                        temp = res[0] / 10000.0;
                        break;
                }
            }
            catch
            {
                string err = "读取plc数据失败！";
                LogNetHelper.Log(err);
                LogUI.AddLog(err);
            }

            return temp;
        }

        public string ReadString(string addr, short count)
        {
            if (_plc == null) return default;
            var res = _plc.ReadPLC_Dstring(int.Parse(addr.Substring(1, addr.Length - 1)), count);
            return res;
        }
    }
}
