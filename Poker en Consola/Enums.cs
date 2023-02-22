using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poker_en_Consola
{
    internal class Enums
    {
    }

    public enum Suit //Este es el palo de la carta con numeros de identificacion
    {
        Club = 1,
        Heart = 2,
        Spade = 3,
        Diamond = 4
    }


    public enum Hands //Esta son todas las distintas manos ganadoras posibles enumeradas
    {
        HighCard = 0,
        Pair = 1,
        DoublePair = 2,
        Set = 3,
        Straight = 4,
        Flush = 5,
        Fullhouse = 6,
        Quads = 7,
        Straightflush = 8,
        Royalflush = 9
    }

    public enum PokerGameModes //modos de juego
    {
        Texas = 0,
        Omaha = 1
    }

    public enum Actions /// To know in class Player what was the last action made by player X
    {
        None = 0,
        Fold = 1,
        Check = 2,
        Call = 3,
        Raise = 4,
            Allin = 5
    }

    public enum GameRounds //Diferentes fases del juego
    {
        Preview = 0,
        PreFlop = 1,
        Flop = 2,
        Turn = 3,
        River = 4,
        Finished = 5
    }

    public enum Games
    {
        Poker,
        Trio
    }

    public enum Status
    {
        None = 0,
        Preflop =1,
        Flop =2,
        Turn =3,
        River =4
    }

   
    

}
