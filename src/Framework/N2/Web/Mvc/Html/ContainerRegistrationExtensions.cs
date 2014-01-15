using N2.Definitions.Runtime;
using N2.Web.UI;

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
