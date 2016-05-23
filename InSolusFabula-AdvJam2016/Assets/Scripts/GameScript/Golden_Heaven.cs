using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityStandardAssets.ImageEffects;

public class Golden_Heaven : MonoBehaviour
{
    [Header("Cameras")]
    public Camera cam;
    public Camera uicam;
    [Header("Dialogue")]
    public BlurOptimized cameraBlur;
    public SmoothIntoPosition dialogueSmoothIn;
    public TextComponent mainTextBox;
    public TextComponent subtitleTextBox;
    public TextComponent option1;
    public TextComponent option2;
    public TextComponent option3;
    public Color shopkeeperDialogColor;
    public Color mcDialogColor;
    public Color mcOtherDialogColor;
    public Color goldRingTextColor;
    public Color jackspotColor;
    public Color objectDialogColor;
    public Color neniaDialogColor;
    public ColliderMouseOver mainTextBoxCollider;
    [Header("Dialogue Portraits")]
    public SmoothIntoPosition shopKeeperFadeIn;
    public SmoothIntoPosition neniaFacingBack;
    public SmoothIntoPosition neniaFiance;
    [Header("Background")]
    public SpriteRenderer goldenHeaven1;
    public SpriteRenderer goldenHeaven2;
    public SpriteRenderer sceneLight;
    [Header("Scene Objects")]
    public SpriteRenderer goldRing;
    public ColliderMouseOver shopkeeper;
    public ColliderMouseOver nenia;
    public ColliderMouseOver goldring;
    public Transform neniaTransform;
    [Header("UI")]
    public ColliderMouseOver backButton;
    public Inventory inventory;
    [Header("Sounds")]
    public AudioSource backgroundMusic;
    public AudioSource hiss;
    [Header("BackButton")]
    public string backScene;
    public SpriteRenderer transition;
    private bool busy;
    private int currentOption;
    private bool waitOption;
    private bool flickLights;
    private bool checkForBack;
    private bool checkActions;
    void Start()
    {
        StartCoroutine(StartGoldenHeaven());
    }

    void SanityCheck()
    {
        if (Global.Inventory.HasItem[Global.Inventory.Ring])
        {
            goldRing.gameObject.SetActive(false);
        }
    }

    IEnumerator StartGoldenHeaven()
    {
        SanityCheck();
        cam.orthographicSize = 0.000000000000000001f;
        uicam.orthographicSize = 0.000000000000000001f;
        StartCoroutine(FlickLights());
        StartCoroutine(CheckForBack());
        StartCoroutine(dialogueSmoothIn.GoToPosition());
        StartCoroutine(ZoomOut());
        subtitleTextBox.Text = Global.Time;

        if (Global.Decisions.NeniaFirstFateShift)
        {
            neniaTransform.gameObject.SetActive(false);
        }

        mainTextBox.Text = "The light flickers." + (!Global.Decisions.NeniaFirstFateShift ? "You see the silhouette of someone you know." : "");
        yield return new WaitForSeconds(1f);
        yield return CheckActions();
    }

    IEnumerator CheckActions()
    {
        checkActions = true;
        while (checkActions)
        {
            if (shopkeeper.IsOverlapping && Input.GetMouseButtonDown(0))
            {
                busy = true;
                yield return TalkToShopkeeper();
                yield return ResetActions();
                busy = false;
            }

            if (nenia.IsOverlapping && Input.GetMouseButtonDown(0))
            {
                busy = true;
                yield return TalkToNenia();
                yield return ResetActions();
                busy = false;
            }

            if (goldring.IsOverlapping && Input.GetMouseButtonDown(0))
            {
                busy = true;
                yield return InspectGoldRing();
                yield return ResetActions();
                busy = false;
            }
            yield return 1;
        }
    }

