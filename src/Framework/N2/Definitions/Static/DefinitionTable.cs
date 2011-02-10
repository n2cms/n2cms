using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Definitions.Static
{
	public class DefinitionTable
	{
		// static

		static DefinitionTable()
		{
			N2.Engine.Singleton<DefinitionTable>.Instance = new DefinitionTable();
		}

		public static DefinitionTable Instance 
		{
			get { return N2.Engine.Singleton<DefinitionTable>.Instance; }
		}

		// instance

		private Dictionary<string, ItemDefinition> definitions = new Dictionary<string, ItemDefinition>();

		public ItemDefinition GetDefinition(Type contentType)
		{
			return GetDefinition(contentType, null);
		}

		public ItemDefinition GetDefinition(Type contentType, string templateName)
		{
			if (contentType == null) throw new ArgumentNullException("contentType");

			string key = contentType.FullName + templateName;

			ItemDefinition definition;
			if (definitions.TryGetValue(key, out definition))
				return definition;

			definition = new ItemDefinition(contentType);
			definition.Template = templateName;
			definition.Initialize(contentType);

			var temp = new Dictionary<string, ItemDefinition>(definitions);
			temp[key] = definition;
			definitions = temp;

			return definition;
		}

		public void SetDefinition(Type contentType, string templateName, ItemDefinition definition)
		{
			if (contentType == null) throw new ArgumentNullException("contentType");

			string key = contentType.FullName + templateName;

			var temp = new Dictionary<string, ItemDefinition>(definitions);
			if (definition != null)
				temp[key] = definition;
			else if (definitions.ContainsKey(key))
				temp.Remove(key);
			definitions = temp;
		}

		public void Clear()
		{
			definitions = new Dictionary<string, ItemDefinition>();
		}
	}
}
