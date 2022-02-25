chrome.runtime.onInstalled.addListener(() => {

});
chrome.tabs.onUpdated.addListener((tabId, changeInfo, tab) => {
    if (changeInfo.status === 'complete' && /^http/.test(tab.url)) {
        chrome.scripting.executeScript({
            target: { tabId: tabId },
            files: ["src/content.js"]
        })
            .then(() => {
                console.log("Script injected.");
            })
            .catch(err => console.log(err));
    }
});


//document.addEventListener("DOMContentLoaded", function () {
//    console.log("scriptou");
//    //chrome.tabs.getSelected(null, function (tab) {
//    //    chrome.scripting.executeScript({
//    //        target: { tabId: tab.tabId },
//    //        files: ["src/content.js"]
//    //    })
//    //        .then(() => {
//    //            console.log("Script injected.");
//    //        })
//    //        .catch(err => console.log(err));
//    //});
//});