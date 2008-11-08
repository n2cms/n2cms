using N2.Web;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace N2.Tests.Web
{
	[TestFixture]
	public class ParsingUrlsWithActions : ParserTestsBase
	{
		MasterDetailsPage master, master1_1, master1_1_1;
		ListDetailsPage list, list1_1, list1_1_1;

		[SetUp]
		public override void SetUp()
		{
			base.SetUp();

			CreateDefaultStructure();

			AppendMasterPages();
			AppendListPages();
		}

		void AppendMasterPages()
		{
			master = CreateOneItem<MasterDetailsPage>(++greatestID, "master", startItem);
			master1_1 = CreateOneItem<MasterDetailsPage>(++greatestID, "master1_1", item1);
			master1_1_1 = CreateOneItem<MasterDetailsPage>(++greatestID, "master1_1_1", item1_1);
		}

		void AppendListPages()
		{
			list = CreateOneItem<ListDetailsPage>(++greatestID, "list", startItem);
			list1_1 = CreateOneItem<ListDetailsPage>(++greatestID, "list1_1", item1);
			list1_1_1 = CreateOneItem<ListDetailsPage>(++greatestID, "list1_1_1", item1_1);
		}

		[Test]
		public void MasterDetails_WithNoAction()
		{
			Url url = "/master.aspx";
			TemplateData data = startItem.FindTemplate(url);

			Assert.That(data.Action, Is.EqualTo(TemplateData.DefaultAction));
			Assert.That(data.TemplateUrl, Is.EqualTo("~/views/master.aspx"));
		}

		[Test]
		public void MasterDetails_WithDetailsAction()
		{
			Url url = "/master/details.aspx";
			TemplateData data = startItem.FindTemplate(url);

			Assert.That(data.Action, Is.EqualTo("details"));
			Assert.That(data.TemplateUrl, Is.EqualTo("~/views/details.aspx"));
		}

		[Test]
		public void ListDetails_WithNoAction()
		{
			Url url = "/list.aspx";
			TemplateData data = startItem.FindTemplate(url);

			Assert.That(data.Action, Is.EqualTo(TemplateData.DefaultAction));
			Assert.That(data.TemplateUrl, Is.EqualTo("~/views/list.aspx"));
		}

		[Test]
		public void ListDetails_WithListAction()
		{
			Url url = "/list/details.aspx";
			TemplateData data = startItem.FindTemplate(url);

			Assert.That(data.Action, Is.EqualTo("details"));
			Assert.That(data.TemplateUrl, Is.EqualTo("~/views/details.aspx"));
		}

		[Test]
		public void ListDetails_WithListAction_AndArguments()
		{
			Url url = "/list/details/123.aspx";
			TemplateData data = startItem.FindTemplate(url);

			Assert.That(data.Action, Is.EqualTo("details"));
			Assert.That(data.Arguments, Is.EqualTo("123"));
			Assert.That(data.TemplateUrl, Is.EqualTo("~/views/details.aspx"));
		}

		[Test]
		public void ListDetails_WithListAction_AndMultipleArguments()
		{
			Url url = "/list/details/123/and/321.aspx";
			TemplateData data = startItem.FindTemplate(url);

			Assert.That(data.Action, Is.EqualTo("details"));
			Assert.That(data.Arguments, Is.EqualTo("123/and/321"));
			Assert.That(data.TemplateUrl, Is.EqualTo("~/views/details.aspx"));
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

	[Template(TemplateData.DefaultAction, "~/views/list.aspx")]
	[Template("details", "~/views/details.aspx")]
	public class ListDetailsPage : PageItem
	{

	}
}
