using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HotCoreUtils.ExpressionEvaluator.ExpressionNotation.Data;

namespace HotCoreUtils.ExpressionEvaluator.ExpressionNotation
{
    internal class ExpressionNotationSingleOperator<T, TResult> : ExpressionNotationToken
    {
        #region Methods

        public override Stack<object> Evaluate(Stack<object> result)
        {
            // NOTE: n is 1 in this case
            // If there are fewer than n values on the stack
            if (result.Count < 1)
            {
                throw new Exception("Evaluation error!");

            }

            T _oper1 = default(T);

            // So, pop the top n values from the stack.        
            _oper1 = (T)(object)result.Pop();

            // Evaluate the function, with the values as arguments.
            object _oprResult = this.evaluateOperation(_oper1);

            // Push the returned results, if any, back onto the stack.
            result.Push(_oprResult);

            return result;
        }

        protected virtual TResult evaluateOperation(T oper1)
        {
            throw new NotImplementedException();
        }
        
        #endregion Methods
    }
}
