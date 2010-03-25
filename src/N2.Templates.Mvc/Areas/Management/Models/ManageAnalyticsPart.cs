using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using N2.Integrity;
using N2.Definitions;
using N2.Details;
using Reimers.Google.Analytics;

namespace N2.Templates.Mvc.Areas.Management.Models
{
	[PartDefinition, RestrictParents(typeof(IRootPage))]
	public class ManageAnalyticsPart : AnalyticsPartBase
	{
		[EditableTextBox("Token", 100)]
		public virtual string Token
		{
			get { return (string)GetDetail("Token"); }
			set { SetDetail("Token", value, string.Empty); }
		}

		[EditableTextBox("AccountID", 100)]
		public virtual int AccountID
		{
			get { return GetDetail("AccountID", 0); }
			set { SetDetail("AccountID", value, 0); }
		}

		[EditableTextBox("Username", 110)]
		public virtual string Username
		{
			get { return (string)GetDetail("Username"); }
			set { SetDetail("Username", value, string.Empty); }
		}

		[EditableTextBox("Password", 120)]
		public virtual string Password
		{
			get { return (string)GetDetail("Password"); }
			set { SetDetail("Password", value, string.Empty); }
		}

		public virtual IEnumerable<Dimension> Dimensions
		{
			get { return GetDetailCollection("Dimensions", true).OfType<int>().Select(i => (Dimension)i); }
			set { GetDetailCollection("Dimensions", true).Replace(value.Select(d => (int)d)); }
		}

		public virtual IEnumerable<Metric> Metrics
		{
			get { return GetDetailCollection("Metrics", true).OfType<int>().Select(i => (Metric)i); }
			set { GetDetailCollection("Metrics", true).Replace(value.Select(d => (int)d)); }
		}

	}
}
