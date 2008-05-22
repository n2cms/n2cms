using System;
using System.Web.UI.WebControls;
using N2.Details;
using N2.Integrity;
using N2.Templates.Items;

namespace N2.Templates.Scrum.Items
{
	[Definition("Sprint", "ScrumSprint")]
	[RestrictParents(typeof(ScrumProject))]
	[AvailableZone("Pending", "Pending"),
		AvailableZone("In progress", "InProgress"),
		AvailableZone("Done", "Done"), 
		AvailableZone("Impediments", "Impediments")]
	public class ScrumSprint : TaskContainer
	{
		[EditableTextBox("Sprint number", 100)]
		public virtual int Number
		{
			get { return (int)(GetDetail("SprintNumber") ?? 0); }
			set { SetDetail("SprintNumber", value, 0); }
		}

		[EditableTextBox("Sprint goal", 100, TextMode = TextBoxMode.MultiLine)]
		public virtual string SprintGoal
		{
			get { return (string)(GetDetail("SprintGoal") ?? string.Empty); }
			set { SetDetail("SprintGoal", value, string.Empty); }
		}

		[Editable("Start date", typeof(N2.Web.UI.WebControls.DatePicker), "SelectedDate", 120)]
		public virtual DateTime? StartDate
		{
			get { return (DateTime?)GetDetail("StartDate"); }
			set { SetDetail("StartDate", value); }
		}

		[Editable("Stop date", typeof(N2.Web.UI.WebControls.DatePicker), "SelectedDate", 120)]
		public virtual DateTime? StopDate
		{
			get { return (DateTime?)GetDetail("StopDate"); }
			set { SetDetail("StopDate", value); }
		}

		public virtual int  EsitimatedValue
		{
			get
			{
				int value = 0;
				foreach(ScrumTask task in GetChildren())
				{
					value += task.Value;
				}
				return value;
			}
		}

		public virtual int EsitimatedEffort
		{
			get
			{
				int effort = 0;
				foreach (ScrumTask task in GetChildren())
				{
					effort += task.Effort;
				}
				return effort;
			}
		}

		public override string TemplateUrl
		{
			get { return "~/Scrum/UI/Sprint.aspx"; }
		}

		public virtual ScrumProject Project
		{
			get { return (ScrumProject) Parent; }
		}
	}
}
