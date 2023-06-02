﻿using System;
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
using StockMarket.Pages;

namespace StockMarket
{
    public partial class Form1 : Form
    {
        //BEGIN INFO UPDATER

        private void NewsPage()
        {
            string connetionString = "datasource=192.168.1.1;port=3306;username=psb202199;password=psb202199;database=psb202199_stockmarket;";

            using (var cnn = new MySqlConnection(connetionString))
            {
                cnn.Open();

                var command = cnn.CreateCommand();
                command.CommandText = "SELECT title, description FROM news";

                using (var result = command.ExecuteReader())
                {
                    int yLocation = 10;
                    while (result.Read())
                    {
                        string title = result.GetString(0);
                        string description = result.GetString(1);

                        this.Invoke((MethodInvoker)delegate
                        {
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
                        });

                        yLocation += 150;
                    }
                }

                cnn.Close();
            }




        }

        private void PortfolioPage()
        {
            string connetionString = "datasource=192.168.1.1;port=3306;username=psb202199;password=psb202199;database=psb202199_stockmarket;";

            using (var cnn = new MySqlConnection(connetionString))
            {
                cnn.Open();

                var command = cnn.CreateCommand();
                command.CommandText = "SELECT * FROM boughtstock WHERE userid = '" + UserIDBox.Text + "'";

                using (var result = command.ExecuteReader())
                {
                    int yLocation = 5;
                    while (result.Read())
                    {
                        int boughtstockid = (int)result.GetValue(0);
                        int companyid = (int)result.GetValue(2);
                        string compname = FindCompanyNameById(companyid);
                        int stockamount = (int)result.GetValue(3);
                        int moneyspent = (int)result.GetValue(4);
                        int stock_price = FindCompanyStockValueById(companyid);

                        Label companyname = new Label();
                        companyname.MaximumSize = new Size(800, 50);
                        companyname.AutoSize = true;
                        companyname.Location = new Point(0, yLocation);
                        panel1.Controls.Add(companyname);
                        companyname.Text = compname;
                        companyname.Font = new Font(companyname.Font.Name, 20, FontStyle.Bold);
                        companyname.TextAlign = ContentAlignment.MiddleCenter;

                        Label stockprice = new Label();
                        stockprice.MaximumSize = new Size(800, 50);
                        stockprice.AutoSize = true;
                        stockprice.Location = new Point(400, yLocation);
                        panel1.Controls.Add(stockprice);
                        stockprice.Text =  (stock_price * stockamount).ToString();
                        stockprice.Font = new Font(stockprice.Font.Name, 20, FontStyle.Bold);
                        stockprice.TextAlign = ContentAlignment.MiddleCenter;
                        stockprice.Name = compname;

                        double expectedValue = stock_price * stockamount;
                        double difference = moneyspent - expectedValue;
                        double percentageDifference = (difference / expectedValue) * 100;
                        percentageDifference = Math.Round(percentageDifference, 2);

                        Label diffe = new Label();
                        diffe.MaximumSize = new Size(800, 50);
                        diffe.AutoSize = true;
                        diffe.Location = new Point(500, yLocation);
                        panel1.Controls.Add(diffe);
                        diffe.Font = new Font(diffe.Font.Name, 20, FontStyle.Bold);
                        diffe.TextAlign = ContentAlignment.MiddleCenter;

                        if (moneyspent > (stock_price * stockamount)) // NEGATIVE RED
                        {
                            diffe.ForeColor = Color.Red;
                            diffe.Text = "-" + Math.Abs(percentageDifference).ToString() + "%";
                        } else if (moneyspent < (stock_price * stockamount)) // POSITIVE GREEN
                        {
                            diffe.ForeColor = Color.Green;
                            diffe.Text = "+" + Math.Abs(percentageDifference).ToString() + "%";
                        }




                        Button sellstock = new Button();
                        sellstock.Text = "SELL";
                        sellstock.Size = new Size(50, 30);
                        sellstock.Location = new Point(650, yLocation);
                        sellstock.Parent = this.panel1;
                        sellstock.TabIndex = 1;
                        sellstock.Name = boughtstockid.ToString();
                        sellstock.Tag = stockamount;
                        sellstock.Click += Sellstock_Click;

                        NumericUpDown amount = new NumericUpDown();
                        amount.Size = new Size(50, 30);
                        amount.Location = new Point(725, yLocation);
                        amount.Parent = this.panel1;
                        amount.Increment = 5;
                        amount.Value = 5;
                        amount.TabIndex = 2;
                        amount.Maximum = stockamount;
                        amount.Name = compname;

                        Button sellall = new Button();
                        sellall.Text = "SELL ALL";
                        sellall.Size = new Size(50, 30);
                        sellall.Location = new Point(650, yLocation);
                        sellall.Parent = this.panel1;
                        sellall.TabIndex = 1;
                        sellall.Name = compname;
                        //sellall.Click += Sellstock_Click;

                        yLocation += 50;
                    }
                }
            }
        }

