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

namespace N2.DocSite
{
    public partial class Default : System.Web.UI.Page, IDocSiteDefault
    {
        #region Public Properties
        #endregion

        #region Private / Protected
        #endregion

        #region Constructors
        /// <summary>
        /// Constructs a new instance of the <see cref="Default" /> class.
        /// </summary>
        public Default()
        {
        }
        #endregion

        #region Methods
        private void SyncSidebar()
        {
            string topic = null, helpFile = null;

            if (!Page.IsPostBack)
            {
                if (Request.QueryString["filenotfound"] != null)
                    ContentPath = DocSiteManager.Settings.FileNotFoundPath;
                else
                {
                    topic = Request.QueryString["topic"];
                    helpFile = Request.QueryString["helpfile"];

                    if (!string.IsNullOrEmpty(topic))
                        topic = DocSiteNavigator.FormatTopic(topic, false);
                }
            }
            else
            {
                string topicPath = ContentUrl.Value;

                if (!string.IsNullOrEmpty(topicPath))
                {
                    Uri topicUri;

                    if (Uri.TryCreate(topicPath, UriKind.RelativeOrAbsolute, out topicUri)
                        && (!topicUri.IsAbsoluteUri || topicUri.Host.Equals(Request.Url.Host, StringComparison.OrdinalIgnoreCase)))
                    {
                        helpFile = (topicUri.IsAbsoluteUri) ? topicUri.AbsolutePath : topicUri.ToString();

                        if (helpFile.StartsWith("/") || helpFile.StartsWith(@"\"))
                            helpFile = helpFile.Substring(1);
                    }
                }
            }

            if (string.IsNullOrEmpty(helpFile))
                helpFile = DocSiteNavigator.ResolveTopicHelpFile(topic, false) ?? DocSiteManager.Settings.HelpFileNotFoundPath;

            if (string.IsNullOrEmpty(topic))
            {
                topic = DocSiteNavigator.ResolveHelpFileTopic(helpFile, false);

                if (topic == null)
                    helpFile = DocSiteManager.Settings.HelpFileNotFoundPath;
            }

            ContentPath = helpFile;

            Controls.DocSiteSidebar sidebar = ((DocSite)Page.Master).Sidebar;

            if (!sidebar.TableOfContents.SelectedTopic.Equals(topic, StringComparison.Ordinal))
                sidebar.Initialize(topic, helpFile);
        }

        protected string GetSyncTocClientCallback()
        {
            return ((DocSite)Master).Sidebar.TableOfContents.GetSyncTocClientCallback();
        }
        #endregion

        #region Event Handlers
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            SyncSidebar();

            string script = @"
function ContentFrame_onload(eventObj)
{
	if (!contentFrame_FirstLoad)
	{
		var docSiteContentUrl = document.getElementById('" + ContentUrl.ClientID + @"');

		try { docSiteContentUrl.value = window.frames[0].location; }
		catch (e) { }  // Access denied

		" + GetSyncTocClientCallback() + @";
	}

	contentFrame_FirstLoad = false;
}";

            ScriptManager.RegisterStartupScript(Page, this.GetType(), "updateDocSitePath", script, true);
        }

        protected override void OnPreRender(EventArgs e)
        {
            ContentFrame.Attributes.Add("onload", "ContentFrame_onload(this);");

            base.OnPreRender(e);
        }
        #endregion

        #region IDocSiteDefault Members
        public string ContentPath
        {
            get
            {
                return ContentFrame.Attributes["src"];
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                ContentFrame.Attributes["src"] = value;
            }
        }

        public string SelectedTopic
        {
            get
            {
                return ((DocSite)Master).Sidebar.TableOfContents.SelectedTopic;
            }
        }
        #endregion
    }
}
