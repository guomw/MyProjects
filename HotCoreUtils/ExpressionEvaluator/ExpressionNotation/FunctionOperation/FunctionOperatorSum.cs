using System;
using System.Linq;
using HotCoreUtils.ExpressionEvaluator.ExpressionNotation.Data;

namespace HotCoreUtils.ExpressionEvaluator.ExpressionNotation.FunctionOperation
{
    internal class FunctionOperatorSum : FunctionOperator<decimal[], decimal>
    {
        #region Methods

        protected override decimal evaluateOperation(decimal[] oper1)
        {
            decimal _result;

            _result = oper1.Sum();

            return _result;
        }

        #endregion Methods
    }
}
