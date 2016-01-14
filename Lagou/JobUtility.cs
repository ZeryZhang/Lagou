using HtmlAgilityPack;
using Lagou.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Lagou
{
    public class JobUtility
    {

        /// <summary>
        /// 取职位分类
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public List<JobTypeModel> GetJobType(string html)
        {

            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(html);
            HtmlNodeCollection jobCatgroy = document.DocumentNode.SelectNodes("//div[@class='mainNavs']/div[@class='menu_box'][position()=1]/div/dl"); //取大分类

            List<JobTypeModel> JobTypeList = new List<JobTypeModel>();
            //大分类(只取前3类)
            for (int i = 0; i < 3; i++)
            {
                HtmlNode dlNode = jobCatgroy[i];

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
        /// <summary>
        /// 取城市
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public List<CityModel> GetCitys(string html)
        {
            HtmlDocument document = new HtmlDocument();
            List<CityModel> cityList = new List<CityModel>();

            document.LoadHtml(html);

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
        /// <summary>
        /// 反序列化成实体对象
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        private ReturnData DeserializeJob(string html)
        {
            ReturnData returnData = new ReturnData();

            if (!string.IsNullOrEmpty(html))
            {
                returnData = JsonConvert.DeserializeObject<ReturnData>(html);
            }

            return returnData;
        }


        /// <summary>
        /// 并行获取所有岗位信息
        /// </summary>
        /// <param name="citys"></param>
        /// <param name="jobTypes"></param>
        /// <returns></returns>
        public List<JobModel> ParallelGetAllJobsToRedis(List<CityModel> citys, List<JobTypeModel> jobTypes)
        {
            string postData = string.Empty;
           
            object locker = new object();
            
            RedisQueue redisQueue = new RedisQueue();
            try
            {
                Parallel.ForEach(citys, o =>
                {
                    
                    string  url = string.Format("http://www.lagou.com/jobs/positionAjax.json?city={0}", o.CityName);

                    foreach (var jobType in jobTypes)
                    {
                        var jobList = new List<JobModel>();
                        for (int  i = 1; i <= 40; i++)
                        {
                            Console.WriteLine(" 当前城市: {0} 职位: {1} ,第: {2} 页数据", o.CityName, jobType.JobName, i);
                            postData = string.Format("first=false&pn={0}&kd={1}", i, jobType.JobName);

                            string jobJson = HttpUtility.SendPostHttpRequest(url, postData);
                            var jobData = DeserializeJob(jobJson);

                            if (i == 40 || jobData.content.result == null || !jobData.content.result.Any())
                            {//只取40页数据
                                lock (locker)
                                {
                                    redisQueue.Enqueue("Job", jobList);
                                }
                                jobList = new List<JobModel>();
                                Console.WriteLine("*********** {0} 查询完成,共 {1} 页数据***********", jobType.JobName, i);
                                break;
                            }
                            else
                            {
                                //Save To Redis Queue
                                jobList.AddRange(jobData.content.result);

                            }
                        }
                    }
                });

            }
            catch (Exception ex)
            {
                Console.WriteLine("发生异常{0}", ex.Message);
                new CommonException("请求数据异常", ex);
            }

            return null;
        }


        /// <summary>
        /// 串行获取所有岗位信息
        /// </summary>
        /// <param name="citys"></param>
        /// <param name="jobTypes"></param>
        /// <returns></returns>
        public List<JobModel> SerialGetAllJobsToRedis(List<CityModel> citys, List<JobTypeModel> jobTypes)
        {
            string postData = string.Empty;
            var jobList = new List<JobModel>();
            RedisQueue redisQueue = new RedisQueue();

            try
            {
                foreach (var city in citys)
                {
                    string url = string.Format("http://www.lagou.com/jobs/positionAjax.json?city={0}", city.CityName);
                    foreach (var jobType in jobTypes)
                    {
                        for (int i = 1; i <= 40; i++)
                        {
                            Console.WriteLine(" 当前城市: {0} 职位: {1} ,第: {2} 页数据", city.CityName, jobType.JobName, i);
                            postData = string.Format("first=false&pn={0}&kd={1}", i, jobType.JobName);

                            string jobData = HttpUtility.SendPostHttpRequest(url, postData);
                            var jobObject = DeserializeJob(jobData);

                            if (i == 40 || jobObject.content.result == null || !jobObject.content.result.Any())
                            {
                                redisQueue.Enqueue("Job", jobList);
                                jobList = new List<JobModel>();

                                Console.WriteLine("*********** {0} 查询完成,共 {1} 页数据***********", jobType.JobName, i);
                                break;
                            }
                            else
                            {
                                //Save To Redis Queue
                                jobList.AddRange(jobObject.content.result);

                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                new CommonException("请求数据异常", ex);
                Console.WriteLine("请求数据异常原因：{0}", ex.Message);
            }

            return jobList;
        }
    }
}
