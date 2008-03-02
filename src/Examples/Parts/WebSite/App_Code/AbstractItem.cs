using N2;

/// <summary>
/// This is the base class for the parts in our soulution. This is a good place 
/// to define and override stuff we need in all our parts.
/// </summary>
public abstract class AbstractItem : ContentItem
{
    /// <summary>
    /// Since this is the base class for our parts we can make override the 
    /// IsPage property here and have it return false (which is required for 
    /// them to work well).
    /// </summary>
	public override bool IsPage
	{
		get { return false; }
	}

    /// <summary>
    /// This is a way to have the computer generate a resonable template url.
    /// </summary>
	public override string TemplateUrl
	{
		get 
		{
			// expect the view to have the same name as the class
			return "~/Parts/" + GetType().Name + ".ascx";
		}
	}
}
