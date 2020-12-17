using System.Collections.Generic;

namespace ApexDiff.Models
{
    public class PackageDirectory
    {
        public PackageDirectory(string name)
        {
            Name = name;
        }

        public string Name { get; private  set; }

        public List<PackageDirectory> Directories { get; private set; } = new List<PackageDirectory>();

        public List<PackageEntry> Files { get; private set; } = new List<PackageEntry>();
    }
}
