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
    /// Item that contains info about a mail for a user
    /// </summary>
    public partial class MailItem : UserControl
    {
        #region DependencyPropertys
        private static readonly DependencyProperty FromProperty = DependencyProperty.Register("From", typeof(string), typeof(MailItem), new UIPropertyMetadata(""));
        private static readonly DependencyProperty SubjectProperty = DependencyProperty.Register("Subject", typeof(string), typeof(MailItem), new UIPropertyMetadata(""));
        private static readonly DependencyProperty DateProperty = DependencyProperty.Register("Date", typeof(string), typeof(MailItem), new UIPropertyMetadata(""));
        private static readonly DependencyProperty MessageIdProperty = DependencyProperty.Register("MessageId", typeof(string), typeof(MailItem), new UIPropertyMetadata(""));
        private static readonly DependencyProperty IsMarkedProperty = DependencyProperty.Register("IsMarked", typeof(bool), typeof(MailItem), new UIPropertyMetadata(false));
        #endregion

        public string From
        {
            get { return (string)GetValue(FromProperty); }
            set { SetValue(FromProperty, value); }
        }
        public string IsMarked
        { 
            get { return (string)GetValue(FromProperty); } 
            set { SetValue(FromProperty, value); } 
        }
        public string Subject 
        {
            get { return (string)GetValue(SubjectProperty); }
            set { SetValue(SubjectProperty, value); } 
        }
        public string Date
        {
            get { return (string)GetValue(DateProperty); }
            set { SetValue(DateProperty, value); }
        }
        public string MessageId
        {
            get { return (string)GetValue(MessageIdProperty); }
            set { SetValue(MessageIdProperty, value); }
        }
        
        public MailItem()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        /// <summary>
        /// Mail in list have been clicked
        /// view mail
        /// </summary>
        private void MailClicked(object sender, MouseButtonEventArgs e)
        {
            foreach (MailClient client in ((MainWindow)Window.GetWindow(this)).ClientPanel.Children) 
            {
                if (client.HeadUser.SelectedItem != null)
                {
                    string Body = client.Get_Mail_Body(((MailItem)sender).MessageId);
                    if (!String.IsNullOrEmpty(Body)) ((MainWindow)Window.GetWindow(this)).MailView.NavigateToString(Body);
                    return;
                }
            }
        }

        #region pretty functions
        private void Mouseover(object sender, MouseEventArgs e)
        {
            this.Background = Brushes.AliceBlue;
        }

        private void Mouseleave(object sender, MouseEventArgs e)
        {
            this.Background = Brushes.Transparent;
        }
        #endregion
    }
}
