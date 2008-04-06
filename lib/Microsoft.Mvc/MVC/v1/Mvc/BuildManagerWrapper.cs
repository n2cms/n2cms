namespace System.Web.Mvc {
    using System.Collections;
    using System.Web.Compilation;

    internal class BuildManagerWrapper : IBuildManager {
        #region IBuildManager Members
        object IBuildManager.CreateInstanceFromVirtualPath(string virtualPath, Type requiredBaseType) {
            return BuildManager.CreateInstanceFromVirtualPath(virtualPath, requiredBaseType);
        }

        ICollection IBuildManager.GetReferencedAssemblies() {
            return BuildManager.GetReferencedAssemblies();
        }
        #endregion
    }
}
