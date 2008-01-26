#region License

/* Copyright (C) 2006-2007 Cristian Libardo
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA 02111-1307, USA.
 */

#endregion

using System;
using System.Web;
using N2.Definitions;
using N2.Edit;
using N2.Integrity;
using N2.Persistence;
using N2.Security;
using N2.Web;

namespace N2.Engine
{
	/// <summary>
	/// Classes implementing this interface can serve as a portal for the 
	/// various services composing the N2 engine. Edit functionality, modules
	/// and implementations access most N2 functionality through this 
	/// interface.
	/// </summary>
	public interface IEngine
	{
		/// <summary>Gets the persistence manager responsible of storing content items to persistence medium (database).</summary>
		IPersister Persister { get; }

		/// <summary>Gets the url parser responsible of mapping urls to items and back again.</summary>
		IUrlParser UrlParser { get; }

		/// <summary>Gets the url rewriter responsible for passing request to the correct page template.</summary>
		IUrlRewriter Rewriter { get; }

		/// <summary>Gets the definition manager responsible of maintaining available item definitions.</summary>
		IDefinitionManager Definitions { get; }

		/// <summary>Gets the integrity manager used to control which items are allowed below which.</summary>
		IIntegrityManager IntegrityManager { get; }

		/// <summary>Gets the security manager responsible of controlling access to items.</summary>
		ISecurityManager SecurityManager { get; }

		/// <summary>Gets the class responsible for plugins in edit mode.</summary>
		IEditManager EditManager { get; }

		void InitializePlugIns();

		/// <summary>Attaches to the appropriate events for usage with ASP.NET.</summary>
		/// <param name="application">A recently initialized http application.</param>
		void Attach(HttpApplication application);

		/// <summary>Resolves a service configured for the factory.</summary>
		/// <typeparam name="T">The type of service to resolve.</typeparam>
		/// <returns>An instance of the resolved service.</returns>
		T Resolve<T>() where T : class;

		/// <summary>Resolves a named service configured for the factory.</summary>
		/// <param name="key">The name of the service to resolve.</param>
		/// <returns>An instance of the resolved service.</returns>
		object Resolve(string key);

		/// <summary>Registers a component in the IoC container.</summary>
		/// <param name="key">The name of the component.</param>
		/// <param name="classType">The type of component.</param>
		void AddComponent(string key, Type classType);

		/// <summary>Registers a component in the IoC container.</summary>
		/// <param name="key">The name of the component.</param>
		/// <param name="serviceType">The service interface of the component.</param>
		/// <param name="classType">The type of component.</param>
		void AddComponent(string key, Type serviceType, Type classType);

		/// <summary>Adds a "facility" to the IoC container. Unless this has been changed it's assumed that tihs is a <see cref="Castle.MicroKernel.IFacility"/>.</summary>
		/// <param name="key">The name of the facility.</param>
		/// <param name="facility">The facility instance.</param>
		void AddFacility(string key, object facility);

		/// <summary>Releases a component from the IoC container.</summary>
		/// <param name="instance">The component instance to release.</param>
		void Release(object instance);
	}
}