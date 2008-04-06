namespace System.Web.Mvc {
    using System;

    [AttributeUsage(AttributeTargets.Method)]
    [AspNetHostingPermission(System.Security.Permissions.SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    public sealed class NonActionAttribute : Attribute {
    }
}
