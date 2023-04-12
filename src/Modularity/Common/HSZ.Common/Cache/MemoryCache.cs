using HSZ.Dependency;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace HSZ.Common.Cache
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：内存缓存
    /// </summary>
    public class MemoryCache : ICache, ISingleton
    {
        private readonly IMemoryCache _memoryCache;

        public MemoryCache(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public long Del(params string[] key)
        {
            foreach (var k in key)
            {
                _memoryCache.Remove(k);
            }
            return key.Length;
        }

        public Task<long> DelAsync(params string[] key)
        {
            foreach (var k in key)
            {
                _memoryCache.Remove(k);
            }

            return Task.FromResult((long)key.Length);
        }

        public async Task<long> DelByPatternAsync(string pattern)
        {
            if (string.IsNullOrEmpty(pattern))
                return default;

            //pattern = Regex.Replace(pattern, @"\{*.\}", "(.*)");
            var keys = GetAllKeys().Where(k => k.StartsWith(pattern));
            if (keys != null && keys.Any())
                return await DelAsync(keys.ToArray());

            return default;
        }

        public bool Exists(string key)
        {
            return _memoryCache.TryGetValue(key, out _);
        }

        public Task<bool> ExistsAsync(string key)
        {
            return Task.FromResult(_memoryCache.TryGetValue(key, out _));
        }

        public string Get(string key)
        {

            return _memoryCache.GetOrCreate(key,entry=> { return entry.Value; })?.ToString();
        }

        public T Get<T>(string key)
        {
            var entry = _memoryCache.Get<ICacheEntry>(key);
            return entry == null ? default(T) : (T)(entry.Value);
        }

        public Task<string> GetAsync(string key)
        {
            return Task.FromResult(Get(key));
        }

        public Task<T> GetAsync<T>(string key)
        {
            return Task.FromResult(Get<T>(key));
        }

        public bool Set(string key, object value)
        {
            var entry = _memoryCache.CreateEntry(key);
            entry.Value = value;
            _memoryCache.Set(key, entry);
            return true;
        }

        public bool Set(string key, object value, TimeSpan expire)
        {
            var entry = _memoryCache.CreateEntry(key);
            entry.Value = value;
            _memoryCache.Set(key, entry, expire);
            return true;
        }

        public Task<bool> SetAsync(string key, object value)
        {
            var entry = _memoryCache.CreateEntry(key);
            entry.Value = value;
            Set(key, entry);
            return Task.FromResult(true);
        }

        public Task<bool> SetAsync(string key, object value, TimeSpan expire)
        {
            var entry = _memoryCache.CreateEntry(key);
            entry.Value = value;
            Set(key, entry, expire);
            return Task.FromResult(true);
        }

        public List<string> GetAllKeys()
        {
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
            var entries = _memoryCache.GetType().GetField("_entries", flags).GetValue(_memoryCache);
            var cacheItems = entries.GetType().GetProperty("Keys").GetValue(entries) as ICollection<object>; //entries as IDictionary;
            var keys = new List<string>();
            if (cacheItems == null) return keys;
            return cacheItems.Select(u => u.ToString()).ToList();
            //foreach (DictionaryEntry cacheItem in cacheItems)
            //{
            //    keys.Add(cacheItem.Key.ToString());
            //}
            //return keys;
        }

        /// <summary>
        /// 获取缓存过期时间
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public DateTime GetCacheOutTime(string key)
        {
            var entry= _memoryCache.GetOrCreate(key, entry =>
            {
                return entry;
            });
            if (entry.AbsoluteExpiration==null)
            {
                return DateTime.Now;
            }
            DateTimeOffset dateTimeOffset = (DateTimeOffset)entry.AbsoluteExpiration;
            return dateTimeOffset.UtcDateTime;
        }
    }
}
