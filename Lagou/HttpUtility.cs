using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using RestSharp;

namespace Lagou
{
    public class HttpUtility
    {
        public static string SendGetHttpRequest(string url)
        {

            string html = string.Empty;
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                WebProxy webproxy = new WebProxy();
                //Uri uri = new Uri(string.Format("http://{0}:{1}", "Adrress", "Port"));
                //webproxy.Address = uri;
                //request.Proxy = webproxy;

                request.Accept = "text/plain, */*; q=0.01";
                request.Method = "GET";
                request.Headers.Add("Accept-Language", "zh-cn,zh;q=0.8,en-us;q=0.5,en;q=0.3");
                request.ContentLength = 0;
                //request.Host = "www.cnblogs.com";
                request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; rv:25.0) Gecko/20100101 Firefox/25.0";

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                using (Stream responseStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                    html = reader.ReadToEnd();
                }

            }
            catch (Exception ex)
            {
                new CommonException(ex.Message, ex);
            }

            return html;

        }

        public static string SendPostHttpRequest(string url, string body)
        {
            string html = string.Empty;
            try
            {
                RestRequest request = new RestRequest(Method.POST);
                request.AddBody(body);
                var restclient = new RestClient(url);
                var response = restclient.Execute(request);

                return response.Content;


                //此写法不定时会异常 无法建立传输连接：连接已关闭
                //HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                ////WebProxy webproxy = new WebProxy();
                ////Uri uri = new Uri(string.Format("http://{0}:{1}", "Adrress", "Port"));
                ////webproxy.Address = uri;
                ////request.Proxy = webproxy;

                //request.Accept = "*/*";
                //request.Method = "POST";
                //request.Headers.Add("Accept-Language", "zh-cn,zh;q=0.8,en-us;q=0.5,en;q=0.3");
                //request.KeepAlive = true;
                //request.ContentType = "application/x-www-form-urlencoded; charset=UTF-8"; //表单提交
                ////request.Host = "www.cnblogs.com";
                //request.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)";
                //byte[] bytes = Encoding.UTF8.GetBytes(body);
                //request.ContentLength = bytes.Length;
                //request.GetRequestStream().Write(bytes, 0, bytes.Length);

                //HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                //using (Stream responseStream = response.GetResponseStream())
                //{
                //    StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                //    html = reader.ReadToEnd();
                //    response = null;
                //}
            }
            catch (Exception ex)
            {
                throw new CommonException(ex.Message, ex);
            }

            return html;
        }


        public string SendRestPostHttpRequest(string url, string body)
        {

            RestRequest request = new RestRequest(Method.POST);
            request.AddBody(body);
            var restclient = new RestClient(url);
            var response = restclient.Execute(request);

            return response.Content;
            
        }



    }
}
