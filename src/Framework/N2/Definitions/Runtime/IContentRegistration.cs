using System;
using N2.Web.Mvc;
using N2.Definitions;
using N2.Details;

namespace N2.Definitions.Runtime
{
	public interface IContentRegistration
	{
		EditableBuilder<T> RegisterEditable<T>(string name, string title) where T : IEditable, new();
		EditableBuilder<T> RegisterEditable<T>(T editable) where T : IEditable;
		void RegisterModifier(IContentTransformer modifier);
	}
}
