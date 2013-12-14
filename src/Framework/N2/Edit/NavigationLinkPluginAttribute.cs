namespace N2.Edit
{
    public class NavigationLinkPluginAttribute : NavigationPluginAttribute
    {
        public NavigationLinkPluginAttribute()
        {
        }

        public NavigationLinkPluginAttribute(string title, string name, string urlFormat)
        {
            Title = title;
            Name = name;
            UrlFormat = urlFormat;
        }
        public NavigationLinkPluginAttribute(string title, string name, string urlFormat, string target, string iconUrl, int sortOrder)
            : this(title, name, urlFormat)
        {
            Target = target;
            IconUrl = iconUrl;
            SortOrder = sortOrder;
        } 
    }
}
