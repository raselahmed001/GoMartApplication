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
    public partial class FrmAddNewSeller : Form
    {
        DBConnect dbConnect = new DBConnect();
        public FrmAddNewSeller()
        {
            InitializeComponent();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void FrmAddNewSeller_Load(object sender, EventArgs e)
        {
            lblSellerId.Visible     = false;
            btnUpdateSeller.Visible = false;
            btnDeleteSeller.Visible = false;
            btnAddSeller.Visible    = true;
            showList();
        }

        private void btnAddSeller_Click(object sender, EventArgs e)
        {
            // Validation
            if (string.IsNullOrWhiteSpace(textSellerName.Text))
            {
                MessageBox.Show("Please Enter Seller Name", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textSellerName.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(textSellerAge.Text))
            {
                MessageBox.Show("Please Enter Age", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textSellerAge.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(textPhone.Text))
            {
                MessageBox.Show("Please Enter Phone Number", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textPhone.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(textPassword.Text))
            {
                MessageBox.Show("Please Enter Age", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textPassword.Focus();
                return;
            }

            string sellerName = textSellerName.Text.Trim();
            string sellerAge = textSellerAge.Text.Trim();
            string sellerPhone = textPhone.Text.Trim();
            string sellerPassword = textPassword.Text.Trim();

            try
            {
                dbConnect.OpenConnection();

                // Check if category name already exists
                SqlCommand checkCmd = new SqlCommand("SELECT COUNT(*) FROM tb1Seller WHERE SellerName = @SellerName", dbConnect.GetConnection());
                checkCmd.Parameters.AddWithValue("@SellerName", sellerName);
                int count = (int)checkCmd.ExecuteScalar();

                if (count > 0)
                {
                    MessageBox.Show("Seller  already exists!", "Duplicate", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    dbConnect.CloseConnection();
                    clearValue();
                    showList();
                    return;
                }

                // Insert if not duplicate
                SqlCommand cmd = new SqlCommand("INSERT INTO tb1Seller (SellerName, SellerAge, SellerPhone, SellerPass) VALUES (@SellerName, @SellerAge, @SellerPhone, @SellerPass)", dbConnect.GetConnection());
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@SellerName", sellerName);
                cmd.Parameters.AddWithValue("@SellerAge", sellerAge);
                cmd.Parameters.AddWithValue("@SellerPhone", sellerPhone);
                cmd.Parameters.AddWithValue("@SellerPass", sellerPassword);

                int rows = cmd.ExecuteNonQuery();
                dbConnect.CloseConnection();

                if (rows > 0)
                {
                    MessageBox.Show("Seller added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    showList();
                    clearValue();
                }
                else
                {
                    MessageBox.Show("Seller not added.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                dbConnect.CloseConnection();
                MessageBox.Show("Error: " + ex.Message, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnUpdateSeller_Click(object sender, EventArgs e)
        {
            // Validation
            if (string.IsNullOrWhiteSpace(textSellerName.Text))
            {
                MessageBox.Show("Please enter seller name", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textSellerName.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(textSellerAge.Text))
            {
                MessageBox.Show("Please enter age", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textSellerAge.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(textPhone.Text))
            {
                MessageBox.Show("Please enter phone number", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textPhone.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(textPassword.Text))
            {
                MessageBox.Show("Please enter password", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textPassword.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(lblSellerId.Text))
            {
                MessageBox.Show("Seller ID is missing. Please select a seller from the list.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string sellerId = lblSellerId.Text.Trim();
            string sellerName = textSellerName.Text.Trim();
            string sellerAge = textSellerAge.Text.Trim();
            string sellerPhone = textPhone.Text.Trim();
            string sellerPassword = textPassword.Text.Trim();

            try
            {
                dbConnect.OpenConnection();

                // Check if same name exists for other seller
                SqlCommand checkCmd = new SqlCommand(
                    "SELECT COUNT(*) FROM tb1Seller WHERE SellerName = @SellerName AND SellerID != @SellerID",
                    dbConnect.GetConnection()
                );
                checkCmd.Parameters.AddWithValue("@SellerName", sellerName);
                checkCmd.Parameters.AddWithValue("@SellerID", sellerId);
                int count = (int)checkCmd.ExecuteScalar();

                if (count > 0)
                {
                    MessageBox.Show("Another seller with this name already exists.", "Duplicate", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    dbConnect.CloseConnection();
                    return;
                }

                // Update seller
                SqlCommand cmd = new SqlCommand(
                    "UPDATE tb1Seller SET SellerName = @SellerName, SellerAge = @SellerAge, SellerPhone = @SellerPhone, SellerPass = @SellerPass WHERE SellerID = @SellerID",
                    dbConnect.GetConnection()
                );
                cmd.Parameters.AddWithValue("@SellerName", sellerName);
                cmd.Parameters.AddWithValue("@SellerAge", sellerAge);
                cmd.Parameters.AddWithValue("@SellerPhone", sellerPhone);
                cmd.Parameters.AddWithValue("@SellerPass", sellerPassword);
                cmd.Parameters.AddWithValue("@SellerID", sellerId);

                int rows = cmd.ExecuteNonQuery();
                dbConnect.CloseConnection();

                if (rows > 0)
                {
                    MessageBox.Show("Seller updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    showList();
                    clearValue();

                    btnAddSeller.Visible = true;
                    btnUpdateSeller.Visible = false;
                    btnDeleteSeller.Visible = false;
                    lblSellerId.Visible = false;
                }
                else
                {
                    MessageBox.Show("Update failed. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                dbConnect.CloseConnection();
                MessageBox.Show("Error: " + ex.Message, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDeleteSeller_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(lblSellerId.Text))
            {
                MessageBox.Show("Please select a seller to delete.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult result = MessageBox.Show("Are you sure you want to delete this seller?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                string sellerId = lblSellerId.Text.Trim();

                try
                {
                    dbConnect.OpenConnection();

                    SqlCommand cmd = new SqlCommand("DELETE FROM tb1Seller WHERE SellerID = @SellerID", dbConnect.GetConnection());
                    cmd.Parameters.AddWithValue("@SellerID", sellerId);

                    int rows = cmd.ExecuteNonQuery();
                    dbConnect.CloseConnection();

                    if (rows > 0)
                    {
                        MessageBox.Show("Seller deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        showList();
                        clearValue();

                        // Reset buttons
                        btnAddSeller.Visible = true;
                        btnUpdateSeller.Visible = false;
                        btnDeleteSeller.Visible = false;
                        lblSellerId.Visible = false;
                    }
                    else
                    {
                        MessageBox.Show("Delete failed. Seller may not exist.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    dbConnect.CloseConnection();
                    MessageBox.Show("Error: " + ex.Message, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void clearValue()
        {
            textSellerName.Clear();
            textSellerAge.Clear();
            textPhone.Clear();
            textPassword.Clear();
        }

        private void showList()
        {
            try
            {
                SqlCommand cmd = new SqlCommand("SELECT [SellerID], [SellerName] AS [Seller Name], [SellerAge] AS [Age] FROM tb1Seller", dbConnect.GetConnection());
                dbConnect.OpenConnection();

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                dbConnect.CloseConnection();

                // Add Serial Number Column
                dt.Columns.Add("S.No", typeof(int));
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    dt.Rows[i]["S.No"] = i + 1;
                }

                // Move "S.No" to the first column
                dt.Columns["S.No"].SetOrdinal(0);

                dataGridView1.DataSource = dt;

                // Hide the SellerID column (not 'id')
                dataGridView1.Columns["SellerID"].Visible = false;
            }
            catch (Exception ex)
            {
                dbConnect.CloseConnection();
                MessageBox.Show("Error loading seller list: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dataGridView1_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow != null && dataGridView1.CurrentRow.Index >= 0)
            {
                // Show update/delete buttons, hide add button
                btnAddSeller.Visible = false;
                lblSellerId.Visible = false;
                btnUpdateSeller.Visible = true;
                btnDeleteSeller.Visible = true;

                DataGridViewRow selectedRow = dataGridView1.CurrentRow;

                // Use correct column indexes (adjust if needed)
                lblSellerId.Text = selectedRow.Cells["SellerID"].Value?.ToString() ?? "";
                textSellerName.Text = selectedRow.Cells["Seller Name"].Value?.ToString() ?? "";
                textSellerAge.Text = selectedRow.Cells["Age"].Value?.ToString() ?? "";

                // You need to fetch phone & password manually from DB (since not in DataGridView)
                string sellerId = lblSellerId.Text;
                try
                {
                    dbConnect.OpenConnection();
                    SqlCommand cmd = new SqlCommand("SELECT SellerPhone, SellerPass FROM tb1Seller WHERE SellerID = @SellerID", dbConnect.GetConnection());
                    cmd.Parameters.AddWithValue("@SellerID", sellerId);
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        textPhone.Text = reader["SellerPhone"].ToString();
                        textPassword.Text = reader["SellerPass"].ToString();
                    }
                    reader.Close();
                    dbConnect.CloseConnection();
                }
                catch (Exception ex)
                {
                    dbConnect.CloseConnection();
                    MessageBox.Show("Error loading seller details: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

    }
}
