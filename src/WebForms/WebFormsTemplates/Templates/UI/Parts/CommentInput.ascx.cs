using System;
using N2.Edit;
using N2.Persistence;

namespace N2.Templates.UI.Parts
{
    public partial class CommentInput : Web.UI.TemplateUserControl<Items.AbstractPage, Items.CommentInput>
    {
        protected override void OnInit(EventArgs e)
        {
            Resources.Register.JQuery(Page);
            base.OnInit(e);
        }
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            Page.Validate("CommentInput");
            if (Page.IsValid)
            {
                Items.CommentList list = CurrentPage.GetChild("Comments") as Items.CommentList;
                if(list == null)
                {
                    list = Engine.Resolve<ContentActivator>().CreateInstance<Items.CommentList>(CurrentPage);
                    list.Title = "Comments";
                    list.Name = "Comments";
                    list.ZoneName = Zones.Content;
                    if (CurrentItem.Parent == CurrentPage && CurrentItem.ZoneName == Zones.Content)
                    {
                        Engine.Resolve<ITreeSorter>().MoveTo(list, NodePosition.Before, CurrentItem);
                    }
                    Engine.Persister.Save(list);
                }
                Items.Comment comment = Engine.Resolve<ContentActivator>().CreateInstance<Items.Comment>(list);
                comment.Title = Server.HtmlEncode(txtTitle.Text);
                comment.AuthorName = Server.HtmlEncode(txtName.Text);
                comment.Email = Server.HtmlEncode(txtEmail.Text);
                comment.AuthorUrl = Server.HtmlEncode(txtUrl.Text);
                comment.Text = Server.HtmlEncode(txtText.Text);
                comment.ZoneName = "Comments";
                Engine.Persister.Save(comment);

                Response.Redirect(comment.Url);
            }
        }
    }
}