    IEnumerator ResetActions()
    {
        mainTextBox.AutoBlinkDialogueArrow = false;
        mainTextBox.DialogueArrow.IsBlinking = false;
        subtitleTextBox.SetColor(mcDialogColor);
        subtitleTextBox.Text = Global.Time;
        mainTextBox.Text = "The light flickers. " + (!Global.Decisions.NeniaFirstFateShift ? "You see the silhouette of someone you know." : "");
        yield break;
    }

    #region Shopkeeper
    IEnumerator TalkToShopkeeper()
    {
        cameraBlur.enabled = true;
        StartCoroutine(shopKeeperFadeIn.GoToPosition());
        subtitleTextBox.SetColor(shopkeeperDialogColor);
        StartCoroutine(subtitleTextBox.SetText("SHOPKEEPER:"));
        yield return mainTextBox.SetText("Hello sir! How may I help you today? ");
        mainTextBox.DialogueArrow.IsBlinking = true;
        yield return WaitForNextClick();

        subtitleTextBox.SetColor(mcDialogColor);
        StartCoroutine(subtitleTextBox.SetText("CALIM:"));
        mainTextBox.enabled = false;

        option1.enabled = option2.enabled = option3.enabled = true;

        option1.Text = "I want to buy something";
        option2.Text = "I want to talk";
        option3.Text = "End conversation.";

        yield return WaitForOptionClick();

        switch (currentOption)
        {
            case 0:
                yield return ShopkeeperBuySomething();
                break;
            case 1:
                yield return ShopkeeperTalk();
                break;
            default:
                break;
        }

        mainTextBox.DialogueArrow.IsBlinking = false;
        StartCoroutine(shopKeeperFadeIn.ReturnToOriginal());
        cameraBlur.enabled = false;
    }

    IEnumerator ShopkeeperBuySomething()
    {
        mainTextBox.DialogueArrow.IsBlinking = false;
        subtitleTextBox.SetColor(shopkeeperDialogColor);
        StartCoroutine(subtitleTextBox.SetText("SHOPKEEPER:"));

        if (Global.Inventory.HasItem[Global.Inventory.Ring])
        {
            yield return mainTextBox.SetText("Oh I'm sorry. I don't have anything else to sell. ");
            mainTextBox.DialogueArrow.IsBlinking = true;
            yield return WaitForNextClick();
            yield break;
        }

        yield return mainTextBox.SetText("Are you interested in one of our most precious ");
        mainTextBox.SetColor(goldRingTextColor);
        yield return mainTextBox.AppendText("golden rings");
        mainTextBox.SetColor(Color.white);
        yield return mainTextBox.AppendText("? ");
        mainTextBox.DialogueArrow.IsBlinking = true;
        yield return WaitForNextClick();
        mainTextBox.DialogueArrow.IsBlinking = false;
        yield return mainTextBox.SetText("I can sell you one for $" + Global.GoldRingPrice.ToString("000.00") + " ");
        mainTextBox.DialogueArrow.IsBlinking = true;
        yield return WaitForNextClick();

        startBuyRing:
        subtitleTextBox.SetColor(mcDialogColor);
        StartCoroutine(subtitleTextBox.SetText("CALIM:"));

        mainTextBox.enabled = false;
        option1.enabled = option2.enabled = true;
        bool hasMoney = Global.Money >= Global.GoldRingPrice;
        if (hasMoney)
        {
            option1.SetColor(Color.yellow);
            option1.Text = "Yes, sure thing! - FATE SHIFT - ";
        }
        else
        {
            option1.SetColor(Color.red);
            option1.Text = "????? - You can't afford it -";
        }

        option2.Text = "No, thanks.";
        yield return WaitForOptionClick();
        option1.SetColor(Color.white);

        switch (currentOption)
        {
            case 0:
                if (!hasMoney)
                {
                    yield return ShopkeeperCantAffordRing();
                    goto startBuyRing;
                }
                else
                {
                    yield return ShopkeeperBuyRing();
                }
                break;
            default:
                yield return ShopkeeperDontBuyRing();
                break;
        }
    }

