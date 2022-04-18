using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSIndexer
{
    [Serializable()]
    public class HashTrackerList2
    {
        public List<HashTrackerSubList2> List { get; set; }
        public static bool AutoCreateLongHash = true;

        public HashTrackerList2()
        {
            List = new List<HashTrackerSubList2>();
        }

        public static HashTrackerList2 LoadFromFile(string file)
        {
            if (!File.Exists(file))
                return new HashTrackerList2();
            else
                return HashTrackerList2.Deserialize(file);
        }

        public static void SaveToFile(string file, HashTrackerList2 mtl)
        {
            AutoCreateLongHash = false;
            Serialize(file, mtl);
            AutoCreateLongHash = true;
        }

        private static void Serialize(string file, HashTrackerList2 mtl)
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

        private static HashTrackerList2 Deserialize(string file)
        {
            try
            {
                using (Stream streamRead = File.OpenRead(file))
                {
                    System.Xml.Serialization.XmlSerializer xs = new System.Xml.Serialization.XmlSerializer(typeof(HashTrackerList));
                    HashTrackerList2 obj = (HashTrackerList2)xs.Deserialize(streamRead);
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
