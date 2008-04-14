using System;
using System.Collections;
using System.Collections.Generic;
using Castle.Core;
using Castle.MicroKernel;
using Castle.MicroKernel.Facilities;
using N2.Definitions;

namespace N2.Edit.Settings
{
	public class SettingsFinder : IFacility
	{
		IKernel kernel;
		private SettingsManager settingsManager;

		public void Init(IKernel kernel, Castle.Core.Configuration.IConfiguration facilityConfig)
		{
			this.kernel = kernel;
			settingsManager = kernel.Resolve<SettingsManager>(new Hashtable());

			HandleRegisteredComponents();

			kernel.ComponentRegistered += Kernel_ComponentRegistered;
			kernel.ComponentUnregistered += Kernel_ComponentUnregistered;
		}

		public void Terminate()
		{
			kernel.ComponentRegistered -= Kernel_ComponentRegistered;
			kernel.ComponentUnregistered -= Kernel_ComponentUnregistered;
		}

		private void HandleRegisteredComponents()
		{
			foreach (GraphNode node in kernel.GraphNodes)
			{
				if (node is ComponentModel)
				{
					HandleComponent(node as ComponentModel);
				}
			}
		}

		private void Kernel_ComponentUnregistered(string key, IHandler handler)
		{
			settingsManager.Remove(key);
		}

		private void Kernel_ComponentRegistered(string key, IHandler handler)
		{
			HandleComponent(handler.ComponentModel);
		}

		private void HandleComponent(ComponentModel model)
		{
			Type serviceType = model.Implementation;
			if (serviceType != null)
			{
				settingsManager.Handle(model.Name, serviceType);
			}
		}
	}
}