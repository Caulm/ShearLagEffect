using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;

namespace AppShearLagEffect.Utils;

public static class TreeViewHelper
{
    private static TreeViewItem? currentItem;

    private static readonly DependencyPropertyKey IsMouseDirectlyOverItemKey =
        DependencyProperty.RegisterAttachedReadOnly("IsMouseDirectlyOverItem",
                                            typeof(bool),
                                            typeof(TreeViewHelper),
                                            new FrameworkPropertyMetadata(null, new CoerceValueCallback(CalculateIsMouseDirectlyOverItem)));

    public static readonly DependencyProperty IsMouseDirectlyOverItemProperty =
        IsMouseDirectlyOverItemKey.DependencyProperty;

    public static bool GetIsMouseDirectlyOverItem(DependencyObject obj)
    {
        return (bool)obj.GetValue(IsMouseDirectlyOverItemProperty);
    }

    private static object CalculateIsMouseDirectlyOverItem(DependencyObject item, object value)
    {
        return item == currentItem;
    }

    private static readonly RoutedEvent UpdateOverItemEvent = EventManager.RegisterRoutedEvent(
        "UpdateOverItem", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(TreeViewHelper));

    static TreeViewHelper()
    {
        EventManager.RegisterClassHandler(typeof(TreeViewItem), TreeViewItem.MouseEnterEvent, new MouseEventHandler(OnMouseTransition), true);
        EventManager.RegisterClassHandler(typeof(TreeViewItem), TreeViewItem.MouseLeaveEvent, new MouseEventHandler(OnMouseTransition), true);
        EventManager.RegisterClassHandler(typeof(TreeViewItem), UpdateOverItemEvent, new RoutedEventHandler(OnUpdateOverItem));
    }


    static void OnUpdateOverItem(object sender, RoutedEventArgs args)
    {
        currentItem = sender as TreeViewItem;
        currentItem?.InvalidateProperty(IsMouseDirectlyOverItemProperty);
        args.Handled = true;
    }

    static void OnMouseTransition(object sender, MouseEventArgs args)
    {
        lock (IsMouseDirectlyOverItemProperty)
        {
            if (currentItem != null)
            {
                var oldItem = currentItem;
                currentItem = null;
                oldItem.InvalidateProperty(IsMouseDirectlyOverItemProperty);
            }
            var currentPosition = Mouse.DirectlyOver;
            if (currentPosition != null)
            {
                var newItemArgs = new RoutedEventArgs(UpdateOverItemEvent);
                currentPosition.RaiseEvent(newItemArgs);

            }
        }
    }
}
