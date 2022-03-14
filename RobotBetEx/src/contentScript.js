'use strict';
require("datejs");

function GetRace() {
  console.log("Getting races...");
  const Race = {
    RaceDate: GetDate(document.querySelector(".vr-EventTimesNavBarButton-selected").textContent),
    Pilots: GetPilot()
  };
  console.log(JSON.stringify(Race));
}

function GetPilot() {
  console.log("Getting pilots...");
  const names = document.querySelectorAll(".vr-ParticipantVirtual_Identifier");
  const odds = document.querySelectorAll(".vr-ParticipantVirtualOddsOnly_Odds");
  if (!names || !odds) return;

  let Pilots = [];
  for (let i = 0; i < 4; i++) {
    Pilots.push({
      PilotCode: i + 1,
      PilotName: names[i].textContent,
      Odd: odds[i].textContent
    });
  };
  return Pilots;
  //console.log(JSON.stringify(Pilots));
};

function GetResult() {
  console.log("Getting results...");
  const resultsRaceName = GetResultsRaceName(document.querySelectorAll(".vrr-MarketGroupOutrightRaceDescription_RaceName"));
  const forecastDividend = GetForecastDividend(document.querySelectorAll(".vrr-DividendParticipant_Text"));
  const tricastResult = document.querySelectorAll(".vrr-ResultParticipant_Text");
  const fullResult = GetFullRaceResults(tricastResult);

  let arrayResults = [];
  for (let i = 0; i <= 1; i++) {
    //resultsRaceName.forEach((result) => {
    let interatorResults = fullResult[i].values();
    arrayResults.push({
      ResultDate: GetDate(resultsRaceName[i]),
      FirstPlace: interatorResults.next().value,
      SecondPlace: interatorResults.next().value,
      ThirdPlace: interatorResults.next().value,
      FourthPlace: interatorResults.next().value,
      ForecastDividend: forecastDividend[i]
    });
    //});
  }
  console.log(JSON.stringify(arrayResults));

  //fullResult.forEach((result) => {
  //    console.log(`${result.index + 1}: ${result}`);
  //});
  //const 
};

function GetResultsRaceName(arrayResultsRaceName) {
  const newArray = [];
  arrayResultsRaceName.forEach((array) => {

    //const time = GetResultTime(array.textContent.substring(0, 5).replace(" ", ""));
    //newArray.push(time.length === 5 ? time : `0${time}`);
    newArray.push(GetResultTime(array.textContent));
  });
  return newArray;
};

function GetResultTime(resultsRaceName) {
  const time = resultsRaceName.substring(0, 5).replace(" ", "");
  return time.length === 5 ? time : `0${time}`;
  //return resultsRaceName.substring(0, 5);
};

function CheckSuspendedPage() {
  if (document.querySelector(".vr-BettingSuspendedScreen_Message"))
    document.location.reload();
};

function CheckEventStarted() {
  return document.querySelector(".vr-MarketGroup-eventstarted") ? true : false;
};

function GetFullRaceResults(array) {
  const newArray = [];
  if (array.length === 4) {
    newArray.push(AddLastResult(array[1].textContent));
    newArray.push(AddLastResult(array[3].textContent));
  }
  return newArray;
};

function GetForecastDividend(array) {
  const newArray = [];
  if (array.length === 4) {
    newArray.push(array[0].textContent);
    newArray.push(array[2].textContent);
  }
  return newArray;
};

function AddLastResult(tricastResult) {
  const resultsNumbers = [];
  let resultSum = 0;

  tricastResult.split("-").forEach((result) => {
    resultSum += Number(result);
    resultsNumbers.push(Number(result));
  });
  const lastResult = 10 - resultSum;
  resultsNumbers.push(lastResult);

  return resultsNumbers;
};

function PageLoaded(mutation) {
  const addedNodesMutation = Array.from(mutation.addedNodes)
    .filter(({ classList }) => classList && classList.contains("vr-EventTimesNavBar"));

  return addedNodesMutation.length ? true : false;
};

