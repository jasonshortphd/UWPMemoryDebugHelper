This is a helper class that I ported from a Silverlight WP7 app and use all the time when debugging my apps.

It is very simple, and shows a RED Current and Peak memory usage of the app.  It is really good for just using the app and seeing those memory leaks pile up.

In your App.xaml.cs OnLaunched() function you will want to put the following code after the block that validates the root frame is valid.

            if (rootFrame == null)
            {
                            // more code here
            }        

#if DEBUG
            new MemoryWatcher(rootFrame) { IsDisplayed = true };
#endif

That's it! As you navigate around your app you will see the memory displayed.

If you want to see a sample app that has this utility included take a look here:
https://github.com/jasonshortphd/virtualizeddatalistUWP 

Enjoy!

