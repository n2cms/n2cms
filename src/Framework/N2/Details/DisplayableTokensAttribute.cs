using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Web.UI;
using N2.Web.Parsing;

namespace N2.Details
{
	public class DisplayableTokensAttribute : AbstractDisplayableAttribute, IWritingDisplayable, IContentTransformer
	{
		public override void Write(ContentItem item, string detailName, TextWriter writer)
		{
			//base.Write(item, detailName, writer);
		}



		#region IContentTransformer Members

		public ContentState ChangingTo
		{
			get { return ContentState.Published; }
		}

		public bool Transform(ContentItem item)
		{
			string html = item[Name] as string;
			if(html != null)
			{
				//var tokens = Engine.Container.ResolveAll<TokenBase>();
				//var context = new TokenizingContext { Text = html };
				//for (context.Index = 0; context.Index < html.Length; context.Index++)
				//{
				//    foreach (var token in tokens)
				//    {

				//    }
				//}
			}
			return false;
		}

		#endregion
	}
}
