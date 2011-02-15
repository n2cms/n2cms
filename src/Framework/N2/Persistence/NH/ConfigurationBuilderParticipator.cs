using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Persistence.NH
{
	/// <summary>
	/// Modifies the NHibernate configuration object before the session factory is built.
	/// </summary>
	public abstract class ConfigurationBuilderParticipator
	{
		public abstract void AlterConfiguration(NHibernate.Cfg.Configuration cfg);
	}
}
