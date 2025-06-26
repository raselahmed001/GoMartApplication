using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GoMartApplication
{
    public partial class ProfileForm : Form
    {
        DBConnect dbConnect = new DBConnect();
        private string selectedPdfPath = "";
        public ProfileForm()
        {
            InitializeComponent();
        }

        private void textName_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void btnImage_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.bmp";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        
                        pictureBox1.Image = new Bitmap(openFileDialog.FileName);

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error loading image: " + ex.Message);
                    }
                }
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void btnUploadPDF_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "PDF Files|*.pdf";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {

                    textPdf.Text = Path.GetFileName(openFileDialog.FileName);
                    selectedPdfPath = openFileDialog.FileName;


                }
            }
        }
        

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textName.Text) || string.IsNullOrWhiteSpace(textEmail.Text))
            {
                MessageBox.Show("Please enter full name and email");
                return;
            }

            try
            {
                // Get form data
                string fullName = textName.Text.Trim();
                string email = textEmail.Text.Trim();
                string gender = checkBoxMale.Checked ? "Male" :
                                (checkBoxFemale.Checked ? "Female" : "Other");

                // Convert image to byte array
                byte[] profileImage = null;
                if (pictureBox1.Image != null)
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        // Use the correct image format
                        ImageFormat format = GetImageFormat(pictureBox1.Image);
                        pictureBox1.Image.Save(ms, format);
                        profileImage = ms.ToArray();

                        if (profileImage.Length == 0)
                        {
                            MessageBox.Show("Image conversion failed - empty byte array");
                            return;
                        }
                    }
                }

                // Read PDF file
                byte[] pdfFile = null;
                string pdfFileName = null;

                if (!string.IsNullOrEmpty(selectedPdfPath) && File.Exists(selectedPdfPath))
                {
                    pdfFile = File.ReadAllBytes(selectedPdfPath);
                    pdfFileName = Path.GetFileName(selectedPdfPath);

                    if (pdfFile.Length == 0)
                    {
                        MessageBox.Show("PDF file is empty");
                        return;
                    }
                }

                dbConnect.OpenConnection();

                using (SqlCommand cmd = new SqlCommand(
                    @"INSERT INTO Profiles 
                    (FullName, Email, Gender, ProfileImage, PDFFile, PDFFileName) 
                    VALUES 
                    (@FullName, @Email, @Gender, @ProfileImage, @PDFFile, @PDFFileName)",
                    dbConnect.GetConnection()))
                {
                    // Explicitly specify parameter types
                    cmd.Parameters.Add("@FullName", SqlDbType.NVarChar, 100).Value = fullName;
                    cmd.Parameters.Add("@Email", SqlDbType.NVarChar, 100).Value = email;
                    cmd.Parameters.Add("@Gender", SqlDbType.NVarChar, 10).Value = gender;

                    // Handle image parameter
                    SqlParameter imageParam = cmd.Parameters.Add("@ProfileImage", SqlDbType.VarBinary);
                    imageParam.Value = profileImage != null ? (object)profileImage : DBNull.Value;

                    // Handle PDF parameters
                    SqlParameter pdfParam = cmd.Parameters.Add("@PDFFile", SqlDbType.VarBinary);
                    pdfParam.Value = pdfFile != null ? (object)pdfFile : DBNull.Value;

                    cmd.Parameters.Add("@PDFFileName", SqlDbType.NVarChar, 255).Value =
                        !string.IsNullOrEmpty(pdfFileName) ? (object)pdfFileName : DBNull.Value;

                    int result = cmd.ExecuteNonQuery();

                    if (result > 0)
                    {
                        MessageBox.Show("Profile saved successfully!", "Success",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        clearForm();
                        showList();
                    }
                    else
                    {
                        MessageBox.Show("Profile save failed.", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving profile: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                dbConnect.CloseConnection();
            }
        }

        private ImageFormat GetImageFormat(Image image)
        {
            if (ImageFormat.Jpeg.Equals(image.RawFormat))
                return ImageFormat.Jpeg;
            else if (ImageFormat.Png.Equals(image.RawFormat))
                return ImageFormat.Png;
            else if (ImageFormat.Gif.Equals(image.RawFormat))
                return ImageFormat.Gif;
            else if (ImageFormat.Bmp.Equals(image.RawFormat))
                return ImageFormat.Bmp;
            else
                return ImageFormat.Png;
        }

        private void clearForm()
        {
            textName.Clear();
            textEmail.Clear();
            checkBoxMale.Checked = false;
            checkBoxFemale.Checked = false;
            textPdf.Clear();
            pictureBox1.Image = null;
            selectedPdfPath = "";
        }
        

        private void dataGridView1_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow != null && dataGridView1.CurrentRow.Index >= 0)
            {
                DataGridViewRow selectedRow = dataGridView1.CurrentRow;

                textName.Text = selectedRow.Cells["Full Name"].Value?.ToString();
                textEmail.Text = selectedRow.Cells["Email"].Value?.ToString();

                string gender = selectedRow.Cells["Gender"].Value?.ToString();
                checkBoxMale.Checked = gender == "Male";
                checkBoxFemale.Checked = gender == "Female";

                // You can also show PDFFileName if needed
                //textPdf.Text = selectedRow.Cells["PDF File Name"].Value?.ToString();
            }
        }


        private void showList()
        {
            try
            {
                SqlCommand cmd = new SqlCommand(@"
                SELECT 
                    Id, 
                    FullName AS [Full Name], 
                    Email, 
                    Gender, 
      
                    CreatedDate AS [Created Date] 
                FROM Profiles", dbConnect.GetConnection());

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

                // Hide the primary key (Id) for UI
                dataGridView1.Columns["Id"].Visible = false;
            }
            catch (Exception ex)
            {
                dbConnect.CloseConnection();
                MessageBox.Show("Error loading profile list: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ProfileForm_Load(object sender, EventArgs e)
        {
            showList();
        }
    }
}
