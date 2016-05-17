using UnityEngine;
using System.Collections;

public class EnableSelectionOnMouse : MonoBehaviour
{
    public SpriteRenderer selection;
    public ColliderMouseOver mouseOver;

    void Update()
    {
        selection.enabled = mouseOver.IsOverlapping;
    }
}
