using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Reflection;
using DaveSexton.DocProject.DocSites;

namespace N2.DocSite.Controls
{
    public partial class DocSiteHeader : System.Web.UI.UserControl
    {
        #region Public Properties
        #endregion

        #region Private / Protected
        #endregion

        #region Constructors
        /// <summary>
        /// Constructs a new instance of the <see cref="DocSiteHeader" /> class.
        /// </summary>
        public DocSiteHeader()
        {
        }
        #endregion

        #region Methods
        #endregion

        #region Event Handlers
        protected override void OnInit(EventArgs e)
        {
            chmHyperLink.NavigateUrl = DocSiteManager.Settings.CompiledHelp1xFilePath;

            base.OnInit(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!Page.IsPostBack)
            {
                bool authenticated = Page.User.Identity.IsAuthenticated;

                searchImageButton.Visible = authenticated || DocSiteManager.Settings.SearchEnabled;
                browseImageButton.Visible = authenticated || DocSiteManager.Settings.BrowseIndexEnabled;

                if (!searchImageButton.Visible && !browseImageButton.Visible)
                    searchTextBox.Visible = false;
            }

            Page.Form.DefaultButton = searchImageButton.UniqueID;
            searchTextBox.Focus();
        }

        protected void searchImageButton_Click(object sender, ImageClickEventArgs e)
        {
            DocSiteNavigator.NavigateToSearchResults(searchTextBox.Text);
        }

        protected void browseImageButton_Click(object sender, ImageClickEventArgs e)
        {
            DocSiteNavigator.NavigateToBrowseIndex(searchTextBox.Text);
        }
        #endregion
    }
}