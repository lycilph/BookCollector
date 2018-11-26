using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace Panda.Behaviors
{
    public class ScrollOnNewItemBehavior : Behavior<ItemsControl>
    {
        protected override void OnAttached()
        {
            AssociatedObject.Loaded += OnLoaded;
            AssociatedObject.Unloaded += OnUnLoaded;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.Loaded -= OnLoaded;
            AssociatedObject.Unloaded -= OnUnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (!(AssociatedObject.ItemsSource is INotifyCollectionChanged source))
                return;

            source.CollectionChanged += OnCollectionChanged;
            Scroll();
        }

        private void OnUnLoaded(object sender, RoutedEventArgs e)
        {
            if (!(AssociatedObject.ItemsSource is INotifyCollectionChanged source))
                return;

            source.CollectionChanged -= OnCollectionChanged;
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                Scroll();
            }
        }

        private void Scroll()
        {
            int count = AssociatedObject.Items.Count;
            if (count == 0)
                return;

            var item = AssociatedObject.Items[count - 1];

            if (!(AssociatedObject.ItemContainerGenerator.ContainerFromItem(item) is FrameworkElement frameworkElement))
                return;

            frameworkElement.BringIntoView();
        }
    }
}
