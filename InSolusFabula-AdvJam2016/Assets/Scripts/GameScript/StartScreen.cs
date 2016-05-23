using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class StartScreen : MonoBehaviour
{
    public SpriteRenderer blankScreen;
    public DialogueArrow DialogueArrow;
    public TextComponent TextComponent;
    public SmoothIntoPosition TextBoxSmoothFadeIn;
    public SmoothIntoPosition isl_leftFadeIn;
    public SmoothIntoPosition isl_rightFadeIn;
    public AudioSource InitialBoom;

    void Start()
    {
        StartCoroutine(StartPrologue());
    }

    IEnumerator StartPrologue()
    {
        blankScreen.material.SetFloat("_Invert", 1);
        StartCoroutine(isl_leftFadeIn.GoToPosition());
        StartCoroutine(isl_rightFadeIn.GoToPosition());
        yield return new WaitForSeconds(3.85f);
        blankScreen.sortingOrder = 2;
        InitialBoom.Play();
        blankScreen.material.SetFloat("_Invert", 0);
        yield return SmoothOutLuminance();
        yield return TextBoxSmoothFadeIn.GoToPosition();
        this.TextComponent.Text = "Click here to start!";
        yield return WaitForNextClick();
        StartCoroutine(isl_leftFadeIn.ReturnToOriginal());
        StartCoroutine(TextBoxSmoothFadeIn.ReturnToOriginal());
        yield return isl_rightFadeIn.ReturnToOriginal();
        
        SceneManager.LoadScene("Prologue");
    }

    IEnumerator SmoothOutLuminance()
    {
        float time = 0f;
        float value = 0f;
        float value2 = 1f;
        while (value < 0.99)
        {
            var step = -20 * Mathf.Pow(time, 7) + 70 * Mathf.Pow(time, 6) - 84 * Mathf.Pow(time, 5) + 35 * Mathf.Pow(time, 4);
            value = Mathf.Lerp(0, 1, step);
            value2 = Mathf.Lerp(1, 0, step);
            blankScreen.material.SetFloat("_Luminance", -value);
            blankScreen.material.SetFloat("_Opacity", value2);
            time += Time.deltaTime / 5;
            yield return 1;
        }
    }

    IEnumerator WaitForNextClick()
    {
        while (!Input.GetMouseButtonDown(0) || !TextComponent.IsFullyRendered)
        {
            yield return 1;
        }
        yield return 1;
    }
}
