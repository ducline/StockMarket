using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySqlConnector;


namespace StockMarket
{
    public partial class Login : Form
    {


        public void LogtoForm()
        {
            string connetionString = null;
            MySqlConnection cnn;
            connetionString = "datasource=192.168.1.1;port=3306;username=psb202199;password=psb202199;database=psb202199_stockmarket;";
            cnn = new MySqlConnection(connetionString);

            try
            {
                cnn.Open();

                MySqlCommand command = cnn.CreateCommand();
                command.CommandText = "SELECT * FROM user";
                MySqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    if (textBox1.Text == reader.GetString(2) && textBox2.Text == reader.GetString(3))
                    {
                        Form Form1 = new Form1(reader.GetString(2), reader.GetInt32(0).ToString());
                        this.Hide();
                        Form1.Show();
                    }
                }



                cnn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Can not open connection ! ");
                MessageBox.Show(Convert.ToString(ex));
            }
        }
        public Login()
        {
            InitializeComponent();
        }

        private void label3_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void Login_Load(object sender, EventArgs e)
        {
            textBox1.Select();
            textBox1.Focus();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            LogtoForm();
        }

        private void textBox2_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                LogtoForm();
            }
        }
    }
}
