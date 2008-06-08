#region License
/* Copyright (C) 2007 Cristian Libardo
 *
 * This is free software; you can redistribute it and/or modify it
 * under the terms of the GNU Lesser General Public License as
 * published by the Free Software Foundation; either version 2.1 of
 * the License, or (at your option) any later version.
 */
#endregion

using System.Configuration;
using System;

namespace N2.Configuration
{
    /// <summary>
    /// May be used in the future.
    /// </summary>
	[Obsolete]
	public class N2ConfigurationSectionHandler : ConfigurationSectionGroup
    {
		public N2ConfigurationSectionHandler()
		{
			throw new ConfigurationErrorsException("The N2ConfigurationSectionHandler has been deprecated. Please use 'N2.Configuration.SectionGroup, N2' instead.");
		}
	}
}
