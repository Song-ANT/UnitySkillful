using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEditor.ShaderKeywordFilter;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using static UnityEditor.Progress;

public class ItemSlot
{
    public ItemData item;
    public int Quantity;
}

public class Inventory : MonoBehaviour
{
    public ItemSlotUI[] uiSlots;
    public ItemSlot[] slots;

    public GameObject inventoryWindow;
    public Transform dropPosition;

    [Header("Selected Item")]
    private ItemSlot selectedItem;
    private int selectedItemIndex;
    public TextMeshProUGUI selectItemName;
    public TextMeshProUGUI selectItemDescription;
    public TextMeshProUGUI selectItemStatNames;
    public TextMeshProUGUI selectItemStatValues;
    public GameObject useBtn;
    public GameObject equipBtn;
    public GameObject unEquipBtn;
    public GameObject dropBtn;

    private int curEquipIndex;

    private PlayerController controller;
    private PlayerConditions condition;

    [Header("Events")]
    public UnityEvent onOpenInventory;
    public UnityEvent onCloseInventory;

    public static Inventory instance;

    private void Awake()
    {
        instance = this;
        controller = GetComponent<PlayerController>();
        condition = GetComponent<PlayerConditions>();
    }
    private void Start()
    {
        inventoryWindow.SetActive(false);
        slots = new ItemSlot[uiSlots.Length];

        for (int i = 0; i < slots.Length; i++)
        {
            slots[i] = new ItemSlot();
            uiSlots[i].index = i;
            uiSlots[i].Clear();
        }

        ClearSelectedItemWindow();
    }

    public void OnInventoryButton(InputAction.CallbackContext callbackContext)
    {
        if(callbackContext.phase == InputActionPhase.Started)
        {
            Toggle();
        }
    }


    public void Toggle()
    {
        if(inventoryWindow.activeInHierarchy)
        {
            inventoryWindow.SetActive(false );
            onCloseInventory?.Invoke();
            controller.ToggleCursor(false );
        }
        else
        {
            inventoryWindow.SetActive(true );
            onOpenInventory?.Invoke();
            controller.ToggleCursor(true);
        }
    }

    public bool IsOpen()
    {
        return inventoryWindow.activeInHierarchy;
    }

    public void AddItem(ItemData item)
    {
        if(item.canStck)
        {
            ItemSlot slotToStackTo = GetItemStack(item);
            if(slotToStackTo != null)
            {
                slotToStackTo.Quantity++;
                UpDateUI();
                return;
            }
        }

        ItemSlot emptySlot = GetEmptySlot();

        if(emptySlot != null)
        {
            emptySlot.item = item;
            emptySlot.Quantity = 1;
            UpDateUI();
            return;
        }

        ThrowItem(item);
    }

    public void ThrowItem(ItemData item)
    {
        Instantiate(item.dropPrefab, dropPosition.position, Quaternion.Euler(Vector3.one * Random.value * 360));
    }

    private void UpDateUI()
    {
        for(int i=0; i<slots.Length; i++)
        {
            if (slots[i].item != null)
            {
                uiSlots[i].Set(slots[i]);
            }
            else
            {
                uiSlots[i].Clear();
            }
        }
    }

    private ItemSlot GetItemStack(ItemData item)
    {
        for(int i=0; i<slots.Length; i++)
        {
            if(slots[i].item == item && slots[i].Quantity < item.maxStackAmount)
            {
                return slots[i];
            }
        }
        return null;
    }

    private ItemSlot GetEmptySlot()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item == null)
            {
                return slots[i];
            }
        }
        return null;
    }

    public void SelectItem(int index)
    {
        if (slots[index].item == null)
        {
            return;
        }

        selectedItem = slots[index];
        selectedItemIndex = index;

        selectItemName.text = selectedItem.item.displayName;
        selectItemDescription.text = selectedItem.item.description;

        selectItemStatNames.text = string.Empty;
        selectItemStatValues.text = string.Empty;

        for(int i=0; i< selectedItem.item.consumables.Length; i++)
        {
            selectItemStatNames.text += selectedItem.item.consumables[i].Type.ToString() + "\n";
            selectItemStatValues.text += selectedItem.item.consumables[i].value.ToString() + "\n";
        }

        useBtn.SetActive(selectedItem.item.type == ItemType.Consumable);
        equipBtn.SetActive(selectedItem.item.type == ItemType.Equipable && !uiSlots[index].equipped);
        unEquipBtn.SetActive(selectedItem.item.type == ItemType.Equipable && uiSlots[index].equipped);
        dropBtn.SetActive(true);

    }
    public void ClearSelectedItemWindow()
    {
        selectedItem = null;
        selectItemName.text = string.Empty;
        selectItemDescription.text = string.Empty;

        selectItemStatNames.text = string.Empty;
        selectItemStatValues.text = string.Empty;

        useBtn.SetActive(false);
        equipBtn.SetActive(false);
        unEquipBtn.SetActive(false);
        dropBtn.SetActive(false);
    }

    public void OnUseButton()
    {
        if(selectedItem.item.type == ItemType.Consumable)
        {
            for (int i = 0; i < selectedItem.item.consumables.Length; i++)
            {
                switch (selectedItem.item.consumables[i].Type)
                {
                    case ConsumalbeType.Health:
                        condition.heal(selectedItem.item.consumables[i].value);
                        break;

                    case ConsumalbeType.Hunger:
                        condition.Eat(selectedItem.item.consumables[i].value);
                        break;
                }
            }
        }
        RemoveSelectedItem();
    }
    public void OnEquipButton()
    {

    }

    private void UnEquip(int index)
    {

    }

    public void OnUnEquipButton()
    {

    }

    public void OnDropButton()
    {
        ThrowItem(selectedItem.item);
        RemoveSelectedItem();
    }

    private void RemoveSelectedItem()
    {
        selectedItem.Quantity--;

        if(selectedItem.Quantity <= 0 )
        {
            if (uiSlots[selectedItemIndex].equipped )
            {
                UnEquip(selectedItemIndex);
            }

            selectedItem.item = null;
            ClearSelectedItemWindow();
        }

        UpDateUI();
    }

    public void RemoveItem(ItemData item)
    {

    }

    public bool HasItems(ItemData item, int quantity)
    {
        return false;
    }
}


