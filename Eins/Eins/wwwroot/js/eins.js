"use strict";
const NoticeTimeShort = 1700;
const NoticeTimeLong = 5000;

var connection = new signalR.HubConnectionBuilder().withUrl("/EinsHub").withAutomaticReconnect().build();
var currentscreen = window.sessionStorage.getItem("CurrentScreen"); // lobby, waiting, table
var myname = window.sessionStorage.getItem("MyName");
var roomid = window.sessionStorage.getItem("RoomID");
var discardcolor = '';
var discardface = '';
var CanPlayDraw4 = false;
var YoureUp = false;
var wildcolor = '';
var playedwild = '';
var hasDrawn = false;
var noticeup = false;
var tablelocked = false;


//Disable send button until connection is established
document.getElementById("newbutton").disabled = true;

//window.addEventListener("beforeunload", function (e) {
//    if (tablelocked == true) {
//        e.preventDefault(); 
//         return "Please don't navigate away, you will leave the current game";
//    }
//    e.returnValue = '';
//});

function debug() {
    let wait = 0;
    connection.invoke("Debug");
}

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

function CleanUp() {
    $('#MyHand').empty();//clear the hand
    $('#History').empty();
    $('#counts').empty();
    //reset to defaults
    CanPlayDraw4 = false;
    YoureUp = false;
    wildcolor = '';
    playedwild = '';
    hasDrawn = false;
    document.getElementById("directionarrow").style = 'transform: scaleX("1")';
    window.sessionStorage.setItem("Dir", 1);
    $('#waitingroom').show();
    $('#gametable').hide();
    $('#endTurn').hide();
    tablelocked = false;
}

function ShowNotice(note, t) {
    if (note == "Deck empty, reshuffling.") { $('#deck').hide(); }
    noticeup = true;
    $('#notification').text(note);
    $('#Notice').show();
    setTimeout(CloseNotice, t);

}

function GoIntoWiating() {
    $("#Lobby").hide();
    $("#waitingroom").show();
    currentscreen = "waiting";
    window.sessionStorage.setItem("CurrentScreen", "waiting");
}

var CloseNotice = function () {
    $('#Notice').hide();
    noticeup = false;
    $('#deck').show(); //just in case it was hidden for reshuffle

};

var EndTurnClick = function (event) {
    $('#endTurn').hide();
    connection.invoke("PlayCard", roomid + ',0,EndTurn'); //This is a dummy card to signal no card played, but process all else.
};

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
    }
};

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
};

connection.on("Players", function (playerList, current) {
    $('.carousel').carousel('pause');
    $('.carousel').carousel(0);
    $("#Players").empty();
    $('#currentPlayerList').empty();
    let first = 0;
    $(playerList).each(function () {
        //$("#Players").append("<li>" + this.name + "</li>");
        let score = (this.score > 0) ? this.score.toString() : '';
        let elem = '<tr><td>' + this.name + '</td><td>' + score + '</td></tr>';
        $('#Players').append(elem);
        if (first == 0) {
            $('#currentPlayerList').append('<div class="carousel-item active">' + this.name + '</div>');
            first = 1;
        }
        else {
            $('#currentPlayerList').append('<div class="carousel-item">' + this.name + '</div>');
        }
    });
    $('.carousel').carousel('pause');
    $('.carousel').carousel(current);
});

connection.on("UpdateDiscard", function (card, currentPlayer) {
    discardcolor = card.color;
    discardface = card.face;
    let dp = document.getElementById("discardPile");
    dp.className = card.cardClass;
    $('.carousel').carousel('pause');
    $('.carousel').carousel(currentPlayer);
});

connection.on("UpdateCanPlayDraw4", function (canplay) { CanPlayDraw4 = canplay; });

connection.on("StartGame", function (left, right) {
    $('#waitingroom').hide();
    $('#tableroomid').text(myname + ' in Room:' + roomid);
    $('#leftplayer').text(left);
    $('#rightplayer').text(right);
    $('#gametable').show();
    window.sessionStorage.setItem("CurrentScreen", "table");
    tablelocked = true;
});

