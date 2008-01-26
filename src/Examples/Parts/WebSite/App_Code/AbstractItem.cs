using N2;

public abstract class AbstractItem : ContentItem
{
	public override bool IsPage
	{
		get { return false; }
	}

	public override string TemplateUrl
	{
		get 
		{
			// expect the view to have the same name as the class
			return "~/Parts/" + GetType().Name + ".ascx";
		}
	}
}
