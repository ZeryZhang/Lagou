using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lagou.Model
{
    public class ReturnData
    {
        public string resubmitToken { get; set; }
        public bool success { get; set; }
        public string requestId { get; set; }
        public string msg { get; set; }
        public string code { get; set; }
        public ContentModel content { get; set; }
    }
}
