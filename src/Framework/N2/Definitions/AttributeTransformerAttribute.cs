using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Engine;

namespace N2.Definitions
{
    /// <summary>
    /// Marks a class as a transformer for editable attributes.
    /// </summary>
    /// <example>
    /// [AttributeTransformer]
    /// public class ExampleTransformer : AttributeTransformerBase<EditableTextAttribute>
    /// {
    ///     public override IUniquelyNamed Transform(EditableTextAttribute value)
    ///     {
    ///         return new EditableFreeTextAreaAttribute { Name = value.Name };
    ///     }
    /// }
    /// </example>
    public class AttributeTransformerAttribute : ServiceAttribute
    {
        public AttributeTransformerAttribute()
            : base(typeof(TransformerBase<IUniquelyNamed>))
        {
        }
    }
}
