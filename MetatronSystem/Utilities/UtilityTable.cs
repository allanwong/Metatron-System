using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Utilities
{
    class UtilityTable
    {
        public static List<object> getColFromDataTable(DataTable dtInput, int iCol)
        {
            List<object> lstCol = new List<object>();
            for (int i = 0; i < dtInput.Rows.Count; i++)
            {
                lstCol.Add(dtInput.Rows[i][iCol]);
            }
            return lstCol;
        }

        public static string getCodeStringFromDataTableCol(DataTable dtInput, int iCol)
        {
            List<object> lstCol = getColFromDataTable(dtInput, iCol);
            string strCode = lstCol[0].ToString();
            for (int i = 1; i < lstCol.Count; i++)
            {
                strCode = strCode + "," + lstCol[i].ToString();
            }
            return strCode;
        }
    }
}
