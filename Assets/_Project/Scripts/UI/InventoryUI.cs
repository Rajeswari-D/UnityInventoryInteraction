using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Inventory inventory;
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private Transform slotContainer;
    [SerializeField] private InventorySlotUI slotPrefab;

    [Header("Selection")]
    [SerializeField] private TMP_Text selectedItemText;

    private readonly List<InventorySlotUI> slotUIs = new();

    private int selectedSlotIndex = -1;

    public Inventory Inventory => inventory;

    public bool IsOpen
    {
        get
        {
            return inventoryPanel != null &&
                   inventoryPanel.activeSelf;
        }
    }

    private void OnEnable()
    {
        InventoryEvents.OnInventoryChanged += Refresh;
    }

    private void OnDisable()
    {
        InventoryEvents.OnInventoryChanged -= Refresh;
    }

    private void Start()
    {
        CreateSlots();

        CloseInventory();

        Refresh();
    }

    private void Update()
    {
        if (Keyboard.current == null)
        {
            return;
        }

        // TAB - Open / Close Inventory
        if (Keyboard.current.tabKey.wasPressedThisFrame)
        {
            ToggleInventory();
            return;
        }

        if (!IsOpen)
        {
            return;
        }

        // ESC - Close Inventory
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            CloseInventory();
            return;
        }

        // Q - Drop Selected Item
        if (Keyboard.current.qKey.wasPressedThisFrame)
        {
            DropSelectedItem();
        }
    }

    private void CreateSlots()
    {
        if (inventory == null)
        {
            Debug.LogError(
                "InventoryUI: Inventory reference is missing."
            );
            return;
        }

        if (slotContainer == null)
        {
            Debug.LogError(
                "InventoryUI: Slot Container reference is missing."
            );
            return;
        }

        if (slotPrefab == null)
        {
            Debug.LogError(
                "InventoryUI: Slot Prefab reference is missing."
            );
            return;
        }

        foreach (Transform child in slotContainer)
        {
            Destroy(child.gameObject);
        }

        slotUIs.Clear();

        for (int i = 0; i < inventory.SlotCount; i++)
        {
            InventorySlotUI slotUI =
                Instantiate(
                    slotPrefab,
                    slotContainer
                );

            slotUI.name =
                $"InventorySlot_{i}";

            slotUI.Initialize(
                i,
                this
            );

            slotUIs.Add(slotUI);
        }
    }

    public void Refresh()
    {
        if (inventory == null)
        {
            return;
        }

        if (slotUIs.Count != inventory.SlotCount)
        {
            CreateSlots();
        }

        foreach (InventorySlotUI slotUI in slotUIs)
        {
            if (slotUI != null)
            {
                slotUI.Refresh();
            }
        }

        ValidateSelection();
        RefreshSelectedItem();
    }

    public void SelectSlot(int index)
    {
        if (inventory == null)
        {
            return;
        }

        if (index < 0 ||
            index >= inventory.SlotCount)
        {
            return;
        }

        ItemData item =
            inventory.GetItem(index);

        if (item == null)
        {
            selectedSlotIndex = -1;
            RefreshSelectedItem();
            return;
        }

        selectedSlotIndex = index;

        RefreshSelectedItem();
    }

    private void ValidateSelection()
    {
        if (selectedSlotIndex < 0)
        {
            return;
        }

        if (selectedSlotIndex >= inventory.SlotCount)
        {
            selectedSlotIndex = -1;
            return;
        }

        if (inventory.GetItem(selectedSlotIndex) == null)
        {
            selectedSlotIndex = -1;
        }
    }

    private void RefreshSelectedItem()
    {
        if (selectedItemText == null ||
            inventory == null)
        {
            return;
        }

        if (selectedSlotIndex < 0)
        {
            selectedItemText.text =
                "Selected: None";

            return;
        }

        ItemData item =
            inventory.GetItem(selectedSlotIndex);

        if (item == null)
        {
            selectedItemText.text =
                "Selected: None";

            return;
        }

        int quantity =
            inventory.GetQuantity(selectedSlotIndex);

        if (item.IsStackable)
        {
            selectedItemText.text =
                $"Selected: {item.DisplayName} x{quantity}\n" +
                "Press Q to Drop";
        }
        else
        {
            selectedItemText.text =
                $"Selected: {item.DisplayName}\n" +
                "Press Q to Drop";
        }
    }

    private void ToggleInventory()
    {
        if (IsOpen)
        {
            CloseInventory();
        }
        else
        {
            OpenInventory();
        }
    }

    private void OpenInventory()
    {
        if (inventoryPanel == null)
        {
            return;
        }

        inventoryPanel.SetActive(true);

        Refresh();
    }

    private void CloseInventory()
    {
        if (inventoryPanel == null)
        {
            return;
        }

        inventoryPanel.SetActive(false);
    }

    private void DropSelectedItem()
    {
        if (inventory == null)
        {
            return;
        }

        if (selectedSlotIndex < 0)
        {
            Debug.Log(
                "No inventory item selected."
            );

            return;
        }

        ItemData item =
            inventory.GetItem(selectedSlotIndex);

        if (item == null)
        {
            selectedSlotIndex = -1;
            Refresh();
            return;
        }

        int quantity =
            inventory.GetQuantity(selectedSlotIndex);

        if (WorldItemDropper.Instance == null)
        {
            Debug.LogError(
                "InventoryUI: WorldItemDropper is missing."
            );

            return;
        }

        // Spawn world item first.
        bool dropped =
            WorldItemDropper.Instance.TryDropItem(
                item,
                quantity
            );

        if (!dropped)
        {
            Debug.LogWarning(
                "InventoryUI: Failed to spawn dropped item."
            );

            return;
        }

        // Remove from inventory.
        bool removed =
            inventory.TryRemoveItem(
                selectedSlotIndex,
                quantity
            );

        if (!removed)
        {
            Debug.LogWarning(
                "InventoryUI: Failed to remove item."
            );

            return;
        }

        Debug.Log(
            $"Dropped {item.DisplayName} x{quantity}."
        );

        // Clear selection.
        selectedSlotIndex = -1;

        // Refresh inventory data.
        Refresh();

        // IMPORTANT:
        // Close inventory after successful drop.
        CloseInventory();
    }
}