using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;

namespace Laika.Event
{
    /// <summary>
    /// 이벤트 스케줄러
    /// </summary>
    public class EventScheduler : IDisposable
    {
        public EventScheduler()
        {
            _workerThread = new Thread(DoJob);
            _workerThread.Start();
        }

        ~EventScheduler()
        {
            Dispose(false);
        }
        /// <summary>
        /// Dispose 패턴
        /// </summary>
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
            _taskTable.Clear();
            _run = false;
            _workerThread.Join();
        }
        /// <summary>
        /// 이벤트 삭제
        /// </summary>
        /// <param name="eventName">이벤트 이름</param>
        /// <returns>삭제 여부</returns>
        public bool RemoveEvent(string eventName)
        {
            TaskService ts = null;
            return _taskTable.TryRemove(eventName, out ts);
        }
        /// <summary>
        /// 이벤트 추가
        /// </summary>
        /// <param name="eventName">이벤트 이름</param>
        /// <param name="eventTask">이벤트 설정</param>
        /// <param name="executeMethod">실행 메소드</param>
        /// <returns>스케쥴러에 10,000개의 이벤트가 있으면 추가 실패로 false가 return 됨.</returns>
        public bool AddEvent(string eventName, Event eventTask, Action executeMethod)
        {
            if (_maxTaskCount <= _taskTable.Count)
                return false;

            TaskService ts = new TaskService(eventName, eventTask, executeMethod);
            ts.EndEvent += EndTask;

            return _taskTable.TryAdd(eventName, ts);
        }

        private void EndTask(object sender, TaskServiceEndEventArgs e)
        {
            TaskService ts = null;
            _taskTable.TryRemove(e.TaskServiceName, out ts);
        }

        private bool _disposed = false;
        private bool _run = true;
        private ConcurrentDictionary<string, TaskService> _taskTable = new ConcurrentDictionary<string, TaskService>();
        private Thread _workerThread = null;
        private const int _maxTaskCount = 10000;

        private void DoJob(object obj)
        {
            while (_run)
            {
                List<TaskService> taskList = _taskTable.Values.ToList();

                foreach (var task in taskList)
                {
                    task.DoTask(null);
                }
                System.Threading.Thread.Sleep(10);

            }
        }
    }
}
