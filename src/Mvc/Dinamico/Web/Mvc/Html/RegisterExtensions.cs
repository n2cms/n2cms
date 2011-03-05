using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using N2.Definitions.Runtime;
using N2.Details;
using System.Web.Mvc;
using N2.Definitions;

namespace N2.Web.Mvc.Html
{
	public static class RegisterExtensions
	{
		public static ContentRegistration Define<T>(this ContentHelper<T> content, Action<ContentRegistration> registration = null) where T : class
		{
			var re = RegistrationExtensions.GetRegistrationExpression(content.Html);
			if (re != null)
			{
				re.GlobalSortOffset = 0;
				if (typeof(ContentItem).IsAssignableFrom(typeof(T)) && re.ContentType == null)
				{
					re.ContentType = typeof(T);
					re.Title = typeof(T).Name;
				}
				re.IsDefined = true;
				if(registration != null)
					registration(re);
			}
			return re;
		}

		public static ContentRegistration AppendDefinition<T>(this ContentHelper<T> content, Action<ContentRegistration> registration = null) where T : class
		{
			var re = RegistrationExtensions.GetRegistrationExpression(content.Html);
			if (re != null)
			{
				re.GlobalSortOffset = 0;
				registration(re);
			}
			return re;
		}

		public static ContentRegistration PrependDefinition<T>(this ContentHelper<T> content, Action<ContentRegistration> registration = null) where T : class
		{
			var re = RegistrationExtensions.GetRegistrationExpression(content.Html);
			if (re != null)
			{
				re.GlobalSortOffset = -1000;
				registration(re);
			}
			return re;
		}

		//// containables

		public static ContentRegistration Tab(this ContentRegistration re, string containerName, string tabText, Action<ContentRegistration> registration, int? sortOrder = null)
		{
			if (re == null) return re;

			re.Add(new N2.Web.UI.TabContainerAttribute(containerName, tabText, re.NextSortOrder(sortOrder)));

			string previousContainerName = re.ContainerName;
			re.ContainerName = containerName;
			if (registration != null)
			{
				registration(re);
				re.ContainerName = previousContainerName;
			}

			return re;
		}

		public static ContentRegistration FieldSet(this ContentRegistration re, string containerName, string legend, Action<ContentRegistration> registration, int? sortOrder = null)
		{
			if (re == null) return re;

			string previousContainerName = re.ContainerName;
			re.ContainerName = containerName;
			re.Add(new N2.Web.UI.FieldSetContainerAttribute(containerName, legend, re.NextSortOrder(sortOrder)));

			if (registration != null)
			{
				registration(re);
				re.ContainerName = previousContainerName;
			}

			return re;
		}

		public static ContentRegistration Container(this ContentRegistration re, string name, Action<ContentRegistration> registration)
		{
			if (re == null) return re;

			string previousContainerName = re.ContainerName;
			re.ContainerName = name;
			if (registration != null)
			{
				registration(re);
				re.ContainerName = previousContainerName;
			}

			return re;
		}

		public static ContentRegistration EndContainer(this ContentRegistration re)
		{
			if (re == null) return re;

			re.ContainerName = null;
			return re;
		}

		public static DisplayRenderer<T> Display<T>(this EditableBuilder<T> builder) where T : IEditable
		{
			return builder as DisplayRenderer<T>;
		}
	}

}