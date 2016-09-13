using System.Collections.Generic;
using HotCoreUtils.ExpressionEvaluator.ExpressionNotation.FunctionOperation;
using System;

namespace HotCoreUtils.ExpressionEvaluator.ExpressionNotation.Typographic
{
    internal class RightParenthesis : Parenthesis
    {
        #region Methods

        public override void Push(Queue<object> output, Stack<object> ops)
        {
            IExpressionNotationToken _opstoken;

            if (ops.Count > 0)
            {
                _opstoken = (ExpressionNotationToken)ops.Peek();
                // Until the token at the top of the stack is a left parenthesis
                while (!(_opstoken is LeftParenthesis))
                {
                    // pop operators off the stack onto the output queue
                    output.Enqueue(ops.Pop());
                    if (ops.Count > 0)
                    {
                        _opstoken = (ExpressionNotationToken)ops.Peek();
                    }
                    else
                    {
                        // If the stack runs out without finding a left parenthesis,
                        // then there are mismatched parentheses.
                        throw new Exception("Unbalanced parenthesis!");
                    }

                }
                // Pop the left parenthesis from the stack, but not onto the output queue.
                ops.Pop();
            }

            if (ops.Count > 0)
            {
                _opstoken = (ExpressionNotationToken)ops.Peek();
                // If the token at the top of the stack is a function token
                if (_opstoken is IFunctionOperator)
                {
                    // pop it and onto the output queue.
                    output.Enqueue(ops.Pop());
                }
            }
        }

        #endregion Methods
    }
}
