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
    /// Interaction logic for MessageboxWithReturn.xaml
    /// </summary>
    public partial class NewClient : Window
    {
        public string[] Answers;
        public NewClient(string[] Questions)
        {
            InitializeComponent();
            Question1.Text = Questions[0];
            if (Questions.Length > 1) {
                Question2.Visibility = System.Windows.Visibility.Visible;
                Question2.Text = Questions[1];
                Answer2.Visibility = System.Windows.Visibility.Visible;
            }
        }

        private void Done_Click(object sender, RoutedEventArgs e)
        {
            if (Answer2.Visibility == System.Windows.Visibility.Visible){
                string[] Answer = {Answer1.Text , Answer2.Password};
                Answers = Answer;
            }
            else {
                string[] Answer = { Answer2.Password};
                Answers = Answer;
            }
            this.DialogResult = true;
            this.Close();
        }
    }
}
