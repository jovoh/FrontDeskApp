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

namespace FrontDeskApp
{//
    public partial class Form1 : Form
    {
        const int maxsmall = 45;
        const int maxmedium = 14;
        const int maxlarge = 12;

        private int small = 0;
        private int medium = 0;
        private int large = 0;


        String strappid; 

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = 0;
            comboBox2.SelectedIndex = 0;
            comboBox3.SelectedIndex = 1;
            RefreshFaciltyStatus();
            RefreshAll();

        }

        private void button1_Click(object sender, EventArgs e)
        {
             if (AllowPackage()==true)
                   CheckInPackage();
        }

        private void button2_Click(object sender, EventArgs e)
        {
          if (strappid == "" || strappid ==null)
                MessageBox.Show("Select an item on the grid and double click", "FrontDeskApp", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
          else 
            CheckoutPackage();
        }
        private bool AllowPackage()
        {
            if (comboBox1.Text == "Small")
            {
                if (small >= maxsmall)
                {
                    MessageBox.Show( "Maximum Small Boxes  Reached " + maxsmall, "FrontDeskApp", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }
                else
                    return true;
            }
            else if (comboBox1.Text == "Medium")
            {
                if (medium >= maxmedium)
                {
                    MessageBox.Show("Maximum Medium Boxes  Reached " + maxmedium, "FrontDeskApp", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }
                else
                    return true;

            }
            else
            {
                if (large >= maxlarge)
                {
                    MessageBox.Show("Maximum Medium Boxes  Reached " + maxmedium, "FrontDeskApp", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }
                else
                    return true;
            }
        }
        private  SqlConnection getConnection()
        {
            //Server=myServerAddress;Database=myDataBase;User Id=myUsername;Password=myPassword;
            SqlConnection con = new SqlConnection("Server=localhost\\SQLDEVELOPER;Database=FrontDeskApp;User Id=dev;Password=sqldev;");
            return con;

        }
        
        private void CheckoutPackage()
        {
            SqlConnection con = getConnection();
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("UPDATE Items   SET CheckOut = @CheckOut WHERE AppId = @AppId", con);
                //cmd.Connection = con;
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Add("AppId", SqlDbType.Int, 4).Value = strappid;
                
                cmd.Parameters.Add("CheckOut", SqlDbType.SmallDateTime, 20).Value = DateTime.Now;

                cmd.ExecuteNonQuery();
                RefreshAll();
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.Message);
            }
        }
        private void CheckInPackage()
        {
            SqlConnection con = getConnection();
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("INSERT INTO Items(LastName,FirstName,PhoneNo,ItemSize,Facility,CheckIn) VALUES(@LastName,@FirstName,@PhoneNo,@ItemSize,@Facility,@CheckIn)", con);
                //cmd.Connection = con;
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Add("LastName", SqlDbType.NVarChar, 50).Value = textBox1.Text;
                cmd.Parameters.Add("FirstName", SqlDbType.NVarChar, 50).Value = textBox2.Text;
                cmd.Parameters.Add("PhoneNo", SqlDbType.NVarChar, 50).Value = textBox3.Text;
                cmd.Parameters.Add("ItemSize", SqlDbType.NVarChar, 10).Value = comboBox1.Text;
                cmd.Parameters.Add("Facility", SqlDbType.NVarChar, 2).Value = comboBox2.Text;
                cmd.Parameters.Add("CheckIn", SqlDbType.SmallDateTime, 20).Value = DateTime.Now;

                cmd.ExecuteNonQuery();
                RefreshAll();
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.Message);
            }

        }

        public void RefreshFaciltyStatus() {

            small = int.Parse( GetFacilityStatus("Small").ToString());
            medium = int.Parse(GetFacilityStatus("Medium").ToString());
            large  = int.Parse(GetFacilityStatus("Large").ToString());

            label6.Text = "Small: " + GetFacilityStatus("Small") + "/45";
            label7.Text = "Medium: " + GetFacilityStatus("Medium") + "/14";
            label8.Text = "Large: " + GetFacilityStatus("Large") + "/12";


        }
        private String GetFacilityStatus(String itemSize) { 

            SqlConnection con = getConnection();

            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("select count(AppId) as ItemCount from Items where ItemSize=@ItemSize and Facility = @Facility and CheckOut is null", con);

                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Add("ItemSize", SqlDbType.NVarChar, 10).Value = itemSize;
                cmd.Parameters.Add("Facility", SqlDbType.NVarChar, 2).Value = comboBox2.Text;


                SqlDataReader dr = null;

                dr = cmd.ExecuteReader();
                dr.Read();
                return dr["ItemCount"].ToString();
            }
            catch(SqlException e)
            {
                Console.WriteLine(e.Message);
                return "0";
            }

        }

        private void GetPackages(String view)
        {

            SqlConnection con = getConnection();
            String strsql = "";
            try
            {
                con.Open();
                if (view=="All")
                    strsql= "select * from Items where Facility = @Facility";
                else if (view=="In")
                    strsql = "select * from Items where ItemSize=@ItemSize and Facility = @Facility and Checkout is Null";
                else
                    strsql = "select * from Items where ItemSize=@ItemSize and Facility = @Facility and Checkout is not Null";

                SqlCommand cmd = new SqlCommand(strsql, con);

                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Add("ItemSize", SqlDbType.NVarChar, 10).Value = comboBox1.Text;
                cmd.Parameters.Add("Facility", SqlDbType.NVarChar, 2).Value = comboBox2.Text;


                SqlDataAdapter dataAdapter = new SqlDataAdapter();
                dataAdapter.SelectCommand = cmd;
                DataSet ds = new DataSet();
                dataAdapter.Fill(ds);
                dataGridView1.DataSource = ds.Tables[0];


            }
            catch (SqlException e)
            {
                Console.WriteLine(e.Message);
            }

        }



        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshAll();
        }


        

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshAll();
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshAll();
        }
        private void RefreshAll()
        {
            RefreshFaciltyStatus();
            GetPackages(comboBox3.Text);
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            strappid = dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString();
            textBox1.Text = dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString();
            textBox2.Text = dataGridView1.Rows[e.RowIndex].Cells[2].Value.ToString();
            textBox3.Text = dataGridView1.Rows[e.RowIndex].Cells[3].Value.ToString();
            comboBox1.Text = dataGridView1.Rows[e.RowIndex].Cells[4].Value.ToString();
            comboBox2.Text = dataGridView1.Rows[e.RowIndex].Cells[5].Value.ToString();

            Console.WriteLine(strappid);
        }

    }
}
