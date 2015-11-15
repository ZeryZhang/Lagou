using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Configuration;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace Lagou
{
    public class RedisQueue
    {
        private object locker = new object();
        private   ConnectionMultiplexer _connectionMultiplexer; 
        private  IDatabase _db;
        public RedisQueue()
        {
            ConfigurationOptions options = new ConfigurationOptions()
            {
                Ssl = false,
                AllowAdmin = false
            };
           // options.EndPoints.Add("127.0.0.1", 6379);
            _connectionMultiplexer = ConnectionMultiplexer.Connect("127.0.0.1,6379");
            _db = _connectionMultiplexer.GetDatabase(0);
        }

       

        /// <summary>
        /// Push
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="equeueName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Enqueue<T>(string equeueName, T value)
        {
            bool result = false;
            if (!string.IsNullOrEmpty(equeueName))
            {
                lock (locker)
                {
                    RedisValue redisValue = JsonConvert.SerializeObject(value);
                    result = _db.ListLeftPush(equeueName, redisValue) > 0;
                }
            }

            return result;
        }

        public bool Enqueue<T>(string equeueName, List<T> value)
        {
            bool result = false;
            if (!string.IsNullOrEmpty(equeueName))
            {
                lock (locker)
                {
                    RedisValue redisValue = JsonConvert.SerializeObject(value);
                    result = _db.ListLeftPush(equeueName, redisValue) > 0;
                }
            }

            return result;
        }

        /// <summary>
        /// Pop
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="equeueName"></param>
        /// <returns></returns>
        public T Dequeue<T>(string equeueName) where T : class
        {
            if (!string.IsNullOrEmpty(equeueName))
            {

                if (!_db.KeyExists("Job"))
                {
                    return default(T);
                }

               RedisValue value =  _db.ListRightPop(equeueName);
                if (value.HasValue)
                {
                    return JsonConvert.DeserializeObject<T>(value);
                }
                
            }

            return default(T);
        }

    }
}
