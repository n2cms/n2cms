using System;

namespace N2.Engine
{
    /// <summary>
    /// Registers an adapter to be used with a content item of the specified type
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class AdaptsAttribute : Attribute
    {
        public AdaptsAttribute(Type contentType)
        {
            ContentType = contentType;
        }

        /// <summary>The type of content item to adapt.</summary>
        public Type ContentType { get; private set; }
    }
}
