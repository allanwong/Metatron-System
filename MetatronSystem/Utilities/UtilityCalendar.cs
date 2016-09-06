using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Connection;
using WAPIWrapperCSharp;

namespace Utilities
{
    class UtilityCalendar
    {
        /// <summary>
        /// fetch trade day from Wind
        /// </summary>
        /// <param name="dtInput">given date</param>
        /// <param name="iPrev">positive is backward, negative is forward</param>
        /// <returns></returns>
        public static DateTime fetchDayOffset(DateTime dtInput, int iPrev)
        {
            WindData wd = ConnWind.w.tdaysoffset(dtInput.ToShortDateString(), iPrev, "");
            return (DateTime)((object[])(wd.data))[0];
        }

        public static bool isTradeDay(DateTime dtInput)
        {
            if (fetchDayOffset(dtInput, 0).Date == dtInput.Date)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool isTradeHour(DateTime dtInput)
        {
            if (isTradeDay(dtInput))
            {
                if (dtInput > dtInput.Date.AddHours(9.5) && dtInput < dtInput.Date.AddHours(11.5))
                    return true;
                if (dtInput > dtInput.Date.AddHours(13) && dtInput < dtInput.Date.AddHours(15))
                    return true;
                return false;
            }
            else
            {
                return false;
            }
        }

        public static bool isAfterTradeHour(DateTime dtInput)
        {
            if (isTradeDay(dtInput))
            {
                if (dtInput > dtInput.Date.AddHours(15))
                    return true;
                return false;
            }
            else
            {
                return false;
            }
        }

        public static DateTime getSeasonReportDate(DateTime dtReportDate, int iPrev)
        {
            int iSeason = 1;
            if (dtReportDate.Month == 3)
            {
                iSeason = 1;
            }
            else if (dtReportDate.Month == 6)
            {
                iSeason = 2;
            }
            else if (dtReportDate.Month == 9)
            {
                iSeason = 3;
            }
            else if (dtReportDate.Month == 12)
            {
                iSeason = 4;
            }

            if (iSeason > iPrev)
            {
                if (iSeason - iPrev == 1)
                {
                    return new DateTime(dtReportDate.Year, (iSeason - iPrev) * 3, 31);
                }
                else
                {
                    return new DateTime(dtReportDate.Year, (iSeason - iPrev) * 3, 30);
                }
            }
            return DateTime.MaxValue;
        }
    }
}
