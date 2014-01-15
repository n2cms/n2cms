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
using N2.Edit.Versioning;

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

            if (!Page.IsPostBack)
            {
                LoadSelectableTypes();
                LoadSelectableItems();
                LoadSelectableEditors();
            }
            else
            {
                ie.EditableNameFilter = Request[EditableNameFilter.UniqueID].Split(',');
            }

            ie.ParentPath = Selection.SelectedItem.Path;
            
        }

        protected void ddlTypes_OnSelectedIndexChanged(object sender, EventArgs args)
        {
            LoadSelectableItems();
            LoadSelectableEditors();
        }

        protected void OnNext(object sender, CommandEventArgs args)
        {
            mvWizard.ActiveViewIndex++;
        }

        protected void OnGotoEdit(object sender, CommandEventArgs args)
        {
            mvWizard.ActiveViewIndex++;

            EditableNameFilter.Value = string.Join(",", GetSelectedEditors());
            
            var definition = GetSelectedDefinition();
            ie.EditableNameFilter = GetSelectedEditors();
            ie.Discriminator = definition.Discriminator;
        }

        protected void OnSave(object sender, CommandEventArgs args)
        {
            var vm = Engine.Resolve<IVersionManager>();

            var ctx = ie.CreateCommandContext();
            if (ie.UpdateObject(ctx))
            {
                var editors = GetSelectedEditors();
                var items = GetSelectedItems();

                using (var tx = Engine.Persister.Repository.BeginTransaction())
                {
                    foreach (var name in editors)
                    {
                        object value = ctx.Content[name];
                        foreach (var item in items)
                        {
                            if (vm.IsVersionable(item))
                                vm.AddVersion(item);

                            item[name] = value;
                        }
                    }
                
                    rptAffectedItems.DataSource = items;
                    rptAffectedItems.DataBind();

                    try
                    {
                        foreach (var item in ctx.GetItemsToSave())
                        {
                            item.VersionIndex++;
                            Engine.Persister.Save(item);
                        }

                        tx.Commit();

                        Refresh(Selection.SelectedItem, ToolbarArea.Navigation);
                        mvWizard.ActiveViewIndex++;
                    }
                    catch (Exception ex)
                    {
                        cvException.ErrorMessage = Server.HtmlEncode(ex.Message);
                        tx.Rollback();
                    }
                }
            }
        }

        protected ItemDefinition GetSelectedDefinition()
        {
            var discriminator = Request[ddlTypes.UniqueID] ?? ddlTypes.SelectedValue;
            var definition = Engine.Definitions.GetDefinition(discriminator);
            return definition;
        }

        protected string[] GetSelectedEditors()
        {
            return cblEditors.Items.OfType<ListItem>().Where(li => li.Selected).Select(li => li.Value).ToArray();
        }

        protected IEnumerable<ContentItem> GetSelectedItems()
        {
            return chkDescendants.Items.OfType<ListItem>()
                .Where(li => li.Selected)
                .Select(li => int.Parse(li.Value))
                .Select(id => Engine.Persister.Get(id));
        }

        private void LoadSelectableItems()
        {
            chkDescendants.DataSource = repository.FindDescendants(Selection.SelectedItem, ddlTypes.SelectedValue)
                .ToList()
                .Where(i => Engine.SecurityManager.IsAuthorized(User, i, Security.Permission.Publish))
                .Select(i => new { Title = string.Format("<img src='{0}' /> {1} <a href='{2}'>&raquo;</a>", i.IconUrl, i.Title, i.Url), i.ID });
            chkDescendants.DataBind();
        }

        private void LoadSelectableEditors()
        {
            var definition = GetSelectedDefinition();
            
            cblEditors.DataSource = definition.Editables
                .Where(e => !string.IsNullOrEmpty(e.Title))
                .Select(e => new { e.Title, e.Name });
            cblEditors.DataBind();
            cblEditors.SelectedIndex = 0;
        }

        private void LoadSelectableTypes()
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
