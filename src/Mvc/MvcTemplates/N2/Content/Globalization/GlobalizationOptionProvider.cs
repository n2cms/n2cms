using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using N2.Engine;
using N2.Edit;
using N2.Definitions;
using N2.Engine.Globalization;
using N2.Security;

namespace N2.Edit.Globalization
{
	[Service]
	public class GlobalizationOptionProvider : IProvider<ToolbarOption>
	{
		ILanguageGateway languages;
		IEditUrlManager editUrlManager;

		public GlobalizationOptionProvider(ILanguageGateway languages, IEditUrlManager editUrlManager)
		{
			this.languages = languages;
			this.editUrlManager = editUrlManager;
		}

		#region IProvider<ToolbarOption> Members

		public ToolbarOption Get()
		{
			return GetAll().FirstOrDefault();
		}

		public IEnumerable<ToolbarOption> GetAll()
		{
			return languages.GetAvailableLanguages()
				.Where(l => l is ContentItem)
				.Select((l, i) => new ToolbarOption
				{
					Title = l.LanguageTitle,
					Target = Targets.Preview,
					SortOrder = i,
					Name = l.LanguageCode,
					Url = editUrlManager.GetPreviewUrl((ContentItem)l)
				});
		}

		#endregion
	}
}
