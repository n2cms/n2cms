using N2.Details;
using N2.Web.Mvc;

namespace N2.Definitions.Runtime
{
	public interface IContentRegistration
	{
		EditableBuilder<T> RegisterEditable<T>(string name, string title) where T : IEditable, new();
		EditableBuilder<T> RegisterEditable<T>(T editable) where T : IEditable;
		void RegisterModifier(IContentTransformer modifier);
		Builder<T> RegisterRefiner<T>(T refiner) where T : ISortableRefiner;
	}
}
