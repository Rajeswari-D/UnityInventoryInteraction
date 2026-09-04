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

    private void Awake()
    {
        CreateSlots();
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
        inventoryPanel.SetActive(false);
        Refresh();
    }

    private void Update()
    {
        if (Keyboard.current == null)
        {
            return;
        }

        if (Keyboard.current.tabKey.wasPressedThisFrame)
        {
            ToggleInventory();
        }
    }

    private void CreateSlots()
    {
        if (inventory == null ||
            slotContainer == null ||
            slotPrefab == null)
        {
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
                Instantiate(slotPrefab, slotContainer);

            slotUI.Initialize(i, this);

            slotUIs.Add(slotUI);
        }
    }

    public void Refresh()
    {
        foreach (InventorySlotUI slotUI in slotUIs)
        {
            slotUI.Refresh();
        }

        RefreshSelectedItem();
    }

    public void SelectSlot(int index)
    {
        if (inventory == null ||
            index < 0 ||
            index >= inventory.SlotCount)
        {
            return;
        }

        selectedSlotIndex = index;

        RefreshSelectedItem();
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
            selectedItemText.text = "Selected: None";
            return;
        }

        ItemData item =
            inventory.GetItem(selectedSlotIndex);

        if (item == null)
        {
            selectedItemText.text = "Selected: Empty";
            return;
        }

        int quantity =
            inventory.GetQuantity(selectedSlotIndex);

        selectedItemText.text =
            $"Selected: {item.DisplayName} x{quantity}";
    }

    private void ToggleInventory()
    {
        bool isOpen = !inventoryPanel.activeSelf;

        inventoryPanel.SetActive(isOpen);

        if (isOpen)
        {
            Refresh();
        }
    }
}