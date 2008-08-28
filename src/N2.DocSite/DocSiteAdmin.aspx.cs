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
using System.Collections.Generic;
using System.Drawing;
using DaveSexton.DocProject.DocSites;
using DaveSexton.DocProject.DocSites.Configuration;

namespace N2.DocSite
{
    public partial class DocSiteAdmin : System.Web.UI.Page
    {
        #region Public Properties
        #endregion

        #region Private / Protected
        #endregion

        #region Constructors
        /// <summary>
        /// Constructs a new instance of the <see cref="DocSiteAdmin" /> class.
        /// </summary>
        public DocSiteAdmin()
        {
        }
        #endregion

        #region Methods
        public static object GetStatisticsForBinding()
        {
            return new SearchIndexStatistics();
        }

        public static DocSiteSettings GetSettingsForBinding()
        {
            return DocSiteManager.Settings;
        }

        public static void UpdateSettings(DocSiteSettings newSettings)
        {
            DocSiteSettings settings = DocSiteManager.Settings;

            settings.Update(newSettings);

            settings.Save();
        }
        #endregion

        #region Event Handlers
        protected override void OnInit(EventArgs e)
        {
            ScriptManager.GetCurrent(Page).AsyncPostBackTimeout = DocSiteManager.Settings.CreateIndexRefreshTimeout;

            base.OnInit(e);
        }

        protected void createIndexLinkButton_Click(object sender, EventArgs e)
        {
            DaveSexton.DocProject.DocSites.DocSiteSearch.CreateSearchIndex();

            searchStatsDetailsView.DataBind();
            searchStatsUpdatePanel.Update();
        }

        protected void searchSettingsDetailsView_ItemUpdating(object sender, DetailsViewUpdateEventArgs e)
        {
            e.NewValues["SearchPath"] = ((TextBox)searchSettingsDetailsView.FindControl("searchPathTextBox")).Text;
        }

        protected void searchWeightFactorsDetailsView_ItemUpdating(object sender, DetailsViewUpdateEventArgs e)
        {
            e.NewValues["SearchEarlyKeywordWeightFactor"] = ((TextBox)searchWeightFactorsDetailsView.FindControl("earlyKeywordWeightFactorTextBox")).Text;
            e.NewValues["SearchTitleKeywordWeightFactor"] = ((TextBox)searchWeightFactorsDetailsView.FindControl("titleKeywordWeightFactorTextBox")).Text;
        }
        #endregion

        #region Nested
        private sealed class SearchIndexStatistics
        {
            public string ProviderName
            {
                get
                {
                    return DaveSexton.DocProject.DocSites.DocSiteSearch.DefaultSearchProvider.Name;
                }
            }

            public DateTime? LastCreationDate
            {
                get
                {
                    return (DaveSexton.DocProject.DocSites.DocSiteSearch.IndexCreated)
                        ? (DateTime?)DaveSexton.DocProject.DocSites.DocSiteSearch.LastCreationDate.ToLocalTime()
                        : null;
                }
            }

            public int KeywordCount
            {
                get
                {
                    return DaveSexton.DocProject.DocSites.DocSiteSearch.KeywordCount;
                }
            }

            public int DocumentCount
            {
                get
                {
                    return DaveSexton.DocProject.DocSites.DocSiteSearch.DocumentCount;
                }
            }

            public bool IndexCreated
            {
                get
                {
                    return DaveSexton.DocProject.DocSites.DocSiteSearch.IndexCreated;
                }
            }
        }
        #endregion
    }
}
