using System;
namespace CMS.Common
{
    public class InMemoryCache : ICacheService
    {
        public T GetOrSet<T>(string cacheKey, Func<T> getItemCallback) where T : class
        {
            T item = System.Runtime.Caching.MemoryCache.Default.Get(cacheKey) as T;
            if (item == null)
            {
                item = getItemCallback();
                System.Runtime.Caching.MemoryCache.Default.Add(cacheKey, item, DateTime.Now.AddMinutes(5));
            }
            return item;
        }
    }

    public interface ICacheService
    {
        T GetOrSet<T>(string cacheKey, Func<T> getItemCallback) where T : class;
    }
}