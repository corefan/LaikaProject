﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Laika.Diagnotics
{
    public class Counter
    {
        public Counter()
            : this(default(TimeSpan))
        { }
        public Counter(TimeSpan initializeInterval)
        {
            if (initializeInterval != default(TimeSpan))
            {
                _isPeriod = true;
            }

            _initializeInterval = initializeInterval;
            _initializeTime = DateTime.Now;
        }

        public long Increment()
        {
            if (_isPeriod == true && DateTime.Now >= _initializeTime + _initializeInterval)
            {
                _initializeTime = DateTime.Now;
                _pastCount = Interlocked.Read(ref _count);
                Interlocked.Exchange(ref _count, 0);
                return Interlocked.Increment(ref _count);
            }

            return Interlocked.Increment(ref _count);
        }

        public long Decrement()
        {
            if (_isPeriod == true && DateTime.Now >= _initializeTime + _initializeInterval)
            {
                _initializeTime = DateTime.Now;
                _pastCount = Interlocked.Read(ref _count);
                Interlocked.Exchange(ref _count, 0);
                return Interlocked.Decrement(ref _count);
            }

            return Interlocked.Decrement(ref _count);
        }

        public long CurrentCount()
        {
            if (_isPeriod == true && DateTime.Now >= _initializeTime + _initializeInterval)
            {
                _initializeTime = DateTime.Now;
                _pastCount = Interlocked.Read(ref _count);
                Interlocked.Exchange(ref _count, 0);
            }
            return Interlocked.Read(ref _count);
        }

        public long PastCount()
        {
            if (_isPeriod == true && DateTime.Now >= _initializeTime + _initializeInterval + _initializeInterval)
            {
                Interlocked.Exchange(ref _count, 0);
            }
            return _pastCount;
        }

        private DateTime _initializeTime;
        private TimeSpan _initializeInterval;
        private long _count;
        private long _pastCount;
        private bool _isPeriod = false;
    }
}
