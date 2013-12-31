/*
 * Efficient hashtable implementations for 32-bit and 64-bit integer types. 
 * 
 * Discussion: 
 * http://stackoverflow.com/questions/3798178/implementing-a-sparse-array-in-c-sharp-fastest-way-to-map-integer-to-a-specifi
 * 
 * Original source:
 * http://pastebin.com/Aq2L7NxQ
 * 
 */

using System;

namespace N2.Collections
{
    /// <summary>
    /// Efficient implementation of a hashtable for 64-bit integer keys.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class NumericHashtable64<T>
    {
        private class element
        {
            public ulong _key;
            public T _value;
        };

        private element[][] _buckets;
        private uint _capacity;

        public NumericHashtable64()
        {
            _capacity = 214373; // some prime number
            _buckets = new element[_capacity][];
        }

        public NumericHashtable64(uint capacity)
        {
            _capacity = capacity;
            _buckets = new element[_capacity][];
        }

        public uint hash(ulong key)
        {
            return (uint) (key%_capacity);
        }

        public void Add(ulong key, T value)
        {
            uint hsh = hash(key);
            element[] e;
            if (_buckets[hsh] == null)
                _buckets[hsh] = e = new element[1];
            else
            {
                foreach (var elem in _buckets[hsh])
                    if (elem._key == key)
                    {
                        elem._value = value;
                        return;
                    }
                e = new element[_buckets[hsh].Length + 1];
                Array.Copy(_buckets[hsh], 0, e, 1, _buckets[hsh].Length);
                _buckets[hsh] = e;
            }
            e[0] = new element {_key = key, _value = value};
        }

        public T Get(ulong key)
        {
            uint hsh = hash(key);
            element[] e = _buckets[hsh];
            if (e == null) return default(T);
            foreach (var f in e)
                if (f._key == key)
                    return f._value;
            return default(T);
        }

        public bool Has(ulong key)
        {
            uint hsh = hash(key);
            element[] e = _buckets[hsh];
            if (e == null) return false;
            foreach (var f in e)
                if (f._key == key)
                    return true;
            return false;
        }

        public int Count()
        {
            int r = 0;
            foreach (var e in _buckets)
                if (e != null)
                    r += e.Length;
            return r;
        }
    }

    /// <summary>
    /// Efficient implementation of a hashtable for 32-bit integer keys.
    /// </summary>
    public class NumericHashtable32
    {
        // ReSharper disable InconsistentNaming
        private class element
        {
            public uint _key;
            public int _value;
        };
        // ReSharper restore InconsistentNaming

        private readonly element[][] _buckets;
        private readonly uint _capacity;

        public NumericHashtable32()
        {
            _capacity = 463; // some prime number
            _buckets = new element[_capacity][];
        }

        public NumericHashtable32(uint capacity)
        {
            _capacity = capacity;
            _buckets = new element[_capacity][];
        }

        public uint hash(uint key)
        {
            return (uint) (key%_capacity);
        }

        public void Add(uint key, int value)
        {
            uint hsh = hash(key);
            element[] e;
            if (_buckets[hsh] == null)
                _buckets[hsh] = e = new element[1];
            else
            {
                foreach (var elem in _buckets[hsh])
                    if (elem._key == key)
                    {
                        elem._value = value;
                        return;
                    }
                e = new element[_buckets[hsh].Length + 1];
                Array.Copy(_buckets[hsh], 0, e, 1, _buckets[hsh].Length);
                _buckets[hsh] = e;
            }
            e[0] = new element {_key = key, _value = value};
        }

        public void Inc(uint key)
        {
            uint hsh = hash(key);
            if (_buckets[hsh] == null)
            {
                _buckets[hsh] = new element[1] {new element {_key = key, _value = 1}};
                return;
            }

            foreach (var elem in _buckets[hsh])
                if (elem._key == key)
                {
                    elem._value++;
                    return;
                }

            var e = new element[_buckets[hsh].Length + 1];
            Array.Copy(_buckets[hsh], 0, e, 1, _buckets[hsh].Length);
            _buckets[hsh] = e;
            e[0] = new element {_key = key, _value = 1};
        }

        public int Get(uint key)
        {
            uint hsh = hash(key);
            element[] e = _buckets[hsh];
            if (e == null) return 0;
            foreach (var f in e)
                if (f._key == key)
                    return f._value;
            return 0;
        }

        public int Count()
        {
            int r = 0;
            foreach (var e in _buckets)
                if (e != null)
                    r += e.Length;
            return r;
        }

        public uint Max()
        {
            uint maxKey = 0;
            int maxValue = 0;
            for (int i = 0; i < _buckets.Length; i++)
                if (_buckets[i] != null)
                    for (int j = 0; j < _buckets[i].Length; j++)
                        if (_buckets[i][j]._value > maxValue)
                        {
                            maxValue = _buckets[i][j]._value;
                            maxKey = _buckets[i][j]._key;
                        }
            return maxKey;
        }
    }
}
