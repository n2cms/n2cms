using N2.Engine;

namespace N2.Web
{
    /// <summary>
    /// Provides access to shared objects stored in the request context.
    /// </summary>
    /// <typeparam name="T">The type of object to get. This class only holds one instance of such an object for each request.</typeparam>
    public class RequestItem<T> : RequestItem where T : class
    {
        /// <summary>
        /// The instance of the object of the type as provided by <see cref="RequestItem.Accessor"/>.
        /// </summary>
        public static T Instance
        {
            get { return Accessor.Get(typeof(T)) as T; }
            set { Accessor.Set(typeof(T), value); }
        }
    }

    /// <summary>
    /// Provides global access to the request item accessor. This can 
    /// be exchanged for a fake for testing.
    /// </summary>
    public class RequestItem
    {
        static RequestItem()
        {
            Accessor = new HttpRequestContextAccessor();
        }

        /// <summary>
        /// Wraps access to the http context's item store.
        /// </summary>
        public static IRequestContextAccessor Accessor
        {
            get { return Singleton<IRequestContextAccessor>.Instance; }
            set { Singleton<IRequestContextAccessor>.Instance = value; }
        }
    }
}
