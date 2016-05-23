using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Ending : MonoBehaviour
{
    public TextComponent endingText;
    private bool resetGame;
    void Start()
    {
        StartCoroutine(End());
    }

    void ResetGame()
    {
        Global.Money = 39.50M;
        Global.GoldRingPrice = 500M;
        Global.DeltaRunnerFee = 20M;
        Global.BlueCardRevealPrice = 100M;
        Global.DeltaRunnerPaused = false;
        Global.DeltaRunnerLevel = 0;
        Global.Time = "10:00 AM";
        Global.LoanValue = 50M;
        Global.LoanTaxes = 1.5M;
        Global.EarlyChickenPrice = 19.99M;
        Global.LateSteakPrice = 23.99M;
        Global.Decisions.NeniaFirstFateShift = false;
        Global.Decisions.YouRememberNenia = false;
        Global.Decisions.YouRememberNenia_Confident = false;
        Global.Decisions.YouRememberNenia_Poor = false;
        Global.Decisions.NeniaIsYourSister = false;
        Global.Decisions.NeniaIsYourFriend = false;
        Global.Decisions.YouAreACreep = false;
        Global.Decisions.NeniaIsNobody = false;
        // Nenia - Memories
        Global.Decisions.FiguredOutNeniaIsSister = false;
        Global.Decisions.FiguredOutNeniaIsFriend = false;
        Global.Decisions.FiguredOutYouAreACreep = false;

        // Nenia - Train
        Global.Decisions.Jump_TooEarly = false;
        Global.Decisions.YouRememberNenia_Jump = false;
        Global.Decisions.YouRememberNenia_DontJump = false;
        Global.Decisions.NeniaIsGirlFriend_Jump = false;
        Global.Decisions.NeniaIsGirlFriend_DontJump = false;
        Global.Decisions.NeniaIsFriend_Jump = false;
        Global.Decisions.NeniaIsFriend_DontJump = false;
        Global.Decisions.YouAreACreep_Jump = false;
        Global.Decisions.YouAreACreep_DontJump = false;

        // Blue Card
        Global.Decisions.BlueCardWrittenAboutTime = false;
        Global.Decisions.BlueCardWrittenAboutReality = false;
        Global.Decisions.BlueCardWrittenAboutLove = false;
        Global.Decisions.BlueCardWrittenAboutFamily = false;
        Global.Decisions.BlueCardWrittenAboutCalim = false;
        Global.Decisions.BlueCardCount = 3;

        // Shopkeeper
        Global.Decisions.YouBoughtTheGoldRing = false;

        // Crop Blank
        Global.Decisions.YouAskedForALoan = false;
        Global.Decisions.TimesYouAskedForALoan = 0;
        Global.Decisions.YouBecameRich = false;

        // Jackspot
        Global.Decisions.YouHitJackpot = false;
        Global.Decisions.YouAreBankrupt = false;

        // Food Court
        Global.Decisions.YouBoughtFood = false;
        Global.Decisions.YouBoughtEarlyFood = false;
        Global.Decisions.YouBoughtLateFood = false;

        Global.Decisions.AteFoodWithNenia = false;
        Global.Decisions.EndedBefore = true;
        Global.Decisions.AteFoodAlone = false;
        Global.Inventory.HasItem = new bool [] { false, false, false, false, false, false, false, false };

        SceneManager.LoadScene("Mall_Central");
    }

    IEnumerator End()
    {
        yield return WriteEnding();
        if (resetGame)
        {
            ResetGame();
        }
    }

    IEnumerator WriteEnding()
    {
        yield return endingText.SetText("");
        yield return new WaitForSeconds(1.5f);
        if (Global.Decisions.EndedBefore)
        {
            yield return endingText.AppendText("And here you are again.");
        }

        if (Global.Decisions.BlueCardWrittenAboutTime)
        {
            yield return endingText.AppendText("You chose TIME. ");
            yield return new WaitForSeconds(0.5f);
            yield return endingText.AppendText("You know that time heals. ");
            yield return new WaitForSeconds(1f);
            yield return endingText.AppendText("And kills. ");
            yield return new WaitForSeconds(1f);
        }

        if (Global.Decisions.BlueCardWrittenAboutReality)
        {
            yield return endingText.AppendText("You chose REALITY. ");
            yield return new WaitForSeconds(0.5f);
            yield return endingText.AppendText("You chose to accept yourself. ");
            yield return new WaitForSeconds(1f);
        }

        if (Global.Decisions.BlueCardWrittenAboutLove)
        {
            yield return endingText.AppendText("You chose LOVE. ");
            yield return new WaitForSeconds(0.5f);
            yield return endingText.AppendText("You know you are alive. Or at least you should be. ");
            yield return new WaitForSeconds(1f);
        }

        if (Global.Decisions.BlueCardWrittenAboutFamily)
        {
            yield return endingText.AppendText("You chose FAMILY. ");
            yield return new WaitForSeconds(0.5f);
            yield return endingText.AppendText("You don't want to let go of the bonds you've created.");
            yield return new WaitForSeconds(1f);
            yield return endingText.AppendText("Are you really free? ");
            yield return new WaitForSeconds(1f);
        }

        if (Global.Decisions.BlueCardWrittenAboutCalim)
        {
            yield return endingText.AppendText("You chose CALIM. ");
            yield return new WaitForSeconds(0.5f);
            yield return endingText.AppendText("You looked into yourself.");
            yield return new WaitForSeconds(1f);
            yield return endingText.AppendText("You've found nothing but deception. ");
            yield return new WaitForSeconds(1f);
        }

        if (Global.Decisions.YouAreACreep)
        {
            yield return endingText.AppendText("Is this who you really are? ");
            yield return new WaitForSeconds(1f);
            yield return endingText.AppendText("A stalker... a creep? ");
            yield return new WaitForSeconds(1f);
            yield return endingText.AppendText("So twisted and turned to the point you would kill someone you desire? ");
            yield return new WaitForSeconds(1f);
            if (!Global.Decisions.BlueCardWrittenAboutLove)
            {
                yield return endingText.AppendText("This isn't exactly what you remember. But people always avoided you. ");
                yield return new WaitForSeconds(1f);
                yield return endingText.AppendText("They are afraid to be like you. To seek the things you seek. ");
                yield return new WaitForSeconds(1f);
            }
        }

        if (!Global.Decisions.AteFoodAlone && !Global.Decisions.AteFoodWithNenia)
        {
            yield return endingText.AppendText("You forgot something important. ");
            yield return new WaitForSeconds(1f);
            yield return endingText.AppendText("It's still 10:00 AM. Nothing really happened. ");
            yield return new WaitForSeconds(1f);
            yield return endingText.AppendText("Listen to a word of advice. You can't get away if you don't turn to the clock. ");
            yield return new WaitForSeconds(1f);
        }

        if (Global.Decisions.YouBoughtEarlyFood && (Global.Decisions.AteFoodAlone || Global.Decisions.AteFoodWithNenia) && Global.Decisions.BlueCardWrittenAboutTime)
        {
            yield return endingText.AppendText("You were trying to avoid the train accident at all costs. Why? ");
            yield return new WaitForSeconds(1f);
        }

        if (Global.Decisions.YouBoughtLateFood && (Global.Decisions.AteFoodAlone || Global.Decisions.AteFoodWithNenia) && Global.Decisions.BlueCardWrittenAboutTime)
        {
            yield return endingText.AppendText("You clearly wanted that train to hit someone, didn't you? ");
            yield return new WaitForSeconds(1f);
        }

        if (Global.Decisions.YouAreACreep && Global.Decisions.YouBoughtEarlyFood && Global.Decisions.AteFoodAlone && Global.Decisions.BlueCardWrittenAboutLove
            && Global.Time == "10:37 AM")
        {
            yield return endingText.AppendText("You were so close. ");
            yield return new WaitForSeconds(1f);

            yield return endingText.AppendText("You did everything right. ");
            yield return new WaitForSeconds(1f);

            yield return endingText.AppendText("But you died. ");
            yield return new WaitForSeconds(0.5f);

            yield return endingText.AppendText("Why? ");
            yield return new WaitForSeconds(1f);

            yield return endingText.AppendText("It is too soon. You can't die yet. ");
            yield return new WaitForSeconds(1f);

            yield return endingText.AppendText("Not in your own mind... ");
            yield return new WaitForSeconds(1f);

            yield return endingText.AppendText("I will send you back. Now please, hang in there just for a little more. ");
            yield return new WaitForSeconds(1f);

            endingText.TimeBetweenCharacters = 0.4f;
            endingText.SetColor(Color.red);
            yield return endingText.AppendText("SEE YOU SOON... ");
            resetGame = true;
            endingText.DialogueArrow.IsBlinking = true;
            yield return WaitForClick();
            yield break;
        }

        else if (Global.Decisions.YouAreACreep && Global.Decisions.YouBoughtEarlyFood && Global.Decisions.AteFoodAlone && Global.Decisions.BlueCardWrittenAboutLove)
        {
            yield return endingText.AppendText("After this neverending nightmare, you finally open your eyes. ");
            yield return new WaitForSeconds(1f);
            yield return endingText.AppendText("You wake up at the hospital. ");
            yield return new WaitForSeconds(1f);
            yield return endingText.AppendText("You look around. ");
            yield return new WaitForSeconds(0.5f);
            yield return endingText.AppendText("No signs of Nenia. ");
            yield return new WaitForSeconds(0.5f);
            yield return endingText.AppendText("No signs of anyone. ");
            yield return new WaitForSeconds(0.5f);
            yield return endingText.AppendText("You get up, then you suddenly see her silhouette. ");
            yield return new WaitForSeconds(1f);
            yield return endingText.AppendText("'I finally found you'. ");
            yield return new WaitForSeconds(0.5f);
            yield return endingText.AppendText("You walk towards her, and she seems to run away, laughing. ");
            yield return new WaitForSeconds(1f);
            yield return endingText.AppendText("You follow her upstairs, ");
            yield return new WaitForSeconds(0.5f);
            yield return endingText.AppendText("until you both get to the top of the building. ");
            yield return new WaitForSeconds(1f);
            yield return endingText.AppendText("Until then, you never really knew what her face was like. ");
            yield return new WaitForSeconds(1f);
            yield return endingText.AppendText("Her smell of flowers entices you, ");
            yield return new WaitForSeconds(0.5f);
            yield return endingText.AppendText("as she approaches the corner of the roof. ");
            yield return new WaitForSeconds(0.5f);
            yield return endingText.AppendText("She turns back at you. ");
            yield return new WaitForSeconds(1f);
            yield return endingText.AppendText("You ask: ");
            yield return new WaitForSeconds(1f);
            yield return endingText.AppendText("Nenia... who are you after all? ");
            yield return new WaitForSeconds(1f);
            yield return endingText.AppendText("She stays silent, as if you knew all along. ");
            yield return new WaitForSeconds(1f);
            yield return endingText.AppendText("You hug her, tighter than you ever could. ");
            yield return new WaitForSeconds(1f);
            yield return endingText.AppendText("Her hair wavers on thin air. ");
            yield return new WaitForSeconds(1f);
            yield return endingText.AppendText("You touch, deeply, her beautiful silhouette. ");
            endingText.TimeBetweenCharacters = 0.5f;
            yield return endingText.AppendText("You're finally free. ");
            yield return new WaitForSeconds(1f);
            yield return endingText.AppendText("You smile, for one last time. ");
            yield return new WaitForSeconds(2f);
            endingText.TimeBetweenCharacters = 1f;
            yield return endingText.AppendText("You fall. ");
            yield return new WaitForSeconds(1f);
            endingText.SetColor(Color.green);
            yield return endingText.AppendText("THE END. ");

            yield return WaitForClick();
            yield break;
        }

        if (!Global.Decisions.NeniaFirstFateShift)
        {
            yield return endingText.AppendText("You didn't get to the bottom of this. ");
            yield return new WaitForSeconds(0.5f);
            yield return endingText.AppendText("You were just curious. ");
            yield return new WaitForSeconds(1f);
            yield return endingText.AppendText("'What happens if I kill myself?' - You thought, with your mind at ease. ");
            yield return new WaitForSeconds(1.5f);
            yield return endingText.AppendText("It seemed easy. ");
            yield return new WaitForSeconds(1f);
            yield return endingText.AppendText("It always does. ");
            yield return new WaitForSeconds(1f);
            yield return endingText.AppendText("Yet, you're here, waiting for the resolution of the purgatory you've put yourself into. ");
            yield return new WaitForSeconds(0.5f);
            yield return endingText.AppendText("You know what happens next right? ");
            yield return new WaitForSeconds(1f);
            yield return endingText.AppendText("I won't take your time, even if you effortlessly wasted mine. ");
            yield return new WaitForSeconds(1f);
            endingText.TimeBetweenCharacters = 0.4f;
            endingText.SetColor(Color.red);
            yield return endingText.AppendText("SEE YOU SOON... ");
            resetGame = true;
            endingText.DialogueArrow.IsBlinking = true;
            yield return WaitForClick();
            yield break;
        }

        if (Global.Decisions.YouRememberNenia)
        {
            yield return endingText.AppendText("You seem to have gone along with the easiest path. ");
            yield return new WaitForSeconds(0.5f);
            yield return endingText.AppendText("You believed your own resolve, ");
            yield return new WaitForSeconds(0.5f);
            yield return endingText.AppendText("you went forward relentlessly, ");
            yield return new WaitForSeconds(0.5f);
            yield return endingText.AppendText("trying to find something to hold on. ");
            yield return new WaitForSeconds(1f);
            yield return endingText.AppendText("What you've found is a lie. ");
            yield return new WaitForSeconds(1f);

            if (Global.Decisions.YouBoughtTheGoldRing)
            {
                yield return endingText.AppendText("That gold ring you bought is made of broken dreams and deceptions. ");
                yield return new WaitForSeconds(1f);
                yield return endingText.AppendText("She never needed one. ");
                yield return new WaitForSeconds(1f);
                yield return endingText.AppendText("She never wanted one. ");
                yield return new WaitForSeconds(1f);
            }

            yield return endingText.AppendText("Nenia wanted you, yes. ");
            yield return new WaitForSeconds(0.5f);

            yield return endingText.AppendText("She still does. ");
            yield return new WaitForSeconds(0.5f);

            yield return endingText.AppendText("But this is not a marriage, ");
            yield return new WaitForSeconds(1f);
            yield return endingText.AppendText("this is deeper. ");
            yield return new WaitForSeconds(0.5f);

            yield return endingText.AppendText("You know... I like you. ");
            yield return new WaitForSeconds(1.3f);

            yield return endingText.AppendText("And I truly hope that you'll find your way out... ");
            yield return new WaitForSeconds(1f);

            if (!Global.Decisions.BlueCardWrittenAboutLove)
            {
                yield return endingText.AppendText("Please understand that you need to be alive to find out. ");
                yield return new WaitForSeconds(1f);
            }

            if (!Global.Decisions.BlueCardWrittenAboutReality)
            {
                yield return endingText.AppendText("And don't be attached to illusions. ");
                yield return new WaitForSeconds(1f);
            }

            endingText.TimeBetweenCharacters = 0.4f;
            endingText.SetColor(Color.red);
            yield return endingText.AppendText("SEE YOU SOON... ");
            resetGame = true;
            endingText.DialogueArrow.IsBlinking = true;
            yield return WaitForClick();
            yield break;
        }

        if (Global.Decisions.NeniaIsYourSister)
        {
            yield return endingText.AppendText("Now, this is closer. ");
            yield return new WaitForSeconds(0.5f);
            yield return endingText.AppendText("She is really close to a sister. ");
            yield return new WaitForSeconds(0.5f);
            yield return endingText.AppendText("But not quite. ");
            yield return new WaitForSeconds(0.5f);
            yield return endingText.AppendText("You're trying to find words to describe a feeling that... ");
            yield return new WaitForSeconds(1f);
            yield return endingText.AppendText("And don't get me wrong. ");
            yield return new WaitForSeconds(0.5f);
            yield return endingText.AppendText("It IS indeed a blood relation. ");
            yield return new WaitForSeconds(1f);
            yield return endingText.AppendText("Albeit a very different one... ");
            yield return new WaitForSeconds(1f);

            if (!Global.Decisions.BlueCardWrittenAboutLove)
            {
                yield return endingText.AppendText("Just remember! You didn't succeed. Not yet. ");
                yield return new WaitForSeconds(1f);
            }

            if (!Global.Decisions.BlueCardWrittenAboutReality)
            {
                yield return endingText.AppendText("And don't be attached to illusions. ");
                yield return new WaitForSeconds(1f);
            }

            yield return endingText.AppendText("Maybe in TIME you'll know what's REALLY going with your LIFE. ");
            yield return endingText.AppendText("You're very close now. Keep going... ");
            yield return new WaitForSeconds(1f);

            endingText.TimeBetweenCharacters = 0.4f;
            endingText.SetColor(Color.red);
            yield return endingText.AppendText("SEE YOU SOON... ");
            resetGame = true;
            endingText.DialogueArrow.IsBlinking = true;
            yield return WaitForClick();
            yield break;
        }

        if (Global.Decisions.NeniaIsYourFriend)
        {
            yield return endingText.AppendText("You're jealous, yeah. ");
            yield return new WaitForSeconds(0.5f);
            yield return endingText.AppendText("You want her all to yourself, and you don't have her yet. ");
            yield return new WaitForSeconds(0.5f);
            yield return endingText.AppendText("But don't worry... ");
            yield return new WaitForSeconds(0.5f);
            yield return endingText.AppendText("She is all yours to take. If you push the right buttons... ");
            yield return new WaitForSeconds(1f);

            if (!Global.Decisions.BlueCardWrittenAboutLove)
            {
                yield return endingText.AppendText("Ask yourself? Are you alive? ");
                yield return new WaitForSeconds(1f);
            }

            if (!Global.Decisions.BlueCardWrittenAboutReality)
            {
                yield return endingText.AppendText("And don't be attached to illusions. ");
                yield return new WaitForSeconds(1f);
            }

            yield return endingText.AppendText("I think you're getting the hang of it. Maybe in TIME you'll know what's REALLY going with your LIFE. ");
            yield return new WaitForSeconds(1f);

            endingText.TimeBetweenCharacters = 0.4f;
            endingText.SetColor(Color.red);
            yield return endingText.AppendText("SEE YOU SOON... ");
            resetGame = true;
            endingText.DialogueArrow.IsBlinking = true;
            yield return WaitForClick();
            yield break;
        }

        endingText.TimeBetweenCharacters = 0.4f;
        endingText.SetColor(Color.red);
        yield return endingText.AppendText("SEE YOU SOON... ");
        resetGame = true;
        endingText.DialogueArrow.IsBlinking = true;
        yield return WaitForClick();
        yield break;
    }

    IEnumerator WaitForClick()
    {
        bool wait=true;
        while (wait)
        {
            if (Input.GetMouseButtonDown(0))
            {
                wait = false;
            }
            yield return 1;
        }
    }
}