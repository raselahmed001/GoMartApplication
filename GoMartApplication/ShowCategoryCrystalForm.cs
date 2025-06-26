using CrystalDecisions.CrystalReports.Engine;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GoMartApplication
{
    public partial class ShowCategoryCrystalForm : Form
    {
        public ShowCategoryCrystalForm()
        {
            InitializeComponent();
        }

        public string ReportName { get; set; }
        public DataTable ReportData { get; set; }

        private void ShowCategoryCrystalForm_Load(object sender, EventArgs e)
        {
           try
            {
                ReportDocument reportDocument = new ReportDocument();
                reportDocument.Load(ReportName);
                reportDocument.SetDataSource(ReportData);
                crystalReportViewer1.ReportSource = reportDocument;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load report: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
        }


    }
}
