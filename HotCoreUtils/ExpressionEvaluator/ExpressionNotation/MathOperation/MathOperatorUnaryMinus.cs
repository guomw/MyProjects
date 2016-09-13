using System;
using System.Collections.Generic;

namespace HotCoreUtils.ExpressionEvaluator.ExpressionNotation.MathOperation
{
    internal class MathOperatorUnaryMinus : MathOperator<decimal, decimal>
    {
        #region Methods

        public override void Push(Queue<object> output, Stack<object> ops)
        {
            ops.Push(this);
        }

        protected override decimal evaluateOperation(decimal oper1, decimal oper2)
        {
            decimal _result;

            _result = -Convert.ToDecimal(oper1);

            return _result;
        }

        #endregion Methods
    }
}
