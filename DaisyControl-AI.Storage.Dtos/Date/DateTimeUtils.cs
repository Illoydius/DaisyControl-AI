namespace DaisyControl_AI.Storage.Dtos.Date
{
    public static class DateTimeUtils
    {
        public enum Unit
        {
            Nanoseconds,
            Microseconds,
            Milliseconds,
            Seconds,
            Minutes,
            Hours,
            Days
        }

        private const int NanoSecondToTicksDividingFactor = 100;
        private const int MilliSecondToMicroSecondMultiplyingFactor = 1000;
        private const int MilliSecondToNanoSecondMultiplyingFactor = 1000000;

        /// <summary>
        /// More extensive ToUnixTime that can handle more unit types than generic one from ubiservices.
        /// </summary>
        public static long ToUnixTime(this DateTime datetime, Unit unit = Unit.Milliseconds)
        {
            var currentDateTime = datetime;
            if (currentDateTime.Kind != DateTimeKind.Utc)
                currentDateTime = currentDateTime.ToUniversalTime();

            var oldestDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan timeSpan = currentDateTime - oldestDateTime;

            var totalNanoseconds = timeSpan.TotalMilliseconds * MilliSecondToNanoSecondMultiplyingFactor;
            var totalMicroseconds = timeSpan.TotalMilliseconds * MilliSecondToMicroSecondMultiplyingFactor;

            return unit switch
            {
                Unit.Nanoseconds => totalNanoseconds < 0 ? 0 : (long)totalNanoseconds,
                Unit.Microseconds => totalMicroseconds < 0 ? 0 : (long)totalMicroseconds,
                Unit.Milliseconds => timeSpan.TotalMilliseconds < 0 ? 0 : (long)timeSpan.TotalMilliseconds,
                Unit.Seconds => timeSpan.TotalSeconds < 0 ? 0 : (long)timeSpan.TotalSeconds,
                Unit.Minutes => timeSpan.TotalMinutes < 0 ? 0 : (long)timeSpan.TotalMinutes,
                Unit.Hours => timeSpan.TotalHours < 0 ? 0 : (long)timeSpan.TotalHours,
                Unit.Days => timeSpan.TotalDays < 0 ? 0 : (long)timeSpan.TotalDays,
                _ => throw new ArgumentOutOfRangeException(nameof(unit), unit, $"The Unit [{unit}] to convert to UnixTime wasn't handled."),
            };
        }

        public static DateTime FromUnixTime(this long self, Unit unit = Unit.Seconds)
        {
            var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            return unit switch
            {
                Unit.Milliseconds => dateTime.AddMilliseconds(self),
                Unit.Seconds => dateTime.AddSeconds(self),
                Unit.Minutes => dateTime.AddMinutes(self),
                Unit.Hours => dateTime.AddHours(self),
                Unit.Days => dateTime.AddDays(self),
                _ => throw new ArgumentOutOfRangeException("unit", unit, null),
            };
        }
    }
}
