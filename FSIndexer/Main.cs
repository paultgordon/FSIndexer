using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using Shell32;

namespace FSIndexer
{
    public partial class Main : Form
    {
        private const long KB = 1024;
        private const long MB = 1024 * KB;
        private const long GB = 1024 * MB;
        private const long TB = 1024 * GB;
        private int PrivateViewingHeight { get; set; }
        private int NormalViewingHeight { get; set; }
        private bool PrivateViewing = false;

        public const long HashSizeShort = 100 * KB;
        public const long HashSizeLong = 200 * MB;

        public const string KeepDirectory = @"E:\_P";
        public const string FavoritesDir = @"E:\_P\_Best";
        public const string FavoritesStarDir = @"E:\_P\_Best\_AllTimeBest";
        public const string StreamingPath = @"E:\_P\_Air";
        public const string VLCPath = @"C:\Program Files\VideoLAN\VLC\vlc.exe";
        public const string NzbPath = @"E:\Downloads\NG\_Decoded";
        public const string TorrentPath = @"E:\Downloads\NG\_Decoded\_Torrents";
        public const string NircmdPath = @"C:\Users\Paul\Documents\Visual Studio Projects\FSIndexer\FSIndexer\bin\nircmd.exe";
        public string TheEndPath { get { return Path.Combine(SaveDirectory, "TheEnd_NS.mp4"); } }
        private string SaveDirectory { get { return Directory.GetParent(Path.GetDirectoryName(Path.GetFullPath(Assembly.GetExecutingAssembly().Location))).FullName; } }
        private string MoveTrackingFile { get { return Path.Combine(SaveDirectory, "MoveTrackerList.xml"); } }
        private string RenameTrackingFile { get { return Path.Combine(SaveDirectory, "RenameTrackerList.xml"); } }
        private string HashTrackingFile { get { return Path.Combine(SaveDirectory, "HashTrackerList.xml"); } }
        private string IgnoreTrackingFile { get { return Path.Combine(SaveDirectory, "IgnoreTrackerList.xml"); } }
        private string InfoTrackingFile { get { return Path.Combine(SaveDirectory, "InfoTrackerList.xml"); } }
        private string ShortcutCreator { get { return Path.Combine(SaveDirectory, "Shortcut.exe"); } }
        private string RemoveForeignCharsScriptFile { get { return Path.Combine(SaveDirectory, "RemoveForeignChars.ps1"); } }

        private static bool IsLoading { get; set; } = false;
        private static bool AutomationRunning { get; set; } = false;
        public const bool WaitForCmd = true;

        private readonly TimeSpan LoadTimeStale = TimeSpan.FromSeconds(300);
        private DateTime LastLoadTime { get; set; }
        private int LastLoadCount { get; set; }

        private Indexer IndexedList = new Indexer();
        internal static List<DirectoryInfoExtended> SourceDirectories = new List<DirectoryInfoExtended>();
        internal static List<DirectoryInfoExtended> SourceDirectoriesToIndex { get { return SourceDirectories.Where(n => n.Index).ToList(); } }
        internal static List<DirectoryInfoExtended> SourceDirectoriesToAutoFile { get { return SourceDirectories.Where(n => n.AutoFile).ToList(); } }
        internal static List<DirectoryInfoExtended> SourceDirectoriesToCheckForDuplicates { get { return SourceDirectories.Where(n => n.CheckForDuplicates).ToList(); } }
        //internal static List<DirectoryInfoExtended> SourceDirectoriesToAutoName = new List<DirectoryInfoExtended>();
        internal static MoveTrackerList MoveTrackerList;
        internal static RenameTrackerList RenameTrackerList;
        internal static HashTrackerList HashTrackerList;
        internal static TrackerList<IgnoreTrackerItem> IgnoreTrackerList;
        internal static TrackerList<InfoTrackerItem> InfoTrackerList;
        internal static List<string> FilesChangesSeen = new List<string>();

        internal static ConsoleInputReader.Reader Reader = null;

        private static object filesMovedLocker = new object();

        private FileSystemWatcher FileWatcher { get; set; } = null;

        internal class TAGS
        {
            public static string TagAutoConvert = "-ac";
        }

        public Main(string[] args = null)
        {
            IsLoading = true;
            Reader = new ConsoleInputReader.Reader(args);
            FilterOnName = new List<string>();
            InitializeComponent();
            CleanUpTempPath();

            PrivateViewingHeight = rtbExecuteWindow.Height + 95;
            cbFilterOnTypes.Items.Clear();

            foreach (var fil in MenuFilterShowTypes)
            {
                cbFilterOnTypes.Items.Add(fil.ToString());
            }

            cbFilterOnTypes.SelectedItem = FilterShowTypes.ALL.ToString();

            foreach (var sort in MenuFilterSortTypes)
            {
                cbSort.Items.Add(sort.ToString());
            }

            cbSort.SelectedItem = SortTypes.SortType.LtoS.ToString();
            IndexedList.IndexedTermsSortType = SortTypes.SortType.LtoS;

            numFilterOnChildrenSize.Value = TermOptions.ExcludeRules.IgnoreTermsWithChildrenLessThan;
            numFilterOnTermLength.Value = TermOptions.ExcludeRules.IgnoreTermsWithLengthLessThan;

            LastLoadCount = -1;
            LastLoadTime = DateTime.UtcNow;

            SourceDirectories.Add(new DirectoryInfoExtended(@"E:\Downloads\NG\_Decoded", DirectoryInfoExtended.SearchOptionExtended.AllDirectories,
                index: true, autoFile: true, checkForDuplicates: true, cleanupSmallFiles: true));

            SourceDirectories.Add(new DirectoryInfoExtended(@"E:\_P", DirectoryInfoExtended.SearchOptionExtended.AllDirectories,
                index: false, autoFile: false, checkForDuplicates: true, cleanupSmallFiles: false));

            SourceDirectories.Add(new DirectoryInfoExtended(@"E:\_P\_Air", DirectoryInfoExtended.SearchOptionExtended.AllDirectories,
                index: false, autoFile: true, checkForDuplicates: false, cleanupSmallFiles: false));

            SourceDirectories.Add(new DirectoryInfoExtended(@"E:\Downloads\DL", DirectoryInfoExtended.SearchOptionExtended.SubDirectories,
                index: false, autoFile: true, checkForDuplicates: true, cleanupSmallFiles: false));

            //SourceDirectories.Add(new DirectoryInfoExtended(@"E:\Downloads\NG\_Decoded\_Sort", DirectoryInfoExtended.SearchOptionExtended.AllDirectories,
            //    index: false, autoFile: true, checkForDuplicates: true, cleanupSmallFiles: false));

            SourceDirectories.RemoveAll(n => !Directory.Exists(n.DirectoryInfo.FullName));

            GetFileCount(SourceDirectoriesToAutoFile);

            MoveTrackerList = MoveTrackerList.LoadFromFile(MoveTrackingFile);
            RenameTrackerList = RenameTrackerList.LoadFromFile(RenameTrackingFile);
            HashTrackerList = HashTrackerList.LoadFromFile(HashTrackingFile);
            IgnoreTrackerList = TrackerList<IgnoreTrackerItem>.LoadFromFile(IgnoreTrackingFile, false);
            InfoTrackerList = TrackerList<InfoTrackerItem>.LoadFromFile(InfoTrackingFile, false);

            if (!AreArgsEmpty(Reader))
            {
                PrivateViewing = true;
            }
        }

        private void Main_Load(object sender, EventArgs e)
        {
            //PrintInfo("ReadMediaInfo Start");
            //var dtStart = DateTime.Now;
            //var mi = ReadMediaInfo(SourceDirectoriesToIndex.First());
            //var tsDur = DateTime.Now.Subtract(dtStart).TotalSeconds;
            //PrintInfo("It took " + (int)tsDur + "s to load the media for " + mi.Count + " items.");
            StartFileWatcher();
        }

        private void Main_Shown(object sender, EventArgs e)
        {
            ReloadList(refreshFileSystem: true);
            IsLoading = false;

            NormalViewingHeight = this.Height;

            this.ActiveControl = TvTerms;

            ProcessArgs(Reader);

            if (this.Visible)
            {
                this.ActiveControl = mtvTerms;
            }
        }

        private int GetFileCount(List<DirectoryInfoExtended> dieList)
        {
            using (new TimeOperation("GetFileCount Operation"))
            {
                Func<int> a = () =>
                {
                    int count = 0;

                    foreach (var die in dieList)
                    {
                        count += die.GetFiles().Where(n => !TermOptions.IgnoreExtensions.Contains(n.Extension.ToLower())).Count();
                    }

                    return count;
                };

                var r = a.BeginInvoke(null, null);

                if (r.AsyncWaitHandle.WaitOne(10000))
                    return a.EndInvoke(r);
                else
                    return 0;
            }
        }

        private bool AreArgsEmpty(ConsoleInputReader.Reader reader)
        {
            return reader == null || reader.Arguments.Count == 0;
        }

        private void ProcessArgs(ConsoleInputReader.Reader reader)
        {
            if (!AreArgsEmpty(Reader))
            {
                if (Debugger.IsAttached)
                {
                    if (MessageBox.Show("Run program using args?", "", MessageBoxButtons.OKCancel) != System.Windows.Forms.DialogResult.OK)
                    {
                        return;
                    }
                }

                if (reader.ContainsKey(TAGS.TagAutoConvert))
                {
                    this.Visible = false;
                    toolStripMenuItemConvert_Click(null, null);
                    this.Close();
                }
            }
        }

        public static List<string> ImageFileExtensions = new List<string>()
            {
                ".jpg", ".jpeg", ".gif"
            };

        // GT = Grouped Together, SV = Previously Saved, MV = Needs To Be Moved, NONE = NOT SV
        [Flags]
        public enum FilterShowTypes { ALL = 0, GT = 1, SV = 2, MV = 4, NONE = 8, NA = 16 }

        private class ConvertOptions
        {
            public static string ConvertToFileExtensionForHandbrake { get { return ConvertToFileExtension.Replace(".", ""); } }
            public static readonly string ConvertToFileExtension = ".mp4";
            public static readonly List<string> ConvertFileExtensions = new List<string>()
            {
                ".wmv", ".avi", ".flv", ".ts", ".mpg", ".mpeg", ".m4v", ".webm", ".vob"
            };
        }

        private static string TempPath
        {
            get
            {
                string tempPath = Path.Combine(Path.GetTempPath(), "FSIndexer");

                if (!Directory.Exists(tempPath))
                {
                    Directory.CreateDirectory(tempPath);
                }

                return tempPath;
            }
        }

        private static void CleanUpTempPath()
        {
            string tempPath = TempPath;

            if (Directory.Exists(tempPath))
            {
                try
                {
                    foreach (FileInfo fi in new DirectoryInfo(tempPath).GetFiles())
                    {
                        if (fi.LastAccessTime < DateTime.Now.Subtract(new TimeSpan(2, 0, 0)))
                        {
                            File.Delete(fi.FullName);
                        }
                    }
                }
                catch
                {
                }
            }
        }

        public MultiSelectTreeview TvTerms
        {
            get
            {
                return this.mtvTerms;
            }
        }

        private TreeNode SelectedNode
        {
            get
            {
                return TvTerms.SelectedNode;
            }
            set
            {
                TvTerms.SelectedNode = value;
            }
        }

        private List<TreeNode> SelectedNodes
        {
            get
            {
                return TvTerms.SelectedNodes;
            }
        }

        private List<string> FilterOnName { get; set; }

        private TreeNode _topNode
        {
            get
            {
                TreeNode node = new TreeNode("Top");
                node.Tag = "Top";
                node.ToolTipText = "Top";
                return node;
            }
        }

        public void UpdateTopNodeCount()
        {
            int count = 0;

            foreach (TreeNode parentNode in TopNode.Nodes)
            {
                count += parentNode.Nodes.Count;
            }

            TopNode.Text = "Top - " + count;
        }

        public TreeNode TopNode
        {
            get
            {
                if (this.mtvTerms.Nodes.Count == 0)
                {
                    this.mtvTerms.Nodes.Add(_topNode);
                }

                this.mtvTerms.Nodes[0].Expand();
                return this.mtvTerms.Nodes[0];
            }
        }

        public List<FilterShowTypes> MenuFilterShowTypes
        {
            get
            {
                return Enum.GetValues(typeof(FilterShowTypes)).Cast<FilterShowTypes>().ToList();
                // return new List<FilterShowTypes>() { FilterShowTypes.ALL, FilterShowTypes.SV, FilterShowTypes.MV, FilterShowTypes.NONE };
            }
        }

        public FilterShowTypes GetFilterShowType
        {
            get
            {
                return (FilterShowTypes)Enum.Parse(typeof(FilterShowTypes), cbFilterOnTypes.SelectedItem.ToString());
            }
        }

        public List<SortTypes.SortType> MenuFilterSortTypes
        {
            get
            {
                var list = new List<SortTypes.SortType>();

                foreach (var sort in Enum.GetNames(typeof(SortTypes.SortType)))
                {
                    list.Add((SortTypes.SortType)Enum.Parse(typeof(SortTypes.SortType), sort));
                }

                return list;
            }
        }

        public SortTypes.SortType GetFilterSortType
        {
            get
            {
                return (SortTypes.SortType)Enum.Parse(typeof(SortTypes.SortType), cbSort.SelectedItem.ToString());
            }
        }

        private Dictionary<string, Dictionary<string, string>> ReadMediaInfo(DirectoryInfoExtended die)
        {
            var mediaInfo = new Dictionary<string, Dictionary<string, string>>();

            Parallel.ForEach(die.GetFiles(), /* new ParallelOptions() { MaxDegreeOfParallelism = 16 }, */ fi =>
                {
                    var mi = ReadMediaInfo(fi.FullName);
                    lock (mediaInfo)
                    {
                        mediaInfo.Add(fi.FullName, mi);
                    }
                });

            return mediaInfo;
        }

        public Shell32.Folder GetShell32NameSpaceFolder(Object folder)
        {
            Type shellAppType = Type.GetTypeFromProgID("Shell.Application");
            Object shell = Activator.CreateInstance(shellAppType);
            return (Shell32.Folder)shellAppType.InvokeMember("NameSpace", System.Reflection.BindingFlags.InvokeMethod, null, shell, new object[] { folder });
        }

        private Dictionary<string, string> ReadMediaInfo(string mediaFile)
        {
            List<string> propertiesOfInterest = new List<string>()
            {
                //"Bit rate",
                "Frame width" //,
                //"Frame height"
            };

            Dictionary<string, string> arrHeaders = new Dictionary<string, string>();

            if (!File.Exists(mediaFile))
                return arrHeaders;

            Shell32.Folder rFolder = GetShell32NameSpaceFolder(Path.GetDirectoryName(mediaFile));
            // Shell32.FolderItem rFile = rFolder.ParseName(mediaFile);
            Shell32.FolderItem rFileMatch = null;

            foreach (Shell32.FolderItem item in rFolder.Items())
            {
                if (item.Path == mediaFile)
                {
                    rFileMatch = item;
                    break;
                }
            }

            int emptyTags = 0;

            for (int i = 0; i < short.MaxValue; i++)
            {
                string name = rFolder.GetDetailsOf(null, i).Trim();

                if (!propertiesOfInterest.Contains(name))
                {
                    continue;
                }

                string value = rFolder.GetDetailsOf(rFileMatch, i).Trim();

                if (!string.IsNullOrEmpty(name))
                {
                    if (!string.IsNullOrEmpty(value))
                    {
                        arrHeaders.Add(name, value);
                    }
                }
                else
                {
                    emptyTags++;

                    if (emptyTags > 4)
                        break;
                }

                if (arrHeaders.Count >= propertiesOfInterest.Count)
                {
                    break;
                }
            }

            return arrHeaders;
        }

