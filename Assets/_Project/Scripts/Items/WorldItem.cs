using UnityEngine;

public class WorldItem : MonoBehaviour, IInteractable
{
    [Header("Item")]
    [SerializeField] private ItemData itemData;

    [Header("Quantity")]
    [SerializeField] private int quantity = 1;

    private bool isBeingCollected;

    public ItemData ItemData => itemData;
    public int Quantity => quantity;

    public string GetInteractionPrompt()
    {
        if (itemData == null)
        {
            return "Invalid Item";
        }

        if (itemData.IsStackable && quantity > 1)
        {
            return $"Press E to pick up {itemData.DisplayName} x{quantity}";
        }

        return $"Press E to pick up {itemData.DisplayName}";
    }

    public bool CanInteract()
    {
        return !isBeingCollected &&
               itemData != null &&
               quantity > 0;
    }

    public void Interact()
    {
        if (!CanInteract())
        {
            return;
        }

        Inventory inventory = FindAnyObjectByType<Inventory>();

        if (inventory == null)
        {
            Debug.LogWarning(
                "WorldItem: No Inventory found in the scene."
            );

            return;
        }

        bool added =
            inventory.TryAddItem(itemData, quantity);

        if (!added)
        {
            Debug.Log(
                $"Inventory is full. Cannot pick up {itemData.DisplayName}."
            );

            return;
        }

        isBeingCollected = true;

        Destroy(gameObject);
    }
}