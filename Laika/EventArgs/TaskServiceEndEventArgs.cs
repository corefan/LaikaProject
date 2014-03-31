using System;

namespace Laika.Event
{
    public class TaskServiceEndEventArgs : EventArgs
    {
        public TaskServiceEndEventArgs(string name)
        {
            TaskServiceName = name;
        }

        public string TaskServiceName { get; private set; }
    }
}