        internal static void PrintInfo(string msg = "")
        {
#if DEBUG
            StackFrame sf = new StackFrame(1, true);
            Debug.WriteLine(DateTime.Now.ToString("hh:mm:ss.ff") + " -> " + sf.GetFileLineNumber() + " : " + sf.GetMethod().Name.ToString() + (string.IsNullOrEmpty(msg) ? "" : " : " + msg));
#endif
        }

        private bool NeedsRefresh(List<DirectoryInfoExtended> dieList)
        {
            bool doRefresh = false;
            int count = -1;

            if (LastLoadTime > DateTime.UtcNow.Add(LoadTimeStale))
            {
                doRefresh = true;
            }
            else if (LastLoadCount <= 0)
            {
                doRefresh = true;
            }
            else
            {
                count = GetFileCount(dieList);

                if (count == LastLoadCount)
                {
                    doRefresh = false;
                }
                else
                {
                    LastLoadCount = count;
                    doRefresh = true;
                }
            }

            if (doRefresh)
            {
                if (count <= 0)
                {
                    LastLoadCount = GetFileCount(dieList);
                }

                LastLoadTime = DateTime.UtcNow;
            }

            return doRefresh;
        }

        private void ReloadList(bool refreshFileSystem = false, bool findSelectedItem = true)
        {
            bool currentlyEnabled = this.Enabled;

            using (new TimeOperation("ReloadList Operation"))
            {
                try
                {
                    if (this.PrivateViewing && this.Height != this.PrivateViewingHeight && !AutomationRunning)
                    {
                        this.Height = this.PrivateViewingHeight;
                    }

                    this.Enabled = false;

                    TreeNode parentNode = SingleSelected ? (IsParent ? SelectedNode : (IsChild ? SelectedNode.Parent : null)) : null;
                    IndexedTerm it = parentNode != null ? IndexedList.IndexedTerms.Find(parentNode.Name) : null;
                    bool expandNode = parentNode != null ? parentNode.IsExpanded : true;

                    bool reallyRefresh = refreshFileSystem || NeedsRefresh(SourceDirectoriesToIndex) || AutomationRunning;

                    if (reallyRefresh)
                    {
                        LastLoadCount = GetFileCount(SourceDirectoriesToIndex);
                        LastLoadTime = DateTime.UtcNow;

                        using (new TimeOperation("RefreshFileSystem Operation"))
                        {
                            IndexedList.Reset();

                            using (new TimeOperation("IndexedList Operation"))
                            {
                                foreach (var die in SourceDirectoriesToIndex)
                                {
                                    IndexedList.Index(die);
                                }
                            }

                            IndexedList.IndexedTerms.AsParallel().Where(n => n.Enabled && TermOptions.AutoSkipTags.Any(m => m.Equals(n.Term, StringComparison.CurrentCultureIgnoreCase))).ToList().AsParallel().ForAll(d => d.PermanentlyDisabled = true);
                            IndexedList.IndexedTerms.AsParallel().Where(n => n.Enabled && IgnoreTrackerList.Contains(n.Term)).ToList().AsParallel().ForAll(d => d.PermanentlyDisabled = true);
                            IndexedList.IndexedTerms.AsParallel().Where(n => n.Enabled && n.IndexedFiles.Count == 0).ToList().AsParallel().ForAll(d => d.PermanentlyDisabled = true);
                        }
                    }

                    IndexedList.Sort();

                    if (!AutomationRunning)
                    {
                        using (new TimeOperation("ApplyFilter Operation"))
                        {
                            ApplyFilter();
                        }

                        UpdateTopNodeCount();

                        if (findSelectedItem)
                        {
                            if (it != null)
                            {
                                foreach (TreeNode node in TopNode.Nodes)
                                {
                                    if (node.Text.StartsWith(it.Term + " - "))
                                    {
                                        TvTerms.HideSelection = true;
                                        SelectedNode = node;

                                        if (expandNode)
                                        {
                                            SelectedNode.Expand();
                                        }

                                        break;
                                    }
                                }
                            }
                        }

                        if (SelectedNode == null)
                        {
                            SelectedNode = TopNode;
                        }

                        SelectedNode.Expand();
                        SelectedNode.EnsureVisible();
                    }
                }
                finally
                {
                    this.Enabled = currentlyEnabled;
                    this.ActiveControl = mtvTerms;
                }
            }
        }

        private void ApplyFilter()
        {
            if (AutomationRunning)
                return;

            using (new TimeOperation("ApplyFilter P1 Operation"))
            {
                IndexedList.IndexedTerms.AsParallel().ForAll(n => n.TemporarilyDisabled = false);
                IndexedList.IndexedTerms.AsParallel().ForAll(n => n.IndexedFiles.ForEach(m => m.TemporarilyDisabled = false));
            }

            using (new TimeOperation("ApplyFilter P2 Operation"))
            {
                if (FilterOnName.Count > 0)
                {
                    IndexedList.IndexedTerms.AsParallel().ForAll(n => n.PermanentlyDisabled = false);
                    IndexedList.IndexedTerms.EnabledList.AsParallel().Where(n => !FilterOnName.Any(f => n.Term.Contains(f, StringComparison.CurrentCultureIgnoreCase))).ToList().AsParallel().ForAll(n => n.TemporarilyDisabled = true);
                }
            }

            using (new TimeOperation("ApplyFilter P3 Operation"))
            {
                if (TermOptions.ExcludeRules.IgnoreTermsWithAgeGreaterThan > 0)
                {
                    foreach (var iterm in IndexedList.IndexedTerms.EnabledList)
                    {
                        iterm.IndexedFiles.AsParallel().Where(n => n.Enabled).Where(n => n.File.CreationTime < DateTime.Now.AddDays(-1 * TermOptions.ExcludeRules.IgnoreTermsWithAgeGreaterThan)).ToList().AsParallel().ForAll(n => n.TemporarilyDisabled = true);
                    }
                }
            }

            using (new TimeOperation("ApplyFilter P4 Operation"))
            {
                if (TermOptions.ExcludeRules.MinimumSizeToIndexInMB > 0)
                {
                    foreach (var iterm in IndexedList.IndexedTerms.EnabledList)
                    {
                        iterm.IndexedFiles.AsParallel().Where(n => n.Enabled).Where(n => n.File.Length < TermOptions.ExcludeRules.MinimumSizeToIndexInMB * MB).ToList().AsParallel().ForAll(n => n.TemporarilyDisabled = true);
                    }
                }
            }

            FilterShowTypes filter = GetFilterShowType;

            if (filter != FilterShowTypes.ALL)
            {
                foreach (var iterm in IndexedList.IndexedTerms.EnabledList)
                {
                    var itemFilter = iterm.Filters;

                    if (filter.HasFlag(FilterShowTypes.NONE) && (itemFilter.HasFlag(FilterShowTypes.MV) || itemFilter.HasFlag(FilterShowTypes.SV)))
                    {
                        iterm.TemporarilyDisabled = true;
                    }
                    if (filter.HasFlag(FilterShowTypes.NA) && (itemFilter.HasFlag(FilterShowTypes.MV) || itemFilter.HasFlag(FilterShowTypes.SV)))
                    {
                        iterm.TemporarilyDisabled = true;
                    }
                    else if (filter.HasFlag(FilterShowTypes.MV) && !itemFilter.HasFlag(FilterShowTypes.MV))
                    {
                        iterm.TemporarilyDisabled = true;
                    }
                    else if (filter.HasFlag(FilterShowTypes.SV) && !itemFilter.HasFlag(FilterShowTypes.SV))
                    {
                        iterm.TemporarilyDisabled = true;
                    }
                }
            }

            using (new TimeOperation("ApplyFilter P5 Operation"))
            {
                TopNode.Nodes.Clear();

                if (filter != FilterShowTypes.NA)
                {
                    IndexedList.IndexedTerms.EnabledList.AsParallel().Where(n => n.IndexedFiles.Count == 0 || !n.IndexedFiles.Any(f => f.Enabled) || n.IndexedFiles.Count < TermOptions.ExcludeRules.IgnoreTermsWithChildrenLessThan).ToList().AsParallel().ForAll(n => n.TemporarilyDisabled = true);

                    foreach (var iterm in IndexedList.IndexedTerms.EnabledList)
                    {
                        var newNode = CreateTreeNode(iterm);
                        var node = TopNode.Nodes[TopNode.Nodes.Add(newNode)];

                        foreach (var subitem in iterm.IndexedFiles.Where(n => n.Enabled))
                        {
                            var newSubNode = CreateTreeNode(subitem);

                            if (!node.Nodes.Cast<TreeNode>().Any(n => n.Text == newSubNode.Text))
                            {
                                node.Nodes.Add(newSubNode);
                            }
                        }
                    }
                }
                else
                {
                    var savedTermsDictionary = new Dictionary<string, bool>();
                    Main.InfoTrackerList.List.Select(n => n.Term.ToLower()).Distinct().ToList().ForEach(n => savedTermsDictionary[n] = true);
                    Main.MoveTrackerList.List.Select(n => n.Tag.ToLower()).Distinct().ToList().ForEach(n => savedTermsDictionary[n] = true);

                    var noSavedTermsInFile = IndexedList.IndexedFiles.Where(n => !n.IndexedStrings.Any(i => savedTermsDictionary.ContainsKey(i))).OrderBy(n => n.File.Name).ToList();
                    var newNode = CreateTreeNode(new IndexedTerm("Unsorted") { IndexedFiles = noSavedTermsInFile.Select(n => n.FileData).ToList() });
                    var node = TopNode.Nodes[TopNode.Nodes.Add(newNode)];

                    foreach (var file in noSavedTermsInFile)
                    {
                        node.Nodes.Add(CreateTreeNode(file.FileData));
                    }

                    TopNode.ExpandAll();
                    TopNode.Nodes[0].ExpandAll();
                    node.Expand();
                }
            }
        }

        private void RemoveItem(TreeNode item)
        {
            TvTerms.HideSelection = true;

            TreeNode next = null;

            if (item == TopNode)
            {
                foreach (TreeNode parentNode in TopNode.Nodes)
                {
                    foreach (TreeNode node in parentNode.Nodes)
                    {
                        IndexedList.IndexedTerms.ForEach(n => n.IndexedFiles.RemoveAll(m => m.Name == node.Text));
                        IndexedList.IndexedFiles.RemoveAll(n => n.File.Name == node.Text);
                    }
                }

                TopNode.Nodes.Clear();
                next = TopNode;
            }
            else if (IsParentNode(item))
            {
                foreach (TreeNode node in item.Nodes)
                {
                    IndexedList.IndexedTerms.ForEach(n => n.IndexedFiles.RemoveAll(m => m.Name == node.Text));
                    IndexedList.IndexedFiles.RemoveAll(n => n.File.Name == node.Text);
                }

                next = item.NextNode != null ? item.NextNode : item.PrevNode;
                TopNode.Nodes.Remove(item);
            }
            else if (item != null)
            {
                IndexedList.IndexedTerms.ForEach(n => n.IndexedFiles.RemoveAll(m => m.Name == item.Text));
                IndexedList.IndexedFiles.RemoveAll(n => n.File.Name == item.Text);

                next = item.NextNode != null ? item.NextNode : item.PrevNode;
                TreeNode parent = item.Parent;
                parent.Nodes.Remove(item);
            }
            else
            {
                return;
            }

            if (next != null)
                SelectedNode = next;

            UpdateTopNodeCount();
        }

        private TreeNode CreateTreeNode(IndexedTerm iterm)
        {
            TreeNode newNode = new TreeNode();
            newNode.Text = iterm.ToString();
            newNode.Name = iterm.ID.ToString();
            newNode.Tag = iterm.ID;
            return newNode;
        }

        private TreeNode CreateTreeNode(IndexedFileData iterm)
        {
            TreeNode newNode = new TreeNode();
            newNode.Text = iterm.Name;
            newNode.Tag = iterm.ID;
            newNode.ToolTipText = iterm.FullName + " (" + FriendlyFileSize(iterm) + " / " + FriendlyDateTime(iterm) + ")";
            return newNode;
        }

        private string GetTerm(TreeNode node)
        {
            if (node == TopNode)
                return node.Tag.ToString();
            else
                return (IsParentNode(node) ? IndexedList.IndexedTerms.Find(node.Name).Term : IndexedList.IndexedTerms.Find(node.Parent.Name).Term);
        }

        private string FriendlyFileSize(FileInfo fi)
        {
            return String.Format(new FileSizeFormatProvider(), "{0:fs}", fi.Length);
        }

        private string FriendlyDateTime(FileInfo fi)
        {
            return fi.LastWriteTime.ToShortDateString().Replace("/", "-") + " " + fi.LastWriteTime.ToShortTimeString();
        }

        private List<FileInfo> _MoveSelected(TreeNode node, DirectoryInfo forcePath)
        {
            List<FileInfo> movedList = new List<FileInfo>();

            if (IsParent || IsMultipleParent || IsMultipleChild)
            {
                return movedList;
            }
            else
            {
                FileInfo fi = IndexedList.IndexedTerms.Find(node.Parent.Name, node.Text);
                fi.IsReadOnly = false;

                if (fi != null && fi.Directory.FullName != forcePath.FullName)
                {
                    string newPath = Path.Combine(forcePath.FullName, fi.Name);

                    if (File.Exists(newPath))
                    {
                        if (MessageBox.Show("Really overwrite file: \n" + newPath, "Overwrite File?", MessageBoxButtons.OKCancel) != System.Windows.Forms.DialogResult.OK)
                        {
                            return movedList;
                        }
                    }

                    string logText = "";
                    logText += "MOVE /-Y \"" + fi.FullName + "\" \"" + forcePath.FullName + "\"" + Environment.NewLine;
                    logText += GetRemoveDirectoryCmd(fi.Directory.FullName);
                    rtbExecuteWindow.Text += logText;

                    movedList.Add(new FileInfo(newPath));

                    RemoveItem(node);
                }
            }

            return movedList;
        }

