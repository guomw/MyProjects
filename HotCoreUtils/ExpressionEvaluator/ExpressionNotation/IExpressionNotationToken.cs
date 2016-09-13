using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HotCoreUtils.ExpressionEvaluator.ExpressionNotation
{
    internal interface IExpressionNotationToken
    {
        string TokenValue { get; set; }

        Stack<object> Evaluate(Stack<object> result);

        void Push(Queue<object> output, Stack<object> ops);
    }
}
