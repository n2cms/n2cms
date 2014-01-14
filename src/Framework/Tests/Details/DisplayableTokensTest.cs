using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Text;
using System.Web;
using System.Web.UI;
using N2.Collections;
using N2.Definitions;
using N2.Details;
using N2.Web.Parts;
using N2.Web.UI.WebControls;
using NUnit.Framework;
using N2.Tests.Details.Models;

namespace N2.Tests.Details
{
    [TestFixture]
    public class DisplayableTokensTest : ItemTestsBase
    {
        DisplayableTokensAttribute dta;
        EnumableItem item;

        [SetUp]
        public override void SetUp()
        {
            dta = new DisplayableTokensAttribute { Name = "DaysString" };
            item = CreateOneItem<EnumableItem>(1, "item", null);
            base.SetUp();
        }

        [Test]
        public void SavingToken_TokenIsAdded_ToDetailCollection()
        {
            item.DaysString = "{{token}}";

            dta.Transform(item);

            var details = item.GetDetailCollection("DaysString_Tokens", false).Details;
            Assert.That(details.Single().StringValue, Is.EqualTo("{{token}}"));
        }

        [Test]
        public void Tokens_MayHaveData()
        {
            item.DaysString = "{{token|with data}}";

            dta.Transform(item);

            var details = item.GetDetailCollection("DaysString_Tokens", false).Details;
            Assert.That(details.Single().StringValue, Is.EqualTo("{{token|with data}}"));
        }

        [Test]
        public void MultipleTokens_CanBeAdded()
        {
            item.DaysString = "{{token}}{{second}}";

            dta.Transform(item);

            var details = item.GetDetailCollection("DaysString_Tokens", false).Details;
            Assert.That(details.Count, Is.EqualTo(2));
        }

        [Test]
        public void Tokens_CanBeRemoved()
        {
            item.DaysString = "{{token}}{{second}}";
            dta.Transform(item);

            item.DaysString = "{{second}}";
            dta.Transform(item);

            var details = item.GetDetailCollection("DaysString_Tokens", false).Details;
            Assert.That(details.Single().StringValue, Is.EqualTo("{{second}}"));
        }

        [Test]
        public void AllTokens_CanBeRemoved()
        {
            item.DaysString = "{{token}}{{second}}";
            dta.Transform(item);

            item.DaysString = "";
            dta.Transform(item);

            var collection = item.GetDetailCollection("DaysString_Tokens", false);
            Assert.That(collection, Is.Null);
        }

        [Test]
        public void SavingToken_Token_IsGivenIndex()
        {
            item.DaysString = "{{token}}";

            dta.Transform(item);

            var details = item.GetDetailCollection("DaysString_Tokens", false).Details;
            Assert.That(details.Single().IntValue, Is.EqualTo(0));
        }

        [Test]
        public void SavingToken_Token_IsGivenIndex_AtBeginning()
        {
            item.DaysString = "{{token}}world";

            dta.Transform(item);

            var details = item.GetDetailCollection("DaysString_Tokens", false).Details;
            Assert.That(details.Single().IntValue, Is.EqualTo(0));
        }

        [Test]
        public void SavingToken_Token_IsGivenIndex_InMiddle()
        {
            item.DaysString = "hello{{token}}world";

            dta.Transform(item);

            var details = item.GetDetailCollection("DaysString_Tokens", false).Details;
            Assert.That(details.Single().IntValue, Is.EqualTo(5));
        }

        [Test]
        public void SavingToken_Token_IsGivenIndex_AtEnd()
        {
            item.DaysString = "hello{{token}}";

            dta.Transform(item);

            var details = item.GetDetailCollection("DaysString_Tokens", false).Details;
            Assert.That(details.Single().IntValue, Is.EqualTo(5));
        }
    }
}
