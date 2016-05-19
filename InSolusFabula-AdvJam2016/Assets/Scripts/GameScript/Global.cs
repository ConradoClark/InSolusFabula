using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public static class Global
{
    public static decimal Money = 400M;
    public static decimal GoldRingPrice = 500M;
    public static decimal DeltaRunnerFee = 20M;
    public static decimal BlueCardRevealPrice = 100M;
    public static bool DeltaRunnerPaused = false;
    public static int DeltaRunnerLevel = 0;
    public static string Time = "10:00 AM";
    public static decimal LoanValue = 50M;
    public static decimal LoanTaxes = 1.5M;
    public static decimal EarlyChickenPrice = 19.99M;
    public static decimal LateSteakPrice = 23.99M;

    public static class Decisions
    {
        // Nenia
        public static bool NeniaFirstFateShift;
        public static bool YouRememberNenia;
        public static bool YouRememberNenia_Confident;
        public static bool YouRememberNenia_Poor;
        public static bool NeniaIsYourSister;
        public static bool NeniaIsYourFriend;
        public static bool YouAreACreep;
        public static bool NeniaIsNobody;

        // Nenia - Memories
        public static bool FiguredOutNeniaIsSister;
        public static bool FiguredOutNeniaIsFriend;
        public static bool FiguredOutYouAreACreep;

        // Nenia - Train
        public static bool Jump_TooEarly;
        public static bool YouRememberNenia_Jump;
        public static bool YouRememberNenia_DontJump;
        public static bool NeniaIsGirlFriend_Jump;
        public static bool NeniaIsGirlFriend_DontJump;
        public static bool NeniaIsFriend_Jump;
        public static bool NeniaIsFriend_DontJump;
        public static bool YouAreACreep_Jump;
        public static bool YouAreACreep_DontJump;

        // Blue Card
        public static bool BlueCardWrittenAboutTime;
        public static bool BlueCardWrittenAboutReality;
        public static bool BlueCardWrittenAboutLove;
        public static bool BlueCardWrittenAboutFamily;
        public static bool BlueCardWrittenAboutCalim;
        public static int BlueCardCount = 3;

        // Shopkeeper
        public static bool YouBoughtTheGoldRing;

        // Crop Blank
        public static bool YouAskedForALoan;
        public static int TimesYouAskedForALoan;
        public static bool YouBecameRich;

        // Jackspot
        public static bool YouHitJackpot;
        public static bool YouAreBankrupt;

        // Food Court
        public static bool YouBoughtFood;
        public static bool YouBoughtEarlyFood;
        public static bool YouBoughtLateFood;
    }

    public static class Inventory
    {
        public static int Ring = 0;
        public static int Time = 1;
        public static int Reality = 2;
        public static int Love = 3;
        public static int Family = 4;
        public static int Calim = 5;
        public static int EarlyChicken = 6;
        public static int LateSteak = 7;

        public static bool[] HasItem = { false, false, false, false, false, false,false, false};
    }
}