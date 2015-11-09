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
using Lagou.Model;
using Lagou.Repository;
using Newtonsoft.Json;

namespace Lagou
{

    class Program
    {
        private static Timer timer;
        private static AsynExcute asynExcute = new AsynExcute();
        static void Main(string[] args)
        {
            try
            {
                int i = 0;
                int j = 1;
                var k = j/i;
            }
            catch (Exception ex)
            {
                    
               new CommonException(ex.Message,ex);
            }


            AsynExcute anycExcute = new AsynExcute();

            //爬取数据
            //Thread getJobThread = new Thread(new ThreadStart(anycExcute.SaveToRedisQueue));
            //getJobThread.IsBackground = true;
            //getJobThread.Start();


            //定时器扫描Redis队列数据 存入数据库中
            //InitThreadTimer();
            Console.Read();


        }

        public static void InitThreadTimer()
        {
            timer = new Timer(SaveToDB, null, 0, 500);
            Console.WriteLine("定时器开始执行.......");

        }
        public static void SaveToDB(object obj)
        {

            RedisQueue redisQueue = new RedisQueue();
            var jobList = redisQueue.Dequeue<List<JobEntity>>("Job");
            if (jobList == null || !jobList.Any())
            {
                return;
            }

            JobRepository respository = new JobRepository();
            respository.Insert(jobList);
        }

    }


    public class AsynExcute
    {
        public void SaveToRedisQueue()
        {

            HttpUtilty httpUtilty = new HttpUtilty();
            JobRelative jobRelative = new JobRelative();
            string html = string.Empty;
            string url = string.Empty;

            //jobType
            html = httpUtilty.SendGetHttpRequest("http://www.lagou.com/");
            var jobTypes = jobRelative.GetJobType(html);
            //City
            html = httpUtilty.SendGetHttpRequest("http://www.lagou.com/zhaopin/");
            var citys = jobRelative.GetCitys(html);
            //Save To Redis Queue
            jobRelative.SerialGetAllJobs(citys, jobTypes);

        }

    }


    public class HttpUtilty
    {
        public string SendGetHttpRequest(string url)
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
                request.ContentType = "keep-alive";
                //request.Host = "www.cnblogs.com";
                request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; rv:25.0) Gecko/20100101 Firefox/25.0";

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream responseStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                html = reader.ReadToEnd();
            }
            catch (Exception ex)
            {
                new CommonException(ex.Message, ex);
            }

            return html;

        }

        public string SendPostHttpRequest(string url, string body)
        {
            string html = string.Empty;
            try
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
                html = reader.ReadToEnd();
            }
            catch (Exception ex)
            {
                new CommonException(ex.Message, ex);
            }

            return html;
        }


    }


    public class JobRelative
    {
        public List<JobTypeModel> GetJobType(string html)
        {

            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(html);
            HtmlNodeCollection jobCatgroy = document.DocumentNode.SelectNodes("//dl[@class='reset']"); //取大分类

            List<JobTypeModel> JobTypeList = new List<JobTypeModel>();
            //大分类
            for (int i = 0; i < 3; i++)
            {
                HtmlNode dlNode = jobCatgroy[i];
                string href = string.Empty;
                HtmlNode aNode = dlNode.SelectSingleNode("dt/a");

                List<HtmlNode> jobNodes = dlNode.SelectNodes("dd/a").ToList();
                jobNodes.ForEach(o => JobTypeList.Add(
                    new JobTypeModel()
                    {
                        JobName = o.InnerText,
                        JobHref = o.GetAttributeValue("href", string.Empty)
                    }));
            }

            return JobTypeList;
        }

        public List<CityModel> GetCitys(string html)
        {
            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(html);
            List<CityModel> cityList = new List<CityModel>();
            List<HtmlNode> hotCityNodes = document.DocumentNode.SelectNodes("//div[@class='has-more'][1]/li/a[position()>1]").ToList();
            HtmlNode li = document.DocumentNode.SelectNodes("//li[@class='other']").FirstOrDefault();
            List<HtmlNode> otherCityNodes = li.SelectNodes("a").ToList();
            hotCityNodes.ForEach(o => cityList.Add(
                    new CityModel()
                    {
                        CityName = o.InnerText,
                        CityHref = o.GetAttributeValue("href", string.Empty)
                    }
                ));

            otherCityNodes.ForEach(o => cityList.Add(
                   new CityModel()
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



        public List<JobModel> ParallelGetAllJobs(List<CityModel> citys, List<JobTypeModel> jobTypes)
        {
            string url = string.Empty;
            string postData = string.Empty;
            var jobList = new List<JobModel>();
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


        public List<JobModel> SerialGetAllJobs(List<CityModel> citys, List<JobTypeModel> jobTypes)
        {
            string url = string.Empty;
            string postData = string.Empty;
            var jobList = new List<JobModel>();
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
                            Thread.Sleep(1500);
                            Console.WriteLine("当前城市{0}职位{1},第{2}页数据", city.CityName, jobType.JobName, i);
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
                new CommonException(ex.Message, ex);
                Console.WriteLine("发生异常{0}", ex.Message);
            }

            return jobList;
        }

    }








}
