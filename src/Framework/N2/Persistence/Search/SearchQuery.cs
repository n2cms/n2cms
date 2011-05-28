using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Principal;

namespace N2.Persistence.Search
{
	/// <summary>
	/// Conveys search settings to the text search feature.
	/// </summary>
	public class SearchQuery
	{
		public SearchQuery()
		{
			Take = 10;
		}

		/// <summary>The ancestor below which the results should be found.</summary>
		public ContentItem Ancestor { get; set; }

		/// <summary>The query text.</summary>
		public string Text { get; set; }

		/// <summary>A number of hits to skip.</summary>
		public int Skip { get; set; }

		/// <summary>A number of hits to take.</summary>
		public int Take { get; set; }

		/// <summary>Specific roles to filter the search by.</summary>
		public string[] Roles { get; set; }

		/// <summary>Only search for pages.</summary>
		public bool? OnlyPages { get; set; }

		/// <summary>Gets a search query for the given search expression.</summary>
		/// <param name="textQuery">The text to search for.</param>
		/// <returns>A <see cref="SearchQuery"/> object.</returns>
		public static SearchQuery For(string textQuery)
		{
			return new SearchQuery { Text = textQuery };
		}

		/// <summary>Restricts the search query to items below the given item.</summary>
		/// <param name="ancestor">The ancestor below which to find items.</param>
		/// <returns>The query itself.</returns>
		public SearchQuery Below(ContentItem ancestor)
		{
			this.Ancestor = ancestor;
			return this;
		}

		/// <summary>Restricts the results to items readable by any one of the given roles.</summary>
		/// <param name="roles">The roles that should be allowed to read an item being returned.</param>
		/// <returns>The query itself.</returns>
		public SearchQuery ReadableBy(params string[] roles)
		{
			this.Roles = roles;
			return this;
		}

		/// <summary>Restricts the results to items readable to roles returned by the get roles delegate.</summary>
		/// <param name="user">The user whose roles to restrict search results by.</param>
		/// <param name="gerRolesForUser">A method that will return an array of roles given a user name.</param>
		/// <returns>The query itself.</returns>
		public SearchQuery ReadableBy(IPrincipal user, Func<string, string[]> gerRolesForUser)
		{
			return ReadableBy(user.Identity.IsAuthenticated ? gerRolesForUser(user.Identity.Name) : new[] { "Everyone" });
		}

		/// <summary>Skip and take results.</summary>
		/// <param name="skip">The number of results to skip.</param>
		/// <param name="take">The number of results to take (default 10).</param>
		/// <returns>The query itself.</returns>
		public SearchQuery Range(int skip, int take)
		{
			this.Skip = skip;
			this.Take = take;
			return this;
		}

		/// <summary>Restrict the search results to pages or part. When searching for pages also text on the parts yields results.</summary>
		/// <param name="onlySearchForPages">True only return pages, false only return parts.</param>
		/// <returns>The query itself.</returns>
		public SearchQuery Pages(bool onlySearchForPages)
		{
			OnlyPages = onlySearchForPages;
			return this;
		}

		/// <summary>Converts a string to a search query.</summary>
		/// <param name="searchText">The search expression.</param>
		/// <returns>A <see cref="SearchQuery"/> object.</returns>
		public static implicit operator SearchQuery(string searchText)
		{
			return SearchQuery.For(searchText);
		}
	}
}
