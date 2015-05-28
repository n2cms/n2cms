using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Definitions
{
    public class DefinitionEventArgs : EventArgs
    {
        public ContentItem AffectedItem { get; set; }

        public ItemDefinition Definition { get; set; }

        public Type ContentType { get; set; }

		public string Discriminator { get; set; }
	}
}
