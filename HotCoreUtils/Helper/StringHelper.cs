using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

using System.Text.RegularExpressions;
using System.Web;

namespace HotCoreUtils.Helper
{
    /// <summary>
    /// 字符串帮助类
    /// </summary>
    public class StringHelper
    {
        private StringHelper() { }

        #region 去除HTML
        /// <summary>
        /// 去除HTML
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static string GetStringNoneHtml(object o)
        {
            string Htmlstring = Convert.ToString(o);
            //删除脚本   
            Htmlstring = Regex.Replace(Htmlstring, @"<script[^>]*?>.*?</script>", "", RegexOptions.IgnoreCase);
            //删除HTML
            Htmlstring = Regex.Replace(Htmlstring, @"(<[^>]+?>)|(&nbsp;)|\s", "", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"([\r\n])[\s]+", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"-->", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"<!--.*", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(quot|#34);", "\"", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(amp|#38);", "&", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(lt|#60);", "<", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(gt|#62);", ">", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(nbsp|#160);", " ", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(iexcl|#161);", "\xa1", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(cent|#162);", "\xa2", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(pound|#163);", "\xa3", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(copy|#169);", "\xa9", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&#(\d+);", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"<img[^>]*>;", "", RegexOptions.IgnoreCase);
            Htmlstring.Replace("<", "");
            Htmlstring.Replace(">", "");
            Htmlstring.Replace("\r\n", "");
            Htmlstring.Replace("&ldquo;", "“");
            Htmlstring.Replace("&rdquo;", "”");
            Htmlstring = HttpContext.Current.Server.HtmlEncode(Htmlstring).Trim();
            return Htmlstring;
        }
        #endregion

        #region 切割字符串
        /// <summary>
        /// 切割字符串【去除HTML】
        /// </summary>
        /// <param name="o"></param>
        /// <param name="length">字节长度</param>
        /// <param name="endStr">结束字符</param>
        /// <returns></returns>
        public static string GetSubStringNoneHtml(object o, int length, string endStr)
        {
            return GetSubString(GetStringNoneHtml(o), length, endStr);
        }


        /// <summary>
        /// 切割字符串
        /// </summary>
        /// <param name="o"></param>
        /// <param name="length">字节长度</param>
        /// <param name="endStr">结束字符</param>
        /// <returns></returns>
        public static string GetSubString(object o, int length, string endStr)
        {
            string sStr = "";
            if (o != null)
                sStr = o.ToString();
            string resultString = string.Empty;
            byte[] myByte = System.Text.Encoding.GetEncoding("gbk").GetBytes(sStr);
            length = length - endStr.Length;
            if (myByte.Length > length)
            {
                resultString = Encoding.GetEncoding("gbk").GetString(myByte, 0, length);
                string lastChar = resultString.Substring(resultString.Length - 1, 1);
                if (lastChar.Equals(sStr.Substring(resultString.Length - 1, 1)))
                {//如果截取后最后一个字符与原始输入字符串中同一位置的字符相等，则表示截取完成
                    sStr = resultString;
                }
                else
                {//如果不相等，则减去一个字节再截取
                    sStr = Encoding.GetEncoding("gbk").GetString(myByte, 0, length - 1);
                }
                return sStr + endStr;
            }
            return sStr;
        }
        #endregion

        #region 过滤特殊字符 用于sql
        /// <summary>
        /// 过滤特殊字符 用于sql
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static string GetSafeString(object o)
        {
            Regex reg = new Regex(@"^(\w|-)+$");
            string tem = o.ToString();
            if (!reg.IsMatch(tem))
            {
                tem = "";
            }
            return tem;
        }
        #endregion


        #region 读取文件
        /// <summary>
        /// 读取文件
        /// </summary>
        /// <param name="strFilePath">文件绝对路径</param>
        /// <returns></returns>
        public static StringBuilder ReadFileByGb2312(string strFilePath)
        {
            using (StreamReader m_streamReader = new StreamReader(strFilePath, System.Text.Encoding.GetEncoding("gb2312")))
            {
                StringBuilder m_strToEnd = new StringBuilder();
                m_strToEnd.Append(m_streamReader.ReadToEnd());
                m_streamReader.Close();
                return m_strToEnd;
            }
        }

        /// <summary>
        /// 从字符串中读取某个标签下的内容，<!--tagname-->内容<!--/tagname-->
        /// </summary>
        /// <param name="strSign"></param>
        /// <param name="strContent"></param>
        /// <returns></returns>
        public static string ReadSign(string strSign, string strContent)
        {
            if (strSign == null || strSign.Length == 0) return strContent;

            string strPattern = @"<!--" + strSign + "-->.*<!--/" + strSign + "-->";
            string strTemp = "";
            MatchCollection mc;
            Regex r = new Regex(strPattern, RegexOptions.Singleline | RegexOptions.Compiled | RegexOptions.IgnoreCase);
            mc = r.Matches(strContent);
            for (int i = 0; i < mc.Count; i++)
            {
                strTemp += mc[i].Value;
            }
            return strTemp;
        }

        
        #endregion

        #region 获得客户的IP
        /// <summary>
        /// 获得客户的IP
        /// </summary>
        /// <returns></returns>
        public static string GetClientIP()
        {

            string result = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (result != null && result != String.Empty)
            {
                //可能有代理  
                if (result.IndexOf(".") == -1) //非IPv4格式  
                    result = string.Empty;
                else
                {
                    if (result.IndexOf(",") != -1)
                    {
                        //有“,”，估计多个代理。取第一个不是内网的IP。  
                        result = result.Replace(" ", "").Replace("'", "");
                        string[] temparyip = result.Split(",;".ToCharArray());
                        for (int i = 0; i < temparyip.Length; i++)
                        {
                            if (IsIPAddress(temparyip[i])
                            && temparyip[i].Substring(0, 3) != "10."
                            && temparyip[i].Substring(0, 7) != "192.168"
                            && temparyip[i].Substring(0, 7) != "172.16.")
                            {
                                return temparyip[i]; //找到不是内网的地址  
                            }
                        }
                    }
                    else if (IsIPAddress(result)) //代理即是IP格式  
                        return result;
                    else
                        result = string.Empty; //代理中的内容 非IP，取IP  
                }

            }

            if (null == result || result == String.Empty)
                result = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            if (result == null || result == String.Empty)
                result = HttpContext.Current.Request.UserHostAddress;
            return result;
        }
        private static bool IsIPAddress(string str1)
        {
            if (str1 == null || str1 == string.Empty || str1.Length < 7 || str1.Length > 15) return false;
            string regformat = @"^\d{1,3}[\.]\d{1,3}[\.]\d{1,3}[\.]\d{1,3}$";
            Regex regex = new Regex(regformat, RegexOptions.IgnoreCase);
            return regex.IsMatch(str1);
        }
        #endregion

        #region 获得两个日期的间隔
        /// <summary>
        /// 获得两个日期的间隔
        /// </summary>
        /// <param name="DateTime1">日期一。</param>
        /// <param name="DateTime2">日期二。</param>
        /// <returns>日期间隔TimeSpan。</returns>
        public static TimeSpan DateDiff(DateTime DateTime1, DateTime DateTime2)
        {
            TimeSpan ts1 = new TimeSpan(DateTime1.Ticks);
            TimeSpan ts2 = new TimeSpan(DateTime2.Ticks);
            TimeSpan ts = ts1.Subtract(ts2).Duration();
            return ts;
        }
        #endregion

        #region 格式化日期时间
        /// <summary>
        /// 格式化日期时间
        /// </summary>
        /// <param name="dateTime1">日期时间</param>
        /// <param name="dateMode">显示模式</param>
        /// <returns>0-9种模式的日期</returns>
        public static string FormatDate(DateTime dateTime1, string dateMode)
        {
            switch (dateMode)
            {
                case "0":
                    return dateTime1.ToString("yyyy-MM-dd");
                case "1":
                    return dateTime1.ToString("yyyy-MM-dd HH:mm:ss");
                case "2":
                    return dateTime1.ToString("yyyy/MM/dd");
                case "3":
                    return dateTime1.ToString("yyyy年MM月dd日");
                case "4":
                    return dateTime1.ToString("MM-dd");
                case "5":
                    return dateTime1.ToString("MM/dd");
                case "6":
                    return dateTime1.ToString("MM月dd日");
                case "7":
                    return dateTime1.ToString("yyyy-MM");
                case "8":
                    return dateTime1.ToString("yyyy/MM");
                case "9":
                    return dateTime1.ToString("yyyy年MM月");
                default:
                    return dateTime1.ToString();
            }
        }
        #endregion

        #region HTML转行成TEXT
        /// <summary>
        /// HTML转行成TEXT
        /// </summary>
        /// <param name="strHtml"></param>
        /// <returns></returns>
        public static string HtmlToTxt(string strHtml)
        {
            string[] aryReg ={
            @"<script[^>]*?>.*?</script>",
            @"<(\/\s*)?!?((\w+:)?\w+)(\w+(\s*=?\s*(([""'])(\\[""'tbnr]|[^\7])*?\7|\w+)|.{0})|\s)*?(\/\s*)?>",
            @"([\r\n])[\s]+",
            @"&(quot|#34);",
            @"&(amp|#38);",
            @"&(lt|#60);",
            @"&(gt|#62);",
            @"&(nbsp|#160);",
            @"&(iexcl|#161);",
            @"&(cent|#162);",
            @"&(pound|#163);",
            @"&(copy|#169);",
            @"&#(\d+);",
            @"-->",
            @"<!--.*\n"
            };

            string newReg = aryReg[0];
            string strOutput = strHtml;
            for (int i = 0; i < aryReg.Length; i++)
            {
                Regex regex = new Regex(aryReg[i], RegexOptions.IgnoreCase);
                strOutput = regex.Replace(strOutput, string.Empty);
            }

            strOutput.Replace("<", "");
            strOutput.Replace(">", "");
            strOutput.Replace("\r\n", "");


            return strOutput;
        }
        #endregion

        #region 得到字符串长度，一个汉字长度为2
        /// <summary>
        /// 得到字符串长度，一个汉字长度为2
        /// </summary>
        /// <param name="inputString">参数字符串</param>
        /// <returns></returns>
        public static int StrLength(string inputString)
        {
            System.Text.ASCIIEncoding ascii = new System.Text.ASCIIEncoding();
            int tempLen = 0;
            byte[] s = ascii.GetBytes(inputString);
            for (int i = 0; i < s.Length; i++)
            {
                if ((int)s[i] == 63)
                    tempLen += 2;
                else
                    tempLen += 1;
            }
            return tempLen;
        }
        #endregion

        #region 生成随机数
        /// <summary>
        /// 获得一个按时间的随机数
        /// </summary>
        /// <returns></returns>
        public static string GetDataTimeRnd()
        {
            string year, month, day, hour, minute, second, millisecond;
            DateTime date = DateTime.Now;
            Random rand = new Random();
            year = date.Year.ToString();
            if (date.Month < 10)
                month = "0" + date.Month.ToString();
            else
                month = date.Month.ToString();
            if (date.Day < 10)
                day = "0" + date.Day.ToString();
            else
                day = date.Day.ToString();
            if (date.Hour < 10)
                hour = "0" + date.Hour.ToString();
            else
                hour = date.Hour.ToString();
            if (date.Minute < 10)
                minute = "0" + date.Minute.ToString();
            else
                minute = date.Minute.ToString();
            if (date.Second < 10)
                second = "0" + date.Second.ToString();
            else
                second = date.Second.ToString();
            if (date.Millisecond < 10)
                millisecond = "00" + date.Millisecond.ToString();
            else if (date.Millisecond < 100)
                millisecond = "0" + date.Millisecond.ToString();
            else
                millisecond = date.Millisecond.ToString();
            return year + month + day + hour + minute + second + millisecond + rand.Next(1000).ToString();
        }
        /// <summary>
        /// [MinNum,MaxNum)
        /// </summary>
        /// <param name="MinNum"></param>
        /// <param name="MaxNum"></param>
        /// <returns></returns>
        public static int GetRandomNumber(int MinNum, int MaxNum)
        {
            Random r = new Random(Guid.NewGuid().GetHashCode());
            return r.Next(MinNum, MaxNum);
        }
        /// <summary>
        /// 生成指定位随机数(包含数据大小写字母)
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public static string CreateCheckCode(int n)
        {
            char[] CharArray = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };
            string sCode = "";
            Random random = new Random(Guid.NewGuid().GetHashCode());
            for (int i = 0; i < n; i++)
            {
                sCode += CharArray[random.Next(CharArray.Length)];
            }
            return sCode;
        }
        /// <summary>
        /// 生成指定位随机数，根据类型生成不同的随机数
        /// </summary>
        /// <param name="n"></param>
        /// <param name="createtype">类型 0:字母加数字 1数字 2字母</param>
        /// <returns></returns>
        public static string CreateCheckCode(int n, int createtype, Random random)
        {
            string sCode = "";
            if (createtype == 0)
            {
                char[] CharArray = { '0', 'A', 'B', 'C', 'D', 'E', 'F', '1', 'G', 'H', 'J', 'K', 'L', 'M', 'N', 'O', 'P', '2', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', '3', 'X', 'Y', 'Z', 'a', 'b', 'c', 'd', '4', 'e', 'f', 'g', 'h', 'j', 'k', '5', 'l', 'm', 'n', 'o', '6', 'p', 'q', 'r', 's', '7', 't', 'u', 'v', 'w', '8', 'x', 'y', 'z', '9' };
                for (int i = 0; i < n; i++)
                {
                    sCode += CharArray[random.Next(CharArray.Length)];
                }
            }
            else if (createtype == 1)
            {
                char[] CharArray = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
                for (int i = 0; i < n; i++)
                {
                    sCode += CharArray[random.Next(CharArray.Length)];
                }
            }
            else if (createtype == 2)
            {
                char[] CharArray = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };
                for (int i = 0; i < n; i++)
                {
                    sCode += CharArray[random.Next(CharArray.Length)];
                }
            }
            return sCode;
        }

        /// <summary>
        /// 生成指定位随机数
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public static string CreateCheckCodeWithNum(int n)
        {
            char[] CharArray = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
            string sCode = "";
            Random random = new Random(Guid.NewGuid().GetHashCode());
            for (int i = 0; i < n; i++)
            {
                sCode += CharArray[random.Next(CharArray.Length)];
            }
            return sCode;
        }
        /// <summary>
        /// 生成随机数
        /// </summary>
        /// <param name="ran"></param>
        /// <param name="xLen"></param>
        /// <returns></returns>
        public static string RandomNo(Random ran, int xLen)
        {
            string[] char_array = new string[34];
            char_array[0] = "1";
            char_array[1] = "2";
            char_array[2] = "3";
            char_array[3] = "4";
            char_array[4] = "5";
            char_array[5] = "6";
            char_array[6] = "7";
            char_array[7] = "8";
            char_array[8] = "9";
            char_array[9] = "A";
            char_array[10] = "B";
            char_array[11] = "C";
            char_array[12] = "D";
            char_array[13] = "E";
            char_array[14] = "F";
            char_array[15] = "G";
            char_array[16] = "H";
            char_array[17] = "I";
            char_array[18] = "J";
            char_array[19] = "K";
            char_array[20] = "L";
            char_array[21] = "M";
            char_array[22] = "N";
            char_array[23] = "P";
            char_array[24] = "Q";
            char_array[25] = "R";
            char_array[26] = "S";
            char_array[27] = "T";
            char_array[28] = "W";
            char_array[29] = "U";
            char_array[30] = "V";
            char_array[31] = "X";
            char_array[32] = "Y";
            char_array[33] = "Z";

            string output = "";
            double tmp = 0;
            while (output.Length < xLen)
            {
                tmp = ran.NextDouble();
                output = output + char_array[(int)(tmp * 34)].ToString();
            }
            return output;
        }



        #endregion

        #region 短地址

        /// <summary>
        /// 短地址
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string ShortUrl(string url)
        {
            //可以自定义生成MD5加密字符传前的混合KEY
            string key = "huotu";
            //要使用生成URL的字符
            string[] chars = new string[]{
                "a","b","c","d","e","f","g","h",
                "i","j","k","l","m","n","o","p",
                "q","r","s","t","u","v","w","x",
                "y","z","0","1","2","3","4","5",
                "6","7","8","9","A","B","C","D",
                "E","F","G","H","I","J","K","L",
                "M","N","O","P","Q","R","S","T",
                "U","V","W","X","Y","Z"
              };
            //对传入网址进行MD5加密
            string hex = EncryptHelper.MD5_8(key + url);

            string[] resUrl = new string[4];

            for (int i = 0; i < 4; i++)
            {
                //把加密字符按照8位一组16进制与0x3FFFFFFF进行位与运算
                int hexint = 0x3FFFFFFF & Convert.ToInt32("0x" + hex.Substring(i * 8, 8), 16);
                string outChars = string.Empty;
                for (int j = 0; j < 6; j++)
                {
                    //把得到的值与0x0000003D进行位与运算，取得字符数组chars索引
                    int index = 0x0000003D & hexint;
                    //把取得的字符相加
                    outChars += chars[index];
                    //每次循环按位右移5位
                    hexint = hexint >> 5;
                }
                //把字符串存入对应索引的输出数组
                resUrl[i] = outChars;
            }

            return resUrl[0];
        }

        #endregion



        #region 获得html中的图片的信息
        /// <summary>
        /// 获得html中的图片
        /// </summary>
        /// <param name="sHtmlText"></param>
        /// <returns></returns>
        public static string[] GetHtmlImageUrlList(string sHtmlText)
        {
            // 定义正则表达式用来匹配 img 标签 

            Regex regImg = new Regex(@"<img\b[^<>]*?\bsrc[\s\t\r\n]*=[\s\t\r\n]*[""']?[\s\t\r\n]*(?<imgUrl>[^\s\t\r\n""'<>]*)[^<>]*?/?[\s\t\r\n]*>", RegexOptions.IgnoreCase);
            MatchCollection matches = regImg.Matches(sHtmlText);
            int i = 0;
            string[] sUrlList = new string[matches.Count];
            foreach (Match match in matches)
                sUrlList[i++] = match.Groups["imgUrl"].Value;
            return sUrlList;

        }
        #endregion

        #region 只保留P标签的存在

        /// <summary>
        /// 获得html中"<p></p>"标签中的内容
        /// </summary>
        /// <param name="sHtmlText"></param>
        /// <returns></returns>
        public static string[] GetContentbyPhtml(string sHtmlText)
        {
            // 定义正则表达式用来匹配 <p></p>之间的内容 标签 
            Regex regImg = new Regex(@"<p[^>]*>(.*?)</p>", RegexOptions.IgnoreCase);
            MatchCollection matches = regImg.Matches(sHtmlText);
            int i = 0;
            string[] sContentList = new string[matches.Count];
            foreach (Match match in matches)
                sContentList[i++] = GetStringNoneHtml(match.Groups[1].Value);
            return sContentList;
        }
        #endregion




        /// <summary>
        /// 获取时间的友好的提示
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string GetFriendlyTime(string dt)
        {
            try
            {
                TimeSpan span = DateTime.Now - Convert.ToDateTime(dt);
                if (span.TotalDays > 30)
                {
                    return string.Format("{0}个月{1}天前", (int)span.TotalDays / 30, (int)span.TotalDays % 30);
                }
                if (span.TotalDays > 1)
                {
                    return string.Format("{0}天{1}小时前", (int)Math.Floor(span.TotalDays), span.Hours);
                }
                else if (span.TotalHours > 1)
                {
                    return string.Format("{0}小时{1}分钟前", (int)Math.Floor(span.TotalHours), span.Minutes);
                }
                else if (span.TotalMinutes > 1)
                {
                    return string.Format("{0}分钟前", (int)Math.Floor(span.TotalMinutes));
                }
                else if (span.TotalSeconds >= 1)
                {
                    return string.Format("{0}秒前", (int)Math.Floor(span.TotalSeconds));
                }
                else
                {
                    return "";
                }
            }
            catch
            {
                return "";
            }
        }
        /// <summary>
        /// 将时间格式转换成友好的提示
        /// </summary>
        /// <param name="dt">The dt.</param>
        /// <returns>System.String.</returns>
        public static string GetConvertFriendlyTime(string dt)
        {
            try
            {
                TimeSpan span = DateTime.Now - Convert.ToDateTime(dt);
                if (span.TotalDays > 30)
                {
                    return string.Format("{0}个月前", (int)span.TotalDays / 30);
                }
                if (span.TotalDays > 1)
                {
                    return string.Format("{0}天前", (int)Math.Floor(span.TotalDays));
                }
                else if (span.TotalHours > 1)
                {
                    return string.Format("{0}小时", (int)Math.Floor(span.TotalHours));
                }
                else if (span.TotalMinutes > 1)
                {
                    return string.Format("{0}分钟前", (int)Math.Floor(span.TotalMinutes));
                }
                else if (span.TotalSeconds >= 1)
                {
                    return string.Format("{0}秒前", (int)Math.Floor(span.TotalSeconds));
                }
                else
                {
                    return "刚刚";
                }
            }
            catch
            {
                return "";
            }
        }




        /// <summary>
        /// 获取枚举的注释
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetEnumDescription<T>(int value) where T : new()
        {
            Type t = typeof(T);
            foreach (System.Reflection.MemberInfo mInfo in t.GetMembers())
            {
                if (mInfo.Name == t.GetEnumName(value))
                {
                    foreach (Attribute attr in Attribute.GetCustomAttributes(mInfo))
                    {
                        if (attr.GetType() == typeof(System.ComponentModel.DescriptionAttribute))
                        {
                            return ((System.ComponentModel.DescriptionAttribute)attr).Description;
                        }
                    }
                }
            }
            return "";
        }


        #region 基础函数
        
        /// <summary>
        /// 取字符左函数
        /// </summary>
        /// <param name="Object">要操作的 string  数据</param>
        /// <param name="MaxLength">最大长度</param>
        /// <returns>string</returns>
        public static string Left(object Object, int MaxLength)
        {
            if (GlobalProvider.IsNull(Object)) return "";
            return Object.ToString().Substring(0, Math.Min(Object.ToString().Length, MaxLength));
        }
        /// <summary>
        /// 取字符中间函数
        /// </summary>
        /// <param name="Object">要操作的 string  数据</param>
        /// <param name="StarIndex">开始的位置索引</param>
        /// <param name="MaxLength">最大长度</param>
        /// <returns>string</returns>
        public static string Mid(string Object, int StarIndex, int MaxLength)
        {
            if (GlobalProvider.IsNull(Object)) return "";
            if (StarIndex >= Object.Length) return "";
            return Object.Substring(StarIndex, MaxLength);
        }
        /// <summary>
        /// 取字符右函数
        /// </summary>
        /// <param name="Object">要操作的 string  数据</param>
        /// <param name="MaxLength">最大长度</param>
        /// <returns>string</returns>
        public static string Right(object Object, int MaxLength)
        {
            if (GlobalProvider.IsNull(Object)) return "";
            int i = Object.ToString().Length;
            if (i < MaxLength) { MaxLength = i; i = 0; } else { i = i - MaxLength; }
            return Object.ToString().Substring(i, MaxLength);
        }

        #endregion


        #region 字符转换函数
        /// <summary>
        /// 字符 int  转换为 char
        /// </summary>
        /// <param name="Int">字符[int]</param>
        /// <returns>char</returns>
        public static char IntToChar(int Int) { return (char)Int; }
        /// <summary>
        /// 字符 int  转换为字符 string
        /// </summary>
        /// <param name="Int">字符 int</param>
        /// <returns>字符 string</returns>
        public static string IntToString(int Int) { return IntToChar(Int).ToString(); }
        /// <summary>
        /// 字符 string  转换为字符 int
        /// </summary>
        /// <param name="Strings">字符 string</param>
        /// <returns>字符 int</returns>
        public static int StringToInt(string Strings)
        {
            if (GlobalProvider.IsNull(Strings)) return -100; char[] chars = Strings.ToCharArray(); return (int)chars[0];
        }
        /// <summary>
        /// 字符 string  转换为 char
        /// </summary>
        /// <param name="Strings">字符 string</param>
        /// <returns>char</returns>
        public static char StringToChar(string Strings) { return IntToChar(StringToInt(Strings)); }

        /// <summary>
        /// 转换为字符串
        /// </summary>
        /// <param name="Object"></param>
        /// <returns></returns>
        public static string ToString(object Object)
        {
            try { return Object.ToString(); }
            catch { return string.Empty; }
        }
        #endregion

        #region 操作 int  数据
      
        /// <summary>
        /// 对象是否为 int  类型数据
        /// </summary>
        /// <param name="Object">要判断的对象</param>
        /// <param name="isTrue">返回是否转换成功</param>
        /// <returns>int值</returns>
        private static int IsInt(object Object, out bool isTrue)
        {
            try { isTrue = true; return int.Parse(Object.ToString()); }
            catch { isTrue = false; return 0; }
        }
        /// <summary>
        /// 转换成为 int  数据
        /// </summary>
        /// <param name="Object">要转换的对象</param>
        /// <returns>int 数据</returns>
        public static int ToInt(object Object) { return ToInt(Object, 0); }
        /// <summary>
        /// 转换成为 int  数据
        /// </summary>
        /// <param name="Object">要转换的对象</param>
        /// <param name="Default">默认值</param>
        /// <returns>int 数据</returns>
        public static int ToInt(object Object, int Default) { return ToInt(Object, Default, -2147483648, 2147483647); }
        /// <summary>
        /// 转换成为 int  数据
        /// </summary>
        /// <param name="Object">要转换的对象</param>
        /// <param name="Default">默认值</param>
        /// <param name="MinInt"> 下界限定的最小值 , 若超过范围 , 则返回 默认值</param>
        /// <returns>int 数据</returns>
        public static int ToInt(object Object, int Default, int MinInt) { return ToInt(Object, Default, MinInt, 2147483647); }
        /// <summary>
        /// 转换成为 int  数据
        /// </summary>
        /// <param name="Object">要转换的对象</param>
        /// <param name="Default">默认值</param>
        /// <param name="MinInt"> 下界限定的最小值 , 若超过范围 , 则返回 默认值</param>
        /// <param name="MaxInt">上界限定的最大值 , 若超过范围 , 则返回 默认值</param>
        /// <returns>int 数据</returns>
        public static int ToInt(object Object, int Default, int MinInt, int MaxInt)
        {
            bool isTrue = false;
            int Int = IsInt(Object, out isTrue);
            if (!isTrue) return Default;
            if (Int < MinInt || Int > MaxInt) return Default;
            return Int;
        }
        #endregion

        #region 操作 long  数据


        /// <summary>
        /// 对象是否为 long  类型数据
        /// </summary>
        /// <param name="Object">要判断的对象</param>
        /// <param name="isTrue">返回是否转换成功</param>
        /// <returns>long值</returns>
        private static long IsLong(object Object, out bool isTrue)
        {
            try { isTrue = true; return long.Parse(Object.ToString()); }
            catch { isTrue = false; return 0; }
        }
        /// <summary>
        /// 转换成为 Long 数据
        /// </summary>
        /// <param name="Object">要转换的对象</param>
        /// <returns>Long 数据</returns>
        public static long ToLong(object Object) { return ToLong(Object, 0); }
        /// <summary>
        /// 转换成为 Long 数据
        /// </summary>
        /// <param name="Object">要转换的对象</param>
        /// <param name="Default">默认值</param>
        /// <returns>Long 数据</returns>
        public static long ToLong(object Object, long Default) { return ToLong(Object, Default, -9223372036854775808, 9223372036854775807); }
        /// <summary>
        /// 转换成为 long 数据
        /// </summary>
        /// <param name="Object">要转换的对象</param>
        /// <param name="Default">转换不成功返回的默认值</param>
        /// <param name="MinLong">下界限定的最小值 , 若超过范围 , 则返回 默认值</param>
        /// <returns>long 数据</returns>
        public static long ToLong(object Object, long Default, long MinLong) { return ToLong(Object, Default, MinLong, 9223372036854775807); }
        /// <summary>
        /// 转换成为 long 数据
        /// </summary>
        /// <param name="Object">要转换的对象</param>
        /// <param name="Default">默认值</param>
        /// <param name="MinLong">下界限定的最小值 , 若超过范围 , 则返回 默认值</param>
        /// <param name="MaxLong">上界限定的最大值 , 若超过范围 , 则返回 默认值</param>
        /// <returns>long 数据</returns>
        public static long ToLong(object Object, long Default, long MinLong, long MaxLong)
        {
            bool isTrue = false;
            long Long = IsLong(Object, out isTrue);
            if (!isTrue) return Default;
            if (Long < MinLong || Long > MaxLong) return Default;
            return Long;
        }
        #endregion

        #region 操作 float  数据


        /// <summary>
        /// 对象是否为 float  类型数据
        /// </summary>
        /// <param name="Object">要判断的对象</param>
        /// <param name="isTrue">返回是否转换成功</param>
        /// <returns>float值</returns>
        private static float IsFloat(object Object, out bool isTrue)
        {            
            try { isTrue = true; return float.Parse(Object.ToString()); }
            catch { isTrue = false; return 0; }
        }
        /// <summary>
        /// 转换成为 float 数据
        /// </summary>
        /// <param name="Object">要转换的对象</param>
        /// <returns>float 数据</returns>
        public static float ToFloat(object Object) { return ToFloat(Object, 0); }
        /// <summary>
        /// 转换成为 float 数据
        /// </summary>
        /// <param name="Object">要转换的对象</param>
        /// <param name="Default">默认值</param>
        /// <returns>float 数据</returns>
        public static float ToFloat(object Object, float Default) { return ToFloat(Object, Default, Convert.ToSingle(-3.4 * Math.Pow(10, 38)), Convert.ToSingle(3.4f * Math.Pow(10, 38))); }
        /// <summary>
        /// 转换成为 float 数据
        /// </summary>
        /// <param name="Object">要转换的对象</param>
        /// <param name="Default">默认值</param>
        /// <param name="MinFloat"> 小于等于 转换成功后,下界限定的最小值,若超过范围 则返回 默认值</param>
        /// <returns>float 数据</returns>
        public static float ToFloat(object Object, float Default, float MinFloat) { return ToFloat(Object, Default, MinFloat, Convert.ToSingle(3.4f * Math.Pow(10, 38))); }
        /// <summary>
        /// 转换成为 float 数据
        /// </summary>
        /// <param name="Object">要转换的对象</param>
        /// <param name="Default">默认值</param>
        /// <param name="MinFloat"> 下界限定的最小值 , 若超过范围 , 则返回 默认值</param>
        /// <param name="MaxFloat"> 上界限定的最大值 , 若超过范围 , 则返回 默认值</param>
        /// <returns>float 数据</returns>
        public static float ToFloat(object Object, float Default, float MinFloat, float MaxFloat)
        {
            bool isTrue = false;
            float Float = IsFloat(Object, out isTrue);
            if (!isTrue) return Default;
            if (Float < MinFloat || Float > MaxFloat) return Default;
            return Float;
        }

       
        #endregion

        #region 操作 decimal  数据

        
        /// <summary>
        /// 对象是否为 decimal  类型数据
        /// </summary>
        /// <param name="Object">要判断的对象</param>
        /// <param name="isTrue">返回是否转换成功</param>
        /// <returns>decimal值</returns>
        private static decimal IsDecimal(object Object, out bool isTrue)
        {
            try { isTrue = true; return decimal.Parse(Object.ToString()); }
            catch { isTrue = false; return 0; }
        }
        /// <summary>
        /// 转换成为 decimal 数据
        /// </summary>
        /// <param name="Object">要转换的对象</param>
        /// <returns>decimal 数据</returns>
        public static decimal ToDecimal(object Object) { return ToDecimal(Object, 0); }
        /// <summary>
        /// 转换成为 decimal 数据
        /// </summary>
        /// <param name="Object">要转换的对象</param>
        /// <param name="Default">默认值</param>
        /// <returns>decimal 数据</returns>
        public static decimal ToDecimal(object Object, decimal Default) { return ToDecimal(Object, Default, Convert.ToDecimal(-7.9 * Math.Pow(10, 28)), Convert.ToDecimal(7.9 * Math.Pow(10, 28))); }

        /// <summary>
        /// 转换成为 decimal 数据
        /// </summary>
        /// <param name="Object">要转换的对象</param>
        /// <param name="Default">默认值</param>
        /// <param name="MinFloat"> 小于等于 转换成功后,下界限定的最小值,若超过范围 则返回 默认值</param>
        /// <returns>decimal 数据</returns>
        public static decimal ToDecimal(object Object, decimal Default, decimal MinFloat) { return ToDecimal(Object, Default, MinFloat, Convert.ToDecimal(7.9 * Math.Pow(10, 28))); }
        /// <summary>
        /// 转换成为 decimal 数据
        /// </summary>
        /// <param name="Object">要转换的对象</param>
        /// <param name="Default">默认值</param>
        /// <param name="MinDecimal"> 下界限定的最小值 , 若超过范围 , 则返回 默认值</param>
        /// <param name="MaxDecimal"> 上界限定的最大值 , 若超过范围 , 则返回 默认值</param>
        /// <returns>decimal 数据</returns>
        public static decimal ToDecimal(object Object, decimal Default, decimal MinDecimal, decimal MaxDecimal)
        {
            bool isTrue = false;
            decimal Decimal = IsDecimal(Object, out isTrue);
            if (!isTrue) return Default;
            if (Decimal < MinDecimal || Decimal > MaxDecimal) return Default;
            return Decimal;
        }
        #endregion

        #region 操作 DateTime 数据

        /// <summary>
        /// 是否为时间格式
        /// </summary>
        /// <param name="Object">要判断的对象</param>
        /// <param name="isTrue">返回是否转换成功</param>
        /// <returns>DateTime</returns>
        public static DateTime IsTime(object Object, out bool isTrue)
        {
            isTrue = false;
            if (GlobalProvider.IsNull(Object)) return (DateTime)System.Data.SqlTypes.SqlDateTime.MinValue;
            try { isTrue = true; return DateTime.Parse(Object.ToString()); }
            catch { isTrue = false; return (DateTime)System.Data.SqlTypes.SqlDateTime.MinValue; }
        }
        /// <summary>
        /// 操作 DateTime  数据
        /// </summary>
        /// <param name="Object">要操作的字符</param>
        /// <returns>DateTime</returns>
        public static DateTime ToTime(string Object) { return ToTime(Object, DateTime.Now); }
        /// <summary>
        /// 字符串转换为时间函数
        /// </summary>
        /// <param name="Object">要操作的字符</param>
        /// <param name="Default">默认时间</param>
        /// <returns>DateTime</returns>
        public static DateTime ToTime(string Object, DateTime Default)
        {
            if (GlobalProvider.IsNull(Object)) return Default;
            bool isTrue = false;
            DateTime Time = IsTime(Object, out isTrue);
            if (!isTrue) return Default;
            return Time;
        }
        /// <summary>
        /// 获得当前时间
        /// </summary>
        /// <param name="format">时间样式</param>
        /// <returns>string</returns>
        public static string ToNow(string format) { return DateTime.Now.ToString(format); }
        /// <summary>
        /// 转换字符串为格式化时间字符串
        /// </summary>
        /// <param name="Object">要操作的字符</param>
        /// <returns>string</returns>
        public static string ToTimes(string Object) { return ToTimes(Object, "yyyy-MM-dd HH:mm:ss"); }
        /// <summary>
        /// 转换字符串为格式化时间字符串
        /// </summary>
        /// <param name="Object">要操作的字符</param>
        /// <param name="format">格式化样式</param>
        /// <returns>string</returns>
        public static string ToTimes(string Object, string format) { return ToTimes(Object, DateTime.Now, format); }
        /// <summary>
        /// 转换字符串为格式化时间字符串
        /// </summary>
        /// <param name="Object">要操作的字符</param>
        /// <param name="Default">默认时间</param>
        /// <returns>string</returns>
        public static string ToTimes(string Object, DateTime Default) { return ToTimes(Object, Default, "yyyy-MM-dd HH:mm:ss"); }
        /// <summary>
        /// 转换字符串为格式化时间字符串
        /// </summary>
        /// <param name="Object">要操作的字符</param>
        /// <param name="Default">默认时间</param>
        /// <param name="format">格式化样式</param>
        /// <returns>string</returns>
        public static string ToTimes(string Object, DateTime Default, string format)
        {
            if (GlobalProvider.IsNull(Object)) return Default.ToString(format);
            bool isTrue = false;
            DateTime Time = IsTime(Object, out isTrue);
            if (!isTrue) return Default.ToString(format);
            return Time.ToString(format);
        }
        /// <summary>
        /// 获取两个时间差值（按天算）
        /// </summary>
        /// <param name="old_time"></param>
        /// <param name="new_time"></param>
        /// <returns></returns>
        public static int DateDiffDays(DateTime old_time, DateTime new_time)
        {
            TimeSpan ts = new TimeSpan(new_time.Ticks - old_time.Ticks);
            return ts.Days;
        }
        /// <summary>
        /// 获取两个时间差值（按周算）
        /// </summary>
        /// <param name="old_time"></param>
        /// <param name="new_time"></param>
        /// <returns></returns>
        public static int DateDiffWeek(DateTime old_time, DateTime new_time)
        {
            TimeSpan ts = new TimeSpan(new_time.Ticks - old_time.Ticks);
            return (ts.Days / 7);
        }
        /// <summary>
        /// 获取当天，按当前时间获取当天的剩下时间
        /// </summary>
        /// <param name="time">当前时间</param>
        /// <returns></returns>
        public static Double DateDiffMinutes(DateTime time)
        {
            TimeSpan ts = new TimeSpan(DateTime.Today.AddHours(24).Ticks - time.Ticks);
            return ts.Minutes;
        }

        #endregion

        #region datetime与unixtime相互转换
        /// <summary>
        /// datetime转换为unixtime
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static int ConvertDateTimeInt(System.DateTime time)
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            return (int)(time - startTime).TotalSeconds;
        }

