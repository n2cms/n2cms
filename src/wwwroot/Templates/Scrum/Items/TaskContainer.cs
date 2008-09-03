using N2.Details;
using N2.Integrity;
using N2.Templates.Items;

namespace N2.Templates.Scrum.Items
{
	[WithEditableName("Name", 20, ContainerName = Tabs.Content)]
	public abstract class TaskContainer : AbstractPage
	{
	}
}
