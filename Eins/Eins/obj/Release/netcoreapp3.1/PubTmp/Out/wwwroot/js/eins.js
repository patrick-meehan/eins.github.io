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
var noticeup = false;
var tablelocked = false;
var myname = '';

const NoticeTimeShort = 1700;
const NoticeTimeLong = 5000;


function debug() {
    let wait = 0;
    connection.invoke("Debug");
}

//Disable send button until connection is established
document.getElementById("newbutton").disabled = true;

connection.on("Players", function (playerList) {
    $("#Players").empty();
    $('#currentPlayerList').empty();
    let first = 0;
    $(playerList).each(function () {
        $("#Players").append("<li>" + this.name + "</li>");
        if (first == 0) {
            $('#currentPlayerList').append('<div class="carousel-item active">' + this.name + '</div>');
            first = 1;
        }
        else {
            $('#currentPlayerList').append('<div class="carousel-item">' + this.name + '</div>');
        }
    });
    $('.carousel').carousel('pause');
});

connection.on("UpdateDiscard", function (card,currentPlayer) {
    discardcolor = card.color;
    discardface = card.face;
    let dp = document.getElementById("discardPile");
    dp.className = card.cardClass;
    $('#nowPlaying').carousel(currentPlayer);
});

connection.on("UpdateCanPlayDraw4", function (canplay) { CanPlayDraw4 = canplay; });

connection.on("StartGame", function (left, right) {
    $('#waitingroom').hide();
    $('#tableroomid').text(myname + ' in Room:' + roomid);
    $('#leftplayer').text(left);
    $('#rightplayer').text(right);
    $('#gametable').show();
    tablelocked = true;
});

connection.on("EndGame", function (winnote) {
    ShowNotice(winnote, NoticeTimeLong);
    setTimeout(CleanUp, NoticeTimeLong + 500);
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

$('#userInput').on('input', NameChanged);
$('#RoomID').on('input', RoomIDChanged);
$('#endTurn').hide();

function CleanUp() {
    $('#MyHand').empty();//clear the hand
    //reset to defaults
    CanPlayDraw4 = false;
    YoureUp = false;
    wildcolor = '';
    playedwild = '';
    hasDrawn = false;
    $('#waitingroom').show();
    $('#gametable').hide();
    $('#endTurn').hide();
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
    ShowNotice(notification, NoticeTimeShort);
});

function ShowNotice(note, t) {
    noticeup = true;
    $('#notification').text(note);
    $('#Notice').show();
    setTimeout(CloseNotice, t);

}

connection.on("Reverse", function (dir) {
    document.getElementById("directionarrow").style = "transform: scaleX(" + dir + ")";
    ShowNotice("Reversed!", NoticeTimeShort);
});

var CloseNotice = function () {
    $('#Notice').hide();
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
    myname = document.getElementById("userInput").value;
    connection.invoke("StartRoom", myname).then(function (result) {
        $('#roomcode').text(result);
        roomid = result;
        GoIntoWiating();
    }).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});

document.getElementById("deck").addEventListener("click", function (event) {
    if (YoureUp && !hasDrawn) {
        $('#endTurn').show();
        hasDrawn = true;
        connection.invoke("DrawCard", roomid, 1, null).catch(function (err) {
            return console.error(err.toString());
        });
    }
    event.preventDefault();
});
//TODO: Add scores to users in lobby
document.getElementById("joinButton").addEventListener("click", function (event) {
    myname = $('#userInput').val();
    let raw = $("#RoomID").val();
    let joinroom = raw.toUpperCase();
    connection.invoke("JoinRoom", myname, joinroom).then(function (result) {
        switch (result) {
            case "Joined":
                roomid = joinroom;
                $('#roomcode').text(roomid);
                GoIntoWiating();
                break;
            case "Locked":
                ShowNotice("Sorry, this game is in progress.  Wait for the current round to end.", NoticeTimeLong);
                break;
            case "Invalid":
                ShowNotice("Can't find room " + joinroom + '! Check spelling.', NoticeTimeLong);
                break;
        }
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
    $('#endTurn').hide();
    connection.invoke("PlayCard", roomid + ',0,EndTurn'); //This is a dummy card to signal no card played, but process all else.
}

var CardEvents = function (event) {
    if (YoureUp) {
        $('#endTurn').hide();
        let playedcard = event.srcElement;
        let cid = playedcard.dataset.cardID;
        WildIf: if (playedcard.dataset.cardwild == "true") {
            if (playedcard.dataset.cardface == '-4' && CanPlayDraw4 == false) break WildIf;
            wildcolor = '';
            playedwild = playedcard;
            let span = document.getElementsByClassName("close")[0];
            span.onclick = function () {
                $('#myModal').hide();
            }
            $('#myModal').show();
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
