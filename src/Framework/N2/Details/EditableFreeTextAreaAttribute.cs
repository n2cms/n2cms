using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.WebControls;
using N2.Edit;
using System.Web.UI.HtmlControls;
using N2.Web;
using NHibernate.Criterion;
using System.Web;
using N2.Web.UI.WebControls;

namespace N2.Details
{

    /// <summary>
    /// Rich text editor settings set (toolbars, features).
    /// </summary>
    public enum EditorModeSetting
    {
        Standard = 0,
        Basic = 3,
        Full = 4,
    }

    /// <summary>Attribute used to mark properties as editable. This attribute is predefined to use the <see cref="N2.Web.UI.WebControls.FreeTextArea"/> web control as editor.</summary>
    /// <example>
    /// Editor with standard toolbars:
    /// [N2.Details.EditableFreeTextArea("Text", 110)] 
    /// public virtual string Text { get; set; }
    /// 
    /// Editor with full toolbars:
    /// [N2.Details.EditableFreeTextArea("Text", 110, FreeTextArea.EditorModeSetting.Full)]
    /// 
    /// Editor with reduced toolbars:
    /// [N2.Details.EditableFreeTextArea("Text", 110, FreeTextArea.EditorModeSetting.Basic)]
    /// 
    /// Default toolbar mode can be set in Web.config, e.g.
    /// <![CDATA[ 
    ///  <n2>
    ///   <edit>
    ///     <tinyMCE cssUrl="/Content/myrichtext.css">
    ///   </edit>
    ///  </n2>
    /// ]]>
    /// 
    /// </example>
    [AttributeUsage(AttributeTargets.Property)]
    public class EditableFreeTextAreaAttribute : EditableTextBoxAttribute, IRelativityTransformer
    {
        private EditorModeSetting editorMode = EditorModeSetting.Standard;
        private string additionalFormats = string.Empty;
        private string useStylesSet = string.Empty;

        public EditableFreeTextAreaAttribute()
            : this(null, 100)
        {
        }

        public EditableFreeTextAreaAttribute(string title, int sortOrder)
            : base(title, sortOrder)
        {
			ClientAdapter = "n2autosave.ckeditor";
		}

        public EditorModeSetting EditorMode
        {
            get { return editorMode; }
            set { editorMode = value; }
        }

        public string AdditionalFormats
        {
            set { additionalFormats = value; }
        }

        public string UseStylesSet
        {
            set { useStylesSet = value; }
        }

        protected override void ModifyEditor(TextBox tb)
        {
            // set width and height to control the size of the ckeditor
            // 1 column is evaluated to 10px
            // 1 row is evaluated to 20px
            if (Columns > 0)
                tb.Style.Add("width", (Columns * 10).ToString() + "px");
            if (Rows > 0)
                tb.Style.Add("height", (Rows * 20).ToString() + "px");
        }

        protected override TextBox CreateEditor()
        {
            FreeTextArea fta = new FreeTextArea();

            fta.EditorMode = editorMode;
            fta.AdditionalFormats = additionalFormats;
            fta.UseStylesSet = useStylesSet;

            return fta;
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

            fta.EditorMode = editorMode;
            fta.AdditionalFormats = additionalFormats;
            fta.UseStylesSet = useStylesSet;

            if (item is IDocumentBaseSource)
                fta.DocumentBaseUrl = (item as IDocumentBaseSource).BaseUrl;

        }

        public override string GetIndexableText(ContentItem item)
        {
            return HttpUtility.HtmlDecode(base.GetIndexableText(item) ?? "");
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
            }, RegexOptions.Singleline | RegexOptions.IgnoreCase);
            return rebased;
        }

        #endregion
    }
}
