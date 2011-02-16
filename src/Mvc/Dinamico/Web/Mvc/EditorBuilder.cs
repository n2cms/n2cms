using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using N2.Definitions.Dynamic;
using N2.Web.Rendering;
using N2.Definitions;
using System.Web.Mvc;

namespace N2.Web.Mvc
{
	public class EditorBuilder<T> : DisplayRenderer where T : IEditable
	{
		DefinitionRegistrationExpression Registration { get; set; }

		public EditorBuilder(RenderingContext context, DefinitionRegistrationExpression re)
			: base(context)
		{
			Registration = re;
		}

		public EditorBuilder(HtmlHelper html, string propertyName, DefinitionRegistrationExpression re)
			: base(html, propertyName)
		{
			this.Registration = re;
		}



		public EditorBuilder<T> Configure(Action<T> configurationExpression)
		{
			if (Registration != null && configurationExpression != null)
				Registration.Configure(Context.PropertyName, configurationExpression);

			return this;
		}

		public EditorBuilder<T> Container(string containerName)
		{
			if (Registration != null && containerName != null)
				((T)Registration.Containables[Context.PropertyName]).ContainerName = containerName;

			return this;
		}

		public EditorBuilder<T> Title(string title)
		{
			if (Registration != null && title != null)
				((T)Registration.Containables[Context.PropertyName]).Title = title;

			return this;
		}

		public EditorBuilder<T> SortOrder(int sortOrder)
		{
			if (Registration != null)
				((T)Registration.Containables[Context.PropertyName]).SortOrder = sortOrder;

			return this;
		}

		public EditorBuilder<T> SortOffset(string name, int offset)
		{
			if (Registration != null)
				((T)Registration.Containables[Context.PropertyName]).SortOrder = ((IContainable)Registration.Containables[Context.PropertyName]).SortOrder + offset;

			return this;
		}

		public EditorBuilder<T> SortBefore(string name, int offset)
		{
			return SortOffset(name, -1);
		}

		public EditorBuilder<T> SortAfter(string name, int offset)
		{
			return SortOffset(name, 1);
		}
	}
}