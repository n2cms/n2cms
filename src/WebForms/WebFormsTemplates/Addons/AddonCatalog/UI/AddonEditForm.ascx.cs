using System;
using System.IO;
using System.Security.Principal;
using System.Web.UI.WebControls;
using N2.Addons.AddonCatalog.Items;
using N2.Edit.FileSystem;
using N2.Resources;
using N2.Templates.Web.UI;
using N2.Web;
using N2.Persistence;

namespace N2.Addons.AddonCatalog.UI
{
    public partial class AddonEditForm : TemplateUserControl<ContentItem, ContentItem>
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Items.Addon addon = CurrentPage as Items.Addon;

            bool isAuthorizedToModify = IsAuthorized(Page.User);
            if(addon == null && isAuthorizedToModify)
            {
                Register.JQuery(Page);
            }
            else if (addon != null && isAuthorizedToModify && IsAuthor(Page.User, addon))
            {
                rfvAddon.Enabled = false;
                LoadAddon(addon);

                Register.JQuery(Page);   
            }
            else
            {
                pnlAddon.Visible = false;
                cvAuthenticated.IsValid = false;
            }
        }

        private void LoadAddon(Items.Addon addon)
        {
            txtTitle.Text = Decode(addon.Title);

            txtVersion.Text = Decode(addon.AddonVersion);

            Select(cblCategory.Items, (int)addon.Category);
            txtEmail.Text = Decode(addon.ContactEmail);
            txtName.Text = Decode(addon.ContactName);
            txtHomepage.Text = Decode(addon.HomepageUrl);
            txtN2Version.Text = Decode(addon.LastTestedVersion);
            Select(cblRequirements.Items, (int)addon.Requirements);
            txtSource.Text = Decode(addon.SourceCodeUrl);
            txtSummary.Text = Decode(addon.Summary);
            txtDescription.Text = Encode(addon.Text);
        }

        private string Decode(string value)
        {
            return Server.HtmlDecode(value);
        }

        private void Select(ListItemCollection items, int value)
        {
            foreach(ListItem li in items)
            {
                int expected = int.Parse(li.Value);
                li.Selected = ((expected & value) == expected);
            }
        }

        private bool IsAuthorized(IPrincipal user)
        {
            bool isEditor = Engine.SecurityManager.IsEditor(user);
            Items.AddonCatalog catalog = CurrentPage as Items.AddonCatalog ?? CurrentPage.Parent as Items.AddonCatalog;
            bool isAllowedRole = false;
            foreach (string role in catalog.ModifyRoles)
            {
                isAllowedRole |= user.IsInRole(role);
            }
            return isEditor || isAllowedRole;
        }

        private bool IsAuthor(IPrincipal user, Items.Addon addon)
        {
            return string.Equals(addon.AuthorUserName ?? addon.SavedBy, user.Identity.Name, StringComparison.InvariantCultureIgnoreCase)
                   || Engine.SecurityManager.IsEditor(Page.User);
        }

        protected void save_Click(object sender, EventArgs e)
        {
            Page.Validate();
            cvAuthenticated.IsValid = IsAuthorized(Page.User);
            if (!Page.IsValid)
                return;

            Items.Addon addon = CurrentItem as Items.Addon;
            if(addon == null)
            {
                addon = Engine.Resolve<ContentActivator>().CreateInstance<Items.Addon>(CurrentPage);
                addon.AuthorUserName = Page.User.Identity.Name;
            }
            else if(!IsAuthor(Page.User, addon))
            {
                cvAuthenticated.IsValid = false;
                return;
            }

            addon.Title = Encode(txtTitle.Text);
            addon.Name = Engine.Resolve<HtmlFilter>().CleanUrl(txtTitle.Text);

            addon.Text = Encode(txtDescription.Text);
            addon.AddonVersion = Encode(txtVersion.Text);
            addon.Category = (Items.CodeCategory)AssembleSelected(cblCategory.Items);
            addon.ContactEmail = Encode(txtEmail.Text);
            addon.ContactName = Encode(txtName.Text);
            addon.HomepageUrl = Encode(txtHomepage.Text);
            addon.LastTestedVersion = Encode(txtN2Version.Text);
            addon.Requirements = (Items.Requirement) AssembleSelected(cblRequirements.Items);
            addon.SourceCodeUrl = Encode(txtSource.Text);
            addon.Summary = Encode(txtSummary.Text);
            if(fuAddon.PostedFile.ContentLength > 0)
            {
                IFileSystem fs = Engine.Resolve<IFileSystem>();
                
                if(!string.IsNullOrEmpty(addon.UploadedFileUrl))
                {
                    if(File.Exists(addon.UploadedFileUrl))
                        File.Delete(addon.UploadedFileUrl);
                }
                string fileName = Path.GetFileName(fuAddon.PostedFile.FileName);
                Url folder = Url.Parse(Engine.EditManager.UploadFolders[0]).AppendSegment("Addons");
                if(!fs.DirectoryExists(folder))
                    fs.CreateDirectory(folder);
                
                addon.UploadedFileUrl = folder.AppendSegment(Path.GetFileNameWithoutExtension(fileName), Path.GetExtension(fileName));
                fs.WriteFile(addon.UploadedFileUrl, fuAddon.PostedFile.InputStream);
            }

            Engine.Persister.Save(addon);
            Response.Redirect(addon.Url);
        }

        private string Encode(string p)
        {
            return Server.HtmlEncode(Engine.Resolve<HtmlFilter>().StripHtml(p));
        }

        private int AssembleSelected(ListItemCollection listItemCollection)
        {
            int selected = 0;
            foreach(ListItem li in listItemCollection)
            {
                if (li.Selected)
                    selected += int.Parse(li.Value);
            }
            return selected;
        }
    }
}
