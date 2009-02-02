using N2;
using N2.Details;
using N2.Integrity;

namespace MvcTest.Models
{
	[Definition("Comment")]
	[RestrictParents(typeof(NewsPage))]
	public class CommentItem : AbstractPage
	{
		public override bool IsPage
		{
			get { return false; }
		}

        public override string IconUrl
        {
            get { return "~/Content/comment.png"; }
        }

		[EditableFreeTextArea("Text", 100)]
		public virtual string Text
		{
			get { return (string)(GetDetail("Text") ?? string.Empty); }
			set { SetDetail("Text", value, string.Empty); }
		}
	}
}
