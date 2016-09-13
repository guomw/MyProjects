using System.Collections.Generic;

namespace HotCoreUtils.ExpressionEvaluator.ExpressionNotation.Data
{
    internal abstract class DataValue<T> : BaseDataValue
    {
        #region Properties

        public T Value { get; set; }

        #endregion Properties


        #region Methods

        public override Stack<object> Evaluate(Stack<object> result)
        {
            result.Push(this.Value);
            return result;
        }

        #endregion Methods
    }
}
