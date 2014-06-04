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
    /// Dynamic messagebox.
    /// </summary>
    public partial class NewClient : Window
    {
        public string[] Answers;

        /// <summary>
        /// Dynamic messagebox that use Questions to set questions text, and saves Answer, in Answers used to return.
        /// </summary>
        /// <param name="Questions">A Array, that shall contain info to the setup,
        /// [0] 1. Textbox visible
        /// [1] 1. Passwordbox visible
        /// [2] 2. Textbox visible
        /// [3] 3. Textbox (only numbers) visible
        /// [4] 1. Checkbox visible
        /// [5] 4. Textbox visible
        /// [6] 5. Textbox (only numbers) visible
        /// [7] 2. Checkbox visible
        /// </param>
        public NewClient(string[] Questions, string[] Answers)
        {
            InitializeComponent();
            for (int i = 0; i < Questions.Length; i++)
            {
                if (String.IsNullOrEmpty(Questions[i])) continue;
                #region initialize View
                switch (i) { 
                    case 0:
                        Question1.Visibility = System.Windows.Visibility.Visible;
                        Question1.Text = Questions[i];
                        Answer1.Visibility = System.Windows.Visibility.Visible;
                        if (Answers != null) { if (Answers.Length > i) { Answer1.Text = Answers[i]; } }
                        break;
                    case 1:
                        Question2.Visibility = System.Windows.Visibility.Visible;
                        Question2.Text = Questions[i];
                        Answer2.Visibility = System.Windows.Visibility.Visible;
                        if (Answers != null) { if (Answers.Length > i) { Answer2.Password = Answers[i]; } }
                        break;
                    case 2:
                        Question3.Visibility = System.Windows.Visibility.Visible;
                        Question3.Text = Questions[i];
                        Answer3.Visibility = System.Windows.Visibility.Visible;
                        if (Answers != null) { if (Answers.Length > i) { Answer3.Text = Answers[i]; } }
                        break;
                    case 3:
                        Question4.Visibility = System.Windows.Visibility.Visible;
                        Question4.Text = Questions[i];
                        Answer4.Visibility = System.Windows.Visibility.Visible;
                        if (Answers != null) { if (Answers.Length > i) { Answer4.Text = Answers[i]; } }
                        break;
                    case 4:
                        Question5.Visibility = System.Windows.Visibility.Visible;
                        Question5.Text = Questions[i];
                        Answer5.Visibility = System.Windows.Visibility.Visible;
                        if (Answers != null) { if (Answers.Length > i) { Answer5.IsChecked = Convert.ToBoolean(Answers[i]); } }
                        break;
                    case 5:
                        Question6.Visibility = System.Windows.Visibility.Visible;
                        Question6.Text = Questions[i];
                        Answer6.Visibility = System.Windows.Visibility.Visible;
                        if (Answers != null) { if (Answers.Length > i) { Answer6.Text = Answers[i]; } }
                        break;
                    case 6:
                        Question7.Visibility = System.Windows.Visibility.Visible;
                        Question7.Text = Questions[i];
                        Answer7.Visibility = System.Windows.Visibility.Visible;
                        if (Answers != null) { if (Answers.Length > i) { Answer7.Text = Answers[i]; } }
                        break;
                    case 7:
                        Question8.Visibility = System.Windows.Visibility.Visible;
                        Question8.Text = Questions[i];
                        Answer8.Visibility = System.Windows.Visibility.Visible;
                        if (Answers != null) { if (Answers.Length > i) { Answer8.IsChecked = Convert.ToBoolean(Answers[i]); } }
                        break;
                }
                #endregion
            }
        }

        #region Ui functions
        /// <summary>
        /// Save all visible results in Answers.
        /// </summary>
        private void Done_Click(object sender, RoutedEventArgs e)
        {
            List<string> answers = new List<string>();
            for (int i = 0; i <= 7; i++)
            {
                #region Get answers
                switch (i) {
                    case 0:
                        if (Answer1.Visibility != System.Windows.Visibility.Visible) answers.Add("");
                        else if (String.IsNullOrEmpty(Answer1.Text)) { 
                            MessageBox.Show("Value can´t bee null", "Error");
                            return;
                        }
                        answers.Add(Answer1.Text);
                        break;
                    case 1:
                        if (Answer2.Visibility != System.Windows.Visibility.Visible) answers.Add("");
                        else if (String.IsNullOrEmpty(Answer2.Password))
                        {
                            MessageBox.Show("Value can´t bee null", "Error");
                            return;
                        }
                        answers.Add(Answer2.Password);
                        break;
                    case 2:
                        if (Answer3.Visibility != System.Windows.Visibility.Visible) answers.Add("");
                        else if (String.IsNullOrEmpty(Answer3.Text))
                        {
                            MessageBox.Show("Value can´t bee null", "Error");
                            return;
                        }
                        answers.Add(Answer3.Text);
                        break;
                    case 3:
                        int Tryint = 0;
                        if (Answer4.Visibility != System.Windows.Visibility.Visible) answers.Add("");
                        else if (String.IsNullOrEmpty(Answer4.Text))
                        {
                            MessageBox.Show("Value can´t be null", "Error");
                            return;
                        }
                        else if (!Int32.TryParse(Answer4.Text, out Tryint)) {
                            MessageBox.Show("Value shall be a valid number", "Error");
                            return;
                        }
                        answers.Add(Answer4.Text);
                        break;
                    case 4:
                        if (Answer5.Visibility != System.Windows.Visibility.Visible) answers.Add("");
                        answers.Add(Answer5.IsChecked.ToString());
                        break;
                    case 5:
                        if (Answer6.Visibility != System.Windows.Visibility.Visible) answers.Add("");
                        else if (String.IsNullOrEmpty(Answer6.Text))
                        {
                            MessageBox.Show("Value can´t bee null", "Error");
                            return;
                        }
                        answers.Add(Answer6.Text);
                        break;
                    case 6:
                        int Tryint2 = 0;
                        if (Answer7.Visibility != System.Windows.Visibility.Visible) answers.Add("");
                        else if (String.IsNullOrEmpty(Answer7.Text))
                        {
                            MessageBox.Show("Value can´t be null", "Error");
                            return;
                        }
                        else if (!Int32.TryParse(Answer7.Text, out Tryint2))
                        {
                            MessageBox.Show("Value shall be a valid number", "Error");
                            return;
                        }
                        answers.Add(Answer7.Text);
                        break;
                    case 7:
                        if (Answer8.Visibility != System.Windows.Visibility.Visible) answers.Add("");
                        answers.Add(Answer8.IsChecked.ToString());
                        break;
                }
                #endregion
            }
            Answers = answers.ToArray();
            this.DialogResult = true;
            this.Close();
        }
        #endregion
    }
}
