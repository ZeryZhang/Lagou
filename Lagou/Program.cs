using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace Lagou
{
    class Program
    {
        static void Main(string[] args)
        {
            HttpUtilty httpUtilty = new HttpUtilty();
            string html = httpUtilty.SendHttpRequest();
        }
    }  

    public class HttpUtilty
    {
        public string SendHttpRequest()
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://www.lagou.com/");
            WebProxy webproxy = new WebProxy();
            //Uri uri = new Uri(string.Format("http://{0}:{1}", "Adrress", "Port"));
            //webproxy.Address = uri;
            //request.Proxy = webproxy;

            request.Accept = "text/plain, */*; q=0.01";
            request.Method = "GET";
            request.Headers.Add("Accept-Language", "zh-cn,zh;q=0.8,en-us;q=0.5,en;q=0.3");
            request.ContentLength = 0;
            request.ContentType = "keep-alive";
            //request.Host = "www.cnblogs.com";
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; rv:25.0) Gecko/20100101 Firefox/25.0";

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream responseStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
            string html = reader.ReadToEnd();

            return html;

        }

        public string GetJobType(string html)
        {
            
            HtmlAgilityPack.HtmlDocument document = new HtmlDocument();
            document.LoadHtml(html);
            HtmlNodeCollection jobCatgroy = document.DocumentNode.SelectNodes("dl[@class='reset']"); //取大分类

            foreach (var item in jobCatgroy)
            {
                item.SelectNodes("");
            }


            return string.Empty;

        }

    }

    

}
