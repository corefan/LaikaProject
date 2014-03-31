using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;

namespace Laika.Event
{
    /// <summary>
    /// 이벤트 스케줄러
    /// </summary>
    public class EventScheduler : IDisposable
    {
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
            lock (_disposeLock)
            {
                List<TaskService> list = _taskTable.Values.ToList();
                list.ForEach(x=>x.Dispose());
                list.Clear();
                _taskTable.Clear();
            }
        }
        /// <summary>
        /// 이벤트 삭제
        /// </summary>
        /// <param name="eventName">이벤트 이름</param>
        /// <returns>삭제 여부</returns>
        public bool RemoveEvent(string eventName)
        {
            TaskService ts = null;
            bool result = _taskTable.TryRemove(eventName, out ts);
            ts.Dispose();
            return result;
        }
        /// <summary>
        /// 이벤트 추가
        /// </summary>
        /// <param name="eventName">이벤트 이름</param>
        /// <param name="eventTask">이벤트 설정</param>
        /// <param name="executeMethod">실행 메소드</param>
        /// <returns></returns>
        public bool AddEvent(string eventName, Event eventTask, Action executeMethod)
        {
            TaskService ts = new TaskService(eventName, eventTask, executeMethod);
            ts.EndEvent += EndTask;
            return _taskTable.TryAdd(eventName, ts);
        }

        private void EndTask(object sender, TaskServiceEndEventArgs e)
        {
            TaskService ts = null;
            _taskTable.TryRemove(e.TaskServiceName, out ts);
        }

        private object _disposeLock = new object();
        private bool _disposed = false;
        private ConcurrentDictionary<string, TaskService> _taskTable = new ConcurrentDictionary<string, TaskService>();
    }
}
