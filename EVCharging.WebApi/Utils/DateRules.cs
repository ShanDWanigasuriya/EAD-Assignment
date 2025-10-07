namespace EVCharging.WebApi.Utils
{
    public static class DateRules
    {
        public static bool IsWithin7Days(DateTime startUtc, DateTime nowUtc)
            => startUtc <= nowUtc.AddDays(7) && startUtc >= nowUtc;

        public static bool IsAtLeast12HoursBefore(DateTime startUtc, DateTime nowUtc)
            => startUtc >= nowUtc.AddHours(12);
    }
}
