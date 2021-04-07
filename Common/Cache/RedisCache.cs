using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Cache
{
    public class RedisCache : ICache
    {
        public int TimeOut { get; set; }

        public bool Exists(string key)
        {

            #region redis
            //var redislist = RedisClient.GetList<FileModel>("list");
            //if (redislist.Result != null && redislist.Result.Count > 0)
            //{
            //    videoList = redislist.Result;

            //} else
            //{
            //    var filePaths = Directory.GetFiles(_path);
            //    videoList = GetAllVideo(filePaths);
            //    RedisClient.SetList("list", videoList);
            //}

            //var hashList = RedisClient.HashGetAll<FileModel>("hash");
            //if (hashList.Result != null && hashList.Result.Count > 0)
            //{
            //    videoList = redislist.Result;

            //}
            //else
            //{
            //    var filePaths = Directory.GetFiles(_path);
            //    videoList = GetAllVideo(filePaths);
            //    List<HashEntry> hashEntries = new List<HashEntry>();
            //    if (videoList != null && videoList.Count > 0)
            //    {
            //        foreach (var item in videoList)
            //        {
            //            hashEntries.Add(new HashEntry(item.FileName, JsonConvert.SerializeObject(item)));
            //        }
            //        RedisClient.HashSet("hash", hashEntries.ToArray());
            //    }
            //}
            #endregion
        }

        public object Get(string key)
        {
            throw new NotImplementedException();
        }

        public T Get<T>(string key)
        {
            throw new NotImplementedException();
        }

        public void Insert(string key, object data)
        {
            throw new NotImplementedException();
        }

        public void Insert<T>(string key, T data)
        {
            throw new NotImplementedException();
        }

        public void Insert(string key, object data, int cacheTime)
        {
            throw new NotImplementedException();
        }

        public void Insert<T>(string key, T data, int cacheTime)
        {
            throw new NotImplementedException();
        }

        public void Insert(string key, object data, DateTime cacheTime)
        {
            throw new NotImplementedException();
        }

        public void Insert<T>(string key, T data, DateTime cacheTime)
        {
            throw new NotImplementedException();
        }

        public void Remove(string key)
        {
            throw new NotImplementedException();
        }
    }
}
