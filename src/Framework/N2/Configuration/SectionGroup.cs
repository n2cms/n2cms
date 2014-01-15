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
        // AKA Web, TODO: Handle upgrade and rename
        public HostSection Host
        {
            get { return (HostSection)Sections["host"]; }
        }
        // AKA Management, TODO: Handle upgrade and rename
        public EditSection Edit
        {
            get { return (EditSection)Sections["edit"]; }
        }
    }
}
