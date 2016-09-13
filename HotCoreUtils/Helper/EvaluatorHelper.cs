using HotCoreUtils.ExpressionEvaluator;
using System.Text.RegularExpressions;

namespace HotCoreUtils.Helper
{
    /// <summary>
    /// 计算器帮助类
    /// 描述: 根据计算公式,计算出结果,支持-加减乘除
    /// 作者：郭孟稳
    /// 时间：2016/08/01 11:01:16
    /// </summary>	
    public class EvaluatorHelper
    {
        /// <summary>
        /// 计算公式并返回计算结果,如果表达式出错则抛出异常，支持-加减乘除
        /// </summary>
        /// <param name="expression">表达式,如"1+2+3+4"</param>
        /// <returns></returns>
        public static object Eval(string expression)
        {
            string output = expression;
            //if (_expression == null)
            //    _expression = new ExpressionInterpreter();
            ExpressionInterpreter _expression = new ExpressionInterpreter();

            //匹配小数和整数正则表达式(包含正负)
            //示例：0*(15+-1+5)+1.5+-2.3+1
            string pattern = @"(?<!\d)-?\d*\.\d*|(?<!\d)-?\d+";
            //将公式替换成规定格式如VALOR[n,0]*(VALOR[n,15]+(VALOR[n,-1]+VALOR[n,5]))+VALOR[n,1.5]+(VALOR[n,-2.3])+VALOR[n,1]
            output = Regex.Replace(output, pattern, new MatchEvaluator((Match m) =>
            {
                return string.Format("VALOR[n,{0}]", m.Value);
            }), RegexOptions.Compiled);

            //匹配加减乘除运算符，并替换成规定格式
            //如VALOR[n,0][*](VALOR[n,15][+](VALOR[n,-1][+]VALOR[n,5]))[+]VALOR[n,1.5][+](VALOR[n,-2.3])[+]VALOR[n,1]
            string o = @"[\*|\+|\-|/](?!\d)";
            output = Regex.Replace(output, o, new MatchEvaluator((Match m) =>
            {
                return string.Format("[{0}]", m.Value);
            }), RegexOptions.Compiled);
            _expression.Parse(output);
            return _expression.Evaluate();
        }
    }
}
