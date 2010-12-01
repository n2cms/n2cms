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

		public ContentSectionTable Sections { get; protected set; }

		public virtual T GetSection<T>(string sectionName) where T : ConfigurationSection
		{
			return ConfigurationManager.GetSection(sectionName) as T;
		}

		public virtual T GetContentSection<T>(string relativeSectionName) where T : ConfigurationSectionBase
		{
			return GetSection<T>(sectionGroup + "/" + relativeSectionName) as T;
		}

		public virtual ConnectionStringsSection GetConnectionStringsSection()
		{
			return GetSection<ConnectionStringsSection>("connectionStrings");
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
