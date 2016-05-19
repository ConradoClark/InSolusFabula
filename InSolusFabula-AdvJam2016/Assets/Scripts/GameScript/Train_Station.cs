using UnityEngine;
using System.Collections;
using UnityStandardAssets.ImageEffects;
using UnityEngine.SceneManagement;

public class Train_Station : MonoBehaviour
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
    public Color neniaDialogColor;
    public Color mcDialogColor;
    public Color mcOtherDialogColor;
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
    public ColliderMouseOver pushingman;
    public ColliderMouseOver nenia;
    public ColliderMouseOver trainTracks;
    public Transform pushingmanTransform;
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
            mainTextBox.Text = "You see train tracks. Is it time to go? ";
        }
        else if (Global.Decisions.BlueCardWrittenAboutCalim)
        {
            mainTextBox.Text = "You feel familiar here. ";
        }
        else if (Global.Decisions.BlueCardWrittenAboutLove)
        {
            mainTextBox.Text = "Nothing is more romantic than a train. ";
        }
        else
        {
            mainTextBox.Text = "This is the train station. It seems calm. ";
        }

        subtitleTextBox.Text = Global.Time;
        yield return new WaitForSeconds(1f);
        yield return CheckActions();
    }

    IEnumerator InspectPushingMan()
    {
        yield break;
    }

    IEnumerator TalkToNenia()
    {
        yield break;
    }

    IEnumerator InspectTrainTracks()
    {
        cameraBlur.enabled = true;

        subtitleTextBox.SetColor(mcDialogColor);
        subtitleTextBox.Text = "-LOOKING-";
        mainTextBox.Text = "You see the train tracks. ";
        yield return WaitForNextClick();

        if (Global.Decisions.YouAreACreep)
        {
            if (Global.Decisions.BlueCardWrittenAboutLove)
            {
                mainTextBox.Text = "Somehow, you know that you must not jump. There is another way. ";
                yield return WaitForNextClick();
            }
            else
            {
                mainTextBox.Text = "Be careful! Right? ";
                yield return WaitForNextClick();
            }
        }
        else if (Global.Decisions.NeniaFirstFateShift)
        {
            if (Global.Decisions.BlueCardWrittenAboutTime)
            {
                mainTextBox.Text = "You know that the train arrives at a precise time. ";
                yield return WaitForNextClick();
            }

            if (Global.Decisions.BlueCardWrittenAboutCalim)
            {
                mainTextBox.Text = "You wonder if this the exit to the real world. ";
                yield return WaitForNextClick();
            }
            else
            {
                mainTextBox.Text = "Surprisingly, you feel an urge to jump. ";
                yield return WaitForNextClick();
            }
        }
        else
        {
            mainTextBox.Text = "Your mind wonders what would happen if you were to jump. ";
            yield return WaitForNextClick();
        }

        mainTextBox.enabled = false;
        option1.enabled = option2.enabled = true;

        option1.Text = "JUMP. -ENDING- ";
        option2.Text = "Don't jump. ";

        yield return WaitForOptionClick();

        switch (currentOption)
        {
            case 0:
                yield return GlimpseOfTheFuture();
                SceneManager.LoadScene("Ending");
                break;
            case 1:
                break;
        }

        cameraBlur.enabled = false;
    }

    IEnumerator GlimpseOfTheFuture()
    {
        subtitleTextBox.SetColor(mcDialogColor);
        subtitleTextBox.Text = "-LOOKING- ";
        mainTextBox.AutoBlinkDialogueArrow = false;
        mainTextBox.DialogueArrow.IsBlinking = false;
        backgroundMusic.Stop();
        hiss.Play();
        mainTextBox.Text = "You touch the ephemeral hand of the future. ";
        StartCoroutine(BlinkGlimpse1());
        yield return new WaitForSeconds(hiss.clip.length);
        mainTextBox.AutoBlinkDialogueArrow = true;
        mainTextBox.DialogueArrow.IsBlinking = true;
        yield return WaitForNextClick();
        yield return mainTextBox.SetText("You open your eyes and see what's beyond. ");
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
            if (pushingman.IsOverlapping && Input.GetMouseButtonDown(0))
            {
                busy = true;
                yield return InspectPushingMan();
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

            if (trainTracks.IsOverlapping && Input.GetMouseButtonDown(0))
            {
                busy = true;
                yield return InspectTrainTracks();
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
