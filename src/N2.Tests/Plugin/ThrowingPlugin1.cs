using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Plugin;

namespace N2.Tests.Plugin
{
    [AutoInitialize]
    public class ThrowingPlugin1 : IPluginInitializer
    {
        public void Initialize(N2.Engine.IEngine engine)
        {
            throw new SomeException("ThrowingPlugin1 isn't happy.");
        }
    }
}
