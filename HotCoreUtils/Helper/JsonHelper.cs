using System;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;

namespace HotCoreUtils.Helper
{
    /// <summary>
    /// Json帮助类
    /// 作者：郭孟稳
    /// 时间：2016.07.08
    /// </summary>
    public class JsonHelper
    {
        /// <summary>
        /// JSON序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public static string JsonSerializer<T>(T t)
        {
            JavaScriptSerializer ser = new JavaScriptSerializer();

            ser.RegisterConverters(new JavaScriptConverter[] { new DataTableConverter() });
            ser.MaxJsonLength = int.MaxValue;
            string jsonString = ser.Serialize(t);
            //替换Json的Date字符串 
            string p = @"\\/Date\((\d+)\)\\/";
            jsonString = Regex.Replace(jsonString, p, match =>
             {
                 return ConvertJsonDateToDateString(match);
             });
            return jsonString;
        }

        /// <summary>
        /// JSON反序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonString"></param>
        /// <returns></returns>
        public static T JsonDeserialize<T>(string jsonString)
        {
            try
            {
                JavaScriptSerializer ser = new JavaScriptSerializer();
                ser.MaxJsonLength = int.MaxValue;
                return ser.Deserialize<T>(jsonString);
            }
            catch (Exception)
            {
                return default(T);
            }
        }

        /// <summary>
        ///  将Json序列化的时间由/Date(1294499956278+0800)转为字符串
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        private static string ConvertJsonDateToDateString(Match m)
        {
            string result = string.Empty;
            DateTime dt = new DateTime(1970, 1, 1);
            dt = dt.AddMilliseconds(long.Parse(m.Groups[1].Value));
            dt = dt.ToLocalTime();
            result = dt.ToString("yyyy-MM-dd HH:mm:ss");
            return result;
        }
    }
    /// <summary>
    /// DataTable JSON转换类
    /// </summary>
    public class DataTableConverter : JavaScriptConverter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="serializer"></param>
        /// <returns></returns>
        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            DataTable dt = obj as DataTable;

            Dictionary<string, object> result = new Dictionary<string, object>();

            List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();

            foreach (DataRow dr in dt.Rows)
            {
                Dictionary<string, object> row = new Dictionary<string, object>();
                foreach (DataColumn dc in dt.Columns)
                {
                    row.Add(dc.ColumnName, dr[dc.ColumnName]);
                }
                rows.Add(row);
            }

            result["Rows"] = rows;

            return result;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dictionary"></param>
        /// <param name="type"></param>
        /// <param name="serializer"></param>
        /// <returns></returns>
        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 获取本转换器支持的类型
        /// </summary>
        public override IEnumerable<Type> SupportedTypes
        {
            get { return new Type[] { typeof(DataTable) }; }
        }
    }
}

