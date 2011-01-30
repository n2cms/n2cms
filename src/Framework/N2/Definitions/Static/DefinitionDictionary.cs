using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Definitions.Static
{
	public class DefinitionDictionary
	{
		// static

		static DefinitionDictionary()
		{
			N2.Engine.Singleton<DefinitionDictionary>.Instance = new DefinitionDictionary();
		}

		public static DefinitionDictionary Instance 
		{
			get { return N2.Engine.Singleton<DefinitionDictionary>.Instance; }
		}

		// instance

		private Dictionary<string, ItemDefinition> definitions = new Dictionary<string, ItemDefinition>();

		public ItemDefinition GetDefinition(Type contentType)
		{
			return GetDefinition(contentType, "");
		}

		public ItemDefinition GetDefinition(Type contentType, string templateName)
		{
			if (contentType == null) throw new ArgumentNullException("contentType");

			string key = contentType.FullName;
			if (!string.IsNullOrEmpty(templateName))
				key += templateName;

			ItemDefinition definition;
			if (definitions.TryGetValue(key, out definition))
				return definition;

			definition = new ItemDefinition(contentType);
			definition.Initialize(contentType);

			var temp = new Dictionary<string, ItemDefinition>(definitions);
			temp[key] = definition;
			definitions = temp;

			return definition;
		}
	}
}
