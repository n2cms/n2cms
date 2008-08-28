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
using System.Xml;
using DaveSexton.DocProject.DocSites;

namespace N2.DocSite.Controls
{
    public partial class DocSiteIndex : System.Web.UI.UserControl, IPostBackEventHandler
    {
        #region Public Properties
        [Browsable(false)]
        public string SelectedHelpFile
        {
            get
            {
                return ViewState["_$SelectedHelpFile"] as string ?? string.Empty;
            }
            set
            {
                if (ViewState["_$SelectedHelpFile"] as string == value)
                    return;

                ViewState["_$SelectedHelpFile"] = value;
                OnSelectedHelpFileChanged(EventArgs.Empty);
            }
        }

        [Category("Behavior"), DefaultValue(""),
        Description("Filters the index so that it only displays entries that are like the specified value.")]
        public string Filter
        {
            get
            {
                return ViewState["_$Filter"] as string ?? string.Empty;
            }
            set
            {
                ViewState["_$Filter"] = value;
            }
        }
        #endregion

        #region Private / Protected
        protected static List<Item> ItemList
        {
            get
            {
                if (itemList == null)
                {
                    lock (sync)
                    {
                        if (itemList == null)
                            itemList = LoadIndex();
                    }
                }

                return itemList;
            }
        }

        protected List<Item> FilteredItemList
        {
            get
            {
                string filter = Filter;

                List<Item> results = new List<Item>();

                if (string.IsNullOrEmpty(filter) || string.IsNullOrEmpty(filter = filter.Trim()))
                    return results;

                List<Item> list = ItemList;

                string[] keywords = filter.Split(new char[] { ' ', '.', ',' }, StringSplitOptions.RemoveEmptyEntries);

                if (keywords.Length == 0)
                    return results;

                foreach (Item item in list)
                {
                    List<string> words = new List<string>(item.Name.Split(new char[] { ' ', '.', ',' }, StringSplitOptions.RemoveEmptyEntries));
                    bool allKeywordsFound = true;

                    foreach (string keyword in keywords)
                    {
                        bool keywordFound = false;

                        foreach (string word in words)
                        {
                            if (word.Equals(keyword, StringComparison.CurrentCultureIgnoreCase))
                            {
                                keywordFound = true;
                                break;
                            }
                        }

                        if (!keywordFound)
                        {
                            allKeywordsFound = false;
                            break;
                        }
                    }

                    if (allKeywordsFound)
                        results.Add(item);
                }

                return results;
            }
        }

        /// <summary>
        /// This field is used by the repeater control's item template to store whether the current item is selected while iterating through the index.
        /// </summary>
        protected bool currentItemSelected;
        private static readonly object sync = new object();
        private volatile static List<Item> itemList;
        #endregion

        #region Constructors
        /// <summary>
        /// Constructs a new instance of the <see cref="DocSiteIndex" /> class.
        /// </summary>
        public DocSiteIndex()
        {
        }
        #endregion

        #region Methods
        private static List<Item> LoadIndex()
        {
            HttpContext context = HttpContext.Current;

            if (context == null)
                throw new InvalidOperationException("The LoadIndex method, which initializes the DocSite index, cannot be called outside of a request context.");

            List<Item> items = new List<Item>();

            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreComments = true;
            settings.IgnoreProcessingInstructions = true;
            settings.IgnoreWhitespace = true;

            using (XmlReader reader = XmlReader.Create(
                context.Server.MapPath(DocSiteManager.Settings.DocSiteIndexXmlSource),
                settings))
            {
                reader.MoveToContent();

                while (reader.Read())
                // for each entry
                {
                    if (!reader.IsStartElement("entry"))
                        continue;

                    string name = reader.GetAttribute("name");
                    string file = reader.GetAttribute("file");

                    items.Add(new Item(name, file));
                }
            }

            items.Sort(delegate(Item item1, Item item2)
            {
                return string.Compare(item1.Name, item2.Name, StringComparison.CurrentCulture);
            });

            return items;
        }

        protected string GetPostBackClientHyperlink(string file)
        {
            return Page.ClientScript.GetPostBackClientHyperlink(this, file, false).Replace('\'', '"');
        }
        #endregion

        #region Events
        private readonly object SelectedHelpFileChangedEvent = new object();

        /// <summary>
        /// Event raised after the <see cref="SelectedHelpFile" /> property value has changed.
        /// </summary>
        [Category("Property Changed")]
        [Description("Event raised after the SelectedHelpFile property value has changed.")]
        public event EventHandler SelectedHelpFileChanged
        {
            add
            {
                lock (SelectedHelpFileChangedEvent)
                {
                    Events.AddHandler(SelectedHelpFileChangedEvent, value);
                }
            }
            remove
            {
                lock (SelectedHelpFileChangedEvent)
                {
                    Events.RemoveHandler(SelectedHelpFileChangedEvent, value);
                }
            }
        }

        /// <summary>
        /// Raises the <see cref="SelectedHelpFileChanged" /> event.
        /// </summary>
        /// <param name="e"><see cref="EventArgs" /> object that provides the arguments for the event.</param>
        protected virtual void OnSelectedHelpFileChanged(EventArgs e)
        {
            EventHandler handler = null;

            lock (SelectedHelpFileChangedEvent)
            {
                handler = (EventHandler)Events[SelectedHelpFileChangedEvent];
            }

            if (handler != null)
                handler(this, e);
        }
        #endregion

        #region Event Handlers
        protected void indexFilterButton_Click(object sender, ImageClickEventArgs e)
        {
            Filter = indexFilterTextBox.Text;
        }

        protected override void OnPreRender(EventArgs e)
        {
            indexFilterTextBox.Attributes.Add("onkeypress", "checkSubmitOnEnter(event, document.getElementById('" + indexFilterButton.ClientID + "'));");

            indexFilterDiv.Visible = indexFilterCheckbox.Checked;

            indexRepeater.DataSource = (indexFilterCheckbox.Checked) ? FilteredItemList : ItemList;
            indexRepeater.DataBind();

            index_list_container.Attributes.Add("class", (indexFilterCheckbox.Checked) ? "index_list index_list_filtered" : "index_list");

            base.OnPreRender(e);
        }
        #endregion

        #region IPostBackEventHandler Members
        public void RaisePostBackEvent(string eventArgument)
        {
            SelectedHelpFile = eventArgument;
        }
        #endregion

        #region Nested
        protected struct Item : IEquatable<Item>
        {
            public string File { get { return file; } }
            public string Name { get { return name; } }

            private readonly string file, name;

            public Item(string name, string file)
            {
                this.file = file;
                this.name = name;
            }

            public override bool Equals(object obj)
            {
                if (obj is Item)
                    return Equals((Item)obj);
                else
                    return false;
            }

            public override int GetHashCode()
            {
                return name.GetHashCode() ^ file.GetHashCode();
            }

            public override string ToString()
            {
                return name;
            }

            public static bool operator ==(Item t1, Item t2)
            {
                return t1.Equals(t2);
            }

            public static bool operator !=(Item t1, Item t2)
            {
                return !t1.Equals(t2);
            }

            #region IEquatable<Item> Members
            public bool Equals(Item other)
            {
                return string.Equals(other.file, file, StringComparison.Ordinal)
                    && string.Equals(other.name, name, StringComparison.Ordinal);
            }
            #endregion
        }
        #endregion
    }
}