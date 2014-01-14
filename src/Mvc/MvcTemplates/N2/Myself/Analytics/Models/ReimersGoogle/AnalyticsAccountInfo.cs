// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AnalyticsAccountInfo.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2006
// </copyright>
// <summary>
//   Defines the AnalyticsAccountInfo type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Reimers.Google.Analytics
{
    /// <summary>
    /// Holds data about a Google Analytics account.
    /// </summary>
    public class AnalyticsAccountInfo
    {
        #region Properties

        /// <summary>
        /// Gets or sets the ID.
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// Gets or sets the account title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the account ID.
        /// </summary>
        public int AccountID { get; set; }

        /// <summary>
        /// Gets or sets the account name.
        /// </summary>
        public string AccountName { get; set; }

        /// <summary>
        /// Gets or sets the profile ID.
        /// </summary>
        public int ProfileID { get; set; }

        /// <summary>
        /// Gets or sets the web property ID.
        /// </summary>
        public string WebPropertyID { get; set; }

        #endregion
    }
}
