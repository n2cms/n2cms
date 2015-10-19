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
using N2.Definitions.Static;
using System.Collections;
using System.Diagnostics;
using System.Collections.Generic;

namespace N2.Persistence
{
    /// <summary>
    /// A repository query parameter.
    /// </summary>
    [DebuggerDisplay("{Name} {Comparison} {Value}, IsDetail={IsDetail}")]
    public class Parameter : N2.Persistence.IParameter
    {
        public string Name { get; set; }

        public object Value { get; set; }

        public Comparison Comparison { get; set; }

        /// <summary> Parameter mode (detail/property) </summary>
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

        public static Parameter TypeEqual(string discriminator)
        {
            return Parameter.Equal("class", discriminator);
        }

        public static Parameter TypeIn(string[] discriminators)
        {
            return Parameter.In("class", discriminators);
        }

        public static Parameter TypeEqual(Type type)
        {
            return TypeEqual(DefinitionMap.Instance.GetOrCreateDefinition(type).Discriminator);
        }

        public static Parameter TypeIn(params Type[] types)
        {
            return TypeIn(types.Select(t => DefinitionMap.Instance.GetOrCreateDefinition(t).Discriminator).ToArray());
        }

        public static Parameter TypeNotEqual(string discriminator)
        {
            return Parameter.NotEqual("class", discriminator);
        }

        public static Parameter TypeNotIn(string[] discriminators)
        {
            return Parameter.NotIn("class", discriminators);
        }

        public static Parameter TypeNotEqual(Type type)
        {
            return TypeNotEqual(DefinitionMap.Instance.GetOrCreateDefinition(type).Discriminator);
        }

        public static Parameter TypeNotIn(params Type[] types)
        {
            return Parameter.NotIn("class", types.Select(t => DefinitionMap.Instance.GetOrCreateDefinition(t).Discriminator).ToArray());
        }

        public static Parameter Below(ContentItem ancestor)
        {
            return Parameter.Like("AncestralTrail", ancestor.GetTrail() + "%");
        }

        public static ParameterCollection BelowOrSelf(ContentItem ancestorOrSelf)
        {
            return Parameter.Equal("ID", ancestorOrSelf.ID)
                | Parameter.Like("AncestralTrail", ancestorOrSelf.GetTrail() + "%");
        }

		public static Parameter In(string name, params object[] anyOf)
		{
			return new Parameter(name, anyOf, Comparison.In);
		}

        public static Parameter NotIn(string name, params object[] anyOf)
        {
            return new Parameter(name, anyOf, Comparison.NotIn);
        }

        public static Parameter State(ContentState expectedState)
        {
            return Parameter.Equal("State", expectedState);
        }

        public bool IsMatch(object item)
        {
            object itemValue = null;
            if (IsDetail && item is ContentItem)
            {
                var ci = (item as ContentItem);
                if (string.IsNullOrEmpty(Name))
                {
                    foreach (var detail in ci.Details)
                        if (Compare(Value, Comparison, detail.Value))
                            return true;
                    foreach (var collection in ci.DetailCollections)
                        if (collection.Any(v => Compare(Value, Comparison, v)))
                            return true;
                    return false;
                }

                itemValue = ci[Name];
                if (itemValue == null)
                {
                    var collection = (item as ContentItem).GetDetailCollection(Name, false);
                    if (collection != null)
                    {
                        return collection.Any(v => Compare(Value, Comparison, v));
                    }
                }
            }
            else if (Name == "class")
            {
                if (item is Proxying.IInterceptableType)
                    itemValue = (item as Proxying.IInterceptableType).GetContentType().Name;
                else
                    itemValue = item.GetType().Name;
            }
            else
                itemValue = N2.Utility.GetProperty(item, Name);
            return Compare(Value, Comparison, itemValue);
        }

        private bool Compare(object value, Comparison comparison, object itemValue)
        {
            switch (comparison)
            {
                case Persistence.Comparison.Equal:
                    if (value == null)
                        return itemValue == null;
                    if (itemValue is Details.IMultipleValue)
                    {
                        return (itemValue as Details.IMultipleValue).Equals(value);
                    }
                    return value.Equals(itemValue);
                case Persistence.Comparison.NotEqual:
                    if (value == null)
                        return itemValue != null;
                    if (itemValue is Details.IMultipleValue)
                    {
                        return !(itemValue as Details.IMultipleValue).Equals(value);
                    }
                    return !value.Equals(itemValue);
                case Persistence.Comparison.Null:
                    return itemValue == null;
                case Persistence.Comparison.NotNull:
                    return itemValue != null;
                case Persistence.Comparison.Like:
                    return CompareInvariant(value, itemValue);
                case Persistence.Comparison.NotLike:
                    return !CompareInvariant(value, itemValue);
                case Comparison.In:
					return ((IEnumerable)value).OfType<object>().Any(v => Compare(v, Persistence.Comparison.Equal, itemValue));
                case Comparison.NotIn:
                    return !Compare(value, comparison, itemValue);
                default:
					if (value == null || itemValue == null)
						return false;
                    bool? result = TryCompare(value, comparison, itemValue as IComparable);
                    if (result.HasValue)
                        return result.Value;
                    throw new NotSupportedException("Operator " + comparison + " not supported for IsMatch " + Name);
            }
        }

