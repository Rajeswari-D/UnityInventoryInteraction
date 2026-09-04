using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [Header("Inventory Settings")]
    [SerializeField] private int slotCount = 6;

    private readonly List<InventorySlot> slots = new();

    public IReadOnlyList<InventorySlot> Slots => slots;

    public int SlotCount => slots.Count;

    private void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        slots.Clear();

        for (int i = 0; i < slotCount; i++)
        {
            slots.Add(
                new InventorySlot()
            );
        }
    }

    public bool TryAddItem(
        ItemData item,
        int quantity)
    {
        if (item == null ||
            quantity <= 0)
        {
            return false;
        }

        // Check capacity BEFORE changing anything.
        if (!CanAddItem(
                item,
                quantity))
        {
            Debug.Log(
                $"Inventory is full. " +
                $"Cannot add {item.DisplayName} x{quantity}."
            );

            return false;
        }

        int remaining = quantity;

        // -------------------------------------------------
        // 1. Fill existing stacks first.
        // -------------------------------------------------

        if (item.IsStackable)
        {
            foreach (InventorySlot slot in slots)
            {
                if (slot.Item != item ||
                    slot.Quantity >= item.MaxStackSize)
                {
                    continue;
                }

                int availableSpace =
                    item.MaxStackSize -
                    slot.Quantity;

                int amountToAdd =
                    Mathf.Min(
                        availableSpace,
                        remaining
                    );

                slot.Add(amountToAdd);

                remaining -= amountToAdd;

                if (remaining <= 0)
                {
                    InventoryEvents
                        .RaiseInventoryChanged();

                    return true;
                }
            }
        }

        // -------------------------------------------------
        // 2. Use empty slots.
        // -------------------------------------------------

        foreach (InventorySlot slot in slots)
        {
            if (!slot.IsEmpty)
            {
                continue;
            }

            int amountToAdd =
                item.IsStackable
                    ? Mathf.Min(
                        item.MaxStackSize,
                        remaining)
                    : 1;

            slot.Set(
                item,
                amountToAdd
            );

            remaining -= amountToAdd;

            if (remaining <= 0)
            {
                InventoryEvents
                    .RaiseInventoryChanged();

                return true;
            }
        }

        return false;
    }

    public bool CanAddItem(
        ItemData item,
        int quantity)
    {
        if (item == null ||
            quantity <= 0)
        {
            return false;
        }

        // Non-stackable item:
        // each item requires one slot.
        if (!item.IsStackable)
        {
            int emptySlots = 0;

            foreach (InventorySlot slot in slots)
            {
                if (slot.IsEmpty)
                {
                    emptySlots++;
                }
            }

            return emptySlots >= quantity;
        }

        // Stackable item:
        // first count remaining space in existing stacks.
        int availableCapacity = 0;

        foreach (InventorySlot slot in slots)
        {
            if (slot.Item == item)
            {
                availableCapacity +=
                    item.MaxStackSize -
                    slot.Quantity;
            }
        }

        // Then count capacity from empty slots.
        foreach (InventorySlot slot in slots)
        {
            if (slot.IsEmpty)
            {
                availableCapacity +=
                    item.MaxStackSize;
            }
        }

        return availableCapacity >= quantity;
    }

    public bool TryRemoveItem(
        int slotIndex,
        int quantity)
    {
        if (!IsValidSlot(slotIndex))
        {
            return false;
        }

        InventorySlot slot =
            slots[slotIndex];

        if (slot.IsEmpty ||
            quantity <= 0)
        {
            return false;
        }

        if (quantity > slot.Quantity)
        {
            return false;
        }

        slot.Remove(quantity);

        InventoryEvents
            .RaiseInventoryChanged();

        return true;
    }

    public ItemData GetItem(
        int slotIndex)
    {
        if (!IsValidSlot(slotIndex))
        {
            return null;
        }

        return slots[slotIndex].Item;
    }

    public int GetQuantity(
        int slotIndex)
    {
        if (!IsValidSlot(slotIndex))
        {
            return 0;
        }

        return slots[slotIndex].Quantity;
    }

    private bool IsValidSlot(
        int index)
    {
        return index >= 0 &&
               index < slots.Count;
    }
}