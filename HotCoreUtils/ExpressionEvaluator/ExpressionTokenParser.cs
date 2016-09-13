using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using HotCoreUtils.ExpressionEvaluator.ExpressionNotation;
using HotCoreUtils.ExpressionEvaluator.ExpressionNotation.CompOperation;
using HotCoreUtils.ExpressionEvaluator.ExpressionNotation.Data;
using HotCoreUtils.ExpressionEvaluator.ExpressionNotation.FunctionOperation;
using HotCoreUtils.ExpressionEvaluator.ExpressionNotation.LogicalOperation;
using HotCoreUtils.ExpressionEvaluator.ExpressionNotation.MathOperation;
using HotCoreUtils.ExpressionEvaluator.ExpressionNotation.Typographic;
using System.Collections.Generic;

namespace HotCoreUtils.ExpressionEvaluator
{
    internal class ExpressionTokenParser
    {
        #region Constants

        public const string RX_Variable = @"\@([^@])+\@";

        public const string RX_Array = @"VALOR\[(?<valueType>(a(n|d|t))),\s*(?<value>(\w+;?)+)\s*\]";
        public const string RX_Boolean = @"VALOR\[(?<valueType>b),\s*(?<value>[0-1])\s*\]";
        public const string RX_Number = @"VALOR\[(?<valueType>(n|i|d)),\s*(?<value>(-?\d+(\.\d+)?))\s*\]";
        public const string RX_String = @"VALOR\[(?<valueType>t),\s*(?<value>([^\]])*)\s*\]";
        public const string RX_DateTime = @"VALOR\[(?<valueType>dt),\s*(?<value>((?<dtYear>(\d{2})|(\d{4}))\-(?<dtMonth>(\d){1,2})\-(?<dtDay>(\d){1,2})(\s((?<dtHour>\d{1,2})\:(?<dtMinute>\d{1,2})\:(?<dtSecond>\d{1,2})))?))\s*\]";
        public const string RX_TypeValues = @"((" + RX_Array + @")|(" + RX_Boolean + @")|(" + RX_Number + @")|(" + RX_String + @")|(" + RX_DateTime + @"))";



        public const string RX_Plus = @"\[\-\]";
        public const string RX_Minus = @"\[\+\]";
        public const string RX_Multiply = @"\[\*\]";
        public const string RX_Divide = @"\[\/\]";
        public const string RX_UnaryMinus = @"\~";
        public const string RX_MathOperatorsMissUnary = @"((" + RX_Plus + @")|(" + RX_Minus + @")|(" + RX_Multiply + @")|(" + RX_Divide + @"))";
        public const string RX_MathOperators = @"((" + RX_Plus + @")|(" + RX_Minus + @")|(" + RX_Multiply + @")|(" + RX_Divide + @")|(" + RX_UnaryMinus + @"))";

        public const string RX_LogicalAnd = @"\[AND\]";
        public const string RX_LogicalOr = @"\[OR\]";
        public const string RX_LogicalNot = @"\[NOT\]";
        public const string RX_LogicalOperators = @"((" + RX_LogicalAnd + @")|(" + RX_LogicalOr + @")|(" + RX_LogicalNot + @"))";

        public const string RX_CompEquals = @"\[\=\]";
        public const string RX_CompNotEquals = @"\[\<\>\]";
        public const string RX_CompGreaterThan = @"\[\>\]";
        public const string RX_CompGreaterEqualsThan = @"\[\>\=\]";
        public const string RX_CompLessThan = @"\[\<\]";
        public const string RX_CompLessEqualsThan = @"\[\<\=\]";
        public const string RX_CompContains = @"\[CONTAINS\]";
        public const string RX_CompOperators = @"((" + RX_CompEquals + @")|(" + RX_CompNotEquals + @")|(" + RX_CompGreaterThan + @")|(" + RX_CompGreaterEqualsThan + @")|(" + RX_CompLessThan + @")|(" + RX_CompLessEqualsThan + @")|(" + RX_CompContains + @"))";

        public const string RX_FuncSine = @"\[SIN\]";
        public const string RX_FuncCosine = @"\[COS\]";
        public const string RX_FuncSum = @"\[SUM\]";
        public const string RX_FuncAverage = @"\[AVERAGE\]";
        public const string RX_Functions = @"((" + RX_FuncSine + @")|(" + RX_FuncCosine + @")|(" + RX_FuncSum + @")|(" + RX_FuncAverage + @"))";

        #endregion Constants

        #region Members

#if !SILVERLIGHT
        private static CultureInfo m_USCultureInfo = CultureInfo.GetCultureInfo("en-US");
#else
        private static CultureInfo m_USCultureInfo = new CultureInfo("en-US");
#endif

