using N2.Tests.Web.Items;
using N2.Web;
using NUnit.Framework;

namespace N2.Tests.Web
{
    [TestFixture]
    public class ParsingUrlsWithActions : ParserTestsBase
    {
        MasterDetailsPage master, master1_1, master1_1_1;
        ListDetailsPage list, list1_1, list1_1_1;
        RegexPage regex, regex1_1, regex1_1_1;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            CreateDefaultStructure();

            AppendMasterPages();
            AppendListPages();
            AppendRegexPages();
        }

        void AppendRegexPages()
        {
            regex = CreateOneItem<RegexPage>(++greatestID, "regex", startItem);
            regex1_1 = CreateOneItem<RegexPage>(++greatestID, "regex1_1", page1);
            regex1_1_1 = CreateOneItem<RegexPage>(++greatestID, "regex1_1_1", page1_1);
        }

        void AppendMasterPages()
        {
            master = CreateOneItem<MasterDetailsPage>(++greatestID, "master", startItem);
            master1_1 = CreateOneItem<MasterDetailsPage>(++greatestID, "master1_1", page1);
            master1_1_1 = CreateOneItem<MasterDetailsPage>(++greatestID, "master1_1_1", page1_1);
        }

        void AppendListPages()
        {
            list = CreateOneItem<ListDetailsPage>(++greatestID, "list", startItem);
            list1_1 = CreateOneItem<ListDetailsPage>(++greatestID, "list1_1", page1);
            list1_1_1 = CreateOneItem<ListDetailsPage>(++greatestID, "list1_1_1", page1_1);
        }

        [Test]
        public void MasterDetails_WithNoAction()
        {
            Url url = "/master";
            PathData data = startItem.FindPath(url);

            Assert.That(data.Action, Is.EqualTo(PathData.DefaultAction));
            Assert.That(data.TemplateUrl, Is.EqualTo("~/views/master.aspx"));
        }

        [Test]
        public void MasterDetails_WithDetailsAction()
        {
            Url url = "/master/details";
            PathData data = startItem.FindPath(url);

            Assert.That(data.Action, Is.EqualTo("details"));
            Assert.That(data.TemplateUrl, Is.EqualTo("~/views/details.aspx"));
        }

        [Test]
        public void ListDetails_WithNoAction()
        {
            Url url = "/list";
            PathData data = startItem.FindPath(url);

            Assert.That(data.Action, Is.EqualTo(PathData.DefaultAction));
            Assert.That(data.TemplateUrl, Is.EqualTo("~/views/list.aspx"));
        }

        [Test]
        public void ListDetails_WithListAction()
        {
            Url url = "/list/details";
            PathData data = startItem.FindPath(url);

            Assert.That(data.Action, Is.EqualTo("details"));
            Assert.That(data.TemplateUrl, Is.EqualTo("~/views/details.aspx"));
        }

        [Test]
        public void ListDetails_WithListAction_AndArguments()
        {
            Url url = "/list/details/123";
            PathData data = startItem.FindPath(url);

            Assert.That(data.Action, Is.EqualTo("details"));
            Assert.That(data.Argument, Is.EqualTo("123"));
            Assert.That(data.TemplateUrl, Is.EqualTo("~/views/details.aspx"));
        }

        [Test]
        public void ListDetails_WithListAction_AndMultipleArguments()
        {
            Url url = "/list/details/123/and/321";
            PathData data = startItem.FindPath(url);

            Assert.That(data.Action, Is.EqualTo("details"));
            Assert.That(data.Argument, Is.EqualTo("123/and/321"));
            Assert.That(data.TemplateUrl, Is.EqualTo("~/views/details.aspx"));
        }

        [Test]
        public void SimpleRegex()
        {
            Url url = "/regex/abcdefg";
            PathData data = startItem.FindPath(url);

            Assert.That(data.Action, Is.Null);
            Assert.That(data.Argument, Is.EqualTo("abcdefg"));
            Assert.That(data.TemplateUrl, Is.EqualTo("~/views/anything.aspx"));
        }

        [Test]
        public void SimpleRegex_NoMatch_FallbacksToSomethingElse()
        {
            Url url = "/regex/bcdefgh.aspx";
            PathData data = startItem.FindPath(url);

            Assert.That(data.TemplateUrl, Is.Not.EqualTo("~/views/anything.aspx"));
        }

        [Test]
        public void SimpleRegex_WithAction()
        {
            Url url = "/regex/zabcdefg";
            PathData data = startItem.FindPath(url);

            Assert.That(data.Action, Is.EqualTo("zee"));
            Assert.That(data.Argument, Is.EqualTo("zabcdefg"));
            Assert.That(data.TemplateUrl, Is.EqualTo("~/views/zeenything.aspx"));
        }
    }

    [Template("details", "~/views/details.aspx")]
    public class MasterDetailsPage : PageItem
    {
        public override string TemplateUrl
        {
            get { return "~/views/master.aspx"; }
        }
    }

    [Template(PathData.DefaultAction, "~/views/list.aspx")]
    [Template("details", "~/views/details.aspx")]
    public class ListDetailsPage : PageItem
    {
    }

    [RegexTemplate("^a.*", "~/views/anything.aspx")]
    [RegexTemplate("^z.*", "~/views/zeenything.aspx", "zee")]
    public class RegexPage : PageItem
    {
    }
}
