namespace System.Web.Mvc {
    using System.Collections.Generic;

    internal sealed class ControllerTypeCache {
        private Dictionary<string, Type> _controllerCache = new Dictionary<string, Type>();
        private bool _initialized;

        public int Count {
            get {
                return _controllerCache.Count;
            }
        }

        public bool Initialized {
            get {
                return _initialized;
            }
        }

        public void Initialize() {
            _initialized = true;
        }

        public void AddControllerType(string name, Type controllerType) {
            _controllerCache[GetNormalizedControllerName(name)] = controllerType;
        }

        public bool ContainsController(string name) {
            return _controllerCache.ContainsKey(GetNormalizedControllerName(name));
        }

        private static string GetNormalizedControllerName(string name) {
            return name.ToUpperInvariant();
        }

        public bool TryGetControllerType(string name, out Type controllerType) {
            return _controllerCache.TryGetValue(GetNormalizedControllerName(name), out controllerType);
        }
    }
}
