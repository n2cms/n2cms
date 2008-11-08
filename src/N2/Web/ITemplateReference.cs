namespace N2.Web
{
	public interface ITemplateReference
	{
		TemplateData GetTemplate(ContentItem item, string remainingUrl);
	}
}