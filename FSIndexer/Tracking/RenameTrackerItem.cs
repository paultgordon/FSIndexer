using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FSIndexer
{
    [Serializable()]
    public class RenameTrackerItem : IComparable<RenameTrackerItem>
    {
        public DateTime DateCreated { get; set; }
        public string SourceString { get; set; }
        public string DestinationString { get; set; }
        public bool TagOnly { get; set; }
        public bool RequireTermAtStart { get; set; }

        public RenameTrackerItem()
        {
            TagOnly = true;
            RequireTermAtStart = false;
            DateCreated = DateTime.Now;
        }

        public RenameTrackerItem(string sourceString, string destinationString, bool tagOnly = true, DateTime? dateCreated = null, bool requireTermAtStart = false)
        {
            SourceString = sourceString.ToLower();
            DestinationString = destinationString;
            TagOnly = tagOnly;
            RequireTermAtStart = requireTermAtStart;

            if (dateCreated.HasValue)
            {
                DateCreated = dateCreated.Value;
            }
            else
            {
                DateCreated = DateTime.Now;
            }
        }

        public bool Equals(RenameTrackerItem obj)
        {
            return obj.SourceString.Equals(this.SourceString, StringComparison.CurrentCultureIgnoreCase) &&
                obj.DestinationString.Equals(this.DestinationString, StringComparison.CurrentCultureIgnoreCase) &&
                obj.TagOnly.Equals(this.TagOnly) && obj.DateCreated.Equals(this.DateCreated) &&
                obj.RequireTermAtStart.Equals(this.RequireTermAtStart);
        }

        public int CompareTo(RenameTrackerItem other)
        {
            return other.SortToString().CompareTo(this.SortToString());
        }

        public string SortToString()
        {
            return SourceString + " " + DestinationString;
        }
    }
}