        private List<FileInfo> _MoveSelected(TreeNode node, bool isParent, bool doNotRememberLocation = false, bool skipKeepDirectoryItems = false)
        {
            List<FileInfo> movedList = new List<FileInfo>();

            if (node == null)
                return movedList;

            MoveTrackerItem trackerItem = null;

            if (isParent)
            {
                trackerItem = MoveTrackerList.GetRecommendedMoveLocation(GetTerm(node), IndexedList.IndexedTerms.Find(node.Name).IndexedFiles.Where(n => n.Enabled).Select(n => n.DirectoryName).ToList());

                if (trackerItem != null && skipKeepDirectoryItems && trackerItem.DestinationDir.StartsWith(KeepDirectory, StringComparison.CurrentCultureIgnoreCase))
                {
                    return new List<FileInfo>();
                }
            }
            else
            {
                trackerItem = MoveTrackerList.GetRecommendedMoveLocation(GetTerm(node), IndexedList.IndexedTerms.Find(node.Parent.Name, node.Text).DirectoryName);
            }

            string suggestedPath;

            if (trackerItem == null)
            {
                suggestedPath = Path.Combine(SourceDirectoriesToIndex.First().DirectoryInfo.FullName, GetTerm(node).ToLower());
            }
            else
            {
                suggestedPath = trackerItem.DestinationDir;
            }

            CustomFolderBrowserDialog brow = new CustomFolderBrowserDialog(suggestedPath);
            brow.Owner = this;

            string logText = "";
            int itemsToMove = 0;

            if (brow.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (!Directory.Exists(brow.SelectedPath))
                {
                    Directory.CreateDirectory(brow.SelectedPath);
                }

                if (brow.SelectedPath == Environment.GetFolderPath(Environment.SpecialFolder.Desktop))
                {
                    brow.SelectedPath = suggestedPath;
                }

                var newFolder = new DirectoryInfo(brow.SelectedPath);
                var selectedNode = node;

                if (!newFolder.Exists)
                {
                    newFolder.Create();
                }

                // Top node
                if (isParent)
                {
                    List<string> diMovedFrom = new List<string>();

                    int totalToMove = IndexedList.IndexedTerms.Find(selectedNode.Name).IndexedFiles.Where(n => n.Directory.FullName != newFolder.FullName).Count();

                    foreach (TreeNode subnode in selectedNode.Nodes)
                    {
                        FileInfo fi = IndexedList.IndexedTerms.Find(subnode.Parent.Name, subnode.Text, subnode.ToolTipText);

                        if (fi != null && File.Exists(fi.FullName))
                        {
                            fi.IsReadOnly = false;

                            if (!fi.Directory.FullName.Equals(newFolder.FullName, StringComparison.CurrentCultureIgnoreCase))
                            {
                                if (File.Exists(Path.Combine(newFolder.FullName, fi.Name)))
                                {
                                    // New is larger than existing
                                    if (new FileInfo(fi.FullName).Length >= new FileInfo(Path.Combine(newFolder.FullName, fi.Name)).Length)
                                    {
                                        rtbExecuteWindow.Text += "REM Duplicate Files, Deleting Smaller File" + Environment.NewLine;
                                        rtbExecuteWindow.Text += "DEL \"" + Path.Combine(newFolder.FullName, fi.Name) + "\"" + Environment.NewLine;
                                    }
                                    else
                                    {
                                        rtbExecuteWindow.Text += "REM Duplicate Files, Deleting Smaller File" + Environment.NewLine;
                                        rtbExecuteWindow.Text += "DEL \"" + fi.FullName + "\"" + Environment.NewLine;
                                    }

                                    totalToMove--;
                                    continue;
                                }

                                itemsToMove++;
                                logText += "ECHO Moving " + itemsToMove.ToString() + " Of " + totalToMove.ToString() + " >NUL" + Environment.NewLine;
                                logText += GetMoveCmd(fi.FullName, newFolder) + Environment.NewLine;
                                movedList.Add(new FileInfo(Path.Combine(newFolder.FullName, fi.Name)));

                                if (!doNotRememberLocation)
                                    MoveTrackerList.Add(GetTerm(node), fi.DirectoryName, newFolder.FullName);

                                if (!diMovedFrom.Contains(fi.DirectoryName, StringComparison.CurrentCultureIgnoreCase))
                                    diMovedFrom.Add(fi.DirectoryName);
                            }
                            else if (trackerItem == null)
                            {
                                if (!doNotRememberLocation)
                                    MoveTrackerList.Add(GetTerm(node), fi.DirectoryName, newFolder.FullName);
                            }
                        }
                    }

                    foreach (var di in diMovedFrom)
                    {
                        logText += GetRemoveDirectoryCmd(di);
                    }

                    if (itemsToMove > 0)
                    {
                        RemoveItem(selectedNode);
                    }
                }
                else if (IsChild)
                {
                    FileInfo fi = IndexedList.IndexedTerms.Find(selectedNode.Parent.Name, selectedNode.Text);
                    fi.IsReadOnly = false;

                    if (fi != null && fi.Directory.FullName != newFolder.FullName)
                    {
                        bool breakEarly = false;

                        if (File.Exists(Path.Combine(newFolder.FullName, fi.Name)))
                        {
                            if (MessageBox.Show("Really overwrite file: \n" + Path.Combine(newFolder.FullName, fi.Name), "Overwrite File?", MessageBoxButtons.OKCancel) != System.Windows.Forms.DialogResult.OK)
                            {
                                breakEarly = true;
                            }
                        }

                        if (!breakEarly)
                        {
                            itemsToMove++;
                            logText += "MOVE /-Y \"" + fi.FullName + "\" \"" + newFolder + "\"" + Environment.NewLine;
                            logText += GetRemoveDirectoryCmd(fi.Directory.FullName);
                            movedList.Add(new FileInfo(Path.Combine(newFolder.FullName, fi.Name)));

                            if (!doNotRememberLocation)
                                MoveTrackerList.Add(GetTerm(node), fi.DirectoryName, newFolder.FullName);

                            RemoveItem(node);
                        }
                    }
                }
            }

            if (itemsToMove > 0)
            {
                rtbExecuteWindow.Text += logText;
                //WaitForCmd = false;
            }

            return movedList;
        }

        private void btnMoveSelected_Click(object sender, EventArgs e)
        {
            contextMenuStrip.Close();
            MoveSelected();
        }

        private void toolStripMenuItemMoveToStreaming_Click(object sender, EventArgs e)
        {
            contextMenuStrip.Close();
            _MoveSelected(SelectedNode, new DirectoryInfo(StreamingPath));
        }

        private void btnMoveSelectedDoNotRemember_Click(object sender, EventArgs e)
        {
            contextMenuStrip.Close();
            MoveSelected(true);
        }

        private List<FileInfo> MoveSelected(bool doNotRememberLocation = false)
        {
            List<FileInfo> fiList = new List<FileInfo>();

            if (IsTopNode)
            {
                List<TreeNode> topNodes = new List<TreeNode>();

                foreach (TreeNode node in TopNode.Nodes)
                {
                    topNodes.Add(node);
                }

                foreach (TreeNode node in topNodes)
                {
                    fiList.AddRange(_MoveSelected(node, true, doNotRememberLocation, true));
                }
            }
            else if (IsParent)
            {
                fiList = _MoveSelected(SelectedNode, true, doNotRememberLocation);
            }
            else if (IsChild)
            {
                fiList = _MoveSelected(SelectedNode, false, doNotRememberLocation);
            }
            else if (IsMultipleParent)
            {
                List<TreeNode> selectedNodesList = new List<TreeNode>();

                selectedNodesList.AddRange(SelectedNodes);

                foreach (TreeNode node in selectedNodesList)
                {
                    fiList.AddRange(_MoveSelected(node, true, doNotRememberLocation));
                }
            }

            return fiList;
        }

        private void btnRemoveTermFromFiles_Click(object sender, EventArgs e)
        {
            // Parent node only
            if (!IsParent)
                return;

            var nextParent = SelectedNode.NextNode != null ? SelectedNode.NextNode : SelectedNode.PrevNode;
            string logText = "";

            var it = IndexedList.IndexedTerms.Find(SelectedNode.Name);

            RenameItem rn = new RenameItem();
            rn.TextName = ""; // it.Term;
            rn.RememberDefaultValue = true;
            rn.BlankAllowed = true;
            rn.TextNameEnabled = false;

            if (rn.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;

            if (rn.Remember)
                RenameTrackerList.Add(it.Term, rn.TextName);

            foreach (TreeNode subnode in SelectedNode.Nodes)
            {
                FileInfo fi = IndexedList.IndexedTerms.Find(it.ID, subnode.Text);

                if (fi != null)
                {
                    string newName = Path.GetFileNameWithoutExtension(fi.Name).Replace(it.Term, "", StringComparison.CurrentCultureIgnoreCase) + fi.Extension;

                    newName = CleanupFileName(newName);

                    if (fi.FullName != newName)
                    {
                        logText += "REN \"" + fi.FullName + "\" \"" + newName + "\"" + Environment.NewLine;
                    }
                }
            }

            var currentParent = SelectedNode;

            TvTerms.HideSelection = true;
            SelectedNode = nextParent;

            currentParent.Remove();

            rtbExecuteWindow.Text += logText;
            //WaitForCmd = true;
        }

        private void ignoreTermToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!IsParent)
                return;

            var nextParent = SelectedNode.NextNode != null ? SelectedNode.NextNode : SelectedNode.PrevNode;

            var it = IndexedList.IndexedTerms.Find(SelectedNode.Name);
            IgnoreTrackerList.Add(new IgnoreTrackerItem(it.Term));

            SelectedNode.Remove();
            SelectedNode = nextParent;
        }

        private enum REMOVENUMBERSOPTIONS { All, AtStart, AtEnd }

        private string RemoveNumbers(REMOVENUMBERSOPTIONS rno, string str)
        {
            string newName = "";

            if (rno == REMOVENUMBERSOPTIONS.All)
            {
                foreach (char c in str)
                {
                    int i;
                    if (!int.TryParse(c.ToString(), out i))
                    {
                        newName += c;
                    }
                }
            }
            else
            {
                int centerSep = (int)((double)str.Count(n => n == '.') / 2.0) + 1;

                if (centerSep == 1)
                {
                    return str;
                }

                int centerIndex = 0;
                int countSep = 0;

                for (int i = 0; i < str.Length; i++)
                {
                    if (str[i] == '.')
                    {
                        countSep++;

                        if (countSep == centerSep)
                        {
                            centerIndex = i;
                            break;
                        }
                    }
                }

                if (rno == REMOVENUMBERSOPTIONS.AtStart)
                {
                    string termRight = "";

                    for (int i = centerIndex + 1; i < str.Length; i++)
                    {
                        if (str[i] == '.')
                        {
                            break;
                        }
                        else
                        {
                            termRight = termRight + str[i];
                        }
                    }

                    if (string.IsNullOrEmpty(RemoveNumbers(REMOVENUMBERSOPTIONS.All, termRight)))
                    {
                        centerIndex = centerIndex + termRight.Length + 1;
                    }

                    newName = RemoveNumbers(REMOVENUMBERSOPTIONS.All, str.Substring(0, centerIndex)) + str.Substring(centerIndex);
                }
                else if (rno == REMOVENUMBERSOPTIONS.AtEnd)
                {
                    string termLeft = "";

                    for (int i = centerIndex - 1; i >= 0; i--)
                    {
                        if (str[i] == '.')
                        {
                            break;
                        }
                        else
                        {
                            termLeft = str[i] + termLeft;
                        }
                    }

                    if (string.IsNullOrEmpty(RemoveNumbers(REMOVENUMBERSOPTIONS.All, termLeft)))
                    {
                        centerIndex = centerIndex + termLeft.Length - 1;
                    }

                    newName = str.Substring(0, centerIndex) + RemoveNumbers(REMOVENUMBERSOPTIONS.All, str.Substring(centerIndex));
                }
            }

            while (newName.Contains(".."))
                newName = newName.Replace("..", ".");

            return newName.TrimEnd(new char[] { '.' });
        }

        private void RemoveNumbersUI(REMOVENUMBERSOPTIONS rno)
        {
            if (IsParent)
            {
                foreach (TreeNode node in SelectedNode.Nodes)
                {
                    var it = IndexedList.IndexedTerms.Find(SelectedNode.Name, node.Text);

                    if (it == null)
                        continue;

                    string newName = RemoveNumbers(rno, Path.GetFileNameWithoutExtension(it.Name));

                    if (string.IsNullOrEmpty(newName))
                        return;

                    newName += Path.GetExtension(it.Name);

                    string newPath = Path.Combine(it.DirectoryName, it.Name.Replace(it.Name, newName, StringComparison.CurrentCultureIgnoreCase));

                    if (!it.FullName.Equals(newPath))
                    {
                        rtbExecuteWindow.Text += GetMoveCmd(it.FullName, new FileInfo(newPath)) + Environment.NewLine;
                    }
                }

                RemoveItem(SelectedNode);
            }
            else if (IsChild)
            {
                var it = IndexedList.IndexedTerms.Find(SelectedNode.Parent.Name, SelectedNode.Text);

                if (it == null)
                    return;

                string newName = RemoveNumbers(rno, Path.GetFileNameWithoutExtension(it.Name));

                if (string.IsNullOrEmpty(newName))
                    return;

                newName += Path.GetExtension(it.Name);

                string newPath = Path.Combine(it.DirectoryName, it.Name.Replace(it.Name, newName, StringComparison.CurrentCultureIgnoreCase));

                if (!it.FullName.Equals(newPath))
                {
                    rtbExecuteWindow.Text += GetMoveCmd(it.FullName, new FileInfo(newPath)) + Environment.NewLine;
                    RemoveItem(SelectedNode);
                }
            }
        }

        private void toolStripMenuItemRemoveNumbers_Click(object sender, EventArgs e)
        {
            RemoveNumbersUI(REMOVENUMBERSOPTIONS.All);
        }

        private void toolStripMenuItemRemoveNumbersAtStart_Click(object sender, EventArgs e)
        {
            RemoveNumbersUI(REMOVENUMBERSOPTIONS.AtStart);
        }

        private void toolStripMenuItemRemoveNumbersAtEnd_Click(object sender, EventArgs e)
        {
            RemoveNumbersUI(REMOVENUMBERSOPTIONS.AtEnd);
        }

        private void toolStripMenuItemAddNote_Click(object sender, EventArgs e)
        {
            var it = IndexedList.IndexedTerms.Find(SelectedNode.Name);

            RenameItem rn = new RenameItem();
            rn.TextName = it.Note.Trim();
            rn.TextNameEnabled = true;
            rn.RememberEnabled = false;
            rn.BlankAllowed = true;

            if (rn.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                it.Note = rn.TextName.Trim();
                SelectedNode.Text = it.ToString();
            }
        }

        private void toolStripMenuItemSetRating_Click(object sender, EventArgs e)
        {
            int rating;
            var it = IndexedList.IndexedTerms.Find(SelectedNode.Name);

            RenameItem rn = new RenameItem();
            rn.TextName = it.Rating.ToString();
            rn.TextNameEnabled = true;
            rn.RememberEnabled = false;
            rn.BlankAllowed = false;

            if (rn.ShowDialog() == System.Windows.Forms.DialogResult.OK && int.TryParse(rn.TextName, out rating))
            {
                if (rating > InfoTrackerItem.RatingMax)
                    rating = InfoTrackerItem.RatingMax;

                if (rating < InfoTrackerItem.RatingMin)
                    rating = InfoTrackerItem.RatingBlank;

                it.Rating = rating;
                SelectedNode.Text = it.ToString();
            }
        }

