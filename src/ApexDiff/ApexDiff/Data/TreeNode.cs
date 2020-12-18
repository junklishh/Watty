using System.Collections.Generic;

namespace ApexDiff.Data
{
    public class TreeNode
    {
        public string Name { get; init; } = string.Empty;

        public string Hash { get; init; } = string.Empty;

        public List<TreeNode> Children { get; } = new List<TreeNode>();

        public static TreeNode CreateDirectoryNode(string directoryName)
        {
            return new TreeNode()
            {
                Name = directoryName,
            };
        }

        public static TreeNode CreateFileNode(string fileName, string hash)
        {
            return new TreeNode()
            {
                Name = fileName,
                Hash = hash,
            };
        }
    }
}
