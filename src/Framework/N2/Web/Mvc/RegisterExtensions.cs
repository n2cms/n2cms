using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using N2.Definitions.Runtime;
using N2.Definitions;

namespace N2.Web.Mvc
{
	public static class RegisterExtensions
	{
		public static void ControlledBy<TController>(this IContentRegistration re) where TController : IController
		{
			re.RegisterRefiner(new RegisterControllerRefinder(typeof(TController)));
		}

		class RegisterControllerRefinder : ISortableRefiner
		{
			private Type controllerType;

			public RegisterControllerRefinder(Type controllerType)
			{
				this.controllerType = controllerType;
			}

			public int RefinementOrder
			{
				get { return 0; }
			}

			public void Refine(ItemDefinition currentDefinition, IList<ItemDefinition> allDefinitions)
			{
				var descriptor = new ReflectedControllerDescriptor(controllerType);
				var methods = descriptor.GetCanonicalActions()
					.Select(m => m.ActionName).ToArray();

				PathDictionary.PrependFinder(currentDefinition.ItemType, new ActionResolver(descriptor.ControllerName, methods));
			}

			public int CompareTo(ISortableRefiner other)
			{
				return RefinementOrder - other.RefinementOrder;
			}
		}
	}
}
