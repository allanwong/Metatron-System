using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using WAPIWrapperCSharp;
using System.Configuration;
using Utilities;

namespace Connection
{
    public class ConnWindData
    {
        /// <summary>
        /// Fetch Sector Constituent
        /// </summary>
        /// <param name="date">Fetch Date</param>
        /// <param name="strIndexName">Sector Name</param>
        /// <param name="strSectorID">App Config Section Name</param>
        /// <returns></returns>
        public static WindData fetchSectorConstituent(DateTime date, String strIndexName, String strSectorID = "DataSet_SectorID")
        {
            string strWindCode = Utilities.UtilityConfig.GetAppConfig(strIndexName, strSectorID);
            string strQuery = "date=" + date.ToShortDateString() + ";sectorid=" + strWindCode;
            WindData wd = ConnWind.w.wset("sectorconstituent", strQuery);
            ConnWind.windEnsureNoErr(wd);
            return wd;
        }

        public static WindData fetchTimeSeriesSecInfo(String strWindCode, string strCodeType, DateTime dtBegin, DateTime dtEnd)
        {
            WindData wd = ConnWind.w.wsd(strWindCode, strCodeType, dtBegin.ToShortDateString(), dtEnd.ToShortDateString(), "");
            ConnWind.windEnsureNoErr(wd);
            return wd;
        }


        /// <summary>
        /// Convert any kind of wind data into data table
        /// </summary>
        /// <param name="wd">Wind Format Data</param>
        /// <returns></returns>
        public static DataTable convertWindDatatoTable(WindData wd)
        {
            DataTable dtTable = new DataTable();
            int iCol = wd.fieldList.Length;

            for (int i = 0; i < iCol; i++)
            {
                dtTable.Columns.Add(wd.fieldList[i]);
            }

            if (wd.data.GetType().FullName == "System.Double[]")
            {
                double[] dblWindData = (double[])wd.data;

                for (int i = 0; i < dblWindData.Length / iCol; i++)
                {
                    DataRow dr = dtTable.NewRow();
                    for (int j = 0; j < iCol; j++)
                    {
                        dr[j] = dblWindData[i * iCol + j];
                    }
                    dtTable.Rows.Add(dr);
                }
            }
            else
            {
                object[] objWindData = (object[])wd.data;

                for (int i = 0; i < objWindData.Length / iCol; i++)
                {
                    DataRow dr = dtTable.NewRow();
                    for (int j = 0; j < iCol; j++)
                    {
                        dr[j] = objWindData[i * iCol + j];
                    }
                    dtTable.Rows.Add(dr);
                }
            }
            
            

            return dtTable;
        }
    }
}
