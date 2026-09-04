using UnityEngine;

public class WorldItemDropper : MonoBehaviour
{
    public static WorldItemDropper Instance { get; private set; }

    [Header("References")]
    [SerializeField] private Transform dropPoint;
    [SerializeField] private WorldItem worldItemPrefab;

    [Header("Drop Settings")]
    [SerializeField] private float forwardOffset = 1.5f;
    [SerializeField] private float upwardOffset = 0.3f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public bool TryDropItem(ItemData item, int quantity)
    {
        if (item == null || quantity <= 0)
        {
            return false;
        }

        if (worldItemPrefab == null)
        {
            Debug.LogError(
                "WorldItemDropper: World Item Prefab is missing."
            );

            return false;
        }

        if (dropPoint == null)
        {
            Debug.LogError(
                "WorldItemDropper: Drop Point is missing."
            );

            return false;
        }

        Vector3 spawnPosition =
            dropPoint.position +
            dropPoint.forward * forwardOffset +
            Vector3.up * upwardOffset;

        WorldItem droppedItem =
            Instantiate(
                worldItemPrefab,
                spawnPosition,
                Quaternion.identity
            );

        if (droppedItem == null)
        {
            return false;
        }

        // Configure the newly spawned item.
        droppedItem.SetItem(
            item,
            quantity
        );

        // Make sure the dropped item is active.
        droppedItem.gameObject.SetActive(true);

        Debug.Log(
            $"World item spawned: {item.DisplayName} x{quantity}"
        );

        return true;
    }
}