using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSIndexer
{
    [Serializable()]
    public class HashTrackerList
    {
        public Dictionary<string, List<HashTrackerItem>> Dictionary;
        public List<HashTrackerItem> List { get; set; }
        public static bool AutoCreateLongHash = true;

        public HashTrackerList()
        {
            Dictionary = new Dictionary<string, List<HashTrackerItem>>();
            List = new List<HashTrackerItem>();
        }

        public void Add(HashTrackerItem item)
        {
            lock (List)
            {
                var found = List.FirstOrDefault(n => n.Equals(item));

                if (found == null)
                {
                    List.Add(item);
                }
            }
        }

        public void Add(FileInfo fi)
        {
            Add(new HashTrackerItem(fi));
        }

        public bool Contains(FileInfo fi)
        {
            lock (List)
            {
                return List.Any(n => n.Path == fi.FullName && n.Length == fi.Length);
            }
        }

        public HashTrackerItem GetItem(FileInfo fi)
        {
            lock (List)
            {
                HashTrackerItem item = List.SingleOrDefault(n => n.Path == fi.FullName && n.Length == fi.Length);

                if (item != null)
                {
                    return item;
                }
                else
                {
                    item = new HashTrackerItem(fi);
                    List.Add(item);
                    return item;
                }
            }
        }

        public static HashTrackerList LoadFromFile(string file)
        {
            if (!File.Exists(file))
                return new HashTrackerList();
            else
                return HashTrackerList.Deserialize(file);
        }

        public static void SaveToFile(string file, HashTrackerList mtl)
        {
            AutoCreateLongHash = false;
            Serialize(file, mtl);
            AutoCreateLongHash = true;
        }

        private static void Serialize(string file, HashTrackerList mtl)
        {
            string tmpFile = file + ".tmp";

            try
            {
                using (Stream streamWrite = File.Create(tmpFile))
                {
                    System.Xml.Serialization.XmlSerializer xs = new System.Xml.Serialization.XmlSerializer(mtl.GetType());
                    xs.Serialize(streamWrite, mtl);
                }

                if (File.Exists(file))
                    File.Delete(file);

                File.Move(tmpFile, file);
            }
            catch
            {
                throw;
            }
        }

        private static HashTrackerList Deserialize(string file)
        {
            try
            {
                using (Stream streamRead = File.OpenRead(file))
                {
                    System.Xml.Serialization.XmlSerializer xs = new System.Xml.Serialization.XmlSerializer(typeof(HashTrackerList));
                    HashTrackerList obj = (HashTrackerList)xs.Deserialize(streamRead);
                    obj.Sort();
                    return obj;
                }
            }
            catch
            {
                throw;
            }
        }

        public void Sort()
        {
            this.List.Sort();
        }
    }
}
