using UnityEngine;
using System.Collections;

public class ColliderMouseOver : MonoBehaviour
{
    public Collider2D AssignedCollider;
    public bool IsOverlapping { get; set; }

    void Update()
    {
        if (AssignedCollider == null) return;

        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (AssignedCollider.OverlapPoint(mousePosition))
        {
            this.IsOverlapping = true;
        }
        else
        {
            this.IsOverlapping = false;
        }
    }
}
