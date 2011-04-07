using System;
using System.Configuration;

namespace N2.Configuration
{
	/// <summary>
	/// A testable configuration manager wrapper.
	/// </summary>
	public class ConfigurationManagerWrapper
	{
		readonly string sectionGroup;

		public ConfigurationManagerWrapper()
			: this("n2")
		{
		}
		public ConfigurationManagerWrapper(string sectionGroup)
		{
			this.sectionGroup = sectionGroup;
		}

		public ContentSectionTable Sections { get; set; }

		public virtual T GetSection<T>(string sectionName) where T : ConfigurationSection
		{
			object section = ConfigurationManager.GetSection(sectionName);
			if (section == null) throw new ConfigurationErrorsException("Missing configuration section at '" + sectionName + "'");
			T contentSection = section as T;
			if (contentSection == null) throw new ConfigurationErrorsException("The configuration section at '" + sectionName + "' is of type '" + section.GetType().FullName + "' instead of '" + typeof(T).FullName + "' which is required.");
			return contentSection;
		}

		public virtual T GetContentSection<T>(string relativeSectionName) where T : ConfigurationSectionBase
		{
			return GetSection<T>(sectionGroup + "/" + relativeSectionName);
		}

		public virtual ConnectionStringsSection GetConnectionStringsSection()
		{
			return GetSection<ConnectionStringsSection>("connectionStrings");
		}

		public virtual string GetConnectionString()
		{
			string connectionStringName = GetContentSection<DatabaseSection>("database").ConnectionStringName;
			return GetConnectionStringsSection().ConnectionStrings[connectionStringName].ConnectionString;
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
			Sections = new ContentSectionTable(
				GetContentSection<HostSection>("host"),
				GetContentSection<EngineSection>("engine"),
				GetContentSection<DatabaseSection>("database"),
				GetContentSection<EditSection>("edit"));
		}

		public void Stop()
		{
			Sections = null;
		}

		#endregion
	}
}