        private void btnExecute_Click(object sender, EventArgs e)
        {
            rtbExecuteWindow.Text = rtbExecuteWindow.Text.Trim();
            rtbExecuteWindow.Lines.ToList().ForEach(l => Console.WriteLine("\t" + l));
            rtbExecuteWindow.Lines = rtbExecuteWindow.Lines.Where(l => l.Length > 0 && !l.StartsWith("REM", StringComparison.CurrentCultureIgnoreCase)).ToArray();

            if (string.IsNullOrEmpty(rtbExecuteWindow.Text))
                return;

            if (!AutomationRunning)
            {
                StartPrivateViewing();
            }

            try
            {
                string batchContents = "CD \\" + Environment.NewLine + /* "CLS" + */ Environment.NewLine + rtbExecuteWindow.Text;

                if (Control.ModifierKeys == Keys.Shift || Control.ModifierKeys == Keys.Control)
                {
                    batchContents += Environment.NewLine + "pause";
                }

                FileInfo fi = new FileInfo(Path.Combine(TempPath, Guid.NewGuid().ToString() + ".bat"));
                File.WriteAllText(fi.FullName, batchContents);

                rtbExecuteWindow.Clear();

                Task t = Task.Factory.StartNew(() =>
                    {
                        Process p = new Process();
                        p.StartInfo.FileName = fi.FullName;
                        p.StartInfo.Verb = "runas";

                        if (AutomationRunning)
                        {
                            p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                        }

                        p.Start();

                        if (WaitForCmd)
                        {
                            p.WaitForExit();
                        }
                    }
                    );

                while (!t.IsCompleted)
                {
                    Application.DoEvents();
                    Thread.Sleep(200);
                }

                File.Delete(fi.FullName);

                LastLoadCount = -1;
            }
            finally
            {
                ReloadList(true, true);

                if (!AutomationRunning)
                {
                    StartNormalViewing();
                }
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            FilterOnName.Clear();
            ReloadList(true, false);
            rtbExecuteWindow.Clear();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            FilterOnName.Clear();
            rtbExecuteWindow.Clear();
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            int maxOpen = 30;

            string args = "";

            List<TreeNode> list = new List<TreeNode>();

            if (IsParent)
            {
                // If ctrl is held then reverse the order
                if (e is KeyEventArgs && ((KeyEventArgs)e).Control)
                {
                    for (int i = SelectedNode.Nodes.Count - 1; i >= 0 && list.Count < maxOpen; i--)
                    {
                        list.Add(SelectedNode.Nodes[i]);
                    }
                }
                else
                {
                    for (int i = 0; i < SelectedNode.Nodes.Count && list.Count < maxOpen; i++)
                    {
                        list.Add(SelectedNode.Nodes[i]);
                    }
                }

                // If shift is held then take 5
                if (e is KeyEventArgs && ((KeyEventArgs)e).Shift)
                {
                    list = list.Take(5).ToList();
                }
            }
            else if (IsChild)
            {
                // If ctrl is held then reverse the order
                if (e is KeyEventArgs && ((KeyEventArgs)e).Control)
                {
                    for (int i = SelectedNode.Index; i >= 0 && list.Count < maxOpen; i--)
                    {
                        list.Add(SelectedNode.Parent.Nodes[i]);
                    }
                }
                else
                {
                    for (int i = SelectedNode.Index; i < SelectedNode.Parent.Nodes.Count && list.Count < maxOpen; i++)
                    {
                        list.Add(SelectedNode.Parent.Nodes[i]);
                    }
                }

                // If shift is held then take 5
                if (e is KeyEventArgs && ((KeyEventArgs)e).Shift)
                {
                    list = list.Take(5).ToList();
                }
                else
                {
                    list = list.Take(1).ToList();
                }
            }
            else
            {
                return;
            }

            SelectedNode.Expand();

            args = GetVideoArgs(list);

            if (!string.IsNullOrEmpty(args))
            {
                Process.Start("\"" + VLCPath + "\"", args);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            List<FileInfo> indexedFiles = new List<FileInfo>();

            if (IsParent)
            {
                foreach (TreeNode node in SelectedNode.Nodes)
                {
                    var it = IndexedList.IndexedTerms.Find(SelectedNode.Name, node.Text);

                    if (it != null)
                    {
                        indexedFiles.Add(it);
                    }
                }

                if (indexedFiles.Count == 1 || MessageBox.Show("Really delete these " + indexedFiles.Count + " files?", "Confirm", MessageBoxButtons.YesNoCancel) == System.Windows.Forms.DialogResult.Yes)
                {
                    List<string> diMovedFrom = new List<string>();

                    foreach (var fi in indexedFiles)
                    {
                        if (fi.FileExists())
                        {
                            fi.IsReadOnly = false;
                            rtbExecuteWindow.Text += "DEL \"" + fi.FullName + "\"" + Environment.NewLine;

                            if (!diMovedFrom.Contains(fi.DirectoryName, StringComparison.CurrentCultureIgnoreCase))
                                diMovedFrom.Add(fi.DirectoryName);
                        }
                    }

                    foreach (var di in diMovedFrom)
                    {
                        rtbExecuteWindow.Text += GetRemoveDirectoryCmd(di);
                    }

                    RemoveItem(SelectedNode);
                }
            }
            else if (IsChild || IsMultipleChild)
            {
                if (IsMultipleChild)
                {
                    foreach (TreeNode selected in SelectedNodes.ToList())
                    {
                        var fi = IndexedList.IndexedTerms.Find(selected.Parent.Name, selected.Text);

                        if (fi != null)
                        {
                            indexedFiles.Add(fi);
                        }

                        RemoveItem(selected);
                    }
                }
                else
                {
                    var fi = IndexedList.IndexedTerms.Find(SelectedNode.Parent.Name, SelectedNode.Text);
                    var fi2 = IndexedList.IndexedFiles.Find(SelectedNode);

                    if (fi != null)
                    {
                        indexedFiles.Add(fi);
                    }

                    RemoveItem(SelectedNode);
                }

                foreach (var fi in indexedFiles)
                {
                    fi.IsReadOnly = false;
                    rtbExecuteWindow.Text += "DEL \"" + fi.FullName + "\"" + Environment.NewLine;
                    rtbExecuteWindow.Text += GetRemoveDirectoryCmd(fi.Directory.FullName);
                }

                if (SelectedNode.Parent != null)
                {
                    var updatedItem = IndexedList.IndexedTerms.Find(SelectedNode.Parent.Name);

                    if (updatedItem != null)
                    {
                        SelectedNode.Parent.Text = updatedItem.ToString();
                    }
                }
            }

            //WaitForCmd = true;
        }

        private string GetRemoveDirectoryCmd(string directory)
        {
            if (!IsSymbolic(directory) && !directory.ToLower().Equals(TorrentPath.ToLower()))
            {
                return "RD /Q \"" + directory + "\" >NUL 2>&1" + Environment.NewLine;
            }

            return string.Empty;
        }

        private bool IsSymbolic(string path)
        {
            FileInfo pathInfo = new FileInfo(path);
            return pathInfo.Attributes.HasFlag(FileAttributes.ReparsePoint);
        }

        private void rtbLogging_TextChanged(object sender, EventArgs e)
        {
            btnExecute.Enabled = rtbExecuteWindow.Text.Length > 0;
        }

        private void rtbExecuteWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F && e.Control)
            {
                TV_KeyDown(sender, e);
            }
            else if (e.KeyCode == Keys.C && e.Control)
            {
                return;
            }
            else if (e.KeyCode != Keys.Delete && e.KeyCode != Keys.Back && e.KeyCode != Keys.Up && e.KeyCode != Keys.Down && e.KeyCode != Keys.Right && e.KeyCode != Keys.Left)
            {
                // Ignore keypress
                e.SuppressKeyPress = true;
            }
        }

        private void TV_NodeMouseDoubleClick(object sender, MouseEventArgs e)
        {
            btnOpen_Click(null, e);
        }

        private void TV_KeyDown(object sender, KeyEventArgs e)
        {
            // Delete
            if (e.KeyCode == Keys.Delete)
            {
                btnDelete_Click(sender, e);
            }
            // Open
            else if (e.KeyCode == Keys.Enter)
            {
                btnOpen_Click(sender, e);
            }
            // Refresh
            else if (e.KeyCode == Keys.F5)
            {
                btnRefresh_Click(sender, e);
            }
            // Remove Numbers - Start
            else if (e.KeyCode == Keys.S && e.Control)
            {
                RemoveNumbersUI(REMOVENUMBERSOPTIONS.AtStart);
            }
            // Remove Numbers - End
            else if (e.KeyCode == Keys.E && e.Control)
            {
                RemoveNumbersUI(REMOVENUMBERSOPTIONS.AtEnd);
            }
            else if (e.KeyCode == Keys.A && e.Control)
            {
                RemoveNumbersUI(REMOVENUMBERSOPTIONS.All);
            }
            // Select Random
            else if (e.KeyCode == Keys.R && e.Control)
            {
                Random rnd = new Random();
                int i = rnd.Next(0, TopNode.Nodes.Count - 1);

                SelectedNode = TopNode.Nodes[i];
                SelectedNode.Expand();

                int j = rnd.Next(0, SelectedNode.Nodes.Count - 1);
                SelectedNode = SelectedNode.Nodes[j];
            }
            // Ignore Term
            else if (e.KeyCode == Keys.I && e.Control)
            {
                if (IsParent)
                {
                    ignoreTermToolStripMenuItem_Click(null, null);
                }
            }
            // Find
            else if ((e.KeyCode == Keys.F && e.Control) || e.KeyCode == Keys.F1)
            {
                RenameItem rn = new RenameItem(blankAllowed: true);
                rn.Text = "Find";
                rn.RememberDefaultValue = false;
                rn.RememberEnabled = false;
                rn.ButtonText = "OK";

                if (rn.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    if (string.IsNullOrEmpty(rn.TextName))
                    {
                        FilterOnName.Clear();
                    }
                    else
                    {
                        if (!FilterOnName.Contains(rn.TextName, StringComparison.CurrentCultureIgnoreCase))
                        {
                            FilterOnName.Add(rn.TextName);
                        }
                    }

                    ApplyFilter();
                    // ApplyNameFilter();
                }
            }
            // Change Sort
            else if (e.Control && (e.KeyValue >= (int)Keys.D1 && e.KeyValue <= (int)Keys.D6))
            {
                cbSort.SelectedIndex = (e.KeyValue - (int)Keys.D1);
            }
            // Hide List 
            else if (e.Control && e.KeyCode == Keys.H)
            {
                PrivateViewing = !PrivateViewing;
                ReloadList();
            }
        }

        private void btnRename_Click(object sender, EventArgs e)
        {
            if (IsParent)
            {
                string term = GetTerm(SelectedNode);

                RenameItem rn = new RenameItem();
                rn.TextName = term;
                rn.RememberDefaultValue = true;
                rn.RememberEnabled = true;

                if (rn.ShowDialog() == System.Windows.Forms.DialogResult.OK && rn.TextName != term)
                {
                    foreach (TreeNode node in SelectedNode.Nodes)
                    {
                        var it = IndexedList.IndexedTerms.Find(SelectedNode.Name, node.Text);

                        if (it == null)
                            continue;

                        string newPath = Path.Combine(it.DirectoryName, it.Name.Replace(term, rn.TextName, StringComparison.CurrentCultureIgnoreCase));

                        if (!it.FullName.Equals(newPath))
                        {
                            rtbExecuteWindow.Text += GetMoveCmd(it.FullName, new FileInfo(newPath)) + Environment.NewLine;

                            if (rn.Remember)
                                RenameTrackerList.Add(term, rn.TextName, true);
                        }
                    }

                    RemoveItem(SelectedNode);
                }
            }
            else if (IsChild)
            {
                var fi = IndexedList.IndexedTerms.Find(SelectedNode.Parent.Name, SelectedNode.Text);

                RenameItem rn = new RenameItem();
                rn.TextName = Path.GetFileNameWithoutExtension(fi.Name);
                rn.RememberDefaultValue = false;
                rn.RememberEnabled = true;

                if (rn.ShowDialog() == System.Windows.Forms.DialogResult.OK && rn.TextName != fi.Name)
                {
                    rtbExecuteWindow.Text += GetMoveCmd(fi.FullName, new FileInfo(Path.Combine(fi.Directory.FullName, rn.TextName + fi.Extension))) + Environment.NewLine;

                    if (rn.Remember)
                        RenameTrackerList.Add(fi.Name, rn.TextName, true);

                    RemoveItem(SelectedNode);
                }
            }
        }

        private void toolStripMenuItemMove_Click(object sender, EventArgs e)
        {
            MoveSelected();
        }

        private void toolStripMenuItemMoveAndShortcut_Click(object sender, EventArgs e)
        {
            contextMenuStrip.Close();

            if (IsChild)
            {
                var list = MoveSelected();

                foreach (var file in list)
                {
                    rtbExecuteWindow.Text += CreateShortcutString(Path.Combine(FavoritesDir, file.Name + " - Shortcut"), file.FullName) + Environment.NewLine;
                }
            }
        }

        private void toolStripMenuItemMoveAndShortcutStar_Click(object sender, EventArgs e)
        {
            contextMenuStrip.Close();

            if (IsChild)
            {
                var list = MoveSelected();

                foreach (var file in list)
                {
                    rtbExecuteWindow.Text += CreateShortcutString(Path.Combine(FavoritesStarDir, file.Name + " - Shortcut"), file.FullName) + Environment.NewLine;
                }
            }
        }

        private void toolStripMenuItemRename_Click(object sender, EventArgs e)
        {
            btnRename_Click(null, null);
        }

        private void toolStripMenuItemOpenFolder_Click(object sender, EventArgs e)
        {
            if (IsChild)
            {
                var fi = IndexedList.IndexedTerms.Find(SelectedNode);

                Process prc = new Process();
                prc.StartInfo.FileName = fi.DirectoryName;
                prc.Start();
            }
        }

        private void toolStripMenuItemAddTag_Click(object sender, EventArgs e)
        {
            if (SelectedNode.Nodes.Count > 0)
            {
                AddTag();
            }
        }

