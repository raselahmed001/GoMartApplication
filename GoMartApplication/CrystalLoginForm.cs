using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using System.Windows.Forms;

namespace GoMartApplication
{
    public partial class CrystalLoginForm : Form
    {
        public CrystalLoginForm()
        {
            InitializeComponent();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {

            CrystalReportShowForm reportShow = new CrystalReportShowForm();
           
            CrystalReportProduct product = new CrystalReportProduct();

            TextObject first = (TextObject)product.ReportDefinition.Sections["Section3"].ReportObjects["first_name"];
            first.Text = textBoxFirstName.Text;
            TextObject last = (TextObject)product.ReportDefinition.Sections["Section3"].ReportObjects["lastName"];
            last.Text = textBoxLastName.Text;
            TextObject user = (TextObject)product.ReportDefinition.Sections["Section3"].ReportObjects["userName"];
            user.Text = textBoxUserName.Text;
            TextObject password = (TextObject)product.ReportDefinition.Sections["Section3"].ReportObjects["password"];
            password.Text = textBoxPassword.Text;

            reportShow.crystalReportViewer2.ReportSource = product;
            reportShow.Show();



            

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}
