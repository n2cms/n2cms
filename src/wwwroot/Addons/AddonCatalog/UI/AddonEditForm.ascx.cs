using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using N2.Resources;
using N2.Templates.Web.UI;
using N2.Web;
using N2.Templates.Wiki;

namespace N2.Addons.AddonCatalog.UI
{
    public partial class AddonEditForm : TemplateUserControl<ContentItem, ContentItem>
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            if(IsAuthorized())
            {
                if(CurrentItem is Items.Addon && IsAuthor())
                {
                    rfvAddon.Enabled = false;
                    LoadAddon(CurrentItem as Items.Addon);
                }

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

        private bool IsAuthorized()
        {
            bool isEditor = Engine.SecurityManager.IsEditor(Page.User);
            bool isMember = Page.User.IsInRole("Members");
            return isEditor || isMember;
        }

        private bool IsAuthor()
        {
            return CurrentItem.SavedBy == Page.User.Identity.Name;
        }

        protected void save_Click(object sender, EventArgs e)
        {
            Page.Validate();
            cvAuthenticated.IsValid = IsAuthorized();
            if (!Page.IsValid)
                return;

            Items.Addon addon = CurrentItem as Items.Addon;
            if(addon == null)
            {
                addon = Engine.Definitions.CreateInstance<Items.Addon>(CurrentPage);
            }
            else if(!IsAuthor())
            {
                cvAuthenticated.IsValid = false;
                return;
            }

            addon.Title = Encode(txtTitle.Text);
            addon.Name = new N2.Templates.Wiki.HtmlFilter().CleanUrl(txtTitle.Text);

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
                if(!string.IsNullOrEmpty(addon.UploadedFileUrl))
                {
                    string existingFile = Server.MapPath(addon.UploadedFileUrl);
                    if(File.Exists(existingFile))
                        File.Delete(existingFile);
                }
                string fileName = Path.GetFileName(fuAddon.PostedFile.FileName);
                Url folder = Url.Parse(Engine.EditManager.UploadFolders[0]).AppendSegment("Addons");
                string folderPath = Server.MapPath(folder);
                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);
                
                addon.UploadedFileUrl = folder.AppendSegment(Path.GetFileNameWithoutExtension(fileName), Path.GetExtension(fileName));
                fuAddon.PostedFile.SaveAs(Server.MapPath(addon.UploadedFileUrl));
            }

            Engine.Persister.Save(addon);
            Response.Redirect(addon.Url);
        }

        private string Encode(string p)
        {
            return Server.HtmlEncode(new HtmlFilter().StripHtml(p));
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