using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Consumable", menuName = "Inventory/Consumable")]
public class Consumable : Item
{
    public int healthVal = 0;
    public override void Use() {
        base.Use();
        Player.updateHealth(healthVal);
		RemoveFromInventory();	
    }
}
