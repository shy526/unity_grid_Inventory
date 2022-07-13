using UnityEngine;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour
{
    public ItemData itemData;

    public int HEIGHT
    {
        get
        {
            if (!rotated)
            {
                return itemData.height;
            }

            return itemData.width;
        }
    }
    
    public int WIDTH
    {
        get
        {
            if (!rotated)
            {
                return itemData.width;
            }

            return itemData.height;
        }
    }

    public int onGridPositionX;
    public int onGridPositionY;
    public bool rotated = false;
    
    public void Set(ItemData item)
    {
        this.itemData = item;
        GetComponent<Image>().sprite = item.itemIcon;
        Vector2 size = new Vector2();
        size.x = WIDTH * ItemGrid.tileSizeWidth;
        size.y = HEIGHT * ItemGrid.tileSizeHeight;
        GetComponent<RectTransform>().sizeDelta = size;
    }

    public void Rotated()
    {
        rotated = !rotated;
        var rectTransform = GetComponent<RectTransform>();
        rectTransform.rotation=Quaternion.Euler(0,0,rotated?90f:0f);
    }
}