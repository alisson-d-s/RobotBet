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

namespace RobotBetWatcher
{
    static class Program
    {
        private static TimeSpan latestTime;
        private static readonly Page page = GetBrowserAsync().Result;
        static void Main(string[] args)
        {
            try
            {
                GetPilot();
                Console.WriteLine("Press any key to stop...");
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

        private static async void GetPilot()
        {
            while (!Console.KeyAvailable)
            {
                GetRace race = new();
                var taskRace = race.GetCurrentPilotAsync(page);
                while (!taskRace.IsCompleted)
                {
                    Thread.Sleep(1000);
                }

                if (taskRace.IsFaulted)
                {
                    Console.WriteLine($"Pilot search error.\r\n{taskRace.Exception.Message}!");
                    return;
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
