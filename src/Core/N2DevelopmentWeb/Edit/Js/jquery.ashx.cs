using System;
using System.Collections;
using System.Data;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;

namespace N2.Edit.Js
{
	public class jquery : DirectoryCompiler
	{
		public override string FolderUrl
		{
			get { return "~/Edit/Js/jquery.ui"; }
		}
	}

}
