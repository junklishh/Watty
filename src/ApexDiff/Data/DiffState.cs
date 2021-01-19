using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApexDiff.Data
{
    public enum DiffState
    {
        None = 0,
        Added = 1,
        Modified = 2,
        Deleted = 3,
    }
}
