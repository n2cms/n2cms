using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Definitions
{
    /// <summary>
    /// Transforms an objects into another shape.
    /// </summary>
    /// <typeparam name="TIn">The type of object that is to be transformed.</typeparam>
    /// <typeparam name="TOut">The type of object to be transformed into.</typeparam>
    public abstract class AttributeTransformerBase<TIn> : TransformerBase<IUniquelyNamed>
    {
        public override bool IsTransformable(object value)
        {
            return value != null
                && value.GetType() == typeof(TIn);
        }

        public override IUniquelyNamed Transform(object value)
        {
            return Transform((TIn)value);
        }

        public abstract IUniquelyNamed Transform(TIn value);
    }
}
