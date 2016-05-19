using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Inventory : MonoBehaviour
{
    public MouseDescription MouseDescription;
    public Vector2 DescriptionOffset;
    public ItemObject[] Items;
    private int j;

    public void Start()
    {
        Draw();
    }

    public void Draw()
    {
        j = 0;
        MouseDescription.ClearInventoryDescriptions();
        foreach (Transform glyph in this.transform)
        {
            Destroy(glyph.gameObject);
        }

        for (int i=0;i<Items.Length;i++)
        {
            ItemObject itemObj = Items[i];
            if (!Global.Inventory.HasItem[itemObj.ItemId]) continue;

            GameObject obj = new GameObject("Item_" + i);
            obj.transform.SetParent(this.transform, false);
            obj.layer = this.gameObject.layer;
            SpriteRenderer renderer = obj.AddComponent<SpriteRenderer>();
            renderer.sprite = itemObj.Sprite;
            renderer.sortingOrder = 29;

            BoxCollider2D collider = obj.AddComponent<BoxCollider2D>();
            collider.size = itemObj.Sprite.bounds.size;

            ColliderMouseOver mouseOver = obj.AddComponent<ColliderMouseOver>();
            mouseOver.AssignedCollider = collider;

            ObjectDescription description = obj.AddComponent<ObjectDescription>();
            description.mouseOver = mouseOver;
            description.Description = itemObj.Description;
            description.offset = DescriptionOffset;

            MouseDescription.AddInventoryDescription(description);

            obj.transform.localPosition = new Vector3(j * 32f + (j>0 ? 10f : 0), 0, 0);
            j++;
        }
    }
}
