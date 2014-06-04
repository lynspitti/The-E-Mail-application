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
        //BackGround mail_list updater
        private MailLoader ClientUpdater;

        /// <summary>
        /// Main Application window, with user interaction
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            #region Initialize Default properties
            if (Properties.Settings.Default.Users == null) Properties.Settings.Default.Users = new System.Collections.Specialized.StringCollection();
            if (Properties.Settings.Default.Password == null) Properties.Settings.Default.Password = new System.Collections.Specialized.StringCollection();
            if (Properties.Settings.Default.Pop3 == null) Properties.Settings.Default.Pop3 = new System.Collections.Specialized.StringCollection();
            if (Properties.Settings.Default.PPort == null) Properties.Settings.Default.PPort = new System.Collections.Specialized.StringCollection();
            if (Properties.Settings.Default.UseSsl == null) Properties.Settings.Default.UseSsl = new System.Collections.Specialized.StringCollection();
            if (Properties.Settings.Default.Smtp == null) Properties.Settings.Default.Smtp = new System.Collections.Specialized.StringCollection();
            if (Properties.Settings.Default.SPort == null) Properties.Settings.Default.SPort = new System.Collections.Specialized.StringCollection();
            if (Properties.Settings.Default.UseTls == null) Properties.Settings.Default.UseTls = new System.Collections.Specialized.StringCollection();
            #endregion
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Load_Clients();
            ClientUpdater = new MailLoader(this);
        }

        /// <summary>
        /// Load Clients form Properties.Settings
        /// and showem in Client panel
        /// </summary>
        public void Load_Clients()
        {
            ClientPanel.Children.Clear();
            int i = 0;
            foreach (string User in Properties.Settings.Default.Users.Cast<string>().ToArray<string>())
            {
                MailClient newclient = new MailClient(i);
                ClientPanel.Children.Add(newclient);
                i++;
            }
        }

        #region Ui functions
        /// <summary>
        /// Menu file "Add Mail" clicked.
        /// Open new Client
        /// </summary>
        private void Add_Mail_Clicked(object sender, RoutedEventArgs e)
        {
            string[] qustions = { "E-Mail Address:", "Password:", "Pop3-Server:", "Pop3-Port:", "Use Ssl:", "Smtp-Server", "Smtp-Port:", "Use Tls:"};
            NewClient Msr = new NewClient(qustions, null);

            if (Msr.ShowDialog() == true)
            {
                #region Save / add new client in properties
                Properties.Settings.Default.Users.Add(Msr.Answers[0]);
                Properties.Settings.Default.Password.Add(Symmetric_Encryption.EncryptString(Msr.Answers[1]));
                Properties.Settings.Default.Pop3.Add(Msr.Answers[2]);
                Properties.Settings.Default.PPort.Add(Msr.Answers[3]);
                Properties.Settings.Default.UseSsl.Add(Msr.Answers[4]);
                Properties.Settings.Default.Smtp.Add(Msr.Answers[5]);
                Properties.Settings.Default.SPort.Add(Msr.Answers[6]);
                Properties.Settings.Default.UseTls.Add(Msr.Answers[7]);
                Properties.Settings.Default.Save();
                #endregion
            }

            //Reload Client Panel
            Load_Clients();
        }

        /// <summary>
        /// Menu New clicked.
        /// Open SendMail
        /// </summary>
        private void New_Mail_Clicked(object sender, RoutedEventArgs e)
        {
            string Sender = "";
            foreach (MailClient client in ((MainWindow)Window.GetWindow(this)).ClientPanel.Children)
            {
                if (client.HeadUser.SelectedItem != null)
                {
                    Sender = client.User.Header.ToString();
                    break;
                }
            }
            SendMail SenderWindow = new SendMail(this, Sender);
            SenderWindow.ShowDialog();
        }
        #endregion
    }
}