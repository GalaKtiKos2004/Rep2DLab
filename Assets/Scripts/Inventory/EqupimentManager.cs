using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Управляет экипировкой персонажа, позволяя надевать и снимать предметы.
/// </summary>
public class EqupimentManager : MonoBehaviour
{
    [SerializeField] private Inventory _inventory;

    [SerializeField] private ItemSlot _weaponSlot;
    [SerializeField] private ItemSlot _armorSlot;
    [SerializeField] private ItemSlot _shieldSlot;
    [SerializeField] private ItemSlot _helmetSlot;

    /// <summary> Текущее оружие персонажа. </summary>
    public Item Weapon => _weaponSlot.ItemInSlot;

    /// <summary> Текущая броня персонажа. </summary>
    public Item Armor => _armorSlot.ItemInSlot;

    /// <summary> Текущий щит персонажа. </summary>
    public Item Shield => _shieldSlot.ItemInSlot;

    /// <summary> Текущий шлем персонажа. </summary>
    public Item Helmet => _helmetSlot.ItemInSlot;

    /// <summary>
    /// Экипирует предмет, если он подходит для соответствующего слота.
    /// </summary>
    /// <param name="item">Предмет для экипировки.</param>
    public void EquipItem(Item item)
    {
        if (item == null)
        {
            return;
        }
        
        if (item.IsSword)
        {
            SwapEquipment(_weaponSlot, item);
        }
        else if (item.IsArmor)
        {
            SwapEquipment(_armorSlot, item);
        }
        else if (item.IsShield)
        {
            SwapEquipment(_shieldSlot, item);
        }
        else if (item.IsHelmet)
        {
            SwapEquipment(_helmetSlot, item);
        }
    }

    // <summary>
    /// Загружает весь инвентарь.
    /// </summary>
    public void Load(IReadOnlyList<Item> items)
    {
        foreach (var item in items)
        {
            EquipItem(item);
        }
    }

    /// <summary>
    /// Очищает все экипированные предметы.
    /// </summary>
    public void Clear()
    {
        _weaponSlot.Clear();
        _armorSlot.Clear();
        _shieldSlot.Clear();
        _helmetSlot.Clear();
    }

    /// <summary>
    /// Заменяет предмет экипировки, возвращая старый предмет в инвентарь.
    /// </summary>
    /// <param name="equipSlot">Слот, в который будет помещен новый предмет.</param>
    /// <param name="newItem">Новый предмет для экипировки.</param>
    private void SwapEquipment(ItemSlot equipSlot, Item newItem)
    {
        Item previousItem = equipSlot.ItemInSlot;

        // Экипируем новый предмет
        equipSlot.SetItem(newItem);

        // Если в слоте уже был предмет, возвращаем его в инвентарь
        if (previousItem != null)
        {
            _inventory.AddItem(previousItem);
        }
    }
}
