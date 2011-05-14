using N2.Engine;
using N2.Plugin;

namespace N2.Resources
{
	[Service]
	public class ResourceInitializer : IAutoStart
	{
		Configuration.ConfigurationManagerWrapper configFactory;

		public ResourceInitializer(Configuration.ConfigurationManagerWrapper configFactory)
		{
			this.configFactory = configFactory;
		}

		#region IAutoStart Members

		public void Start()
		{
			Register.Debug = configFactory.Sections.Web.Resources.Debug;
		}

		public void Stop()
		{
		}

		#endregion
	}
}
