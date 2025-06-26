using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GoMartApplication
{
    public partial class AddProducts : Form
    {
        DBConnect dbConnect = new DBConnect();
        public AddProducts()
        {
            InitializeComponent();
        }

        private void AddProducts_Load(object sender, EventArgs e)
        {
            BindCategory();
            LoadSearchCategory();
            LID.Visible = false;
            btnAdd.Visible = true;
            btnUpdate.Visible = false;
            btnDelete.Visible = false;
            textProductName.Focus();
            showList();
        }

        private void BindCategory()
        {

            try
            {
                SqlCommand cmd = new SqlCommand("SELECT id, category_name FROM categories", dbConnect.GetConnection());
                dbConnect.OpenConnection();

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                dbConnect.CloseConnection();

                comboCategoryId.DataSource = dt;
                comboCategoryId.DisplayMember = "category_name";
                comboCategoryId.ValueMember = "id";              
                comboCategoryId.SelectedIndex = 0;              
            }
            catch (Exception ex)
            {
                dbConnect.CloseConnection();
                MessageBox.Show("Error loading categories: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            // Validation
            if (string.IsNullOrWhiteSpace(textProductName.Text))
            {
                MessageBox.Show("Please Enter Product Name", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textProductName.Focus();
                return;
            }

            if (comboCategoryId.SelectedIndex == 0 || comboCategoryId.SelectedValue == null)
            {
                MessageBox.Show("Please select a Category", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                comboCategoryId.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(textQuantity.Text))
            {
                MessageBox.Show("Please Enter Quantity", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textQuantity.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(textPrice.Text))
            {
                MessageBox.Show("Please Enter Price", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textPrice.Focus();
                return;
            }

            string productName = textProductName.Text.Trim();
            int categoryId = Convert.ToInt32(comboCategoryId.SelectedValue);
            decimal price = decimal.Parse(textPrice.Text.Trim());
            int quantity = int.Parse(textQuantity.Text.Trim());

            try
            {
                dbConnect.OpenConnection();                

                // Insert if not duplicate
                SqlCommand cmd = new SqlCommand("INSERT INTO products (product_name, category_id, quantity, price) VALUES (@product_name, @category_id, @quantity, @price)", dbConnect.GetConnection());
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@product_name", productName);
                cmd.Parameters.AddWithValue("@category_id", categoryId);
                cmd.Parameters.AddWithValue("@quantity", quantity);
                cmd.Parameters.AddWithValue("@price", price);

                int rows = cmd.ExecuteNonQuery();
                dbConnect.CloseConnection();

                if (rows > 0)
                {
                    MessageBox.Show("Product added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    showList();
                    clearValue();
                }
                else
                {
                    MessageBox.Show("Product not added.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                dbConnect.CloseConnection();
                MessageBox.Show("Error: " + ex.Message, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(LID.Text))
            {
                MessageBox.Show("Please select a product to update.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(textProductName.Text))
            {
                MessageBox.Show("Please Enter Product Name", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textProductName.Focus();
                return;
            }

            if (comboCategoryId.SelectedIndex == 0 || comboCategoryId.SelectedValue == null)
            {
                MessageBox.Show("Please select a Category", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                comboCategoryId.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(textQuantity.Text))
            {
                MessageBox.Show("Please Enter Quantity", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textQuantity.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(textPrice.Text))
            {
                MessageBox.Show("Please Enter Price", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textPrice.Focus();
                return;
            }

            int id = Convert.ToInt32(LID.Text.Trim());
            string productName = textProductName.Text.Trim();
            int categoryId = Convert.ToInt32(comboCategoryId.SelectedValue);
            int quantity = int.Parse(textQuantity.Text.Trim());
            decimal price = decimal.Parse(textPrice.Text.Trim());

            try
            {
                dbConnect.OpenConnection();
                SqlCommand cmd = new SqlCommand("UPDATE products SET product_name = @product_name, category_id = @category_id, quantity = @quantity, price = @price WHERE id = @id", dbConnect.GetConnection());
                cmd.Parameters.AddWithValue("@product_name", productName);
                cmd.Parameters.AddWithValue("@category_id", categoryId);
                cmd.Parameters.AddWithValue("@quantity", quantity);
                cmd.Parameters.AddWithValue("@price", price);
                cmd.Parameters.AddWithValue("@id", id);

                int rows = cmd.ExecuteNonQuery();
                dbConnect.CloseConnection();

                if (rows > 0)
                {
                    MessageBox.Show("Product updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    showList();
                    clearValue();
                    btnAdd.Visible = true;
                    btnUpdate.Visible = false;
                    btnDelete.Visible = false;
                }
                else
                {
                    MessageBox.Show("Update failed!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                MessageBox.Show("Please select a product to delete.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult result = MessageBox.Show("Are you sure you want to delete this product?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                int id = Convert.ToInt32(LID.Text.Trim());

                try
                {
                    dbConnect.OpenConnection();
                    SqlCommand cmd = new SqlCommand("DELETE FROM products WHERE id = @id", dbConnect.GetConnection());
                    cmd.Parameters.AddWithValue("@id", id);

                    int rows = cmd.ExecuteNonQuery();
                    dbConnect.CloseConnection();

                    if (rows > 0)
                    {
                        MessageBox.Show("Product deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        showList();
                        clearValue();
                        btnAdd.Visible = true;
                        btnUpdate.Visible = false;
                        btnDelete.Visible = false;
                    }
                    else
                    {
                        MessageBox.Show("Delete failed!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            textProductName.Clear();
            textQuantity.Clear();
            textPrice.Clear();
            comboCategoryId.SelectedIndex = 0;
        }

        private void dataGridView1_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow != null && dataGridView1.CurrentRow.Index >= 0)
            {
                btnAdd.Visible = false;
                btnUpdate.Visible = true;
                btnDelete.Visible = true;
                LID.Visible = false;

                DataGridViewRow selectedRow = dataGridView1.CurrentRow;

                textProductName.Text = selectedRow.Cells["Product Name"].Value?.ToString() ?? "";
                textQuantity.Text = selectedRow.Cells["Quantity"].Value?.ToString() ?? "";
                textPrice.Text = selectedRow.Cells["Price"].Value?.ToString() ?? "";

                // Set category from hidden column (category_id)
                if (selectedRow.Cells["category_id"].Value != null)
                {
                    comboCategoryId.SelectedValue = Convert.ToInt32(selectedRow.Cells["category_id"].Value);
                }

                LID.Text = selectedRow.Cells["id"].Value?.ToString() ?? "";
            }
        }


        

        private void showList()
        {
            try
            {
                SqlCommand cmd = new SqlCommand(@"
            SELECT 
                p.id,
                p.product_name AS [Product Name],
                c.category_name AS [Category Name],
                p.quantity AS [Quantity],
                p.price AS [Price],
                p.category_id 
            FROM products p
            INNER JOIN categories c ON p.category_id = c.id", dbConnect.GetConnection());

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

                dt.Columns["S.No"].SetOrdinal(0);

                dataGridView1.DataSource = dt;

                // Hide id and category_id for cleaner UI
                dataGridView1.Columns["id"].Visible = false;
                dataGridView1.Columns["category_id"].Visible = false;
            }
            catch (Exception ex)
            {
                dbConnect.CloseConnection();
                MessageBox.Show("Error loading product list: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadSearchCategory()
        {
            try
            {
                SqlCommand cmd = new SqlCommand("SELECT id, category_name FROM categories", dbConnect.GetConnection());
                dbConnect.OpenConnection();

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                dbConnect.CloseConnection();

                // Add "--Select Category--" manually
                DataRow row = dt.NewRow();
                row["id"] = 0;
                row["category_name"] = "-- Select Category --";
                dt.Rows.InsertAt(row, 0);

                comboSearch.DataSource = dt;
                comboSearch.DisplayMember = "category_name";
                comboSearch.ValueMember = "id";
                comboSearch.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                dbConnect.CloseConnection();
                MessageBox.Show("Error loading search categories: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void comboSearch_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboSearch.SelectedValue != null && int.TryParse(comboSearch.SelectedValue.ToString(), out int selectedCategoryId))
            {
                if (selectedCategoryId == 0)
                {
                    showList(); 
                    return;
                }

                try
                {
                    SqlCommand cmd = new SqlCommand(
                        "SELECT p.id, p.product_name AS [Product Name], c.category_name AS [Category Name], p.quantity AS [Quantity], p.price AS [Price] " +
                        "FROM products p " +
                        "INNER JOIN categories c ON p.category_id = c.id " +
                        "WHERE p.category_id = @category_id", dbConnect.GetConnection());
                    cmd.Parameters.AddWithValue("@category_id", selectedCategoryId);

                    dbConnect.OpenConnection();
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    dbConnect.CloseConnection();

                    // Add serial number
                    dt.Columns.Add("S.No", typeof(int));
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        dt.Rows[i]["S.No"] = i + 1;
                    }
                    dt.Columns["S.No"].SetOrdinal(0);

                    dataGridView1.DataSource = dt;
                }
                catch (Exception ex)
                {
                    dbConnect.CloseConnection();
                    MessageBox.Show("Error filtering products: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnReferesh_Click(object sender, EventArgs e)
        {
            clearValue();              
            showList();                 
            comboSearch.SelectedIndex = 0;
            btnAdd.Visible = true;
            btnUpdate.Visible = false;
            btnDelete.Visible = false;
            LID.Visible = false;
            textProductName.Focus();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
