using System;
using System.Collections.Generic;
using System.Linq;
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

namespace The_E_Mail_application
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Load_Clients();
        }

        private void Add_Mail_Clicked(object sender, RoutedEventArgs e)
        {
            string[] qustions = { "E-Mail Address", "Password" };
            NewClient Msr = new NewClient(qustions);
            if (Msr.ShowDialog() == true) {
                Properties.Settings.Default.Users = Msr.Answers[0];
                Properties.Settings.Default.Password = Msr.Answers[1];
                Properties.Settings.Default.Save();
            }
            Load_Clients();
        }

        private void Load_Clients() 
        {
            if (String.IsNullOrEmpty(Properties.Settings.Default.Users) || String.IsNullOrEmpty(Properties.Settings.Default.Password)) return;
            try {
                MailUser newclient = new MailUser(Properties.Settings.Default.Users, Properties.Settings.Default.Password);
                ClientPanel.Children.Add(newclient);
            }
            catch {
            }
        }
    }
}
