namespace N2.Tests.Definitions.Items
{
	[N2.Integrity.RestrictParents(typeof(DefinitionNewsPage))]
	[N2.Integrity.AllowedZones("", "Right")]
	public class DefinitionNewsList : DefinitionRightColumnPart
	{
		public override bool IsPage
		{
			get
			{
				return string.IsNullOrEmpty(ZoneName) ? true : false;
			}
		}
	}
}
