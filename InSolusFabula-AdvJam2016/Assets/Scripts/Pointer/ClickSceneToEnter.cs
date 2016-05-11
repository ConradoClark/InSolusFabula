using UnityEngine;
using System.Collections;

public class ClickSceneToEnter : MonoBehaviour
{
    public Camera cam;
    public ColliderMouseOver mouseOver;
    public AudioSource playOnClick;
    public Mall_Central eventCentral;

    bool clicked = false;
    void Start()
    {

    }

    void Update()
    {
        if (mouseOver.IsOverlapping && Input.GetMouseButtonDown(0) && !clicked)
        {
            clicked = true;
            if (playOnClick != null)
            {
                eventCentral.StopMusic();
                playOnClick.Play();
            }
            StartCoroutine(ZoomIn());
        }
    }

    IEnumerator ZoomIn()
    {
        float time = 0f;
        while (cam.orthographicSize > 0)
        {
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, 0, time/10);
            cam.transform.position = Vector3.Lerp(cam.transform.position, new Vector3(this.transform.position.x, this.transform.position.y, cam.transform.position.z), time/10);
            time += Time.deltaTime;
            yield return 1;
        }
    }
}
