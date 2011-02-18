using System;
using N2.Web.Mvc;
using N2.Definitions;

namespace N2.Definitions.Runtime
{
	public interface IContentRegistration
	{
		EditableBuilder<T> RegisterEditable<T>(string name, string title) where T : IEditable, new();
	}
}
