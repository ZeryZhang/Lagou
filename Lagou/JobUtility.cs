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
        public List<JobModel> ParallelGetAllJobs(List<CityModel> citys, List<JobTypeModel> jobTypes)
        {
            string url = string.Empty;
            string postData = string.Empty;
            var jobList = new List<JobModel>();
            object locker = new object();
            HttpUtility httpUtilty = new HttpUtility();
            RedisQueue redisQueue = new RedisQueue();
            try
            {

                Parallel.ForEach(citys, o =>
                {
                    //Thread.Sleep(1000);

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
                            var jobdata = DeserializeJob(jobJson);

                            if (i == 40 || jobdata.content.result == null || !jobdata.content.result.Any())
                            {
                                lock(locker)
                                {
                                   // redisQueue.Enqueue("Job", jobList);
                                }
                                jobList = new List<JobModel>();
                                Console.WriteLine("==========={0}查询完成,共{1}页数据=========", c.JobName, i);
                                break;
                            }
                            else
                            {
                                //Save To Redis Queue
                                //redisQueue.Enqueue("Job", jobdata.content.result);
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


        /// <summary>
        /// 串行获取所有岗位信息
        /// </summary>
        /// <param name="citys"></param>
        /// <param name="jobTypes"></param>
        /// <returns></returns>
        public List<JobModel> SerialGetAllJobs(List<CityModel> citys, List<JobTypeModel> jobTypes)
        {
            string url = string.Empty;
            string postData = string.Empty;
            var jobList = new List<JobModel>();
            HttpUtility httpUtilty = new HttpUtility();
            RedisQueue redisQueue = new RedisQueue();
            
            try
            {
                foreach (var city in citys)
                {
                    url = string.Format("http://www.lagou.com/jobs/positionAjax.json?city={0}", city.CityName);
                    foreach (var jobType in jobTypes)
                    {
                        for (int i = 1; i <= 40; i++)
                        {
                            //Thread.Sleep(500);
                            Console.WriteLine("当前城市{0}职位{1},第{2}页数据", city.CityName, jobType.JobName, i);
                            postData = string.Format("first=false&pn={0}&kd={1}", i, jobType.JobName);

                            string jobJson = httpUtilty.SendPostHttpRequest(url, postData);
                            var jobdata = DeserializeJob(jobJson);

                            if (i==40||jobdata.content.result == null || !jobdata.content.result.Any())
                            {
                                redisQueue.Enqueue("Job", jobList);
                                jobList = new List<JobModel>();
                                Console.WriteLine("==========={0}查询完成,共{1}页数据=========", jobType.JobName, i);
                                break;
                            }
                            else
                            {
                                //Save To Redis Queue
                                //redisQueue.Enqueue("Job", jobdata.content.result);
                                jobList.AddRange(jobdata.content.result);

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
