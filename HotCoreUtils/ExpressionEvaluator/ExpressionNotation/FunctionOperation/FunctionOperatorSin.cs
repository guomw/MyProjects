using System;
using HotCoreUtils.ExpressionEvaluator.ExpressionNotation.Data;

namespace HotCoreUtils.ExpressionEvaluator.ExpressionNotation.FunctionOperation
{
    internal class FunctionOperatorSin : FunctionOperator<decimal, decimal>
    {
        #region Methods

        protected override decimal evaluateOperation(decimal oper1)
        {
            decimal _result;

            _result = Convert.ToDecimal(Math.Sin(Convert.ToDouble(oper1)));

            return _result;
        }

        #endregion Methods
    }
}
