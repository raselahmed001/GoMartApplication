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
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            if(Form1.loginName != null)
            {
                toolStripStatusLabel2.Text = Form1.loginName;
            }
            if(Form1.loginType != null && Form1.loginType == "Seller")
            {
                categoryToolStripMenuItem.Visible = false;
                productToolStripMenuItem.Visible = false;
                addUserToolStripMenuItem.Visible = false;
            }
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {
            
        }

        private void categoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Category category = new Category();
            category.Show();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox1 aboutBox = new AboutBox1();
            aboutBox.Show();

        }

        private void frmMain_FormClosed(object sender, FormClosedEventArgs e)
        {
           

        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult result = MessageBox.Show(
               "Are you sure you want to exit?",
               "Exit Confirmation",
               MessageBoxButtons.YesNo,
               MessageBoxIcon.Question
           );
            if (result == DialogResult.No)
            {
                e.Cancel = true;
            }
            else
            {

                Application.Exit();
            }
        }

        private void sellerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmAddNewSeller frmAddNewSeller = new FrmAddNewSeller();
            frmAddNewSeller.ShowDialog();
  
        }

        private void adminToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddAdmin addAdmin = new AddAdmin();
            addAdmin.ShowDialog();
        }

        private void productToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddProducts product = new AddProducts();
            product.ShowDialog();
        }

        private void sellerToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            SellingForm sellerForm = new SellingForm();
            sellerForm.ShowDialog();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
               "Are you sure you want to exit?",
               "Exit Confirmation",
               MessageBoxButtons.YesNo,
               MessageBoxIcon.Question
           );
            if (result == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private void addImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ProfileForm profileForm = new ProfileForm();
            profileForm.ShowDialog();
        }

        private void reportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CrystalLoginForm crystalLoginForm = new CrystalLoginForm();
            crystalLoginForm.ShowDialog();
        }
    }
}
