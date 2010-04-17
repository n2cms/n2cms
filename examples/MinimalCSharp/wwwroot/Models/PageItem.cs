using N2;
using N2.Details;

namespace App.Models
{
	/// <summary>
	/// This is my custom content data container class. Since this class derives 
	/// from <see cref="N2.ContentItem"/> N2 will pick up this class and make it 
	/// available when we create pages in edit mode. 
	/// 
	/// Another thing to notice is that in addition to Text defined to be editable 
	/// in this class it's Title and Name are also editable. This is because of the 
	/// abstract base class <see cref="MyItemBase"/> it derives from.
	/// </summary>
	[PageDefinition("Default page")]
	[WithEditableName]
	public class PageItem : MyItemBase
	{
		/// <summary>
		/// There are code snippets to help generate these properties.
		/// Look in the snippets directory of the package.
		/// </summary>
		[EditableFreeTextArea("Text", 100)]
		public virtual string Text
		{
			get { return (string)GetDetail("Text"); }
			set { SetDetail("Text", value); }
		}
	}
}