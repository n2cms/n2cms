using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using N2.Collections;
using N2.Engine.Globalization;
using N2.Templates.Mvc.Models;
using N2.Templates.Mvc.Models.Pages;
using N2.Edit;
using N2.Web.Mvc;

namespace N2.Templates.Mvc.Controllers
{
    public class NavigationController : TemplatesControllerBase<ContentItem>
    {
        private readonly ILanguageGateway _languageGateway;

        public NavigationController(LanguageGatewaySelector languageGatewaySelector)
        {
            _languageGateway = languageGatewaySelector.GetLanguageGateway();
        }

        [ContentOutputCache]
        public PartialViewResult TopMenu()
        {
            // Top menu for the current language starts at the nearest language root
            ContentItem branchRoot = Find.ClosestLanguageRoot;
            var selected = Find.AncestorAtLevel(2, Find.EnumerateParents(CurrentPage, branchRoot, true), CurrentPage);
            var pages = branchRoot.Children.WhereNavigatable();
            pages.Insert(0, branchRoot);
            var topLevelPages = pages.TryAppendCreatorNode(Engine, branchRoot);
            var model = new TopMenuModel(GetTranslations(), selected ?? branchRoot, topLevelPages);

            return PartialView(model);
        }

        private IEnumerable<Translation> GetTranslations()
        {
            ItemFilter languageFilter = new AllFilter(new AccessFilter(), new PublishedFilter());
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

        [ContentOutputCache]
        public PartialViewResult SubMenu()
        {
            ContentItem startPage = Find.ClosestLanguageRoot;
            var ancestors = Find.EnumerateParents(N2.Find.CurrentPage, Find.ClosestLanguageRoot, true).ToList();
            ContentItem branchRoot = Find.AncestorAtLevel(2, ancestors, N2.Find.CurrentPage);
            var model = new SubMenuModel();

            if (branchRoot != null && !startPage.Equals(CurrentPage))
            {
                var children = branchRoot.Children.WhereNavigatable().TryAppendCreatorNode(Engine, branchRoot).ToList();
                if(children.Count > 0)
                {
                    model.CurrentItem = Find.AncestorAtLevel(3, ancestors, CurrentPage) ?? CurrentPage;
                    model.BranchRoot = branchRoot;
                    model.Items = children;
                    return PartialView(model);
                }
            }

            model.Visible = false;
            return PartialView(model);
        }

        [ContentOutputCache]
        public PartialViewResult Breadcrumb()
        {
            var items = Find.EnumerateParents(CurrentPage, Find.Closest<LanguageRoot>(CurrentPage) ?? Find.ClosestLanguageRoot, true).Reverse().ToArray();

            if(items.Length == 1)
                return null;

            return PartialView(items);
        }
    }
}
