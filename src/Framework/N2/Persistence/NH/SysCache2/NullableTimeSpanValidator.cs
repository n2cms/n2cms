using System;
using System.Configuration;

namespace NHibernate.Caches.SysCache2
{
    /// <summary>
    /// Timespan validator that can accept a null value as valid input
    /// </summary>
    public class NullableTimeSpanValidator : TimeSpanValidator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NullableTimeSpanValidator"/> class.
        /// </summary>
        /// <param name="minValue">A <see cref="T:System.TimeSpan"></see> object specifying the minimum time allowed to pass validation.</param>
        /// <param name="maxValue">A <see cref="T:System.TimeSpan"></see> object specifying the maximum time allowed to pass validation.</param>
        public NullableTimeSpanValidator(TimeSpan minValue, TimeSpan maxValue) : this(minValue, maxValue, false, 0L) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="NullableTimeSpanValidator"/> class.
        /// </summary>
        /// <param name="minValue">A <see cref="T:System.TimeSpan"></see> object specifying the minimum time allowed to pass validation.</param>
        /// <param name="maxValue">A <see cref="T:System.TimeSpan"></see> object specifying the maximum time allowed to pass validation.</param>
        /// <param name="rangeIsExclusive">A <see cref="T:System.Boolean"></see> value specifying whether the validation range is exclusive.</param>
        public NullableTimeSpanValidator(TimeSpan minValue, TimeSpan maxValue, bool rangeIsExclusive)
            : this(minValue, maxValue, rangeIsExclusive, 0L) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="NullableTimeSpanValidator"/> class.
        /// </summary>
        /// <param name="minValue">A <see cref="T:System.TimeSpan"></see> object specifying the minimum time allowed to pass validation.</param>
        /// <param name="maxValue">A <see cref="T:System.TimeSpan"></see> object specifying the maximum time allowed to pass validation.</param>
        /// <param name="rangeIsExclusive">A <see cref="T:System.Boolean"></see> value specifying whether the validation range is exclusive.</param>
        /// <param name="resolutionInSeconds">An <see cref="T:System.Int64"></see> value that specifies a number of seconds.</param>
        public NullableTimeSpanValidator(TimeSpan minValue, TimeSpan maxValue, bool rangeIsExclusive, long resolutionInSeconds)
            : base(minValue, maxValue, rangeIsExclusive, resolutionInSeconds) {}

        /// <summary>
        /// Determines whether the type of the object can be validated.
        /// </summary>
        /// <param name="type">The type of object.</param>
        /// <returns>
        /// true if the type parameter matches a nullable <see cref="T:System.TimeSpan"></see> value; otherwise, false.
        /// </returns>
        public override bool CanValidate(System.Type type)
        {
            return (type == typeof (TimeSpan?));
        }
    }
}
