using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Utilities;
using Connection;
using WAPIWrapperCSharp;

namespace Strategy
{
    class EarningPositivePreannouncement
    {
        public static DataTable dtEarningPositivePreannouncement = new DataTable("EarningPositivePreannouncement");
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
        public static WindData windProfitNoticeNetProfitMin(String strWindCode, DateTime dtReportDate)
        {
            WindData wd = ConnWindData.fetchTimeSeriesSecInfo(strWindCode, "profitnotice_netprofitmin", dtReportDate, dtReportDate);
            ConnWind.windEnsureNoErr(wd);
            return wd;
        }

        /// <summary>
        /// 预告净利润同比增长下限
        /// </summary>
        /// <param name="strWindCode"></param>
        /// <param name="dtReportDate"></param>
        public static WindData windProfitNoticeChangeMin(String strWindCode, DateTime dtReportDate)
        {
            WindData wd = ConnWindData.fetchTimeSeriesSecInfo(strWindCode, "profitnotice_changemin", dtReportDate, dtReportDate);
            ConnWind.windEnsureNoErr(wd);
            return wd;
        }


        public static WindData windNetProfitBelongToParComSH(String strWindCode, DateTime dtReportDate)
        {
            WindData wd = ConnWindData.fetchTimeSeriesSecInfo(strWindCode, "np_belongto_parcomsh", dtReportDate, dtReportDate, "rptType=1");
            ConnWind.windEnsureNoErr(wd);
            return wd;
        }

        /// <summary>
        /// 单季度净利润下限
        /// </summary>
        /// <param name="strWindCode"></param>
        /// <param name="dtReportDater"></param>
        public static void PreannouncementConditionFilter(DataTable dtStock, DateTime dtReportDate)
        {
            DateTime dtPrevReportDate = UtilityCalendar.getSeasonReportDate(dtReportDate, 1);

            string strWindCode = UtilityTable.getCodeStringFromDataTableCol(dtStock, 1);
            WindData wd1 = windProfitNoticeNetProfitMin(strWindCode, dtReportDate);
            DataTable dtNPM = ConnWindData.convertWindDatatoTable(wd1);

            dtEarningPositivePreannouncement.Columns.Add("DATETIME", Type.GetType("System.DateTime"));
            dtEarningPositivePreannouncement.Columns.Add("WINDCODE", typeof(String));
            dtEarningPositivePreannouncement.Columns.Add("COMNAME", typeof(String));
            dtEarningPositivePreannouncement.Columns.Add("PROFITNOTICE_NETPROFITMIN", typeof(Double));                          
            for (int i = 0; i < dtStock.Rows.Count; i++)
            {
                DataRow dr = dtEarningPositivePreannouncement.NewRow();
                dr["DATETIME"] = dtStock.Rows[i][0];
                dr["WINDCODE"] = dtStock.Rows[i][1];
                dr["COMNAME"] = dtStock.Rows[i][2];
                double dPNNPM = double.Parse(dtNPM.Rows[i]["PROFITNOTICE_NETPROFITMIN"].ToString());
                if (dPNNPM > 10000000 && !Double.IsNaN(dPNNPM))
                {
                    dr["PROFITNOTICE_NETPROFITMIN"] = dPNNPM / 10000;
                    dtEarningPositivePreannouncement.Rows.Add(dr);
                }      
            }

            String strSelectedWindCode = UtilityTable.getCodeStringFromDataTableCol(dtEarningPositivePreannouncement, 1);

            WindData wd2 = windNetProfitBelongToParComSH(strSelectedWindCode, dtPrevReportDate);
            DataTable dtNPBTPCSH = ConnWindData.convertWindDatatoTable(wd2);
          
            dtEarningPositivePreannouncement.Columns.Add("SEASON_NETPROFITMIN", typeof(double));
            for (int i = dtEarningPositivePreannouncement.Rows.Count - 1; i >= 0; i--)
            {
                double dProfitNoticeNetProfit = Double.Parse(dtEarningPositivePreannouncement.Rows[i]["PROFITNOTICE_NETPROFITMIN"].ToString());
                double dNetPorfitBTParComSH = Double.Parse(dtNPBTPCSH.Rows[i]["NP_BELONGTO_PARCOMSH"].ToString());
                if ((dProfitNoticeNetProfit - dNetPorfitBTParComSH) > 10000000)
                {
                    dtEarningPositivePreannouncement.Rows[i]["SEASON_NETPROFITMIN"] = (dProfitNoticeNetProfit - dNetPorfitBTParComSH) / 10000;
                }
                else
                {
                    dtEarningPositivePreannouncement.Rows[i].Delete();
                }
            }
            strSelectedWindCode = UtilityTable.getCodeStringFromDataTableCol(dtEarningPositivePreannouncement, 1);

            WindData wd3 = windProfitNoticeChangeMin(strSelectedWindCode, dtReportDate);
            DataTable dtProfitNoticeChangeMin = ConnWindData.convertWindDatatoTable(wd3);

            dtEarningPositivePreannouncement.Columns.Add("PN_CHANGEMIN", typeof(double));
            for (int i = dtEarningPositivePreannouncement.Rows.Count - 1; i >= 0; i--)
            {
                double dChangeMin = double.Parse(dtProfitNoticeChangeMin.Rows[i]["PROFITNOTICE_CHANGEMIN"].ToString());
                if (dChangeMin > 0.7)
                {
                    dtEarningPositivePreannouncement.Rows[i]["PN_CHANGEMIN"] = dChangeMin;
                }
                else
                {
                    dtEarningPositivePreannouncement.Rows[i].Delete();
                }
            }


        }
    }
}
