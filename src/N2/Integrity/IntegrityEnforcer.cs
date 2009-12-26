using System;
using N2.Persistence;
using N2.Plugin;

namespace N2.Integrity
{
	/// <summary>
	/// Subscribes to persister envents and throws exceptions if something 
	/// illegal is about to be done.
	/// </summary>
	public class IntegrityEnforcer : IIntegrityEnforcer, IAutoStart
	{
		private readonly IPersister persister;
		private readonly IIntegrityManager integrity;
		private bool enabled = true;

		public IntegrityEnforcer(IPersister persister, IIntegrityManager integrity)
		{
			this.persister = persister;
			this.integrity = integrity;
		}

		/// <summary>Gets or sets wether the integrity is enforced.</summary>
		public bool Enabled
		{
			get { return enabled; }
			set { enabled = value; }
		}

		#region Event Dispatchers

		private void ItemSavingEvenHandler(object sender, CancellableItemEventArgs e)
		{
			OnItemSaving(e.AffectedItem);
		}

		private void ItemMovingEvenHandler(object sender, CancellableDestinationEventArgs e)
		{
			OnItemMoving(e.AffectedItem, e.Destination);
		}

		private void ItemDeletingEvenHandler(object sender, CancellableItemEventArgs e)
		{
			OnItemDeleting(e.AffectedItem);
		}

		private void ItemCopyingEvenHandler(object sender, CancellableDestinationEventArgs e)
		{
			OnItemCopying(e.AffectedItem, e.Destination);
		}

		#endregion

		#region Event Handlers

		protected virtual void OnItemSaving(ContentItem item)
		{
			N2Exception ex = integrity.GetSaveException(item);
			if (Enabled && ex != null)
				throw ex;
		}

		protected virtual void OnItemMoving(ContentItem source, ContentItem destination)
		{
			N2Exception ex = integrity.GetMoveException(source, destination);
			if (Enabled && ex != null)
				throw ex;
		}

		protected virtual void OnItemDeleting(ContentItem item)
		{
			N2Exception ex = integrity.GetDeleteException(item);
			if (Enabled && ex != null)
				throw ex;
		}

		protected virtual void OnItemCopying(ContentItem source, ContentItem destination)
		{
			N2Exception ex = integrity.GetCopyException(source, destination);
			if (Enabled && ex != null)
				throw ex;
		}

		#endregion

		#region IStartable Members

		public virtual void Start()
		{
			persister.ItemCopying += ItemCopyingEvenHandler;
			persister.ItemDeleting += ItemDeletingEvenHandler;
			persister.ItemMoving += ItemMovingEvenHandler;
			persister.ItemSaving += ItemSavingEvenHandler;
		}

		public virtual void Stop()
		{
			persister.ItemCopying -= ItemCopyingEvenHandler;
			persister.ItemDeleting -= ItemDeletingEvenHandler;
			persister.ItemMoving -= ItemMovingEvenHandler;
			persister.ItemSaving -= ItemSavingEvenHandler;
		}

		#endregion

		#region IAutoStart Members

		void IAutoStart.Start()
		{
            Start();
		}

		#endregion
	}
}