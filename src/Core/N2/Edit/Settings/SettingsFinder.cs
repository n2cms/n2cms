using System;
using System.Collections;
using System.Collections.Generic;
using Castle.Core;
using Castle.MicroKernel;
using Castle.MicroKernel.Facilities;
using N2.Definitions;

namespace N2.Edit.Settings
{
	public class SettingsFinder : AbstractFacility
	{
		private SettingsManager settingsManager;

		protected override void Init()
		{
			settingsManager = Kernel.Resolve<SettingsManager>(new Hashtable());

			HandleRegisteredComponents();

			Kernel.ComponentRegistered += Kernel_ComponentRegistered;
			Kernel.ComponentUnregistered += Kernel_ComponentUnregistered;
		}

		private void HandleRegisteredComponents()
		{
			foreach (GraphNode node in Kernel.GraphNodes)
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