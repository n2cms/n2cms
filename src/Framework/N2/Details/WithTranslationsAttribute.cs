using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using N2.Engine.Globalization;
using System.Web;

namespace N2.Details
{
	/// <summary>
	/// Used in combination with <see cref="ITranslator"/> to support editing of collected content translations.
	/// </summary>
	/// <example>
	/// [WithTranslations(ContainerName = "SiteContainer")]
	/// public class StartPage : ContentPage, ITranslator
	/// {
	///		public string Translate(string key, string fallback = null)
	///		{
	///			return DetailCollections.GetTranslation(key) ?? fallback;
	///		}
	///		public IDictionary<string, string> GetTranslations()
	///		{
	///			return DetailCollections.GetTranslations();
	///		}
	/// }
	/// </example>
	[AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public class WithTranslationsAttribute : EditableTextAttribute
	{
        public WithTranslationsAttribute()
			: base("Translations", 2000)
        {
            Name = "ContentTranslations";
			TextMode = TextBoxMode.MultiLine;
			Rows = 5;
			CollectionKey = TranslationExtensions.DefaultCollectionKey;
			ClientAdapter = null;
        }

		public string CollectionKey { get; set; }

		public override void UpdateEditor(ContentItem item, Control editor)
		{
			var tb = (TextBox)editor;
			var translations = item.DetailCollections.GetTranslations(CollectionKey);
			foreach (var kvp in Engine.Resolve<MissingTranslationsCollector>().Get(item.ID))
				if (!translations.ContainsKey(kvp.Key))
					translations.Add(kvp.Key, kvp.Value);

			tb.Text = string.Join(Environment.NewLine, translations.Select(kvp => HttpUtility.UrlEncode(kvp.Key) + "=" + HttpUtility.UrlEncode(kvp.Value)));
		}

		public override bool UpdateItem(ContentItem item, Control editor)
		{
			bool changed = false;
			var tb = (TextBox)editor;
			var inputTranslations = tb.Text.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
				.Select(row => row.Split('=')).ToDictionary(kvp => HttpUtility.UrlDecode(kvp[0]), kvp => HttpUtility.UrlDecode(kvp[1]));
			var translations = item.DetailCollections.GetTranslations(CollectionKey);
			foreach (var removedKey in translations.Keys.Except(inputTranslations.Keys))
            {
				item.DetailCollections.SetTranslation(removedKey, null, CollectionKey);
			}
			
			foreach (var kvp in inputTranslations)
			{
				if (kvp.Key == kvp.Value)
					item.DetailCollections.SetTranslation(kvp.Key, null, CollectionKey);

				item.DetailCollections.SetTranslation(kvp.Key, kvp.Value, CollectionKey);
				changed = true;
			}
			return changed;
        }

		protected override Control AddEditor(Control container)
		{
			var editor = base.AddEditor(container);
			var tb = (TextBox)editor;
			tb.Attributes.Add("custom-translations", null);
			tb.CssClass += " custom-translations";
			return editor;
		}
	}
}
