using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Resource,
    Equipable,
    Consumable
}

public enum ConsumalbeType
{
    Hunger,
    Health
}

[System.Serializable]
public class ItemDataConsumable
{
    public ConsumalbeType Type;
    public float value;
}

[CreateAssetMenu(fileName ="Item", menuName = "New Item")]

public class ItemData : ScriptableObject
{
    [Header("Info")]
    public string displayName;
    public string description;
    public ItemType type;
    public Sprite icon;
    public GameObject dropPrefab;

    [Header("Stacking")]
    public bool canStck;
    public int maxStackAmount;

    [Header("Consumable")]
    public ItemDataConsumable[] consumables;

}
