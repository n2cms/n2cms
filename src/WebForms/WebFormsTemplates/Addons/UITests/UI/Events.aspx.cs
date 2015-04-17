
namespace N2.Addons.UITests.UI
{
	using System;
	using System.Text;
	using System.Reflection;
	using N2.Edit.Versioning;

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
			N2.Context.Current.Persister.ItemCopied += this.Persister_ItemCopied;
			N2.Context.Current.Persister.ItemCopying += this.Persister_ItemCopying;
			N2.Context.Current.Persister.ItemDeleted += this.Persister_ItemDeleted;
			N2.Context.Current.Persister.ItemDeleting += this.Persister_ItemDeleting;
			if (chkLoad.Checked)
				N2.Context.Current.Persister.ItemLoaded += this.Persister_ItemLoaded;
			loadEvent = chkLoad.Checked;
			N2.Context.Current.Persister.ItemMoved += this.Persister_ItemMoved;
			N2.Context.Current.Persister.ItemMoving += this.Persister_ItemMoving;
			N2.Context.Current.EventsManager.ItemSaved += this.Persister_ItemSaved;
			N2.Context.Current.EventsManager.ItemSaving += this.Persister_ItemSaving;
			N2.Context.Current.UrlParser.PageNotFound += this.UrlParser_PageNotFound;
			N2.Context.Current.Definitions.ItemCreated += this.Definitions_ItemCreated;
			((VersionManager)N2.Context.Current.Resolve<IVersionManager>()).ItemReplacedVersion += this.Events_ItemReplacedVersion;
			((VersionManager)N2.Context.Current.Resolve<IVersionManager>()).ItemReplacingVersion += this.Events_ItemReplacingVersion;
			((VersionManager)N2.Context.Current.Resolve<IVersionManager>()).ItemSavedVersion += this.Events_ItemSavedVersion;
			((VersionManager)N2.Context.Current.Resolve<IVersionManager>()).ItemSavingVersion += this.Events_ItemSavingVersion;
		}

		protected void btnUnsubscribe_Click(object sender, EventArgs e)
		{
			eventsSubscribed = false;
			events.Length = 0;
			N2.Context.Current.Persister.ItemCopied -= this.Persister_ItemCopied;
			N2.Context.Current.Persister.ItemCopying -= this.Persister_ItemCopying;
			N2.Context.Current.Persister.ItemDeleted -= this.Persister_ItemDeleted;
			N2.Context.Current.Persister.ItemDeleting -= this.Persister_ItemDeleting;
			if (chkLoad.Checked)
				N2.Context.Current.Persister.ItemLoaded -= this.Persister_ItemLoaded;
			N2.Context.Current.Persister.ItemMoved -= this.Persister_ItemMoved;
			N2.Context.Current.Persister.ItemMoving -= this.Persister_ItemMoving;
			N2.Context.Current.EventsManager.ItemSaved -= this.Persister_ItemSaved;
			N2.Context.Current.EventsManager.ItemSaving -= this.Persister_ItemSaving;
			N2.Context.Current.UrlParser.PageNotFound -= this.UrlParser_PageNotFound;
			N2.Context.Current.Definitions.ItemCreated -= this.Definitions_ItemCreated;
			((VersionManager)N2.Context.Current.Resolve<IVersionManager>()).ItemReplacedVersion -= this.Events_ItemReplacedVersion;
			((VersionManager)N2.Context.Current.Resolve<IVersionManager>()).ItemReplacingVersion -= this.Events_ItemReplacingVersion;
			((VersionManager)N2.Context.Current.Resolve<IVersionManager>()).ItemSavedVersion -= this.Events_ItemSavedVersion;
			((VersionManager)N2.Context.Current.Resolve<IVersionManager>()).ItemSavingVersion -= this.Events_ItemSavingVersion;
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
