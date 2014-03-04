using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laika.Event
{
    /// <summary>
    /// event 클래스
    /// </summary>
    public class Event
    {
        /// <summary>
        /// 생성자
        /// </summary>
        public Event()
        {
            DueTime = new TimeSpan(0, 0, 0);
            Interval = new TimeSpan(0, 0, 1);

            StartTime = DateTime.MinValue;
            EndTime = DateTime.MaxValue;
        }
        /// <summary>
        /// 최대 실행 횟수
        /// </summary>
        /// <param name="maxRuns">최대 실행 횟수</param>
        /// <returns>해당 인스턴스</returns>
        public Event MaxRuns(int maxRuns)
        {
            MaxRunCount = maxRuns;

            return this;
        }
        /// <summary>
        /// 시작 시간 설정
        /// </summary>
        /// <param name="timespan">
        /// 현재 시간으로 부터 timespan이후
        /// 설정하지 않을 시 현재 시간으로 설정 됩니다.
        /// </param>
        /// <returns>해당 인스턴스</returns>
        public Event StartAt(TimeSpan timespan)
        {
            DueTime = timespan;
            StartTime = DateTime.Now.AddTicks(timespan.Ticks);
            return this;
        }
        /// <summary>
        /// 인터벌 설정
        /// </summary>
        /// <param name="timespan">
        /// timespan 시간마다 실행.
        /// 미설정 시 1초다마 실행 됩니다.
        /// </param>
        /// <returns></returns>
        public Event Every(TimeSpan timespan)
        {
            Interval = timespan;
            
            return this;
        }
        /// <summary>
        /// 종료 시간 설정
        /// </summary>
        /// <param name="timespan">
        /// 시작시간으로 부터 timespan 후 종료.
        /// 미설정 시 계속 실행 됩니다.
        /// </param>
        /// <returns>해당 인스턴스</returns>
        public Event StopAt(TimeSpan timespan)
        {
            Stop = timespan;
            EndTime = StartTime.AddTicks(timespan.Ticks);
            return this;
        }
        /// <summary>
        /// 설정된 최대 실행 횟수
        /// </summary>
        public int MaxRunCount { get; private set; }
        /// <summary>
        /// 시작 딜레이
        /// </summary>
        public TimeSpan DueTime { get; private set; }
        /// <summary>
        /// 메소드 실행 인터벌
        /// </summary>
        public TimeSpan Interval { get; private set; }
        /// <summary>
        /// 시작시간으로 부터 종료 시간 설정
        /// </summary>
        public TimeSpan Stop { get; private set; }
        /// <summary>
        /// 시작시간
        /// </summary>
        public DateTime StartTime { get; private set; }
        /// <summary>
        /// 종료시간
        /// </summary>
        public DateTime EndTime { get; private set; }
    }
}
