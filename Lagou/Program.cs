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
using Newtonsoft.Json;

namespace Lagou
{
    class Program
    {
        static void Main(string[] args)
        {

            HttpUtilty httpUtilty = new HttpUtilty();
            JobRelative jobRelative = new JobRelative();
            string html = string.Empty;
            string url = string.Empty;
            //jobType
            //html = httpUtilty.SendGetHttpRequest("http://www.lagou.com/");
            //jobRelative.GetJobType(html);
            //City
            //html = httpUtilty.SendGetHttpRequest("http://www.lagou.com/zhaopin/");
            //jobRelative.GetCitys(html);
            url = string.Format("http://www.lagou.com/jobs/positionAjax.json?city={0}", "深圳");
            string body = "first=false&pn=2&kd=Python";
            html = httpUtilty.SendPostHttpRequest(url, body);
            jobRelative.GetJob(html);
            /*
             * 1 遍历每个城市下的每一种职位类型
             * 2 遍历每页数据，(没数据时要处理)
             * 
             */
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
            List<HtmlNode> hotCityNodes = document.DocumentNode.SelectNodes("//div[@class='has-more'][1]/li/a[position()>1]").ToList();
            HtmlNode li = document.DocumentNode.SelectNodes("//li[@class='other']").FirstOrDefault();
            List<HtmlNode> otherCityNodes = li.SelectNodes("a").ToList();
            hotCityNodes.ForEach(o => cityList.Add(
                    new CityEntity()
                    {
                        CityName = o.InnerText,
                        CityHref = o.GetAttributeValue("href", string.Empty)
                    }
                ));

            otherCityNodes.ForEach(o => cityList.Add(
                   new CityEntity()
                   {
                       CityName = o.InnerText,
                       CityHref = o.GetAttributeValue("href", string.Empty)
                   }
               ));

            return cityList;
        }

        public ReturnData GetJob(string html)
        {
            ReturnData returnData = new ReturnData();
            if (!string.IsNullOrEmpty(html))
            {

                returnData = JsonConvert.DeserializeObject<ReturnData>(html);
            }

            return returnData;
        }

    }

    public class JobTypeEntity
    {
        public string JobName { get; set; }

        public string JobHref { get; set; }
    }

    public class CityEntity
    {
        public string CityName { get; set; }

        public string CityHref { get; set; }
    }

    public class JobEntity
    {
        public int score { get; set; }
        public DateTime createTime { get; set; }
        public string formatCreateTime { get; set; }
        public string positionId { get; set; }
        public string positionName { get; set; }
        /// <summary>
        /// 职位分类
        /// </summary>
        public string positionType { get; set; }//后端开发

        public string workYear { get; set; }
        /// <summary>
        /// 教育程度
        /// </summary>
        public string education { get; set; }
        /// <summary>
        /// 工作性质 (全职)
        /// </summary>
        public string jobNature { get; set; }
        public string companyName { get; set; }
        public string companyId { get; set; }
        public string city { get; set; }
        public string companyLogo { get; set; }
        /// <summary>
        /// 行业
        /// </summary>
        public string industryField { get; set; }
        //职位福利
        public string positionAdvantag { get; set; }// : 千万级用户产品+年底奖金+分红+期权激励 ,
        /// <summary>
        /// 薪水范畴
        /// </summary>
        public string salary { get; set; }

        public string positionFirstType { get; set; } //: 技术 ,

        public string leaderName { get; set; }// : 王乐 ,
        //公司人数
        public string companySize { get; set; }
        /// <summary>
        /// 融资情况
        /// </summary>
        public string financeStage { get; set; }//: 成长型(A轮) ,
        /// <summary>
        /// 公司福利
        /// </summary>
        public List<string> companyLabelList { get; set; }//:[节日礼物 ,带薪年假 ,绩效奖金 ,年度旅游 ]
    }

    public class ContentEntity
    {
        public int totalCount { get; set; }
        public int pageNo { get; set; }
        public int pageSize { get; set; }
        public bool hasNextPage { get; set; }
        public int totalPageCount { get; set; }
        public int currentPageNo { get; set; }
        public bool hasPreviousPage { get; set; }
        public List<JobEntity> result { get; set; }
    }

    public class ReturnData
    {
        public string resubmitToken { get; set; }
        public bool success { get; set; }
        public string requestId { get; set; }
        public string msg { get; set; }
        public string code { get; set; }
        public ContentEntity content { get; set; }
    }

    public class HttpUtilty
    {
        public string SendGetHttpRequest(string url)
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

        public string SendPostHttpRequest(string url, string body)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            //WebProxy webproxy = new WebProxy();
            //Uri uri = new Uri(string.Format("http://{0}:{1}", "Adrress", "Port"));
            //webproxy.Address = uri;
            //request.Proxy = webproxy;

            request.Accept = "*/*";
            request.Method = "POST";
            request.Headers.Add("Accept-Language", "zh-cn,zh;q=0.8,en-us;q=0.5,en;q=0.3");
            request.ContentType = "application/x-www-form-urlencoded; charset=UTF-8"; //表单提交
            //request.Host = "www.cnblogs.com";
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; rv:25.0) Gecko/20100101 Firefox/25.0";
            byte[] bytes = Encoding.UTF8.GetBytes(body);
            request.ContentLength = bytes.Length;
            request.GetRequestStream().Write(bytes,0,bytes.Length);

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream responseStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
            string html = reader.ReadToEnd();

            return html;
        }


    }



}
