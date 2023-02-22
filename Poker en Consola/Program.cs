using Poker_en_Consola;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

internal class Program
{
    private static void Main(string[] args)
    {



            Console.WriteLine("Presione una tecla para crear mesa...");
                ConsoleKeyInfo teclaPresionada = Console.ReadKey();

            PokerTable table = null;
            List<PokerPlayer> players = new List<PokerPlayer>();
        PokerPlayer player1 = new PokerPlayer("El Mago", 5, Actions.None, true);
        PokerPlayer player2 = new PokerPlayer("Huron Perdedor", 5, Actions.None, true);
        PokerPlayer player3 = new PokerPlayer("Marco", 8, Actions.None, true);
        PokerPlayer player4 = new PokerPlayer("Jimagua", 9, Actions.None, true);
        PokerPlayer player5 = new PokerPlayer("Peter", 10, Actions.None, true);
        PokerPlayer player6 = new PokerPlayer("Yohan",11, Actions.None, true);


            players.Add(player1);
            players.Add(player2);
            players.Add(player3);
            players.Add(player4);
            players.Add(player5);
            players.Add(player6);

        PokerGameModes gameMode = PokerGameModes.Texas;
            decimal bb = 0.10m;
            table = new PokerTable(1,gameMode, 6, bb, 2, 10, players, true);

            Console.WriteLine(" Nueva Mesa de: {0}, Asientos: {1}, nivel: ${2}", table.gameMode, table.seats, (table.bb)*100);
        Console.WriteLine("Presione una tecla para comenzar...");
          teclaPresionada = Console.ReadKey();

        /*
        //TESTING
        List<Card> board = new List<Card>();
        Card c1 = new Card(6, Suit.Club);
        Card c2 = new Card(7, Suit.Heart);
        Card c3 = new Card(8, Suit.Diamond);
        Card c4 = new Card(9, Suit.Heart);
        Card c5 = new Card(1, Suit.Diamond);
        board.Add(c1);
        board.Add(c2);
        board.Add(c3);
        board.Add(c4);
        board.Add(c5);

        List<Card> player = new List<Card>();
        c1 = new Card(1, Suit.Heart);
        c2 = new Card(8, Suit.Club);
        player.Add(c1);
        player.Add(c2);

        List<Card> twoPair = player1.twoPair(board, player);
        //TESTING   
        */

        DBController dataBase = new DBController(); 
       /*
        List<PokerPlayer> allPlayers = dataBase.GetAllPlayers();
        foreach(PokerPlayer player in allPlayers)
        {
            Console.WriteLine("Nickname: {0}", player.nickname); 
            Console.WriteLine();
        }*/

      //  dataBase.updatePlayerPlayMoney(player5);

        PokerDealer dealer = new PokerDealer(table);

        /*
            Console.WriteLine("Presione una tecla para mostrar mazo barajeado...");
            teclaPresionada = Console.ReadKey();
        Console.WriteLine("  ");
        Deck myDeck1 = table.deck;

                for (int i = 0; i < 52; i++)
                {
                    Console.WriteLine("Carta: {0} of {1}", myDeck1.getCard(i).getStringValue(myDeck1.getCard(i).rank), myDeck1.getCard(i).suit);
                }
                 

            Console.WriteLine("Presione una tecla para repartir cartas...");
            teclaPresionada = Console.ReadKey();
        Console.WriteLine("");
        table.DealCards();
        Console.WriteLine("......Jugadores activos.......");

        for (int i = 0; i<players.Count; i++)
            {
            if (players[i].active == true) { //if player is not sitting out...
                int rank1 = players[i].cards[0].rank;
                int rank2 = players[i].cards[1].rank;
                Console.WriteLine("......................................................");    
                Console.WriteLine("/ Nombre: {0}, STACK: ${1}, Posicion: {2}         /", players[i].nickname, players[i].stack, i);
                Console.WriteLine("/ {0} of {1}, {2} of {3}                            /", players[i].cards[0].getStringValue(rank1), players[i].cards[0].suit, players[i].cards[1].getStringValue(rank2), players[i].cards[1].suit);
            Console.WriteLine("/...................................................../");
            }
            }
        Console.WriteLine("");

        Console.WriteLine("Presione una tecla para repartir el board...");
        teclaPresionada = Console.ReadKey();

        Console.WriteLine("");

        table.DealBoard();
        for (int i=0; i<table.board.Count; i++)
        {
            if(i == 0)
            {
                Console.Write("/ Flop ----> ");
                Console.WriteLine("{0} of {1}, {2} of {3}, {4} of {5} /", table.board[i].getStringValue(table.board[i].rank), table.board[i].suit, table.board[i+1].getStringValue(table.board[i+1].rank), table.board[i+1].suit, table.board[i+2].getStringValue(table.board[i+2].rank), table.board[i+2].suit);
                Console.WriteLine("");

                foreach(PokerPlayer player in table.players){ //I will run this list from sb to the botton
                    if (player.active)
                    {
                        Console.WriteLine("Presione una tecla para ver la mejor mano del jugador: {0}", player.nickname);
                        teclaPresionada = Console.ReadKey();
                        Console.WriteLine("");
                        player.hand.Clear(); //Im cleaning the last deducted hand of this player because i will deduct a new hand on this street of game
                        List<Card> flop = table.board.Take(3).ToList(); //Just the flop this time..
                        Hands hand = player.myHandIs(flop, table.gameMode);
                        Console.Write(hand + ": ");
                        foreach(Card card in player.hand)
                        {
                            Console.Write("{0} of {1}, ", card.getStringValue(card.rank), card.suit);
                        }
                        Console.WriteLine("");
                        Console.WriteLine("");
                    }
                 }
                Console.WriteLine("");
                i += 2;
            }
            if (i == 3)
            {
                Console.Write("/ Turn ----> ");
                Console.WriteLine("{0} of {1} /", table.board[i].getStringValue(table.board[i].rank), table.board[i].suit);
                Console.WriteLine("");

                foreach (PokerPlayer player in table.players)
                { //I will run this list from sb to the botton
                    if (player.active)
                    {
                        Console.WriteLine("Presione una tecla para ver la mejor mano del jugador: {0}", player.nickname);
                        teclaPresionada = Console.ReadKey();
                        Console.WriteLine("");
                        player.hand.Clear(); //Im cleaning the last deducted hand of this player because i will deduct a new hand on this street of game
                        List<Card> flopNTurn = table.board.Take(4).ToList(); //Just flop and turn this time..
                        Hands hand = player.myHandIs(flopNTurn, table.gameMode);
                        Console.Write(hand + ": ");
                        foreach (Card card in player.hand)
                        {
                            Console.Write("{0} of {1}, ", card.getStringValue(card.rank), card.suit);
                        }
                        Console.WriteLine("");
                        Console.WriteLine("");
                    }
                }
                Console.WriteLine("");
            }
            if (i == 4)
            {
                Console.Write("/ River ----> ");
                Console.WriteLine("{0} of {1} /", table.board[i].getStringValue(table.board[i].rank), table.board[i].suit);
                Console.WriteLine("");

                foreach (PokerPlayer player in table.players)
                { //I will run this list from sb to the botton
                    if (player.active)
                    {
                        Console.WriteLine("Presione una tecla para ver la mejor mano del jugador: {0}", player.nickname);
                        teclaPresionada = Console.ReadKey();
                        Console.WriteLine("");
                        player.hand.Clear(); //Im cleaning the last deducted hand of this player because i will deduct a new hand on this street of game
                        List<Card> flopNTurnNRiver = table.board.Take(5).ToList(); //All cards this time
                        Hands hand = player.myHandIs(flopNTurnNRiver, table.gameMode);
                        Console.Write(hand + ": ");
                        foreach (Card card in player.hand)
                        {
                            Console.Write("{0} of {1}, ", card.getStringValue(card.rank), card.suit);
                        }
                        Console.WriteLine("");
                        Console.WriteLine("");
                    }
                }

                Console.WriteLine("");
            }
            


            
        }
        
        Console.WriteLine("");
        Console.WriteLine("Presione una tecla para cerrar...");
        teclaPresionada = Console.ReadKey();

        */
        /*
        Card c = new Card(11, Suit.Diamond);
        Card d = new Card(4, Suit.Diamond);
        List<Card> cards1 = new List<Card>();
        cards1.Add(c);
        cards1.Add(d);
        player1.cards = cards1;

        c = new Card(11, Suit.Spade);
        d = new Card(5, Suit.Club);
        Card e = new Card(5, Suit.Diamond);
        Card f = new Card(11, Suit.Heart);
        Card g = new Card(1, Suit.Spade);
        List<Card> cards2 = new List<Card>();
        cards2.Add(c);
        cards2.Add(d);
        cards2.Add(e);
        cards2.Add(f);
        cards2.Add(g);
        table.board= cards2;
        
        player1.myHandIs(table.board, table.gameMode);
        */



    }

}