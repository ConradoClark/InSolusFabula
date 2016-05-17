using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Prologue : MonoBehaviour
{
    public SpriteRenderer blankScreen;
    public DialogueArrow DialogueArrow;
    public ColliderMouseOver TextBoxCollider;
    public TextComponent TextComponent;
    public SmoothIntoPosition TextBoxSmoothFadeIn;
    public SmoothIntoPosition TextBoxSmoothFadeOut;
    public AudioSource InitialBoom;

    void Start()
    {
        StartCoroutine(StartPrologue());
    }

    IEnumerator StartPrologue()
    {
        blankScreen.material.SetFloat("_Invert", 1);
        yield return new WaitForSeconds(3.85f);
        blankScreen.material.SetFloat("_Invert", 0);
        InitialBoom.Play();
        this.TextComponent.Text = "";
        yield return SmoothOutLuminance();
        yield return TextBoxSmoothFadeIn.GoToPosition();
        this.TextComponent.SoundEnabled = true;
        yield return this.TextComponent.SetText("I never wanted");
        this.TextComponent.SetColor(Color.red);
        yield return this.TextComponent.AppendText(" ANY");
        yield return new WaitForSeconds(0.5f);
        this.TextComponent.SetColor(Color.white);
        yield return this.TextComponent.AppendText(" of");
        yield return this.TextComponent.AppendText(" this... ");
        this.TextComponent.DialogueArrow.IsBlinking = true;

        yield return WaitForNextClick();
        this.TextComponent.SetColor(Color.white);
        this.TextComponent.DialogueArrow.IsBlinking = false;
        yield return this.TextComponent.SetText("For one last time");
        yield return new WaitForSeconds(0.5f);
        yield return this.TextComponent.AppendText(".");
        yield return new WaitForSeconds(0.5f);
        yield return this.TextComponent.AppendText(".");
        yield return new WaitForSeconds(0.5f);
        yield return this.TextComponent.AppendText(".");
        yield return new WaitForSeconds(1f);
        yield return this.TextComponent.AppendText("I pretended to see her smile. ");
        this.TextComponent.DialogueArrow.IsBlinking = true;
        yield return WaitForNextClick();        

        this.TextComponent.DialogueArrow.IsBlinking = false;
        yield return this.TextComponent.SetText("Truthfully...");
        yield return new WaitForSeconds(1f);
        yield return this.TextComponent.AppendText(" It was the last time the world saw mine. ");
        this.TextComponent.DialogueArrow.IsBlinking = true;
        yield return WaitForNextClick();
        yield return TextBoxSmoothFadeOut.GoToPosition();
        SceneManager.LoadScene("Mall_Central");
    }

    IEnumerator SmoothOutLuminance()
    {
        float time = 0f;
        float value = 0f;
        while (value < 0.99)
        {
            var step = -20 * Mathf.Pow(time, 7) + 70 * Mathf.Pow(time, 6) - 84 * Mathf.Pow(time, 5) + 35 * Mathf.Pow(time, 4);
            value = Mathf.Lerp(0, 1, step);
            blankScreen.material.SetFloat("_Luminance", -value);
            time += Time.deltaTime / 5;
            yield return 1;
        }
    }

    IEnumerator WaitForNextClick()
    {
        while (!TextComponent.IsFullyRendered || !(TextBoxCollider.IsOverlapping && Input.GetMouseButtonDown(0)))
        {
            yield return 1;
        }
        yield return 1;
    }
}
