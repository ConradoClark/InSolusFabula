using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityStandardAssets.ImageEffects;

public class Golden_Heaven : MonoBehaviour
{
    public Camera cam;
    public Camera uicam;
    public SmoothIntoPosition dialogueSmoothIn;
    public SpriteRenderer sceneLight;
    public SpriteRenderer goldenHeaven1;
    public SpriteRenderer goldenHeaven2;
    public SpriteRenderer goldRing;
    private bool flickLights;
    private bool checkForBack;
    public ColliderMouseOver backButton;
    public AudioSource backgroundMusic;
    public string backScene;
    public SpriteRenderer transition;
    public ColliderMouseOver shopkeeper;
    public ColliderMouseOver nenia;
    public ColliderMouseOver goldring;
    private bool checkActions;
    public TextComponent mainTextBox;
    public TextComponent subtitleTextBox;
    public Color shopkeeperDialogColor;
    public Color mcDialogColor;
    public Color mcOtherDialogColor;
    public Color goldRingTextColor;
    public Color jackspotColor;
    public Color objectDialogColor;
    public Color neniaDialogColor;
    public ColliderMouseOver mainTextBoxCollider;
    private bool busy;
    public BlurOptimized cameraBlur;
    public SmoothIntoPosition shopKeeperFadeIn;
    public TextComponent option1;
    public TextComponent option2;
    public TextComponent option3;
    public AudioSource hiss;
    private int currentOption;

    void Start()
    {
        StartCoroutine(StartGoldenHeaven());
    }

    IEnumerator StartGoldenHeaven()
    {
        cam.orthographicSize = 0.000000000000000001f;
        uicam.orthographicSize = 0.000000000000000001f;
        StartCoroutine(FlickLights());
        StartCoroutine(CheckForBack());
        StartCoroutine(dialogueSmoothIn.GoToPosition());
        StartCoroutine(ZoomOut());
        mainTextBox.Text = "The light flickers. You see the silhouette of someone you know.";
        yield return new WaitForSeconds(1f);
        yield return CheckActions();
    }

    IEnumerator CheckActions()
    {
        checkActions = true;
        while (checkActions)
        {
            if (busy)
            {
                yield return 1;
                continue;
            }
            if (shopkeeper.IsOverlapping && Input.GetMouseButtonDown(0))
            {
                yield return TalkToShopkeeper();
                yield return ResetActions();
            }

            if (nenia.IsOverlapping && Input.GetMouseButtonDown(0))
            {
                yield return TalkToNenia();
                yield return ResetActions();
            }

            if (goldring.IsOverlapping && Input.GetMouseButtonDown(0))
            {
                yield return InspectGoldRing();
                yield return ResetActions();
            }
            yield return 1;
        }
    }

    IEnumerator ResetActions()
    {
        subtitleTextBox.SetColor(mcDialogColor);
        subtitleTextBox.Text = "10:00 AM";
        mainTextBox.Text = "The light flickers. You see the silhouette of someone you know.";
        yield break;
    }

    #region Shopkeeper
    IEnumerator TalkToShopkeeper()
    {
        cameraBlur.enabled = true;
        StartCoroutine(shopKeeperFadeIn.GoToPosition());
        subtitleTextBox.SetColor(shopkeeperDialogColor);
        StartCoroutine(subtitleTextBox.SetText("SHOPKEEPER:"));
        yield return mainTextBox.SetText("Hello sir! How may I help you today?");
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
        yield return mainTextBox.SetText("Are you interested in one of our most precious ");
        mainTextBox.SetColor(goldRingTextColor);
        yield return mainTextBox.AppendText("gold rings");
        mainTextBox.SetColor(Color.white);
        yield return mainTextBox.AppendText("? ");
        mainTextBox.DialogueArrow.IsBlinking = true;
        yield return WaitForNextClick();
        mainTextBox.DialogueArrow.IsBlinking = false;
        yield return mainTextBox.SetText("I can sell you one for $500.00 ");
        mainTextBox.DialogueArrow.IsBlinking = true;
        yield return WaitForNextClick();

        startBuyRing:
        subtitleTextBox.SetColor(mcDialogColor);
        StartCoroutine(subtitleTextBox.SetText("CALIM:"));

        mainTextBox.enabled = false;
        option1.enabled = option2.enabled = true;
        bool hasMoney = Global.Money >= 500M;
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
        yield return mainTextBox.SetText("Thanks for your patronage. I'm sure your partner is going to love it! ");
        Global.Inventory.HasRing = true;
        goldRing.gameObject.SetActive(false);

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
        if (Global.Inventory.HasRing)
        {
            subtitleTextBox.SetColor(shopkeeperDialogColor);
            StartCoroutine(subtitleTextBox.SetText("SHOPKEEPER:"));
            yield return mainTextBox.SetText("When are you going to get married? This is so exciting! ");
            yield return WaitForNextClick();
        }
        else
        {
            subtitleTextBox.SetColor(shopkeeperDialogColor);
            StartCoroutine(subtitleTextBox.SetText("SHOPKEEPER:"));
            yield return mainTextBox.SetText("Are you enjoying the mall? I've heard you can earn a lot of money at the  ");
            mainTextBox.SetColor(jackspotColor);
            yield return mainTextBox.AppendText("Jackspot ");
            mainTextBox.SetColor(Color.white);
            yield return mainTextBox.AppendText(". ");
            yield return WaitForNextClick();
        }
    }
    #endregion

