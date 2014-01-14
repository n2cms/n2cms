using System;
using System.Web.Mvc;
using N2.Edit;
using N2.Templates.Mvc.Models.Parts;
using N2.Templates.Mvc.Models;
using N2.Web;
using N2.Persistence;

namespace N2.Templates.Mvc.Controllers
{
    [Controls(typeof(CommentInput))]
    public class CommentInputController : TemplatesControllerBase<CommentInput>
    {
        public override ActionResult Index()
        {
            return View(new CommentInputModel(CurrentItem) { Trap = "Fall" });
        }

        public ActionResult Submit(CommentInputModel model)
        {
            if(!ModelState.IsValid)
            {
                return ViewParentPage();
            }

            var list = CurrentPage.GetChild("Comments") as CommentList;
            if (list == null)
            {
                list = Engine.Resolve<ContentActivator>().CreateInstance<CommentList>(CurrentPage);
                list.Title = "Comments";
                list.Name = "Comments";
                list.ZoneName = Zones.Content;
                if (CurrentItem.Parent == CurrentPage && CurrentItem.ZoneName == Zones.Content)
                {
                    Engine.Resolve<ITreeSorter>().MoveTo(list, NodePosition.Before, CurrentItem);
                }
                Engine.Persister.Save(list);
            }
            Comment comment = Engine.Resolve<ContentActivator>().CreateInstance<Comment>(list);
            comment.Title = Server.HtmlEncode(model.Title);
            comment.AuthorName = Server.HtmlEncode(model.Name);
            comment.Email = Server.HtmlEncode(model.Email);
            comment.AuthorUrl = Server.HtmlEncode(model.Url);
            comment.Text = Server.HtmlEncode(model.Text);
            comment.ZoneName = "Comments";

            Engine.Persister.Save(comment);

            return Redirect(CurrentPage.Url);
        }
    }
}
