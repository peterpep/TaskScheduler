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
    /// Interaction logic for EditingTask.xaml
    /// </summary>
    public partial class EditingTask : Window
    {
        public TaskObj _taskToEdit;
        private bool isCanceled = false;

        public EditingTask(TaskObj inputTask)
        {
            InitializeComponent();

            _taskToEdit = inputTask;

            TaskNameTxt.Text = _taskToEdit.TaskName;
            TaskFrequencyTxt.Text = _taskToEdit.Frequency.ToString();
            TaskDescriptionTxt.Text = _taskToEdit.Description;

        }

        private void Confirm_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                _taskToEdit.TaskName = TaskNameTxt.Text;
                _taskToEdit.Frequency = Convert.ToInt32(TaskFrequencyTxt.Text);
                _taskToEdit.Description = TaskDescriptionTxt.Text;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            }
            
        }

        public bool IsCanceled
        {
            get { return isCanceled; }
            set { isCanceled = value; }
        }

        private void Cancel_OnClick(object sender, RoutedEventArgs e)
        {
            IsCanceled = true;
            Close();
        }
    }
}
