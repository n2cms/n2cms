using N2;
using N2.Integrity;
using N2.Details;
using System.Collections.Generic;

namespace MvcTest.Models
{
	[Definition("News")]
	[RestrictParents(typeof(NewsContainer))]
	public class NewsPage : AbstractPage
	{
		public NewsPage()
		{
			Visible = false;
		}

		[EditableFreeTextArea("Text", 100)]
		public virtual string Text
		{
			get { return (string)(GetDetail("Text") ?? string.Empty); }
			set { SetDetail("Text", value, string.Empty); }
		}

		public virtual IEnumerable<CommentItem> GetComments()
		{
			foreach (ContentItem item in GetChildren())
			{
				if (item is CommentItem)
				{
					yield return item as CommentItem;
				}
			}
		}
	}
}
