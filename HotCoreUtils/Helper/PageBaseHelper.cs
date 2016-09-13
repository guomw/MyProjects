using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace HotCoreUtils.Helper
{
    /// <summary>
    /// 页面基础类
    /// </summary>
    public class PageBaseHelper : Page
    {

        #region 基本判断
        /// <summary>
        /// 是否为POST提交方式   在这里可以判断是否重复提交的  Request.Headers["Accept"] != "*/*"
        /// </summary>
        public bool IsHttpPOST { get { return Request.HttpMethod == "POST"; } }
        /// <summary>
        /// 是否为异步请求
        /// </summary>
        public bool IsAjaxRequest
        {
            get
            {
                string sheader = Request.Headers["X-Requested-With"];
                return (sheader != null && sheader == "XMLHttpRequest");
            }
        }
        #endregion

        /// <summary>
        /// JSONP 回调函数名
        /// </summary>
        public string CallbackName
        {
            get
            {
                return GetQueryString("jsonpcallback", "");
            }
        }

        /// <summary>
        /// 获取Request.QueryString中的指定键的值
        /// </summary>
        /// <param name="sQueryKey">键</param>
        /// <param name="sDefaultValue">默认值</param>
        /// <returns></returns>
        public string GetQueryString(string sQueryKey, string sDefaultValue)
        {
            if (HttpContext.Current != null && HttpContext.Current.Request.QueryString[sQueryKey] != null && !string.IsNullOrEmpty(HttpContext.Current.Request.QueryString[sQueryKey]))
            {
                return HttpContext.Current.Request.QueryString[sQueryKey];
            }
            return sDefaultValue;
        }

        /// <summary>
        /// 获取Request.QueryString中的指定键的值
        /// </summary>
        /// <param name="sQueryKey">键</param>
        /// <param name="iDefaultValue">默认值</param>
        /// <returns></returns>
        public int GetQueryString(string sQueryKey, int iDefaultValue)
        {
            int iValue = 0;
            if (Int32.TryParse(GetQueryString(sQueryKey, iDefaultValue.ToString()), out iValue))
            {
                return iValue;
            }
            return iDefaultValue;
        }
        /// <summary>
        /// 获取Request.QueryString中的指定键的值
        /// </summary>
        /// <param name="sQueryKey"></param>
        /// <param name="lDefaultValue"></param>
        /// <returns></returns>
        public long GetQueryString(string sQueryKey, long lDefaultValue)
        {
            long lValue = 0;
            if (long.TryParse(GetQueryString(sQueryKey, lDefaultValue.ToString()), out lValue))
            {
                return lValue;
            }
            return lDefaultValue;
        }

        /// <summary>
        /// 获取Request.QueryString中的指定键的值
        /// </summary>
        /// <param name="sQueryKey">键</param>
        /// <param name="dDefaultValue">默认值</param>
        /// <returns></returns>
        public double GetQueryString(string sQueryKey, double dDefaultValue)
        {
            double dValue = 0;
            if (double.TryParse(GetQueryString(sQueryKey, dDefaultValue.ToString()), out dValue))
            {
                return dValue;
            }
            return dDefaultValue;
        }
        /// <summary>
        /// 获取Request.Form中的指定键的值
        /// </summary>
        /// <param name="sFormName">键</param>
        /// <param name="sDefaultValue">默认值</param>
        /// <returns></returns>
        public string GetFormValue(string sFormName, string sDefaultValue)
        {
            if (HttpContext.Current != null && HttpContext.Current.Request.Form[sFormName] != null)
            {
                return HttpContext.Current.Request.Form[sFormName];
            }
            return sDefaultValue;
        }

        /// <summary>
        /// 获取Request.Form中的指定键的值
        /// </summary>
        /// <param name="sFormName">键</param>
        /// <param name="iDefaultValue">默认值</param>
        /// <returns></returns>
        public int GetFormValue(string sFormName, int iDefaultValue)
        {
            int iValue = 0;
            if (Int32.TryParse(GetFormValue(sFormName, iDefaultValue.ToString()), out iValue))
            {
                return iValue;
            }
            return iDefaultValue;
        }
    }
}