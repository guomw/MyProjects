using System;
using System.Linq;

namespace HotCoreUtils.ExpressionEvaluator.ExpressionNotation.CompOperation
{
    internal class CompOperatorNotEquals : CompOperator<object, object>
    {
        #region Methods

        protected override object evaluateOperation(object oper1, object oper2)
        {
            object _result = null;

            Type _operType = oper1.GetType();

            if (_operType != oper2.GetType())
            {
                throw new InvalidOperationException("Invalid operation between different types.");
            }

            if (_operType == typeof(bool))
            {
                _result = (Convert.ToBoolean(oper1)) != (Convert.ToBoolean(oper2));
            }
            else if (_operType == typeof(decimal))
            {
                _result = (Convert.ToDecimal(oper1)) != (Convert.ToDecimal(oper2));
            }
            else if (_operType == typeof(string))
            {
                _result = (oper1.ToString()) != (oper2.ToString());
            }
            else if (_operType == typeof(DateTime))
            {
                _result = (Convert.ToDateTime(oper1)) != (Convert.ToDateTime(oper2));
            }
            else if (_operType == typeof(decimal[]))
            {
                decimal[] _array1 = (oper1 as decimal[]);
                decimal[] _array2 = (oper2 as decimal[]);

                _result = _array1.Except(_array2).Count() > 0 ||
                    _array2.Except(_array1).Count() > 0;
            }
            else if (_operType == typeof(string[]))
            {
                string[] _array1 = (oper1 as string[]);
                string[] _array2 = (oper2 as string[]);

                _result = _array1.Except(_array2).Count() > 0 ||
                    _array2.Except(_array1).Count() > 0;
            }
            else if (_operType == typeof(DateTime[]))
            {
                DateTime[] _array1 = (oper1 as DateTime[]);
                DateTime[] _array2 = (oper2 as DateTime[]);

                _result = _array1.Except(_array2).Count() > 0 ||
                    _array2.Except(_array1).Count() > 0;
            }
            else
            {
                throw new InvalidOperationException("Invalid operation between types.");
            }

            return _result;
        }

        #endregion Methods
    }
}
