namespace N2.Edit.Wizard
{
    public class Settings
    {
        private string name = string.Empty;
        private string zoneName = null;
        private string title = string.Empty;

        public Settings()
        {
        }
        public Settings(string name, string zoneName, string title)
        {
            this.name = name;
            this.zoneName = zoneName;
            this.title = title;
        }

        public string Title
        {
            get { return title; }
            set { title = value; }
        }

        public string ZoneName
        {
            get { return zoneName; }
            set { zoneName = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }
    }
}
