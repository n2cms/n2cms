using System;
using System.Web;
using System.Web.UI;
using System.Collections;
using System.Collections.Generic;
using N2.Definitions;

namespace N2.Web.UI
{
	/// <summary>
	/// Some helpful methods that havn't founda better place yet (use at your 
	/// own risk).
	/// </summary>
    public static class ItemUtility
    {
		public static T FindInParents<T>(Control parentControl) where T:class
		{
			if (parentControl == null || parentControl is T)
				return parentControl as T;
			else
				return FindInParents<T>(parentControl.Parent);
		}

        public static ContentItem FindCurrentItem(Control startControl)
        {
            IItemContainer itemContainer = FindInParents<IItemContainer>(startControl);
            if (itemContainer != null)
                return itemContainer.CurrentItem;
            return null;
        }

		public static ContentItem WalkPath(ContentItem startItem, string path)
		{
			// find starting point
            if (path.StartsWith("/"))
            {
                startItem = Context.Current.Persister.Get(Context.Current.Host.CurrentSite.RootItemID);
                path = path.Substring(1);
            }
            else if (path.StartsWith("~/"))
            {
                startItem = Context.Current.UrlParser.StartPage;
                path = path.Substring(2);
            }

			// walk path
			ContentItem item = startItem;
			string[] names = path.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
			foreach (string name in names)
			{
				if (item == null)
					break;
				else if (name == "..")
					item = item.Parent;
				else if (name != "." && !string.IsNullOrEmpty(name))
					item = item.GetChild(name);
			}
			return item;
		}

		public static Control AddUserControl(Control container, ContentItem item)
		{
			PathData path = item.FindPath(PathData.DefaultAction);
			if (!path.IsEmpty())
			{
				return AddUserControl(path.TemplateUrl, container, item);
			}
			return null;
		}

		public static Control AddUserControl(string templateUrl, Control container, ContentItem item)
		{
			using (new ItemStacker(item))
			{
				Control templateItem = container.Page.LoadControl(Context.Current.EditUrlManager.ResolveManagementInterfaceUrl(templateUrl));
				if (templateItem is IContentTemplate)
					(templateItem as IContentTemplate).CurrentItem = item;
				container.Controls.Add(templateItem);
				return templateItem;
			}
		}

		private class ItemStacker : IDisposable
		{
			public ItemStacker(ContentItem currentItem)
			{
				ItemStack.Push(currentItem);
			}
			public void Dispose()
			{
				ItemStack.Pop();
			}
		}

		internal static ContentItem CurrentContentItem
		{
			get { return ItemStack.Peek(); }
		}

		internal static Stack<ContentItem> ItemStack
		{
			get
			{
				Stack<ContentItem> stack = HttpContextItems["ItemStack"] as Stack<ContentItem>;
				if (stack == null)
				{
					HttpContextItems["ItemStack"] = stack = new Stack<ContentItem>();
					ContentItem currentPage = HttpContextItems["CurrentPage"] as ContentItem;
					if(currentPage != null)
						stack.Push(currentPage);
				}
				return stack;
			}
		}

		internal static IDictionary HttpContextItems
		{
			get
			{
				return HttpContext.Current.Items;
			}
		}

		public static IEnumerable<T> FindInChildren<T>(Control container)
			where T:class
		{
			foreach (Control child in container.Controls)
			{
				if (child is T)
					yield return child as T;
				foreach(T match in FindInChildren<T>(child))
					yield return match;
			}
		}

		public static T EnsureType<T>(ContentItem item)
			where T: ContentItem
		{
			if (item != null && !(item is T))
			{
				throw new N2Exception("Cannot cast the current page " + item
					+ " from type '" + item.GetContentType()
					+ "' to type '" + typeof(T)
					+ "' required by this template. It might help to change the generic argument of the template to something less explicit (like N2.ContentItem) or moving a user control to a page with the correct type or overriding the TemplateUrl property and referencing a more specific template.");
			}
			return (T)item;
		}
	}
}
