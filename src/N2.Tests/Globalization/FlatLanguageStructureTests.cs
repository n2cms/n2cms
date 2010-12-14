using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using N2.Web;

namespace N2.Tests.Globalization
{
	[TestFixture, Category("Integration")]
	public class FlatLanguageStructureTests : GlobalizationTests
	{
		protected override void CreatePageStructure()
		{
			root = engine.Definitions.CreateInstance<Items.TranslatedPage>(null);

			english = engine.Definitions.CreateInstance<Items.LanguageRoot>(root);
			english.LanguageCode = "en-GB";
			english.Name = english.Title = "english";
            english.AddTo(root);

			swedish = engine.Definitions.CreateInstance<Items.LanguageRoot>(root);
			swedish.LanguageCode = "sv-SE";
			swedish.Name = swedish.Title = "swedish";
            swedish.AddTo(root);

			italian = engine.Definitions.CreateInstance<Items.LanguageRoot>(root);
			italian.LanguageCode = "it-IT";
			italian.Name = italian.Title = "italian";
            italian.AddTo(root);

            engine.Persister.Save(root);

			engine.Resolve<IHost>().DefaultSite.RootItemID = root.ID;
			engine.Resolve<IHost>().DefaultSite.StartPageID = root.ID;
		}

	}
}
