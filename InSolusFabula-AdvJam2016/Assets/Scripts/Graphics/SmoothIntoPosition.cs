using UnityEngine;
using System.Collections;

public class SmoothIntoPosition : MonoBehaviour
{
    public Vector2 finalPosition;
    private Vector2 initialPosition;
    public Transform target;
    public float speed;
    private bool active;

    private void Start()
    {
        this.initialPosition = target.localPosition;
    }

    public void Activate()
    {
        if (!active)
        {
            StartCoroutine(GoToPosition());
        }
        active = true;
    }

    public IEnumerator GoToPosition()
    {
        float time = 0;
        Vector2 localPosition = target.localPosition;
        while (Vector2.Distance(target.localPosition, finalPosition) > 0.1f)
        {
            var step = -20 * Mathf.Pow(time, 7) + 70 * Mathf.Pow(time, 6) - 84 * Mathf.Pow(time, 5) + 35 * Mathf.Pow(time, 4);

            float x = Mathf.Lerp(localPosition.x, finalPosition.x, step);
            float y = Mathf.Lerp(localPosition.y, finalPosition.y, step);
            target.localPosition = new Vector3(x, y, target.localPosition.z);
            time += Time.deltaTime * speed;
            yield return 1;
        }
    }

    public IEnumerator ReturnToOriginal()
    {
        float time = 0;
        Vector2 localPosition = target.localPosition;
        while (Vector2.Distance(target.localPosition, initialPosition) > 0.1f)
        {
            var step = -20 * Mathf.Pow(time, 7) + 70 * Mathf.Pow(time, 6) - 84 * Mathf.Pow(time, 5) + 35 * Mathf.Pow(time, 4);

            float x = Mathf.Lerp(localPosition.x, initialPosition.x, step);
            float y = Mathf.Lerp(localPosition.y, initialPosition.y, step);
            target.localPosition = new Vector3(x, y, target.localPosition.z);
            time += Time.deltaTime * speed;
            yield return 1;
        }
    }
}
