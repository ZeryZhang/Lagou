using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Lagou.Repository;
using Newtonsoft.Json;
using Lagou.Repository.Entity;

namespace LagouDataAnalyze.Controllers
{

    public class LagouDataController : ApiController
    {
        private JobRepository repository;
        public LagouDataController()
        {
            repository = new JobRepository();
        }

        // GET api/values
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }


        /// <summary>
        /// 城市职位需求总数与公司总数
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public string QueryJCityNeedJobNum()
        {
            var jobCount = repository.QueryJCityNeedJobNum();
            var companyCount = repository.QueryCityCompanyNum();

            jobCount.ForEach(o =>
            {
                var company = companyCount.FirstOrDefault(c => c.City == o.City);
                if (company != null)
                {
                    o.CompanyNum = company.CompanyNum;
                }
            });
            var json = JsonConvert.SerializeObject(jobCount);

            return json;
        }

        /// <summary>
        /// 城市对某一职位的需求数据
        /// </summary>
        /// <param name="positionName"></param>
        /// <returns></returns>
        [HttpGet]
        public string QueryPositionNum(string positionName)
        {
            var jobCount = repository.QueryPositionNum(positionName);

            return JsonConvert.SerializeObject(jobCount);
                
        }

        public string QuerySalaryWorkYear(string positionName)
        {
            var result = repository.QueryWorkYearSalary(positionName);

        }

        /// <summary>
        /// 划定薪水区间
        /// </summary>
        /// <param name="salary"></param>
        private void getSalaryRange(string salary)
        {
            /*
             *0-5K
             *6-10K
             *11-15K
             *16-20K
             *21-25K
             *26-30K
             *30以上
             */
            
            
            //6k-10k
            //(?<s>\d+(?=k-))取前数据
            //(?<e>(?<=k-)\d+) 取后数据

            //3k以上  
            //(?<e>\d+(?=\D))取数字
            //(?<e>\W\D) 取中文 


        }


        // GET api/values/5
        //public string Get(int id)
        //{
        //    return "value";
        //}

        // POST api/values
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}