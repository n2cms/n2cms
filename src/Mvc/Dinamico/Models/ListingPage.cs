using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using N2;
using N2.Details;
using N2.Definitions;

namespace Dinamico.Models
{
	[PageDefinition]
	[WithEditableTitle, WithEditableName]
	public class ListingPage : ContentItem,
		IStructuralPage // this interface can be used by modules to interact with this app
	{
	}
}