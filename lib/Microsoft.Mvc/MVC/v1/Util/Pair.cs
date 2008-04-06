//------------------------------------------------------------------------------
// <copyright file="Pair.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
 
namespace System.Web.Util {
    using System;

    // Generic Pair class.  Overrides Equals() and GetHashCode(), so it can be used as a dictionary key.
    internal sealed class Pair<TFirst, TSecond> {
        private readonly TFirst _first;
        private readonly TSecond _second;

        public Pair(TFirst first, TSecond second) {
            _first = first;
            _second = second;
        }

        public TFirst First {
            get {
                return _first;
            }
        }

        public TSecond Second {
            get {
                return _second;
            }
        }

        public override bool Equals(object obj) {
            if (obj == this) {
                return true;
            }

            Pair<TFirst, TSecond> other = obj as Pair<TFirst, TSecond>;
            return (other != null) &&
                (((other._first == null) && (_first == null)) ||
                    ((other._first != null) && other._first.Equals(_first))) &&
                (((other._second == null) && (_second == null)) ||
                    ((other._second != null) && other._second.Equals(_second)));
        }

        public override int GetHashCode() {
            int a = (_first == null) ? 0 : _first.GetHashCode();
            int b = (_second == null) ? 0 : _second.GetHashCode();
            return CombineHashCodes(a, b);
        }

        // Copied from ndp\fx\src\xsp\System\Web\Util\HashCodeCombiner.cs
        private static int CombineHashCodes(int h1, int h2) {
            return ((h1 << 5) + h1) ^ h2;
        }
    }
}
