using System.Collections.Generic;
using N2.Collections;
using N2.Details;
using N2.Integrity;
using N2.Templates.Items;
using N2.Web.UI;

namespace N2.Templates.Scrum.Items
{
	[Definition("Scrum Project", "ScrumProject", SortOrder = 1000)]
	[WithEditablePublishedRange("Published Between", 30, ContainerName = Tabs.Advanced, BetweenText = " and ")]
	[AvailableZone("Deferred", "Deferred")]
	[AvailableZone("Planned", "Planned")]
	[TabPanel(Tabs.Advanced, "Advanced", 100)]
	[RestrictParents(typeof(IStructuralPage))]
	public class ScrumProject : TaskContainer
	{
		public override void AddTo(ContentItem newParent)
		{
			base.AddTo(newParent);

			//CreateProductBacklog();

			CreateSprints();
		}

		private void CreateSprints()
		{
			IList<ScrumSprint> sprints = Sprints;
			if (SprintCount > sprints.Count)
			{
				for (int i = sprints.Count; i < SprintCount; ++i)
				{
					ScrumSprint sprint = new ScrumSprint();
					sprint.Number = i + 1;
					string name = "Sprint " + sprint.Number;
					sprint.Title = name;
					sprint.Name = name;
					sprint.AddTo(this);
				}
				Utility.UpdateSortOrder(sprints);
			}
		}

		//private void CreateProductBacklog()
		//{
		//    if (Backlog == null)
		//    {
		//        ProductBacklog backlog = new ProductBacklog();
		//        backlog.Title = "Product backlog";
		//        backlog.Name = "Product backlog";
		//        backlog.AddTo(this);
		//        Backlog = backlog;
		//    }
		//}

		//[EditableLink("Product backlog", 100)]
		//public virtual ProductBacklog Backlog
		//{
		//    get { return (ProductBacklog)GetDetail("ProductBacklog"); }
		//    set { SetDetail("ProductBacklog", value); }
		//}

		[EditableLink("Current sprint", 110)]
		public virtual ScrumSprint CurrentSprint
		{
			get { return (ScrumSprint)GetDetail("CurrentSprint"); }
			set { SetDetail("CurrentSprint", value); }
		}

		[EditableTextBox("# of sprints", 120)]
		public virtual int SprintCount
		{
			get { return (int)(GetDetail("SprintCount") ?? 1); }
			set { SetDetail("SprintCount", value, 1); }
		}

		public virtual IList<ScrumSprint> Sprints
		{
			get
			{
				return new ItemList<ScrumSprint>(Children, 
					new NavigationFilter(), new TypeFilter(typeof (ScrumSprint)));
			}
		}

		public override string TemplateUrl
		{
			get { return "~/Scrum/UI/Project.aspx"; }
		}
	}
}
