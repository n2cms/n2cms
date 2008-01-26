using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Reflection;

namespace N2.Engine
{
	/// <summary>
	/// Provides information about types in the current web application. 
	/// Optionally this class can look at all assemblies in the bin folder.
	/// </summary>
	public class WebAppTypeFinder : AppDomainTypeFinder
	{
		#region Private Fields
		private Web.IWebContext webContext;
		private bool ensureBinFolderAssembliesLoaded = true;
		private bool binFolderAssembliesLoaded = false; 
		#endregion

		#region Constructor
		public WebAppTypeFinder(Web.IWebContext webContext)
		{
			this.webContext = webContext;
		}

		public WebAppTypeFinder(Web.IWebContext webContext, bool ensureBinFolderAssembliesLoaded)
		{
			this.webContext = webContext;
			this.ensureBinFolderAssembliesLoaded = ensureBinFolderAssembliesLoaded;
		}
		#endregion

		#region Properties
		/// <summary>Gets or sets wether assemblies in the bin folder of the web application should be specificly checked for beeing loaded on application load. This is need in situations where plugins need to be loaded in the AppDomain after the application been reloaded.</summary>
		public bool EnsureBinFolderAssembliesLoaded
		{
			get { return ensureBinFolderAssembliesLoaded; }
			set { ensureBinFolderAssembliesLoaded = value; }
		}
	
		#endregion

		#region Methods
		public override IList<Assembly> GetAssemblies()
		{
			if (EnsureBinFolderAssembliesLoaded && !binFolderAssembliesLoaded)
			{
				binFolderAssembliesLoaded = true;
				LoadMatchingAssemblies(webContext.MapPath("~/bin"));
			}

			return base.GetAssemblies();
		} 
		#endregion
	}
}
