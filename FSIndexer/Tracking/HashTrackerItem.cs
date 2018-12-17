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
        [XmlIgnore]
        public string Path { get; set; } = string.Empty;
        public long Length { get; set; } = -1;

        private string _shortHash { get; set; } = string.Empty;
        private string _longHash { get; set; } = string.Empty;

        public string EncryptedPath
        {
            get
            {
                return EncodeTo64(Path);
            }
            set
            {
                Path = DecodeFrom64(value);
            }
        }

        public string ShortHash
        {
            get
            {
                if (string.IsNullOrEmpty(_shortHash))
                {
                    _shortHash = Main.GetMD5Hash(new FileInfo(Path), Main.HashSizeShort);
                }

                return _shortHash;
            }
            set
            {
                _shortHash = value;
            }
        }

        public string LongHash
        {
            get
            {
                if (string.IsNullOrEmpty(_longHash) && HashTrackerList.AutoCreateLongHash)
                {
                    _longHash = Main.GetMD5Hash(new FileInfo(Path), Main.HashSizeLong);
                }

                return _longHash;
            }
            set
            {
                _longHash = value;
            }
        }

        public HashTrackerItem()
        {

        }

        public HashTrackerItem(FileInfo fi)
        {
            Path = fi.FullName;
            Length = fi.Length;
            var fsh = ShortHash;
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
