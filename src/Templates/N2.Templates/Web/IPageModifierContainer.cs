namespace N2.Templates.Web
{
	public interface IPageModifierContainer : IPageModifier
	{
		void Add(IPageModifier modifier);
		void Remove(IPageModifier modifier);
	}
}