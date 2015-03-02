using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Persistence;
using System.Diagnostics;
using N2.Collections;

namespace N2.Persistence
{
	[DebuggerDisplay("ParameterCollection: Count = {parameters.Count}")]
	[DebuggerTypeProxy(typeof(CollectionDebugView<>))]
	public class ParameterCollection : ICollection<IParameter>, IParameter
    {
        public ParameterCollection()
			: this(Operator.And)
		{
        }

        public ParameterCollection(Operator op)
        {
            Operator = op;
        }

        public ParameterCollection(params IParameter[] parameters)
            : this(Operator.And, parameters)
        {
        }

		public ParameterCollection(IEnumerable<IParameter> parameters)
			: this(Operator.And, parameters)
		{
		}

		public ParameterCollection(Operator op, IEnumerable<IParameter> parameters)
			: this(op)
		{
			this.parameters.AddRange(parameters);
		}

        List<IParameter> parameters = new List<IParameter>();

        public Operator Operator { get; set; }

        #region ICollection<IParameter>
        public void Add(IParameter item)
        {
            parameters.Add(item);
        }

        public void Clear()
        {
            parameters.Clear();
        }

        public bool Contains(IParameter item)
        {
            return parameters.Contains(item);
        }

        public void CopyTo(IParameter[] array, int arrayIndex)
        {
            parameters.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return parameters.Count; }
        }

        bool ICollection<IParameter>.IsReadOnly
        {
            get { return ((ICollection<IParameter>)parameters).IsReadOnly; }
        }

        public bool Remove(IParameter item)
        {
            return parameters.Remove(item);
        }
        #endregion

        #region IEnumerable<IParameter>
        public IEnumerator<IParameter> GetEnumerator()
        {
            return parameters.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion

        public static ParameterCollection operator &(ParameterCollection q1, IParameter q2)
        {
            return new ParameterCollection(Persistence.Operator.And) { { q1 }, { q2 } };
        }
        public static ParameterCollection operator |(ParameterCollection q1, IParameter q2)
        {
            return new ParameterCollection(Persistence.Operator.Or) { { q1 }, { q2 } };
        }

		public static ParameterCollection Empty
		{
			get { return new ParameterCollection(); }
		}

        public bool IsMatch(object item)
        {
			if (Count == 0)
				return true;

            if (Operator == Persistence.Operator.And)
                return this.All(p => p.IsMatch(item));

            return this.Any(p => p.IsMatch(item));
        }

        public ParameterCollection OrderBy(string expression)
        {
            this.Order = new Order(expression);
            return this;
        }

        public ParameterCollection Skip(int skip)
        {
            this.Range = new Range(skip, Range != null ? Range.Take : 0);
            return this;
        }

        public ParameterCollection Take(int take)
        {
            this.Range = new Range(Range != null ? Range.Skip : 0, take);
            return this;
        }

        public Order Order { get; set; }
        public Range Range { get; set; }

		#region Equals & GetHashCode
		public override bool Equals(object obj)
		{
			var other = obj as ParameterCollection;
			return other != null
				&& other.Operator == Operator
				&& Utility.Compare(other.Order, Order)
				&& ParametersEquals(other)
				&& Utility.Compare(other.Range, Range);
		}

		private bool ParametersEquals(ParameterCollection other)
		{
			if (other.parameters.Count != parameters.Count)
				return false;

			for (int i = 0; i < parameters.Count; i++)
			{
				if (other.parameters[i] != parameters[i])
					return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			int hash = 17;
			Utility.AppendHashCode(ref hash, Operator);
			Utility.AppendHashCode(ref hash, Order);
			foreach(var parameter in parameters)
				Utility.AppendHashCode(ref hash, parameter);
			Utility.AppendHashCode(ref hash, Range);
			return hash;
		}

        public override string ToString()
        {
            return string.Join((Operator == Persistence.Operator.And ? " & " : " | "), parameters.Select(p => p.ToString()))
                + (Range == null ? "" : (" (" + Range.Skip + " - " + (Range.Skip + Range.Take)) + ")")
                + (Order == null ? "" : (" (by " + Order.Property + (Order.Descending ? " DESC" : "")) + ")");
        }
		#endregion
	}
}
