using System.Collections.Generic;
using System.Linq;

namespace ApexDiff.Data
{
    public class DirectoryTreeNode : ITreeNode
    {
        public string Name { get; init; }

        public DiffState DiffState
        {
            get
            {
                if (Files.Count > 0)
                {
                    if (Files.Count(c => c.DiffState == DiffState.Added) == Files.Count)
                    {
                        return DiffState.Added;
                    }

                    if (Files.Count(c => c.DiffState == DiffState.Deleted) == Files.Count)
                    {
                        return DiffState.Deleted;
                    }

                    if (Files.Count(c => c.DiffState == DiffState.Modified) > 0)
                    {
                        return DiffState.Modified;
                    }
                }

                if (Directories.Count(c => c.DiffState != DiffState.None) > 0)
                {
                    return DiffState.Modified;
                }

                return DiffState.None;
            }
        }

        public IList<FileTreeNode> Files { get; } = new List<FileTreeNode>();

        public IList<DirectoryTreeNode> Directories { get; } = new List<DirectoryTreeNode>();

        public List<ITreeNode> Children
        {
            get
            {
                var flatList = new List<ITreeNode>(Directories);
                flatList.AddRange(Files);

                return flatList;
            }
        }
    }
}
