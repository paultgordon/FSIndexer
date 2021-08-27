using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace FSIndexer
{
    public class Indexer
    {
        public static IndexExtensions IndexExtensions = new IndexExtensions();
        public static IndexSeperators IndexSeperators = new IndexSeperators();
        public IndexedFiles IndexedFiles = new IndexedFiles();
        public SortTypes.SortType IndexedFilesSortType = SortTypes.SortType.LtoS;
        public IndexedTerms IndexedTerms = new IndexedTerms();
        public SortTypes.SortType IndexedTermsSortType = SortTypes.SortType.LtoS;

        public Indexer()
        {
            IndexedFiles = new IndexedFiles();
            IndexedTerms = new IndexedTerms();
        }

        public void Reset()
        {
            IndexedFiles = new IndexedFiles();
            IndexedTerms = new IndexedTerms();
        }

        public void Index(DirectoryInfoExtended die, SearchOption so = SearchOption.AllDirectories)
        {
            Parallel.ForEach(IndexExtensions.FindExtensionMatches(die), fi =>
            {
                if (fi.Length < TermOptions.ExcludeRules.MinimumSizeToIndexInB)
                    return; 

                var indFile = new IndexedFile(fi);

                lock (IndexedFiles)
                {
                    IndexedFiles.Add(indFile);
                }

                foreach (var indTerm in indFile.IndexedStrings)
                {
                    int loc = 0;

                    lock (IndexedFiles)
                    {
                        loc = IndexedTerms.FindIndex(n => n.Term.Equals(indTerm, StringComparison.CurrentCultureIgnoreCase));
                    }

                    var ri = Main.RenameTrackerList.GetItem(indTerm);

                    if (loc < 0)
                    {
                        var newIndTerm = new IndexedTerm(indTerm);

                        if (ri == null || !ri.RequireTermAtStart || (ri.RequireTermAtStart && fi.Name.StartsWith(indTerm, StringComparison.CurrentCultureIgnoreCase) && indFile.IndexedStrings[0].Equals(indTerm, StringComparison.CurrentCultureIgnoreCase)))
                        {
                            newIndTerm.IndexedFiles.Add(fi);
                        }

                        lock (IndexedTerms)
                        {
                            IndexedTerms.Add(newIndTerm);
                        }
                    }
                    else
                    {
                        if (ri == null || !ri.RequireTermAtStart || (ri.RequireTermAtStart && fi.Name.StartsWith(indTerm, StringComparison.CurrentCultureIgnoreCase) && indFile.IndexedStrings[0].Equals(indTerm, StringComparison.CurrentCultureIgnoreCase)))
                        {
                            lock (IndexedTerms)
                            {
                                if (!IndexedTerms[loc].IndexedFiles.Contains(fi))
                                {
                                    IndexedTerms[loc].IndexedFiles.Add(fi);
                                }
                            }
                        }
                    }
                }
            });

            Sort();
        }

        public void Sort()
        {
            IndexedFiles.Sort(IndexedFilesSortType);
            IndexedTerms.Sort(IndexedTermsSortType);
        }
    }
}
