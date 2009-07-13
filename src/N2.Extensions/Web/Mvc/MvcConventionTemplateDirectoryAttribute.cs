using System;
using N2.Engine;

namespace N2.Web.Mvc
{
	/// <summary>
	/// Used in combination with the [MvcConventionTemplate] attribute to
	/// point out the location of content item templates. All content
	/// item types with a ConventionTemplate attribute in the same 
	/// assembly as this attribute are affected by this optin.
	/// </summary>
	[AttributeUsage(AttributeTargets.Assembly)]
	public sealed class MvcConventionTemplateDirectoryAttribute : Attribute
	{
		private string _viewLocationFormat;

		public MvcConventionTemplateDirectoryAttribute(string viewLocationFormat)
		{
			ViewLocationFormat = viewLocationFormat;
		}

		public string ViewLocationFormat
		{
			get { return _viewLocationFormat; }
			private set
			{
				if (value == null) throw new ArgumentNullException("value");
				if (!value.Contains("{0") || !value.Contains("{1"))
					throw new ArgumentException("ViewLocationFormat is invalid. It must include {0} (Item Name) and {1} (View Name). Supplied: " + value, "value");

				_viewLocationFormat = value;
			}
		}

		/// <summary>
		/// Gets the directory specified as convention for the type. By default this is
		/// the directory specified by an MvcConventionTemplateDirectoryAttribute in the 
		/// same assembly as the supplied type, or "~/Views/{0}/{1}.aspx" if no such attribute is 
		/// found.
		/// </summary>
		/// <param name="itemType">The type of item whose convention directory to get.</param>
		/// <returns>The virtual directory path for the specified item type.</returns>
		public static string GetDirectory(Type itemType)
		{
			if (itemType == null) throw new ArgumentNullException("itemType");

			MvcConventionTemplateDirectoryAttribute locationAttribute;
			if (SingletonDictionary<Type, MvcConventionTemplateDirectoryAttribute>.Instance.TryGetValue(itemType,
			                                                                                            out locationAttribute))
				return locationAttribute.ViewLocationFormat;

			object[] directoryAttributes = itemType.Assembly.GetCustomAttributes(
				typeof (MvcConventionTemplateDirectoryAttribute), false);
			if (directoryAttributes.Length > 0)
				SetDirectoryConvention(itemType, directoryAttributes[0] as MvcConventionTemplateDirectoryAttribute);
			else
				SetDirectoryConvention(itemType, new MvcConventionTemplateDirectoryAttribute("~/Views/{0}/{1}.aspx"));

			return GetDirectory(itemType);
		}

		public static void SetDirectoryConvention(Type itemType, MvcConventionTemplateDirectoryAttribute instance)
		{
			if (itemType == null) throw new ArgumentNullException("itemType");
			if (instance == null) throw new ArgumentNullException("instance");

			SingletonDictionary<Type, MvcConventionTemplateDirectoryAttribute>.Instance[itemType] = instance;
		}
	}
}