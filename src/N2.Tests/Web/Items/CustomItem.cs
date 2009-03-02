using N2.Web;
namespace N2.Tests.Web.Items
{
	public class CustomItem : ContentItem
	{
	}
	public class ParticularCustomItem : CustomItem
	{
	}
	public class SpecialCustomItem : ParticularCustomItem
	{
	}
	public class OtherCustomItem : ContentItem
	{
	}

	[Controls(typeof(CustomItem))]
	public class CustomController : RequestController
	{
	}

	[Controls(typeof(SpecialCustomItem))]
	public class SpecialCustomController : RequestController
	{
	}

	[Controls(typeof(OtherCustomItem))]
	public class OtherCustomController : RequestController
	{
	}
}
