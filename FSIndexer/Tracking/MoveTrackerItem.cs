using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FSIndexer
{
    [Serializable()]
    public class MoveTrackerItem : IComparable<MoveTrackerItem>
    {
        public string Tag { get; set; }
        public string SourceDir { get; set; }
        public string DestinationDir { get; set; }
        public int Strength = 1;

        public MoveTrackerItem()
        {
        }

        public MoveTrackerItem(string tag, string sourcedir, string destdir)
        {
            Tag = tag;
            SourceDir = sourcedir;
            DestinationDir = destdir;
        }

        public bool Equals(MoveTrackerItem obj)
        {
            return obj.Tag.Equals(this.Tag, StringComparison.CurrentCultureIgnoreCase) &&
                obj.SourceDir.Equals(this.SourceDir, StringComparison.CurrentCultureIgnoreCase) &&
                obj.DestinationDir.Equals(this.DestinationDir, StringComparison.CurrentCultureIgnoreCase);
        }
        
        public int CompareTo(MoveTrackerItem other)
        {
            return SortToString(other).CompareTo(SortToString(this));
        }

        private static string SortToString(MoveTrackerItem mtl)
        {
            return mtl.Strength.ToString().PadLeft(6, '0') + " " + mtl.Tag + " " + mtl.SourceDir + " " + mtl.DestinationDir;
        }
    }
}
