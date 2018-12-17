using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace FSIndexer
{
    public class DirectoryInfoExtended
    {
        public enum SearchOptionExtended { TopDirectoryOnly = System.IO.SearchOption.TopDirectoryOnly, AllDirectories = System.IO.SearchOption.AllDirectories, SubDirectories }

        public DirectoryInfo DirectoryInfo { get; set; }
        public SearchOptionExtended SearchOption { get; set; }
        public bool Index { get; set; }
        public bool RenameFolders { get; set; }
        public bool AutoFile { get; set; }
        public bool CheckForDuplicates { get; set; }
        public bool CleanupSmallFiles { get; set; }

        public DirectoryInfoExtended(string di, SearchOptionExtended so = SearchOptionExtended.AllDirectories, 
            bool index = false, bool renameFolders = false, bool autoFile = false, bool checkForDuplicates = false, bool cleanupSmallFiles = true)
        {
            DirectoryInfo = new DirectoryInfo(di);
            SearchOption = so;
            Index = index;
            RenameFolders = renameFolders;
            AutoFile = autoFile;
            CheckForDuplicates = checkForDuplicates;
            CleanupSmallFiles = cleanupSmallFiles;
        }
        
        public IEnumerable<DirectoryInfo> GetDirectories(bool includeHidden = false)
        {
            if (SearchOption == SearchOptionExtended.SubDirectories)
            {
                var topDirs = DirectoryInfo.GetDirectories("*", System.IO.SearchOption.TopDirectoryOnly);
                var allDirs = DirectoryInfo.GetDirectories("*", System.IO.SearchOption.AllDirectories);

                if (includeHidden)
                {
                    return allDirs.Where(n => !topDirs.Contains(n));
                }
                else
                {
                    return allDirs.Where(n => !topDirs.Contains(n)).Where(n => !n.Attributes.HasFlag(FileAttributes.Hidden));
                }
            }
            else
            {
                if (includeHidden)
                    return DirectoryInfo.GetDirectories("*", SearchOption.As<System.IO.SearchOption>());
                else
                    return DirectoryInfo.GetDirectories("*", SearchOption.As<System.IO.SearchOption>()).Where(n => !n.Attributes.HasFlag(FileAttributes.Hidden));
            }
        }

        public IEnumerable<FileInfo> GetFiles(bool includeHidden = false)
        {
            return GetFileList("*", DirectoryInfo.FullName, SearchOption, includeHidden);
        }

        private static IEnumerable<FileInfo> GetFileList(string fileSearchPattern, string rootFolderPath, SearchOptionExtended so, bool includeHidden = false)
        {
            DateTime writeTimeCutoff = DateTime.UtcNow.AddSeconds(-5);
            string folderPath = string.Empty;
            Queue<string> pending = new Queue<string>();
            pending.Enqueue(rootFolderPath);
            string[] tmp;
            while (pending.Count > 0)
            {
                folderPath = pending.Dequeue();
                if ((folderPath == rootFolderPath && (so == SearchOptionExtended.AllDirectories || so == SearchOptionExtended.TopDirectoryOnly)) || folderPath != rootFolderPath)
                {
                    tmp = Directory.GetFiles(folderPath, fileSearchPattern);
                    for (int i = 0; i < tmp.Length; i++)
                    {
                        var fi = new FileInfo(tmp[i]);

                        if (includeHidden || (!includeHidden && !fi.Attributes.HasFlag(FileAttributes.Hidden)))
                        {
                            if (fi.LastWriteTimeUtc < writeTimeCutoff)
                            {
                                yield return fi;
                            }
                        }
                    }
                }

                if (so == SearchOptionExtended.AllDirectories || so == SearchOptionExtended.SubDirectories)
                {
                    tmp = Directory.GetDirectories(folderPath);
                    for (int i = 0; i < tmp.Length; i++)
                    {
                        pending.Enqueue(tmp[i]);
                    }
                }
            }
        }
    }
}
