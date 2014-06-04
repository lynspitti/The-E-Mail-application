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
using OpenPop.Pop3;
using OpenPop.Mime;
using OpenPop.Mime.Header;
using System.Windows.Threading;
using System.Threading;
using System.Data.SQLite;
using System.Net.Mail;

namespace The_E_Mail_application
{
    /// <summary>
    /// Contains a User´s pop3 information
    /// and Mail contact
    /// </summary>
    public partial class MailClient : UserControl
    {
        DispatcherTimer dispatcherTimer = new DispatcherTimer();
        Pop3Client client;
        public bool IsConnected = false;
        public string UserEmail = "";

        int SettingsIndex = 0;
        /// <summary>
        /// Contains a User´s pop3 information
        /// and Mail contact
        /// </summary>
        /// <param name="Settings_Index">Index used to get Client information</param>
        public MailClient(int Settings_Index)
        {
            SettingsIndex = Settings_Index;
            InitializeComponent();
            UserEmail = Properties.Settings.Default.Users.Cast<string>().ToArray<string>()[SettingsIndex];
            User.Header = UserEmail;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            IsConnected = Connect();
        }

        #region Pretty funcktions
        private void Mouseover(object sender, MouseEventArgs e)
        {
            TreeViewItem Sender = (TreeViewItem)sender;
            Sender.Background = Brushes.AliceBlue;
        }

        private void Mouseleave(object sender, MouseEventArgs e)
        {
            TreeViewItem Sender = (TreeViewItem)sender;
            Sender.Background = Brushes.Transparent;
        }
        #endregion

        /// <summary>
        /// Used to connect client to server.
        /// </summary>
        private bool Connect()
        {
            client = new Pop3Client();
            try
            {
                // connect pop3 server, pop3 port, use ssl
                client.Connect(Properties.Settings.Default.Pop3.Cast<string>().ToArray<string>()[SettingsIndex], 
                    Convert.ToInt32(Properties.Settings.Default.PPort.Cast<string>().ToArray<string>()[SettingsIndex]), 
                    Convert.ToBoolean(Properties.Settings.Default.UseSsl.Cast<string>().ToArray<string>()[SettingsIndex])
                );
                try
                {
                    // authenticate user, password, methode
                    client.Authenticate(UserEmail,
                        Symmetric_Encryption.DecryptString(Properties.Settings.Default.Password.Cast<string>().ToArray<string>()[SettingsIndex]), 
                        AuthenticationMethod.Auto
                    );
                    client.NoOperation();

                    // Start NoOperation every 100 milisec
                    ThreadStart processTaskThread = delegate
                    {
                        dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
                        dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 100);
                        dispatcherTimer.Start();
                    };
                    Thread thread = new Thread(processTaskThread);
                    thread.Start();
                    return true;
                }
                catch {
                    // user authentication error
                    MessageBox.Show("User authentication error.", "error");
                }
            }
            catch {
                // POP3 connection error
                MessageBox.Show("POP3 connection error.", "error");
            }
            return false;
        }

