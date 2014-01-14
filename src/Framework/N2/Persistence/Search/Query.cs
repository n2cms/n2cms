using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Principal;
using N2.Engine.Globalization;
using System.Linq.Expressions;

namespace N2.Persistence.Search
{
    /// <summary>
    /// A query for a specific types. This allows for strongly typed expressions.
    /// </summary>
    /// <typeparam name="T">The type of item to query for.</typeparam>
    public class Query<T> : Query
    {
        /// <summary>
        /// Allows search for property contents via a strongly typed expression, e.g. query.Contains(pi => pi.Title, "Root");
        /// </summary>
        /// <typeparam name="TProperty">The return type of the property.</typeparam>
        /// <param name="propertyExpression">The property expression.</param>
        /// <param name="value">The property value to search for.</param>
        /// <returns>The query object itself.</returns>
        public Query<T> Contains<TProperty>(Expression<Func<T, TProperty>> propertyExpression, string value)
        {
            MemberExpression(propertyExpression.Body as MemberExpression, value);
            return this;
        }

        private void MemberExpression(MemberExpression body, string value)
        {
            if (body == null) return;

            base.Property(body.Member.Name, value);
        }
    }

    /// <summary>
    /// Conveys search settings to the text search feature.
    /// </summary>
    public class Query
    {
        public Query()
        {
            TakeHits = 10;
            Details = new Dictionary<string, string>();
            SortFields = new List<SortFieldData>();
        }

        /// <summary>The ancestor trail below which the results should be found.</summary>
        public string Ancestor { get; set; }

        /// <summary>The query text.</summary>
        public string Text { get; set; }

        /// <summary>A number of hits to skip.</summary>
        public int SkipHits { get; set; }

        /// <summary>A number of hits to take. Defaults to 10.</summary>
        public int TakeHits { get; set; }

        /// <summary>Specific roles to filter the search by.</summary>
        public string[] Roles { get; set; }

        /// <summary>Only search for pages.</summary>
        public bool? OnlyPages { get; set; }

        /// <summary>Types the matches should belong to (either one of them).</summary>
        public string[] Types { get; set; }

        /// <summary>Search for pages belonging to the given language code.</summary>
        public string LanguageCode { get; set; }

        public IDictionary<string, string> Details { get; set; }

        /// <summary>Query whose hits excludes hits from this query.</summary>
        public Query Exclution { get; set; }

        /// <summary>Query whose hits this query results must also match.</summary>
        public Query Intersection { get; set; }

        /// <summary>Query whose hits are added to this query results.</summary>
        public Query Union { get; set; }

        public string SortField
        {
            get 
            {
                return SortFields.Select(sf => sf.SortField).FirstOrDefault(); 
            }
            set 
            {
                var sortFields = SortFields.FirstOrDefault();
                if (sortFields == null)
                    SortFields.Add(new SortFieldData(value));
                else
                    sortFields.SortField = value;
            }
        }

        public bool SortDescending
        {
            get
            {
                return SortFields.Select(sf => sf.SortDescending).FirstOrDefault();
            }
            set
            {
                var sortFields = SortFields.FirstOrDefault();
                if (sortFields == null)
                    SortFields.Add(new SortFieldData(null, value));
                else
                    sortFields.SortDescending = value;
            }
        }

        public List<SortFieldData> SortFields { get; private set; }

        /// <summary>Gets a search query for the given search expression.</summary>
        /// <param name="textQuery">The text to search for.</param>
        /// <returns>A <see cref="Query"/> object.</returns>
        public static Query For(string textQuery)
        {
            return new Query { Text = textQuery };
        }

        /// <summary>Gets a search query for the given search expression.</summary>
        /// <param name="textQuery">The text to search for.</param>
        /// <returns>A <see cref="Query"/> object.</returns>
        public static Query For(params Type[] types)
        {
            return new Query { Types = types.Select(t => t.Name).ToArray() };
        }

        /// <summary>
        /// Allows querying for specific 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="comparisons"></param>
        /// <returns></returns>
        public static Query<T> For<T>()
        {
            return new Query<T> { Types = new[] { typeof(T).Name } };
        }

        /// <summary>Restricts the search query to items below the given item.</summary>
        /// <param name="ancestor">The ancestor below which to find items.</param>
        /// <returns>The query itself.</returns>
        public Query Below(ContentItem ancestor)
        {
            this.Ancestor = ancestor.GetTrail();
            return this;
        }

        /// <summary>Restricts the search query to items below the given item.</summary>
        /// <param name="ancestor">The ancestor below which to find items.</param>
        /// <returns>The query itself.</returns>
        public Query Below(string ancestorTrail)
        {
            this.Ancestor = ancestorTrail;
            return this;
        }

        /// <summary>Restricts the results to items readable by any one of the given roles.</summary>
        /// <param name="roles">The roles that should be allowed to read an item being returned.</param>
        /// <returns>The query itself.</returns>
        public Query ReadableBy(params string[] roles)
        {
            this.Roles = roles;
            return this;
        }

        /// <summary>Restricts the results to items readable to roles returned by the get roles delegate.</summary>
        /// <param name="user">The user whose roles to restrict search results by.</param>
        /// <param name="getRolesForUser">A method that will return an array of roles given a user name.</param>
        /// <returns>The query itself.</returns>
        public Query ReadableBy(IPrincipal user, Func<string, string[]> getRolesForUser)
        {
            return ReadableBy(user.Identity.IsAuthenticated ? getRolesForUser(user.Identity.Name) : new[] { N2.Security.AuthorizedRole.Everyone });
        }

