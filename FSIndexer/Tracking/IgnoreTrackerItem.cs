using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FSIndexer
{
    [Serializable()]
    public class IgnoreTrackerItem : IComparable<IgnoreTrackerItem>, IHashable
    {
        public string Tag { get; set; }

        public IgnoreTrackerItem()
        {
        }

        public IgnoreTrackerItem(string tag)
        {
            Tag = tag;
        }

        public bool Equals(IgnoreTrackerItem obj)
        {
            return obj.Tag.Equals(this.Tag, StringComparison.CurrentCultureIgnoreCase);
        }

        public int CompareTo(IgnoreTrackerItem other)
        {
            return other.ToString().CompareTo(this.ToString());
        }

        public override string ToString()
        {
            return this.Tag;
        }

        public string ToHashString()
        {
            return this.ToString();
        }
    }
}
