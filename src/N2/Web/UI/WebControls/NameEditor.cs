using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Web.UI;
using System.Web.UI.WebControls;
using N2.Configuration;
using N2.Resources;
using System.Text;
using System.Text.RegularExpressions;

namespace N2.Web.UI.WebControls
{
	/// <summary>A webcontrol that pulls out the page title and transforms it into a web-compatible name that can be used in url's</summary>
	[ValidationProperty("Text")]
	public class NameEditor : CompositeControl, IValidator, ITextControl
	{
		static NameEditorElement config;
		
		private CheckBox keepUpdated = new CheckBox();
		private TextBox editor = new TextBox();
		private bool? showKeepUpdated;
		private char? whitespaceReplacement;
		private bool? toLower;
		private string prefix = string.Empty;
		private string suffix = string.Empty;
		private string uniqueNameErrorFormat = "Another item already has the name '{0}'.";
		private string nameTooLongErrorFormat = "Name too long, the full url cannot exceed 260 characters: {0}";
		private string invalidCharactersErrorFormat = "Invalid characters in path. Only english alphanumerical characters allowed.";

		/// <summary>Constructs the name editor.</summary>
		public NameEditor()
		{
			editor.MaxLength = 60;
			CssClass = "nameEditor";
		}

		public static NameEditorElement Config
		{
			get { return config ?? (Config = LoadConfiguration()); }
			set { config = value; }
		}

		/// <summary>Gets or sets the name of the editor containing the title to input in the name editor.</summary>
		public string TitleEditorName
		{
			get { return (string) (ViewState["TitleEditorName"] ?? "Title"); }
			set { ViewState["TitleEditorName"] = value; }
		}

		public string Prefix
		{
			get { return prefix; }
			set { prefix = value; }
		}

		public string Suffix
		{
			get { return suffix; }
			set { suffix = value; }
		}

		public char? WhitespaceReplacement
		{
			get { return whitespaceReplacement; }
			set { whitespaceReplacement = value; }
		}

		public bool? ToLower
		{
			get { return toLower; }
			set { toLower = value; }
		}

		public bool? ShowKeepUpdated
		{
			get { return showKeepUpdated; }
			set { showKeepUpdated = value; }
		}

		public string UniqueNameErrorFormat
		{
			get { return uniqueNameErrorFormat; }
			set { uniqueNameErrorFormat = value; }
		}

		public string NameTooLongErrorFormat
		{
			get { return nameTooLongErrorFormat; }
			set { nameTooLongErrorFormat = value; }
		}

		public string InvalidCharactersErrorFormat
		{
			get { return invalidCharactersErrorFormat; }
			set { invalidCharactersErrorFormat = value; }
		}

		public string Text
		{
			get { return editor.Text; }
			set { editor.Text = value; }
		}

		[NotifyParentProperty(true)]
		public CheckBox KeepUpdated
		{
			get { return keepUpdated; }
		}
		
		[NotifyParentProperty(true)]
		public TextBox Editor
		{
			get { return editor; }
		}

		#region Methods

		private static NameEditorElement LoadConfiguration()
		{
			EditSection editSection = ConfigurationManager.GetSection("n2/edit") as EditSection;
			if (editSection != null)
				return editSection.NameEditor;

			NameEditorElement config = new NameEditorElement();
			config.WhitespaceReplacement = '-';
			config.ShowKeepUpdated = true;
			config.ToLower = true;
			return config;
		}

		protected override void OnInit(EventArgs e)
		{
			Page.Validators.Add(this);
			base.OnInit(e);
		}

		protected override void CreateChildControls()
		{
			editor.ID = "e";
			Controls.Add(editor);

			keepUpdated.ID = "ku";
			keepUpdated.Checked = true;
			Controls.Add(keepUpdated);

			base.CreateChildControls();
		}

		/// <summary>Initializes the name editor attaching to the title control onchange event.</summary>
		/// <param name="e">ignored</param>
		protected override void OnPreRender(EventArgs e)
		{
			IItemEditor itemEditor = ItemUtility.FindInParents<IItemEditor>(Parent);

			if (itemEditor != null)
			{
				try
				{
					if (itemEditor.AddedEditors.ContainsKey(TitleEditorName))
					{
                        TextBox tbTitle = itemEditor.AddedEditors[TitleEditorName] as TextBox;
                        keepUpdated.Visible = ShowKeepUpdated ?? Config.ShowKeepUpdated;

						string titleID = tbTitle.ClientID;
						string nameID = editor.ClientID;
						char whitespaceReplacement = (WhitespaceReplacement ?? Config.WhitespaceReplacement);
						string toLower = (ToLower ?? Config.ToLower).ToString().ToLower();
						string replacements = GetReplacements();
						string keepUpdatedBox = (ShowKeepUpdated ?? Config.ShowKeepUpdated) ? keepUpdated.ClientID : "";
						string s = updateNameScript + Environment.NewLine +
						           string.Format(startupScriptFormat, titleID, nameID, whitespaceReplacement, toLower, replacements, keepUpdatedBox);
						Page.ClientScript.RegisterStartupScript(typeof(NameEditor), "UpdateScript", s, true);
                        tbTitle.Attributes["onblur"] = "invokeUpdateName();";
                        Attributes["onfocus"] = "invokeUpdateName();";
					}
				}
				catch (KeyNotFoundException ex)
				{
					throw new N2Exception("No editor definition found for the Title property. The NameEditor copies the title and adjusts it for beeing part of the url. Either add a title editor or use another control to edit the name.", ex);
				}
			}

			base.OnPreRender(e);
		}

