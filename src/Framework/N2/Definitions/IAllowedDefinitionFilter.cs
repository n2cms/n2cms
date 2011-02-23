using System;
using System.Collections.Generic;
using N2.Definitions;

namespace N2.Definitions
{
	public interface IAllowedDefinitionFilter
	{
		AllowedDefinitionResult IsAllowed(AllowedDefinitionQuery query);
	}
}
