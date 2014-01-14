using System;

namespace N2.Definitions.Runtime
{
    public class ViewTemplateSource
    {
        public string ControllerName { get; set; }
        public Type ModelType { get; set; }
        public string ViewFileExtension { get; set; }
    }
}
