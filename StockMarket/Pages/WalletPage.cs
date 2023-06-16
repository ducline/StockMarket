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

namespace StockMarket.Pages
{
    public partial class WalletPage : Form
    {
        string connetionString = "datasource=192.168.1.1;port=3306;username=psb202199;password=psb202199;database=psb202199_stockmarket;";
        string userid;
        public WalletPage(string id)
        {
            InitializeComponent();
            userid = id;
        }

        private void WalletPage_Load(object sender, EventArgs e)
        {
            using (var cnn = new MySqlConnection(connetionString))
            {
                cnn.Open();

                var command = cnn.CreateCommand();
                command.CommandText = "SELECT * FROM user WHERE id = " + userid;

                using (var result = command.ExecuteReader())
                {
                    while (result.Read())
                    {
                        label1.Text = result.GetValue(8).ToString();
                        label2.Text = result.GetValue(2).ToString();
                    }
                }
            }

        }
    }
}
