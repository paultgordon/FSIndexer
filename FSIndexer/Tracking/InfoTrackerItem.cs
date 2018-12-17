using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FSIndexer
{
    [Serializable()]
    public class InfoTrackerItem : IComparable<InfoTrackerItem>, IHashable
    {
        public const int RatingBlank = 0;
        public const int RatingMin = 1;
        public const int RatingMax = 9;

        public string Term { get; set; }
        public string Note { get; set; }
        public int Rating { get; set; }

        public InfoTrackerItem()
        {
        }

        public InfoTrackerItem(string tag, string note = "", int rating = RatingBlank)
        {
            Term = tag;
            Note = note;
            Rating = rating;
        }

        public bool Equals(InfoTrackerItem obj)
        {
            return obj.Term.Equals(this.Term, StringComparison.CurrentCultureIgnoreCase);
        }

        public int CompareTo(InfoTrackerItem other)
        {
            return other.ToString().CompareTo(this.ToString());
        }

        public override string ToString()
        {
            return this.Term + (string.IsNullOrEmpty(Note) ? "" : " - " + Note) + (Rating == 0 ? "" : " - R" + Rating);
        }

        public string ToHashString()
        {
            return this.Term;
        }
    }
}
