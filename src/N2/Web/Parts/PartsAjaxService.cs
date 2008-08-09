using System;
using System.Collections.Specialized;
using N2.Web;
using System.Text;
using Castle.Core;
using N2.Plugin;

namespace N2.Web.Parts
{
	/// <summary>
	/// Ajax service that adds itself to the ajax request dispatecher upon start.
	/// </summary>
	public abstract class PartsAjaxService : IAjaxService, IStartable, IAutoStart
	{
		private readonly AjaxRequestDispatcher dispatcher;

		public PartsAjaxService(AjaxRequestDispatcher dispatcher)
		{
			this.dispatcher = dispatcher;
		}

		public abstract string Name { get;}

		public bool RequiresEditAccess
		{
			get { return true; }
		}

		public string Handle(NameValueCollection request)
		{
			NameValueCollection response = HandleRequest(request);
            response["error"] = "false";
			return Json.ToObject(response);
		}

		public abstract NameValueCollection HandleRequest(NameValueCollection request);

		public void Start()
		{
			dispatcher.AddHandler(this);
		}

		public void Stop()
		{
			dispatcher.RemoveHandler(this);
		}
	}
}
