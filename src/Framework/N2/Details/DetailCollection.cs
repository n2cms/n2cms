#region License
/* Copyright (C) 2007 Cristian Libardo
 *
 * This is free software; you can redistribute it and/or modify it
 * under the terms of the GNU Lesser General Public License as
 * published by the Free Software Foundation; either version 2.1 of
 * the License, or (at your option) any later version.
 *
 * This software is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this software; if not, write to the Free
 * Software Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA
 * 02110-1301 USA, or see the FSF site: http://www.fsf.org.
 */
#endregion

using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using N2.Collections;
using System.Diagnostics;

namespace N2.Details
{
    /// <summary>A named collection of details. This is used by content items to group related details together.</summary>
    [DebuggerDisplay("DetailCollection, {name}, #{id}: Count = {Details.Count}")]
    public class DetailCollection : IList, ICloneable, INameable, IEnumerable<object>
    {
        #region Constructors
        /// <summary>Creates a new (uninitialized) instance of the DetailCollection class.</summary>
        public DetailCollection()
        {
        }
        /// <summary>Crates a new instance of the DetailCollection bound to a content item.</summary>
        /// <param name="item">The content item enclosing this collection.</param>
        /// <param name="name">The name of the collection.</param>
        /// <param name="values">The values of this collection.</param>
        public DetailCollection(ContentItem item, string name, params object[] values)
        {
            this.EnclosingItem = item;
            this.Name = name;
            foreach (object value in values)
            {
                this.Add(value);
            }
        }

        private ContentDetail GetDetail(object val)
        {
            ContentDetail detail;
            if (val is ContentDetail)
                detail = (ContentDetail)val;
            else
                detail = ContentDetail.New(EnclosingItem, null, val);
            if(detail.Name == null || !detail.Name.StartsWith(this.Name))
                detail.Name = this.Name;
            detail.EnclosingItem = this.EnclosingItem;
            detail.EnclosingCollection = this;
            return detail;
        }
        #endregion

        #region Private Fields
        private int id;
        private ContentItem enclosingItem;
        private string name;
        private IList<ContentDetail> details = new List<ContentDetail>(); 
        #endregion

        #region Properties
        /// <summary>Gets or sets the collection's primary key.</summary>
        public virtual int ID
        {
            get { return id; }
            set { id = value; }
        }

        /// <summary>Gets or sets the name of the collection.</summary>
        public virtual string Name
        {
            get { return name; }
            set { name = value; }
        }

        /// <summary>Gets or sets the details collection. To access the objects directly you can use e.g. collection[index].</summary>
        public virtual IList<ContentDetail> Details
        {
            get { return details; }
            set { details = value; }
        } 

        /// <summary>Gets or sets the the item containing this collection.</summary>
		[System.Xml.Serialization.XmlIgnore]
		public virtual N2.ContentItem EnclosingItem
        {
            get { return enclosingItem; }
            set { enclosingItem = value; }
        }
        #endregion

        #region Methods
        /// <summary>Adds the elements of the specified collection to the end of this collection.</summary>
        /// <param name="values">The values to add.</param>
        public virtual void AddRange(IEnumerable values)
        {
            foreach (object value in values)
                this.Add(value);
        }

        /// <summary>Replaces the values in this collection adding those not present in this collection and removing those not present in the supplied collection. This method will not add duplicate elements.</summary>
        /// <param name="values">The values to replace the this collection with.</param>
        public virtual void Replace(IEnumerable values)
        {
            bool[] valuesToKeep = new bool[this.Count];
            var valuesToAdd = new List<object>();
            // Add new items and mark items that should be kept.
            foreach (object value in values)
            {
                int i = this.IndexOf(value);
                if (i < 0)
                    valuesToAdd.Add(value);
                else
                    valuesToKeep[i] = true;
            }

            // Remove items that are not present in the supplied collection
            for (int i = valuesToKeep.Length - 1; i >= 0; i--)
            {
                if (!valuesToKeep[i])
                    this.RemoveAt(i);
            }

            foreach (var value in valuesToAdd)
                this.Add(value);
        }

