using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;



namespace TaskScheduler
{
    [Serializable()]
    class TasksView : ObservableCollection<TaskObj>
    {
        
        public void AddToTasks(TaskObj taskToAdd)
        {
            foreach (var task in this)
            {
                if (task.TaskName == taskToAdd.TaskName)
                {
                    MessageBox.Show("This item already exists");
                    return;
                }
            }
            this.Add(taskToAdd);
            
        }

       
        public void RemoveTask(TaskObj Name)
        {
            this.Remove(Name);
        }


        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
