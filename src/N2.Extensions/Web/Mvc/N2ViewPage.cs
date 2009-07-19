using System;
using System.Collections;
using System.Web;
using System.Web.Mvc;
using N2.Web.UI;

namespace N2.Web.Mvc
{
	/// <summary>
	/// A ViewPage implementation that allows N2 Display helpers to be used
	/// 
	/// The Model must be a ContentItem
	/// </summary>
	/// <typeparam name="TItem"></typeparam>
	public class N2ViewPage<TItem> : ViewPage<TItem>, IItemContainer<TItem>
		where TItem : ContentItem
	{
		#region IItemContainer<TItem> Members

		public virtual TItem CurrentItem
		{
			get { return Model; }
		}

		ContentItem IItemContainer.CurrentItem
		{
			get { return CurrentItem; }
		}

		#endregion

		public override void RenderView(ViewContext viewContext)
		{
			ViewContext = viewContext;
			InitHelpers();
			ID = Guid.NewGuid().ToString();

			var response = new HttpResponse(viewContext.HttpContext.Response.Output);
			var context = new HttpContext(HttpContext.Current.Request, response) {User = viewContext.HttpContext.User};
			foreach (DictionaryEntry contextItem in viewContext.HttpContext.Items)
			{
				context.Items[contextItem.Key] = contextItem.Value;
			}

			ProcessRequest(context);
		}
	}
}