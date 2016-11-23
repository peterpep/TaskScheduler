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

namespace TaskScheduler
{
    /// <summary>
    /// Interaction logic for EmailLogin.xaml
    /// </summary>
    [Serializable()]
    public partial class EmailLogin : Window
    {

        private string emailUser;
        private string emailPass;
        private string sendToEmail;
        private bool willSerialize = false;

        public EmailLogin()
        {
            InitializeComponent();
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;

        }

        public string EmailUser
        {
            get { return emailUser; }
            private set { emailUser = value; }
        }

        public string EmailPass
        {
            get { return emailPass; }
            private set { emailPass = value; }
        }

        public string SendToEmail
        {
            get { return sendToEmail; }
            private set { sendToEmail = value; }
        }

        public bool WillSerialize
        {
            get { return willSerialize; }
            private set { willSerialize = value; }
        }


        private void OkBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                
                EmailUser = UsernameTxt.Text;
                EmailPass = PasswordTxt.Text;
                SendToEmail = SendToTxt.Text;
                if (emailUser.Contains("@") && sendToEmail.Contains("@"))
                {
                    Close();
                }
                else
                {
                    MessageBox.Show("Please enter valid email addresses.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SaveLoginInfo_Checked(object sender, RoutedEventArgs e)
        {
            WillSerialize = true;
        }
    }
}
