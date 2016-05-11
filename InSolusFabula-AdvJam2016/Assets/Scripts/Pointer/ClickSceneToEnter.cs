using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System;

public class ClickSceneToEnter : MonoBehaviour
{
    public Camera cam;
    public ColliderMouseOver mouseOver;
    public AudioSource playOnClick;
    public Mall_Central eventCentral;
    public string SceneName;

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
            StartCoroutine(ZoomIn(LoadScene));
        }
    }

    IEnumerator ZoomIn(Action callback)
    {
        float time = 0f;
        while (cam.orthographicSize > 0.0000000000001f) // weird camera effect
        {
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, 0, time/15);
            cam.transform.position = Vector3.Lerp(cam.transform.position, new Vector3(this.transform.position.x, this.transform.position.y, cam.transform.position.z), time/15);
            time += Time.deltaTime;
            yield return 1;
        }
        callback();
    }

    private void LoadScene()
    {
        SceneManager.LoadScene(SceneName);
    }
}
