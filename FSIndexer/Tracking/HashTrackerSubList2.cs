using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSIndexer
{
    public class HashTrackerSubList2 : IComparable<HashTrackerSubList2>
    {
        public List<HashTrackerItem2> List;

        public HashTrackerSubList2()
        {
            List = new List<HashTrackerItem2>();
        }

        public void Add(HashTrackerItem2 item)
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
            Add(new HashTrackerItem2(fi));
        }

        public bool Contains(FileInfo fi)
        {
            lock (List)
            {
                return List.Any(n => n.Path == fi.FullName && n.Length == fi.Length);
            }
        }

        public HashTrackerItem2 GetItem(FileInfo fi)
        {
            lock (List)
            {
                HashTrackerItem2 item = List.SingleOrDefault(n => n.Path == fi.FullName && n.Length == fi.Length);

                if (item != null)
                {
                    return item;
                }
                else
                {
                    item = new HashTrackerItem2(fi);
                    List.Add(item);
                    return item;
                }
            }
        }
    }
}
