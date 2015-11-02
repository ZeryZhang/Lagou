using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Remoting.Messaging;
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
            JobRelative jobRelative = new JobRelative();
            string html = string.Empty;
            //jobType
            //string html = httpUtilty.SendHttpRequest("http://www.lagou.com/");
            //jobRelative.GetJobType(html);
            //City
            html = httpUtilty.SendHttpRequest("http://www.lagou.com/zhaopin/");
            jobRelative.GetCitys(html);

        }
    }

    public class JobRelative
    {
        public List<JobTypeEntity> GetJobType(string html)
        {

            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(html);
            HtmlNodeCollection jobCatgroy = document.DocumentNode.SelectNodes("//dl[@class='reset']"); //取大分类

            List<JobTypeEntity> JobTypeList = new List<JobTypeEntity>();
            //大分类
            for (int i = 0; i < 3; i++)
            {
                HtmlNode dlNode = jobCatgroy[i];
                string href = string.Empty;
                HtmlNode aNode = dlNode.SelectSingleNode("dt/a");

                List<HtmlNode> jobNodes = dlNode.SelectNodes("dd/a").ToList();
                jobNodes.ForEach(o => JobTypeList.Add(
                    new JobTypeEntity()
                    {
                        JobName = o.InnerText,
                        JobHref = o.GetAttributeValue("href", string.Empty)
                    }));
            }

            return JobTypeList;

        }

        public List<CityEntity> GetCitys(string html)
        {
            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(html);
            List<CityEntity> cityList = new List<CityEntity>();
            List<HtmlNode> cityNodes = document.DocumentNode.SelectNodes("//div[@id='filterCollapse']/li[@class='hot']/a[position()>1]").ToList();

            cityNodes.ForEach(o => cityList.Add(
                    new CityEntity()
                    {
                        CityName = o.InnerText,
                        CityHref = o.GetAttributeValue("href",string.Empty)
                    }
                ));

            return cityList;
        }

    }

    public class JobTypeEntity
    {
        public string JobName { get; set; }

        public string JobHref { get; set; }
    }

    public class CityEntity
    {
        public string CityName { get; set;}

        public string CityHref { get; set; }
    }


    public class HttpUtilty
    {
        public string SendHttpRequest(string url)
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
            request.ContentType = "keep-alive";
            //request.Host = "www.cnblogs.com";
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; rv:25.0) Gecko/20100101 Firefox/25.0";

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream responseStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
            string html = reader.ReadToEnd();

            return html;

        }



    }



}
