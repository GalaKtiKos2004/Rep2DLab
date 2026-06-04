using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]

public class Item : ScriptableObject
{
    [Header("Базовые Характеристики")]
    public string Name = " ";
    public string Description = "Описание предмета";
    public Sprite icon = null;

    public bool IsSword;
    public bool IsHelmet;
    public bool IsShield;
    public bool IsArmor;
    public int Def;
    public int Damage;
    public float DropChance;
}