    IEnumerator ShopkeeperCantAffordRing()
    {
        subtitleTextBox.SetColor(shopkeeperDialogColor);
        StartCoroutine(subtitleTextBox.SetText("SHOPKEEPER:"));
        yield return mainTextBox.SetText("Oh, I'm sorry dear, but you can't afford this ring.");
        mainTextBox.DialogueArrow.IsBlinking = true;
        yield return WaitForNextClick();
    }

    IEnumerator ShopkeeperBuyRing()
    {
        subtitleTextBox.SetColor(shopkeeperDialogColor);
        StartCoroutine(subtitleTextBox.SetText("SHOPKEEPER:"));

        yield return GlimpseOfTheFuture();
        subtitleTextBox.SetColor(shopkeeperDialogColor);
        StartCoroutine(subtitleTextBox.SetText("SHOPKEEPER:"));

        if (Global.Decisions.YouBecameRich)
        {
            yield return mainTextBox.SetText("Thank you a lot, sir. You have such a fine taste. ");
        }
        else if (Global.Decisions.YouAreACreep)
        {
            yield return mainTextBox.SetText("Thank you. ");
        }
        else
        {
            yield return mainTextBox.SetText("Thanks for your patronage. I'm sure your partner is going to love it! ");
        }
        Global.Money -= Global.GoldRingPrice;
        Global.Inventory.HasItem[Global.Inventory.Ring] = true;
        goldRing.gameObject.SetActive(false);
        inventory.Draw();

        mainTextBox.DialogueArrow.IsBlinking = true;
        yield return WaitForNextClick();
    }

    IEnumerator ShopkeeperDontBuyRing()
    {
        subtitleTextBox.SetColor(shopkeeperDialogColor);
        StartCoroutine(subtitleTextBox.SetText("SHOPKEEPER:"));
        yield return mainTextBox.SetText("Anytime!");
        mainTextBox.DialogueArrow.IsBlinking = true;
        yield return WaitForNextClick();
    }

    IEnumerator ShopkeeperTalk()
    {
        if (Global.Inventory.HasItem[Global.Inventory.Ring])
        {
            subtitleTextBox.SetColor(shopkeeperDialogColor);
            StartCoroutine(subtitleTextBox.SetText("SHOPKEEPER:"));

            if (Global.Decisions.YouRememberNenia)
            {
                if (Global.Decisions.BlueCardWrittenAboutReality)
                {
                    yield return mainTextBox.SetText("Are you really giving this to the person you love most? ");
                }
                else
                {
                    yield return mainTextBox.SetText("When are you going to get married? This is so exciting! ");
                }
            }
            else if (Global.Decisions.NeniaIsYourSister)
            {
                if (Global.Decisions.BlueCardWrittenAboutReality)
                {
                    yield return mainTextBox.SetText("I don't know, something is off. Does your sister have a fiance?");
                }
                else
                {
                    yield return mainTextBox.SetText("Oh, I'd really like to be your sister right now! ");
                }
            }
            else if (Global.Decisions.NeniaIsYourFriend)
            {
                yield return mainTextBox.SetText("Someday, I want to have friends such as you. ");
            }
            else if (Global.Decisions.YouAreACreep)
            {
                if (Global.Decisions.BlueCardWrittenAboutReality)
                {
                    yield return mainTextBox.SetText("You are scaring me... ");
                }
                else
                {
                    yield return mainTextBox.SetText("Are you alright? ");
                }
            }
            else
            {
                yield return mainTextBox.SetText("How are you doing today? ");
            }

            yield return WaitForNextClick();
        }
        else
        {
            subtitleTextBox.SetColor(shopkeeperDialogColor);
            StartCoroutine(subtitleTextBox.SetText("SHOPKEEPER:"));
            if (Global.Decisions.YouAreACreep)
            {
                yield return mainTextBox.SetText("You sure have a weird look on you, mister. ");
            }
            else if (Global.Decisions.YouBecameRich)
            {
                yield return mainTextBox.SetText("I'm sure you'll love all we have to offer here. ");
            }
            else if (Global.Decisions.BlueCardWrittenAboutTime)
            {
                yield return mainTextBox.SetText("Tick. Tock. It's time to buy! But please, have money. ");
            }
            else
            {
                yield return mainTextBox.SetText("Are you enjoying the mall? I've heard you can earn a lot of money at the  ");
                mainTextBox.SetColor(jackspotColor);
                yield return mainTextBox.AppendText("Jackspot ");
                mainTextBox.SetColor(Color.white);
                yield return mainTextBox.AppendText(". ");
            }
            yield return WaitForNextClick();
        }
    }
    #endregion

