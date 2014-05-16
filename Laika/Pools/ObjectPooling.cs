using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;

namespace Laika.Pools
{
    public class ObjectPooling<T> : ConcurrentQueue<Instance<T>>, IDisposable
            where T : IDisposable, new()
    {
        public ObjectPooling()
            : this(DefaultPoolSize)
        { }

        public ObjectPooling(int capacity)
            : this(capacity, () => { return new T(); })
        { }

        public ObjectPooling(int capacity, Func<T> generateObjectFunc)
        {
            Initializer(capacity, generateObjectFunc);
        }

        private void Initializer(int capacity, Func<T> generateObjectFunc)
        {
            _capacity = capacity;
            _generateObjectFunc = generateObjectFunc;
            for (int i = 0; i < _capacity; i++)
            {
                this.Enqueue(new Instance<T>(this, _generateObjectFunc));
            }
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
            Instance<T> obj;
            while (this.TryDequeue(out obj))
            {
                obj.Dispose();
            }
        }

        public Instance<T> Get()
        {
            Instance<T> obj;
            if (this.TryDequeue(out obj) == false)
                return new Instance<T>(_generateObjectFunc);

            return obj;
        }
        internal void EnqueueInstance(Instance<T> instance)
        {
            this.Enqueue(instance);
        }

        private bool _disposed = false;
        private int _capacity;
        private Func<T> _generateObjectFunc = null;
        public const int DefaultPoolSize = 10;
    }

    public class Instance<T> : IDisposable
        where T : IDisposable, new()
    {
        internal Instance(Func<T> generateFunc)
            : this(null, generateFunc)
        {

        }

        internal Instance(ObjectPooling<T> master, Func<T> generateFunc)
        {
            _master = master;
            _generateObject = generateFunc;
            PooledObject = _generateObject();
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

        private bool _disposed = false;
        private ObjectPooling<T> _master = null;
        private Func<T> _generateObject = null;
        public T PooledObject { get; private set; }
    }
}
