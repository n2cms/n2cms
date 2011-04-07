using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Engine;

namespace N2.Definitions.Static
{
	[Service(StaticAccessor = "Instance")]
	public class DefinitionMap
	{
		// static

		static DefinitionMap()
		{
			N2.Engine.Singleton<DefinitionMap>.Instance = new DefinitionMap();
		}

		public static DefinitionMap Instance 
		{
			get { return N2.Engine.Singleton<DefinitionMap>.Instance; }
		}

		// instance

		private Dictionary<string, ItemDefinition> definitions = new Dictionary<string, ItemDefinition>();

		public ItemDefinition GetOrCreateDefinition(Type contentType)
		{
			return GetOrCreateDefinition(contentType, null);
		}

		public ItemDefinition GetOrCreateDefinition(Type contentType, string templateName)
		{
			if (contentType == null) throw new ArgumentNullException("contentType");

			return GetDefinition(contentType, templateName)
				?? CreateDefinition(contentType, templateName);
		}

		public ItemDefinition GetOrCreateDefinition(ContentItem item)
		{
			return GetOrCreateDefinition(item.GetContentType(), item["TemplateName"] as string);
		}

		private ItemDefinition GetDefinition(Type contentType, string templateName)
		{
			string key = contentType.FullName + templateName;
			ItemDefinition definition;
			if (definitions.TryGetValue(key, out definition))
				return definition;

			return null;
		}

		private ItemDefinition CreateDefinition(Type contentType, string templateName)
		{
			ItemDefinition definition = GetDefinition(contentType, null);
			if (definition != null)
				definition = definition.Clone();
			else
				definition = new ItemDefinition(contentType);

			definition.Template = templateName;
			definition.Initialize(contentType);

			SetDefinition(contentType, templateName, definition);

			return definition;
		}

		public IEnumerable<ItemDefinition> GetDefinitions()
		{
			return definitions.Values.ToList();
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
