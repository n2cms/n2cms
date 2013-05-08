using System.Configuration;

namespace N2.Configuration
{
	public class SectionGroup : ConfigurationSectionGroup
	{
		public EngineSection Engine
		{
			get { return (EngineSection)Sections["engine"]; }
		}
		public DatabaseSection Database
		{
			get { return (DatabaseSection)Sections["database"]; }
		}
		public HostSection Host
		{
			get { return (HostSection)Sections["host"]; }
		}
		public EditSection Edit
		{
			get { return (EditSection)Sections["edit"]; }
		}
		public WebSection Web
		{
			get { return (WebSection)Sections["web"]; }
		}
	}
}
