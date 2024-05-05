using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class PickUp : MonoBehaviour
{
    public BaseItem item;
    public bool isInRange = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P) && isInRange)
        {
            if (item != null && item.itemType == "Item")
                PickupItem(InventoryManager.instance.itemList, InventoryManager.instance.itemPlace,
                           InventoryManager.instance.ItemInventoryFull(InventoryManager.instance.itemFull));

            else if (item != null && item.itemType == "Insect")
                PickupItem(InventoryManager.instance.insectItemList, InventoryManager.instance.insectPlace,
                           InventoryManager.instance.ItemInventoryFull(InventoryManager.instance.insectFull));

            else if (item != null && item.itemType == "Fish")
                PickupItem(InventoryManager.instance.fishItemList, InventoryManager.instance.fishPlace,
                           InventoryManager.instance.ItemInventoryFull(InventoryManager.instance.fishFull));

            else if (item != null && item.itemType == "Plant")
                PickupItem(InventoryManager.instance.plantItemList, InventoryManager.instance.plantPlace,
                           InventoryManager.instance.ItemInventoryFull(InventoryManager.instance.plantFull));
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isInRange = true;
            InventoryManager.instance.DisplayPickUpMessage();

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isInRange = false;
            InventoryManager.instance.ClosePickUpMessage();
        }
    }

    void PickupItem(List<BaseItem> wantedList, int maxPlace, IEnumerator message)
    {
        if (wantedList.Count >= maxPlace)
            StartCoroutine(message);

        else
        {
            wantedList.Add(item);
            InventoryManager.instance.ClosePickUpMessage();
            Destroy(gameObject);
        }
    }
}
