using System;

namespace Laika.ExtendBundle
{
    public static class ExtendDateTime
    {
        public static string GetLogDateTimeString(this DateTime date)
        {
            return string.Format("{0:D4}-{1:D2}-{2:D2} {3:D2}:{4:D2}:{5:D2}.{6:D3}", date.Year, date.Month, date.Day, date.Hour, date.Minute, date.Second, date.Millisecond);
        }

        public static string GetMySQLDateTimeString(this DateTime date)
        {
            return string.Format("{0:D4}-{1:D2}-{2:D2} {3:D2}:{4:D2}:{5:D2}", date.Year, date.Month, date.Day, date.Hour, date.Minute, date.Second);
        }
    }
}
