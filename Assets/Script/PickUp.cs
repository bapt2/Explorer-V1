using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : MonoBehaviour
{
    public BaseItem item;
    public InsectItem insectItem;
    public FishItem fishItem;
    public PlantItem plantItem;
    public bool isInRange = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P) && isInRange)
        {
            if (insectItem != null)
                PickupInsect();

            else if (fishItem != null)
                PickupFish();

            else if (plantItem != null)
                PickupPlant();

            else if (item != null)
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
        InventoryManager.instance.itemList.Add(item);
        Destroy(gameObject);
    }
    void PickupInsect()
    {
        InventoryManager.instance.insectItemList.Add(insectItem);
        Destroy(gameObject);
    }
    void PickupFish()
    {
        InventoryManager.instance.fishItemList.Add(fishItem);
        Destroy(gameObject);
    }
    void PickupPlant()
    {
        InventoryManager.instance.plantItemList.Add(plantItem);
        Destroy(gameObject);
    }
}
