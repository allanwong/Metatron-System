using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Connection;
using WAPIWrapperCSharp;

namespace MetatronSystem
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            Connection.ConnWind.windEnsureStart();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            IndustryExcessReturnMonitor ierm = new IndustryExcessReturnMonitor();
            ierm.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            WindData wd = ConnWindData.fetchSectorConstituent(DateTime.Now, "全部A股");
            DataTable dtStock = ConnWindData.convertWindDatatoTable(wd);
            String strWindCode = Utilities.UtilityTable.getCodeStringFromDataTableCol(dtStock, 1);

            Strategy.EarningPositivePreannouncement.PreannouncementConditionFilter(dtStock, new DateTime(2016, 6, 30));
        }


    }
}
