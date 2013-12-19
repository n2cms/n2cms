using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using N2.Security;
using N2.Engine;
using N2.Web;

namespace N2.Collections
{
    public class FilterHelper
    {
        Func<IEngine> engine;

        public FilterHelper(Func<IEngine> engine)
        {
            this.engine = engine;
        }

        /// <summary>Filters by access.</summary>
        /// <returns>A filter.</returns>
        public ItemFilter Accessible()
        {
            return Accessible(engine().Resolve<IWebContext>().User, engine().SecurityManager);
        }

        /// <summary>Filters by access.</summary>
        /// <returns>A filter.</returns>
        public ItemFilter Accessible(IPrincipal user, ISecurityManager security)
        {
            return new AllFilter(new AccessFilter(user, security), new PublishedFilter());
        }

        /// <summary>Filters by access and pages.</summary>
        /// <returns>A filter.</returns>
        public ItemFilter AccessiblePage()
        {
            return AccessiblePage(engine().Resolve<IWebContext>().User, engine().SecurityManager);
        }

        /// <summary>Filters by access and pages.</summary>
        /// <returns>A filter.</returns>
        public ItemFilter AccessiblePage(IPrincipal user, ISecurityManager security)
        {
            return new AccessiblePageFilter(user, security);
        }

        /// <summary>Filters by all the provided filters.</summary>
        /// <param name="filters">The filters to aggregate.</param>
        /// <returns>A filter.</returns>
        public ItemFilter All(params ItemFilter[] filters)
        {
            return new AllFilter(filters);
        }

        /// <summary>Filters by all the provided filters.</summary>
        /// <param name="filters">The filters to aggregate.</param>
        /// <returns>A filter.</returns>
        public ItemFilter All(IEnumerable<ItemFilter> filters)
        {
            return new AllFilter(filters);
        }

        /// <summary>Filters by all the provided filters.</summary>
        /// <param name="filters">The filters to aggregate.</param>
        /// <returns>A filter.</returns>
        public ItemFilter Any(params ItemFilter[] filters)
        {
            return new AnyFilter(filters);
        }

        /// <summary>Filters by all the provided filters.</summary>
        /// <param name="filters">The filters to aggregate.</param>
        /// <returns>A filter.</returns>
        public ItemFilter Any(IEnumerable<ItemFilter> filters)
        {
            return new AnyFilter(filters);
        }

        /// <summary>Filters by all the provided filters.</summary>
        /// <param name="filters">The filters to aggregate.</param>
        /// <returns>A filter.</returns>
        public ItemFilter All(ItemFilter first, IEnumerable<ItemFilter> alsoRequired)
        {
            return new AllFilter(new [] { first }.Union(alsoRequired));
        }

        /// <summary>Filters by all the provided filters.</summary>
        /// <param name="filters">The filters to aggregate.</param>
        /// <returns>A filter.</returns>
        public ItemFilter Any(ItemFilter first, IEnumerable<ItemFilter> filters)
        {
            return new AnyFilter(new [] { first }.Union(filters));
        }

        /// <summary>Filters by counting items. This filter must be reset after each usage.</summary>
        /// <param name="skip">Number of items to skip.</param>
        /// <param name="take">Number of items to take.</param>
        /// <returns>A filter.</returns>
        public ItemFilter Count(int skip, int take)
        {
            return new CountFilter(skip, take);
        }

        /// <summary>Filters by the passed delegate.</summary>
        /// <param name="isMatch">A function that returns true if the item can be filtered.</param>
        /// <returns>A filter.</returns>
        public ItemFilter Custom(Func<ContentItem, bool> isMatch)
        {
            return new DelegateFilter(isMatch);
        }

        /// <summary>Filters away duplicates.</summary>
        /// <returns>A filter.</returns>
        public ItemFilter Distinct()
        {
            return new DuplicateFilter();
        }

        /// <summary>Filters by items that can be shown in a navigation. This is a composition of page, access, visibility and published filter.</summary>
        /// <returns>A filter.</returns>
        public ItemFilter Navigatable()
        {
            return new NavigationFilter();
        }

        /// <summary>Doesn't filter.</summary>
        /// <returns>A filter.</returns>
        public ItemFilter Anything()
        {
            return new NullFilter();
        }

        /// <summary>Filters by pages.</summary>
        /// <returns>A filter.</returns>
        public ItemFilter Page()
        {
            return new PageFilter();
        }

