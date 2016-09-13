using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.Caching;
using System.Collections;
using System.IO;

namespace HotCoreUtils.Caching
{
    /// <summary>
    /// 包装了system.web.caching.cache的缓存助手类
    /// </summary>
    public class WebCacheHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public delegate object CacheLoaderDelegate();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="e"></param>
        public delegate void CacheLoaderErrorDeletegate(string key, Exception e);
        /// <summary>
        /// 
        /// </summary>
        public static CacheLoaderErrorDeletegate _cacheLoaderErrorDelegate = null;
        /// <summary>
        /// 
        /// </summary>
        private static readonly System.Web.Caching.Cache _cache = HttpRuntime.Cache;
        /// <summary>
        /// 
        /// </summary>
        private const string CACHEDEPFILE = "/resource/HotCoreUtils/cache/{0}.cache";
        /// <summary>
        /// 
        /// </summary>
        private WebCacheHelper()
        {
        }

        #region 清除缓存

        /// <summary>
        /// 清空缓存
        /// </summary>
        public static void Clear()
        {
            IDictionaryEnumerator enumerator = _cache.GetEnumerator();
            ArrayList list = new ArrayList();
            while (enumerator.MoveNext())
            {
                list.Add(enumerator.Key);
            }

            foreach (string str in list)
            {
                Remove(str);
            }
        }

        /// <summary>
        /// 移除单个缓存
        /// </summary>
        /// <param name="key"></param>
        public static void Remove(string key)
        {
            _cache.Remove(key);
        }

        /// <summary>
        /// 根据正则移除缓存
        /// </summary>
        /// <param name="pattern"></param>
        public static void RemoveByPattern(string pattern)
        {

        }
        #endregion

        #region 插入缓存
        /// <summary>
        /// 插入缓存 未使用文件依赖于过期时间
        /// </summary>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        public static void Insert(string key, object obj)
        {
            if (obj != null)
            {
                BaseInsert(key, obj, null, System.Web.Caching.Cache.NoAbsoluteExpiration, System.Web.Caching.Cache.NoSlidingExpiration, System.Web.Caching.CacheItemPriority.AboveNormal, null);
            }
        }

        /// <summary>
        /// 插入缓存 仅使用过期时间
        /// </summary>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        /// <param name="absoluteExprition"></param>
        public static void Insert(string key, object obj, TimeSpan absoluteExprition)
        {
            Insert(key, obj, absoluteExprition, null);
        }

        /// <summary>
        /// 插入缓存 仅使用文件依赖
        /// </summary>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        /// <param name="dep"></param>
        public static void Insert(string key, object obj, CacheDependency dep)
        {
            Insert(key, obj, new TimeSpan(100, 0, 0, 0), dep);
        }
        /// <summary>
        /// 插入缓存 仅使用文件依赖
        /// </summary>
        /// <param name="key">缓存key</param>
        /// <param name="obj">缓存对象</param>
        /// <param name="depFileName">依赖文件名称或key</param>
        /// <param name="timeType">WebCacheTimeOption缓存时间</param>
        public static void Insert(string key, object obj, string depFileName,WebCacheTimeOption timeType=WebCacheTimeOption.永久)
        {
            Insert(key, obj, new TimeSpan(100, 0, 0, 0), new CacheDependency(GetDepFile(depFileName, timeType)));
        }


        /// <summary>
        /// 插入缓存 使用文件依赖与过期时间
        /// </summary>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        /// <param name="absoluteExprition"></param>
        /// <param name="dep"></param>
        public static void Insert(string key, object obj, TimeSpan absoluteExprition, CacheDependency dep)
        {
            Insert(key, obj, absoluteExprition, dep, System.Web.Caching.CacheItemPriority.Normal, null);
        }

        /// <summary>
        /// 插入缓存 使用文件依赖与过期时间，另可设置缓存优先级与缓存失效回调函数
        /// </summary>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        /// <param name="absoluteExprition"></param>
        /// <param name="dep"></param>
        /// <param name="priority"></param>
        /// <param name="onRemoveCallback"></param>
        public static void Insert(string key, object obj, TimeSpan absoluteExprition, CacheDependency dep, System.Web.Caching.CacheItemPriority priority, CacheItemRemovedCallback onRemoveCallback)
        {
            if (obj != null)
            {
                BaseInsert(key, obj, dep, DateTime.Now.Add(absoluteExprition), System.Web.Caching.Cache.NoSlidingExpiration, priority, onRemoveCallback);
            }
        }

