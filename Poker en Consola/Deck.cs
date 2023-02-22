using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poker_en_Consola
{
    public class Deck
    {

        public List<Card> deck {get;}

        public Deck()
        {

            this.deck = new List<Card>();
            this.deck = fillDeck(deck);
        }

        public Card getCard(int id) ///Devuelve una carta del 1 al 52 en el mazo
        {
            if (id >= 0 && id < 52)
                return deck[id];
            else
                return null;
        }



        public List<Card>  fillDeck(List<Card> deck) ///devuelve un mazo de cartas nuevo y ordenado del As de trebol al K de diamantes
        {
            Card a = null;
              
            for(int i=1; i<14; i++)
                {
                a = new Card(i, Suit.Club);
                deck.Add(a);
                a = new Card(i, Suit.Heart);
                deck.Add(a);
                a = new Card(i, Suit.Spade);
                deck.Add(a);
                a = new Card(i, Suit.Diamond);
                deck.Add(a);
                 }

            return deck;
        }

        public void ShuffleDeck() /// Function to Shuffle the deck 
            {
                Random rnd = new Random();

                for (int i = 0; i < deck.Count; i++)
                    {
                    int rndIndex = rnd.Next(deck.Count);
                    Card temp = deck[i];
                    deck[i] = deck[rndIndex];
                    deck[rndIndex] = temp;
                    }
            }


    }
}
