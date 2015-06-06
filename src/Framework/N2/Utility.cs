using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using N2.Collections;
using N2.Engine;
using N2.Integrity;
using N2.Persistence.NH;
using NHibernate;
using N2.Web;
using N2.Edit;
using N2.Details;

namespace N2
{
    /// <summary>
    /// Mixed utility functions used by N2.
    /// </summary>
    public static class Utility
    {
	    public class String2Version : TypeConverter
	    {
		    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		    {
			    if (sourceType == typeof (String))
				    return true;
			    return false;
		    }

		    public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		    {
			    return (destinationType == typeof (Version));
		    }

		    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		    {
				if (value == null)
					throw new ArgumentNullException("value");
				return Version.Parse(value.ToString());
		    }

		    public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		    {
				if (value == null)
					throw new ArgumentNullException("value");
			    if (destinationType == typeof (Version))
				    return Version.Parse(value.ToString());
				throw new NotSupportedException("destinationType isn't supported, can only convert to System.Version");
		    }
	    }

        /// <summary>A global settings indicating whether persistence events should be triggered for versions of items.</summary>
        public static bool VersionsTriggersEvents { get; set; }

        /// <summary>Converts a value to a destination type.</summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="destinationType">The type to convert the value to.</param>
        /// <returns>The converted value.</returns>
        public static object Convert(object value, Type destinationType)
        {
            if (value != null)
            {
                TypeConverter destinationConverter = TypeDescriptor.GetConverter(destinationType);
                TypeConverter sourceConverter = TypeDescriptor.GetConverter(value.GetType());
                if (destinationConverter != null && destinationConverter.CanConvertFrom(value.GetType()))
                    return destinationConverter.ConvertFrom(value);
                if (sourceConverter != null && sourceConverter.CanConvertTo(destinationType))
                    return sourceConverter.ConvertTo(value, destinationType);
                if (destinationType.IsEnum && value is int)
                    return Enum.ToObject(destinationType, (int)value);
                if (!destinationType.IsAssignableFrom(value.GetType()))
                    try
                    {
                        return System.Convert.ChangeType(value, destinationType);
                    }
                    catch (InvalidCastException)
                    {
                        if (value is string)
                        {
                            var parseMethod = destinationType.GetMethods(BindingFlags.Static | BindingFlags.Public)
                                .Where(m => m.Name == "Parse")
                                .Where(m => m.GetParameters().Length == 1)
                                .Where(m => m.GetParameters()[0].ParameterType == typeof(string))
                                .FirstOrDefault();
                            if (parseMethod != null && parseMethod.GetParameters().Length == 1 && parseMethod.GetParameters()[0].ParameterType == typeof(string))
                                return parseMethod.Invoke(null, new[] { value });
                            var constructor = destinationType.GetConstructors()
                                .Where(c => c.GetParameters().Length == 1)
                                .Where(c => c.GetParameters()[0].ParameterType == typeof(string))
                                .FirstOrDefault();
                            if (constructor != null)
                                return constructor.Invoke(new[] { value });
                        }
                        throw;
                    }
            }
            return value;
        }

        /// <summary>Converts a value to a destination type.</summary>
        /// <param name="value">The value to convert.</param>
        /// <typeparam name="T">The type to convert the value to.</typeparam>
        /// <returns>The converted value.</returns>
        public static T Convert<T>(object value)
        {
            return (T)Convert(value, typeof (T));
        }

        /// <summary>Tries to find a property matching the supplied expression, returns null if no property is found with the first part of the expression.</summary>
        /// <param name="item">The object to query.</param>
        /// <param name="expression">The expression to evaluate.</param>
        public static object Evaluate(object item, string expression)
        {
            if (item == null) return null;

            PropertyInfo info = item.GetType().GetProperty(expression);
            if (info != null)
                return info.GetValue(item, new object[0]);
            else if (expression.IndexOf('.') > 0)
            {
                int dotIndex = expression.IndexOf('.');
                object obj = Evaluate(item, expression.Substring(0, dotIndex));
                if (obj != null)
                    return Evaluate(obj, expression.Substring(dotIndex + 1, expression.Length - dotIndex - 1));
            }
            return null;
        }

        /// <summary>Evaluates an expression and applies a format string.</summary>
        /// <param name="item">The object to query.</param>
        /// <param name="expression">The expression to evaluate.</param>
        /// <param name="format">The format string to apply.</param>
        /// <returns>The formatted result ov the evaluation.</returns>
        public static string Evaluate(object item, string expression, string format)
        {
            return string.Format(format, Evaluate(item, expression));
        }

