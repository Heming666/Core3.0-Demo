using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DBModel
{
    public static class RedisClient
    {

        private static ConnectionMultiplexer redisMultiplexer;

        static IDatabase  db = null;


        public static void InitConnect(string conStr)
        {
            try
            {
                redisMultiplexer = ConnectionMultiplexer.Connect(conStr);
                db = redisMultiplexer.GetDatabase();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                redisMultiplexer = null;
                db = null;
            }
        }


        #region String 
        /// <summary>
        /// 保存单个key value
        /// </summary>
        /// <param name="value">保存的值</param>
        /// <param name="expiry">过期时间</param>
        public static bool SetStringKey(string key, string value, TimeSpan? expiry = default(TimeSpan?))
        {
            return db.StringSet(key, value, expiry);
        }

        /// <summary>
        /// 获取单个key的值
        /// </summary>
        public static RedisValue GetStringKey(string key)
        {
            return db.StringGet(key);
        }


        /// <summary>
        /// 获取一个key的对象
        /// </summary>
        public static T GetStringKey<T>(string key) where T : class, new()
        {
            if (db == null)
            {
                return default;
            }
            var value = db.StringGet(key);
            if (value.IsNullOrEmpty)
            {
                return default;
            }
            return JsonConvert.DeserializeObject<T>(value);
        }

        /// <summary>
        /// 保存一个对象
        /// </summary>
        /// <param name="obj"></param>
        public static bool SetStringKey<T>(string key, T obj, TimeSpan? expiry = default(TimeSpan?)) where T : class,new ()
        {
            if (db == null)
            {
                return false;
            }
            string json = JsonConvert.SerializeObject(obj);
            return db.StringSet(key, json, expiry);
        }


        public static Task<bool> SetList<T>(string key, T obj, TimeSpan? expiry = default(TimeSpan?)) where T : class, new()
        {
            return Task.Run(() =>
            {
                if (db == null)
                {
                    return false;
                }
                string json = JsonConvert.SerializeObject(obj);
                return db.ListLeftPushAsync(key,json).Result > 0;
            });
        }

        public static Task<bool> SetList<T>(string key, List<T> objList,  TimeSpan? expiry = default(TimeSpan?))
        {
            return Task.Run(() =>
            {
                if (db == null)
                {
                    return false;
                }
                List<RedisValue> redisValues = new List<RedisValue>();
                if (objList != null && objList.Count > 0)
                {
                    foreach (var item in objList)
                    {
                        redisValues.Add(JsonConvert.SerializeObject(item));
                    }

                    return db.ListLeftPushAsync(key, redisValues.ToArray()).Result > 0;
                }
                return false;
            });
        }

        public static Task<List<T>> GetList<T>(string key, TimeSpan? expiry = default(TimeSpan?)) where T : class, new()
        {
            return  Task.Run(() =>
            {
                if (db == null)
                {
                    return null;
                }
                var value= db.ListRangeAsync(key);
                if (value != null && value.Result.Length > 0)
                {
                    List<T> list = new List<T>();
                    foreach (var item in value.Result)
                    {
                        list.Add(JsonConvert.DeserializeObject<T>(item));
                    }
                    return list;
                }
                return null;
            });
        }

        public static Task<bool> HashSet<T>(string key, T obj, string fielId,TimeSpan? expiry = default(TimeSpan?)) where T : class, new()
        {
            return Task.Run(() =>
            {
                if (db == null)
                {
                    return false;
                }
                string json = JsonConvert.SerializeObject(obj);
                return db.HashSet(key, fielId, json);
            });
        }
        public static Task<bool> HashSet(string key, HashEntry[] hashEntries, TimeSpan? expiry = default(TimeSpan?)) 
        {
            return Task.Run(() =>
            {
                if (db == null)
                {
                    return false;
                }
                 db.HashSet(key, hashEntries);
                return true;
            });
        }
        public static Task<List<T>> HashGetAll<T>(string key, TimeSpan? expiry = default(TimeSpan?)) where T : class, new()
        {
            return Task.Run(() =>
            {
                if (db == null)
                {
                    return null ;
                }
                var value = db.HashGetAll(key);
                if (value !=null && value.Length>0)
                {
                    List<T> list = new List<T>();
                    foreach (var item in value)
                    {
                        list.Add(JsonConvert.DeserializeObject<T>(item.Value));
                    }
                    return list;
                }
                return null;
            });
        }
        public static Task<List<T>> HashGet<T>(string key,RedisValue[] fielIds, TimeSpan? expiry = default(TimeSpan?)) where T : class, new()
        {
            return Task.Run(() =>
            {
                if (db == null)
                {
                    return null;
                }
                var value = db.HashGet(key,fielIds);
                if (value != null && value.Length > 0)
                {
                    List<T> list = new List<T>();
                    foreach (var item in value)
                    {
                        list.Add(JsonConvert.DeserializeObject<T>(item));
                    }
                    return list;
                }
                return null;
            });
        }
        public static Task<T> HashGet<T>(string key, string fielId, TimeSpan? expiry = default(TimeSpan?)) where T : class, new()
        {
            return Task.Run(() =>
            {
                if (db == null)
                {
                    return null;
                }
                var value = db.HashGet(key, fielId);
                if (value.IsNullOrEmpty)
                {
                    return default;
                }
                return JsonConvert.DeserializeObject<T>(value);
            });
        }
        #endregion
    }
}