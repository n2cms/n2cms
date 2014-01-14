using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using N2.Persistence;
using System.Reflection;
using N2.Edit.Versioning;

namespace N2.Addons.UITests.UI
{
    public partial class Events : System.Web.UI.Page
    {
        protected static bool eventsSubscribed;
        protected static StringBuilder events = new StringBuilder();
        protected static bool loadEvent;

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
        
            txtEventList.Text = events.ToString();
            btnSubscribe.Enabled = !eventsSubscribed;
            btnUnsubscribe.Enabled = eventsSubscribed;
            chkLoad.Enabled = !eventsSubscribed;
            chkLoad.Checked = loadEvent;
        }

        protected void btnSubscribe_Click(object sender, EventArgs e)
        {
            eventsSubscribed = true;
            N2.Context.Current.Persister.ItemCopied += new EventHandler<DestinationEventArgs>(Persister_ItemCopied);
            N2.Context.Current.Persister.ItemCopying += new EventHandler<CancellableDestinationEventArgs>(Persister_ItemCopying);
            N2.Context.Current.Persister.ItemDeleted += new EventHandler<ItemEventArgs>(Persister_ItemDeleted);
            N2.Context.Current.Persister.ItemDeleting += new EventHandler<CancellableItemEventArgs>(Persister_ItemDeleting);
            if (chkLoad.Checked)
                N2.Context.Current.Persister.ItemLoaded += new EventHandler<ItemEventArgs>(Persister_ItemLoaded);
            loadEvent = chkLoad.Checked;
            N2.Context.Current.Persister.ItemMoved += new EventHandler<DestinationEventArgs>(Persister_ItemMoved);
            N2.Context.Current.Persister.ItemMoving += new EventHandler<CancellableDestinationEventArgs>(Persister_ItemMoving);
            N2.Context.Current.Persister.ItemSaved += new EventHandler<ItemEventArgs>(Persister_ItemSaved);
            N2.Context.Current.Persister.ItemSaving += new EventHandler<CancellableItemEventArgs>(Persister_ItemSaving);
            N2.Context.Current.UrlParser.PageNotFound += new EventHandler<N2.Web.PageNotFoundEventArgs>(UrlParser_PageNotFound);
            N2.Context.Current.Definitions.ItemCreated += new EventHandler<ItemEventArgs>(Definitions_ItemCreated);
            ((VersionManager)N2.Context.Current.Resolve<IVersionManager>()).ItemReplacedVersion += new EventHandler<ItemEventArgs>(Events_ItemReplacedVersion);
            ((VersionManager)N2.Context.Current.Resolve<IVersionManager>()).ItemReplacingVersion += new EventHandler<CancellableDestinationEventArgs>(Events_ItemReplacingVersion);
            ((VersionManager)N2.Context.Current.Resolve<IVersionManager>()).ItemSavedVersion += new EventHandler<ItemEventArgs>(Events_ItemSavedVersion);
            ((VersionManager)N2.Context.Current.Resolve<IVersionManager>()).ItemSavingVersion += new EventHandler<CancellableItemEventArgs>(Events_ItemSavingVersion);
        }

        protected void btnUnsubscribe_Click(object sender, EventArgs e)
        {
            eventsSubscribed = false;
            events.Length = 0;
            N2.Context.Current.Persister.ItemCopied -= new EventHandler<DestinationEventArgs>(Persister_ItemCopied);
            N2.Context.Current.Persister.ItemCopying -= new EventHandler<CancellableDestinationEventArgs>(Persister_ItemCopying);
            N2.Context.Current.Persister.ItemDeleted -= new EventHandler<ItemEventArgs>(Persister_ItemDeleted);
            N2.Context.Current.Persister.ItemDeleting -= new EventHandler<CancellableItemEventArgs>(Persister_ItemDeleting);
            if (chkLoad.Checked)
                N2.Context.Current.Persister.ItemLoaded -= new EventHandler<ItemEventArgs>(Persister_ItemLoaded);
            N2.Context.Current.Persister.ItemMoved -= new EventHandler<DestinationEventArgs>(Persister_ItemMoved);
            N2.Context.Current.Persister.ItemMoving -= new EventHandler<CancellableDestinationEventArgs>(Persister_ItemMoving);
            N2.Context.Current.Persister.ItemSaved -= new EventHandler<ItemEventArgs>(Persister_ItemSaved);
            N2.Context.Current.Persister.ItemSaving -= new EventHandler<CancellableItemEventArgs>(Persister_ItemSaving);
            N2.Context.Current.UrlParser.PageNotFound -= new EventHandler<N2.Web.PageNotFoundEventArgs>(UrlParser_PageNotFound);
            N2.Context.Current.Definitions.ItemCreated -= new EventHandler<ItemEventArgs>(Definitions_ItemCreated);
            ((VersionManager)N2.Context.Current.Resolve<IVersionManager>()).ItemReplacedVersion -= new EventHandler<ItemEventArgs>(Events_ItemReplacedVersion);
            ((VersionManager)N2.Context.Current.Resolve<IVersionManager>()).ItemReplacingVersion -= new EventHandler<CancellableDestinationEventArgs>(Events_ItemReplacingVersion);
            ((VersionManager)N2.Context.Current.Resolve<IVersionManager>()).ItemSavedVersion -= new EventHandler<ItemEventArgs>(Events_ItemSavedVersion);
            ((VersionManager)N2.Context.Current.Resolve<IVersionManager>()).ItemSavingVersion -= new EventHandler<CancellableItemEventArgs>(Events_ItemSavingVersion);
        }

