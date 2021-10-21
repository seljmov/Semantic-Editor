using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Semantic.Editor.Core.Controls
{
    public partial class MultiLineBlock : Block, IHaveItems
    {
        public MultiLineBlock()
        {
            InitializeComponent();
            // ListBox mainListBox = this.FindControl<ListBox>("MainListBox");

            ItemList = new ObservableCollection<Block>
            {
                new SingleLineBlock { CustomText = "Im first", Parent = this },
                new SingleLineBlock { CustomText = "Im second", Parent = this },
                new SingleLineBlock { CustomText = "Im third", Parent = this },
            };

            DataContext = this;

            PointerPressed += OnPressed;
            PointerReleased += OnReleased;
            PointerMoved += OnMoved;
        }

        public IList<Block> ItemList { get; set; }
        public int SelectedIndex { get; set; }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
