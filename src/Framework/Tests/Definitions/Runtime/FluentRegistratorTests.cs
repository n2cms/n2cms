using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Definitions.Runtime;
using N2.Web.Mvc.Html;
using NUnit.Framework;
using Shouldly;
using N2.Details;
using N2.Web.Mvc;
using System.Web.UI.WebControls;
using System.Web.Mvc;

namespace N2.Tests.Definitions.Runtime
{
    [TestFixture]
    public class FluentRegistratorTests
    {
        private N2.Definitions.Static.DefinitionMap map;
        private FluentItemRegistration registration;

        class FluentItem : ContentItem
        {
            public virtual string Text { get; set; }
            public virtual int Number { get; set; }
            public virtual TestStatus Enum { get; set; }
            public virtual IEnumerable<string> Strings { get; set; }
            public virtual IEnumerable<FluentItem> Items { get; set; }
            public virtual FluentItem Item { get; set; }
            public virtual bool Choice { get; set; }
        }

        class FluentItemRegistration : FluentRegisterer<FluentItem>
        {
            public Action<IContentRegistration<FluentItem>> registerAction = delegate { };
            public override void RegisterDefinition(IContentRegistration<FluentItem> re)
            {
                registerAction(re);
            }
        }

        [SetUp]
        public void SetUp()
        {
            map = new N2.Definitions.Static.DefinitionMap();
            registration = new FluentItemRegistration();
        }

        [Test]
        public void Register_Title()
        {
            registration.registerAction = (re) => re.Title();

            var definitions = registration.Register(map);

            var editable = definitions.Single().Editables.Single();
			editable.ShouldBeOfType<WithEditableTitleAttribute>();
            editable.Name.ShouldBe("Title");
        }

        [Test]
        public void Register_Name()
        {
            registration.registerAction = (re) => re.Name();

            var definitions = registration.Register(map);

            var editable = definitions.Single().Editables.Single();
			editable.ShouldBeOfType<WithEditableNameAttribute>();
            editable.Name.ShouldBe("Name");
        }

        [Test]
        public void Register_FreeText_ByExpression()
        {
            registration.registerAction = (re) => re.On(fi => fi.Text).FreeText();

            var definitions = registration.Register(map);

            var editable = definitions.Single().Editables.Single();
            editable.ShouldBeOfType<EditableFreeTextAreaAttribute>();
            editable.Name.ShouldBe("Text");
        }

        [Test]
        public void Register_FreeText_ByName()
        {
            registration.registerAction = (re) =>
            {
                re.On<string>("Text").FreeText();
            };

            var definitions = registration.Register(map);

            var editable = definitions.Single().Editables.Single();
            editable.ShouldBeOfType<EditableFreeTextAreaAttribute>();
            editable.Name.ShouldBe("Text");
        }

        [Test]
        public void Register_FreeText_AndConfigure()
        {
            registration.registerAction = (re) =>
            {
                re.On(fi => fi.Text).Text()
                    .DefaultValue("hello")
                    .Help("help title", "help body")
                    .Required("This value is required")
                    .Require(N2.Security.Permission.Administer)
                    .Container("ExtrasContainer")
                    .Configure(ee => ee.Placeholder = "Some text in the box");
            };

            var definitions = registration.Register(map);

            var editable = (EditableTextAttribute)definitions.Single().Editables.Single();
            editable.Name.ShouldBe("Text");
            editable.DefaultValue.ShouldBe("hello");
            editable.HelpTitle.ShouldBe("help title");
            editable.HelpText.ShouldBe("help body");
            editable.Required.ShouldBe(true);
            editable.RequiredMessage.ShouldBe("This value is required");
            editable.RequiredPermission.ShouldBe(N2.Security.Permission.Administer);
            editable.ContainerName.ShouldBe("ExtrasContainer");
            editable.Placeholder.ShouldBe("Some text in the box");
        }

        [Test]
        public void Register_DropDown()
        {
            registration.registerAction = (re) => re.On(fi => fi.Text).DropDown("Hello", "World");

            var definitions = registration.Register(map);

            var editable = (EditableDropDownAttribute)definitions.Single().Editables.Single();
            var ddl = (DropDownList)editable.AddTo(new System.Web.UI.Page());
            ddl.Items.Count.ShouldBe(3);
            ddl.Items[0].Text.ShouldBe("");
            ddl.Items[1].Text.ShouldBe("Hello");
            ddl.Items[2].Text.ShouldBe("World");
        }

        [Test]
        public void Register_FileUpload()
        {
            registration.registerAction = (re) => re.On(fi => fi.Text).FileUpload();

            var definitions = registration.Register(map);

            var editable = (EditableFileUploadAttribute)definitions.Single().Editables.Single();
            editable.Name.ShouldBe("Text");
        }

        [Test]
        public void Register_FolderSelection()
        {
            registration.registerAction = (re) => re.On(fi => fi.Text).FolderSelection();

            var definitions = registration.Register(map);

            var editable = (EditableFolderSelectionAttribute)definitions.Single().Editables.Single();
            editable.Name.ShouldBe("Text");
        }

        [Test]
        public void Register_Image()
        {
            registration.registerAction = (re) => re.On(fi => fi.Text).Image();

            var definitions = registration.Register(map);

            var editable = (EditableImageAttribute)definitions.Single().Editables.Single();
            editable.Name.ShouldBe("Text");
        }

