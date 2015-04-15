using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Navigation;

namespace Samples
{
    public class BrowserWindow
    {
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        private Application _app;
        private Window _window;

        public void Show(string uriString)
        {
            if (Thread.CurrentThread.GetApartmentState() != ApartmentState.STA)
            {
                var message = "Please set STAThreadAttribute on application entry point (Main method)";
                Console.WriteLine(message);
                throw new InvalidOperationException(message);
            }

            _app = new Application();
            _window = new Window();
            var browser = new WebBrowser();
            _window.Content = browser;

            browser.Loaded += (sender, eventArgs) =>
            {
                browser.Navigate(uriString);
                SetForegroundWindow(new WindowInteropHelper(_window).Handle);
            };

            browser.Navigating += (sender, eventArgs) =>
            {
                if (this.Navigating != null)
                {
                    this.Navigating(this, eventArgs);
                }
            };

            browser.Navigated += (sender, eventArgs) =>
            {
                if (this.Navigated != null)
                {
                    this.Navigated(this, eventArgs);
                }
            };

            _window.Closed += (sender, eventArgs) => _app.Shutdown();

            _app.Run(_window);
        }

        public void Close()
        {
            _window.Close();
        }

        public event NavigatingCancelEventHandler Navigating;
        public event NavigatedEventHandler Navigated;
    }
}
