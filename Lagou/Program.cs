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
        private static RedisQueue redisQueue = null;

        static void Main(string[] args)
        {
            redisQueue = new RedisQueue();
            //定时器扫描Redis队列数据 存入数据库中
            InitThreadTimer();

            //取数据存入Redis队列中
            GetDataToRedisQueue();
          
            Console.Read();
        }


        /// <summary>
        /// 初始化定时器 (5s 扫描一次Redis队列)
        /// </summary>
        public static void InitThreadTimer()
        {
            timer = new Timer(SaveToDB, null, 0, 5000);
            Console.WriteLine("*************开始扫描Redis队列**************");

        }
        /// <summary>
        /// 从Reids队列中取中存入DB中
        /// </summary>
        /// <param name="obj"></param>
        public static void SaveToDB(object obj)
        {
           
            var jobList = redisQueue.Dequeue<List<JobEntity>>("Job");
            if (jobList == null || !jobList.Any())
            {
                return;
            }

            JobRepository respository = new JobRepository();
            respository.Insert(jobList);
        }

        /// <summary>
        /// 获取职位数据存入Redis队列
        /// </summary>
        public static void GetDataToRedisQueue()
        {
            
            JobUtility jobUtility = new JobUtility();
            string html = string.Empty;

            //jobType
            html = HttpUtility.SendGetHttpRequest("http://www.lagou.com/");
            var jobTypes = jobUtility.GetJobType(html);
            //City
            html = HttpUtility.SendGetHttpRequest("http://www.lagou.com/zhaopin/");
            var citys = jobUtility.GetCitys(html);
            //Save To Redis Queue
            jobUtility.SerialGetAllJobsToRedis(citys, jobTypes);

            //jobUtility.ParallelGetAllJobsToRedis(citys, jobTypes);


        }

    }

}
