using UnityEngine;
using System.Collections;

public class MouseDescription : MonoBehaviour
{
    public Transform objectToFollow;
    public TextComponent textComponent;
    public ObjectDescription[] objectDescriptions;
    public Vector2 offset;

    void Update()
    {
        var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        objectToFollow.transform.position = new Vector3(mousePos.x + offset.x, mousePos.y + offset.y, objectToFollow.transform.position.z);

        bool foundOverlap = false;

        for (int i = 0; i < objectDescriptions.Length; i++)
        {
            ObjectDescription desc = objectDescriptions[i];
            if (desc.mouseOver.IsOverlapping)
            {
                objectToFollow.gameObject.SetActive(true);
                textComponent.Text = desc.Description;
                foundOverlap = true;
                break;                
            }
        }

        if (!foundOverlap)
        {
            textComponent.Text = "";
            textComponent.Restart();
            objectToFollow.gameObject.SetActive(false);
        }
    }
}
