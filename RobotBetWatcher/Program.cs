using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PuppeteerSharp;
using RobotBetApi.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.ComponentModel;

namespace RobotBetWatcher
{
    static class Program
    {
        private static TimeSpan latestTime;
        static async Task Main(string[] args)
        {
            //var backgroundWorker = new BackgroundWorker();
            //backgroundWorker.DoWork += DoWork;
            //backgroundWorker.RunWorkerCompleted += RunWorkerCompleted;

            //Console.ReadKey();
            //backgroundWorker.RunWorkerAsync();

            //Console.ReadLine();
            try
            {
                //IConfiguration config = Configuration.Default.WithDefaultLoader().WithCss().WithCookies().WithJavaScript();
                //IBrowsingContext context = BrowsingContext.New(config);
                //IDocument document = await context.OpenAsync(url);

                Console.Write("Downloading browser... ");
                if (GetBrowser.GetBrowserAsync().Result)
                {
                    Console.WriteLine("Completed!");
                    Console.WriteLine(GetBrowser.Browser.GetUserAgentAsync().Result);
                }
                else
                {
                    Console.WriteLine("Browser download error.");
                    Console.ReadKey();
                }

                Console.Write("Opening pages... ");
                if (GetPage.OpenPagesAsync().Result)
                {
                    Console.WriteLine("Completed!");
                    //var htmlPageResults = await GetPage.PageResults.GetContentAsync();                    
                }
                else
                {
                    Console.WriteLine("Open pages error.");
                    Console.ReadKey();
                }
                //await DeleteCookie(GetPage.PageCurrentRace);
                await GetPilot();
                Console.ReadKey();
            }
            catch (Exception)
            {
                Console.WriteLine("erro");
            }
        }
        private static async Task DeleteCookie(Page page)
        {
            var cookies = new List<CookieParam>
            {
                new CookieParam{ Name = "__cf_bm", Domain = ".bet365.com" },
                new CookieParam{ Name = "__cf_bm", Domain = ".imagecache365.com" },
                new CookieParam{ Name = "aaat" },
                new CookieParam{ Name = "aps03" },
                new CookieParam{ Name = "pstk" },
                new CookieParam{ Name = "qBvrxRyB" },
                new CookieParam{ Name = "rmbs" },
                new CookieParam{ Name = "session" },
                new CookieParam{ Name = "usdi" }
            };

            while (!Console.KeyAvailable)
                await page.DeleteCookieAsync(cookies.ToArray());
                //    new IEnumerable<CookieParam>()
                //    {
                //        Name = "__cf_bm",
                //        Domain = ".imagecache365.com"
                //    },
                //);
        }

        private static void RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            GetBrowser.Browser.CloseAsync();
        }

        private static async void DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                Console.Write("Downloading browser... ");
                if (await GetBrowser.GetBrowserAsync())
                {
                    Console.WriteLine("Completed!");
                }
                else
                {
                    Console.WriteLine("Browser download error.");
                    return;
                }

                Console.Write("Opening pages... ");
                if (await GetPage.OpenPagesAsync())
                    Console.WriteLine("Completed!");
                else
                {
                    Console.WriteLine("Open pages error.");
                    return;
                }

                GetPilot();
                Console.ReadKey();
            }
            catch (Exception)
            {
                Console.WriteLine("erro");
            }
        }

        public static void SetLatestTime(TimeSpan value)
        {
            latestTime = value;
        }
        public static TimeSpan GetLatestTime()
        {
            return latestTime;
        }
        private static async Task<Page> GetBrowserAsync()
        {
            var browserFetcher = new BrowserFetcher();
            await browserFetcher.DownloadAsync();
            Browser browser = await Puppeteer.LaunchAsync(new LaunchOptions
            {
                Headless = false,
            });
            var page = (await browser.PagesAsync())[0];
            page.DefaultTimeout = 0;
            var navigation = new NavigationOptions
            {
                Timeout = 0,
                WaitUntil = new[] {
                        WaitUntilNavigation.DOMContentLoaded }
            };
            await page.GoToAsync("https://www.bet365.com/#/AVR/B24/", navigation);
            return page;
        }

        private static async Task GetPilot()
        {
            while (!Console.KeyAvailable)
            {
                GetRace race = new();
                var taskRace = race.GetCurrentPilotAsync(GetPage.PageCurrentRace);
                while (!taskRace.IsCompleted)
                {
                    Thread.Sleep(1000);
                }

                if (taskRace.IsFaulted)
                {
                    Console.WriteLine($"Pilot search error.\r\n{taskRace.Exception.Message}!");
                    await GetPage.PageCurrentRace.ReloadAsync();
                    continue;
                }

                var raceTask = taskRace.Result;
                if (raceTask.Pilots == null)
                    continue;

                Console.WriteLine("Sending to API...");
                var result = await PostRaceAsync(raceTask);

                RaceDto resultObject = JsonConvert.DeserializeObject<RaceDto>(result);

                JObject parsed = JObject.Parse(result);
                foreach (var pair in parsed)
                {
                    Console.WriteLine("{0}: {1}", pair.Key, pair.Value.ToString());
                }

                Console.WriteLine();
                //Console.WriteLine($"Result: {JsonConvert.DeserializeObject<RaceDto>(result)}\r\n");
            }
        }
        private static async Task<string> PostRaceAsync(Race race)
        {
            //Uri uri = new($"http://192.168.1.192:5000/api/Races");
            Uri uri = new($"http://localhost:5000/api/Races");
            string stringPilot = JsonConvert.SerializeObject(race, Formatting.Indented);
            HttpContent httpContent = new StringContent(stringPilot, Encoding.UTF8, "application/json");

            using HttpClient client = new();
            using var httpResponse = await client.PostAsync(uri, httpContent);

            if (!httpResponse.IsSuccessStatusCode)
                return httpResponse.StatusCode.ToString();

            return await httpResponse.Content.ReadAsStringAsync();

            //var insertedRace = JsonConvert.DeserializeObject<RaceDto>(await httpResponse.Content.ReadAsStringAsync());
            //Pilot newPilot = JsonConvert.DeserializeObject<Pilot>(await httpResponse.Content.ReadAsStringAsync());
        }
    }
}