        /// <summary>Gets the type from a string.</summary>
        /// <param name="name">The type name string.</param>
        /// <returns>The type.</returns>
        public static Type TypeFromName(string name)
        {
            if (name == null) throw new ArgumentNullException("name");

            Type t = Type.GetType(name);
            if(t == null)
                throw new N2Exception("Couldn't find any type with the name '{0}'", name);
            return t;
        }

        /// <summary>Sets a property on an object to a valuae.</summary>
        /// <param name="instance">The object whose property to set.</param>
        /// <param name="propertyName">The name of the property to set.</param>
        /// <param name="value">The value to set the property to.</param>
        public static void SetProperty(object instance, string propertyName, object value)
        {
            if (instance == null) throw new ArgumentNullException("instance");
            if (propertyName == null) throw new ArgumentNullException("propertyName");

            Type instanceType = instance.GetType();
            PropertyInfo pi = instanceType.GetProperty(propertyName);
            if (pi == null)
            {
                var dotIndex = propertyName.IndexOf('.');
                if (dotIndex > 0)
                {
                    var subObject = GetProperty(instance, propertyName.Substring(0, dotIndex));
                    if (subObject != null)
                    {
                        SetProperty(subObject, propertyName.Substring(dotIndex + 1), value);
                        return;
                    }
                }
                throw new N2Exception("No property '{0}' found on the instance of type '{1}'.", propertyName, instanceType);
            }
            if(!pi.CanWrite)
                throw new N2Exception("The property '{0}' on the instance of type '{1}' does not have a setter.", propertyName, instanceType);
            if (value != null && !value.GetType().IsAssignableFrom(pi.PropertyType))
                value = Convert(value, pi.PropertyType);
            pi.SetValue(instance, value, new object[0]);
        }

