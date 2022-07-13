using System.Collections.Generic;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    [HideInInspector] private ItemGrid selectedItemGrid;
    public ItemGrid SelectedItemGrid
    {
        get => selectedItemGrid;
        set
        {
             selectedItemGrid = value;
             inventoryHighlight.setParent(value);
        }
    }

    InventoryItem selectedItem;
    InventoryItem overlapItem;
    RectTransform rectTransform;

    [SerializeField] List<ItemData> items;
    [SerializeField] GameObject itemPrefab;
    [SerializeField] Transform canvasTransform;
    InventoryHighlight  inventoryHighlight;

    private void Awake()
    {
        inventoryHighlight = GetComponent<InventoryHighlight>();
    }

    private void Update()
    {
        
        ItemIconDrag();
       
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (selectedItem==null)
            {
                CreateRandomItem();   
            }
           
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            InsertRandomItem();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            RotateItem();
        }
        if (selectedItemGrid == null)
        {
            inventoryHighlight.Show(false);
            return;
            
        }
        
        handleHighlight();
        if (Input.GetMouseButtonDown(0))
        {
            LeftMouseButtonPress();
        }
    }

    private void RotateItem()
    {
        if (selectedItem==null)
        {
            return;
        }

        selectedItem.Rotated();
    }

    private void InsertRandomItem()
    {
        if (selectedItemGrid==null)
        {
            return;
        }
        CreateRandomItem();
        InventoryItem item2Insert = selectedItem;
        selectedItem = null;
        rectTransform = null;
        InsertItem(item2Insert);
    }

    private void InsertItem(InventoryItem item2Insert)
    {
        Vector2Int? posOnGrid = selectedItemGrid.FindSpaceForObject(item2Insert);
        if (posOnGrid==null)
        {
            return;
        }

        selectedItemGrid.PlaceItem(item2Insert, posOnGrid.Value.x, posOnGrid.Value.y);
    }

    InventoryItem itemToHighlight; 
    Vector2Int oldPosition;
    private void handleHighlight()
    {
        Vector2Int positionOnGrid = GetTileGridPosition();
        if (oldPosition==positionOnGrid)
        {
            return;
        }
        oldPosition = positionOnGrid;
        if (selectedItem==null)
        {
            itemToHighlight = selectedItemGrid.GetItem(positionOnGrid.x,positionOnGrid.y) ;
            if (itemToHighlight!=null)
            {
                inventoryHighlight.Show(true);
                inventoryHighlight.SetSize(itemToHighlight);
               // inventoryHighlight.setParent(selectedItemGrid);
                inventoryHighlight.SetPosition(selectedItemGrid,itemToHighlight);    
            } else
            {
                inventoryHighlight.Show(false); 
            }
        }
        else
        {
            //移动时高亮
            inventoryHighlight.Show(selectedItemGrid.BoundryCheck(positionOnGrid.x
                ,positionOnGrid.y,
                selectedItem.WIDTH,
                selectedItem.HEIGHT
                
                ));
            inventoryHighlight.SetSize(selectedItem);
           // inventoryHighlight.setParent(selectedItemGrid);
            inventoryHighlight.SetPosition(selectedItemGrid,selectedItem,positionOnGrid.x,positionOnGrid.y);   
        }
    }

    private void CreateRandomItem()
    {
        InventoryItem inventoryItem = Instantiate(itemPrefab).GetComponent<InventoryItem>();
        selectedItem = inventoryItem;
        rectTransform = inventoryItem.GetComponent<RectTransform>();
        rectTransform.SetParent(canvasTransform);
        rectTransform.SetAsLastSibling();
        var selectedItemId = Random.Range(0, items.Count);
        inventoryItem.Set(items[selectedItemId]);
    }

    private void LeftMouseButtonPress()
    {   
        var tileGridPosition = GetTileGridPosition();
        if (selectedItem == null)
        {
            PickUpItem(tileGridPosition);
        }
        else
        {
            PlaceItem(tileGridPosition);
        }
    }

    private Vector2Int GetTileGridPosition()
    {
        Vector2 position = Input.mousePosition;
        if (selectedItem != null)
        {
            position.x -= (selectedItem.WIDTH - 1) * ItemGrid.tileSizeWidth / 2;
            position.y += (selectedItem.HEIGHT - 1) * ItemGrid.tileSizeHeight / 2;
        }

        return selectedItemGrid.GetTileGridPosition(position);;
    }

    private void PlaceItem(Vector2Int tileGridPosition)
    {
        if (selectedItemGrid.PlaceItem(selectedItem, tileGridPosition.x,
                tileGridPosition.y, ref overlapItem)
           )
        {
            selectedItem = null;
            rectTransform = null;
            if (overlapItem != null)
            {
                selectedItem = overlapItem;
                overlapItem = null;
                rectTransform = selectedItem.GetComponent<RectTransform>();
                rectTransform.SetAsLastSibling();
            }
        }
    }

    private void PickUpItem(Vector2Int tileGridPosition)
    {
        selectedItem = selectedItemGrid.PickUpItem(tileGridPosition.x, tileGridPosition.y);
        if (selectedItem != null)
        {
            rectTransform = selectedItem.GetComponent<RectTransform>();
        }
    }

    private void ItemIconDrag()
    {
        if (rectTransform != null)
        {
            rectTransform.position = Input.mousePosition;
        }
    }
}