        [Test]
        public void Register_ImageSize()
        {
            registration.registerAction = (re) => re.On(fi => fi.Text).ImageSize();

            var definitions = registration.Register(map);

            var editable = (EditableImageSizeAttribute)definitions.Single().Editables.Single();
            editable.Name.ShouldBe("Text");
        }

        [Test]
        public void Register_Languages()
        {
            registration.registerAction = (re) => re.On(fi => fi.Text).Languages();

            var definitions = registration.Register(map);

            var editable = (EditableLanguagesDropDownAttribute)definitions.Single().Editables.Single();
            editable.Name.ShouldBe("Text");
        }

        [Test]
        public void Register_MediaUpload()
        {
            registration.registerAction = (re) => re.On(fi => fi.Text).MediaUpload();

            var definitions = registration.Register(map);

            var editable = (EditableMediaUploadAttribute)definitions.Single().Editables.Single();
            editable.Name.ShouldBe("Text");
        }

        [Test]
        public void Register_MetaTag()
        {
            registration.registerAction = (re) => re.On(fi => fi.Text).Meta();

            var definitions = registration.Register(map);

            var editable = (EditableMetaTagAttribute)definitions.Single().Editables.Single();
            editable.Name.ShouldBe("Text");
        }

        [Test]
        public void Register_Summary()
        {
            registration.registerAction = (re) => re.On(fi => fi.Text).Summary();

            var definitions = registration.Register(map);

            var editable = (EditableSummaryAttribute)definitions.Single().Editables.Single();
            editable.Name.ShouldBe("Text");
        }

        [Test]
        public void Register_ThemeSelection()
        {
            registration.registerAction = (re) => re.On(fi => fi.Text).ThemeSelection();

            var definitions = registration.Register(map);

            var editable = (EditableThemeSelectionAttribute)definitions.Single().Editables.Single();
            editable.Name.ShouldBe("Text");
        }

        [Test]
        public void Register_Url()
        {
            registration.registerAction = (re) => re.On(fi => fi.Text).Url();

            var definitions = registration.Register(map);

            var editable = (EditableUrlAttribute)definitions.Single().Editables.Single();
            editable.Name.ShouldBe("Text");
        }

        [Test]
        public void Register_UserControl()
        {
            registration.registerAction = (re) => re.On(fi => fi.Text).UserControl("~/HelloWorld.ascx");

            var definitions = registration.Register(map);

            var editable = (EditableUserControlAttribute)definitions.Single().Editables.Single();
            editable.Name.ShouldBe("Text");
            editable.UserControlPath.ShouldBe("~/HelloWorld.ascx");
        }

        [Test]
        public void Register_Number()
        {
            registration.registerAction = (re) => re.On(fi => fi.Number).Number();

            var definitions = registration.Register(map);

            var editable = (EditableNumberAttribute)definitions.Single().Editables.Single();
            editable.Name.ShouldBe("Number");
        }

        [Test]
        public void Register_Item()
        {
            registration.registerAction = (re) => re.On(fi => fi.Item).Item();

            var definitions = registration.Register(map);

            var editable = (EditableItemAttribute)definitions.Single().Editables.Single();
            editable.Name.ShouldBe("Item");
        }

        [Test]
        public void Register_ItemSelection()
        {
            registration.registerAction = (re) => re.On(fi => fi.Item).ItemSelection();

            var definitions = registration.Register(map);

            var editable = (EditableItemSelectionAttribute)definitions.Single().Editables.Single();
            editable.Name.ShouldBe("Item");
            editable.LinkedType.ShouldBe(typeof(FluentItem));
        }

        [Test]
        public void Register_MultipleItemSelection()
        {
            registration.registerAction = (re) => re.On(fi => fi.Items).MultipleItemSelection();

            var definitions = registration.Register(map);

            var editable = (EditableMultipleItemSelectionAttribute)definitions.Single().Editables.Single();
            editable.Name.ShouldBe("Items");
            editable.LinkedType.ShouldBe(typeof(FluentItem));
        }

        [Test]
        public void Register_Tags()
        {
            registration.registerAction = (re) => re.On(fi => fi.Strings).Tags();

            var definitions = registration.Register(map);

            var editable = (EditableTagsAttribute)definitions.Single().Editables.Single();
            editable.Name.ShouldBe("Strings");
        }

        [Test]
        public void Register_CheckBox()
        {
            registration.registerAction = (re) => re.On(fi => fi.Choice).CheckBox();

            var definitions = registration.Register(map);

            var editable = (EditableCheckBoxAttribute)definitions.Single().Editables.Single();
            editable.Name.ShouldBe("Choice");
        }

        [Test]
        public void Register_Enum()
        {
            registration.registerAction = (re) => re.On(fi => fi.Enum).Enum();

            var definitions = registration.Register(map);

            var editable = (EditableEnumAttribute)definitions.Single().Editables.Single();
            editable.Name.ShouldBe("Enum");
            editable.EnumType.ShouldBe(typeof(TestStatus));
        }

        class FluentItemController : Controller
        {
            public ActionResult Hello()
            {
                throw new NotImplementedException();
            }
        }

        [Test]
        public void Register_Controller()
        {
            registration.registerAction = (re) => re.ControlledBy<FluentItemController>();

            var definitions = registration.Register(map);

            definitions.Single().Metadata["ControlledBy"].ShouldBe(typeof(FluentItemController));
        }
    }
}
