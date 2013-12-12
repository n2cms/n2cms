using System;
using System.Linq;
using N2.Definitions;
using N2.Edit.LinkTracker;
using N2.Edit.Web;
using N2.Integrity;
using N2.Security;
using N2.Web;
using N2.Edit.Activity;
using N2.Management.Activity;
using N2.Persistence;

namespace N2.Edit
{
    [NavigationSeparatorPlugin("copyPasteSeparator", 40, Legacy = true)]
    [NavigationLinkPlugin("Cut", "move", "javascript:n2nav.memorize('{selected}','move');", "", "{ManagementUrl}/Resources/icons/cut.png", 42,
        GlobalResourceClassName = "Navigation",
        RequiredPermission = Permission.Publish,
        Legacy = true)]
    [ToolbarPlugin("", "move_tool", "javascript:n2.memorize('{selected}','move');", ToolbarArea.Operations, "", "{ManagementUrl}/Resources/icons/cut.png", 30, ToolTip = "move",
        GlobalResourceClassName = "Toolbar",
        RequiredPermission = Permission.Publish,
        Legacy = true)]
    public partial class Move : EditPage
    {
        private readonly Engine.Logger<Move> logger;
        private Tracker tracker;

        public Move()
        {
            tracker = Engine.Resolve<Tracker>();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            btnCancel.NavigateUrl = Selection.SelectedItem.FindPath(PathData.DefaultAction).GetRewrittenUrl();

            ContentItem toMove = Selection.MemorizedItem;
            if (toMove == null)
                return;
            
            if (!IsPostBack)
            {
                pnlNewName.Visible = false;

                try
                {
                    PerformMove(toMove);
                }
                catch (NameOccupiedException ex)
                {
                    SetErrorMessage(cvMove, ex);
                    pnlNewName.Visible = true;
                }
                catch (DestinationOnOrBelowItselfException ex)
                {
                    SetErrorMessage(cvMove, ex);
                    btnMove.Enabled = false;
                }
                catch (PermissionDeniedException ex)
                {
                    SetErrorMessage(cvMove, ex);
                    btnMove.Enabled = false;
                }
                catch (NotAllowedParentException ex)
                {
                    SetErrorMessage(cvMove, ex);
                    btnMove.Enabled = false;
                }
                catch(NullReferenceException ex)
                {
                    logger.Error(ex);
                    SetErrorMessage(cvException, "Nothing to move");
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                    SetErrorMessage(cvMove, ex);
                }

                txtNewName.Text = toMove.Name;
            }

            LoadDefaultsAndInfo(toMove, Selection.SelectedItem);
        }

        private void PerformMove(ContentItem toMove)
        {
            EnsureAuthorization(Permission.Write);
            EnsureAuthorization(toMove, toMove.IsPublished() ? Permission.Publish : Permission.Write);

            var previousParent = toMove.Parent;
            var previousUrl = toMove.Url;

            Engine.AddActivity(new ManagementActivity { Operation = "Move", PerformedBy = User.Identity.Name, Path = toMove.Path, ID = toMove.ID });
            Engine.Persister.Move(toMove, Selection.SelectedItem);

            if (tracker.FindReferrers(previousUrl).Any())
            {
                Response.Redirect(Selection.SelectedUrl("{ManagementUrl}/Content/LinkTracker/UpdateReferences.aspx", toMove).ToUrl().AppendQuery("previousParent", previousParent != null ? previousParent.Path : null).AppendQuery("previousName", toMove.Name));
            }
            else if (toMove.IsPage && !(toMove.Parent is IActiveContent))
                Response.Redirect(Selection.SelectedUrl("{ManagementUrl}/Content/LinkTracker/UpdateReferences.aspx", toMove).ToUrl().AppendQuery("previousParent", previousParent != null ? previousParent.Path : null).AppendQuery("previousName", toMove.Name));
            else
                Refresh(toMove);
        }

        private void LoadDefaultsAndInfo(ContentItem moved, ContentItem destination)
        {
            Title = string.Format(GetLocalResourceString("MovePage.TitleFormat", "Move \"{0}\" onto \"{1}\""),
                                  moved.Title,
                                  destination.Title);

            from.Text = string.Format(GetLocalResourceString("from.TextFormat", "{0}&lt;b&gt;{1}&lt;/b&gt;"),
                                      moved.Parent != null ? moved.Parent.Path : "",
                                      moved.Name);

            to.Text = string.Format(GetLocalResourceString("to.TextFormat", "{0}&lt;b&gt;{1}&lt;/b&gt;"),
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
                PerformMove(movedItem);
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
