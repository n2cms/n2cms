namespace N2.Web
{
	public interface ITemplateReference
	{
		PathData GetTemplate(ContentItem item, string remainingUrl);
	}
}