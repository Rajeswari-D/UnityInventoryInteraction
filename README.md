# Unity Interaction & Inventory System

A small first-person interactive Unity prototype demonstrating a
reusable interaction system, fixed-slot inventory,
stackable/non-stackable items, item dropping, and scalable C#
architecture.

## Overview

This project was developed as a Unity Developer take-home assignment.
The core experience allows the player to:

-   Look at interactable world items within a valid interaction range.
-   See an interaction prompt.
-   Pick up items using **E**.
-   Store items in a fixed-size inventory.
-   Stack compatible items such as ammunition.
-   Keep non-stackable items such as weapons in separate slots.
-   Select inventory items with the mouse.
-   Drop selected items using **Q**.
-   Pick dropped items back up from the world.
-   Move using **WASD** and look around using the mouse.

The implementation focuses on clean separation of responsibilities,
reusable interfaces, ScriptableObjects for item definitions, and
event-driven UI updates.

## Unity Version

-   **Unity 6.6**

## Controls

  Input               Action
  ------------------- --------------------------
  **W / A / S / D**   Move
  **Mouse**           Look around
  **E**               Pick up/interact
  **TAB**             Open / close inventory
  **ESC**             Close inventory
  **Mouse Click**     Select an inventory item
  **Q**               Drop selected item

## How to Run

1.  Open the project in **Unity 6.6**.
2.  Open the main scene: `Assets/_Project/Scenes/Main.unity`
3.  Press **Play**.
4.  Use **WASD** to move and the mouse to look around.
5.  Approach a world item and look at it from within interaction range.
6.  Press **E** to pick it up.
7.  Press **TAB** to open the inventory.
8.  Click an occupied inventory slot to select an item.
9.  Press **Q** to drop the selected item.
10. Look at the dropped item and press **E** to pick it up again.

## Architecture

The project is organized around separate responsibilities rather than
putting interaction, inventory, item data, and UI logic into a single
class.

### Interaction System

#### `IInteractable`

Defines the reusable contract for objects that can be interacted with.

Responsibilities:

-   Provide an interaction prompt.
-   Report whether interaction is currently possible.
-   Execute the interaction and report whether it succeeded.

This allows additional interactable objects to be introduced without
modifying the player interaction system.

#### `PlayerInteractor`

Responsible for:

-   Casting a ray from the player's camera.
-   Checking the configured interaction range.
-   Detecting objects implementing `IInteractable`.
-   Handling the **E** interaction input.
-   Raising interaction-related events.

The player does not need to know the concrete type of the object being
interacted with.

#### `WorldItem`

Implements `IInteractable` for inventory items existing in the world.

Responsibilities:

-   Store an `ItemData` reference and quantity.
-   Provide the pickup prompt.
-   Ask the inventory to add the item.
-   Destroy itself only after a successful pickup.
-   Configure itself when spawned as a dropped item.

### Item System

#### `ItemData`

`ItemData` is a ScriptableObject containing item configuration:

-   Item ID
-   Display name
-   Item type
-   Stackable/non-stackable setting
-   Maximum stack size
-   Optional icon reference

This separates item configuration from runtime inventory logic.

Adding a new item generally requires creating/configuring another
`ItemData` asset rather than changing the inventory implementation.

### Inventory System

#### `Inventory`

Maintains a fixed number of inventory slots and provides operations such
as:

-   `TryAddItem`
-   `CanAddItem`
-   `TryRemoveItem`
-   `GetItem`
-   `GetQuantity`

Stackable items first use available space in existing stacks and then
create additional stacks when required.

Non-stackable items require one inventory slot per item.

The inventory checks capacity before modifying its contents, preventing
partial pickups when insufficient space is available.

#### `InventorySlot`

Represents the runtime contents of a single inventory slot:

-   Item reference
-   Quantity
-   Empty/non-empty state

It provides simple operations for setting, adding, removing, and
clearing slot contents.

### Inventory UI

#### `InventoryUI`

Responsible for:

-   Creating the configured number of inventory slot UI elements.
-   Opening and closing the inventory.
-   Refreshing the UI when inventory data changes.
-   Tracking the currently selected slot.
-   Initiating item drops.

#### `InventorySlotUI`

Represents one visual inventory slot and handles:

-   Displaying item names.
-   Displaying stack quantities.
-   Handling mouse selection.

### Events

`InventoryEvents` provides an inventory-change event.

The inventory raises the event after successful inventory modifications,
allowing the UI to refresh without tightly coupling the inventory
implementation to UI objects.

### Drop System

#### `WorldItemDropper`

Responsible for creating a world item when an inventory item is dropped.

The dropped `WorldItem` is configured using its `SetItem` method,
allowing the same world-item implementation to represent different item
types.

This avoids creating separate pickup scripts for ammunition, weapons, or
future item categories.

### Player Systems

#### `PlayerMovement`

Uses Unity's `CharacterController` to provide basic first-person
movement and gravity.

#### `PlayerLook`

Handles mouse-based first-person camera/player rotation and manages
cursor locking depending on whether the inventory is open.

## Stackable vs Non-Stackable Items

The inventory behavior is data-driven through `ItemData`.

### Ammunition

Example:

-   `9mm Ammo`
-   Stackable: **Yes**
-   Maximum stack size: **30**

