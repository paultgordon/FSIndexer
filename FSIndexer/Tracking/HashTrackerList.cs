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
        private Dictionary<string, List<HashTrackerItem>> Dictionary;
        public List<HashTrackerItem> List { get; set; }
        public static bool AutoCreateLongHash = true;

        public HashTrackerList()
        {
            Dictionary = new Dictionary<string, List<HashTrackerItem>>();
            List = new List<HashTrackerItem>();
        }

        public void Add(HashTrackerItem item)
        {
            lock (Dictionary)
            {
                if (Dictionary.ContainsKey(item.Path))
                {
                    var found = Dictionary[item.Path].FirstOrDefault(n => n.Equals(item));

                    if (found == null)
                    {
                        Dictionary[item.Path].Add(item);
                    }
                }
                else
                {
                    Dictionary.Add(item.Path, new List<HashTrackerItem>() { item });
                }
            }
        }

        public void Add(FileInfo fi)
        {
            Add(new HashTrackerItem(fi));
        }

        public bool Contains(FileInfo fi)
        {
            lock (Dictionary)
            {
                return Dictionary.ContainsKey(fi.FullName) && Dictionary[fi.FullName].Any(n => n.Length == fi.Length);
            }
        }

        public List<HashTrackerItem> GetItems(FileInfo fi)
        {
            lock (Dictionary)
            {
                if (!Dictionary.ContainsKey(fi.FullName))
                {
                    Dictionary.Add(fi.FullName, new List<HashTrackerItem>() { new HashTrackerItem(fi) });
                }

                return Dictionary[fi.FullName];
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
                mtl.SyncToDictionary();

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
                    obj.SyncFromList();
                    return obj;
                }
            }
            catch
            {
                throw;
            }
        }

        public void SyncFromList()
        {
            foreach (var item in List)
            {
                if (!Dictionary.ContainsKey(item.Path))
                {
                    Dictionary.Add(item.Path, new List<HashTrackerItem>() { item });
                }
                else
                {
                    Dictionary[item.Path].Add(item);
                }
            }
        }

        public void SyncToDictionary()
        {
            List.Clear();

            foreach (var list in Dictionary)
            {
                foreach (var item in list.Value)
                {
                    List.Add(item);
                }
            }
        }

        public void Sort()
        {
            this.List.Sort();
        }
    }
}
