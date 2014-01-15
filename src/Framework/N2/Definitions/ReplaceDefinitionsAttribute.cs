using System;

namespace N2.Definitions
{
    [Obsolete("Renamed to RemoveDefinitionsAttribute.")]
    public class ReplaceDefinitionsAttribute : RemoveDefinitionsAttribute
    {
        public ReplaceDefinitionsAttribute(params Type[] replacedDefinitions)
            : base(DefinitionReplacementMode.Disable, replacedDefinitions)
        {
        }

        public ReplaceDefinitionsAttribute(Type replacedDefinition)
            : base(DefinitionReplacementMode.Disable, replacedDefinition)
        {
        }
    }
}
