using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_Text itemNameText;
    [SerializeField] private TMP_Text quantityText;
    [SerializeField] private Button button;

    private int slotIndex;
    private InventoryUI inventoryUI;

    public void Initialize(int index, InventoryUI owner)
    {
        slotIndex = index;
        inventoryUI = owner;

        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(OnClicked);
        }

        Refresh();
    }

    public void Refresh()
    {
        if (inventoryUI == null)
        {
            return;
        }

        Inventory inventory = inventoryUI.Inventory;

        if (inventory == null)
        {
            return;
        }

        ItemData item = inventory.GetItem(slotIndex);
        int quantity = inventory.GetQuantity(slotIndex);

        if (item == null)
        {
            itemNameText.text = "Empty";
            quantityText.text = string.Empty;
            return;
        }

        itemNameText.text = item.DisplayName;

        quantityText.text = item.IsStackable
            ? $"x{quantity}"
            : "x1";
    }

    private void OnClicked()
    {
        if (inventoryUI != null)
        {
            inventoryUI.SelectSlot(slotIndex);
        }
    }
}