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
    public bool IsConsumable;
    public bool IsLegendary;
    public int Def;
    public int Damage;
    public int HealAmount;
    public float DropChance;

    [Header("Торговля")]
    public int BuyPrice;
    public int SellPrice;

    public int GetBuyPrice()
    {
        if (BuyPrice > 0)
        {
            return BuyPrice;
        }

        if (IsConsumable)
        {
            return Mathf.Max(15, HealAmount);
        }

        int price = Mathf.Max(20, Damage * 8 + Def * 6);

        if (IsLegendary)
        {
            return Mathf.Max(280, Damage * 25);
        }

        return price;
    }

    public int GetSellPrice()
    {
        if (SellPrice > 0)
        {
            return SellPrice;
        }

        return Mathf.Max(5, GetBuyPrice() / 2);
    }
}
