namespace N2.Collections
{
	using System;
	using System.Collections.Generic;


	/// <summary>
	/// Filters based on start index and count.
	/// </summary>
	public class CountFilter : ItemFilter
	{
		#region Constructors
		/// <summary>Creates a new instance of the <see cref="CountFilter"/>.</summary>
		/// <param name="startIndex">The index of the first item to show.</param>
		/// <param name="maxCount">The maximum number of items to leave.</param>
		public CountFilter(int startIndex, int maxCount)
		{
			this.startIndex = startIndex >= 0 ? startIndex : 0;
			this.maxCount = maxCount >= 0 ? maxCount : 0;
		}
		#endregion

		#region Private Members
		private int startIndex;
		private int maxCount;
		private int currentIndex;
		#endregion

		#region Properties
		public int StartIndex
		{
			get { return startIndex; }
			set { startIndex = value; }
		}

		public int MaxCount
		{
			get { return maxCount; }
			set { maxCount = value; }
		}

		public int CurrentIndex
		{
			get { return currentIndex; }
			set { currentIndex = value; }
		}
		#endregion

		#region Methods
		public override void Filter(IList<ContentItem> items)
		{
			int frontSurplus = Math.Min(items.Count, startIndex);
			int endSurplus = items.Count - frontSurplus - maxCount;

			if (startIndex >= items.Count || maxCount == 0 || frontSurplus + endSurplus >= items.Count)
			{
				items.Clear();
			}
			else if (startIndex == 0 && maxCount >= items.Count)
			{
				// select all, hence do nothing
			}
			else
			{
				for (int i = 0; i < frontSurplus; i++) items.RemoveAt(0);
				for (int i = 0; i < endSurplus; i++) items.RemoveAt(items.Count - 1);
			}
		}

		/// <summary>This method doesn't consider the input item at all. Instead it increments and compares against this filter's CurrentIndex property.</summary>
		public override bool Match(ContentItem item)
		{
			return Match(CurrentIndex++);
		}

		public virtual bool Match(int itemIndex)
		{
			return itemIndex >= StartIndex
				&& (MaxCount == int.MaxValue || itemIndex < (StartIndex + MaxCount));
		}

		public virtual void Reset()
		{
			CurrentIndex = 0;
		}
		#endregion

		#region Static Methods
		public static void Filter(IList<ContentItem> items, int startIndex, int maxCount)
		{
			ItemFilter.Filter(items, new CountFilter(startIndex, maxCount));
		}
		#endregion

		#region IDisposable Members

		public override void Dispose()
		{
			Reset();
		}

		#endregion

		public override string ToString()
		{
			return StartIndex + "+" + MaxCount;
		}
	}
}
