namespace ScreenTime.utils
{
    internal class DateTimeUtils
    {
        public static string CurrentDate()
        {
            return DateTime.Now.Date.ToString("d");
        }
    }
}