        /// <summary>Skip and take results.</summary>
        /// <param name="skip">The number of results to skip.</param>
        /// <param name="take">The number of results to take (default 10).</param>
        /// <returns>The query itself.</returns>
        public Query Range(int skip, int take)
        {
            this.SkipHits = skip;
            this.TakeHits = take;
            return this;
        }

        /// <summary>Skip results.</summary>
        /// <param name="skipHits">The number of results to skip.</param>
        /// <returns>The query itself.</returns>
        public Query Skip(int skipHits)
        {
            this.SkipHits = skipHits;
            return this;
        }

        /// <summary>Take results.</summary>
        /// <param name="takeHits">The number of results to take.</param>
        /// <returns>The query itself.</returns>
        public Query Take(int takeHits)
        {
            this.TakeHits = takeHits;
            return this;
        }

        /// <summary>Restrict the search results to pages or part. When searching for pages also text on the parts yields results.</summary>
        /// <param name="onlySearchForPages">True only return pages, false only return parts.</param>
        /// <returns>The query itself.</returns>
        public Query Pages(bool onlySearchForPages)
        {
            OnlyPages = onlySearchForPages;
            return this;
        }

        /// <summary>Restrict the search results to certain types.</summary>
        /// <param name="types">The types the search result should belong to.</param>
        /// <returns>The query itself.</returns>
        public Query OfType(params Type[] types)
        {
            Types = types.Select(t => t.Name).ToArray();
            return this;
        }

        /// <summary>Restrict the search results to certain types.</summary>
        /// <param name="types">The types the search result should belong to.</param>
        /// <returns>The query itself.</returns>
        public Query OfType(params string[] typeNames)
        {
            Types = typeNames;
            return this;
        }

        public Query Except(Query excludeQuery)
        {
            if (Exclution != null)
                excludeQuery.Or(Exclution);
            Exclution = excludeQuery;
            return this;
        }

        public Query Except(string text)
        {
            return Except(Query.For(text));
        }

        public Query Except(params Type[] types)
        {
            return Except(Query.For(types));
        }

        public static Query operator -(Query q1, Query q2)
        {
            return q1.Except(q2);
        }

        public Query And(Query andQuery)
        {
            if (Intersection != null)
                andQuery.Intersection = Intersection;
            Intersection = andQuery;
            return this;
        }

        public static Query operator &(Query q1, Query q2)
        {
            return q1.And(q2);
        }

        public Query Or(Query orQuery)
        {
            if (Union != null)
                orQuery.Union = Union;
            Union = orQuery;
            return this;
        }

        public static Query operator |(Query q1, Query q2)
        {
            return q1.Or(q2);
        }

        /// <summary>Converts a string to a search query.</summary>
        /// <param name="searchText">The search expression.</param>
        /// <returns>A <see cref="Query"/> object.</returns>
        public static implicit operator Query(string searchText)
        {
            return Query.For(searchText);
        }

        public Query Language(string languageCode)
        {
            LanguageCode = languageCode;
            return this;
        }

        public Query Language(ILanguage language)
        {
            if (language != null && !string.IsNullOrEmpty(language.LanguageCode))
                return Language(language.LanguageCode);
            
            return this;
        }

        public Query Property(string expression, string value)
        {
            this.Details[expression] = value;
            return this;
        }

        public Query State(ContentState state)
        {
            this.Details["State"] = ((int)state).ToString();
            return this;
        }

        public Query OrderBy(string field, bool descending = false)
        {
            SortFields.Add(new SortFieldData(field, descending));
            return this;
        }

        public bool IsValid()
        {
            bool isInvalid = string.IsNullOrEmpty(this.Text)
                && this.Ancestor == null
                && this.Details.Count == 0
                && this.Exclution == null
                && this.Intersection == null
                && string.IsNullOrEmpty(this.LanguageCode)
                && !this.OnlyPages.HasValue
                && (this.Roles == null || this.Roles.Length == 0)
                && (this.Types == null || this.Types.Length == 0)
                && this.Union == null;

            return !isInvalid;
        }

        public static Query Parse(System.Web.HttpRequestBase request)
        {
            var q = Query.For(request["q"]);
            if (!string.IsNullOrEmpty(request["below"]))
                q = q.Below(request["below"]);
            if (!string.IsNullOrEmpty(request["pages"]))
                q = q.Pages(Convert.ToBoolean(request["pages"]));

            if (!string.IsNullOrEmpty(request["skip"]))
                q = q.Skip(int.Parse(request["skip"]));
            if (!string.IsNullOrEmpty(request["take"]))
                q = q.Take(int.Parse(request["take"]));
            if (!string.IsNullOrEmpty(request["types"]))
                q = q.OfType(request["types"].Split(','));
            if (!string.IsNullOrEmpty(request["roles"]))
                q = q.ReadableBy(request["roles"].Split(','));
            if (!string.IsNullOrEmpty(request["orderBy"]))
            {
                var by = request["orderBy"].Split(' ');
                q = q.OrderBy(by[0], by.Length > 1 && string.Equals(by[1], "DESC", StringComparison.InvariantCultureIgnoreCase));
            }
            return q;
        }
    }
}
