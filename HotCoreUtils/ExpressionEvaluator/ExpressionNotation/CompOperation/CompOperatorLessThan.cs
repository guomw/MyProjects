using System;

namespace HotCoreUtils.ExpressionEvaluator.ExpressionNotation.CompOperation
{
    internal class CompOperatorLessThan : CompOperator<object, bool>
    {
        #region Methods

        protected override bool evaluateOperation(object oper1, object oper2)
        {
            bool _result;

            Type _operType = oper1.GetType();

            if (_operType != oper2.GetType())
            {
                throw new InvalidOperationException("Invalid operation between different types.");
            }

            if (_operType == typeof(decimal))
            {
                _result = (Convert.ToDecimal(oper1)) < (Convert.ToDecimal(oper2));
            }
            else if (_operType == typeof(DateTime))
            {
                _result = (Convert.ToDateTime(oper1)) < (Convert.ToDateTime(oper2));
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
