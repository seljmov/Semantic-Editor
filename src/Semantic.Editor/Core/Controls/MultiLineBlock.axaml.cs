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
                new SingleLineBlock { PhysicalText = "Im first (main)" },
                new SingleLineBlock { PhysicalText = "Im second (main)" },
                new SingleLineBlock { PhysicalText = "Im third (main)" },
            };

            MyItems = new ObservableCollection<ListBoxItem>
            {
                new ListBoxItem {Content = "1"},
                new ListBoxItem {Content = "2"},
                new ListBoxItem {Content = "3"},
                // new ListBoxItem {Content = new SingleLineBlock { PhysicalText = "Im second" }},
                // new ListBoxItem {Content = new SingleLineBlock { PhysicalText = "Im third" }},
            };

            foreach (var item in MyItems)
            {
                item.PointerPressed += OnPressed;
                item.PointerReleased += OnReleased;
                item.PointerMoved += OnMoved;
            }

            DataContext = this;

            // PointerPressed += OnPressed;
            // PointerReleased += OnReleased;
            // PointerMoved += OnMoved;
        }

        public ObservableCollection<ListBoxItem> MyItems { get; set; }

        public ObservableCollection<Block> ItemList { get; set; }
        public override string? PhysicalText { get; set ; }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
