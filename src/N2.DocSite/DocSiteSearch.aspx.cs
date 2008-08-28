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
using DaveSexton.DocProject.DocSites;

namespace N2.DocSite
{
    public partial class DocSiteSearch : System.Web.UI.Page
    {
        #region Public Properties
        public string DocSiteFramePath
        {
            get
            {
                return DocSiteManager.Settings.DocSiteFramePath;
            }
        }

        public string Query
        {
            get
            {
                return ViewState["query"] as string;
            }
            set
            {
                ViewState["query"] = value;
            }
        }
        #endregion

        #region Private / Protected
        #endregion

        #region Constructors
        /// <summary>
        /// Constructs a new instance of the <see cref="DocSiteSearch" /> class.
        /// </summary>
        public DocSiteSearch()
        {
        }
        #endregion

        #region Methods
        protected override void OnInit(EventArgs e)
        {
            if (!User.Identity.IsAuthenticated && !DocSiteManager.Settings.SearchEnabled)
                Response.Redirect(DocSiteManager.Settings.DocSiteFramePath);

            base.OnInit(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!Page.IsPostBack)
            {
                browseImageButton.Visible = DocSiteManager.Settings.BrowseIndexEnabled || User.Identity.IsAuthenticated;

                string query = Request.QueryString["q"];

                this.Query = query;

                if (string.IsNullOrEmpty(query))
                {
                    resultsGridView.Visible = false;
                    statisticsPanel.Visible = false;
                    searchHelpPanel.Visible = true;
                }
            }
        }

        protected void resultsDataSource_Selected(object sender, ObjectDataSourceStatusEventArgs e)
        {
            ICollection value = e.ReturnValue as ICollection;

            string format = GetLocalResourceObject("resultsCountLabel.CalculatedText") as string;

            resultsCountLabel.Text = string.Format(System.Globalization.CultureInfo.CurrentCulture, format,
                (value == null) ? 0 : value.Count);
        }

        protected void browseImageButton_Click(object sender, ImageClickEventArgs e)
        {
            DocSiteNavigator.NavigateToBrowseIndex(Query);
        }
        #endregion
    }
}
