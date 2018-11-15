using NLog;
using System.Windows;

namespace Panda.Infrastructure
{
    public class ViewAwareScreenBase : ScreenBase, IViewAware
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        void IViewAware.AttachView(object view)
        {
            logger.Trace($"Attaching view {view.GetType().Name} to {GetType().Name}");

            var fe = view as FrameworkElement;
            if (fe == null)
            {
                logger.Warn($"View must derived from FrameworkElement");
                return;
            }

            // Attach loaded event handler
            void loaded(object s, RoutedEventArgs e)
            {
                fe.Loaded -= loaded;
                OnViewLoaded(view);
            }
            fe.Loaded += loaded;

            // Attach unloaded event handler
            void unloaded(object s, RoutedEventArgs e)
            {
                fe.Unloaded -= unloaded;
                OnViewUnloaded(view);
            }
            fe.Unloaded += unloaded;

            if (view is Window win)
            {
                win.Closing += (s, e) => 
                {
                    // It is not necessary to unsubscribe to this event, as the application is already closing down
                    OnViewUnloaded(view);
                };
            }
        }

        protected virtual void OnViewLoaded(object view) { }

        protected virtual void OnViewUnloaded(object view) { }
    }
}
