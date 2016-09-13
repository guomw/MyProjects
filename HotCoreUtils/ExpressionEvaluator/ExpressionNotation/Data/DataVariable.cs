using System;
using System.Collections.Generic;

namespace HotCoreUtils.ExpressionEvaluator.ExpressionNotation.Data
{
    internal class DataVariable : ExpressionNotationToken
    {
        #region Properties

        public Func<string, object> GetVariableValue { get; set; }

        #endregion Properties


        #region Methods

        public override void Push(Queue<object> output, Stack<object> ops)
        {
            output.Enqueue(this);
        }

        public override Stack<object> Evaluate(Stack<object> result)
        {
            result.Push(GetVariableValue(this.TokenValue));

            return result;
        }

        #endregion Methods
    }
}
