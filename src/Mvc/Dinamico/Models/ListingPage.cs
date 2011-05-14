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
	[WithEditableTemplateSelection(ContainerName = Defaults.Containers.Metadata)]
	public class ListingPage : ContentPage
	{
	}
}