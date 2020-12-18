using ApexDiff.Data;
using ApexDiff.Models;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace ApexDiff.ViewModels
{
    public class IndexViewModel : INotifyPropertyChanged
    {
        private IReadOnlyList<AssetInventory> _inventories = new List<AssetInventory>();
        private int _sourceSelectedIndex = -1;

        private AssetInventoryRepository InventoryRepository { get; } = null;

        public IndexViewModel(AssetInventoryRepository inventoryRepository)
        {
            InventoryRepository = inventoryRepository;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public AssetInventory SelectedSourceInventory
        {
            get
            {
                if (SourceSelectedIndex < 0 || SourceSelectedIndex >= _inventories.Count)
                {
                    return null;
                }

                return _inventories[SourceSelectedIndex];
            }
        }

        public int SourceSelectedIndex
        {
            get => _sourceSelectedIndex;
            set
            {
                if (_sourceSelectedIndex != value)
                {
                    _sourceSelectedIndex = value;
                    RaisePropertyChanged();
                    RaisePropertyChanged(nameof(SelectedSourceInventory));
                }
            }
        }

        public IReadOnlyList<IGrouping<string, AssetInventory>> GroupedInventories
        {
            get
            {
                return _inventories
                    .GroupBy<AssetInventory, string>(i => i.Game)
                    .ToList();
            }
        }

        public Task InitializeAsync()
        {
            _inventories = InventoryRepository.GetInventories();

            return Task.CompletedTask;
        }

        private void RaisePropertyChanged([CallerMemberName]string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