        public virtual void Replace(IDictionary<string, object> dictionary)
        {
            var kvps = dictionary.ToList();
            // replace existing details
            for (int i = 0; i < Details.Count && i < kvps.Count; i++)
            {
                Details[i].Meta = kvps[i].Key;
                Details[i].Value = kvps[i].Value;
            }
            // add extra values
            for (int i = Details.Count; i < kvps.Count; i++)
            {
                Add(new ContentDetail(EnclosingItem, Name, kvps[i].Value) { Meta = kvps[i].Key });
            }
            // remove superflous
            for (int i = Details.Count - 1; i >= kvps.Count; i--)
            {
                RemoveAt(i);
            }
        }
        #endregion

        #region IList Members

        /// <summary>Gets the index of an object in the collection..</summary>
        /// <param name="value">The value whose index to get.</param>
        /// <returns>The index or -1 if the item isn't in the collection.</returns>
        public virtual int IndexOf(object value)
        {
            if (value is ContentDetail)
                return Details.IndexOf(value as ContentDetail);

            for (int i = 0; i < Details.Count; i++)
                if (Details[i].Equals(value) || (Details[i].Value != null && Details[i].Value.Equals(value)))
                    return i;
            return -1;
        }

        /// <summary>Inserts a value in the collection.</summary>
        /// <param name="index">The index to insert into.</param>
        /// <param name="value">The value to insert.</param>
        public virtual void Insert(int index, object value)
        {
            Untemporarize();
            ContentDetail detail = GetDetail(value);
            Details.Insert(index, detail);
        }

        /// <summary>Removes a value at the given index.</summary>
        /// <param name="index">The index of the value to remove.</param>
        public virtual void RemoveAt(int index)
        {
            Details.RemoveAt(index);
        }

        /// <summary>Gets or sets a value at the specified index.</summary>
        /// <param name="index">The index of the value.</param>
        /// <returns>The value get or set from the specified index.</returns>
        public virtual object this[int index]
        {
            get { return Details[index].Value; }
            set 
            {
                Untemporarize();
                Details[index] = GetDetail(value); 
            }
        }

        /// <summary>Gets false.</summary>
        public virtual bool IsFixedSize
        {
            get { return false; }
        }

        #endregion

        #region ICollection Members

        /// <summary>Adds a value to the collection.</summary>
        /// <param name="value">The value to add.</param>
        /// <returns>the index of the added value.</returns>
        public virtual int Add(object value)
        {
            Untemporarize();
            ContentDetail detail = GetDetail(value);
            Details.Add(detail);
            return Details.Count - 1;
        }

        private void Untemporarize()
        {
            if (Temporary)
            {
                Temporary = false;
                AddTo(EnclosingItem);
            }
        }

        /// <summary>Clears the collection.</summary>
        public virtual void Clear()
        {
            Details.Clear();
        }

        /// <summary>Check if the collection contains a value.</summary>
        /// <param name="value">The value to look for.</param>
        /// <returns>True if the collection contains the value.</returns>
        public virtual bool Contains(object value)
        {
            if (value == null)
                return false;

            if (value is ContentDetail)
            {
                foreach (ContentDetail detail in this.Details)
                    if (detail.Equals(value))
                        return true;
            }
            else
            {
                foreach (ContentDetail detail in this.Details)
                    if (value.Equals(detail.Value))
                        return true;
            }
            return false;
        }

        /// <summary>Copies the collection to an array.</summary>
        /// <param name="array">The array to copy values to.</param>
        /// <param name="index">The start index to copy from.</param>
        public virtual void CopyTo(Array array, int index)
        {
            for (int i = index; i < array.Length; i++)
                array.SetValue(Details[i].Value, i);
        }

        /// <summary>Gets the number of values in the collection.</summary>
        public virtual int Count
        {
            get { return Details.Count; }
        }

        /// <summary>Gets false.</summary>
        public virtual bool IsReadOnly
        {
            get { return Details.IsReadOnly; }
        }

