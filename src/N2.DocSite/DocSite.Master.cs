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
    public partial class DocSite : System.Web.UI.MasterPage
    {
        #region Public Properties
        public static readonly string ProjectTitle = GetAssemblyTitle();
        public static readonly string ProjectCompany = GetAssemblyCompany();
        public static readonly string ProjectCopyright = GetAssemblyCopyright();

        public Controls.DocSiteSidebar Sidebar
        {
            get
            {
                return sideBarControl;
            }
        }

        public string CurrentTopicTitle
        {
            get
            {
                string topic = sideBarControl.TableOfContents.SelectedTopic;

                if (string.IsNullOrEmpty(topic))
                    topic = sideBarControl.TableOfContents.DefaultTopic;
                else
                {
                    int topicNameIndex = topic.LastIndexOf('/');

                    if (topicNameIndex > -1 && topic.Length > topicNameIndex + 1)
                        topic = topic.Substring(topicNameIndex + 1);
                }

                return topic;
            }
        }
        #endregion

        #region Private / Protected
        private HtmlMeta robotsMeta;
        #endregion

        #region Constructors
        /// <summary>
        /// Constructs a new instance of the <see cref="DocSite" /> class.
        /// </summary>
        public DocSite()
        {
        }
        #endregion

        #region Methods
        private static string GetAssemblyTitle()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            AssemblyTitleAttribute[] attributes = (AssemblyTitleAttribute[])assembly.GetCustomAttributes(typeof(AssemblyTitleAttribute), false);

            if (attributes == null || attributes.Length == 0)
                return string.Empty;
            else
                return attributes[0].Title;
        }

        private static string GetAssemblyCompany()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            AssemblyCompanyAttribute[] attributes = (AssemblyCompanyAttribute[])assembly.GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);

            if (attributes == null || attributes.Length == 0)
                return string.Empty;
            else
                return attributes[0].Company;
        }

        private static string GetAssemblyCopyright()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            AssemblyCopyrightAttribute[] attributes = (AssemblyCopyrightAttribute[])assembly.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);

            if (attributes == null || attributes.Length == 0)
                return string.Empty;
            else
                return attributes[0].Copyright;
        }

        protected string GetPersistSidebarHandleScript()
        {
            return
                "<script type=\"text/javascript\">" +
                    "var enablePersistSidebarHandle = " +
                    DocSiteManager.Settings.SidebarHandlePersisted.ToString().ToLowerInvariant() +
                ";</script>";
        }

        public HtmlMeta AddMetaTag(string name, string content)
        {
            HtmlMeta meta = new HtmlMeta();
            meta.Name = name;
            meta.Content = content;

            Page.Header.Controls.Add(meta);

            return meta;
        }

        public void RemoveMetaTag(HtmlMeta meta)
        {
            Page.Header.Controls.Remove(meta);
        }

        public void EnsureRobotsMetaTag(bool noindex, bool nofollow)
        {
            if (robotsMeta == null)
            {
                if (noindex || nofollow)
                    robotsMeta = AddMetaTag("ROBOTS", CreateRobotsMetaTagContent(noindex, nofollow));
            }
            else if (!noindex && !nofollow)
                RemoveMetaTag(robotsMeta);
            else
                robotsMeta.Content = CreateRobotsMetaTagContent(noindex, nofollow);
        }

        private static string CreateRobotsMetaTagContent(bool noindex, bool nofollow)
        {
            System.Text.StringBuilder content = new System.Text.StringBuilder(17);

            if (noindex)
                content.Append("NOINDEX");

            if (nofollow)
            {
                if (noindex)
                    content.Append(", ");

                content.Append("NOFOLLOW");
            }

            return content.ToString();
        }
        #endregion

        #region Event Handlers
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            if (Context.CurrentHandler is IDocSiteDefault)
                Page.Title = string.Format(System.Globalization.CultureInfo.CurrentCulture, Page.Title, ProjectTitle, CurrentTopicTitle);
        }
        #endregion
    }
}
