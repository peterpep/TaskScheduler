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
    /// Interaction logic for NewTaskPage.xaml
    /// </summary>
    public partial class NewTaskPage : Window
    {
        private string taskName;
        private DateTime remind;
        private int frequency;

        public NewTaskPage()
        {
            InitializeComponent();
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
        }

        public string TaskName
        {
            get { return taskName; }
            private set { taskName = value; }
        }

        public DateTime Remind
        {
            get { return remind; }
            private set { remind = value; }
        }

        public int Frequency
        {
            get { return frequency; }
            private set { frequency = value; }
        }


        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(newTaskName.Text) == false)
            {
                try
                {
                    TaskName = newTaskName.Text;
                    Remind = Convert.ToDateTime(remindDate.Text);
                    Frequency = Convert.ToInt32(frequencyInput.Text);
                    Close();
                }
                catch (Exception ex)
                {
                    
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Please enter required data", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
