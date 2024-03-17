using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;
using System.Web;

namespace 卓汇数据追溯系统
{
    class RequestAPI3
    {
        public static string HttpPostWebService
            (string url, string IP, string Process, string Line_id, string Station_id, string SoftwareName,string Version,out string ErrMsg)
        {
            string result = string.Empty;
            string param = string.Empty;
            byte[] bytes = null;

            Stream writer = null;
            HttpWebRequest request = null;
            HttpWebResponse response = null;

            param = HttpUtility.UrlEncode("IP") + "=" + IP + "&" + HttpUtility.UrlEncode("Process") + "=" + Process + "&" + HttpUtility.UrlEncode("Line_id") + "=" + Line_id + "&" + HttpUtility.UrlEncode("Station_id") + "=" + Station_id + "&" + HttpUtility.UrlEncode("SoftwareName") + "=" + SoftwareName + "&" + HttpUtility.UrlEncode("SoftwareVersion") + "=" + Version;
            bytes = Encoding.UTF8.GetBytes(param);

            request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.Timeout = 6000;
            request.ContentLength = bytes.Length;

            try
            {
                writer = request.GetRequestStream();        //获取用于写入请求数据的Stream对象
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message;
                Log.WriteLog(ex.ToString());
                return "";
               
            }
            writer.Write(bytes, 0, bytes.Length);       //把参数数据写入请求数据流
            writer.Close();

            try
            {
                response = (HttpWebResponse)request.GetResponse();      //获得响应
            }
            catch (WebException ex)
            {
                ErrMsg = ex.Message;
                Log.WriteLog(ex.ToString());
                return "";
               
            }
            Stream stream = response.GetResponseStream();        //获取响应流
            XmlTextReader Reader = new XmlTextReader(stream);
            Reader.MoveToContent();
            result = Reader.ReadInnerXml();
            response.Close();
            Reader.Close();
            stream.Dispose();
            stream.Close();
            ErrMsg = string.Empty;
            return result;
        }


        public static string Add(string url,int a,int b)
        {
            string result = string.Empty;
            string param = string.Empty;
            byte[] bytes = null;

            Stream writer = null;
            HttpWebRequest request = null;
            HttpWebResponse response = null;

            param = HttpUtility.UrlEncode("a") + "=" + a + "&" + HttpUtility.UrlEncode("b") + "=" + b;
            bytes = Encoding.UTF8.GetBytes(param);

            request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.Timeout = 3000;
            request.ContentLength = bytes.Length;

            try
            {
                writer = request.GetRequestStream();        //获取用于写入请求数据的Stream对象
            }
            catch (Exception ex)
            {
               // ErrMsg = ex.Message;
                Log.WriteLog(ex.ToString());
                return "";

            }
            writer.Write(bytes, 0, bytes.Length);       //把参数数据写入请求数据流
            writer.Close();

            try
            {
                response = (HttpWebResponse)request.GetResponse();      //获得响应
            }
            catch (WebException ex)
            {
               // ErrMsg = ex.Message;
                Log.WriteLog(ex.ToString());
                return "";

            }
            Stream stream = response.GetResponseStream();        //获取响应流
            XmlTextReader Reader = new XmlTextReader(stream);
            Reader.MoveToContent();
            result = Reader.ReadInnerXml();
            response.Close();
            Reader.Close();
            stream.Dispose();
            stream.Close();
           // ErrMsg = string.Empty;
            return result;
        }
    }
}
