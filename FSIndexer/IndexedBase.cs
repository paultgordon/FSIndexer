using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FSIndexer
{
    public class IndexedBase
    {
        public string ID { get; private set; }
        public bool Enabled { get { return !PermanentlyDisabled && !TemporarilyDisabled; } }
        public bool PermanentlyDisabled { get; set; }
        public bool TemporarilyDisabled { get; set; }

        public IndexedBase()
        {
            ID = Guid.NewGuid().ToString().ToUpper();
            TemporarilyDisabled = false;
            PermanentlyDisabled = false;
        }
    }
}
