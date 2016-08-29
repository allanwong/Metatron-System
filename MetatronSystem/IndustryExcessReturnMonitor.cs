using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WAPIWrapperCSharp;
using Utilities;
using Connection;
using System.Windows.Forms.DataVisualization.Charting;

namespace MetatronSystem
{
    public partial class IndustryExcessReturnMonitor : Form
    {
        DataTable dtIndustryIndex;

        public IndustryExcessReturnMonitor()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string strIndexName = comboBox1.SelectedItem.ToString();
            fetchShenWanIndex(strIndexName);

            plotDailyIndustryPctChg(chart1);
        }

        private void fetchShenWanIndex(string strIndexName)
        {
            WindData wd = ConnWindData.fetchSectorConstituent(DateTime.Now, "申银万国一级行业指数");
            dtIndustryIndex = ConnWindData.convertWindDatatoTable(wd);
        }

        private DataTable fetchPeriodPctChgTable()
        {
            DateTime dtBegin;
            DateTime dtEnd;
            DataTable dtIndustryPctChg;

            string strWindCode = UtilityTable.getCodeStringFromDataTableCol(dtIndustryIndex, 1);
            if(UtilityCalendar.isAfterTradeHour(DateTime.Now))
            {
                dtBegin=DateTime.Now;
            }
            else
            {
                dtBegin=UtilityCalendar.fetchDayOffset(DateTime.Now.AddDays(-1), 0);
            }
            dtEnd=dtBegin.AddDays(-1);

            WindData wd = ConnWindData.fetchMultiSecPctChg(strWindCode, dtEnd, dtBegin);
            dtIndustryPctChg = ConnWindData.convertWindDatatoTable(wd);

            dtIndustryPctChg.Columns.Add("Sector Name");
            dtIndustryPctChg.Columns.Add("Sector Code");
            dtIndustryPctChg.Columns.Add("PCT CHG", typeof(double));
            for (int i = 0; i < dtIndustryPctChg.Rows.Count; i++)
            {
                string strIndustryName = dtIndustryIndex.Rows[i][2].ToString();
                dtIndustryPctChg.Rows[i]["Sector Name"] = strIndustryName.Substring(0, strIndustryName.Length-4);
                dtIndustryPctChg.Rows[i]["Sector Code"] = dtIndustryIndex.Rows[i][1];
                dtIndustryPctChg.Rows[i]["PCT CHG"] = double.Parse(dtIndustryPctChg.Rows[i]["PCT_CHG"].ToString()) / 100;
            }

            dtIndustryPctChg.Columns["PCT_CHG"].Dispose();
            DataView dv = dtIndustryPctChg.DefaultView;
            dv.Sort = "PCT CHG Asc";
            return dv.ToTable();
        }


        private void plotDailyIndustryPctChg(Chart chartPlot)
        {
            DataTable dtPlotData = fetchPeriodPctChgTable();

            chartPlot.DataSource = dtPlotData;
            chartPlot.Series.Clear();

            Series chrtSeries = chartPlot.Series.Add("Industry Pct Chg");
            chrtSeries.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Column;

            chrtSeries.XValueMember = "Sector Name";
            chrtSeries.YValueMembers = "PCT CHG";

            chartPlot.ChartAreas[0].AxisX.Interval = 1;
            chartPlot.ChartAreas[0].AxisX.MajorGrid.Enabled = false;

            chrtSeries.ToolTip = "涨幅: #VAL{P3} \r\n行业: #AXISLABEL";
            chrtSeries.Palette = ChartColorPalette.BrightPastel;
        }
    }
}
