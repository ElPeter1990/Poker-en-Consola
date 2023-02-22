using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.Xml.Linq;

namespace Poker_en_Consola
{
    internal class PokerDealer
    {

        //Every table has one dealer, and every dealer has one table, when a table is active, it has a dealer, when it is not, dealer goes to another table
        public PokerDealer(PokerTable table)
        {

            while (table.isActive())
            {
                decimal pot = 0;
                decimal sidePot = 0;
                decimal blinds = 0;
                decimal betToCall = 0;
                table.pot = 0; //When a new round starts.. pot is $0 dollars
                PokerPlayer winnerPlayer = null;
                PokerPlayer lastPlayerWhoRaised = null;
                PokerPlayer firstPlayerWhoCheck = null;
                List<PokerPlayer> players = table.activePlayers(); //I will take just the active players on the table, no money players and sitting out players will not speak or pay blinds or nothing

                if (table.status == Status.None)
                {
                    table.pot = 0;
                    table.allPlayersDropCards();
                    table.board.Clear();
                    table.deck.ShuffleDeck();
                    table.DealCards();
                    table.status = Status.Preflop;
                    /*
                    //************TESTING..**************************
                    Card c1 = new Card(13, Suit.Diamond);
                    Card c2 = new Card(5, Suit.Club);
                    table.players[0].cards.Clear();
                    table.players[0].cards.Add(c1);
                    table.players[0].cards.Add(c2);
                    table.players[0].lastAction = Actions.Call;
                    table.pot += table.players[0].stack;

                    c1 = new Card(7, Suit.Heart);
                    c2 = new Card(10, Suit.Club);
                    table.players[1].cards.Clear();
                    table.players[1].cards.Add(c1);
                    table.players[1].cards.Add(c2);
                    table.players[1].lastAction = Actions.Call;
                    table.pot += table.players[1].stack;

                    c1 = new Card(7, Suit.Diamond);
                    c2 = new Card(9, Suit.Diamond);
                    table.players[2].cards.Clear();
                    table.players[2].cards.Add(c1);
                    table.players[2].cards.Add(c2);
                    table.players[2].lastAction = Actions.Call;
                    table.pot += table.players[2].stack;

                    c1 = new Card(1, Suit.Club);
                    c2 = new Card(10, Suit.Heart);
                    table.players[3].cards.Clear();
                    table.players[3].cards.Add(c1);
                    table.players[3].cards.Add(c2);
                    table.players[3].lastAction = Actions.Call;
                    table.pot += table.players[3].stack;

                    c1 = new Card(10, Suit.Heart);
                    c2 = new Card(13, Suit.Heart);
                    table.players[4].cards.Clear();
                    table.players[4].cards.Add(c1);
                    table.players[4].cards.Add(c2);
                    table.players[4].lastAction = Actions.Call;
                    table.pot += table.players[4].stack;

                    c1 = new Card(7, Suit.Heart);
                    c2 = new Card(7, Suit.Spade);
                    table.players[5].cards.Clear();
                    table.players[5].cards.Add(c1);
                    table.players[5].cards.Add(c2);
                    table.players[5].lastAction = Actions.Call;
                    table.pot += table.players[5].stack;
                    //************************TESTING*******************
                    */
                }

                //STARTING PREFLOP STREET
                if (table.status == Status.Preflop)
                {
                    sidePot = 0;
                    blinds = 0; //This is the side pot of bets on this street of game
                    if (players[0].getMoney(table.bb / 2))
                    {
                        blinds += table.bb / 2;
                        players[0].moneyOnThePot += table.bb / 2;
                    } //Paying blinds, this bouth functions will always be true because all active players have at least 1bb on their stacks
                    if (players[1].getMoney(table.bb))
                    {
                        blinds += table.bb;
                        players[1].moneyOnThePot += table.bb;
                        players[1].lastAction = Actions.Call; //Big blind is automatically calling the bb when poker hand begins
                    }
                    table.pot += blinds;
                    Console.WriteLine();
                    Console.WriteLine("JUGANDO EL PREFLOP, JUGADORES ACTIVOS: {0}, POZO: ${1}", players.Count, table.pot);
                    betToCall = table.bb; //If nobody bet yet.. the price to call is 1bb..
                    lastPlayerWhoRaised = null;


                    for (int i = 0; i < players.Count; i++)
                    {
                        int rank1 = players[i].cards[0].rank;
                        int rank2 = players[i].cards[1].rank;
                        Console.WriteLine("......................................................");
                        Console.WriteLine("/ Nombre: {0}, STACK: ${1}, Posicion: {2}         /", players[i].nickname, players[i].stack, i);
                        Console.WriteLine("/ {0} of {1}, {2} of {3}                            /", players[i].cards[0].getStringValue(rank1), players[i].cards[0].suit, players[i].cards[1].getStringValue(rank2), players[i].cards[1].suit);
                        Console.WriteLine("/...................................................../");
                    }
                    Console.WriteLine("");
                    /*
                    //TESTING
                    //TESTING
                    table.board.Clear();
                    Card c1 = new Card(6, Suit.Club);
                    Card c2 = new Card(7, Suit.Club);
                    Card c3 = new Card(8, Suit.Club);
                    Card c4 = new Card(9, Suit.Club);
                    Card c5 = new Card(1, Suit.Diamond);
                    table.board.Add(c1);
                    table.board.Add(c2);
                    table.board.Add(c3);
                    table.board.Add(c4);
                    table.board.Add(c5);
                    table.status = Status.River;
                    //TESTING   
                    //TESTING
                    */
                    while (table.status == Status.Preflop)
                    {

                        for (int i = 0; i < players.Count; i++) //Running cycle from UTG to the botton, and on headsup first speak who?
                        {

                            int a = i + 2;
                            if (a == players.Count) { a = 0; }
                            if (a == players.Count + 1) { a = 1; }
                            PokerPlayer player = players[a];


                            if (player.lastAction != Actions.Fold) //If he is not folded
                            {
                                if (lastPlayerWhoRaised == player) //If this was the last player who raised the pot...
                                {
                                    betToCall = 0;
                                    //THIS METHOD IS TO KNOW IF SOMEBODY CALLED MY RAISE.. remember that iam the last player to speak on table..
                                    if (table.playersWhoMakeLastAction(players, Actions.Call).Count > 0)
                                    {

                                        table.status = Status.Flop;
                                        //table.reAdjustFoldedPlayersPropertiesToZero();
                                        break;
                                    }
                                    else
                                    {
                                        table.status = Status.None; //if nobody called my bet.. then this hand will not be played

                                        decimal winnedPot = table.pot;
                                        player.stack += table.pot;
                                        winnerPlayer = player;
                                        lastPlayerWhoRaised.lastAction = Actions.Fold;
                                        table.reAdjustAllPlayersPropertiesToZero(); //Now botton will be SB and BB will be UTG position
                                        table.reAdjustPlayersPosition(); //Moving players to their next position
                                        Console.WriteLine("ESTA MANO HA TERMINADO EN EL PREFLOP. HA GANADO {0} UN POZO DE ${1}", winnerPlayer.nickname, winnedPot);

                                        Console.WriteLine("PRESIONE UNA TECLA PARA PASAR A LA SIGUIENTE MANO");
                                        Console.ReadKey();
                                        break;
                                    }
                                }

                                if (player.lastAction != Actions.Allin)
                                {
                                    if (a == 1 && lastPlayerWhoRaised == null) //IM ASKING TO BIG BLIND AND NOBODY RAISED (INCLUDING THE BIG BLIND), THEN THIS IS A LIMP POT
                                    {
                                        if (table.playersWhoMakeLastAction(players, Actions.Call).Count > 1) //If at least two players called bb, including me, remember i called bb on the beginning of preflop logic
                                        {
                                            table.status = Status.Flop;
                                           // table.reAdjustFoldedPlayersPropertiesToZero();
                                            break;
                                        }
                                        else //Everybody folds on this hand... BB wins..
                                        {
                                            decimal winnedPot = table.pot;
                                            winnerPlayer = player;
                                            player.stack += winnedPot;
                                            player.lastAction = Actions.Fold;
                                            table.reAdjustAllPlayersPropertiesToZero();
                                            table.reAdjustPlayersPosition();
                                            table.status = Status.None;
                                            betToCall = 0;
                                            Console.WriteLine("ESTA MANO HA TERMINADO EN EL PREFLOP. HA GANADO {0} UN POZO DE ${1}", winnerPlayer.nickname, winnedPot);
                                            break;
                                        }
                                    }

                                    decimal betToCallForMe = betToCall - player.moneyOnThePot; //This is the real money that player have to call on this right moment
                                    Actions action = Actions.None; //This var will never be NONE at the end of this if sentence..
                                    Console.WriteLine();
                                    Console.WriteLine("{0} -----> STACK EFECTIVO: {1}, POZO: {2}, APUESTA A PAGAR: {3}", player.nickname, player.stack, pot, betToCallForMe);
                                    if (a == 1) //If this is the big blind..
                                    {
                                        if (betToCall == table.bb) //If this is the bb and nobody raised the pot..
                                            action = player.newAction(false, true, false, true); //big blind cannot fold, and cannot call because he already called the bb..
                                        if (betToCall > table.bb)
                                        {  //If this is a raised pot..
                                            if (player.stack > betToCallForMe)
                                                action = player.newAction(true, false, true, true);
                                            else
                                                action = player.newAction(true, false, true, false);
                                        }
                                    }
                                    else
                                    {
                                        if (player.stack <= betToCallForMe)
                                            action = player.newAction(true, false, true, false); //Player cannot raise
                                        if (player.stack > betToCallForMe)
                                            action = player.newAction(true, false, true, true); //Player can raise
                                    } //If im not the big blind..

                                    if (action == Actions.Check)
                                    {
                                        player.lastAction = Actions.Check;
                                        Console.WriteLine("{0} ha jugado check", player.nickname);
                                        Console.WriteLine();
                                    }

                                    if (action == Actions.Fold)
                                    {
                                        player.lastAction = Actions.Fold;
                                        Console.WriteLine("{0} ha foldeado cobardemente", player.nickname);
                                        Console.WriteLine("POZO ACTUALIZADO: ${0}, STACK ACTUALIZADO DE {1}: ${2}", table.pot, player.nickname, player.stack);
                                        Console.WriteLine();
                                    }
                                    if (action == Actions.Call)
                                    {
                                        decimal amount = betToCallForMe;
                                        decimal lastStack = player.stack;
                                        if (player.getMoney(amount))
                                        {
                                            player.lastAction = Actions.Call;
                                            table.pot += amount;
                                            player.moneyOnThePot += amount;
                                            if (betToCall == table.bb)
                                                Console.WriteLine("{0} ha pagado la ciega grande (${1})", player.nickname, table.bb);
                                            if (betToCall > table.bb) //if he is not calling the big blind
                                                Console.Write("{0} ha pagado la apuesta de {1} || ", player.nickname, lastPlayerWhoRaised.nickname);
                                            if (lastStack == amount)
                                            {
                                                player.lastAction = Actions.Allin;
                                                Console.WriteLine(" y esta ALLIN..");
                                            }
                                            Console.WriteLine("POZO ACTUALIZADO: ${0}, STACK ACTUALIZADO DE {1}: ${2}", table.pot, player.nickname, player.stack);
                                            Console.WriteLine();
                                        }
                                        else //If he could not pay the last bet completely
                                        {
                                            table.pot += player.stack;
                                            player.moneyOnThePot += player.stack;
                                            sidePot += player.moneyOnThePot; //tengo k hacer metodo para sacar el sidepot entero

                                            player.getMoney(player.stack); //Now the stack of this player will be 0..
                                            player.lastAction = Actions.Allin;
                                            Console.WriteLine("El jugador {0} esta allin..", player.nickname);
                                            Console.WriteLine("POZO ACTUALIZADO: ${0}, STACK ACTUALIZADO DE {1}: ${2}", table.pot, player.nickname, player.stack);
                                            Console.WriteLine();
                                        }
                                    }
                                    if (action == Actions.Raise)
                                    {
                                        lastPlayerWhoRaised = player;
                                        player.lastAction = Actions.Raise;

                                        decimal raiseAmount = player.raiseAmount(table.pot, betToCallForMe);
                                        decimal realExtraAmount = raiseAmount - betToCallForMe;

                                        if (player.moneyOnThePot == table.bb) //This two if sentence are here because there was one blind more or one blind less on stacks and total pot when i go allin after just made the automatic call of the blinds
                                        {
                                            if (raiseAmount != player.stack)
                                                realExtraAmount -= table.bb;
                                        }
                                        if (player.moneyOnThePot == table.bb / 2)
                                        {
                                            if (raiseAmount != player.stack)
                                                realExtraAmount -= table.bb / 2;
                                        }

                                        decimal lastStack = player.stack;
                                        player.getMoney(betToCallForMe);
                                        player.getMoney(realExtraAmount);

                                        table.pot += betToCallForMe;
                                        table.pot += realExtraAmount;
                                        player.moneyOnThePot += betToCallForMe;
                                        player.moneyOnThePot += realExtraAmount;
                                        betToCall = betToCall + realExtraAmount;

                                        if (raiseAmount == lastStack)
                                        {
                                            player.lastAction = Actions.Allin;
                                            Console.WriteLine("{0} ha pagado ${1}, ha subido ${2} mas y esta ALLIN || Apuesta total a pagar: ${3}", player.nickname, betToCallForMe, realExtraAmount, betToCall);
                                        }
                                        else
                                        {
                                            Console.WriteLine("{0} ha pagado ${1} y ha subido ${2} mas.. || Apuesta total a pagar: ${3}", player.nickname, betToCallForMe, realExtraAmount, betToCall);
                                        }
                                        Console.WriteLine("POZO ACTUALIZADO: ${0}, STACK ACTUALIZADO DE {1}: ${2}", table.pot, player.nickname, player.stack);
                                        Console.WriteLine();
                                    }
                                }
                            }
                        }
                    }
                }
                //STARTING FLOP STREET
                if (table.status == Status.Flop)
                {
                    betToCall = 0; //New round of bets begins now..
                    Console.WriteLine();
                    Console.WriteLine("JUGANDO EL FLOP, POZO: ${0}", table.pot);
                    Console.WriteLine();
                    lastPlayerWhoRaised = null;
                    firstPlayerWhoCheck = null;
                    decimal[] moneyOnPot = moneyOnPotOnThisStreet(players);
                    table.DealBoard();




                    //SHOWING THE BOARD
                    table.showBoard(table.status);

                    while (table.status == Status.Flop)
                    {
                        if (table.areEveryPlayersAllin(players))
                        {
                            table.status = Status.Turn;
                            break; //This will end while's cycle right here
                        }

                        for (int i = 0; i < players.Count; i++) //Running cycle from SB to Botton..  
                        {
                            int a = i;
                            PokerPlayer player = players[a];

                            if (player.lastAction != Actions.Fold && player.lastAction != Actions.None) //If he is not folded
                            {
                                if (lastPlayerWhoRaised == player) //If this was the last player who raised the pot.. then this street is over
                                {

                                    //THIS METHOD IS TO KNOW IF SOMEBODY CALLED MY RAISE.. remember that iam the last player to speak on table..
                                    if (table.playersWhoMakeLastAction(players, Actions.Call).Count > 0)
                                    {
                                        table.status = Status.Turn;
                                        //table.reAdjustFoldedPlayersPropertiesToZero();
                                        break;
                                    }
                                    else
                                    {
                                        table.status = Status.None; //if nobody call my bet.. then this hand will not be played
                                        decimal winnedPot = table.pot;
                                        player.stack += table.pot;
                                        winnerPlayer = player;
                                        lastPlayerWhoRaised.lastAction = Actions.Fold;
                                        table.reAdjustFoldedPlayersPropertiesToZero(); //Now botton will be SB and BB will be UTG position
                                        table.reAdjustPlayersPosition(); //Moving players to their next position
                                        Console.WriteLine("ESTA MANO HA TERMINADO EN EL FLOP. HA GANADO {0} UN POZO DE ${1}", winnerPlayer.nickname, winnedPot);

                                        Console.WriteLine("PRESIONE UNA TECLA PARA PASAR A LA SIGUIENTE MANO");
                                        Console.ReadKey();
                                        break;
                                    }
                                }
                                if (player.lastAction != Actions.Allin)
                                {
                                    if (player == firstPlayerWhoCheck && lastPlayerWhoRaised == null) //If nobody raised and i was first player checking, then everyone else checked
                                    {
                                        table.status = Status.Turn;
                                        //table.reAdjustFoldedPlayersPropertiesToZero();
                                        break;
                                    }

                                    decimal betToCallForMe = betToCall - moneyOnPot[a]; //This is the real money that player have to call on this right moment
                                    Actions action = Actions.None; //This var will never be NONE at the end of this if sentence..
                                    Console.WriteLine();
                                    Console.WriteLine("{0} -----> STACK EFECTIVO: {1}, POZO: ${2}, APUESTA A PAGAR: {3}", player.nickname, player.stack, table.pot, betToCallForMe);

                                    if (betToCallForMe == 0)
                                    {
                                        action = player.newAction(false, true, false, true); //Player cannot fold if there is not any bet
                                    }
                                    else
                                    {
                                        if (player.stack <= betToCallForMe)
                                            action = player.newAction(true, false, true, false); //Player cannot raise
                                        if (player.stack > betToCallForMe)
                                            action = player.newAction(true, false, true, true); //Player can raise
                                    }
                                    if (action == Actions.Check)
                                    {
                                        player.lastAction = Actions.Check;
                                        if (firstPlayerWhoCheck == null)
                                            firstPlayerWhoCheck = player;
                                        Console.WriteLine("{0} ha jugado check", player.nickname);
                                        Console.WriteLine();
                                    }

                                    if (action == Actions.Fold)
                                    {
                                        player.lastAction = Actions.Fold;
                                        Console.WriteLine("{0} ha foldeado cobardemente", player.nickname);
                                        Console.WriteLine("POZO ACTUALIZADO: ${0}, STACK ACTUALIZADO DE {1}: ${2}", table.pot, player.nickname, player.stack);
                                        Console.WriteLine();
                                    }
                                    if (action == Actions.Call)
                                    {
                                        decimal amount = betToCallForMe;
                                        decimal lastStack = player.stack;
                                        if (player.getMoney(amount))
                                        {
                                            player.lastAction = Actions.Call;
                                            table.pot += amount;
                                            player.moneyOnThePot += amount;
                                            moneyOnPot[a] += amount;
                                            if (betToCall > 0 && lastPlayerWhoRaised != null) //This always will be true
                                                Console.Write("{0} ha pagado la apuesta de {1} || ", player.nickname, lastPlayerWhoRaised.nickname);
                                            if (lastStack == amount)
                                            {
                                                player.lastAction = Actions.Allin;
                                                Console.WriteLine(" y esta ALLIN..");
                                            }
                                            Console.WriteLine("POZO ACTUALIZADO: ${0}, STACK ACTUALIZADO DE {1}: ${2}", table.pot, player.nickname, player.stack);
                                            Console.WriteLine();
                                        }
                                        else //If he could not pay the last bet completely
                                        {
                                            table.pot += player.stack;
                                            player.moneyOnThePot += player.stack;
                                            moneyOnPot[a] += player.stack;
                                            sidePot += player.moneyOnThePot; //tengo k hacer metodo para sacar el sidepot entero

                                            player.getMoney(player.stack); //Now the stack of this player will be 0..
                                            player.lastAction = Actions.Allin;
                                            Console.WriteLine("El jugador {0} esta allin..", player.nickname);
                                            Console.WriteLine("POZO ACTUALIZADO: ${0}, STACK ACTUALIZADO DE {1}: ${2}", table.pot, player.nickname, player.stack);
                                            Console.WriteLine();
                                        }
                                    }
                                    if (action == Actions.Raise)
                                    {
                                        lastPlayerWhoRaised = player;
                                        player.lastAction = Actions.Raise;

                                        decimal raiseAmount = player.raiseAmount(table.pot, betToCallForMe);
                                        decimal realExtraAmount = raiseAmount - betToCallForMe;
                                        /*
                                            if (player.moneyOnThePot == table.bb) //This two if sentence are here because there was one blind more or one blind less on stacks and total pot when i go allin after just made the automatic call of the blinds
                                            {
                                                if (raiseAmount != player.stack)
                                                    realExtraAmount -= table.bb;
                                            }
                                            if (player.moneyOnThePot == table.bb / 2)
                                            {
                                                if (raiseAmount != player.stack)
                                                    realExtraAmount -= table.bb / 2;
                                            }
                                        */
                                        decimal lastStack = player.stack;
                                        player.getMoney(betToCallForMe);
                                        player.getMoney(realExtraAmount);

                                        table.pot += betToCallForMe;
                                        table.pot += realExtraAmount;
                                        player.moneyOnThePot += betToCallForMe;
                                        player.moneyOnThePot += realExtraAmount;
                                        moneyOnPot[a] += betToCallForMe;
                                        moneyOnPot[a] += realExtraAmount;
                                        betToCall = betToCall + realExtraAmount;

                                        if (raiseAmount == lastStack)
                                        {
                                            player.lastAction = Actions.Allin;
                                            Console.WriteLine("{0} ha pagado ${1}, ha subido ${2} mas y esta ALLIN || Apuesta total a pagar: ${3}", player.nickname, betToCallForMe, realExtraAmount, betToCall);
                                        }
                                        else
                                        {
                                            Console.WriteLine("{0} ha pagado ${1} y ha subido ${2} mas.. || Apuesta total a pagar: ${3}", player.nickname, betToCallForMe, realExtraAmount, betToCall);
                                        }
                                        Console.WriteLine("POZO ACTUALIZADO: ${0}, STACK ACTUALIZADO DE {1}: ${2}", table.pot, player.nickname, player.stack);
                                        Console.WriteLine();

                                    }
                                }

                            }

                        }

                    }

                }//Ends of Flop

                //STARTING TURN STREET
                if (table.status == Status.Turn)
                {
                    betToCall = 0; //New round of bets begins now..
                    Console.WriteLine();
                    Console.WriteLine("JUGANDO EL TURN, POZO: ${0}", table.pot);
                    Console.WriteLine();
                    lastPlayerWhoRaised = null;
                    firstPlayerWhoCheck = null;
                    decimal[] moneyOnPot = moneyOnPotOnThisStreet(players);

                    //SHOWING BOARD WITH FLOP + TURN CARD
                    table.showBoard(table.status);

                    while (table.status == Status.Turn)
                    {
                        if (table.areEveryPlayersAllin(players))
                        {
                            table.status = Status.River;
                            break; //This will end while's cycle right here
                        }

                        for (int i = 0; i < players.Count; i++) //Running cycle from SB to Botton..  
                        {
                            int a = i;
                            PokerPlayer player = players[a];

                            if (player.lastAction != Actions.Fold && player.lastAction != Actions.None) //If he is not folded
                            {
                                if (lastPlayerWhoRaised == player) //If this was the last player who raised the pot.. then this street is over
                                {

                                    //THIS METHOD IS TO KNOW IF SOMEBODY CALLED MY RAISE.. remember that iam the last player to speak on table..
                                    if (table.playersWhoMakeLastAction(players, Actions.Call).Count > 0)
                                    {
                                        table.status = Status.River;
                                       // table.reAdjustFoldedPlayersPropertiesToZero();
                                        break;
                                    }
                                    else
                                    {
                                        table.status = Status.None; //if nobody call my bet.. then this hand will not be played
                                        decimal winnedPot = table.pot;
                                        player.stack += table.pot;
                                        winnerPlayer = player;
                                        lastPlayerWhoRaised.lastAction = Actions.Fold;
                                        table.reAdjustFoldedPlayersPropertiesToZero(); //Now botton will be SB and BB will be UTG position
                                        table.reAdjustPlayersPosition(); //Moving players to their next position
                                        Console.WriteLine("ESTA MANO HA TERMINADO EN EL TURN. HA GANADO {0} UN POZO DE ${1}", winnerPlayer.nickname, winnedPot);

                                        Console.WriteLine("PRESIONE UNA TECLA PARA PASAR A LA SIGUIENTE MANO");
                                        Console.ReadKey();
                                        break;
                                    }
                                }
                                if (player.lastAction != Actions.Allin)
                                {
                                    if (player == firstPlayerWhoCheck && lastPlayerWhoRaised == null)
                                    {
                                        table.status = Status.River;
                                      //  table.reAdjustFoldedPlayersPropertiesToZero();
                                        break;

                                    }

                                    decimal betToCallForMe = betToCall - moneyOnPot[a]; //This is the real money that player have to call on this right moment
                                    Actions action = Actions.None; //This var will never be NONE at the end of this if sentence..
                                    Console.WriteLine();
                                    Console.WriteLine("{0} -----> STACK EFECTIVO: {1}, POZO: ${2}, APUESTA A PAGAR: {3}", player.nickname, player.stack, table.pot, betToCallForMe);

                                    if (betToCallForMe == 0)
                                    {
                                        action = player.newAction(false, true, false, true); //Player cannot fold if there is not any bet
                                    }
                                    else
                                    {
                                        if (player.stack <= betToCallForMe)
                                            action = player.newAction(true, false, true, false); //Player cannot raise
                                        if (player.stack > betToCallForMe)
                                            action = player.newAction(true, false, true, true); //Player can raise
                                    }
                                    if (action == Actions.Check)
                                    {
                                        player.lastAction = Actions.Check;
                                        if (firstPlayerWhoCheck == null)
                                            firstPlayerWhoCheck = player;
                                        Console.WriteLine("{0} ha jugado check", player.nickname);
                                        Console.WriteLine();
                                    }

                                    if (action == Actions.Fold)
                                    {
                                        player.lastAction = Actions.Fold;
                                        Console.WriteLine("{0} ha foldeado cobardemente", player.nickname);
                                        Console.WriteLine("POZO ACTUALIZADO: ${0}, STACK ACTUALIZADO DE {1}: ${2}", table.pot, player.nickname, player.stack);
                                        Console.WriteLine();
                                    }
                                    if (action == Actions.Call)
                                    {
                                        decimal amount = betToCallForMe;
                                        decimal lastStack = player.stack;
                                        if (player.getMoney(amount))
                                        {
                                            player.lastAction = Actions.Call;
                                            table.pot += amount;
                                            player.moneyOnThePot += amount;
                                            moneyOnPot[a] += amount;
                                            if (betToCall > 0 && lastPlayerWhoRaised != null) //This always will be true
                                                Console.Write("{0} ha pagado la apuesta de {1} || ", player.nickname, lastPlayerWhoRaised.nickname);
                                            if (lastStack == amount)
                                            {
                                                Console.WriteLine(" y esta ALLIN..");
                                            }
                                            Console.WriteLine("POZO ACTUALIZADO: ${0}, STACK ACTUALIZADO DE {1}: ${2}", table.pot, player.nickname, player.stack);
                                            Console.WriteLine();
                                        }
                                        else //If he could not pay the last bet completely
                                        {
                                            table.pot += player.stack;
                                            player.moneyOnThePot += player.stack;
                                            moneyOnPot[a] += player.stack;
                                            sidePot += player.moneyOnThePot; //tengo k hacer metodo para sacar el sidepot entero

                                            player.getMoney(player.stack); //Now the stack of this player will be 0..
                                            player.lastAction = Actions.Allin;
                                            Console.WriteLine("El jugador {0} esta allin..", player.nickname);
                                            Console.WriteLine("POZO ACTUALIZADO: ${0}, STACK ACTUALIZADO DE {1}: ${2}", table.pot, player.nickname, player.stack);
                                            Console.WriteLine();
                                        }
                                    }
                                    if (action == Actions.Raise)
                                    {
                                        lastPlayerWhoRaised = player;
                                        player.lastAction = Actions.Raise;

                                        decimal raiseAmount = player.raiseAmount(table.pot, betToCallForMe);
                                        decimal realExtraAmount = raiseAmount - betToCallForMe;
                                        /*
                                        if (player.moneyOnThePot == table.bb) //This two if sentence are here because there was one blind more or one blind less on stacks and total pot when i go allin after just made the automatic call of the blinds
                                        {
                                            if (raiseAmount != player.stack)
                                                realExtraAmount -= table.bb;
                                        }
                                        if (player.moneyOnThePot == table.bb / 2)
                                        {
                                            if (raiseAmount != player.stack)
                                                realExtraAmount -= table.bb / 2;
                                        }
                                        */
                                        decimal lastStack = player.stack;
                                        player.getMoney(betToCallForMe);
                                        player.getMoney(realExtraAmount);

                                        table.pot += betToCallForMe;
                                        table.pot += realExtraAmount;
                                        player.moneyOnThePot += betToCallForMe;
                                        player.moneyOnThePot += realExtraAmount;
                                        moneyOnPot[a] += betToCallForMe;
                                        moneyOnPot[a] += realExtraAmount;
                                        betToCall = betToCall + realExtraAmount;

                                        if (raiseAmount == lastStack)
                                        {
                                            player.lastAction = Actions.Allin;
                                            Console.WriteLine("{0} ha pagado ${1}, ha subido ${2} mas y esta ALLIN || Apuesta total a pagar: ${3}", player.nickname, betToCallForMe, realExtraAmount, betToCall);
                                        }
                                        else
                                        {
                                            Console.WriteLine("{0} ha pagado ${1} y ha subido ${2} mas.. || Apuesta total a pagar: ${3}", player.nickname, betToCallForMe, realExtraAmount, betToCall);
                                        }
                                        Console.WriteLine("POZO ACTUALIZADO: ${0}, STACK ACTUALIZADO DE {1}: ${2}", table.pot, player.nickname, player.stack);
                                        Console.WriteLine();

                                    }
                                }

                            }
                        }
                    }
                }//ENDS OF TURN
                //STARTING RIVER STREET
                if (table.status == Status.River)
                {
                    betToCall = 0; //New round of bets begins now..
                    Console.WriteLine();
                    Console.WriteLine("JUGANDO EL RIVER, POZO: ${0}", table.pot);
                    Console.WriteLine();
                    lastPlayerWhoRaised = null;
                    firstPlayerWhoCheck = null;
                    decimal[] moneyOnPot = moneyOnPotOnThisStreet(players);

                    //SHOWING BOARD WITH FLOP + TURN CARD + RIVER CARD
                    table.showBoard(table.status);

                    /*
                    //*************TESTING***********
                    foreach (PokerPlayer player in players)
                    {
                        player.moneyOnThePot += player.stack;
                        player.stack = 0;
                        player.lastAction = Actions.Allin;
                    }
                    lastPlayerWhoRaised = players[5];
                    //**************TESTING*******************
                    */

                    while (table.status == Status.River)
                    {
                        
                        if (table.areEveryPlayersAllin(players))
                        {
                            //LETS SEE WHO WIN THIS HAND
                            getWinnersAndShowThem(table, players); //Function to get all winners, split the table pot, and show them one by one

                            table.status = Status.None;
                            table.pot = 0;
                            table.reAdjustAllPlayersPropertiesToZero();
                            table.reAdjustPlayersPosition();
                            break;
                        }

                        for (int i = 0; i < players.Count; i++) //Running cycle from SB to Botton..  
                        {
                            int a = i;
                            PokerPlayer player = players[a];

                            if (player.lastAction != Actions.Fold && player.lastAction != Actions.None) //If he is not folded
                            {
                                if (lastPlayerWhoRaised == player)//If this was the last player who raised the pot.. then this street is over
                                {
                                    //LETS SEE WHO WIN THIS HAND
                                    getWinnersAndShowThem(table, players); //Function to get all winners, split the table pot, and show them one by one

                                    table.status = Status.None;
                                    table.pot = 0;
                                    table.reAdjustAllPlayersPropertiesToZero();
                                    table.reAdjustPlayersPosition();
                                    break;
                                }

                                if (player.lastAction != Actions.Allin)
                                {
                                    if (player == firstPlayerWhoCheck && lastPlayerWhoRaised == null)
                                    {
                                        //LETS SEE WHO WIN THIS HAND..
                                        getWinnersAndShowThem(table, players); //Function to get all winners, split the table pot, and show them one by one
                                        table.status = Status.None;
                                        table.pot = 0;
                                        table.reAdjustAllPlayersPropertiesToZero();
                                        table.reAdjustPlayersPosition();
                                        break;
                                    }

                                    decimal betToCallForMe = betToCall - moneyOnPot[a]; //This is the real money that player have to call on this right moment
                                    Actions action = Actions.None; //This var will never be NONE at the end of this if sentence..
                                    Console.WriteLine();
                                    Console.WriteLine("{0} -----> STACK EFECTIVO: {1}, POZO: ${2}, APUESTA A PAGAR: {3}", player.nickname, player.stack, table.pot, betToCallForMe);

                                    if (betToCallForMe == 0)
                                    {
                                        action = player.newAction(false, true, false, true); //Player cannot fold if there is not any bet
                                    }
                                    else
                                    {
                                        if (player.stack <= betToCallForMe)
                                            action = player.newAction(true, false, true, false); //Player cannot raise
                                        if (player.stack > betToCallForMe)
                                            action = player.newAction(true, false, true, true); //Player can raise
                                    }
                                    if (action == Actions.Check)
                                    {
                                        player.lastAction = Actions.Check;
                                        if (firstPlayerWhoCheck == null)
                                            firstPlayerWhoCheck = player;
                                        Console.WriteLine("{0} ha jugado check", player.nickname);
                                        Console.WriteLine();
                                    }

                                    if (action == Actions.Fold)
                                    {
                                        player.lastAction = Actions.Fold;
                                        Console.WriteLine("{0} ha foldeado cobardemente", player.nickname);
                                        Console.WriteLine("POZO ACTUALIZADO: ${0}, STACK ACTUALIZADO DE {1}: ${2}", table.pot, player.nickname, player.stack);
                                        Console.WriteLine();
                                    }
                                    if (action == Actions.Call)
                                    {
                                        decimal amount = betToCallForMe;
                                        decimal lastStack = player.stack;
                                        if (player.getMoney(amount))
                                        {
                                            player.lastAction = Actions.Call;
                                            table.pot += amount;
                                            player.moneyOnThePot += amount;
                                            moneyOnPot[a] += amount;
                                            if (betToCall > 0 && lastPlayerWhoRaised != null) //This always will be true
                                                Console.WriteLine("{0} ha pagado la apuesta de {1} || ", player.nickname, lastPlayerWhoRaised.nickname);
                                            if (lastStack == amount)
                                            {
                                                player.lastAction = Actions.Allin;
                                                Console.WriteLine(" y esta ALLIN..");
                                            }
                                            Console.WriteLine("POZO ACTUALIZADO: ${0}, STACK ACTUALIZADO DE {1}: ${2}", table.pot, player.nickname, player.stack);
                                            Console.WriteLine();
                                        }
                                        else //If he could not pay the last bet completely
                                        {
                                            table.pot += player.stack;
                                            player.moneyOnThePot += player.stack;
                                            moneyOnPot[a] += player.stack;
                                            sidePot += player.moneyOnThePot; //tengo k hacer metodo para sacar el sidepot entero

                                            player.getMoney(player.stack); //Now the stack of this player will be 0..
                                            player.lastAction = Actions.Allin;
                                            Console.WriteLine("El jugador {0} esta allin..", player.nickname);
                                            Console.WriteLine("POZO ACTUALIZADO: ${0}, STACK ACTUALIZADO DE {1}: ${2}", table.pot, player.nickname, player.stack);
                                            Console.WriteLine();
                                        }
                                    }
                                    if (action == Actions.Raise)
                                    {
                                        lastPlayerWhoRaised = player;
                                        player.lastAction = Actions.Raise;

                                        decimal raiseAmount = player.raiseAmount(table.pot, betToCallForMe);
                                        decimal realExtraAmount = raiseAmount - betToCallForMe;
                                        /*
                                        if (player.moneyOnThePot == table.bb) //This two if sentence are here because there was one blind more or one blind less on stacks and total pot when i go allin after just made the automatic call of the blinds
                                        {
                                            if (raiseAmount != player.stack)
                                                realExtraAmount -= table.bb;
                                        }
                                        if (player.moneyOnThePot == table.bb / 2)
                                        {
                                            if (raiseAmount != player.stack)
                                                realExtraAmount -= table.bb / 2;
                                        }
                                        */
                                        decimal lastStack = player.stack;
                                        player.getMoney(betToCallForMe);
                                        player.getMoney(realExtraAmount);

                                        table.pot += betToCallForMe;
                                        table.pot += realExtraAmount;
                                        player.moneyOnThePot += betToCallForMe;
                                        player.moneyOnThePot += realExtraAmount;
                                        moneyOnPot[a] += betToCallForMe;
                                        moneyOnPot[a] += realExtraAmount;
                                        betToCall = betToCall + realExtraAmount;

                                        if (raiseAmount == lastStack)
                                        {
                                            player.lastAction = Actions.Allin;
                                            Console.WriteLine("{0} ha pagado ${1}, ha subido ${2} mas y esta ALLIN || Apuesta total a pagar: ${3}", player.nickname, betToCallForMe, realExtraAmount, betToCall);
                                        }
                                        else
                                        {
                                            Console.WriteLine("{0} ha pagado ${1} y ha subido ${2} mas.. || Apuesta total a pagar: ${3}", player.nickname, betToCallForMe, realExtraAmount, betToCall);
                                        }
                                        Console.WriteLine("POZO ACTUALIZADO: ${0}, STACK ACTUALIZADO DE {1}: ${2}", table.pot, player.nickname, player.stack);
                                        Console.WriteLine();
                                    }
                                }
                            }

                        }
                    }

                }//ENDS OF RIVER
            } //ENDS OF WHILE
            //HERE WILL BE THE METHODS FOR UPDATE PLAYERS STACK


        }

