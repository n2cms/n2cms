// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CityReport.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2006
// </copyright>
// <summary>
//   Analytics report holding information about visits on city level.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;

namespace Reimers.Google.Analytics.Reports
{
    /// <summary>
    /// Analytics report holding information about visits on city level.
    /// </summary>
    public class CityReport
    {
        #region Fields

        /// <summary>
        /// Backing field for the Date property.
        /// </summary>
        private DateTime _date = DateTime.MinValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="CityReport"/> class.
        /// </summary>
        public CityReport()
        {
            City = string.Empty;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets city of the visit.
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// Gets or sets latitude of the city.
        /// </summary>
        public double Latitude { get; set; }

        /// <summary>
        /// Gets or sets longitude of the city.
        /// </summary>
        public double Longitude { get; set; }

        /// <summary>
        /// Gets or sets amount of visits.
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// Gets or sets the date of the visit.
        /// </summary>
        public DateTime Date
        {
            get { return _date; }
            set { _date = value.Date; }
        }

        #endregion
    }
}
