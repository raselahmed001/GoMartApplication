using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.VisualBasic;

namespace GoMartApplication
{
    public partial class Category : Form
    {
        DBConnect dbConnect = new DBConnect();
        private int selectedCategoryId = -1;

       
        public Category()
        {
            InitializeComponent();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

           
        }

        private void Category_Load(object sender, EventArgs e)
        {
            btnUpdateCategory.Visible = false;
            btnDeleteCategory.Visible = false;
            lblCatId.Visible = false;
            //calculateTotalPages();
            showCategoryList();
        }

        private void btnAddCategory_Click(object sender, EventArgs e)
        {
            // Validation
            if (string.IsNullOrWhiteSpace(textCategoryName.Text))
            {
                MessageBox.Show("Please Enter Category Name", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textCategoryName.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(richTextCategoryDesc.Text))
            {
                MessageBox.Show("Please Enter Category Description", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                richTextCategoryDesc.Focus();
                return;
            }

            string categoryName = textCategoryName.Text.Trim();
            string categoryDesc = richTextCategoryDesc.Text.Trim();

            try
            {
                dbConnect.OpenConnection();

                // Check if category name already exists
                SqlCommand checkCmd = new SqlCommand("SELECT COUNT(*) FROM categories WHERE category_name = @CategoryName", dbConnect.GetConnection());
                checkCmd.Parameters.AddWithValue("@CategoryName", categoryName);
                int count = (int)checkCmd.ExecuteScalar();

                if (count > 0)
                {
                    MessageBox.Show("Category already exists!", "Duplicate", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    dbConnect.CloseConnection();
                    clearValue();
                    return;
                }

                // Insert if not duplicate
                SqlCommand cmd = new SqlCommand("INSERT INTO categories (category_name, category_dece) VALUES (@CategoryName, @CategoryDesc)", dbConnect.GetConnection());
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@CategoryName", categoryName);
                cmd.Parameters.AddWithValue("@CategoryDesc", categoryDesc);

                int rows = cmd.ExecuteNonQuery();
                dbConnect.CloseConnection();

                if (rows > 0)
                {
                    MessageBox.Show("Category added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    showCategoryList();
                    clearValue();
                }
                else
                {
                    MessageBox.Show("Category not added.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            textCategoryName.Clear();
            richTextCategoryDesc.Clear();
        }

        private void showCategoryList()
        {
            try
            {
                SqlCommand cmd = new SqlCommand("SELECT [id], [category_name] AS [Category Name], [category_dece] AS [Category Description] FROM categories", dbConnect.GetConnection());
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

                // Hide the 'id' column
                dataGridView1.Columns["id"].Visible = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading category list: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                dbConnect.CloseConnection();
            }
        }


        private void btnUpdateCategory_Click(object sender, EventArgs e)
        {
            // Validation
            if (string.IsNullOrWhiteSpace(textCategoryName.Text))
            {
                MessageBox.Show("Please Enter Category Name", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textCategoryName.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(richTextCategoryDesc.Text))
            {
                MessageBox.Show("Please Enter Category Description", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                richTextCategoryDesc.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(lblCatId.Text))
            {
                MessageBox.Show("Category ID is missing. Please select a category.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string categoryName = textCategoryName.Text.Trim();
            string categoryDesc = richTextCategoryDesc.Text.Trim();
            string categoryID = lblCatId.Text.Trim();

            try
            {
                dbConnect.OpenConnection();

                //Check if same name exists in another category (exclude current ID)
                SqlCommand checkCmd = new SqlCommand("SELECT COUNT(*) FROM categories WHERE category_name = @CategoryName AND id != @CategoryId", dbConnect.GetConnection());
                checkCmd.Parameters.AddWithValue("@CategoryName", categoryName);
                checkCmd.Parameters.AddWithValue("@CategoryId", categoryID);
                int count = (int)checkCmd.ExecuteScalar();

                if (count > 0)
                {
                    MessageBox.Show("Another category with this name already exists!", "Duplicate", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    dbConnect.CloseConnection();
                    return;
                }

                //Perform the update
                SqlCommand cmd = new SqlCommand("UPDATE categories SET category_name = @CategoryName, category_dece = @CategoryDesc WHERE id = @CategoryId", dbConnect.GetConnection());
                cmd.Parameters.AddWithValue("@CategoryName", categoryName);
                cmd.Parameters.AddWithValue("@CategoryDesc", categoryDesc);
                cmd.Parameters.AddWithValue("@CategoryId", categoryID);

                int rows = cmd.ExecuteNonQuery();
                dbConnect.CloseConnection();

                if (rows > 0)
                {
                    MessageBox.Show("Category updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    showCategoryList();
                    clearValue();

                    btnAddCategory.Visible = true;
                    btnUpdateCategory.Visible = false;
                    btnDeleteCategory.Visible = false;
                    lblCatId.Visible = false;
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


        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView1_Click(object sender, EventArgs e)
        {            

            if (dataGridView1.SelectedRows.Count > 0)
            {
                // Show update/delete buttons, hide add button
                btnAddCategory.Visible = false;
                lblCatId.Visible = false;
                btnUpdateCategory.Visible = true;
                btnDeleteCategory.Visible = true;

                DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];

                // Safely assign values to the text fields
                lblCatId.Text = selectedRow.Cells[0].Value?.ToString() ?? "";
                textCategoryName.Text = selectedRow.Cells[2].Value?.ToString() ?? "";
                richTextCategoryDesc.Text = selectedRow.Cells[3].Value?.ToString() ?? "";
            }

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void btnDeleteCategory_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(lblCatId.Text))
            {
                MessageBox.Show("Please select a category to delete.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult result = MessageBox.Show("Are you sure you want to delete this category?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                string categoryId = lblCatId.Text.Trim();

                try
                {
                    dbConnect.OpenConnection();

                    SqlCommand cmd = new SqlCommand("DELETE FROM categories WHERE id = @CategoryId", dbConnect.GetConnection());
                    cmd.Parameters.AddWithValue("@CategoryId", categoryId);

                    int rows = cmd.ExecuteNonQuery();
                    dbConnect.CloseConnection();

                    if (rows > 0)
                    {
                        MessageBox.Show("Category deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        showCategoryList();
                        clearValue();

                        // Reset buttons
                        btnAddCategory.Visible = true;
                        btnUpdateCategory.Visible = false;
                        btnDeleteCategory.Visible = false;
                        lblCatId.Visible = false;
                    }
                    else
                    {
                        MessageBox.Show("Delete failed. Category may not exist.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    dbConnect.CloseConnection();
                    MessageBox.Show("Error: " + ex.Message, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }


        }

       

        private void btnShowCategoryReport_Click(object sender, EventArgs e)
        {
            try
            {
                dbConnect.OpenConnection();
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM categories", dbConnect.GetConnection()))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    DataTable dt = new DataTable();
                    dt.Load(reader);

                    if (dt.Rows.Count > 0)
                    {
                        ShowCategoryCrystalForm showForm = new ShowCategoryCrystalForm();
                        string fullPath = Path.Combine(Application.StartupPath, "CategoryCrystalReport.rpt");
                        showForm.ReportName = fullPath;
                        showForm.ReportData = dt;
                        showForm.ShowDialog();
                    }
                    else
                    {
                        MessageBox.Show("No categories found.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading category list: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                dbConnect.CloseConnection();
            }
        }


        private void btnClear_Click(object sender, EventArgs e)
        {
            btnAddCategory.Visible = true;
            btnUpdateCategory.Visible = false;
            btnDeleteCategory.Visible = false;
            lblCatId.Visible = false;
            clearValue();
        }
        

        private void btnCategorySingleId_Click(object sender, EventArgs e)
        {
            string input = Interaction.InputBox("Enter Category Id", "Get By Category Id");

            if (!string.IsNullOrWhiteSpace(input))
            {
                if (int.TryParse(input, out int categoryId))
                {
                    using (SqlCommand cmd = new SqlCommand("SELECT * FROM categories WHERE id = @id", dbConnect.GetConnection()))
                    {
                        cmd.Parameters.AddWithValue("@id", categoryId);

                        dbConnect.OpenConnection();

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            DataTable dt = new DataTable();
                            dt.Load(reader);

                            if (dt.Rows.Count > 0)
                            {
                                ShowCategoryCrystalForm showForm = new ShowCategoryCrystalForm();
                                string fullPath = Path.Combine(Application.StartupPath, "ShowCategorySingleRecordCrystalReport.rpt");
                                showForm.ReportName = fullPath;
                                showForm.ReportData = dt;
                                showForm.ShowDialog();
                            }
                            else
                            {
                                MessageBox.Show("Category not found.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                        }

                        dbConnect.CloseConnection();
                    }
                }
                else
                {
                    MessageBox.Show("Invalid Category ID. Please enter a valid number.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {

        }
    }
}
