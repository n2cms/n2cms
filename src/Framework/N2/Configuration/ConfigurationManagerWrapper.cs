using System.Configuration;
using System.Collections.Generic;

namespace N2.Configuration
{
    /// <summary>
    /// A testable configuration manager wrapper.
    /// </summary>
    public class ConfigurationManagerWrapper
    {
        readonly string sectionGroup;
        private ContentSectionTable sections;

        public ConfigurationManagerWrapper()
            : this("n2")
        {
        }
        public ConfigurationManagerWrapper(string sectionGroup)
        {
            this.sectionGroup = sectionGroup;
        }

        public ContentSectionTable Sections
        {
            get { return sections ?? (sections = GetOrCreateSections()); }
            set { sections = value; }
        }

        private ContentSectionTable GetOrCreateSections()
        {
            return new ContentSectionTable(
                GetContentSection<HostSection>("host") ?? new HostSection(),
                GetContentSection<EngineSection>("engine") ?? new EngineSection(),
                GetContentSection<DatabaseSection>("database") ?? new DatabaseSection(),
                GetContentSection<EditSection>("edit") ?? new EditSection());
        }

        public virtual T GetSection<T>(string sectionName, bool required = true) where T : ConfigurationSection, new()
        {
            object section = ConfigurationManager.GetSection(sectionName);
            if (section == null)
            {
                if (required)
                    throw new ConfigurationErrorsException("Missing configuration section at '" + sectionName + "'");
                else
                    return new T();
            }

            T contentSection = section as T;
            if (contentSection == null) throw new ConfigurationErrorsException("The configuration section at '" + sectionName + "' is of type '" + section.GetType().FullName + "' instead of '" + typeof(T).FullName + "' which is required.");
            return contentSection;
        }

        public virtual T GetContentSection<T>(string relativeSectionName, bool required = true) where T : ConfigurationSectionBase, new()
        {
            return GetSection<T>(sectionGroup + "/" + relativeSectionName, required);
        }

        public virtual ConnectionStringsSection GetConnectionStringsSection()
        {
            return GetSection<ConnectionStringsSection>("connectionStrings");
        }

        public virtual string GetConnectionString()
        {
            string connectionStringName = GetContentSection<DatabaseSection>("database").ConnectionStringName;
			var element = GetConnectionStringsSection().ConnectionStrings[connectionStringName];
			if (element == null)
				throw new ConfigurationErrorsException("The connection string '" + connectionStringName + "' referenced by n2/database[connectionStringName] was not found.");
			return element.ConnectionString;
        }


        /// <summary>
        /// Keeps references to used config sections.
        /// </summary>
        public class ContentSectionTable
        {
            public ContentSectionTable(HostSection web, EngineSection engine, DatabaseSection database, EditSection management)
            {
                Web = web;
                Engine = engine;
                Database = database;
                Management = management;
            }

            public HostSection Web { get; protected set; }
            public EngineSection Engine { get; protected set; }
            public DatabaseSection Database { get; protected set; }
            public EditSection Management { get; protected set; }
        }

        #region IAutoStart Members

        public void Start()
        {
            Sections = GetOrCreateSections();
        }

        public void Stop()
        {
            Sections = null;
        }

        #endregion

        public virtual string[] GetComponentConfigurationKeys()
        {
            List<string> configurationKeys = new List<string>();

            Sections.Database.ApplyComponentConfigurationKeys(configurationKeys);
            Sections.Management.ApplyComponentConfigurationKeys(configurationKeys);
            Sections.Web.ApplyComponentConfigurationKeys(configurationKeys);
            Sections.Engine.ApplyComponentConfigurationKeys(configurationKeys);
            
            return configurationKeys.ToArray();
        }
    }
}
