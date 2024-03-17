using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace 卓汇数据追溯系统.ConfigHelper
{
    public class MD5Helper
    {
        public static string EncryptMD5(string EncodeData)
        {
            byte[] bytes = Encoding.Default.GetBytes(EncodeData);
            MD5 md5 = MD5.Create();
            EncodeData = string.Empty;
            byte[] byteNew = md5.ComputeHash(bytes);
            EncodeData = Convert.ToBase64String(byteNew);
            //for (int i = 0; i < byteNew.Length; i++)
            //{
            //    EncodeData += byteNew[i].ToString("X2");
            //}

            return EncodeData;
        }

        public static string GetBase64Encode(string EncodeData)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(EncodeData);
            EncodeData = Convert.ToBase64String(bytes);
            return EncodeData;
        }
    }
}
