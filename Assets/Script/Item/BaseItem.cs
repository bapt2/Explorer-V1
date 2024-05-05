using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName ="Base Item", menuName = "Item/Base Item", order = 1)]
public class BaseItem : ScriptableObject
{
    public string itemType;

    public string itemName;
    public int spriteID;

}
