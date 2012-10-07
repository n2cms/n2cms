#region license
// Copyright (c) 2005 - 2007 Ayende Rahien (ayende@ayende.com)
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
// 
//     * Redistributions of source code must retain the above copyright notice,
//     this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright notice,
//     this list of conditions and the following disclaimer in the documentation
//     and/or other materials provided with the distribution.
//     * Neither the name of Ayende Rahien nor the names of its
//     contributors may be used to endorse or promote products derived from this
//     software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
// THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion

using NHibernate.Type;
using System.Linq;
using System;

namespace N2.Persistence
{
	/// <summary>
	/// A repository query parameter.
	/// </summary>
	public class Parameter : N2.Persistence.IParameter
	{
		public string Name { get; set; }

		public object Value { get; set; }

		public Comparison Comparison { get; set; }

		public bool IsDetail { get; set; }

		public Parameter(string name, object value)
			: this(name, value, Comparison.Equal)
		{
		}

		public Parameter(string name, object value, Comparison comparisonType)
		{
			Name = name;
			Value = value;
			Comparison = comparisonType;
		}

		public static Parameter Equal(string name, object value)
		{
			return new Parameter(name, value);
		}

		public static Parameter NotEqual(string name, object value)
		{
			return new Parameter(name, value, Comparison.NotEqual);
		}

		public static Parameter GreaterThan(string name, object value)
		{
			return new Parameter(name, value, Comparison.GreaterThan);
		}

		public static Parameter GreaterOrEqual(string name, object value)
		{
			return new Parameter(name, value, Comparison.GreaterOrEqual);
		}

		public static Parameter LessThan(string name, object value)
		{
			return new Parameter(name, value, Comparison.LessThan);
		}

		public static Parameter LessOrEqual(string name, object value)
		{
			return new Parameter(name, value, Comparison.LessOrEqual);
		}

		public static Parameter StartsWith(string name, string value)
		{
			return new Parameter(name, value + "%", Comparison.Like);
		}

		public static Parameter Like(string name, string value)
		{
			return new Parameter(name, value, Comparison.Like);
		}

		public static Parameter NotLike(string name, string value)
		{
			return new Parameter(name, value, Comparison.NotLike);
		}

		public static Parameter IsNull(string propertyName)
		{
			return new Parameter(propertyName, null, Comparison.Null);
		}

		public static Parameter IsNotNull(string propertyName)
		{
			return new Parameter(propertyName, null, Comparison.NotNull);
		}

		public bool IsMatch(object item)
		{
            object itemValue = null;
            if (IsDetail && item is ContentItem)
            {
                itemValue = (item as ContentItem)[Name];
                if (itemValue == null)
                {
                    var collection = (item as ContentItem).GetDetailCollection(Name, false);
                    if (collection != null)
                    {
                        return collection.Any(v => Compare(v));
                    }
                }
            }
            else
                itemValue = N2.Utility.GetProperty(item, Name);
            
            return Compare(itemValue);
		}

        private bool Compare(object itemValue)
        {
            switch (this.Comparison)
            {
                case Persistence.Comparison.Equal:
                    if (Value == null)
                        return itemValue == null;
                    if (itemValue is Details.IMultipleValue)
                    {
                        return (itemValue as Details.IMultipleValue).Equals(Value);
                    }
                    return Value.Equals(itemValue);
                case Persistence.Comparison.NotEqual:
                    if (Value == null)
                        return itemValue != null;
                    if (itemValue is Details.IMultipleValue)
                    {
                        return !(itemValue as Details.IMultipleValue).Equals(Value);
                    }
                    return !Value.Equals(itemValue);
                case Persistence.Comparison.Null:
                    return itemValue == null;
                case Persistence.Comparison.NotNull:
					return itemValue != null;
                default:
					bool? result = TryCompare(itemValue as IComparable);
					if (result.HasValue)
						return result.Value;
                    throw new NotSupportedException("Operator " + Comparison + " not supported for IsMatch " + Name);
            }
        }

		private bool? TryCompare(IComparable comparable)
		{
			if (comparable == null)
				return null;

			if (this.Comparison == Persistence.Comparison.GreaterOrEqual)
				return comparable.CompareTo(Value) >= 0;
			if (this.Comparison == Persistence.Comparison.GreaterThan)
				return comparable.CompareTo(Value) > 0;
			if (this.Comparison == Persistence.Comparison.LessOrEqual)
				return comparable.CompareTo(Value) <= 0;
			if (this.Comparison == Persistence.Comparison.LessThan)
				return comparable.CompareTo(Value) < 0;

			return null;
		}

    
		#region Operators
		public static ParameterCollection operator &(Parameter q1, IParameter q2)
		{
			return new ParameterCollection(Persistence.Operator.And) { { q1 }, { q2 } };
		}
		public static ParameterCollection operator |(Parameter q1, IParameter q2)
		{
			return new ParameterCollection(Persistence.Operator.Or) { { q1 }, { q2 } };
		}
		#endregion

		#region Equals & GetHashCode
		public override bool Equals(object obj)
		{
			var other = obj as Parameter;
			return other != null && other.Name == Name && other.Value == Value;
		}

		public override int GetHashCode()
		{
			return (Name != null ? Name.GetHashCode() : GetHashCode())
				+ (Value != null ? Value.GetHashCode() : GetHashCode());
		}
		#endregion
	}
}