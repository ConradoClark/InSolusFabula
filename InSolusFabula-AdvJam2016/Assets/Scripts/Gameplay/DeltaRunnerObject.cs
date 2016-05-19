using UnityEngine;
using System.Collections;

public class DeltaRunnerObject : MonoBehaviour
{
    void Start()
    {

    }

    void Update()
    {
        if (Global.DeltaRunnerPaused)
        {
            return;
        }

        this.transform.localPosition = new Vector3(this.transform.localPosition.x - Time.deltaTime * 300f - Time.deltaTime*50f*Global.DeltaRunnerLevel,
            this.transform.localPosition.y,
            this.transform.localPosition.z);

        if (this.transform.localPosition.x < -350f)
        {
            GameObject.Destroy(this.gameObject);
        }
    }
}
