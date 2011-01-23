using System.Linq;
using System.Web.UI.WebControls;
using N2.Configuration;

namespace N2.Details
{
	public class EditableImageSizeAttribute : EditableDropDownAttribute
	{
		public EditableImageSizeAttribute()
			: this("Image size", 12)
		{
		}

		public EditableImageSizeAttribute(string title, int sortOrder)
			: base(title, sortOrder)
		{
		}

		protected override System.Web.UI.WebControls.ListItem[] GetListItems()
		{
			return Engine.Resolve<ConfigurationManagerWrapper>()
				.GetContentSection<EditSection>("edit").Images.Sizes
				.OfType<ImageSizeElement>()
				.Select(ise => new ListItem(GetText(ise), ise.Name))
				.ToArray();
		}

		private static string GetText(ImageSizeElement ise)
		{
			return ise.Name + " " + GetLengthText(ise.Width) + "," + GetLengthText(ise.Height);
		}

		private static string GetLengthText(int length)
		{
			return length == 0 ? "*" : length.ToString();
		}
	}
}
