using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Edit.Api
{
	public class TemplateInfo
	{
		public TemplateInfo()
		{
		}

		public TemplateInfo(Definitions.ItemDefinition d)
			: this()
		{
			Title = d.Title;
			Description = d.Description;
			Discriminator = d.Discriminator;
			ToolTip = d.ToolTip;
			IconUrl = d.IconUrl;
			IconClass = d.IconClass;
			TypeName = d.ItemType.Name;
			TemplateKey = d.TemplateKey;
		}

		public TemplateInfo(Definitions.TemplateDefinition d)
			: this(d.Definition)
		{
			Title = d.Title;
			Description = d.Description;
		}

		public string EditUrl { get; set; }

		public string TemplateKey { get; set; }

		public string TypeName { get; set; }

		public string IconClass { get; set; }

		public string IconUrl { get; set; }

		public string ToolTip { get; set; }

		public string Discriminator { get; set; }

		public string Description { get; set; }

		public string Title { get; set; }
	}
}
