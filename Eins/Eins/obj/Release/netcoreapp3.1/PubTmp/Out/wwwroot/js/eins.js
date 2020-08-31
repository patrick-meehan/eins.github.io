﻿"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/EinsHub").build();

//Disable send button until connection is established
document.getElementById("sendButton").disabled = true;

connection.on("ReceiveMessage", function (user, message) {
    var msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
    var encodedMsg = user + " says " + msg;
    var li = document.createElement("li");
    li.textContent = encodedMsg;
    document.getElementById("messagesList").appendChild(li);
});

connection.start().then(function () {
    document.getElementById("sendButton").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

document.getElementById("sendButton").addEventListener("click", function (event) {
    var user = document.getElementById("userInput").value;
    var message = document.getElementById("messageInput").value;
    connection.invoke("SendMessage", user, message).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});

document.getElementById("cardButton").addEventListener("click", function (event) {
    var color = 'Card' + document.getElementById("ColorInput").value;
    var face = 'Card' + document.getElementById("FaceInput").value;
    var div = document.createElement("div");
    div.className = 'Card ' + color + ' ' + face;
    div.innerHTML = '&nbsp;'
    var hand = document.getElementById("MyHand");
    hand.appendChild(div);
});