        private bool CompareInvariant(object parameterValue, object itemValue)
        {
            if (parameterValue == null)
                return itemValue == null;

            var value = parameterValue.ToString();
			if (value.EndsWith("%"))
			{
				if (value.StartsWith("%"))
					return FindContaining(itemValue, value.Substring(1, value.Length - 2));

				return FindStartingWith(itemValue, value.Substring(0, value.Length - 1));
			}
			else if (value.StartsWith("%"))
				return FindEndingWith(itemValue, value.Substring(1));

            return string.Equals(itemValue != null ? itemValue.ToString() : null, parameterValue != null ? parameterValue.ToString() : null, StringComparison.InvariantCultureIgnoreCase);
        }

		private static bool FindStartingWith(object itemValue, string value)
		{
			if (itemValue is Details.IMultipleValue)
				return itemValue != null && (itemValue as Details.IMultipleValue).StringValue.StartsWith(value, StringComparison.InvariantCultureIgnoreCase);

			return itemValue != null && itemValue.ToString().StartsWith(value, StringComparison.InvariantCultureIgnoreCase);
		}

		private static bool FindContaining(object itemValue, string value)
		{
			if (itemValue is Details.IMultipleValue)
				return itemValue != null && (itemValue as Details.IMultipleValue).StringValue.IndexOf(value, StringComparison.InvariantCultureIgnoreCase) >= 0;

			return itemValue != null && itemValue.ToString().IndexOf(value, StringComparison.InvariantCultureIgnoreCase) >= 0;
		}

		private static bool FindEndingWith(object itemValue, string value)
		{
			if (itemValue is Details.IMultipleValue)
				return itemValue != null && (itemValue as Details.IMultipleValue).StringValue.EndsWith(value, StringComparison.InvariantCultureIgnoreCase);

			return itemValue != null && itemValue.ToString().EndsWith(value, StringComparison.InvariantCultureIgnoreCase);
		}

        private bool? TryCompare(object value, Comparison comparison, IComparable comparable)
        {
            if (comparable == null)
                return null;

            if (comparison == Persistence.Comparison.GreaterOrEqual)
                return comparable.CompareTo(value) >= 0;
            if (comparison == Persistence.Comparison.GreaterThan)
                return comparable.CompareTo(value) > 0;
            if (comparison == Persistence.Comparison.LessOrEqual)
                return comparable.CompareTo(value) <= 0;
            if (comparison == Persistence.Comparison.LessThan)
                return comparable.CompareTo(value) < 0;

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

		//public static ParameterCollection operator &(Parameter q1, ParameterCollection q2)
		//{
		//	return new ParameterCollection(Persistence.Operator.And) { { q1 }, { q2 } };
		//}
		//public static ParameterCollection operator |(Parameter q1, ParameterCollection q2)
		//{
		//	return new ParameterCollection(Persistence.Operator.Or) { { q1 }, { q2 } };
		//}

        public static implicit operator ParameterCollection(Parameter p)
        {
            return new ParameterCollection(p);
        }

        #endregion

        #region Equals & GetHashCode
        public override bool Equals(object obj)
        {
            var other = obj as Parameter;
			return other != null
				&& other.Name == Name
				&& ValueEquals(other)
				&& other.Comparison == Comparison
				&& other.IsDetail == IsDetail;
        }

		private bool ValueEquals(Parameter other)
		{
			return (Value != null && Value.Equals(other.Value))
				|| (Value == null && other.Value == null)
				|| (Value is object[] && other.Value is object[] && (Value as object[]).SequenceEqual(other.Value as object[]));
		}

        public override int GetHashCode()
        {
			int hash = 17;
			Utility.AppendHashCode(ref hash, Name);
			if (Value is object[])
				foreach(var value in (Value as object[]))
					Utility.AppendHashCode(ref hash, value);
			else
				Utility.AppendHashCode(ref hash, Value);
			Utility.AppendHashCode(ref hash, Comparison);
			Utility.AppendHashCode(ref hash, IsDetail);
			return hash;
        }

        public override string ToString()
        {
            return (IsDetail ? "Detail." : "Property.") 
                + (Name ?? "(Any)") 
                + " " + Comparison + " " 
                + Value;
        }
        #endregion

		public ParameterCollection Join(params Parameter[] parameters)
		{
			return new ParameterCollection(parameters) { this };
		}
	}
}
