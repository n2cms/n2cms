using System;
using System.Collections.Generic;
using System.Text;
using N2.Plugin;

namespace N2.Addons.Wiki
{
    public interface ITemplateRenderer : IPlugin, IRenderer
    {
        string VirtualPath { get; }
    }
}
