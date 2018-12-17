using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;

namespace FSIndexer
{
    [Serializable()]
    public class TrackerList<T> where T : IHashable, new()
    {
        [XmlIgnore]
        public bool OrderByDescending = true;
        public List<T> List { get; set; }
        private Dictionary<string, T> DictionaryList { get; set; }

        public TrackerList()
        {
            List = new List<T>();
            DictionaryList = new Dictionary<string, T>();
        }

        public void SyncListToDictionary()
        {
            DictionaryList.Clear();

            foreach (var item in List)
            {
                this.Add(item);
            }
        }

        public void SyncDictionaryToList()
        {
            List = GetList();
        }

        public List<T> GetList()
        {
            lock (DictionaryList)
            {
                var list = DictionaryList.AsEnumerable();

                if (OrderByDescending)
                {
                    list = list.OrderByDescending(n => n.Value.ToString());
                }
                else
                {
                    list = list.OrderBy(n => n.Value.ToString());
                }

                return list.Select(n => n.Value).ToList();
            }
        }

        private string GetHashKey(string str)
        {
            return System.Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(str.ToLower()));
        }

        private string GetHashKey(T item)
        {
            return System.Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(item.ToHashString().ToLower()));
        }

        public void Add(T item)
        {
            string hashKey = GetHashKey(item);

            lock (DictionaryList)
            {
                if (!DictionaryList.ContainsKey(hashKey))
                {
                    DictionaryList.Add(hashKey, item);
                }
                else
                {
                    DictionaryList[hashKey] = item;
                }
            }
        }

        public bool Contains(string item)
        {
            return GetItem(item) != null;
        }

        public T GetItem(string item)
        {
            string hashKey = GetHashKey(item);

            lock (DictionaryList)
            {
                if (DictionaryList.ContainsKey(hashKey))
                    return DictionaryList[hashKey];
                else
                    return default(T);
            }
        }

        //public void Reset(string sourceString)
        //{
        //    if (GetItem(sourceString) != null)
        //    {
        //        lock (DictionaryList)
        //        {
        //            DictionaryList.Remove(GetHashKey(sourceString));
        //        }
        //    }
        //}

        public static TrackerList<T> LoadFromFile(string file, bool orderByDescending = true)
        {
            TrackerList<T> tl = null;

            if (!File.Exists(file))
            {
                tl = new TrackerList<T>();
            }
            else
            {
                tl = TrackerList<T>.Deserialize(file);
            }

            tl.OrderByDescending = orderByDescending;
            return tl;
        }

        public void SaveToFileInst(string file)
        {
            Serialize(file, this);
        }

        public void SaveToFileInst(string file, TrackerList<T> mtl)
        {
            Serialize(file, mtl);
        }

        public static void SaveToFile(string file, TrackerList<T> mtl)
        {
            Serialize(file, mtl);
        }

        // File Write
        private static void Serialize(string file, TrackerList<T> mtl)
        {
            string tmpFile = file + ".tmp";

            try
            {
                mtl.SyncDictionaryToList();

                Stream streamWrite = File.Create(tmpFile);
                System.Xml.Serialization.XmlSerializer xs = new System.Xml.Serialization.XmlSerializer(mtl.GetType());
                xs.Serialize(streamWrite, mtl);
                streamWrite.Close();

                if (File.Exists(file))
                    File.Delete(file);

                File.Move(tmpFile, file);
            }
            catch
            {
                throw;
            }
        }

        // File Read
        private static TrackerList<T> Deserialize(string file)
        {
            try
            {
                Stream streamRead = File.OpenRead(file);
                System.Xml.Serialization.XmlSerializer xs = new System.Xml.Serialization.XmlSerializer(typeof(TrackerList<T>));
                TrackerList<T> mtl = (TrackerList<T>)xs.Deserialize(streamRead);

                mtl.SyncListToDictionary();
                streamRead.Close();
                return mtl;
            }
            catch
            {
                throw;
            }
        }
    }
}
