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
using System.Text;
using System.ComponentModel;
using DaveSexton.DocProject.DocSites;

namespace N2.DocSite.Controls
{
    public partial class DocSiteSidebar : System.Web.UI.UserControl
    {
        #region Public Properties
        [Category("Appearance"), DefaultValue(true)]
        public bool ContentsSelected
        {
            get
            {
                return (bool?)ViewState["_$ContentsSelected"] ?? true;
            }
            set
            {
                ViewState["_$ContentsSelected"] = value;
            }
        }

        [Browsable(false)]
        public DocSiteContents TableOfContents
        {
            get
            {
                return contentsControl;
            }
        }

        [Browsable(false)]
        public DocSiteIndex Index
        {
            get
            {
                return indexControl;
            }
        }
        #endregion

        #region Private / Protected
        private bool changingSelection;
        #endregion

        #region Constructors
        /// <summary>
        /// Constructs a new instance of the <see cref="DocSiteSidebar" /> class.
        /// </summary>
        public DocSiteSidebar()
        {
        }
        #endregion

        #region Methods
        public void Initialize(string topic, string helpFile)
        {
            changingSelection = true;

            contentsControl.SelectedTopic = topic;
            indexControl.SelectedHelpFile = (helpFile == null) ? string.Empty : helpFile.ToLowerInvariant();

            changingSelection = false;
        }
        #endregion

        #region Event Handlers
        protected override void OnLoad(EventArgs e)
        {
            if (Page.IsPostBack)
                ContentsSelected = !"sidebar_index_button".Equals(
                    selectedButtonHiddenField.Value, StringComparison.OrdinalIgnoreCase);

            base.OnLoad(e);
        }

        protected override void OnPreRender(EventArgs e)
        {
            string script = string.Format("selectDocsiteSidebarButton(document.getElementById(\"{0}\"), document.getElementById(\"{1}\"));",
                (ContentsSelected) ? "sidebar_contents_button" : "sidebar_index_button",
                (ContentsSelected) ? "docsite_toc" : "docsite_index");

            Page.ClientScript.RegisterStartupScript(typeof(DocSiteSidebar), "initSidebar", script, true);

            base.OnPreRender(e);
        }

        protected void contentsControl_SelectedTopicChanged(object sender, EventArgs e)
        {
            if (!changingSelection)
            {
                changingSelection = true;
                ContentsSelected = true;

                if (DocSiteNavigator.NavigateToTopic(contentsControl.SelectedTopic, false))
                    indexControl.SelectedHelpFile = DocSiteNavigator.ResolveTopicHelpFile(contentsControl.SelectedTopic, false);

                changingSelection = false;
            }
        }

        protected void indexControl_SelectedHelpFileChanged(object sender, EventArgs e)
        {
            if (!changingSelection)
            {
                changingSelection = true;
                ContentsSelected = false;

                DocSiteNavigator.NavigateToHelpFile(indexControl.SelectedHelpFile);

                contentsControl.SelectedTopic = DocSiteNavigator.ResolveHelpFileTopic(indexControl.SelectedHelpFile, false);

                changingSelection = false;
            }
        }
        #endregion
    }
}