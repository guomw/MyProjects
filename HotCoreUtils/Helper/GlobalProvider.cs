using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Text.RegularExpressions;

namespace HotCoreUtils.Helper
{
    /// <summary>
    /// 全局扩展属性类
    /// </summary>
    public static class GlobalProvider
    {
        #region 判断是否为空

        #region 判断字符串是否为空,返回true为空，否则不为空
        /// <summary>
        /// 判断字符串是否为空,返回true为空，否则不为空
        /// </summary>
        /// <param name="boolValue">字符串值</param>
        /// <returns></returns>
        public static bool StrIsNull(this string boolValue)
        {
            if (boolValue != null && boolValue != "" && boolValue.ToLower() != "null" && !string.IsNullOrEmpty(boolValue) && boolValue.ToString().Trim().Length != 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        #endregion

        #region 判断字符串是否为空,返回true为空，否则不为空
        /// <summary>
        /// 判断字符串是否为空,为空返回空字符 否则源数据返回
        /// </summary>
        /// <param name="boolValue">字符串值</param>
        /// <returns></returns>
        public static string StrToString(this object boolValue)
        {
            if (boolValue != null && boolValue.ToString() != "" && boolValue.ToString().ToLower() != "null" && boolValue.ToString() != "undefined" && !string.IsNullOrEmpty(boolValue.ToString()) && boolValue.ToString().Trim().Length != 0)
            {
                return boolValue.ToString();
            }
            else
            {
                return "";
            }
        }
        #endregion


        #region 判断数据集(DataSet)是否为空
        /// <summary>
        /// 判断数据集(DataSet)是否为空
        /// </summary>
        /// <param name="ds">数据集</param>
        /// <returns></returns>
        public static bool DsIsNull(this DataSet ds)
        {
            if (ds == null || ds.Tables.Count == 0 || ds.Tables[0] == null || ds.Tables[0].Columns.Count == 0 || ds.Tables[0].Rows.Count == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region 判断DataTable表是否为空
        /// <summary>
        /// 判断DataTable表是否为空
        /// </summary>
        /// <param name="dt">数据表</param>
        /// <returns></returns>
        public static bool DtIsNull(this DataTable dt)
        {
            if (dt == null || dt.Columns.Count == 0 || dt.Rows.Count == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region 判断DataRow是否为空
        /// <summary>
        /// 判断DataRow是否为空
        /// </summary>
        /// <param name="dr">Rows行数组</param>
        /// <returns></returns>
        public static bool DRIsNull(this DataRow[] dr)
        {
            if (dr == null || dr.Length == 0 || dr[0].ItemArray.Length == 0 || dr[0].Table.Columns.Count == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region 判断List对象是否为空
        /// <summary>
        /// 判断List对象是否为空
        /// </summary>
        /// <param name="list">List对象</param>
        /// <returns></returns>
        public static bool ListIsNull<T>(this List<T> list)
        {
            if (list == null || list.Count == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region 判断IList对象是否为空
        /// <summary>
        /// 判断IList对象是否为空
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="list">集合类型</param>
        /// <returns></returns>
        public static bool IListIsNull<T>(this IList<T> list)
        {
            if (list == null || list.Count == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region 判断object[]对象是否为空
        /// <summary>
        /// 判断object[]对象是否为空
        /// </summary>
        /// <param name="obj">对象数组</param>
        /// <returns></returns>
        public static bool ObjIsNull(this object[] obj)
        {
            if (obj == null || obj.Length == 0)
                return true;
            else
                return false;
        }
        #endregion

        #endregion


        /// <summary>
        /// 判断版本是否有更新
        /// </summary>
        /// <param name="version">最新版本号</param>
        /// <param name="currentVersion">当前版本号</param>
        /// <returns></returns>
        public static bool IsVersionUpdate(this string version, string currentVersion)
        {
            try
            {
                string _newVersion, _currentVersion;
                //处理当前版本号
                int firstIdx = currentVersion.IndexOf('.');
                if (firstIdx >= 0)
                    _currentVersion = currentVersion.Replace(".", "").Insert(firstIdx, ".");
                else
                    _currentVersion = currentVersion;

                //处理新版本号
                int _firstIdx = version.IndexOf('.');
                if (_firstIdx >= 0)
                    _newVersion = version.Replace(".", "").Insert(_firstIdx, ".");
                else
                    _newVersion = version;

                return Convert.ToDecimal(_newVersion) > Convert.ToDecimal(_currentVersion);
            }
            catch (Exception)
            {
                return false;
            }
        }



        /// <summary>
        /// 对象是否非空
        /// 为空返回 false
        /// 不为空返回 true
        /// </summary>
        /// <param name="Object">要判断的对象</param>
        /// <returns>bool值</returns>
        public static bool NotNull(object Object) { return !IsNull(Object, false); }
        /// <summary>
        /// 对象是否非空
        /// 为空返回 false
        /// 不为空返回 true
        /// </summary>
        /// <param name="Object">要判断的对象</param>
        /// <param name="IsRemoveSpace">是否去除空格</param>
        /// <returns>bool值</returns>
        public static bool NotNull(this object Object, bool IsRemoveSpace) { return !IsNull(Object, IsRemoveSpace); }
        /// <summary>
        /// 对象是否为空
        /// 为空返回 false
        /// 不为空返回 true
        /// </summary>
        /// <param name="Object">要判断的对象</param>
        /// <returns>bool值</returns>
        public static bool IsNull(this object Object) { return IsNull(Object, false); }
        /// <summary>
        /// 对象是否为空
        /// 为空返回 false
        /// 不为空返回 true
        /// </summary>
        /// <param name="Object">要判断的对象</param>
        /// <param name="IsRemoveSpace">是否去除空格</param>
        /// <returns>bool值</returns>
        public static bool IsNull(this object Object, bool IsRemoveSpace)
        {
            if (Object == null) return true;
            string Objects = Object.ToString();
            if (Objects == "") return true;
            if (IsRemoveSpace)
            {
                if (Objects.Replace(" ", "") == "") return true;
                if (Objects.Replace("　", "") == "") return true;
            }
            return false;
        }
        /// <summary>
        /// 对象是否为bool值
        /// </summary>
        /// <param name="Object">要判断的对象</param>
        /// <returns>bool值</returns>
        public static bool IsBool(this object Object) { return IsBool(Object, false); }
        /// <summary>
        /// 判断是否为bool值
        /// </summary>
        /// <param name="Object">要判断的对象</param>
        /// <param name="Default">默认bool值</param>
        /// <returns>bool值</returns>
        public static bool IsBool(this object Object, bool Default)
        {
            if (IsNull(Object)) return Default;
            try { return bool.Parse(Object.ToString()); }
            catch { return Default; }
        }

        /// <summary>
        /// 是否URL地址
        /// </summary>
        /// <param name="HttpUrl">等待验证的Url地址</param>
        /// <returns>bool</returns>
        public static bool IsHttp(this string HttpUrl) { return Regex.IsMatch(HttpUrl, @"^(http|https):\/\/[A-Za-z0-9%\-_@]+\.[A-Za-z0-9%\-_@]{2,}[A-Za-z0-9\.\/=\?%\-&_~`@[\]:+!;]*$"); }
        /// <summary>
        /// 对象是否为int值
        /// </summary>
        /// <param name="Object"></param>
        /// <returns></returns>
        public static bool IsInt(this object Object)
        {
            try { int.Parse(Object.ToString()); return true; }
            catch { return false; }
        }
        /// <summary>
        /// 对象是否为long值
        /// </summary>
        /// <param name="Object"></param>
        /// <returns></returns>
        public static bool IsLong(this object Object)
        {
            try { long.Parse(Object.ToString()); return true; }
            catch { return false; }
        }
        /// <summary>
        /// 转换为BOOL
        /// </summary>
        /// <param name="Object"></param>
        /// <param name="Default"></param>
        /// <returns></returns>
        public static bool ToBoolen(this object Object, bool Default)
        {
            bool isTrue = false;
            try
            {
                isTrue = bool.Parse(Object.ToString());
            }
            catch
            {
                isTrue = Default;
            }
            return isTrue;
        }
        /// <summary>
        /// 对象转BOOL类型
        /// </summary>
        /// <param name="Object"></param>
        /// <returns></returns>
        public static bool ToBoolen(this object Object)
        {
            return ToBoolen(Object, false);
        }

        /// <summary>
        /// 判断是否为时间格式
        /// </summary>
        /// <param name="Object">要判断的对象</param>
        /// <returns>返回是否转换成功</returns>
        public static bool IsTime(this object Object)
        {
            try
            {
                bool val = false;
                try
                {
                    if (IsNull(Object)) return false;
                    DateTime.Parse(Object.ToString());
                    val = true;
                }
                catch
                {
                    return false;
                }
                return val;
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// 是否为decimal值
        /// </summary>
        /// <param name="Object"></param>
        /// <returns></returns>
        public static bool IsDecimal(this object Object)
        {
            try { decimal.Parse(Object.ToString()); return true; }
            catch { return false; }
        }

        /// <summary>
        /// 是否为float值
        /// </summary>
        /// <param name="Object"></param>
        /// <returns></returns>
        public static bool IsFloat(this object Object)
        {
            try { float.Parse(Object.ToString()); return true; }
            catch { return false; }
        }

    }
}