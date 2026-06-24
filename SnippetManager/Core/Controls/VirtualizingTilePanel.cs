namespace SnippetManager.Controls
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Media;

    public class VirtualizingTilePanel : VirtualizingPanel, IScrollInfo
    {
        public static readonly DependencyProperty ItemWidthProperty =
            DependencyProperty.Register(
                nameof(ItemWidth),
                typeof(double),
                typeof(VirtualizingTilePanel),
                new FrameworkPropertyMetadata(160.0,
                    FrameworkPropertyMetadataOptions.AffectsMeasure));

        public static readonly DependencyProperty ItemHeightProperty =
            DependencyProperty.Register(
                nameof(ItemHeight),
                typeof(double),
                typeof(VirtualizingTilePanel),
                new FrameworkPropertyMetadata(160.0,
                    FrameworkPropertyMetadataOptions.AffectsMeasure));

        public double ItemWidth
        {
            get => (double)GetValue(ItemWidthProperty);
            set => SetValue(ItemWidthProperty, value);
        }

        public double ItemHeight
        {
            get => (double)GetValue(ItemHeightProperty);
            set => SetValue(ItemHeightProperty, value);
        }

        private Size _extent;
        private Size _viewport;
        private Point _offset;

        protected override Size MeasureOverride(Size availableSize)
        {
            ItemsControl itemsControl = ItemsControl.GetItemsOwner(this);

            if (itemsControl == null)
                return availableSize;

            int itemCount = itemsControl.Items.Count;

            int columns = Math.Max(1,
                (int)Math.Floor(availableSize.Width / ItemWidth));

            int rows = (int)Math.Ceiling(
                (double)itemCount / columns);

            _extent = new Size(
                columns * ItemWidth,
                rows * ItemHeight);

            _viewport = availableSize;

            ScrollOwner?.InvalidateScrollInfo();

            int firstVisibleRow =
                (int)Math.Floor(_offset.Y / ItemHeight);

            int visibleRows =
                (int)Math.Ceiling(
                    availableSize.Height / ItemHeight) + 1;

            int firstIndex =
                firstVisibleRow * columns;

            int lastIndex =
                Math.Min(
                    itemCount - 1,
                    ((firstVisibleRow + visibleRows) * columns) - 1);

            CleanUpItems(firstIndex, lastIndex);

            IItemContainerGenerator generator =
                ItemContainerGenerator;

            GeneratorPosition startPos =
                generator.GeneratorPositionFromIndex(firstIndex);

            int childIndex =
                startPos.Offset == 0
                    ? startPos.Index
                    : startPos.Index + 1;

            using (generator.StartAt(
                startPos,
                GeneratorDirection.Forward,
                true))
            {
                for (int itemIndex = firstIndex;
                     itemIndex <= lastIndex;
                     itemIndex++, childIndex++)
                {
                    bool newlyRealized;

                    var child =
                        (UIElement)generator.GenerateNext(
                            out newlyRealized);

                    if (newlyRealized)
                    {
                        if (childIndex >= InternalChildren.Count)
                            AddInternalChild(child);
                        else
                            InsertInternalChild(childIndex, child);

                        generator.PrepareItemContainer(child);
                    }

                    child.Measure(
                        new Size(ItemWidth, ItemHeight));
                }
            }

            return availableSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            int columns = Math.Max(
                1,
                (int)Math.Floor(finalSize.Width / ItemWidth));

            for (int i = 0; i < InternalChildren.Count; i++)
            {
                UIElement child = InternalChildren[i];

                GeneratorPosition pos =
                    new GeneratorPosition(i, 0);

                int itemIndex =
                    ItemContainerGenerator.IndexFromGeneratorPosition(pos);

                int row = itemIndex / columns;
                int column = itemIndex % columns;

                child.Arrange(
                    new Rect(
                        column * ItemWidth,
                        row * ItemHeight - _offset.Y,
                        ItemWidth,
                        ItemHeight));
            }

            return finalSize;
        }

        private void CleanUpItems(
            int firstVisible,
            int lastVisible)
        {
            for (int i = InternalChildren.Count - 1;
                 i >= 0;
                 i--)
            {
                GeneratorPosition pos =
                    new GeneratorPosition(i, 0);

                int itemIndex =
                    ItemContainerGenerator
                        .IndexFromGeneratorPosition(pos);

                if (itemIndex < firstVisible ||
                    itemIndex > lastVisible)
                {
                    RemoveInternalChildRange(i, 1);

                    ItemContainerGenerator.Remove(
                        pos,
                        1);
                }
            }
        }

        #region IScrollInfo

        public ScrollViewer ScrollOwner { get; set; }

        public bool CanVerticallyScroll { get; set; }
        public bool CanHorizontallyScroll { get; set; }

        public double ExtentWidth => _extent.Width;
        public double ExtentHeight => _extent.Height;

        public double ViewportWidth => _viewport.Width;
        public double ViewportHeight => _viewport.Height;

        public double HorizontalOffset => _offset.X;
        public double VerticalOffset => _offset.Y;

        public void SetVerticalOffset(double offset)
        {
            offset = Math.Max(
                0,
                Math.Min(offset,
                    ExtentHeight - ViewportHeight));

            _offset.Y = offset;

            InvalidateMeasure();

            ScrollOwner?.InvalidateScrollInfo();
        }

        public void LineDown() => SetVerticalOffset(VerticalOffset + 30);
        public void LineUp() => SetVerticalOffset(VerticalOffset - 30);

        public void MouseWheelDown() => LineDown();
        public void MouseWheelUp() => LineUp();

        public void PageDown()
            => SetVerticalOffset(
                VerticalOffset + ViewportHeight);

        public void PageUp()
            => SetVerticalOffset(
                VerticalOffset - ViewportHeight);

        public void SetHorizontalOffset(double offset) { }
        public void LineLeft() { }
        public void LineRight() { }
        public void MouseWheelLeft() { }
        public void MouseWheelRight() { }
        public void PageLeft() { }
        public void PageRight() { }

        public Rect MakeVisible(
            Visual visual,
            Rect rectangle)
        {
            return rectangle;
        }

        #endregion
    }
}
