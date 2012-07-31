using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Definitions.Runtime;
using N2.Web.Mvc;
using N2.Templates.Mvc.Areas.Tests.Controllers;
using N2.Templates.Mvc.Models.Pages;

namespace N2.Templates.Mvc.Areas.Tests.Models
{
	public class FluentPage : TestItemBase
	{
		public virtual bool EditableCheckBox { get; set; }
		public virtual string EditableChildren { get; set; }
		public virtual string EditableDate { get; set; }
		public virtual string EditableDefinition { get; set; }
		public virtual Base64FormattingOptions EditableEnum { get; set; }
		public virtual string EditableFileUpload { get; set; }
		public virtual string EditableFolderSelection { get; set; }
		public virtual string EditableFreeTextArea { get; set; }
		public virtual string EditableImage { get; set; }
		public virtual string EditableImageSize { get; set; }
		public virtual string EditableImageUpload { get; set; }
		public virtual TestPart EditableItem { get; set; }
		public virtual string EditableLanguagesDropDown { get; set; }
		public virtual string EditableLink { get; set; }
		public virtual string EditableMediaUpload { get; set; }
		public virtual string EditableMetaTag { get; set; }
		public virtual int EditableNumber { get; set; }
		public virtual string EditableSummary { get; set; }
		public virtual IEnumerable<string> EditableTags { get; set; }
		public virtual string EditableText { get; set; }
		public virtual string DefaultValue { get; set; }
		public virtual string EditableThemeSelection { get; set; }
		public virtual string EditableUrl { get; set; }
		public virtual string EditableUserControl { get; set; }
		public virtual IEnumerable<ContentItem> EditableMultipleItemSelection { get; set; }
		public virtual ContentItem EditableItemSelection { get; set; }
	}

	[Registration]
	public class FluentPageRegistration : N2.Definitions.Runtime.FluentRegisterer<FluentPage>
	{
		public override void RegisterDefinition(Definitions.Runtime.IContentRegistration<FluentPage> register)
		{
			register.ControlledBy<TestContentController>();

			register.Definition.Title = "Fluently registered page";
			register.Definition.Description = "Release compile template project to remove this item";

			register.RestrictParents(typeof(StartPage));

			register.Icon("{IconsUrl}/star.png");

			using (register.Tab("Content").Begin())
			{
				register.Title();

				register.On(tp => tp.EditableFreeTextArea).FreeText();
				register.On(tp => tp.EditableNumber).Number();
			}

			using (register.Sidebar("Settings").Begin())
			{
				register.PublishedRange();
				register.Name();
			}

			using (register.Tab("Advanced").Begin())
			{
				register.On<string>("NonExistant").Text();
			}
		}
	}
}
