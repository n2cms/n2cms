using System;
using System.Web;
using System.Web.Mvc;
using N2.Engine;

namespace N2.Web.Mvc
{
	/// <summary>
	/// Controller Factory class for instantiating controllers using the Windsor IoC container.
	/// </summary>
	[Service(typeof(IControllerFactory))]
	public class ServiceLocatingControllerFactory : DefaultControllerFactory
	{
		private IEngine _engine;

		/// <summary>
		/// Creates a new instance of the <see cref="ServiceLocatingControllerFactory"/> class.
		/// </summary>
		/// <param name="engine">The N2 engine instance to use when creating controllers.</param>
		public ServiceLocatingControllerFactory(IEngine engine)
		{
			if (engine == null)
				throw new ArgumentNullException("engine");
			_engine = engine;
		}

		public override IController CreateController(System.Web.Routing.RequestContext requestContext, string controllerName)
		{
			// TODO
			EnsureDataToken(ContentRoute.ContentItemKey, requestContext.RouteData);
			EnsureDataToken(ContentRoute.ContentPageKey, requestContext.RouteData);
			if(!requestContext.RouteData.DataTokens.ContainsKey(ContentRoute.ContentEngineKey))
			    requestContext.RouteData.DataTokens[ContentRoute.ContentEngineKey] = _engine;
			return base.CreateController(requestContext, controllerName);
		}

		private void EnsureDataToken(string key, System.Web.Routing.RouteData routeData)
		{
			if (!routeData.DataTokens.ContainsKey(key) && routeData.Values.ContainsKey(key))
			{
				int id = Convert.ToInt32(routeData.Values[key]);
				routeData.DataTokens[key] = _engine.Persister.Get(id);
			}
		}

		protected override IController GetControllerInstance(System.Web.Routing.RequestContext requestContext, Type controllerType)
		{
			if (controllerType == null)
			{
				throw new HttpException(404, string.Format("The controller for path '{0}' could not be found or it does not implement IController.", requestContext.HttpContext.Request.Path));
			}

			return (IController)_engine.Resolve(controllerType);
		}

		public override void ReleaseController(IController controller)
		{
			var disposable = controller as IDisposable;

			if (disposable != null)
			{
				disposable.Dispose();
			}

			_engine.Release(controller);
		}
	}
}