using System;
using N2.Web;

namespace N2.Engine.MediumTrust
{
    /// <summary>
    /// Uses a lightweight container for resolving services in a medium trust environment.
    /// </summary>
    /// <remarks>
    /// Since N2 2.0 this is not used since .NET 3.5 and changes to the Windsor container allows usage in medium trust.
    /// </remarks>
    [Obsolete("Create ContentEngine passing it MediumTrustServiceContainer", true)]
    public class MediumTrustEngine : ContentEngine
    {
        public MediumTrustEngine()
            : base(new MediumTrustServiceContainer(), EventBroker.Instance, new ContainerConfigurer())
        {
        }
    }
}
