using System;
using System.Linq;

namespace N2.Definitions.Runtime
{
	public class EditableBuilder<T> : Builder<T> where T : IEditable
	{
		string PropertyName { get; set; }
		
		public EditableBuilder(string propertyName, ContentRegistration re)
			: base(propertyName, re)
		{
			this.PropertyName = propertyName;
			this.Registration = re;
		}



		private T Current
		{
			get { return Registration.Definition.NamedOperators.OfType<T>().First(no => no.Name == PropertyName); }
		}

		new public EditableBuilder<T> Configure(Action<T> configurationExpression)
		{
			if (Registration != null && configurationExpression != null)
				Registration.Configure(PropertyName, configurationExpression);

			return this;
		}

		public EditableBuilder<T> Container(string containerName)
		{
			if (Registration != null && containerName != null)
				Current.ContainerName = containerName;

			return this;
		}

		public EditableBuilder<T> Title(string title)
		{
			if (Registration != null && title != null)
				Current.Title = title;

			return this;
		}

		public EditableBuilder<T> SortOrder(int sortOrder)
		{
			if (Registration != null)
				Current.SortOrder = sortOrder;

			return this;
		}

		public EditableBuilder<T> SortOffset(string name, int offset)
		{
			if (Registration != null)
				Current.SortOrder = Current.SortOrder + offset;

			return this;
		}

		public EditableBuilder<T> SortBefore(string name, int offset)
		{
			return SortOffset(name, -1);
		}

		public EditableBuilder<T> SortAfter(string name, int offset)
		{
			return SortOffset(name, 1);
		}
	}
}