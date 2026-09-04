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

        PlayerInteractor interactor =
            FindAnyObjectByType<PlayerInteractor>();

        if (interactor != null)
        {
            interactor.InteractionSucceeded +=
                HandleInteractionSucceeded;
        }
    }

    private void OnDisable()
    {
        InventoryEvents.OnInventoryChanged -= Refresh;

        PlayerInteractor interactor =
            FindAnyObjectByType<PlayerInteractor>();

        if (interactor != null)
        {
            interactor.InteractionSucceeded -=
                HandleInteractionSucceeded;
        }
    }

    private void Start()
    {
        // Inventory initializes its data in Awake().
        CreateSlots();

        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(false);
        }

        Refresh();
    }

    private void Update()
    {
        if (Keyboard.current == null)
        {
            return;
        }

        // TAB = Open / Close Inventory
        if (Keyboard.current.tabKey.wasPressedThisFrame)
        {
            ToggleInventory();
        }

        // ESC = Close Inventory
        if (IsOpen &&
            Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            CloseInventory();
        }
    }

    // Called when an interaction successfully completes.
    private void HandleInteractionSucceeded(
        IInteractable interactable)
    {
        if (interactable == null)
        {
            return;
        }

        // Open the inventory automatically after a successful pickup.
        OpenInventory();
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

        // Remove any existing slots.
        foreach (Transform child in slotContainer)
        {
            Destroy(child.gameObject);
        }

        slotUIs.Clear();

        // Create one UI slot for every inventory slot.
        for (int i = 0; i < inventory.SlotCount; i++)
        {
            InventorySlotUI slotUI =
                Instantiate(slotPrefab, slotContainer);

            slotUI.name =
                $"InventorySlot_{i}";

            slotUI.Initialize(i, this);

            slotUIs.Add(slotUI);
        }
    }

    public void Refresh()
    {
        if (inventory == null)
        {
            return;
        }

        // Safety check in case inventory size changes.
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

        // Don't select empty slots.
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

        ItemData selectedItem =
            inventory.GetItem(selectedSlotIndex);

        // Selected item was removed.
        if (selectedItem == null)
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
                $"Selected: {item.DisplayName} x{quantity}";
        }
        else
        {
            selectedItemText.text =
                $"Selected: {item.DisplayName}";
        }
    }

    private void ToggleInventory()
    {
        if (inventoryPanel == null)
        {
            return;
        }

        if (inventoryPanel.activeSelf)
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
}