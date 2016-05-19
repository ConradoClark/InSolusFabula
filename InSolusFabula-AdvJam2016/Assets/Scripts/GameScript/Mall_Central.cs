using UnityEngine;
using System.Collections;

public class Mall_Central : MonoBehaviour
{
    public TextComponent subtitleTextBox;
    public AudioSource[] InitialMallMusic;
    public bool musicPlaying;
    int currentMusic = 0;

    void Start()
    {
        StartCoroutine(InitialMall());
    }

    IEnumerator InitialMall()
    {
        subtitleTextBox.Text = Global.Time;
        StartCoroutine(InitialMallMusicLoop());
        while (true)
        {
            yield return 1;
        }
    }

    IEnumerator InitialMallMusicLoop()
    {
        musicPlaying = true;
        while (musicPlaying)
        {
            currentMusic = Mathf.RoundToInt(Random.value * (InitialMallMusic.Length-1));
            InitialMallMusic[currentMusic].pitch = 1.5f;
            InitialMallMusic[currentMusic].Play();
            float wait = (InitialMallMusic[currentMusic].clip.length - 0.03f) /1.5f;

            yield return new WaitForSeconds(wait);
        }

        InitialMallMusic[currentMusic].Stop();
    }

    public void StopMusic()
    {
        musicPlaying = false;
        InitialMallMusic[currentMusic].Stop();
    }
}