        private void Sellstock_Click(object sender, EventArgs e) // TO COMPLETE
        {
            Button button = sender as Button;
            string boughtstockid = button.Name;
            int ownedstock = Convert.ToInt32(button.Tag.ToString());
            int stockamount = 0;
            int overallstockvalue = 0;

            foreach (Control control in panel1.Controls)
            {
                if (control.Name != button.Name) { continue; }
                if (control is NumericUpDown)
                {
                    NumericUpDown num = control as NumericUpDown;
                    stockamount = (int)num.Value;
                }
                if (control is Label)
                {
                    Label stk = control as Label;
                    overallstockvalue = Convert.ToInt32(stk.Text);
                }

            }



            int ind_stock_price = overallstockvalue / ownedstock;
            int moneyToAdd = ind_stock_price * stockamount;


            string connetionString = "datasource=192.168.1.1;port=3306;username=psb202199;password=psb202199;database=psb202199_stockmarket;";

            using (var cnn = new MySqlConnection(connetionString))
            {
                cnn.Open(); //CHECK FOR HOW MUCH MONEY

                var command = cnn.CreateCommand();
                command.CommandText = "SELECT cash, stock FROM user WHERE id = '" + UserIDBox.Text + "'";
                int cash = 0;
                int crypto = 0;
                using (var result = command.ExecuteReader())
                {
                    while (result.Read())
                    {
                        cash = (int)result.GetValue(0);
                        crypto = (int)result.GetValue(1);

                        cash -= price;
                        label2.Text = "€" + cash.ToString();

                    }
                }

                command.CommandText = "UPDATE user SET cash = " + cash + " WHERE id = '" + UserIDBox.Text + "'";
                command.ExecuteReader();


                cnn.Close();
                cnn.Open(); // FIND COMPANYID BY NAME

                var findcompany = cnn.CreateCommand();
                findcompany.CommandText = "SELECT companyID FROM company WHERE name = '" + company + "'";
                int companyid = 0;
                using (var result = findcompany.ExecuteReader())
                {
                    while (result.Read())
                    {
                        companyid = (int)result.GetValue(0);
                    }
                }

                cnn.Close();
                cnn.Open(); //CHECK IF STOCK ALREADY BOUGHT BY USER TO ADD TO IT OR UPDATE

                var boughtstock = cnn.CreateCommand();
                boughtstock.CommandText = "SELECT * FROM boughtstock WHERE userid = '" + UserIDBox.Text + "' AND  companyid = '" + companyid + "'";
                int boughtstockid = 0;
                int currentamount = 0;
                int moneyspent = 0;
                using (var result = boughtstock.ExecuteReader())
                {
                    while (result.Read())
                    {
                        boughtstockid = (int)result.GetValue(0);
                        currentamount = (int)result.GetValue(3);
                        moneyspent = (int)result.GetValue(4);
                        price += moneyspent;
                        stockamount += currentamount;

                        UpdateStock(stockamount, boughtstockid, price);
                        return;


                    }
                }




                boughtstock.CommandText = "INSERT INTO boughtstock (userid, companyid, stockamount, moneyspent) VALUES (" + UserIDBox.Text + ", " + companyid + ", " + stockamount + ", " + price + ");";
                boughtstock.ExecuteReader();

                cnn.Close();
            }


        }

