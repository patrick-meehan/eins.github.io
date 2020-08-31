using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using Eins.Core;
using System.Collections.Generic;
using System.Threading;
using System.Xml.Linq;
using Microsoft.AspNetCore.Server.HttpSys;
using System.Linq;
using System;

namespace Eins.Hubs
{
    public class EinsHub : Hub
    {
        private static List<Room> Rooms = new List<Room>();
      
        public async Task<string> StartRoom(string user)
        {
            Room newroom = new Room();
            Player p = new Player();
            p.Name = user;
            p.ID = Context.ConnectionId;
            newroom.Players.Add(p);
            Rooms.Add(newroom);
            await Clients.Caller.SendAsync("PlayerAdded", user);
            return newroom.RoomID;

        }
        public async Task<bool> JoinRoom(string user, string roomid)
        {
            Room r = Rooms.Find(x => x.RoomID == roomid);
            if (r != null)
            {
                Player p = new Player();
                p.Name = user;
                p.ID = Context.ConnectionId;
                r.Players.Add(p);
                foreach(Player np in r.Players)
                {
                    await Clients.Client(np.ID).SendAsync("Players", r.Players);
                }
                //await Clients.All.SendAsync("Players", r.Players);
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Deal(string roomid)
        {
            Room curroom = Rooms.Find(x => x.RoomID == roomid);
            Deck curdeck = curroom.RoomDeck;
            Card next;

            foreach(Player p in curroom.Players)
            {
                Clients.Client(p.ID).SendAsync("StartGame");
            }
            for (int q = 0; q < 7; q++)
            {
                foreach (Player p in curroom.Players)
                {
                    next = curdeck.Cards[0];
                    p.Hand.Add(next);
                    Clients.Client(p.ID).SendAsync("DealCard", next);
                    curdeck.Cards.RemoveAt(0);
                }
            }
            int c = 0;
            //To avoid complex logic, we skip wilds from the top of deck.
            //  it'll be lucky for the next persons to draw.
            next = curdeck.Cards[0];
            while (next.Wild)
            {
                c++;
                next = curdeck.Cards[c];
            }
            curroom.DiscardPile.Add(next);
            curdeck.Cards.RemoveAt(c);

            foreach (Player p in curroom.Players)
            {
                Clients.Client(p.ID).SendAsync("UpdateDiscard", next);
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

        public void PlayCard(string parms)
        {
            string[] passed = parms.Split(',');
            string roomid = passed[0];
            int cardid = int.Parse( passed[1]);
            string newcolor = passed[2];
            Room curroom = Rooms.Find(x => x.RoomID == roomid);
            Player player = curroom.Players.Find(p => p.ID == Context.ConnectionId);
            Card played = player.Hand.Find(c => c.CardID == cardid);
            player.Hand.Remove(played);           
            played.Color = newcolor;
            curroom.DiscardPile.Add(played);
            foreach (Player p in curroom.Players)
            {
                Clients.Client(p.ID).SendAsync("UpdateDiscard", played);
            }

            UpdateCanPlayDraw4(roomid);
            Clients.Client(player.ID).SendAsync("TurnOver");
            switch (played.Face)
            {
                case "R":
                    curroom.Direction *= -1;
                    break;
                case "S":
                    curroom.NextPlayer();
                    break;
                case "D":
                    curroom.NextPlayer();
                    DrawCard(roomid, 2, curroom.Players[curroom.PlayerIndex]);
                    break;
                case "-4":
                    curroom.NextPlayer();
                    DrawCard(roomid, 4, curroom.Players[curroom.PlayerIndex]);
                    break;
            }
            curroom.NextPlayer();
            Clients.Client(curroom.Players[curroom.PlayerIndex].ID).SendAsync("YourTurn");        
        
        }

        public void DrawCard(string roomid, int howMany, Player player = null)
        {
            Room curroom = Rooms.Find(x => x.RoomID == roomid);
            if (player == null) { player = curroom.Players.Find(p => p.ID == Context.ConnectionId); }
            for (int c = 0; c < howMany; c++)
            {
                if (curroom.RoomDeck.Cards.Count == 0) curroom.Reshuffle();  //We won't need to update the discard to the clients since it did not change.
                Card next = curroom.RoomDeck.Cards[0];
                player.Hand.Add(next);
                Clients.Client(player.ID).SendAsync("DealCard", next);
                curroom.RoomDeck.Cards.RemoveAt(0);
            }
        }
    }
}