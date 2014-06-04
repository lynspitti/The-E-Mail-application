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
using System.Windows.Shapes;

namespace The_E_Mail_application
{
    /// <summary>
    /// Send a E-mail
    /// </summary>
    public partial class SendMail : Window
    {
        private MainWindow Main_Window;
        public SendMail(MainWindow Main_window,string SelectedClient)
        {
            Main_Window = Main_window;
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (MailClient client in Main_Window.ClientPanel.Children)
            {
                ComboBoxItem Newclient = new ComboBoxItem();
                Newclient.Content = client.User.Header.ToString();
                if (client.HeadUser.SelectedItem != null)
                {
                    Newclient.IsSelected = true;
                }
                Senders.Items.Add(Newclient);
            }
        }

        #region Ui functions
        /// <summary>
        /// Send button Clicked
        /// send mail with selected client
        /// </summary>
        private void Send_Click(object sender, RoutedEventArgs e)
        {
            #region Non valid send informations handlers
            if (Senders.SelectedItem == null) {
                MessageBox.Show("Please select a Sender client","Missing sender");
            }
            else if (String.IsNullOrEmpty(Receiver.Text) || String.IsNullOrWhiteSpace(Receiver.Text)) {
                MessageBox.Show("Please give a receiving mail","Missing receiver");
            }
            else if (!Receiver.Text.Contains("@") || Receiver.Text.Split('.').Length < 2)
            {
                MessageBox.Show("Please give a valid receiving mail", "Invalid");
            }
            #endregion
            else {
                foreach (MailClient client in Main_Window.ClientPanel.Children)
                {
                    if (client.User.Header.ToString() == ((ComboBoxItem)Senders.SelectedItem).Content.ToString())
                    {
                        client.sendEmail(Receiver.Text, Subject.Text,Message.Text);
                        this.Close();
                    }
                }
            }
        }
        #endregion
    }
}
