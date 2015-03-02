using System;
using System.Collections.Specialized;
using N2.Configuration;
using N2.Edit.Versioning;
using N2.Engine;
using N2.Web.UI.WebControls;

namespace N2.Web
{
    /// <summary>
    /// Resolves the controller to handle a certain request. Supports a default 
    /// controller or additional imprativly using the ConnectControllers method 
    /// or declarativly using the [Controls] attribute registered.
    /// </summary>
    [Service]
    public class RequestPathProvider
    {
        private readonly IWebContext webContext;
        private readonly IUrlParser parser;
        private readonly IErrorNotifier errorHandler;
        private readonly DraftRepository draftRepository;
        private readonly bool rewriteEmptyExtension = true;
        private readonly bool observeAllExtensions = true;
        private readonly string[] observedExtensions = new[] {".aspx"};
        private readonly string[] nonRewritablePaths;

        public RequestPathProvider(IWebContext webContext, IUrlParser parser, IErrorNotifier errorHandler, HostSection config, DraftRepository draftRepository)
        {
            this.webContext = webContext;
            this.parser = parser;
            this.errorHandler = errorHandler;
            this.draftRepository = draftRepository;

            observeAllExtensions = config.Web.ObserveAllExtensions;
            rewriteEmptyExtension = config.Web.ObserveEmptyExtension;
            StringCollection additionalExtensions = config.Web.ObservedExtensions;
            if (additionalExtensions != null && additionalExtensions.Count > 0)
            {
                observedExtensions = new string[additionalExtensions.Count + 1];
                additionalExtensions.CopyTo(observedExtensions, 1);
            }
            observedExtensions[0] = config.Web.Extension;
            nonRewritablePaths = config.Web.Urls.NonRewritable.GetPaths(webContext);
        }

        public virtual PathData GetCurrentPath()
        {
            Url url = webContext.Url;
            if (!IsRewritable(url) || !IsObservable(url))
                return PathData.Empty;

            PathData data = ResolveUrl(url);
            return data;
        }

        public virtual PathData ResolveUrl(Url url)
        {
            try
            {
                var path = parser.FindPath(url.RemoveDefaultDocument(Url.DefaultDocument).RemoveExtension(observedExtensions));

                path.CurrentItem = path.CurrentPage;

                if (draftRepository.Versions.TryParseVersion(url[PathData.VersionIndexQueryKey], url[PathData.VersionKeyQueryKey], path))
                    return path;

                string viewPreferenceParameter = url.GetQuery(WebExtensions.ViewPreferenceQueryString);
                if (viewPreferenceParameter == WebExtensions.DraftQueryValue && draftRepository.HasDraft(path.CurrentItem))
                {
                    var draft = draftRepository.Versions.GetVersion(path.CurrentPage);
					path.TryApplyVersion(draft, url["versionKey"], draftRepository.Versions);
                }

                return path;
            }
            catch (Exception ex)
            {
                errorHandler.Notify(ex);
            }
            return PathData.Empty;
        }

        public virtual bool IsRewritable(Url url)
        {
            string path = url.Path;
            foreach (string nonRewritablePath in nonRewritablePaths)
            {
                if (path.StartsWith(nonRewritablePath, StringComparison.InvariantCultureIgnoreCase))
                    return false;
            }
            return true;
        }

        [Obsolete("Use ResolveUrl((Url)url))")]
        public virtual PathData ResolveUrl(string url)
        {
            return ResolveUrl(new Url(url));
        }

        public virtual bool IsObservable(Url url)
        {
            if (observeAllExtensions)
                return true;

            if(url.LocalUrl == Url.ApplicationPath)
                return true;

            string extension = url.Extension;
            if (rewriteEmptyExtension && string.IsNullOrEmpty(extension))
                return true;
            foreach (string observed in observedExtensions)
                if (string.Equals(observed, extension, StringComparison.InvariantCultureIgnoreCase))
                    return true;
            if (url.GetQuery(PathData.PageQueryKey) != null)
                return true;

            return false;
        }
    }
}