        private string FindCompanyNameById(int companyid)
        {
            string connetionString = "datasource=192.168.1.1;port=3306;username=psb202199;password=psb202199;database=psb202199_stockmarket;";
            string companyname = "";
            using (var cnn = new MySqlConnection(connetionString))
            {
                cnn.Open();

                var command = cnn.CreateCommand();
                command.CommandText = "SELECT name FROM company WHERE companyid = '" + companyid + "'";

                using (var result = command.ExecuteReader())
                {
                    while (result.Read())
                    {
                        companyname = result.GetString(0);
                    }
                }
            }

            return companyname;

        }

        private int FindCompanyStockValueById(int companyid)
        {
            string connetionString = "datasource=192.168.1.1;port=3306;username=psb202199;password=psb202199;database=psb202199_stockmarket;";
            int stock_price = 0;
            using (var cnn = new MySqlConnection(connetionString))
            {
                cnn.Open();

                var command = cnn.CreateCommand();
                command.CommandText = "SELECT stock_price FROM company WHERE companyid = '" + companyid + "'";

                using (var result = command.ExecuteReader())
                {
                    while (result.Read())
                    {
                        stock_price = (int)result.GetValue(0);
                    }
                }
            }

            return stock_price;

        }

        private void StockPage()
        {
            string connetionString = "datasource=192.168.1.1;port=3306;username=psb202199;password=psb202199;database=psb202199_stockmarket;";

            using (var cnn = new MySqlConnection(connetionString))
            {
                cnn.Open();

                var command = cnn.CreateCommand();
                command.CommandText = "SELECT * FROM company";

                using (var result = command.ExecuteReader())
                {
                    int yLocation = 5;
                    while (result.Read())
                    {
                        Label companyname = new Label();
                        companyname.MaximumSize = new Size(800, 50);
                        companyname.AutoSize = true;
                        companyname.Location = new Point(0, yLocation);
                        panel1.Controls.Add(companyname);
                        companyname.Text = result.GetString(1);
                        companyname.Font = new Font(companyname.Font.Name, 20, FontStyle.Bold);
                        companyname.TextAlign = ContentAlignment.MiddleCenter;

                        Label stockprice = new Label();
                        stockprice.MaximumSize = new Size(800, 50);
                        stockprice.AutoSize = true;
                        stockprice.Location = new Point(400, yLocation);
                        panel1.Controls.Add(stockprice);
                        stockprice.Text = result.GetValue(3).ToString();
                        stockprice.Font = new Font(stockprice.Font.Name, 20, FontStyle.Bold);
                        stockprice.TextAlign = ContentAlignment.MiddleCenter;
                        stockprice.Name = result.GetString(1);

                        PictureBox news = new PictureBox();
                        news.Size = new Size(50, 30);
                        news.Location = new Point(525, yLocation);
                        news.Parent = this.panel1;
                        news.SizeMode = PictureBoxSizeMode.Zoom;

                        Label change = new Label();
                        change.MaximumSize = new Size(800, 50);
                        change.AutoSize = true;
                        change.Location = new Point(475, yLocation);
                        panel1.Controls.Add(change);
                        int fluctuation = (int)result.GetValue(3) - (int)result.GetValue(4);
                        if(fluctuation > 0)
                        {
                            news.Image = Image.FromFile("../Images/increase.png");
                            change.ForeColor = Color.Green;
                        } else if (fluctuation < 0)
                        {
                            news.Image = Image.FromFile("../Images/decrease.png");
                            change.ForeColor = Color.Red;
                        }

                        change.Text = Math.Abs(fluctuation).ToString();
                        change.Font = new Font(change.Font.Name, 20, FontStyle.Bold);
                        change.TextAlign = ContentAlignment.MiddleCenter;

                        NumericUpDown amount = new NumericUpDown();
                        amount.Size = new Size(50, 30);
                        amount.Location = new Point(725, yLocation);
                        amount.Parent = this.panel1;
                        amount.Increment = 5;
                        amount.Value = 5;
                        amount.TabIndex = 2;
                        amount.Name = result.GetString(1);

                        Button buystock = new Button();
                        buystock.Text = "BUY";
                        buystock.Size = new Size(50, 30);
                        buystock.Location = new Point(650, yLocation);
                        buystock.Parent = this.panel1;
                        buystock.TabIndex = 1;
                        buystock.Name = result.GetString(1);
                        buystock.Click += Buystock_Click;

                        yLocation += 50;
                    }
                }
            }
        }