If the player collects 10 ammo and then 15 more, the inventory produces:

``` text
9mm Ammo x25
```

If the player collects enough to exceed the maximum stack size,
additional quantities are placed into another stack.

### Weapons

Example:

-   `Pistol`
-   Stackable: **No**
-   Maximum stack size: **1**

Each pistol occupies its own inventory slot.

## Capacity Handling

The inventory contains a fixed number of slots.

When there is insufficient capacity:

-   The pickup fails.
-   The world item remains in the scene.
-   The inventory is not partially modified.
-   A diagnostic message is written to the Unity Console.

This keeps item state consistent and prevents accidental item loss.

## Design Decisions

### ScriptableObjects for item definitions

Item properties are data rather than hard-coded into inventory logic.
This makes it easier to add new item definitions without changing the
inventory system.

### Interface-based interaction

`IInteractable` keeps the player interaction system independent from
specific world-item implementations.

The same system can later support doors, switches, containers, NPCs, or
other interactable objects.

### Separation of responsibilities

Responsibilities are intentionally separated:

``` text
PlayerInteractor
    ↓
IInteractable
    ↓
WorldItem
    ↓
Inventory
    ↓
InventorySlot
```

UI observes inventory changes rather than owning inventory data.

### Events for UI updates

The inventory does not directly reference `InventoryUI`. Instead, it
raises an inventory-change event. This reduces coupling between gameplay
data and presentation.

### Atomic inventory operations

Capacity is checked before adding an item. This prevents a situation
where part of a pickup is added before discovering that the inventory
cannot hold the remainder.

## SOLID & Scalability

The implementation applies SOLID principles where they provide practical
value:

-   **Single Responsibility:** interaction detection, item behavior,
    inventory state, slot state, and UI responsibilities are separated.
-   **Open/Closed:** new item definitions can be created through
    `ItemData` without modifying the core inventory implementation.
-   **Liskov Substitution:** objects implementing `IInteractable` can be
    handled through the same interaction contract.
-   **Interface Segregation:** the interaction contract is small and
    focused on interaction-specific behavior.
-   **Dependency Inversion:** the player interaction flow depends on
    `IInteractable` rather than concrete world-item classes.

The architecture is intentionally lightweight for the size of the
assignment rather than introducing unnecessary framework complexity.

## Assumptions

-   The prototype is designed for PC.
-   Interaction is performed from the player's camera using a forward
    raycast.
-   Inventory capacity is represented as a fixed number of slots.
-   A stackable item uses its configured maximum stack size.
-   Non-stackable items require one slot per item.
-   World items have colliders so they can be detected by the
    interaction raycast.
-   The current scene uses simple/available visual assets to demonstrate
    the gameplay systems.

## Limitations

-   There is no persistence/save system.
-   Inventory contents are runtime-only.
-   There is no equipment/weapon firing system.
-   Item icons are optional and are not required for the core inventory
    behavior.
-   The prototype focuses on the requested interaction and inventory
    requirements rather than a complete game loop.
-   VR support is not implemented; the assignment states that VR is
    preferred but not mandatory.

## Possible Future Improvements

If this prototype were extended beyond the assignment:

-   Add item icons and richer item metadata.
-   Add drag-and-drop inventory management.
-   Add item splitting/merging controls.
-   Add equipment slots and weapon handling.
-   Add inventory persistence.
-   Add interaction outlines/highlighting.
-   Add audio/visual feedback for pickup and drop actions.
-   Add automated unit tests for inventory operations.
-   Add configurable input actions rather than direct keyboard checks.

## Project Structure

``` text
Assets/_Project/
├── Art/
├── Prefabs/
│   └── UI/
├── Scenes/
│   └── Main.unity
├── ScriptableObjects/
├── Scripts/
│   ├── Core/
│   ├── Interaction/
│   ├── Inventory/
│   ├── Items/
│   ├── Player/
│   └── UI/
└── UI/
```

## Assignment Coverage

  -----------------------------------------------------------------------
  Requirement                         Implementation
  ----------------------------------- -----------------------------------
  Reusable interaction system         `IInteractable` +
                                      `PlayerInteractor`

  Look + range requirement            Camera raycast with configurable
                                      interaction range

  Interaction UI                      `InteractionUI`

  Fixed inventory slots               `Inventory`

  Multiple item types                 `ItemData`

  Stackable items                     Data-driven stack settings

  Non-stackable items                 Data-driven item behavior

  No available slots                  Capacity validation

  Drop items into world               `WorldItemDropper`

  Re-pick dropped items               `WorldItem` + `IInteractable`

  Separation of concerns              Dedicated systems/components

  Scalable item definitions           ScriptableObjects

  Event-driven inventory UI           `InventoryEvents`
  -----------------------------------------------------------------------

## Demo

The demonstration video should show:

1.  Moving around the scene.
2.  Looking at an item and showing the interaction prompt.
3.  Picking up ammunition.
4.  Picking up additional ammunition and demonstrating stacking.
5.  Picking up a pistol and demonstrating non-stackable behavior.
6.  Opening and selecting items in the inventory.
7.  Dropping an item.
8.  Picking the dropped item up again.
9.  Filling the inventory and demonstrating the full-inventory response.
