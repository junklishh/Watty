namespace ApexDiff.Models
{
    public record AssetInventory
    {
        public AssetInventory(string game, string version, string url) => (Game, Version, Url) = (game, version, url);

        public string Game { get; }

        public string Version { get; }

        public string Url { get; }
    }
}
