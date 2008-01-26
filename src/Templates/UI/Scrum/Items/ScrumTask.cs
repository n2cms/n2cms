using System.Web.UI.WebControls;
using N2.Details;
using N2.Integrity;
using N2.Web.UI;

namespace N2.Templates.Scrum.Items
{
	[Definition("Task", "ScrumTask")]
	[RestrictParents(typeof(TaskContainer))]
	[AllowedZones("Pending", "InProgress", "Done", "SubTask", "Impediments", "Planned", "Deferred")]
	[AvailableZone("Sub task", "SubTask")]
	[WithEditableTitle]
	public class ScrumTask : Templates.Items.AbstractItem
	{
		[EditableTextBox("Story", 100, TextMode = TextBoxMode.MultiLine, Rows = 5)]
		public virtual string Story
		{
			get { return (string)(GetDetail("Story") ?? string.Empty); }
			set { SetDetail("Story", value, string.Empty); }
		}

		[EditableTextBox("Description", 110, TextMode = TextBoxMode.MultiLine, Rows = 10)]
		public virtual string Description
		{
			get { return (string)(GetDetail("Description") ?? string.Empty); }
			set { SetDetail("Description", value, string.Empty); }
		}

		[Editable("Color", typeof(DropDownList), "SelectedValue", 120, DataBind = true)]
		[EditorModifier("DataSource", new string[] {"", "red", "blue", "green", "gray", "olive", "maroon", "purple", "teal"})]
		public virtual string Color
		{
			get { return (string)(GetDetail("Color") ?? string.Empty); }
			set { SetDetail("Color", value, string.Empty); }
		}
		
		[EditableTextBox("Estimated value", 130)]
		public virtual int Value
		{
			get { return (int)(GetDetail("Value") ?? 0); }
			set { SetDetail("Value", value, 0); }
		}

		[EditableTextBox("Esitmated effort", 140)]
		public virtual int Effort
		{
			get { return (int)(GetDetail("Effort") ?? 0); }
			set { SetDetail("Effort", value, 0); }
		}

		public override string TemplateUrl
		{
			get { return "~/Scrum/UI/Task.ascx"; }
		}
	}
}
