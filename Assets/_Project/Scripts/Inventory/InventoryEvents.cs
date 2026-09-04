using System;

public static class InventoryEvents
{
    public static event Action OnInventoryChanged;

    public static void RaiseInventoryChanged()
    {
        OnInventoryChanged?.Invoke();
    }
}