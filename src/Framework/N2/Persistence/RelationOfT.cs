using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Collections;
using N2.Web;

namespace N2.Persistence
{
	public class Relation<T> : Web.IJsonWriter
	{
		public static T DefaultAccessor(object id)
		{
			if (0.Equals(id))
				return default(T);
			throw new NotSupportedException("This relation has not been initialized for retrieving values.");
		}
		
		public Relation()
		{
			ValueAccessor = DefaultAccessor;
		}

		public int? ID { get; set; }
		
		public bool HasValue 
		{ 
			get { return ID.HasValue && ID.Value != 0; } 
		}

		public T Value 
		{
			get
			{
				return (ValueAccessor != null)
					? ValueAccessor(ID ?? 0)
					: DefaultAccessor(ID ?? 0);
			}
		}
		public Func<object, T> ValueAccessor { get; set; }

		void IJsonWriter.Write(System.IO.TextWriter writer)
		{
			if (HasValue)
				writer.Write(ID);
			else
				writer.Write("null");
		}
	}

	public class ContentRelation : Relation<ContentItem>
	{
		public string Path 
		{ 
			get { return HasValue ? Value.Path : null; } 
		}
		public ContentRelation Parent 
		{ 
			get { return HasValue ? Value.Parent : null; } 
		}
		public PathData FindPath(string remainingUrl) 
		{ 
			return HasValue ? Value.FindPath(remainingUrl) : PathData.Empty; 
		}
		public IContentItemList<ContentItem> Children 
		{ 
			get { return HasValue ? Value.Children : new ItemList<ContentItem>(); } 
		}

		public static implicit operator ContentItem(ContentRelation relation)
		{
			if(relation.HasValue)
				return relation.Value;
			return null;
		}

		public static implicit operator ContentRelation(ContentItem item)
		{
			if (item == null)
				return new ContentRelation();
			return new ContentRelation { ValueAccessor = (id) => item.ID.Equals(id) ? item : DefaultAccessor(id), ID = item.ID };
		}

		public static bool operator ==(ContentRelation first, ContentRelation second)
		{
			if (first is ContentRelation)
				return first.Equals(second);
			return false;
		}

		public static bool operator !=(ContentRelation first, ContentRelation second)
		{
			if (first is ContentRelation)
				return !first.Equals(second);
			return false;
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
				return !HasValue;

			var otherRealation = obj as Relation<ContentItem>;
			if (otherRealation != null)
			{
				if (!HasValue && !otherRealation.HasValue)
					return true;
				if (HasValue && otherRealation.HasValue)
					return Value == otherRealation.Value;
				return false;
			}
			var otherValue = obj as ContentItem;
			if (otherValue != null)
			{
				if (HasValue)
					return Value == otherValue;
				return false;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}

	public class ParentRelation : ContentRelation
	{
	}
}
