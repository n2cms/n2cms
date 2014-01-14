#if DEBUG
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using N2.Definitions.Runtime;
using N2.Definitions;

namespace N2.Templates.Mvc.Areas.Tests.Models
{
    // TODO: move to webform templates
    public class FluentWebFormPage : ContentItem, IContentPage
    {
        public virtual string Text { get; set; }
    }

    [Registration]
    public class FluentWebFormPageRegistrator : FluentRegisterer<FluentWebFormPage>
    {
        public override void RegisterDefinition(IContentRegistration<FluentWebFormPage> register)
        {
            register.Page();
            register.UsingConventions();
            register.Definition.SortOrder = 1000;
        }
    }
}
#endif
