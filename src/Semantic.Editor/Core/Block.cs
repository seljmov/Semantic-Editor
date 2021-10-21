using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using System.Collections;
using Semantic.Editor.Core.Controls;
using Avalonia.Controls.Primitives;
using System;

namespace Semantic.Editor.Core
{
    public abstract class Block : UserControl
    {
        private bool _enableDrag;
        private Point _start;
        private int _draggedIndex;
        private int _targetIndex;
        private ItemsControl? _itemsControl;
        private IControl? _draggedContainer;

        public abstract string? PhysicalText { get; set; }

        public void OnPressed(object? sender, PointerPressedEventArgs e) 
        {
            var item = ((IControl)sender!);

            if (item.Parent is not ItemsControl itemsControl)
            {
                return;
            }

            Console.WriteLine("OnPressed");

            _enableDrag = true;
            _start = e.GetPosition(item.Parent);
            _draggedIndex = -1;
            _targetIndex = -1;
            _itemsControl = itemsControl;
            _draggedContainer = item;

            AddTransform(_itemsControl);
        }
        
        public void OnReleased(object? sender, PointerReleasedEventArgs e) 
        {
            if (_enableDrag)
            {
                Console.WriteLine("OnReleased");

                RemoveTransform(_itemsControl);

                if (_draggedIndex >= 0 && _targetIndex >= 0 && _draggedIndex != _targetIndex)
                {
                    Console.WriteLine($"MoveItem {_draggedIndex} -> {_targetIndex}");
                    MoveDraggedItem(_itemsControl, _draggedIndex, _targetIndex);
                }

                _draggedIndex = -1;
                _targetIndex = -1;
                _enableDrag = false;
                _itemsControl = null;
                _draggedContainer = null;
            }
        }
        
        private void AddTransform(ItemsControl? itemsControl)
        {
            if (itemsControl?.Items is null)
            {
                return;
            }

            var i = 0;

            foreach (var _ in itemsControl.Items)
            {
                var container = itemsControl.ItemContainerGenerator.ContainerFromIndex(i);
                if (container is not null)
                {
                    container.RenderTransform = new TranslateTransform();
                }

                i++;
            }
        }

        private void RemoveTransform(ItemsControl? itemsControl)
        {
            if (itemsControl?.Items is null)
            {
                return;
            }

            var i = 0;

            foreach (var _ in itemsControl.Items)
            {
                var container = itemsControl.ItemContainerGenerator.ContainerFromIndex(i);
                if (container is not null)
                {
                    container.RenderTransform = null;
                }

                i++;
            }
        }

        private void MoveDraggedItem(ItemsControl? itemsControl, int draggedIndex, int targetIndex)
        {
            if (itemsControl?.Items is not IList items)
            {
                return;
            }

            Console.WriteLine("MoveDraggedItem");

            var draggedItem = items[draggedIndex];
            items.RemoveAt(draggedIndex);
            items.Insert(targetIndex, draggedItem);

            if (itemsControl is SelectingItemsControl selectingItemsControl)
            {
                selectingItemsControl.SelectedIndex = targetIndex;
            }
        }

        public void OnMoved(object? sender, PointerEventArgs e) 
        {
            if (_itemsControl?.Items is null || _draggedContainer?.RenderTransform is null || !_enableDrag)
            {
                return;
            }

            Console.WriteLine("OnMoved");

            var position = e.GetPosition(_itemsControl);
            var delta = position.Y - _start.Y;

            ((TranslateTransform)_draggedContainer.RenderTransform).Y = delta;

            _draggedIndex = _itemsControl.ItemContainerGenerator.IndexFromContainer(_draggedContainer);
            _targetIndex = -1;

            var draggedBounds = _draggedContainer.Bounds;
            var draggedStart = draggedBounds.Y;
            var draggedDeltaStart = draggedBounds.Y + delta;
            var draggedDeltaEnd = draggedBounds.Y + delta + draggedBounds.Height;

            var i = 0;

            foreach (var _ in _itemsControl.Items)
            {
                var targetContainer = _itemsControl.ItemContainerGenerator.ContainerFromIndex(i);
                if (targetContainer?.RenderTransform is null || ReferenceEquals(targetContainer, _draggedContainer))
                {
                    i++;
                    continue;
                }

                var targetBounds = targetContainer.Bounds;
                var targetStart = targetBounds.Y;
                var targetMid = targetBounds.Y + targetBounds.Height / 2;
                var targetIndex = _itemsControl.ItemContainerGenerator.IndexFromContainer(targetContainer);

                if (targetStart > draggedStart && draggedDeltaEnd >= targetMid)
                {
                    ((TranslateTransform)targetContainer.RenderTransform).Y = -draggedBounds.Height;

                    _targetIndex = _targetIndex == -1 
                        ? targetIndex 
                        : targetIndex > _targetIndex ? targetIndex : _targetIndex;

                    Console.WriteLine($"Moved Right {_draggedIndex} -> {_targetIndex}");
                }
                else if (targetStart < draggedStart && draggedDeltaStart <= targetMid)
                {
                    ((TranslateTransform)targetContainer.RenderTransform).Y = draggedBounds.Height;

                    _targetIndex = _targetIndex == -1 
                        ? targetIndex 
                        : targetIndex < _targetIndex ? targetIndex : _targetIndex;
                    
                    Console.WriteLine($"Moved Left {_draggedIndex} -> {_targetIndex}");
                }
                else
                {
                    ((TranslateTransform)targetContainer.RenderTransform).Y = 0;
                }

                i++;
            }

            Console.WriteLine($"Moved {_draggedIndex} -> {_targetIndex}");
        }
    }
}
