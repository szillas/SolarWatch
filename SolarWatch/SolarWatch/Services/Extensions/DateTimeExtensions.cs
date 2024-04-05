namespace SolarWatch.Services.Extensions;

public static class DateTimeExtensions
{
    public static DateTime ParseDateOrDefaultToToday(this string? date)
    {
        if (string.IsNullOrEmpty(date))
        {
            return DateTime.Today;
        }
        else
        {
            if (!DateTime.TryParse(date, out var dateTime))
            {
                throw new FormatException("Date format is not correct!");
            }

            return dateTime;
        }
    }
}