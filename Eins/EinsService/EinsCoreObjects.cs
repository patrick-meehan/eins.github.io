using System;
using System.Collections.Generic;
using System.Text;


namespace EinsService.Core
{
    public class Deck
    {

    }

    public class Player
    {
        public string Name { get; set; }

    }

    public class Room
    {
        public string RoomID { get; set; }
        public Room()
        {
            RoomID = GenID();
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