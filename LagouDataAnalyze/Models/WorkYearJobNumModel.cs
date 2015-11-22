using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Lagou.Web
{
    public class WorkYearJobNumModel
    {
        public string city { get; set; }
        public string name { get; set; }

        public string type {get;set;}

        public List<int> data { get; set; }
    }
}