        #endregion Members

        #region Properties

        public static Func<string, object> GetVariableValue { get; set; }

        public static Dictionary<string, object> DictVariableValues { get; set; }

        #endregion Properties

        #region Methods

        public static IExpressionNotationToken GetExpressionNotationToken(string inputExp)
        {
            IExpressionNotationToken _returnValue = null;

            _returnValue = getExpressionNotationToken_Values(inputExp) ??
                            getExpressionNotationToken_CheckParenthesis(inputExp) ??
                            getExpressionNotationToken_Variable(inputExp) ??
                            getExpressionNotationToken_MathOperators(inputExp) ??
                            getExpressionNotationToken_LogicalOperators(inputExp) ??
                            (IExpressionNotationToken)getExpressionNotationToken_CompOperators(inputExp) ??
                            getExpressionNotationToken_Functions(inputExp);

            return _returnValue;
        }

        private static Parenthesis getExpressionNotationToken_CheckParenthesis(string inputExp)
        {
            Parenthesis _returnValue = null;

            switch (inputExp)
            {
                case "(":
                    {
                        _returnValue = new LeftParenthesis();
                        _returnValue.TokenValue = inputExp;
                        break;
                    }
                case ")":
                    {
                        _returnValue = new RightParenthesis();
                        _returnValue.TokenValue = inputExp;
                        break;
                    }
            }

            return _returnValue;
        }

        private static DataVariable getExpressionNotationToken_Variable(string inputExp)
        {
            DataVariable _returnValue = null;

            Match _match = Regex.Match(inputExp, ExpressionTokenParser.RX_Variable, RegexOptions.IgnoreCase);

            if (_match.Success)
            {
                _returnValue = new DataVariable();
                _returnValue.TokenValue = inputExp;
                _returnValue.GetVariableValue = GetVariableValue;

                if (!DictVariableValues.ContainsKey(_returnValue.TokenValue))
                {
                    DictVariableValues.Add(_returnValue.TokenValue, null);
                }
            }

            return _returnValue;
        }

        private static BaseDataValue getExpressionNotationToken_Values(string inputExp)
        {
            BaseDataValue _returnValue = null;

            Match _match = Regex.Match(inputExp, ExpressionTokenParser.RX_TypeValues, RegexOptions.IgnoreCase);

            if (_match.Success)
            {
                string _strValueType = _match.Groups["valueType"].Value;
                string _strValue = _match.Groups["value"].Value;

                switch (_strValueType.ToLower())
                {
                    case "an": //Array Number
                        {
                            _returnValue = new DataArrayDecimal
                            {
                                Value = _strValue.Split(';')
                                                                .Select(a => decimal.Parse(a, m_USCultureInfo))
                                                                .ToArray()
                            };
                            break;
                        }
                    case "ad": //Array Date
                        {
                            _returnValue = new DataArrayDateTime
                            {
                                Value = _strValue.Split(';')
                                                            .Select(a => DateTime.Parse(a, m_USCultureInfo))
                                                            .ToArray()
                            };
                            break;
                        }
                    case "at": //Array Text
                        {
                            _returnValue = new DataArrayString
                            {
                                Value = _strValue.Split(';')
                                   .Select(a => a.ToString())
                                   .ToArray()
                            };
                            break;
                        }
                    case "b":
                        {
                            _returnValue = new DataBoolean
                            {
                                Value = Convert.ToBoolean(Int32.Parse(_strValue))
                            };
                            break;
                        }
                    case "n":
                    case "i":
                    case "d":
                        {
                            _returnValue = new DataNumber
                            {
                                Value = decimal.Parse(_strValue, m_USCultureInfo)
                            };
                            break;
                        }
                    case "t":
                        {
                            _returnValue = new DataString
                            {
                                Value = _strValue
                            };
                            break;
                        }
                    case "dt":
                        {
                            _returnValue = new DataDateTime
                            {
                                Value = parseDateTime(_match)
                            };
                            break;
                        }
                }

                _returnValue.TokenValue = inputExp;
            }

            return _returnValue;
        }

        private static IMathOperator getExpressionNotationToken_MathOperators(string inputExp)
        {
            IMathOperator _returnValue = null;

            Match _match = Regex.Match(inputExp, ExpressionTokenParser.RX_MathOperators, RegexOptions.IgnoreCase);

            if (_match.Success)
            {
                string _strValueType = _match.Value;

                switch (_strValueType)
                {
                    case "[+]":
                        {
                            _returnValue = new MathOperatorPlus();
                            break;
                        }
                    case "[-]":
                        {
                            _returnValue = new MathOperatorMinus();
                            break;
                        }
                    case "[*]":
                        {
                            _returnValue = new MathOperatorMultiply();
                            break;
                        }
                    case "[/]":
                        {
                            _returnValue = new MathOperatorDivision();
                            break;
                        }
                    case "~":
                        {
                            _returnValue = new MathOperatorUnaryMinus();
                            break;
                        }
                }

                _returnValue.TokenValue = inputExp;
            }

            return _returnValue;
        }

