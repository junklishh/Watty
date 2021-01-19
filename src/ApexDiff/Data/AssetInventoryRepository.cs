using ApexDiff.Models;
using ApexVpkInventory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
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
                new AssetInventory("Apex Legends", "v3.0.5.171", "inventories/Apex Legends v3.0.5.171.json"),
                new AssetInventory("Apex Legends", "v3.0.4.111", "inventories/Apex Legends v3.0.4.111.json"),
                new AssetInventory("Apex Legends", "v3.0.3.105", "inventories/Apex Legends v3.0.3.105.json"),
                // new AssetInventory("Titanfall 2", "v2.0.11.0", "inventories/Titanfall 2 v2.0.11.0.json"),
            };
        }

        public async Task<DirectoryTreeNode> GetInventoryFileTreeAsync(AssetInventory inventory, CancellationToken cancellationToken = default)
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
                return new DirectoryTreeNode()
                {
                    Name = "Root"
                };
            }
        }

        public async Task<DirectoryTreeNode> CalculateFileTreeDiffAsync(DirectoryTreeNode sourceTree, DirectoryTreeNode targetTree, CancellationToken cancellationToken = default)
        {
            var directoryName = sourceTree != null ? sourceTree.Name : targetTree.Name;

            var diffDirectory = new DirectoryTreeNode()
            {
                Name = directoryName,
            };

            if (sourceTree == null)
            {
                foreach (var file in targetTree.Files)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    diffDirectory.Files.Add(new FileTreeNode()
                    {
                        Name = file.Name,
                        DiffState = DiffState.Added
                    });
                }

                foreach (var directory in targetTree.Directories)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    diffDirectory.Directories.Add(await CalculateFileTreeDiffAsync(null, directory, cancellationToken));
                }
            }
            else if (targetTree == null)
            {
                foreach (var file in sourceTree.Files)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    diffDirectory.Files.Add(new FileTreeNode()
                    {
                        Name = file.Name,
                        DiffState = DiffState.Deleted
                    });
                }

                foreach (var directory in sourceTree.Directories)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    diffDirectory.Directories.Add(await CalculateFileTreeDiffAsync(directory, null, cancellationToken));
                }
            }
            else
            {
                foreach (var targetFile in targetTree.Files)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var fileDiffStatus = DiffState.None;
                    var sourceFile = sourceTree.Files.SingleOrDefault(c => c.Name == targetFile.Name);

                    if (sourceFile == null)
                    {
                        fileDiffStatus = DiffState.Added;
                    }
                    else if (sourceFile.Hash != targetFile.Hash)
                    {
                        fileDiffStatus = DiffState.Modified;
                    }    

                    diffDirectory.Files.Add(new FileTreeNode()
                    {
                        Name = targetFile.Name,
                        DiffState = fileDiffStatus
                    });
                }

                foreach (var targetDirectory in targetTree.Directories)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var sourceDirectory = sourceTree.Directories.SingleOrDefault(c => c.Name == targetDirectory.Name);
                    diffDirectory.Directories.Add(await CalculateFileTreeDiffAsync(sourceDirectory, targetDirectory, cancellationToken));
                }
            }

            return diffDirectory;
        }

        private async Task<DirectoryTreeNode> GetVpkInventoryFileTreeAsync(VpkInventory vpkInventory, CancellationToken cancellationToken = default)
        {
            var rootNode = new DirectoryTreeNode()
            {
                Name = "Root"
            };

            foreach (var package in vpkInventory.Packages)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var packageNode = new DirectoryTreeNode()
                {
                    Name = package.Filename
                };

                foreach (var entry in package.Entries)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var currentDirectory = packageNode;
                    var pathParts = entry.Filename.Split('/');

                    // Last part is always filename
                    for (int i = 0; i < pathParts.Length - 1; i++)
                    {
                        var childDirectory = currentDirectory.Directories.SingleOrDefault(d => d.Name == pathParts[i]);

                        if (childDirectory == null)
                        {
                            childDirectory = new DirectoryTreeNode()
                            {
                                Name = pathParts[i]
                            };
                            currentDirectory.Directories.Add(childDirectory);
                        }

                        currentDirectory = childDirectory;
                    }

                    currentDirectory.Files.Add(new FileTreeNode()
                    {
                        Name = pathParts.Last(),
                        Hash = entry.Sha256,
                    });
                }

                rootNode.Directories.Add(packageNode);
            }

            return rootNode;
        }
    }
}
