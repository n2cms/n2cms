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
    }
}
