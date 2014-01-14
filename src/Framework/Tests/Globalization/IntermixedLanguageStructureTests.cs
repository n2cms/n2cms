using System.Collections.Generic;
using N2.Engine.Globalization;
using N2.Persistence;
using N2.Tests.Globalization.Items;
using N2.Web;
using NUnit.Framework;

namespace N2.Tests.Globalization
{
    [TestFixture, Category("Integration")]
    public class IntermixedLanguageStructureTests : GlobalizationTests
    {
        protected override void CreatePageStructure()
        {
            root = engine.Resolve<ContentActivator>().CreateInstance<Items.TranslatedPage>(null);

            english = engine.Resolve<ContentActivator>().CreateInstance<Items.LanguageRoot>(root);
            english.LanguageCode = "en-GB";
            english.Name = english.Title = "english";
            english.AddTo(root);

            swedish = engine.Resolve<ContentActivator>().CreateInstance<Items.LanguageRoot>(english);
            swedish.LanguageCode = "sv-SE";
            swedish.Name = swedish.Title = "swedish";
            swedish.AddTo(root);

            italian = engine.Resolve<ContentActivator>().CreateInstance<Items.LanguageRoot>(swedish);
            italian.LanguageCode = "it-IT";
            italian.Name = italian.Title = "italian";
            italian.AddTo(root);

            engine.Persister.Repository.SaveOrUpdate(root, english, swedish, italian);

            engine.Resolve<IHost>().DefaultSite.RootItemID = root.ID;
            engine.Resolve<IHost>().DefaultSite.StartPageID = root.ID;
        }

        [Test]
        public void DoesntFind_LanguageRoot_InTrashcan()
        {
            TrashCan trash = engine.Resolve<ContentActivator>().CreateInstance<TrashCan>(root);
            engine.Persister.Save(trash);
            italian.AddTo(trash);
            engine.Persister.Save(italian);
            swedish.AddTo(trash);
            engine.Persister.Save(swedish);

            ILanguageGateway gateway = engine.Resolve<ILanguageGateway>();
            IList<ILanguage> languageRoot = new List<ILanguage>(gateway.GetAvailableLanguages());

            Assert.That(languageRoot.Count, Is.EqualTo(1));
        }
    }
}
