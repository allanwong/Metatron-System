using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utilities;
using Connection;
using WAPIWrapperCSharp;

namespace Strategy
{
    class EarningPositivePreannouncement
    {
        // 2015-12-09 海通 《潜伏利好之业绩预增》
        // 筛选条件：
        // 1. 净利润下限》=1000万
        // 2. 单季净利润下限》=1000万
        // 3. 净利润增速下限》=30%
        // 4. 单季净利润增速下限》=30%

        /// <summary>
        /// 预告净利润下限
        /// </summary>
        /// <param name="strWindCode"></param>
        /// <param name="dtReportDate"></param>
        public static void windProfitNoticeNetProfitMin(String strWindCode, DateTime dtReportDate)
        {
            WindData wd = ConnWindData.fetchTimeSeriesSecInfo(strWindCode, "profitnotice_netprofitmin", dtReportDate, dtReportDate);
        }

        /// <summary>
        /// 预告净利润同比增长下限
        /// </summary>
        /// <param name="strWindCode"></param>
        /// <param name="dtReportDate"></param>
        public static void windProfitNoticeChangeMin(String strWindCode, DateTime dtReportDate)
        {
            WindData wd = ConnWindData.fetchTimeSeriesSecInfo(strWindCode, "profitnotice_changemin", dtReportDate, dtReportDate);
        }
    }
}
