"use strict";

const userEmail = document.getElementById("messageIcon").getAttribute("data-user-email")
var connection = new signalR.HubConnectionBuilder().withUrl("/messageHub").build();

connection.on(userEmail, function () {
    GetUnreadedMsgCount()
});

connection.start().then(function () {
    GetUnreadedMsgCount()
}).catch(function (err) {
    return console.error(err.toString());
});

function GetUnreadedMsgCount() {
    let msgLink = document.getElementById("messageLink")
    connection.invoke("GetUnreadedMessagesCount", userEmail).then(function (count) {
        ChangeCount(count)
    }).catch(function (err) {
        return console.error(err.toString());
    })
}

function ChangeCount(count) {
    let span = document.getElementById("msgCount")
    if (count == 0) {
        span.classList.add("d-none")
        span.innerText = ""
    }
    else {
        span.classList.remove("d-none")
        span.innerText = count
    }
}