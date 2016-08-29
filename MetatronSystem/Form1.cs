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
            WindData wd = ConnWindData.fetchSectorConstituent(DateTime.Now, "申银万国一级行业指数");
            DataTable dt = ConnWindData.convertWindDatatoTable(wd);
        }


    }
}
