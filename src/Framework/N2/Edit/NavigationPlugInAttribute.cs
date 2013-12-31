using System;

namespace N2.Edit
{
    /// <summary>
    /// An attribute defining a right-click item in the navigation pane.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Assembly, AllowMultiple = true)]
    public abstract class NavigationPluginAttribute : LinkPluginAttribute
    {
    }
}
