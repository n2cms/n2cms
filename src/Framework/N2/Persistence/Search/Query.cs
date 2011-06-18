using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Principal;
using N2.Engine.Globalization;

namespace N2.Persistence.Search
{
	/// <summary>
	/// Conveys search settings to the text search feature.
	/// </summary>
	public class Query
	{
		public Query()
		{
			TakeHits = 10;
		}

		/// <summary>The ancestor below which the results should be found.</summary>
		public ContentItem Ancestor { get; set; }

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
		public Type[] Types { get; set; }

		/// <summary>Search for pages belonging to the given language code.</summary>
		public string LanguageCode { get; set; }

		/// <summary>Query whose hits excludes hits from this query.</summary>
		public Query Exclution { get; set; }

		/// <summary>Query whose hits this query results must also match.</summary>
		public Query Intersection { get; set; }

		/// <summary>Query whose hits are added to this query results.</summary>
		public Query Union { get; set; }

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
			return new Query { Types = types };
		}

		/// <summary>Restricts the search query to items below the given item.</summary>
		/// <param name="ancestor">The ancestor below which to find items.</param>
		/// <returns>The query itself.</returns>
		public Query Below(ContentItem ancestor)
		{
			this.Ancestor = ancestor;
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
		/// <param name="gerRolesForUser">A method that will return an array of roles given a user name.</param>
		/// <returns>The query itself.</returns>
		public Query ReadableBy(IPrincipal user, Func<string, string[]> gerRolesForUser)
		{
			return ReadableBy(user.Identity.IsAuthenticated ? gerRolesForUser(user.Identity.Name) : new[] { "Everyone" });
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
			Types = types;
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
	}
}