        private void UpdateStock(int stockamount, int boughtstockid, int price)
        {
            string connetionString = "datasource=192.168.1.1;port=3306;username=psb202199;password=psb202199;database=psb202199_stockmarket;";

            using (var cnn = new MySqlConnection(connetionString))
            {
                cnn.Open(); //CHECK FOR IF ENOUGH MONEY

                var command = cnn.CreateCommand();
                command.CommandText = "UPDATE boughtstock SET stockamount = " + stockamount + ", moneyspent = " + price + " WHERE boughtstockid = '" + boughtstockid + "'";
                command.ExecuteReader();


                cnn.Close();
            }

        }
        private void Buystock_Click(object sender, EventArgs e)
        {
            Button button = sender as Button;
            string company = button.Name;
            int stockamount = 0;
            int stockvalue = 0;

            foreach (Control control in panel1.Controls)
            {
                if (control.Name != button.Name) { continue; }
                if (control is NumericUpDown)
                {
                    NumericUpDown num = control as NumericUpDown;
                    stockamount = (int)num.Value;
                }
                if (control is Label)
                {
                    Label stk = control as Label;
                    stockvalue = Convert.ToInt32(stk.Text);
                }
                
            }

            int price = stockamount * stockvalue;


            string connetionString = "datasource=192.168.1.1;port=3306;username=psb202199;password=psb202199;database=psb202199_stockmarket;";

            using (var cnn = new MySqlConnection(connetionString))
            { 
                cnn.Open(); //CHECK FOR IF ENOUGH MONEY

                var command = cnn.CreateCommand();
                command.CommandText = "SELECT cash, stock FROM user WHERE id = '" + UserIDBox.Text + "'";
                int cash = 0;
                int crypto = 0;
                using (var result = command.ExecuteReader())
                {
                    while (result.Read())
                    {
                        cash = (int)result.GetValue(0);
                        crypto = (int)result.GetValue(1);

                        if ((cash - price) < 0) { button.Text = "INSUFFICIENT MONEY"; button.ForeColor = Color.Red; return; }

                        cash -= price;
                        label2.Text = "€" + cash.ToString();

                    }
                }

                command.CommandText = "UPDATE user SET cash = " + cash + " WHERE id = '" + UserIDBox.Text + "'";
                command.ExecuteReader();


                cnn.Close();
                cnn.Open(); // FIND COMPANYID BY NAME

                var findcompany = cnn.CreateCommand();
                findcompany.CommandText = "SELECT companyID FROM company WHERE name = '" + company + "'";
                int companyid = 0;
                using (var result = findcompany.ExecuteReader())
                {
                    while (result.Read())
                    {
                        companyid = (int)result.GetValue(0);
                    }
                }

                cnn.Close();
                cnn.Open(); //CHECK IF STOCK ALREADY BOUGHT BY USER TO ADD TO IT OR UPDATE

                var boughtstock = cnn.CreateCommand();
                boughtstock.CommandText = "SELECT * FROM boughtstock WHERE userid = '" + UserIDBox.Text + "' AND  companyid = '" + companyid + "'";
                int boughtstockid = 0;
                int currentamount = 0;
                int moneyspent = 0;
                using (var result = boughtstock.ExecuteReader())
                {
                    while (result.Read())
                    {
                        boughtstockid = (int)result.GetValue(0);
                        currentamount = (int)result.GetValue(3);
                        moneyspent = (int)result.GetValue(4);
                        price += moneyspent;
                        stockamount += currentamount;

                        UpdateStock(stockamount, boughtstockid, price);
                        return;


                    }
                }




                boughtstock.CommandText = "INSERT INTO boughtstock (userid, companyid, stockamount, moneyspent) VALUES (" + UserIDBox.Text + ", " + companyid + ", " + stockamount + ", " + price + ");";
                boughtstock.ExecuteReader();

                cnn.Close();
            }
        }

