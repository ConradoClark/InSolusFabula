using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public static class Global
{
    public static decimal Money = 100M;

    public static class Decisions
    {
        // Nenia
        public static bool YouRememberNenia;
        public static bool NeniaIsYourGirlfriend;
        public static bool NeniaIsYourFriend;
        public static bool YouAreACreep;
        public static bool NeniaIsNobody;

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

        // Shopkeeper
        public static bool YouBoughtTheGoldRing;

        // Crop Blank
        public static bool YouAskedForALoan;
        public static bool YouBecameRich;

        // Jackspot
        public static bool YouHitJackpot;
        public static bool YouAreBankrupt;
    }

    public static class Inventory
    {
        public static bool HasRing = false;
    }
}