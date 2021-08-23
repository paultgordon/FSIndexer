using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSIndexer
{
    public class TimeOperation : IDisposable
    {
        public bool LogToConsole = false;
        public bool LogToDebug = true;

        public string Operation { get; set; } = "Op";
        private Stopwatch Stopwatch { get; set; } = null;
        private bool disposedValue;

        public string ElapsedTime
        {
            get
            {
                return Stopwatch.Elapsed.ToString(@"hh\:mm\:ss\.fff");
            }
        }

        public TimeOperation(string operation = "")
        {
            Operation = operation;
            Stopwatch = new Stopwatch();
#if DEBUG
            Stopwatch.Start();
#endif
        }

        protected virtual void Log()
        {
#if DEBUG
            if (LogToConsole)
            {
                Console.WriteLine("[" + ElapsedTime + "]: " + Operation);
            }

            if (LogToDebug)
            {
                Debug.WriteLine("[" + ElapsedTime + "]: " + Operation);
            }
#endif
        }

        #region IDisposable
        protected virtual void Dispose(bool disposing)
        {
            Stopwatch.Stop();

            Log();

            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion IDisposable
    }
}
