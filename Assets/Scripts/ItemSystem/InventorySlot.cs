using UnityEngine;
using UnityEngine.UI;

/* Sits on all InventorySlots. */

public class InventorySlot : MonoBehaviour {

	public Image icon;			// Reference to the Icon image
	// public Button removeButton;	// Reference to the remove button

	public GameObject SelectedPanel;
	public Image selectedIcon;
	public Text description;
	private static Item selectedItem;
	Item item;  // Current item in the slot

	// Add item to the slot
	public void AddItem (Item newItem)
	{
		item = newItem;

		icon.sprite = item.icon;
		icon.enabled = true;
		// removeButton.interactable = true;
	}

	// Clear the slot
	public void ClearSlot ()
	{
		item = null;

		icon.sprite = null;
		icon.enabled = false;
		// removeButton.interactable = false;
	}

	// Called when the remove button is pressed
	public void OnRemoveButton ()
	{
		Debug.Log("Remove button pressed!");
		// SelectedPanel.SetActive(false);
		Inventory.instance.Remove(selectedItem);
	}

	public void setActiveItem() {
		if (item != null) {
			SelectedPanel.SetActive(true);
			selectedIcon.sprite = item.icon;
			description.text = "\t<b>"+item.name+"</b>\n\n"+ item.description;
			if(item.GetType() == typeof(Equipment)) {
				if(((Equipment)item).armorModifier != 0) {
					description.text += "\n"+"+"+((Equipment)item).armorModifier+" defense";
				}
				if(((Equipment)item).damageModifier != 0) {
					description.text += "\n"+"+"+((Equipment)item).damageModifier+" attack";
				}
			}
			selectedItem = item;
		}
	}

	// Called when the item is pressed
	public void UseItem ()
	{
		if (selectedItem != null)
		{
			selectedItem.Use();
		}
	}

}
