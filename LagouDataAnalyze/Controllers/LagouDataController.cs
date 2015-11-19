using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Lagou.Repository;
using Newtonsoft.Json;
using Lagou.Repository.Entity;
using System.Text.RegularExpressions;

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




            return string.Empty;

        }

        /// <summary>
        /// 划定薪水区间
        /// </summary>
        /// <param name="salary"></param>
        private string getSalaryRange(string salary)
        {

            /*
             *0k-5K
             *6k-10K
             *11k-15K
             *16k-20K
             *21k-25K
             *26k-30K
             *30k以上
             */
            Regex regex;
            if (!string.IsNullOrEmpty(salary))
            {
                regex = new Regex(@"(?<salary>(?<=k-)\d+)");
                string salaryValue = regex.Match(salary).Groups["salary"].Value;
                int value = string.IsNullOrEmpty(salaryValue) ? 0 : Convert.ToInt32(salaryValue);
                return getRange(value);
            }

            if (salary.Contains("以上"))
            {
                 regex = new Regex(@"(?<salary>\d+(?=\D))",RegexOptions.Singleline);
                string salaryValue = regex.Match(salary).Groups["salary"].Value;
                int value = string.IsNullOrEmpty(salaryValue) ? 0 : Convert.ToInt32(salaryValue);
                return getRange(value);
            }
            else if (salary.Contains("以下"))
            {
                regex = new Regex(@"(?<salary>\d+(?=\D))", RegexOptions.Singleline);
                string salaryValue = regex.Match(salary).Groups["salary"].Value;
                int value = string.IsNullOrEmpty(salaryValue) ? 0 : Convert.ToInt32(salaryValue);
                return getRange(value);
            }


            return string.Empty;
            //6k-10k
            //(?<s>\d+(?=k-))取前数据
            //(?<e>(?<=k-)\d+) 取后数据

            //3k以上  
            //(?<e>\d+(?=\D))取数字
            //(?<e>\W\D) 取中文 

        }

        private string getRange(int salary)
        {
            /*
            *0k-5K
            *6k-10K
            *11k-15K
            *16k-20K
            *21k-25K
            *26k-30K
            *30k以上
            */
            //未确定范围 **以上  的类型
            if (salary == 0)
            {
                return "0k-5k";
            }
            else if (salary > 0 && salary <= 5)
            {
                return "0k-5k";
            }
            else if (salary > 5 && salary <= 10)
            {
                return "6k-10K";
            }
            else if (salary > 10 && salary <= 15)
            {
                return "11k-15K";
            }
            else if (salary > 15 && salary <= 20)
            {
                return "16k-20K";
            }
            else if (salary > 20 && salary <= 25)
            {
                return "26k-30K";
            }
            else {
                return "30k以上";
            }
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