using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FSIndexer
{
    public static class SortTypes
    {
        public enum SortType { AtoZ=0, ZtoA=1, LtoS=3, StoL=2, NtoO=5, OtoN=4 }
        public static bool IsReverseSortType(SortType sort)
        {
            return ((int)sort) % 2 != 0;
        }
    }
}
