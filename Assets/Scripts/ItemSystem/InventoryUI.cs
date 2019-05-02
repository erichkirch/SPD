using UnityEngine;

/* This object updates the inventory UI. */

public class InventoryUI : MonoBehaviour {

	public Transform itemsParent;	// The parent object of all the items
	public GameObject inventoryUI;	// The entire UI
	public GameObject inventoryButton;
	public GameObject equipmentPanel;
	Inventory inventory;	// Our current inventory

	InventorySlot[] slots;	// List of all the slots

	void Start () {
		inventory = Inventory.instance;
		inventory.onItemChangedCallback += UpdateUI;	// Subscribe to the onItemChanged callback

		// Populate our slots array
		slots = itemsParent.GetComponentsInChildren<InventorySlot>();
	}
	
	void Update () {
		// Check to see if we should open/close the inventory
		if (Input.GetButtonDown("Inventory"))
		{
			inventoryUI.SetActive(!inventoryUI.activeSelf);
		}
		if (Input.GetMouseButton(0) && inventoryUI.activeSelf && !RectTransformUtility.RectangleContainsScreenPoint(inventoryUI.GetComponent<RectTransform>(),Input.mousePosition,Camera.main) && !RectTransformUtility.RectangleContainsScreenPoint(equipmentPanel.GetComponent<RectTransform>(),Input.mousePosition,Camera.main)) {
			inventoryUI.SetActive(false);
			inventoryButton.SetActive(true);
		}
	}

	// Update the inventory UI by:
	//		- Adding items
	//		- Clearing empty slots
	// This is called using a delegate on the Inventory.
	void UpdateUI ()
	{
		// Loop through all the slots
		for (int i = 0; i < slots.Length; i++)
		{
			if (i < inventory.items.Count)	// If there is an item to add
			{
				slots[i].AddItem(inventory.items[i]);	// Add it
			} else
			{
				// Otherwise clear the slot
				slots[i].ClearSlot();
			}
		}
	}
}