		string GetReplacements()
		{
			StringBuilder sb = new StringBuilder("[");
			foreach(PatternValueElement element in Config.Replacements)
			{
				if (sb.Length > 1)
					sb.Append(", ");
				sb.AppendFormat("{{pattern:/{0}/g, value:'{1}'}}", element.Pattern, element.Value);
			}
			sb.Append("]");
			return sb.ToString();
		}

		#region OnPreRender Helpers

		private const string startupScriptFormat = @"
var invokeUpdateName = function(){{
	updateName('{0}', '{1}', '{2}', {3}, {4}, '{5}');
}};
if('{5}'){{
	var chk = document.getElementById('{5}');
	var name = getName('{0}', '{2}', {3}, {4});
	var title = document.getElementById('{1}').value
	chk.checked = !name || !title || (name == title);
	jQuery(chk).click(invokeUpdateName);
}}
";

		private const string updateNameScript =
			@"
function getName(titleid, whitespace, tolower, replacements){
    var titleBox=document.getElementById(titleid);
	var name = titleBox.value.replace(/[.]+/g, '-')
		.replace(/[%?&/+:<>]/g, '')
		.replace(/\s+/g, whitespace)
		.replace(/[-]+/g, '-')
		.replace(/[-]+$/g, '');
	if(tolower) name = name.toLowerCase();
	for (var i in replacements){
		name = name.replace(replacements[i].pattern, replacements[i].value);
	}
	return name;
}
function updateName(titleid, nameid, whitespace, tolower, replacements, checkboxid){
	var name = getName(titleid, whitespace, tolower, replacements);
    if(checkboxid && document.getElementById(checkboxid).checked)
		document.getElementById(nameid).value = name;
}
";

		#endregion

		#endregion

		#region IValidator Members

		/// <summary>Gets or sets the error message generated when the name editor contains invalid values.</summary>
		public string ErrorMessage
		{
			get { return (string) ViewState["ErrorMessage"] ?? ""; }
			set { ViewState["ErrorMessage"] = value; }
		}

		/// <summary>Gets or sets wether the name editor's value passes validaton.</summary>
		public bool IsValid
		{
			get { return ViewState["IsValid"] != null ? (bool) ViewState["IsValid"] : true; }
			set { ViewState["IsValid"] = value; }
		}

		/// <summary>Validates the name editor's value checking uniqueness and lenght.</summary>
		public void Validate()
		{
			ContentItem currentItem = ItemUtility.FindCurrentItem(Parent);

			if (currentItem != null)
			{
				if (!string.IsNullOrEmpty(Text))
				{
					// Ensure that the chosen name is locally unique
					if (!N2.Context.IntegrityManager.IsLocallyUnique(Text, currentItem))
					{
						//Another item with the same parent and the same name was found 
						ErrorMessage = string.Format(UniqueNameErrorFormat, Text);
						IsValid = false;
						return;
					}

					// Ensure that the path isn't longer than 260 characters
					if (currentItem.Parent != null && (currentItem.Parent.Url.Length > 248 || currentItem.Parent.Url.Length + Text.Length > 260))
					{
						ErrorMessage = string.Format(NameTooLongErrorFormat, Text);
						IsValid = false;
						return;
					}

					foreach(PatternValueElement element in Config.Replacements)
					{
						if(element.ServerValidate && Regex.IsMatch(Text, element.Pattern, RegexOptions.Compiled))
						{
							ErrorMessage = InvalidCharactersErrorFormat;
							IsValid = false;
							return;
						}
					}

					if(Text.IndexOfAny(new char[]{'?', '&', '/', '+', ':', '%'}) >= 0)
					{
						ErrorMessage = InvalidCharactersErrorFormat;
						IsValid = false;
						return;
					}
				}
			}

			IsValid = true;
		}

		public override void RenderBeginTag(HtmlTextWriter writer)
		{
			writer.Write(Prefix);
			base.RenderBeginTag(writer);
		}

		/// <summary>Renders an additional asterix when validation didn't pass.</summary>
		/// <param name="writer">The writer to render the asterix to.</param>
		public override void RenderEndTag(HtmlTextWriter writer)
		{
			base.RenderEndTag(writer);
			writer.Write(Suffix);
			if (!IsValid)
				writer.Write("<span style='color:red'>*</span>");
		}

		#endregion
	}
}
