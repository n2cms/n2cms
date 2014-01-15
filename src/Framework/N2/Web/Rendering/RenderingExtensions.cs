using System.Collections.Generic;
using N2.Details;
using System.Linq;
using System.Web.Mvc;
using System;
namespace N2.Web.Rendering
{
    public static class RenderingExtensions
    {
        internal const string ContextKey = "DisplayableToken.Context";
        internal const string TokenKey = "DisplayableToken.Token";

        public static string TextUntil(this string text, params char[] untilAny)
        {
            return TextUntil(text, 0, untilAny);
        }

        public static string TextUntil(this string text, int startIndex, params char[] untilAny)
        {
            int index = text.IndexOfAny(untilAny, startIndex);
            if (index < 0)
                return text;
            return text.Substring(startIndex, index - startIndex);
        }

        public static string TextUntil(this string text, string untilFirst)
        {
            int index = text.IndexOf(untilFirst);
            if (index < 0)
                return text;
            return text.Substring(0, index);
        }

        public static IEnumerable<DisplayableToken> GetTokens(this ContentItem item, string propertyName)
        {
            var tokens = item.GetDetailCollection(propertyName + DisplayableTokensAttribute.CollectionSuffix, false);
            if (tokens == null)
                return Enumerable.Empty<DisplayableToken>();

            return tokens.Details.Select(d => d.StringValue.ExtractToken(d.IntValue ?? 0));
        }

        public static DisplayableToken ExtractToken(this ContentDetail tokenDetail)
        {
            return tokenDetail.StringValue.ExtractToken(tokenDetail.IntValue ?? 0);
        }

        public static DisplayableToken ExtractToken(this string tokenString, int index)
        {
            string name = tokenString.TextUntil(2, '|', '}');
            var value = tokenString.Length >= name.Length + 2 && tokenString[name.Length + 2] == '|'
                ? tokenString.Substring(name.Length + 3, tokenString.Length - name.Length - 5)
                : null;

            return new DisplayableToken { Name = name, Value = value, Index = index };
        }

        public static RenderingContext RenderingContext(this HtmlHelper html)
        {
            return html.ViewContext.ViewData[RenderingExtensions.ContextKey] as RenderingContext;
        }

        public static DisplayableToken DisplayableToken(this HtmlHelper html)
        {
            return html.ViewContext.ViewData[RenderingExtensions.TokenKey] as DisplayableToken;
        }

        public static string[] GetComponents(this DisplayableToken token)
        {
            if (string.IsNullOrEmpty(token.Value))
                return new string[] { "" };

            return token.Value.Split('|');
        }

        public static bool Is(this DisplayableToken token, string tokenName)
        {
            return token.Name.Equals(tokenName, StringComparison.InvariantCultureIgnoreCase);
        }

        public static string GetOptionalInputName(this DisplayableToken token, int position, int treshold)
        {
            string[] components = token.GetComponents();
            return components.Length > treshold ? components[position] : (token.Name + token.Index);
        }

        public static string GenerateInputName(this DisplayableToken token)
        {
            return token.Name + token.Index;
        }
    }
}
