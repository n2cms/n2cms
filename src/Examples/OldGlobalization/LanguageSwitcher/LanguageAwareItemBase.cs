using N2.Details;
using System.Web.UI.WebControls;

namespace LanguageSwitcher
{
	/// <summary>
	/// This is an abstract class that we can derive from on in all
	/// situations when we want edit the item's title and name.
	/// </summary>
	[N2.Web.UI.FieldSet("translated", "Translated fields", 10)]
	public abstract class LanguageAwareItemBase : N2.ContentItem
	{
		[EditableTextBox("Title", 20, Required = true, ContainerName = "translated")]
		public override string Title
		{
			get { return (string)GetDetail("Title"); }
			set { SetDetail("Title", value); }
		}

		[LanguageSwitcher("Current Language")]
		public virtual string CurrentLanguageSuffix
		{
			get { return "$" + LanguageSwitcherModule.RequestLanguage; }
		}

		private string GetTranslatedDetailName(string name)
		{
			if (name.Contains("$"))
				return name;
			else
				return name + CurrentLanguageSuffix;
		}

		public override System.DateTime? Published
		{
			get
			{
				return base.Published;
			}
			set
			{
				base.Published = value;
			}
		}

		public override System.DateTime? Expires
		{
			get
			{
				return base.Expires;
			}
			set
			{
				base.Expires = value;
			}
		}

		protected override void SetDetail<T>(string name, T value)
		{
			name = GetTranslatedDetailName(name);
			base.SetDetail<T>(name, value);
		}

		public override object GetDetail(string name)
		{
			return base.GetDetail(name + CurrentLanguageSuffix);
		}

		public virtual string GetUrlForLanguage(string language)
		{
			return LanguageSwitcherModule.decorateUrlForPersistentLanguageChange(Url, language);
		}
	}
}
