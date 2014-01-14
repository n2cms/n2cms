using System;

namespace N2.Definitions
{
    /// <summary>
    /// Whether versions are allowed.
    /// </summary>
    public enum AllowVersions
    {
        Yes,
        No
    }

    /// <summary>
    /// When used to decorate a content class this attribute can tell the edit 
    /// manager not to store versions of items of that class.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class VersionableAttribute : Attribute
    {
        public VersionableAttribute(AllowVersions versionable)
        {
            Versionable = versionable;
        }

        public AllowVersions Versionable { get; private set; }
    }
}
