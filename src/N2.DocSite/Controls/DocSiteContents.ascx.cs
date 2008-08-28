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
using System.ComponentModel;
using System.Collections.Generic;
using DaveSexton.DocProject.DocSites;

namespace N2.DocSite.Controls
{
    public partial class DocSiteContents : System.Web.UI.UserControl
    {
        #region Public Properties
        [Browsable(false)]
        public string SelectedTopicFullTitle
        {
            get
            {
                return string.Format(System.Globalization.CultureInfo.CurrentCulture,
                    (string)GetGlobalResourceObject("General", "TopicTitleFormat"), DocSite.ProjectTitle, SelectedTopicTitle);
            }
        }

        [Browsable(false)]
        public string SelectedTopicTitle
        {
            get
            {
                string topic = SelectedTopic;
                return topic.Substring(topic.LastIndexOf('/') + 1);
            }
        }

        [Browsable(false)]
        public string SelectedTopicUrl
        {
            get
            {
                return new Uri(Request.Url,
                    DocSiteNavigator.GetTopicUrl(SelectedTopic, true)).ToString();
            }
        }

        [Browsable(false)]
        public string SelectedTopic
        {
            get
            {
                TreeNode node = contentsTreeView.SelectedNode;
                return (node == null) ? string.Empty : node.ValuePath;
            }
            set
            {
                if (contentsTreeView.Nodes.Count == 0)
                    contentsTreeView.DataBind();

                TreeNode node = FindTopicNode(value);

                if (node != null)
                {
                    if (!node.Selected)
                        node.Select();

                    do
                    {
                        if (!node.Expanded.GetValueOrDefault())
                            node.Expand();

                        node = node.Parent;
                    }
                    while (node != null);
                }
            }
        }

        [Browsable(false)]
        public string DefaultTopic
        {
            get
            {
                return (contentsTreeView.Nodes.Count > 0)
                    ? contentsTreeView.Nodes[0].Value
                    : string.Empty;
            }
        }
        #endregion

        #region Private / Protected
        #endregion

        #region Constructors
        /// <summary>
        /// Constructs a new instance of the <see cref="DocSiteContents" /> class.
        /// </summary>
        public DocSiteContents()
        {
        }
        #endregion

        #region Methods
        private TreeNode FindTopicNode(string topic)
        {
            if (contentsTreeView.Nodes.Count == 0)
                return null;
            else if (string.IsNullOrEmpty(topic))
                return contentsTreeView.Nodes[0];
            else
            {
                List<string> topics = new List<string>(topic.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries));

                if (topics.Count == 0)
                    return null;

                // this method is used in place of TreeView.FindNode because it supports delayed loading
                return FindTopicNodeRecursive(topics, contentsTreeView.Nodes);
            }
        }

        private TreeNode FindTopicNodeRecursive(List<string> topics, TreeNodeCollection nodes)
        {
            foreach (TreeNode currentNode in nodes)
            {
                if (topics[0].Equals(currentNode.Text, StringComparison.Ordinal))
                {
                    topics.RemoveAt(0);

                    if (topics.Count == 0)
                        return currentNode;
                    else if (!currentNode.Expanded.GetValueOrDefault(false))
                        // This call is required for nodes where PopulateOnDemand is true;
                        // otherwise, ChildNodes is always empty.
                        currentNode.Expand();

                    TreeNode foundNode = FindTopicNodeRecursive(topics, currentNode.ChildNodes);

                    if (foundNode != null)
                        return foundNode;
                }
            }

            return null;
        }

        public string GetSyncTocClientCallback()
        {
            return Page.ClientScript.GetPostBackEventReference(tocSyncButton, string.Empty);
        }
        #endregion

        #region Events
        private readonly object SelectedTopicChangedEvent = new object();

        /// <summary>
        /// Event raised after the <see cref="SelectedTopic" /> property value has changed.
        /// </summary>
        [Category("Property Changed")]
        [Description("Event raised after the SelectedTopic property value has changed.")]
        public event EventHandler SelectedTopicChanged
        {
            add
            {
                lock (SelectedTopicChangedEvent)
                {
                    Events.AddHandler(SelectedTopicChangedEvent, value);
                }
            }
            remove
            {
                lock (SelectedTopicChangedEvent)
                {
                    Events.RemoveHandler(SelectedTopicChangedEvent, value);
                }
            }
        }

        /// <summary>
        /// Raises the <see cref="SelectedTopicChanged" /> event.
        /// </summary>
        /// <param name="e"><see cref="EventArgs" /> object that provides the arguments for the event.</param>
        protected virtual void OnSelectedTopicChanged(EventArgs e)
        {
            EventHandler handler = null;

            lock (SelectedTopicChangedEvent)
            {
                handler = (EventHandler)Events[SelectedTopicChangedEvent];
            }

            if (handler != null)
                handler(this, e);
        }
        #endregion

        #region Event Handlers
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            string data = DocSiteNavigator.DocSiteContentsDocument.CreateNavigator().OuterXml;
            contentsXmlDataSource.Data = data;

            ScriptManager.RegisterStartupScript(Page, GetType(), "initializeTocButtons", "initializeTocButtons();", true);
        }

        protected override void OnPreRender(EventArgs e)
        {
            string subject = Server.HtmlEncode(string.Format(System.Globalization.CultureInfo.CurrentCulture,
                (string)GetGlobalResourceObject("General", "EmailTopicUrlSubjectFormat"), DocSite.ProjectTitle, SelectedTopicTitle, SelectedTopicUrl));

            string body = Server.HtmlEncode(string.Format(System.Globalization.CultureInfo.CurrentCulture,
                (string)GetGlobalResourceObject("General", "EmailTopicUrlBodyFormat"), DocSite.ProjectTitle, SelectedTopic, SelectedTopicUrl));

            emailImageButton.OnClientClick = "emailUrl(event, '" + subject + "', '" + body + "'); return false;";

            base.OnPreRender(e);
        }

        protected void contentsTreeView_SelectedNodeChanged(object sender, EventArgs e)
        {
            OnSelectedTopicChanged(e);
        }
        #endregion
    }
}