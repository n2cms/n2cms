using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcTest.Models;
using N2.Web.Mvc;
using N2.Collections;
using MvcTest.Views.NewsContainer;

namespace MvcTest.Controllers
{
	[Controls(typeof(NewsContainer))]
	public class NewsContainerController : ContentController<NewsContainer>
	{
		public override void Index()
		{
			RenderView("Index", new NewsContainerViewData { Container = CurrentItem, News = CurrentItem.GetNews()});
		}
	}
}
