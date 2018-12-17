using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace FSIndexer
{
    public class IndexedTerm : IndexedBase
    {
        public string Term { get; private set; }
        public List<IndexedFileData> IndexedFiles { get; set; }
        public string Note
        {
            get
            {
                var match = Main.InfoTrackerList.List.SingleOrDefault(n => n.Term.Equals(Term, StringComparison.CurrentCultureIgnoreCase));

                if (match != null)
                {
                    return match.Note;
                }
                else
                {
                    return string.Empty;
                }
            }
            set
            {
                var match = Main.InfoTrackerList.List.SingleOrDefault(n => n.Term.Equals(Term, StringComparison.CurrentCultureIgnoreCase));

                if (match != null)
                {
                    if (match.Note != value)
                    {
                        match.Note = value;
                        Main.InfoTrackerList.SyncDictionaryToList();
                    }
                }
                else
                {
                    Main.InfoTrackerList.Add(new InfoTrackerItem(Term, note: value));
                    Main.InfoTrackerList.SyncDictionaryToList();
                }
            }
        }
        public int Rating
        {
            get
            {
                var match = Main.InfoTrackerList.List.SingleOrDefault(n => n.Term.Equals(Term, StringComparison.CurrentCultureIgnoreCase));

                if (match != null)
                {
                    return match.Rating;
                }
                else
                {
                    return 0;
                }
            }
            set
            {
                var match = Main.InfoTrackerList.List.SingleOrDefault(n => n.Term.Equals(Term, StringComparison.CurrentCultureIgnoreCase));

                if (match != null)
                {
                    if (match.Rating != value)
                    {
                        match.Rating = value;
                        Main.InfoTrackerList.SyncDictionaryToList();
                    }
                }
                else
                {
                    Main.InfoTrackerList.Add(new InfoTrackerItem(Term, rating: value));
                    Main.InfoTrackerList.SyncDictionaryToList();
                }
            }
        }

        public IndexedTerm(string term)
        {
            Term = term;
            IndexedFiles = new List<IndexedFileData>();
        }

        public override string ToString()
        {
            bool allInSameDir = IndexedFiles.Count > 0 ? IndexedFiles.All(n => n.DirectoryName == IndexedFiles[0].DirectoryName) && !Main.SourceDirectoriesToIndex.Any(m => m.DirectoryInfo.FullName == IndexedFiles[0].DirectoryName) : false;
            bool savedTag = Main.MoveTrackerList.MoveLocationExists(Term);

            string addedString = "";

            if (allInSameDir)
            {
                addedString += " " + Main.FilterShowTypes.GT.ToString();
            }

            if (savedTag)
            {
                addedString += " " + Main.FilterShowTypes.SV.ToString();
            }

            if (!allInSameDir && savedTag)
            {
                addedString += " " + Main.FilterShowTypes.MV.ToString();
            }

            if (!string.IsNullOrEmpty(addedString))
            {
                addedString = " -" + addedString;
            }

            return Term + " - " + IndexedFiles.Where(n => n.Enabled).Distinct().Count() + addedString + (string.IsNullOrEmpty(Note) ? "" : " - " + Note) + (Rating == 0 ? "" : " - R" + Rating.ToString());
        }

        public Main.FilterShowTypes Filters
        {
            get
            {
                bool allInSameDir = IndexedFiles.Count > 0 ? IndexedFiles.All(n => n.DirectoryName == IndexedFiles[0].DirectoryName) && !Main.SourceDirectoriesToIndex.Any(m => m.DirectoryInfo.FullName == IndexedFiles[0].DirectoryName) : false;
                bool savedTag = Main.MoveTrackerList.MoveLocationExists(Term);

                Main.FilterShowTypes filter = Main.FilterShowTypes.NONE;
                filter = filter & ~Main.FilterShowTypes.NONE;

                if (allInSameDir)
                {
                    filter = filter | Main.FilterShowTypes.GT;
                }

                if (savedTag)
                {
                    filter = filter | Main.FilterShowTypes.SV;
                }

                if (!allInSameDir && savedTag)
                {
                    filter = filter | Main.FilterShowTypes.MV;
                }

                return filter;
            }
        }

        public string SortString(SortTypes.SortType sort)
        {
            if (sort == SortTypes.SortType.AtoZ || sort == SortTypes.SortType.ZtoA)
                return Term + " " + IndexedFiles.Distinct().Count().ToString().PadLeft(10, '0');
            else if (sort == SortTypes.SortType.StoL || sort == SortTypes.SortType.LtoS)
                return IndexedFiles.Count.ToString().PadLeft(10, '0') + " " + Term;
            else if (sort == SortTypes.SortType.NtoO || sort == SortTypes.SortType.OtoN)
                return (IndexedFiles.Count > 0 ?
                    IndexedFiles.OrderBy(n => n.File.LastWriteTimeUtc.ToFileTime()).First().File.LastWriteTimeUtc.ToFileTime().ToString() + " " + Term :
                    new DateTime(2000, 1, 1).ToFileTime().ToString() + " " + Term);
            else
                return Term;
        }

        public void SortIndexedFiles(SortTypes.SortType sort)
        {
            if (sort == SortTypes.SortType.NtoO || sort == SortTypes.SortType.OtoN)
            {
                IndexedFiles.Sort(delegate(IndexedFileData a, IndexedFileData b)
                {
                    if (SortTypes.IsReverseSortType(sort))
                    {
                        return b.SortString(sort).CompareTo(a.SortString(sort));
                    }
                    else
                    {
                        return a.SortString(sort).CompareTo(b.SortString(sort));
                    }
                });
            }
            else
            {
                IndexedFiles.Sort(delegate(IndexedFileData a, IndexedFileData b)
                {
                   return a.SortString(sort).CompareTo(b.SortString(sort));
                });
            }

        }
    }
}
