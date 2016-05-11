using UnityEngine;
using System.Collections;

public class SpinFadeIn : MonoBehaviour
{
    public int anglesPerFrame;
    public int amountOfCycles;
    void Start()
    {
        StartCoroutine(Spin());
    }

    IEnumerator Spin()
    {
        int cycles = 0;
        float totalAngles = 0f;
        while (cycles<amountOfCycles)
        {
            this.transform.Rotate(new Vector3(0, anglesPerFrame, 0));
            totalAngles += anglesPerFrame;
            cycles = (int)totalAngles / 360;
            yield return 1;
        }
        this.transform.Rotate(new Vector3(0, 0, 0));
    }
}
