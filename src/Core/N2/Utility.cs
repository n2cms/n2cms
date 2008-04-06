using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Resources;
using System.Web;
using N2.Integrity;
using System.Diagnostics;

namespace N2
{
	/// <summary>
	/// Mixed utility functions used by N2.
	/// </summary>
	public class Utility
	{
		private static string webRootPath = null;
		
		/// <summary>Gets the root path of the web application. e.g. "/" if the application doesn't run in a virtual directory.</summary>
		public static string WebRootPath
		{
			get { return webRootPath ?? (webRootPath = GetRootPath()); }
		}

		private static string GetRootPath()
		{
			try
			{
				return VirtualPathUtility.ToAbsolute("~/");
			}
			catch
			{
				return "/";
			}
		}

		/// <summary>Converts a value to a destination type.</summary>
		/// <param name="value">The value to convert.</param>
		/// <param name="destinationType">The type to convert the value to.</param>
		/// <returns>The converted value.</returns>
		public static object Convert(object value, Type destinationType)
		{
			if(value != null)
			{
				TypeConverter converter = TypeDescriptor.GetConverter(destinationType);
				if (converter != null && converter.CanConvertFrom(value.GetType()))
					return converter.ConvertFrom(value);
				else if (!destinationType.IsAssignableFrom(value.GetType()))
					return System.Convert.ChangeType(value, destinationType);
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
				throw new N2Exception("No property '{0}' found on the instance of type '{1}'.", propertyName, instanceType);
			if (value != null && !value.GetType().IsAssignableFrom(pi.PropertyType))
				value = Convert(value, pi.PropertyType);
			pi.SetValue(instance, value, new object[0]);
		}

		/// <summary>Gets a value from a property.</summary>
		/// <param name="instance">The object whose property to get.</param>
		/// <param name="propertyName">The name of the property to get.</param>
		/// <returns>The value of the property.</returns>
		public static object GetProperty(object instance, string propertyName)
		{
			if (instance == null) throw new ArgumentNullException("instance");
			if (propertyName == null) throw new ArgumentNullException("propertyName");

			Type instanceType = instance.GetType();
			PropertyInfo pi = instanceType.GetProperty(propertyName);

			if (pi == null)
				throw new N2Exception("No property '{0}' found on the instance of type '{1}'.", propertyName, instanceType);

			return pi.GetValue(instance, null);
		}

		/// <summary>Iterates items and ensures that the item's sort order is ascending.</summary>
		/// <param name="siblings">The items to iterate.</param>
		/// <returns>A list of items whose sort order was changed.</returns>
		public static IEnumerable<ContentItem> UpdateSortOrder(IEnumerable siblings)
		{
			List<ContentItem> updatedItems = new List<ContentItem>();
			int lastSortOrder = int.MinValue;
			foreach (ContentItem sibling in siblings)
			{
				if (sibling.SortOrder <= lastSortOrder)
				{
					sibling.SortOrder = ++lastSortOrder;
					updatedItems.Add(sibling);
				}
				else
					lastSortOrder = sibling.SortOrder;
			}
			return updatedItems;
		}

		/// <summary>Moves an item in a list to a new index.</summary>
		/// <param name="siblings">A list of items where the item to move is listed.</param>
		/// <param name="itemToMove">The item that should be moved (must be in the list)</param>
		/// <param name="newIndex">The new index onto which to place the item.</param>
		/// <remarks>To persist the new ordering one should call <see cref="Utility.UpdateSortOrder"/> and save the returned items. If the items returned from the <see cref="ContentItem.GetChildren"/> are moved with this method the changes will not be persisted since this is a new list instance.</remarks>
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

		/// <summary>Converts a possibly relative to an absolute url.</summary>
		/// <param name="url">The url to convert.</param>
		/// <returns>The absolute url.</returns>
		public static string ToAbsolute(string url)
		{
			if (!string.IsNullOrEmpty(url) && url[0] == '~' && url.Length > 1)
				return WebRootPath + url.Substring(2);
			return url;
		}

		/// <summary>Gets a global resource string.</summary>
		/// <param name="classKey">The name of the global resource file.</param>
		/// <param name="resourceKey">The key in the resource file.</param>
		/// <returns>The string if possible, otherwise null.</returns>
		public static string GetGlobalResourceString(string classKey, string resourceKey)
		{
			try
			{
				if (classKey != null && resourceKey != null && HttpContext.Current != null)
				{
					return HttpContext.GetGlobalResourceObject(classKey, resourceKey) as string;
				}
			}
			catch (MissingManifestResourceException)
			{
			}
			return null; // it's okay to use default text
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
					return HttpContext.GetLocalResourceObject(HttpContext.Current.Request.AppRelativeCurrentExecutionFilePath, resourceKey) as string;
				}
			}
			catch (InvalidOperationException)
			{
			}
			catch (MissingManifestResourceException)
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
	}
}