        /// <summary>Removes a value from the collection.</summary>
        /// <param name="value">The value to remove.</param>
        public virtual void Remove(object value)
        {
            int index = IndexOf(value);
            if (index >= 0)
                RemoveAt(index);
        }

        /// <summary>Gets true.</summary>
        public virtual bool IsSynchronized
        {
            get { return true; }
        }

        /// <summary>Gets null.</summary>
        public virtual object SyncRoot
        {
            get { return null; }
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return new DetailCollectionEnumerator(this);
        }

        #endregion

        #region ICloneable Members

        /// <summary>Clones the collection and </summary>
        /// <returns></returns>
        public virtual DetailCollection Clone()
        {
            DetailCollection collection = new DetailCollection();
            collection.ID = 0;
            collection.Name = this.Name;
            collection.EnclosingItem = this.EnclosingItem;
            foreach (ContentDetail detail in this.Details)
            {
                ContentDetail cloned = detail.Clone();
                cloned.EnclosingCollection = collection;
                collection.Add(cloned);
            }
            return collection;
        }

        object ICloneable.Clone()
        {
            return Clone();
        }

        #endregion

        #region DetailCollectionEnumerator
        private class DetailCollectionEnumerator : IEnumerator
        {
            DetailCollection collection;

            public DetailCollectionEnumerator(DetailCollection collection)
            {
                this.collection = collection;
            }

            #region IEnumerator Members
            int enumeratorIndex = -1;
            public object Current
            {
                get { return collection[enumeratorIndex]; }
            }

            public bool MoveNext()
            {
                return ++enumeratorIndex < collection.Count;
            }

            public virtual void Reset()
            {
                enumeratorIndex = -1;
            }
            #endregion
        } 
        #endregion

        public virtual IList<T> ToList<T>()
        {
            List<T> list = new List<T>();
            foreach (ContentDetail cd in this.Details)
            {
                list.Add((T)cd.Value);
            }
            return list;
        }

        public virtual IEnumerable<T> Enumerate<T>()
        {
            foreach (ContentDetail cd in this.Details)
            {
                yield return (T)cd.Value;
            }
        }

        public virtual T[] ToArray<T>()
        {
            T[] list = new T[Details.Count];
            for (int i = 0; i < list.Length; i++)
            {
                list[i] = (T)Details[i].Value;
            }
            return list;
        }

        #region ToString, Equals & GetHashCode
        int? hashCode;
        /// <summary>Gets a hash code based on the ID.</summary>
        /// <returns>A hash code.</returns>
        public override int GetHashCode()
        {
            if (!hashCode.HasValue)
                hashCode = (id > 0 ? id.GetHashCode() : base.GetHashCode());
            return hashCode.Value;
        }
        public override bool Equals(object obj)
        {
            if (this == obj) return true;
            DetailCollection other = obj as DetailCollection;
            return other != null && id != 0 && id == other.id;
        }
        #endregion

        #region IEnumerable<object> Members

        public virtual IEnumerator<object> GetEnumerator()
        {
            return this.Details.Select(d => d.Value).GetEnumerator();
        }

        #endregion

        /// <summary>
        /// Gets the detail values of the given generic type.
        /// </summary>
        /// <typeparam name="T">The type of value to retrieve.</typeparam>
        /// <returns>An enumeration of values with matching type.</returns>
        public virtual IEnumerable<T> OfType<T>()
        {
            foreach (ContentDetail cd in this.Details)
            {
                if (cd.Value is T)
                    yield return (T)cd.Value;
            }
        }

        internal virtual void AddTo(ContentItem destination)
        {
            destination.DetailCollections[Name] = this;
            EnclosingItem = destination;
            foreach (var detail in Details)
                detail.EnclosingItem = destination;
        }

        public IDictionary<string, object> AsDictionary()
        {
            var map = new Dictionary<string, object>();
            foreach (var d in Details)
                if (d.Meta != null)
                    map[d.Meta] = d.Value;
            return map;
        }

        public bool Temporary { get; set; }
    }
}

