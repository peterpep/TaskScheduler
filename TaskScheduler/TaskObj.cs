using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskScheduler
{
    [Serializable()]
    class TaskObj
    {
        private string taskName;
        private string description;
        private DateTime remainderSendAt;
        private int frequency;

        public TaskObj(string Name)
        {
            TaskName = Name;
            Description = "";
        }

        public string TaskName
        {
            get { return taskName; }
            private set { taskName = value; }
        }

        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        public DateTime RemainderSendAt
        {
            get { return remainderSendAt; }
            set { remainderSendAt = value; }
        }

        public int Frequency
        {
            get { return frequency; }
            set { frequency = value; }
        }

        public override string ToString()
        {
            return TaskName;
        }
    }
}
