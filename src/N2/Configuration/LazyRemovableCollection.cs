using System.Collections.Generic;
using System.Configuration;
using System.Xml;

namespace N2.Configuration
{
	/// <summary>
	/// Allws to "remove" items not in the collection. The reader of the configuration must 
	/// implement "remove" semantics reading from the RemovedElements collection.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public abstract class LazyRemovableCollection<T> : ConfigurationElementCollection
		where T : NamedElement, new()
	{
		List<T> removedElements = new List<T>();
		bool clear;

		/// <summary>Elements that were "removed".</summary>
		public IEnumerable<T> RemovedElements
		{
			get { return removedElements.AsReadOnly(); }
			set { removedElements = new List<T>(value ?? new T[0]); }
		}

		/// <summary>Elements that were added.</summary>
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

		public bool Clear
		{
			get { return clear; }
			set { clear = value; }
		}


		/// <summary>Adds an element to the collection.</summary>
		/// <param name="element">The element to add.</param>
		public void Add(T element)
		{
			BaseAdd(element);
		}

		/// <summary>"Removes" an element from the collection.</summary>
		/// <param name="element">The element to "remove".</param>
		public void Remove(T element)
		{
			removedElements.Add(element);
		}



		protected override ConfigurationElement CreateNewElement()
		{
			return new T();
		}

		protected override object GetElementKey(ConfigurationElement element)
		{
			return ((T)element).Name;
		}

		protected override bool OnDeserializeUnrecognizedElement(string elementName, XmlReader reader)
		{
			if (elementName == "remove")
			{
				T element = new T();
				element.Name = reader.GetAttribute("name");

				OnDeserializeRemoveElement(element, reader);
				
				removedElements.Add(element);
				return true;
			}
			if(elementName == "clear")
			{
				clear = true;
				return true;
			}
			return base.OnDeserializeUnrecognizedElement(elementName, reader);
		}

		protected virtual void OnDeserializeRemoveElement(T element, XmlReader reader)
		{
		}
	}
}