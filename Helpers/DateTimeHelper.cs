using System;
using System.Globalization;

namespace API.Helpers
{
    public static class DateTimeHelper
    {
        private static DateTime FirstDateOfWeekISO8601( int weekOfYear , int year)
        {
            DateTime jan1 = new DateTime(year, 1, 1);
            int daysOffset = DayOfWeek.Thursday - jan1.DayOfWeek;

            DateTime firstThursday = jan1.AddDays(daysOffset);
            var cal = CultureInfo.CurrentCulture.Calendar;
            int firstWeek = cal.GetWeekOfYear(firstThursday, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);

            var weekNum = weekOfYear;
            if (firstWeek <= 1)
            {
                weekNum -= 1;
            }
            var result = firstThursday.AddDays(weekNum * 7);
            return result.AddDays(-3);
        }

        public static DateTime ConvertWeekYearToDateTime(string weekYear)
        {
                string[] stringSeparators = new string[] { "/" };
                var result = weekYear.Split(stringSeparators, StringSplitOptions.None);
                if (result.Length != 2)
                {
                    throw new Exception("ProductionWeekYear must has format week/year");
                }
                return FirstDateOfWeekISO8601(Int32.Parse(result[0]), Int32.Parse(result[1]));     
        }

        public static DateTime ConvertUTCTimeToVietnamTime(DateTime convertedDateTime)
        {
            return convertedDateTime.AddHours(CONSTANT.TIME_ZONE);
        }

        public static DateTime GetVietnamNow()
        {
            DateTime utcToday = DateTime.UtcNow;
            return ConvertUTCTimeToVietnamTime(utcToday);
        }

        public static DateTime UnixTimeStampToDateTime(long unixTimeStampinSecond)
        {
            // Unix timestamp is seconds past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStampinSecond).ToLocalTime();
            return dtDateTime;
        }
    }
}
