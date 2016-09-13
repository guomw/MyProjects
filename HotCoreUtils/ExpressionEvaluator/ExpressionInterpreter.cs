using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using HotCoreUtils.ExpressionEvaluator.ExpressionNotation;
using HotCoreUtils.ExpressionEvaluator.ExpressionNotation.Typographic;

namespace HotCoreUtils.ExpressionEvaluator
{
    /// <summary>
    /// 
    /// </summary>
    public class ExpressionInterpreter
    {
        private static string TOKEN_SEP = ((char)254).ToString();

        #region Members

        private Queue<object> m_Output;
        private Stack<object> m_Ops;

        private string m_OriginalExpression;
        private string m_TransitionExpression;

        private Dictionary<string, object> m_dictVariableValues = new Dictionary<string, object>();

        #endregion Members

        #region Properties

        public string OriginalExpression
        {
            get { return m_OriginalExpression; }
            private set { m_OriginalExpression = value; }
        }

        public string TransitionExpression
        {
            get { return m_TransitionExpression; }
            private set { m_TransitionExpression = value; }
        }

        #endregion Properties

        #region Constructors

        public ExpressionInterpreter()
        {
            OriginalExpression = string.Empty;
            TransitionExpression = string.Empty;
        }

        #endregion Constructors

        #region Methods

        public void Parse(string Expression)
        {
            m_Output = new Queue<object>();
            m_Ops = new Stack<object>();


            OriginalExpression = Expression;
            TransitionExpression = generateTransitionExpression(Expression);

            PushToken();
        }

        public void SetVariable(string varName, object varValue)
        {
            if (m_dictVariableValues.ContainsKey(varName))
            {
                m_dictVariableValues.Remove(varName);

                if ((varValue is byte) ||
                    (varValue is int) ||
                    (varValue is long) ||
                    (varValue is decimal) ||
                    (varValue is float))
                {
                    varValue = Convert.ToDecimal(varValue);
                }

                m_dictVariableValues.Add(varName, varValue);
            }
        }

        public object Evaluate()
        {
            Stack<object> _result = new Stack<object>();
            ExpressionNotationToken _token;

            // While there are input tokens left
            foreach (object obj in m_Output)
            {
                // Read the next token from input.
                _token = (ExpressionNotationToken)obj;
                _result = _token.Evaluate(_result);
            }

            // If there is only one value in the stack
            if (_result.Count == 1)
            {
                // That value is the result of the calculation.
                return _result.Pop();
            }
            else
            {
                // If there are more values in the stack
                // (Error) The user input too many values.
                throw new Exception("Evaluation error!");
            }
        }

        private string generateTransitionExpression(string expression)
        {
            string _RXTokenSep = @"[" + TOKEN_SEP + "]";

            string _sBuffer = expression;

            // Capture Boolean (1 or 0)
            _sBuffer = Regex.Replace(_sBuffer, @"(?<boolean>(" + ExpressionTokenParser.RX_Boolean + @"))", TOKEN_SEP + "${boolean}" + TOKEN_SEP, RegexOptions.IgnoreCase);

            // Capture Numbers (10; 10.56; etc...)
            _sBuffer = Regex.Replace(_sBuffer, @"(?<number>(" + ExpressionTokenParser.RX_Number + @"))", TOKEN_SEP + "${number}" + TOKEN_SEP, RegexOptions.IgnoreCase);

            // Capture String
            _sBuffer = Regex.Replace(_sBuffer, @"(?<string>(" + ExpressionTokenParser.RX_String + "))", TOKEN_SEP + "${string}" + TOKEN_SEP, RegexOptions.IgnoreCase);

            // Capture DateTime (yyyy-MM-dd HH:mm:ss | yyyy-MM-dd)
            _sBuffer = Regex.Replace(_sBuffer, @"(?<dateTime>(" + ExpressionTokenParser.RX_DateTime + "))", TOKEN_SEP + "${dateTime}" + TOKEN_SEP, RegexOptions.IgnoreCase);

            // Capture Array (45;89;63)
            _sBuffer = Regex.Replace(_sBuffer, @"(?<array>(" + ExpressionTokenParser.RX_Array + "))", TOKEN_SEP + "${array}" + TOKEN_SEP, RegexOptions.IgnoreCase);

            // captures these symbols: [+] [-] [*] [/] [=] [<>] [>] [>=] [<] [<=] [AND] [OR] ( )
            _sBuffer = Regex.Replace(_sBuffer, @"(?<ops>(" + ExpressionTokenParser.RX_MathOperatorsMissUnary + @")|(" + ExpressionTokenParser.RX_CompOperators + @")|(" + ExpressionTokenParser.RX_LogicalOperators + @")|[()])", TOKEN_SEP + "${ops}" + TOKEN_SEP, RegexOptions.IgnoreCase);

            // captures alphabets. Currently captures the two math constants PI and E,
            // and the 3 basic trigonometry functions, sine, cosine and tangent
            _sBuffer = Regex.Replace(_sBuffer, @"(?<var>(" + ExpressionTokenParser.RX_Variable + @"))", TOKEN_SEP + "${var}" + TOKEN_SEP, RegexOptions.IgnoreCase);

            // trims up consecutive spaces and replace it with just one space
            _sBuffer = _sBuffer.Trim();


            return _sBuffer;
        }

        private object getVariable(string TokenValue)
        {
            object result = null;

            if (m_dictVariableValues.ContainsKey(TokenValue))
            {
                result = m_dictVariableValues[TokenValue];
            }
            else
            {
                throw new Exception("Variable values does not contain token");
            }

            return result;
        }

        private void PushToken()
        {
            // tokenise it!
            string[] _saParsed = TransitionExpression
                                        .Split(TOKEN_SEP.ToCharArray())
                                        .Where(a => !String.IsNullOrWhiteSpace(a)).ToArray();

            ExpressionTokenParser.GetVariableValue = getVariable;
            ExpressionTokenParser.DictVariableValues = m_dictVariableValues;

            IExpressionNotationToken _token;
            foreach (string _strParsedValue in _saParsed)
            {
                _token = ExpressionTokenParser.GetExpressionNotationToken(_strParsedValue);

                if (_token != null)
                {
                    _token.Push(m_Output, m_Ops);
                }
            }


            // When there are no more tokens to read:
            IExpressionNotationToken _opstoken;

            // While there are still operator tokens in the stack:
            while (m_Ops.Count != 0)
            {
                _opstoken = (ExpressionNotationToken)m_Ops.Pop();
                // If the operator token on the top of the stack is a parenthesis
                if (_opstoken is LeftParenthesis)
                {
                    // then there are mismatched parenthesis.
                    throw new Exception("Unbalanced parenthesis!");
                }
                else
                {
                    // Pop the operator onto the output queue.
                    m_Output.Enqueue(_opstoken);
                }
            }
        }

        #endregion Methods
    }
}
