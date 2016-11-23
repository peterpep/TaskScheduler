using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskScheduler
{
    [Serializable()]
    public class TaskObj
    {
        private string _taskName;
        private string _description;
        private DateTime _remainderSendAt;
        private int _frequency;

        public TaskObj(string name)
        {
            TaskName = name;
            Description = "";
        }

        public string TaskName
        {
            get { return _taskName; }
            set { _taskName = value; }
        }

        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        public DateTime RemainderSendAt
        {
            get { return _remainderSendAt; }
            set { _remainderSendAt = value; }
        }

        public int Frequency
        {
            get { return _frequency; }
            set { _frequency = value; }
        }

        public override string ToString()
        {
            return TaskName;
        }
    }
}
