using System;
using System.Collections.Generic;
using HotCoreUtils.ExpressionEvaluator.ExpressionNotation.MathOperation;
using HotCoreUtils.ExpressionEvaluator.ExpressionNotation.LogicalOperation;

namespace HotCoreUtils.ExpressionEvaluator.ExpressionNotation.CompOperation
{
    internal abstract class CompOperator<T, TResult> : ExpressionNotationDoubleOperator<T, T, TResult>, ICompOperator
    {
        #region Methods

        public override void Push(Queue<object> output, Stack<object> ops)
        {
            IExpressionNotationToken _opstoken;

            if (ops.Count > 0)
            {
                _opstoken = (ExpressionNotationToken)ops.Peek();
                // while there is an operator, o2, at the top of the stack
                while (_opstoken is IMathOperator ||
                        _opstoken is ILogicalOperator ||
                        _opstoken is ICompOperator)
                {
                    // pop o2 off the stack, onto the output queue;
                    output.Enqueue(ops.Pop());
                    if (ops.Count > 0)
                    {
                        _opstoken = (ExpressionNotationToken)ops.Peek();
                    }
                    else
                    {
                        break;
                    }
                }
            }

            // push o1 onto the operator stack.
            ops.Push(this);
        }

        #endregion Methods
    }
}
