using System;
using System.Web.UI;
using N2.Web.UI.WebControls;
using System.Web.UI.WebControls;

namespace N2.Details
{
    /// <summary>Attribute used to mark properties as editable. This attribute is predefined to use the <see cref="N2.Web.UI.WebControls.UrlSelector"/> web control as editor/url selector.</summary>
    /// <example>
    /// [N2.Details.EditableUrl("Url to page or document", 50)]
    /// public virtual string PageOrDocumentUrl { get; set; }
    /// </example>
    [AttributeUsage(AttributeTargets.Property)]
    public class EditableUrlAttribute : AbstractEditableAttribute, IRelativityTransformer, IWritingDisplayable, IDisplayable
    {
        private UrlSelectorMode openingMode = UrlSelectorMode.Items;
        private UrlSelectorMode availableModes = UrlSelectorMode.All;

        public EditableUrlAttribute()
            : this(null, 30)
        {
        }

        /// <summary>Initializes a new instance of the EditableUrlAttribute class.</summary>
        /// <param name="title">The label displayed to editors</param>
        /// <param name="sortOrder">The order of this editor</param>
        public EditableUrlAttribute(string title, int sortOrder)
            : base(title, sortOrder)
        {
        }

        /// <summary>Defines whether files or content items are available to be picked</summary>
        public UrlSelectorMode AvailableModes
        {
            get { return availableModes; }
            set { availableModes = value; }
        }

        /// <summary>Defines whether files or content items are first shown when picking an url.</summary>
        public UrlSelectorMode OpeningMode
        {
            get { return openingMode; }
            set { openingMode = value; }
        }

        /// <summary>Defines whether the managementUrls should be stored as app- or server relative.</summary>
        public UrlRelativityMode RelativeTo { get; set; }

        public override bool UpdateItem(ContentItem item, Control editor)
        {
            UrlSelector selector = (UrlSelector)editor;
            if(selector.Url != (string)item[Name])
            {
                item[Name] = RelativeTo == UrlRelativityMode.Absolute ? selector.Url : N2.Web.Url.ToRelative(selector.Url);
                return true;
            }
            return false;
        }

        public override void UpdateEditor(ContentItem item, Control editor)
        {
            UrlSelector selector = (UrlSelector)editor;
            selector.Url = (string)item[Name];
        }

        protected override Control AddEditor(Control container)
        {
            UrlSelector selector = new UrlSelector(Name);
            selector.AvailableModes = AvailableModes;
            selector.DefaultMode = OpeningMode;
            selector.Placeholder(GetLocalizedText("Placeholder") ?? Placeholder);

            container.Controls.Add(selector);

            return selector;
        }

		protected override Control AddRequiredFieldValidator(Control container, Control editor)
		{
			var validator = (BaseValidator)base.AddRequiredFieldValidator(container, editor);
			validator.ControlToValidate = ((UrlSelector)editor).Input.ID;
			return validator;
		}

		#region IRelativityTransformer Members

		public RelativityMode RelativeWhen { get; set; }

        string IRelativityTransformer.Rebase(string currentPath, string fromAppPath, string toAppPath)
        {
            return N2.Web.Url.Rebase(currentPath, fromAppPath, toAppPath);
        }

        #endregion

        #region IWritingDisplayable Members

        public void Write(ContentItem item, string propertyName, System.IO.TextWriter writer)
        {
            writer.Write(item[propertyName]);
        }

        #endregion
    }
}
