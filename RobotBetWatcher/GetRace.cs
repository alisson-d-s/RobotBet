using PuppeteerSharp;
using RobotBetApi.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

namespace RobotBetWatcher
{
    public class GetRace
    {
        private DateTimeOffset ValidateDate(TimeSpan time)
        {
            return new(DateTime.Today
                .AddHours(DateTime.Now.Hour)
                .AddMinutes(time.Minutes)
                .ToUniversalTime());
        }

        public async Task<Race> GetCurrentPilotAsync(Page page)
        {
            Race race = new();
            List<Pilot> listPilot = new();
            TimeSpan time;

            await page.WaitForSelectorAsync(".vr-EventTimesNavBarButton");
            var tagTime = await page.QuerySelectorAllAsync(".vr-EventTimesNavBarButton");
            if (tagTime.Length == 0)
                return race;

            time = await tagTime[0].EvaluateFunctionAsync<TimeSpan>(@"el => el.textContent");

            if (Program.GetLatestTime() == time)
                return race;

            Program.SetLatestTime(time);
            Thread.Sleep(2000);

            var tagName = await page.QuerySelectorAllAsync(".vr-ParticipantVirtual_Identifier");
            if (tagName.Length == 0)
                return race;

            var tagOdds = await page.QuerySelectorAllAsync(".vr-ParticipantVirtualOddsOnly");
            if (tagOdds.Length == 0)
                return race;

            race.RaceDate = ValidateDate(time);
            for (int i = 0; i < tagOdds.Length; i++)
            {
                Pilot pilot = new()
                {
                    PilotCode = i + 1,
                    PilotName = await tagName[i].EvaluateFunctionAsync<string>(@"el => el.textContent"),
                    Odd = await tagOdds[i].EvaluateFunctionAsync<double>(@"el => el.textContent")
                };
                listPilot.Add(pilot);
            }
            race.Pilots = listPilot;
            return race;
        }
    }
}
