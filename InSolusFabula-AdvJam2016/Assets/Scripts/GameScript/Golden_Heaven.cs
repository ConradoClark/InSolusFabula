using UnityEngine;
using System.Collections;

public class Golden_Heaven : MonoBehaviour
{
    public Camera cam;
    public SmoothIntoPosition dialogueSmoothIn;

    void Start()
    {
        StartCoroutine(StartGoldenHeaven());
    }

    IEnumerator StartGoldenHeaven()
    {
        cam.orthographicSize = 0.000000000000000001f;
        StartCoroutine(dialogueSmoothIn.GoToPosition());
        yield return ZoomOut();
    }

    IEnumerator ZoomOut()
    {
        float time = 0f;
        while (cam.orthographicSize < 360f)
        {
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, 360, time / 15);
            cam.transform.position = Vector3.Lerp(cam.transform.position, new Vector3(this.transform.position.x, this.transform.position.y, cam.transform.position.z), time / 15);
            time += Time.deltaTime;
            yield return 1;
        }
    }
}
