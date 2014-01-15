using System.Collections.Generic;

namespace N2.Persistence.Serialization
{
    public class DefinitionNotFoundException : DeserializationException
    {
        readonly Dictionary<string, string> attributes;

        public DefinitionNotFoundException(string message, Dictionary<string, string> attributes)
            :base (message)
        {
            this.attributes = attributes;
        }

        public Dictionary<string, string> Attributes
        {
            get { return attributes; }
        }
    }
}
