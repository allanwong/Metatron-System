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
        String strSelectIndexName;
        String strSelectedWindCode;

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
            DateTime dtBegin = dateTimePicker1.Value;
            DateTime dtEnd;
            DataTable dtIndustryPctChg;
            //DataTable dtIndustryVolume;

            string strWindCode = UtilityTable.getCodeStringFromDataTableCol(dtIndustryIndex, 1);
            if (UtilityCalendar.isAfterTradeHour(dateTimePicker1.Value))
            {
                dtBegin = dateTimePicker1.Value;
            }
            else
            {
                dtBegin=UtilityCalendar.fetchDayOffset(DateTime.Now.AddDays(-1), 0);
            }
            dtEnd=dtBegin;

            WindData wd = ConnWindData.fetchTimeSeriesSecInfo(strWindCode,"pct_chg", dtEnd, dtBegin);
            dtIndustryPctChg = ConnWindData.convertWindDatatoTable(wd);

            //wd = ConnWindData.fetchTimeSeriesSecInfo(strWindCode, "volume", dtEnd, dtBegin);
            //dtIndustryVolume = ConnWindData.convertWindDatatoTable(wd);

            dtIndustryPctChg.Columns.Add("Sector Name");
            dtIndustryPctChg.Columns.Add("Sector Code");
            dtIndustryPctChg.Columns.Add("PCT CHG", typeof(double));
            dtIndustryPctChg.Columns.Add("Volume", typeof(double));
            for (int i = 0; i < dtIndustryPctChg.Rows.Count; i++)
            {
                string strIndustryName = dtIndustryIndex.Rows[i][2].ToString();
                dtIndustryPctChg.Rows[i]["Sector Name"] = strIndustryName.Substring(0, strIndustryName.Length-4);
                dtIndustryPctChg.Rows[i]["Sector Code"] = dtIndustryIndex.Rows[i][1];
                dtIndustryPctChg.Rows[i]["PCT CHG"] = double.Parse(dtIndustryPctChg.Rows[i]["PCT_CHG"].ToString()) / 100;
                //dtIndustryPctChg.Rows[i]["Volume"] = double.Parse(dtIndustryVolume.Rows[i]["VOLUME"].ToString()) / 10000;
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

            Series seriesPctChg = chartPlot.Series.Add("Industry Pct Chg");
            seriesPctChg.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Column;
            chartPlot.ChartAreas[0].AxisY.LabelStyle.Format = "P2";
            chartPlot.ChartAreas[0].AxisY.LabelAutoFitStyle = LabelAutoFitStyles.DecreaseFont;

            seriesPctChg.XValueMember = "Sector Name";
            seriesPctChg.YValueMembers = "PCT CHG";

            chartPlot.ChartAreas[0].AxisX.Interval = 1;
            chartPlot.ChartAreas[0].AxisX.MajorGrid.Enabled = false;

            seriesPctChg.ToolTip = "涨幅: #VAL{P3} \r\n行业: #AXISLABEL";
            seriesPctChg.Palette = ChartColorPalette.BrightPastel;

            //Series seriesVolume = chartPlot.Series.Add("Industry Volume");
            //seriesPctChg.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Column;

            //seriesPctChg.XValueMember = "Sector Name";
            //seriesPctChg.YValueMembers = "Volume";
            //seriesVolume.YAxisType = AxisType.Secondary;

            //seriesPctChg.ToolTip = "成交量: #VAL \r\n行业: #AXISLABEL";
            //seriesPctChg.Palette = ChartColorPalette.BrightPastel;
        }


        private void plotIndexPriceSeries(Chart chartPlot)
        {
            string strBenchmark = "000300.SH";

            WindData wd = ConnWindData.fetchTimeSeriesSecInfo(strSelectedWindCode, "close", dateTimePicker1.Value.AddMonths(-3), dateTimePicker1.Value);
            DataTable dtIndex = ConnWindData.convertWindDatatoTable(wd);
            DataTable dtBenchmark = ConnWindData.convertWindDatatoTable(
                ConnWindData.fetchTimeSeriesSecInfo(strBenchmark, "close", dateTimePicker1.Value.AddMonths(-3), dateTimePicker1.Value));

            DataTable dtMixTable = new DataTable("Mix Data Table");
            dtMixTable.Columns.Add("DateTime", typeof(DateTime));
            dtMixTable.Columns.Add("Index", typeof(Double));
            dtMixTable.Columns.Add("Benchmark", typeof(double));

            for (int i = 0; i < dtBenchmark.Rows.Count; i++)
            {
                DataRow dr = dtMixTable.NewRow();
                dr["DateTime"] = wd.timeList[i];
                dr["Index"] = dtIndex.Rows[i]["CLOSE"];
                dr["Benchmark"] = dtBenchmark.Rows[i]["CLOSE"];
                dtMixTable.Rows.Add(dr);
            }

            DataView dv = dtMixTable.DefaultView;
            dv.Sort = "Index";
            double dMinimal = (double)dtMixTable.Rows[0]["Index"];
            dv.Sort = "Benchmark";
            double dMinimalBenchmark = (double)dtMixTable.Rows[0]["Benchmark"];

            chartPlot.ChartAreas[0].AxisY.Minimum = dMinimal * 0.95;
            chartPlot.ChartAreas[0].AxisY2.Minimum = dMinimalBenchmark * 0.95;
            chartPlot.ChartAreas[0].AxisY.LabelStyle.Format = "N1";
            chartPlot.ChartAreas[0].AxisY2.LabelStyle.Format = "N1";
            chartPlot.ChartAreas[0].AxisY.LabelAutoFitStyle = LabelAutoFitStyles.DecreaseFont;
            chartPlot.ChartAreas[0].AxisY2.LabelAutoFitStyle = LabelAutoFitStyles.DecreaseFont;
            chartPlot.ChartAreas[0].AxisX.LabelAutoFitStyle = LabelAutoFitStyles.DecreaseFont;

            chartPlot.ChartAreas[0].AxisY.MajorGrid.Enabled = false;
            chartPlot.ChartAreas[0].AxisY2.MajorGrid.Enabled = false;
            chartPlot.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
            chartPlot.ChartAreas[0].AxisX2.MajorGrid.Enabled = false;
            chartPlot.Legends[0].Docking = Docking.Right;

            chartPlot.Titles.Clear();
            chartPlot.Titles.Add(strSelectIndexName + "vs 沪深300 近三个月走势");
            chartPlot.DataSource = dtMixTable;
            chartPlot.Series.Clear();

            Series SeriesIndex = chartPlot.Series.Add("Industry Close Price");
            SeriesIndex.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;

            SeriesIndex.MarkerStyle = MarkerStyle.Circle;
            SeriesIndex.MarkerSize = 5;

            SeriesIndex.XValueMember = "DateTime";
            SeriesIndex.YValueMembers = "Index";

            SeriesIndex.ToolTip = strSelectedWindCode + "\r\n收盘价: #VAL\r\n日期: #VALX";

            Series SeriesBenchmark = chartPlot.Series.Add("BenchMark Close Price");
            SeriesBenchmark.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;

            SeriesBenchmark.YAxisType = AxisType.Secondary;

            SeriesBenchmark.MarkerStyle = MarkerStyle.Circle;
            SeriesBenchmark.MarkerSize = 5;

            SeriesBenchmark.XValueMember = "DateTime";
            SeriesBenchmark.YValueMembers = "Benchmark";

            SeriesBenchmark.ToolTip = strBenchmark + "\r\n收盘价: #VAL\r\n日期: #VALX";
        }

        private void chart1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            HitTestResult hitResult = new HitTestResult();
            hitResult = chart1.HitTest(e.X, e.Y);

            if (hitResult.Series != null)
            {
                strSelectIndexName = hitResult.Series.Points[hitResult.PointIndex].AxisLabel;
                if (strSelectIndexName != null)
                {
                    DataRow[] dr = dtIndustryIndex.Select("sec_name LIKE " + "'" + strSelectIndexName + "%'");
                    strSelectedWindCode = dr[0]["wind_code"].ToString();
                    if(strSelectedWindCode != null)
                        plotIndexPriceSeries(chart3);
                }
            }
        }
    }
}