    #region Nenia
    IEnumerator TalkToNenia()
    {
        cameraBlur.enabled = true;

        if (Global.Decisions.NeniaFirstFateShift)
        {
            if (Global.Decisions.YouRememberNenia)
            {
                yield return YourRememberNeniaStandardDialogue();
            }
            else if (Global.Decisions.NeniaIsYourSister)
            {
                mainTextBox.AutoBlinkDialogueArrow = true;
                subtitleTextBox.SetColor(neniaDialogColor);
                StartCoroutine(subtitleTextBox.SetText("NENIA:"));
                StartCoroutine(neniaFiance.GoToPosition());
                yield return mainTextBox.SetText("Are you hungry? I'm paying today, let's go to the food court. ");
                yield return WaitForNextClick();
                StartCoroutine(neniaFiance.ReturnToOriginal());
                mainTextBox.AutoBlinkDialogueArrow = false;
            }
            else if (Global.Decisions.NeniaIsYourFriend)
            {
                mainTextBox.AutoBlinkDialogueArrow = true;
                subtitleTextBox.SetColor(neniaDialogColor);
                StartCoroutine(subtitleTextBox.SetText("NENIA:"));
                StartCoroutine(neniaFiance.GoToPosition());
                yield return mainTextBox.SetText("Hey, let's grab a snack. ");
                yield return WaitForNextClick();
                StartCoroutine(neniaFiance.ReturnToOriginal());
                mainTextBox.AutoBlinkDialogueArrow = false;
            }
            else if (Global.Decisions.YouAreACreep)
            {
                subtitleTextBox.SetColor(neniaDialogColor);
                StartCoroutine(subtitleTextBox.SetText("NENIA:"));
                yield return mainTextBox.SetText("Stop looking at me like this! I'm going to call the police. ");
                yield return WaitForNextClick();
            }
            cameraBlur.enabled = false;
            yield break;
        }

        StartCoroutine(subtitleTextBox.SetText("CALIM:"));
        mainTextBox.AutoBlinkDialogueArrow = true;
        yield return mainTextBox.SetText("Nenia...! ");
        yield return WaitForNextClick();
        subtitleTextBox.SetColor(neniaDialogColor);
        StartCoroutine(subtitleTextBox.SetText("-LOOKING-"));
        StartCoroutine(neniaFacingBack.GoToPosition());
        yield return mainTextBox.SetText("She starts turning around with an awkward smile.");
        yield return WaitForNextClick();

        rememberNenia1:
        subtitleTextBox.SetColor(mcDialogColor);
        StartCoroutine(subtitleTextBox.SetText("-THINKING-"));
        mainTextBox.enabled = false;
        option1.enabled = option2.enabled = option3.enabled = true;

        option1.SetColor(Color.yellow);
        option1.Text = "You remember Nenia. - FATE SHIFT - ";
        option2.SetColor(Global.Decisions.BlueCardWrittenAboutFamily ? Color.yellow : Color.red);
        option2.Text = Global.Decisions.BlueCardWrittenAboutFamily ? "Hey, my lovely sister. - FATE SHIFT - " : "You remember something else - FATE SHIFT - ";
        option3.Text = "You have something else to think";

        yield return WaitForOptionClick();

        switch (currentOption)
        {
            case 0:
                yield return YouRememberNenia();
                break;
            case 1:
                if (Global.Decisions.BlueCardWrittenAboutFamily)
                {
                    yield return YouRememberSomethingElse();
                }
                else
                {
                    yield return IDontRememberThisYet();
                    goto rememberNenia1;
                }
                break;
            case 2:
                yield return TalkToNeniaMoreOptions();
                break;
            default:
                break;
        }
        mainTextBox.enabled = false;
        option1.SetColor(Color.white);
        option2.SetColor(Color.white);
        option3.SetColor(Color.white);
        yield return neniaFacingBack.ReturnToOriginal();
        mainTextBox.enabled = true;
        cameraBlur.enabled = false;
    }

