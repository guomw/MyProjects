using System;
using System.Collections.Generic;
using HotCoreUtils.ExpressionEvaluator.ExpressionNotation.Data;

namespace HotCoreUtils.ExpressionEvaluator.ExpressionNotation.FunctionOperation
{
    internal abstract class FunctionOperator<T, TResult> : ExpressionNotationSingleOperator<T, TResult>, IFunctionOperator
    {
        #region Methods

        public override void Push(Queue<object> output, Stack<object> ops)
        {           
            ops.Push(this);
        }

        #endregion Methods
    }
}
