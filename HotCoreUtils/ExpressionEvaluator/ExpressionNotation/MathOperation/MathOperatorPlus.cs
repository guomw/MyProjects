using System;
using System.Collections.Generic;
using HotCoreUtils.ExpressionEvaluator.ExpressionNotation.LogicalOperation;
using HotCoreUtils.ExpressionEvaluator.ExpressionNotation.CompOperation;

namespace HotCoreUtils.ExpressionEvaluator.ExpressionNotation.MathOperation
{
    internal class MathOperatorPlus : MathOperator<object, object>
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

        protected override object evaluateOperation(object oper1, object oper2)
        {
            object _result = null;

            Type _resultType = oper1.GetType();

            if (_resultType != oper2.GetType())
            {
                throw new InvalidOperationException("Invalid operation between different types.");
            }

            if (_resultType == typeof(decimal))
            {
                _result = (Convert.ToDecimal(oper1)) + (Convert.ToDecimal(oper2));
            }
            else if (_resultType == typeof(string))
            {
                _result = (oper1.ToString()) + (oper2.ToString());
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
