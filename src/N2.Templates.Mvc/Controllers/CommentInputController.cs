using System;
using System.Web.Mvc;
using MvcContrib.Filters;
using N2.Templates.Mvc.Items.Items;
using N2.Templates.Mvc.Models;
using N2.Web;

namespace N2.Templates.Mvc.Controllers
{
	[ModelStateToTempData]
	[Controls(typeof(CommentInput))]
	public class CommentInputController : N2Controller<CommentInput>
	{
		public override ActionResult Index()
		{
			return View(new CommentInputModel(CurrentItem));
		}

		public ActionResult Submit(CommentInputModel model)
		{
			if(!ModelState.IsValid)
			{
				return Redirect(CurrentItem.Parent.Url);
			}

			// TODO: Implement
			throw new NotImplementedException();
		}
	}
}