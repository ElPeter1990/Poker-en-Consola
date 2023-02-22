using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poker_en_Consola
{
    public class Card 
    {

        public int rank { get; set; }
        public Suit suit { get; set; }
        
        
        public Card(int numericValue, Suit palo )
            {

                this.rank = numericValue;
                this.suit = palo;

            }

        public String getStringValue(int rank)
        {
            string stringValue = null;
            if (rank == 1)
                stringValue= "A";
            if (rank == 2)
                stringValue= "2";
            if (rank == 3)
                stringValue= "3";
            if (rank == 4)
                stringValue= "4";
            if (rank == 5)
                stringValue= "5";
            if (rank == 6)
                stringValue= "6";
            if (rank == 7)
                stringValue= "7";
            if (rank == 8)
                stringValue= "8";
            if (rank == 9)
                stringValue= "9";
            if (rank == 10)
                stringValue= "10";
            if (rank == 11)
                stringValue ="J";
            if (rank == 12)
                stringValue= "Q";
            if (rank == 13)
                stringValue ="K";
            if (rank == 14)
                stringValue ="Ace";

            return stringValue;
        }

        public int comparativeValue //Para darle al As lo mismo el 1 que el 14 en dependencia de la situacion en concreto
        {
            get
            {
                if (rank == 1)
                    return 14;
                else
                    return rank;
            }
        }


    }

}
