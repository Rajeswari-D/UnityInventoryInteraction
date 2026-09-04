using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Button button;
    [SerializeField] private TMP_Text itemNameText;
    [SerializeField] private TMP_Text quantityText;

    private int slotIndex;
    private InventoryUI inventoryUI;

    public void Initialize(
        int index,
        InventoryUI owner)
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
        if (inventoryUI == null ||
            inventoryUI.Inventory == null)
        {
            return;
        }

        Inventory inventory =
            inventoryUI.Inventory;

        ItemData item =
            inventory.GetItem(slotIndex);

        int quantity =
            inventory.GetQuantity(slotIndex);

        if (item == null)
        {
            SetEmpty();
            return;
        }

        SetItem(
            item,
            quantity
        );
    }

    private void SetEmpty()
    {
        if (itemNameText != null)
        {
            itemNameText.text = "EMPTY";
        }

        if (quantityText != null)
        {
            quantityText.text = "";
        }
    }

    private void SetItem(
        ItemData item,
        int quantity)
    {
        if (itemNameText != null)
        {
            itemNameText.text =
                item.DisplayName;
        }

        if (quantityText != null)
        {
            quantityText.text =
                item.IsStackable
                    ? $"x{quantity}"
                    : "";
        }
    }

    private void OnClicked()
    {
        if (inventoryUI != null)
        {
            inventoryUI.SelectSlot(
                slotIndex
            );
        }
    }
}