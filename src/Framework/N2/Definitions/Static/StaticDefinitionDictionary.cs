using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Definitions.Static
{
	public class StaticDefinitionDictionary
	{
		// static

		static StaticDefinitionDictionary()
		{
			N2.Engine.Singleton<StaticDefinitionDictionary>.Instance = new StaticDefinitionDictionary();
		}

		public static StaticDefinitionDictionary Instance 
		{
			get { return N2.Engine.Singleton<StaticDefinitionDictionary>.Instance; }
		}

		// instance

		private Dictionary<Type, ItemDefinition> definitions = new Dictionary<Type, ItemDefinition>();

		public ItemDefinition GetDefinition(Type contentType)
		{
			ItemDefinition definition;
			if (definitions.TryGetValue(contentType, out definition))
				return definition;

			definition = new ItemDefinition(contentType);
			definition.Initialize(contentType);

			var clone = new Dictionary<Type, ItemDefinition>(definitions);
			clone[contentType] = definition;
			definitions = clone;

			return definition;
		}
	}
}
