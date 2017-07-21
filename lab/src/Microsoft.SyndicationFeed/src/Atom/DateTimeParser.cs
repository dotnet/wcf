using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Microsoft.SyndicationFeed.Atom
{
    static class DateTimeParser
    {
        public static bool TryParseDate(string dateTimeString, out DateTimeOffset result)
        {
            const string Rfc3339LocalDateTimeFormat = "yyyy-MM-ddTHH:mm:sszzz";
            const string Rfc3339UTCDateTimeFormat = "yyyy-MM-ddTHH:mm:ssZ";

            dateTimeString = dateTimeString.Trim();

            if (dateTimeString[19] == '.')
            {
                // remove any fractional seconds, we choose to ignore them
                int i = 20;
                while (dateTimeString.Length > i && char.IsDigit(dateTimeString[i]))
                {
                    ++i;
                }
                dateTimeString = dateTimeString.Substring(0, 19) + dateTimeString.Substring(i);
            }

            DateTimeOffset localTime;
            if (DateTimeOffset.TryParseExact(dateTimeString, Rfc3339LocalDateTimeFormat,
                CultureInfo.InvariantCulture.DateTimeFormat,
                DateTimeStyles.None, out localTime))
            {
                result = localTime;
                return true;
            }
            DateTimeOffset utcTime;
            if (DateTimeOffset.TryParseExact(dateTimeString, Rfc3339UTCDateTimeFormat,
                CultureInfo.InvariantCulture.DateTimeFormat,
                DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out utcTime))
            {
                result = utcTime;
                return true;
            }
            return false;
        }        
    }
}
