using N2.Persistence;
using N2.Web;
using NUnit.Framework;

namespace N2.Tests.Globalization
{
    [TestFixture, Category("Integration")]
    public class FlatLanguageStructureTests : GlobalizationTests
    {
        protected override void CreatePageStructure()
        {
            root = engine.Resolve<ContentActivator>().CreateInstance<Items.TranslatedPage>(null);

            english = engine.Resolve<ContentActivator>().CreateInstance<Items.LanguageRoot>(root);
            english.LanguageCode = "en-GB";
            english.Name = english.Title = "english";
            english.AddTo(root);

            swedish = engine.Resolve<ContentActivator>().CreateInstance<Items.LanguageRoot>(root);
            swedish.LanguageCode = "sv-SE";
            swedish.Name = swedish.Title = "swedish";
            swedish.AddTo(root);

            italian = engine.Resolve<ContentActivator>().CreateInstance<Items.LanguageRoot>(root);
            italian.LanguageCode = "it-IT";
            italian.Name = italian.Title = "italian";
            italian.AddTo(root);

            engine.Persister.Repository.SaveOrUpdate(root, english, swedish, italian);

            engine.Resolve<IHost>().DefaultSite.RootItemID = root.ID;
            engine.Resolve<IHost>().DefaultSite.StartPageID = root.ID;
        }

    }
}
