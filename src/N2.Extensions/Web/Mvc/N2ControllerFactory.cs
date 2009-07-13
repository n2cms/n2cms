using System;
using System.Web;
using System.Web.Mvc;
using N2.Engine;

namespace N2.Web.Mvc
{
	/// <summary>
	/// Controller Factory class for instantiating controllers using the Windsor IoC container.
	/// </summary>
	public class N2ControllerFactory : DefaultControllerFactory
	{
		private IEngine _engine;

		/// <summary>
		/// Creates a new instance of the <see cref="N2ControllerFactory"/> class.
		/// </summary>
		/// <param name="engine">The N2 engine instance to use when creating controllers.</param>
		public N2ControllerFactory(IEngine engine)
		{
			if (engine == null)
				throw new ArgumentNullException("engine");
			_engine = engine;
		}

		protected override IController GetControllerInstance(Type controllerType)
		{
			if (controllerType == null)
			{
				throw new HttpException(404, string.Format("The controller for path '{0}' could not be found or it does not implement IController.", RequestContext.HttpContext.Request.Path));
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