        private void SetTreeNodeUIOptions()
        {
            bool isChild = IsChild;
            bool isParent = IsParent;
            bool isMultipleParent = IsMultipleParent;
            bool isTop = IsTopNode;

            // Valid for all types
            toolStripMenuItemConvert.Enabled = true;

            // Valid for all types except top
            toolStripMenuItemMove.Enabled = (!isTop || GetFilterShowType == FilterShowTypes.MV);
            toolStripMenuItemRemoveNumbers.Enabled = !isTop;
            moveStandardToolStripMenuItem.Enabled = !isTop;
            doNotRememberLocationToolStripMenuItem.Enabled = (!isTop && GetFilterShowType != FilterShowTypes.MV && GetFilterShowType != FilterShowTypes.SV);
            toolStripMenuItemRename.Enabled = !isTop;

            toolStripMenuItemResetAutoMove.Enabled = !isTop;
            toolStripMenuItemAddNote.Enabled = (isParent && !IsMultipleParent);
            toolStripMenuItemSetRating.Enabled = (isParent && !IsMultipleParent);

            // Only valid for parent
            removeTermToolStripMenuItem.Enabled = IsParent;
            termMustStartToolStripMenuItem.Checked = false;
            termMustStartToolStripMenuItem.Visible = false;
            toolStripMenuItemAddTag.Enabled = IsParent; // && SelectedNode.Text.ToString().Contains(FilterShowTypes.GT.ToString());

            // Only valid for children
            toolStripMenuItemOpenFolder.Enabled = isChild;
            toolStripMenuItemStreaming.Enabled = isChild;
            moveGoodToolStripMenuItem.Enabled = isChild;
            moveGreatToolStripMenuItem.Enabled = isChild;
            toolStripMenuItemGoogle.Enabled = isChild;

            foreach (var item in contextMenuStrip.Items)
            {
                if (item is ToolStripMenuItem && item != actionsToolStripMenuItem)
                {
                    SetToolStripMenuChildrenHidden((ToolStripMenuItem)item);
                }
            }

            doNotRememberLocationToolStripMenuItem.Enabled = doNotRememberLocationToolStripMenuItem.Visible = moveStandardToolStripMenuItem.Enabled;

            toolStripMenuItemRemoveNumbers.Enabled = true;

            if (isParent)
            {
                var term = IndexedList.IndexedTerms.Find(SelectedNode.Tag.ToString());

                if (term != null)
                {
                    if (!isTop && term.ToString().Contains(Main.FilterShowTypes.SV.ToString()))
                    {
                        termMustStartToolStripMenuItem.Visible = true;

                        var ri = RenameTrackerList.GetItem(term.Term);

                        if (ri != null)
                        {
                            termMustStartToolStripMenuItem.Checked = ri.RequireTermAtStart;
                        }
                        else
                        {
                            ri = new RenameTrackerItem(term.Term, term.Term);
                            RenameTrackerList.Add(ri);
                            termMustStartToolStripMenuItem.Checked = ri.RequireTermAtStart;
                        }
                    }
                }
            }
        }

        private void SetToolStripMenuChildrenHidden(ToolStripMenuItem item)
        {
            var enabledCount = item.DropDown.Items.Cast<ToolStripItem>().Count(n => n.Enabled);

            if (item.DropDown.Items.Cast<ToolStripItem>().Count(n => n.Enabled) == 1)
            {
                for (int i = 0; i < item.DropDown.Items.Count; i++)
                {
                    item.DropDown.Items[i].Visible = false;
                }

                return;
            }

            if (item.DropDown.Items.Count == 1)
            {
                item.DropDown.Items[0].Visible = false;
                return;
            }

            for (int i = 0; i < item.DropDown.Items.Count; i++)
            {
                item.DropDown.Items[i].Visible = item.DropDown.Items[i].Enabled;

                if (item is ToolStripMenuItem)
                {
                    SetToolStripMenuChildrenHidden(item.DropDown.Items[i] as ToolStripMenuItem);
                }

            }
        }

        private void SetToolStripMenuChildren(ToolStripMenuItem item, bool visible, bool enabled)
        {
            for (int i = 0; i < item.DropDown.Items.Count; i++)
            {
                ToolStripItem subItem = item.DropDown.Items[i];
                subItem.Visible = visible;
                subItem.Enabled = enabled;

                if (item is ToolStripMenuItem)
                {
                    SetToolStripMenuChildren(subItem as ToolStripMenuItem, visible, enabled);
                }

            }
        }

        private bool SingleSelected
        {
            get
            {
                return SelectedNode != null && SelectedNodes.Count == 1;
            }
        }

        private bool IsParent
        {
            get
            {
                return !IsTopNode && SingleSelected && SelectedNode.Parent == TopNode && SelectedNode.Nodes.Count > 0;
            }
        }

        private bool IsParentNode(TreeNode node)
        {
            return node != null && node.Parent != null && node.Parent.Parent == null;
        }

        private bool IsMultipleParent
        {
            get
            {
                if (SelectedNodes == null || SelectedNodes.Count < 2)
                    return false;

                foreach (TreeNode selected in SelectedNodes)
                {
                    if (!IsParentNode(selected))
                        return false;
                }

                return true;
            }
        }

        private bool IsChild
        {
            get
            {
                return SelectedNode != null && SelectedNodes.Count == 1 && !IsTopNode && !IsParent;
            }
        }

        private bool IsChildNode(TreeNode node)
        {
            return node != null && node.Parent != null && node.Parent.Parent != null;
        }

        private bool IsMultipleChild
        {
            get
            {
                if (SelectedNodes == null || SelectedNodes.Count < 2)
                    return false;

                foreach (TreeNode selected in SelectedNodes)
                {
                    if (!IsChildNode(selected))
                        return false;
                }

                return true;
            }
        }

        private bool IsTopNode
        {
            get
            {
                return SelectedNode.Parent == null;
                //return SelectedNode != null && SelectedNodes.Count == 1 && SelectedNodes[0].Tag == _topNode.Tag;
            }
        }

