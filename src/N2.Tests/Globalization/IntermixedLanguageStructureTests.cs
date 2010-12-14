using System.Collections.Generic;
using NUnit.Framework;
using N2.Web;
using N2.Tests.Globalization.Items;
using N2.Engine.Globalization;

namespace N2.Tests.Globalization
{
	[TestFixture, Category("Integration")]
	public class IntermixedLanguageStructureTests : GlobalizationTests
	{
		protected override void CreatePageStructure()
		{
			root = engine.Definitions.CreateInstance<Items.TranslatedPage>(null);

			english = engine.Definitions.CreateInstance<Items.LanguageRoot>(root);
			english.LanguageCode = "en-GB";
			english.Name = english.Title = "english";
            english.AddTo(root);

			swedish = engine.Definitions.CreateInstance<Items.LanguageRoot>(english);
			swedish.LanguageCode = "sv-SE";
			swedish.Name = swedish.Title = "swedish";
            swedish.AddTo(root);

			italian = engine.Definitions.CreateInstance<Items.LanguageRoot>(swedish);
			italian.LanguageCode = "it-IT";
			italian.Name = italian.Title = "italian";
            italian.AddTo(root);

            engine.Persister.Save(root);

			engine.Resolve<IHost>().DefaultSite.RootItemID = root.ID;
			engine.Resolve<IHost>().DefaultSite.StartPageID = root.ID;
		}

        [Test]
        public void DoesntFind_LanguageRoot_InTrashcan()
        {
            TrashCan trash = engine.Definitions.CreateInstance<TrashCan>(root);
            italian.AddTo(trash);
            swedish.AddTo(trash);
            engine.Persister.Save(trash);

            ILanguageGateway gateway = engine.Resolve<ILanguageGateway>();
            IList<ILanguage> languageRoot = new List<ILanguage>(gateway.GetAvailableLanguages());

            Assert.That(languageRoot.Count, Is.EqualTo(1));
        }
	}
}
