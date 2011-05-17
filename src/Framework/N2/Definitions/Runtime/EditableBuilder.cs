using System;

namespace N2.Definitions.Runtime
{
	public class EditableBuilder<T> : Builder<T> where T : IEditable
	{
		ContentRegistration Registration { get; set; }
		string PropertyName { get; set; }
		
		public EditableBuilder(string propertyName, ContentRegistration re)
			: base(propertyName, re)
		{
			this.PropertyName = propertyName;
			this.Registration = re;
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
				((T)Registration.Containables[PropertyName]).ContainerName = containerName;

			return this;
		}

		public EditableBuilder<T> Title(string title)
		{
			if (Registration != null && title != null)
				((T)Registration.Containables[PropertyName]).Title = title;

			return this;
		}

		public EditableBuilder<T> SortOrder(int sortOrder)
		{
			if (Registration != null)
				((T)Registration.Containables[PropertyName]).SortOrder = sortOrder;

			return this;
		}

		public EditableBuilder<T> SortOffset(string name, int offset)
		{
			if (Registration != null)
				((T)Registration.Containables[PropertyName]).SortOrder = ((IContainable)Registration.Containables[PropertyName]).SortOrder + offset;

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