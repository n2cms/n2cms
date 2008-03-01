using System;
using System.Collections.Specialized;
using N2.Web;

namespace N2.Parts
{
	/// <summary>
	/// Ajax service that adds itself to the ajax request dispatecher upon start.
	/// </summary>
	public abstract class PartsAjaxService : IAjaxService, Castle.Core.IStartable
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

		public abstract NameValueCollection Handle(NameValueCollection request);

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
