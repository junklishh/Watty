namespace ApexDiff.Models
{
    public class Package
    {
        public Package(string name, PackageDirectory rootDirectory)
        {
            Name = name;
            RootDirectory = rootDirectory;
        }

        public string Name { get; private set; }

        public PackageDirectory RootDirectory { get; private set; }
    }
}