        //Create a list of decimal with value 0 for save the money of players on this street of game
        public decimal[] moneyOnPotOnThisStreet(List<PokerPlayer> players)
        {
            decimal[] money = new decimal[players.Count];
            int i = 0;
            foreach (PokerPlayer player in players)
            {
                money[i] = 0;
                i++;
            }
            return money;
        }


        /// <summary>
        /// Function to get all winners, split the table pot between them, and show them one by one
        /// </summary>
        /// <param name="table"></param>
        /// <param name="players"></param>
        public void getWinnersAndShowThem(PokerTable table, List<PokerPlayer> players)
        {
            List<PokerPlayer> activeAndNotFoldedPlayers = table.playersWhoDidNotMakeLastAction(players, Actions.Fold); //This is all active and not folded players on this hand..
            List<PokerPlayer> playersWhoCanWinThePot = new List<PokerPlayer>(); //Total of players with money on pot to split, this list will keep original elements to the end
            playersWhoCanWinThePot.AddRange(activeAndNotFoldedPlayers);
            //above list will never be empty because lastPlayerWhoRaised is not null..

            List<PokerPlayer> bestHandPlayers = new List<PokerPlayer>(); //Player(s) with the best hand
            List<PokerPlayer> winnerPlayers = new List<PokerPlayer>();

            decimal sidePot = 0;
            decimal substractedMoney = 0; //To know money substracted from table pot
            List<PokerPlayer> playersWhoCanWinThePotSorted = playersWhoCanWinThePot.OrderByDescending(p => p.moneyOnThePot).ToList();
            playersWhoCanWinThePotSorted.Reverse(); //List of all players in this hand, from min to max stack on pot

            Console.WriteLine("!!!ESTA MANO HA TERMINADO EN EL RIVER!!!");
            Console.WriteLine("RESULTADOS:");
            Console.WriteLine();

            if (playersWhoCanWinThePot.Count > 1)
            { //IF THERE ARE AT LEAST TWO PLAYERS NOT FOLDED.. IT MEANS SOMEBODY IS ALLIN OR SOMEBODY CALLED MY RAISE ON RIVER..
                PokerPlayer biggerPlayerOnList = playersWhoCanWinThePotSorted[playersWhoCanWinThePotSorted.Count - 1];
                if (table.playersWithSameOrMajorMoneyOnPotThanMe(playersWhoCanWinThePotSorted, biggerPlayerOnList.moneyOnThePot).Count <= 1) //im he has more money on pot than everybody else.. then he gets the difference with the bigger stack player before him
                {
                    decimal stackDiference = biggerPlayerOnList.moneyOnThePot - playersWhoCanWinThePotSorted[playersWhoCanWinThePotSorted.Count - 2].moneyOnThePot;
                    biggerPlayerOnList.moneyOnThePot -= stackDiference;
                    biggerPlayerOnList.stack += stackDiference;
                    table.pot -= stackDiference;
                    Console.WriteLine("[[[El jugador {0} recoge ${1} del bote como monto sobrante, pozo restante total: ${2}]]]", biggerPlayerOnList.nickname, stackDiference, table.pot);
                    Console.WriteLine();
                }
                while (playersWhoCanWinThePotSorted.Count > 0)
                {

                    sidePot = 0;
                    bestHandPlayers = table.winnerPlayers(playersWhoCanWinThePotSorted); //Player(s) in playersWhoCanWinThePotSorted with the best hand
                    List<PokerPlayer> playersToSplitSidePot = new List<PokerPlayer>();
                    PokerPlayer shorterStackPlayer = bestHandPlayers[0]; //This list will always have at least one element
                    List<PokerPlayer> bestHandPlayerSorted = bestHandPlayers.OrderByDescending(p => p.moneyOnThePot).ToList();
                    bestHandPlayerSorted.Reverse(); //order from min to max by moneyOnPot.. sidePot of d+1player will never be less than sidePot of player[d]
                    decimal winnedMoney = 0;

                    for (int d = 0; d < bestHandPlayerSorted.Count; d++)
                    {
                        PokerPlayer winnerPlayer1 = bestHandPlayerSorted[d];
                        sidePot = table.sidepotForPlayer(winnerPlayer1);
                        if (d == 0)
                        {
                            winnedMoney += (sidePot - substractedMoney) / bestHandPlayerSorted.Count; //because if this is while cycle round two or more, winner players with shorter stack before this round taked money from table pot
                        }
                        else
                        {
                            decimal SidePotOfPlayerBefore = table.sidepotForPlayer(bestHandPlayerSorted[d - 1]);
                            /* if (sidePot > SidePotOfPlayerBefore)
                             {*/
                            decimal diference = sidePot - SidePotOfPlayerBefore;
                            int playersToSplit = table.playersWithSameOrMajorMoneyOnPotThanMe(bestHandPlayerSorted, winnerPlayer1.moneyOnThePot).Count; //If there is nobody except me, then Count = 1..
                            winnedMoney = winnedMoney + (diference / playersToSplit); //If player bafore has same sidePot than me, difference will be 0, so i will just add value of winnedMoney var
                        }
                        winnerPlayer1.stack += winnedMoney;
                        substractedMoney += winnedMoney;
                        table.pot -= winnedMoney;
                        winnerPlayers.Add(winnerPlayer1);
                        Console.WriteLine(" - El jugador {0} ha ganado un bote de ${1}.. pozo restante de la mesa: ${2}", winnerPlayer1.nickname, winnedMoney, table.pot);
                        Console.WriteLine("       Presione una tecla para continuar...");
                        ConsoleKeyInfo teclaPresionada = Console.ReadKey();
                    }
                    //DELETING ALL PLAYERS WILL SAME OR LESS MONEY ON POT THAN WINNER PLAYERS OF THIS HAND
                    PokerPlayer biggerStackPlayer = bestHandPlayerSorted[bestHandPlayerSorted.Count - 1];
                    List<PokerPlayer> shorterAndSamePlayers = table.playersWithSameOrMinorMoneyOnPotThanMe(playersWhoCanWinThePot, biggerStackPlayer.moneyOnThePot);
                    for (int e = 0; e < shorterAndSamePlayers.Count; e++)
                    {
                        playersWhoCanWinThePotSorted.Remove(shorterAndSamePlayers[e]); //This players will always exists on playersWhoCanWinThePotSorted list
                    }
                }
            }//END IF OF WINNER PLAYERS.COUNT >1
            else
            {
                PokerPlayer bigWinner = playersWhoCanWinThePot[0];
                bigWinner.stack += table.pot;
                winnerPlayers.Add(bigWinner);
                Console.WriteLine(" - Los demas foldearon y el jugador {0} ha ganado todo el bote de la mesa: ${1}", bigWinner.nickname, table.pot);
            }
            
        }

    }
}
