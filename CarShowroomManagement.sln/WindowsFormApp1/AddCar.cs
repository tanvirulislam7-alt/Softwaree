using CarShowroomManagement;
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
using System.Xml.Linq;

namespace WindowsFormsApp1
{
    public partial class AddCar : Form
    {
        private Option optionForm;
        private LogP loginForm;

        public AddCar(Option option, LogP login)
        {
            InitializeComponent();
            this.optionForm = option;
            this.loginForm = login;
        }

        public AddCar()
        {
            InitializeComponent();
        }

        private void NewData()
        {
            txtID.Text = "Auto Generated";
            cmbType.SelectedValue = -1;
            cmbBrand.SelectedValue = -1;
            txtModel.Text = "";
            txtCC.Text = "";
            txtColor.Text = "";
            txtPrice.Text = "";
            dtpDop.Value = DateTime.Now;
            cmbDealer.SelectedValue = -1;
            dgvAddCar.ClearSelection();
        }

        private void btnLogOut_Click(object sender, EventArgs e)
        {
            this.Close();
            this.optionForm.Hide();
            this.loginForm.Show();
        }

        private void btnBack_Click_1(object sender, EventArgs e)
        {
            this.optionForm.Show();
            this.Close();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            NewData();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            string id = txtID.Text;

            if (cmbType.SelectedValue == null)
            {
                MessageBox.Show("Error : Type is required");
                return;
            }
            string type = cmbType.SelectedValue.ToString();

            if (cmbBrand.SelectedValue == null)
            {
                MessageBox.Show("Error : Brand is required");
                return;
            }
            string brand = cmbBrand.SelectedValue.ToString();

            string model = txtModel.Text;
            if (string.IsNullOrWhiteSpace(model))
            {
                MessageBox.Show("Please enter Model Name.");
                return;
            }

            string cc = txtCC.Text;
            if (string.IsNullOrWhiteSpace(cc))
            {
                MessageBox.Show("Please enter CC.");
                return;
            }

            string color = txtColor.Text;
            if (string.IsNullOrWhiteSpace(color))
            {
                MessageBox.Show("Please enter Color.");
                return;
            }

            string price = txtPrice.Text;
            if (string.IsNullOrWhiteSpace(price))
            {
                MessageBox.Show("Please enter Price.");
                return;
            }

            // FIX: Format date properly for SQL Server
            string dop = dtpDop.Value.ToString("yyyy-MM-dd");

            if (cmbDealer.SelectedValue == null)

            {
                MessageBox.Show("Error : Dealer is required");
                return;
            }
            string dealer = cmbDealer.SelectedValue.ToString();

            try
            {
                var con = new SqlConnection();
                con.ConnectionString = ApplicationHelper.ConnectionPath;
                con.Open();

                var cmd = new SqlCommand();
                cmd.Connection = con;

                // BETTER FIX: Use parameterized queries (prevents SQL injection too!)
                if (id == "Auto Generated")
                {
                    cmd.CommandText = "INSERT INTO CarInfo (CTID, BID, Model, CC, Color, Price, DateOfPurchase, DID) VALUES (@type, @brand, @model, @cc, @color, @price, @dop, @dealer)";
                }
                else
                {
                    cmd.CommandText = "UPDATE CarInfo SET CTID = @type, BID = @brand, Model = @model, CC = @cc, Color = @color, Price = @price, DateOfPurchase = @dop, DID = @dealer WHERE ID = @id";
                    cmd.Parameters.AddWithValue("@id", id);
                }

                cmd.Parameters.AddWithValue("@type", type);
                cmd.Parameters.AddWithValue("@brand", brand);
                cmd.Parameters.AddWithValue("@model", model);
                cmd.Parameters.AddWithValue("@cc", cc);
                cmd.Parameters.AddWithValue("@color", color);
                cmd.Parameters.AddWithValue("@price", price);
                cmd.Parameters.AddWithValue("@dop", dop);
                cmd.Parameters.AddWithValue("@dealer", dealer);

                cmd.ExecuteNonQuery();
                MessageBox.Show("Done", "Confirmation", MessageBoxButtons.OK, MessageBoxIcon.Information);

                this.LoadData();
                this.NewData();

                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadData();
            NewData();
        }

        private void AddCar_Load_1(object sender, EventArgs e)
        {
            
            
            LoadData();
            NewData();
        }

        private void LoadData()
        {
            try
            {
                var con = new SqlConnection();
                con.ConnectionString = ApplicationHelper.ConnectionPath;
                con.Open();

                var cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = "select CarInfo.ID ,CarType.Type,CarBrand.BrandName 'Brand',Model, CC,Color,Price,DateOfPurchase,CarDealer.DealerName, CarInfo.CTID, CarInfo.BID,CarInfo.DID from CarInfo inner join CarType on CarType.ID= CarInfo.CTID inner join CarBrand on CarBrand.ID= CarInfo.BID inner join CarDealer on CarDealer.ID = CarInfo.DID;select * from CarType;select * from CarBrand;select * from CarDealer;";

                DataSet ds = new DataSet();
                SqlDataAdapter adp = new SqlDataAdapter(cmd);
                adp.Fill(ds);

                dgvAddCar.AutoGenerateColumns = false;
                dgvAddCar.DataSource = ds.Tables[0];
                dgvAddCar.Refresh();
                dgvAddCar.ClearSelection();

                cmbType.DataSource = ds.Tables[1];
                cmbType.DisplayMember = "Type";
                cmbType.ValueMember = "ID";

                cmbBrand.DataSource = ds.Tables[2];
                cmbBrand.DisplayMember = "BrandName";
                cmbBrand.ValueMember = "ID";

                cmbDealer.DataSource = ds.Tables[3];
                cmbDealer.DisplayMember = "DealerName";
                cmbDealer.ValueMember = "ID";

                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void dgvAddCar_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            try
            {
                // Actual column order in DataGridView:
                // 0=ID, 1=CTID, 2=BID, 3=Model, 4=CC, 5=Color, 6=Price, 7=DateOfPurchase, 8=DID

                txtID.Text = dgvAddCar.Rows[e.RowIndex].Cells[0].Value?.ToString() ?? "";
                txtModel.Text = dgvAddCar.Rows[e.RowIndex].Cells[3].Value?.ToString() ?? "";
                txtCC.Text = dgvAddCar.Rows[e.RowIndex].Cells[4].Value?.ToString() ?? "";
                txtColor.Text = dgvAddCar.Rows[e.RowIndex].Cells[5].Value?.ToString() ?? "";
                txtPrice.Text = dgvAddCar.Rows[e.RowIndex].Cells[6].Value?.ToString() ?? "";

                // FIX: ComboBox SelectedValue needs integer, not string
                var ctidValue = dgvAddCar.Rows[e.RowIndex].Cells[1].Value;
                if (ctidValue != null && ctidValue != DBNull.Value)
                {
                    cmbType.SelectedValue = Convert.ToInt32(ctidValue);
                }

                var bidValue = dgvAddCar.Rows[e.RowIndex].Cells[2].Value;
                if (bidValue != null && bidValue != DBNull.Value)
                {
                    cmbBrand.SelectedValue = Convert.ToInt32(bidValue);
                }

                var didValue = dgvAddCar.Rows[e.RowIndex].Cells[8].Value;
                if (didValue != null && didValue != DBNull.Value)
                {
                    cmbDealer.SelectedValue = Convert.ToInt32(didValue);
                }

                // FIX: Safely handle date conversion with null check
                var dateValue = dgvAddCar.Rows[e.RowIndex].Cells[7].Value;
                if (dateValue != null && dateValue != DBNull.Value)
                {
                    dtpDop.Value = Convert.ToDateTime(dateValue);
                }
                else
                {
                    dtpDop.Value = DateTime.Now;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            string id = txtID.Text;
            if (id == "Auto Generated")
            {
                MessageBox.Show("Please select a row first", "Incorrect", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var result = MessageBox.Show("Are you sure?", "Information", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (result == DialogResult.No)
                return;

            if (result == DialogResult.Yes)
            {
                try
                {
                    var con = new SqlConnection();
                    con.ConnectionString = ApplicationHelper.ConnectionPath;
                    con.Open();

                    var cmd = new SqlCommand();
                    cmd.Connection = con;
                    cmd.CommandText = "DELETE FROM CarInfo WHERE id = @id";
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Delete Done");

                    this.LoadData();
                    this.NewData();

                    con.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }

        private void AddCar_FormClosing(object sender, FormClosingEventArgs e)
        {
            //Application.Exit();
        }

        private void dgvAddCar_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }
    }
}