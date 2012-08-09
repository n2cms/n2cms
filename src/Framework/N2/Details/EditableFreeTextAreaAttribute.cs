using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.WebControls;
using N2.Edit;
using N2.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using N2.Web;
using NHibernate.Criterion;

namespace N2.Details
{
	/// <summary>
	/// Rich text editor settings set (toolbars, features).
	/// </summary>
	public enum FreeTextAreaSettingsSet
	{
		/// <summary>Setting set is defined by configuration, DEFAULT by default.</summary>
		Undefined,
		/// <summary>Fixed rich text editor toolbar, basic features, no additional toolbars.</summary>
		Fixed,
		/// <summary>Single line toolbar, no additional toolbars.</summary>
		Minimal,
		/// <summary>Single line tooolbar, all other toolbars shown by toogle icon.</summary>
		Simple,
		/// <summary>Extended toolbar with all features, less frequently used toolbars shown by toogle icon.</summary>
		Extended
	}


	/// <summary>Attribute used to mark properties as editable. This attribute is predefined to use the <see cref="N2.Web.UI.WebControls.FreeTextArea"/> web control as editor.</summary>
	/// <example>
	/// [N2.Details.EditableFreeTextArea("Text", 110)] 
	/// public virtual string Text { get; set; }
	/// 
	/// Setting set with full features:
	/// [N2.Details.EditableFreeTextArea("Text", 110, FreeTextAreaSettingsSet.Extended)]
	/// 
	/// Default toolbar mode can be set in Web.config, e.g.
	/// <![CDATA[ 
	///  <n2>
	///   <edit>
	///     <tinyMCE cssUrl="/Content/myrichtext.css">
	///       <settings>
	///         <add key="settings_set" value="Extended" />
	///       </settings>
	///   </edit>
	///  </n2>
	/// ]]>
	/// 
	/// Notes: 
	/// settings_set property is nonstantard.
	/// See also standard TinyMCE settings at http://tinymce.moxiecode.com/wiki.php/Configuration
	/// Toogle toolbars: see PWD plugin at http://www.neele.name/pdw_toggle_toolbars/
	/// </example>
	[AttributeUsage(AttributeTargets.Property)]
	public class EditableFreeTextAreaAttribute : EditableTextBoxAttribute, IRelativityTransformer
	{
		public EditableFreeTextAreaAttribute()
			: base(null, 100)
		{
		}

		public EditableFreeTextAreaAttribute(string title, int sortOrder)
			: base(title, sortOrder)
		{
		}

		public EditableFreeTextAreaAttribute(string title, int sortOrder, FreeTextAreaSettingsSet toolbars)
			: base(title, sortOrder)
		{
			Toolbars = toolbars;
		}

		/// <summary> Current rich text editor setting set (e.g. basic features with simple toolbar, extended etc) </summary>
		public FreeTextAreaSettingsSet Toolbars { get; set; }

		public override Control AddTo(Control container)
		{
			HtmlTableCell labelCell = new HtmlTableCell();
			Label label = AddLabel(labelCell);

			HtmlTableCell editorCell = new HtmlTableCell();
			Control editor = AddEditor(editorCell);
			if (label != null && editor != null && !string.IsNullOrEmpty(editor.ID))
				label.AssociatedControlID = editor.ID;

			HtmlTableCell extraCell = new HtmlTableCell();
			if (Required)
				AddRequiredFieldValidator(extraCell, editor);
			if (Validate)
				AddRegularExpressionValidator(extraCell, editor);

			AddHelp(extraCell);

			HtmlTableRow row = new HtmlTableRow();
			row.Cells.Add(labelCell);
			row.Cells.Add(editorCell);
			row.Cells.Add(extraCell);

			HtmlTable editorTable = new HtmlTable();
			editorTable.Attributes["class"] = "editDetail";
			editorTable.Controls.Add(row);
			container.Controls.Add(editorTable);

			return editor;
		}

		protected override void ModifyEditor(TextBox tb)
		{
			// set width and height to control the size of the tinyMCE editor
			// 1 column is evaluated to 10px
			// 1 row is evaluated to 20px
			if (Columns > 0)
				tb.Style.Add("width", (Columns * 10).ToString() + "px");
			if (Rows > 0)
				tb.Style.Add("height", (Rows * 20).ToString() + "px");
		}

		protected override TextBox CreateEditor()
		{
			return new FreeTextArea();
		}

		protected override Control AddRequiredFieldValidator(Control container, Control editor)
		{
			RequiredFieldValidator rfv = base.AddRequiredFieldValidator(container, editor) as RequiredFieldValidator;
			rfv.EnableClientScript = false;
			return rfv;
		}

		public override void UpdateEditor(ContentItem item, Control editor)
		{
			base.UpdateEditor(item, editor);

			FreeTextArea fta = (FreeTextArea)editor;

			if (item is IDocumentBaseSource)
				fta.DocumentBaseUrl = (item as IDocumentBaseSource).BaseUrl;

			string rt_mode = GetSettingsSetString(Toolbars);
			if (!string.IsNullOrEmpty(rt_mode))
				fta.CustomOverrides["settings_set"] = rt_mode;

			string content_css = GetCssFiles(string.Empty);
			if (!string.IsNullOrEmpty(content_css))
			{
				fta.CustomOverrides["content_css"] = content_css;
			}
		}

		/// <summary>Stringify current settingset mode.</summary>
		/// <remarks>Extended class might implement extended logics, 
		///   e.g. check current site StartPage properties when UNDEFINED. 
		///   When defined will override default config. value.
		/// </remarks>
		protected virtual string GetSettingsSetString(FreeTextAreaSettingsSet settingsSet)
		{
			if (settingsSet == FreeTextAreaSettingsSet.Undefined)
				settingsSet = FreeTextAreaSettingsSet.Simple;

			if (settingsSet != FreeTextAreaSettingsSet.Fixed)
				return settingsSet.ToString();
			else
				return string.Empty;
		}

		/// <summary> Comma separated list of CSS file Urls to be used by TinyMCE, defined by application, default empty (unset) </summary>
		/// <remarks> Extended class might implement extended logics, e.g. to check current site StartPage properties to set site specific styling. 
		///   When defined will override default config. value.
		/// </remarks>
		protected virtual string GetCssFiles(string cssFiles)
		{
			return cssFiles;
		}

		#region IRelativityTransformer Members

		public RelativityMode RelativeWhen { get; set; }

		string IRelativityTransformer.Rebase(string value, string fromAppPath, string toAppPath)
		{
			if (value == null || fromAppPath == null)
				return value;

			string from = string.Join("", fromAppPath.Select(c => "[" + c + "]").ToArray());
			string pattern = string.Format("((href|src)=[\"'](?<url>{0}))", from);
			string rebased = Regex.Replace(value, pattern, me =>
			{
				int urlIndex = me.Groups["url"].Index - me.Index;
				string before = me.Value.Substring(0, urlIndex);
				string after = me.Value.Substring(urlIndex + fromAppPath.Length);
				return before + toAppPath + after;
			}, RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.Compiled);
			return rebased;
		}

		#endregion
	}
}
