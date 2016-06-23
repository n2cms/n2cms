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
using N2.Persistence.Proxying;
using Shouldly;
using N2.Definitions.Static;
using N2.Edit.Workflow;

namespace N2.Tests.Details
{
    [TestFixture]
    public class EditableChildrenTest
    {
        private Fakes.FakeEngine engine;
		private DefinitionMap map;

        [SetUp]
        public void SetUp()
        {
            Context.Replace(engine = new Fakes.FakeEngine(new[] { typeof(DecoratedItem), typeof(DecoratedItem2) }));
			engine.AddComponentInstance<ITemplateAggregator>(new TemplateAggregator(engine.Definitions, new ITemplateProvider[] { new TemplateProvider(TestSupport.SetupContentActivator(), map = new DefinitionMap()) }));
        }

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
            editor.Parts = new FakePartsAdapter(map);

            editor.GetType()
                .GetMethod("CreateChildControls", BindingFlags.NonPublic | BindingFlags.Instance)
                .Invoke(editor, null);
            
            Assert.That(editor.AddButtons.Count(), Is.EqualTo(3));
        }

        [Test]
        public void CreatedEditor_FiltersDefinitions_ByPropertyGenericType()
        {
            var attribute = typeof(DecoratedItem).GetProperty("GenericChildren").GetCustomAttributes(typeof(IEditable), false).First() as IEditable;
            attribute.Name = "GenericChildren";

            FakeEditorContainer p = new FakeEditorContainer();
            p.CurrentItem = new DecoratedItem();

            var editor = attribute.AddTo(p) as ItemEditorList;
            editor.Parts = new FakePartsAdapter(map);
            attribute.UpdateEditor(p.CurrentItem, editor);

            editor.GetType()
                .GetMethod("CreateChildControls", BindingFlags.NonPublic | BindingFlags.Instance)
                .Invoke(editor, null);

            Assert.That(editor.AddButtons.Count(), Is.EqualTo(2));
        }

        [Test]
        public void AssignedChildren_AreAddedToChildCollection_WithPropertyName_AsZoneName()
        {
            var attribute = typeof(DecoratedItem).GetProperty("GenericChildren").GetCustomAttributes(typeof(IEditable), false).First() as IEditable;
            attribute.Name = "GenericChildren";

            var proxyFactory = engine.Resolve<IProxyFactory>();
            var item = (DecoratedItem)proxyFactory.Create(typeof(DecoratedItem).FullName, 0);

            item.GenericChildren = new[] { new BaseItem(), new SuperficialItem() };

            item.Children.Count.ShouldBe(2);
            item.Children.All(i => i.ZoneName == "GenericChildren").ShouldBe(true);
        }

        [Test]
        public void AssignedChildren_AreAddedToChildCollection_WithCustomZoneName_AsZoneName()
        {
            var proxyFactory = engine.Resolve<IProxyFactory>();
            var item = (DecoratedItem)proxyFactory.Create(typeof(DecoratedItem).FullName, 0);

            item.EditableChildren = new ItemList { new BaseItem(), new SuperficialItem() };

            item.Children.Count.ShouldBe(2);
            item.Children.All(i => i.ZoneName == "Children").ShouldBe(true);
        }

        [Test]
        public void AssignedChildren_AreAddedAsChildren_BeforeSaving_UnproxiedInstance()
        {
            var item = new DecoratedItem();

            item.GenericChildren = new [] { new BaseItem(), new SuperficialItem() };

            var proxyFactory = engine.Resolve<IProxyFactory>();
            proxyFactory.OnSaving(item);

            item.Children.Count.ShouldBe(2);
            item.Children.All(c => c.ZoneName == "GenericChildren").ShouldBe(true);
        }
        
        private class FakePartsAdapter : PartsAdapter
        {
			private DefinitionMap map;
			
			public FakePartsAdapter(DefinitionMap map)
			{
				this.map = map;
			}

            public override IEnumerable<ItemDefinition> GetAllowedDefinitions(ContentItem parentItem, string zoneName, System.Security.Principal.IPrincipal user)
            {
				yield return map.GetOrCreateDefinition(typeof(OtherItem)).Define();
				yield return map.GetOrCreateDefinition(typeof(BaseItem)).Define();
				yield return map.GetOrCreateDefinition(typeof(SuperficialItem)).Define();
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
			public event Action<object, CommandContext> CreatingContext;

			#endregion

			#region IItemContainer Members

			public ContentItem CurrentItem { get; set; }

            #endregion
        }
    }

	public static class FakePartsAdapterExtensions
	{
		public static ItemDefinition Define(this ItemDefinition definition)
		{
			definition.IsDefined = true;
			return definition;
		}
	}
}
