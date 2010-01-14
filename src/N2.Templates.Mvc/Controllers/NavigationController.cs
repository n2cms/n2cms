using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using N2.Collections;
using N2.Engine.Globalization;
using N2.Templates.Mvc.Models;

namespace N2.Templates.Mvc.Controllers
{
	public class NavigationController : N2Controller<ContentItem>
	{
		private readonly ILanguageGateway _languageGateway;

		public NavigationController(ILanguageGateway languageGateway)
		{
			_languageGateway = languageGateway;
		}

		public PartialViewResult TopMenu()
		{
            // Top menu for the current language starts at the nearest language root
            ContentItem branchRoot = Find.ClosestLanguageRoot; 

            var model = new TopMenuModel(GetTranslations(), branchRoot, branchRoot
			                                                            	.GetChildren(new NavigationFilter())
			                                                            	.OrderBy(i => i.SortOrder));

			return PartialView(model);
		}

		private IEnumerable<Translation> GetTranslations()
		{
			ItemFilter languageFilter = new CompositeFilter(new AccessFilter(), new PublishedFilter());
			IEnumerable<ContentItem> translations = _languageGateway.FindTranslations(Find.ClosestStartPage);
			foreach (ContentItem translation in languageFilter.Pipe(translations))
			{
				ILanguage language = _languageGateway.GetLanguage(translation);

				// Hide translations when filtered access to their language
				ContentItem languageItem = language as ContentItem;
				if (languageItem == null || languageFilter.Match(languageItem))
					yield return new Translation(translation, language);
			}
		}

		public PartialViewResult SubMenu()
		{
			ContentItem branchRoot = Find.AncestorAtLevel(2, Find.EnumerateParents(N2.Find.CurrentPage, Find.ClosestStartPage, true).ToList(),
			                                              N2.Find.CurrentPage);
			var model = new SubMenuModel();

			if (branchRoot != null && branchRoot.GetChildren(new NavigationFilter()).Count > 0)
			{
				model.CurrentItem = N2.Find.CurrentPage;
				model.BranchRoot = branchRoot;
				model.Items = branchRoot.GetChildren(new NavigationFilter());
			}
			else
			{
				model.Visible = false;
			}

			return PartialView(model);
		}

		public PartialViewResult Breadcrumb()
		{
			var items = Find.EnumerateParents(Find.CurrentPage, Find.ClosestStartPage, true).Reverse().ToArray();

			if(items.Length == 1)
				return null;

			return PartialView(items);
		}
	}
}