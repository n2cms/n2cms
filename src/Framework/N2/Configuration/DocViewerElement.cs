using NHibernate.Criterion;
using NHibernate.Mapping;
using System.Configuration;
using System.Linq;
using System.Runtime.CompilerServices;

namespace N2.Configuration
{
    [ConfigurationCollection(typeof(DocViewerElement))]
    public class DocViewerElementCollection : LazyRemovableCollection<DocViewerElement>
    {
        public DocViewerElementCollection()
        {
            AddDefault(new DocViewerElement("docx,docm,dotm,dotx,xlsx,xlsb,xls,xlsm,pptx,ppsx,ppt,pps,pptm,potm,ppam,potx,ppsm", "http://view.officeapps.live.com/op/view.aspx?src={abs}"));
            AddDefault(new DocViewerElement(".TIFF,.CSS,.HTML,.PHP,.C,.CPP,.H,.HPP,.JS,.PDF,.PAGES,.AI,.PSD,.TIFF,.DXF,.SVG,.EPS,.PS,.TTF,.XPS", "http://docs.google.com/viewer?url={abs}"));
        }

        [ConfigurationProperty("enabled", DefaultValue = true)]
        public bool Enabled
        {
            get { return (bool)base["enabled"]; }
            set { base["enabled"] = value; }
        }

        public DocViewerElement GetElementForFilename(string filename)
        {
            var li = filename.LastIndexOf('.');
            if (li > 0)
                filename = filename.Substring(li + 1).ToUpper();

            var query = from item in AllElements
                        where item.Enabled
                        let extensions = System.Array.ConvertAll(item.FileExtensionsArray, ext => ext.Trim('.', ' ')) // convert to uppercase and trim leading .'s
                        where extensions.Contains(filename)
                        select item;

            return query.FirstOrDefault();
        }
    }

    /// <summary>
    /// Configuration related to document viewer extension.
    /// </summary>
    public class DocViewerElement : ConfigurationElement, IIdentifiable
    {
        public DocViewerElement()
        {
            Enabled = true;
        }

        public DocViewerElement(string extensions, string url)
        {
            // ReSharper disable once RedundantThisQualifier
            FileExtensions = extensions;
            Url = url;
            Enabled = true;
        }

        [ConfigurationProperty("enabled", DefaultValue = true)]
        public bool Enabled
        {
            get { return (bool)base["enabled"]; }
            set { base["enabled"] = value; }
        }

        /// <summary>
        /// The URL to the document viewer. Put {abs} and {rel} for the absolute and/or relative URL of the document to be viewed. 
        /// For URL Escaping, put {abs_encoded} and {rel_encoded} instead. 
        /// </summary>
        [ConfigurationProperty("url")]
        public string Url
        {
            get { return (string)base["url"]; }
            set { base["url"] = value; }
        }

        /// <summary>
        /// The maximum size of any document to be loaded by the Document Viewer.
        /// </summary>
        [ConfigurationProperty("maxsize", DefaultValue = "")]
        public string MaxSize
        {
            get { return (string)base["maxsize"]; }
            set { base["maxsize"] = value; }
        }

        /// <summary>
        /// The file extensions that the Document Viewer will handle.
        /// </summary>
        [ConfigurationProperty("extensions")]
        public string FileExtensions
        {
            get { return ((string)base["extensions"]).ToUpper(); }
            set { base["extensions"] = value.ToUpper(); }
        }

        public string[] FileExtensionsArray
        {
            get { return FileExtensions.Split(','); }
        }


        public object ElementKey
        {
            get { return FileExtensions; }
            set { FileExtensions = value.ToString(); }
        }
    }
}
