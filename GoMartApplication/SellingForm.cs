using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GoMartApplication
{
    public partial class SellingForm : Form
    {
        DBConnect dbConnect = new DBConnect();

        public SellingForm()
        {
            InitializeComponent();
          
        }

        double GrandTotal = 0.0;
        int n = 0;

        private void SellingForm_Load(object sender, EventArgs e)
        {
            LoadSearchCategory();            
            showList();
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

                dataGridView2_product.DataSource = dt;

                // Hide id and category_id for cleaner UI
                dataGridView2_product.Columns["id"].Visible = false;
                dataGridView2_product.Columns["category_id"].Visible = false;
            }
            catch (Exception ex)
            {
                dbConnect.CloseConnection();
                MessageBox.Show("Error loading product list: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        private void btnReferesh_Click(object sender, EventArgs e)
        {

                      
          
        }

        private void dataGridView2_product_Click(object sender, EventArgs e)
        {
            if (dataGridView2_product.CurrentRow != null && dataGridView2_product.CurrentRow.Index >= 0)
            {
               
                LID.Visible = true;

                DataGridViewRow selectedRow = dataGridView2_product.CurrentRow;

                LID.Clear();
                LID.Text = selectedRow.Cells["id"].Value?.ToString() ?? "";
                txtProductName.Clear();
                txtProductName.Text = selectedRow.Cells["Product Name"].Value?.ToString() ?? "";
                textPrice.Clear();
                textPrice.Text = selectedRow.Cells["Price"].Value?.ToString() ?? "";
                txtQuantity.Focus();
                txtQuantity.Clear();

            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
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

                    dataGridView2_product.DataSource = dt;
                }
                catch (Exception ex)
                {
                    dbConnect.CloseConnection();
                    MessageBox.Show("Error filtering products: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {

        }

        private void btnAddOrder_Click(object sender, EventArgs e)
        {
            try
            {
                if (textPrice.Text == "" || txtQuantity.Text == "")
                {
                    MessageBox.Show("Enter valid Price or quantity", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                }
                else 
                {
                    double Total = Convert.ToDouble(textPrice.Text) * Convert.ToInt32(txtQuantity.Text);

                    DataGridViewRow dr = new DataGridViewRow();
                    dr.CreateCells(dataGridView1_order);

                    dr.Cells[0].Value = ++n;
                    dr.Cells[1].Value = txtProductName.Text;
                    dr.Cells[2].Value = txtQuantity.Text;
                    dr.Cells[3].Value = textPrice.Text;
                    dr.Cells[4].Value = Total;

                    dataGridView1_order.Rows.Add(dr);
                    GrandTotal += Total;

                    //Show total
                    lblGrandTotal.Text = $"Total: {GrandTotal:0.00} Tk";
                }
               
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adding order: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void button4_Click(object sender, EventArgs e)
        {
            showList();
            comboSearch.SelectedIndex = 0;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnAddBill_Click(object sender, EventArgs e)
        {
            try
            {
                if(txtBillNum.Text == "")
                {
                    MessageBox.Show("Please enter bill number.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                else
                {
                    int sellerId = Convert.ToInt32(txtBillNum.Text.Trim());
                    decimal totalAmount = Convert.ToDecimal(txtQuantity.Text.Trim());
                    DateTime sellDate = DateTime.Now;

                    dbConnect.OpenConnection();

                    SqlCommand cmd = new SqlCommand("INSERT INTO bills (seller_id, sell_date, total_amount) VALUES (@seller_id, @sell_date, @total_amount)", dbConnect.GetConnection());
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@seller_id", sellerId);
                    cmd.Parameters.AddWithValue("@sell_date", sellDate);
                    cmd.Parameters.AddWithValue("@total_amount", totalAmount);

                    int rows = cmd.ExecuteNonQuery();
                    dbConnect.CloseConnection();

                    if (rows > 0)
                    {
                        BindBillList();
                        MessageBox.Show("Bill added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    
                    }
                    else
                    {
                        MessageBox.Show("Bill not added.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                dbConnect.CloseConnection();
                MessageBox.Show("Error filtering products: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


    private void BindBillList()
    {
        try
        {
            SqlCommand cmd = new SqlCommand(@"
                SELECT 
                    b.bill_id AS [Bill No],
                    s.SellerName AS [Seller Name],
                    b.sell_date AS [Sell Date],
                    b.total_amount AS [Total Amount]
                FROM bills b
                INNER JOIN tb1Seller s ON b.seller_id = s.SellerID
                ORDER BY b.bill_id DESC", dbConnect.GetConnection());

            dbConnect.OpenConnection();

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            dataGridView1_selling.DataSource = dt;

            dbConnect.CloseConnection();
        }
        catch (Exception ex)
        {
            dbConnect.CloseConnection();
            MessageBox.Show("Error loading bills: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
     }

        private void dataGridView2_product_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
