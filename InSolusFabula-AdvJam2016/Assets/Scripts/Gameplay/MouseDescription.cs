using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class MouseDescription : MonoBehaviour
{
    public Transform objectToFollow;
    public TextComponent textComponent;
    public ObjectDescription[] objectDescriptions;
    private List<ObjectDescription> inventoryDescriptions;
    public Vector2 offset;

    public void ClearInventoryDescriptions()
    {
        if (this.inventoryDescriptions == null)
        {
            this.inventoryDescriptions = new List<ObjectDescription>();
        }
        this.inventoryDescriptions.Clear();
    }

    public void AddInventoryDescription(ObjectDescription description)
    {
        if (this.inventoryDescriptions == null)
        {
            this.inventoryDescriptions = new List<ObjectDescription>();
        }
        this.inventoryDescriptions.Add(description);
    }

    void Update()
    {
        var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        objectToFollow.transform.position = new Vector3(mousePos.x + offset.x, mousePos.y + offset.y, objectToFollow.transform.position.z);

        bool foundOverlap = false;

        var fullList = objectDescriptions.Concat(inventoryDescriptions).ToArray();

        for (int i = 0; i < fullList.Length ; i++)
        {
            ObjectDescription desc = fullList[i];
            if (desc != null && desc.mouseOver.IsOverlapping)
            {
                objectToFollow.gameObject.SetActive(true);
                textComponent.Text = desc.Description;
                objectToFollow.transform.position = new Vector3(mousePos.x + offset.x + desc.offset.x, mousePos.y + offset.y + desc.offset.y, objectToFollow.transform.position.z);
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
