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
        public static bool Throw { get; set; }
        public void Initialize(N2.Engine.IEngine engine)
        {
            if (Throw)
                throw new SomeException("ThrowingPlugin1 isn't happy.");
        }
    }
}
