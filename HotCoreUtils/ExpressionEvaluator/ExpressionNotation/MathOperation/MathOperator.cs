using System;
using System.Collections.Generic;

namespace HotCoreUtils.ExpressionEvaluator.ExpressionNotation.MathOperation
{
    internal abstract class MathOperator<T, TResult> : ExpressionNotationDoubleOperator<T, T, TResult>, IMathOperator
    {
    }
}
