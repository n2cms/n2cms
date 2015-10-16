using N2.Edit.Collaboration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace N2.Management.Statistics
{
	[MessageSource]
	public class StatisticsErrorMessageProvider : MessageSourceBase
	{
		public override IEnumerable<CollaborationMessage> GetMessages(N2.Edit.Collaboration.CollaborationContext context)
		{
			if (SqlBucketRepository.AdoExceptionDetected.HasValue)
				yield return new CollaborationMessage { Title = "Statistics error", Updated = SqlBucketRepository.AdoExceptionDetected.Value, Alert = false, Text = "An error was logged when saving or retrieving statistics. <a href='/n2/installation/upgrade.aspx'>An upgrade might be required to set up statistics. <b class='ico fa fa-external-link'></b></a>", RequiredPermission = Security.Permission.Administer };
		}
	}
}