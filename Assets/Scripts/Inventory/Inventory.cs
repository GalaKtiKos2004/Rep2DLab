using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Управляет хранением и взаимодействием с предметами в инвентаре.
/// </summary>
public class Inventory : MonoBehaviour
{
    private const int MaxItems = 15;

    [SerializeField] private List<Item> _items = new List<Item>();
    [SerializeField] private List<ItemSlot> _itemSlots = new List<ItemSlot>();
    [SerializeField] private List<ItemSlot> _equipSlots = new List<ItemSlot>();
    [SerializeField] private InventoryUi _inventoryUi;

    /// <summary> Список предметов в инвентаре (только для чтения). </summary>
    public IReadOnlyList<Item> Items => _items;

    /// <summary>
    /// Добавляет предмет в инвентарь, если есть место.
    /// </summary>
    /// <param name="item">Предмет для добавления.</param>
    /// <returns>Возвращает true, если предмет успешно добавлен, иначе false.</returns>
    public bool AddItem(Item item)
    {
        foreach (var slot in _itemSlots)
        {
            if (slot.IsEmpty)
            {
                slot.SetItem(item);
                _items.Add(item);
                return true;
            }
        }

        Debug.Log("Нет свободных слотов в инвентаре!");
        return false;
    }

    /// <summary>
    /// Удаляет предмет из указанного слота инвентаря.
    /// </summary>
    /// <param name="selectedSlot">Слот, из которого будет удален предмет.</param>
    public void RemoveItemFromSlot(ItemSlot selectedSlot)
    {
        if (selectedSlot == null || selectedSlot.ItemInSlot == null)
        {
            Debug.Log("Пустой слот, нечего удалять!");
            return;
        }

        Item itemToRemove = selectedSlot.ItemInSlot;

        if (_items.Contains(itemToRemove))
        {
            _items.Remove(itemToRemove);
            selectedSlot.Clear();
            Debug.Log($"Удален предмет: {itemToRemove.Name}");
        }
        else if (_equipSlots.Contains(selectedSlot))
        {
            _equipSlots.Remove(selectedSlot);
            selectedSlot.Clear();
            Debug.Log($"Удален предмет: {itemToRemove.Name}");
        }
    }

    /// <summary>
    /// Загружает предметы из другого инвентаря, очищая текущий список.
    /// </summary>
    /// <param name="items">Список предметов для загрузки.</param>
    public void LoadFromOtherInventory(IReadOnlyList<Item> items)
    {
        _items.Clear();

        foreach (var item in items)
        {
            AddItem(item);
        }
    }
}
