using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using N2.Details;
using N2.Web.UI;
using N2.Definitions.Runtime;

namespace N2.Web.Mvc.Html
{
	public static class ContainerRegistrationExtensions
	{
		public static Builder<TabContainerAttribute> Tab(this IRegistration display, string containerName, string tabText = null)
		{
			return display.Register(new TabContainerAttribute(containerName, tabText ?? containerName, 0));
		}

		public static Builder<FieldSetContainerAttribute> FieldSet(this IRegistration display, string containerName, string legend = null)
		{
			return display.Register(new FieldSetContainerAttribute(containerName, legend ?? containerName, 0));
		}

		public static Builder<ExpandableContainerAttribute> ExpandableContainer(this IRegistration display, string containerName, string legend)
		{
			return display.Register(new ExpandableContainerAttribute(containerName, legend ?? containerName, 0));
		}
	}

}