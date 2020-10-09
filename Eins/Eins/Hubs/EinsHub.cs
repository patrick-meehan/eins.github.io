using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using Eins.Core;
using System.Collections.Generic;
using System.Threading;
using System.Xml.Linq;
using Microsoft.AspNetCore.Server.HttpSys;
using System.Linq;
using System;
using Microsoft.Identity.Client;
using Azure.Communication.Administration;

namespace Eins.Hubs
{
    public class EinsHub : Hub
    {
        private static List<Room> Rooms = new List<Room>();

        public override Task OnDisconnectedAsync(Exception exception)
        {
            string clientid = Context.ConnectionId;
            bool foundit = false;
            bool RoomOccupied = false;
            Room remove = null;
            foreach (Room r in Rooms)
            {
                foreach (Player p in r.Players)
                {
                    if (p.ID == clientid)
                    {
                        p.Active = false;
                        foundit = true;
                    }
                    RoomOccupied = RoomOccupied || p.Active;
                }
                if (foundit)
                {
                    if (!RoomOccupied) remove = r;
                    break;
                }

            }
            if (remove != null) Rooms.Remove(remove);
            return base.OnDisconnectedAsync(exception);
        }
        public async Task<string> StartRoom(string user)
        {
            Room newroom = new Room();
            Player p = new Player();
            p.Name = user;
            p.ID = Context.ConnectionId;
            p.Active = true;
            newroom.Players.Add(p);
            Rooms.Add(newroom);
            await SendPlayers(newroom.RoomID);
            return newroom.RoomID;

        }

        public string RejoinRoom(string roomid, string user, string stage)
        {
            Room r = Rooms.Find(x => x.RoomID == roomid);
            if (r != null)
            {
                Player p = r.Players.Find(y => y.Name == user);
                if (p != null)
                {
                    p.Active = true;
                    p.ID = Context.ConnectionId; //This will have been updated.
                    SendPlayers(roomid);
                    if (stage == "table")
                    {
                        Card discard = r.DiscardPile[r.DiscardPile.Count - 1];
                        SendPlayers(roomid); // shouldn't bother actives
                        Clients.Client(p.ID).SendAsync("UpdateDiscard", discard, r.PlayerIndex);

                        p.CalcPlayDraw4(discard);
                        Clients.Client(p.ID).SendAsync("UpdateCanPlayDraw4", p.CanPlayDraw4);
                        foreach (Card c in p.Hand)
                        {
                            Clients.Client(p.ID).SendAsync("DealCard", c);
                        }
                        Clients.Client(p.ID).SendAsync("StartGame", p.ToLeft, p.ToRight); //DO NOT UPDATE PLAYERS.
                        if (p.ID == r.Players[r.PlayerIndex].ID) Clients.Client(p.ID).SendAsync("YourTurn");
                    }
                    return "Rejoined";
                }
                else
                {
                    return "NoUser";
                }//found room
            }
            else
            {
                return "NoRoom";
            }
        }
        public async Task<string> JoinRoom(string user, string roomid)
        {
            Room r = Rooms.Find(x => x.RoomID == roomid);
            if (r != null)
            {
                if (r.RoomLocked)
                {
                    return "Locked";
                }
                else
                {
                    if (r.Players.FindAll(x => x.Name == user).Count() > 0) return "NameInUse";
                    Player p = new Player();
                    p.Name = user;
                    p.ID = Context.ConnectionId;
                    p.Active = true;
                    r.Players.Add(p);
                    await SendPlayers(roomid);
                    return "Joined";
                }
            }
            else
            {
                return "Invalid";
            }
        }

        //This method used by both new and join room plus 
        //  can be called for a rejoin operation from the client
        public async Task SendPlayers(string roomid)
        {
            Room r = Rooms.Find(x => x.RoomID == roomid);
            foreach (Player np in r.Players)
            {
                if (np.Active) await Clients.Client(np.ID).SendAsync("Players", r.Players, r.PlayerIndex);
            }
        }

        public void Deal(string roomid)
        {
            Room curroom = Rooms.Find(x => x.RoomID == roomid);
            curroom.RoomLocked = true;
            Deck curdeck = curroom.RoomDeck;
            Card next;
            curroom.SetPlayers();
            foreach (Player p in curroom.Players)
            {

                if (p.Active) Clients.Client(p.ID).SendAsync("StartGame", p.ToLeft, p.ToRight);
            }
            for (int q = 0; q < 7; q++)
            {
                foreach (Player p in curroom.Players)
                {

                    next = curdeck.Cards[0];
                    p.Hand.Add(next);
                    if (p.Active) Clients.Client(p.ID).SendAsync("DealCard", next);
                    curdeck.Cards.RemoveAt(0);
                }
            }
            int c = 0;
            //To avoid complex logic, we skip wilds from the top of deck.
            //  it'll be lucky for the next persons to draw.
            next = curdeck.Cards[0];
            while (next.Wild || "DRS".Contains(next.Face))
            {
                c++;
                next = curdeck.Cards[c];
            }
            curroom.DiscardPile.Add(next);
            curdeck.Cards.RemoveAt(c);

            foreach (Player p in curroom.Players)
            {
                if (p.Active) Clients.Client(p.ID).SendAsync("UpdateDiscard", next, 0); //Current player should be zero to start
            }
            UpdateCanPlayDraw4(roomid);
            Clients.Client(curroom.Players[0].ID).SendAsync("YourTurn");
        }

