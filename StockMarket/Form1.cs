using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using MySqlConnector;



namespace StockMarket
{
    public partial class Form1 : Form
    {
        //BEGIN INFO UPDATER

        private void NewsPage()
        {
            string connetionString = null;
            MySqlConnection cnn;
            connetionString = "datasource=192.168.1.1;port=3306;username=psb202199;password=psb202199;database=psb202199_stockmarket;";
            cnn = new MySqlConnection(connetionString);
            cnn.Open();

            MySqlCommand command = cnn.CreateCommand();
            command.CommandText = "SELECT title, description from news";
            MySqlDataReader result = command.ExecuteReader();
            string title;
            string description;
            int yLocation = 10;
            while (result.Read())
            {
                title = result.GetString(0);
                description = result.GetString(1); 

                PictureBox news = new PictureBox();
                news.Size = new Size(50, 30);
                news.Location = new Point(25, yLocation);
                news.Parent = this.panel1;
                news.SizeMode = PictureBoxSizeMode.Zoom;
                news.Image = Image.FromFile("../Images/newspaper.png");

                Label titlelabel = new Label();
                titlelabel.MaximumSize = new Size(800, 50);
                titlelabel.AutoSize = true;
                titlelabel.Location = new Point(70, yLocation);
                titlelabel.Parent = this.panel1;
                titlelabel.Text = title;
                titlelabel.Font = new Font(titlelabel.Font.Name, 20, FontStyle.Bold);
                titlelabel.TextAlign = ContentAlignment.MiddleCenter;


                Label desclabel = new Label();
                desclabel.MaximumSize = new Size(800, 50);
                desclabel.AutoSize = true;
                desclabel.Location = new Point(70, yLocation + 50);
                desclabel.Parent = this.panel1;
                desclabel.Text = description;

                yLocation = yLocation + 150;
            }

            cnn.Close();


        }

        private void PortfolioPage()
        {

        }

        private void StockPage()
        {

        }

        private void CryptoPage()
        {

            
        }
        private void WalletPage()
        {
            PictureBox wallet = new PictureBox();
            wallet.Size = new Size(50, 30);
            wallet.Location = new Point(25, 50);
            wallet.Parent = this.panel1;
            wallet.SizeMode = PictureBoxSizeMode.Zoom;
            wallet.Image = Image.FromFile("../Images/Wallet.png");

            Label cash = new Label();
            cash.Location = new Point(50, 50);
            cash.Parent = this.panel1;
            cash.Text = this.label2.Text;

            //Label cash = new Label();
            //cash.MaximumSize = new Size(800, 50);
            //cash.AutoSize = true;
            //cash.Location = new Point(70, yLocation + 50);
            //cash.Parent = this.panel1;
            //cash.Text = description;
        }
        private void ShopPage()
        {

        }

        private void AdminPage()
        {
            AdminPage admin = new AdminPage();
            admin.TopLevel = false;
            admin.Parent = this.panel1;
            admin.Dock = DockStyle.Fill;
            admin.Show();

        }

        private void RankingsPage()
        {

        }

        private void SettingsPage()
        {

        }

        private void ClearInfo()
        {
            foreach (Control control in panel1.Controls) 
            {
                control.Dispose();
            }
            panel1.Controls.Clear();
        }


        //END INFO UPDATER

        private void UpdateInfo(string Button)
        {
            Random random = new Random();
            int chance = random.Next(1, 31);

            if (chance == 30) // NEWS HAPPENS
            {
                int value = random.Next(1, 101);
                if (value < 40)
                {
                    // BAD SHIT HAPPENS
                } else
                {
                    // GOOD SHIT HAPPENS
                }
            }

            Point scrollPosition = panel1.AutoScrollPosition;

            ClearInfo();
            switch (Button)
            {
                case "NEWS":
                    NewsPage();
                    break;

                case "PORTFOLIO":
                    PortfolioPage();
                    break;

                case "STOCK":
                    StockPage();
                    break;

                case "CRYPTO":
                    CryptoPage();
                    break;

                case "WALLET":
                    WalletPage();
                    break;

                case "SHOP":
                    ShopPage();
                    break;

                case "ADMIN":
                    AdminPage();
                    break;

                case "RANKINGS":
                    RankingsPage();
                    break;

                case "SETTINGS":
                    SettingsPage();
                    break;

                default:
                    label3.ForeColor = Color.White;
                    break;
            }

            panel1.AutoScrollPosition = new Point(-scrollPosition.X, -scrollPosition.Y);

        }

        private void btn_Clicked(object sender, EventArgs e)
        {
            if (sender is Button)
            {
                var Button = sender as Button;
                label3.Text = Button.Text;

                switch (Button.Text)
                {
                    case "SHOP":
                        label3.ForeColor = Color.ForestGreen;
                        break;
                    case "ADMIN":
                        label3.ForeColor = Color.DarkRed;
                        break;
                    default:
                        label3.ForeColor = Color.White;
                        break;
                }

                foreach (Control control in groupBox1.Controls) // IF BUTTON SELECTED
                {
                    switch (control.Text)
                    {
                        case "SHOP":
                            control.ForeColor = Color.ForestGreen;
                            break;
                        case "ADMIN":
                            control.ForeColor = Color.DarkRed;
                            break;
                        default:
                            control.ForeColor = Color.DimGray;
                            break;
                    }

                }

                Button.ForeColor = Color.White;


                UpdateInfo(Button.Text);

            }
        }
        
        private void NewWeek(string Button, DateTime date)
        {
            UpdateInfo(Button);
        }

        private System.Timers.Timer timer;
        private Button button;
        private DateTime date;

        public Form1(string username, string userid)
        {
            InitializeComponent();
            UserIDBox.Text = userid;

            label4.Text = username;

            // Get the reference of the existing button
            button = this.Controls.Find("myButton", true)[0] as Button;
            button.Text = "Jan 1st, 2000";
            button.Click += new EventHandler(ButtonClick);


            // Create the timer
            timer = new System.Timers.Timer();
            timer.Interval = 5000;
            timer.Elapsed += new ElapsedEventHandler(TimerElapsed);

            timer.Start();

            // Initialize the date
            date = new DateTime(2000, 1, 1);

        }

        private void ButtonClick(object sender, EventArgs e)
        {
            if (timer.Enabled)
            {
                timer.Enabled = false;
            }
            else
            {
                timer.Enabled = true;
            }
        }


        private void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            date = date.AddDays(1);
            this.Invoke((MethodInvoker)delegate
            {
                var text = date.ToString("MMM d") + GetDaySuffix(date.Day) + ", " + date.Year.ToString();
                button.Text = text;


                    NewWeek(label3.Text, date);
                
            });
        }


        private string GetDaySuffix(int day)
        {
            if (day % 10 == 1 && day != 11)
                return "st";
            else if (day % 10 == 2 && day != 12)
                return "nd";
            else if (day % 10 == 3 && day != 13)
                return "rd";
            else
                return "th";
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            UpdateInfo("NEWS");

            foreach (Control control in groupBox1.Controls) // IF BUTTON SELECTED
            {
                control.Click += new EventHandler(btn_Clicked);
            }

        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
