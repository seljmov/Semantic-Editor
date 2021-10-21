using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using System.Collections;
using Semantic.Editor.Core.Controls;

namespace Semantic.Editor.Core
{
    public abstract class Block : UserControl
    {
        private bool _enableDrag;
        private Point _start;
        private int _draggedIndex;
        private int _targetIndex;
        private IHaveItems? _haveItems;
        private IControl? _draggedContainer;

        public new Block? Parent { get; set; }

        public void OnPressed(object? sender, PointerPressedEventArgs e) 
        {
            if (Parent is not IHaveItems haveItems)
            {
                return;
            }

            _enableDrag = true;
            _start = e.GetPosition(Parent);
            _draggedIndex = -1;
            _targetIndex = -1;
            _haveItems = haveItems;
            _draggedContainer = ((IControl?)sender);

            AddTransform(_haveItems);
        }
        
        public void OnReleased(object? sender, PointerReleasedEventArgs e) 
        {
            if (_enableDrag)
            {
                RemoveTransform(_haveItems);

                if (_draggedIndex >= 0 && _targetIndex >= 0 && _draggedIndex != _targetIndex)
                {
                    MoveDraggedItem(_haveItems, _draggedIndex, _targetIndex);
                }

                _draggedIndex = -1;
                _targetIndex = -1;
                _enableDrag = false;
                _haveItems = null;
                _draggedContainer = null;
            }
        }
        
        private void AddTransform(IHaveItems? haveItems)
        {
            if (haveItems?.ItemList is null)
            {
                return;
            }

            var i = 0;

            foreach (var _ in haveItems.ItemList)
            {
                var container = haveItems.ItemList[i];
                if (container is not null)
                {
                    container.RenderTransform = new TranslateTransform();
                }

                i++;
            }
        }

        private void RemoveTransform(IHaveItems? haveItems)
        {
            if (haveItems?.ItemList is null)
            {
                return;
            }

            var i = 0;

            foreach (var _ in haveItems.ItemList)
            {
                var container = haveItems.ItemList[i];
                if (container is not null)
                {
                    container.RenderTransform = null;
                }

                i++;
            }
        }

        private void MoveDraggedItem(IHaveItems? haveItems, int draggedIndex, int targetIndex)
        {
            if (haveItems?.ItemList is not IList items)
            {
                return;
            }

            var draggedItem = items[draggedIndex];
            items.RemoveAt(draggedIndex);
            items.Insert(targetIndex, draggedItem);

            _haveItems!.SelectedIndex = targetIndex;
        }

        public void OnMoved(object? sender, PointerEventArgs e) 
        {
            if (_haveItems?.ItemList is null || _draggedContainer?.RenderTransform is null || !_enableDrag)
            {
                return;
            }

            var position = e.GetPosition(Parent);
            var delta = position.Y - _start.Y;

            ((TranslateTransform)_draggedContainer.RenderTransform).Y = delta;

            _draggedIndex = _haveItems.ItemList.IndexOf((Block)_draggedContainer);
            _targetIndex = -1;

            var draggedBounds = _draggedContainer.Bounds;
            var draggedStart = draggedBounds.Y;
            var draggedDeltaStart = draggedStart + delta;
            var draggedDeltaEnd = draggedBounds.Y + delta + draggedBounds.Height;

            var i = 0;

            foreach (var _ in _haveItems.ItemList)
            {
                var targetContainer = _haveItems.ItemList[i];
                if (targetContainer?.RenderTransform is null || ReferenceEquals(targetContainer, _draggedContainer))
                {
                    i++;
                    continue;
                }

                var targetBounds = targetContainer.Bounds;
                var targetStart = targetBounds.Y;
                var targetMid = targetBounds.Y + targetBounds.Height / 2;
                var targetIndex = _haveItems.ItemList.IndexOf(targetContainer);

                if (targetStart > draggedStart && draggedDeltaEnd >= targetMid)
                {
                    ((TranslateTransform)targetContainer.RenderTransform).Y = -draggedBounds.Height;

                    _targetIndex = _targetIndex == -1
                        ? targetIndex
                        : targetIndex < _targetIndex ? targetIndex : _targetIndex;
                }
                else if (targetStart < draggedStart && draggedDeltaStart <= targetMid)
                {
                    ((TranslateTransform)targetContainer.RenderTransform).Y = draggedBounds.Height;

                    _targetIndex = _targetIndex == -1
                        ? targetIndex
                        : targetIndex > _targetIndex ? targetIndex : _targetIndex;
                }
                else
                {
                    ((TranslateTransform)targetContainer.RenderTransform).Y = 0;
                }

                i++;
            }
        }
    }
}
