// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GenericEntry.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2006
// </copyright>
// <summary>
//   Defines the GenericEntry type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;

namespace Reimers.Google.Analytics
{
    /// <summary>
    /// Defines a generic report entry.
    /// </summary>
    public class GenericEntry
    {
        #region Fields

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the report dimensions.
        /// </summary>
        public List<KeyValuePair<Dimension, string>> Dimensions { get; set; }

        /// <summary>
        /// Gets or sets the report metrics.
        /// </summary>
        public List<KeyValuePair<Metric, string>> Metrics { get; set; }

        #endregion
    }
}
