using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Web.Mvc;
using N2.Collections;
using System.Web.Routing;

namespace N2.Web
{
    internal class TagWrapper : IDisposable
    {
        TagBuilder tag;
        TextWriter writer;

        public TagWrapper(TagBuilder tag, TextWriter writer)
        {
            this.tag = tag;
            this.writer = writer;

            writer.Write(tag.ToString(TagRenderMode.StartTag));
        }

        public static TagWrapper Begin(string tagName, HierarchyNode<ContentItem> node, Action<HierarchyNode<ContentItem>, TagBuilder> tagModifier, TextWriter writer)
        {
            var tag = new TagBuilder(tagName);
            if (tagModifier != null)
                tagModifier(node, tag);

            return new TagWrapper(tag, writer);
        }

        public static IDisposable Begin(string tagName, TextWriter writer, object htmlAttributes)
        {
            var tag = new TagBuilder(tagName);
            tag.MergeAttributes(htmlAttributes);

            return new TagWrapper(tag, writer);
        }

        public static IDisposable Begin(string tagName, TextWriter writer, RouteValueDictionary htmlAttributes)
        {
            var tag = new TagBuilder(tagName);
            tag.MergeAttributes(htmlAttributes);

            return new TagWrapper(tag, writer);
        }

        public static TagWrapper Begin(string tagName, string id, string cssClass, TextWriter writer)
        {
            var tag = new TagBuilder(tagName);
            tag.AddAttributeUnlessEmpty("id", id);
            tag.AddAttributeUnlessEmpty("class", cssClass);

            return new TagWrapper(tag, writer);
        }

        #region IDisposable Members

        public void Dispose()
        {
            writer.Write(tag.ToString(TagRenderMode.EndTag));
        }

        #endregion
    }
}
