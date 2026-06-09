using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Класс, управляющий игровой сессией и сохранением инвентаря между сценами.
/// Реализует паттерн Singleton.
/// </summary>
public class GameSession : MonoBehaviour
{
    /// <summary> Единственный экземпляр GameSession. </summary>
    public static GameSession Instance { get; private set; }

    /// <summary> Список предметов, сохраненных между сценами. </summary>
    private IReadOnlyList<Item> _items;

    /// <summary> Список предметов, сохраненных между сценами. </summary>
    private IReadOnlyList<Item> _equipItems;

    private List<QuestProgressData> _questProgress = new List<QuestProgressData>();

    private const int StartingGold = 50;

    private int _gold = StartingGold;

    public int Gold => _gold;

    public event Action<int> GoldChanged;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Сохраняет текущий инвентарь перед сменой сцены.
    /// </summary>
    /// <param name="inventory">Инвентарь, который нужно сохранить.</param>
    public void SaveInventory(Inventory inventory, EqupimentManager equipment)
    {
        _items = inventory.Items;
        List<Item> equipItems = new List<Item>(new Item[] { equipment.Weapon, equipment.Armor, equipment.Helmet, equipment.Shield });
        _equipItems = equipItems;

        if (QuestManager.Instance != null)
        {
            _questProgress = new List<QuestProgressData>(QuestManager.Instance.GetSaveData());
        }
    }

    /// <summary>
    /// Загружает сохраненные предметы в новый инвентарь после смены сцены.
    /// </summary>
    /// <param name="newInventory">Инвентарь, в который нужно загрузить предметы.</param>
    public void LoadInventory(Inventory newInventory, EqupimentManager equipment)
    {
        Debug.Log("Loading inventory");
        if (_items != null)
        {
            Debug.Log("New inventory loaded");
            newInventory.LoadFromOtherInventory(_items);
            equipment.Load(_equipItems);
        }

        if (QuestManager.Instance != null && _questProgress != null)
        {
            QuestManager.Instance.LoadSaveData(_questProgress);
        }
    }

    public bool CanAfford(int price)
    {
        return price > 0 && _gold >= price;
    }

    public bool TrySpendGold(int price)
    {
        if (CanAfford(price) == false)
        {
            return false;
        }

        _gold -= price;
        GoldChanged?.Invoke(_gold);
        return true;
    }

    public void AddGold(int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        _gold += amount;
        GoldChanged?.Invoke(_gold);
    }
}
