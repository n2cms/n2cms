using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web.UI;
using N2.Collections;
using N2.Web.UI.WebControls;
using System;
using System.Web.Mvc;

namespace N2.Web
{
    /// <summary>
    /// Creates a hierarchical tree of ul and li:s for usage on web pages.
    /// </summary>
    public class Tree : System.Web.IHtmlString
    {
        private HierarchyBuilder hierarchy;

        private Action<HierarchyNode<ContentItem>, TextWriter> linkWriter;
        private event Action<HierarchyNode<ContentItem>, TagBuilder> tagModifier;
        private ItemFilter filter = new NullFilter();
        private bool excludeRoot = false;
        Func<ContentItem, string> liClassProvider; // obsolete

        #region Constructor

        public Tree(HierarchyBuilder builder)
        {
            hierarchy = builder ?? new FixedHierarchyBuilder(new HierarchyNode<ContentItem>(null));
            linkWriter = (n, w) => Link.To(n.Current).WriteTo(w);
        }

        public Tree(HierarchyNode<ContentItem> root)
            : this(new FixedHierarchyBuilder(root ?? new HierarchyNode<ContentItem>(null)))
        {
        }

        #endregion

        #region Methods

        [Obsolete("Use LinkWriter")]
        public Tree LinkProvider(Func<ContentItem, ILinkBuilder> linkProvider)
        {
            LinkWriter((n, w) => linkProvider(n.Current).WriteTo(w));
            return this;
        }

        public Tree LinkWriter(Action<HierarchyNode<ContentItem>, TextWriter> linkWriter)
        {
            this.linkWriter = linkWriter;
            return this;
        }

        /// <summary>Provides the id for the list item elements in the resulting html.</summary>
        /// <param name="ulIdProvider">Returns an id for the list element given a content item and hierarchy depth.</param>
        /// <returns>The same <see cref="Tree"/> instance.</returns>
        public Tree IdProvider(Func<HierarchyNode<ContentItem>, string> ulIdProvider, Func<HierarchyNode<ContentItem>, string> liIdProvider)
        {
            if(ulIdProvider != null)
                tagModifier += (n, t) => { if (t.TagName == "ul") t.AddAttributeUnlessEmpty("id", ulIdProvider(n)); };
            if(liIdProvider != null)
                tagModifier += (n, t) => { if (t.TagName == "li") t.AddAttributeUnlessEmpty("id", liIdProvider(n)); };

            return this;
        }

        /// <summary>Provides the class name for the list item elements in the resulting html.</summary>
        /// <param name="liClassProvider">Returns a css class for a content item.</param>
        /// <returns>The same tree instance.</returns>
        [Obsolete("Use other ClassProvider overload")]
        public Tree ClassProvider(Func<ContentItem, string> liClassProvider)
        {
            this.liClassProvider = liClassProvider;
            ClassProvider(null, (n) => liClassProvider(n.Current));
            return this;
        }

        /// <summary>Provides the class name for the list item elements in the resulting html.</summary>
        /// <param name="ulClassProvider">Returns a css class for the list element given a content item and hierarchy depth.</param>
        /// <param name="liClassProvider">Returns a css class for the list item element given a content item.</param>
        /// <returns>The same <see cref="Tree"/> instance.</returns>
        public Tree ClassProvider(Func<HierarchyNode<ContentItem>, string> ulClassProvider, Func<HierarchyNode<ContentItem>, string> liClassProvider)
        {
            if (ulClassProvider != null)
                tagModifier += (n, t) => { if (t.TagName == "ul") t.AddAttributeUnlessEmpty("class", ulClassProvider(n)); };
            if (liClassProvider != null)
                tagModifier += (n, t) => { if (t.TagName == "li") t.AddAttributeUnlessEmpty("class", liClassProvider(n)); };

            return this;
        }

        /// <summary>Applies filters to the hierarchy for this tree.</summary>
        /// <param name="filters">The filters to apply.</param>
        /// <returns>The same <see cref="Tree"/> instance.</returns>
        public Tree Filters(params ItemFilter[] filters)
        {
            hierarchy.Children(filters);

            return this;
        }

