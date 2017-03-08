using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CMS.Cache
{
    interface ICachingReposity
    {
        //bool SetCacheByDay(string key, object value, int expiry);
        //bool SetCacheByHour(string key, object value, int expiry);
        //bool SetCacheByMinutes(string key, object value, int expiry);
        //bool SetCacheBySecond(string key, object value, int expiry);
        void DeleteCache(string key);
        bool KeyExistsCache(string key);
        object GetCache(string key);
        void Set(string key, object data, int cacheTime);
    }
}