using UnityEngine;
using System.Collections;

public class DialogueArrow : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public bool IsBlinking { get; set; }

    void Start()
    {
        StartCoroutine(Wait());
    }

    IEnumerator Blink()
    {
        while (IsBlinking)
        {
            spriteRenderer.enabled = true;
            yield return new WaitForSeconds(0.3f);
            spriteRenderer.enabled = false;
            yield return new WaitForSeconds(0.3f);
        }
        StartCoroutine(Wait());
    }

    IEnumerator Wait()
    {
        spriteRenderer.enabled = false;
        while (!IsBlinking)
        {
            yield return 1;
        }
        StartCoroutine(Blink());
    }
}
