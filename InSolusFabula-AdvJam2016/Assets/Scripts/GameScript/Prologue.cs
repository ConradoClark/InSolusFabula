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
        this.TextComponent.Text = "I never wanted any of this.";
        yield return WaitForNextClick();
        this.TextComponent.Text = "For one last time, I pretended to see her smile...";
        yield return WaitForNextClick();
        this.TextComponent.Text = "Truthfully, it was the last time the world saw mine...";
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
