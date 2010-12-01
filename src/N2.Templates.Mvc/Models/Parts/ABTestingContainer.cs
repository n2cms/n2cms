using System;

namespace N2.Templates.Mvc.Models.Parts
{
	[PartDefinition("A/B testing container",
		IconUrl = "{ManagementUrl}/Resources/icons/arrow_divide.png",
		Description = "Randomly displays the contents of one of the nested zones.",
		SortOrder = 175)]
	[N2.Web.UI.FieldSetContainer("Buckets", "A/B test buckets", 100)]
	public class ABTestingContainer : PartBase
	{
		[N2.Details.EditableTextBox("% bucket 1", 100, DefaultValue = 50, Required = true, ContainerName = "Buckets")]
		public virtual int Zone1Percentage { get; set; }

		[N2.Details.EditableTextBox("% bucket 2", 101, DefaultValue = 50, Required = true, ContainerName = "Buckets")]
		public virtual int Zone2Percentage { get; set; }

		[N2.Details.EditableTextBox("% bucket 3", 102, DefaultValue = 0, Required = true, ContainerName = "Buckets")]
		public virtual int Zone3Percentage { get; set; }
	}
}