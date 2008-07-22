#region License
/* Copyright (C) 2007-2008 Cristian Libardo
 *
 * This is free software; you can redistribute it and/or modify it
 * under the terms of the GNU Lesser General Public License as
 * published by the Free Software Foundation; either version 2.1 of
 * the License, or (at your option) any later version.
 */
#endregion

using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Web;
using System;
using System.ComponentModel;
using System.Configuration;
using System.Web.Configuration;
using System.Security;
using N2.Engine;
using N2.Engine.MediumTrust;

namespace N2
{
    /// <summary>
	/// Provides access to the singleton instance of the N2 CMS engine.
	/// </summary>
    public class Context
    {
		#region Private Fields
		private static Engine.IEngine instance = null;
		#endregion

		#region Initialization Methods
    	/// <summary>Initializes a static instance of the N2 factory.</summary>
		/// <param name="forceRecreate">Creates a new factory instance even though the factory has been previously initialized.</param>
		[MethodImpl(MethodImplOptions.Synchronized)]
		public static IEngine Initialize(bool forceRecreate)
		{
			if (instance == null || forceRecreate)
			{
                Debug.WriteLine("Constructing " + DateTime.Now);
				instance = CreateEngineInstance();
                Debug.WriteLine("Initializing " + DateTime.Now);
                instance.Initialize();
			}
			return instance;
		}

		/// <summary>Sets the static factory instance to the supplied factory. Use this method to supply your own factory implementation.</summary>
		/// <param name="engine">The factory to use.</param>
		/// <remarks>Only use this method if you know what you're doing.</remarks>
		public static void Initialize(Engine.IEngine engine)
		{
			instance = engine;
		}

		/// <summary>Creates a factory instance and adds a http application injecting facility.</summary>
		/// <returns>A new factory.</returns>
		public static Engine.IEngine CreateEngineInstance()
		{
            try
            {
                var cfg = System.Web.Hosting.HostingEnvironment.IsHosted
                    ? WebConfigurationManager.OpenWebConfiguration("~")
                    : ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

                return new Engine.ContentEngine(cfg);
            }
            catch (SecurityException)
            {
                Trace.TraceInformation("Caught SecurityException, reverting to MediumTrustEngine.");
                return new MediumTrustEngine();
            }
            catch (ConfigurationErrorsException ex)
            {
                Trace.TraceWarning("Caught InvalidOperationException. This can happen when running a web site project in a virtual directory.");
                return new Engine.ContentEngine();
            }
		}

		#endregion

		#region Properties: Persister, Definitions, Integrity, UrlParser, CurrentPage

		/// <summary>Gets the singleton N2 engine used to access N2 services.</summary>
		public static Engine.IEngine Current
		{
			get
			{
				if (instance == null)
				{
					Initialize(false);
					if (HttpContext.Current != null)
					{
						instance.Attach(HttpContext.Current.ApplicationInstance);
					}
				}
				return instance;
			}
		}

		/// <summary>Access to the singleton N2 engine. This property has been deprecated in favor for 'Current'.</summary>
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("Name changed to Current", true)]
		public static Engine.IEngine Instance
		{
			get { return Current; }
		}

		/// <summary>Gets N2 persistence manager used for database persistence of content.</summary>
		public static Persistence.IPersister Persister
		{
			get { return Current.Persister; }
		}

		/// <summary>Gets N2 definition manager</summary>
		public static Definitions.IDefinitionManager Definitions
		{
			get { return Current.Definitions; }
		}
        
		/// <summary>Gets N2 integrity manager </summary>
		public static Integrity.IIntegrityManager IntegrityManager
        {
			get { return Current.IntegrityManager; }
        }

		/// <summary>Gets N2 security manager responsible of item access checks.</summary>
		public static Security.ISecurityManager SecurityManager
		{
			get { return Current.SecurityManager; }
		}

		/// <summary>Gets the url parser responsible of mapping urls to items and back again.</summary>
		public static Web.IUrlParser UrlParser
		{
			get { return Current.UrlParser; }
		}

        /// <summary>Gets the current page. This is retrieved by the page querystring.</summary>
        public static ContentItem CurrentPage
        {
            get { return Current.UrlParser.CurrentPage; }
        }
        #endregion
	}
}
