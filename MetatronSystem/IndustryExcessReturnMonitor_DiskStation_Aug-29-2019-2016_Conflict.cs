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

        }

        private void fetchShenWanIndex()
        {
            WindData wd = ConnWindData.fetchSectorConstituent(DateTime.Now, "申银万国一级行业指数");
            dtIndustryIndex = ConnWindData.convertWindDatatoTable(wd);
        }

        private void 
    }
}
