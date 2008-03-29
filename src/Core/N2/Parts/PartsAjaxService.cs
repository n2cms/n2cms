using System;
using System.Collections.Specialized;
using N2.Web;
using System.Text;

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

		public string Handle(NameValueCollection request)
		{
			NameValueCollection response = HandleRequest(request);
			return ToJson(response);
		}

		protected string ToJson(NameValueCollection response)
		{
			StringBuilder sb = new StringBuilder();
			using (new N2.Persistence.NH.Finder.StringWrapper(sb, "{", "}"))
			{
				sb.AppendFormat("{0}: {1}", "error", "false");
				foreach (string key in response.Keys)
				{
					sb.AppendFormat(",{0}: '{1}'", key, response[key]);
				}
			}
			return sb.ToString();
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
