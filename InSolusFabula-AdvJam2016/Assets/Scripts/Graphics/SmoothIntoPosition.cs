using UnityEngine;
using System.Collections;

public class SmoothIntoPosition : MonoBehaviour
{
    public Vector2 finalPosition;
    public Transform target;
    public float speed;
    private bool active;

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
        Vector2 initialPosition = target.localPosition;
        while (Vector2.Distance(target.localPosition, finalPosition) > 0.1f)
        {
            var step = -20 * Mathf.Pow(time, 7) + 70 * Mathf.Pow(time, 6) - 84 * Mathf.Pow(time, 5) + 35 * Mathf.Pow(time, 4);

            float x = Mathf.Lerp(initialPosition.x, finalPosition.x, step);
            float y = Mathf.Lerp(initialPosition.y, finalPosition.y, step);
            target.localPosition = new Vector3(x, y, target.localPosition.z);
            time += Time.deltaTime * speed;
            yield return 1;
        }
    }
}
