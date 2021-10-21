using System.Collections.ObjectModel;

namespace Semantic.Editor.Core
{
    public interface IHaveItems
    {
        public ObservableCollection<Block> ItemList { get; set; }
        
        // public int SelectedIndex { get; set; }
    }
}
