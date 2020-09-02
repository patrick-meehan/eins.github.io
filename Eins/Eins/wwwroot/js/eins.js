"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/EinsHub").build();
var roomid = '';
var discardcolor = '';
var discardface = '';
var CanPlayDraw4 = false;
var YoureUp = false;
var wildcolor = '';
var playedwild = '';
var hasDrawn = false;
var leftplayer = '';
var rightplayer = '';
var noticeup = false;
var tablelocked = false;

const NoticeTimeShort = 2500;
const NoticeTimeLong = 5000;
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
});

connection.on("UpdateCanPlayDraw4", function (canplay) { CanPlayDraw4 = canplay; });

connection.on("StartGame", function (left, right) {
    let waiting = document.getElementById("waitingroom");
    waiting.style = "display: none;";
    let rl = document.getElementById("tableroomid");
    rl.innerText = roomid;
    leftplayer = left;
    document.getElementById("leftplayer").innerText = leftplayer;
    rightplayer = right;
    document.getElementById("rightplayer").innerText = rightplayer;
    let tbl = document.getElementById("gametable");
    tbl.style = "display: normal;";
    tablelocked = true;
});

connection.on("EndGame", function (winnote) {
    ShowNotice(winnote);
    setTimeout(CleanUp, NoticeTimeLong);
});

function NameChanged() {
    let nm = $('#userInput').val();
    if (nm != '') {
        $('#roomChoices').show();
    }
    else {
        $('#roomChoices').hide();
    }
}

function RoomIDChanged() {
    let ri = $('#RoomID').val();
    if (ri.length == 4) {
        $('#joinButton').removeAttr("disabled");
    }
    else {
        $('#joinButton').attr("disabled", true);
        
    }
}

$('#userInput').on('input', NameChanged);  //.change(NameChanged);
$('#RoomID').on('input', RoomIDChanged);  //.change(RoomIDChanged);



function CleanUp() {
    let hand = document.getElementById("MyHand");
    hand.textContent = ''; //clear the hand
    //reset to defaults
    CanPlayDraw4 = false;
    YoureUp = false;
    wildcolor = '';
    playedwild = '';
    hasDrawn = false;
    leftplayer = '';
    rightplayer = '';
    let waiting = document.getElementById("waitingroom");
    waiting.style = "display: block;";
    let tbl = document.getElementById("gametable");
    tbl.style = "display: none;";
    tablelocked = false;
}

connection.on("TurnOver", function () {
    YoureUp = false;
    let tbl = document.getElementById("MyHand");
    tbl.style.backgroundColor = "white";

});

connection.on("YourTurn", function () {
    YoureUp = true;
    hasDrawn = false;
    let tbl = document.getElementById("MyHand");
    tbl.style.backgroundColor = "teal";
});

connection.on("Notify", function (notification) {
    ShowNotice(notification);
});

function ShowNotice(note) {
    noticeup = true;
    document.getElementById("notification").innerText = note;
    let notice = document.getElementById("Notice");
    notice.style.display = "block";
    setTimeout(CloseNotice, NoticeTimeShort);

}

connection.on("Reverse", function (dir) {
    document.getElementById("directionarrow").style = "transform: scaleX(" + dir + ")";
    ShowNotice("Reversed!");
});

var CloseNotice = function () {
    let notice = document.getElementById("Notice");
    notice.style.display = "none";
    noticeup = false;

}

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
    if (YoureUp && !hasDrawn) {
        let et = document.getElementById("endTurn");
        et.className = "btn btn-info";
        hasDrawn = true;
        connection.invoke("DrawCard", roomid, 1, null).catch(function (err) {
            return console.error(err.toString());

        });
    }
    event.preventDefault();
});
//TODO: Add scores to users in lobby
document.getElementById("joinButton").addEventListener("click", function (event) {
    let user = document.getElementById("userInput").value;
    let raw = document.getElementById("RoomID").value;
    let joinroom = raw.toUpperCase();
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

var EndTurnClick = function (event) {
    let et = document.getElementById("endTurn");
    et.className = "btn btn-info disabled";
    connection.invoke("PlayCard", roomid + ',0,EndTurn'); //This is a dummy card to signal no card played, but process all else.
}

var CardEvents = function (event) {
    if (YoureUp) {
        let et = document.getElementById("endTurn");
        et.className = "btn btn-info disabled";
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