function SetRaceScheduleCookie(raceSchedules) {
  document.cookie = `LastRace=${raceSchedules[0].textContent}`;
  for (let i = 0; i < 6; i++) {
    let raceCode = i + 1
    document.cookie = `Race${raceCode}=${JSON.stringify({ RaceId: 1, Time: raceSchedules[i].textContent })}`;
  }
};

function SetCookies(cookieName, cookieValue) {
    document.cookie = `${cookieName}=${cookieValue}`;
};

function GetMinutes(time) {
  return time.substring(3, 5);
};

function GetDate(time) {
  const minutes = GetMinutes(time);
  //console.log(`time: ${time}, minutes: ${minutes}`);

  let date = new Date();
  date.setMinutes(minutes, 0, 0);
  date.toLocaleDateString();

  return date;
};

function GetCookie(name) {
  const value = `; ${document.cookie}`;
  const parts = value.split(`; ${name}=`);
  if (parts.length === 2) return parts.pop().split(';').shift();
};

function WaitForResult() {

};

function SetObserver() {
  const targetNode = document.body;

  const config = {
    attributes: false,
    characterData: false,
    childList: true,
    subtree: true
  };

  const callback = function (mutationsList, observer) {
    CheckSuspendedPage();
    if (CheckEventStarted()) return;
    console.log("entrou-func");

    mutationsList.forEach(async (mutation) => {
      //console.log(mutation);
      console.log("entrou-loop");

      if (!PageLoaded(mutation)) return;

      console.log("executou-mutation");

      const raceSchedules = await document.querySelectorAll(".vr-EventTimesNavBarButton");
      if (!raceSchedules) return console.log("Search race shedule error.");

      SetRaceScheduleCookie(raceSchedules);

      const results = await document.querySelector(".vr-ResultsNavBarButton");
      if (!results) return;

      const eventTimesNavBarElement = await document.querySelector(".vr-EventTimesNavBar_ButtonContainer");
      const callbackEventTimesBar = function (mutationEventList, observerEvent) {
        try {
          console.log("entrou-event");
          mutationEventList.forEach(async (mutationEvent) => {
            CheckSuspendedPage();
            console.log("entrou-loop-event");
            const timeLastRace = GetDate(GetResultTime(document.querySelectorAll(".vr-EventTimesNavBarButton").item(0).textContent));
            const timeLastResult = GetDate(GetResultTime(document.querySelectorAll(".vrr-MarketGroupOutrightRaceDescription_RaceName").item(0).textContent));
            if (!timeLastRace || !timeLastResult) return;

            //const timeLastRace = GetDate(GetResultTime(lastRace.item(0)));
            //const timeLastResult = GetDate(GetResultTime(lastResult.item(0)));
            console.log(timeLastRace + " " + timeLastResult);

            observer.disconnect();
            const timeLastResultPlus3 = new Date.parse(timeLastResult).add({ minutes: 3 });
            console.log(`TimeLastRace: ${timeLastRace} | TimeLastResult ${timeLastResult} | TimeLastResult-Plus3: ${timeLastResultPlus3}`);
            if (timeLastRace.compareTo(timeLastResultPlus3) > 0) {
              observerEvent.disconnect();

              document.location.reload();

              //WaitForResult();
              //observer.observe();
            }
          });
        } catch (err) {
          CheckSuspendedPage();
        }
      };

      //console.log(raceSchedules);
      const observerEventTimesBar = new MutationObserver(callbackEventTimesBar);
      observerEventTimesBar.observe(eventTimesNavBarElement, config);

      results.click();
      if (await document.querySelector(".vrr-Price"))//vrr-Price    vr-ResultsNavBarButton-selected
        GetResult();


      //if (document.querySelector(".vr-EventTimesNavBarButton-selected"))
      //    GetRace();
      //if (document.querySelector(".vr-ResultsNavBarButton-selected"))
      //    GetResult();

      //observer.disconnect();
    });
    console.log("saiu");
  };

  const observer = new MutationObserver(callback);

  observer.observe(targetNode, config);
};

SetObserver();