using System.Web.UI.WebControls;
using N2.Definitions;
using N2.Details;
using N2.Edit.Trash;
using N2.Integrity;
using N2.Templates.Mvc.Models.Pages;
using N2.Templates.Mvc.Services;
using N2.Web.Mvc;

namespace N2.Templates.Mvc.Areas.Blog.Models.Pages
{
    [Disable]
    [PageDefinition("Blog Post Comment", Description = "A Comment on a blog post.", SortOrder = 155,
        IconUrl = "~/Content/Img/comment.png")]
    [Throwable(AllowInTrash.No)]
    [Versionable(AllowVersions.No)]
    [RestrictParents(typeof(BlogPost))]
	public class BlogComment : BlogBase
    {
        public BlogComment()
        {
            Visible = false;
        }

        public virtual int CommentID
        {
            get { return GetDetail("CommentID", 1); }
            set { SetDetail("CommentID", value, 1); }
        }

        [EditableTextBox("AuthorName", 90, ContainerName = Tabs.Content)]
        public virtual string AuthorName
        {
            get;
            set;
        }

        [EditableTextBox("Email", 93, ContainerName = Tabs.Content)]
        public virtual string Email
        {
            get;
            set;
        }

        [EditableTextBox("AuthorUrl", 96, ContainerName = Tabs.Content)]
        public virtual string AuthorUrl
        {
            get;
            set;
        }

    }
}