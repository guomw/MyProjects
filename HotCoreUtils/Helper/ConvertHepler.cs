/*
 * 版权所有:杭州火图科技有限公司
 * 地址:浙江省杭州市滨江区西兴街道阡陌路智慧E谷B幢4楼在地图中查看
 * (c) Copyright Hangzhou Hot Technology Co., Ltd.
 * Floor 4,Block B,Wisdom E Valley,Qianmo Road,Binjiang District
 * 2013-2017. All rights reserved.
 * author guomw
**/


using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;

namespace HotCoreUtils.Helper
{
    /// <summary>
    /// 对象转换助手
    /// </summary>
    public class ConvertHepler
    {
        #region XML转换DataTable
        /// <summary>
        /// 获取xml 数据转换换DataTable
        /// </summary>
        /// <returns></returns>
        public static DataTable ConvertXmlToDataTable(string xmlpath)
        {
            DataTable dt = new DataTable();
            string filePath = HttpContext.Current.Server.MapPath(xmlpath);
            if (File.Exists(filePath))
            {
                DataSet ds = new DataSet();
                ds.ReadXml(filePath);
                dt = ds.Tables[0];
                ds.Dispose();

            }

            return dt;
        }
        #endregion



        #region IList 转 DataTable
        /// <summary>
        /// 将List数据转换成DataTable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="_list">数据源</param>
        /// <param name="customField">自定义字段,true:获取对象属性Description内容为字段名称,如果为true，T对象中的属性必须都有description描述</param>
        /// <returns>DataTable.</returns>
        public static DataTable ConvertListToDataTable<T>(List<T> _list, bool customField = false)
        {
            DataTable dt = new DataTable();
            //转换成table            
            if (_list != null)
            {
                //通过反射获取list中的字段 
                PropertyInfo[] p = _list[0].GetType().GetProperties();
                int c = customField ? 0 : p.Length;
                foreach (PropertyInfo pi in p)
                {
                    if (customField)
                    {
                        string des = ((DescriptionAttribute)Attribute.GetCustomAttribute(pi, typeof(DescriptionAttribute))).Description;// 属性值
                        if (!string.IsNullOrEmpty(des))
                        {
                            dt.Columns.Add(des, Type.GetType(pi.PropertyType.ToString()));
                            c++;
                        }
                    }
                    else
                        dt.Columns.Add(pi.Name, Type.GetType(pi.PropertyType.ToString()));
                }
                for (int i = 0; i < _list.Count; i++)
                {
                    IList TempList = new ArrayList();
                    //将IList中的一条记录写入ArrayList
                    foreach (PropertyInfo pi in p)
                    {
                        if (customField)
                        {
                            string des = ((DescriptionAttribute)Attribute.GetCustomAttribute(pi, typeof(DescriptionAttribute))).Description;// 属性值
                            if (!string.IsNullOrEmpty(des))
                            {
                                object oo = pi.GetValue(_list[i], null);
                                TempList.Add(oo);
                            }
                        }
                        else
                        {
                            object oo = pi.GetValue(_list[i], null);
                            TempList.Add(oo);
                        }
                    }
                    object[] itm = new object[c];
                    for (int j = 0; j < TempList.Count; j++)
                    {
                        itm.SetValue(TempList[j], j);
                    }
                    dt.LoadDataRow(itm, true);
                }
            }
            return dt;
        }
        #endregion

    }
}
