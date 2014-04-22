using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Laika.Pools
{
    public class ObjectPooling<T> : Queue<Instance<T>>, IDisposable
            where T : IDisposable, new()
    {
        public ObjectPooling()
            : this(DefaultPoolSize)
        { }

        public ObjectPooling(int capacity)
        {
            _capacity = capacity;
            EnqueueObject();
        }

        ~ObjectPooling()
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
                ClearInstance();

            _disposed = true;
        }

        private void ClearInstance()
        {
            foreach (var i in this)
            {
                i.Dispose();
                this.Clear();
            }
        }

        private void EnqueueObject()
        {
            for (int i = 0; i < _capacity; i++)
            {
                this.Enqueue(new Instance<T>(this));
            }
        }

        public Instance<T> Get()
        {
            lock (_lockQueue)
            {
                if (this.Count > 0)
                    return this.Dequeue();

                else return new Instance<T>();
            }
        }
        internal void EnqueueInstance(Instance<T> instance)
        {
            lock (_lockQueue)
            {
                this.Enqueue(instance);
            }
        }

        private bool _disposed = false;
        private object _lockQueue = new object();
        private int _capacity;
        public const int DefaultPoolSize = 10;
    }

    public class Instance<T> : IDisposable
        where T : IDisposable, new()
    {
        internal Instance()
            : this(null)
        {

        }

        ~Instance()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            if (_master == null)
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }
            else
            {
                _master.EnqueueInstance(this);
            }
        }

        public bool HasMaster()
        {
            if (_master == null)
                return false;

            return true;
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
            _master = null;
            PooledObject.Dispose();
        }

        internal Instance(ObjectPooling<T> master)
        {
            _master = master;
            PooledObject = new T();
        }

        private bool _disposed = false;
        private ObjectPooling<T> _master = null;
        public T PooledObject { get; private set; }
    }
}
