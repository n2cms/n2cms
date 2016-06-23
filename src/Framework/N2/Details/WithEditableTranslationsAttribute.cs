using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using N2.Engine.Globalization;
using System.Web;
using System.Web.Script.Serialization;

namespace N2.Details
{
	/// <summary>
	/// Used in combination with <see cref="ITranslator"/> to support editing of collected content translations.
	/// </summary>
	/// <example>
	/// [WithEditableTranslations(ContainerName = "SiteContainer")]
	/// public class StartPage : ContentPage, ITranslator
	/// {
	///		public string Translate(string key, string fallback = null)
	///		{
	///			return DetailCollections.GetTranslation(key) ?? fallback;
	///		}
	///		public IDictionary&lt;string, string&gt; GetTranslations()
	///		{
	///			return DetailCollections.GetTranslations();
	///		}
	/// }
	/// </example>
	[AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public class WithEditableTranslationsAttribute : EditableTextAttribute
	{
        public WithEditableTranslationsAttribute()
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

			tb.Text = new JavaScriptSerializer().Serialize(translations);
		}

		public override bool UpdateItem(ContentItem item, Control editor)
		{
			bool changed = false;
			var tb = (TextBox)editor;
			var inputTranslations = new JavaScriptSerializer().Deserialize<Dictionary<string, string>>(tb.Text);
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
