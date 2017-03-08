using System.Runtime.Caching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CMS.Cache
{
    public class CacheController : ICachingReposity
    {
        #region fields
        //   private static RedisClient _instance = null;
        private static readonly object lockDB = new object();
        private static readonly CacheController Instance = new CacheController();
        private ObjectCache Cache { get { return System.Runtime.Caching.MemoryCache.Default; } }
        #endregion

        #region contructor
        public static CacheController GetInstance()
        {
            return Instance;
        }

        //public CacheController()
        //{
        //    lock (lockDB)
        //    {
        //        if (Cache == null)
        //        {
        //            try
        //            {
        //                _instance = InitRedis.GetInstanceMC();
        //            }
        //            catch (Exception ex)
        //            {
        //                ex.Message.ToString();
        //            }
        //        }
        //    }
        //}
        #endregion

        #region methods
        //public bool SetCacheByDay(string key, object value, int expiry)
        //{
        //    try
        //    {
        //        var eDateTime = DateTime.Now.AddDays(expiry);
        //        return _instance.Set(key, value, eDateTime);
        //    }
        //    catch (Exception)
        //    {
        //    }
        //    return false;
        //}

        //public bool SetCacheByHour(string key, object value, int expiry)
        //{
        //    try
        //    {
        //        var eDateTime = DateTime.Now.AddHours(expiry);
        //        return _instance.Set(key, value, eDateTime);
        //    }
        //    catch (Exception)
        //    {
        //    }
        //    return false;
        //}

        //public bool SetCacheByMinutes(string key, object value, int expiry)
        //{
        //    try
        //    {
        //        var eDateTime = DateTime.Now.AddMinutes(expiry);
        //        return _instance.Set(key, value, eDateTime);
        //    }
        //    catch (Exception)
        //    {
        //    }
        //    return false;
        //}

        //public bool SetCacheBySecond(string key, object value, int expiry)
        //{
        //    try
        //    {
        //        var eDateTime = DateTime.Now.AddSeconds(expiry);
        //        return _instance.Set(key, value, eDateTime);
        //    }
        //    catch (Exception)
        //    {
        //    }
        //    return false;
        //}

        public void Set(string key, object data, int cacheTime)
        {
            CacheItemPolicy policy = new CacheItemPolicy();
            policy.AbsoluteExpiration = DateTime.Now + TimeSpan.FromMinutes(cacheTime);
            if (data != null)
                Cache.Add(new CacheItem(key, data), policy);
        }
        public object GetCache(string key)
        {
            return Cache[key];
        }

        public void DeleteCache(string key)
        {
            try
            {
                Cache.Remove(key);
            }
            catch (Exception)
            {
            }

        }

        public bool KeyExistsCache(string key)
        {
            try
            {
                return Cache.Get(key) != null;
            }
            catch (Exception)
            {
            }
            return false;
        }

        #endregion
    }
}