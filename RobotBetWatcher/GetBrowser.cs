using PuppeteerSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotBetWatcher
{
    public static class GetBrowser
    {
        public static Browser Browser { get; private set; }

        public static async Task<bool> GetBrowserAsync() 
        {
            Browser = await DownloadBrowserAsync();

            return Browser.IsConnected;
        }
        public static Task CloseBrowserAsync()
        {
            return Browser.CloseAsync();
        }

        private static async Task<Browser> DownloadBrowserAsync()
        {
            //BrowserFetcher browserFetcher = new();
            //await browserFetcher.DownloadAsync(BrowserFetcher.DefaultChromiumRevision);
            return await Puppeteer.LaunchAsync(new LaunchOptions
            {
                Headless = false,
                DefaultViewport = null,
                ExecutablePath = @"C:\Program Files\Google\Chrome\Application\chrome.exe",
                Product = Product.Chrome,
                UserDataDir = @"C:\Users\TI-N001\Documents\Docs"
            });
        }
    }
}
