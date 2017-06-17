using System;
using System.Diagnostics;
using System.Security.Principal;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using N2.Definitions;
using N2.Engine;
using N2.Persistence;
using N2.Persistence.Proxying;
using N2.Security;
using N2.Resources;

namespace N2.Details
{
    /// <summary>
    /// Basic implementation of the <see cref="IEditable"/>. This 
    /// class implements properties, provides comparison and equality but does
    /// not add any controls.
    /// </summary>
    [DebuggerDisplay("{name, nq} [{TypeName, nq}]")]
    public abstract class AbstractEditableAttribute : Attribute, IEditable, IViewEditable, ISecurable, IPermittable, IInterceptableProperty, IContentTransformer, IComparable<IUniquelyNamed>, ICloneable
    {
        private string[] authorizedRoles;
        private string containerName = null;
        private string name;
        private bool required = false;
        private string requiredMessage = null;
        private string requiredText = "*";
        private int sortOrder;
        private string title;
        private bool validate = false;
        private string validationExpression = null;
        private string validationMessage = null;
        private string validationText = "*";
        private string helpTitle;
        private string helpText;
        private string localizationClassKey = "Editables";
        private IEngine engine = null;

        #region Properties

        /// <summary>Gets or sets placeholder text displayed inside the editor.</summary>
        public string Placeholder { get; set; }

        /// <summary>Gets or sets whether a required field validator should be appended.</summary>
        public bool Required
        {
            get { return required; }
            set { required = value; }
        }
            
        /// <summary>Gets or sets the validation expression for a regular expression validator.</summary>
        public string ValidationExpression
        {
            get { return validationExpression; }
            set { validationExpression = value; }
        }

        /// <summary>Gets or sets whether a regular expression validator should be added.</summary>
        public bool Validate
        {
            get { return validate; }
            set { validate = value; }
        }

        /// <summary>Gets or sets the message for the regular expression validator.</summary>
        public string ValidationMessage
        {
            get { return validationMessage ?? Title + " is invalid."; }
            set { validationMessage = value; }
        }

        /// <summary>Gets or sets the message for the required field validator.</summary>
        public string RequiredMessage
        {
            get { return requiredMessage ?? Title + " is required."; }
            set { requiredMessage = value; }
        }

        /// <summary>Gets or sets the text for the required field validator.</summary>
        public string RequiredText
        {
            get { return requiredText; }
            set { requiredText = value; }
        }

        /// <summary>Gets or sets the text for the regular expression validator.</summary>
        public string ValidationText
        {
            get { return validationText; }
            set { validationText = value; }
        }

        /// <summary>Gets the current Content Engine.</summary>
        /// <remarks>Note that a setter is made available for testing purposes only and there shouldn't be any need to use it. It's also important not to call this property from the constructor.</remarks>
        protected virtual IEngine Engine
        {
            get { return engine ?? N2.Context.Current; }
            set { engine = value; }
        }
        #endregion

        #region Constructors

        /// <summary>Default/empty constructor.</summary>
        public AbstractEditableAttribute()
        {
            IsViewEditable = true;
            PersistAs = PropertyPersistenceLocation.Detail;
        }

        /// <summary>Initializes a new instance of the AbstractEditableAttribute.</summary>
        /// <param name="title">The label displayed to editors</param>
        /// <param name="sortOrder">The order of this editor</param>
        public AbstractEditableAttribute(string title, int sortOrder)
            : this()
        {
            Title = title;
            SortOrder = sortOrder;
        }

        /// <summary>Initializes a new instance of the AbstractEditableAttribute.</summary>
        /// <param name="title">The label displayed to editors</param>
        /// <param name="name">The name used for equality comparison and reference.</param>
        /// <param name="sortOrder">The order of this editor</param>
        public AbstractEditableAttribute(string title, string name, int sortOrder)
            : this(title, sortOrder)
        {
            Name = name;
        }

        #endregion

        #region IEditable Members

        /// <summary>Gets or sets the label used for presentation.</summary>
        public string Title
        {
            get { return title ?? Name; }
            set { title = value; }
        }

        /// <summary>Updates the object with the values from the editor.</summary>
        /// <param name="item">The object to update.</param>
        /// <param name="editor">The editor contorl whose values to update the object with.</param>
        /// <returns>True if the item was changed (and needs to be saved).</returns>
        public abstract bool UpdateItem(ContentItem item, Control editor);

