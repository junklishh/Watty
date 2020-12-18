using ApexDiff.Models;
using ApexVpkInventory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace ApexDiff.Data
{
    public class AssetInventoryRepository
    {
        private HttpClient Http { get; }

        public AssetInventoryRepository(HttpClient httpClient)
        {
            // Http = clientFactory.CreateClient("DefaultClient");
            Http = httpClient;
        }

        public IReadOnlyList<AssetInventory> GetInventories()
        {
            return new List<AssetInventory>()
            {
                new AssetInventory("Apex Legends", "v3.0.3.106", "inventories/Apex Legends v3.0.3.105.json"),
                new AssetInventory("Apex Legends", "v3.0.3.105", "inventories/Apex Legends v3.0.3.105.json"),
                new AssetInventory("Titanfall 2", "v2.0.11.0", "inventories/Titanfall 2 v2.0.11.0.json"),
            };
        }

        public async Task<TreeNode> GetInventoryFileTreeAsync(AssetInventory inventory, CancellationToken cancellationToken = default)
        {
            if (inventory == null)
            {
                throw new ArgumentException("AssetInventory cannot be null", nameof(inventory));
            }

            try
            {
                var vpkInventory = await Http.GetFromJsonAsync<VpkInventory>(inventory.Url, cancellationToken);

                return await GetVpkInventoryFileTreeAsync(vpkInventory, cancellationToken);
            }
            catch (OperationCanceledException canceledException) when (canceledException.CancellationToken == cancellationToken)
            {
                return TreeNode.CreateDirectoryNode("Root");
            }
        }

        private async Task<TreeNode> GetVpkInventoryFileTreeAsync(VpkInventory vpkInventory, CancellationToken cancellationToken = default)
        {
            var rootNode = TreeNode.CreateDirectoryNode("Root");

            foreach (var package in vpkInventory.Packages)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var packageNode = TreeNode.CreateDirectoryNode(package.Filename);

                foreach (var entry in package.Entries)
                {
                    var currentDirectory = packageNode;
                    var pathParts = entry.Filename.Split('/');

                    // Last part is always filename
                    for (int i = 0; i < pathParts.Length - 1; i++)
                    {
                        var childDirectory = currentDirectory.Children.SingleOrDefault(d => d.Name == pathParts[i]);

                        if (childDirectory == null)
                        {
                            childDirectory = TreeNode.CreateDirectoryNode(pathParts[i]);
                            currentDirectory.Children.Add(childDirectory);
                        }

                        currentDirectory = childDirectory;
                    }

                    currentDirectory.Children.Add(TreeNode.CreateFileNode(pathParts.Last(), entry.Sha256));
                }

                rootNode.Children.Add(packageNode);
            }

            return rootNode;
        }
    }
}
