using N2.Details;
using N2.Integrity;
using System.Web.UI.WebControls;
using N2.Edit.Trash;
using N2.Definitions;

namespace N2.Templates.Mvc.Models.Parts
{
    [PartDefinition("Comment",
        IconUrl = "~/Content/Img/comment.png")]
    [Throwable(AllowInTrash.No)]
    [Versionable(AllowVersions.No)]
    [RestrictParents(typeof(CommentList))]
    [WithEditableTitle]
    public class Comment : PartBase
    {
        [EditableText("AuthorName", 100)]
        public virtual string AuthorName
        {
            get { return (string) (GetDetail("AuthorName") ?? string.Empty); }
            set { SetDetail("AuthorName", value, string.Empty); }
        }

        [EditableText("Email", 110)]
        public virtual string Email
        {
            get { return (string) (GetDetail("Email") ?? string.Empty); }
            set { SetDetail("Email", value, string.Empty); }
        }

        [EditableText("AuthorUrl", 120)]
        public virtual string AuthorUrl
        {
            get { return (string) (GetDetail("AuthorUrl") ?? string.Empty); }
            set { SetDetail("AuthorUrl", value, string.Empty); }
        }

        [EditableText("Text", 130, TextMode = TextBoxMode.MultiLine)]
        public virtual string Text
        {
            get { return (string) (GetDetail("Text") ?? string.Empty); }
            set { SetDetail("Text", value, string.Empty); }
        }
    }
}