        /// <summary>Updates the editor with data from the item.</summary>
        /// <param name="item">The content item containing the values to bind to the editor.</param>
        /// <param name="editor">The editor to be bound with data from the item.</param>
        public abstract void UpdateEditor(ContentItem item, Control editor);

        /// <summary>Gets or sets the name of the detail (property) on the content item's object.</summary>
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        /// <summary>Gets or sets the container name associated with this editor. The name must match a container attribute defined on the item class.</summary>
        public string ContainerName
        {
            get { return containerName; }
            set { containerName = value; }
        }

        /// <summary>Gets or sets the order of the associated control</summary>
        public int SortOrder
        {
            get { return sortOrder; }
            set { sortOrder = value; }
        }

		/// <summary>Name of a client function which will be invoked to perform client operations such as change tracking and auto-save.</summary>
		public string ClientAdapter { get; set; }

        /// <summary>Adds a label and an editor to a panel.</summary>
        /// <param name="container">The container onto which the panel is added.</param>
        /// <returns>A reference to the addeed editor.</returns>
        /// <remarks>Please note that this method was abstract before version 1.3.1. It's now recommended to add the editor through the <see cref="AddEditor"/> method.</remarks>
        public virtual Control AddTo(Control container)
        {
            Control panel = AddPanel(container);
            Label label = AddLabel(panel);
            Control editor = AddEditor(panel);
			if (!string.IsNullOrEmpty(ClientAdapter))
			{
				panel.Page.JavaScript(string.Format("window.n2autosave && n2autosave.register('{0}', '{1}', '{2}')", editor.ClientID, Name, ClientAdapter), ScriptOptions.DocumentReady);
			}
            if (label != null && editor != null && !string.IsNullOrEmpty(editor.ID))
                label.AssociatedControlID = editor.ID;

            AddValidation(panel, editor);

            return editor;
        }

        protected virtual void AddValidation(Control container, Control editor)
        {
            if (Required)
                AddRequiredFieldValidator(container, editor);
            if (Validate)
                AddRegularExpressionValidator(container, editor);
        }

        protected virtual Control AddHelp(Control container)
        {
            string text = GetLocalizedText("HelpText") ?? HelpText;
            string title = GetLocalizedText("HelpTitle") ?? HelpTitle;

            if (string.IsNullOrEmpty(text) && string.IsNullOrEmpty(title))
                return null;

            var b = new HtmlGenericControl("b");
            b.ID = "hi_" + Name;
            b.Attributes["data-placement"] = "right";
            b.Attributes["title"] = title;
            container.Controls.Add(b);

            if (string.IsNullOrEmpty(text))
            {
                b.Attributes["class"] = "help help-tooltip fa fa-question-circle";
                b.Attributes["data-toggle"] = "tooltip";
            }
            else
            {
                b.Attributes["class"] = "help help-popover fa fa-question-circle";
                b.Attributes["data-toggle"] = "popover";
                b.Attributes["data-content"] = text;
            }

            //if (!string.IsNullOrEmpty(text))
            //{
            //  HtmlGenericControl helpPanel = new HtmlGenericControl("span");
            //  helpPanel.ID = "hp_" + Name;
            //  helpPanel.Attributes["class"] = "helpPanel revealer";
            //  container.Controls.Add(helpPanel);

            //  AddHelpButton(helpPanel, title);

            //  HtmlGenericControl div = new HtmlGenericControl("span");
            //  div.ID = "hd_" + Name;
            //  div.Attributes["class"] = "helpText";
            //  helpPanel.Controls.Add(div);

            //  HtmlGenericControl header = new HtmlGenericControl("b");
            //  header.InnerHtml = title;
            //  div.Controls.Add(header);

            //  HtmlGenericControl span = new HtmlGenericControl("span");
            //  span.InnerHtml = text;
            //  div.Controls.Add(span);

            //}
            //else if (!string.IsNullOrEmpty(title))
            //{
            //  return AddHelpButton(container, title);
            //}

            return b;
        }

