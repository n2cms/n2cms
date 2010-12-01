using System;
using N2.Security;
using N2.Web;

namespace N2.Edit
{
	[NavigationLinkPlugin("Copy", "copy", "javascript:n2nav.memorize('{selected}','copy');", "", "{ManagementUrl}/Resources/icons/page_copy.png", 50,
		GlobalResourceClassName = "Navigation")]
	[ToolbarPlugin("COPY", "copy", "javascript:n2.memorize('{selected}','copy');", ToolbarArea.Operations, "", "{ManagementUrl}/Resources/icons/page_copy.png", 40,
		ToolTip = "copy",
		GlobalResourceClassName = "Toolbar")]
	public partial class Copy : Web.EditPage
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
				try
				{
					EnsureAuthorization(Permission.Write);
					EnsureAuthorization(MemorizedItem, Permission.Read);

					N2.ContentItem newItem = Engine.Persister.Copy(Selection.MemorizedItem, Selection.SelectedItem);
					Refresh(newItem, ToolbarArea.Both);
				}
				catch (N2.Integrity.NameOccupiedException ex)
				{
					this.pnlNewName.Visible = true;
					SetErrorMessage(this.cvCopy, ex);
				}
				catch (PermissionDeniedException ex)
				{
					SetErrorMessage(cvCopy, ex);
				}
				catch (N2.Definitions.NotAllowedParentException ex)
				{
					SetErrorMessage(this.cvCopy, ex);
				}
				catch (N2Exception ex)
				{
					SetErrorMessage(this.cvCopy, ex);
				}
				LoadDefaultsAndInfo();
			}
		}

		private void LoadDefaultsAndInfo()
		{
			txtNewName.Text = MemorizedItem.Name;

			this.Title = string.Format(GetLocalResourceString("CopyPage.TitleFormat"),
				Selection.MemorizedItem.Title,
				Selection.SelectedItem.Title);

			this.from.Text = string.Format(GetLocalResourceString("from.TextFormat"),
										   MemorizedItem.Parent != null ? MemorizedItem.Parent.Path : "",
										   MemorizedItem.Path);

			this.to.Text = string.Format(GetLocalResourceString("to.TextFormat"),
				Selection.SelectedItem.Path,
				Selection.MemorizedItem.Name);

			itemsToCopy.CurrentItem = MemorizedItem;
			itemsToCopy.DataBind();
		}

		protected void OnCopyClick(object sender, EventArgs e)
		{
			try
			{
				pnlNewName.Visible = false;
				N2.ContentItem newItem = Selection.MemorizedItem.Clone(true);
				newItem.Name = txtNewName.Text;
				newItem = Engine.Persister.Copy(newItem, Selection.SelectedItem);
				Refresh(newItem, ToolbarArea.Both);
			}
			catch (N2.Integrity.NameOccupiedException ex)
			{
				this.pnlNewName.Visible = true;
				SetErrorMessage(this.cvCopy, ex);
			}
			catch (N2.Definitions.NotAllowedParentException ex)
			{
				SetErrorMessage(this.cvCopy, ex);
			}
			catch (Exception ex)
			{
				SetErrorMessage(this.cvCopy, ex);
			}
		}
	}
}