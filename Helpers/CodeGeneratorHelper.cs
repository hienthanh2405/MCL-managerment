using System;

namespace API.Helpers
{
    public static class CodeGeneratorHelper
    {
        public static string GetGeneratedCode(string prefix, string lastestCode, int wantedNumberLength)
        {
            DateTime today = DateTime.Now;
            string year = today.Year.ToString();
            string dateString = year;

            if (lastestCode == null)
            {
                return prefix + dateString + "1".PadLeft(wantedNumberLength, '0');
            }

            int number = Int32.Parse(lastestCode.Substring(lastestCode.Length - wantedNumberLength));

            string dateOflastestCode = lastestCode.Substring(prefix.Length ,  4 );
            // ex: lastestCode PX2018000001 => dateOflastestCode = 2018

            if (dateOflastestCode != dateString) // this is the fist code of day
            {
                number = 0;
            }
            return prefix + dateString + (number + 1).ToString().PadLeft(wantedNumberLength, '0');
        }
    }
}