        /// <summary>Gets a localized resource string from the global resource with the name denoted by <see cref="LocalizationClassKey"/>. The resource key follows the pattern <see cref="Name"/>.key where the name is the name of the detail and the key is the supplied parameter.</summary>
        /// <param name="key">A part of the resource key used for finding the localized resource.</param>
        /// <returns>A localized string if found, or null.</returns>
        protected virtual string GetLocalizedText(string key)
        {
            try
            {
                return Utility.GetGlobalResourceString(LocalizationClassKey, Name + "." + key) as string;
            }
            catch
            {
                return null; // it's okay to use default text
            }
        }

        /// <summary>Find out whether a user has permission to edit this detail.</summary>
        /// <param name="user">The user to check.</param>
        /// <returns>True if the user has the required permissions.</returns>
        public virtual bool IsAuthorized(IPrincipal user)
        {
            if (authorizedRoles == null)
                return true;
            else if (user == null)
                return false;

            foreach (string role in AuthorizedRoles)
                if (string.Equals(user.Identity.Name, role, StringComparison.OrdinalIgnoreCase) || user.IsInRole(role))
                    return true;
            return false;
        }

        int IComparable<IContainable>.CompareTo(IContainable other)
        {
            int delta = SortOrder - other.SortOrder;
            return delta != 0 ? delta : Name.CompareTo(other.Name);
        }

        /// <summary>Compares the sort order of editable attributes.</summary>
        public int CompareTo(IEditable other)
        {
            if (SortOrder != other.SortOrder)
                return SortOrder - other.SortOrder;
            if (Title != null && other.Title != null)
                return Title.CompareTo(other.Title);
            if (Title != null)
                return -1;
            if (other.Title != null)
                return 1;
            
            return 0;
        }

        #endregion

        #region ISecurable Members

        /// <summary>Gets or sets roles allowed to edit this detail. This property can be set by the DetailAuthorizedRolesAttribute.</summary>
        public string[] AuthorizedRoles
        {
            get { return authorizedRoles; }
            set { authorizedRoles = value; }
        }

        public string HelpTitle
        {
            get { return helpTitle; }
            set { helpTitle = value; }
        }

        public string HelpText
        {
            get { return helpText; }
            set { helpText = value; }
        }

        public string LocalizationClassKey
        {
            get { return localizationClassKey; }
            set { localizationClassKey = value; }
        }

        #region Equals & GetHashCode

        /// <summary>Checks another object for equality.</summary>
        /// <param name="obj">The other object to check.</param>
        /// <returns>True if the items are of the same type and have the same name.</returns>
        public override bool Equals(object obj)
        {
            var other = obj as AbstractEditableAttribute;
            if (other == null)
                return false;
            return (Name == other.Name);
        }

        int? hashCode;
        /// <summary>Gets a hash code based on the attribute's name.</summary>
        /// <returns>A hash code.</returns>
        public override int GetHashCode()
        {
            if (hashCode.HasValue)
                return hashCode.Value;

            if (Name != null)
                hashCode = (GetType().FullName + Name).GetHashCode();
            else
                hashCode = base.GetHashCode();

            return hashCode.Value;
        }

        private string TypeName
        {
            get { return GetType().Name; }
        }

        #endregion

        #region Methods

        /// <summary>Adds the panel to the container. Creating this panel and adding labels and editors to it will help to avoid web controls from interfere with each other.</summary>
        /// <param name="container">The container onto which add the panel.</param>
        /// <returns>A panel that can be used to add editor and label.</returns>
        protected virtual Control AddPanel(Control container)
        {
            HtmlGenericControl detailContainer = new HtmlGenericControl("div");
            detailContainer.Attributes["class"] = "editDetail " + GetType().Name.Replace("Attribute", "");
            container.Controls.Add(detailContainer);
            return detailContainer;
        }

        /// <summary>Adds a label with the text set to the current Title to the container.</summary>
        /// <param name="container">The container control for the label.</param>
        protected virtual Label AddLabel(Control container)
        {
            Label label = new Label();
            label.ID = "lbl" + Name;
            label.CssClass = "editorLabel";
            label.Attributes["data-sortorder"] = SortOrder.ToString();
            container.Controls.Add(label);

            label.Controls.Add(new LiteralControl(GetLocalizedText("Title") ?? Title));
            AddHelp(label);

            return label;
        }

