using PuppeteerSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotBetWatcher
{
    public static class GetPage
    {
        public static Page PageCurrentRace { get; private set; }
        public static Page PageResults { get; private set; }
        private static IEnumerable<Page> Pages { get; set; }

        public static async Task<bool> OpenPagesAsync()
        {
            try
            {
                var browser = GetBrowser.Browser;
                var pages = await browser.PagesAsync();

                while (pages.Length < 2)
                    pages = pages.Append(await browser.NewPageAsync()).ToArray();

                Pages = pages;
                PageResults = pages[0];
                PageCurrentRace = pages[1];

                return await ConfigurePagesAsync();
            }
            catch (Exception)
            {
                return false;
            }
        }
        private static async Task<bool> ConfigurePagesAsync()
        {
            try
            {
                var navigation = new NavigationOptions
                {
                    Timeout = 0,
                    WaitUntil = new[]
                    {
                        WaitUntilNavigation.DOMContentLoaded
                    }
                };
                //Mozilla/5.0 (iPad; CPU OS 6_0 like Mac OS X) AppleWebKit/536.26 (KHTML, like Gecko) Version/6.0 Mobile/10A5355d Safari/8536.25
                var dic = new Dictionary<string, string>
                {
                    //{ "Accept-Language", "en-GB,en-US;q=0.9,en;q=0.8" }
                    //{ "user-agent", "Mozilla/5.0 (iPad; CPU OS 6_0 like Mac OS X) AppleWebKit/536.26 (KHTML, like Gecko) Version/6.0 Mobile/10A5355d Safari/8536.25" }
                };
                //en-GB,en-US;q=0.9,en;q=0.8

                await PageResults.SetExtraHttpHeadersAsync(dic);
                await PageResults.SetUserAgentAsync("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/98.0.4758.102 Safari/537.36");

                foreach (var page in Pages)
                {
                    page.DefaultTimeout = 0;
                    await page.SetExtraHttpHeadersAsync(dic);
                    await page.SetUserAgentAsync("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/98.0.4758.102 Safari/537.36");
                    
                    var response = await page.GoToAsync("https://www.bet365.com/#/AVR/B24/R^1/", navigation);
                    //await page.SetCookieAsync(await page.GetCookiesAsync());


                    if (!response.Ok)
                        return false;
                }
                return await NavigateToResultsDiv();
            }
            catch
            {
                return false;
            }
        }
        private static async Task<bool> NavigateToResultsDiv()
        {
            Login login = new();
            try
            {
                await PageResults.BringToFrontAsync();
                //await login.LoginUserAsync();
                var tagResults = await PageResults.WaitForSelectorAsync(".vr-ResultsNavBarButton");
                await tagResults.ClickAsync();

                await PageResults.WaitForSelectorAsync(".vrr-MarketGroupOutrightRaceDescription");

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
