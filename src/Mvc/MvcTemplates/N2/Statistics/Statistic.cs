using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace N2.Management.Statistics
{
	public class Statistic
	{
		public int PageID { get; set; }
		public DateTime TimeSlot { get; set; }
		public int Views { get; set; }

		public override string ToString()
		{
			return string.Format("{0:yyyy-MM-dd hh:mm:ss} #{1}: {2}", TimeSlot, PageID, Views);
		}

		public override bool Equals(object obj)
		{
			var other = obj as Statistic;
			return other != null 
				&& other.PageID == PageID 
				&& other.TimeSlot == TimeSlot;
		}

		public override int GetHashCode()
		{
			return 27
				* PageID.GetHashCode()
				* TimeSlot.GetHashCode();
		}
	}
}