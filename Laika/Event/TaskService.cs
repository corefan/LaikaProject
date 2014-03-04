using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Laika.Event
{
    internal class TaskService : IDisposable
    {
        internal TaskService(string name, Event eventInstance, Action task)
        {
            _taskName = name;
            _event = eventInstance;
            _task = task;

            _timer = new Timer(DoTask);
            _timer.Change(eventInstance.DueTime, eventInstance.Interval);
        }

        ~TaskService()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed == true)
                return;

            if (disposing == true)
                Clear();

            _disposed = true;
        }

        private void Clear()
        {
            _timer.Dispose();
        }

        private void DoTask(object state)
        {
            if (DateTime.Now < _event.StartTime)
                return;

            if (DateTime.Now > _event.EndTime)
            {
                EndTask();
                return;
            }

            if (_event.MaxRunCount > 0 && _event.MaxRunCount <= _runCount)
            {
                EndTask();
                return;
            }

            _task();
            _runCount++;
        }

        private void EndTask()
        {
            if (EndEvent != null)
                EndEvent(_taskName);

            Dispose();
        }

        private string _taskName;
        private Event _event;
        private Action _task;
        private Timer _timer;
        private int _runCount;

        public delegate void EndEventHandler(string name);
        public event EndEventHandler EndEvent;
        private bool _disposed = false;
    }
}
