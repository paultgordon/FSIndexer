using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FSIndexer
{
    public class IndexedFiles : List<IndexedFile>
    {
        public IndexedFiles()
            : base()
        {
        }

        public List<IndexedFile> EnabledList { get { return this.Where(n => n.Enabled).ToList(); } }

        public FileInfo FindByID(string fileID)
        {
            return this.SingleOrDefault(n => n.ID == fileID || n.FileData.ID == fileID)?.File;
        }

        public FileInfo Find(System.Windows.Forms.TreeNode childNode)
        {
            return FindByID(childNode.Tag.ToString());
        }

        public void Sort(SortTypes.SortType sort)
        {
            base.Sort(delegate(IndexedFile a, IndexedFile b)
            {
                if (SortTypes.IsReverseSortType(sort))
                {
                    return b.SortString(sort).CompareTo(a.SortString(sort));
                }
                else
                {
                    return a.SortString(sort).CompareTo(b.SortString(sort));
                }
            }
            );
        }
    }
}
