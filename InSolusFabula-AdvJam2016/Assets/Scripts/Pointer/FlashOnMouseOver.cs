using UnityEngine;
using System.Collections;

[RequireComponent(typeof(ColliderMouseOver))]
public class FlashOnMouseOver : MonoBehaviour {

    public ColliderMouseOver Collider;
    public SpriteRenderer SpriteRenderer;
    public TextComponent textComponent;
    public string Text;

	void Start () {
        StartCoroutine(Wait());
	}

    IEnumerator Wait()
    {
        while (!this.Collider.IsOverlapping)
        {
            yield return 1;
        }
        StartCoroutine(Flash());
    }

    IEnumerator Flash()
    {
        if (textComponent != null)
        {
            textComponent.Text = Text;
        }

        if (this.SpriteRenderer == null)
        {
            while (this.Collider.IsOverlapping)
            {
                yield return 1;
            }
            StartCoroutine(Wait());
            yield break;
        }

        float tick = 0;
        while (this.Collider.IsOverlapping)
        {
            tick += Time.deltaTime;
            this.SpriteRenderer.material.SetFloat("_Luminance", (1-Mathf.Cos(tick*10))/3 );
            yield return 1;
        }
        this.SpriteRenderer.material.SetFloat("_Luminance", 0);
        StartCoroutine(Wait());
    }
}
