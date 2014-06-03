using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
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
    /// Main Application window, with user interaction
    /// </summary>
    public partial class MainWindow : Window
    {
        MailLoader ClientUpdater;
        /// <summary>
        /// Main Application window, with user interaction
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Load_Clients();
            ClientUpdater = new MailLoader(this);
        }

        /// <summary>
        /// Menu file "Add Mail" clicked.
        /// Open new Client
        /// </summary>
        private void Add_Mail_Clicked(object sender, RoutedEventArgs e)
        {
            string[] qustions = { "E-Mail Address:", "Password:", "Pop3-Server:", "Port:", "Use Ssl:" };
            NewClient Msr = new NewClient(qustions);
            if (Msr.ShowDialog() == true)
            {
                Properties.Settings.Default.Users = Msr.Answers[0];
                Properties.Settings.Default.Password = Msr.Answers[1];
                Properties.Settings.Default.Pop3 = Msr.Answers[2];
                Properties.Settings.Default.Port = Convert.ToInt32(Msr.Answers[3]);
                Properties.Settings.Default.UseSsl = Convert.ToBoolean(Msr.Answers[4]);
                Properties.Settings.Default.Save();
            }
            Load_Clients();
        }

        /// <summary>
        /// Load Clients form Properties.Settings
        /// </summary>
        private void Load_Clients()
        {
            if (String.IsNullOrEmpty(Properties.Settings.Default.Users) || String.IsNullOrEmpty(Properties.Settings.Default.Password)) return;
            MailClient newclient = new MailClient(Properties.Settings.Default.Users, Properties.Settings.Default.Password, Properties.Settings.Default.Pop3, Properties.Settings.Default.Port, Properties.Settings.Default.UseSsl);
            ClientPanel.Children.Add(newclient);
        }

        private void New_Mail_Clicked(object sender, RoutedEventArgs e)
        {
            string Sender = "";
            foreach (MailClient client in ((MainWindow)Window.GetWindow(this)).ClientPanel.Children)
            {
                if (client.HeadUser.SelectedItem != null)
                {
                    Sender = client.Client_Mail;
                    break;
                }
            }
            SendMail SenderWindow = new SendMail(this, Sender);
            SenderWindow.ShowDialog();
        }
    }

    public static class Misc
    {
        // Instantiate the secure string.
        static SecureString securePwd = new SecureString();

        public static SecureString convertToSecureString(string strPassword)
        {
            var secureStr = new SecureString();
            if (strPassword.Length > 0)
            {
                foreach (var c in strPassword.ToCharArray()) secureStr.AppendChar(c);
            }
            return secureStr;
        }
    }
}