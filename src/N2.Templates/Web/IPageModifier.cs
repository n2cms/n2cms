namespace N2.Templates.Web
{
	public interface IPageModifier
	{
		void Modify<T>(UI.TemplatePage<T> page)
			where T : Items.AbstractPage;
	}
}