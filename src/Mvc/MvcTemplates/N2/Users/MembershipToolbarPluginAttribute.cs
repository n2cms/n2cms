namespace N2.Edit.Membership
{
    public class MembershipToolbarPluginAttribute : ToolbarPluginAttribute
    {
        public MembershipToolbarPluginAttribute(string title, string name, string urlFormat, string iconUrl, int sortOrder)
            : base(title, name, urlFormat, ToolbarArea.Navigation, Targets.Preview, iconUrl, sortOrder)
        {
            Legacy = true;
        }

        public override System.Web.UI.Control AddTo(System.Web.UI.Control container, PluginContext context)
        {
            try
            {
                if (System.Web.Security.Membership.Providers.Count == 0) 
                    return null;
            }
            catch (System.Configuration.Provider.ProviderException)
            {
                return null;
            }
        
            return base.AddTo(container, context);
        }
    }
}
