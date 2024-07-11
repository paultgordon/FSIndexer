using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace FSIndexer
{
    public class IndexExtensions : List<string>
    {
        public static List<string> DefaultIndexExtentions
        {
            get
            {
                var list = new List<string>();

                list.Add(".avi");
                list.Add(".flv");
                list.Add(".iso");
                list.Add(".mkv");
                list.Add(".mp4");
                list.Add(".mpg");
                list.Add(".mpeg");
                list.Add(".wmv");
                list.Add(".f4v");
                list.Add(".mov");
                list.Add(".ts");
                list.Add(".divx");
                list.Add(".m4v");
                list.Add(".webm");
                list.Add(".vob");
                list.Add(".vid");

                return list;
            }
        }

        public IndexExtensions()
            : base()
        {
            this.AddRange(DefaultIndexExtentions);
        }

        public new void Add(string item)
        {
            lock (this)
            {
                base.Add(Format(item));
            }

        }

        public new bool Contains(string item)
        {
            lock (this)
            {
                return base.Contains(Format(item));
            }
        }

        public bool Contains(FileInfo fi)
        {
            return this.Contains(fi.Extension);
        }

        private string Format(string item)
        {
            if (!item.StartsWith("."))
                item += "." + item;

            return item.ToLower().Trim();
        }

        public List<FileInfo> FindExtensionMatches(DirectoryInfoExtended die)
        {
            var list = new List<FileInfo>();

            if (!die.DirectoryInfo.Exists)
                return list;

            using (new TimeOperation("GetFiles Operation"))
            {
                list.AddRange(die.GetFiles(false).Where(n => this.Contains(n)));
            }

            return list.Where(n => n != null).ToList();
        }
    }
}