        private static ILogicalOperator getExpressionNotationToken_LogicalOperators(string inputExp)
        {
            ILogicalOperator _returnValue = null;

            Match _match = Regex.Match(inputExp, ExpressionTokenParser.RX_LogicalOperators, RegexOptions.IgnoreCase);

            if (_match.Success)
            {
                string _strValueType = _match.Value.ToLower();

                switch (_strValueType)
                {
                    case "[and]":
                        {
                            _returnValue = new LogicalOperatorAnd();
                            break;
                        }
                    case "[or]":
                        {
                            _returnValue = new LogicalOperatorOr();
                            break;
                        }
                    case "[not]":
                        {
                            _returnValue = new LogicalOperatorNot();
                            break;
                        }
                }

                _returnValue.TokenValue = inputExp;
            }

            return _returnValue;
        }

        private static ICompOperator getExpressionNotationToken_CompOperators(string inputExp)
        {
            ICompOperator _returnValue = null;

            Match _match = Regex.Match(inputExp, ExpressionTokenParser.RX_CompOperators, RegexOptions.IgnoreCase);

            if (_match.Success)
            {
                string _strValueType = _match.Value.ToLower();

                switch (_strValueType)
                {
                    case "[=]":
                        {
                            _returnValue = new CompOperatorEquals();
                            break;
                        }
                    case "[<>]":
                        {
                            _returnValue = new CompOperatorNotEquals();
                            break;
                        }
                    case "[>]":
                        {
                            _returnValue = new CompOperatorGreaterThan();
                            break;
                        }
                    case "[>=]":
                        {
                            _returnValue = new CompOperatorGreaterEqualsThan();
                            break;
                        }
                    case "[<]":
                        {
                            _returnValue = new CompOperatorLessThan();
                            break;
                        }
                    case "[<=]":
                        {
                            _returnValue = new CompOperatorLessEqualsThan();
                            break;
                        }
                    case "[contains]":
                        {
                            _returnValue = new CompOperatorContains();
                            break;
                        }
                }

                _returnValue.TokenValue = inputExp;
            }

            return _returnValue;
        }

        private static IFunctionOperator getExpressionNotationToken_Functions(string inputExp)
        {
            IFunctionOperator _returnValue = null;

            Match _match = Regex.Match(inputExp, ExpressionTokenParser.RX_Functions, RegexOptions.IgnoreCase);

            if (_match.Success)
            {
                string _strValueType = _match.Value.ToLower();

                switch (_strValueType)
                {
                    case "[sin]":
                        {
                            _returnValue = new FunctionOperatorSin();
                            break;
                        }
                    case "[cos]":
                        {
                            _returnValue = new FunctionOperatorCos();
                            break;
                        }
                    case "[sum]":
                        {
                            _returnValue = new FunctionOperatorSum();
                            break;
                        }
                    case "[average]":
                        {
                            _returnValue = new FunctionOperatorAverage();
                            break;
                        }
                }


                _returnValue.TokenValue = inputExp;
            }

            return _returnValue;
        }

        private static DateTime parseDateTime(Match match)
        {
            DateTime _returnValue = DateTime.MinValue;

            int _year = 1;
            int _month = 1;
            int _day = 1;

            int _hour = 0;
            int _minute = 0;
            int _second = 0;

            if (match.Groups["dtYear"].Success)
            {
                _year = Int32.Parse(match.Groups["dtYear"].Value);
            }
            if (match.Groups["dtMonth"].Success)
            {
                _month = Int32.Parse(match.Groups["dtMonth"].Value);
            }
            if (match.Groups["dtDay"].Success)
            {
                _day = Int32.Parse(match.Groups["dtDay"].Value);
            }

            if (match.Groups["dtHour"].Success)
            {
                _hour = Int32.Parse(match.Groups["dtHour"].Value);
            }
            if (match.Groups["dtMinute"].Success)
            {
                _minute = Int32.Parse(match.Groups["dtMinute"].Value);
            }
            if (match.Groups["dtSecond"].Success)
            {
                _second = Int32.Parse(match.Groups["dtSecond"].Value);
            }

            _returnValue = new DateTime(_year, _month, _day, _hour, _minute, _second);

            return _returnValue;
        }

        #endregion Methods
    }
}
