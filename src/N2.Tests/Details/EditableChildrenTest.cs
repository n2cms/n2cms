using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using N2.Details;
using N2.Definitions;
using System.Web.UI;
using N2.Web.UI.WebControls;
using N2.Edit.Web;
using N2.Collections;
using N2.Web.Parts;
using System.Reflection;
using System.Security.Principal;
using System.Web;
using System.Web.Hosting;
using System.IO;

namespace N2.Tests.Details
{
    public class DecoratedItem : ContentItem
    {
        [EditableChildren("Children", "Children", 100)]
        public ItemList EditableChildren
        {
            get { return null; }
        }

        [EditableChildren("GenericChildren", "GenericChildren", 100)]
        public IList<BaseItem> GenericChildren
        {
            get { return null; }
        }

        public override ItemList GetChildren(string childZoneName)
        {
            return base.GetChildren(new ZoneFilter(childZoneName));
        }
    }

    public class OtherItem : ContentItem
    {
    }

    public class BaseItem : ContentItem
    {
    }

    public class SuperficialItem : BaseItem
    {
    }

    [TestFixture]
    public class EditableChildrenTest
    {
        [Test]
        public void CreatedEditor_Is_ItemEditorList()
        {
            var attribute = typeof(DecoratedItem).GetProperty("EditableChildren").GetCustomAttributes(typeof(IEditable), false).First() as IEditable;
            attribute.Name = "EditableChildren";

            FakeEditorContainer p = new FakeEditorContainer();
            p.CurrentItem = new DecoratedItem();
            var editor = attribute.AddTo(p);

            Assert.That(editor, Is.TypeOf<ItemEditorList>());
        }

        [Test]
        public void CreatedEditor_Has_ParentItem()
        {
            var attribute = typeof(DecoratedItem).GetProperty("EditableChildren").GetCustomAttributes(typeof(IEditable), false).First() as IEditable;
            attribute.Name = "EditableChildren";

            FakeEditorContainer p = new FakeEditorContainer();
            p.CurrentItem = new DecoratedItem();

            var editor = attribute.AddTo(p) as ItemEditorList;
            Assert.That(editor.ParentItem, Is.EqualTo(p.CurrentItem));
        }

        [Test]
        public void CreatedEditor_UsesDefinitions()
        {
            var attribute = typeof(DecoratedItem).GetProperty("EditableChildren").GetCustomAttributes(typeof(IEditable), false).First() as IEditable;
            attribute.Name = "EditableChildren";

            FakeEditorContainer p = new FakeEditorContainer();
            p.CurrentItem = new DecoratedItem();

            var editor = attribute.AddTo(p) as ItemEditorList;
            editor.Parts = new FakePartsAdapter();

            editor.GetType()
                .GetMethod("CreateChildControls", BindingFlags.NonPublic | BindingFlags.Instance)
                .Invoke(editor, null);
            
            Assert.That(editor.Types.Items.Count, Is.EqualTo(3));
        }

        [Test]
        public void CreatedEditor_FiltersDefinitions_ByPropertyGenericType()
        {
            var attribute = typeof(DecoratedItem).GetProperty("GenericChildren").GetCustomAttributes(typeof(IEditable), false).First() as IEditable;
            attribute.Name = "GenericChildren";

            FakeEditorContainer p = new FakeEditorContainer();
            p.CurrentItem = new DecoratedItem();

            var editor = attribute.AddTo(p) as ItemEditorList;
            editor.Parts = new FakePartsAdapter();
            attribute.UpdateEditor(p.CurrentItem, editor);

            editor.GetType()
                .GetMethod("CreateChildControls", BindingFlags.NonPublic | BindingFlags.Instance)
                .Invoke(editor, null);

            Assert.That(editor.Types.Items.Count, Is.EqualTo(2));
        }
         
         
         
         
         
        private class FakePartsAdapter : PartsAdapter
        {
            public override IEnumerable<ItemDefinition> GetAllowedDefinitions(ContentItem parentItem, string zoneName, System.Security.Principal.IPrincipal user)
            {
                yield return new ItemDefinition(typeof(OtherItem));
                yield return new ItemDefinition(typeof(BaseItem));
                yield return new ItemDefinition(typeof(SuperficialItem));
          }
        }

        private class FakeEditorContainer : Page, IItemEditor
        {
            public FakeEditorContainer()
            {
            }

            protected override System.Web.HttpContext Context
            {
                get
                {
                    return new HttpContext(new HttpRequest("/Default.aspx", "http://localhost/", ""), new HttpResponse(new StringWriter(new StringBuilder())))
                    { 
                        User = new GenericPrincipal(new GenericIdentity("user"), new string[0]) 
                    };
                }
            }

            #region IItemEditor Members

            public ItemEditorVersioningMode VersioningMode
            {
                get
                {
                    throw new NotImplementedException();
                }
                set
                {
                    throw new NotImplementedException();
                }
            }

            public string ZoneName
            {
                get
                {
                    throw new NotImplementedException();
                }
                set
                {
                    throw new NotImplementedException();
                }
            }

            public IDictionary<string, Control> AddedEditors
            {
                get { throw new NotImplementedException(); }
            }

			public event EventHandler<ItemEventArgs> Saved = delegate { };

            #endregion

            #region IItemContainer Members

            public ContentItem CurrentItem { get; set; }

            #endregion
        }

    }
}