        private static void BaseInsert(string key, object value, CacheDependency dep, DateTime absoluteExpiration, TimeSpan slidingExpiration, System.Web.Caching.CacheItemPriority priority, CacheItemRemovedCallback onRemoveCallback)
        {
            if (value != null)
            {
                _cache.Insert(key, value, dep, absoluteExpiration, slidingExpiration, priority, onRemoveCallback);
            }
        }

        #endregion

        #region 获取缓存
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static object Get(string key)
        {
            return _cache[key];
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="absoluteExprition"></param>
        /// <param name="cacheLoader"></param>
        /// <returns></returns>
        public static object Get(string key, TimeSpan absoluteExprition, CacheLoaderDelegate cacheLoader)
        {
            object obj = _cache[key];
            if (obj == null)
            {
                if (cacheLoader != null)
                {
                    try
                    {
                        obj = cacheLoader();
                        Insert(key, obj, absoluteExprition);
                    }
                    catch (Exception ex)
                    {
                        if (_cacheLoaderErrorDelegate != null)
                        {
                            _cacheLoaderErrorDelegate(key, ex);
                        }
                    }
                }
            }
            return obj;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dep"></param>
        /// <param name="cacheLoader"></param>
        /// <returns></returns>
        public static object Get(string key, CacheDependency dep, CacheLoaderDelegate cacheLoader)
        {
            object obj = _cache[key];
            if (obj == null)
            {
                if (cacheLoader != null)
                {
                    try
                    {
                        obj = cacheLoader();
                        Insert(key, obj, dep);
                    }
                    catch (Exception ex)
                    {
                        if (_cacheLoaderErrorDelegate != null)
                        {
                            _cacheLoaderErrorDelegate(key, ex);
                        }
                    }
                }
            }
            return obj;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="absoluteExprition"></param>
        /// <param name="dep"></param>
        /// <param name="cacheLoader"></param>
        /// <returns></returns>
        public static object Get(string key, TimeSpan absoluteExprition, CacheDependency dep, WebCacheHelper.CacheLoaderDelegate cacheLoader)
        {
            object obj = _cache[key];
            if (obj == null)
            {
                if (cacheLoader != null)
                {
                    try
                    {
                        obj = cacheLoader();
                        Insert(key, obj, absoluteExprition, dep);
                    }
                    catch (Exception ex)
                    {
                        if (_cacheLoaderErrorDelegate != null)
                        {
                            _cacheLoaderErrorDelegate(key, ex);
                        }
                    }
                }
            }
            return obj;
        }

        /// <summary>
        /// 得到所有缓存
        /// </summary>
        public static System.Web.Caching.Cache CacheList
        {
            get
            {
                return _cache;
            }
        }
        #endregion

        #region 检查是否存在
        /// <summary>
        /// 检查是否存在
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool Contains(string key)
        {
            IDictionaryEnumerator enumerator = _cache.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (enumerator.Key.ToString().Equals(key, StringComparison.CurrentCultureIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }
        #endregion
        /// <summary>
        /// 
        /// </summary>
        /// <param name="handler"></param>
        public static void SetCacheLoaderErrorHandler(CacheLoaderErrorDeletegate handler)
        {
            _cacheLoaderErrorDelegate = handler;
        }


        /// <summary>
        /// 得到缓存依赖KEY
        /// </summary>
        /// <param name="token">唯一值</param>
        /// <param name="timeType">WebCacheTimeOption 缓存时间类型</param>
        /// <returns></returns>
        public static string GetDepFile(string token, WebCacheTimeOption timeType = WebCacheTimeOption.永久)
        {
            string depFile = GetDepFilePath(token, timeType);
            if (!System.IO.File.Exists(depFile))
            {
                System.IO.File.Create(depFile).Dispose();
            }
            return depFile;
        }
        /// <summary>
        /// 刷新缓存文件
        /// </summary>
        /// <param name="token"></param>
        /// <param name="timeType">缓存时间类型0=每小时，1=每天，2=每月,3=永久,默认1</param>
        public static void RefreshCache(string token, WebCacheTimeOption timeType = WebCacheTimeOption.永久)
        {
            string depFile = GetDepFilePath(token, timeType);
            if (System.IO.File.Exists(depFile))
                File.WriteAllText(depFile, DateTime.Now.ToString());
        }
        /// <summary>
        /// 删除缓存文件
        /// </summary>
        /// <param name="token"></param>
        /// <param name="timeType">缓存时间类型0=每小时，1=每天，2=每月,3=永久,默认1</param>
        /// <returns></returns>
        public static bool DeleteDepFile(string token, WebCacheTimeOption timeType= WebCacheTimeOption.永久)
        {
            try
            {
                string depFile = GetDepFilePath(token, timeType);
                if (System.IO.File.Exists(depFile))
                {
                    System.IO.File.Delete(depFile);
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static string GetDepFilePath(string token, WebCacheTimeOption timeType)
        {
            string depKey = "";
            switch (timeType)
            {
                case WebCacheTimeOption.每小时:
                    depKey = token + DateTime.Now.ToString("yyyyMMddHH");
                    break;
                case WebCacheTimeOption.每天:
                    depKey = token + DateTime.Now.ToString("yyyyMMdd");
                    break;
                case WebCacheTimeOption.每月:
                    depKey = token + DateTime.Now.ToString("yyyyMM");
                    break;
                case WebCacheTimeOption.永久:
                    depKey = token;
                    break;
                default:
                    depKey = token + DateTime.Now.ToString("yyyyMMdd");
                    break;
            }
            string depFile = HttpContext.Current.Server.MapPath(string.Format(CACHEDEPFILE, depKey));
            if (!System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(depFile)))
            {
                System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(depFile));
            }
            return depFile;
        }
    }


    /// <summary>
    /// 缓存时间
    /// </summary>
    public enum WebCacheTimeOption
    {        
        /// <summary>
        /// 每小时
        /// </summary>
        每小时 = 0,
        /// <summary>
        /// 每天
        /// </summary>
        每天 = 1,
        /// <summary>
        /// 每月
        /// </summary>
        每月 = 2,
        /// <summary>
        /// 永久
        /// </summary>
        永久 = 3
    }



    /// <summary>
    /// 缓存助手，内部已进行类型转换
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class WebCacheHelper<T> where T : class
    {
        /// <summary>
        /// 获取缓存值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T Get(string key)
        {
            T obj = WebCacheHelper.Get(key) as T;
            return obj;
        }

        /// <summary>
        /// 获取缓存值，如果不存在，则调用cacheLoader，absoluteExprition时间段后失效
        /// </summary>
        /// <param name="key"></param>
        /// <param name="absoluteExprition"></param>
        /// <param name="cacheLoader"></param>
        /// <returns></returns>
        public static T Get(string key, TimeSpan absoluteExprition, WebCacheHelper.CacheLoaderDelegate cacheLoader)
        {
            return WebCacheHelper.Get(key, absoluteExprition, cacheLoader) as T;
        }

        /// <summary>
        /// 获取缓存值，如果不存在，则调用cacheLoader，使用文件依赖策略
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dep"></param>
        /// <param name="cacheLoader"></param>
        /// <returns></returns>
        public static T Get(string key, CacheDependency dep, WebCacheHelper.CacheLoaderDelegate cacheLoader)
        {
            return WebCacheHelper.Get(key, dep, cacheLoader) as T;
        }

        /// <summary>
        /// 获取缓存值，如果不存在，则调用cacheLoader，使用时间段和文件依赖两个策略
        /// </summary>
        /// <param name="key"></param>
        /// <param name="absoluteExprition"></param>
        /// <param name="dep"></param>
        /// <param name="cacheLoader"></param>
        /// <returns></returns>
        public static T Get(string key, TimeSpan absoluteExprition, CacheDependency dep, WebCacheHelper.CacheLoaderDelegate cacheLoader)
        {
            return WebCacheHelper.Get(key, absoluteExprition, dep, cacheLoader) as T;
        }
    }
}
