using System;
using System.Collections.Generic;

namespace HotCoreUtils.ExpressionEvaluator.ExpressionNotation
{
    public abstract class ExpressionNotationToken : IExpressionNotationToken
    {
        #region Properties

        public string TokenValue { get; set; }

        #endregion Properties

        #region Methods

        public virtual void Push(Queue<object> output, Stack<object> ops)
        {
            throw new NotImplementedException();
        }

        public virtual Stack<object> Evaluate(Stack<object> result)
        {
            throw new NotImplementedException();
        }

        #endregion Methods
    }
}
