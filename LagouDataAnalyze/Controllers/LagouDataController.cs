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



        // GET api/values/5
        public string Get(int id)
        {
            return "value";
        }

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