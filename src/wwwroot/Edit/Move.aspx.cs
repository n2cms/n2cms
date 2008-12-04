using System;
using N2.Definitions;
using N2.Edit.Web;
using N2.Integrity;
using N2.Web;

namespace N2.Edit
{
	[NavigationSeparatorPlugin("copyPasteSeparator", 40)]
    [NavigationLinkPlugin("Cut", "move", "javascript:n2nav.memorize('{selected}','move');", "", "~/edit/img/ico/cut.gif", 42, GlobalResourceClassName = "Navigation")]
    [ToolbarPlugin("", "move", "javascript:n2.memorize('{selected}','move');", ToolbarArea.Navigation, "", "~/Edit/Img/Ico/cut.gif", 30, ToolTip = "move", GlobalResourceClassName = "Toolbar")]
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
                    Engine.Persister.Move(toMove, SelectedItem);
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
			btnCancel.NavigateUrl = SelectedItem.FindTemplate(TemplateData.DefaultAction).RewrittenUrl;
			txtNewName.Text = MemorizedItem.Name;

			Title = string.Format(GetLocalResourceString("MovePage.TitleFormat"),
			                      MemorizedItem.Title,
			                      SelectedItem.Title);

			from.Text = string.Format(GetLocalResourceString("from.TextFormat"),
			                          GetBreadcrumbPath(MemorizedItem.Parent),
			                          MemorizedItem.Name);

			to.Text = string.Format(GetLocalResourceString("to.TextFormat"),
			                        GetBreadcrumbPath(SelectedItem),
			                        MemorizedItem.Name);

			itemsToMove.CurrentItem = MemorizedItem;
			itemsToMove.DataBind();
		}

		protected void OnMoveClick(object sender, EventArgs e)
		{
			try
			{
				MemorizedItem.Name = txtNewName.Text;
				Engine.Persister.Move(MemorizedItem, SelectedItem);
				Refresh(MemorizedItem, ToolbarArea.Both);
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