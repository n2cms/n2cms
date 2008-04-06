namespace System.Web.Mvc {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Web;
    using System.Web.Util;

    // REVIEW: Should this type be serializable? We store it in session so it might be a good idea.

    [AspNetHostingPermission(System.Security.Permissions.SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    [AspNetHostingPermission(System.Security.Permissions.SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    public class TempDataDictionary : IDictionary<string, object> {
        internal static string TempDataSessionStateKey = "__ControllerTempData";

        // The First is the real TempData storage dictionary
        // The Second is the list of tracked keys: the keys that have been modified during this request,
        // and thus must survive to the next request (but not beyond).
        private Pair<Dictionary<string, object>, HashSet<string>> _sessionData;

        public HttpContextBase HttpContext {
            get;
            private set;
        }

        public TempDataDictionary(HttpContextBase httpContext) {
            if (httpContext == null) {
                throw new ArgumentNullException("httpContext");
            }

            HttpContext = httpContext;

            EnsureReadData();
        }

        private void EnsureReadData() {
            if (_sessionData != null) {
                // If we already got the TempData, do nothing
                return;
            }

            // Try to retrieve the TempData from Session
            if (HttpContext.Session != null) {
                _sessionData = HttpContext.Session[TempDataSessionStateKey] as Pair<Dictionary<string, object>, HashSet<string>>;
            }
            if (_sessionData != null) {
                // If we got it from Session, remove it so that no other request gets it
                HttpContext.Session.Remove(TempDataSessionStateKey);

                // Clear out any items that weren't being tracked (they were from two requests ago, and thus invalid)
                HashSet<string> untrackedKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                foreach (string key in _sessionData.First.Keys) {
                    if (!_sessionData.Second.Contains(key)) {
                        untrackedKeys.Add(key);
                    }
                }
                foreach (string untrackedKey in untrackedKeys) {
                    _sessionData.First.Remove(untrackedKey);
                }
                _sessionData.Second.Clear();
            }
            else {
                // If there wasn't anything in Session, create a new TempData
                _sessionData = new Pair<Dictionary<string, object>, HashSet<string>>(
                    new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase),
                    new HashSet<string>(StringComparer.OrdinalIgnoreCase));
            }
        }

        private void EnsureWriteData(string key) {
            // If someone wants to write to TempData, we have to add it to Session
            HttpSessionStateBase session = HttpContext.Session;
            if (session != null) {
               session[TempDataSessionStateKey] = _sessionData;
            }

            // And track all keys that have been modified (added, removed, overwritten)
            if (key == null) {
                // If the user is clearing all the keys, then we have nothing to track
                _sessionData.Second.Clear();
            }
            else {
                // If they're just modifying a single key, add it to the list (if it isn't already there)
                if (!_sessionData.Second.Contains(key)) {
                    _sessionData.Second.Add(key);
                }
            }
        }

        public int Count {
            [SuppressMessage("Microsoft.Security", "CA2123:OverrideLinkDemandsShouldBeIdenticalToBase")]
            get {
                return _sessionData.First.Count;
            }
        }

        public Dictionary<string, object>.KeyCollection Keys {
            get {
                return _sessionData.First.Keys;
            }
        }

        public Dictionary<string, object>.ValueCollection Values {
            get {
                return _sessionData.First.Values;
            }
        }

        public object this[string key] {
            [SuppressMessage("Microsoft.Security", "CA2123:OverrideLinkDemandsShouldBeIdenticalToBase")]
            get {
                object value;
                if (_sessionData.First.TryGetValue(key, out value)) {
                    return value;
                }
                return null;
            }
            [SuppressMessage("Microsoft.Security", "CA2123:OverrideLinkDemandsShouldBeIdenticalToBase")]
            set {
                EnsureWriteData(key);
                _sessionData.First[key] = value;
            }
        }

        [SuppressMessage("Microsoft.Security", "CA2123:OverrideLinkDemandsShouldBeIdenticalToBase")]
        public void Add(string key, object value) {
            EnsureWriteData(key);
            _sessionData.First.Add(key, value);
        }

        [SuppressMessage("Microsoft.Security", "CA2123:OverrideLinkDemandsShouldBeIdenticalToBase")]
        public void Clear() {
            EnsureWriteData(null);
            _sessionData.First.Clear();
        }

        [SuppressMessage("Microsoft.Security", "CA2123:OverrideLinkDemandsShouldBeIdenticalToBase")]
        public bool ContainsKey(string key) {
            return _sessionData.First.ContainsKey(key);
        }

        public bool ContainsValue(object value) {
            return _sessionData.First.ContainsValue(value);
        }

        public Dictionary<string, object>.Enumerator GetEnumerator() {
            return _sessionData.First.GetEnumerator();
        }

        [SuppressMessage("Microsoft.Security", "CA2123:OverrideLinkDemandsShouldBeIdenticalToBase")]
        public bool Remove(string key) {
            EnsureWriteData(key);
            return _sessionData.First.Remove(key);
        }

        [SuppressMessage("Microsoft.Security", "CA2123:OverrideLinkDemandsShouldBeIdenticalToBase")]
        public bool TryGetValue(string key, out object value) {
            return _sessionData.First.TryGetValue(key, out value);
        }

        #region IDictionary<string, object> Implementation
        ICollection<string> IDictionary<string, object>.Keys {
            [SuppressMessage("Microsoft.Security", "CA2123:OverrideLinkDemandsShouldBeIdenticalToBase")]
            get {
                return ((IDictionary<string, object>)_sessionData.First).Keys;
            }
        }

        ICollection<object> IDictionary<string, object>.Values {
            [SuppressMessage("Microsoft.Security", "CA2123:OverrideLinkDemandsShouldBeIdenticalToBase")]
            get {
                return ((IDictionary<string, object>)_sessionData.First).Values;
            }
        }
        #endregion

        #region IEnumerable<KeyValuePair<string, object>> Implementation
        [SuppressMessage("Microsoft.Security", "CA2123:OverrideLinkDemandsShouldBeIdenticalToBase")]
        IEnumerator<KeyValuePair<string, object>> IEnumerable<KeyValuePair<string, object>>.GetEnumerator() {
            return ((IEnumerable<KeyValuePair<string, object>>)_sessionData.First).GetEnumerator();
        }
        #endregion

        #region ICollection<KeyValuePair<string, object>> Implementation
        bool ICollection<KeyValuePair<string, object>>.IsReadOnly {
            [SuppressMessage("Microsoft.Security", "CA2123:OverrideLinkDemandsShouldBeIdenticalToBase")]
            get {
                return ((ICollection<KeyValuePair<string, object>>)_sessionData.First).IsReadOnly;
            }
        }

        [SuppressMessage("Microsoft.Security", "CA2123:OverrideLinkDemandsShouldBeIdenticalToBase")]
        void ICollection<KeyValuePair<string, object>>.CopyTo(KeyValuePair<string, object>[] array, int index) {
            ((ICollection<KeyValuePair<string, object>>)_sessionData.First).CopyTo(array, index);
        }

        [SuppressMessage("Microsoft.Security", "CA2123:OverrideLinkDemandsShouldBeIdenticalToBase")]
        void ICollection<KeyValuePair<string, object>>.Add(KeyValuePair<string, object> keyValuePair) {
            EnsureWriteData(keyValuePair.Key);
            ((ICollection<KeyValuePair<string, object>>)_sessionData.First).Add(keyValuePair);
        }

        [SuppressMessage("Microsoft.Security", "CA2123:OverrideLinkDemandsShouldBeIdenticalToBase")]
        bool ICollection<KeyValuePair<string, object>>.Contains(KeyValuePair<string, object> keyValuePair) {
            return ((ICollection<KeyValuePair<string, object>>)_sessionData.First).Contains(keyValuePair);
        }

        [SuppressMessage("Microsoft.Security", "CA2123:OverrideLinkDemandsShouldBeIdenticalToBase")]
        bool ICollection<KeyValuePair<string, object>>.Remove(KeyValuePair<string, object> keyValuePair) {
            EnsureWriteData(keyValuePair.Key);
            return ((ICollection<KeyValuePair<string, object>>)_sessionData.First).Remove(keyValuePair);
        }
        #endregion

        #region IEnumerable Implementation
        [SuppressMessage("Microsoft.Security", "CA2123:OverrideLinkDemandsShouldBeIdenticalToBase")]
        IEnumerator IEnumerable.GetEnumerator() {
            return ((IEnumerable)_sessionData.First).GetEnumerator();
        }
        #endregion
    }
}
