using UnityEngine;
using UnityEngine.UI;

public class EquipmentSlotUI : MonoBehaviour {
    public int slotID;
    public Button slotButton;
    public Sprite oldSprite;
    void Start () {
        EquipmentManager.instance.onEquipmentChanged += OnEquipmentChanged;
    }

    public void onUnequipButton() {
        // Debug.Log("Unequipping slot "+slotID);
        EquipmentManager.instance.Unequip(slotID);
    }

    public void OnEquipmentChanged(Equipment newItem, Equipment oldItem) {
        // Debug.Log("Equipment changed!");
        if(newItem == null) {
            if((int)oldItem.equipSlot == slotID) {
                slotButton.image.sprite = oldSprite;
                Color tempColor = slotButton.image.color;
                tempColor.a = 0.391f;
                slotButton.image.color = tempColor;
            }
            
        } else if((int)newItem.equipSlot == slotID) {
            // Debug.Log("Equipping item type "+slotID);
            // oldSprite = slotButton.image.sprite;
            slotButton.image.sprite = newItem.icon;
            Color tempColor = slotButton.image.color;
            tempColor.a = 1.0f;
            slotButton.image.color = tempColor;
        }
    }
}