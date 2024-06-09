using LeadsTracker_FinalsProject1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LeadsTracker_FinalsProject1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DataClasses1DataContext _ltDC = null;
        string userName = "";
        bool loginFlag = false;

        public MainWindow()
        {
            InitializeComponent();

            _ltDC = new DataClasses1DataContext(
                Properties.Settings.Default.Lead_TrackerConnectionString);
        }

        private void LoginBtn_Click(object sender, RoutedEventArgs e)
        {
            loginFlag = false;
            DateTime cDT = DateTime.Now;

            if (UsernameBox.Text.Length > 0 && PasswordBox.Text.Length > 0)
            {
                var loginQuery = from s in _ltDC.Staffs
                                 where
                                    s.Staff_Username == UsernameBox.Text
                                 //&& s.Password == txtbPassword.Text
                                 select s;

                if (loginQuery.Count() == 1)
                {
                    foreach (var login in loginQuery)
                    {
                        if (login.Staff_Password == PasswordBox.Text)
                        {
                            loginFlag = true;
                            userName = login.Staff_Username;
                            _ltDC.SubmitChanges();
                        }
                    }
                }


                if (loginFlag)
                {
                    MessageBox.Show($"Login success! Welcome back {userName}!");
                    Menu w1 = new Menu();
                    w1.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Username and/or password is incorrect");
                }
            }
            else
            {
                MessageBox.Show("Please input username and/or password");
            }
        }


    }
}
