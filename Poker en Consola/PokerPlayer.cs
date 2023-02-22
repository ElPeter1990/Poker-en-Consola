using Poker_en_Consola;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Poker_en_Consola
{
    public class PokerPlayer
    {

        public List<Card> cards { get; set; }
        public string nickname { get; set; }
        public decimal stack { get; set; }
        public Actions lastAction { get; set; }
        public decimal moneyOnThePot { get; set; } //This atributte is to calculate sidePots
        public Boolean active { get; set; } //Player is not active when he is on the table but he is sitting out
        public List<Card> handCards { get; set; } //This is player handCards, it is contained by 5cards including comunity cards and player cards
        public Hands myHandName { get; set; }
        public PokerPlayer(string nickname1, decimal stack1, Actions lastAction1, bool active1) ///When creating a new player, system should give him a nickname and stack, list of card of players will be empty
        {
            this.cards = new List<Card>();
            this.nickname = nickname1;
            this.stack = stack1;
            this.lastAction = lastAction1;
            this.moneyOnThePot = 0; //When a player is created, he did not any bet or calls until this moment
            this.active = active1;
            this.handCards = new List<Card>(); //this element is empty because when player is created, there is no handCards for him
            this.myHandName = new Hands(); //this will be highcard by default
        }

        /// <summary>
        /// I get a list of 3 to 5 cards (board), I add my cards to that list and i return Enum value with the name of my handCards. Also a list of 5 better cards in player's "handCards" var (this.handCards)
        /// </summary>
        /// <param name="board"></param>
        /// <param name="game"></param>
        /// <returns></returns>
        public Hands myHandIs(List<Card> board, PokerGameModes game)
        {
            Hands myHand = new Hands();
            myHand = Hands.HighCard; //Because if no other hand returns, then player has just a high card, this is not necesarry but it is just in case of accidents on code

            if (game == PokerGameModes.Texas) //if this game is a Texas Holdem game
            {


                //LOOKING FOR ROYAL FLUSH
                List<Card> royflush = royalFlush(board, this.cards);
                if(royflush.Count > 0)
                {
                    myHand = Hands.Royalflush;
                    this.myHandName = myHand;
                    this.handCards = royflush;
                    return myHand;
                }

                //lOOKING FOR STRAIGHT FLUSH AND ROYAL FLUSH
                List<Card> sflush = straightFlush(board, this.cards);
                if (sflush.Count > 0)
                {
                    myHand = Hands.Straightflush;
                    this.myHandName = myHand;

                    /*
                    Console.WriteLine(" ESCALERA DE COLOR!!!!!!! ");
                    this.hand = sflush;
                    for (int i = 0; i < sflush.Count; i++)
                    {
                        Console.WriteLine(" {0} of {1}", sflush[i].rank, sflush[i].suit);
                    }
                    */
                    this.handCards = sflush;
                    return myHand;
                }

                //QUADS
                List<Card> q = quads(board, this.cards);
                if (q.Count>0)
                {
                    myHand = Hands.Quads;
                    this.myHandName = myHand;
                    this.handCards = q;
                    return myHand;
                }

                //FULLHOUSE
                List<Card> fh = fullHouse(board, this.cards);
                if (fh.Count > 0)
                {
                    myHand = Hands.Fullhouse;
                    this.myHandName = myHand;
                    this.handCards = fh;
                    return myHand;
                }

                //Looking for flush
                List<Card> flush1 = flush(board, this.cards); //This is the list of all same suited card if there is a flush
                if (flush1.Count >0)
                {
                    List<Card> flush2 = flush1.Take(5).ToList();
                    myHand = Hands.Flush;
                    this.myHandName = myHand;
                    this.handCards = flush2;
                    return myHand;
                }


                //Looking for straight.. calling straight() function declared behind

                List<Card> straight1 = straight(board, this.cards); //This is the list of all same suited card if there is a flush
                if (straight1.Count > 0)
                {
                    myHand = Hands.Straight;
                    this.myHandName = myHand;
                    this.handCards = straight1;
                    return myHand;
                }

                //SET
                List<Card> st = set(board, this.cards);
                if (st.Count > 0)
                {
                    myHand = Hands.Set;
                    this.myHandName = myHand;
                    this.handCards = st;
                    return myHand;
                }

                //TWO PAIR
                List<Card> tpr = twoPair(board, this.cards);
                if (tpr.Count > 0)
                {
                    myHand = Hands.DoublePair;
                    this.myHandName = myHand;
                    this.handCards = tpr;
                    return myHand;
                }

                //PAIR
                List<Card> pr = pair(board, this.cards);
                if (pr.Count > 0)
                {
                    myHand = Hands.Pair;
                    this.myHandName = myHand;
                    this.handCards = pr;
                    return myHand;
                }


                //HIGH CARD
                List<Card> hcards = highCard(board, this.cards);
                if (hcards.Count > 0)
                {
                    myHand = Hands.HighCard;
                    this.myHandName = myHand;
                    this.handCards = hcards;
                    return myHand;
                }



            }

            return myHand;
        }

        /// <summary>
        /// ddfgdfgdfhfdgfdgdgdfg
        /// </summary>
        /// <param name="board">marco</param>
        /// <param name="player">peter</param>
        /// <returns></returns>
        public List<Card> royalFlush(List<Card> board, List<Card> player)
        {
            List<Card> royflush = straightFlush(board, player); //This is the list of all same suited card if there is a flush
            if (royflush.Count > 0)
            {
                if (royflush[0].rank != 1) //because if Ace is not the first card in a sorted sttraight fluish from max to min, it is not a royal flush
                {
                    royflush.Clear();   
                }

            }
            return royflush;

        }


        public List<Card> fullHouse(List<Card> board, List<Card> player)
        {
            List<Card> playerHand = new List<Card>();
            List<Card> board1 = new List<Card>();
            List<Card> player1 = new List<Card>();
            board1.AddRange(board);
            player1.AddRange(player);

            List<Card> allCards = new List<Card>();
            allCards.AddRange(board1);
            allCards.AddRange(player1);

            List<Card> isSet = set(board1, player1); //If there is a set, it is on the first 3elements of this list
            if (isSet.Count > 0)
            {
                playerHand.Add(isSet[0]);
                playerHand.Add(isSet[1]);
                playerHand.Add(isSet[2]);

                int rankOfSet = isSet[0].rank;
                board1.RemoveAll(item => item.rank == rankOfSet); //Deleting all cards of the set form the cards list
                player1.RemoveAll(item => item.rank == rankOfSet); //Deleting all cards of the set form the cards list
                List<Card> pair1 = new List<Card>(); //looking for a pair on remaining cards
                pair1 = pair(board1, player1);

                if(pair1.Count > 0)
                {
                    playerHand.Add(pair1[0]);
                    playerHand.Add(pair1[1]);
                }
            }
            if (playerHand.Count !=5) {  //If there is not 5elements, then there is not a fullHouse..
                playerHand.Clear();
            }

            return playerHand;

        }
            public List<Card> quads(List<Card> board, List<Card> player)
        {

            List<Card> playerHand = new List<Card>();
            List<Card> board1 = new List<Card>();
            List<Card> player1= new List<Card>();
            board1.AddRange(board);
            player1.AddRange(player);

            List<Card> allCards = new List<Card>();
            allCards.AddRange(board1);
            allCards.AddRange(player1);
            int a = 0;
            int rank = 0;


                for (int i = 0; i < allCards.Count && a<3; i++)
                {
                    a = 0;
                    for (int b = i + 1; b < allCards.Count; b++)
                    {
                        if (allCards[i].rank == allCards[b].rank)
                        {
                             a = a+1;
                            if (a == 3)
                            {
                                rank = allCards[i].rank;
                                break;
                            }
                        }
                    }
                }
                if (a == 3) //if a==3 there is Quads
                {
                    for(int i=0; i < allCards.Count; i++)
                    {
                        if (allCards[i].rank == rank)
                        {
                            Card card = allCards[i];
                            playerHand.Add(card);
                        }
                    }
                //Now im looking for the highest cards on list
                List<Card> highestCards = highCard(board1, player1); //Sorted list from max to min with no repeated cards 

                for (int i = 0; i < highestCards.Count; i++)
                {
                    if (highestCards[i].rank != rank)
                    {
                        if (highestCards[i].rank == 14)
                            highestCards[i].rank = 1;
                        if (playerHand.FirstOrDefault(x => x.rank == highestCards[i].rank) == null)
                        {
                            Card card = allCards.FirstOrDefault(x => x.rank == highestCards[i].rank); //Find card by card rank
                            playerHand.Add(card);
                            break;
                        }
                    }
                }
                }

            return playerHand;
        }


        public List<Card> set(List<Card> board, List<Card> player)
        {

            List<Card> playerHand = new List<Card>();
            List<Card> board1 = new List<Card>();
            List<Card> player1 = new List<Card>();
            board1.AddRange(board);
            player1.AddRange(player);

            List<Card> allCards = new List<Card>();
            allCards.AddRange(board1);
            allCards.AddRange(player1);
            int a = 0;
            int rank = 0;


            for (int i = 0; i < allCards.Count && a < 2; i++)
            {
                a = 0;
                for (int b = i + 1; b < allCards.Count; b++)
                {
                    if (allCards[i].rank == allCards[b].rank)
                    {
                        a = a + 1;
                        if (a == 2) //There are 3same cards on the list
                        {
                            rank = allCards[i].rank;
                            break;
                        }
                    }
                }
            }
            if (a == 2) //if a==2 there is a set
            {
                for (int i = 0; i < allCards.Count; i++)
                {
                    if (allCards[i].rank == rank)
                    {
                        Card card = allCards[i];
                        playerHand.Add(card);
                    }
                }
               
                //Now im looking for the highest cards on list
                List<Card> highestCards = highCard(board1, player1); //Sorted list from max to min with no repeated cards 
                int b = 0;

                for (int i = 0; i < highestCards.Count && b<2; i++)
                {
                    if (highestCards[i].rank != rank)
                    {
                        if (highestCards[i].rank == 14)
                            highestCards[i].rank = 1;
                        if (playerHand.FirstOrDefault(x => x.rank == highestCards[i].rank) == null)
                        {
                            Card card = allCards.FirstOrDefault(x => x.rank == highestCards[i].rank); //Find card by card rank
                            playerHand.Add(card);
                            b++;
                        }
                    }
                }


            }

            return playerHand;
        }

        public List<Card> twoPair(List<Card>board, List<Card> player)
        {
            List<Card> playerHand = new List<Card>();
            List<Card> board1 = new List<Card>();
            List<Card> player1 = new List<Card>();
            board1.AddRange(board);
            player1.AddRange(player);

            List<Card> allCards = new List<Card>();
            allCards.AddRange(board1);
            allCards.AddRange(player1);

            List<Card> firstPair = pair(board1, player1); //If there is a pair, it is on the first 2elements of this list
            if (firstPair.Count > 0)
            {
                playerHand.Add(firstPair[0]);
                playerHand.Add(firstPair[1]);

                int rankOfPair = firstPair[0].rank;
                board1.RemoveAll(item => item.rank == rankOfPair); //Removing all cards with the pair rank from the cards list
                player1.RemoveAll(item => item.rank == rankOfPair); //Removing all cards with the pair rank from the cards list
                List<Card> secondPair = new List<Card>(); //looking for a pair on remaining cards
                secondPair = pair(board1, player1);

                if (secondPair.Count > 0)
                {
                    playerHand.Add(secondPair[0]);
                    playerHand.Add(secondPair[1]);
                    playerHand.Add(secondPair[2]); //This will be the card#5 of player hand, and the highest card of the list that is not a pair
                }
            }
            if (playerHand.Count != 5)
            {  //If there is not 5elements, then there is not two pair
                playerHand.Clear();
            }

            return playerHand;
        }

        public List<Card> highCard(List<Card> board, List<Card> player)
        {

            List<Card> playerHand = new List<Card>();
            List<Card> board1 = new List<Card>();
            List<Card> player1 = new List<Card>();
            board1.AddRange(board);
            player1.AddRange(player);

            List<Card> allCards = new List<Card>();
            allCards.AddRange(board1);
            allCards.AddRange(player1);


            List<Card> sortedCards = allCards.OrderByDescending(p => p.comparativeValue).ToList(); //Sorted from max rank to min rank
            List<Card> sortedAndNoRepeatedCards = cleanRepeatedCards(sortedCards);

            int b = 0;
            for(int i =0; i< sortedAndNoRepeatedCards.Count && b<5; i++) //JUST THE FIRST 5CARDS ON THE LIST.. THAT WILL BE THE HIGHEST CARDS..
            { 
                playerHand.Add(sortedAndNoRepeatedCards[i]);
                b++;
            }

            return playerHand;

        }

            public List<Card> pair(List<Card> board, List<Card> player)
        {

            List<Card> playerHand = new List<Card>();
            List<Card> board1 = new List<Card>();
            List<Card> player1 = new List<Card>();
            board1.AddRange(board);
            player1.AddRange(player);

            List<Card> allCards = new List<Card>();
            allCards.AddRange(board1);
            allCards.AddRange(player1);
            int a = 0;
            int rank = 0;

            List<Card> sortedCards = allCards.OrderByDescending(p => p.comparativeValue).ToList(); //Sorted from max rank to min rank


            for (int i = 0; i < sortedCards.Count && a < 1; i++)
            {
                a = 0;
                for (int b = i + 1; b < sortedCards.Count; b++)
                {
                    if (sortedCards[i].rank == sortedCards[b].rank)
                    {
                        a = a + 1;
                        if (a == 1) //There are 2same cards on the list
                        {
                            rank = sortedCards[i].rank;
                            break;
                        }
                    }
                }
            }
            if (a == 1) //if a==1 there is a pair
            {
                for (int i = 0; i < sortedCards.Count; i++)
                {
                    if (sortedCards[i].rank == rank)
                    {
                        Card card = sortedCards[i];
                        playerHand.Add(card);
                    }
                }


                //Now im looking for the highest cards on list
                List<Card> highestCards = highCard(board1, player1); //Sorted list from max to min with no repeated cards 


                int b = 0;

                for (int i = 0; i < highestCards.Count && b < 3; i++)
                {
                    if (highestCards[i].rank != rank)
                    {
                        if (highestCards[i].rank == 14)
                            highestCards[i].rank = 1;
                        if (playerHand.FirstOrDefault(x => x.rank == highestCards[i].rank) == null)
                        {
                            Card card = allCards.FirstOrDefault(x => x.rank == highestCards[i].rank); //Find card by card rank
                            playerHand.Add(card);
                            b++;
                        }
                    }
                }

            }

            return playerHand;
        }


        public List<Card> straightFlush (List<Card> board, List<Card> player1) 
        {
            List<Card> straightFlush = new List<Card>();
            List<Card> isFlush = flush(board, player1);
            if(isFlush.Count>0) {
                straightFlush = straight(isFlush, isFlush);
            }

            return straightFlush;
        }

        //This function can return a list with more than five cards
        public List<Card> flush(List<Card> board, List<Card> player) ///To know if player has a flush (on Texas Holdem)
        {

            List<Card> flush = new List<Card>();
            List<Card> allCards = new List<Card>();
            List<Card> playerHand = new List<Card>();
            List<Card> board1 = new List<Card>();
            List<Card> player1 = new List<Card>();
            board1.AddRange(board);
            player1.AddRange(player);

            allCards.AddRange(board1);
            allCards.AddRange(player1);
            List<Card> allCards1= new List<Card>();
            allCards1.AddRange(allCards); //This list will only be used for function list(), because it will be modified inside the function

            /*
            for(int i= 0; i< allCards.Count; i++)
            {
                Console.WriteLine("{0} off {1}: ", allCards[i].rank, allCards[i].suit);
            }
            */

            int[] arrayBoard = new int[board1.Count]; //array for sort all cards on board de from min to max
            int[] arrayAllcards = new int[allCards.Count];
            int a = 0;

            arrayAllcards = list(allCards1);
            Array.Sort(arrayAllcards); //array of 7cards sorted of board + player's cards
            Array.Reverse(arrayAllcards); //same array from max to min

            a = 0;
            int e = 0;
            Suit winnerSuit = Suit.Heart; //This is the most used suit on the list, Heart is just for declare but that value will change behind and it doesnt matter because this var will take another value if there is a flush
            while (a < 4 && e < allCards.Count)
            {
                a = 0;
                for (int b = e + 1; b < allCards.Count; b++)
                {
                    if (allCards[e].suit == allCards[b].suit)
                    {
                        winnerSuit = allCards[b].suit; //this is the last suit coincidence registered in list before a = 4
                        a++;
                    }
                }
                e++;
            }

           
            

            if (a >= 4) //if there is a flush
            {
                Card card = null;
                Card card1 = null;
                for (int b = 0; b < arrayAllcards.Length; b++)
                {
                    card = null;
                    card1 = null;
                    // Card card = allCards.FirstOrDefault(x => x.rank == arrayAllcards[b]);
                    if (arrayAllcards[b] == 14)
                    {
                        card = allCards.Where(x => x.rank == 1 && x.suit == winnerSuit).FirstOrDefault();
                        if (card != null)
                        {
                            card1 = new Card(1, winnerSuit);
                        }
                    }
                    else
                    {
                        card = allCards.Where(x => x.rank == arrayAllcards[b] && x.suit == winnerSuit).FirstOrDefault();
                        if (card != null)
                        {
                            card1 = new Card(arrayAllcards[b], winnerSuit);
                        }
                    }
                    if (card1 != null && playerHand.FirstOrDefault(x => x.rank == card1.rank) == null)
                    {
                        playerHand.Add(card1);
                        //Console.WriteLine("La carta a agregar es: {0} of {1}", card.rank, card.suit);
                    }
                }

                flush.AddRange(playerHand);
            }

            return flush; //This is the list of all elements with the same suit, with Ace = 1, and this list count always will be 0 or >=5

        }

        public List<int> cleanRepeatedNumbers(int[] list)
        {

            List<int> newList = new List<int>();
            for (int i = 0; i < list.Length; i++)
            {
                int number = newList.Find(n => n == list[i]); //If number do no existes, number will be 0
                if (number == 0)
                {
                    newList.Add(list[i]);
                }
            }

            return newList;

        }

        public List<Card> cleanRepeatedCards(List<Card> list)
        {
            List<Card> newList = new List<Card>();
            Card card = null;
            for (int i = 0; i < list.Count; i++)
            {
                card = newList.FirstOrDefault(x => x.rank == list[i].rank); //Find card by card rank
                if (card == null)
                {
                    newList.Add(list[i]);
                }
            }

            return newList;

        }

        /// <summary>
        /// Recursive method to know if there is an straight of 5 consecutive items on a list of any amount of items
        /// </summary>
        /// <param name="list"></param>
        /// <param name="firstItem"></param>
        /// <returns></returns>
        public List<int> majorStraightOnList(List<int> list, int firstItem)
        {
            List<int> straight = new List<int>(); //This list will be empty in case there is not straight there
            int a = 0;
            int b = 0;
            if (firstItem <=list.Count - 5) //Because if it is lower than this value, then there are no 5 remaining elements
            {
                for (int i = firstItem; i < list.Count && a < 5; i++)
                {
                    if (i > 0 && list[i] == list[i - 1] - 1) //if two runner cards have the same value, cycle will not join this if on that moment
                    {
                        if (b == 0) //if this is the first element checked on the list
                        {
                            int c = list[i - 1];
                            straight.Add(c);
                            //  Console.WriteLine("La carta annadida es: {0}", c);
                            a = a + 1;
                            b++;
                        }
                        int d = list[i];
                        straight.Add(d);
                        //Console.WriteLine("La carta1 annadida es: {0}", d);
                        a = a + 1;
                    }
                    else { a = 0; } //If there is a non consecutive number, a=0 to start the find again
                }
                if (a == 5)
                {
                    return straight;
                }
                else
                {
                    firstItem++;
                    return majorStraightOnList(list, firstItem);
                }
            }

            return straight;


        }

        public List<Card> straight(List<Card> board, List<Card> player) ///Best straight on two list of cards
        {

            List<Card> listAllCards = new List<Card>();
            List<Card> board1 = new List<Card>();
            List<Card> player1 = new List<Card>();
            List<Card> straightToReturn = new List<Card>();
            board1.AddRange(board);
            player1.AddRange(player);
            List<Card> playerHand = new List<Card>();

            listAllCards.AddRange(board1);
            listAllCards.AddRange(player1);

            int[] arrayAllcards = list(listAllCards); //In this function aces with rank = 14 will be added to listAllCards
            Array.Sort(arrayAllcards); //array of 7cards sorted of board + player's cards
            Array.Reverse(arrayAllcards); //because i need the highest straight of 7cards..
            List<int> allCardWithoutRepeatedNumbers = cleanRepeatedNumbers(arrayAllcards);

            List<int> straight = majorStraightOnList(allCardWithoutRepeatedNumbers, 0);

            if (straight.Count > 0)
            {  //If there is an straight
                for (int i = 0; i < straight.Count; i++)
                {
                    Card card1;
                    if (straight[i] == 14)
                    {
                        straight[i] = 1; //Giving Ace his rank on the real list.. 1
                    }
                    Card card = listAllCards.FirstOrDefault(x => x.rank == straight[i]); //Find card by card rank
                    if (card != null) //Just in Case of accidents..
                    {
                        card1 = new Card(straight[i], card.suit);
                        playerHand.Add(card);
                        //  Console.WriteLine("La carta es: {0} of {1}", card.rank, card.suit);
                    }

                }
            }

            straightToReturn.AddRange(playerHand);

            return straightToReturn;


        }

        /// <summary>
        /// This funtion takes a list of cards and create an array of int with all ranks of cards, and adding value14 to aces on cards list
        /// </summary>
        /// <param name="listOfCards"></param>
        /// <returns></returns>
        public int[] list (List<Card> listOfCards)
        {

            List<Card> listAllCards = new List<Card>();

            int a = 0;
            List<Card> acesOfList = new List<Card>();
            Card ace;

            for (int i = 0; i < listOfCards.Count; i++)  //To know if player hand has an As
            {
                if (listOfCards[i].rank == 1)
                {
                    ace = new Card(14, listOfCards[i].suit);
                    acesOfList.Add(ace);
                }
            }
            
            if (acesOfList.Count > 0) //If player hand has an As, (remember As value is 1..)
            {
                listOfCards.AddRange(acesOfList); //Now listOfCards has more elements and them can be used on another funtion from where this function was called
            }
            
            int[] arrayAllcards = new int[listOfCards.Count]; //array of all cards of board + player cards that always will be two of them

            for (int i = 0; i < listOfCards.Count; i++) //Board1 +player1 because there will be all cards
            {
                    arrayAllcards[i] = listOfCards[i].rank;
                    
            }
            /*
            for(int i= 0; i < arrayAllcards.Length; i++)
                Console.WriteLine(arrayAllcards[i]);
            */
            return arrayAllcards;
        }


       public Boolean getMoney (decimal amount)
        {
            if(this.stack >= amount)
            {
                this.stack -= amount;
                return true;
            }
            return false;

        }

        public decimal raiseAmount(decimal pot, decimal betToCallForMe)
        {
            
            Console.Write("Escriba la cantidad a la que desea subir: ");
            string input = Console.ReadLine();
            decimal num = decimal.Parse(input);

            while (num == null || num > this.stack)
            {
                Console.WriteLine("Tamano no seleccionado o mayor a su stack");
                Console.Write("Escriba la cantidad que desea subir: ");
                input = Console.ReadLine();
                num = decimal.Parse(input);
            }

            return num;
        }

        /// <summary>
        /// What do you want to do player? fold, check, call or raise?
        /// </summary>
        /// <returns></returns>
        public Actions newAction(Boolean Fold, Boolean Check, Boolean Call, Boolean Raise)
        {

            Console.Write("Que desea hacer {0} con stack efectivo: ${1}? ||", this.nickname, this.stack);
            Console.Write("Opciones: ");
            if (Fold)
                Console.Write("F: Fold, ");
            if (Check)
                Console.Write("E: Check, ");
            if (Call)
                Console.Write("C: Call, ");
            if (Raise)
                Console.Write("R: Raise, ");
            Console.WriteLine("");
            //Console.WriteLine("F: Fold, E: Check, C: Call, R: Raise)");
            
            ConsoleKeyInfo tecla = Console.ReadKey();
            Actions action= new Actions();

            if(Fold)
            if (tecla.Key == ConsoleKey.F)
            {
                action = Actions.Fold;
            }
            if(Check)
            if (tecla.Key == ConsoleKey.E)
            {
                action = Actions.Check;
            }
            if(Call)
            if (tecla.Key == ConsoleKey.C)
            {
                action = Actions.Call;
            }
            if(Raise)
            if (tecla.Key == ConsoleKey.R)
            {
                action = Actions.Raise;
            }

            return action;
        }



       

    }

}
