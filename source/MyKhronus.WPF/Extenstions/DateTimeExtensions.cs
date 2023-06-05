namespace MyKhronus.WPF.Extenstions;

public static class DateTimeExtensions
{

    public static DateTime GetMondayDateOfWeek(this DateTime date)
    {
        var dateOfWeek = (int)date.DayOfWeek;

        if (dateOfWeek == 1)
        {
            return date;
        }

        if (dateOfWeek == 0)
        {
            return date.AddDays(1);
        }

        return date.AddDays(-dateOfWeek + 1);
    }

}
