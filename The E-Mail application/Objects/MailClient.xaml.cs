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

namespace The_E_Mail_application
{
    /// <summary>
    /// Interaction logic for MailUser.xaml
    /// </summary>
    public partial class MailUser : UserControl
    {
        string Client_Mail;
        string Password;
        string Pop3 = "mail.renennielsen.dk";
        int Port = 110;
        bool UseSsl = true;

        public MailUser(string E_Mail, string password)//, string pop3, int port, bool usessl)
        {
            Client_Mail = E_Mail;
            Password = password;
            //Pop3 = pop3;
            //Port = port;
            //UseSsl = usessl;
            InitializeComponent();
            User.Header = Client_Mail;
            //Load_Mail_List("pop3.live.com", 995, true);
            Load_Mail_List();
        }

        public struct MailDataList
        {
            public string From { get; set; }
            public string Subject { get; set; }
            public string Date { get; set; }
            public string MessageId { get; set; }
        }

        public MailDataList[] Load_Mail_List()
        {
            using (Pop3Client client = new Pop3Client())
            {
                client.Connect(Pop3, Port, UseSsl);
                client.Authenticate(Client_Mail.ToString(), Password, AuthenticationMethod.Auto);

                int messageCount = client.GetMessageCount();
                MailDataList[] AllMails = new MailDataList[messageCount];
                for (int i = 0; i < messageCount; i--)
                {
                    MessageHeader headers = client.GetMessageHeaders(i);
                    RfcMailAddress from = headers.From;
                    AllMails[i].Subject = headers.Subject;
                    AllMails[i].Date = headers.Date;
                    if (!String.IsNullOrEmpty(from.DisplayName)) AllMails[i].From = from.DisplayName;
                    else AllMails[i].From = from.Address;
                    AllMails[i].MessageId = headers.MessageId;
                }
                return AllMails;
            }
        }
    }
}
