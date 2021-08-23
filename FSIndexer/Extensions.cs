using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.ComponentModel;

namespace FSIndexer
{
    public static class Extensions
    {
        public static T As<T>(this FSIndexer.DirectoryInfoExtended.SearchOptionExtended c) where T : struct
        {
            return (T)System.Enum.Parse(typeof(T), c.ToString(), false);
        }

        public static void InvokeEx<T>(this T @this, Action<T> action) where T : ISynchronizeInvoke
        {
            if (@this.InvokeRequired)
            {
                @this.Invoke(action, new object[] { @this });
            }
            else
            {
                action(@this);
            }
        }
    }
}