        /// <summary>
        /// unix时间转换为datetime
        /// </summary>
        /// <param name="timeStamp"></param>
        /// <returns></returns>
        public static DateTime UnixTimeToTime(string timeStamp)
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            long lTime = long.Parse(timeStamp + "0000000");
            TimeSpan toNow = new TimeSpan(lTime);
            return dtStart.Add(toNow);
        }
        /// <summary>
        /// 得到时间戳unix
        /// </summary>
        /// <returns></returns>
        public static string GetTimeStamp()
        {
            return ((DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000).ToString();
        }


        /// <summary>
        /// 获取UTC时间
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static Int64 GetUTCTime(DateTime time)
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            return (Int64)(time - startTime).TotalMilliseconds;
        }
        /// <summary>
        /// 从UTC时间转为正常时间格式（毫秒）
        /// </summary>
        /// <param name="t">毫秒</param>
        /// <returns></returns>
        public static DateTime GetTimeFromUTC(Int64 t)
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            return startTime.AddMilliseconds(t).ToLocalTime();
        }
        #endregion


        /// <summary>
        /// 将对象属性转换为key-value对 jwei
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static Dictionary<String, Object> ObjectToDic(Object o)
        {
            Dictionary<String, Object> map = new Dictionary<string, object>();
            Type t = o.GetType();
            PropertyInfo[] pi = t.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo p in pi)
            {
                MethodInfo mi = p.GetGetMethod();

                if (mi != null && mi.IsPublic)
                {
                    map.Add(p.Name, mi.Invoke(o, new Object[] { }));
                }
            }

            return map;
        }
    }
}

//newtonsoft报错，添加
namespace System.Runtime.CompilerServices
{
    /// <summary>
    /// 
    /// </summary>
    public class ExtensionAttribute : Attribute { }
}