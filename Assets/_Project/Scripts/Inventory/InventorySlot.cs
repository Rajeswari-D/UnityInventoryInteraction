using System;

[Serializable]
public class InventorySlot
{
    public ItemData Item { get; private set; }
    public int Quantity { get; private set; }

    public bool IsEmpty => Item == null;

    public void Set(ItemData item, int quantity)
    {
        Item = item;
        Quantity = quantity;
    }

    public void Add(int amount)
    {
        Quantity += amount;
    }

    public void Remove(int amount)
    {
        Quantity -= amount;

        if (Quantity <= 0)
        {
            Clear();
        }
    }

    public void Clear()
    {
        Item = null;
        Quantity = 0;
    }
}