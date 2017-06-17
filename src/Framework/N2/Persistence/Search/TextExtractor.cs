using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Engine;
using N2.Web;
using System.IO;
using N2.Engine.Globalization;

namespace N2.Persistence.Search
{
    /// <summary>
    /// Extracts text from content.
    /// </summary>
    [Service]
    public class TextExtractor
    {
        Logger<TextExtractor> logger;

        ITextExtractor[] extractors;

        public TextExtractor(params ITextExtractor[] extractors)
        {
            this.extractors = extractors;
        }

        public virtual bool IsIndexable(ContentItem item)
        {
            if (item.GetContentType().GetCustomAttributes(true).OfType<IIndexableType>().Any(it => !it.IsIndexable))
                return false;

            return true;
        }

        public virtual IEnumerable<IndexableField> Extract(ContentItem item)
        {
            return extractors.SelectMany(e => e.Extract(item)).Where(ic => !string.IsNullOrEmpty(ic.Value));
        }

        public virtual string Join(IEnumerable<IndexableField> content)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var c in content)
                sb.AppendLine(c.Value);
            return sb.ToString();
        }

        /// <summary>Creates a document that can be indexed.</summary>
        /// <param name="item">The item containing values to be indexed.</param>
        /// <returns>A document that can be passed to the Update method to be indexed.</returns>
        public virtual IndexableDocument CreateDocument(ContentItem item)
        {
            var doc = new IndexableDocument();
            doc.ID = item.ID;

            doc.Add(Properties.ID, item.ID.ToString(), store: true, analyze: false);
            doc.Add(Properties.Title, item.Title, store: true, analyze: true);
            doc.Add(Properties.Name, item.Name, store: true, analyze: false);
            doc.Add(Properties.SavedBy, item.SavedBy, store: true, analyze: false);
            doc.Add(Properties.Created, item.Created, store: true, analyze: false);
            doc.Add(Properties.Updated, item.Updated, store: true, analyze: false);
            doc.Add(Properties.Published, item.Published, store: true, analyze: false);
            doc.Add(Properties.Expires, item.Expires, store: true, analyze: false);
            if (item.IsPage && item.State != ContentState.Deleted)
            {
                try
                {
                    doc.Add(Properties.Url, item.Url, store: true, analyze: false);
                }
                catch (TemplateNotFoundException ex)
                {
                    logger.Warn("Failed to retrieve Url on " + item, ex);
                    return null;
                }
            }
            doc.Add(Properties.Path, item.Path, store: true, analyze: false);
            doc.Add(Properties.AncestralTrail, item.AncestralTrail, store: true, analyze: false);
            doc.Add(Properties.Trail, Utility.GetTrail(item), store: true, analyze: false);
            doc.Add(Properties.AlteredPermissions, (int)item.AlteredPermissions, store: true, analyze: false);
            doc.Add(Properties.State, (int)item.State, store: true, analyze: false);
            doc.Add(Properties.IsPage, item.IsPage.ToString().ToLower(), store: true, analyze: false);
            doc.Add(Properties.Roles, GetRoles(item), store: true, analyze: true);
            doc.Add(Properties.Types, GetTypes(item), store: true, analyze: true);
            doc.Add(Properties.Language, GetLanguage(item), store: true, analyze: true);
            doc.Add(Properties.Visible, item.Visible.ToString().ToLower(), store: true, analyze: false);
            doc.Add(Properties.SortOrder, item.SortOrder, store: true, analyze: false);

            var texts = Extract(item);
            foreach (var t in texts)
                doc.Add("Detail." + t.Name, t.Value, store: t.Stored, analyze: t.Analyzed);
            
            doc.Add("Text", Join(texts), store: false, analyze: true);

            using (var sw = new StringWriter())
            {
                AppendPartsRecursive(item, sw);
                doc.Add("PartsText", sw.ToString(), store: false, analyze: true);
            }

            return doc;
        }

        private string GetLanguage(ContentItem item)
        {
            var language = Find.Closest<ILanguage>(item);
            if (language == null)
                return "";

            return language.LanguageCode ?? "";
        }

        private static string GetTypes(ContentItem item)
        {
            string types = string.Join(" ",
                Utility.GetBaseTypesAndSelf(item.GetContentType())
                .Union(item.GetContentType().GetInterfaces()).Select(t => t.Name).ToArray());
            return types;
        }

        private static string GetRoles(ContentItem item)
        {
            string roles = string.Join(" ", item.AuthorizedRoles.Select(r => r.Role).ToArray());
            if (string.IsNullOrEmpty(roles))
                roles = Security.AuthorizedRole.Everyone;
            return roles;
        }

        private void AppendPartsRecursive(ContentItem parent, StringWriter partTexts)
        {
            foreach (var part in parent.Children.FindParts())
            {
                partTexts.WriteLine(part.Title);
                string text = Join(Extract(part));
                partTexts.WriteLine(text);

                AppendPartsRecursive(part, partTexts);
            }
        }

        private static string CombineTexts(IDictionary<string, string> texts)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var value in texts.Values)
                sb.AppendLine(value);
            return sb.ToString();
        }

        public static class Properties
        {
            public const string ID = "ID";
            public const string Title = "Title";
            public const string Name = "Name";
            public const string SavedBy = "SavedBy";
            public const string Created = "Created";
            public const string Updated = "Updated";
            public const string Published = "Published";
            public const string Expires = "Expires";
            public const string Url = "Url";
            public const string Path = "Path";
            public const string AncestralTrail = "AncestralTrail";
            public const string Trail = "Trail";
            public const string AlteredPermissions = "AlteredPermissions";
            public const string State = "State";
            public const string IsPage = "IsPage";
            public const string Roles = "Roles";
            public const string Types = "Types";
            public const string Language = "Language";
            public const string Visible = "Visible";
            public const string SortOrder = "SortOrder";

            public static HashSet<string> All = new HashSet<string> { ID, Title, Name, SavedBy, Created, Updated, Published, Expires, Url, Path, AncestralTrail, Trail, AlteredPermissions, State, IsPage, Roles, Types, Language, Visible };
        }
    }
}
