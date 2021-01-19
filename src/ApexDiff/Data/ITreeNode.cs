namespace ApexDiff.Data
{
    public interface ITreeNode
    {
        string Name { get; }

        DiffState DiffState { get; }
    }
}