    #region Nenia
    IEnumerator TalkToNenia()
    {
        cameraBlur.enabled = true;
        StartCoroutine(subtitleTextBox.SetText("CALIM:"));
        mainTextBox.AutoBlinkDialogueArrow = true;
        yield return mainTextBox.SetText("Nenia...! ");
        yield return WaitForNextClick();
        subtitleTextBox.SetColor(neniaDialogColor);
        StartCoroutine(subtitleTextBox.SetText("-LOOKING-"));
        yield return mainTextBox.SetText("- She turns around with an awkward smile. - ");
        yield return WaitForNextClick();
        subtitleTextBox.SetColor(mcDialogColor);
        StartCoroutine(subtitleTextBox.SetText("CALIM:"));

        mainTextBox.enabled = false;
        option1.enabled = option2.enabled = option3.enabled = true;

        option1.SetColor(Color.yellow);
        option1.Text = "- You remember Nenia. - FATE SHIFT - ";
        option2.SetColor(Color.red);
        option2.Text = "??????? - You remember something else - FATE SHIFT - ";
        option3.Text = "- You have something else to think - ";

        yield return WaitForOptionClick();

        switch (currentOption)
        {
            case 2:
                yield return TalkToNeniaMoreOptions();                
                break;
            case 0:
                yield return YouRememberNenia();
                break;
            default:
                break;
        }
        option1.SetColor(Color.white);
        option2.SetColor(Color.white);
        option3.SetColor(Color.white);
        cameraBlur.enabled = false;
    }

    IEnumerator YouRememberNenia()
    {
        StartCoroutine(subtitleTextBox.SetText("-LOOKING-"));
        yield return mainTextBox.SetText("You feel a faint glimpse of the future. ");
        backgroundMusic.Stop();
        hiss.Play();
        mainTextBox.AutoBlinkDialogueArrow = true;
        yield return new WaitForSeconds(hiss.clip.length);        
        subtitleTextBox.SetColor(neniaDialogColor);        
        yield return WaitForNextClick();
        yield return mainTextBox.SetText("Is this what happened that day? ");
        yield return WaitForNextClick();
        backgroundMusic.Play();
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
    }

    IEnumerator YouRememberNeniaFlirt()
    {
        subtitleTextBox.SetColor(neniaDialogColor);
        StartCoroutine(subtitleTextBox.SetText("NENIA:"));
        yield return mainTextBox.SetText("Ohh, you're such a wooer. Either way, I'm not serious... This is way to expensive. ");
        yield return WaitForNextClick();
    }

    IEnumerator YouRememberNeniaPoor()
    {
        subtitleTextBox.SetColor(neniaDialogColor);
        StartCoroutine(subtitleTextBox.SetText("NENIA:"));
        yield return mainTextBox.SetText("Don't be such a downer... But yeah, you're right. This is way out of our league. ");
        yield return WaitForNextClick();
    }

    IEnumerator TalkToNeniaMoreOptions()
    {
        mainTextBox.enabled = false;
        yield return 1;        
        option1.enabled = option2.enabled = option3.enabled = true;
        option1.SetColor(Color.red);
        option1.Text = "??????? - Things are a little clouded - FATE SHIFT - ";
        option2.SetColor(Color.red);
        option2.Text = "??????? - You ARE the most important to me. FATE SHIFT - ";
        option3.Text = "- Nevermind. My memories are still too weak - ";
        yield return WaitForOptionClick();
    }

    #endregion

    #region GoldRing

    IEnumerator InspectGoldRing()
    {
        cameraBlur.enabled = true;
        subtitleTextBox.SetColor(objectDialogColor);
        StartCoroutine(subtitleTextBox.SetText("-LOOKING-"));
        mainTextBox.AutoBlinkDialogueArrow = true;
        yield return mainTextBox.SetText("You see a shining gold ring. It seems expensive. ");
        yield return WaitForNextClick();
        yield return mainTextBox.SetText("You have delusional thoughts of giving this to someone you love.");
        yield return WaitForNextClick();
        cameraBlur.enabled = false;
    }
    #endregion

    IEnumerator WaitForOptionClick()
    {
        busy = true;

        while (busy)
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
        busy = false;
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
            while (!backButton.IsOverlapping || !Input.GetMouseButtonDown(0))
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
