using System;
using System.Collections.Generic;
using System.Text;
using N2.Details;
using N2.Integrity;

namespace N2.Templates.Survey.Items
{
	[WithEditableTitle("Question", 10)]
	[RestrictParents(typeof(ISurvey))]
	[AllowedZones("Questions", "")]
	public abstract class Question : Templates.Items.AbstractItem
	{
	}
}
