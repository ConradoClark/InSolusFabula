using UnityEngine;
using System.Collections;
using UnityStandardAssets.ImageEffects;
using UnityEngine.SceneManagement;

public class Jackspot : MonoBehaviour
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
    public Color jackspotColor;
    public Color cropBlankColor;
    public Color slotsColor;
    public Color objectDialogColor;
    public ColliderMouseOver mainTextBoxCollider;
    public SmoothIntoPosition managerSmoothIn;
    public SmoothIntoPosition arcadeSmoothIn;

    [Header("UI")]
    public ColliderMouseOver backButton;
    public Inventory inventory;
    public Transform canvas;

    [Header("Sounds")]
    public AudioSource backgroundMusic;
    public AudioSource deltaRunnerMusic;
    public AudioSource hiss;

    [Header("BackButton")]
    public string backScene;
    public SpriteRenderer transition;

    [Header("Scene Objects")]
    public ColliderMouseOver manager;
    public ColliderMouseOver deltaRunner;
    public ColliderMouseOver luckySlots;

    [Header("Delta Runner")]
    private Transform deltaRunnerCanvas;
    public GameObject deltaRunnerPrefab;
    //public SpriteRenderer deltaRunnerCharacter;
    public GameObject deltaRunnerGroundObstacle;
    public GameObject deltaRunnerAirObstacle;
    public GameObject deltaRunnerCoin;
    //public TextComponent deltaRunnerCoinCounter;
    public AudioSource deltaRunnerCoinSound;
    // public SpriteRenderer deltaRunnerGround;
    public Material deltaRunnerObstacleMaterial;
    //public SpriteRenderer deltaRunnerPause;
    public LayerMask deltaRunnerLayerMask;

    private bool busy;
    private bool waitOption;
    private int currentOption;
    private bool checkForBack;
    private bool checkActions;

    void Start()
    {
        StartCoroutine(StartJackspot());
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

    IEnumerator StartJackspot()
    {
        SanityCheck();
        cam.orthographicSize = 0.000000000000000001f;
        uicam.orthographicSize = 0.000000000000000001f;
        StartCoroutine(CheckForBack());
        StartCoroutine(dialogueSmoothIn.GoToPosition());
        StartCoroutine(ZoomOut());
        if (Global.Decisions.BlueCardWrittenAboutReality)
        {
            mainTextBox.Text = "You see all kinds of machines. You can see right through them. ";
        }
        else
        {
            mainTextBox.Text = "You see all kinds of machines. You wonder if you can win something. ";
        }
        subtitleTextBox.Text = Global.Time;
        yield return new WaitForSeconds(1f);
        yield return CheckActions();
    }

    IEnumerator ResetActions()
    {
        mainTextBox.AutoBlinkDialogueArrow = false;
        mainTextBox.DialogueArrow.IsBlinking = false;
        subtitleTextBox.SetColor(mcDialogColor);
        subtitleTextBox.Text = Global.Time;
        if (Global.Decisions.BlueCardWrittenAboutReality)
        {
            mainTextBox.Text = "You see all kinds of machines. You can see right through them. ";
        }
        else
        {
            mainTextBox.Text = "You see all kinds of machines. You wonder if you can win something. ";
        }
        yield break;
    }

    IEnumerator TalkToManager()
    {
        cameraBlur.enabled = true;
        mainTextBox.DialogueArrow.IsBlinking = false;
        subtitleTextBox.SetColor(shopkeeperDialogColor);
        StartCoroutine(subtitleTextBox.SetText("MANAGER:"));
        StartCoroutine(managerSmoothIn.GoToPosition());

        if (Global.Decisions.BlueCardWrittenAboutReality)
        {
            yield return mainTextBox.SetText("Welcome to Jackspot! You can use all our machines for a... overpriced fee! I hope you lose! ");
        }
        else
        {
            yield return mainTextBox.SetText("Welcome to Jackspot! You can use all our machines for a... modest fee!  ");
        }

        mainTextBox.DialogueArrow.IsBlinking = true;
        yield return WaitForNextClick();
        mainTextBox.DialogueArrow.IsBlinking = false;

        needAnything:

        yield return mainTextBox.SetText("Do you need anything, " + (Global.Decisions.BlueCardWrittenAboutReality ? "loser" : "sir") + "? ");
        mainTextBox.DialogueArrow.IsBlinking = true;
        yield return WaitForNextClick();

        subtitleTextBox.SetColor(mcDialogColor);
        StartCoroutine(subtitleTextBox.SetText("CALIM:"));
        mainTextBox.enabled = false;
        option1.enabled = option2.enabled = option3.enabled = true;

        option1.Text = "Tell me about the odds. ";
        option2.Text = "What if I lose all my money? ";
        option3.Text = "END Conversation.";

        yield return WaitForOptionClick();

        switch (currentOption)
        {
            case 0:
                yield return AboutTheOdds();
                goto needAnything;
            case 1:
                yield return IfILoseAllMoney();
                goto needAnything;
            default:
                break;
        }

        mainTextBox.Text = "";
        subtitleTextBox.Text = "";
        yield return managerSmoothIn.ReturnToOriginal();
        cameraBlur.enabled = false;
    }

    IEnumerator AboutTheOdds()
    {
        subtitleTextBox.SetColor(shopkeeperDialogColor);
        StartCoroutine(subtitleTextBox.SetText("MANAGER:"));
        StartCoroutine(managerSmoothIn.GoToPosition());

        yield return mainTextBox.SetText("Well, the ");
        mainTextBox.SetColor(jackspotColor);
        yield return mainTextBox.AppendText("Delta Runner ");
        mainTextBox.SetColor(Color.white);
        yield return mainTextBox.AppendText("is the \"supposedly\" easier one, and yields money based on how far you can go. ");
        yield return WaitForNextClick();

        yield return mainTextBox.SetText("Now now, the ");
        mainTextBox.SetColor(slotsColor);
        yield return mainTextBox.AppendText("Lucky Slots ");
        mainTextBox.SetColor(Color.white);
        yield return mainTextBox.AppendText("are absolutely amazing. Be wary though, the game is extremely unfair. ");
        yield return WaitForNextClick();

        yield return mainTextBox.SetText("It can drain your money in a matter of seconds. But who knows? You might end up rich. ");
        yield return WaitForNextClick();
    }

    IEnumerator IfILoseAllMoney()
    {
        subtitleTextBox.SetColor(shopkeeperDialogColor);
        StartCoroutine(subtitleTextBox.SetText("MANAGER:"));
        StartCoroutine(managerSmoothIn.GoToPosition());

        yield return mainTextBox.SetText("You can get a loan from ");
        mainTextBox.SetColor(cropBlankColor);
        yield return mainTextBox.AppendText("Crop Blank ");
        mainTextBox.SetColor(Color.white);
        yield return mainTextBox.AppendText("if that happens.");
        yield return WaitForNextClick();

        if (!Global.Decisions.BlueCardWrittenAboutReality)
        {
            yield return mainTextBox.SetText("As our partners, they are very reliable. ");
            yield return WaitForNextClick();
            yield return mainTextBox.SetText("But be careful... You can only loan so much money. ");
            yield return WaitForNextClick();
        }
        else
        {
            yield return mainTextBox.SetText("We'll make you bankrupt and dammed in not time! ");
            yield return WaitForNextClick();
        }

        yield return mainTextBox.SetText("And I don't want any more gambling junkies around here.");
        yield return WaitForNextClick();

        if (Global.Decisions.BlueCardWrittenAboutCalim)
        {
            yield return mainTextBox.SetText("But you seem like a good lad. Just don't overdo it. ");
            yield return WaitForNextClick();
        }
    }

    IEnumerator PlayDeltaRunner()
    {
        cameraBlur.enabled = true;
        StartCoroutine(arcadeSmoothIn.GoToPosition());

        subtitleTextBox.SetColor(objectDialogColor);
        StartCoroutine(subtitleTextBox.SetText("ARCADE:"));
        yield return mainTextBox.SetText("HEEEEEEY cowboy! Do you want to play ");
        mainTextBox.SetColor(jackspotColor);
        yield return mainTextBox.AppendText("Delta Runner ");
        mainTextBox.SetColor(Color.white);
        yield return mainTextBox.AppendText("? ");

        heycowboy:
        mainTextBox.DialogueArrow.IsBlinking = false;
        subtitleTextBox.SetColor(objectDialogColor);
        StartCoroutine(subtitleTextBox.SetText("ARCADE:"));
        mainTextBox.DialogueArrow.IsBlinking = true;
        yield return WaitForNextClick();

        subtitleTextBox.SetColor(mcDialogColor);
        StartCoroutine(subtitleTextBox.SetText("CALIM:"));
        mainTextBox.enabled = false;
        option1.enabled = option2.enabled = option3.enabled = true;

        option1.Text = "1. PLAY game. ";
        option2.Text = "2. CHECK rules. ";
        option3.Text = "3. EXIT arcade. ";

        yield return WaitForOptionClick();

        switch (currentOption)
        {
            case 0:
                yield return PlayDeltaRunnerNow();
                break;
            case 1:
                yield return CheckDeltaRunnerRules();
                subtitleTextBox.SetColor(objectDialogColor);
                StartCoroutine(subtitleTextBox.SetText("ARCADE:"));
                yield return mainTextBox.SetText("So, do you want to play ");
                mainTextBox.SetColor(jackspotColor);
                yield return mainTextBox.AppendText("Delta Runner ");
                mainTextBox.SetColor(Color.white);
                yield return mainTextBox.AppendText("? ");
                goto heycowboy;
            default:
                break;
        }
        mainTextBox.Text = "";
        subtitleTextBox.Text = "";
        yield return arcadeSmoothIn.ReturnToOriginal();
        cameraBlur.enabled = false;
    }

    IEnumerator CheckDeltaRunnerRules()
    {
        subtitleTextBox.SetColor(objectDialogColor);
        mainTextBox.AutoBlinkDialogueArrow = true;
        StartCoroutine(subtitleTextBox.SetText("ARCADE:"));
        yield return mainTextBox.SetText("It's veeeeeery EASY! Just keep moving and pay attention to the ");
        mainTextBox.SetColor(jackspotColor);
        yield return mainTextBox.AppendText("OBSTACLES. ");
        yield return WaitForNextClick();
        mainTextBox.SetColor(Color.white);
        yield return mainTextBox.SetText("If you see one coming, click with the right timing to ");
        mainTextBox.SetColor(jackspotColor);
        yield return mainTextBox.AppendText("jump ");
        mainTextBox.SetColor(Color.white);
        yield return mainTextBox.AppendText("over. ");
        yield return WaitForNextClick();
        yield return mainTextBox.SetText("You can also hold the mouse button to stay in the air. ");
        yield return WaitForNextClick();
        yield return mainTextBox.SetText("You earn ");
        mainTextBox.SetColor(jackspotColor);
        yield return mainTextBox.AppendText("coins ");
        mainTextBox.SetColor(Color.white);
        yield return mainTextBox.AppendText("by collecting them around. ");
        yield return WaitForNextClick();
        yield return mainTextBox.SetText("Each 100m you're allowed to ");
        mainTextBox.SetColor(jackspotColor);
        yield return mainTextBox.AppendText("double your coins ");
        mainTextBox.SetColor(Color.white);
        yield return mainTextBox.AppendText("or stop playing. ");
        yield return WaitForNextClick();
        yield return mainTextBox.SetText("But if you choose to ");
        mainTextBox.SetColor(jackspotColor);
        yield return mainTextBox.AppendText("double your coins ");
        mainTextBox.SetColor(Color.white);
        yield return mainTextBox.AppendText(", you also have to ");
        mainTextBox.SetColor(Color.red);
        yield return mainTextBox.AppendText("double your fee! ");
        mainTextBox.SetColor(Color.white);
        yield return WaitForNextClick();
        mainTextBox.AutoBlinkDialogueArrow = false;
        yield break;
    }

    IEnumerator PlayDeltaRunnerNow()
    {
        mainTextBox.DialogueArrow.IsBlinking = false;
        subtitleTextBox.SetColor(objectDialogColor);
        StartCoroutine(subtitleTextBox.SetText("ARCADE:"));
        mainTextBox.DialogueArrow.IsBlinking = true;
        yield return mainTextBox.SetText("You have to pay ");
        mainTextBox.SetColor(cropBlankColor);
        yield return mainTextBox.AppendText("$" + Global.DeltaRunnerFee + " ");
        mainTextBox.SetColor(Color.white);
        yield return mainTextBox.AppendText("up front. Is that ok? ");
        yield return WaitForNextClick();

        subtitleTextBox.SetColor(mcDialogColor);
        StartCoroutine(subtitleTextBox.SetText("CALIM:"));
        mainTextBox.enabled = false;
        option1.enabled = option2.enabled = true;

        StartCoroutine(SetFeeText());
        option2.Text = "2. EXIT arcade. ";

        yield return WaitForOptionClick();

        switch (currentOption)
        {
            case 0:
                yield return StartDeltaRunner();
                break;
            default:
                break;
        }
        yield break;
    }

    IEnumerator SetFeeText()
    {
        yield return option1.SetText("1. PAY ");
        option1.SetColor(cropBlankColor);
        yield return option1.AppendText("$" + Global.DeltaRunnerFee + " ");
        option1.SetColor(Color.white);
        yield return option1.AppendText("and play. ");
    }

    IEnumerator SetExtraFeeText(int fee)
    {
        yield return option1.SetText("1. PAY ");
        option1.SetColor(cropBlankColor);
        yield return option1.AppendText("$" + fee + " ");
        option1.SetColor(Color.white);
        yield return option1.AppendText("more to double your coins and keep playing. ");
    }

    private bool deltaRunnerRunning;
    private bool deltaRunnerDied;
    private float deltaRunnerCoins;

    IEnumerator StartDeltaRunner()
    {
        deltaRunnerCoins = 0;
        deltaRunnerDied = false;
        Global.DeltaRunnerPaused = false;
        deltaRunnerMusic.pitch = 1;
        Global.Money -= Global.DeltaRunnerFee;
        backgroundMusic.Pause();
        deltaRunnerMusic.Play();

        var deltaRunnerInstance = GameObject.Instantiate(deltaRunnerPrefab);
        deltaRunnerInstance.transform.SetParent(canvas, false);
        deltaRunnerCanvas = deltaRunnerInstance.transform;
        deltaRunnerCanvas.gameObject.SetActive(true);
        yield return 1;

        var deltaRunnerGameCanvas = deltaRunnerCanvas.Find("GameCanvas");
        var deltaRunnerGround = deltaRunnerGameCanvas.Find("ground").GetComponent<SpriteRenderer>();
        var deltaRunnerCharacter = deltaRunnerGameCanvas.Find("character").GetComponent<SpriteRenderer>();
        var deltaRunnerCoinCounter = deltaRunnerGameCanvas.Find("coins").GetComponent<TextComponent>();
        var deltaRunnerPause = deltaRunnerCanvas.Find("deltaRunnerPause").GetComponent<SpriteRenderer>();

        subtitleTextBox.SetColor(objectDialogColor);
        StartCoroutine(subtitleTextBox.SetText("ARCADE:"));
        mainTextBox.Text = "Running Game...";
        mainTextBox.DialogueArrow.IsBlinking = false;

        deltaRunnerRunning = true;

        int level = 0;
        float i = 0;
        float j = 0;
        int meters = 0;
        int achievedMeters = 100;
        StartCoroutine(HandlePlayer(deltaRunnerCharacter, deltaRunnerPause, deltaRunnerCoinCounter));
        Global.DeltaRunnerLevel = level;

        while (deltaRunnerRunning)
        {
            if (i > 15f)
            {
                i = 0;
                level++;
                deltaRunnerMusic.pitch = 1 + level * 0.05f;
                deltaRunnerGround.material.SetFloat("_HScroll", -0.1f - 0.15f * level);
                deltaRunnerGround.material.SetColor("_Colorize", new Color(0.3f + Random.value * 0.7f, 0.3f + Random.value * 0.7f, 0.3f + Random.value * 0.7f, 1.0f));

                deltaRunnerObstacleMaterial.SetColor("_Colorize", new Color(0.3f + Random.value * 0.7f, 0.3f + Random.value * 0.7f, 0.3f + Random.value * 0.7f, 1.0f));

                Global.DeltaRunnerLevel = level;
            }

            if (j > 4)
            {
                meters += 10;
                var mark = new GameObject("mark_" + meters);
                mark.transform.SetParent(deltaRunnerCanvas);
                mark.transform.localPosition = new Vector3(363f, -173f, 0f);
                mark.layer = deltaRunnerCharacter.gameObject.layer;
                mark.AddComponent<DeltaRunnerObject>();
                mark.AddComponent<RectTransform>();
                var txtMark = mark.AddComponent<TextComponent>();
                txtMark.SetColor(Color.white);
                txtMark.AnglesPerFrame = 15;
                txtMark.AmountOfCycles = 1;
                txtMark.Layer = 34;
                txtMark.Text = meters + "m";
                txtMark.Scale = new Vector2(0.7f, 0.7f);
                txtMark.CharDistance = -16f;
                j = 0;
            }

            int whatToSpawn = Mathf.RoundToInt(Random.value * 4f - Mathf.Clamp(level / 3, 0, 2));
            switch (whatToSpawn)
            {
                case 0:
                    var obstacle = GameObject.Instantiate(deltaRunnerGroundObstacle);
                    obstacle.transform.SetParent(deltaRunnerCanvas, false);
                    obstacle.transform.localPosition = new Vector3(363f, -112f, 0f);
                    obstacle.layer = deltaRunnerCharacter.gameObject.layer;
                    obstacle.tag = "obstacle";
                    break;
                case 1:
                    var airobstacle = GameObject.Instantiate(deltaRunnerAirObstacle);
                    airobstacle.transform.SetParent(deltaRunnerCanvas, false);
                    airobstacle.transform.localPosition = new Vector3(363f, Random.Range(-80f, 16f), 0f);
                    airobstacle.layer = deltaRunnerCharacter.gameObject.layer;
                    airobstacle.tag = "obstacle";
                    break;
                default:
                    var coin = GameObject.Instantiate(deltaRunnerCoin);
                    coin.transform.SetParent(deltaRunnerCanvas, false);
                    coin.transform.localPosition = new Vector3(363f, Random.Range(-112f, 16f), 0f);
                    coin.layer = deltaRunnerCharacter.gameObject.layer;
                    coin.tag = "coin";
                    break;
            }
            float time = 0.4f + Random.Range(0.3f, 0.4f) - Mathf.Clamp(level * 0.11f, 0, 0.59f);
            i += time;
            j += time;

            if (deltaRunnerDied)
            {
                Global.DeltaRunnerPaused = true;
                mainTextBox.AutoBlinkDialogueArrow = true;
                mainTextBox.SetColor(Color.white);
                yield return mainTextBox.SetText("OH NO! You died. Sorry, but we'll be keeping all the coins and money. ");
                yield return WaitForNextClick();
                yield return mainTextBox.SetText("Thanks for playing! ");
                yield return WaitForNextClick();
                mainTextBox.AutoBlinkDialogueArrow = false;
                GameObject.Destroy(deltaRunnerInstance);
                deltaRunnerMusic.Stop();
                backgroundMusic.UnPause();
                Global.DeltaRunnerPaused = false;
                yield break;
            }

            yield return new WaitForSeconds(time);

            if (meters == achievedMeters)
            {
                achievedMeters += 100;
                Global.DeltaRunnerPaused = true;

                deltaRunnerMusic.Pause();
                backgroundMusic.UnPause();

                mainTextBox.DialogueArrow.IsBlinking = false;
                mainTextBox.SetColor(Color.white);
                yield return mainTextBox.SetText("Congratulations! You've reached ");
                mainTextBox.SetColor(jackspotColor);
                yield return mainTextBox.AppendText(meters + "m ");
                mainTextBox.SetColor(Color.white);
                yield return mainTextBox.AppendText("!!!!!");
                mainTextBox.DialogueArrow.IsBlinking = true;
                yield return WaitForNextClick();

                int extraFee = (int)(Mathf.Pow(2f, (meters / 100f) - 1f) * (float)Global.DeltaRunnerFee);
                if (Global.Money < extraFee)
                {
                    mainTextBox.AutoBlinkDialogueArrow = true;
                    mainTextBox.SetColor(Color.white);
                    yield return mainTextBox.SetText("Sorry, but you don't have enough money to ");
                    mainTextBox.SetColor(cropBlankColor);
                    yield return mainTextBox.AppendText("double your coins. ");
                    mainTextBox.SetColor(Color.white);
                    yield return mainTextBox.AppendText("Wanna continue anyway? ");
                    yield return WaitForNextClick();
                    mainTextBox.AutoBlinkDialogueArrow = false;
                    mainTextBox.enabled = false;
                    option1.enabled = option2.enabled = true;
                    option1.Text = "1. YES. Continue without doubling my coins. ";
                    option2.Text = "2. STOP and change coins for money. ";
                    yield return WaitForOptionClick();
                }
                else
                {

                    mainTextBox.DialogueArrow.IsBlinking = false;
                    mainTextBox.SetColor(jackspotColor);
                    yield return mainTextBox.SetText("W A N N A  D O U B L E  Y O U R  C O I N S ? ");
                    mainTextBox.DialogueArrow.IsBlinking = true;
                    yield return WaitForNextClick();
                    mainTextBox.enabled = false;
                    option1.enabled = option2.enabled = true;


                    StartCoroutine(SetExtraFeeText(extraFee));
                    option2.Text = "2. STOP and change coins for money. ";
                    yield return WaitForOptionClick();
                }

                if (currentOption == 0)
                {
                    if (Global.Money >= extraFee)
                    {
                        Global.Money -= extraFee;
                        yield return mainTextBox.SetText("");
                        deltaRunnerCoins *= 2f;
                        deltaRunnerCoinSound.Play();
                        deltaRunnerCoinCounter.Text = Mathf.RoundToInt(deltaRunnerCoins).ToString().PadLeft(3, '0');
                        yield return new WaitForSeconds(0.75f);
                    }
                    yield return mainTextBox.SetText("Thanks! Just click when you are ready to continue. ");
                    yield return WaitForNextClick();
                }
                else
                {
                    yield return EndDeltaRunner(deltaRunnerCoins, deltaRunnerInstance);
                    yield break;
                }

                deltaRunnerMusic.UnPause();
                backgroundMusic.Pause();

                subtitleTextBox.SetColor(objectDialogColor);
                StartCoroutine(subtitleTextBox.SetText("ARCADE:"));
                mainTextBox.SetColor(Color.white);
                mainTextBox.Text = "Running Game...";
                mainTextBox.DialogueArrow.IsBlinking = false;

                mainTextBox.DialogueArrow.IsBlinking = false;
                Global.DeltaRunnerPaused = false;
            }
        }

        yield return WaitForNextClick();
        deltaRunnerMusic.Stop();
        backgroundMusic.UnPause();
        yield break;
    }

    IEnumerator EndDeltaRunner(float coins, GameObject deltaRunnerInstance)
    {
        mainTextBox.AutoBlinkDialogueArrow = true;
        mainTextBox.SetColor(Color.white);
        yield return mainTextBox.SetText("You got a total of ");
        mainTextBox.SetColor(cropBlankColor);
        yield return mainTextBox.AppendText(coins + " ");
        mainTextBox.SetColor(Color.white);
        yield return mainTextBox.AppendText("coins. ");
        yield return WaitForNextClick();
        float ratio = 2.5f - Random.value * 1.1f;
        yield return mainTextBox.SetText("Converting to our current ratio of ");
        mainTextBox.SetColor(jackspotColor);
        yield return mainTextBox.AppendText(((decimal)ratio).ToString("0.00") + " ");
        mainTextBox.SetColor(Color.white);
        yield return mainTextBox.AppendText(", you'll receive ");
        mainTextBox.SetColor(cropBlankColor);
        decimal amount = ((decimal)(coins / ratio));
        yield return mainTextBox.AppendText("$" + amount.ToString("0.00"));
        yield return WaitForNextClick();
        Global.Money += amount;
        mainTextBox.SetColor(Color.white);
        yield return mainTextBox.SetText("Thanks for playing! ");
        yield return WaitForNextClick();
        GameObject.Destroy(deltaRunnerInstance);
        mainTextBox.AutoBlinkDialogueArrow = false;
        deltaRunnerRunning = false;
    }

    IEnumerator HandlePlayer(SpriteRenderer deltaRunnerCharacter, SpriteRenderer deltaRunnerPause, TextComponent deltaRunnerCoinCounter)
    {
        float currentVelocityY = 0f;
        float currentVelocityX = 0f;
        float randomPosX = deltaRunnerCharacter.transform.localPosition.x + Random.value * Random.Range(-2f, 2f) * 40f;
        int i = 0;
        while (deltaRunnerRunning)
        {
            if (deltaRunnerDied)
            {
                yield break;
            }

            if (Global.DeltaRunnerPaused)
            {
                deltaRunnerPause.enabled = true;
                yield return 1;
                continue;
            }
            else
            {
                deltaRunnerPause.enabled = false;
            }

            i++;
            if (i % 30 == 0)
            {
                randomPosX = deltaRunnerCharacter.transform.localPosition.x + Random.value * Random.Range(-2f, 2f) * 40f;
            }
            deltaRunnerCharacter.transform.localPosition = new Vector3(
                Mathf.Clamp(Mathf.SmoothDamp(deltaRunnerCharacter.transform.localPosition.x, randomPosX, ref currentVelocityX, 0.85f), -361f, -100f),
                Mathf.SmoothDamp(deltaRunnerCharacter.transform.localPosition.y, Input.GetMouseButton(0) ? 16f : -112, ref currentVelocityY, 0.25f),
                deltaRunnerCharacter.transform.localPosition.z);

            RaycastHit2D hit = Physics2D.Raycast(deltaRunnerCharacter.transform.position, Vector2.right, 10f + Global.DeltaRunnerLevel, deltaRunnerLayerMask);
            if (hit.collider != null)
            {
                switch (hit.collider.gameObject.tag)
                {
                    case "coin":
                        deltaRunnerCoinSound.Play();
                        deltaRunnerCoins += 1f;
                        deltaRunnerCoinCounter.Text = Mathf.RoundToInt(deltaRunnerCoins).ToString().PadLeft(3, '0');
                        break;
                    case "obstacle":
                        deltaRunnerDied = true;
                        break;
                }
                GameObject.Destroy(hit.collider.gameObject);
            }

            yield return 1;
        }
    }

    IEnumerator PlayLuckySlots()
    {
        subtitleTextBox.SetColor(objectDialogColor);
        mainTextBox.AutoBlinkDialogueArrow = true;
        StartCoroutine(arcadeSmoothIn.GoToPosition());
        StartCoroutine(subtitleTextBox.SetText("ARCADE:"));
        yield return mainTextBox.SetText("I'm sorry! The developer didn't have enough time to create this mini-game for Adventure Jam! ");
        yield return WaitForNextClick();
        yield return arcadeSmoothIn.ReturnToOriginal();
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

            if (deltaRunner.IsOverlapping && Input.GetMouseButtonDown(0))
            {
                busy = true;
                yield return PlayDeltaRunner();
                yield return ResetActions();
                busy = false;
            }


            if (luckySlots.IsOverlapping && Input.GetMouseButtonDown(0))
            {
                busy = true;
                yield return PlayLuckySlots();
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
}
