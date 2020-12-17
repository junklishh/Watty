namespace ApexDiff.Models
{
    public class PackageEntry
    {
        public PackageEntry(string name, string hash)
        {
            Name = name;
            Hash = hash;
        }

        public string Name { get; private set; }

        public string Hash { get; private set; }
    }
}
