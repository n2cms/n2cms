using System;
using N2.Web.UI;
using N2.Templates.Services;

namespace N2.Templates.Web.UI
{
    /// <summary>
    /// Base class for UI views in the functional templates project. Pages 
    /// inheriting from this class will be associated with certain behaviours 
    /// such as injected master page and theme.
    /// </summary>
    public class TemplatePage : TemplatePage<ContentItem>, ITemplatePage
    {
    }

    /// <summary>
    /// Base class for UI views in the functional templates project. Pages 
    /// inheriting from this class will be associated with certain behaviours 
    /// such as injected master page and theme.
    /// </summary>
    /// <typeparam name="TPage">Bind this template to strongly typed content class. This facilitates accessing class properties.</typeparam>
    public class TemplatePage<TPage> : ContentPage<TPage>, ITemplatePage
        where TPage: ContentItem
    {
        public override string ID
        {
            get { return base.ID ?? "P"; }
        }
    }
}
