using System;
using System.Collections.Generic;
using System.Text;

namespace N2.Installation
{
	public class DatabaseStatus
	{
		private bool isConnected = false;
		private bool schema = false;
		private bool installation = false;
		private int items = 0;
		private int details = 0;
		private int detailCollections = 0;
		private int authorizedRoles = 0;
		private string connectionError = null;
		private Exception connectionException = null;
		private string schemaError = null;
		private Exception schemaException = null;
		private int rootItemID = 0;
		private int startPageID = 0;
		private ContentItem rootItem = null;
		private ContentItem startPage = null;
		private string itemsError = null;
		private string connectionType = null;

		public string ConnectionType
		{
			get { return connectionType; }
			set { connectionType = value; }
		}

		public int AuthorizedRoles
		{
			get { return authorizedRoles; }
			set { authorizedRoles = value; }
		}

		public int DetailCollections
		{
			get { return detailCollections; }
			set { detailCollections = value; }
		}

		public int Details
		{
			get { return details; }
			set { details = value; }
		}

		public int Items
		{
			get { return items; }
			set { items = value; }
		}

		public bool HasSchema
		{
			get { return schema; }
			set { schema = value; }
		}

		public bool IsConnected
		{
			get { return isConnected; }
			set { isConnected = value; }
		}

		public string ConnectionError
		{
			get { return connectionError; }
			set { connectionError = value; }
		}

		public string SchemaError
		{
			get { return schemaError; }
			set { schemaError = value; }
		}

		public Exception ConnectionException
		{
			get { return connectionException; }
			set { connectionException = value; }
		}

		public Exception SchemaException
		{
			get { return schemaException; }
			set { schemaException = value; }
		}

		public ContentItem StartPage
		{
			get { return startPage; }
			set { startPage = value; }
		}

		public ContentItem RootItem
		{
			get { return rootItem; }
			set { rootItem = value; }
		}

		public int StartPageID
		{
			get { return startPageID; }
			set { startPageID = value; }
		}

		public int RootItemID
		{
			get { return rootItemID; }
			set { rootItemID = value; }
		}

		public bool IsInstalled
		{
			get { return installation; }
			set { installation = value; }
		}

		public string ItemsError
		{
			get { return itemsError; }
			set { itemsError = value; }
		}

		public string ToStatusString()
		{
			if (StartPage != null && RootItem != null)
			{
				return string.Format("Everything looks fine, Start page: {0}, Root item: {1}, # of items: {2}",
					 StartPage.Title, RootItem.Title, Items);
			}
			else if (RootItem != null)
			{
				return string.Format("There is a root item but couldn't find a start page with the id: {0}", RootItemID);
			}
			else if (HasSchema)
			{
				return string.Format("The database is installed but there is no root item with the id: {0}", RootItemID);
			}
			else if(IsConnected)
			{
				return string.Format(
					"The connection to the database seems fine but the database tables might not be created (error message: {0})",
					SchemaError);
			}
			else
			{
				return string.Format(
					"No database or not properly configured connection string (error message: {0}", 
					ConnectionError);
			}
		}

		public string ToStartPageStatusString()
		{
			return StartPage != null
				? StartPage.Title
				: string.Format("Start page with id {0} not found.</span>", StartPageID);
		}

		public string ToRootItemStatusString()
		{
			return RootItem != null
				? RootItem.Title
				: string.Format("Root item with id {0} not found.</span>", RootItemID);
		}

		public string ToConnectionStatusString()
		{
			return IsConnected
					? "Database connection OK."
					: string.Format("<span title='{0}'>Couldn't open connection to database.</span><!--{1}-->", ConnectionError, ConnectionException);
		}
	}
}
