using System;

namespace N2.Definitions.Static
{
	/// <summary>
	/// Describes key aspects of a class.
	/// </summary>
	public class Description
	{
		/// <summary>The type beeing described.</summary>
		public Type DescribedType { get; private set; }
		/// <summary>Whether the described type represents a page.</summary>
		public bool IsPage { get; set; }
		/// <summary>The icon associated with the described type.</summary>
		public string IconUrl { get; set; }

		public Description(Type describedType)
		{
			DescribedType = describedType;
			IsPage = true;
		}
	}
}
