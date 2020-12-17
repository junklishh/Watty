using System.Collections.Generic;

namespace ApexVpkInventory
{
    public class VpkInventory
    {
        public GameInfo GameInfo { get; set; } = new GameInfo();

        public List<VpkFile> Packages { get; set; } = new List<VpkFile>();
    }
}
