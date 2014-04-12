using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Persistence
{
    public class Range
    {
        public Range(int skip, int take)
        {
            Skip = skip;
            Take = take;
        }

        public int Skip { get; set; }

        public int Take { get; set; }

		public override bool Equals(object obj)
		{
			var other = obj as Range;
			return other != null
				&& other.Skip == Skip
				&& other.Take == Take;
		}

		public override int GetHashCode()
		{
			int hash = 17;
			Utility.AppendHashCode(ref hash, Skip);
			Utility.AppendHashCode(ref hash, Take);
			return hash;
		}
    }
}
