using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace N2.Edit.Api
{
	public interface ITemplateInfoProvider
	{
		string Area { get; }
		IEnumerable<TemplateInfo> GetTemplates();
	}
}
