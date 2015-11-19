using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lagou.Repository.Entity
{
    public class CityCompanyJobEntity
    {
        public string City { get; set; }

        public int JobNum { get; set; }

        public int CompanyNum { get; set; }
    }

    //同一职位不同城市,各个年限的薪水 实体
    public class WorkYearSalaryEntity
    {
        public string City { get; set; }

        public int JobNum { get; set; }

        public string Salary { get; set; }

        public string WorkYear { get; set; }
    }

}
