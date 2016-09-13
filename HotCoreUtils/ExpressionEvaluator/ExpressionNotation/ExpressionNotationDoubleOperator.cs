using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HotCoreUtils.ExpressionEvaluator.ExpressionNotation
{
    internal class ExpressionNotationDoubleOperator<T1, T2, TResult> : ExpressionNotationToken
    {
        public override Stack<object> Evaluate(Stack<object> result)
        {
            // NOTE: n is 2 in this case
            // If there are fewer than n values on the stack
            if (result.Count < 2)
            {
                throw new Exception("Evaluation error!");
            }

            T1 _oper1 = default(T1);
            T2 _oper2 = default(T2);


            // So, pop the top n values from the stack.
            _oper2 = (T2)(object)result.Pop();
            _oper1 = (T1)(object)result.Pop();

            object _oprResult = this.evaluateOperation(_oper1, _oper2);
            // Evaluate the function, with the values as arguments.
            // Push the returned results, if any, back onto the stack.
            result.Push(_oprResult);

            return result;
        }

        protected virtual TResult evaluateOperation(T1 oper1, T2 oper2)
        {
            throw new NotImplementedException();
        }
    }
}
