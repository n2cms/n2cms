using System;
using System.Collections.Generic;
using System.Text;
using N2.Globalization;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using N2.Persistence;
using Rhino.Mocks;
using N2.Web;

namespace N2.Tests.Globalization
{
	[TestFixture]
	public class LanguageRootTests : ItemTestsBase
	{
		ContentItem root;

		[SetUp]
		public override void SetUp()
		{
			base.SetUp();

			root = CreateOneItem<Items.TranslatedPage>(1, "root", null);
			ContentItem english = CreateOneItem<Items.LanguageRoot>(2, "english", root);
			ContentItem swedish = CreateOneItem<Items.LanguageRoot>(3, "swedish", root);
			ContentItem italian = CreateOneItem<Items.LanguageRoot>(4, "italian", root);
		}

		[Test]
		public void CanList_LanguageRoots()
		{
			IPersister persister = mocks.CreateMock<IPersister>();
			Expect.Call(persister.Get(1)).Return(root);
			mocks.ReplayAll();

			LanguageGateway gateway = new LanguageGateway(persister, new Site(1));
			IList<ILanguageRoot> languageRoot = new List<ILanguageRoot>(gateway.GetLanguages());

			Assert.That(languageRoot.Count, Is.EqualTo(3));
		}
	}
}
