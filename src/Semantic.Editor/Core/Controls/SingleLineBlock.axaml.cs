using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;
using System;

namespace Semantic.Editor.Core.Controls
{
    public partial class SingleLineBlock : Block, IStyleable
    {
        private string? _customText;

        public SingleLineBlock()
        {
            InitializeComponent();

            PointerPressed += OnPressed;
            PointerReleased += OnReleased;
            PointerMoved += OnMoved;
        }

        public string? CustomText
        {
            get => _customText;
            set
            {
                _customText = value;
                // Content = new ListBoxItem { Content = _customText };
                Content = _customText;
            }
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