        public void UpdateCanPlayDraw4(string roomid)
        {
            Room curroom = Rooms.Find(x => x.RoomID == roomid);
            Card discard = curroom.DiscardPile[curroom.DiscardPile.Count - 1];
            foreach (Player p in curroom.Players)
            {
                p.CalcPlayDraw4(discard);
                Clients.Client(p.ID).SendAsync("UpdateCanPlayDraw4", p.CanPlayDraw4);
            }

        }

        public async Task PlayCard(string parms)
        {
            string[] passed = parms.Split(',');
            string roomid = passed[0];
            int cardid = int.Parse(passed[1]);
            string newcolor = passed[2];
            Room curroom = Rooms.Find(x => x.RoomID == roomid);
            Player player = curroom.Players.Find(p => p.ID == Context.ConnectionId);
            await Clients.Client(player.ID).SendAsync("TurnOver");
            Card played;
            if (newcolor != "EndTurn")
            {
                played = player.Hand.Find(c => c.CardID == cardid);
                curroom.RecordHistory(player.Name, played);
                player.Hand.Remove(played);
                played.Color = newcolor;
                curroom.DiscardPile.Add(played);
                UpdateCanPlayDraw4(roomid);
                switch (played.Face)
                {
                    case "R":
                        curroom.Direction *= -1;
                        foreach (Player p in curroom.Players)
                        {
                            if (p.Active) await Clients.Client(p.ID).SendAsync("Reverse", curroom.Direction);
                        }
                        break;
                    case "S":
                        curroom.NextPlayer();
                        await Clients.Client(curroom.Players[curroom.PlayerIndex].ID).SendAsync("Notify", "You were Skipped!");
                        break;
                    case "D":
                        curroom.NextPlayer();
                        await Clients.Client(curroom.Players[curroom.PlayerIndex].ID).SendAsync("Notify", "Draw 2!");
                        DrawCard(roomid, 2, curroom.Players[curroom.PlayerIndex]);
                        break;
                    case "-4":
                        curroom.NextPlayer();
                        await Clients.Client(curroom.Players[curroom.PlayerIndex].ID).SendAsync("Notify", "Have some more cards! +4 to you.");
                        DrawCard(roomid, 4, curroom.Players[curroom.PlayerIndex]);
                        break;
                }
            }
            else
            {
                curroom.RecordHistory(player.Name, null);
                played = curroom.DiscardPile[curroom.DiscardPile.Count - 1];
            }
            curroom.NextPlayer();
            List<CardCount> cc = curroom.CountCards();
            foreach (Player p in curroom.Players)
            {
                if (p.Active)
                {
                    await Clients.Client(p.ID).SendAsync("UpdateDiscard", played, curroom.PlayerIndex);
                    await Clients.Client(p.ID).SendAsync("UpdateHistory", curroom.History, cc);
                }
            }
            if (player.Hand.Count() > 0)
            {
                await Clients.Client(curroom.Players[curroom.PlayerIndex].ID).SendAsync("YourTurn");
            }
            else
            {
                Player winner = curroom.EndGame();
                foreach (Player p in curroom.Players)
                {
                    string note;
                    if (p.ID == winner.ID)
                    {
                        note = string.Format("You won!{0}Score:{1}", System.Environment.NewLine, winner.Score);
                    }
                    else
                    {
                        note = string.Format("Player {0} won this game.", winner.Name);
                    }
                    if (p.Active) await Clients.Client(p.ID).SendAsync("EndGame", note);

                }
                await SendPlayers(roomid);
            }

        }

        public void DrawCard(string roomid, int howMany, Player player = null)
        {
            Room curroom = Rooms.Find(x => x.RoomID == roomid);
            if (player == null) { player = curroom.Players.Find(p => p.ID == Context.ConnectionId); }
            for (int c = 0; c < howMany; c++)
            {
                if (curroom.RoomDeck.Cards.Count == 0)
                {
                    foreach (Player p in curroom.Players)
                    {
                        Clients.Client(p.ID).SendAsync("Notify", "Deck empty, reshuffling."); //note if you change this wording, update the notice in eins.js
                    }
                    curroom.Reshuffle();
                }//We won't need to update the discard to the clients since it did not change.
                Card next = curroom.RoomDeck.Cards[0];
                player.Hand.Add(next);
                Clients.Client(player.ID).SendAsync("DealCard", next);
                curroom.RoomDeck.Cards.RemoveAt(0);
            }
        }
#if(DEBUG)
        public void Debug()
        {
            //dummy method to allow for inspecting hub state
            int wait = 0;
        }
#endif
    }
}