    IEnumerator YouRememberSomethingElse()
    {
        mainTextBox.AutoBlinkDialogueArrow = true;
        StartCoroutine(neniaFiance.GoToPosition());
        StartCoroutine(neniaFacingBack.ReturnToOriginal());
        subtitleTextBox.SetColor(neniaDialogColor);
        StartCoroutine(subtitleTextBox.SetText("NENIA:"));
        yield return mainTextBox.SetText("Hey, little brother. I was just looking at the jewelry here. ");
        yield return WaitForNextClick();

        StartCoroutine(subtitleTextBox.SetText("CALIM:"));
        yield return mainTextBox.SetText("They are so expensive, aren't they? ");
        yield return WaitForNextClick();

        StartCoroutine(neniaFiance.ReturnToOriginal());

        Global.Decisions.NeniaFirstFateShift = true;
        Global.Decisions.NeniaIsYourSister = true;
    }

    IEnumerator ThingsAreALittleClouded()
    {
        mainTextBox.AutoBlinkDialogueArrow = true;
        StartCoroutine(neniaFiance.GoToPosition());
        StartCoroutine(neniaFacingBack.ReturnToOriginal());
        subtitleTextBox.SetColor(neniaDialogColor);
        StartCoroutine(subtitleTextBox.SetText("NENIA:"));
        yield return mainTextBox.SetText("My boyfriend promised this ring to me. Don't you think it's cute?");
        yield return WaitForNextClick();

        StartCoroutine(subtitleTextBox.SetText("CALIM:"));
        yield return mainTextBox.SetText("Oh, I didn't know you were still dating... Yeah, the ring looks okay. ");
        yield return WaitForNextClick();

        StartCoroutine(subtitleTextBox.SetText("NENIA:"));
        yield return mainTextBox.SetText("Heh, now you're just being silly. ");
        yield return WaitForNextClick();

        Global.Decisions.NeniaFirstFateShift = true;
        Global.Decisions.NeniaIsYourFriend = true;
    }

    IEnumerator YouAreACreep()
    {
        subtitleTextBox.SetColor(neniaDialogColor);
        StartCoroutine(subtitleTextBox.SetText("NENIA:"));
        yield return mainTextBox.SetText("Who are you?");
        yield return WaitForNextClick();

        subtitleTextBox.SetColor(mcDialogColor);
        StartCoroutine(subtitleTextBox.SetText("CALIM:"));
        yield return mainTextBox.SetText("Uh, oh. No one. Sorry.");
        yield return WaitForNextClick();

        subtitleTextBox.SetColor(neniaDialogColor);
        StartCoroutine(subtitleTextBox.SetText("NENIA:"));
        yield return mainTextBox.SetText("Please, don't stare at me like that.");
        yield return WaitForNextClick();

        Global.Decisions.NeniaFirstFateShift = true;
        Global.Decisions.YouAreACreep = true;

        yield break;
    }

