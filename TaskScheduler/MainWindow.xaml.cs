using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Timer = System.Timers.Timer;

namespace TaskScheduler
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region variables and constants
        const double interval60Minutes = 60 * 60 * 1000; // ms in hour
        Timer checkTime = new Timer(interval60Minutes);
        private EmailProcess emailer;
        private TasksView listOfTasks;
        private bool emailSentToday = false;
        private DateTime defaultEmailTime;
        private NewTaskPage newTask = new NewTaskPage();
        private EmailLogin newEmailPerson = new EmailLogin();
        private PersonEmail newEmailer;
        private bool isEmailSaved = false;
        private string emailInfo = "EmailInfo.bin";
        private string taskData = "TasksData.bin";
        #endregion

        public MainWindow()
        {
            InitializeComponent();

            DeserializeEmail(emailInfo);

            if (isEmailSaved == false)
            {
                newEmailPerson.ShowDialog();

                newEmailer = new PersonEmail(newEmailPerson.EmailUser, newEmailPerson.EmailPass);
                newEmailer.SendingTo = newEmailPerson.SendToEmail;

                if (newEmailPerson.WillSerialize == true)
                {
                    SerializeTask(newEmailer, emailInfo);
                }
            }

            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;

            checkTime.Elapsed += new ElapsedEventHandler(checkTime_Elapsed);
            checkTime.Enabled = true;
            emailer = new EmailProcess(newEmailer.EmailAddress, newEmailer.Password, newEmailer.SendingTo);
            listOfTasks = new TasksView();
            taskList.ItemsSource = listOfTasks;
            taskList.Items.Refresh();
            DeserializeTasks(taskData);
            //MessageBox.Show(System.IO.Path.GetDirectoryName(
            //    System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase));
        }


        private void checkTime_Elapsed(object sender, ElapsedEventArgs e)
        {
            var timeNow = DateTime.Now;

            foreach (var emails in listOfTasks)
            {
                if (timeNow.CompareTo(emails.RemainderSendAt) > 0)
                {
                    emailer.SendMail(emails.TaskName, emails.Description);
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

        private bool isTimeReady(DateTime timerunning)
        {
            if (DateTime.Now.CompareTo(timerunning) > 0)
            {
                return true;
            }
            return false;
        }

        private void Send_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var testsub = "ToDo: " + listOfTasks[taskList.Items.IndexOf(taskList.SelectedItem)].TaskName;
                var testMessage = listOfTasks[taskList.Items.IndexOf(taskList.SelectedItem)].Description;

                emailer.SendMail(testsub, testMessage);

                MessageBox.Show("COMPLETED!");
            }
            catch (Exception ex)
            {
                
                MessageBox.Show(ex.Message);
            }
            
        }

        private void AddTask_Click(object sender, RoutedEventArgs e)
        {
            newTask = new NewTaskPage();

            newTask.ShowDialog();
            
            TaskObj newTaskItem = new TaskObj(newTask.TaskName);
            newTaskItem.RemainderSendAt = newTask.Remind;
            newTaskItem.Frequency = newTask.Frequency;
            listOfTasks.AddToTasks(newTaskItem);
            taskList.Items.Refresh();


            taskList.Items.MoveCurrentToLast();
            taskList.SelectedItem = taskList.Items.CurrentItem;
        }

        private void RemoveTask_Click(object sender, RoutedEventArgs e)
        {
            
            try
            {
                var temp = taskList.Items.IndexOf(taskList.SelectedItem);
                int newIndex = temp;
                if (temp == 0)
                {
                    newIndex++;
                }
                else
                {
                    newIndex--;
                }

                taskList.SelectedIndex = newIndex;
                listOfTasks.RemoveAt(temp);
                taskList.ItemsSource = listOfTasks;
                taskList.Items.Refresh();
            }
            catch (Exception ex)
            {
                
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            
        }

        private void TaskList_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listOfTasks.Count != 0)
            {
                textInfo.IsEnabled = true;
                textInfo.Text = listOfTasks[taskList.Items.IndexOf(taskList.SelectedItem)].Description;
            }
            else
            {
                textInfo.Text = "";
            }
            
        }

        private void SaveText_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                listOfTasks[taskList.Items.IndexOf(taskList.SelectedItem)].Description = textInfo.Text;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void MainWindow_OnClosed(object sender, EventArgs e)
        {
            SerializeTask(listOfTasks, taskData);
            GC.Collect();
            Environment.Exit(0);
        }

        private static void SerializeTask(TasksView listOfTasksSerialize, string FileName)
        {
            try
            {
                using (Stream stream = File.Open(FileName, FileMode.Create))
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

        private static void SerializeTask(PersonEmail savedEmail, string FileName)
        {
            try
            {
                using (Stream stream = File.Open(FileName, FileMode.Create))
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

        private void DeserializeEmail(string FileName)
        {
            try
            {
                
                using (Stream stream = File.Open(FileName, FileMode.Open))
                {
                    BinaryFormatter bin = new BinaryFormatter();
                    newEmailer = (PersonEmail) bin.Deserialize(stream);
                }

                isEmailSaved = true;

            }
            catch (Exception ex)
            {
                isEmailSaved = false;
            }
        }

        private void SerializeButton_OnClick(object sender, RoutedEventArgs e)
        {
            SerializeTask(listOfTasks, taskData);
        }

        private void DeserializeTasks(string FileName)
        {
            try
            {
                if (File.Exists(FileName) == true)
                {
                    using (Stream stream = File.Open(FileName, FileMode.Open))
                    {
                        BinaryFormatter bin = new BinaryFormatter();
                        listOfTasks = (TasksView)bin.Deserialize(stream);
                        taskList.ItemsSource = listOfTasks;
                        taskList.Items.Refresh();
                    }
                }
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
