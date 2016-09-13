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
    /// ��װ��system.web.caching.cache�Ļ���������
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

        #region �������

        /// <summary>
        /// ��ջ���
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
        /// �Ƴ���������
        /// </summary>
        /// <param name="key"></param>
        public static void Remove(string key)
        {
            _cache.Remove(key);
        }

        /// <summary>
        /// ���������Ƴ�����
        /// </summary>
        /// <param name="pattern"></param>
        public static void RemoveByPattern(string pattern)
        {

        }
        #endregion

        #region ���뻺��
        /// <summary>
        /// ���뻺�� δʹ���ļ������ڹ���ʱ��
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
        /// ���뻺�� ��ʹ�ù���ʱ��
        /// </summary>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        /// <param name="absoluteExprition"></param>
        public static void Insert(string key, object obj, TimeSpan absoluteExprition)
        {
            Insert(key, obj, absoluteExprition, null);
        }

        /// <summary>
        /// ���뻺�� ��ʹ���ļ�����
        /// </summary>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        /// <param name="dep"></param>
        public static void Insert(string key, object obj, CacheDependency dep)
        {
            Insert(key, obj, new TimeSpan(100, 0, 0, 0), dep);
        }
        /// <summary>
        /// ���뻺�� ��ʹ���ļ�����
        /// </summary>
        /// <param name="key">����key</param>
        /// <param name="obj">�������</param>
        /// <param name="depFileName">�����ļ����ƻ�key</param>
        /// <param name="timeType">WebCacheTimeOption����ʱ��</param>
        public static void Insert(string key, object obj, string depFileName,WebCacheTimeOption timeType=WebCacheTimeOption.����)
        {
            Insert(key, obj, new TimeSpan(100, 0, 0, 0), new CacheDependency(GetDepFile(depFileName, timeType)));
        }


        /// <summary>
        /// ���뻺�� ʹ���ļ����������ʱ��
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
        /// ���뻺�� ʹ���ļ����������ʱ�䣬������û������ȼ��뻺��ʧЧ�ص�����
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

        #region ��ȡ����
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
        /// �õ����л���
        /// </summary>
        public static System.Web.Caching.Cache CacheList
        {
            get
            {
                return _cache;
            }
        }
        #endregion

        #region ����Ƿ����
        /// <summary>
        /// ����Ƿ����
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
        /// �õ���������KEY
        /// </summary>
        /// <param name="token">Ψһֵ</param>
        /// <param name="timeType">WebCacheTimeOption ����ʱ������</param>
        /// <returns></returns>
        public static string GetDepFile(string token, WebCacheTimeOption timeType = WebCacheTimeOption.����)
        {
            string depFile = GetDepFilePath(token, timeType);
            if (!System.IO.File.Exists(depFile))
            {
                System.IO.File.Create(depFile).Dispose();
            }
            return depFile;
        }
        /// <summary>
        /// ˢ�»����ļ�
        /// </summary>
        /// <param name="token"></param>
        /// <param name="timeType">����ʱ������0=ÿСʱ��1=ÿ�죬2=ÿ��,3=����,Ĭ��1</param>
        public static void RefreshCache(string token, WebCacheTimeOption timeType = WebCacheTimeOption.����)
        {
            string depFile = GetDepFilePath(token, timeType);
            if (System.IO.File.Exists(depFile))
                File.WriteAllText(depFile, DateTime.Now.ToString());
        }
        /// <summary>
        /// ɾ�������ļ�
        /// </summary>
        /// <param name="token"></param>
        /// <param name="timeType">����ʱ������0=ÿСʱ��1=ÿ�죬2=ÿ��,3=����,Ĭ��1</param>
        /// <returns></returns>
        public static bool DeleteDepFile(string token, WebCacheTimeOption timeType= WebCacheTimeOption.����)
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
                case WebCacheTimeOption.ÿСʱ:
                    depKey = token + DateTime.Now.ToString("yyyyMMddHH");
                    break;
                case WebCacheTimeOption.ÿ��:
                    depKey = token + DateTime.Now.ToString("yyyyMMdd");
                    break;
                case WebCacheTimeOption.ÿ��:
                    depKey = token + DateTime.Now.ToString("yyyyMM");
                    break;
                case WebCacheTimeOption.����:
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
    /// ����ʱ��
    /// </summary>
    public enum WebCacheTimeOption
    {        
        /// <summary>
        /// ÿСʱ
        /// </summary>
        ÿСʱ = 0,
        /// <summary>
        /// ÿ��
        /// </summary>
        ÿ�� = 1,
        /// <summary>
        /// ÿ��
        /// </summary>
        ÿ�� = 2,
        /// <summary>
        /// ����
        /// </summary>
        ���� = 3
    }



    /// <summary>
    /// �������֣��ڲ��ѽ�������ת��
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class WebCacheHelper<T> where T : class
    {
        /// <summary>
        /// ��ȡ����ֵ
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T Get(string key)
        {
            T obj = WebCacheHelper.Get(key) as T;
            return obj;
        }

        /// <summary>
        /// ��ȡ����ֵ����������ڣ������cacheLoader��absoluteExpritionʱ��κ�ʧЧ
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
        /// ��ȡ����ֵ����������ڣ������cacheLoader��ʹ���ļ���������
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
        /// ��ȡ����ֵ����������ڣ������cacheLoader��ʹ��ʱ��κ��ļ�������������
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
