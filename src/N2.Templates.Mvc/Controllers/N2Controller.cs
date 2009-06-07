using System;
using N2.Web.Mvc;

namespace N2.Templates.Mvc.Controllers
{
	public abstract class N2Controller<T> : ContentController<T> where T : ContentItem
	{
	}
}