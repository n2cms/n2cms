using System;
using Dinamico.Models;
using N2.Web;
using N2.Web.Mvc;

namespace Dinamico.Controllers
{
	[Controls(typeof(PageModelBase))]
	public class FallbackController : ContentController
	{
	}
}