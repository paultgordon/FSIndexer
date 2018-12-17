using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace FSIndexer
{
    public static class Extensions
    {
        public static T As<T>(this FSIndexer.DirectoryInfoExtended.SearchOptionExtended c) where T : struct
        {
            return (T)System.Enum.Parse(typeof(T), c.ToString(), false);
        }
    }
}
