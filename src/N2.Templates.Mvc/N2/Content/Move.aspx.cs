using System;
using N2.Definitions;
using N2.Edit.Web;
using N2.Integrity;
using N2.Web;
using N2.Security;

namespace N2.Edit
{
	[NavigationSeparatorPlugin("copyPasteSeparator", 40)]
    [NavigationLinkPlugin("Cut", "move", "javascript:n2nav.memorize('{selected}','move');", "", "{ManagementUrl}/Resources/icons/cut.png", 42, GlobalResourceClassName = "Navigation")]
    [ToolbarPlugin("CUT", "move", "javascript:n2.memorize('{selected}','move');", ToolbarArea.Operations, "", "{ManagementUrl}/Resources/icons/cut.png", 30, ToolTip = "move", GlobalResourceClassName = "Toolbar")]
	public partial class Move : EditPage
	{
		protected void Page_Load(object sender, EventArgs e)
		{
            btnCancel.NavigateUrl = Selection.SelectedItem.FindPath(PathData.DefaultAction).RewrittenUrl;
            
            if (!IsPostBack)
			{
                pnlNewName.Visible = false;
                ContentItem toMove = Selection.MemorizedItem;

                try
                {
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

				if(toMove != null)
                    LoadDefaultsAndInfo(toMove, Selection.SelectedItem);
			}
		}

		private void LoadDefaultsAndInfo(ContentItem moved, ContentItem destination)
		{
            txtNewName.Text = moved.Name;

            Title = string.Format(GetLocalResourceString("MovePage.TitleFormat"),
                                  moved.Title,
                                  destination.Title);

            from.Text = string.Format(GetLocalResourceString("from.TextFormat"),
                                      moved.Parent != null ? moved.Parent.Path : "",
                                      moved.Path);

            to.Text = string.Format(GetLocalResourceString("to.TextFormat"),
                                    destination.Path,
                                    moved.Name);

            itemsToMove.CurrentItem = moved;
            itemsToMove.DataBind();
		}

		protected void OnMoveClick(object sender, EventArgs e)
		{
			try
			{
                var movedItem = Selection.MemorizedItem;
                movedItem.Name = txtNewName.Text;
                Engine.Persister.Move(movedItem, Selection.SelectedItem);
                Refresh(movedItem, ToolbarArea.Both);
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