        private void CryptoPage()
        {

            
        }
        private void WalletPage()
        {
            WalletPage wallet = new WalletPage(UserIDBox.Text);
            wallet.TopLevel = false;
            wallet.Parent = this.panel1;
            wallet.Dock = DockStyle.Fill;
            wallet.Show();
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


        private void ChangeStockValue(string company, int valuetochange)
        {
            string connetionString = "datasource=192.168.1.1;port=3306;username=psb202199;password=psb202199;database=psb202199_stockmarket;";

            using (var cnn = new MySqlConnection(connetionString))
            {
                cnn.Open();

                var command = cnn.CreateCommand();
                command.CommandText = "SELECT * FROM company WHERE name = '" + company + "'";
                int value = 0;
                int oldvalue = 0;
                using (var result = command.ExecuteReader())
                {
                    while (result.Read())
                    {
                        value = (int)result.GetValue(3);
                        oldvalue = value;

                        value += valuetochange;

                        if(value < 0) { value = 0; }
                    }
                }

                command.CommandText = "UPDATE company SET stock_price = " + value + ", oldprice = " + oldvalue + " WHERE name = '" + company + "'";
                command.ExecuteReader();
            }
        }

        private void UpdateStocks()
        {
            string connetionString = "datasource=192.168.1.1;port=3306;username=psb202199;password=psb202199;database=psb202199_stockmarket;";

            using (var cnn = new MySqlConnection(connetionString))
            {
                cnn.Open();

                var command = cnn.CreateCommand();
                command.CommandText = "SELECT * FROM company";
                using (var result = command.ExecuteReader())
                {
                    Random random = new Random();
                    while (result.Read())
                    {
                        int value = random.Next(-15, 20);
                        ChangeStockValue(result.GetString(1), value);


                    }
                }

            }
        }

        private void UpdateUserInfo()
        {
            string connetionString = "datasource=192.168.1.1;port=3306;username=psb202199;password=psb202199;database=psb202199_stockmarket;";

            using (var cnn = new MySqlConnection(connetionString))
            {
                cnn.Open();

                var command = cnn.CreateCommand();
                command.CommandText = "SELECT cash FROM user WHERE id = " + UserIDBox.Text;
                using (var result = command.ExecuteReader())
                {
                    while (result.Read())
                    {
                        label2.Text = result.GetValue(0).ToString() + " EUR";
                    }
                }

            }
        }

        private void UpdateInfo(string Button)
        {
            Random random = new Random();

            int chance = random.Next(1, 31);

            if (chance == 30) // NEWS HAPPENS
            {
                int value = random.Next(1, 101);
                if (value < 60)
                {
                    // BAD SHIT HAPPENS
                } else
                {
                    // GOOD SHIT HAPPENS
                }
            } 
            //NORMAL FLUCTUATION OF STOCK PRICE
            UpdateStocks();
            UpdateUserInfo();


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
            timer.Interval = 10000;
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
