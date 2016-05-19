using UnityEngine;
using System.Collections;
using UnityStandardAssets.ImageEffects;
using UnityEngine.SceneManagement;

public class Food_Court : MonoBehaviour
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
    public Color neniaDialogColor;
    public Color mcDialogColor;
    public Color mcOtherDialogColor;
    public Color foodCourtColor;
    public Color cropBlankColor;
    public Color objectDialogColor;
    public ColliderMouseOver mainTextBoxCollider;
    public SmoothIntoPosition clerkSmoothIn;
    public SmoothIntoPosition neniaSmoothIn;

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
    public ColliderMouseOver chair;
    public ColliderMouseOver clock;
    public ColliderMouseOver nenia;
    public Transform neniaTransform;
    public AudioSource coinSound;

    private bool busy;
    private int currentOption;
    private bool checkForBack;
    private bool checkActions;
    private bool waitOption;

    void Start()
    {
        StartCoroutine(StartFoodCourt());
    }

    void SanityCheck()
    {
        if (!Global.Decisions.YouRememberNenia &&
                !Global.Decisions.NeniaIsYourSister &&
                !Global.Decisions.NeniaIsYourFriend)
        {
            GameObject.Destroy(neniaTransform.gameObject);
        }
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

    IEnumerator StartFoodCourt()
    {
        SanityCheck();
        cam.orthographicSize = 0.000000000000000001f;
        uicam.orthographicSize = 0.000000000000000001f;
        StartCoroutine(CheckForBack());
        StartCoroutine(dialogueSmoothIn.GoToPosition());
        StartCoroutine(ZoomOut());
        if (Global.Decisions.BlueCardWrittenAboutReality)
        {
            mainTextBox.Text = "Instead of food, you smell blood. Enough of reality for today... ";
        }
        else if (Global.Decisions.BlueCardWrittenAboutLove)
        {
            mainTextBox.Text = "The food smells unusually sweet today. It reeks of lust. ";
        }
        else
        {
            mainTextBox.Text = "Delicious aromas fill this place. Your mouth fills with water. ";
        }
        
        subtitleTextBox.Text = Global.Time;
        yield return new WaitForSeconds(1f);
        yield return CheckActions();
    }

    IEnumerator TalkToClerk()
    {
        cameraBlur.enabled = true;
        mainTextBox.DialogueArrow.IsBlinking = false;
        mainTextBox.AutoBlinkDialogueArrow = true;
        subtitleTextBox.SetColor(shopkeeperDialogColor);
        StartCoroutine(subtitleTextBox.SetText("CLERK:"));
        StartCoroutine(clerkSmoothIn.GoToPosition());

        if (Global.Decisions.YouBoughtFood)
        {
            yield return mainTextBox.SetText("Enjoy your meal! ");
            yield return WaitForNextClick();
        }
        else
        {
            yield return mainTextBox.SetText("Hey there! What is your order? ");
            yield return WaitForNextClick();

            subtitleTextBox.SetColor(mcDialogColor);
            StartCoroutine(subtitleTextBox.SetText("CALIM:"));

            mainTextBox.enabled = false;
            option1.enabled = option2.enabled = option3.enabled = true;
            option1.Text = "I'd like the Early Chicken, please. ";
            option2.Text = "I'd like the Late Steak, please. ";
            option3.Text = "Nevermind. ";

            yield return WaitForOptionClick();

            switch (currentOption)
            {
                case 0:
                    yield return BuyEarlyChicken();
                    break;
                case 1:
                    yield return BuyLateSteak();
                    break;
                case 2:
                    break;
            }
        }
        mainTextBox.Text = "";
        subtitleTextBox.Text = "";
        yield return clerkSmoothIn.ReturnToOriginal();
        cameraBlur.enabled = false;
    }

    IEnumerator BuyEarlyChicken()
    {
        subtitleTextBox.SetColor(shopkeeperDialogColor);
        StartCoroutine(subtitleTextBox.SetText("CLERK:"));
        yield return mainTextBox.SetText("It will be ");
        mainTextBox.SetColor(cropBlankColor);
        yield return mainTextBox.AppendText("$ " + Global.EarlyChickenPrice.ToString("0.00") + " ");
        mainTextBox.SetColor(Color.white);
        yield return mainTextBox.AppendText(". Is that ok? ");
        yield return WaitForNextClick();

        mainTextBox.enabled = false;
        option1.enabled = option2.enabled = true;

        bool hasMoney = Global.Money >= Global.EarlyChickenPrice;
        option1.SetColor(hasMoney ? Color.yellow : Color.red);
        option1.Text = "Yes. -FATE SHIFT- ";
        option2.Text = "No, nevermind. ";

        yield return WaitForOptionClick();
        option1.SetColor(Color.white);

        switch (currentOption)
        {
            case 0:
                yield return GlimpseOfTheFuture();
                Global.Money -= Global.EarlyChickenPrice;
                subtitleTextBox.SetColor(shopkeeperDialogColor);
                StartCoroutine(subtitleTextBox.SetText("CLERK:"));
                yield return mainTextBox.SetText("Enjoy your meal! ");
                yield return WaitForNextClick();
                
                Global.Decisions.YouBoughtFood = Global.Decisions.YouBoughtEarlyFood = true;
                Global.Inventory.HasItem[Global.Inventory.LateSteak] = true;
                inventory.Draw();
                break;
            default:
                break;
        }
    }

    IEnumerator BuyLateSteak()
    {
        subtitleTextBox.SetColor(shopkeeperDialogColor);
        StartCoroutine(subtitleTextBox.SetText("CLERK:"));
        yield return mainTextBox.SetText("It will be ");
        mainTextBox.SetColor(cropBlankColor);
        yield return mainTextBox.AppendText("$ " + Global.LateSteakPrice.ToString("0.00") + " ");
        mainTextBox.SetColor(Color.white);
        yield return mainTextBox.AppendText(". Is that ok? ");
        yield return WaitForNextClick();

        mainTextBox.enabled = false;
        option1.enabled = option2.enabled = true;

        bool hasMoney = Global.Money >= Global.LateSteakPrice;
        option1.SetColor(hasMoney ? Color.yellow : Color.red);
        option1.Text = "Yes. -FATE SHIFT- ";
        option2.Text = "No, nevermind. ";

        yield return WaitForOptionClick();
        option1.SetColor(Color.white);

        switch (currentOption)
        {
            case 0:
                yield return GlimpseOfTheFuture();
                Global.Money -= Global.LateSteakPrice;
                subtitleTextBox.SetColor(shopkeeperDialogColor);
                StartCoroutine(subtitleTextBox.SetText("CLERK:"));
                yield return mainTextBox.SetText("Enjoy your meal! ");
                yield return WaitForNextClick();                
                Global.Decisions.YouBoughtFood = Global.Decisions.YouBoughtLateFood = true;
                Global.Inventory.HasItem[Global.Inventory.LateSteak] = true;
                inventory.Draw();
                break;
            default:
                break;
        }
    }

    IEnumerator ClerkSmallTalk()
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
        yield return mainTextBox.SetText("You become more aware of your surroundings. ");
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

    IEnumerator InspectChair()
    {
        cameraBlur.enabled = true;
        subtitleTextBox.SetColor(objectDialogColor);
        StartCoroutine(subtitleTextBox.SetText("-LOOKING-"));
        mainTextBox.AutoBlinkDialogueArrow = true;
        yield return mainTextBox.SetText("You see a chair in the food court. It is empty. ");
        yield return WaitForNextClick();

        if (Global.Decisions.YouBoughtFood)
        {
            mainTextBox.enabled = false;
            option1.enabled = option2.enabled = true;

            if (Global.Decisions.YouRememberNenia ||
                Global.Decisions.NeniaIsYourSister ||
                Global.Decisions.NeniaIsYourFriend)
            {
                option1.Text = "Eat your food with Nenia. -FATE SHIFT- ";
            }
            else
            {
                option1.Text = "Eat your food alone. -FATE SHIFT- ";
            }
            option2.Text = "Do nothing. ";

            yield return WaitForOptionClick();

            switch (currentOption)
            {
                case 0:
                    yield return EatFood();
                    break;
                case 1:
                    break;
            }
        }

        mainTextBox.SetColor(Color.white);
        mainTextBox.AutoBlinkDialogueArrow = false;
        cameraBlur.enabled = false;
    }

    IEnumerator EatFood()
    {
        mainTextBox.AutoBlinkDialogueArrow = true;
        if (Global.Decisions.YouRememberNenia ||
                Global.Decisions.NeniaIsYourSister ||
                Global.Decisions.NeniaIsYourFriend)
        {
            neniaSmoothIn.GoToPosition();
            subtitleTextBox.SetColor(neniaDialogColor);
            StartCoroutine(subtitleTextBox.SetText("NENIA:"));

            yield return mainTextBox.SetText("Whoa, this tastes awesome. ");
            yield return WaitForNextClick();

            subtitleTextBox.SetColor(mcDialogColor);
            StartCoroutine(subtitleTextBox.SetText("CALIM:"));

            if (Global.Decisions.NeniaIsYourSister)
            {
                yield return mainTextBox.SetText("Sure it does! Thanks for everything Nenia... I'm glad I have a sister like you. ");
                yield return WaitForNextClick();
            }

            if (Global.Decisions.YouRememberNenia)
            {
                yield return mainTextBox.SetText("Yup! And eating by your side is the best part of it. ");
                yield return WaitForNextClick();

                subtitleTextBox.SetColor(neniaDialogColor);
                StartCoroutine(subtitleTextBox.SetText("NENIA:"));

                yield return mainTextBox.SetText("Oh, you... ");
                yield return WaitForNextClick();
            }
            if (Global.Decisions.NeniaIsYourFriend)
            {
                yield return mainTextBox.SetText("It's been a while since I've eaten here. They really got better. ");
                yield return WaitForNextClick();
            }
            neniaSmoothIn.ReturnToOriginal();
        }
        else
        {
            subtitleTextBox.SetColor(mcDialogColor);
            StartCoroutine(subtitleTextBox.SetText("CALIM:"));

            if (Global.Decisions.BlueCardWrittenAboutReality)
            {
                yield return mainTextBox.SetText("Hmm. This tastes like metal. Like... blood? ");
            }
            else if (Global.Decisions.BlueCardWrittenAboutLove)
            {
                yield return mainTextBox.SetText("Hmm. This tastes like tears? Weird. ");
            }
            else if (Global.Decisions.BlueCardWrittenAboutCalim)
            {
                yield return mainTextBox.SetText("Hmm. I feel like I want to throw up... ");
            }
            else
            {
                yield return mainTextBox.SetText("Hmm. This is tasty. ");
            }
            
            yield return WaitForNextClick();
        }

        backgroundMusic.Pause();
        subtitleTextBox.SetColor(objectDialogColor);
        if (Global.Decisions.BlueCardWrittenAboutTime)
        {
            yield return GlimpseOfTheFuture();
            StartCoroutine(subtitleTextBox.SetText("-LOOKING-"));
            yield return mainTextBox.SetText("You are full. You look at the clock, and suddenly feel a shiver down your spine. ");
            yield return WaitForNextClick();

            Global.Time = Global.Decisions.YouBoughtEarlyFood ? "10:34 AM" : "10:37 AM";

            yield return mainTextBox.SetText("It is now ");
            mainTextBox.SetColor(mcDialogColor);
            yield return mainTextBox.AppendText(Global.Time + ". ");
            yield return WaitForNextClick();
        }
        else
        {
            yield return GlimpseOfTheFuture();
            StartCoroutine(subtitleTextBox.SetText("-LOOKING-"));
            yield return mainTextBox.SetText("You are full. You look at the clock, maybe you're late for the train. Time stands still, though. ");
            yield return WaitForNextClick();
        }
        backgroundMusic.UnPause();
    }

    IEnumerator InspectClock()
    {
        cameraBlur.enabled = true;
        subtitleTextBox.SetColor(objectDialogColor);
        StartCoroutine(subtitleTextBox.SetText("-LOOKING-"));
        mainTextBox.AutoBlinkDialogueArrow = true;
        yield return mainTextBox.SetText("You see the clock. It shows " + Global.Time + ". " + (!Global.Decisions.BlueCardWrittenAboutTime ? "It doesn't seem to be moving. " : ""));
        yield return WaitForNextClick();

        if (Global.Decisions.BlueCardWrittenAboutTime)
        {
            yield return mainTextBox.SetText("It ticks ever so slightly. But it ticks, anyway.");
            yield return WaitForNextClick();
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
                yield return TalkToClerk();
                yield return ResetActions();
                busy = false;
            }

            if (clock.IsOverlapping && Input.GetMouseButtonDown(0))
            {
                busy = true;
                yield return InspectClock();
                yield return ResetActions();
                busy = false;
            }

            if (chair.IsOverlapping && Input.GetMouseButtonDown(0))
            {
                busy = true;
                yield return InspectChair();
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
