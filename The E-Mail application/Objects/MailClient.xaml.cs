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
        public string Client_Mail;
        string Password;
        string Pop3 = "mail.renennielsen.dk";
        int Port = 110;
        bool UseSsl = true;

        DispatcherTimer dispatcherTimer = new DispatcherTimer();
        Pop3Client client;
        public bool IsConnected = false;
        /// <summary>
        /// Contains a User´s pop3 information
        /// and Mail contact
        /// </summary>
        /// <param name="E_Mail">Users E-Mail</param>
        /// <param name="password">Users Password</param>
        /// <param name="pop3">Pop3 server address</param>
        /// <param name="port">Pop3 port</param>
        /// <param name="usessl">How ever or not to use Ssl</param>
        public MailClient(string E_Mail, string password, string pop3, int port, bool usessl)
        {
            Client_Mail = E_Mail;
            Password = password;
            Pop3 = pop3;
            Port = port;
            UseSsl = usessl;
            InitializeComponent();
            User.Header = Client_Mail;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            IsConnected = Connect();
        }
        /// <summary>
        /// Used to connect client to server.
        /// </summary>
        private bool Connect()
        {
            client = new Pop3Client();
            try
            {
                client.Connect(Pop3, Port, UseSsl);
                try
                {
                    client.Authenticate(Client_Mail.ToString(), Password.ToString(), AuthenticationMethod.Auto);
                    client.NoOperation();
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
                dispatcherTimer.Stop();
                Thread.Sleep(0500);
                try{
                    int messageCount = client.GetMessageCount();
                    string[][] All_Mails = new string[messageCount][];
                    for (int i = messageCount; i > 0; i--)
                    {
                        MessageHeader headers = client.GetMessageHeaders(i);
                        RfcMailAddress from = headers.From;
                        string[] Mail = new string[4];
                        Mail[0] = headers.MessageId;
                        if (!String.IsNullOrEmpty(from.DisplayName)) Mail[1] = from.DisplayName;
                        else Mail[1] = from.Address;
                        Mail[2] = headers.DateSent.Day.ToString("D2") + "/" + headers.DateSent.Month.ToString("D2") + " " + headers.DateSent.Year.ToString("D4") + " " + headers.DateSent.Hour.ToString("D2") + ":" + headers.DateSent.Minute.ToString("D2") + ":" + headers.DateSent.Second.ToString("D2");
                        Mail[3] = headers.Subject;
                        All_Mails[messageCount - i] = Mail;
                    }
                    return All_Mails;
                }
                catch {
                    // mail receiving error
                    MessageBox.Show("Mail receiving error.", "error");
                }
                dispatcherTimer.Start();
            }
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

        /// <summary>
        /// A Client folder have been selected
        /// </summary>
        public void MailListMap_Selected(object sender, RoutedEventArgs e)
        {
            if (!IsConnected) return;
            SQLiteConnection m_dbConnection = new SQLiteConnection("Data Source=MyDatabase.sqlite;Version=3;");
            m_dbConnection.Open();

            TreeViewItem Sender = (TreeViewItem)sender;
            if (Sender == null) return;
            SQLiteCommand sqlComm = new SQLiteCommand("SELECT * FROM MailList WHERE Receiver = '" + Client_Mail + "' AND Container = '" + Sender.Header.ToString() + "'", m_dbConnection);
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
    }
}
