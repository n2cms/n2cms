using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Details;
using N2.Tests.Definitions.Items;
using N2.Definitions;

namespace N2.Tests.Definitions
{
    public class TitleTransformer : AttributeTransformerBase<WithEditableTitleAttribute>
    {
        public override IUniquelyNamed Transform(WithEditableTitleAttribute attribute)
        {
            attribute.Title += " Transformed";
            return attribute;
        }
    }
}
