using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Poker_en_Consola
{
    public class PokerTable
    {
        public int id { get; set; } //SUPOSED FOR DATE BASE I THINK
        public Deck deck { get; } // New deck of cards of the table shuffled and ready to deal
        public List<Card> board { get; set; }
        public List<PokerPlayer> players { get; set; } //First player is the SB... and last player is the button
        public decimal pot { get; set; }

        public Boolean active { get; set; } //If this table is active or not
        public Status status { get; set; } //When table is created, first status is None
        public int seats { get; set; } //Number of seats.. it can be 6max or fullRing (9seats)
        public PokerGameModes gameMode { get; } // Game Mode of the table.. it can be Texas or Omaha

        public decimal minBuyIn { get; set; } //Minimun buy in for this table
        public decimal maxBuyIn { get; set; } //Maximun buy in for this table (it should be 100bbs)
        public decimal bb { get; set; } //Bing Blind of this table..


        public PokerTable(int id1, PokerGameModes gameMode1, int seats1, decimal bb1, decimal minBuyIn1, decimal maxBuyIn1, List<PokerPlayer> players1, Boolean active) //If table is opened with just one player, this list will contain just one element
            {
               this.players = players1;
               this.deck = new Deck();
                deck.ShuffleDeck(); // When a new table is created, the deck will be shuffled before dealing cards
                this.board = new List<Card>();
                this.pot = 0;
                this.gameMode= gameMode1;
                this.bb = bb1;
                this.seats= seats1;
                this.minBuyIn = minBuyIn1;
                this.maxBuyIn = maxBuyIn1;
                this.id= id1;
            this.status = Status.None; //When a new table is created, its status will always be None..
            this.active= active;
            }

        public void ShuffleDeck() ///Function to shuffle the deck of the table before dealing cards
            {
                this.deck.ShuffleDeck();
            }

        public void BestHand(PokerPlayer player1, PokerPlayer player2, List<Card> board) ///Function for know what is the best hand between two players and the board
        {
           



        }

        /// <summary>
        /// Table will be active while at least two players be active
        /// 
        /// </summary>
        /// <returns></returns>
        public Boolean isActive()
        {
            int a = 0;
            if(this.players.Count > 1) { 
            foreach (PokerPlayer player in this.players) {
                if (player.active == true)
                    a++;
                if (a ==2)
                    return true;
            }
            }


            return false;
        }

        public void DealBoard()
            {
            int a = 0; //save index for start looking for player's cards to deal board
            

            if (this.board.Count == 0) //if there is no cards on board
            {
                for (int i=0; i<players.Count; i++)
            {
                if (players[i].cards.Count > 0) //if player has cards..
                     a = a + players[i].cards.Count;
            }

            
                for (int i = a+1; i < a+8; i++) //8cards.. three burned cards and five cards on the board (a+1 because first card is burned)
                {
                    if (this.board.Count() < 3)
                    {
                        this.board.Add(deck.getCard(i));
                    }
                    if(this.board.Count() == 3 && i == a+5) //3card on board and 1card burned
                    {
                        this.board.Add((Card)deck.getCard(i));
                    }
                    if (this.board.Count() == 4 && i == a + 7) //4card on board and 2cards burned
                    {
                        this.board.Add((Card)deck.getCard(i));
                    }
                }
            }
                
            }

        public void DealCards()
            {
            int b = 0;
                 for (int i=0;i<this.players.Count;i++) //First card for SB and last card for Botton
                    {
                         int c = i + 1;
                        if (c == players.Count) { c = 0; }
                        PokerGameModes enumValue = this.gameMode;
                        int a = ((int)enumValue);
                        
                        if (a == 0) //If gameMode is Texas Holdem table will deal just 2card for each player
                            {
                                PokerPlayer player = this.players[c];
                                Card FirstcardForPlayer = this.deck.getCard(b);
                                b = b + 1;   
                                Card SecondcardForPlayer = this.deck.getCard(b);
                                b = b+ 1;
                                player.cards.Add(FirstcardForPlayer);
                                player.cards.Add(SecondcardForPlayer);
                                
                            }


                    }
                 
            }

        /// <summary>
        /// Check if all players have at least 1bb stack, and return all active players
        /// </summary>
        /// <returns></returns>
        public List<PokerPlayer> activePlayers()
        {
            List<PokerPlayer> players= new List<PokerPlayer>();
            foreach (PokerPlayer player in this.players)
            {
                if(player.stack < this.bb) { player.active = false; } //If this player doesnt have 1bb on stack, he should be sitting out
                if(player.active)
                {
                    players.Add(player);
                }
            }

            return players;
        }

        public decimal sidepotForPlayer(PokerPlayer player)
        {
            decimal sidePot = 0;
            List<PokerPlayer> players = this.players; //List of all players of this table.. folded or not folded.. all of them
            foreach (PokerPlayer playerOnList in players) //If a player has money on pot, then i will take it, and i dont care if player is sitting out or is hi is folded or if he leaved table on the middle of a running hand
            {
                if (playerOnList.nickname != player.nickname)
                {
                    if (player.moneyOnThePot >= playerOnList.moneyOnThePot )
                    {
                        sidePot += playerOnList.moneyOnThePot;
                    }
                    else
                        sidePot += player.moneyOnThePot;
                }
            }
            sidePot += player.moneyOnThePot;
            return sidePot;
        }


        public void reAdjustFoldedPlayersPropertiesToZero()
        {
            List<PokerPlayer> activePlayers = this.activePlayers();
            if(activePlayers != null)
            {
                foreach(PokerPlayer player in activePlayers)
                {
                    if (player.lastAction == Actions.Fold)
                    {
                        player.moneyOnThePot = 0;
                        // player.cards.Clear(); ////Player will have his cards to the end because i cant use that cards for anything else while game is not over..
                        player.handCards.Clear();
                        player.lastAction = Actions.None;
                    }
                }
            }

        }
        /// <summary>
        /// All players.. active or not active.. will be restarted for beginning a new round of the game.. lastAction will be None
        /// </summary>
        public void reAdjustAllPlayersPropertiesToZero()
        {
                foreach (PokerPlayer player in this.players)
                {
                        player.moneyOnThePot = 0;
                        // player.cards.Clear(); ////Player will have his cards to the end because i cant use that cards for anything else while running round is not over..
                        player.handCards.Clear();
                        player.cards.Clear();
                        player.lastAction = Actions.None;
                }
        }

        /// <summary>
        /// Function to change players positions on table, moving from sb to botton
        /// </summary>
        /// 
        public void reAdjustPlayersPosition()
        {
            List<PokerPlayer> players = this.players;
            List<PokerPlayer> newPlayers = new List<PokerPlayer>();
            newPlayers.Add(players[players.Count - 1]); //First player of new list will be the last player of old list

            for (int i = 0; i < players.Count - 1; i++)
            {
                newPlayers.Add(players[i]);
            }

            this.players= newPlayers;
        }

        public List<PokerPlayer> playersWhoMakeLastAction(List<PokerPlayer> players, Actions action) //List of players who maked fold, or call, or allin, or raise.. or None..
        {
            List<PokerPlayer> playersWhoMakeLastAction = new List<PokerPlayer>();
            foreach (PokerPlayer player in players)
            {
                if (player.lastAction == action)
                    playersWhoMakeLastAction.Add(player);
            }
            return playersWhoMakeLastAction;
        }

        public List<PokerPlayer> playersWhoDidNotMakeLastAction(List<PokerPlayer> players, Actions action) //List of players who did not make a last action x
        {
            List<PokerPlayer> playersWhoDidNotMakeLastAction = new List<PokerPlayer>();
            foreach (PokerPlayer player in players)
            {
                if (player.lastAction != Actions.None && player.lastAction != action)
                    playersWhoDidNotMakeLastAction.Add(player);
            }
            return playersWhoDidNotMakeLastAction;
        }

        public void showBoard(Status street)
        {

            for (int i = 0; i < this.board.Count; i++)
            {
                if (i == 0)
                {
                    if (street == Status.Flop)
                    {
                        Console.Write("/ Flop ----> ");
                        Console.WriteLine("{0} of {1}, {2} of {3}, {4} of {5} /", this.board[i].getStringValue(this.board[i].rank), this.board[i].suit, this.board[i + 1].getStringValue(this.board[i + 1].rank), this.board[i + 1].suit, this.board[i + 2].getStringValue(this.board[i + 2].rank), this.board[i + 2].suit);
                        Console.WriteLine("");
                    }
                }
                if (i == 3)
                {
                    if (street == Status.Turn)
                    {
                        Console.Write("/ Turn ----> ");
                        Console.Write("{0} of {1}, {2} of {3}, {4} of {5}, ", this.board[i - 3].getStringValue(this.board[i - 3].rank), this.board[i - 3].suit, this.board[i - 2].getStringValue(this.board[i - 2].rank), this.board[i - 2].suit, this.board[i - 1].getStringValue(this.board[i - 1].rank), this.board[i - 1].suit);
                        Console.WriteLine("{0} of {1} /", this.board[i].getStringValue(this.board[i].rank), this.board[i].suit);
                        Console.WriteLine("");
                    }
                }
                if (i == 4)
                {
                    if (street == Status.River)
                    {
                        Console.Write("/ River ----> ");
                        Console.Write("{0} of {1}, {2} of {3}, {4} of {5}, ", this.board[i - 4].getStringValue(this.board[i - 4].rank), this.board[i - 4].suit, this.board[i - 3].getStringValue(this.board[i - 3].rank), this.board[i - 3].suit, this.board[i - 2].getStringValue(this.board[i - 2].rank), this.board[i - 2].suit);
                        Console.Write("{0} of {1}, ", this.board[i - 1].getStringValue(this.board[i - 1].rank), this.board[i - 1].suit);
                        Console.WriteLine("{0} of {1} /", this.board[i].getStringValue(this.board[i].rank), this.board[i].suit);
                        Console.WriteLine("");
                    }
                }
            }
        }

        public Boolean areEveryPlayersAllin(List<PokerPlayer> players)
        {
            int a = 0;
            int b = 0;
            foreach(PokerPlayer player in players)
            {
                if(player.lastAction !=Actions.Fold && player.lastAction != Actions.None)
                {
                    a++;
                    if (player.lastAction == Actions.Allin)
                        b++;
                }
            }

            if (a == b)
                return true;

            return false;
        }


        public PokerPlayer bestHandBetweenTwoPlayers(PokerPlayer player1, PokerPlayer player2)
        {
            Hands hand1 = player1.myHandName;
            Hands hand2 = player2.myHandName;

            if (hand1 == Hands.HighCard) //Because when player is created, his default hand is HighCard, then i shoukd find his hand on this cases to make sure it is a high card
                hand1 = player1.myHandIs(this.board, this.gameMode);
            if (hand2 == Hands.HighCard)
                hand2 = player2.myHandIs(this.board, this.gameMode);

            if ((int)hand1 == (int)hand2)
            {  //Bouth have same hand
                return cardByCardComparator(player1, player2);
            }

            if ((int)hand1 > (int)hand2)
                return player1;

            return player2;
        }

        /// <summary>
        /// Funtion to compare the 5cards of two same hands in poker and says what is the best combination, or if it is a split
        /// </summary>
        /// <param name="player1"></param>
        /// <param name="player2"></param>
        /// <returns></returns>
        public PokerPlayer cardByCardComparator(PokerPlayer player1, PokerPlayer player2)
        {
            List<Card> cards1 = player1.handCards;
            List<Card> cards2 = player2.handCards;


            /*
            Console.WriteLine("MANO DEL JUGADOR 1: ");
            foreach (Card card in cards1)
            {
                Console.WriteLine("{0} of {1}", card.rank, card.suit);
            }

            Console.WriteLine();

            Console.WriteLine("MANO DEL JUGADOR 2: ");
            foreach (Card card in cards2)
            {
                Console.WriteLine("{0} of {1}", card.rank, card.suit);
            }
            */

            for(int i=0; i< 5; i++)
            {
                if (cards1[i].comparativeValue > cards2[i].comparativeValue) //This will be an split until i have just one higher card than you card on the same position
                    return player1;
                if (cards2[i].comparativeValue > cards1[i].comparativeValue) //This will be an split until i have just one higher card than you card on the same position
                    return player2;
            }

            return null; //If this did not return up there, then they have the same cards.. and the same hand.. its a split


        }

        /// <summary>
        /// Players who have the winner hand on poker game.. is this list.Count is >1 then there is more than just one winner
        /// </summary>
        /// <param name="players"></param>
        /// <returns></returns>
        public List<PokerPlayer> winnerPlayers(List<PokerPlayer> players)
        {
            List<PokerPlayer> winners = new List<PokerPlayer>();
            bool winOrSplit = true;

            foreach (PokerPlayer player in players)
            {
                winOrSplit = true;
                for(int i = 0; i<players.Count; i++)
                {
                    if(bestHandBetweenTwoPlayers(player, players[i]) != null && player.nickname != bestHandBetweenTwoPlayers(player, players[i]).nickname)
                        winOrSplit= false; //if players[i] has the best hand, im not the big winner of this game
                }
                if(winOrSplit) //If nobody has best hand then me.. im the winner and other players who has the same hand than me will be on winner list too
                { 
                    winners.Add(player); 
                } 
            }

            return winners;
        }


        public void allPlayersDropCards()
        {
            foreach(PokerPlayer player in this.players)
            {
                player.cards.Clear();
            }
        }

        public List<PokerPlayer> findPlayersByMoneyOnPot(List<PokerPlayer> players, decimal moneyOnPot)
        {
            List<PokerPlayer> playersSelected = new List<PokerPlayer>();
            foreach(PokerPlayer player in players)
            {
                if(player.moneyOnThePot == moneyOnPot)
                    playersSelected.Add(player);
            }
            return playersSelected;
        }

        public List<PokerPlayer> playersWithSameOrMajorMoneyOnPotThanMe(List<PokerPlayer> players, decimal moneyOnPot)
        {
            List<PokerPlayer> playersSelected = new List<PokerPlayer>();
            foreach (PokerPlayer player in players)
            {
                if (player.moneyOnThePot >= moneyOnPot)
                    playersSelected.Add(player);
            }
            return playersSelected;
        }

        public List<PokerPlayer> playersWithSameOrMinorMoneyOnPotThanMe(List<PokerPlayer> players, decimal moneyOnPot)
        {
            List<PokerPlayer> playersSelected = new List<PokerPlayer>();
            foreach (PokerPlayer player in players)
            {
                if (player.moneyOnThePot <= moneyOnPot)
                    playersSelected.Add(player);
            }
            return playersSelected;
        }

    }
}
