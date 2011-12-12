using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using N2.Edit.Web;
using N2.Definitions;
using N2.Persistence.NH;
using N2.Persistence;
using N2.Edit.Workflow;
using N2.Edit;

namespace N2.Management.Content.Export
{
	public partial class BulkEditing : EditPage
	{
		private IContentItemRepository repository;
		private ContentActivator activator;

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

			repository = Engine.Resolve<IContentItemRepository>();
			activator = Engine.Resolve<ContentActivator>();

			LoadTypes();

			if (Page.IsPostBack)
			{
				LoadEditors();
				InitEditor();
			}
			else
			{
				LoadType();
				InitEditor(); 
			}
		}

		protected string[] GetSelectedEditors()
		{
			return (Request["Editors"] ?? GetSelectedDefinition().Editables.Select(e => e.Name).FirstOrDefault() ?? "").Split(',');
		}

		private void InitEditor()
		{
			if (Request["__EVENTARGUMENT"] == "EditorSelectionChanged")
			{
				ie.Clear();
				ClearChildState();
			}

			var definition = GetSelectedDefinition();
			ie.EditableNameFilter = GetSelectedEditors();
			ie.CurrentItem = activator.CreateInstance(definition.ItemType, Selection.SelectedItem);
		}

		private ItemDefinition GetSelectedDefinition()
		{
			var discriminator = Request[ddlTypes.UniqueID] ?? ddlTypes.SelectedValue;
			var definition = Engine.Definitions.GetDefinition(discriminator);
			return definition;
		}

		protected void ddlTypes_OnSelectedIndexChanged(object sender, EventArgs args)
		{
			LoadType();
		}

		protected void OnNext(object sender, CommandEventArgs args)
		{
			int nextIndex = 1 + int.Parse((string)args.CommandArgument);
			mvWizard.ActiveViewIndex = nextIndex;
		}

		protected void OnSave(object sender, CommandEventArgs args)
		{
			var vm = Engine.Resolve<IVersionManager>();

			var ctx = ie.CreateCommandContext();
			if (ie.UpdateObject(ctx))
			{
				var editors = GetSelectedEditors();
				var items = GetSelectedItems();
				
				foreach (var name in editors)
				{
					object value = ctx.Content[name];
					foreach (var item in items)
					{
						if (vm.IsVersionable(item))
							vm.SaveVersion(item);

						item[name] = value;
					}
				}
				
				rptAffectedItems.DataSource = items;
				rptAffectedItems.DataBind();
				
				foreach (var item in ctx.GetItemsToSave())
				{
					item.VersionIndex++;
					using (var tx = Engine.Persister.Repository.BeginTransaction())
					{
						Engine.Persister.Save(item);
						tx.Commit();
					}
				}

				Refresh(Selection.SelectedItem, ToolbarArea.Navigation);
				mvWizard.ActiveViewIndex = 3;
			}
		}

		private IEnumerable<ContentItem> GetSelectedItems()
		{
			return chkDescendants.Items.OfType<ListItem>()
				.Where(li => li.Selected)
				.Select(li => int.Parse(li.Value))
				.Select(id => Engine.Persister.Get(id));
		}

		private void LoadType()
		{
			chkDescendants.DataSource = repository.FindDescendants(Selection.SelectedItem, ddlTypes.SelectedValue);
			chkDescendants.DataBind();

			LoadEditors();
		}

		private void LoadEditors()
		{
			var definition = GetSelectedDefinition();
			var selectedEditors = new HashSet<string>(GetSelectedEditors());
			rptEditors.DataSource = definition.Editables
				.Where(e => !string.IsNullOrEmpty(e.Title))
				.Select(e => new { e.Title, e.Name, Checked = selectedEditors.Contains(e.Name) ? " checked='checked'" : "" });
			rptEditors.DataBind();
		}

		private void LoadTypes()
		{
			var discriminators = new HashSet<string>(repository
				.FindDescendantDiscriminators(Selection.SelectedItem)
				.Select(d => d.Discriminator));

			ddlTypes.DataSource = Engine.Definitions.GetDefinitions()
				.Where(d => discriminators.Contains(d.Discriminator))
				.OrderByDescending(d => typeof(ISystemNode).IsAssignableFrom(d.ItemType) ? 0 : 1)
				.ThenBy(d => d.IsPage ? 0 : 1)
				.ThenByDescending(d => d.NumberOfItems / 10)
				.ThenBy(d => d.SortOrder)
				.ThenBy(d => d.Title);
			ddlTypes.DataBind();
		}
	}
}