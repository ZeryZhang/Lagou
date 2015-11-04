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

        private readonly ConnectionMultiplexer _connectionMultiplexer;
        private readonly IDatabase _db;
        public RedisQueue()
        {
            ConfigurationOptions options = new ConfigurationOptions()
            {
                Ssl = true,
                AllowAdmin = true
            };
            options.EndPoints.Add("192.168.0.121",6379);
            _connectionMultiplexer = ConnectionMultiplexer.Connect(options);
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
                RedisValue redisValue = JsonConvert.SerializeObject(value);
                result =_db.ListLeftPush(equeueName, redisValue) > 0;
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
               RedisValue value =  _db.ListRightPop(equeueName);
                return JsonConvert.DeserializeObject<T>(value);
            }

            return default(T);
        }

    }
}