        /// <summary>Filters items that are parts on a page.</summary>
        /// <returns>A filter apart.</returns>
        public ItemFilter Part()
        {
            return new PartFilter();
        }

        /// <summary>Filters items below an ancestor.</summary>
        /// <param name="ancestor">The ancestor of the items to pass.</param>
        /// <returns>A filter.</returns>
        public ItemFilter DescendantOf(ContentItem ancestor)
        {
            return new AncestorFilter(ancestor);
        }

        /// <summary>Filters items below an ancestor or the ancestor itself.</summary>
        /// <param name="ancestor">The ancestor of the items to pass.</param>
        /// <returns>A filter.</returns>
        public ItemFilter DescendantOrSelf(ContentItem ancestor)
        {
            return new AncestorFilter(ancestor, true);
        }

        /// <summary>Filters by items that are published and not expired.</summary>
        /// <returns>A filter.</returns>
        public ItemFilter Published()
        {
            return new PublishedFilter();
        }

        /// <summary>Filters by types of page.</summary>
        /// <param name="types">The types of pages to allow.</param>
        /// <returns>A filter.</returns>
        public ItemFilter Type(params Type[] types)
        {
            return new TypeFilter(types);
        }

        /// <summary>Filters by definition name.</summary>
        /// <param name="discriminatorAndTemplateKey">A discriminator and/or template key to filter by, e.g. ContentPage, ContentPage/News or /News.</param>
        /// <returns>A filter.</returns>
        public ItemFilter Definition(string discriminatorAndTemplateKey)
        {
            int slashIndex = discriminatorAndTemplateKey.IndexOf('/');
            if(slashIndex == 0)
                return TemplateOnly(discriminatorAndTemplateKey.Substring(1));
            if(slashIndex < 0)
                return DefinitionOnly(discriminatorAndTemplateKey);

            return All(DefinitionOnly(discriminatorAndTemplateKey.Substring(0, slashIndex)), TemplateOnly(discriminatorAndTemplateKey.Substring(slashIndex + 1)));
        }

        private ItemFilter DefinitionOnly(string discriminator)
        {
            var d = engine().Definitions.GetDefinition(discriminator);
            if (d == null)
                return Anything();
            return Type(d.ItemType);
        }

        /// <summary>Filters items by temlate key.</summary>
        /// <param name="templateKey">The template key the item should have.</param>
        /// <returns>A filter.</returns>
        private ItemFilter TemplateOnly(string templateKey)
        {
            if (string.IsNullOrEmpty(templateKey))
                return Anything();

            return Custom(ci => ci.TemplateKey == templateKey);
        }

        /// <summary>Filters by type of page.</summary>
        /// <typeparam name="T">The type of page to allow.</typeparam>
        /// <returns>A filter.</returns>
        public ItemFilter Type<T>()
        {
            return new TypeFilter(typeof(T));
        }

        /// <summary>Filters by items that are visible.</summary>
        /// <returns>A filter.</returns>
        public ItemFilter Visible()
        {
            return new VisibleFilter();
        }

        /// <summary>Inverses the given filter.</summary>
        /// <param name="itemFilter">The filter to inverse.</param>
        /// <returns>A filter that does the inverse of the given filter.</returns>
        public ItemFilter Not(ItemFilter filterToInverse)
        {
            return new InverseFilter(filterToInverse);
        }

        /// <summary>Filters items returning those of the given state.</summary>
        /// <param name="requiredState">The state to return.</param>
        /// <returns>A filter that filters on content state.</returns>
        public ItemFilter State(ContentState requiredState)
        {
            return new StateFilter(requiredState);
        }

        /// <summary>Filters items below an item of a certain type.</summary>
        /// <typeparam name="T">The type of item to exist as ancestor or self.</typeparam>
        /// <returns>A filter that filters on a type of ancestor.</returns>
        public ItemFilter DescendantOf<T>() where T : class
        {
            return new DelegateFilter(ci => Find.Closest<T>(ci) != null);
        }

        /// <summary>Filters items in a certain zone.</summary>
        /// <param name="zoneName">The name of the zone the items should be in.</param>
        /// <returns>A filter that filters on zone name.</returns>
        public ItemFilter InZone(string zoneName)
        {
            return new ZoneFilter(zoneName);
        }
    }
}