        /// <summary>Sets a property on an object to a valuae.</summary>
        /// <param name="instance">The object whose property to set.</param>
        /// <param name="propertyName">The name of the property to set.</param>
        /// <param name="value">The value to set the property to.</param>
        public static bool TrySetProperty(object instance, string propertyName, object value)
        {
            if (instance == null) return false;
            if (propertyName == null) return false;

            PropertyInfo pi = GetPropertyInfo(ref instance, propertyName);
            
            if (pi == null) return false;
            if (!pi.CanWrite) return false;

            if (value != null && !value.GetType().IsAssignableFrom(pi.PropertyType))
                value = Convert(value, pi.PropertyType);

            try
            {
                pi.SetValue(instance, value, new object[0]);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static PropertyInfo GetPropertyInfo(ref object instance, string propertyName)
        {
            Type instanceType = instance.GetType();

            PropertyInfo pi = instanceType.GetProperty(propertyName);
            if (pi == null)
            {
                var dotIndex = propertyName.IndexOf('.');
                if (dotIndex > 0)
                {
                    instance = GetProperty(instance, propertyName.Substring(0, dotIndex));
                    if (instance == null)
                        return null;

                    return GetPropertyInfo(ref instance, propertyName.Substring(dotIndex + 1));
                }
            }
            return pi;
        }

        /// <summary>Gets a value from a property.</summary>
        /// <param name="instance">The object whose property to get.</param>
        /// <param name="propertyName">The name of the property to get.</param>
        /// <returns>The value of the property.</returns>
        public static object GetProperty(object instance, string propertyName)
        {
            if (instance == null) throw new ArgumentNullException("instance");
            if (propertyName == null) throw new ArgumentNullException("propertyName");

            var originalInstance = instance;
            var pi = GetPropertyInfo(ref instance, propertyName);

            if (pi == null)
                return null;
            //  throw new N2Exception("No property '{0}' found on the instance of type '{1}'.", propertyName, originalInstance.GetType());

            return pi.GetValue(instance, null);
        }

        /// <summary>Iterates items and ensures that the item's sort order is ascending.</summary>
        /// <param name="siblings">The items to iterate.</param>
        /// <returns>A list of items whose sort order was changed.</returns>
        public static IEnumerable<ContentItem> UpdateSortOrder(IEnumerable<ContentItem> siblings)
        {
            List<ContentItem> updatedItems = new List<ContentItem>();
            int lastSortOrder = int.MinValue;
            int sortOrderBeforeLast = int.MinValue;
            ContentItem last = null;
            bool lastSortOrderChanged = false;
            int index = 0;
            foreach (ContentItem current in siblings)
            {
                if (current.SortOrder <= lastSortOrder)
                {
                    int gapBeforeLast = (int)Math.Min((long)lastSortOrder - sortOrderBeforeLast, int.MaxValue);
                    int gapBeforeCurrent = (int)Math.Min((long)current.SortOrder - sortOrderBeforeLast, int.MaxValue);
                    if (gapBeforeLast > 1 && gapBeforeCurrent > 2)
                    {
                        if (index == 1)
                        {
                            // we added something first with a higher sortorder than the next
                            last.SortOrder = lastSortOrder = current.SortOrder - 10;
                            updatedItems.Add(last);
                        }
                        else if (current.SortOrder > sortOrderBeforeLast)
                        {
                            // 0
                            // -1b
                            last.SortOrder = lastSortOrder = current.SortOrder - (current.SortOrder - sortOrderBeforeLast) / 2;
                            updatedItems.Add(last);
                        }
                        else
                        {
                            // 0
                            // -2b
                            last.SortOrder = lastSortOrder = sortOrderBeforeLast + 1;
                            updatedItems.Add(last);
                            current.SortOrder = sortOrderBeforeLast + 2;
                            updatedItems.Add(current);
                            lastSortOrderChanged = true;
                        }
                    }
                    else
                    {
                        current.SortOrder = lastSortOrder + 1;
                        updatedItems.Add(current);
                        lastSortOrderChanged = true;
                    }
                }
                else
                    lastSortOrderChanged = false;

                sortOrderBeforeLast = lastSortOrder;
                lastSortOrder = current.SortOrder;
                last = current;
                index++;
            }
            if (lastSortOrderChanged)
                last.SortOrder += 9;

            return updatedItems;
        }

        /// <summary>Moves an item in a list to a new index.</summary>
        /// <param name="siblings">A list of items where the item to move is listed.</param>
        /// <param name="itemToMove">The item that should be moved (must be in the list)</param>
        /// <param name="newIndex">The new index onto which to place the item.</param>
        /// <remarks>To persist the new ordering one should call <see cref="Utility.UpdateSortOrder"/> and save the returned items. If the items returned from the <see cref="ContentItem.GetChildren()"/> are moved with this method the changes will not be persisted since this is a new list instance.</remarks>
        public static void MoveToIndex(IList<ContentItem> siblings, ContentItem itemToMove, int newIndex)
        {
            siblings.Remove(itemToMove);
            siblings.Insert(newIndex, itemToMove);
        }

        /// <summary>Inserts an item among a parent item's children using a comparer to determine the location.</summary>
        /// <param name="item">The item to insert.</param>
        /// <param name="newParent">The parent item.</param>
        /// <param name="sortExpression">The sort expression to use.</param>
        /// <returns>The index of the item among it's siblings.</returns>
        public static int Insert(ContentItem item, ContentItem newParent, string sortExpression)
        {
            return Insert(item, newParent, new Collections.ItemComparer(sortExpression));
        }

        /// <summary>Inserts an item among a parent item's children using a comparer to determine the location.</summary>
        /// <param name="item">The item to insert.</param>
        /// <param name="newParent">The parent item.</param>
        /// <param name="comparer">The comparer to use.</param>
        /// <returns>The index of the item among it's siblings.</returns>
        public static int Insert(ContentItem item, ContentItem newParent, IComparer<ContentItem> comparer)
        {
            if (item.Parent != null && item.Parent.Children.Contains(item))
                item.Parent.Children.Remove(item);

            item.Parent = newParent;
            if (newParent != null)
            {
                if (IsDestinationBelowSource(item, newParent))
                {
                    throw new DestinationOnOrBelowItselfException(item, newParent);
                }

                IList<ContentItem> siblings = newParent.Children;
                for (int i = 0; i < siblings.Count; i++)
                {
                    if (comparer.Compare(item, siblings[i]) < 0)
                    {
                        siblings.Insert(i, item);
                        return i;
                    }
                }
                siblings.Add(item);
                return siblings.Count - 1;
            }
            return -1;
        }

        /// <summary>Inserts an item among a parent item's children using a comparer to determine the location.</summary>
        /// <param name="item">The item to insert.</param>
        /// <param name="newParent">The parent item.</param>
        /// <param name="index">The index where to insert the item.</param>
        /// <returns>The index of the item among it's siblings.</returns>
        public static void Insert(ContentItem item, ContentItem newParent, int index)
        {
            if (item == null) throw new ArgumentNullException("item");
            if (newParent == null) throw new ArgumentNullException("newParent");
            if(index < 0 || index > newParent.Children.Count) throw  new ArgumentOutOfRangeException("index");

            if(item.Parent != null && item.Parent == newParent)
            {
                int previousIndex = newParent.Children.IndexOf(item);
                if (previousIndex < index)
                    --index;
                MoveToIndex(item.Parent.Children, item, index);
            }
            else
            {
                if (item.Parent != null)
                    item.Parent.Children.Remove(item);
                item.Parent = newParent;
                newParent.Children.Insert(index, item);
            }
        }

        /// <summary>Checks that the destination isn't below the source.</summary>
        private static bool IsDestinationBelowSource(ContentItem source, ContentItem destination)
        {
            if(source == destination)
                return true;
            foreach(ContentItem ancestor in Find.EnumerateParents(destination))
                if (ancestor == source)
                    return true;
            return false;
        }

        /// <summary>Gets a global resource string.</summary>
        /// <param name="classKey">The name of the global resource file.</param>
        /// <param name="resourceKey">The key in the resource file.</param>
        /// <returns>The string if possible, otherwise null.</returns>
        public static string GetGlobalResourceString(string classKey, string resourceKey)
        {
            if (classKey != null && resourceKey != null && HttpContext.Current != null)
            {
                string cultureName = Thread.CurrentThread.CurrentUICulture.Name;
                ResourceKey key = new ResourceKey(classKey, resourceKey, cultureName);
                try
                {
                    if (SingletonDictionary<ResourceKey, string>.Instance.ContainsKey(key))
                        return SingletonDictionary<ResourceKey, string>.Instance[key];

                    return HttpContext.GetGlobalResourceObject(classKey, resourceKey) as string;
                }
                catch (MissingManifestResourceException)
                {
                    if (SingletonDictionary<ResourceKey, string>.Instance.ContainsKey(key)) // fixes NullReferenceException
                        SingletonDictionary<ResourceKey, string>.Instance[key] = null;
                }
            }
            return null; // it's okay to use default text
        }

        /// <summary>Maps a physical path back to a virtual path, thanks to ahmadh21.</summary>
        /// <param name="physicalPath"></param>
        /// <returns></returns>
        public static string RemapVirtualPath(string physicalPath)
        {
            physicalPath = physicalPath.Replace('\\', '/').TrimEnd('/', '\\');
            var appPhysicalPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath.Replace('\\', '/').TrimEnd('/', '\\');
            var appVirtualPath = System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath.Replace('\\', '/').TrimEnd('/', '\\');

            if (!physicalPath.StartsWith(appPhysicalPath, StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("Physical path given must map below application physical path");
            
            var finalPath = physicalPath.Substring(appPhysicalPath.Length);

            if (appVirtualPath.Length > 1)
            {
                finalPath = appVirtualPath + finalPath;
            }

            return finalPath;
        }


        public static IEnumerable<string> ListFiles(string physicalBasePath, string filter, bool recursive = true)
        {
            var toExplore = new List<string> { physicalBasePath };
			var files = new List<string>();
            while (toExplore.Count > 0)
            {
				if (System.IO.Directory.Exists(toExplore[0]))
				{
					files.AddRange(System.IO.Directory.GetFiles(toExplore[0], filter));
					if (recursive)
						toExplore.AddRange(System.IO.Directory.GetDirectories(toExplore[0]));
				}
                toExplore.RemoveAt(0);
            }
			return files;
        }

        /// <summary>
        /// Somewhat convoluted code to avoid a few exceptions.
        /// </summary>
        private class ResourceKey
        {
            readonly string classKey;
            readonly string resourceKey;
            readonly string cultureName;

            public ResourceKey(string classKey, string resourceKey, string cultureName)
            {
                this.classKey = classKey ?? "";
                this.resourceKey = resourceKey ?? "";
                this.cultureName = cultureName ?? "";
            }

            public override bool Equals(object obj)
            {
                ResourceKey other = obj as ResourceKey;
                return other != null
                       && other.classKey == classKey
                       && other.resourceKey == resourceKey
                       && other.cultureName == cultureName;
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return classKey.GetHashCode()
                           + resourceKey.GetHashCode()
                           + cultureName.GetHashCode();
                }
            }
        }

        /// <summary>Gets a local resource string.</summary>
        /// <param name="resourceKey">The key in the resource file.</param>
        /// <returns>The string if possible, otherwise null.</returns>
        public static string GetLocalResourceString(string resourceKey)
        {
            try
            {
                if (resourceKey != null && HttpContext.Current != null)
                {
                    var path = HttpContext.Current.Request.AppRelativeCurrentExecutionFilePath;
                    if (path.EndsWith("/"))
                        path += Url.DefaultDocument;
                    return HttpContext.GetLocalResourceObject(path, resourceKey) as string;
                }
            }
            catch (InvalidOperationException)
            {
            }
            catch (MissingManifestResourceException)
            {
            }
            catch (NullReferenceException)
            {
            }
            return null; // it's okay to use default text
        }

        /// <summary>Tries to get a global or local resource string.</summary>
        /// <param name="classKey">The name of the global resource file.</param>
        /// <param name="resourceKey">The key in the resource file.</param>
        /// <returns>The string if possible, otherwise null.</returns>
        public static string GetResourceString(string classKey, string resourceKey)
        {
            if(classKey != null)
                return GetGlobalResourceString(classKey, resourceKey) ?? GetLocalResourceString(resourceKey);
            else
                return GetLocalResourceString(resourceKey);
        }

		public static bool UseUniversalTime { get; set; }
		public static Func<DateTime> CurrentTime = () => UseUniversalTime ? DateTime.UtcNow : DateTime.Now;
		public static Func<DateTime, DateTime> ToUtc = (contentTime) => UseUniversalTime ? contentTime : contentTime.ToUniversalTime();

        public static IDisposable TimeCapsule(DateTime frozenTime)
        {
            var previous = CurrentTime;
            CurrentTime = () => frozenTime;
            return new Engine.Globalization.Scope(() => CurrentTime = previous);
        }

        [Obsolete("Moved to N2.Web.Url.ToAbsolute")]
        public static string ToAbsolute(string relativePath)
        {
            return Url.ToAbsolute(relativePath);
        }

        /// <summary>Invokes an event and and executes an action unless the event is cancelled.</summary>
        /// <param name="preHandlers">The event handler to signal.</param>
        /// <param name="item">The item affected by this operation.</param>
        /// <param name="sender">The source of the event.</param>
        /// <param name="finalAction">The default action to execute if the event didn't signal cancel.</param>
        public static void InvokeEvent(EventHandler<CancellableItemEventArgs> preHandlers, ContentItem item, object sender, Action<ContentItem> finalAction, EventHandler<ItemEventArgs> postHandlers)
        {
            if (preHandlers != null && (VersionsTriggersEvents || !item.VersionOf.HasValue))
            {
                CancellableItemEventArgs args = new CancellableItemEventArgs(item, finalAction);

                preHandlers.Invoke(sender, args);

                if (!args.Cancel)
                {
                    args.FinalAction(args.AffectedItem);
                    if (postHandlers != null)
                        postHandlers(sender, args);
                }
            }
            else
            {
                finalAction(item);

                if (postHandlers != null)
                    postHandlers(sender, new ItemEventArgs(item));
            }
        }

        /// <summary>Invokes an event and and executes an action unless the event is cancelled.</summary>
        /// <param name="handler">The event handler to signal.</param>
        /// <param name="source">The item affected by this operation.</param>
        /// <param name="destination">The destination of this operation.</param>
        /// <param name="sender">The source of the event.</param>
        /// <param name="finalAction">The default action to execute if the event didn't signal cancel.</param>
        /// <returns>The result of the action (if any).</returns>
        public static ContentItem InvokeEvent(EventHandler<CancellableDestinationEventArgs> handler, object sender, ContentItem source, ContentItem destination, Func<ContentItem, ContentItem, ContentItem> finalAction, EventHandler<DestinationEventArgs> postHandler)
        {
            if (handler != null && (VersionsTriggersEvents || !source.VersionOf.HasValue))
            {
                CancellableDestinationEventArgs args = new CancellableDestinationEventArgs(source, destination, finalAction);

                handler.Invoke(sender, args);

                if (args.Cancel)
                    return null;

                args.AffectedItem = args.FinalAction(args.AffectedItem, args.Destination);
                if (postHandler != null)
                    postHandler(sender, args);
                return args.AffectedItem;
            }
            
            var result2 = finalAction(source, destination);
            if (postHandler != null)
                postHandler(sender, new DestinationEventArgs(result2, destination));
            return result2;
        }

        /// <summary>Gets the trail to a certain item. A trail is a slash-separated sequence of IDs, e.g. "/1/6/12/".</summary>
        /// <param name="item">The item whose trail to get.</param>
        /// <returns>The trail leading to the item. The trail contains the item's ID.</returns>
        public static string GetTrail(this ContentItem item)
        {
            if (item == null)
                return "/";

            string ancestralTrail = item.AncestralTrail ?? GetTrail(item.Parent);
            return ancestralTrail + item.ID + "/";
        }

		public static IEnumerable<ContentItem> UpdateAncestralTrailRecursive(ContentItem item, ContentItem parent)
		{
			item.AncestralTrail = parent.GetTrail();
			yield return item;

			foreach (var child in item.Children)
				foreach (var descendant in UpdateAncestralTrailRecursive(child, item))
					yield return descendant;
		}

        /// <summary>Gets the base types of a given item.</summary>
        /// <param name="type">The type whose base types to get.</param>
        /// <returns>The base types of the type.</returns>
        public static IEnumerable<Type> GetBaseTypes(Type type)
        {
            if (type == null || type.IsInterface || type.IsValueType)
                return new Type[0];

            return GetBaseTypesAndSelf(type.BaseType);
        }

        /// <summary>Gets the base types of a given item.</summary>
        /// <param name="type">The type whose base types to get.</param>
        /// <returns>The base types of the type.</returns>
        public static IEnumerable<Type> GetBaseTypesAndSelf(Type type)
        {
            if (type == null || type.IsInterface || type.IsValueType)
                yield break;
            
            while (type != null)
            {
                yield return type;
                type = type.BaseType;
            }
        }

        private static AspNetHostingPermissionLevel? trustLevel;
        /// <summary>
        /// Finds the trust level of the running application (http://blogs.msdn.com/dmitryr/archive/2007/01/23/finding-out-the-current-trust-level-in-asp-net.aspx)
        /// </summary>
        /// <returns>The current trust level.</returns>
        internal static AspNetHostingPermissionLevel GetTrustLevel()
        {
            if (trustLevel.HasValue)
                return trustLevel.Value;

            foreach (AspNetHostingPermissionLevel level in new[] { AspNetHostingPermissionLevel.Unrestricted, AspNetHostingPermissionLevel.High, AspNetHostingPermissionLevel.Medium })
            {
                try
                {
                    new AspNetHostingPermission(level).Demand();
                }
                catch (System.Security.SecurityException)
                {
                    continue;
                }

                trustLevel = level;
                return level;
            }

            return AspNetHostingPermissionLevel.None;
        }

        static readonly Regex StripTagsExpression = new Regex("<!*[^<>]*>", RegexOptions.Multiline);
        /// <summary>Strips tags and extracts sentences uptil the given max string length.</summary>
        /// <param name="html">The html to strip and substring.</param>
        /// <param name="maxLength">The maximum length of the returned string.</param>
        /// <returns>A string with complete sentences uptil the supplied max length.</returns>
        public static string ExtractFirstSentences(string html, int maxLength)
        {
            if (string.IsNullOrEmpty(html)) return html;

            html = StripTagsExpression.Replace(html, "");
            if (string.IsNullOrEmpty(html)) return html;

            int separatorIndex = 0;
            for (int i = 0; i < html.Length && i < maxLength; i++)
            {
                switch (html[i])
                {
                    case '.':
                    case '!':
                    case '?':
                        separatorIndex = i;
                        break;
                    default:
                        break;
                }
            }

            if (separatorIndex == 0 && html.Length > 0)
                return html.Substring(0, Math.Min(html.Length, maxLength));

            return html.Substring(0, separatorIndex + 1);
        }

        public static int InheritanceDepth(Type type)
        {
            if (type == null)
                return -1;
            if (type == typeof(object))
                return 0;

            return Math.Max(
                1 + InheritanceDepth(type.BaseType), 
                type.GetInterfaces().Select(t => 1 + InheritanceDepth(t)).OrderByDescending(d => d).FirstOrDefault());
        }

        /// <summary>Shorthand for resolving an adapter.</summary>
        /// <typeparam name="T">The type of adapter to get.</typeparam>
        /// <param name="engine">Used to resolve the provider.</param>
        /// <param name="item">The item whose adapter to get.</param>
        /// <returns>The most relevant adapter.</returns>
        internal static T GetContentAdapter<T>(this IEngine engine, ContentItem item) where T : AbstractContentAdapter
        {
            return engine.Resolve<IContentAdapterProvider>().ResolveAdapter<T>(item);
        }

        /// <summary>Shorthand for resolving a node adapter.</summary>
        /// <param name="engine">Used to resolve the provider.</param>
        /// <param name="item">The item whose adapter to get.</param>
        /// <returns>The most relevant adapter.</returns>
        internal static NodeAdapter GetNodeAdapter(this IEngine engine, ContentItem item)
        {
            return engine.GetContentAdapter<NodeAdapter>(item);
        }

        /// <summary>Renders a file size (in bytes) in MB (base 10) or MiB (base 2).</summary>
        /// <param name="p">File size in bytes</param>
        /// <returns></returns>
        public static string GetFileSizeString(long p, bool base10)
        {
            if (base10)
            {
                if (p >= 1000000)
                    return string.Format("{0:N1} MB", (double)p / 1000000.0);
                if (p >= 4000)
                    return string.Format("{0:N1} KB", (double)p / 1000.0);
                else 
                    return string.Format("{0:N0} bytes", (double)p);
            }
            else
            {
                if (p >= 1048576)
                    return string.Format("{0:N1} MiB", (double)p / 1048576.0);
                if (p >= 4096)
                    return string.Format("{0:N1} KiB", (double)p / 1024.0);
                else
                    return string.Format("{0:N0} bytes", (double)p);
            }
        }

        /// <summary>Shorthand for resolving an object via an IProvider.</summary>
        /// <typeparam name="T">The type of object to retrieve.</typeparam>
        /// <param name="engine">Used to resolve the provider.</param>
        /// <returns>The the provided value.</returns>
        internal static T GetProviderInstance<T>(this IEngine engine) where T : class
        {
            return engine.Resolve<IProvider<T>>().Get();
        }

        /// <summary>Tries to retrieve the engine provided by an accessor on the page, or falls back to the global singleton.</summary>
        /// <param name="page">The accessor from which to retrieve the engine.</param>
        /// <returns>The content engine.</returns>
        internal static IEngine Engine(this System.Web.UI.Page page)
        {
            var accessor = page as IProvider<IEngine>;
            if (accessor != null)
                return accessor.Get();
            else
                return Context.Current;
        }

        // NH Extensions

        public static ICriteria Criteria<T>(this SessionContext sc) where T : class
        {
            return sc.Session.CreateCriteria<T>();
        }

        public static IMultiCriteria MultiCriteria<T>(this SessionContext sc)
        {
            return sc.Session.CreateMultiCriteria();
        }

        public static IQuery Query(this SessionContext sc, string queryString)
        {
            return sc.Session.CreateQuery(queryString);
        }

        public static IMultiQuery MultiQuery(this SessionContext sc)
        {
            return sc.Session.CreateMultiQuery();
        }

        // Content Extensions

        public static bool IsPublished(this ContentItem item)
        {
            if (item == null)
                return false;

            switch (item.State)
            {
                case ContentState.New:
                case ContentState.None:
                case ContentState.Published:
                    return item.Published.HasValue && item.Published <= Utility.CurrentTime()
                        && (!item.Expires.HasValue || Utility.CurrentTime() < item.Expires.Value);
                default:
                    return false;
            }
        }
        public static bool IsExpired(this ContentItem item)
        {
            if (item == null)
                return false;

            return item.State == ContentState.Unpublished || (item.Expires.HasValue && item.Expires.Value < Utility.CurrentTime());
        }

        public static bool SetPropertyOrDetail(ContentItem item, string detailName, object value)
        {
            object existing = item[detailName];
            if (existing == null && value == null)
                return false;

            if ((existing == null && value != null) || (existing != null && value == null))
            {
                item[detailName] = value;
                return true;
            }

            if (value.Equals(existing))
                return false;

            item[detailName] = value;
            return true;
        }

        /// <summary>Formats a string using properties from the value object.</summary>
        /// <param name="format">A format string, e.g. Hello {Title}.</param>
        /// <param name="values">A value object, e.g. new { Title = "Hello" }</param>
        /// <returns>The format string with any format placeholders replaced by value properties.</returns>
        public static string Format(string format, object values)
        {
            return Regex.Replace(format, "{([\\w\\.]+)}", m => (string)Utility.Evaluate(values, m.Groups[1].Value), RegexOptions.Compiled);
        }

        internal static List<INameable> FindEmpty(ContentItem item)
        {
            var list = new List<INameable>();
            if (item.Parent != null && item.Parent.ID == 0)
                list.Add(item);
            foreach (var detail in item.Details)
                if (detail.EnclosingItem.ID == 0 && !object.ReferenceEquals(detail.EnclosingItem, item))
                    list.Add(detail);
                else if (detail.LinkedItem.HasValue && detail.LinkedItem.ID == 0)
                    list.Add(detail);
            foreach (var dc in item.DetailCollections)
                foreach (var detail in dc.Details)
                    if (detail.EnclosingItem.ID == 0 && !object.ReferenceEquals(detail.EnclosingItem, item))
                        list.Add(detail);
                    else if (detail.LinkedItem.HasValue && detail.LinkedItem.ID == 0)
                        list.Add(detail);

            foreach (var child in item.Children.FindParts())
                list.AddRange(FindEmpty(child));

            return list;
        }

        public static T ResolveAdapter<T>(this IEngine engine, ContentItem item) where T : AbstractContentAdapter
        {
            return engine.Resolve<IContentAdapterProvider>().ResolveAdapter<T>(item);
        }

        public static IDictionary<string, object> ToDictionary(this ContentItem item)
        {
            return new ContentDictionary(item);
        }

        class ContentDictionary : IDictionary<string, object>
        {
            private ContentItem item;

            public ContentDictionary(ContentItem item)
            {
                this.item = item;
            }

            public void Add(string key, object value)
            {
                item[key] = value;
            }

            public bool ContainsKey(string key)
            {
                return item[key] != null;
            }

            public ICollection<string> Keys
            {
                get { return ContentItem.KnownProperties.WritableProperties.Union(item.Details.Select(d => d.Name)).Union(item.DetailCollections.Select(dc => dc.Name)).ToList(); }
            }

            public bool Remove(string key)
            {
                if (!ContainsKey(key))
                    return false;

                item[key] = null;
                return true;
            }

            public bool TryGetValue(string key, out object value)
            {
                try
                {
                    value = item[key];
                    return value != null;
                }
                catch (Exception)
                {
                    value = null;
                    return false;
                }
            }

            public ICollection<object> Values
            {
                get 
                {
                    return ContentItem.KnownProperties.WritableProperties.Select(wp => WriteProperty(wp))
                        .Union(item.Details.Select(d => WriteDetailValue(d)))
                        .Union(item.DetailCollections.Select(dc => dc.Details.Select(d => WriteDetailValue(d)).ToList()))
                        .ToList();
                }
            }

            private object WriteProperty(string wp)
            {
                if (wp == "Parent")
                    return item.Parent != null
                        ? item.Parent.ID
                        : 0;
                else
                    return item[wp];
            }

            public object this[string key]
            {
                get { return item[key]; }
                set { item[key] = value; }
            }

            public void Add(KeyValuePair<string, object> item)
            {
                this.item[item.Key] = item.Value;
            }

            public void Clear()
            {
                item.Details.Clear();
                item.DetailCollections.Clear();
            }

            public bool Contains(KeyValuePair<string, object> item)
            {
                return this.item[item.Key] != null && this.item[item.Key] == item.Value;
            }

            public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
            {
                var kvps = this.ToList();
                for (int i = 0; i < kvps.Count; i++)
                {
                    array[i + arrayIndex] = kvps[i];
                }
            }

            public int Count
            {
                get { return Keys.Count; }
            }

            public bool IsReadOnly
            {
                get { return false; }
            }

            public bool Remove(KeyValuePair<string, object> item)
            {
                if (!Contains(item))
                    return false;
                this.item[item.Key] = null;
                return true;
            }

            public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
            {
                return ContentItem.KnownProperties.WritableProperties.Select(wp => new KeyValuePair<string, object>(wp, WriteProperty(wp)))
                    .Union(item.Details.Select(d => new KeyValuePair<string, object>(d.Name, WriteDetailValue(d))))
                    .Union(item.DetailCollections.Select(dc => new KeyValuePair<string, object>(dc.Name, dc.Details.Select(d => WriteDetailValue(d)).ToList())))
                    .ToList()
                    .GetEnumerator();
            }

            private object WriteDetailValue(Details.ContentDetail d)
            {
	            return d.ValueTypeKey == ContentDetail.TypeKeys.LinkType ? d.LinkedItem.ID : d.Value;
            }

	        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

        }

		internal static void AppendHashCode(ref int currentHashCode, object nextComponent)
		{
			currentHashCode *= 23;
			if (nextComponent != null)
				currentHashCode += nextComponent.GetHashCode();
		}

		internal static bool Compare(object first, object second)
		{
			if (first != null)
				return first.Equals(second);

			return second == null;
		}

		public static IEnumerable<T> Filter<T>(this IEnumerable<T> items, params ItemFilter[] filters)
			where T : ContentItem
		{
			return new ItemList<T>(items, filters);
		}

		public static ItemList ToContentList(this IEnumerable<ContentItem> items)
		{
			return new ItemList(items);
		}

		public static ItemList<T> ToContentList<T>(this IEnumerable<T> items)
			where T: ContentItem
		{
			return new ItemList<T>(items);
		}

		public static ItemList<T> WhereNavigatable<T>(this IContentList<T> items, System.Security.Principal.IPrincipal byUser = null, N2.Security.ISecurityManager security = null)
			where T : ContentItem
		{
			return new ItemList<T>(items, new NavigationFilter(byUser, security));
		}

		public static ItemList<T> WhereAccessible<T>(this IContentList<T> items, System.Security.Principal.IPrincipal byUser = null, N2.Security.ISecurityManager security = null)
			where T : ContentItem
		{
			return new ItemList<T>(items, new AccessFilter(byUser, security));
		}
	}
}
