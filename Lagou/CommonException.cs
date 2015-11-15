using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Lagou
{
    public class CommonException : ApplicationException
    {
        public CommonException() : base()
        {
           
        }

        public CommonException(SerializationInfo serialization, StreamingContext context)
            : base(serialization, context)
        {
            
        }

        public CommonException(string message, Exception innerException) : base(message, innerException)
        {
            string errorMgs = string.Format(" 错误来源:{0}\r\n 错误信息:{1}\r\n 调用堆栈{2}\r\n 异常时间:{3} ", innerException.Source,message, innerException.StackTrace,DateTime.Now);
            LogHelper.Writetxtlog(errorMgs);
        }

      

        public override Exception GetBaseException()
        {
            return base.GetBaseException();
        }
    }
}