    IEnumerator YouRememberNenia()
    {
        Global.Decisions.NeniaFirstFateShift = true;
        Global.Decisions.YouRememberNenia = true;
        StartCoroutine(subtitleTextBox.SetText("-THINKING-"));
        subtitleTextBox.SetColor(neniaDialogColor);
        yield return GlimpseOfTheFuture();
        StartCoroutine(neniaFiance.GoToPosition());
        StartCoroutine(neniaFacingBack.ReturnToOriginal());
        subtitleTextBox.SetColor(neniaDialogColor);
        StartCoroutine(subtitleTextBox.SetText("NENIA:"));
        yield return mainTextBox.SetText("Oh, just look how beautiful this is... ");
        yield return WaitForNextClick();
        mainTextBox.AutoBlinkDialogueArrow = false;
        mainTextBox.enabled = false;

        subtitleTextBox.SetColor(mcDialogColor);
        StartCoroutine(subtitleTextBox.SetText("CALIM:"));

        option1.enabled = option2.enabled = true;
        option1.SetColor(Color.white);
        option2.SetColor(Color.white);
        option1.Text = "It will look a lot better with our names on it. ";
        option2.Text = "I don't think we can afford it. ";
        yield return WaitForOptionClick();
        switch (currentOption)
        {
            case 0:
                yield return YouRememberNeniaFlirt();
                break;
            default:
                yield return YouRememberNeniaPoor();
                break;
        }
        yield return neniaFiance.ReturnToOriginal();
    }

    IEnumerator YouRememberNeniaFlirt()
    {
        Global.Decisions.YouRememberNenia_Confident = true;
        subtitleTextBox.SetColor(neniaDialogColor);
        StartCoroutine(subtitleTextBox.SetText("NENIA:"));
        yield return mainTextBox.SetText("Ohh, you're such a wooer. Either way, I'm not serious... This is way too expensive. ");
        yield return WaitForNextClick();
    }

    IEnumerator YouRememberNeniaPoor()
    {
        Global.Decisions.YouRememberNenia_Poor = true;
        subtitleTextBox.SetColor(neniaDialogColor);
        StartCoroutine(subtitleTextBox.SetText("NENIA:"));
        yield return mainTextBox.SetText("Don't be such a downer... But yeah, you're right. This is way out of our league. ");
        yield return WaitForNextClick();
    }

    IEnumerator YourRememberNeniaStandardDialogue()
    {
        mainTextBox.AutoBlinkDialogueArrow = true;
        subtitleTextBox.SetColor(neniaDialogColor);
        StartCoroutine(subtitleTextBox.SetText("NENIA:"));
        StartCoroutine(neniaFiance.GoToPosition());
        yield return mainTextBox.SetText(Global.Decisions.YouRememberNenia_Confident ?
            "Aw I'm starving... How about we grab something to eat? " :
            "Are you hungry? I'm paying today, let's go to the food court. "
            );
        yield return WaitForNextClick();
        StartCoroutine(neniaFiance.ReturnToOriginal());
        mainTextBox.AutoBlinkDialogueArrow = false;
    }

    IEnumerator TalkToNeniaMoreOptions()
    {
        rememberNenia:
        mainTextBox.enabled = false;
        yield return 1;
        option1.enabled = option2.enabled = option3.enabled = true;
        bool clouded = (Global.Decisions.BlueCardWrittenAboutFamily && Global.Decisions.BlueCardWrittenAboutLove);
        option1.SetColor(clouded ? Color.yellow : Color.red);
        option1.Text = clouded ? "Hey, are you okay? " : "Things are a little clouded - FATE SHIFT - ";
        bool unlockCreep = (Global.Decisions.BlueCardWrittenAboutReality && Global.Decisions.BlueCardWrittenAboutLove);
        option2.SetColor(unlockCreep ? Color.yellow : Color.red);
        option2.Text = unlockCreep ? "I am her stalker. -FATE SHIFT - " : "You ARE the most important to me. - FATE SHIFT - ";
        option3.Text = "Nevermind. My memories are still too weak. ";
        yield return WaitForOptionClick();

        switch (currentOption)
        {
            case 0:
                if (clouded)
                {
                    yield return ThingsAreALittleClouded();
                }
                else
                {
                    yield return IDontRememberThisYet();
                    goto rememberNenia;
                }
                break;
            case 1:
                if (unlockCreep)
                {
                    yield return YouAreACreep();
                }
                else
                {
                    yield return IDontRememberThisYet();
                    goto rememberNenia;
                }
                break;
            case 2:
                break;
        }
    }

