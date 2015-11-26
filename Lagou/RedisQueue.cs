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
            _db = _connectionMultiplexer.GetDatabase();
        }

        public static ConnectionMultiplexer connection;
        public static ConnectionMultiplexer Connection
        {
            get
            {

                if (connection == null ||!connection.IsConnected)
                {
                    connection =  ConnectionMultiplexer.Connect("127.0.0.1,6379");
                }
                return connection;
            }
        }

        private static IDatabase cache
        {
            get
            {
                return Connection.GetDatabase();
            }

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

                /*
                “System.TimeoutException”类型的未经处理的异常在 StackExchange.Redis.dll 中发生 
                    其他信息: Timeout performing RPOP Job, inst: 2, mgr: ProcessReadQueue, err: never, 
                    queue: 0, qu: 0, qs: 0, qc: 0, wr: 0, wq: 0, in: 0, ar: 1, 
                    IOCP: (Busy=0,Free=1000,Min=4,Max=1000), WORKER: (Busy=6,Free=1017,Min=4,Max=1023),
                    clientName: ZERY-ZHANG
                */
                //可能问题stockExchange dll 有问题。2 redis 服务有问题 不稳定，3 如果队列中没有数据 再用pop会不会有问题？
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
