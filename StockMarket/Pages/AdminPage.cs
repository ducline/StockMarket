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
using MySqlConnector;
using static StockMarket.Login;

namespace StockMarket
{
    public partial class AdminPage : Form
    {
        string connectionString = "datasource=192.168.1.1;port=3306;username=psb202199;password=psb202199;database=psb202199_stockmarket;";
        string userid;
        private Label labelToUpdate;
        public AdminPage(string id, Label label)
        {
            InitializeComponent();
            userid = id;
            labelToUpdate = label;
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            // Update the value in the database
            int currentValue = RetrieveValueFromDatabase();
            int newValue = currentValue + (int)numericUpDown1.Value;
            UpdateValueInDatabase(newValue);
        }

        private int RetrieveValueFromDatabase()
        {
            int value = 0;

            using (var cnn = new MySqlConnection(connectionString))
            {
                cnn.Open();

                var command = cnn.CreateCommand();
                command.CommandText = "SELECT cash FROM user WHERE id = " + userid;

                using (var result = command.ExecuteReader())
                {
                    while (result.Read())
                    {
                        value = (int)result.GetValue(0);
                    }
                }
            }

            return value;
        }

        private void UpdateValueInDatabase(int newValue)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = "UPDATE user SET cash = " + newValue + " WHERE id = " + userid;
                command.ExecuteReader();
            }

            labelToUpdate.Text = newValue.ToString() + " EUR";
        }
    }
}
