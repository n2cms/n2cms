using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Definitions
{
    /// <summary>
    /// Transforms an objects into another shape.
    /// </summary>
    /// <typeparam name="TOut">The type of object to be transformed into.</typeparam>
    public abstract class TransformerBase<TOut>
    {
        public abstract bool IsTransformable(object value);
        public abstract TOut Transform(object value);
    }
}
