using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lagou
{
    public class LogHelper
    {

        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="log"></param>
        public static  void Writetxtlog(string log)
        {
            try
            {
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "lagoulog.txt");
                bool isExists = File.Exists(path);
                if (!isExists)
                {
                    File.Create(path);
                }

                byte[] buffer = Encoding.UTF8.GetBytes(log + "\r\n");
                FileStream wirter = File.Open(path, FileMode.Append); //Append to file 
                wirter.Write(buffer, 0, buffer.Length);
                wirter.Flush();
                wirter.Close();

            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
