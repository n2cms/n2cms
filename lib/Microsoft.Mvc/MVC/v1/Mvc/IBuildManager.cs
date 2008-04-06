namespace System.Web.Mvc {
    using System.Collections;

    // REVIEW: Should we make this public?
    internal interface IBuildManager {
        object CreateInstanceFromVirtualPath(string virtualPath, Type requiredBaseType);
        ICollection GetReferencedAssemblies();
    }
}
