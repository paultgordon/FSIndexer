using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using System.Diagnostics;

namespace FSIndexer
{
    [Serializable()]
    public class RenameTrackerList
    {
        public List<RenameTrackerItem> List
        {
            get
            {
                return DictionaryList.Select(n => n.Value).ToList();
            }
            set
            {
                DictionaryList = new Dictionary<string, RenameTrackerItem>();

                foreach (var item in value)
                {
                    this.Add(item);
                }
            }
        }

        [XmlIgnore]
        private Dictionary<string, RenameTrackerItem> DictionaryList { get; set; }

        private static DateTime SystemTrackerItemDateTime = new DateTime(2000, 1, 1);

        public RenameTrackerList()
        {
            // List = new List<RenameTrackerItem>();
            DictionaryList = new Dictionary<string, RenameTrackerItem>();
        }

        //public void SyncListToDictionary()
        //{
        //    Debug.Assert(!(DictionaryList.Count > List.Count));

        //    DictionaryList.Clear();

        //    foreach (var item in List)
        //    {
        //        this.Add(item);
        //    }
        //}

        //public void SyncDictionaryToList()
        //{
        //    Debug.Assert(!(List.Count > DictionaryList.Count));

        //    List = GetList();

        //    foreach (var item in TermOptions.AutoReplaceTags)
        //    {
        //        if (List.Any(n => n.SourceString == item.Key && n.DestinationString == item.Value && n.DateCreated != SystemTrackerItemDateTime))
        //        {
        //            List.Single(n => n.SourceString == item.Key && n.DestinationString == item.Value).DateCreated = SystemTrackerItemDateTime;
        //        }
        //    }

        //    List = List.OrderBy(n => n.DateCreated).ThenBy(n => n.SortToString()).ToList();
        //}

        public List<RenameTrackerItem> GetList()
        {
            return DictionaryList.AsEnumerable().OrderBy(n => n.Value.DateCreated).ThenBy(n => n.Value.SortToString()).Select(n => n.Value).ToList();
        }

        private string GetHashKey(string str)
        {
            return System.Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(str.ToLower()));
        }

        public void Add(RenameTrackerItem item)
        {
            if (!string.IsNullOrEmpty(item.SourceString))
            {
                string hashKey = GetHashKey(item.SourceString);

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

        public void Add(string sourceString, string destinationString, bool tagOnly = true, DateTime? dateCreated = null, bool requireTermAtStart = false)
        {
            Add(new RenameTrackerItem(sourceString, destinationString, tagOnly, dateCreated, requireTermAtStart));
        }

        public bool Contains(string sourceString)
        {
            return GetItem(sourceString) != null;
        }

        public RenameTrackerItem GetItem(string sourceString)
        {
            string hashKey = GetHashKey(sourceString);
            return DictionaryList.ContainsKey(hashKey) ? DictionaryList[GetHashKey(sourceString)] : null;
        }

        public void Reset(string sourceString)
        {
            if (GetItem(sourceString) != null)
            {
                DictionaryList.Remove(GetHashKey(sourceString));
            }
        }

        public static RenameTrackerList LoadFromFile(string file)
        {
            if (!File.Exists(file))
            {
                RenameTrackerList rtList = new RenameTrackerList();

                foreach (var item in TermOptions.AutoReplaceTags)
                {
                    rtList.Add(item.Key, item.Value, true, new DateTime?(SystemTrackerItemDateTime));
                }

                return rtList;
            }
            else
                return RenameTrackerList.Deserialize(file);
        }

        public static void SaveToFile(string file, RenameTrackerList mtl)
        {
            Serialize(file, mtl);
        }

        // File Write
        private static void Serialize(string file, RenameTrackerList mtl)
        {
            string tmpFile = file + ".tmp";

            try
            {
                Stream streamWrite = File.Create(tmpFile);
                System.Xml.Serialization.XmlSerializer xs = new System.Xml.Serialization.XmlSerializer(typeof(List<RenameTrackerItem>));
                xs.Serialize(streamWrite, mtl.List);
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
        private static RenameTrackerList Deserialize(string file)
        {
            try
            {
                Stream streamRead = File.OpenRead(file);
                System.Xml.Serialization.XmlSerializer xs = new System.Xml.Serialization.XmlSerializer(typeof(List<RenameTrackerItem>));
                RenameTrackerList mtl = new RenameTrackerList();
                mtl.List = (List<RenameTrackerItem>)xs.Deserialize(streamRead);

                foreach (var item in TermOptions.AutoReplaceTags)
                {
                    if (!mtl.Contains(item.Key))
                    {
                        mtl.Add(item.Key, item.Value, true, new DateTime?(SystemTrackerItemDateTime));
                    }
                }

                streamRead.Close();
                return mtl;
            }
            catch
            {
                // File.Delete(file);
                throw;
            }
        }
    }
}
