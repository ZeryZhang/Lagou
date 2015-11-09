using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lagou.Model
{
    public class ContentModel
    {
        public int totalCount { get; set; }
        public int pageNo { get; set; }
        public int pageSize { get; set; }
        public bool hasNextPage { get; set; }
        public int totalPageCount { get; set; }
        public int currentPageNo { get; set; }
        public bool hasPreviousPage { get; set; }
        public List<JobModel> result { get; set; }
    }

    public class JobTypeModel
    {
        public string JobName { get; set; }

        public string JobHref { get; set; }
    }

    public class CityModel
    {
        public string CityName { get; set; }

        public string CityHref { get; set; }
    }
}
