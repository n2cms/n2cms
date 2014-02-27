using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Engine;
using System.Web.Mvc;
using System.IO;

namespace N2.Web.Tokens
{
    [Service]
    public class TokenDefinitionFinder
    {
        private IProvider<ViewEngineCollection> viewEngineProvider;
        private IWebContext webContext;

        public TokenDefinitionFinder(IWebContext webContext, Engine.IProvider<ViewEngineCollection> viewEngineProvider)
        {
            this.webContext = webContext;
            this.viewEngineProvider = viewEngineProvider;
        }

        public virtual IEnumerable<TokenDefinition> FindTokens()
        {
            var tokens = new Dictionary<string, TokenDefinition>(StringComparer.InvariantCultureIgnoreCase);
            foreach (var token in FindTokens(viewEngineProvider.Get()))
            {
                if (tokens.ContainsKey(token.Name))
                    // there can only be 1
                    continue;
                
                tokens[token.Name] = token;
            }

            return tokens.Values.OrderBy(t => t.Name).ToList();
        }

        protected IEnumerable<TokenDefinition> FindTokens(IEnumerable<IViewEngine> viewEngines)
        {
            foreach (var ve in viewEngines)
            {
                foreach (var location in GetLocations(ve))
                {
                    if (location.Contains("{1}"))
                        continue;
                    
                    var actionIndex = location.IndexOf("{0}");
                    if (actionIndex < 0)
                        continue;

                    string extension = location.Substring(actionIndex + 3);
                    string tokenDir = webContext.MapPath(location.Substring(0, actionIndex) + "TokenTemplates");
                    if (!Directory.Exists(tokenDir))
                        continue;

                    foreach (var token in Directory.GetFiles(tokenDir, "*" + extension).Select(file => ParseToken(file)))
                    {
                        yield return token;
                    }
                }
            }
        }

        private TokenDefinition ParseToken(string file)
        {
            return new TokenDefinition { Name = Path.GetFileNameWithoutExtension(file) };
        }

        private IEnumerable<string> GetLocations(IViewEngine ve)
        {
            if (ve is IDecorator<IViewEngine>)
            {
                var inner = (ve as IDecorator<IViewEngine>).Component;
                if (inner != null)
                    return GetLocations(inner);
            }
            else if (ve is VirtualPathProviderViewEngine)
            {
                var vppViewEngine = ve as VirtualPathProviderViewEngine;
                return vppViewEngine.PartialViewLocationFormats;
            }

            return new string[0];
        }
    }
}
