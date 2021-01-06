using System;

namespace ApexDiff.Data
{
    public class FileTreeNode : ITreeNode
    {
        public string Name { get; init; }

        public DiffState DiffState { get; init; } = DiffState.None;

        public string Hash { get; init; }
    }
}