        /// <summary>Compares two values regarding null values as equal.</summary>
        protected bool AreEqual(object editorValue, object itemValue)
        {
            return (editorValue == null && itemValue == null)
                   || (editorValue != null && editorValue.Equals(itemValue))
                   || (itemValue != null && itemValue.Equals(editorValue));
        }

        #endregion

        #endregion

        /// <summary>Adds a regular expression validator.</summary>
        /// <param name="container">The container control for this validator.</param>
        /// <param name="editor">The editor control to validate.</param>
        protected virtual Control AddRegularExpressionValidator(Control container, Control editor)
        {
            RegularExpressionValidator rev = new RegularExpressionValidator();
            rev.ID = Name + "_rev";
            rev.ControlToValidate = editor.ID;
            rev.ValidationExpression = GetLocalizedText("ValidationExpression") ?? ValidationExpression;
            rev.Display = ValidatorDisplay.Dynamic;
            rev.Text = GetLocalizedText("ValidationText") ?? ValidationText;
            rev.ErrorMessage = GetLocalizedText("ValidationMessage") ?? ValidationMessage;
            container.Controls.Add(rev);

            return rev;
        }

        /// <summary>Adds a required field validator.</summary>
        /// <param name="container">The container control for this validator.</param>
        /// <param name="editor">The editor control to validate.</param>
        protected virtual Control AddRequiredFieldValidator(Control container, Control editor)
        {
            RequiredFieldValidator rfv = new RequiredFieldValidator();
            rfv.ID = Name + "_rfv";
            rfv.ControlToValidate = editor.ID;
            rfv.Display = ValidatorDisplay.Dynamic;
            rfv.Text = GetLocalizedText("RequiredText") ?? RequiredText;
            rfv.ErrorMessage = GetLocalizedText("RequiredMessage") ?? RequiredMessage;
            container.Controls.Add(rfv);

            return rfv;
        }

        /// <summary>Adds the editor control to the edit panel. This method is invoked by <see cref="N2.Details.AbstractEditableAttribute.AddTo(N2.ContentItem, string, System.Web.UI.Control)"/> and the editor is prepended a label and wrapped in a panel. To remove these controls also override the <see cref="N2.Details.AbstractEditableAttribute.AddTo(N2.ContentItem, string, System.Web.UI.Control)"/> method.</summary>
        /// <param name="container">The container onto which to add the editor.</param>
        /// <returns>A reference to the addeed editor.</returns>
        /// <remarks>Please note that this is a breaking change. This method was added after version 1.3.1 to reduce duplicated code induced by having <see cref="N2.Details.AbstractEditableAttribute.AddTo(N2.ContentItem, string, System.Web.UI.Control)"/> abstract.</remarks>
        protected abstract Control AddEditor(Control container);

        #region IInterceptableProperty Members

        public PropertyPersistenceLocation PersistAs { get; set; }

        public object DefaultValue { get; set; }

        #endregion

        #region IDisplayable Members

        public virtual Control AddTo(ContentItem item, string detailName, Control container)
        {
            var value = item[detailName];
            if (value != null)
            {
                var literal = new LiteralControl(value.ToString());
                container.Controls.Add(literal);
                return literal;
            }
            return null;
        }

        #endregion

        #region IContentTransformer Members

        ContentState IContentTransformer.ChangingTo
        {
            get { return ContentState.New; }
        }

        bool IContentTransformer.Transform(ContentItem item)
        {
            if (DefaultValue == null)
                return false;

            item[Name] = DefaultValue;
            return true;
        }

        #endregion

        #region IPermittable Members

        public Permission RequiredPermission { get; set; }

        #endregion

        #region IComparable<IUniquelyNamed> Members

        int IComparable<IUniquelyNamed>.CompareTo(IUniquelyNamed other)
        {
            var containable = other as IContainable;
            if (containable != null)
                return ((IComparable<IContainable>)this).CompareTo(containable);
            
            if (other is IDisplayable)
                return -1;
            if (other == null)
                return 1;

            return Name.CompareTo(other.Name);
        }

        #endregion

        #region IViewEditable Members

        /// <summary>Determines whether this editable can be managed when viewing a page in drag'n'drop mode.</summary>
        public bool IsViewEditable { get; set; }

        #endregion

        object ICloneable.Clone()
        {
            return MemberwiseClone();
        }
    }
}

