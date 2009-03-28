using System.Collections.Generic;
using System.Configuration;
using System.Xml;

namespace N2.Configuration
{
	public abstract class LazyRemovableCollection<T> : ConfigurationElementCollection
		where T : NamedElement, new()
	{
		protected override ConfigurationElement CreateNewElement()
		{
			return new T();
		}

		protected override object GetElementKey(ConfigurationElement element)
		{
			return ((T)element).Name;
		}

		List<T> removedElements = new List<T>();

		public IEnumerable<T> RemovedElements
		{
			get { return removedElements.AsReadOnly(); }
			set { removedElements = new List<T>(value ?? new T[0]); }
		}

		public IEnumerable<T> AddedElements
		{
			get
			{
				foreach (T element in this)
					yield return element;
			}
			set
			{
				BaseClear();
				if(value == null) return;

				foreach (T element in value)
					BaseAdd(element);
			}
		}

		protected override bool OnDeserializeUnrecognizedElement(string elementName, System.Xml.XmlReader reader)
		{
			if (elementName == "remove")
			{
				T element = new T();
				element.Name = reader.GetAttribute("name");

				OnDeserializeElement(element, reader);
				
				removedElements.Add(element);
				return true;
			}
			return base.OnDeserializeUnrecognizedElement(elementName, reader);
		}

		protected abstract void OnDeserializeElement(T element, XmlReader reader);
	}
}