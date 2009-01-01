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
	public class CustomController : BaseController
	{
	}

	[Controls(typeof(SpecialCustomItem))]
	public class SpecialCustomController : BaseController
	{
	}

	[Controls(typeof(OtherCustomItem))]
	public class OtherCustomController : BaseController
	{
	}
}
