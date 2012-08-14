using System;

namespace N2.Definitions
{
	/// <summary>
	/// Describes a template for content
	/// </summary>
	public class TemplateDefinition
	{
		/// <summary>The name is used to refernce this template.</summary>
		public string Name { get; set; }

		/// <summary>Short description of the template.</summary>
		public string Title { get; set; }
		/// <summary>Longer description of the template.</summary>
		public string Description { get; set; }

		/// <summary>The url where the template can be observed.</summary>
		public string TemplateUrl { get; set; }

		/// <summary>A copy of the template.</summary>
		public ContentItem Template { get { return TemplateFactory(); } }

		/// <summary>A copy of the template.</summary>
		public Func<ContentItem> TemplateFactory { get; set; }

		/// <summary>A copy of the template.</summary>
		public ContentItem Original { get { return OriginalFactory(); } }

		/// <summary>The original template.</summary>
		public Func<ContentItem> OriginalFactory { get; set; }

		/// <summary>The item definition of the template.</summary>
		public ItemDefinition Definition { get; set; }

		/// <summary>Replace the defualt template.</summary>
		public bool ReplaceDefault { get; set; }

		#region Equals & GetHashCode

		public override bool Equals(object obj)
		{
			var other = obj as TemplateDefinition;
			if (other == null)
				return false;

			return other.Definition.Equals(Definition);
		}

		public override int GetHashCode()
		{
			return Definition.GetHashCode();
		}

		public override string ToString()
		{
			return Definition.ToString();
		}

		#endregion
	}
}
