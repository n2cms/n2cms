using System;
using System.Collections;
using System.Data;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Collections.Generic;

namespace N2.Edit.Js
{
	public class tinymce : DirectoryCompiler
	{
		public override string FolderUrl
		{
			get { return "~/Edit/Js/tiny_mce"; }
		}

		protected override IEnumerable<string> GetFiles(HttpContext context)
		{
			return base.GetFiles(context);
		}
	}
}
