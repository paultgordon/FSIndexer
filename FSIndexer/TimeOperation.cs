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
        public static bool LogToConsole { get; set; } = false;
        public static bool LogToDebug { get; set; } = true;
        public static bool WriteRunDateTime { get; set; } = true;

        private bool ForceSilent { get; set; } = false;

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

        public TimeOperation(string operation = "", bool forceSilent = false)
        {
            Operation = operation;
            ForceSilent = forceSilent;
            Stopwatch = new Stopwatch();
#if DEBUG
            if (!forceSilent)
            {
                Stopwatch.Start();
            }
#endif
        }

        private string GetElapsedTimeString()
        {
            return "[" + ElapsedTime + "]: ";
        }

        private string GetRunDateTimeString()
        {
            if (WriteRunDateTime)
            {
                return "[" + DateTime.Now.ToString("hh:mm:ss.fff") + "] --> ";
            }
            else
            {
                return "";
            }
        }

        protected virtual void Log()
        {
            if (ForceSilent)
            {
                return;
            }

#if DEBUG
            string msg = GetRunDateTimeString() + GetElapsedTimeString() + Operation;

            if (LogToConsole)
            {
                Console.WriteLine(msg);
            }

            if (LogToDebug)
            {
                Debug.WriteLine(msg);
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