    IEnumerator IDontRememberThisYet()
    {
        subtitleTextBox.SetColor(mcDialogColor);
        StartCoroutine(subtitleTextBox.SetText("-THINKING-"));
        yield return mainTextBox.SetText("Dammit... What was it? It doesn't seem that I'll remember this yet.");
        yield return WaitForNextClick();
    }

    #endregion

    #region GoldRing

    IEnumerator InspectGoldRing()
    {
        cameraBlur.enabled = true;
        subtitleTextBox.SetColor(objectDialogColor);
        StartCoroutine(subtitleTextBox.SetText("-LOOKING-"));
        mainTextBox.AutoBlinkDialogueArrow = true;
        

        if (Global.Decisions.BlueCardWrittenAboutReality)
        {
            yield return mainTextBox.SetText("You see a tainted, cursed gold ring. It stares into your soul. ");
        }
        else
        {
            yield return mainTextBox.SetText("You see a shining gold ring. It seems expensive. ");
        }
        yield return WaitForNextClick();

        if (Global.Decisions.BlueCardWrittenAboutCalim)
        {
            yield return mainTextBox.SetText("You can see your face reflected in it. You look uglier than you thought you were. ");
            yield return WaitForNextClick();
        }

        if (Global.Decisions.YouRememberNenia_Confident)
        {
            if (Global.Money >= Global.GoldRingPrice)
            {
                yield return mainTextBox.SetText("You look at the shopkeeper. You want to buy this ring. ");
            }
            else
            {
                yield return mainTextBox.SetText("You wonder how you could get money for this... ");
            }
        }
        else if (Global.Decisions.YouRememberNenia_Poor)
        {
            if (Global.Money >= Global.GoldRingPrice)
            {
                yield return mainTextBox.SetText("You desperately want to buy her the ring. ");
            }
            else
            {
                yield return mainTextBox.SetText("You don't think you'll be able to afford this. ");
            }
        }
        else
        {
            yield return mainTextBox.SetText("You have delusional thoughts of giving this to someone you love. ");
        }

        if (Global.Decisions.NeniaIsYourSister)
        {
            yield return mainTextBox.SetText("You feel that it's indeed too early to think about marriage. ");
        }

        if (Global.Decisions.NeniaIsYourFriend)
        {
            yield return mainTextBox.SetText("You feel that one day she'll love you back. ");
        }

        yield return WaitForNextClick();
        if (Global.Decisions.YouAreACreep)
        {
            if (Global.Money >= Global.GoldRingPrice)
            {
                mainTextBox.SetColor(Color.red);
                yield return mainTextBox.SetText("YOU KNOW IT'S ALL A LIE. YOU DON'T CARE. YOU WANT TO BUY HER THE RING ANYWAY. ");
                yield return WaitForNextClick();
                mainTextBox.SetColor(Color.white);
            }
            else
            {
                mainTextBox.SetColor(Color.red);
                yield return mainTextBox.SetText("YOU NEED MONEY. YOU HAVE NO REGRETS. ");
                yield return WaitForNextClick();
                mainTextBox.SetColor(Color.white);
            }
        }

        cameraBlur.enabled = false;
    }
    #endregion

    IEnumerator GlimpseOfTheFuture()
    {
        subtitleTextBox.SetColor(mcDialogColor);
        subtitleTextBox.Text = "-LOOKING- ";
        mainTextBox.AutoBlinkDialogueArrow = false;
        mainTextBox.DialogueArrow.IsBlinking = false;
        backgroundMusic.Stop();
        hiss.Play();
        mainTextBox.Text = "You see a faint glimpse of the future. ";
        StartCoroutine(BlinkGlimpse1());
        yield return new WaitForSeconds(hiss.clip.length);
        mainTextBox.AutoBlinkDialogueArrow = true;
        mainTextBox.DialogueArrow.IsBlinking = true;
        yield return WaitForNextClick();
        yield return mainTextBox.SetText("Is this what happened that day? ");
        yield return WaitForNextClick();
        StartCoroutine(BlinkGlimpse2());
        backgroundMusic.Play();
    }

