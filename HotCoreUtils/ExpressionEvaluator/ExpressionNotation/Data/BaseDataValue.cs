using System.Collections.Generic;

namespace HotCoreUtils.ExpressionEvaluator.ExpressionNotation.Data
{
    internal abstract class BaseDataValue : ExpressionNotationToken
    {
        public override void Push(Queue<object> output, Stack<object> ops)
        {
            output.Enqueue(this);
        }
    }
}
