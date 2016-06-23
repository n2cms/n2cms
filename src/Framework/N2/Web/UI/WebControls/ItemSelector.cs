using System.Web.UI;
using N2.Resources;
using System;

namespace N2.Web.UI.WebControls
{
    /// <summary>
    /// An input box that can be updated with a page selected through a popup 
    /// window.
    /// </summary>
    public class ItemSelector : UrlSelector, IValidator
    {
        public ItemSelector()
        {
            DefaultMode = UrlSelectorMode.Items;
            AvailableModes = UrlSelectorMode.Items;
            BrowserUrl = N2.Web.Url.Parse(Engine.ManagementPaths.EditTreeUrl).AppendQuery("location=contentselection");
            IsValid = true;
            ErrorMessage = "The selected item is not of the required type.";
        }

		public ItemSelector(string name) 
			: this()
		{
			ID = name;
			Input.ID = name + "_input";

		}

		/// <summary>Gets the selected item or null if none is selected.</summary>
		public ContentItem SelectedItem
        {
            get { return string.IsNullOrEmpty(Url) ? null : N2.Context.UrlParser.Parse(Url); }
            set { Url = value != null ? value.Url : ""; }
        }

        /// <summary>Gets the ID of the selected item or 0 if no item is selected.</summary>
        public int SelectedItemID
        {
            get { return SelectedItem != null ? SelectedItem.ID : 0; }
            set { SelectedItem = N2.Context.Persister.Get(value); }
        }

        /// <summary>The type the select item should conform to.</summary>
        public System.Type RequiredType
        {
            get { return Type.GetType(ViewState["RequiredType"] as string ?? "") ?? typeof( N2.ContentItem); }
            set { ViewState["RequiredType"] = value.AssemblyQualifiedName; }
        }

        protected override void OnInit(System.EventArgs e)
        {
            base.OnInit(e);
            Page.Validators.Add(this);
        }

		protected override void CreateChildControls()
		{
			base.CreateChildControls();
			Input.CssClass = "itemSelector selector";
		}

		protected override void RegisterClientScripts()
        {
            Page.JavaScript("$('#" + Input.ClientID + "').n2autocomplete({ filter: 'pages', selectableTypes:'" + SelectableTypes + "' });", ScriptPosition.Bottom, ScriptOptions.DocumentReady | ScriptOptions.ScriptTags);
        }

        #region IValidator Members

        /// <summary>The message to display.</summary>
        public string ErrorMessage { get; set; }

        /// <summary>True if the item is valid.</summary>
        public bool IsValid { get; set; }

        /// <summary>Validates the selected item.</summary>
        public void Validate()
        {
            IsValid = RequiredType == null
                || SelectedItem == null
                || RequiredType.IsAssignableFrom(SelectedItem.GetContentType());
        }

        #endregion
    }
}