    IEnumerator BlinkGlimpse1()
    {
        float op = 0.1f;
        float time = 0f;
        transition.enabled = true;
        transition.material.SetFloat("_Invert", 0f);
        transition.material.SetFloat("_Opacity", 0.1f);
        while (op < 0.5f)
        {
            op = Mathf.Lerp(0.1f, 0.5f, time / 3f);
            transition.material.SetFloat("_Opacity", op);
            time += Time.deltaTime;
            yield return 1;
        }
        transition.material.SetFloat("_Opacity", op);
    }

    IEnumerator BlinkGlimpse2()
    {
        float op = 0.5f;
        float time = 0f;
        transition.enabled = true;
        transition.material.SetFloat("_Invert", 0f);
        transition.material.SetFloat("_Opacity", 0.5f);
        while (op >= 0.1f)
        {
            op = Mathf.Lerp(0.5f, 0f, 5 * time * time);
            transition.material.SetFloat("_Opacity", op);
            time += Time.deltaTime;
            yield return 1;
        }
        transition.material.SetFloat("_Opacity", 0f);
        transition.enabled = false;
        transition.material.SetFloat("_Invert", 1f);
    }

    IEnumerator WaitForOptionClick()
    {
        waitOption = true;
        while (waitOption)
        {
            if (option1.enabled && option1.optionMouseOver.IsOverlapping && Input.GetMouseButtonDown(0))
            {
                currentOption = 0;
                break;
            }

            if (option2.enabled && option2.optionMouseOver.IsOverlapping && Input.GetMouseButtonDown(0))
            {
                currentOption = 1;
                break;
            }

            if (option3.enabled && option3.optionMouseOver.IsOverlapping && Input.GetMouseButtonDown(0))
            {
                currentOption = 2;
                break;
            }

            yield return 1;
        }
        waitOption = false;
        option1.enabled = option2.enabled = option3.enabled = false;
        mainTextBox.enabled = true;
        yield break;
    }

    IEnumerator WaitForNextClick()
    {
        while (!mainTextBox.IsFullyRendered || !(mainTextBoxCollider.IsOverlapping && Input.GetMouseButtonDown(0)))
        {
            yield return 1;
        }
        yield return 1;
    }

    IEnumerator CheckForBack()
    {
        checkForBack = true;
        while (checkForBack)
        {
            while (!backButton.IsOverlapping || !Input.GetMouseButtonDown(0) || busy)
            {
                yield return 1;
            }

            float time = 0f;
            while (time < 1f)
            {
                transition.enabled = true;
                transition.material.SetFloat("_Opacity", Mathf.SmoothStep(0, 1, time));
                time += Time.deltaTime;
                yield return 1;
            }

            yield return new WaitForSeconds(1f);

            backgroundMusic.Stop();
            SceneManager.LoadScene(backScene);
            break;
        }
    }

    IEnumerator FlickLights()
    {
        flickLights = true;
        while (flickLights)
        {
            for (int i = 0; i < 10; i++)
            {
                bool what = Random.value >= 0.75f;
                yield return new WaitForSeconds(Random.value * 0.15f);
                sceneLight.enabled = what;
                goldenHeaven1.enabled = what;
                goldenHeaven2.enabled = !what;
            }

            yield return new WaitForSeconds(Random.value * 2);
        }

    }

    IEnumerator ZoomOut()
    {
        float time = 0f;
        while (cam.orthographicSize < 360f)
        {
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, 360, time / 15);
            cam.transform.position = Vector3.Lerp(cam.transform.position, new Vector3(this.transform.position.x, this.transform.position.y, cam.transform.position.z), time / 15);

            uicam.orthographicSize = cam.orthographicSize;
            uicam.transform.position = cam.transform.position;
            time += Time.deltaTime;
            yield return 1;
        }
    }
}