        /// <summary>
        /// Contains the connection from client to server open/accesable
        /// </summary>
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                dispatcherTimer.Stop();
                client.NoOperation();
                dispatcherTimer.Start();
            }
            catch { }
        }

        /// <summary>
        /// Load every mail from client in MailItems
        /// </summary>
        /// <returns>A list off MailItems</returns>
        public string[][] Load_Mail_List()
        {
            if (IsConnected)
            {
                this.dispatcherTimer.Stop();
                Thread.Sleep(0500);
                try{
                    int messageCount = client.GetMessageCount();
                    string[][] All_Mails = new string[messageCount][];
                    for (int i = messageCount; i > 0; i--)
                    {
                        MessageHeader headers = client.GetMessageHeaders(i);
                        RfcMailAddress from = headers.From;

                        //Check on Spam
                        if (String.IsNullOrEmpty(headers.MessageId)) continue;

                        string[] Mail = new string[4];
                        Mail[0] = headers.MessageId;
                        if (!String.IsNullOrEmpty(from.DisplayName)) Mail[1] = from.DisplayName;
                        else Mail[1] = from.Address;
                        Mail[2] = headers.DateSent.Year.ToString("D4") + " " + headers.DateSent.Month.ToString("D2") + "/" + headers.DateSent.Day.ToString("D2") + " " + headers.DateSent.Hour.ToString("D2") + ":" + headers.DateSent.Minute.ToString("D2") + ":" + headers.DateSent.Second.ToString("D2");
                        Mail[3] = headers.Subject;
                        All_Mails[messageCount - i] = Mail;
                    }
                    this.dispatcherTimer.Start();
                    return All_Mails;
                }
                catch {
                    // mail receiving error
                    MessageBox.Show("Mail receiving error.", "error");
                }
            }
            this.dispatcherTimer.Start();
            return null;
        }

        /// <summary>
        /// Gets a mails body html by its messageid
        /// </summary>
        /// <param name="MessageId"></param>
        /// <returns>Mail Body in Hml as string</returns>
        public string Get_Mail_Body(string MessageId)
        {
            int messageCount = 0;
            try
            {
                messageCount = client.GetMessageCount();
            }
            catch { }
            for (int i = messageCount; i > 0; i--)
            {
                if (client.GetMessageHeaders(i).MessageId == MessageId)
                {
                    Message Email = client.GetMessage(i);

                    StringBuilder builder = new StringBuilder();
                    MessagePart html = Email.FindFirstHtmlVersion();
                    if (html != null)
                    {
                        // We found some html!
                        builder.Append(html.GetBodyAsText());
                    }
                    else
                    {
                        // Might include a part holding plaintext instead
                        MessagePart plainText = Email.FindFirstPlainTextVersion();
                        if (plainText != null)
                        {
                            // We found some plaintext!
                            builder.Append(plainText.GetBodyAsText());
                        }
                    }
                    return builder.ToString();
                }
            }
            return "";
        }

        #region Ui functions
        /// <summary>
        /// Send E-Mail
        /// </summary>
        /// <param name="reciverMail">Mail that recives mail</param>
        /// <param name="subject">Mail subject</param>
        /// <param name="textBody">Mail context</param>
        public void sendEmail(string reciverMail, string subject, string textBody)
        {
            SmtpClient c = new SmtpClient(@"smtp.live.com", 25);
            MailAddress add = new MailAddress(reciverMail);
            MailMessage msg = new MailMessage();
            msg.To.Add(add);
            msg.From = new MailAddress(@"f.isse2009@live.dk");
            msg.IsBodyHtml = true;
            msg.Subject = subject;
            msg.Body = textBody;
            c.Credentials = new System.Net.NetworkCredential(@"f.isse2009@live.dk", @"Testtest");
            c.EnableSsl = true;

            try
            {
                c.Send(msg);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        /// <summary>
        /// A Client folder have been selected
        /// </summary>
        public void MailListMap_Selected(object sender, RoutedEventArgs e)
        {
            if (!IsConnected) return;
            ((MainWindow)Window.GetWindow(this)).MailList.Children.Clear();
            SQLiteConnection m_dbConnection = new SQLiteConnection("Data Source=MyDatabase.sqlite;Version=3;");
            m_dbConnection.Open();

            TreeViewItem Sender = (TreeViewItem)sender;
            if (Sender == null) return;
            // get all mails in database " from client / user, under folder name
            SQLiteCommand sqlComm = new SQLiteCommand("SELECT * FROM MailList WHERE Receiver = '" + UserEmail + "' AND Container = '" + Sender.Header.ToString() + "' ORDER BY Date DESC", m_dbConnection);
            SQLiteDataReader r = sqlComm.ExecuteReader();
            while (r.Read())
            {
                MailItem NewMail = new MailItem();
                NewMail.Subject = (string)r["Subject"];
                NewMail.Date = (string)r["Date"];
                NewMail.From = (string)r["Sender"];
                NewMail.MessageId = (string)r["MessageId"];
                ((MainWindow)Window.GetWindow(this)).MailList.Children.Add(NewMail);
            }
            m_dbConnection.Close();
        }

        /// <summary>
        /// Edit menu have been clicked
        /// </summary>
        private void EditClient_Clicked(object sender, RoutedEventArgs e)
        {
            string[] qustions = { "E-Mail Address:", "Password:", "Pop3-Server:", "Pop3-Port:", "Use Ssl:", "Smtp-Server", "Smtp-Port:", "Use Tls:" };
            string[] Answers = { UserEmail, Symmetric_Encryption.DecryptString(Properties.Settings.Default.Password.Cast<string>().ToArray<string>()[SettingsIndex]), Properties.Settings.Default.Pop3.Cast<string>().ToArray<string>()[SettingsIndex],Properties.Settings.Default.PPort.Cast<string>().ToArray<string>()[SettingsIndex] ,
                               Properties.Settings.Default.UseSsl.Cast<string>().ToArray<string>()[SettingsIndex], Properties.Settings.Default.Smtp.Cast<string>().ToArray<string>()[SettingsIndex], Properties.Settings.Default.SPort.Cast<string>().ToArray<string>()[SettingsIndex], Properties.Settings.Default.UseTls.Cast<string>().ToArray<string>()[SettingsIndex]
            };
            NewClient Msr = new NewClient(qustions, Answers);

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
            DeleteClient_Clicked(null, new RoutedEventArgs());
            ((MainWindow)Window.GetWindow(this)).Load_Clients();
        }

        /// <summary>
        /// Delete menu have been clicked
        /// </summary>
        private void DeleteClient_Clicked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.Users.RemoveAt(SettingsIndex);
            Properties.Settings.Default.Password.RemoveAt(SettingsIndex);
            Properties.Settings.Default.Pop3.RemoveAt(SettingsIndex);
            Properties.Settings.Default.PPort.RemoveAt(SettingsIndex);
            Properties.Settings.Default.UseSsl.RemoveAt(SettingsIndex);
            Properties.Settings.Default.Smtp.RemoveAt(SettingsIndex);
            Properties.Settings.Default.SPort.RemoveAt(SettingsIndex);
            Properties.Settings.Default.UseTls.RemoveAt(SettingsIndex);
            Properties.Settings.Default.Save();
            ((MainWindow)Window.GetWindow(this)).ClientPanel.Children.Remove(this);
        }
        #endregion
    }
}
