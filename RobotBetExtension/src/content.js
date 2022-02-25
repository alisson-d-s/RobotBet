////document.addEventListener("DOMContentLoaded", function () {
////    chrome.tabs.getSelected(null, function (tab) {
////        d = document;
////        console.log("DOMContentLoaded.");
////    });
////});

////function GetCurrentRace() {
////    console.log("await...");
////    let element = document.querySelector(".vr-EventTimesNavBarButton");
////    if (!element) {
////        console.log("left.");
////        return;
////    }

////    console.log(element.innerHTML);
////    element.innerHTML = "66:66";
////}

////GetCurrentRace();

function GetPilot() {
    const names = document.querySelectorAll(".vr-ParticipantVirtual_Identifier");
    const odds = document.querySelectorAll(".vr-ParticipantVirtualOddsOnly_Odds");
    if (!names) return;

    let Pilots = [];

    for (let i = 0; i < 4; i++) {
        Pilots.push({
            PilotCode: i + 1,
            PilotName: names[i].textContent,
            Odd: odds[i].textContent
        });
    };
    console.log(JSON.stringify(Pilots));
};
const targetNode = document.body;

const config = {
    attributes: false,
    characterData: false,
    childList: true,
    subtree: true
};

const callback = function (mutationsList, observer) {
    mutationsList.forEach((mutation) => { //.filter(m => m.length > 0)
        //if (mutation.length === 0)
        //    continue;

        const addedNodesMutation = Array.from(mutation.addedNodes)
            .filter(({ classList }) => classList && classList.contains("vr-EventTimesNavBar"));

        if (addedNodesMutation.length) {
            console.log(addedNodesMutation.length + " | " + addedNodesMutation[0].innerHTML);
            console.log("achou");

            const raceSchedules = document.querySelectorAll(".vr-EventTimesNavBarButton");
            if (!raceSchedules)
                console.log("Search race shedule error.");

            let cont = 1;
            raceSchedules.forEach((schedule) => {
                //const detail = {
                //    name: "Race: " + cont,
                //    value: schedule.textContent
                //};
                //document.cookie = "Race" + cont + "=" + schedule.textContent;
                document.cookie = `Race${cont}=${JSON.stringify({ RaceId: 1, Time: schedule.textContent })}`;
                //console.log(schedule.textContent);
                //chrome.cookies.set(detail, function (cookie) {
                //    console.log(cookie.value);
                //});
                cont++;
            });
            GetPilot();
            console.log(document.cookie)

            //console.log(element.textContent);

            //observer.disconnect();
        }
    });
    console.log("saiu");
};

const observer = new MutationObserver(callback);

observer.observe(targetNode, config);