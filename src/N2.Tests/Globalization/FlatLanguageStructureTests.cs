using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using N2.Web;

namespace N2.Tests.Globalization
{
	[TestFixture]
	public class FlatLanguageStructureTests : GlobalizationTests
	{
		protected override void CreatePageStructure()
		{
			root = engine.Definitions.CreateInstance<Items.TranslatedPage>(null);
			engine.Persister.Save(root);

			english = engine.Definitions.CreateInstance<Items.LanguageRoot>(root);
			english.LanguageCode = "en-GB";
			english.Name = english.Title = "english";
			engine.Persister.Save(english);

			swedish = engine.Definitions.CreateInstance<Items.LanguageRoot>(root);
			swedish.LanguageCode = "sv-SE";
			swedish.Name = swedish.Title = "swedish";
			engine.Persister.Save(swedish);

			italian = engine.Definitions.CreateInstance<Items.LanguageRoot>(root);
			italian.LanguageCode = "it-IT";
			italian.Name = italian.Title = "italian";
			engine.Persister.Save(italian);

			engine.Resolve<Site>().RootItemID = root.ID;
			engine.Resolve<Site>().StartPageID = root.ID;
		}

	}
}
