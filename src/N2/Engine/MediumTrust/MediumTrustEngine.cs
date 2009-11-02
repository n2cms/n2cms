using System;

namespace N2.Engine.MediumTrust
{
	public class MediumTrustEngine : ContentEngine
	{
		public MediumTrustEngine()
			: base(new MediumTrustServiceContainer())
		{
		}

		public MediumTrustEngine(EventBroker broker)
			: base(new MediumTrustServiceContainer(), broker)
		{
		}
	}
}