using System;

namespace N2.Configuration
{
	[Obsolete("Replaced by string config")]
	public enum SearchIndexType
	{
		Database,
		Lucene,
		RemoteServer
	}
}
