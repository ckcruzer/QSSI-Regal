using System;
using System.Globalization;

namespace BSP.PowerHouse.DynamicsGP.Integration.Extensions
{
    public static class DateExtensions
    {
        private static DateTimeFormatInfo GpDateFormat = new CultureInfo("en-US").DateTimeFormat;
        public static string GpFormattedDate(this DateTime dte)
        {
            return dte != null ? dte.ToString("MM/dd/yyyy", GpDateFormat) : string.Empty;
        }
    }
}
