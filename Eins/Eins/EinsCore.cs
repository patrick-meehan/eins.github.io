﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Microsoft.AspNetCore.Mvc.RazorPages.Infrastructure;
using Microsoft.AspNetCore.SignalR;


namespace Eins.Core
{
    public class Deck
    {
        public List<Card> Cards;
        public Deck()
        {
            Cards = new List<Card>();
            List<Card> hold = new List<Card>();
            //Build the deck.  For each color there is 1 zero and 2 each
            //  of 1-9 plus skip, draw 2 and reverse
            //Then add 4 each of wild and wild draw four
            //There should be 108 cards total.
            int cid = 0;
            for (int c = 0; c < 4; c++)
            {
                hold.Add(new Card(c, "0", false, cid++));
                for (int d = 0; d < 2; d++)
                {
                    hold.Add(new Card(c, "S", false, cid++));
                    hold.Add(new Card(c, "D", false, cid++));
                    hold.Add(new Card(c, "R", false, cid++));
                    for (int f = 1; f < 10; f++)
                    {
                        hold.Add(new Card(c, f.ToString(), false, cid++));
                    }
                }
            }
            for (int c = 0; c < 4; c++)
            {
                hold.Add(new Card(4, "W", true, cid++));
                hold.Add(new Card(4, "-4", true, cid++));
            }
            //Shuffle
            Random rng = new Random();
            while (hold.Count > 0)
            {
                Card x = hold[rng.Next(0, hold.Count - 1)];
                Cards.Add(x);
                hold.Remove(x);
            }
        }

    }

    public class Card
    {
        public static readonly string[] CardColors = { "Red", "Green", "Blue", "Yellow", "Black" };
        private string _color = CardColors[0];
        private string _face = "";
        public string Color { get { return _color; } set { _color = value; CardClass = string.Format("Card Card{0} Card{1}", _face, _color); } }
        public string Face { get { return _face; } set { _face = value; CardClass = string.Format("Card Card{0} Card{1}", _face, _color); } }
        public bool Wild { get; set; }
        public int CardID { get; set; }

        public string CardClass { get; set; }

        public Card(int c, string f, bool w, int id)
        {
            Color = CardColors[c];
            Face = f;
            Wild = w;
            CardID = id;



        }

    }

    public class Player
    {
        public string Name { get; set; }
        public string ID { get; set; }
        public int Score { get; set; }

        public string ToLeft { get; set; }
        public string ToRight { get; set; }

        public bool Active { get; set; }

        public List<Card> Hand = new List<Card>();

        public bool CanPlayDraw4 { get; set; }

        public void CalcPlayDraw4(Card discard)
        {
            CanPlayDraw4 = true;
            foreach (Card c in Hand)
            {
                if ((!c.Wild) && (c.Color == discard.Color))
                {
                    CanPlayDraw4 = false;
                    break;
                }
            }
        }

    }

    public class PlayHistoryEvent
    {
        public string Who { get; set; }
        public Card What { get; set; }

        public PlayHistoryEvent(string who, Card what)
        {
            Who = who;
            What = what;
        }
    }

    public class CardCount
    {
        public string Who { get; set; }
        public int Cards { get; set; }
    }

    public class Room
    {
        const int maxHistoryCount = 10;
        public string RoomID { get; set; }
        public Deck RoomDeck { get; set; }

        public int Direction { get; set; }

        public int PlayerIndex { get; set; }

        public bool RoomLocked { get; set; }

        public List<Card> DiscardPile = new List<Card>();

        public List<Player> Players = new List<Player>();

        public List<PlayHistoryEvent> History = new List<PlayHistoryEvent>();
        public Room()
        {
            RoomID = GenID();
            RoomDeck = new Deck();
            Direction = 1;
            PlayerIndex = 0;
        }

        public void RecordHistory(string Name, Card card)
        {
            History.Add(new PlayHistoryEvent(Name, card));
            if (History.Count >= maxHistoryCount) History.Remove(History[0]);
        }
        public Player EndGame()
        {
            Player winner = ScoreRoom();
            RoomDeck = new Deck();
            Direction = 1;
            PlayerIndex = 0;
            DiscardPile.Clear();
            History.Clear();
            int pcount = Players.Count;
            //Delete all the hands and rotate players
            if (pcount > 1)
            {
                Player first = Players[0];
                first.Hand.Clear();
                for (int p = 1; p < pcount; p++)
                {
                    Players[p].Hand.Clear();
                    Players[p - 1] = Players[p];
                }
                Players[pcount - 1] = first;
            }
            RoomLocked = false;
            return winner;

        }

        public Player ScoreRoom()
        {
            int score = 0;
            int winner = -1;
            for (int p = 0; p < Players.Count; p++)
            {
                Player me = Players[p];
                if (me.Hand.Count == 0) winner = p;
                foreach (Card c in me.Hand)
                {
                    switch (c.Face)
                    {
                        case "S":
                        case "R":
                        case "D":
                            score += 20;
                            break;
                        case "W":
                        case "-4":
                            score += 50;
                            break;
                        default:
                            score += int.Parse(c.Face);
                            break;
                    }
                }
            }
            if (winner > -1)
            {
                Players[winner].Score = score;
                return Players[winner];
            }
            else
            {
                return null;
            }


        }
        private string GenID()
        {
            StringBuilder id = new StringBuilder(4);
            Random r = new Random();
            for (var i = 0; i < 4; i++)
            {
                var c = (char)r.Next(65, 90);
                id.Append(c);
            }
            return id.ToString();
        }

        public void Reshuffle()
        {
            Card topdiscard = DiscardPile[DiscardPile.Count - 1]; // get the last card
            DiscardPile.Remove(topdiscard);
            Random rng = new Random();
            while (DiscardPile.Count > 0)
            {
                Card x = DiscardPile[rng.Next(0, DiscardPile.Count - 1)];
                if (x.Wild) x.Color = "Black"; // reset the wild cards back to black.
                RoomDeck.Cards.Add(x);
                DiscardPile.Remove(x);
            }
            DiscardPile.Add(topdiscard);
        }
        public void NextPlayer()
        {
            int n = PlayerIndex + Direction;
            if (n < 0) { n = Players.Count - 1; }
            if (n >= Players.Count) { n = 0; }
            PlayerIndex = n;
        }

        public List<CardCount> CountCards()
        {
            List<CardCount> counts = new List<CardCount>();
            foreach(Player p in Players)
            {
                if(p.Active)
                {
                    CardCount cc = new CardCount();
                    cc.Who = p.Name;
                    cc.Cards = p.Hand.Count;
                    counts.Add(cc);
                }
            }
            return counts;
        }
        public void SetPlayers()
        {
            Players.RemoveAll(x => !x.Active); 
            //Drop any that dropped when dealing, otherwise
            //  a player could rejoin that does not have any cards
            //  or they get dealt cards and hold up play.
            for (int n = 0; n < Players.Count; n++)
            {
                int l = n - 1;
                if (l < 0) l = Players.Count - 1;
                int r = n + 1;
                if (r == Players.Count) r = 0;
                Players[n].ToLeft = Players[l].Name;
                Players[n].ToRight = Players[r].Name;
            }
        }

    }

}

