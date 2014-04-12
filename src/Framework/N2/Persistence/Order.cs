using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Persistence
{
    public class Order
    {
        public Order(string sortExpression)
        {
            if (sortExpression.EndsWith(" DESC", StringComparison.InvariantCultureIgnoreCase))
            {
                Descending = true;
                Property = sortExpression.Substring(0, sortExpression.Length - 5);
            }
            else
            {
                Descending = false;
                Property = sortExpression;
            }
        }

        public Order(string property, bool descending)
        {
            Property = property;
            Descending = descending;
        }

        public string Property { get; set; }
        public bool Descending { get; set; }

		public bool HasValue { get { return !string.IsNullOrEmpty(Property); } }

		public override bool Equals(object obj)
		{
			var other = obj as Order;
			return other != null
				&& other.Descending == Descending
				&& other.Property == Property;
		}

		public override int GetHashCode()
		{
			int hash = 17;
			Utility.AppendHashCode(ref hash, Descending);
			Utility.AppendHashCode(ref hash, Property);
			return hash;
		}
    }
}
