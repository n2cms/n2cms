using System;

namespace N2.Definitions.Static
{
	internal interface IDescriptionRefiner
	{
		void Describe(Type entityType, Description description);
	}
}
