using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Definitions
{
	/// <summary>
	/// Implemented by content items and used to specify type and template to use for a specific item.
	/// </summary>
	public interface ITemplateable
	{
		string TemplateKey { get; }
		Type GetContentType();
	}
}
