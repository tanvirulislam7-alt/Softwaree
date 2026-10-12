using CarShowroomManagement;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class CustomerInfo : Form
    {
        private LogP loginForm;
        private Option optionForm;

        public CustomerInfo()
        {
            InitializeComponent();
        }

        public CustomerInfo(Option option, LogP login) : this()
        {
            this.optionForm = option;
            this.loginForm = login;
        }

        // ✅ MUST MATCH DESIGNER
        private void Form1_Load(object sender, EventArgs e)
        {
            LoadData();
            NewData();
        }

        private void LoadData()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(ApplicationHelper.ConnectionPath))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("SELECT * FROM CustomerInfo", con);

                    DataTable dt = new DataTable();
                    SqlDataAdapter adp = new SqlDataAdapter(cmd);
                    adp.Fill(dt);

                    dgvData.AutoGenerateColumns = false;
                    dgvData.DataSource = dt;
                    dgvData.ClearSelection();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void NewData()
        {
            txtID.Text = "Auto generated";
            txtFullName.Text = "";
            rbMale.Checked = false;
            rbFemale.Checked = false;
            txtPhone.Text = "";
            txtAdd.Text = "";
            dgvData.ClearSelection();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadData();
            NewData();
        }

        private void dgvData_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            txtID.Text = dgvData.Rows[e.RowIndex].Cells[0].Value?.ToString();
            txtFullName.Text = dgvData.Rows[e.RowIndex].Cells[1].Value?.ToString();

            string gender = dgvData.Rows[e.RowIndex].Cells[2].Value?.ToString();
            rbMale.Checked = (gender == "Male");
            rbFemale.Checked = (gender == "Female");

            txtPhone.Text = dgvData.Rows[e.RowIndex].Cells[3].Value?.ToString();
            txtAdd.Text = dgvData.Rows[e.RowIndex].Cells[4].Value?.ToString();
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            NewData();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (txtID.Text == "Auto generated")
            {
                MessageBox.Show("Select a row first.");
                return;
            }

            if (MessageBox.Show("Are you sure?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.No)
                return;

            try
            {
                using (SqlConnection con = new SqlConnection(ApplicationHelper.ConnectionPath))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("DELETE FROM CustomerInfo WHERE ID=@id", con);
                    cmd.Parameters.AddWithValue("@id", txtID.Text);

                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Deleted successfully");

                    LoadData();
                    NewData();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            string id = txtID.Text;
            string name = txtFullName.Text.Trim();

            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("Enter Full Name.");
                return;
            }

            string gender = rbMale.Checked ? "Male" :
                            rbFemale.Checked ? "Female" : "";

            if (gender == "")
            {
                MessageBox.Show("Select Gender.");
                return;
            }

            string phone = txtPhone.Text.Trim();
            if (string.IsNullOrWhiteSpace(phone))
            {
                MessageBox.Show("Enter Phone.");
                return;
            }

            string address = txtAdd.Text.Trim();
            if (string.IsNullOrWhiteSpace(address))
            {
                MessageBox.Show("Enter Address.");
                return;
            }

            try
            {
                using (SqlConnection con = new SqlConnection(ApplicationHelper.ConnectionPath))
                {
                    con.Open();
                    SqlCommand cmd;

                    if (id == "Auto generated")
                    {
                        cmd = new SqlCommand(
                            "INSERT INTO CustomerInfo (FullName, Gender, PhoneNumber, Address) VALUES (@name,@gender,@phone,@address)", con);
                    }
                    else
                    {
                        cmd = new SqlCommand(
                            "UPDATE CustomerInfo SET FullName=@name, Gender=@gender, PhoneNumber=@phone, Address=@address WHERE ID=@id", con);
                        cmd.Parameters.AddWithValue("@id", id);
                    }

                    cmd.Parameters.AddWithValue("@name", name);
                    cmd.Parameters.AddWithValue("@gender", gender);
                    cmd.Parameters.AddWithValue("@phone", phone);
                    cmd.Parameters.AddWithValue("@address", address);

                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Saved successfully");

                    LoadData();
                    NewData();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void backBtn_Click(object sender, EventArgs e)
        {
            this.Hide();
            optionForm?.Show();
        }

        private void btnlgt_Click(object sender, EventArgs e)
        {
            this.Hide();
            loginForm?.Show();
        }

        // ✅ REQUIRED (to fix designer crash)
        private void dgvData_CellContentClick(object sender, DataGridViewCellEventArgs e) { }
        private void textBox1_TextChanged(object sender, EventArgs e) { }
        private void label1_Click(object sender, EventArgs e) { }
    }
}