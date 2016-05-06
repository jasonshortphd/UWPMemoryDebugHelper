using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// UWP Memory Debug Helper
namespace MemoryWatcherUtility
{
    //  Read the Readme.md for more details on how to use this class
    //  https://github.com/jasonshortphd/UWPMemoryDebugHelper
    //
    //  This is based upon a WP7 Silverlight utility class that I used to use in my apps (doesn't work in UWP)
    //  http://www.japf.fr/2011/11/track-memory-usage-of-your-windows-phone-7-1-app-in-real-time/ 
    //  Updated to support UWP syntax
    //
    public class MemoryWatcher : ContentControl
    {
        private readonly DispatcherTimer timer;
        private readonly Frame frame;
        private const float ByteToMega = 1024 * 1024;

        public bool IsDisplayed { get; set; }

        public MemoryWatcher(Frame frame)
        {
            if (frame == null)
                throw new ArgumentNullException("frame");

            this.frame = frame;
            this.frame.Navigated += new NavigatedEventHandler(this.OnFrameNavigated);
            this.frame.Navigating += new NavigatingCancelEventHandler(this.OnFrameNavigating);

            this.timer = new DispatcherTimer();
            this.timer.Interval = TimeSpan.FromMilliseconds(100);
            this.timer.Tick += OnTimerTick;

            // setup some basic properties to ensure the content will be visible
            this.Foreground = new SolidColorBrush(Colors.Red);
            this.VerticalContentAlignment = VerticalAlignment.Center;
            this.HorizontalContentAlignment = HorizontalAlignment.Center;
            this.Margin = new Thickness(0, 0, 0, 0);
        }

        static ulong peakMemoryUsage = 0;

        //
        // Here we render a display in RED over the current content to show current and peak memory usage
        //
        private void OnTimerTick(object sender, object e)
        {
            try
            {
                ulong currentMemoryUsage = MemoryManager.AppMemoryUsage;
                if (currentMemoryUsage > peakMemoryUsage)
                    peakMemoryUsage = currentMemoryUsage;

                string currentMemory = (currentMemoryUsage / ByteToMega).ToString("#.00");
                string peakMemory = (peakMemoryUsage / ByteToMega).ToString("#.00");

                this.Content = string.Format("Current: {0}MB Peak: {1}MB", currentMemory, peakMemory);
            }
            catch (Exception)
            {
                this.timer.Stop();
            }
        }

        private void OnFrameNavigated(object sender, NavigationEventArgs e)
        {
            if (!this.IsDisplayed)
                return;

            var page = this.frame.Content as Page;
            if (page != null)
            {
                Panel host = page.Content as Panel;
                if (host != null && !host.Children.Any(c => c is MemoryWatcher))
                {
                    this.timer.Start();
                    host.Children.Add(this);
                    peakMemoryUsage = 0;
                }
            }
        }

        private void OnFrameNavigating(object sender, NavigatingCancelEventArgs navigatingCancelEventArgs)
        {
            var page = this.frame.Content as Page;
            if (page != null)
            {
                Panel host = page.Content as Panel;
                if (host != null && host.Children.Contains(this))
                {
                    peakMemoryUsage = 0;
                    this.timer.Stop();
                    host.Children.Remove(this);
                }
            }
        }
    }
}
