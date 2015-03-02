using N2.Edit.Workflow;
using N2.Web.UI.WebControls;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;

namespace N2.Web.UI
{
    /// <summary>
    /// Some helpful methods that havn't found a better place yet (use at your own risk).
    /// </summary>
    public static class ItemUtility
    {
        public static Control Closest(this Control control, Func<Control, bool> predicate)
        {
            return Closest<Control>(control, predicate);
        }

        public static T Closest<T>(this Control control, Func<T, bool> predicate) where T : Control
        {
            if (control == null)
                return null;

            if (control is T && predicate((T)control))
            {
                return (T)control;
            }

            return Closest<T>(control.Parent, predicate);
        }

        public static T FindInParents<T>(Control parentControl) where T : class
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

        [Obsolete("Use Engine.ResolveAdapter<PartsAdapter>(page).AddChildPart(item, page);")]
        public static Control AddUserControl(Control container, ContentItem item)
        {
            PathData path = item.FindPath(PathData.DefaultAction);
            if (!path.IsEmpty())
            {
                return AddUserControl(path.TemplateUrl, container, item);
            }
            return null;
        }

        [Obsolete("Use Engine.ResolveAdapter<PartsAdapter>(page).AddChildPart(item, page);")]
        public static Control AddUserControl(string templateUrl, Control container, ContentItem item)
        {
            using (new ItemStacker(item))
            {
                Control templateItem = container.Page.LoadControl(Url.ResolveTokens(templateUrl));
                if (templateItem is IContentTemplate)
                    (templateItem as IContentTemplate).CurrentItem = item;
                container.Controls.Add(templateItem);
                return templateItem;
            }
        }

        public static void RegisterItemToSave(this Control descendantControl, ContentItem item)
        {
            var itemEditor = FindInParents<ItemEditor>(descendantControl);
            if (itemEditor == null)
                throw new ArgumentException("Couldn't find an associated ItemEditor control", "descendantControl");
            if (itemEditor.BinderContext == null)
                throw new ArgumentException("Couldn't find an active BinderContext on the ItemEditor control", "descendantControl");
            itemEditor.BinderContext.RegisterItemToSave(item);
        }

        internal class ItemStacker : IDisposable
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
            get { return ItemStack.Count > 0 ? ItemStack.Peek() : null; }
        }

        internal static Stack<ContentItem> ItemStack
        {
            get
            {
                var stack = HttpContextItems["ItemStack"] as Stack<ContentItem>;
                if (stack == null)
                {
                    HttpContextItems["ItemStack"] = stack = new Stack<ContentItem>();
                    var currentPage = HttpContextItems["CurrentPage"] as ContentItem;
                    if (currentPage != null)
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
            where T : class
        {
            foreach (Control child in container.Controls)
            {
                if (child is T)
                    yield return child as T;
                foreach (T match in FindInChildren<T>(child))
                    yield return match;
            }
        }

        public static T EnsureType<T>(ContentItem item)
            where T : ContentItem
        {
            if (item != null && !(item is T))
            {
                throw new N2Exception(string.Format("Cannot cast the current page {0} from type '{1}' to type '{2}' required by this template. It might help to change the generic argument of the template to something less explicit (like N2.ContentItem) or moving a user control to a page with the correct type or overriding the TemplateUrl property and referencing a more specific template.", item, item.GetContentType(), typeof(T)));
            }
            return (T)item;
        }
    }
}
