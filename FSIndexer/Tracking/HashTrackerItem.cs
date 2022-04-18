using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace FSIndexer
{
    [Serializable()]
    public class HashTrackerItem : IComparable<HashTrackerItem>
    {
        public string Path { get; set; } = string.Empty;
        public string Name
        {
            get
            {
                return System.IO.Path.GetFileName(Path);
            }
        }

        // public string EncryptedPath { get; set; } = string.Empty;
        public long Length { get; set; } = -1;
        internal DateTime DateCreated { get; set; } = DateTime.MinValue;
        public DateTime DateModified { get; set; } = DateTime.MinValue;
        public string ShortHash { get; set; } = string.Empty;

        public HashTrackerItem()
        {
        }

        public HashTrackerItem(FileInfo fi)
        {
            Path = fi.FullName;
            Length = fi.Length;
            DateCreated = fi.CreationTimeUtc;
            DateModified = fi.LastWriteTimeUtc;
            ShortHash = Main.GetMD5Hash(new FileInfo(Path), Main.HashSizeShort);
        }

        public int CompareTo(HashTrackerItem other)
        {
            return other.GetHashCode().CompareTo(this.GetHashCode());
        }

        private static string EncodeTo64(string toEncode)
        {
            return System.Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(toEncode));
        }

        private static string DecodeFrom64(string encodedData)
        {
            return System.Text.ASCIIEncoding.ASCII.GetString(System.Convert.FromBase64String(encodedData));
        }
    }
}