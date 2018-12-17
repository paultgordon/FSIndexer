using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FSIndexer
{
    public interface IHashable
    {
        string ToHashString();
    }

    public interface IHashable<in T>
    {
        string ToHashString();
    }
}
