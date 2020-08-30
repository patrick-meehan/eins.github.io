"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/EinsHub").build();
var roomid = '';
var discardcolor = '';
var discardface = '';
var CanPlayDraw4 = false;
var YoureUp = false;
var wildcolor = '';
var playedwild = '';
//TODO: Setup YoureUP logic
//Disable send button until connection is established
document.getElementById("newbutton").disabled = true;


connection.on("Players", function (something) {
    $("#Players").empty();
    $(something).each(function () {
        $("#Players").append("<li>" + this.name + "</li>");
    });
});

connection.on("UpdateDiscard", function (card) {
    discardcolor = card.color;
    discardface = card.face;
    let dp = document.getElementById("discardPile");
    dp.className = card.cardClass;

})

connection.on("UpdateCanPlayDraw4", function (canplay) { CanPlayDraw4 = canplay; });

connection.on("StartGame", function () {
    let waiting = document.getElementById("waitingroom");
    waiting.style = "display: none;";
    let tbl = document.getElementById("gametable");
    tbl.style = "displayL normal;";
});


connection.on("TurnOver", function () {
    YoureUp = false;
    let tbl = document.getElementById("MyHand");
    tbl.style.backgroundColor = "white";

});

connection.on("YourTurn", function () {
    YoureUp = true;
    let tbl = document.getElementById("MyHand");
    tbl.style.backgroundColor = "grey";

});



connection.on("DealCard", function (card) {
    let div = document.createElement("div");
    div.className = card.cardClass;
    div.innerHTML = '&nbsp;';
    div.dataset.cardcolor = card.color;
    div.dataset.cardface = card.face;
    div.dataset.cardwild = card.wild;
    div.dataset.cardID = card.cardID;
    div.addEventListener("click", CardEvents);
    let hand = document.getElementById("MyHand");
    hand.appendChild(div);
});

connection.start().then(function () {
    document.getElementById("newbutton").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

function GoIntoWiating() {
    $("#Lobby").hide();
    $("#waitingroom").show();
}

document.getElementById("newbutton").addEventListener("click", function (event) {
    var user = document.getElementById("userInput").value;
    connection.invoke("StartRoom", user).then(function (result) {
        var lbl = document.getElementById("roomcode");
        lbl.innerHTML = result;
        roomid = result;
        GoIntoWiating();
    }).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});

document.getElementById("deck").addEventListener("click", function (event) {
    if (YoureUp) {
        connection.invoke("DrawCard", roomid, 1, null).catch(function (err) {
            return console.error(err.toString());

        });
    }
    event.preventDefault();
});

document.getElementById("joinButton").addEventListener("click", function (event) {
    var user = document.getElementById("userInput").value;
    var joinroom = document.getElementById("messageInput").value;
    connection.invoke("JoinRoom", user, joinroom).then(function (result) {
        if (result) {
            roomid = joinroom;
            GoIntoWiating();
        }
        else {
            alert("Not a Room!");
        };
    }).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});

document.getElementById("dealbutton").addEventListener("click", function (event) {
    connection.invoke("Deal", roomid).catch(function (err) {
        return console.error(err.toString());
    });

    event.preventDefault();
});

var CardEvents = function (event) {
    if (YoureUp) {
        let playedcard = event.srcElement;
        let cid = playedcard.dataset.cardID;
        WildIf: if (playedcard.dataset.cardwild == "true") {
            if (playedcard.dataset.cardface == '-4' && CanPlayDraw4 == false) break WildIf;
            wildcolor = '';
            playedwild = playedcard;
            let modal = document.getElementById("myModal");
            let span = document.getElementsByClassName("close")[0];
            span.onclick = function () {
                modal.style.display = "none";
            }
            modal.style.display = "block";
            //TODO: Finish this up in the modal close out code.
        }
        else {
            let ccolor = playedcard.dataset.cardcolor;
            if ((ccolor == discardcolor) || (playedcard.dataset.cardface == discardface)) {

                let parms = roomid + ',' + cid + ',' + ccolor;

                connection.invoke("PlayCard", parms).then(function () {
                    let hand = document.getElementById("MyHand");
                    hand.removeChild(playedcard);
                }).catch(function (err) {
                    return console.error(err.toString());
                });
            }
        }
        //TODO: Add logic to select wild color

        let bc = 3;
        //TODO: Else signal not an appropriate card.
    }
}

var WildColorClick = function (newcolor) {
    wildcolor = newcolor;
    let cid = playedwild.dataset.cardID;
    let parms = roomid + ',' + cid + ',' + wildcolor;
    connection.invoke("PlayCard", parms).then(function () {
        let hand = document.getElementById("MyHand");
        hand.removeChild(playedwild);
    }).catch(function (err) {
        return console.error(err.toString());
    });
    let modal = document.getElementById("myModal");
    modal.style.display = "none";
}
