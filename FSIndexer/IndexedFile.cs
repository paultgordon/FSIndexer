using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace FSIndexer
{
    public class IndexedFile : IndexedBase
    {
        public IndexedFileData FileData { get; set; }
        public List<string> IndexedStrings { get; set; }
        public FileInfo File { get { return FileData.File; } }

        public IndexedFile(FileInfo fi)
        {
            FileData = new IndexedFileData(fi);
            IndexedStrings = TermOptions.ApplyRules(Indexer.IndexSeperators.Seperate(fi));
        }

        public override string ToString()
        {
            return FileData.Name + " - " + IndexedStrings.Count;
        }

        public string SortString(SortTypes.SortType sort)
        {
            if (sort == SortTypes.SortType.AtoZ)
                return FileData.Name + " " + IndexedStrings.Count.ToString().PadLeft(10, '0');
            else if (sort == SortTypes.SortType.ZtoA)
                return FileData.Name + " " + IndexedStrings.Count.ToString().PadLeft(10, '0');
            else if (sort == SortTypes.SortType.LtoS)
                return IndexedStrings.Count.ToString().PadLeft(10, '0') + " " + FileData.Name;
            else if (sort == SortTypes.SortType.StoL)
                return IndexedStrings.Count.ToString().PadLeft(10, '0') + " " + FileData.Name;
            else
                return FileData.Name;
        }
    }
}
