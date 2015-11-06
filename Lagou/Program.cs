using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Lagou.Repository;
using Newtonsoft.Json;

namespace Lagou
{
    class Program
    {
        static void Main(string[] args)
        {

            //EF
            JobRepository repository = new JobRepository();

            repository.Insert(new Lagou.Repository.JobEntity()
            {
                Score = 3424234,
                CompanyId="534534535345",
                CompanyLogo="533453",
                CreateTime =  DateTime.Now
            });



            HttpUtilty httpUtilty = new HttpUtilty();
            JobRelative jobRelative = new JobRelative();
            string html = string.Empty;
            string url = string.Empty;

            //RedisCache redisCache = new RedisCache();
            //redisCache.Set("zery", "zhang");

            //RedisQueue redisQueue = new RedisQueue();
            //bool result = redisQueue.Enqueue("Job", "Zhangqipeng");

            //Console.Read();
            ////jobType
            html = httpUtilty.SendGetHttpRequest("http://www.lagou.com/");
            var jobTypes = jobRelative.GetJobType(html);
            ////City
            html = httpUtilty.SendGetHttpRequest("http://www.lagou.com/zhaopin/");
            var citys = jobRelative.GetCitys(html);
            var jobList = jobRelative.SerialGetAllJobs(citys, jobTypes);

            //url = string.Format("http://www.lagou.com/jobs/positionAjax.json?city={0}", "深圳");
            //string body = "first=false&pn=2&kd=Python";
            //html = httpUtilty.SendPostHttpRequest(url, body);
            //var returnData =  jobRelative.SerializeJob(html);

            //RedisQueue redisQueue = new RedisQueue();
            //bool result = redisQueue.Enqueue("Job", returnData.content.result);

       
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

        public ReturnData SerializeJob(string html)
        {
            ReturnData returnData = new ReturnData();
            if (!string.IsNullOrEmpty(html))
            {

                returnData = JsonConvert.DeserializeObject<ReturnData>(html);
            }

            return returnData;
        }

        public List<JobEntity> ParallelGetAllJobs(List<CityEntity> citys, List<JobTypeEntity> jobTypes)
        {
            string url = string.Empty;
            string postData = string.Empty;
            var jobList = new List<JobEntity>();
            HttpUtilty httpUtilty = new HttpUtilty();
            JobRelative jobRelative = new JobRelative();
            RedisQueue redisQueue = new RedisQueue();
            try
            {

                Parallel.ForEach(citys, o =>
                {
                    Thread.Sleep(1000);

                    url = string.Format("http://www.lagou.com/jobs/positionAjax.json?city={0}", o.CityName);
                    Parallel.ForEach(jobTypes, c =>
                    {
                        Thread.Sleep(1000);
                        for (int i = 1; i <= 30; i++)
                        {
                            Thread.Sleep(1000);
                            Console.WriteLine("当前是{0},第{1}页数据", c.JobName, i);
                            postData = string.Format("first=false&pn={0}&kd={1}", i, c.JobName);

                            string jobJson = httpUtilty.SendPostHttpRequest(url, postData);
                            var jobdata = jobRelative.SerializeJob(jobJson);

                            if (jobdata.content.result == null || !jobdata.content.result.Any())
                            {
                                Console.WriteLine("==========={0}查询完成,共{1}页数据=========", c.JobName, i);
                                break;
                            }
                            else
                            {
                                //Save To Redis Queue
                                redisQueue.Enqueue("Job", jobdata.content.result);
                                jobList.AddRange(jobdata.content.result);
                            }
                        }

                    });
                });


            }
            catch (Exception ex)
            {

                Console.WriteLine("发生异常{0}", ex.Message);

            }

            return jobList;
        }




        public List<JobEntity> SerialGetAllJobs(List<CityEntity> citys, List<JobTypeEntity> jobTypes)
        {
            string url = string.Empty;
            string postData = string.Empty;
            var jobList = new List<JobEntity>();
            HttpUtilty httpUtilty = new HttpUtilty();
            JobRelative jobRelative = new JobRelative();
            RedisQueue redisQueue = new RedisQueue();
            try
            {
                foreach (var city in citys)
                {
                    url = string.Format("http://www.lagou.com/jobs/positionAjax.json?city={0}", city.CityName);
                    foreach (var jobType in jobTypes)
                    {
                        for (int i = 1; i <= 30; i++)
                        {
                            Thread.Sleep(1000);
                            Console.WriteLine("当前城市{0}职位{1},第{2}页数据",city.CityName, jobType.JobName, i);
                            postData = string.Format("first=false&pn={0}&kd={1}", i, jobType.JobName);

                            string jobJson = httpUtilty.SendPostHttpRequest(url, postData);
                            var jobdata = jobRelative.SerializeJob(jobJson);

                            if (jobdata.content.result == null || !jobdata.content.result.Any())
                            {
                                Console.WriteLine("==========={0}查询完成,共{1}页数据=========", jobType.JobName, i);
                                break;
                            }
                            else
                            {
                                //Save To Redis Queue
                                redisQueue.Enqueue("Job", jobdata.content.result);
                                //jobList.AddRange(jobdata.content.result);
                            }
                        }


                    }

                }

            }
            catch (Exception ex)
            {

                Console.WriteLine("发生异常{0}", ex.Message);

            }

            return jobList;
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
            request.GetRequestStream().Write(bytes, 0, bytes.Length);

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream responseStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
            string html = reader.ReadToEnd();

            return html;
        }


    }



}
