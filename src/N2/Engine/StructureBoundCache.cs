using System;
using System.Collections.Generic;
using N2.Persistence;
using N2.Plugin;

namespace N2.Engine
{

	/// <summary>
	/// Caches a value that is expired when a content item is moved or deleted.
	/// </summary>
	/// <typeparam name="T">The type of value to store.</typeparam>
	[Service]
	public class StructureBoundCache<T> : IAutoStart where T:class
	{
		IPersister persister;

		public StructureBoundCache(IPersister persister)
		{
			this.persister = persister;
		}

		/// <summary>The value that is cached.</summary>
		public T Value { get; set; }

		/// <summary>Gets an existing or creates a new cached value.</summary>
		/// <param name="valueFactory">The method that creates the value when it is null.</param>
		/// <returns>The value.</returns>
		public virtual T GetValue(Func<T> valueFactoryMethod)
		{
			if (Value == null)
			{
				lock (this)
				{
					if (Value != null)
						return Value;

					Value = valueFactoryMethod();
					if (ValueCreated != null)
						ValueCreated.Invoke(this, new ValueEventArgs<T> { Value = Value });
				}
			}

			return Value;
		}

		public void Expire()
		{
			if (ValueExpiring != null)
				ValueExpiring.Invoke(this, new ValueEventArgs<T> { Value = Value });
			Value = null;
		}

		public event EventHandler<ValueEventArgs<T>> ValueCreated;
		public event EventHandler<ValueEventArgs<T>> ValueExpiring;

		#region IAutoStart Members

		public void Start()
		{
			persister.ItemCopied += StateChanged;
			persister.ItemDeleted += StateChanged;
			persister.ItemMoved += StateChanged;
			persister.ItemSaved += StateChanged;
		}

		public void Stop()
		{
			persister.ItemCopied -= StateChanged;
			persister.ItemDeleted -= StateChanged;
			persister.ItemMoved -= StateChanged;
			persister.ItemSaved -= StateChanged;
		}

		void StateChanged(object sender, EventArgs e)
		{
			Expire();
		}

		#endregion
	}
}
