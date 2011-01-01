using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using N2.Engine;
using System.Linq.Expressions;

namespace N2.Web.Mvc
{
	/// <summary>
	/// Controller Factory class for instantiating controllers using the Windsor IoC container.
	/// </summary>
	[Service]
	public class ServiceLocatingControllerFactory : DefaultControllerFactory
	{
		private IEngine engine;

		/// <summary>
		/// Creates a new instance of the <see cref="ServiceLocatingControllerFactory"/> class.
		/// </summary>
		/// <param name="engine">The N2 engine instance to use when creating controllers.</param>
		public ServiceLocatingControllerFactory(IEngine engine)
		{
			if (engine == null) throw new ArgumentNullException("engine");
			this.engine = engine;
		}

		public override IController CreateController(RequestContext requestContext, string controllerName)
		{
			// TODO
			EnsureDataToken(ContentRoute.ContentItemKey, requestContext.RouteData);
			EnsureDataToken(ContentRoute.ContentPageKey, requestContext.RouteData);
			if(!requestContext.RouteData.DataTokens.ContainsKey(ContentRoute.ContentEngineKey))
			    requestContext.RouteData.DataTokens[ContentRoute.ContentEngineKey] = engine;
			return base.CreateController(requestContext, controllerName);
		}

		private void EnsureDataToken(string key, System.Web.Routing.RouteData routeData)
		{
			if (!routeData.DataTokens.ContainsKey(key) && routeData.Values.ContainsKey(key))
			{
				int id = Convert.ToInt32(routeData.Values[key]);
				routeData.DataTokens[key] = engine.Persister.Get(id);
			}
		}

		protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType)
		{
			if (controllerType == null)
			{
				if (NotFoundControllerSelector != null)
				{
					var pair = NotFoundControllerSelector(requestContext);
					controllerType = pair.ControllerType;
					foreach (var kvp in pair.RotueValues)
						requestContext.RouteData.Values[kvp.Key] = kvp.Value;
				}

				if(controllerType == null)
					throw new HttpException(404, string.Format("The controller for path '{0}' could not be found or it does not implement IController.", requestContext.HttpContext.Request.Path));
			}

			return (IController)engine.Resolve(controllerType);
		}


		public override void ReleaseController(IController controller)
		{
			var disposable = controller as IDisposable;

			if (disposable != null)
			{
				disposable.Dispose();
			}

			engine.Release(controller);
		}

		class Pair
		{
			public Type ControllerType { get; set; }
			public RouteValueDictionary RotueValues { get; set; }
		}

		/// <summary>An optional factory to be invoked when no controller is found.</summary>
		private Func<RequestContext, Pair> NotFoundControllerSelector { get; set; }

		public virtual void NotFound<T>(System.Linq.Expressions.Expression<Func<T, ActionResult>> expression) where T : IController
		{
			var method = (MethodCallExpression)expression.Body;
			var p = new Pair();
			p.ControllerType = method.Object.Type;
			p.RotueValues = new RouteValueDictionary { { "action", method.Method.Name } };
			NotFoundControllerSelector = (r) => p;
		}
	}
}