using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace FSIndexer
{
    public class IndexSeperators : List<string>
    {
        public const string PrimarySeperator = ".";

        public static List<string> DefaultIndexSeperators
        {
            get
            {
                var list = new List<string>();

                list.Add(".");
                list.Add("-");
                list.Add("_");
                list.Add(" ");
                list.Add("(");
                list.Add(")");
                list.Add("]");
                list.Add("[");

                return list;
            }
        }

        public IndexSeperators()
            : base()
        {
            this.AddRange(DefaultIndexSeperators);
        }

        public new void Add(string item)
        {
            if (!Contains(item))
            {
                base.Add(Format(item));
            }
        }

        public new bool Contains(string item)
        {
            return base.Contains(Format(item));
        }

        private string Format(string item)
        {
            return item.ToLower();
        }

        public List<string> Seperate(FileInfo fi)
        {
            List<string> list = new List<string>();

            string filenameWithoutExt = Path.GetFileNameWithoutExtension(fi.FullName);

            filenameWithoutExt = TermOptions.InsertSeperatorBetweenNumbers(filenameWithoutExt);

            list.AddRange(filenameWithoutExt.Split(this.ToArray(), StringSplitOptions.RemoveEmptyEntries));

            return list;
        }
    }
}
