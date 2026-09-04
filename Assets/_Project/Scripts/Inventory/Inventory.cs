using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField]
    private int slotCount = 6;

    private readonly List<InventorySlot> slots = new();

    public IReadOnlyList<InventorySlot> Slots => slots;

    private void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        slots.Clear();

        for (int i = 0; i < slotCount; i++)
        {
            slots.Add(new InventorySlot());
        }
    }

    public bool TryAddItem(ItemData item, int quantity)
    {
        if (item == null || quantity <= 0)
        {
            return false;
        }

        int remaining = quantity;

        // 1. Fill existing stacks first.
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
                    item.MaxStackSize - slot.Quantity;

                int amountToAdd =
                    Mathf.Min(availableSpace, remaining);

                slot.Add(amountToAdd);
                remaining -= amountToAdd;

                if (remaining <= 0)
                {
                    InventoryEvents.RaiseInventoryChanged();
                    return true;
                }
            }
        }

        // 2. Create new stacks / add non-stackable items.
        foreach (InventorySlot slot in slots)
        {
            if (!slot.IsEmpty)
            {
                continue;
            }

            int amountToAdd = item.IsStackable
                ? Mathf.Min(item.MaxStackSize, remaining)
                : 1;

            slot.Set(item, amountToAdd);
            remaining -= amountToAdd;

            if (remaining <= 0)
            {
                InventoryEvents.RaiseInventoryChanged();
                return true;
            }
        }

        // Inventory was not large enough.
        InventoryEvents.RaiseInventoryChanged();

        return false;
    }

    public bool TryRemoveItem(int slotIndex, int quantity)
    {
        if (!IsValidSlot(slotIndex))
        {
            return false;
        }

        InventorySlot slot = slots[slotIndex];

        if (slot.IsEmpty || quantity <= 0)
        {
            return false;
        }

        if (quantity > slot.Quantity)
        {
            return false;
        }

        slot.Remove(quantity);

        InventoryEvents.RaiseInventoryChanged();

        return true;
    }

    public ItemData GetItem(int slotIndex)
    {
        if (!IsValidSlot(slotIndex))
        {
            return null;
        }

        return slots[slotIndex].Item;
    }

    public int GetQuantity(int slotIndex)
    {
        if (!IsValidSlot(slotIndex))
        {
            return 0;
        }

        return slots[slotIndex].Quantity;
    }

    public int SlotCount => slots.Count;

    private bool IsValidSlot(int index)
    {
        return index >= 0 && index < slots.Count;
    }
}