        void Events_ItemSavingVersion(object sender, CancellableItemEventArgs e)
        {
            events.AppendLine(DateTime.Now + ": " + MethodBase.GetCurrentMethod().Name + ", " + e.AffectedItem);
        }

        void Events_ItemSavedVersion(object sender, ItemEventArgs e)
        {
            events.AppendLine(DateTime.Now + ": " + MethodBase.GetCurrentMethod().Name + ", " + e.AffectedItem);
        }

        void Events_ItemReplacingVersion(object sender, CancellableDestinationEventArgs e)
        {
            events.AppendLine(DateTime.Now + ": " + MethodBase.GetCurrentMethod().Name + ", " + e.AffectedItem);
        }

        void Events_ItemReplacedVersion(object sender, ItemEventArgs e)
        {
            events.AppendLine(DateTime.Now + ": " + MethodBase.GetCurrentMethod().Name + ", " + e.AffectedItem);
        }

        void Definitions_ItemCreated(object sender, ItemEventArgs e)
        {
            events.AppendLine(DateTime.Now + ": " + MethodBase.GetCurrentMethod().Name + ", " + e.AffectedItem);
        }

        void UrlParser_PageNotFound(object sender, N2.Web.PageNotFoundEventArgs e)
        {
            events.AppendLine(DateTime.Now + ": " + MethodBase.GetCurrentMethod().Name + ", " + e.AffectedItem);
        }

        void Persister_ItemSaving(object sender, CancellableItemEventArgs e)
        {
            events.AppendLine(DateTime.Now + ": " + MethodBase.GetCurrentMethod().Name + ", " + e.AffectedItem);
        }

        void Persister_ItemSaved(object sender, ItemEventArgs e)
        {
            events.AppendLine(DateTime.Now + ": " + MethodBase.GetCurrentMethod().Name + ", " + e.AffectedItem);
        }

        void Persister_ItemMoving(object sender, CancellableDestinationEventArgs e)
        {
            events.AppendLine(DateTime.Now + ": " + MethodBase.GetCurrentMethod().Name + ", " + e.AffectedItem);
        }

        void Persister_ItemMoved(object sender, DestinationEventArgs e)
        {
            events.AppendLine(DateTime.Now + ": " + MethodBase.GetCurrentMethod().Name + ", " + e.AffectedItem);
        }

        void Persister_ItemLoaded(object sender, ItemEventArgs e)
        {
            events.AppendLine(DateTime.Now + ": " + MethodBase.GetCurrentMethod().Name + ", " + e.AffectedItem);
        }

        void Persister_ItemDeleting(object sender, CancellableItemEventArgs e)
        {
            events.AppendLine(DateTime.Now + ": " + MethodBase.GetCurrentMethod().Name + ", " + e.AffectedItem);
        }

        void Persister_ItemDeleted(object sender, ItemEventArgs e)
        {
            events.AppendLine(DateTime.Now + ": " + MethodBase.GetCurrentMethod().Name + ", " + e.AffectedItem);
        }

        void Persister_ItemCopying(object sender, CancellableDestinationEventArgs e)
        {
            events.AppendLine(DateTime.Now + ": " + MethodBase.GetCurrentMethod().Name + ", " + e.AffectedItem);
        }

        void Persister_ItemCopied(object sender, DestinationEventArgs e)
        {
            events.AppendLine(DateTime.Now + ": " + MethodBase.GetCurrentMethod().Name + ", " + e.AffectedItem);
        }
    }
}
