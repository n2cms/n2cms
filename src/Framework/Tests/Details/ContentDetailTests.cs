using NUnit.Framework;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Tests.Details
{
    [TestFixture]
    public class ContentDetailTests
    {
        [Test]
        public void SetEnum()
        {
            var item = new Models.EnumableItem();

            item["enumy"] = Models.enumDays.Fri;

            item["enumy"].ShouldBe(Models.enumDays.Fri);
            var detail = item.Details["enumy"];

            detail.IntValue.ShouldBe((int)Models.enumDays.Fri);
            detail.StringValue.ShouldBe("Fri");
            detail.Meta.ShouldBe(typeof(Models.enumDays).AssemblyQualifiedName);
        }

        [Test]
        public void GetEnum()
        {
            var item = new Models.EnumableItem();
            item.Details.Add(new N2.Details.ContentDetail { Name = "enumy", Meta = typeof(Models.enumDays).AssemblyQualifiedName, StringValue = "Fri", ValueTypeKey = "Enum", IntValue = (int)Models.enumDays.Fri });
            item["enumy"].ShouldBe(Models.enumDays.Fri);
        }
    }
}
