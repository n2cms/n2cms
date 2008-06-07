using System;
using System.Collections.Generic;
using System.Text;
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
	}
}
