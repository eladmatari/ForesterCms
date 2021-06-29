using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Common.Utils.Standard
{
    public class BaseProcess
    {
        private Thread p_thread;
        private ManualResetEvent p_shutdownEvent;

        private TimeSpan p_Delay
        {
            get
            {
                if (Interval <= 0)
                    throw new Exception(string.Format("Invalid interval: {0}", Interval));

                return TimeSpan.FromMilliseconds(Interval);
            }
        }

        private readonly object p_IsActiveLockObj = new object();
        private bool p_IsActive;
        public bool IsActive
        {
            get
            {
                lock (p_IsActiveLockObj)
                {
                    return p_IsActive;
                }
            }
            private set
            {
                lock (p_IsActiveLockObj)
                {
                    p_IsActive = value;
                }
            }
        }

        public int Interval { get; protected set; }

        public virtual void Start()
        {
            if (IsActive)
                return;

            IsActive = true;

            // create our threadstart object to wrap our delegate method
            ThreadStart ts = new ThreadStart(this.ProcessMain);

            // create the manual reset event and
            // set it to an initial state of unsignaled
            p_shutdownEvent = new ManualResetEvent(false);

            // create the worker thread
            p_thread = new Thread(ts);

            // go ahead and start the worker thread
            p_thread.Start();
        }

        private void ProcessMain()
        {
            bool bSignaled = false;

            while (true)
            {
                try
                {
                    bSignaled = p_shutdownEvent.WaitOne(p_Delay, true);

                    if (bSignaled == true)
                        break;

                    Execute();
                }
                catch (Exception ex)
                {
                    Error(ex);
                }
            }
        }

        protected virtual void Execute() { }

        protected void Error(Exception ex)
        {
            if (OnError != null)
                OnError(ex);
        }

        public virtual void Stop()
        {
            if (!IsActive)
                return;

            IsActive = false;

            // signal the event to shutdown
            p_shutdownEvent.Set();

            // wait for the thread to stop giving it 10 seconds
            p_thread.Join(10000);
        }

        public event Action<Exception> OnError;
    }
}
