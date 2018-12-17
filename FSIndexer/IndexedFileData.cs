using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace FSIndexer
{
    public class IndexedFileData : IndexedBase
    {
        public FileInfo File { get; set; }

        public string Name { get { return File.Name; } }
        public string FullName { get { return File.FullName; } }
        public string DirectoryName { get { return File.DirectoryName; } }
        public DirectoryInfo Directory { get { return File.Directory; } }

        public IndexedFileData(FileInfo fi) : base()
        {
            File = fi;
        }

        static public implicit operator IndexedFileData(FileInfo fi)
        {
            return new IndexedFileData(fi);
        }

        static public implicit operator FileInfo(IndexedFileData ifd)
        {
            return ifd.File;
        }

        public string SortString(SortTypes.SortType sort)
        {
            if (sort == SortTypes.SortType.AtoZ || sort == SortTypes.SortType.ZtoA)
                return Name;
            else if (sort == SortTypes.SortType.StoL || sort == SortTypes.SortType.LtoS)
                return Name;
            else if (sort == SortTypes.SortType.NtoO || sort == SortTypes.SortType.OtoN)
                return File.LastWriteTimeUtc.ToFileTime().ToString();
            else
                return Name;
        }
    }
}
