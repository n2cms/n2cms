using System;
using N2.Definitions;
using N2.Edit.Web;
using N2.Integrity;
using N2.Web;
using N2.Security;

namespace N2.Edit
{
	[NavigationSeparatorPlugin("copyPasteSeparator", 40)]
    [NavigationLinkPlugin("Cut", "move", "javascript:n2nav.memorize('{selected}','move');", "", "~/edit/img/ico/cut.gif", 42, GlobalResourceClassName = "Navigation")]
    [ToolbarPlugin("CUT", "move", "javascript:n2.memorize('{selected}','move');", ToolbarArea.Operations, "", "~/Edit/Img/Ico/cut.gif", 30, ToolTip = "move", GlobalResourceClassName = "Toolbar")]
	public partial class Move : EditPage
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
                try
                {
					pnlNewName.Visible = false;
                    ContentItem toMove = MemorizedItem;

                	EnsureAuthorization(Permission.Write);
					EnsureAuthorization(toMove, Permission.Write);

                    Engine.Persister.Move(toMove, Selection.SelectedItem);
                    Refresh(toMove, ToolbarArea.Both);
                }
                catch (NameOccupiedException ex)
                {
                    SetErrorMessage(cvMove, ex);
                    pnlNewName.Visible = true;
                }
                catch (DestinationOnOrBelowItselfException ex)
                {
                    SetErrorMessage(cvMove, ex);
                }
                catch (PermissionDeniedException ex)
                {
                    SetErrorMessage(cvMove, ex);
                }
                catch (NotAllowedParentException ex)
                {
                    SetErrorMessage(cvMove, ex);
                }
				catch(NullReferenceException ex)
				{
					SetErrorMessage(cvException, "Nothing to move");
				}
                catch (Exception ex)
                {
                    SetErrorMessage(cvMove, ex);
                }

				if(MemorizedItem != null)
					LoadDefaultsAndInfo();
			}
		}

		private void LoadDefaultsAndInfo()
		{
            btnCancel.NavigateUrl = Selection.MemorizedItem.Url;
            txtNewName.Text = Selection.MemorizedItem.Name;

			Title = string.Format(GetLocalResourceString("MovePage.TitleFormat"),
                                  Selection.MemorizedItem.Title,
                                  Selection.SelectedItem.Title);

			from.Text = string.Format(GetLocalResourceString("from.TextFormat"),
                                      Selection.MemorizedItem.Parent != null ? Selection.MemorizedItem.Parent.Path : "",
                                      Selection.MemorizedItem.Path);

			to.Text = string.Format(GetLocalResourceString("to.TextFormat"),
                                    Selection.SelectedItem.Path,
                                    Selection.MemorizedItem.Name);

            itemsToMove.CurrentItem = Selection.MemorizedItem;
			itemsToMove.DataBind();
		}

		protected void OnMoveClick(object sender, EventArgs e)
		{
			try
			{
				MemorizedItem.Name = txtNewName.Text;
                Engine.Persister.Move(Selection.MemorizedItem, Selection.SelectedItem);
                Refresh(Selection.MemorizedItem, ToolbarArea.Both);
			}
			catch (NameOccupiedException ex)
			{
				SetErrorMessage(cvMove, ex);
				pnlNewName.Visible = true;
			}
			catch (DestinationOnOrBelowItselfException ex)
			{
				SetErrorMessage(cvMove, ex);
			}
			catch (NotAllowedParentException ex)
			{
				SetErrorMessage(cvMove, ex);
			}
			catch (N2Exception ex)
			{
				SetErrorMessage(cvMove, ex);
			}
		}
	}
}