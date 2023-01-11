using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace YzProject.Common
{
    public class HttpRequestMethod
    {
        public static string WebRequestGet(string sUrl)
        {
            Uri uriurl = new Uri(sUrl);
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(uriurl);
            req.Method = "GET";
            req.Timeout = 120 * 1000;
            req.ContentType = "application/x-www-form-urlencoded;";
            try
            {
                using (HttpWebResponse res = (HttpWebResponse)req.GetResponse())
                {
                    //在这里对接收到的页面内容进行处理 
                    StreamReader reader = new StreamReader(res.GetResponseStream());
                    string result = reader.ReadToEnd().Trim();
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        /// <summary>
        /// Post 提交调用抓取
        /// </summary>
        /// <param name="url">提交地址</param>
        /// <param name="param">参数</param>
        /// <returns>Stream</returns>
        public static Stream WebRequestPost(string sUrl, string sParam)
        {
            byte[] bt = System.Text.Encoding.UTF8.GetBytes(sParam);

            Uri uriurl = new Uri(sUrl);
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(uriurl);
            req.Method = "POST";
            req.Timeout = 120 * 1000;
            req.ContentType = "application/json;charset=UTF-8";
            req.ContentLength = bt.Length;

            Stream writer = req.GetRequestStream();
            writer.Write(bt, 0, bt.Length);
            writer.Close();
            try
            {
                HttpWebResponse response = (HttpWebResponse)req.GetResponse();
                //在这里对接收到的页面内容进行处理 
                Stream resStream = response.GetResponseStream();
                return resStream;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
    }
}
