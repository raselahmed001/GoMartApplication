using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GoMartApplication
{
    public partial class Form1 : Form
    {
        DBConnect dbConnect = new DBConnect();
        public static string loginName, loginType;
        public Form1()
        {
            InitializeComponent();
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {
           
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            cmbRole.SelectedIndex = 1;
            txtUsername.Text = "Admin";
            txtPass.Text = "12345678";

        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                if (cmbRole.SelectedIndex > 0)
                {
                    //validation
                    if (txtUsername.Text == string.Empty)
                    {
                        MessageBox.Show("Please Enter Valid User Name", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        txtUsername.Focus();
                        return;
                    }

                    if (txtPass.Text == string.Empty)
                    {
                        MessageBox.Show("Please Enter Valid Password", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        txtPass.Focus();
                        return;
                    }


                    if (!string.IsNullOrEmpty(txtUsername.Text) && !string.IsNullOrEmpty(txtPass.Text))
                    {
                        if (cmbRole.Text == "Admin")
                        {
                            SqlCommand cmd = new SqlCommand("SELECT TOP 1 AdminID, password, FullName FROM tb1Admin WHERE AdminID = @AdminID AND password = @Password", dbConnect.GetConnection());
                            cmd.Parameters.AddWithValue("@AdminID", txtUsername.Text.Trim());
                            cmd.Parameters.AddWithValue("@Password", txtPass.Text.Trim());

                            dbConnect.OpenConnection();

                            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);

                            dbConnect.CloseConnection();

                            if (dt.Rows.Count > 0)
                            {
                                MessageBox.Show("Login Successfully", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                loginName = txtUsername.Text;
                                loginType = cmbRole.Text;
                                clear_value();

                                this.Hide();
                                frmMain fm = new frmMain();
                                fm.FormClosed += (s, args) => Application.Exit();
                                fm.Show();
                            }
                            else
                            {
                                MessageBox.Show("Invalid User Name or Password.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        else if (cmbRole.Text == "Seller")
                        {
                            SqlCommand cmd = new SqlCommand("SELECT SellerID, SellerName FROM tb1Seller WHERE SellerName = @SellerName AND SellerPass = @SellerPass", dbConnect.GetConnection());
                            cmd.Parameters.AddWithValue("@SellerName", txtUsername.Text.Trim());
                            cmd.Parameters.AddWithValue("@SellerPass", txtPass.Text.Trim());

                            dbConnect.OpenConnection();

                            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);

                            dbConnect.CloseConnection();

                            if (dt.Rows.Count > 0)
                            {
                                MessageBox.Show("Login Successfully", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                               
                                loginName = dt.Rows[0]["SellerName"].ToString();
                                loginType = cmbRole.Text;
                                clear_value();

                                this.Hide();
                                frmMain fm = new frmMain();
                                fm.FormClosed += (s, args) => Application.Exit();
                                fm.Show();
                            }
                            else
                            {
                                MessageBox.Show("Invalid Seller Name or Password.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        else
                        {
                            MessageBox.Show("Unknown role selected.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Please Enter User Name And Password", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        clear_value();
                    }
                }
                else
                {
                    MessageBox.Show("Please select any role", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    clear_value();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void clear_value()
        {
            cmbRole.SelectedIndex = 0;
            txtUsername.Clear();
            txtPass.Clear();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {

        }

        private void txtPass_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
