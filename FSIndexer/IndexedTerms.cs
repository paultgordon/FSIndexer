using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace FSIndexer
{
    public class IndexedTerms : List<IndexedTerm>
    {
        public IndexedTerms()
            : base()
        {
        }

        public List<IndexedTerm> EnabledList { get { return this.Where(n => n.Enabled).ToList(); } }

        public FileInfo Find(string termID, string filename)
        {
            foreach (var item in this)
            {
                if (item.ID == termID)
                {
                    return item.IndexedFiles.Find(n => n.Name.Equals(filename, StringComparison.CurrentCultureIgnoreCase)).File;
                }
            }

            return null;
        }

        public FileInfo FindByID(string termID, string fileID)
        {
            var termMatch = this.FirstOrDefault(n => n.ID == termID);

            if (termMatch != null)
            {
                var fileMatch = termMatch.IndexedFiles.FirstOrDefault(n => n.ID == fileID);

                if (fileMatch != null)
                {
                    return fileMatch.File;
                }
            }

            return null;
       }

        public FileInfo Find(System.Windows.Forms.TreeNode childNode)
        {
            return FindByID(childNode.Parent.Tag.ToString(), childNode.Tag.ToString());
        }

        public FileInfo Find(string termID, string filename, string tooltip)
        {
            foreach (var item in this.Where(n => n.ID == termID))
            {
                if (item.ID == termID)
                {
                    var tooltipDir = GetDirectoryFromTooltip(tooltip);

                    foreach (var found in item.IndexedFiles.FindAll(n => n.Name.Equals(filename, StringComparison.CurrentCultureIgnoreCase)
                        && tooltipDir.Equals(n.DirectoryName, StringComparison.CurrentCultureIgnoreCase)).OrderByDescending(n => n.DirectoryName.Length))
                    {
                        return found;
                    }
                }
            }

            return null;
        }

        public IndexedTerm Find(string id)
        {
            return this.SingleOrDefault(n => n.ID == id);
        }

        public bool Update(string id, IndexedTerm it)
        {
            var oit = Find(id);

            if (oit == null)
            {
                return false;
            }
            else
            {
                oit = it;
                return true;
            }
        }

        public void Sort(SortTypes.SortType sort)
        {
            base.Sort(delegate(IndexedTerm a, IndexedTerm b) 
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

            base.ForEach(n => n.SortIndexedFiles(sort));
        }

        public string GetDirectoryFromTooltip(string tooltip, string filename = "")
        {
            if (!string.IsNullOrEmpty(filename) && tooltip.Contains(filename, StringComparison.CurrentCultureIgnoreCase))
            {
                return Path.GetDirectoryName(tooltip.Substring(0, tooltip.IndexOf(filename, StringComparison.CurrentCultureIgnoreCase) + filename.Length));
            }
            else if (tooltip.Contains("("))
            {
                return Path.GetDirectoryName(tooltip.Substring(0, tooltip.IndexOf("(")).Trim());
            }
            else
            {
                return Path.GetDirectoryName(tooltip);
            }
        }
    }
}
