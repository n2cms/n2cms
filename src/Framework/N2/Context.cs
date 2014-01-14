#region License
/* Copyright (C) 2007-2008 Cristian Libardo
 *
 * This is free software; you can redistribute it and/or modify it
 * under the terms of the GNU Lesser General Public License as
 * published by the Free Software Foundation; either version 2.1 of
 * the License, or (at your option) any later version.
 */
#endregion

using System;
using System.Configuration;
using System.Runtime.CompilerServices;
using System.Security;
using System.Web.Configuration;
using N2.Configuration;
using N2.Engine;

namespace N2
{
    /// <summary>
    /// Provides access to the singleton instance of the N2 CMS engine.
    /// </summary>
    public class Context
    {
        private static readonly Logger<Context> Logger;

        #region Initialization Methods
        /// <summary>Initializes a static instance of the N2 factory.</summary>
        /// <param name="forceRecreate">Creates a new factory instance even though the factory has been previously initialized.</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static IEngine Initialize(bool forceRecreate)
        {
            if (Singleton<IEngine>.Instance == null || forceRecreate)
            {
                try
                {
                    Logger.Info("Constructing engine");
                }
                catch (SecurityException)
                {
                    Engine.Logger.WriterFactory = t => new LogWriterBase();
                }
                var engine = CreateEngineInstance();
                Logger.InfoFormat("Initializing engine {0}", engine);
                engine.Initialize();
                Replace(engine);
            }
            return Singleton<IEngine>.Instance;
        }

        [Obsolete("Use Context.Replace(IEngine)", false)]
        public static void Initialize(IEngine engine)
        {
            Singleton<IEngine>.Instance = engine;
        }

        /// <summary>Sets the static engine instance to the supplied engine. Use this method to supply your own engine implementation.</summary>
        /// <param name="engine">The engine to use.</param>
        /// <remarks>Only use this method if you know what you're doing.</remarks>
        public static void Replace(IEngine engine)
        {
            Singleton<IEngine>.Instance = engine;
        }

        private static System.Configuration.Configuration GetConfiguration()
        {
            try
            {
                return System.Web.Hosting.HostingEnvironment.IsHosted
                    ? WebConfigurationManager.OpenWebConfiguration("~")
                    : ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            }
            catch (Exception ex)
            {
                Engine.Logger.Warn("Error reading configuration. This has happened when running a web site project in a virtual directory (reason unknown). " + ex);
                return null;
            }
        }

        /// <summary>Creates a factory instance and adds a http application injecting facility.</summary>
        /// <returns>A new factory.</returns>
        public static IEngine CreateEngineInstance()
        {
            var config = ConfigurationManager.GetSection("n2/engine") as EngineSection;
            if (config != null && !string.IsNullOrEmpty(config.EngineType))
            {
                Type engineType = Type.GetType(config.EngineType);
                if(engineType == null)
                    throw new ConfigurationErrorsException("The type '" + config.EngineType + "' could not be found. Please check the configuration at /configuration/n2/engine[@engineType] or check for missing assemblies.");
                if(!typeof(IEngine).IsAssignableFrom(engineType))
                    throw new ConfigurationErrorsException("The type '" + config.EngineType + "' doesn't implement 'N2.Engine.IEngine' and cannot be configured in /configuration/n2/engine[@engineType] for that purpose.");
                return Activator.CreateInstance(engineType) as IEngine;
            }
            if (config != null && !string.IsNullOrEmpty(config.ContainerType))
            {
                Type containerType = Type.GetType(config.ContainerType);
                if (containerType == null)
                    throw new ConfigurationErrorsException("The type '" + config.ContainerType + "' could not be found. Please check the configuration at /configuration/n2/engine[@containerType] or check for missing assemblies.");
                if (!typeof(IServiceContainer).IsAssignableFrom(containerType))
                    throw new ConfigurationErrorsException("The type '" + config.ContainerType + "' doesn't implement 'N2.Engine.IServiceContainer' and cannot be configured in /configuration/n2/engine[@containerType] for that purpose.");
                var container = Activator.CreateInstance(containerType) as IServiceContainer;
                return new ContentEngine(container);
            }
            return new ContentEngine();
        }

        #endregion

        #region Properties: Persister, Definitions, Integrity, UrlParser, CurrentPage

        /// <summary>Gets the singleton N2 engine used to access N2 services.</summary>
        public static IEngine Current
        {
            get
            {
                if (Singleton<IEngine>.Instance == null)
                {
                    Initialize(false);
                }
                return Singleton<IEngine>.Instance;
            }
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

        /// <summary>Gets the url parser responsible of mapping managementUrls to items and back again.</summary>
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
