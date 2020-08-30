using System;
using System.Collections.Generic;
using System.Text;


namespace EinsService.Core
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
            for(int c = 0;c<4;c++)
            {
                hold.Add(new Card((CardColor)c, "0", false));
                for(int d=0;d<2;d++)
                {
                    hold.Add(new Card((CardColor)c, "S", false));
                    hold.Add(new Card((CardColor)c, "D", false));
                    hold.Add(new Card((CardColor)c, "R", false));
                    for(int f = 1;f<10;f++)
                    {
                        hold.Add(new Card((CardColor)c, f.ToString(), false));
                    }
                }
            }
            for(int c=0;c<4;c++)
            {
                hold.Add(new Card(CardColor.Black, "W", true));
                hold.Add(new Card(CardColor.Black, "4", true));
            }
           //Shuffle
            Random rng = new Random();
            while (hold.Count>0)   
            {
                Card x = hold[rng.Next(0, hold.Count - 1)];
                Cards.Add(x);
                hold.Remove(x);

            }
        }


    }

    public enum CardColor {Red,Green,Blue,Yellow,Black }

    public class Card
    {
        public CardColor Color { get; set; }
        public string Face { get; set; }
        public bool Wild { get; set; }

        public Card(CardColor c,string f, bool w)
        {
            Color = c;
            Face = f;
            Wild = w;
        }

    }

    public class Player
    {
        public string Name { get; set; }

    }

    public class Room
    {
        public string RoomID { get; set; }
        public Deck RoomDeck { get; set; }
        public Room()
        {
            RoomID = GenID();
            RoomDeck = new Deck();
        }

        private string GenID()
        {
            StringBuilder id = new StringBuilder(4);
            Random r = new Random();
            for(var i = 0; i < 4; i++)
            {
                var c = (char)r.Next(65, 90);
                id.Append(c);
            }
            return id.ToString();
        }



    }

}