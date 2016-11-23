using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Resources;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MessageBox = System.Windows.MessageBox;
using Timer = System.Timers.Timer;
using MouseEventHandler = System.Windows.Forms.MouseEventHandler;
using MouseEventArgs = System.Windows.Forms.MouseEventArgs;

namespace TaskScheduler
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region variables and constants
        private const double Interval60Minutes = 60 * 60 * 1000; // ms in hour
        private readonly Timer _checkTime = new Timer(Interval60Minutes);
        private EmailProcess _emailer;
        private TasksView _listOfTasks;
        private bool _emailSentToday = false;
        private DateTime _defaultEmailTime;
        private NewTaskPage _newTask = new NewTaskPage();
        private readonly EmailLogin _newEmailPerson = new EmailLogin();
        private PersonEmail _newEmailer;
        private bool _isEmailSaved = false;
        private readonly string _emailInfo = "EmailInfo.bin";
        private readonly string _taskData = "TasksData.bin";
        private readonly NotifyIcon _notifyIcon = new NotifyIcon();
        #endregion

        public MainWindow()
        {
            InitializeComponent();

            _notifyIcon.Icon = new Icon(Resource.notepad2, 40, 40);
            this.StateChanged += new EventHandler(Window_Resize);
            _notifyIcon.MouseDoubleClick += new MouseEventHandler(Window_Unminimize);

            DeserializeEmail(_emailInfo); //check if saved email login exists

            //with no saved login, prompt user
            if (_isEmailSaved == false)
            {
                _newEmailPerson.ShowDialog();

                _newEmailer = new PersonEmail(_newEmailPerson.EmailUser, _newEmailPerson.EmailPass)
                {
                    SendingTo = _newEmailPerson.SendToEmail
                };

                if (_newEmailPerson.WillSerialize)
                {
                    SerializeTask(_newEmailer, _emailInfo);
                }
            }

            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            _checkTime.Elapsed += checkTime_Elapsed;
            _checkTime.Enabled = true;
            _emailer = new EmailProcess(_newEmailer.EmailAddress, _newEmailer.Password, _newEmailer.SendingTo);
            _listOfTasks = new TasksView();
            TaskList.ItemsSource = _listOfTasks;
            TaskList.Items.Refresh();
            DeserializeTasks(_taskData);

            if (_taskData.Count() > 0) // if tasks were loaded, auto select first time
            {
                TaskList.SelectedIndex = 0;
            }
            //MessageBox.Show(System.IO.Path.GetDirectoryName(
            //    System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase));
        }

        #region NotifyIcon Handlers
        private void Window_Resize(object sender, EventArgs e)
        {
            if (this.WindowState != WindowState.Minimized) return;
            _notifyIcon.Visible = true;
            this.ShowInTaskbar = false;
            this.Hide();
        }

        private void Window_Unminimize(object sender, MouseEventArgs e)
        {
            this.Show();
            this.WindowState = WindowState.Normal;
            _notifyIcon.Visible = false;
            this.ShowInTaskbar = true;
        }
        #endregion

        private void checkTime_Elapsed(object sender, ElapsedEventArgs e)
        {
            var timeNow = DateTime.Now;

            foreach (var emails in _listOfTasks)
            {
                if (timeNow.CompareTo(emails.RemainderSendAt) > 0)
                {
                    _emailer.SendMail(emails.TaskName, emails.Description);
                    emails.RemainderSendAt = emails.RemainderSendAt.AddDays(emails.Frequency);
                }

            }
            //if (temp.Day == defaultEmailTime.Day)
            //{
            //    defaultEmailTime = new DateTime(temp.Year, temp.Month, temp.Day, 17, 0, 0);
            //}
            //if (isTimeReady(defaultEmailTime) == true)
            //{
            //    var testsub = "ToDo: " + listOfTasks[taskList.Items.IndexOf(taskList.SelectedItem)].TaskName;
            //    var testMessage = listOfTasks[taskList.Items.IndexOf(taskList.SelectedItem)].Description;

            //    emailer.SendMail(testsub, testMessage);


            //    emailSentToday = true;

            //    var newday = temp.Day + 1;

            //    defaultEmailTime = new DateTime(temp.Year, temp.Month, newday, 17, 0, 0);

            //    MessageBox.Show($"COMPLETED!");
            //}

            //foreach (var itemTask in listOfTasks)
            //{
            //    if (temp.CompareTo(itemTask.RemainderSendAt) > 0 && temp.Day == itemTask.RemainderSendAt.Day)
            //    {
            //        emailer.SendMail(itemTask.TaskName, itemTask.Description);
            //        itemTask.RemainderSendAt = Convert.ToDateTime($"{itemTask.RemainderSendAt.Month}/{itemTask.RemainderSendAt.Day + 30}/{itemTask.RemainderSendAt.Year}");

            //        MessageBox.Show(itemTask.RemainderSendAt.ToString() + "Completed!");
            //    }
            //}


        }

        private bool IsTimeReady(DateTime timerunning)
        {
            if (DateTime.Now.CompareTo(timerunning) > 0)
            {
                return true;
            }
            return false;
        }

        private void AddTask_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void TaskList_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_listOfTasks.Count != 0)
            {
                TextInfo.IsEnabled = true;
                TextInfo.Text = _listOfTasks[TaskList.Items.IndexOf(TaskList.SelectedItem)].Description;
            }
            else
            {
                TextInfo.Text = "";
            }
            
        }

        private void SaveText_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _listOfTasks[TaskList.Items.IndexOf(TaskList.SelectedItem)].Description = TextInfo.Text;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void MainWindow_OnClosed(object sender, EventArgs e)
        {
            SerializeTask(_listOfTasks, _taskData);
            GC.Collect();
            Environment.Exit(0);
        }

        private static void SerializeTask(TasksView listOfTasksSerialize, string fileName)
        {
            try
            {
                using (Stream stream = File.Open(fileName, FileMode.Create))
                {
                    BinaryFormatter bin = new BinaryFormatter();
                    bin.Serialize(stream, listOfTasksSerialize);
                    
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private static void SerializeTask(PersonEmail savedEmail, string fileName)
        {
            try
            {
                using (Stream stream = File.Open(fileName, FileMode.Create))
                {
                    BinaryFormatter bin = new BinaryFormatter();
                    bin.Serialize(stream, savedEmail);

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeserializeEmail(string fileName)
        {
            try
            {
                
                using (Stream stream = File.Open(fileName, FileMode.Open))
                {
                    BinaryFormatter bin = new BinaryFormatter();
                    _newEmailer = (PersonEmail) bin.Deserialize(stream);
                }

                _isEmailSaved = true;

            }
            catch (Exception)
            {
                _isEmailSaved = false;
            }
        }

        private void SerializeButton_OnClick(object sender, RoutedEventArgs e)
        {
            SerializeTask(_listOfTasks, _taskData);
        }

        private void DeserializeTasks(string fileName)
        {
            try
            {
                if (File.Exists(fileName))
                {
                    using (Stream stream = File.Open(fileName, FileMode.Open))
                    {
                        BinaryFormatter bin = new BinaryFormatter();
                        _listOfTasks = (TasksView)bin.Deserialize(stream);
                        TaskList.ItemsSource = _listOfTasks;
                        TaskList.Items.Refresh();
                    }
                }
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void MenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var testsub = "ToDo: " + _listOfTasks[TaskList.Items.IndexOf(TaskList.SelectedItem)].TaskName;
                var testMessage = _listOfTasks[TaskList.Items.IndexOf(TaskList.SelectedItem)].Description;

                _emailer.SendMail(testsub, testMessage);

                MessageBox.Show("COMPLETED!", "Email Sent Successfully", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        private void EmailLogin_OnClick(object sender, RoutedEventArgs e)
        {
            _newEmailPerson.ShowDialog();

            if (!string.IsNullOrEmpty(_newEmailPerson.EmailUser))
            {
                
            }

            try
            {
                _newEmailer = new PersonEmail(_newEmailPerson.EmailUser, _newEmailPerson.EmailPass)
                {
                    SendingTo = _newEmailPerson.SendToEmail
                };

                if (_newEmailPerson.WillSerialize)
                {
                    SerializeTask(_newEmailer, _emailInfo);
                }

                _emailer = new EmailProcess(_newEmailer.EmailAddress, _newEmailer.Password, _newEmailer.SendingTo);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Email information was not entered", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            
        }

        #region Commands
        private void OpenCommandBinding_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                EditingTask newEdit = new EditingTask(_listOfTasks[TaskList.Items.IndexOf(TaskList.SelectedItem)]);
                newEdit.ShowDialog();

                if (newEdit.IsCanceled == false)
                {
                    _listOfTasks[TaskList.Items.IndexOf(TaskList.SelectedItem)].TaskName = newEdit._taskToEdit.TaskName;
                    _listOfTasks[TaskList.Items.IndexOf(TaskList.SelectedItem)].Description = newEdit._taskToEdit.Description;
                    _listOfTasks[TaskList.Items.IndexOf(TaskList.SelectedItem)].Frequency = newEdit._taskToEdit.Frequency;
                    TaskList.ItemsSource = _listOfTasks;
                    TextInfo.Text = _listOfTasks[TaskList.Items.IndexOf(TaskList.SelectedItem)].Description;
                    TaskList.Items.Refresh();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            
        }

        private void SaveCommandBinding_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                _listOfTasks[TaskList.Items.IndexOf(TaskList.SelectedItem)].Description = TextInfo.Text;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeleteCommandBinding_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                var temp = TaskList.Items.IndexOf(TaskList.SelectedItem);
                var newIndex = temp;
                if (temp == 0)
                {
                    newIndex++;
                }
                else
                {
                    newIndex--;
                }

                TaskList.SelectedIndex = newIndex;
                _listOfTasks.RemoveAt(temp);
                TaskList.ItemsSource = _listOfTasks;
                TaskList.Items.Refresh();
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SendCommandBinding_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                var testsub = "ToDo: " + _listOfTasks[TaskList.Items.IndexOf(TaskList.SelectedItem)].TaskName;
                var testMessage = _listOfTasks[TaskList.Items.IndexOf(TaskList.SelectedItem)].Description;

                _emailer.SendMail(testsub, testMessage);

                MessageBox.Show("COMPLETED!");
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        private void AddNewTaskCommandBinding_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            _newTask = new NewTaskPage();

            _newTask.ShowDialog();

            TaskObj newTaskItem = new TaskObj(_newTask.TaskName)
            {
                RemainderSendAt = _newTask.Remind,
                Frequency = _newTask.Frequency
            };

            _listOfTasks.AddToTasks(newTaskItem);
            TaskList.Items.Refresh();


            TaskList.Items.MoveCurrentToLast();
            TaskList.SelectedItem = TaskList.Items.CurrentItem;

            TextInfo.Focus();
        }

        #endregion

    }
}
