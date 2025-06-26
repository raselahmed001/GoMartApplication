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
    public partial class AddAdmin : Form
    {
        DBConnect dbConnect = new DBConnect();
        public AddAdmin()
        {
            InitializeComponent();
        }

        private void AddAdmin_Load(object sender, EventArgs e)
        {
            LID.Visible = false;
            btnAdd.Visible = true;
            btnUpdate.Visible = false;
            btnDelete.Visible = false;
            textAdminName.Focus();
            showList();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            // Validation
            if (string.IsNullOrWhiteSpace(textAdminName.Text))
            {
                MessageBox.Show("Please Enter Admin Name", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textAdminName.Focus();
                return;
            }                    

            if (string.IsNullOrWhiteSpace(textPassword.Text))
            {
                MessageBox.Show("Please Enter Age", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textPassword.Focus();
                return;
            }

            string adminName = textAdminName.Text.Trim();
            string adminId = textAdminId.Text.Trim();
            string adminPassword = textPassword.Text.Trim();

            try
            {
                dbConnect.OpenConnection();

                // Check if category name already exists
                SqlCommand checkCmd = new SqlCommand("SELECT COUNT(*) FROM tb1Admin WHERE AdminID = @AdminID", dbConnect.GetConnection());
                checkCmd.Parameters.AddWithValue("@AdminID", adminId);
                int count = (int)checkCmd.ExecuteScalar();

                if (count > 0)
                {
                    MessageBox.Show("Admin  already exists!", "Duplicate", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    dbConnect.CloseConnection();
                    clearValue();
                    showList();
                    return;
                }

                // Insert if not duplicate
                SqlCommand cmd = new SqlCommand("INSERT INTO tb1Admin (AdminID, FullName, password) VALUES (@AdminID, @FullName, @password)", dbConnect.GetConnection());
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@AdminID", adminId);
                cmd.Parameters.AddWithValue("@FullName", adminName);
                cmd.Parameters.AddWithValue("@password", adminPassword);

                int rows = cmd.ExecuteNonQuery();
                dbConnect.CloseConnection();

                if (rows > 0)
                {
                    MessageBox.Show("Admin added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    showList();
                    clearValue();
                }
                else
                {
                    MessageBox.Show("Admin not added.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                dbConnect.CloseConnection();
                MessageBox.Show("Error: " + ex.Message, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void clearValue()
        {
            textAdminId.Clear();
            textAdminName.Clear();
            textPassword.Clear();
        }

        private void showList()
        {
            try
            {
                SqlCommand cmd = new SqlCommand("SELECT [AdminID] AS [Name], [FullName] AS [Full Name] FROM tb1Admin", dbConnect.GetConnection());
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
                //dataGridView1.Columns["SellerID"].Visible = false;
            }
            catch (Exception ex)
            {
                dbConnect.CloseConnection();
                MessageBox.Show("Error loading admin list: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            // Validation
            if (string.IsNullOrWhiteSpace(textAdminName.Text))
            {
                MessageBox.Show("Please Enter Admin Name", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textAdminName.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(textPassword.Text))
            {
                MessageBox.Show("Please Enter Password", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textPassword.Focus();
                return;
            }

            string id = textAdminId.Text.Trim(); 
            string adminName = textAdminName.Text.Trim();
            string adminPassword = textPassword.Text.Trim();

            try
            {
                dbConnect.OpenConnection();

                // Check if same AdminID exists (excluding self if needed)
                SqlCommand checkCmd = new SqlCommand(
                    "SELECT COUNT(*) FROM tb1Admin WHERE AdminID = @AdminID",
                    dbConnect.GetConnection()
                );
                checkCmd.Parameters.AddWithValue("@AdminID", id);
                int count = (int)checkCmd.ExecuteScalar();

                if (count == 0)
                {
                    MessageBox.Show("Admin not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    dbConnect.CloseConnection();
                    return;
                }

                // Corrected Update Query (was using tb1Seller instead of tb1Admin)
                SqlCommand cmd = new SqlCommand(
                    "UPDATE tb1Admin SET FullName = @FullName, password = @password WHERE AdminID = @AdminID",
                    dbConnect.GetConnection()
                );
                cmd.Parameters.AddWithValue("@AdminID", id);
                cmd.Parameters.AddWithValue("@FullName", adminName);
                cmd.Parameters.AddWithValue("@password", adminPassword);

                int rows = cmd.ExecuteNonQuery();
                dbConnect.CloseConnection();

                if (rows > 0)
                {
                    MessageBox.Show("Admin updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    showList();
                    clearValue();

                    btnAdd.Visible = true;
                    btnUpdate.Visible = false;
                    btnDelete.Visible = false;
                    LID.Visible = false;
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


        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(LID.Text))
            {
                MessageBox.Show("Please select a admin to delete.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult result = MessageBox.Show("Are you sure you want to delete this admin?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                string adminId = LID.Text.Trim();

                try
                {
                    dbConnect.OpenConnection();

                    SqlCommand cmd = new SqlCommand("DELETE FROM tb1Admin WHERE AdminID = @AdminID", dbConnect.GetConnection());
                    cmd.Parameters.AddWithValue("@AdminID", adminId);

                    int rows = cmd.ExecuteNonQuery();
                    dbConnect.CloseConnection();

                    if (rows > 0)
                    {
                        MessageBox.Show("Admin deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        showList();
                        clearValue();

                        // Reset buttons
                        btnAdd.Visible = true;
                        btnUpdate.Visible = false;
                        btnDelete.Visible = false;
                        LID.Visible = false;
                    }
                    else
                    {
                        MessageBox.Show("Delete failed. Admin may not exist.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    dbConnect.CloseConnection();
                    MessageBox.Show("Error: " + ex.Message, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
      

        private void dataGridView1_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow != null && dataGridView1.CurrentRow.Index >= 0)
            {
                // Show update/delete buttons, hide add button
                btnAdd.Visible = false;
                LID.Visible = false;
                btnUpdate.Visible = true;
                btnDelete.Visible = true;

                DataGridViewRow selectedRow = dataGridView1.CurrentRow;

                // Match aliases used in showList() method
                string selectedAdminId = selectedRow.Cells["Name"].Value?.ToString() ?? "";
                string selectedFullName = selectedRow.Cells["Full Name"].Value?.ToString() ?? "";

                // Set values in textboxes
                textAdminId.Text = selectedAdminId;
                textAdminName.Text = selectedFullName;
                LID.Text = selectedAdminId;

                try
                {
                    dbConnect.OpenConnection();
                    SqlCommand cmd = new SqlCommand("SELECT password FROM tb1Admin WHERE AdminID = @AdminID", dbConnect.GetConnection());
                    cmd.Parameters.AddWithValue("@AdminID", selectedAdminId);
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        textPassword.Text = reader["password"].ToString();
                    }
                    reader.Close();
                    dbConnect.CloseConnection();
                }
                catch (Exception ex)
                {
                    dbConnect.CloseConnection();
                    MessageBox.Show("Error loading admin details: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }


    }
}