connection.on("EndGame", function (winnote) {
    ShowNotice(winnote, NoticeTimeLong);
    setTimeout(CleanUp, NoticeTimeLong + 500);
});

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

connection.on("Reverse", function (dir) {
    document.getElementById("directionarrow").style = "transform: scaleX(" + dir + ")";
    window.sessionStorage.setItem("Dir", dir);
    ShowNotice("Reversed!", NoticeTimeShort);
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

connection.on("UpdateHistory", function (history, cardcounts) {
    $('#History').empty();
    $('#counts').empty();
    for (let i = history.length - 1; i > -1; i--) {
        let name = history[i].who;
        let what = history[i].what;
        let entry = '';
        let div = document.createElement("div");
        div.classList.add('history');
        if (what == null) {
            div.innerText = name + ' did not play a card.';
        }
        else {
            div.innerText = name + ' played ';
            let cdiv = document.createElement("span");
            cdiv.innerHTML = '&nbsp;';
            let hclass = what.cardClass.replace(/Card/g, "historyCard");
            cdiv.className = hclass;
            div.appendChild(cdiv);
        }
        let hist = document.getElementById("History");
        hist.appendChild(div);
    }
    let tbl = document.createElement("table");
    for (let c = 0; c < cardcounts.length; c++) {
        let r = tbl.insertRow();
        let n = r.insertCell(0);
        let nc = r.insertCell(1);
        n.innerText = cardcounts[c].who;
        nc.innerText = cardcounts[c].cards;
        //let entry = '<tr><td>' +  + '</td><td>' +  + '</td></tr>'
        //tbl.appendChild(entry);
    }
    let cc = document.getElementById("counts");
    cc.appendChild(tbl);

});

connection.start().then(function () {
    document.getElementById("newbutton").disabled = false;
    CheckRestart();
}).catch(function (err) {
    return console.error(err.toString());
});

document.getElementById("newbutton").addEventListener("click", function (event) {
    myname = document.getElementById("userInput").value;
    connection.invoke("StartRoom", myname).then(function (result) {
        $('#roomcode').text(result);
        roomid = result;
        window.sessionStorage.setItem("RoomID", result);
        GoIntoWiating();
    }).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});

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
            case "NameInUse":
                ShowNotice("Screen name already present in " + joinroom + ".  Please choose a different name.", NoticeTimeLong);
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

$('#userInput').on('input', NameChanged);
$('#RoomID').on('input', RoomIDChanged);
$('#endTurn').hide();

$('#userInput').blur(function () {
    window.sessionStorage.setItem("MyName", $('#userInput').val());
});

//if roomid isn't blank, then we need to call the server and setup the page.

function CheckRestart() {
    if (currentscreen == null) {
        //new session
        currentscreen = "lobby";
        window.sessionStorage.setItem("CurrentScreen", currentscreen);
    }
    else {
        $('#userInput').val(myname);
        NameChanged();
        $('RoomID').val(roomid);
        RoomIDChanged();
        //reload session here



        //Change in logic.  Rather than control from client, set state in room
        //   Between deal and endgame, the room is locked
        //   which is the same state as the table showing
        //   
        //   There are also two rejoin scenarios:
        //    1. simple refresh, so sessionStorage has values
        //    2. browser close.
        //   for opt 1, there are values in sessstorage so we can auto rejoin here.
        //   opt 2 would require taking action in the lobby
        //   There is also the issue of a game halting because someone dropped.  
        //   If an inactive player is up next, then there should be a waiting period before 
        //     dropping the player from the game.


        //   Random thought, perhaps we should setup an expiration period to keep room active for 10 minutes.
        //     this would allow for a rejoin of a room, but then it eventually clean up.
        switch (currentscreen) {
            case "waiting":
                //need to move to the waiting and pull names
                connection.invoke("RejoinRoom", roomid, myname, currentscreen);
                GoIntoWiating();
                break;
            case "table":
                let dir = window.sessionStorage.getItem("Dir");
                document.getElementById("directionarrow").style = "transform: scaleX(" + dir + ")";
                connection.invoke("RejoinRoom", roomid, myname, currentscreen).then(function (result) {
                    //TODO: Process results.
                });
                //need to restore the whole hand
                break;
        }
    }
}


