using System;
using N2.Definitions;
using N2.Installation;

namespace N2
{
    /// <summary>
    /// Defines a page (ASPX) available to the CMS and provides a way to define 
    /// useful meta-data. Unlike <see cref="DefinitionAttribute"/> this attribute
    /// makes more assumptions about the item beeing defined and provides more
    /// meta-data options.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class PageDefinitionAttribute : AbstractDefinition
    {
        /// <summary>Instructs the installation screen how to treat this definition during installation.</summary>
        public InstallerHint InstallerVisibility { get; set; }

		public PageDefinitionAttribute(string title)
            :this()
        {
            Title = title;
        }

        public PageDefinitionAttribute()
        {
            InstallerVisibility = InstallerHint.Default;
            IsPage = true;
        }

        public override void Refine(ItemDefinition currentDefinition)
        {
			if (InstallerVisibility != InstallerHint.Default)
				currentDefinition.Installer = InstallerVisibility;

            base.Refine(currentDefinition);
        }
    }
}
