using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Класс, представляющий сундук, содержащий предметы.
/// Позволяет игроку открывать сундук и получать случайный предмет.
/// </summary>
public class Chest : MonoBehaviour
{
    private const KeyCode OpenKey = KeyCode.Return;
    
    [SerializeField] private Trigger _trigger; // Компонент триггера для определения игрока рядом с сундуком
    [SerializeField] private Inventory _inventory; // Инвентарь игрока, куда добавляются предметы из сундука
    [SerializeField] private List<Item> _items; // Список возможных предметов в сундуке
    
    private bool _canOpen = false; // Можно ли открыть сундук
    private bool _isOpen = false; // Открыт ли сундук
    
    /// <summary>
    /// Событие, вызываемое при открытии сундука.
    /// </summary>
    public event Action Opening;

    private void OnEnable()
    {
        _trigger.TriggerEntered += OnTriggerEntered;
        _trigger.TriggerExited += OnTriggerExited;
    }

    private void OnDisable()
    {
        _trigger.TriggerEntered -= OnTriggerEntered;
        _trigger.TriggerExited -= OnTriggerExited;
    }

    private void Update()
    {
        // Проверяем, нажата ли клавиша открытия, можно ли открыть сундук и закрыт ли он
        if (Input.GetKeyDown(OpenKey) && _canOpen && _isOpen == false)
        {
            Opening?.Invoke(); // Вызываем событие открытия сундука
            Open(); // Открываем сундук
            _isOpen = true;
        }
    }

    /// <summary>
    /// Обрабатывает вход игрока в зону действия сундука.
    /// </summary>
    private void OnTriggerEntered(Collider2D collision)
    {
        if (collision.TryGetComponent(out PlayerMover _))
        {
            _canOpen = true;
        }
    }
    
    /// <summary>
    /// Обрабатывает выход игрока из зоны действия сундука.
    /// </summary>
    private void OnTriggerExited(Collider2D collision)
    {
        if (collision.TryGetComponent(out PlayerMover _))
        {
            _canOpen = false;
        }
    }

    /// <summary>
    /// Открывает сундук и добавляет случайный предмет в инвентарь игрока.
    /// </summary>
    private void Open()
    {
        Item item = GetRandomItem();
        
        if (item != null && _inventory != null)
        {
            _inventory.AddItem(item);
            Debug.Log($"Игрок получил {item.Name} из сундука!");
        }
    }

    /// <summary>
    /// Выбирает случайный предмет из сундука на основе вероятности выпадения.
    /// </summary>
    /// <returns>Случайный предмет или null, если сундук пуст.</returns>
    private Item GetRandomItem()
    {
        float totalWeight = 0f;

        // Подсчитываем общий вес всех предметов
        foreach (var loot in _items)
        {
            totalWeight += loot.DropChance;
        }
        
        float randomValue = UnityEngine.Random.Range(0f, totalWeight);
        float currentSum = 0f;
        
        // Выбираем предмет на основе случайного значения
        foreach (var loot in _items)
        {
            currentSum += loot.DropChance;
            if (randomValue <= currentSum)
            {
                return loot;
            }
        }
        
        return null;
    }
}
