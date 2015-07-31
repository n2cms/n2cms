using N2.Details;

namespace N2.Definitions.Runtime
{
	public interface IContentRegistration<TModel> : IContentRegistration, IDefinitionRegistration
    {
        PropertyRegistration<TModel, TProperty> On<TProperty>(string detailName);
        ContainerBuilder<TModel, T> Register<T>(T named) where T : IEditableContainer;
    }

    public interface IDefinitionRegistration
    {
        ItemDefinition Definition { get; }
        N2.Definitions.Runtime.ContentRegistration.ContentRegistrationContext Context { get; }
        RegistrationConventions DefaultConventions { get; }
    }

    public interface IContentRegistration
    {
        Builder<T> RegisterDisplayable<T>(string name) where T : class, IDisplayable, new();
        Builder<T> RegisterDisplayable<T>(T displayable) where T : IDisplayable;
        EditableBuilder<T> RegisterEditable<T>(string name, string title) where T : class, IEditable, new();
        EditableBuilder<T> RegisterEditable<T>(T editable) where T : IEditable;
        void RegisterModifier(IContentTransformer modifier);
        Builder<T> RegisterRefiner<T>(T refiner) where T : ISortableRefiner;
    }
}
