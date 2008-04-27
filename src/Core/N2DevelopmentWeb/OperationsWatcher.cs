using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Text;

namespace N2DevelopmentWeb
{
	public static class OperationsWatcher
	{
		public static string LogText
		{
			get { return log.ToString(); }
		}
		static StringBuilder log = new StringBuilder();

		public static void StartWatching()
		{
			//N2.Factory.Created += new EventHandler<N2.Engine.FactoryEventArgs>(Factory_Initialized);
			//N2.Factory.Creating += new EventHandler<N2.Engine.FactoryEventArgs>(Factory_Initializing);
			//N2.Factory.Persister.ItemCopied += new EventHandler<N2.Persistence.DestinationEventArgs>(Persister_ItemCopied);
			//N2.Factory.Persister.ItemCopying += new EventHandler<N2.Persistence.CancellableDestinationEventArgs>(Persister_ItemCopying);
			//N2.Factory.Persister.ItemDeleted += new EventHandler<N2.Persistence.ItemEventArgs>(Persister_ItemDeleted);
			//N2.Factory.Persister.ItemDeleting += new EventHandler<N2.Persistence.CancellableItemEventArgs>(Persister_ItemDeleting);
			////N2.Factory.Persister.ItemLoaded += new EventHandler<N2.Persistence.ItemEventArgs>(Persister_ItemLoaded);
			//N2.Factory.Persister.ItemMoved += new EventHandler<N2.Persistence.DestinationEventArgs>(Persister_ItemMoved);
			//N2.Factory.Persister.ItemMoving += new EventHandler<N2.Persistence.CancellableDestinationEventArgs>(Persister_ItemMoving);
			//N2.Factory.Persister.ItemSaved += new EventHandler<N2.Persistence.ItemEventArgs>(Persister_ItemSaved);
			//N2.Factory.Persister.ItemSaving += new EventHandler<N2.Persistence.CancellableItemEventArgs>(Persister_ItemSaving);
		}

		static void Persister_ItemSaving(object sender, N2.Persistence.CancellableItemEventArgs e)
		{
			AppendToLog("Persister_ItemSaving");
		}

		static void Persister_ItemSaved(object sender, N2.Persistence.ItemEventArgs e)
		{
			AppendToLog("Persister_ItemSaved");
		}

		static void Persister_ItemMoving(object sender, N2.Persistence.CancellableDestinationEventArgs e)
		{
			AppendToLog("Persister_ItemMoving");
		}

		static void Persister_ItemMoved(object sender, N2.Persistence.DestinationEventArgs e)
		{
			AppendToLog("Persister_ItemMoved");
		}

		static void Persister_ItemLoaded(object sender, N2.Persistence.ItemEventArgs e)
		{
			AppendToLog("Persister_ItemLoaded");
		}

		static void Persister_ItemDeleting(object sender, N2.Persistence.CancellableItemEventArgs e)
		{
			AppendToLog("Persister_ItemDeleting");
		}

		static void Persister_ItemDeleted(object sender, N2.Persistence.ItemEventArgs e)
		{
			AppendToLog("Persister_ItemDeleted");
		}

		static void Persister_ItemCopying(object sender, N2.Persistence.CancellableDestinationEventArgs e)
		{
			AppendToLog("Persister_ItemCopying");
		}

		static void Persister_ItemCopied(object sender, N2.Persistence.DestinationEventArgs e)
		{
			AppendToLog("Persister_ItemCopied");
		}

		private static void AppendToLog(string text)
		{
			log.Append(DateTime.Now.ToString() + ": " + text + Environment.NewLine);
		}
	}
}
