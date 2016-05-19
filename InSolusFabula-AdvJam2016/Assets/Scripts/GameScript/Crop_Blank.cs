using UnityEngine;
using System.Collections;
using UnityStandardAssets.ImageEffects;
using UnityEngine.SceneManagement;

public class Crop_Blank : MonoBehaviour
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
    public TextComponent option4;
    public TextComponent option5;
    public TextComponent option6;
    public TextComponent option7;
    public TextComponent option8;
    public TextComponent option9;
    public Color shopkeeperDialogColor;
    public Color mcDialogColor;
    public Color mcOtherDialogColor;
    public Color jackspotColor;
    public Color cropBlankColor;
    public Color objectDialogColor;
    public ColliderMouseOver mainTextBoxCollider;
    public SmoothIntoPosition managerSmoothIn;

    [Header("UI")]
    public ColliderMouseOver backButton;
    public Inventory inventory;
    public Transform canvas;

    [Header("Sounds")]
    public AudioSource backgroundMusic;
    public AudioSource hiss;

    [Header("BackButton")]
    public string backScene;
    public SpriteRenderer transition;

    [Header("Scene Objects")]
    public ColliderMouseOver manager;
    public ColliderMouseOver blueCard;
    public ColliderMouseOver pinnedNote;
    public AudioSource coinSound;

    private bool busy;
    private int currentOption;
    private bool checkForBack;
    private bool checkActions;
    private bool waitOption;

    void Start()
    {
        StartCoroutine(StartCropBlank());
    }

    void SanityCheck()
    {

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

    IEnumerator StartCropBlank()
    {
        SanityCheck();
        cam.orthographicSize = 0.000000000000000001f;
        uicam.orthographicSize = 0.000000000000000001f;
        StartCoroutine(CheckForBack());
        StartCoroutine(dialogueSmoothIn.GoToPosition());
        StartCoroutine(ZoomOut());
        mainTextBox.Text = "The air is filled up with the scent of riches and wealth. ";
        subtitleTextBox.Text = Global.Time;
        yield return new WaitForSeconds(1f);
        yield return CheckActions();
    }

    IEnumerator TalkToManager()
    {
        cameraBlur.enabled = true;
        mainTextBox.DialogueArrow.IsBlinking = false;
        mainTextBox.AutoBlinkDialogueArrow = true;
        subtitleTextBox.SetColor(shopkeeperDialogColor);
        StartCoroutine(subtitleTextBox.SetText("MANAGER:"));
        StartCoroutine(managerSmoothIn.GoToPosition());

        switch (Global.Decisions.TimesYouAskedForALoan)
        {
            case 0:
                if (!Global.Decisions.YouAskedForALoan)
                {
                    yield return mainTextBox.SetText("The Crop Blank is glad to greet you, gentleman. Is there any way I can be of use? ");
                }
                else
                {
                    yield return mainTextBox.SetText("Welcome, gentleman. Have you enjoyed our services? We are sure to satisfy your needs. ");
                }
                break;
            case 1:
                yield return mainTextBox.SetText("Well, if it's not our most valuable customer! How may I be of help? ");
                break;
            case 2:
                yield return mainTextBox.SetText("Oh, it's you again. Need anything? ");
                break;
            case 3:
                yield return mainTextBox.SetText("Are you here to pay your debts, by any chance? ");
                break;
        }

        mainTextBox.DialogueArrow.IsBlinking = true;
        yield return WaitForNextClick();
        mainTextBox.DialogueArrow.IsBlinking = false;

        options:

        subtitleTextBox.SetColor(mcDialogColor);
        StartCoroutine(subtitleTextBox.SetText("CALIM:"));
        mainTextBox.enabled = false;
        option1.enabled = option2.enabled = option3.enabled = true;

        option1.Text = "I want to borrow some money. ";
        option2.Text = "I want to pay my debts. ";
        option3.Text = "MORE Options.";

        yield return WaitForOptionClick();

        switch (currentOption)
        {
            case 0:
                yield return BorrowMoney();
                goto options;
            case 1:
                yield return PayDebts();
                goto options;
            default:
                yield return ManagerMoreOptions();
                if (currentOption != 2) goto options;
                break;
        }

        mainTextBox.Text = "";
        subtitleTextBox.Text = "";
        yield return managerSmoothIn.ReturnToOriginal();
        cameraBlur.enabled = false;
    }

    IEnumerator ManagerMoreOptions()
    {
        yield return 1;
        mainTextBox.enabled = false;
        option1.enabled = option2.enabled = option3.enabled = true;
        option1.Text = "I want to talk. ";
        option2.Text = "Are you the owner of jackspot? ";
        option3.Text = "END Conversation.";

        yield return WaitForOptionClick();

        switch (currentOption)
        {
            case 0:
                yield return ManagerSmallTalk();
                break;
            case 1:
                yield return ManagerAreYouOwner();
                break;
            default:
                break;
        }
    }

    IEnumerator PayDebts()
    {
        mainTextBox.AutoBlinkDialogueArrow = true;
        subtitleTextBox.SetColor(shopkeeperDialogColor);
        StartCoroutine(subtitleTextBox.SetText("MANAGER:"));
        if (Global.Decisions.TimesYouAskedForALoan == 0)
        {
            yield return mainTextBox.SetText("You don't owe us anything. Yet. ");
            yield return WaitForNextClick();
            if (Global.Decisions.BlueCardWrittenAboutCalim)
            {
                yield return mainTextBox.SetText("But you do owe someone else... ");
                yield return WaitForNextClick();
            }
            else if (Global.Decisions.BlueCardWrittenAboutTime)
            {
                yield return mainTextBox.SetText("But you do have a borrowed time, don't you know? ");
                yield return WaitForNextClick();
            }
        }
        else
        {
            decimal amountYouOwe = Global.Decisions.TimesYouAskedForALoan * Global.LoanValue * Global.LoanTaxes;
            if (Global.Money >= amountYouOwe)
            {
                subtitleTextBox.SetColor(mcDialogColor);
                StartCoroutine(subtitleTextBox.SetText("CALIM:"));

                mainTextBox.enabled = false;
                option1.enabled = option2.enabled = true;

                option2.Text = "Nevermind. ";
                yield return PayAllDebtsOption();

                yield return WaitForOptionClick();
                yield return 1;
                switch (currentOption)
                {
                    case 0:
                        Global.Money -= amountYouOwe;
                        Global.Decisions.TimesYouAskedForALoan = 0;

                        subtitleTextBox.SetColor(shopkeeperDialogColor);
                        StartCoroutine(subtitleTextBox.SetText("MANAGER:"));

                        yield return mainTextBox.SetText("Thanks a lot! Let's make business again, okay? ");
                        yield return WaitForNextClick();
                        break;
                    default:
                        break;
                }
            }
            else
            {
                mainTextBox.SetColor(Color.white);
                yield return mainTextBox.SetText("You don't have money to pay me back. You need ");
                mainTextBox.SetColor(cropBlankColor);
                yield return mainTextBox.AppendText("$ " + (Global.Decisions.TimesYouAskedForALoan * Global.LoanValue * Global.LoanTaxes).ToString("0.00") + " ");
                mainTextBox.SetColor(Color.white);
                yield return mainTextBox.AppendText(" with included taxes. Come back later. ");
                yield return WaitForNextClick();
            }
            yield return 1;
        }
    }

    IEnumerator PayAllDebtsOption()
    {
        yield return option1.SetText("Pay all ");
        option1.SetColor(cropBlankColor);
        string text = "$ " + (Global.Decisions.TimesYouAskedForALoan * Global.LoanValue * Global.LoanTaxes).ToString("0.00") + " ";
        yield return option1.AppendText(text);
        option1.SetColor(Color.white);
        yield return option1.AppendText(" that you owe them. Taxes included. ");        
    }

    IEnumerator BorrowMoney()
    {
        mainTextBox.AutoBlinkDialogueArrow = true;
        subtitleTextBox.SetColor(shopkeeperDialogColor);
        StartCoroutine(subtitleTextBox.SetText("MANAGER:"));
        if (Global.Decisions.TimesYouAskedForALoan == 3)
        {
            yield return mainTextBox.SetText("Sorry, you can't borrow any more. Pay your debts first. ");
            yield return WaitForNextClick();
            yield break;
        }

        yield return mainTextBox.SetText("You can borrow ");
        mainTextBox.SetColor(cropBlankColor);
        yield return mainTextBox.AppendText("$50 ");
        mainTextBox.SetColor(Color.white);
        yield return mainTextBox.AppendText("right now. Do you want it? ");
        yield return WaitForNextClick();
        mainTextBox.AutoBlinkDialogueArrow = false;

        subtitleTextBox.SetColor(mcDialogColor);
        StartCoroutine(subtitleTextBox.SetText("CALIM:"));

        mainTextBox.enabled = false;
        option1.enabled = option2.enabled = true;

        option1.Text = "Yes. ";
        option2.Text = "No, thanks. ";

        yield return WaitForOptionClick();
        yield return 1;

        switch (currentOption)
        {
            case 0:
                mainTextBox.AutoBlinkDialogueArrow = true;
                subtitleTextBox.SetColor(shopkeeperDialogColor);
                StartCoroutine(subtitleTextBox.SetText("MANAGER:"));
                coinSound.Play();
                Global.Money += 50M;
                Global.Decisions.TimesYouAskedForALoan++;
                yield return mainTextBox.SetText("Here. It was a pleasure to do business with you.");
                yield return WaitForNextClick();
                mainTextBox.AutoBlinkDialogueArrow = false;
                break;
            default:
                break;
        }
    }

    IEnumerator ManagerSmallTalk()
    {
        subtitleTextBox.SetColor(shopkeeperDialogColor);
        StartCoroutine(subtitleTextBox.SetText("MANAGER:"));

        if (Global.Decisions.BlueCardWrittenAboutTime)
        {
            yield return mainTextBox.SetText("Oh, it's almost time to go home. ");
        }

        if (Global.Decisions.BlueCardWrittenAboutReality)
        {
            yield return mainTextBox.SetText("This mall is quite empty, don't you think? Do you believe those people are real? ");
        }
        else
        {
            yield return mainTextBox.SetText("You're free to look around. Some things here are... I'd say... magical. ");
        }
        yield return WaitForNextClick();
    }

    IEnumerator ManagerAreYouOwner()
    {
        subtitleTextBox.SetColor(shopkeeperDialogColor);
        StartCoroutine(subtitleTextBox.SetText("MANAGER:"));

        if (Global.Decisions.BlueCardWrittenAboutReality)
        {
            yield return mainTextBox.SetText("No. Are you? ");
        }
        else
        {
            yield return mainTextBox.SetText("Yes! My name is Jack Leather. ");
        }
        yield return WaitForNextClick();
    }

    IEnumerator InspectBlueCard()
    {
        cameraBlur.enabled = true;
        subtitleTextBox.SetColor(objectDialogColor);
        mainTextBox.AutoBlinkDialogueArrow = true;
        StartCoroutine(subtitleTextBox.SetText("-LOOKING-"));
        yield return mainTextBox.SetText("You see a blue card laying on the counter. ");
        yield return WaitForNextClick();
        StartCoroutine(subtitleTextBox.SetText("-READING-"));
        yield return mainTextBox.SetText("You approach the note. It says: ");
        mainTextBox.SetColor(objectDialogColor);
        yield return mainTextBox.AppendText("'Crop Blank Co. Owned by Jack Leather.' ");
        mainTextBox.SetColor(Color.white);
        yield return WaitForNextClick();
        mainTextBox.AutoBlinkDialogueArrow = false;
        StartCoroutine(subtitleTextBox.SetText("-LOOKING-"));
        yield return mainTextBox.SetText("There is a blank space below. And the following text: ");
        yield return WaitForNextClick();
        mainTextBox.SetColor(objectDialogColor);
        yield return mainTextBox.SetText("'You can reveal up to 3 secrets about yourself. However, you need to pay ");
        mainTextBox.SetColor(cropBlankColor);
        yield return mainTextBox.AppendText("$ " + Global.BlueCardRevealPrice + " ");
        mainTextBox.SetColor(objectDialogColor);
        yield return mainTextBox.AppendText("for each revelation'");
        yield return WaitForNextClick();

        mainTextBox.enabled = false;
        option1.enabled = option2.enabled = option3.enabled = true;
        option4.enabled = option5.enabled = option6.enabled = true;

        bool hasMoney = Global.Money >= 100M;

        if (!Global.Decisions.BlueCardWrittenAboutTime)
        {
            if (Global.Decisions.BlueCardCount > 0)
            {
                option1.SetColor(hasMoney ? Color.yellow : Color.red);
                option1.Text = "WRITE Time.    -FATE REVEAL-";
            }
            else
            {
                option1.SetColor(Color.red);
                option1.Text = "WRITE Time.    -NOT ALLOWED-";
            }
        }
        else
        {
            option1.SetColor(Color.white);
            option1.Text = "WRITE Time.   -PAID -";
        }

        if (!Global.Decisions.BlueCardWrittenAboutReality)
        {
            if (Global.Decisions.BlueCardCount > 0)
            {
                option2.SetColor(hasMoney ? Color.yellow : Color.red);
                option2.Text = "WRITE Reality. -FATE REVEAL-";
            }
            else
            {
                option2.SetColor(Color.red);
                option2.Text = "WRITE Reality. -NOT ALLOWED-";
            }
        }
        else
        {
            option2.SetColor(Color.white);
            option2.Text = "WRITE Reality.   -PAID -";
        }

        if (!Global.Decisions.BlueCardWrittenAboutLove)
        {
            if (Global.Decisions.BlueCardCount > 0)
            {
                option3.SetColor(hasMoney ? Color.yellow : Color.red);
                option3.Text = "WRITE Love.    -FATE REVEAL-";
            }
            else
            {
                option3.SetColor(Color.red);
                option3.Text = "WRITE Love.    -NOT ALLOWED-";
            }
        }
        else
        {
            option3.SetColor(Color.white);
            option3.Text = "WRITE Love.   -PAID -";
        }

        if (!Global.Decisions.BlueCardWrittenAboutFamily)
        {
            if (Global.Decisions.BlueCardCount > 0)
            {
                option4.SetColor(hasMoney ? Color.yellow : Color.red);
                option4.Text = "WRITE Family. -FATE REVEAL-";
            }
            else
            {
                option4.SetColor(Color.red);
                option4.Text = "WRITE Family. -NOT ALLOWED-";
            }
        }
        else
        {
            option4.SetColor(Color.white);
            option4.Text = "WRITE Family.    -PAID - ";
        }

        if (!Global.Decisions.BlueCardWrittenAboutCalim)
        {
            if (Global.Decisions.BlueCardCount > 0)
            {
                option5.SetColor(hasMoney ? Color.yellow : Color.red);
                option5.Text = "WRITE Calim.  -FATE REVEAL-";
            }
            else
            {
                option5.SetColor(Color.red);
                option5.Text = "WRITE Calim.  -NOT ALLOWED-";
            }
        }
        else
        {
            option5.SetColor(Color.white);
            option5.Text = "WRITE Calim.   -PAID -";
        }

        option6.SetColor(Color.white);
        option6.Text = "DON'T WRITE anything. ";

        yield return WaitForOptionClick();
        switch (currentOption)
        {
            case 0:
                yield return BlueCardWriteTime();
                break;
            case 1:
                yield return BlueCardWriteReality();
                break;
            case 2:
                yield return BlueCardWriteLove();
                break;
            case 3:
                yield return BlueCardWriteFamily();
                break;
            case 4:
                yield return BlueCardWriteCalim();
                break;
            default:
                break;
        }
        mainTextBox.SetColor(Color.white);

        option1.SetColor(Color.white); option2.SetColor(Color.white); option3.SetColor(Color.white);
        option4.SetColor(Color.white); option5.SetColor(Color.white); option6.SetColor(Color.white);

        cameraBlur.enabled = false;
    }

    IEnumerator BlueCardWriteTime()
    {
        mainTextBox.SetColor(Color.white);
        if (Global.Decisions.BlueCardCount <= 0)
        {
            yield return mainTextBox.SetText("You try to write something, but the ink vanishes. ");
            yield return WaitForNextClick();
            yield break;
        }

        if (!Global.Decisions.BlueCardWrittenAboutTime && Global.Money <= Global.BlueCardRevealPrice)
        {
            yield return mainTextBox.SetText("You try to write something, but all that shows up are dollar signs. ");
            yield return WaitForNextClick();
            yield break;
        }

        Global.Money -= Global.Decisions.BlueCardWrittenAboutTime ? 0 : Global.BlueCardRevealPrice;


        if (!Global.Decisions.BlueCardWrittenAboutTime) yield return GlimpseOfTheFuture();
        Global.Decisions.BlueCardWrittenAboutTime = true;

        Global.Inventory.HasItem[Global.Inventory.Time] = true;
        inventory.Draw();

        backgroundMusic.Pause();
        mainTextBox.SetColor(objectDialogColor);
        yield return mainTextBox.SetText("You stand. You wonder. Why is it always " + Global.Time + " ?");
        yield return WaitForNextClick();
        yield return mainTextBox.SetText("There are only two things in this world that tumbles on the hands of time... ");
        yield return WaitForNextClick();
        yield return mainTextBox.SetText("One of them is human. Mundane. A means for survival. ");
        yield return WaitForNextClick();
        yield return mainTextBox.SetText("The other, of course, is love. Take your time, and choose wisely: ");
        yield return WaitForNextClick();
        yield return mainTextBox.SetText("Will you feast on the gears of the watch? Or will you tick your heart away? ");
        yield return WaitForNextClick();
        backgroundMusic.UnPause();
    }

    IEnumerator BlueCardWriteReality()
    {
        mainTextBox.SetColor(Color.white);
        if (Global.Decisions.BlueCardCount <= 0)
        {
            yield return mainTextBox.SetText("You try to write something, but the ink vanishes. ");
            yield return WaitForNextClick();
            yield break;
        }

        if (!Global.Decisions.BlueCardWrittenAboutReality && Global.Money <= Global.BlueCardRevealPrice)
        {
            yield return mainTextBox.SetText("You try to write something, but all that shows up are dollar signs. ");
            yield return WaitForNextClick();
            yield break;
        }

        Global.Money -= Global.Decisions.BlueCardWrittenAboutReality ? 0 : Global.BlueCardRevealPrice;

        if (!Global.Decisions.BlueCardWrittenAboutReality) yield return GlimpseOfTheFuture();
        Global.Decisions.BlueCardWrittenAboutReality = true;

        Global.Inventory.HasItem[Global.Inventory.Reality] = true;
        inventory.Draw();

        backgroundMusic.Pause();
        mainTextBox.SetColor(objectDialogColor);
        yield return mainTextBox.SetText("This world is lacking colors. It's lacking rhythm. ");
        yield return WaitForNextClick();
        yield return mainTextBox.SetText("Do you really think you can trust everything you hear? This is your mind, Calim. Try and remember. ");
        yield return WaitForNextClick();
        yield return mainTextBox.SetText("Are you afraid? Are you aware? Is your conscience really clean? Do you want to figure out the truth after all? ");
        yield return WaitForNextClick();
        yield return mainTextBox.SetText("If so, doubt. Place mistrust. Be skeptical about everything, even yourself. Hesitate.");
        yield return WaitForNextClick();
        yield return mainTextBox.SetText("Will you forever be tricked by your own illusions? Or will you find out the reason you suffer? ");
        yield return WaitForNextClick();
        backgroundMusic.UnPause();
    }

    IEnumerator BlueCardWriteLove()
    {
        mainTextBox.SetColor(Color.white);
        if (Global.Decisions.BlueCardCount <= 0)
        {
            yield return mainTextBox.SetText("You try to write something, but the ink vanishes. ");
            yield return WaitForNextClick();
            yield break;
        }

        if (!Global.Decisions.BlueCardWrittenAboutLove && Global.Money <= Global.BlueCardRevealPrice)
        {
            yield return mainTextBox.SetText("You try to write something, but all that shows up are dollar signs. ");
            yield return WaitForNextClick();
            yield break;
        }

        Global.Money -= Global.Decisions.BlueCardWrittenAboutLove ? 0 : Global.BlueCardRevealPrice;

        if (!Global.Decisions.BlueCardWrittenAboutLove) yield return GlimpseOfTheFuture();
        Global.Decisions.BlueCardWrittenAboutLove = true;

        Global.Inventory.HasItem[Global.Inventory.Love] = true;
        inventory.Draw();

        backgroundMusic.Pause();
        mainTextBox.SetColor(objectDialogColor);
        yield return mainTextBox.SetText("You feel your heart aching. You miss her so much. ");
        yield return WaitForNextClick();
        yield return mainTextBox.SetText("She is your everything. Your reason to live. And there's nothing that could take that away. ");
        yield return WaitForNextClick();
        yield return mainTextBox.SetText("You struggle, day by day. Are you close, is she near? What's wrong with her voice? ");
        yield return WaitForNextClick();
        yield return mainTextBox.SetText("Those whispers, they make you crazy. You look for her, everywhere. She's there, then she's not. Where is she? ");
        yield return WaitForNextClick();
        yield return mainTextBox.SetText("You hear those words, faintly: 'Will you love me forever?'. You know you are not dead. ");
        yield return WaitForNextClick();
        backgroundMusic.UnPause();
    }

    IEnumerator BlueCardWriteFamily()
    {
        mainTextBox.SetColor(Color.white);
        if (Global.Decisions.BlueCardCount <= 0)
        {
            yield return mainTextBox.SetText("You try to write something, but the ink vanishes. ");
            yield return WaitForNextClick();
            yield break;
        }

        if (!Global.Decisions.BlueCardWrittenAboutFamily && Global.Money <= Global.BlueCardRevealPrice)
        {
            yield return mainTextBox.SetText("You try to write something, but all that shows up are dollar signs. ");
            yield return WaitForNextClick();
            yield break;
        }

        Global.Money -= Global.Decisions.BlueCardWrittenAboutFamily ? 0 : Global.BlueCardRevealPrice;

        if (!Global.Decisions.BlueCardWrittenAboutFamily) yield return GlimpseOfTheFuture();
        Global.Decisions.BlueCardWrittenAboutFamily = true;

        Global.Inventory.HasItem[Global.Inventory.Family] = true;
        inventory.Draw();

        backgroundMusic.Pause();
        mainTextBox.SetColor(objectDialogColor);
        yield return mainTextBox.SetText("You know her. She is so close to you. ");
        yield return WaitForNextClick();
        yield return mainTextBox.SetText("Yet, you're confused. Is she your lover? Is she a relative? ");
        yield return WaitForNextClick();
        yield return mainTextBox.SetText("You see her silhouette through your wavering mind. Why do you care so much? ");
        yield return WaitForNextClick();
        yield return mainTextBox.SetText("She is closer than you think. Deeply attached. Deeply in love. Yet you cannot remember the day you met her. ");
        yield return WaitForNextClick();
        yield return mainTextBox.SetText("Will you accept her as your own? Or will you be forever in denial? ");
        yield return WaitForNextClick();
        backgroundMusic.UnPause();
    }

    IEnumerator BlueCardWriteCalim()
    {
        mainTextBox.SetColor(Color.white);
        if (Global.Decisions.BlueCardCount <= 0)
        {
            yield return mainTextBox.SetText("You try to write something, but the ink vanishes. ");
            yield return WaitForNextClick();
            yield break;
        }

        if (!Global.Decisions.BlueCardWrittenAboutCalim && Global.Money <= Global.BlueCardRevealPrice)
        {
            yield return mainTextBox.SetText("You try to write something, but all that shows up are dollar signs. ");
            yield return WaitForNextClick();
            yield break;
        }

        Global.Money -= Global.Decisions.BlueCardWrittenAboutCalim ? 0 : Global.BlueCardRevealPrice;

        if (!Global.Decisions.BlueCardWrittenAboutCalim) yield return GlimpseOfTheFuture();
        Global.Decisions.BlueCardWrittenAboutCalim = true;

        Global.Inventory.HasItem[Global.Inventory.Calim] = true;
        inventory.Draw();

        backgroundMusic.Pause();
        mainTextBox.SetColor(objectDialogColor);
        yield return mainTextBox.SetText("You look into the mirror. ");
        yield return WaitForNextClick();
        yield return mainTextBox.SetText("You see nothing. No shadow, no image. Just eternal darkness. Are you dead? ");
        yield return WaitForNextClick();
        yield return mainTextBox.SetText("What happened to your hopes and dreams? Have you fulfilled anything? ");
        yield return WaitForNextClick();
        yield return mainTextBox.SetText("You're afraid that you'll never be able to understand. That it will consume you. ");
        yield return WaitForNextClick();
        yield return mainTextBox.SetText("If this isn't the real world, where is it? Could someone be pulling your strings? Are you nothing but a puppet? ");
        yield return WaitForNextClick();
        yield return mainTextBox.SetText("Will you ever meet your master? ");
        yield return WaitForNextClick();
        backgroundMusic.UnPause();
    }

    IEnumerator GlimpseOfTheFuture()
    {
        Global.Decisions.BlueCardCount--;

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
        yield return mainTextBox.SetText("You become more acknowledged of your own reality. ");
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

    IEnumerator InspectPinnedNote()
    {
        cameraBlur.enabled = true;
        subtitleTextBox.SetColor(objectDialogColor);
        StartCoroutine(subtitleTextBox.SetText("-LOOKING-"));
        mainTextBox.AutoBlinkDialogueArrow = true;
        yield return mainTextBox.SetText("You see a peculiar note pinned on the wall. ");
        yield return WaitForNextClick();
        StartCoroutine(subtitleTextBox.SetText("-READING-"));
        yield return mainTextBox.SetText("You approach the note. It says: ");
        mainTextBox.SetColor(objectDialogColor);
        int phrases = 0;

        if (Global.Decisions.YouAreACreep)
        {
            phrases = Mathf.RoundToInt(Random.value * 5);
            switch (phrases)
            {
                case 0:
                    yield return mainTextBox.AppendText("'I SEE YOU KNOW. THIS IS WHO YOU ARE.' ");
                    yield return WaitForNextClick();
                    break;
                case 1:
                    yield return mainTextBox.AppendText("'THE WORLD IS YOURS TO TAKE.' ");
                    yield return WaitForNextClick();
                    break;
                case 2:
                    yield return mainTextBox.AppendText("'Remember: Timing matters.... AND YOU HAVE ALL THE TIME IN THE WORLD, DON'T YOU?' ");
                    yield return WaitForNextClick();
                    break;
                case 3:
                    yield return mainTextBox.AppendText("'Sometimes I'm mistaken for a fortune cookie. WHAT ARE YOU MISTAKEN FOR?' ");
                    yield return WaitForNextClick();
                    break;
                case 4:
                    yield return mainTextBox.AppendText("'Your mind is set, it seems. YOU DON'T REGRET, DO YOU?' ");
                    yield return WaitForNextClick();
                    break;
                case 5:
                    yield return mainTextBox.AppendText("'...' ");
                    yield return WaitForNextClick();
                    break;
            }
        }
        if (Global.Decisions.NeniaFirstFateShift)
        {
            phrases = Mathf.RoundToInt(Random.value * 5);
            switch (phrases)
            {
                case 0:
                    yield return mainTextBox.AppendText("'Your mind, it seems, is set. Will you regret your path?' ");
                    yield return WaitForNextClick();
                    break;
                case 1:
                    yield return mainTextBox.AppendText("'You think you know it all, huh?' ");
                    yield return WaitForNextClick();
                    break;
                case 2:
                    yield return mainTextBox.AppendText("'Remember: Timing matters.' ");
                    yield return WaitForNextClick();
                    break;
                case 3:
                    yield return mainTextBox.AppendText("'Sometimes I'm mistaken for a fortune cookie. Don't be deceived, I've got no cookie.' ");
                    yield return WaitForNextClick();
                    break;
                case 4:
                    yield return mainTextBox.AppendText("'Your mind is set, it seems. Do you regret?' ");
                    yield return WaitForNextClick();
                    break;
                case 5:
                    yield return mainTextBox.AppendText("'Set, as it seems, your mind is. Regret your path, will you?' ");
                    yield return WaitForNextClick();
                    break;
            }
        }
        else if (Global.Decisions.TimesYouAskedForALoan >= 3)
        {
            phrases = Mathf.RoundToInt(Random.value * 6 + (Global.Money < 50M ? 3 : 0));
            switch (phrases)
            {
                case 0:
                    yield return mainTextBox.AppendText("'It is indeed not just the bank you owe.' ");
                    yield return WaitForNextClick();
                    break;
                case 1:
                    yield return mainTextBox.AppendText("'Borrowed coins, borrowed money, borrowed love. Same old, same old.' ");
                    yield return WaitForNextClick();
                    break;
                case 2:
                    yield return mainTextBox.AppendText("'A ********* always pays his debts.' ");
                    yield return WaitForNextClick();
                    break;
                case 3:
                    yield return mainTextBox.AppendText("'Sometimes I'm mistaken for a fortune cookie. Don't be deceived, I've got no cookie.' ");
                    yield return WaitForNextClick();
                    break;
                case 4:
                    yield return mainTextBox.AppendText("'It's not like this message will change every time you read it.' ");
                    yield return WaitForNextClick();
                    break;
                case 5:
                    yield return mainTextBox.AppendText("'Borrowed coins, borrowed money, borrowed love. Same old, same old.' ");
                    yield return WaitForNextClick();
                    break;
                case 6:
                    yield return mainTextBox.AppendText("'Remember: Timing matters.' ");
                    yield return WaitForNextClick();
                    break;
                case 7:
                    yield return mainTextBox.AppendText("'Ohhh you're broke. And broken...' ");
                    yield return WaitForNextClick();
                    break;
                case 8:
                    yield return mainTextBox.AppendText("'Dude, how are you going to get rid of your debts?' ");
                    yield return WaitForNextClick();
                    break;
                case 9:
                    yield return mainTextBox.AppendText("'Are we done here?' ");
                    yield return WaitForNextClick();
                    break;
            }
        }
        else
        {
            phrases = Mathf.RoundToInt(Random.value * 10);
            switch (phrases)
            {
                case 0:
                    yield return mainTextBox.AppendText("'Not all that exists in this world is, in fact, real. Look for the signs.' ");
                    yield return WaitForNextClick();
                    break;
                case 1:
                    yield return mainTextBox.AppendText("'I was once told that each person carries a world within. How is yours like?' ");
                    yield return WaitForNextClick();
                    break;
                case 2:
                    yield return mainTextBox.AppendText("'Truths... Lies... does it matter? Who is the real judge? And who is the executioner?' ");
                    yield return WaitForNextClick();
                    break;
                case 3:
                    yield return mainTextBox.AppendText("'Sometimes I'm mistaken for a fortune cookie. Don't be deceived, I've got no cookie.' ");
                    yield return WaitForNextClick();
                    break;
                case 4:
                    yield return mainTextBox.AppendText("'It's not like this message will change every time you read it.' ");
                    yield return WaitForNextClick();
                    break;
                case 5:
                    yield return mainTextBox.AppendText("'Sometimes I'm mistaken for a fortune cookie. Don't be deceived, I've got no cookie.' ");
                    yield return WaitForNextClick();
                    break;
                case 6:
                    yield return mainTextBox.AppendText("'Remember: Timing matters.' ");
                    yield return WaitForNextClick();
                    break;
                case 7:
                    yield return mainTextBox.AppendText("'Tick-tock, watch the clock.' ");
                    yield return WaitForNextClick();
                    break;
                case 8:
                    yield return mainTextBox.AppendText("'Tick-tock, watch the clock.' ");
                    yield return WaitForNextClick();
                    break;
                case 9:
                    yield return mainTextBox.AppendText("'Abandon all hope ye who forgets.' ");
                    yield return WaitForNextClick();
                    break;
                case 10:
                    yield return mainTextBox.AppendText("'Just close your eyes, and quit the game. This isn't worthy.' ");
                    yield return WaitForNextClick();
                    break;
            }
        }

        mainTextBox.SetColor(Color.white);
        mainTextBox.AutoBlinkDialogueArrow = false;
        cameraBlur.enabled = false;
    }

    IEnumerator ResetActions()
    {
        mainTextBox.AutoBlinkDialogueArrow = false;
        mainTextBox.DialogueArrow.IsBlinking = false;
        subtitleTextBox.SetColor(mcDialogColor);
        subtitleTextBox.Text = Global.Time;
        mainTextBox.Text = "The air is filled up with the scent of riches and wealth. ";
        yield break;
    }

    IEnumerator CheckActions()
    {
        checkActions = true;
        while (checkActions)
        {
            if (manager.IsOverlapping && Input.GetMouseButtonDown(0))
            {
                busy = true;
                yield return TalkToManager();
                yield return ResetActions();
                busy = false;
            }

            if (pinnedNote.IsOverlapping && Input.GetMouseButtonDown(0))
            {
                busy = true;
                yield return InspectPinnedNote();
                yield return ResetActions();
                busy = false;
            }

            if (blueCard.IsOverlapping && Input.GetMouseButtonDown(0))
            {
                busy = true;
                yield return InspectBlueCard();
                yield return ResetActions();
                busy = false;
            }

            yield return 1;
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

            if (option4.enabled && option4.optionMouseOver.IsOverlapping && Input.GetMouseButtonDown(0))
            {
                currentOption = 3;
                break;
            }

            if (option5.enabled && option5.optionMouseOver.IsOverlapping && Input.GetMouseButtonDown(0))
            {
                currentOption = 4;
                break;
            }

            if (option6.enabled && option6.optionMouseOver.IsOverlapping && Input.GetMouseButtonDown(0))
            {
                currentOption = 5;
                break;
            }

            if (option7.enabled && option7.optionMouseOver.IsOverlapping && Input.GetMouseButtonDown(0))
            {
                currentOption = 6;
                break;
            }

            if (option8.enabled && option8.optionMouseOver.IsOverlapping && Input.GetMouseButtonDown(0))
            {
                currentOption = 7;
                break;
            }

            if (option9.enabled && option9.optionMouseOver.IsOverlapping && Input.GetMouseButtonDown(0))
            {
                currentOption = 8;
                break;
            }

            yield return 1;
        }
        waitOption = false;
        option1.enabled = option2.enabled = option3.enabled = false;
        option4.enabled = option5.enabled = option6.enabled = false;
        option7.enabled = option8.enabled = option9.enabled = false;
        mainTextBox.enabled = true;
        yield return 1;
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

}
