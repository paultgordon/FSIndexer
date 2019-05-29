using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace FSIndexer
{
    [Serializable()]
    public class MoveTrackerList
    {
        public List<MoveTrackerItem> List { get; set; }

        public MoveTrackerList()
        {
            List = new List<MoveTrackerItem>();
        }

        public void Add(MoveTrackerItem item)
        {
            var found = List.FirstOrDefault(n => n.Equals(item));

            if (found == null)
            {
                List.Add(item);
            }
            else
            {
                found.Strength++;
            }
        }

        public void Add(string tag, string sourcedir, string destdir)
        {
            Add(new MoveTrackerItem(tag, sourcedir, destdir));
        }

        public bool MoveLocationExists(string tag)
        {
            return List.Any(n => n.Tag.Equals(tag, StringComparison.CurrentCultureIgnoreCase));
        }

        public MoveTrackerItem GetRecommendedMoveLocation(string tag, string sourcedir)
        {
            if (sourcedir.Contains(Main.TorrentPath))
            {
                return (from item in List
                        where item.Tag.Equals(tag, StringComparison.CurrentCultureIgnoreCase)
                          && !item.SourceDir.Contains(Main.KeepDirectory)
                        orderby item.Strength descending
                        select item).FirstOrDefault();
            }
            else
            {
                return (from item in List
                        where sourcedir.Equals(item.SourceDir, StringComparison.CurrentCultureIgnoreCase)
                          && item.Tag.Equals(tag, StringComparison.CurrentCultureIgnoreCase)
                        orderby item.Strength descending
                        select item).FirstOrDefault();
            }
        }

        public MoveTrackerItem GetRecommendedMoveLocation(string tag, List<string> sourcedir)
        {
            if (sourcedir.Any(n => n.Contains(Main.TorrentPath)))
            {
                return (from item in List
                        where item.Tag.Equals(tag, StringComparison.CurrentCultureIgnoreCase)
                          && !item.SourceDir.Contains(Main.KeepDirectory)
                        orderby item.Strength descending
                        select item).FirstOrDefault();
            }
            else
            {
                return (from item in List
                        where sourcedir.Contains(item.SourceDir, StringComparison.CurrentCultureIgnoreCase)
                          && item.Tag.Equals(tag, StringComparison.CurrentCultureIgnoreCase)
                        orderby item.Strength descending
                        select item).FirstOrDefault();
            }
        }

        public void ResetRecommendedMoveLocation(string tag)
        {
            List.RemoveAll(n => n.Tag.Equals(tag, StringComparison.CurrentCultureIgnoreCase));
        }

        public static MoveTrackerList LoadFromFile(string file)
        {
            if (!File.Exists(file))
                return new MoveTrackerList();
            else
                return MoveTrackerList.Deserialize(file);
        }

        public static void SaveToFile(string file, MoveTrackerList mtl)
        {
            mtl.List.RemoveAll(n => n.DestinationDir == n.SourceDir);
            Serialize(file, mtl);
        }

        private static void Serialize(string file, MoveTrackerList mtl)
        {
            //string tmpFile = file + ".tmp";

            string tmpFile = Path.GetTempFileName();

            try
            {
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

        private static MoveTrackerList Deserialize(string file)
        {
            try
            {
                Stream streamRead = File.OpenRead(file);
                System.Xml.Serialization.XmlSerializer xs = new System.Xml.Serialization.XmlSerializer(typeof(MoveTrackerList));
                MoveTrackerList obj = (MoveTrackerList)xs.Deserialize(streamRead);
                obj.Sort();
                streamRead.Close();
                return obj;
            }
            catch
            {
                // File.Delete(file);
                throw;
            }
        }

        public void Sort()
        {
            this.List.Sort();
        }
    }
}
