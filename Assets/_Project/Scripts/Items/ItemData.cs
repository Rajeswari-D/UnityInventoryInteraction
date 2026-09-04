using UnityEngine;

public enum ItemType
{
    Ammo,
    Weapon,
    Consumable,
    Miscellaneous
}

[CreateAssetMenu(
    fileName = "NewItemData",
    menuName = "Inventory/Item Data"
)]
public class ItemData : ScriptableObject
{
    [Header("Identity")]
    [SerializeField] private string itemId;
    [SerializeField] private string displayName;
    [SerializeField] private ItemType itemType;

    [Header("Inventory")]
    [SerializeField] private bool isStackable;
    [SerializeField] private int maxStackSize = 1;

    [Header("Visual")]
    [SerializeField] private Sprite icon;

    public string ItemId => itemId;
    public string DisplayName => displayName;
    public ItemType ItemType => itemType;
    public bool IsStackable => isStackable;
    public int MaxStackSize => Mathf.Max(1, maxStackSize);
    public Sprite Icon => icon;
}