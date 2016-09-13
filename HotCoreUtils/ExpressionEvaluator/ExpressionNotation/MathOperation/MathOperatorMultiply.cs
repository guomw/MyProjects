using System;
using System.Collections.Generic;
using HotCoreUtils.ExpressionEvaluator.ExpressionNotation.LogicalOperation;
using HotCoreUtils.ExpressionEvaluator.ExpressionNotation.CompOperation;

namespace HotCoreUtils.ExpressionEvaluator.ExpressionNotation.MathOperation
{
    internal class MathOperatorMultiply : MathOperator<decimal, decimal>
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
                    if (_opstoken is MathOperatorPlus ||
                        _opstoken is MathOperatorMinus)
                    {
                        break;
                    }
                    else
                    {
                        // Once we're in here, the following algorithm condition is satisfied.
                        // o1 is associative or left-associative and its precedence is less than (lower precedence) or equal to that of o2, or
                        // o1 is right-associative and its precedence is less than (lower precedence) that of o2,

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
            }
            // push o1 onto the operator stack.
            ops.Push(this);
        }

        protected override decimal evaluateOperation(decimal oper1, decimal oper2)
        {
            decimal _result;

            _result = (Convert.ToDecimal(oper1)) * (Convert.ToDecimal(oper2));

            return _result;
        }

        #endregion Methods
    }
}