        /// <summary>Do not render the root element and surrounding list element.</summary>
        /// <param name="exclude">True to exclude the root.</param>
        /// <returns>The same <see cref="Tree"/> instance.</returns>
        public Tree ExcludeRoot(bool exclude = true)
        {
            this.excludeRoot = exclude;
            return this;
        }

        /// <summary>Modifies container tags before they are written.</summary>
        /// <param name="tagModifier"></param>
        /// <returns></returns>
        public Tree Tag(Action<HierarchyNode<ContentItem>, TagBuilder> tagModifier)
        {
            this.tagModifier += tagModifier;
            return this;
        }

        #endregion

        #region Static Methods

        public static Tree From(ContentItem root)
        {
            Tree t = Using(new TreeHierarchyBuilder(root));
            return t;
        }

        public static Tree From(ContentItem root, int depth)
        {
            Tree t = Using(new TreeHierarchyBuilder(root, depth));
            return t;
        }

        public static Tree Between(ContentItem initialItem, ContentItem lastAncestor)
        {
            return new Tree(new BranchHierarchyBuilder(initialItem, lastAncestor));
        }

        public static Tree Between(ContentItem initialItem, ContentItem lastAncestor, bool appendAdditionalLevel, int startingDepth)
        {
            lastAncestor = Find.AtLevel(initialItem, lastAncestor, startingDepth);
            return Using(new BranchHierarchyBuilder(initialItem, lastAncestor ?? initialItem, lastAncestor != null && appendAdditionalLevel));
        }

        public static Tree Between(ContentItem initialItem, ContentItem lastAncestor, bool appendAdditionalLevel)
        {
            return Using(new BranchHierarchyBuilder(initialItem, lastAncestor, appendAdditionalLevel));
        }

        public static Tree Using(HierarchyBuilder hierarchy)
        {
            return TreeFactory(hierarchy);
        }

        public static Tree Using(HierarchyNode<ContentItem> node)
        {
            return TreeFactory(new FixedHierarchyBuilder(node));
        }

        public static Func<HierarchyBuilder, Tree> TreeFactory { get; set; }

        static Tree()
        {
            TreeFactory = (hierarchy) => new Tree(hierarchy);
        }

        #endregion

        public void WriteTo(TextWriter writer)
        {
            var root = hierarchy.Build();
            if (excludeRoot || root.Current == null)
                WriteChildren(writer, root, !excludeRoot);
            else
            {
                using (TagWrapper.Begin("ul", root, tagModifier, writer))
                {
                    using (TagWrapper.Begin("li", root, tagModifier, writer))
                    {
                        linkWriter(root, writer);
                        WriteChildren(writer, root, !excludeRoot);
                    }
                }
            }
        }

        private void WriteChildren(TextWriter writer, HierarchyNode<ContentItem> parent, bool renderUl)
        {
            IDisposable wrapper = null;
            try
            {
                foreach (var child in parent.Children)
                {
                    if (!filter.Match(child.Current))
                        continue;

                    if (renderUl && wrapper == null)
                        wrapper = TagWrapper.Begin("ul", child, tagModifier, writer);

                    using (TagWrapper.Begin("li", child, tagModifier, writer))
                    {
                        linkWriter(child, writer);
                        WriteChildren(writer, child, true);
                    }
                }

            }
            finally
            {
                if (wrapper != null)
                    wrapper.Dispose();
            }
        }

        public override string ToString()
        {
            using (var sw = new StringWriter())
            {
                WriteTo(sw);
                return sw.ToString();
            }
        }

        class DelegateControl : Control
        {
            public Action<TextWriter> RenderDelegate { get; set; }

            protected override void Render(HtmlTextWriter writer)
            {
                RenderDelegate(writer);
            }
        }

        public Control ToControl()
        {
            return new DelegateControl { RenderDelegate = this.WriteTo };
        }

        private string Invoke(HierarchyNode<ContentItem> parent, Action<HierarchyNode<ContentItem>, TextWriter> actor)
        {
            using (var sw = new StringWriter())
            {
                actor(parent, sw);
                return sw.ToString();
            }
        }

        #region IHtmlString Members

        public string ToHtmlString()
        {
            return ToString();
        }

        #endregion
    }
}