        private void TV_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                if (!e.Node.IsSelected)
                    SelectedNode = e.Node;
            }
        }

        private void TV_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            btnOpen_Click(sender, e);
        }

        private void CreateShortcut(string shortcutFile, string shortcutSource)
        {
            if (File.Exists(shortcutFile))
                File.Delete(shortcutFile);

            Process prc = new Process();
            prc.StartInfo.FileName = ShortcutCreator;
            prc.StartInfo.Arguments = string.Format("/F:\"{0}\" /A:C /T:\"{1}\"", shortcutFile, shortcutSource);
            prc.Start();
        }

        private string CreateShortcutString(string shortcutFile, string shortcutSource)
        {
            if (!shortcutFile.EndsWith(".lnk"))
                shortcutFile += ".lnk";

            return string.Format("\"{0}\" /F:\"{1}\" /A:C /T:\"{2}\"", ShortcutCreator, shortcutFile, shortcutSource);
        }

        private void btnAutoFile_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;

            RunPowershellScript(RemoveForeignCharsScriptFile);

            List<string> filesMoved = new List<string>();

            foreach (var ilTerm in IndexedList.IndexedTerms)
            {
                foreach (var ri in RenameTrackerList.GetList().Where(n => n.TagOnly).OrderByDescending(n => n.SourceString.Length))
                {
                    if (ilTerm.Term.Equals(ri.SourceString, StringComparison.CurrentCultureIgnoreCase))
                    {
                        foreach (var indFile in ilTerm.IndexedFiles)
                        {
                            string newPath = Path.Combine(indFile.DirectoryName, indFile.Name.Replace(ri.SourceString, ri.DestinationString, StringComparison.CurrentCultureIgnoreCase));

                            if (!indFile.FullName.Equals(newPath, StringComparison.CurrentCultureIgnoreCase))
                            {
                                rtbExecuteWindow.Text += GetMoveCmd(indFile.FullName, new FileInfo(newPath)) + Environment.NewLine;
                                filesMoved.Add(indFile.FullName);
                            }
                        }
                    }
                }
            }

            string executeText = "";

            // Fix sub directory names
            foreach (var ilDir in SourceDirectories.Where(n => n.AutoFile && n.RenameFolders))
            {
                Parallel.ForEach(ilDir.GetFiles(true), (ilFile) =>
                {
                    // Skip files located in the root directory
                    if (ilDir.DirectoryInfo.FullName == ilFile.Directory.FullName)
                    {
                        return;
                    }
                    else if (filesMoved.Any(n => n == ilFile.FullName))
                    {
                        return;
                    }

                    var newDir = ilFile.Directory.Name;

                    if (ilFile.Directory.Name[0] != '_')
                    {
                        foreach (string ct in TermOptions.AutoRemoveContainingTags.OrderByDescending(n => n.Length))
                        {
                            if (newDir.Contains(ct, StringComparison.CurrentCultureIgnoreCase))
                            {
                                newDir = newDir.Replace(ct, "", StringComparison.CurrentCultureIgnoreCase);
                            }
                        }

                        foreach (var ar in TermOptions.AutoReplaceTags.OrderByDescending(n => n.Key.Length))
                        {
                            if (newDir.Contains(ar.Key, StringComparison.CurrentCultureIgnoreCase))
                            {
                                newDir = newDir.Replace(ar.Key, ar.Value, StringComparison.CurrentCultureIgnoreCase);
                            }
                        }

                        foreach (var ar in TermOptions.AutoReplaceTags.OrderByDescending(n => n.Key.Length))
                        {
                            if (newDir.Contains(ar.Key, StringComparison.CurrentCultureIgnoreCase))
                            {
                                newDir = newDir.Replace(ar.Key, ar.Value, StringComparison.CurrentCultureIgnoreCase);
                            }
                        }

                        if (newDir != ilFile.Directory.Name)
                        {
                            executeText += GetRenameCmd(ilFile.Directory.FullName, newDir) + Environment.NewLine;

                            lock (filesMoved)
                            {
                                filesMoved.Add(ilFile.FullName);
                            }
                        }
                    }
                });
            }

            foreach (var ilDir in SourceDirectoriesToAutoFile)
            {
                Parallel.ForEach(ilDir.GetFiles(true), (ilFile) =>
                {
                    if (!IndexExtensions.DefaultIndexExtentions.Any(n => n.Equals(ilFile.Extension, StringComparison.CurrentCultureIgnoreCase)))
                    {
                        return;
                    }

                    lock (filesMovedLocker)
                    {
                        if (filesMoved.Any(n => n == ilFile.FullName))
                        {
                            return;
                        }
                    }

                    bool badCharsFound = false;

                    string name = Path.GetFileNameWithoutExtension(ilFile.Name).ToLower();
                    var invalidChars = Path.GetInvalidFileNameChars().ToList();
                    invalidChars.Add('%');

                    foreach (char c in invalidChars)
                    {
                        if (name.Contains(c))
                        {
                            badCharsFound = true;
                            name = name.Replace(c, ' ');
                        }
                    }

                    if (badCharsFound)
                    {
                        File.Move(ilFile.FullName, Path.Combine(ilFile.DirectoryName, name + Path.GetExtension(ilFile.Name).ToLower()));
                    }
                    else
                    {
                        name = Path.GetFileNameWithoutExtension(ilFile.Name).ToLower();
                    }

                    name = RemoveDiacritics(name);

                    while (true)
                    {
                        string startingName = name;

                        if (name.Length <= 4 && name.All(n => !char.IsLetter(n)))
                        {
                            name = name + "_" + DateTime.Now.Ticks;
                        }

                        foreach (var ar in RenameTrackerList.GetList().Where(n => !n.TagOnly).OrderByDescending(n => n.SourceString.Length))
                        {
                            if (name.Contains(ar.SourceString, StringComparison.CurrentCultureIgnoreCase) && ar.SourceString != ar.DestinationString)
                            {
                                name = name.Replace(ar.SourceString, ar.DestinationString, StringComparison.CurrentCultureIgnoreCase);
                            }
                        }

                        foreach (string ct in TermOptions.AutoRemoveContainingTags.OrderByDescending(n => n.Length))
                        {
                            if (name.Contains(ct, StringComparison.CurrentCultureIgnoreCase))
                            {
                                name = name.Replace(ct, "", StringComparison.CurrentCultureIgnoreCase);
                            }
                        }

                        foreach (var ar in TermOptions.AutoReplaceTags.OrderByDescending(n => n.Key.Length))
                        {
                            if (name.Contains(ar.Key, StringComparison.CurrentCultureIgnoreCase))
                            {
                                name = name.Replace(ar.Key, ar.Value, StringComparison.CurrentCultureIgnoreCase);
                            }
                        }

                        name = TermOptions.ReplaceMonthData(name);

                        foreach (string sep in Indexer.IndexSeperators.OrderByDescending(n => n.Length))
                        {
                            foreach (string ae in TermOptions.AutoRemoveStartingTags.OrderByDescending(n => n.Length))
                            {
                                if (name.StartsWith(ae, StringComparison.CurrentCultureIgnoreCase))
                                {
                                    name = name.Substring(ae.Length);
                                }
                            }

                            if (name.StartsWith(sep))
                            {
                                name = name.Substring(sep.Length);
                            }

                            if (name.EndsWith(sep))
                            {
                                name = name.Substring(0, name.Length - 1);
                            }

                            if (name.Contains(sep + sep))
                            {
                                name = name.Replace(sep + sep, sep);
                            }

                            if (name.Contains("("))
                            {
                                name = name.Replace("(", " ");
                            }

                            if (name.Contains(")"))
                            {
                                name = name.Replace(")", " ");
                            }
                        }

                        name = RemoveStartsWithOnlyNumbers(name);
                        name = TermOptions.InsertSeperatorBetweenNumbers(name, minNumbersLength: 2, minLettersLength: 3);

                        // Try to normalize date strings
                        name = FormatDate(name);

                        if (name == startingName)
                        {
                            break;
                        }
                    }

                    if (name != Path.GetFileNameWithoutExtension(ilFile.Name))
                    {
                        string newPath = Path.Combine(ilFile.DirectoryName, name + Path.GetExtension(ilFile.Name).ToLower());
                        executeText += GetMoveCmd(ilFile.FullName, new FileInfo(newPath)) + Environment.NewLine;

                        lock (filesMovedLocker)
                        {
                            filesMoved.Add(ilFile.FullName);
                        }
                    }
                });
            }

            if (!string.IsNullOrEmpty(executeText))
            {
                rtbExecuteWindow.Text += executeText;
            }

            foreach (var sd in SourceDirectoriesToAutoFile.Where(n => n.CleanupSmallFiles))
            {
                ProcessEmptyDirectories(sd);
            }

            rtbExecuteWindow.Lines = rtbExecuteWindow.Lines.Distinct().ToArray();

            Cursor = Cursors.Default;
        }

        private string RemoveDiacritics(string s)
        {
            String normalizedString = s.Normalize(NormalizationForm.FormD);
            StringBuilder stringBuilder = new StringBuilder();

            for (int i = 0; i < normalizedString.Length; i++)
            {
                Char c = normalizedString[i];
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                    stringBuilder.Append(c);
            }

            return stringBuilder.ToString();
        }

        private string RemoveStartsWithOnlyNumbers(string s)
        {
            if (s.All(n => char.IsDigit(n) || n.ToString() == IndexSeperators.PrimarySeperator))
            {
                return s;
            }

            string partialStr = "";

            for (int i = 0; i < s.Length; i++)
            {
                if (s[i].ToString() == IndexSeperators.PrimarySeperator)
                {
                    break;
                }
                else
                {
                    partialStr += s[i];
                }
            }

            if (partialStr.All(n => char.IsDigit(n)))
            {
                string result = s.Substring(partialStr.Length + IndexSeperators.PrimarySeperator.Length);

                if (result.Length > 4)
                {
                    return result;
                }
                else
                {
                    return s;
                }
            }
            else
            {
                return s;
            }
        }

        private void btnRemoveDups_Click(object sender, EventArgs e)
        {
            this.Enabled = false;
            int heightBefore = this.Height;

            using (new TimeOperation("Remove Duplicates Operation"))
            {
                try
                {
                    if (!AutomationRunning)
                    {
                        this.Height = PrivateViewingHeight;
                    }

                    using (new TimeOperation("Duplicates Seen Before Operation"))
                    {
                        // Search for files that have been deleted in the past
                        var seenBefore = HashTrackerList.List.AsParallel().
                        Where(n => n.Length > TermOptions.ExcludeRules.MinimumSizeToIndexInB).
                        GroupBy(n => new { FileName = Path.GetFileName(n.Path) }).
                        Select(n => new
                        {
                            FileName = n.Key,
                            List = HashTrackerList.List.Where(i => Path.GetFileName(i.Path) == n.Key.FileName && i.DateModified != DateTime.MinValue).ToList()
                        }).
                        Select(n => new
                        {
                            FileName = n.FileName,
                            List = n.List,
                            Master = n.List.OrderBy(d => d.DateModified).FirstOrDefault()
                        }).
                        Select(n => new
                        {
                            Master = n.Master,
                            FilesToDelete = n.List.Where(i => !i.Path.StartsWith(KeepDirectory) && File.Exists(i.Path) && (i.Length > n.Master.Length ? (i.Length / 2) < n.Master.Length : (n.Master.Length / 2) < i.Length) && (i.DateModified != n.Master.DateModified || i.DateModified != (new FileInfo(i.Path).LastAccessTimeUtc))).ToList()
                        }).
                        Where(n => n.FilesToDelete.Count > 0).
                        ToList();

                        foreach (var item in seenBefore)
                        {
                            bool headerWritten = false;

                            foreach (var file in item.FilesToDelete)
                            {
                                FileInfo latest = new FileInfo(file.Path);

                                if (latest.LastWriteTimeUtc != item.Master.DateModified)
                                {
                                    if (!headerWritten)
                                    {
                                        headerWritten = true;
                                        rtbExecuteWindow.Text += "REM Earliest File Seen Date: " + item.Master.DateModified.ToShortDateString() + " " + item.Master.DateModified.ToLongTimeString() + Environment.NewLine;
                                    }

                                    rtbExecuteWindow.Text += "REM This File Seen Date:        " + latest.LastWriteTimeUtc.ToShortDateString() + " " + latest.LastWriteTimeUtc.ToLongTimeString() + Environment.NewLine;
                                    rtbExecuteWindow.Text += "DEL \"" + file.Path + "\"" + Environment.NewLine;
                                    //rtbExecuteWindow.Text += "REM This File Seen Date:        " + file.DateModified.ToShortDateString() + " " + file.DateModified.ToLongTimeString() + Environment.NewLine;
                                    //rtbExecuteWindow.Text += "DEL \"" + file.Path + "\"" + Environment.NewLine;
                                }
                            }
                        }
                    }

                    Dictionary<string, List<FileInfo>> dictHash = new Dictionary<string, List<FileInfo>>();
                    Dictionary<string, List<FileInfo>> dictName = new Dictionary<string, List<FileInfo>>();
                    List<string> dupesHash = new List<string>();
                    List<string> dupesName = new List<string>();

                    using (new TimeOperation("Hash Update Operation"))
                    {
                        foreach (var sd in SourceDirectoriesToCheckForDuplicates)
                        {
                            Task t = Task.Factory.StartNew(() =>
                            {
                                Parallel.ForEach(sd.GetFiles(), (fi) =>
                                {
                                    if (IndexExtensions.DefaultIndexExtentions.Any(n => n.Equals(fi.Extension, StringComparison.CurrentCultureIgnoreCase)) && !fi.FullName.StartsWith(@"E:\_P\_Air"))
                                    {
                                        var hti = HashTrackerList.GetItem(fi);

                                        lock (dictHash)
                                        {
                                            if (dictHash.ContainsKey(hti.ShortHash))
                                            {
                                                dictHash[hti.ShortHash].Add(fi);
                                                dupesHash.Add(hti.ShortHash);
                                            }
                                            else
                                            {
                                                dictHash[hti.ShortHash] = new List<FileInfo>() { fi };
                                            }
                                        }

                                        string name = Path.GetFileNameWithoutExtension(fi.Name.ToLower());

                                        lock (dictName)
                                        {
                                            if (dictName.ContainsKey(name))
                                            {
                                                dictName[name].Add(fi);
                                                dupesName.Add(name);
                                            }
                                            else
                                            {
                                                dictName[name] = new List<FileInfo>() { fi };
                                            }
                                        }

                                    }
                                });
                            }
                            );

                            while (!t.IsCompleted)
                            {
                                Application.DoEvents();
                                Thread.Sleep(500);
                            }
                        }
                    }

                    List<string> NoLongerExist = new List<string>();

                    if (dupesName.Count > 0)
                    {
                        rtbExecuteWindow.Text += "REM Removing Dupes based on file name" + Environment.NewLine;
                    }

                    foreach (string name in dupesName)
                    {
                        var winner = dictName[name].Where(n => n.Length == dictName[name].Max(m => m.Length)).OrderBy(n => n.FullName.Length).First();

                        foreach (var item in dictName[name])
                        {
                            if (item == winner)
                            {
                                rtbExecuteWindow.Text += "REM Keeping winner: " + item.FullName + "\"" + Environment.NewLine;
                            }
                            else
                            {
                                rtbExecuteWindow.Text += "REM Removing loser: " + item.FullName + "\"" + Environment.NewLine;
                                rtbExecuteWindow.Text += "DEL \"" + item.FullName + "\"" + Environment.NewLine;
                                NoLongerExist.Add(item.FullName);
                            }
                        }

                        foreach (var item in dictName[name])
                        {
                            if (item != winner && item.FullName.StartsWith(KeepDirectory) && !winner.FullName.StartsWith(KeepDirectory))
                            {
                                rtbExecuteWindow.Text += "REM Moving winner: " + item.FullName + "\"" + Environment.NewLine;
                                rtbExecuteWindow.Text += GetMoveCmd(winner.FullName, item.Directory) + Environment.NewLine;
                                NoLongerExist.Add(winner.FullName);
                                NoLongerExist.Add(Path.Combine(item.Directory.FullName, winner.Name));
                                break;
                            }
                        }
                    }

                    if (dupesHash.Count > 0)
                    {
                        rtbExecuteWindow.Text += "REM Removing Dupes based on file hash" + Environment.NewLine;
                    }

                    foreach (string hash in dupesHash)
                    {
                        var winner = HashTrackerList.GetItem(dictHash[hash].Where(n => n.Length == dictHash[hash].Max(m => m.Length)).OrderBy(n => n.FullName.Length).First());

                        rtbExecuteWindow.Text += "REM Keeping winner: " + winner.Path + "\"" + Environment.NewLine;

                        foreach (var item in dictHash[hash])
                        {
                            if (!NoLongerExist.Contains(item.FullName, StringComparison.CurrentCultureIgnoreCase))
                            {
                                if (item.FullName != winner.Path)
                                {
                                    var hti = HashTrackerList.GetItem(item);

                                    // Check the hash farther in
                                    if (hti.LongHash == winner.LongHash)
                                    {
                                        rtbExecuteWindow.Text += "REM Removing loser: " + item.FullName + "\"" + Environment.NewLine;
                                        rtbExecuteWindow.Text += "DEL \"" + item.FullName + "\"" + Environment.NewLine;
                                    }
                                    // Same small hashes but different large hashes, probably a longer version of the same movie
                                    else if (item.Length <= winner.Length)
                                    {
                                        rtbExecuteWindow.Text += "REM Removing loser: " + item.FullName + "\"" + Environment.NewLine;
                                        rtbExecuteWindow.Text += "DEL \"" + item.FullName + "\"" + Environment.NewLine;
                                    }
                                }
                            }
                        }
                    }
                }
                finally
                {
                    this.Height = heightBefore;
                    this.Enabled = true;
                }
            }
        }

        private void btnClearTrash_Click(object sender, EventArgs e)
        {
            foreach (var sd in SourceDirectoriesToAutoFile.Where(n => n.CleanupSmallFiles))
            {
                ProcessSmallFiles(sd);
                ProcessEmptyDirectories(sd);
            }
        }

        public static string GetMD5Hash(FileInfo fi, long readBytes = 0, bool writeTiming = false)
        {
            if (!File.Exists(fi.FullName))
            {
                return Guid.Empty.ToString();
            }

            using (new TimeOperation("GetMD5Hash Operation", !writeTiming))
            {
                try
                {
                    if (readBytes <= 0)
                    {
                        using (MD5CryptoServiceProvider csp = new MD5CryptoServiceProvider())
                        {
                            using (FileStream fs = new FileStream(fi.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                            {
                                return BitConverter.ToString(csp.ComputeHash(fs)).Replace("-", "");
                            }
                        }
                    }
                    else
                    {
                        using (FileStream fs = new FileStream(fi.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                        {
                            using (BinaryReader br = new BinaryReader(fs))
                            {
                                if (readBytes > fs.Length)
                                {
                                    readBytes = fs.Length;
                                }
                                else if (readBytes > int.MaxValue)
                                {
                                    readBytes = 0;
                                }

                                return GetMD5Hash(br.ReadBytes((int)readBytes));
                            }
                        }
                    }
                }
                finally
                {
                }
            }
        }

        private static string GetMD5Hash(byte[] fileContents)
        {
            using (MD5CryptoServiceProvider csp = new MD5CryptoServiceProvider())
            {
                return BitConverter.ToString(csp.ComputeHash(fileContents)).Replace("-", "");
            }
        }

        private string FormatDate(string input)
        {
            string result = input;

            try
            {
                // Insert seperators between date parts
                result = Regex.Replace(result, @"\b(\d{2})(\d{2})20(\d{2})\b", "$1.$2.$3");

                //// Reduce from YYYY to YY when year at end
                result = Regex.Replace(result, @"\b(\d{2})\.(\d{2})\.20(\d{2})\b", "$1.$2.$3");

                // Reduce from YYYY to YY and put year last when year at end
                result = Regex.Replace(result, @"\b20(\d{2})\.(\d{2})\.(\d{2})\b", "$2.$3.$1");

                //// Reduce from YYYY to YY When year at end
                //result = Regex.Replace(result,
                //      "\\b(?<a>\\d{2,2})(?<b>\\d{2,2})20(?<c>\\d{2,2})\\b",
                //      "${a}${b}${c}");

                //// Reduce from YYYY to YY When year at start
                //result = Regex.Replace(result,
                //      "\\b20(?<a>\\d{2,2})(?<b>\\d{2,2})(?<c>\\d{2,2})\\b",
                //      "${a}${b}${c}");

                //// Insert seperators between date parts
                result = Regex.Replace(result,
                      "\\b(?<a>\\d{2,2})(?<b>\\d{2,2})(?<c>\\d{2,2})\\b",
                      "${a}.${b}.${c}");

                //result = Regex.Replace(result,
                //      "\\b(?<a>\\d{2,2})(?<b>\\d{2,2})(?<c>\\d{2,2})\\b",
                //      "${a}.${b}.${c}");

                //result = Regex.Replace(result,
                //      "\\b(?<s>[a-z]+)(?<a>\\d{2,2})(?<b>\\d{2,2})(?<c>\\d{2,2})(?<e>[a-z]+)\\b",
                //      "${s}.${a}.${b}.${c}.${e}");

                if (input != result)
                {
                    return result;
                }
                else
                {
                    return input;
                }
            }
            catch
            {
                return result;
            }
        }

        private void ProcessEmptyDirectories(DirectoryInfoExtended root)
        {
            string executeText = "";

            Parallel.ForEach(root.GetDirectories(true), (subDir) =>
            // foreach (var subDir in root.GetDirectories(true))
            {
                if (subDir.GetDirectories().Count() == 0 && subDir.GetFiles().Count() == 0 && subDir.Name[0] != '_')
                {
                    executeText += GetRemoveDirectoryCmd(subDir.FullName);
                }
            });

            rtbExecuteWindow.Text += executeText;
        }

        private void ProcessSmallFiles(DirectoryInfoExtended root)
        {
            string executeText = "";

            Parallel.ForEach(root.GetFiles(true).Where(n => n.Length < TermOptions.ExcludeRules.MinimumSizeToKeepInB && n.Name[0] != '_' && !n.FullName.StartsWith(KeepDirectory) &&  !TermOptions.IgnoreExtensions.Contains(new FileInfo(n.FullName).Extension.ToLower())), (fi) =>
            {
                // Skip files that have been modified recently
                if (fi.LastWriteTimeUtc > DateTime.UtcNow.Subtract(TimeSpan.FromSeconds(30)))
                {
                    return;
                }

                // Skip system files like thumbs.db
                if (fi.Attributes.HasFlag(FileAttributes.System))
                {
                    return;
                }

                if (fi.LastWriteTimeUtc == File.GetLastWriteTimeUtc(fi.FullName) && fi.Length == new FileInfo(fi.FullName).Length) // Hasn't changed since loop query
                {
                    executeText += "REM Remove Files Below Acceptable Size Limit" + Environment.NewLine;
                    executeText += UnhideFile(fi.FullName);
                    executeText += "DEL \"" + fi.FullName + "\"" + Environment.NewLine;
                }
            });

            rtbExecuteWindow.Text += executeText;
        }

        private string UnhideFile(string fullFilePath)
        {
            FileInfo fi = new FileInfo(fullFilePath);

            if (!fi.Exists)
                return string.Empty;

            if (fi.Attributes.HasFlag(FileAttributes.Hidden))
            {
                return "ATTRIB -H \"" + fi.FullName + "\"" + Environment.NewLine;
            }
            else
            {
                return string.Empty;
            }
        }

        private string HideFile(string fullFilePath)
        {
            FileInfo fi = new FileInfo(fullFilePath);

            if (!fi.Exists)
                return string.Empty;

            if (!fi.Attributes.HasFlag(FileAttributes.Hidden))
            {
                return "ATTRIB +H \"" + fi.FullName + "\"" + Environment.NewLine;
            }
            else
            {
                return string.Empty;
            }
        }

        private void AddTag()
        {
            string term = GetTerm(SelectedNode);

            RenameItem ri = new RenameItem();
            ri.TextName = term + ".";
            ri.RememberEnabled = false;
            ri.RememberDefaultValue = false;
            ri.ShowDialog();

            if (ri.DialogResult == System.Windows.Forms.DialogResult.OK)
            {
                var files = SelectedNode.Nodes.Cast<TreeNode>().Select(n => IndexedList.IndexedTerms.Find(SelectedNode.Name, n.Text)).ToList();

                foreach (var fi in files)
                {
                    if (IndexExtensions.DefaultIndexExtentions.Contains(fi.Extension, StringComparison.CurrentCultureIgnoreCase))
                    {
                        string name = fi.Name;

                        name = name.Replace(ri.TextName, "", StringComparison.CurrentCultureIgnoreCase);

                        if (name.StartsWith(ri.TextName, StringComparison.CurrentCultureIgnoreCase))
                            name = name.Substring(ri.TextName.Length);

                        name = ri.TextName + name;
                        name = CleanupFileName(name);

                        string newPath = Path.Combine(fi.DirectoryName, name);

                        if (!newPath.Equals(fi.FullName, StringComparison.CurrentCultureIgnoreCase))
                        {
                            rtbExecuteWindow.Text += GetMoveCmd(fi.FullName, new FileInfo(newPath)) + Environment.NewLine;
                        }
                    }
                }
            }
        }

        private string GetRenameCmd(string sourceDir, string destDir)
        {
            string cmd = "";

            if (Directory.Exists(destDir))
            {

            }
            else
            {
                cmd += "REN \"" + sourceDir + "\" \"" + (Path.GetDirectoryName(destDir) == "" ? destDir : Path.GetDirectoryName(destDir)) + "\"";
            }

            return cmd;
        }

        private string GetMoveCmd(string sourceFile, FileInfo destinationFile, bool overwriteIfExistsAndNotSmaller = true)
        {
            string cmd = "";

            if (File.Exists(sourceFile))
            {
                if (!Directory.Exists(destinationFile.Directory.FullName))
                {
                    cmd += "IF NOT EXIST \"" + destinationFile.Directory.FullName + "\" MD \"" + destinationFile.Directory.FullName + "\"" + Environment.NewLine;
                }

                cmd += UnhideFile(sourceFile);
                cmd += "MOVE ";

                if (overwriteIfExistsAndNotSmaller)
                {
                    string newFilePath = destinationFile.FullName;

                    // file already exists at destination and is not smaller, delete source
                    if (!sourceFile.Equals(newFilePath, StringComparison.CurrentCultureIgnoreCase) && File.Exists(newFilePath) && new FileInfo(newFilePath).Length >= new FileInfo(sourceFile).Length)
                    {
                        var fi = new FileInfo(sourceFile);
                        fi.IsReadOnly = false;

                        cmd = "";
                        cmd += UnhideFile(sourceFile);
                        cmd += "REM Duplicate Files, Deleting Smaller File" + Environment.NewLine;
                        cmd += "DEL \"" + sourceFile + "\"";
                        return cmd;
                    }
                    else
                    {
                        cmd += "/Y ";
                    }
                }
                else
                {
                    return "";
                }

                cmd += "\"" + sourceFile + "\" \"" + destinationFile.FullName + "\"";
            }

            return cmd;
        }

        private string GetMoveCmd(string sourceFile, DirectoryInfo destinationFolder, bool overwriteIfExistsAndNotSmaller = true)
        {
            string newFilePath = Path.Combine(destinationFolder.FullName, Path.GetFileName(sourceFile));
            return GetMoveCmd(sourceFile, new FileInfo(newFilePath), overwriteIfExistsAndNotSmaller);
        }

        private string CleanupFileName(string name)
        {
            foreach (string sep in Indexer.IndexSeperators)
            {
                if (name.StartsWith(sep))
                {
                    name = name.Substring(sep.Length);
                }

                if (name.EndsWith(sep))
                {
                    name = name.Substring(0, name.Length - 1);
                }

                if (name.Contains(sep + sep))
                {
                    name = name.Replace(sep + sep, sep);
                }
            }

            return name;
        }

        private string GoogleSearch(string searchTerm)
        {
            string searchUrl = string.Format("http://www.google.com/search?q={0}", searchTerm.Replace(" ", "+"));
            string chromeDir = @"C:\Program Files (x86)\Google\Chrome\Application";
            string args = string.Format("--incognito --homepage \"{0}\"", searchUrl);
            return "CD " + chromeDir + " & " + "chrome.exe " + args;
        }

        private string GetVideoArgs(List<TreeNode> treeNodes)
        {
            if (treeNodes.Count == 0)
            {
                return string.Empty;
            }

            string args = "";

            foreach (TreeNode node in treeNodes)
            {
                var it = IndexedList.IndexedFiles.Find(node);

                if (it == null)
                {
                    it = IndexedList.IndexedTerms.Find(node);
                }

                if (it != null && it.FileExists())
                {
                    args += "\"" + it.FullName + "\" ";
                }
            }

            if (!string.IsNullOrEmpty(args))
            {
                args += "\"" + TheEndPath + "\" ";
            }

            return args;
        }

        private string ConvertVideo(string source, string dest, string format = null, int quality = 20, bool vfr = true, bool sameDates = true)
        {
            if (string.IsNullOrEmpty(format))
                format = ConvertOptions.ConvertToFileExtensionForHandbrake;

            format = format.Replace(".", "");

            if (new FileInfo(source).Length > (1024.0 * 1024.0 * 1024.0 * 2.0)) // 2 GB
            {
                quality--;
            }

            string program = "";

            if (File.Exists(@"C:\Program Files (x86)\Handbrake\HandBrakeCLI.exe"))
            {
                program = "\"" + @"C:\Program Files (x86)\Handbrake\HandBrakeCLI.exe" + "\"";
            }
            else if (File.Exists(@"C:\Program Files\Handbrake\HandBrakeCLI.exe"))
            {
                program = "\"" + @"C:\Program Files\Handbrake\HandBrakeCLI.exe" + "\"";
            }
            else
            {
                MessageBox.Show("Could not find HandBrakeCLI.exe!");
                return null;
            }

            FileInfo fiSource = new FileInfo(source);
            string args = string.Format("-i \"{0}\" -t 1 -c 1 -o \"{1}\" -f {4} -e x264 -q {2} --{3} -a 1 -E av_aac -B 160 -6 dpl2 -R Auto -D 0 --gain=0 --audio-fallback av_aac -x ref=1:weightp=1:subq=2:rc-lookahead=10:trellis=0:8x8dct=0 --verbose=1", source, dest, quality, vfr ? "vfr" : "cfr", format);
            return program + " " + args + Environment.NewLine + string.Format("\"{0}\" setfiletime \"{1}\" \"{2}\" \"{3}\" \"{4}\"", NircmdPath, dest, fiSource.CreationTime.ToString("dd-MM-yyyy HH:mm:ss"), fiSource.LastWriteTime.ToString("dd-MM-yyyy HH:mm:ss"), "now");
            // nircmd.exe setfiletime "d:\test\log1.txt" "03/08/2019 17:00:00" "" "03/08/2019 17:10:00"
            // "27-01-2022 13:47:15"
        }

        private void toolStripMenuItemGoogle_Click(object sender, EventArgs e)
        {
            if (IsParent)
            {
                foreach (TreeNode node in SelectedNode.Nodes)
                {
                    rtbExecuteWindow.Text += GoogleSearch(node.Text) + Environment.NewLine;
                }
            }
            if (IsChild)
            {
                rtbExecuteWindow.Text += GoogleSearch(SelectedNode.Text) + Environment.NewLine;
            }
        }

        private void toolStripMenuItemResetAutoMove_Click(object sender, EventArgs e)
        {
            MoveTrackerList.ResetRecommendedMoveLocation(GetTerm(SelectedNode));
            ReloadList();
        }

        private Tuple<FileInfo, FileInfo> GetConvertParams(TreeNode childNode)
        {
            Tuple<FileInfo, FileInfo> convertParams = null;

            var fi = IndexedList.IndexedTerms.Find(childNode.Parent.Name, childNode.Text);

            if (fi == null || !ConvertOptions.ConvertFileExtensions.Any(n => n.Equals(fi.Extension, StringComparison.CurrentCultureIgnoreCase)))
                return null;

            if (!File.Exists(fi.FullName))
                return null;

            if (fi != null && fi.Extension.ToLower() != ConvertOptions.ConvertToFileExtension)
            {
                var fiNew = new FileInfo(Path.Combine(fi.DirectoryName, Path.GetFileNameWithoutExtension(fi.FullName)) + ConvertOptions.ConvertToFileExtension);

                if (fi.FullName != fiNew.FullName)
                {
                    convertParams = new Tuple<FileInfo, FileInfo>(fi, fiNew);
                }
            }

            return convertParams;
        }

        private void toolStripMenuItemConvert_Click(object sender, EventArgs e)
        {
            bool soloExecute = true;
            bool deleteAfterConvert = true;

            if (IsTopNode || IsParent)
            {
                string batchContents = "@ECHO OFF" + Environment.NewLine;
                string convertContents = "";

                List<Tuple<FileInfo, FileInfo>> convertList = new List<Tuple<FileInfo, FileInfo>>();

                foreach (TreeNode parentNode in SelectedNode.Nodes)
                {
                    if (IsTopNode)
                    {
                        foreach (TreeNode node in parentNode.Nodes)
                        {
                            var convertParams = GetConvertParams(node);

                            if (convertParams != null)
                            {
                                convertList.Add(convertParams);
                            }
                        }
                    }
                    else
                    {
                        var convertParams = GetConvertParams(parentNode);

                        if (convertParams != null)
                        {
                            convertList.Add(convertParams);
                        }
                    }
                }

                // Get distinct
                convertList = convertList.GroupBy(n => n.Item2.FullName).Select(grp => grp.First()).ToList();

                const int minimumSizeToSplitOn = 10;
                const int maximumSizeToSplitOn = 99999;
                int batchSize = (int)(convertList.Count / 3.0);
                bool splitBatchWork = false;

                if (convertList.Count >= minimumSizeToSplitOn && convertList.Count < maximumSizeToSplitOn && AreArgsEmpty(Reader))
                {
                    RenameItem ri = new RenameItem(false);
                    ri.Text = "Files Per Batch (" + convertList.Count + " Total)";
                    ri.TextName = batchSize.ToString();
                    ri.RememberDefaultValue = false;
                    ri.RememberEnabled = false;
                    ri.ButtonText = "OK";

                    if (ri.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        if (int.TryParse(ri.TextName, out batchSize) && batchSize > 0)
                        {
                            splitBatchWork = true;
                        }
                    }
                    else
                    {
                        return;
                    }
                }

                if (convertList.Count > 0)
                {
                    int index = 0;
                    List<string> convertContentsList = new List<string>();

                    foreach (var item in convertList)
                    {
                        item.Item1.IsReadOnly = false;

                        index++;
                        convertContents = "";
                        convertContents += "ECHO." + Environment.NewLine + string.Format("ECHO ***** Starting {0} *****", index) + Environment.NewLine + "ECHO." + Environment.NewLine;
                        convertContents += ConvertVideo(item.Item1.FullName, item.Item2.FullName) + Environment.NewLine + "ECHO." + Environment.NewLine;

                        if (deleteAfterConvert)
                            convertContents += "if exist \"" + item.Item2.FullName + "\" (del \"" + item.Item1.FullName + "\")" + Environment.NewLine + "ECHO." + Environment.NewLine;

                        convertContentsList.Add(convertContents);
                    }

                    if (splitBatchWork)
                    {
                        int counter = 0;
                        convertContents = "";
                        string itemsToConvert = "";

                        while (counter < convertContentsList.Count)
                        {
                            convertContents += convertContentsList[counter];
                            itemsToConvert += "ECHO " + convertList[counter].Item1 + Environment.NewLine;

                            if (counter % batchSize == 0 && counter != 0)
                            {
                                batchContents = "@ECHO OFF" + Environment.NewLine;
                                batchContents += "CD \\" + Environment.NewLine + Environment.NewLine;
                                batchContents += "ECHO Hit any key to begin converting: " + GetTerm(SelectedNode) + " (" + batchSize.ToString() + " total)" + Environment.NewLine;
                                batchContents += itemsToConvert;

                                if (AreArgsEmpty(Reader))
                                    batchContents += "pause" + Environment.NewLine + Environment.NewLine + "ECHO." + Environment.NewLine + "ECHO." + Environment.NewLine;

                                convertContents = batchContents + convertContents;
                                convertContents += "ping 128.0.1.2 -n 5 >NUL";
                                FileInfo fiBatch = new FileInfo(Path.Combine(TempPath, Guid.NewGuid().ToString() + ".bat"));
                                File.WriteAllText(fiBatch.FullName, convertContents + Environment.NewLine);
                                Process p = Process.Start(fiBatch.FullName);
                                convertContents = "";
                                itemsToConvert = "";
                            }

                            counter++;
                        }

                        // process the remaining
                        if (counter % batchSize != 0)
                        {
                            batchContents = "@ECHO OFF" + Environment.NewLine;
                            batchContents += "CD \\" + Environment.NewLine + Environment.NewLine;
                            batchContents += "ECHO Hit any key to begin converting: " + GetTerm(SelectedNode) + " (" + (counter % batchSize).ToString() + " total)" + Environment.NewLine;
                            batchContents += itemsToConvert;

                            if (AreArgsEmpty(Reader))
                                batchContents += "pause" + Environment.NewLine + Environment.NewLine + "ECHO." + Environment.NewLine + "ECHO." + Environment.NewLine;

                            convertContents = batchContents + convertContents;
                            convertContents += "ping 128.0.1.2 -n 5 >NUL";
                            FileInfo fiBatch = new FileInfo(Path.Combine(TempPath, Guid.NewGuid().ToString() + ".bat"));
                            File.WriteAllText(fiBatch.FullName, convertContents + Environment.NewLine);
                            Process p = Process.Start(fiBatch.FullName);
                            convertContents = "";
                            itemsToConvert = "";
                        }
                    }
                    else
                    {
                        batchContents = "@ECHO OFF" + Environment.NewLine;
                        batchContents += "CD \\" + Environment.NewLine + Environment.NewLine;
                        batchContents += "ECHO Hit any key to begin converting: " + GetTerm(SelectedNode) + " (" + convertList.Count.ToString() + " total)" + Environment.NewLine;

                        foreach (var item in convertList)
                        {
                            batchContents += "ECHO " + item.Item1.Name + Environment.NewLine;
                        }

                        if (AreArgsEmpty(Reader))
                            batchContents += "pause" + Environment.NewLine + Environment.NewLine + "ECHO." + Environment.NewLine + "ECHO." + Environment.NewLine;

                        convertContents = batchContents + string.Join("", convertContentsList);
                        convertContents += "ping 128.0.1.2 -n 5 >NUL";
                        FileInfo fiBatch = new FileInfo(Path.Combine(TempPath, Guid.NewGuid().ToString() + ".bat"));
                        File.WriteAllText(fiBatch.FullName, convertContents + Environment.NewLine);
                        Process p = Process.Start(fiBatch.FullName);
                    }

                    this.BringToFront();

                    RemoveItem(SelectedNode);
                }
            }
            else if (IsChild)
            {
                var fi = IndexedList.IndexedTerms.Find(SelectedNode.Parent.Name, SelectedNode.Text);
                string fiNewBase = Path.Combine(fi.DirectoryName, Path.GetFileNameWithoutExtension(fi.FullName));
                var fiNew = new FileInfo(fiNewBase + ConvertOptions.ConvertToFileExtension);

                if (fi.Extension.ToLower() != ConvertOptions.ConvertToFileExtension && fi.FullName != fiNew.FullName && !fiNew.Exists)
                {
                    fi.IsReadOnly = false;

                    if (soloExecute)
                    {
                        string batchContents = "CD \\" + Environment.NewLine + "CLS" + Environment.NewLine;
                        batchContents += ConvertVideo(fi.FullName, fiNew.FullName) + Environment.NewLine;

                        if (deleteAfterConvert)
                            batchContents += "if exist \"" + fiNew.FullName + "\" (del \"" + fi.FullName + "\")" + Environment.NewLine;

                        batchContents += "ping 128.0.1.2 -n 5 >NUL";

                        FileInfo fiBatch = new FileInfo(Path.Combine(TempPath, Guid.NewGuid().ToString() + ".bat"));
                        File.WriteAllText(fiBatch.FullName, batchContents);
                        Process p = Process.Start(fiBatch.FullName);

                        this.BringToFront();
                    }
                    else
                    {
                        rtbExecuteWindow.Text += ConvertVideo(fi.FullName, fiNew.FullName) + Environment.NewLine;
                        rtbExecuteWindow.Text += "REM Deleting Source After File Conversion" + Environment.NewLine;
                        rtbExecuteWindow.Text += "DEL \"" + fi.FullName + "\"" + Environment.NewLine;
                    }

                    RemoveItem(SelectedNode);
                }
            }
        }

        private void cbFilterOnTypes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!IsLoading)
            {
                ReloadList(findSelectedItem: false);
                this.ActiveControl = TvTerms;
            }
        }

        private void cbSort_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!IsLoading)
            {
                IndexedList.IndexedTermsSortType = GetFilterSortType;

                if (GetFilterSortType == SortTypes.SortType.NtoO || GetFilterSortType == SortTypes.SortType.OtoN)
                {
                    IndexedList.IndexedFilesSortType = GetFilterSortType;
                }
                else
                {
                    IndexedList.IndexedFilesSortType = SortTypes.SortType.AtoZ;
                }

                ReloadList(findSelectedItem: false);
                this.ActiveControl = TvTerms;
            }
        }

        private void numFilterOnChildrenLessThan_ValueChanged(object sender, EventArgs e)
        {
            TermOptions.ExcludeRules.IgnoreTermsWithChildrenLessThan = (int)numFilterOnChildrenSize.Value;

            if (!IsLoading)
            {
                ReloadList(false, false);
                this.ActiveControl = TvTerms;
            }
        }

        private void numFilterOnTermLength_ValueChanged(object sender, EventArgs e)
        {
            TermOptions.ExcludeRules.IgnoreTermsWithLengthLessThan = (int)numFilterOnTermLength.Value;

            if (!IsLoading)
            {
                ReloadList(false, false);
                this.ActiveControl = TvTerms;
            }
        }

        private void numFilterOnMinimumSize_ValueChanged(object sender, EventArgs e)
        {
            TermOptions.ExcludeRules.MinimumSizeToIndexInMB = (int)numFilterOnMinimumSize.Value;

            if (!IsLoading)
            {
                ReloadList(false, false);
                this.ActiveControl = TvTerms;
            }
        }

        private void numFilterOnAge_ValueChanged(object sender, EventArgs e)
        {
            TermOptions.ExcludeRules.IgnoreTermsWithAgeGreaterThan = (int)numFilterOnAge.Value;

            if (!IsLoading)
            {
                ReloadList(false, false);
                this.ActiveControl = TvTerms;
            }
        }

        private void btnAutomate_Click(object sender, EventArgs e)
        {
            // Stop automation
            if (AutomationRunning)
            {
                StopFileWatcher();
                StartNormalViewing();
                AutomationRunning = false;
                rtbExecuteWindow.Clear();
                btnAutomate.Text = "Automate";
                Application.DoEvents();
                this.Controls.Cast<Control>().Where(c => c is Button && c != btnAutomate).ToList().ForEach(c => c.Enabled = true);
                Application.DoEvents();
                ReloadList(true, false);
            }
            // Start automation
            else
            {
                StartPrivateViewing();
                AutomationRunning = true;
                btnAutomate.Text = "Stop";
                Application.DoEvents();
                this.Controls.Cast<Control>().Where(c => c is Button && c != btnAutomate).ToList().ForEach(c => c.Enabled = false);
                Application.DoEvents();

                var AutomationTask = Task.Factory.StartNew(() =>
                {
                    StartFileWatcher();
                    bool ForceRun = true;

                    while (AutomationRunning)
                    {
                        if (FilesChangesSeen.Count > 0 || ForceRun)
                        {
                            this.InvokeEx(a => a.rtbExecuteWindow.Clear());

                            FilesChangesSeen.RemoveAll(n => !File.Exists(n));
                            var FilesChangesSeenSnapshot = new List<string>(FilesChangesSeen);
                            var logList = new List<string>();

                            for (int i = 0; i < 8; i++)
                            {
                                logList.Add("REM btnClearTrash_Click");
                                this.InvokeEx(a => a.btnClearTrash_Click(null, null));
                                logList.Add("REM btnAutoFile_Click");
                                this.InvokeEx(a => a.btnAutoFile_Click(null, null));
                                this.InvokeEx(a =>
                                {
                                    if (a.rtbExecuteWindow.Text.Trim().Length == 0)
                                    {
                                        i = int.MaxValue - 1; // Break loop
                                    }
                                    else
                                    {
                                        logList.Add("REM Lines = " + a.rtbExecuteWindow.Lines.Count());
                                        logList.Add("REM btnExecute_Click");
                                        logList.Add("");
                                        a.btnExecute_Click(null, null);
                                    }
                                });
                            }

                            if (FilesChangesSeen.Any(f => IndexExtensions.DefaultIndexExtentions.Contains(Path.GetExtension(f))) || ForceRun)
                            {
                                for (int i = 0; i < 1; i++)
                                {
                                    logList.Add("REM btnRemoveDups_Click");
                                    this.InvokeEx(a => a.btnRemoveDups_Click(null, null));
                                    this.InvokeEx(a =>
                                    {
                                        if (a.rtbExecuteWindow.Text.Trim().Length == 0)
                                        {
                                            i = int.MaxValue - 1; // Break loop
                                        }
                                        else
                                        {
                                            logList.Add("REM btnExecute_Click");
                                            logList.Add("");
                                            a.btnExecute_Click(null, null);
                                        }
                                    });
                                }
                            }
                            else
                            {
                                logList.Add("REM No duplicates to remove");
                            }

                            logList.ForEach(l => this.InvokeEx(a => a.rtbExecuteWindow.Text += l + Environment.NewLine));

                            this.InvokeEx(a =>
                            {
                                a.rtbExecuteWindow.Text += "REM Automation Run: " + DateTime.Now.ToString("hh:mm:ss.fff") + Environment.NewLine;
                                a.rtbExecuteWindow.SelectionStart = a.rtbExecuteWindow.Text.Length;
                                a.rtbExecuteWindow.ScrollToCaret();
                            });

                            lock (FilesChangesSeen)
                            {
                                FilesChangesSeen.RemoveAll(n => FilesChangesSeenSnapshot.Contains(n));
                            }

                            ForceRun = false;

                            int sleepTime = 1 * 10;
                            DateTime stopTime = DateTime.UtcNow.AddSeconds(sleepTime);

                            while (DateTime.UtcNow < stopTime)
                            {
                                if (!AutomationRunning)
                                {
                                    break;
                                }
                                else
                                {
                                    Application.DoEvents();
                                    Thread.Sleep(500);
                                }
                            }
                        }
                    }

                    StopFileWatcher();
                });
            }
        }

        private void StartFileWatcher()
        {
            if (FileWatcher != null)
            {
                FileWatcher.Changed -= FileWatcher_Changed;
            }

            FilesChangesSeen.Clear();

            FileWatcher = new FileSystemWatcher();
            FileWatcher.Path = TorrentPath;
            FileWatcher.Filter = "*.*";
            FileWatcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.CreationTime | NotifyFilters.Size | NotifyFilters.FileName;
            FileWatcher.Created += FileWatcher_Changed;
            FileWatcher.Changed += FileWatcher_Changed;
            FileWatcher.EnableRaisingEvents = true;
        }

        private void FileWatcher_Created(object sender, FileSystemEventArgs e)
        {
            // FilesChangesSeen.Add(e.FullPath);
        }

        private void FileWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            if (TermOptions.IgnoreExtensions.Contains(new FileInfo(e.FullPath).Extension.ToLower()))
            {
                return;
            }

            lock (FilesChangesSeen)
            {
                using (new TimeOperation($"FileWatcher_Changed ({e.ChangeType.ToString()}): " + e.FullPath))
                {
                }

                FilesChangesSeen.Add(e.FullPath);
            }
        }

        private void StopFileWatcher()
        {
            if (FileWatcher != null)
            {
                FileWatcher.Changed -= FileWatcher_Changed;
                FileWatcher = null;
            }

            lock (FilesChangesSeen)
            {
                FilesChangesSeen.Clear();
            }
        }

        private void btnGlobalReplace_Click(object sender, EventArgs e)
        {
            RenameItem rnSearchFor = new RenameItem();
            rnSearchFor.Text = "Old String";
            rnSearchFor.TextName = "";
            rnSearchFor.RememberDefaultValue = false;
            rnSearchFor.RememberEnabled = false;
            rnSearchFor.ButtonText = "OK";

            RenameItem rnReplaceWith = new RenameItem(true);
            rnReplaceWith.Text = "New String";
            rnReplaceWith.TextName = "";
            rnReplaceWith.RememberDefaultValue = false;
            rnReplaceWith.RememberEnabled = true;
            rnReplaceWith.ButtonText = "OK";

            List<string> filesToChange = new List<string>();

            if (rnSearchFor.ShowDialog() == System.Windows.Forms.DialogResult.OK && rnReplaceWith.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (rnReplaceWith.Remember)
                    RenameTrackerList.Add(rnSearchFor.TextName, rnReplaceWith.TextName, false);

                foreach (TreeNode parentNode in TopNode.Nodes)
                {
                    foreach (TreeNode node in parentNode.Nodes)
                    {
                        var it = IndexedList.IndexedFiles.Find(node);

                        if (it == null)
                        {
                            it = IndexedList.IndexedTerms.Find(node);
                        }

                        if (it != null && it.FileExists() && it.Name.Contains(rnSearchFor.TextName, StringComparison.CurrentCultureIgnoreCase))
                        {
                            string newFullName = Path.Combine(it.DirectoryName, it.Name.Replace(rnSearchFor.TextName, rnReplaceWith.TextName, StringComparison.CurrentCultureIgnoreCase));

                            if (it.FullName != newFullName)
                            {
                                filesToChange.Add(GetMoveCmd(it.FullName, new FileInfo(newFullName)));
                            }
                        }
                    }
                }

                //foreach (var item in IndexedList.IndexedFiles)
                //{
                //    if (item.File.Name.Contains(rnSearchFor.TextName, StringComparison.CurrentCultureIgnoreCase) && File.Exists(item.File.FullName))
                //    {
                //        string newFullName = Path.Combine(item.File.DirectoryName, item.File.Name.Replace(rnSearchFor.TextName, rnReplaceWith.TextName, StringComparison.CurrentCultureIgnoreCase));

                //        if (item.File.FullName != newFullName)
                //        {
                //            filesToChange.Add(GetMoveCmd(item.File.FullName, new FileInfo(newFullName)));
                //        }
                //    }
                //}
            }

            if (filesToChange.Count > 0)
            {
                foreach (string cmd in filesToChange)
                {
                    rtbExecuteWindow.Text += cmd + Environment.NewLine;
                }
            }
        }

        private void btnResetHashes_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure?", "Reset Hashes", MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
            {
                HashTrackerList = new HashTrackerList();
            }

            // HashTrackerList.SaveToFile(HashTrackingFile, new HashTrackerList());
        }

        private void contextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            SetTreeNodeUIOptions();
        }

        private void termMustStartToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var term = IndexedList.IndexedTerms.Find(SelectedNode.Tag.ToString());

            if (term != null)
            {
                var ri = RenameTrackerList.GetItem(term.Term);

                if (ri != null)
                {
                    termMustStartToolStripMenuItem.Checked = !termMustStartToolStripMenuItem.Checked;
                    ri.RequireTermAtStart = termMustStartToolStripMenuItem.Checked;
                    RenameTrackerList.SaveToFile(RenameTrackingFile, RenameTrackerList);
                }
            }
        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            MoveTrackerList.SaveToFile(MoveTrackingFile, MoveTrackerList);
            RenameTrackerList.SaveToFile(RenameTrackingFile, RenameTrackerList);
            HashTrackerList.SaveToFile(HashTrackingFile, HashTrackerList);
            IgnoreTrackerList.SaveToFileInst(IgnoreTrackingFile);
            InfoTrackerList.SaveToFileInst(InfoTrackingFile);
        }

        private void Main_FormClosed(object sender, FormClosedEventArgs e)
        {
        }

        private void Main_Resize(object sender, EventArgs e)
        {
        }

        private void numFilterOnMinimumSize_DoubleClick(object sender, EventArgs e)
        {
            MessageBox.Show("Filter On Minimum Size", "Tooltip");
        }

        private void numFilterOnTermLength_DoubleClick(object sender, EventArgs e)
        {
            MessageBox.Show("Filter On Character Length", "Tooltip");
        }

        private void numFilterOnChildrenSize_DoubleClick(object sender, EventArgs e)
        {
            MessageBox.Show("Filter On Child Count", "Tooltip");
        }

        private void numFilterOnAge_DoubleClick(object sender, EventArgs e)
        {
            MessageBox.Show("Filter On Age", "Tooltip");
        }

        private void StartNormalViewing()
        {
            if (this.Height != this.NormalViewingHeight)
            {
                this.Height = this.NormalViewingHeight;
            }

            this.PrivateViewing = false;
        }

        private void StartPrivateViewing()
        {
            if (this.Height != this.PrivateViewingHeight)
            {
                this.Height = this.PrivateViewingHeight;
            }

            this.PrivateViewing = true;
        }

        private List<System.Management.Automation.PSObject> RunPowershellScript(string scriptPath)
        {
            System.Management.Automation.Runspaces.RunspaceConfiguration runspaceConfiguration = System.Management.Automation.Runspaces.RunspaceConfiguration.Create();
            
            using (System.Management.Automation.Runspaces.Runspace runspace = System.Management.Automation.Runspaces.RunspaceFactory.CreateRunspace(runspaceConfiguration))
            {
                runspace.Open();

                using (System.Management.Automation.Runspaces.Pipeline pipeline = runspace.CreatePipeline())
                {
                    // Here's how you add a new script with arguments
                    System.Management.Automation.Runspaces.Command myCommand = new System.Management.Automation.Runspaces.Command(scriptPath); //, true, true);

                    System.Management.Automation.Runspaces.CommandParameter dirParam = new System.Management.Automation.Runspaces.CommandParameter("Directory", NzbPath);
                    myCommand.Parameters.Add(dirParam);

                    pipeline.Commands.Add(myCommand);

                    // Execute PowerShell script
                    var results = pipeline.Invoke().ToList();
                    return results;
                }
            }

        }
    }
}
