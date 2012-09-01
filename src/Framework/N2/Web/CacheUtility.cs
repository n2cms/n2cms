using System;
using System.Collections.Generic;
using System.IO;
using System.Web;

namespace N2.Web
{
	/// <summary>
	/// Helper methods for setting and checking cache headers.
	/// </summary>
	public static class CacheUtility
	{
		/// <summary>Checks the request's If-Modified-Since against a file last write time.</summary>
		/// <param name="request">The request to check for last headers.</param>
		/// <param name="filePaths">Paths to file that may have changed.</param>
		/// <returns>True if the file has changed or no request header.</returns>
		public static bool IsUnmodifiedSince(this HttpRequest request, params string[] filePaths)
		{
			return IsUnmodifiedSince(request, (IEnumerable<string>)filePaths);
		}

		/// <summary>Checks the request's If-Modified-Since against a file last write time.</summary>
		/// <param name="request">The request to check for last headers.</param>
		/// <param name="filePaths">Paths to file that may have changed.</param>
		/// <returns>True if the file has changed or no request header.</returns>
		public static bool IsUnmodifiedSince(this HttpRequest request, IEnumerable<string> filePaths)
		{
			string ifModifiedSince = request.Headers["If-Modified-Since"];
			if (!string.IsNullOrEmpty(ifModifiedSince))
			{
				DateTimeOffset since;
				if (DateTimeOffset.TryParse(ifModifiedSince, out since))
					foreach (string file in filePaths)
						if (file != null && File.Exists(file) && File.GetLastWriteTimeUtc(file) < since)
							return true;
			}
			return false;
		}

		/// <summary>Checks the request's If-Modified-Since against a date.</summary>
		/// <param name="request">The request to check for last headers.</param>
		/// <param name="utcDate">The date to compare against.</param>
		/// <returns>True if the file has changed or no request header.</returns>
		public static bool IsUnmodifiedSince(this HttpRequest request, DateTime utcDate)
		{
			string ifModifiedSince = request.Headers["If-Modified-Since"];
			if (!string.IsNullOrEmpty(ifModifiedSince))
			{
				DateTimeOffset since;
				if (DateTimeOffset.TryParse(ifModifiedSince, out since))
					if (utcDate < since)
						return true;
			}
			return false;
		}

		/// <summary>Sets public cacheablility (ask server if resources is modified) on the response header.</summary>
		/// <param name="response">The response whose cache to modify.</param>
		/// <param name="expirationTime">The time before the resource expires.</param>
		public static void SetValidUntilExpires(this HttpResponse response, TimeSpan expirationTime)
		{
			response.Cache.SetExpires(DateTime.UtcNow.Add(expirationTime));
			response.Cache.SetCacheability(HttpCacheability.Public);
			response.Cache.SetLastModified(DateTime.UtcNow);
			response.Cache.SetMaxAge(expirationTime);
			response.Cache.SetValidUntilExpires(true);
		}

		/// <summary>Sets public cacheablility (ask server if resources is modified) on the response header.</summary>
		/// <param name="response">The response whose cache to modify.</param>
		/// <param name="utcLastModified">The time the resource was modified.</param>
		public static void SetValidUntilExpires(this HttpResponse response, DateTime utcLastModified)
		{
			response.Cache.SetExpires(DateTime.UtcNow.AddMonths(1));
			response.Cache.SetCacheability(HttpCacheability.Public);
			response.Cache.SetLastModified(utcLastModified);
			response.Cache.SetMaxAge(TimeSpan.FromDays(31));
			response.Cache.SetValidUntilExpires(true);
		}

		/// <summary>Ends the response with not modified status.</summary>
		/// <param name="response">The response to end.</param>
		public static void NotModified(this HttpResponse response)
		{
			response.Status = "304 Not Modified";
			response.End();
		}

		/// <summary>Sets public cacheablility (ask server if resources is modified) on the response header.</summary>
		/// <param name="response">The response whose cache to modify.</param>
		/// <param name="expires">The time the resource expires.</param>
		public static HttpResponse SetOutputCache(this HttpResponse response, DateTime expires)
		{
			response.Cache.SetExpires(expires);
			response.Cache.SetCacheability(HttpCacheability.ServerAndPrivate);
			response.Cache.SetValidUntilExpires(true);
			return response;
		}

		/// <summary>Sets public cacheablility (ask server if resources is modified) on the response header.</summary>
		/// <param name="response">The response whose cache to modify.</param>
		/// <param name="expires">The time the resource expires.</param>
		public static HttpResponseBase SetOutputCache(this HttpResponseBase response, DateTime expires)
		{
			response.Cache.SetExpires(expires);
			response.Cache.SetCacheability(HttpCacheability.ServerAndPrivate);
			response.Cache.SetValidUntilExpires(true);
			return response;
		}
	}
}
