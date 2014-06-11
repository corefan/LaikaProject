using System;
using System.Threading;
using System.Threading.Tasks;

namespace Laika.Event
{
    internal class TaskService
    {
        internal TaskService(string name, Event eventInstance, Action task)
        {
            _taskName = name;
            _event = eventInstance;
            _task = task;
            _runTime = DateTime.MinValue;
        }

        internal void DoTask(object state)
        {
            DateTime reference = DateTime.Now;
            if (reference < _event.StartTime)
                return;

            if (reference > _event.EndTime)
            {
                EndTask();
                return;
            }

            if (_event.MaxRunCount > 0 && _event.MaxRunCount <= _runCount)
            {
                EndTask();
                return;
            }

            if (reference < _runTime.Add(_event.Interval))
                return;

            if (_run == true)
                return;

            _run = true;
            var result = Task.Factory.StartNew(_task);
            result.ContinueWith(x => _run = false);

            _runTime = reference;
            _runCount++;
        }

        private void EndTask()
        {
            if (EndEvent != null)
                EndEvent(this, new TaskServiceEndEventArgs(_taskName));
        }

        private bool _run = false;
        private string _taskName;
        private Event _event;
        private Action _task;
        private int _runCount;
        private DateTime _runTime;

        public event EndEventHandler EndEvent;
    }
}
