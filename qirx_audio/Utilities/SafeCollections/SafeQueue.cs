// After a code published by Brian Rudolph
// http://devplanet.com/blogs/brianr/archive/2008/09/26/thread-safe-dictionary-in-net.aspx</remarks>
using System;
using System.Collections.Generic;
using System.Threading;

namespace softsyst.Generic.SafeCollections
{
    /// <summary>
    /// Thread safe generic queue
    /// </summary>
    /// <remarks>
    /// The object has to obtain an identifier value on construction, to be easily
    /// identified when comparing with other objects.
    public sealed class SafeQueue<T> 
    {
        /// <summary>
        /// The identifier of the queue
        /// </summary>
        public readonly UInt32 identifier;

        /// <summary>
        /// The enumerator for the safe queue
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator() { return queue.GetEnumerator(); }

        /// <summary>
        /// aggregated generic (non-threadsafe) Queue
        /// </summary>
        public Queue<T> queue {get; private set;}

        /// <summary>
        /// Setup of the lock
        /// </summary>
        [NonSerialized]
        internal ReaderWriterLockSlim objLock = Locks.GetLockInstance(LockRecursionPolicy.SupportsRecursion);

        /// <summary>
        /// Private constructor
        /// </summary>
        private SafeQueue() { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="id">Identifier of the queue</param>
        public SafeQueue(uint id)
        {
            queue = new Queue<T>();
            identifier = id;
        }

        /// <summary>
        /// Constructor overload
        /// </summary>
        /// <param name="id">Identifier of the queue</param>
        /// <param name="count">Initial number of elements</param>
        public SafeQueue(uint id, int count)
        {
            queue = new Queue<T>(count);
            identifier = id;
        }


        /// <summary>
        /// Threadsafe wrapper of the Queue class element
        /// </summary>
        public int Count
        {
            get
            {
                using (new ReadOnlyLock(this.objLock))
                {
                    return queue.Count;
                }
            }
        }

        /// <summary>
        /// Threadsafe wrapper of the Queue class element
        /// </summary>
        public void Clear()
        {
            using (new WriteLock(this.objLock))
            {
                queue.Clear(); ;
            }
        }

        /// <summary>
        /// Threadsafe wrapper of the Queue class element
        /// </summary>
        public bool Contains(T item)
        {
            using (new ReadOnlyLock(this.objLock))
            {
                return queue.Contains(item);
            }
        }

        /// <summary>
        /// Threadsafe wrapper of the Queue class element
        /// </summary>
        public bool TryDequeue(out T obj)
        {
            using (new ReadLock(this.objLock))
            {
                if (queue.Count != 0)
                {
                    obj = Dequeue();
                    return true;
                }
            }

            obj = default(T);
            return false;
        }

        /// <summary>
        /// Threadsafe wrapper of the Queue class element
        /// </summary>
        public T Dequeue()
        {
            using (new WriteLock(this.objLock))
            {
                return queue.Dequeue();
            }
        }

        /// <summary>
        /// Threadsafe wrapper of the Queue class element
        /// </summary>
        public void Enqueue(T item)
        {
            using (new WriteLock(this.objLock))
            {
                queue.Enqueue(item);
            }
        }

        /// <summary>
        /// Threadsafe wrapper of the Queue class element
        /// </summary>
        public T Peek()
        {
            using (new ReadOnlyLock(this.objLock))
            {
                return queue.Peek();
            }
        }
    }
}
