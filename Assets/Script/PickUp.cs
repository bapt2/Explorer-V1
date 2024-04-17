using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : MonoBehaviour
{
    public BaseItem item;
    public bool isInRange = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P) && isInRange)
        {
            Debug.Log(PlayerController.instance.gameObject.name);

            PickupItem();
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isInRange = false;
        }
    }

    void PickupItem()
    {
        Debug.Log(InventoryManager.instance.gameObject.name);
        InventoryManager.instance.itemList.Add(item);
        InventoryManager.instance.CreateItemSlot(InventoryManager.instance.itemList);
        Destroy(gameObject);
    }
}
